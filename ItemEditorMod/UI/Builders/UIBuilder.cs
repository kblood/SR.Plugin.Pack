using System;
using UnityEngine;
using UnityEngine.UI;

namespace ItemEditorMod.UI.Builders
{
    /// <summary>
    /// UIBuilder - Base UI construction utilities
    /// Provides helper methods for creating common UI elements
    /// </summary>
    public static class UIBuilder
    {
        /// <summary>
        /// Create a labeled input field
        /// </summary>
        public static GameObject CreateLabeledInputField(Transform parent, string label, string initialValue = "")
        {
            return new GameObject("InputField");
        }

        /// <summary>
        /// Create a labeled checkbox
        /// </summary>
        public static GameObject CreateLabeledCheckbox(Transform parent, string label, bool initialValue = false)
        {
            return new GameObject("Checkbox");
        }

        /// <summary>
        /// Create a button
        /// </summary>
        public static Button CreateButton(Transform parent, string buttonText)
        {
            var buttonGO = new GameObject("Button");
            buttonGO.transform.SetParent(parent);
            return buttonGO.AddComponent<Button>();
        }
    }
}
