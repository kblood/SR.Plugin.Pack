using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    static class Program
    {
        private static string _gamePath;
        private static string _managedPath;
        private const int MAX_RETRIES = 3;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (!InitializeGamePath())
                {
                    MessageBox.Show(
                        "Application cannot start without a valid game installation path.\n\n" +
                        "Please ensure Satellite Reign is properly installed and try again.",
                        "Initialization Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Set up assembly resolution
                AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

                // Verify required assemblies can be loaded
                if (!VerifyRequiredAssemblies())
                {
                    MessageBox.Show(
                        "Unable to load required game assemblies.\n\n" +
                        "Please ensure you selected the correct Satellite Reign installation folder.",
                        "Missing Required Files",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Start the application
                Application.Run(new MainMenu());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fatal error starting application:\n\n{ex.Message}\n\n" +
                    "Please ensure Satellite Reign is properly installed and try again.",
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                // Log the full exception details
                try
                {
                    File.AppendAllText("error_log.txt",
                        $"[{DateTime.Now}] Fatal startup error:\n{ex}\n\n");
                }
                catch
                {
                    // Silently fail if we can't write to the log
                }
            }
        }

        private static bool InitializeGamePath()
        {
            int attempts = 0;
            while (attempts < MAX_RETRIES)
            {
                _gamePath = GamePathConfig.GetGamePath();
                if (!string.IsNullOrEmpty(_gamePath))
                {
                    // Set the managed path
                    _managedPath = Path.Combine(_gamePath, "SatelliteReignWindows_Data", "Managed");
                    return true;
                }
                attempts++;
            }
            return false;
        }

        private static bool VerifyRequiredAssemblies()
        {
            string[] requiredAssemblies = {
                "UnityEngine",
                "UnityEngine.UI",
                "Assembly-CSharp",
                "Assembly-CSharp-firstpass",
                "UnityEngine.Networking"
            };

            foreach (string assembly in requiredAssemblies)
            {
                try
                {
                    string path = Path.Combine(_managedPath, assembly + ".dll");
                    if (!File.Exists(path))
                    {
                        return false;
                    }
                    // Try loading the assembly to verify it's valid
                    Assembly.LoadFrom(path);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            try
            {
                // Get assembly name without version, culture, etc.
                var assemblyName = new AssemblyName(args.Name).Name;

                // Check if it's a game DLL
                string dllPath = Path.Combine(_managedPath, assemblyName + ".dll");
                if (File.Exists(dllPath))
                {
                    return Assembly.LoadFrom(dllPath);
                }

                // For any other assemblies, let .NET handle it
                return null;
            }
            catch (Exception ex)
            {
                try
                {
                    File.AppendAllText("error_log.txt",
                        $"[{DateTime.Now}] Assembly resolution error for {args.Name}:\n{ex}\n\n");
                }
                catch
                {
                    // Silently fail if we can't write to the log
                }
                return null;
            }
        }
    }
}
