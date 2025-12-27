using System;
using UnityEngine;
using UnityEngine.UI;

namespace ItemEditorMod.Utilities
{
    /// <summary>
    /// UnityUIExtensions - Helper extension methods for Unity UI
    /// </summary>
    public static class UnityUIExtensions
    {
        /// <summary>
        /// Set layout as 100% width
        /// </summary>
        public static void SetFullWidth(this RectTransform rect)
        {
            rect.anchorMin = new Vector2(0, rect.anchorMin.y);
            rect.anchorMax = new Vector2(1, rect.anchorMax.y);
            rect.offsetMin = new Vector2(0, rect.offsetMin.y);
            rect.offsetMax = new Vector2(0, rect.offsetMax.y);
        }

        /// <summary>
        /// Set layout as 100% height
        /// </summary>
        public static void SetFullHeight(this RectTransform rect)
        {
            rect.anchorMin = new Vector2(rect.anchorMin.x, 0);
            rect.anchorMax = new Vector2(rect.anchorMax.x, 1);
            rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
            rect.offsetMax = new Vector2(rect.offsetMax.x, 0);
        }

        /// <summary>
        /// Set as child to parent while preserving size
        /// </summary>
        public static void SetParentKeepSize(this RectTransform rect, Transform newParent)
        {
            Vector3 oldScale = rect.localScale;
            Vector3 oldPosition = rect.localPosition;

            rect.SetParent(newParent);

            rect.localScale = oldScale;
            rect.localPosition = oldPosition;
        }
    }
}
