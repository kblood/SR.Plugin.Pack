using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NetworkingReplacementMod.Services
{
    public class FileManager
    {
        public static bool isLogging = true;
        public static bool isVerboseLogging = true; // Extra detailed logging for debugging

        /// <summary>
        /// Log a message with timestamp to both Unity console and log file
        /// </summary>
        public static void Log(string message)
        {
            if (!isLogging) return;

            string timestampedMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} [NET]: {message}";
            
            // Log to Unity console
            Debug.Log(timestampedMessage);
            
            try
            {
                // Log to file
                var logList = LoadList(GetLogFilePath());
                logList.Add(timestampedMessage);
                
                // Keep only last 1000 entries to prevent huge log files
                if (logList.Count > 1000)
                {
                    var newList = new List<string>();
                    int startIndex = logList.Count - 1000;
                    for (int i = startIndex; i < logList.Count; i++)
                    {
                        newList.Add(logList[i]);
                    }
                    logList = newList;
                }
                
                SaveList(logList, GetLogFilePath());
            }
            catch (Exception e)
            {
                Debug.LogError($"NetworkingReplacementMod: Failed to write to log file: {e.Message}");
            }
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        public static void LogWarning(string message)
        {
            if (!isLogging) return;

            string timestampedMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} [NET-WARN]: {message}";
            
            Debug.LogWarning(timestampedMessage);
            
            try
            {
                var logList = LoadList(GetLogFilePath());
                logList.Add(timestampedMessage);
                SaveList(logList, GetLogFilePath());
            }
            catch (Exception e)
            {
                Debug.LogError($"NetworkingReplacementMod: Failed to write warning to log file: {e.Message}");
            }
        }

        /// <summary>
        /// Log error message
        /// </summary>
        public static void LogError(string message)
        {
            string timestampedMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} [NET-ERROR]: {message}";
            
            Debug.LogError(timestampedMessage);
            
            try
            {
                var logList = LoadList(GetLogFilePath());
                logList.Add(timestampedMessage);
                SaveList(logList, GetLogFilePath());
            }
            catch (Exception e)
            {
                Debug.LogError($"NetworkingReplacementMod: Failed to write error to log file: {e.Message}");
            }
        }

        /// <summary>
        /// Log verbose debug information (only when verbose logging is enabled)
        /// </summary>
        public static void LogVerbose(string message)
        {
            if (!isLogging || !isVerboseLogging) return;

            string timestampedMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} [NET-VERBOSE]: {message}";
            
            Debug.Log(timestampedMessage);
            
            try
            {
                var logList = LoadList(GetLogFilePath());
                logList.Add(timestampedMessage);
                SaveList(logList, GetLogFilePath());
            }
            catch (Exception e)
            {
                Debug.LogError($"NetworkingReplacementMod: Failed to write verbose log: {e.Message}");
            }
        }

        /// <summary>
        /// Log method entry (for debugging method calls) - Simplified to avoid LINQ issues
        /// </summary>
        public static void LogMethodEntry(string methodName, params object[] parameters)
        {
            if (!isVerboseLogging) return;
            
            LogVerbose($"ENTER {methodName}()");
        }

        /// <summary>
        /// Log method exit (for debugging method calls)
        /// </summary>
        public static void LogMethodExit(string methodName, object returnValue = null)
        {
            if (!isVerboseLogging) return;

            string returnString = returnValue != null ? $" -> {returnValue}" : "";
            LogVerbose($"EXIT {methodName}{returnString}");
        }

        /// <summary>
        /// Log an exception with full details
        /// </summary>
        public static void LogException(string context, Exception exception)
        {
            string message = $"EXCEPTION in {context}: {exception.GetType().Name}: {exception.Message}";
            if (exception.StackTrace != null)
            {
                message += $"\nStack Trace:\n{exception.StackTrace}";
            }
            
            LogError(message);
        }

        /// <summary>
        /// Get the path to the log file
        /// </summary>
        private static string GetLogFilePath()
        {
            try
            {
                return Path.Combine(Manager.GetPluginManager().PluginPath, "NetworkingReplacementMod.log");
            }
            catch
            {
                // Fallback if plugin manager is not available
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NetworkingReplacementMod.log");
            }
        }

        /// <summary>
        /// Save a list of strings to a file - NO LINQ TO AVOID Array.Empty ISSUES
        /// </summary>
        private static void SaveList(List<string> stringsToSave, string fileNameWithPath)
        {
            try
            {
                // Convert List to Array manually to avoid LINQ issues
                var arrayToSave = new string[stringsToSave.Count];
                for (int i = 0; i < stringsToSave.Count; i++)
                {
                    arrayToSave[i] = stringsToSave[i];
                }
                File.WriteAllLines(fileNameWithPath, arrayToSave);
            }
            catch (Exception e)
            {
                Debug.LogError($"NetworkingReplacementMod: Failed to save list to {fileNameWithPath}: {e.Message}");
            }
        }

        /// <summary>
        /// Load a list of strings from a file
        /// </summary>
        private static List<string> LoadList(string fileNameWithPath)
        {
            try
            {
                if (File.Exists(fileNameWithPath))
                {
                    var lines = File.ReadAllLines(fileNameWithPath);
                    var result = new List<string>();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        result.Add(lines[i]);
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"NetworkingReplacementMod: Failed to load list from {fileNameWithPath}: {e.Message}");
            }
            
            return new List<string>();
        }

        /// <summary>
        /// Clear the log file
        /// </summary>
        public static void ClearLog()
        {
            try
            {
                string logPath = GetLogFilePath();
                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                }
                Log("Log file cleared");
            }
            catch (Exception e)
            {
                Debug.LogError($"NetworkingReplacementMod: Failed to clear log file: {e.Message}");
            }
        }

        /// <summary>
        /// Log networking state information
        /// </summary>
        public static void LogNetworkState(string context, object networkObject = null)
        {
            try
            {
                string message = $"NETWORK STATE - {context}";
                
                if (networkObject != null)
                {
                    message += $" | Object: {networkObject.GetType().Name}";
                }

                // Add game state information
                if (Manager.Get() != null)
                {
                    message += " | Manager: Available";
                    
                    if (Manager.GetUIManager() != null)
                    {
                        message += " | UI: Available";
                    }
                    else
                    {
                        message += " | UI: NULL";
                    }
                }
                else
                {
                    message += " | Manager: NULL";
                }

                LogVerbose(message);
            }
            catch (Exception e)
            {
                LogException("LogNetworkState", e);
            }
        }

        /// <summary>
        /// Log Unity Time information for debugging timing issues
        /// </summary>
        public static void LogTimeInfo(string context)
        {
            try
            {
                string message = $"TIME INFO - {context} | " +
                               $"Time.time: {Time.time:F3} | " +
                               $"Time.realtimeSinceStartup: {Time.realtimeSinceStartup:F3} | " +
                               $"Time.frameCount: {Time.frameCount}";
                
                LogVerbose(message);
            }
            catch (Exception e)
            {
                LogException("LogTimeInfo", e);
            }
        }
    }
}