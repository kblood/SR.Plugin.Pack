using System;
using UnityEngine;
using UnityEngine.UI;

namespace ItemEditorMod.UI.Builders
{
    /// <summary>
    /// InputFieldBuilder - Creates labeled input fields
    /// Provides helper methods for creating consistent labeled input field UI elements
    /// </summary>
    public static class InputFieldBuilder
    {
        /// <summary>
        /// Create a labeled input field (label on left, field on right)
        /// </summary>
        public static GameObject CreateLabeledInputField(Transform parent, string label,
            string initialValue = "", Action<string> onValueChanged = null)
        {
            try
            {
                // Create horizontal container
                var container = new GameObject("InputField_" + label);
                var containerRect = container.AddComponent<RectTransform>();
                containerRect.SetParent(parent);
                containerRect.localScale = Vector3.one;
                containerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                containerRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 30);

                // Add horizontal layout group
                var hlg = container.AddComponent<HorizontalLayoutGroup>();
                hlg.childForceExpandWidth = true;
                hlg.childForceExpandHeight = true;
                hlg.spacing = 10;
                hlg.padding = new RectOffset(5, 5, 5, 5);

                // Create label
                var labelGO = new GameObject("Label");
                var labelRect = labelGO.AddComponent<RectTransform>();
                labelRect.SetParent(container.transform);
                labelRect.SetPreferredSize(150, 0);

                var labelText = labelGO.AddComponent<Text>();
                labelText.text = label + ":";
                labelText.font = Resources.Load<Font>("Arial");
                labelText.fontSize = 14;
                labelText.fontStyle = FontStyle.Bold;
                labelText.alignment = TextAnchor.MiddleLeft;
                labelText.color = Color.white;

                // Create input field
                var inputGO = new GameObject("InputField");
                var inputRect = inputGO.AddComponent<RectTransform>();
                inputRect.SetParent(container.transform);

                var image = inputGO.AddComponent<Image>();
                image.color = new Color(0.2f, 0.2f, 0.2f, 1);

                var inputField = inputGO.AddComponent<InputField>();
                inputField.textComponent = CreateInputText(inputGO);
                inputField.text = initialValue;

                if (onValueChanged != null)
                {
                    inputField.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(onValueChanged));
                }

                return container;
            }
            catch (Exception e)
            {
                Debug.LogError($"InputFieldBuilder: CreateLabeledInputField failed: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create a labeled numeric input field
        /// </summary>
        public static GameObject CreateLabeledFloatField(Transform parent, string label,
            float initialValue = 0f, Action<float> onValueChanged = null)
        {
            try
            {
                var container = CreateLabeledInputField(parent, label, initialValue.ToString(),
                    (value) =>
                    {
                        if (float.TryParse(value, out float floatValue))
                        {
                            onValueChanged?.Invoke(floatValue);
                        }
                    });

                return container;
            }
            catch (Exception e)
            {
                Debug.LogError($"InputFieldBuilder: CreateLabeledFloatField failed: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create a labeled integer input field
        /// </summary>
        public static GameObject CreateLabeledIntField(Transform parent, string label,
            int initialValue = 0, Action<int> onValueChanged = null)
        {
            try
            {
                var container = CreateLabeledInputField(parent, label, initialValue.ToString(),
                    (value) =>
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            onValueChanged?.Invoke(intValue);
                        }
                    });

                return container;
            }
            catch (Exception e)
            {
                Debug.LogError($"InputFieldBuilder: CreateLabeledIntField failed: {e.Message}");
                return null;
            }
        }

        private static Text CreateInputText(GameObject parent)
        {
            var textGO = new GameObject("Text");
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.SetParent(parent.transform);
            textRect.SetFullRect();

            var text = textGO.AddComponent<Text>();
            text.text = "";
            text.font = Resources.Load<Font>("Arial");
            text.fontSize = 14;
            text.fontStyle = FontStyle.Normal;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;

            return text;
        }
    }

    /// <summary>
    /// RectTransform extension helpers
    /// </summary>
    public static class RectTransformExtensions
    {
        public static void SetFullRect(this RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static void SetPreferredSize(this RectTransform rect, float width, float height)
        {
            var layoutElement = rect.GetComponent<LayoutElement>() ?? rect.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
            layoutElement.preferredHeight = height;
        }
    }
}
