using System;
using System.IO;
using UnityEngine;

namespace SimpleFrameworkTestMod
{
    public class SimpleFrameworkTestMod : ISrPlugin
    {
        private string _logPath;

        public void Initialize()
        {
            try
            {
                _logPath = Path.Combine(Application.dataPath, "..", "SimpleFrameworkTestResults.txt");
                
                // Test the most basic functionality first
                string testResults = "";
                testResults += "=== Simple .NET Framework Test ===" + Environment.NewLine;
                testResults += "Test started at: " + DateTime.Now.ToString() + Environment.NewLine;
                testResults += "" + Environment.NewLine;

                // Test 1: Check current runtime version
                try
                {
                    string frameworkVersion = Environment.Version.ToString();
                    testResults += "--- Runtime Version Test ---" + Environment.NewLine;
                    testResults += "Environment.Version: " + frameworkVersion + Environment.NewLine;
                    testResults += "SUCCESS: Runtime version detection works" + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    testResults += "FAILED: Runtime version test failed - " + ex.Message + Environment.NewLine;
                }

                // Test 2: Test System.Array.Empty (the critical failing method)
                try
                {
                    testResults += "--- System.Array.Empty Test ---" + Environment.NewLine;
                    
                    // This was the specific method that was failing before
                    string[] emptyStringArray = Array.Empty<string>();
                    int[] emptyIntArray = Array.Empty<int>();
                    
                    testResults += "Array.Empty<string>() length: " + emptyStringArray.Length.ToString() + Environment.NewLine;
                    testResults += "Array.Empty<int>() length: " + emptyIntArray.Length.ToString() + Environment.NewLine;
                    testResults += "SUCCESS: System.Array.Empty<T>() works correctly" + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    testResults += "FAILED: System.Array.Empty test failed - " + ex.Message + Environment.NewLine;
                    
                    // Test fallback approach
                    try
                    {
                        string[] fallbackArray = new string[0];
                        testResults += "Fallback new string[0] length: " + fallbackArray.Length.ToString() + Environment.NewLine;
                        testResults += "INFO: Fallback array creation works" + Environment.NewLine;
                    }
                    catch (Exception fallbackEx)
                    {
                        testResults += "FAILED: Even fallback array creation failed - " + fallbackEx.Message + Environment.NewLine;
                    }
                }

                // Test 3: Basic exception handling
                try
                {
                    testResults += "--- Exception Handling Test ---" + Environment.NewLine;
                    
                    try
                    {
                        // Intentionally cause an exception to test handling
                        int zero = 0;
                        int result = 10 / zero;
                    }
                    catch (DivideByZeroException dzEx)
                    {
                        testResults += "Caught specific exception: " + dzEx.GetType().Name + Environment.NewLine;
                    }
                    catch (Exception generalEx)
                    {
                        testResults += "Caught general exception: " + generalEx.GetType().Name + Environment.NewLine;
                    }
                    
                    testResults += "SUCCESS: Exception handling works correctly" + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    testResults += "FAILED: Exception handling test failed - " + ex.Message + Environment.NewLine;
                }

                testResults += "" + Environment.NewLine;
                testResults += "=== Test Summary ===" + Environment.NewLine;
                testResults += "Simple framework test completed" + Environment.NewLine;
                
                // Save results to file
                try
                {
                    File.WriteAllText(_logPath, testResults);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[SimpleFrameworkTestMod] Failed to save results: " + ex.Message);
                }

                Debug.Log("[SimpleFrameworkTestMod] Initialization completed - check SimpleFrameworkTestResults.txt");
            }
            catch (Exception ex)
            {
                Debug.LogError("[SimpleFrameworkTestMod] Failed to initialize: " + ex.Message);
                Debug.LogError("[SimpleFrameworkTestMod] Stack trace: " + ex.StackTrace);
            }
        }

        public void Update()
        {
            // Test mod doesn't need frame updates, but implement for interface compliance
        }

        public string GetName()
        {
            return "Simple Framework Test Mod v1.0";
        }
    }
}