using System;
using System.Collections;
using UnityEngine;
using ItemEditorMod.Services;
using ItemEditorMod.UI;

namespace ItemEditorMod
{
    /// <summary>
    /// ItemEditorMod - In-game item editor for Satellite Reign
    /// Allows players to create, edit, and validate items with a full-featured UI
    /// Changes are saved to XML and auto-loaded by LoadCustomData
    /// </summary>
    public class ItemEditorMod : ISrPlugin
    {
        #region Fields
        private bool isInitialized = false;
        private ItemEditorUI editorUI;
        private ItemEditorService editorService;
        private ValidationService validationService;
        private IconManagementService iconService;
        private ItemCloneService cloneService;

        private bool editorVisible = false;
        private const KeyCode EDITOR_HOTKEY = KeyCode.F8;
        #endregion

        #region ISrPlugin Implementation

        public string GetName()
        {
            return "ItemEditorMod v1.0";
        }

        public void Initialize()
        {
            try
            {
                Debug.Log("ItemEditorMod: Initializing...");

                // Initialize services
                editorService = new ItemEditorService();
                validationService = new ValidationService();
                iconService = new IconManagementService();
                cloneService = new ItemCloneService();

                // Initialize UI
                editorUI = new ItemEditorUI(editorService, validationService, iconService, cloneService);

                isInitialized = true;
                Debug.Log("ItemEditorMod: Initialization complete");

                // Show initialization message
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("ItemEditorMod loaded! Press F8 to open the in-game item editor.", 5);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: Initialization failed: {e.Message}\n{e.StackTrace}");
            }
        }

        public void Update()
        {
            if (!isInitialized)
                return;

            try
            {
                // Check for F8 hotkey to toggle editor
                if (Input.GetKeyDown(EDITOR_HOTKEY))
                {
                    if (editorVisible)
                    {
                        HideEditor();
                    }
                    else
                    {
                        ShowEditor();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: Update error: {e.Message}\n{e.StackTrace}");
            }
        }

        #endregion

        #region Private Methods

        private void ShowEditor()
        {
            try
            {
                editorVisible = true;
                Debug.Log("ItemEditorMod: Showing editor");

                if (Manager.Get() != null)
                {
                    Manager.Get().StartCoroutine(ShowEditorRoutine());
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowEditor failed: {e.Message}");
                editorVisible = false;
            }
        }

        private void HideEditor()
        {
            try
            {
                editorVisible = false;
                Debug.Log("ItemEditorMod: Hiding editor");
                editorUI.Hide();
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: HideEditor failed: {e.Message}");
            }
        }

        private IEnumerator ShowEditorRoutine()
        {
            // Disable game input
            bool inputControlWasActive = Manager.GetUIManager().InputControlUi.gameObject.activeSelf;
            Manager.GetUIManager().InputControlUi.gameObject.SetActive(false);
            Manager.ptr.DisableKeyCommands();

            // Show editor UI
            editorUI.Show();

            // Wait for user to close editor
            yield return new WaitUntil(() => !editorVisible);

            // Re-enable game input
            try
            {
                Manager.GetUIManager().InputControlUi.gameObject.SetActive(inputControlWasActive);
                Manager.ptr.EnableKeyCommands();
                Debug.Log("ItemEditorMod: Editor closed");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowEditorRoutine cleanup failed: {e.Message}");
                editorVisible = false;
                Manager.ptr.EnableKeyCommands();
            }
        }

        #endregion
    }
}
