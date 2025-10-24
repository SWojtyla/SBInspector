use tauri::Manager;
#[cfg(not(debug_assertions))]
use std::process::{Child, Command, Stdio};
#[cfg(not(debug_assertions))]
use std::time::Duration;
#[cfg(not(debug_assertions))]
use std::thread;
#[cfg(not(debug_assertions))]
use std::sync::Mutex;
#[cfg(not(debug_assertions))]
use std::net::TcpListener;

#[cfg(not(debug_assertions))]
struct ServerProcess(Mutex<Option<Child>>);

#[cfg(not(debug_assertions))]
fn find_available_port() -> Option<u16> {
  // Try ports starting from 5000
  for port in 5000..5100 {
    if TcpListener::bind(("127.0.0.1", port)).is_ok() {
      return Some(port);
    }
  }
  None
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
  tauri::Builder::default()
    .plugin(
      tauri_plugin_log::Builder::default()
        .level(log::LevelFilter::Info)
        .targets([
          tauri_plugin_log::Target::new(tauri_plugin_log::TargetKind::Stdout),
          tauri_plugin_log::Target::new(tauri_plugin_log::TargetKind::LogDir { file_name: None }),
          tauri_plugin_log::Target::new(tauri_plugin_log::TargetKind::Webview),
        ])
        .build(),
    )
    .setup(|app| {
      log::info!("SBInspector starting up...");
      log::info!("App data dir: {:?}", app.path().app_data_dir());
      log::info!("App log dir: {:?}", app.path().app_log_dir());
      
      // Start the Blazor server as a background process in production
      #[cfg(not(debug_assertions))]
      {
        let resource_path = app.path().resource_dir().expect("Failed to get resource dir");
        
        // Try multiple possible paths for the server executable
        let mut server_path = resource_path.join("SBInspector");
        #[cfg(target_os = "windows")]
        {
          server_path = server_path.with_extension("exe");
        }
        
        // If not found, try in the dist subdirectory
        if !server_path.exists() {
          server_path = resource_path.join("dist").join("SBInspector");
          #[cfg(target_os = "windows")]
          {
            server_path = server_path.with_extension("exe");
          }
        }

        log::info!("Resource directory: {:?}", resource_path);
        log::info!("Looking for Blazor server at: {:?}", server_path);

        // Check if the server executable exists
        if !server_path.exists() {
          log::error!("Blazor server executable not found at: {:?}", server_path);
          log::error!("Resource directory contents:");
          if let Ok(entries) = std::fs::read_dir(&resource_path) {
            for entry in entries.flatten() {
              log::error!("  - {:?}", entry.path());
            }
          }
          return Err("Failed to find Blazor server executable. Please ensure the app was built correctly.".into());
        }

        // Find an available port
        let port = find_available_port().ok_or("Could not find an available port")?;
        log::info!("Using port: {}", port);
        
        let server_url = format!("http://localhost:{}", port);
        log::info!("Server will be available at: {}", server_url);

        log::info!("Server executable found, attempting to start...");
        
        // Determine the working directory (where the executable is located)
        let working_dir = server_path.parent().expect("Failed to get server directory");
        
        // Start the server process
        #[cfg(target_os = "windows")]
        let mut cmd = {
          use std::os::windows::process::CommandExt;
          const CREATE_NO_WINDOW: u32 = 0x08000000;
          let mut cmd = Command::new(&server_path);
          cmd.creation_flags(CREATE_NO_WINDOW);
          cmd
        };
        
        #[cfg(not(target_os = "windows"))]
        let mut cmd = Command::new(&server_path);
        
        match cmd
           .current_dir(working_dir)
           .env("ASPNETCORE_ENVIRONMENT", "Production")
           .env("ASPNETCORE_URLS", &server_url)
           .stdout(Stdio::null())
           .stderr(Stdio::null())
           .spawn() {
          Ok(child) => {
            log::info!("Blazor server started successfully with PID: {}", child.id());
            // Store the server process handle
            app.manage(ServerProcess(Mutex::new(Some(child))));

            // Give the server time to start
            log::info!("Waiting for server to initialize...");
            thread::sleep(Duration::from_secs(3));
            log::info!("Server should be ready at {}", server_url);
            
            // Navigate the window to the server URL
            if let Some(window) = app.get_webview_window("main") {
              log::info!("Navigating window to server URL...");
              match window.navigate(server_url.parse().expect("Invalid URL")) {
                Ok(_) => log::info!("Window navigation successful"),
                Err(e) => log::error!("Failed to navigate window: {}", e),
              }
            } else {
              log::error!("Could not find main window");
            }
          }
          Err(e) => {
            log::error!("Failed to start Blazor server: {}", e);
            return Err(format!("Failed to start Blazor server: {}", e).into());
          }
        }
      }

      #[cfg(debug_assertions)]
      {
        let _ = app; // Suppress unused variable warning in debug mode
      }

      log::info!("Setup complete");
      Ok(())
    })
    .on_window_event(|window, event| {
      #[cfg(not(debug_assertions))]
      {
        if let tauri::WindowEvent::Destroyed = event {
          // Kill the server process when the window closes
          if let Some(server) = window.app_handle().try_state::<ServerProcess>() {
            if let Ok(mut child_opt) = server.0.lock() {
              if let Some(mut child) = child_opt.take() {
                let _ = child.kill();
                log::info!("Blazor server process terminated");
              }
            }
          }
        }
      }
      #[cfg(debug_assertions)]
      {
        let _ = (window, event); // Suppress unused variable warnings
      }
    })
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
