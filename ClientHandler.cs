using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using Path = System.IO.Path;

namespace Flarial
{
    /// <summary>
    /// Responsible for handling the ACTUAL flarial client
    /// such as downloading the DLL and Launcher if they don't exist
    /// </summary>
    static class ClientHandler
    {
        private static readonly HttpClient client = new HttpClient();
        private const string LauncherURL = "https://cdn.flarial.xyz/launcher/Flarial.Launcher.exe";
        private const string LauncherPath = "./Assets/DLL/Launcher.exe";
        private const string LauncherVersion = "https://cdn.flarial.xyz/launcher/launcherVersion.txt";
        private const string DLLURL = "https://cdn.flarial.xyz/dll/latest.dll";
        private const string DLLPath = "./Assets/DLL/Flarial.dll";
        private const string DLLHASHES = "https://cdn.flarial.xyz/dll_hashes.json";


        /// <summary>
        /// Responsible for downloading the Launcher
        /// </summary>
        private static async Task DownloadLauncher()
        {

            if (!File.Exists(LauncherPath))
            {
                try
                {
                    // Ensure directory exists
                    string directory = System.IO.Path.GetDirectoryName(LauncherPath)!;
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (client)
                    {
                        var response = await client.GetAsync(LauncherURL);
                        response.EnsureSuccessStatusCode();

                        using (var fs = new FileStream(LauncherPath, FileMode.Create))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download Launcher: {ex.Message}");
                }
            }
        }



        /// <summary>
        /// Responsible for downloading the DLL
        /// </summary>
        private static async Task DownloadDLL()
        {


            if (!File.Exists(DLLPath))
            {
                try
                {
                    // Ensure directory exists
                    string directory = System.IO.Path.GetDirectoryName(DLLPath)!;
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(DLLURL);
                        response.EnsureSuccessStatusCode();

                        using (var fs = new FileStream(DLLPath, FileMode.Create))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download DLL: {ex.Message}");
                }
            }
        }


        /// <summary>
        /// Checks for Updates if needed and downloads them
        /// </summary>
        public static async Task CheckForUpdates()
        {
            /*
             * Start by getting the launcher version from the flarial
             * CDN and checking if it matches the local version.
             * if not, Update the launcher.
             */
            try
            {
                if (!File.Exists(LauncherPath))
                {
                    await DownloadLauncher();
                    return;
                }
                string json = await client.GetStringAsync(LauncherVersion);
                using JsonDocument doc = JsonDocument.Parse(json);
                string? version = doc.RootElement.GetProperty("version").GetString();

                var info = FileVersionInfo.GetVersionInfo(LauncherPath);
                if (info.FileVersion != version)
                {
                    await DownloadLauncher();
                }
            }
            catch { }

            /*
             * The dll doesnt have a version, so we get the hash of the 
             * local dll and compare it to the hash of the remote dll, 
             * if they dont match, we download the new dll.
             */
            try
            {
                if (!File.Exists(DLLPath))
                {
                    await DownloadDLL();
                    return;
                }
                string json = await client.GetStringAsync(DLLHASHES);
                using JsonDocument doc = JsonDocument.Parse(json);
                string? hash = doc.RootElement.GetProperty("Release").GetString();
                if (await GetLocalHashAsync() != hash)
                {
                    await DownloadDLL();
                }
            }
            catch { }
        }


        /// <summary>
        /// Get the local hash of the DLL, used for comparing 
        /// with the remote hash to check for updates
        /// </summary>
        static readonly HashAlgorithm _algorithm = SHA256.Create();
        static readonly object _lock = new();
        static async Task<string> GetLocalHashAsync() => await Task.Run(() =>
        {
            try
            {
                lock (_lock)
                {
                    using var stream = File.OpenRead(DLLPath);
                    var value = _algorithm.ComputeHash(stream);
                    var @string = BitConverter.ToString(value);
                    return @string.Replace("-", string.Empty);
                }
            }
            catch { return string.Empty; }
        });



        /// <summary>
        /// Interact with the launcher to start the game with the dll injected,
        /// this is done by starting the launcher with the appropriate arguments.
        /// </summary>
        public static async Task StartGame()
        {
            try
            {
                // Run the launcher with the --inject [DLLPath] to start the game
                await Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = LauncherPath,
                        Arguments = $"--inject \"{Path.GetFullPath(DLLPath)}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    Process.Start(startInfo);
                });
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
