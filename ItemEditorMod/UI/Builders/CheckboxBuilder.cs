using System;
using UnityEngine;
using UnityEngine.UI;

namespace ItemEditorMod.UI.Builders
{
    /// <summary>
    /// CheckboxBuilder - Creates labeled checkboxes
    /// Provides helper methods for creating consistent labeled checkbox UI elements
    /// </summary>
    public static class CheckboxBuilder
    {
        /// <summary>
        /// Create a labeled checkbox (label on right of checkbox)
        /// </summary>
        public static GameObject CreateLabeledCheckbox(Transform parent, string label,
            bool initialValue = false, Action<bool> onValueChanged = null)
        {
            try
            {
                // Create horizontal container
                var container = new GameObject("Checkbox_" + label);
                var containerRect = container.AddComponent<RectTransform>();
                containerRect.SetParent(parent);
                containerRect.localScale = Vector3.one;
                containerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                containerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 30);

                // Add horizontal layout group
                var hlg = container.AddComponent<HorizontalLayoutGroup>();
                hlg.childForceExpandHeight = true;
                hlg.spacing = 10;
                hlg.padding = new RectOffset(5, 5, 5, 5);

                // Create checkbox button
                var checkboxGO = new GameObject("Checkbox");
                var checkboxRect = checkboxGO.AddComponent<RectTransform>();
                checkboxRect.SetParent(container.transform);
                checkboxRect.SetPreferredSize(20, 20);

                var checkboxImage = checkboxGO.AddComponent<Image>();
                checkboxImage.color = new Color(0.2f, 0.2f, 0.2f, 1);

                var toggle = checkboxGO.AddComponent<Toggle>();
                toggle.isOn = initialValue;

                // Create checkbox graphics
                var graphicGO = new GameObject("Checkmark");
                var graphicRect = graphicGO.AddComponent<RectTransform>();
                graphicRect.SetParent(checkboxGO.transform);
                graphicRect.SetFullRect();

                var graphic = graphicGO.AddComponent<Image>();
                graphic.color = new Color(0.2f, 0.8f, 0.2f, 1);

                toggle.graphic = graphic;
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    onValueChanged?.Invoke(isOn);
                });

                // Create label
                var labelGO = new GameObject("Label");
                var labelRect = labelGO.AddComponent<RectTransform>();
                labelRect.SetParent(container.transform);
                labelRect.SetPreferredSize(250, 0);

                var labelText = labelGO.AddComponent<Text>();
                labelText.text = label;
                labelText.font = Resources.Load<Font>("Arial");
                labelText.fontSize = 14;
                labelText.fontStyle = FontStyle.Normal;
                labelText.alignment = TextAnchor.MiddleLeft;
                labelText.color = Color.white;

                return container;
            }
            catch (Exception e)
            {
                Debug.LogError($"CheckboxBuilder: CreateLabeledCheckbox failed: {e.Message}");
                return null;
            }
        }
    }
}
