using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Services;

namespace FrameworkTestMod
{
    public class FrameworkTestMod : ISrPlugin
    {
        private string _logPath;
        private List<string> _testResults = new List<string>();

        public void Initialize()
        {
            try
            {
                Debug.Log("[FrameworkTestMod] Initializing Framework Test Mod...");
                FileManager.isLogging = true;
                FileManager.Log("Framework Test Mod initializing.");


                // Fix for CS1501: Use Path.Combine with two arguments at a time
                _logPath = Path.Combine(Application.dataPath, "..");
                _logPath = Path.Combine(_logPath, "FrameworkTestResults.txt");
                
                //_logPath = Path.Combine(Application.dataPath, "..", "FrameworkTestResults.txt");
                LogTest("=== .NET Framework Upgrade Test Results ===");
                LogTest("Test started at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                LogTest("");

                // Test 1: Check current runtime version (.NET 2.0 compatible)
                TestRuntimeVersion();

                // Test 2: Test basic array operations (.NET 2.0 baseline)
                TestBasicArrayOperations();

                // Test 3: Test reflection for .NET Framework info
                TestFrameworkInfo();

                // Test 4: Test exception handling
                TestExceptionHandling();

                // Test 5: Test if .NET 4.8 features are available through assembly redirection
                TestNet48Features();

                LogTest("");
                LogTest("=== Test Summary ===");
                LogTest("Total tests completed: " + _testResults.Count.ToString());
                
                // Save results to file
                SaveResults();

                Debug.Log("[FrameworkTestMod] Initialization completed - check FrameworkTestResults.txt");
            }
            catch (Exception ex)
            {
                LogTest("CRITICAL ERROR during initialization: " + ex.Message);
                LogTest("Stack trace: " + ex.StackTrace);
                SaveResults();
                Debug.LogError("[FrameworkTestMod] Failed to initialize: " + ex.Message);
            }
        }

        private void TestRuntimeVersion()
        {
            try
            {
                string frameworkVersion = Environment.Version.ToString();
                
                LogTest("--- Runtime Version Test ---");
                LogTest("Environment.Version: " + frameworkVersion);
                LogTest("SUCCESS: Runtime version detection works");
            }
            catch (Exception ex)
            {
                LogTest("FAILED: Runtime version test failed - " + ex.Message);
            }
        }

        private void TestBasicArrayOperations()
        {
            try
            {
                LogTest("--- Basic Array Operations Test (.NET 2.0) ---");
                
                // Test basic array creation (should work in .NET 2.0)
                string[] basicArray = new string[0];
                int[] basicIntArray = new int[0];
                
                LogTest("new string[0] length: " + basicArray.Length.ToString());
                LogTest("new int[0] length: " + basicIntArray.Length.ToString());
                LogTest("SUCCESS: Basic array creation works");

                // Test basic list operations
                List<string> testList = new List<string>();
                testList.Add("test1");
                testList.Add("test2");
                LogTest("List operations work - count: " + testList.Count.ToString());
            }
            catch (Exception ex)
            {
                LogTest("FAILED: Basic array operations failed - " + ex.Message);
            }
        }


        private void TestFrameworkInfo()
        {
            try
            {
                LogTest("--- Framework Information Test ---");
                
                Assembly mscorlibAssembly = typeof(string).Assembly;
                AssemblyName mscorlibName = mscorlibAssembly.GetName();
                
                LogTest("mscorlib Version: " + mscorlibName.Version.ToString());
                LogTest("mscorlib Location: " + mscorlibAssembly.Location);
                
                Assembly systemAssembly = typeof(System.Collections.Generic.List<string>).Assembly;
                AssemblyName systemName = systemAssembly.GetName();
                
                LogTest("System Assembly Version: " + systemName.Version.ToString());
                LogTest("System Assembly Location: " + systemAssembly.Location);
                
                LogTest("SUCCESS: Framework information retrieval works");
            }
            catch (Exception ex)
            {
                LogTest("FAILED: Framework information test failed - " + ex.Message);
            }
        }

        private void TestExceptionHandling()
        {
            try
            {
                LogTest("--- Exception Handling Test ---");
                
                try
                {
                    // Intentionally cause an exception to test handling
                    int zero = 0;
                    int result = 10 / zero;
                }
                catch (DivideByZeroException dzEx)
                {
                    LogTest("Caught specific exception: " + dzEx.GetType().Name);
                }
                catch (Exception generalEx)
                {
                    LogTest("Caught general exception: " + generalEx.GetType().Name);
                }
                
                LogTest("SUCCESS: Exception handling works correctly");
            }
            catch (Exception ex)
            {
                LogTest("FAILED: Exception handling test failed - " + ex.Message);
            }
        }

        private void TestNet48Features()
        {
            try
            {
                LogTest("--- .NET 4.8 Features Test ---");
                
                // Test 1: Try to detect if Array.Empty exists using reflection
                try
                {
                    Type arrayType = typeof(Array);
                    MethodInfo emptyMethod = arrayType.GetMethod("Empty");
                    if (emptyMethod != null)
                    {
                        LogTest("Array.Empty method detected via reflection");
                        
                        // Try to call it using reflection
                        MethodInfo genericEmpty = emptyMethod.MakeGenericMethod(typeof(string));
                        object result = genericEmpty.Invoke(null, null);
                        string[] emptyArray = (string[])result;
                        LogTest("Array.Empty<string>() via reflection - length: " + emptyArray.Length.ToString());
                        LogTest("SUCCESS: Array.Empty is available!");
                    }
                    else
                    {
                        LogTest("Array.Empty method NOT found via reflection");
                    }
                }
                catch (Exception arrayEx)
                {
                    LogTest("Array.Empty reflection test failed: " + arrayEx.Message);
                }
                
                // Test 2: Check mscorlib version to see if redirection worked
                try
                {
                    Assembly mscorlib = typeof(string).Assembly;
                    Version version = mscorlib.GetName().Version;
                    LogTest("Actual mscorlib version loaded: " + version.ToString());
                    
                    if (version.Major >= 4)
                    {
                        LogTest("SUCCESS: Assembly redirection loaded .NET 4.x mscorlib!");
                    }
                    else
                    {
                        LogTest("INFO: Still using .NET 2.x mscorlib (version " + version.ToString() + ")");
                    }
                }
                catch (Exception versionEx)
                {
                    LogTest("Version check failed: " + versionEx.Message);
                }

                // Test 3: Try to use newer string methods if available
                try
                {
                    string testString = "test";
                    bool hasIsNullOrWhiteSpace = typeof(string).GetMethod("IsNullOrWhiteSpace") != null;
                    LogTest("String.IsNullOrWhiteSpace available: " + hasIsNullOrWhiteSpace.ToString());
                }
                catch (Exception stringEx)
                {
                    LogTest("String method check failed: " + stringEx.Message);
                }
            }
            catch (Exception ex)
            {
                LogTest("FAILED: .NET 4.8 features test failed - " + ex.Message);
            }
        }

        private void LogTest(string message)
        {
            _testResults.Add(message);
            Debug.Log("[FrameworkTestMod] " + message);
        }

        private void SaveResults()
        {
            try
            {
                // Convert List to Array manually to avoid LINQ issues in .NET 2.0
                string[] arrayToSave = new string[_testResults.Count];
                for (int i = 0; i < _testResults.Count; i++)
                {
                    arrayToSave[i] = _testResults[i];
                }
                File.WriteAllLines(_logPath, arrayToSave);
            }
            catch (Exception ex)
            {
                Debug.LogError("[FrameworkTestMod] Failed to save results: " + ex.Message);
            }
        }

        public void Update()
        {
            // Test mod doesn't need frame updates, but implement for interface compliance
        }

        public string GetName()
        {
            return "Framework Test Mod v1.0 - .NET Framework Upgrade Verification";
        }
    }
}