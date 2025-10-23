#[cfg(not(debug_assertions))]
use tauri::Manager;
#[cfg(not(debug_assertions))]
use std::process::{Command, Stdio};
#[cfg(not(debug_assertions))]
use std::time::Duration;
#[cfg(not(debug_assertions))]
use std::thread;

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

      // Start the Blazor server as a background process in production
      #[cfg(not(debug_assertions))]
      {
        let resource_path = app.path().resource_dir().expect("Failed to get resource dir");
        let server_path = resource_path.join("dist").join("SBInspector");
        
        #[cfg(target_os = "windows")]
        let server_path = server_path.with_extension("exe");

        log::info!("Starting Blazor server from: {:?}", server_path);

        // Start the server process
        Command::new(server_path)
           .env("ASPNETCORE_ENVIRONMENT", "Production")
           .env("ASPNETCORE_URLS", "https://localhost:5000")
           .stdout(Stdio::null())
           .stderr(Stdio::null())
           .spawn()
           .expect("Failed to start Blazor server");

        // Give the server time to start
        thread::sleep(Duration::from_secs(3));
      }

      Ok(())
    })
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
