use tauri::Manager;
use std::process::{Command, Child};
use std::sync::Mutex;

struct ServerProcess(Mutex<Option<Child>>);

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
  tauri::Builder::default()
    .setup(|app| {
      if cfg!(debug_assertions) {
        app.handle().plugin(
          tauri_plugin_log::Builder::default()
            .level(log::LevelFilter::Info)
            .build(),
        )?;
      }

      // Start the Blazor server in production builds
      #[cfg(not(debug_assertions))]
      {
        let resource_path = app.path().resource_dir().expect("failed to resolve resource dir");
        let server_path = resource_path.join("SBInspector");
        
        let server_process = Command::new(&server_path)
          .current_dir(&resource_path)
          .env("ASPNETCORE_URLS", "http://localhost:5000")
          .spawn()
          .expect("Failed to start Blazor server");

        app.manage(ServerProcess(Mutex::new(Some(server_process))));

        // Give the server time to start
        std::thread::sleep(std::time::Duration::from_secs(2));

        // Update the window URL to point to the server
        if let Some(window) = app.get_webview_window("main") {
          window.eval("window.location.href = 'http://localhost:5000'").ok();
        }
      }

      Ok(())
    })
    .on_window_event(|window, event| {
      if let tauri::WindowEvent::Destroyed = event {
        #[cfg(not(debug_assertions))]
        {
          // Stop the server process when the window is destroyed
          if let Some(state) = window.try_state::<ServerProcess>() {
            if let Ok(mut process_guard) = state.0.lock() {
              if let Some(server_process) = process_guard.as_mut() {
                server_process.kill().ok();
              }
            }
          }
        }
      }
    })
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
