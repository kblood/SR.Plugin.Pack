using System;
using BepInEx.Logging;

namespace SatelliteReignNetworkingFix.Utils
{
    /// <summary>
    /// Enhanced logging system for the Networking Fix plugin
    /// Provides structured logging with different severity levels
    /// </summary>
    public class Logger
    {
        private readonly string prefix;
        private readonly ManualLogSource logSource;
        
        public Logger(string prefix)
        {
            this.prefix = prefix;
            // Get BepInEx's logging system
            this.logSource = BepInEx.Logging.Logger.CreateLogSource("NetworkingFix");
        }
        
        public void Info(string message)
        {
            LogMessage(LogLevel.Info, message);
        }
        
        public void Warn(string message)
        {
            LogMessage(LogLevel.Warning, message);
        }
        
        public void Error(string message)
        {
            LogMessage(LogLevel.Error, message);
        }
        
        public void Debug(string message)
        {
            LogMessage(LogLevel.Debug, message);
        }
        
        public void Fatal(string message)
        {
            LogMessage(LogLevel.Fatal, message);
        }
        
        private void LogMessage(LogLevel level, string message)
        {
            try
            {
                string formattedMessage = prefix + " " + message;
                logSource.Log(level, formattedMessage);
                
                // Also log to Unity console for development
                if (level == LogLevel.Error || level == LogLevel.Fatal)
                {
                    UnityEngine.Debug.LogError(formattedMessage);
                }
                else if (level == LogLevel.Warning)
                {
                    UnityEngine.Debug.LogWarning(formattedMessage);
                }
                else
                {
                    UnityEngine.Debug.Log(formattedMessage);
                }
            }
            catch (Exception e)
            {
                // Fallback logging if BepInEx logging fails
                UnityEngine.Debug.LogError("[NetworkingFix] Logging system error: " + e.Message);
                UnityEngine.Debug.LogError("[NetworkingFix] Original message: " + message);
            }
        }
    }
}