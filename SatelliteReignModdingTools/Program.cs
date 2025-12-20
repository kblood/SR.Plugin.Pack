using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Custom assembly resolver to find game DLLs
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                try
                {
                    // Get assembly name from the request
                    string assemblyName = args.Name.Split(',')[0].Trim();

                    // Search in these locations in order
                    string[] searchPaths = new[]
                    {
                        // Current directory (where exe/launcher are)
                        System.Environment.CurrentDirectory,

                        // Game DLL path from environment variable (set by launcher)
                        System.Environment.GetEnvironmentVariable("SATELLITE_REIGN_DLL_PATH") ?? "",

                        // Game installation paths
                        @"D:\SteamLibrary\steamapps\common\SatelliteReign\SatelliteReignWindows_Data\Managed",
                        @"c:\Modding\SatelliteReign\SatelliteReignWindows_Data\Managed",

                        // Steam registry path
                        GetSteamGamePath(),

                        // Multiple drives
                        @"C:\SteamLibrary\steamapps\common\SatelliteReign\SatelliteReignWindows_Data\Managed",
                        @"E:\SteamLibrary\steamapps\common\SatelliteReign\SatelliteReignWindows_Data\Managed",
                        @"G:\SteamLibrary\steamapps\common\SatelliteReign\SatelliteReignWindows_Data\Managed",
                    };

                    foreach (var path in searchPaths)
                    {
                        if (string.IsNullOrEmpty(path)) continue;

                        string dllPath = System.IO.Path.Combine(path, assemblyName + ".dll");
                        if (System.IO.File.Exists(dllPath))
                        {
                            return System.Reflection.Assembly.LoadFrom(dllPath);
                        }
                    }
                }
                catch { }

                return null;
            };

            // Global exception handler for all unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show(
                    $"Unhandled Exception:\n\n{ex.GetType().Name}: {ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                System.Diagnostics.Debugger.Break();
            };

            // Handle UI thread exceptions
            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show(
                    $"Thread Exception:\n\n{e.Exception.GetType().Name}: {e.Exception.Message}\n\nStack trace:\n{e.Exception.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ItemBrowser());
            Application.Run(new MainMenu());
        }

        static string GetSteamGamePath()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                {
                    if (key != null)
                    {
                        string steamPath = key.GetValue("SteamPath")?.ToString();
                        if (!string.IsNullOrEmpty(steamPath))
                        {
                            return System.IO.Path.Combine(steamPath, @"steamapps\common\SatelliteReign\SatelliteReignWindows_Data\Managed");
                        }
                    }
                }
            }
            catch { }
            return "";
        }
    }
}
