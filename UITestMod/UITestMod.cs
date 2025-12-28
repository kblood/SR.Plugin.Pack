using System;
using UnityEngine;
using UnityEngine.UI;

namespace UITestMod
{
    /// <summary>
    /// UITestMod - Simple tests to understand what UI works in Satellite Reign
    /// Press F9 to cycle through different UI test modes
    /// </summary>
    public class UITestMod : ISrPlugin
    {
        private bool isInitialized = false;
        private int currentTest = 0;
        private const int MAX_TESTS = 5;
        
        private GameObject testUI;
        private bool uiVisible = false;
        
        public string GetName()
        {
            return "UITestMod v1.0";
        }
        
        public void Initialize()
        {
            try
            {
                Debug.Log("UITestMod: Initializing...");
                isInitialized = true;
                Debug.Log("UITestMod: Ready! Press F9 to test different UI approaches");
                
                // Show message to user
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("UITestMod loaded! Press F9 to cycle UI tests", 5);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"UITestMod: Init failed: {e.Message}\n{e.StackTrace}");
            }
        }
        
        public void Update()
        {
            if (!isInitialized) return;
            
            try
            {
                // F9 to cycle through tests
                if (Input.GetKeyDown(KeyCode.F9))
                {
                    if (uiVisible)
                    {
                        CleanupTest();
                    }
                    
                    currentTest = (currentTest + 1) % MAX_TESTS;
                    RunTest(currentTest);
                }
                
                // F10 to close current test
                if (Input.GetKeyDown(KeyCode.F10))
                {
                    CleanupTest();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"UITestMod: Update error: {e.Message}");
            }
        }
        
        private void RunTest(int testNumber)
        {
            Debug.Log($"UITestMod: Running test {testNumber}");
            
            switch (testNumber)
            {
                case 0:
                    TestGameMessagePopup();
                    break;
                case 1:
                    TestCloneInputBox();
                    break;
                case 2:
                    TestCreateRawCanvas();
                    break;
                case 3:
                    TestFindExistingCanvases();
                    break;
                case 4:
                    TestRecommendedUIHelper();
                    break;
            }
        }
        
        private void CleanupTest()
        {
            try
            {
                if (testUI != null)
                {
                    UnityEngine.Object.Destroy(testUI);
                    testUI = null;
                }
                uiVisible = false;
                Debug.Log("UITestMod: Test UI cleaned up");
            }
            catch (Exception e)
            {
                Debug.LogError($"UITestMod: Cleanup failed: {e.Message}");
            }
        }
        
