using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

public class GamePathConfig
{
    private const string CONFIG_FILE = "gamepath.txt";
    private const string STEAM_REG_KEY = @"SOFTWARE\Valve\Steam";
    private const string MANAGED_FOLDER_PATH = @"SatelliteReignWindows_Data\Managed";

    private static readonly string[] REQUIRED_FILES = {
        "UnityEngine.Networking.dll",
        "UnityEngine.UI.dll",
        "Assembly-CSharp.dll",
        "Assembly-CSharp-firstpass.dll",
        "UnityEngine.dll"
    };

    // Get config file path in the application directory
    private static string ConfigFilePath => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        CONFIG_FILE);

    [STAThread] // Ensure the thread is STA
    public static string GetGamePath()
    {
        try
        {
            // First check if we have a saved path
            if (File.Exists(ConfigFilePath))
            {
                string savedPath = File.ReadAllText(ConfigFilePath);

                if (IsValidGamePath(savedPath))
                {
                    return savedPath;
                }
            }

            // Try to find the game in Steam library folders
            string steamGamePath = GetGamePathFromSteam();
            if (!string.IsNullOrEmpty(steamGamePath))
            {
                SaveGamePath(steamGamePath);
                return steamGamePath;
            }

            // Inform the user that a dialog will be shown
            MessageBox.Show("The application will now prompt you to locate the Satellite Reign game folder.", "Locate Game Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Prompt the user to locate the game folder
            string selectedPath = PromptForGamePath();

            if (string.IsNullOrEmpty(selectedPath))
            {
                MessageBox.Show("A valid game installation path is required to continue.",
                    "Game Path Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (IsValidGamePath(selectedPath))
            {
                SaveGamePath(selectedPath);
                return selectedPath;
            }

            MessageBox.Show(
                "Selected folder does not appear to be a valid Satellite Reign installation.\n\n" +
                "Expected path should contain: SatelliteReignWindows_Data\\Managed\\\n\n" +
                "Please try again.",
                "Invalid Game Path",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            // Recursively try again
            return GetGamePath();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error getting game path: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    private static void SaveGamePath(string path)
    {
        try
        {
            File.WriteAllText(ConfigFilePath, path);
        }
        catch (Exception ex)
        {
            // Handle exceptions
            MessageBox.Show($"Error saving game path: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string PromptForGamePath()
    {
        // Attempt to show the FolderBrowserDialog
        try
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Please locate your Satellite Reign installation folder (typically in Steam\\steamapps\\common\\SatelliteReign)";
                dialog.ShowNewFolderButton = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
        }
        catch (Exception ex)
        {
            // If the dialog fails, prompt the user via the console
            MessageBox.Show("FolderBrowserDialog could not be displayed. Please enter the path manually.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Console.WriteLine("Please enter the full path to your Satellite Reign installation folder:");
            return Console.ReadLine();
        }
        return null;
    }

    private static bool IsValidGamePath(string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            return false;
        }

        string managedPath = Path.Combine(path, MANAGED_FOLDER_PATH);
        if (!Directory.Exists(managedPath))
        {
            return false;
        }

        foreach (string file in REQUIRED_FILES)
        {
            string fullPath = Path.Combine(managedPath, file);
            if (!File.Exists(fullPath))
            {
                return false;
            }
        }

        return true;
    }

    private static string GetGamePathFromSteam()
    {
        try
        {
            string steamPath = GetSteamInstallPath();
            if (string.IsNullOrEmpty(steamPath))
                return null;

            List<string> libraryFolders = GetSteamLibraryFolders(steamPath);
            foreach (string library in libraryFolders)
            {
                string gamePath = Path.Combine(library, "SatelliteReign");
                if (IsValidGamePath(gamePath))
                {
                    return gamePath;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            MessageBox.Show($"Error finding game path from Steam: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return null;
    }

    private static string GetSteamInstallPath()
    {
        // Try both 32-bit and 64-bit registry views
        string steamPath = GetSteamInstallPathFromRegistry(RegistryView.Registry64);
        if (string.IsNullOrEmpty(steamPath))
        {
            steamPath = GetSteamInstallPathFromRegistry(RegistryView.Registry32);
        }
        return steamPath;
    }

    private static string GetSteamInstallPathFromRegistry(RegistryView registryView)
    {
        using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(STEAM_REG_KEY))
        {
            if (key != null)
            {
                string steamPath = key.GetValue("InstallPath") as string;
                if (!string.IsNullOrEmpty(steamPath))
                {
                    return steamPath;
                }
            }
        }
        return null;
    }

    private static List<string> GetSteamLibraryFolders(string steamPath)
    {
        var libraries = new List<string>();

        string configPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
        if (File.Exists(configPath))
        {
            string content = File.ReadAllText(configPath);

            // Regex to match library paths
            var matches = Regex.Matches(content, @"^\s*""path""\s*""([^""]+)""", RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                string path = match.Groups[1].Value.Replace(@"\\", @"\");
                libraries.Add(Path.Combine(path, "steamapps", "common"));
            }

            // For older versions, match numeric keys
            matches = Regex.Matches(content, @"^\s*""\d+""\s*""([^""]+)""", RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                string path = match.Groups[1].Value.Replace(@"\\", @"\");
                libraries.Add(Path.Combine(path, "steamapps", "common"));
            }
        }

        // Add default Steam library
        libraries.Add(Path.Combine(steamPath, "steamapps", "common"));

        return libraries;
    }
}
