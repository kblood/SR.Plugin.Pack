using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ItemEditorMod.Services
{
    /// <summary>
    /// IconManagementService - Manages icon loading, caching, and sprite creation
    /// </summary>
    public class IconManagementService
    {
        #region Fields

        private Dictionary<string, Texture2D> _iconCache;
        private List<string> _availableIconNames;
        private string _iconsPath;

        #endregion

        #region Constructor

        public IconManagementService()
        {
            _iconCache = new Dictionary<string, Texture2D>();
            _availableIconNames = new List<string>();

            // Try to locate icons directory
            string basePath = Path.Combine(Application.persistentDataPath, "ItemEditorMod");
            _iconsPath = Path.Combine(basePath, "icons");

            if (!Directory.Exists(_iconsPath))
            {
                Directory.CreateDirectory(_iconsPath);
                Debug.Log($"IconManagementService: Created icons directory at {_iconsPath}");
            }

            LoadAvailableIcons();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load all available icons from icons directory
        /// </summary>
        public void LoadAvailableIcons()
        {
            try
            {
                _availableIconNames.Clear();
                _iconCache.Clear();

                if (!Directory.Exists(_iconsPath))
                {
                    Debug.LogWarning($"IconManagementService: Icons directory not found: {_iconsPath}");
                    return;
                }

                var pngFiles = Directory.GetFiles(_iconsPath, "*.png");
                Debug.Log($"IconManagementService: Found {pngFiles.Length} icon files");

                foreach (var filePath in pngFiles)
                {
                    string iconName = Path.GetFileNameWithoutExtension(filePath);
                    _availableIconNames.Add(iconName);
                }

                _availableIconNames.Sort();
            }
            catch (Exception e)
            {
                Debug.LogError($"IconManagementService: LoadAvailableIcons failed: {e.Message}");
            }
        }

        /// <summary>
        /// Get all available icon names
        /// </summary>
        public List<string> GetAvailableIconNames()
        {
            return _availableIconNames;
        }

        /// <summary>
        /// Load texture from file
        /// </summary>
        public Texture2D GetIconTexture(string iconName)
        {
            try
            {
                // Check cache first
                if (_iconCache.ContainsKey(iconName))
                {
                    return _iconCache[iconName];
                }

                string filePath = Path.Combine(_iconsPath, iconName + ".png");
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"IconManagementService: Icon file not found: {filePath}");
                    return null;
                }

                // Load texture
                byte[] imageData = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
                texture.LoadImage(imageData);
                texture.name = iconName;

                // Cache it
                _iconCache[iconName] = texture;

                return texture;
            }
            catch (Exception e)
            {
                Debug.LogError($"IconManagementService: GetIconTexture failed for {iconName}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create sprite from texture
        /// </summary>
        public Sprite CreateSprite(string iconName)
        {
            try
            {
                Texture2D texture = GetIconTexture(iconName);
                if (texture == null)
                    return null;

                return Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f);
            }
            catch (Exception e)
            {
                Debug.LogError($"IconManagementService: CreateSprite failed: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Search icons by name
        /// </summary>
        public List<string> SearchIcons(string searchTerm)
        {
            var results = new List<string>();
            string searchLower = searchTerm.ToLower();

            foreach (var iconName in _availableIconNames)
            {
                if (iconName.ToLower().Contains(searchLower))
                {
                    results.Add(iconName);
                }
            }

            return results;
        }

        #endregion
    }
}