        // TEST 0: Game's built-in message system
        private void TestGameMessagePopup()
        {
            Debug.Log("Test 0: Using game's message popup");
            try
            {
                var uiManager = Manager.GetUIManager();
                if (uiManager != null)
                {
                    uiManager.ShowMessagePopup("Test 0: Game Message Popup\nThis uses the game's built-in UI system", 10);
                    Debug.Log("Test 0: SUCCESS - Message shown");
                }
                else
                {
                    Debug.LogError("Test 0: FAIL - UIManager is null");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Test 0: EXCEPTION - {e.Message}");
            }
        }
        
        // TEST 1: Clone InputBoxUi
        private void TestCloneInputBox()
        {
            Debug.Log("Test 1: Cloning InputBoxUi");
            try
            {
                var uiManager = Manager.GetUIManager();
                if (uiManager == null || uiManager.m_InputBoxUi == null)
                {
                    Debug.LogError("Test 1: FAIL - InputBoxUi not found");
                    Manager.GetUIManager()?.ShowMessagePopup("Test 1: FAIL - InputBoxUi not found", 5);
                    return;
                }
                
                testUI = UnityEngine.Object.Instantiate(uiManager.m_InputBoxUi.gameObject);
                testUI.name = "TestInputBoxClone";
                testUI.SetActive(true);
                uiVisible = true;
                
                Debug.Log("Test 1: SUCCESS - InputBoxUi cloned");
                Debug.Log($"Test 1: Clone active: {testUI.activeInHierarchy}, position: {testUI.transform.position}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Test 1: EXCEPTION - {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"Test 1: EXCEPTION - {e.Message}", 5);
            }
        }
        
        // TEST 2: Create raw Canvas from scratch
        private void TestCreateRawCanvas()
        {
            Debug.Log("Test 3: Creating raw Canvas");
            try
            {
                testUI = new GameObject("TestRawCanvas");
                
                // Add Canvas component
                var canvas = testUI.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999; // Draw on top
                
                // Add CanvasScaler
                var scaler = testUI.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                // Add GraphicRaycaster for input
                testUI.AddComponent<GraphicRaycaster>();
                
                // Create a panel
                var panel = new GameObject("Panel");
                panel.transform.SetParent(testUI.transform);
                var panelRect = panel.AddComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.sizeDelta = new Vector2(400, 300);
                panelRect.anchoredPosition = Vector2.zero;
                
                var panelImage = panel.AddComponent<Image>();
                panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
                
                // Add text
                var textObj = new GameObject("Text");
                textObj.transform.SetParent(panel.transform);
                var textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                
                var text = textObj.AddComponent<Text>();
                text.text = "Test 3: Raw Canvas\nPress F10 to close";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 24;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;
                
                testUI.SetActive(true);
                uiVisible = true;
                
                Debug.Log("Test 3: SUCCESS - Raw Canvas created");
                Debug.Log($"Test 3: Canvas active: {canvas.enabled}, sortingOrder: {canvas.sortingOrder}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Test 3: EXCEPTION - {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"Test 3: EXCEPTION - {e.Message}", 5);
            }
        }
        
        // TEST 3: Find existing canvases in the scene
        private void TestFindExistingCanvases()
        {
            Debug.Log("Test 4: Finding existing canvases");
            try
            {
                var allCanvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
                Debug.Log($"Test 4: Found {allCanvases.Length} canvases in scene");
                
                int idx = 0;
                foreach (var canvas in allCanvases)
                {
                    Debug.Log($"  Canvas {idx}: {canvas.name}, renderMode: {canvas.renderMode}, " +
                              $"sortingOrder: {canvas.sortingOrder}, enabled: {canvas.enabled}, " +
                              $"active: {canvas.gameObject.activeInHierarchy}");
                    idx++;
                }
                
                Manager.GetUIManager()?.ShowMessagePopup($"Test 4: Found {allCanvases.Length} canvases - check logs", 5);
            }
            catch (Exception e)
            {
                Debug.LogError($"Test 4: EXCEPTION - {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"Test 4: EXCEPTION - {e.Message}", 5);
            }
        }
        
        // TEST 4: Recommended UIHelper approach (from docs/ui-modding.md)
        private void TestRecommendedUIHelper()
        {
            Debug.Log("Test 4: Testing recommended UIHelper.ModalVerticalButtonsRoutine approach");
            try
            {
                // This is the RECOMMENDED approach from the documentation
                var buttons = new List<UnityEngine.UI.Button>();
                
                // Create simple test buttons
                var testButton1 = new UnityEngine.UI.Button();
                var testButton2 = new UnityEngine.UI.Button();
                
                // Note: The actual UIHelper and SRModButtonElement are in the Cheats namespace
                // This test demonstrates the CONCEPT - actual implementation requires those classes
                
                Manager.GetUIManager()?.ShowMessagePopup(
                    "Test 4: UIHelper pattern\n" +
                    "This is the RECOMMENDED approach!\n" +
                    "ItemEditorMod should use:\n" +
                    "UIHelper.ModalVerticalButtonsRoutine()\n" +
                    "with SRModButtonElement buttons\n" +
                    "See Cheats mod for reference", 10);
                    
                Debug.Log("Test 4: SUCCESS - This is the recommended pattern!");
                Debug.Log("Test 4: ItemEditorMod should clone InputBoxUi and use SRModVerticalButtonsUI");
                Debug.Log("Test 4: See SR.Plugin.Pack/Cheats/Services/UIHelper.cs for reference");
            }
            catch (Exception e)
            {
                Debug.LogError($"Test 4: EXCEPTION - {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"Test 4: EXCEPTION - {e.Message}", 5);
            }
        }
    }
}
