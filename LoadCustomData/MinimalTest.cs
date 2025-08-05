using System;
using UnityEngine;
using SRMod.Services;

namespace MinimalTestMod
{
    /// <summary>
    /// Minimal test mod to verify ISrPlugin loading works
    /// </summary>
    public class MinimalTest : ISrPlugin
    {
        public void Initialize()
        {
            try
            {
                Debug.Log("MinimalTest: Plugin loaded successfully!");
                SRInfoHelper.Log("MinimalTest: Plugin loaded successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError("MinimalTest: Error - " + e.Message);
            }
        }

        public void Update()
        {
            // Minimal update - do nothing
        }

        public string GetName()
        {
            return "Minimal Test Plugin";
        }
    }
}