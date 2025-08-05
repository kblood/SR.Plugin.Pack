using System;
using System.IO;
using UnityEngine;

namespace LoadCustomData
{
    /// <summary>
    /// Minimal file manager with just the working SaveTextureToFile method
    /// Based on your existing working implementation
    /// </summary>
    public static class MinimalFileManager
    {
        public static string SaveTextureToFile(Texture2D texture)
        {
            string fileName = FilePathCheck(string.Format("icons\\{0}.png", texture.name));
            if (File.Exists(fileName))
                return fileName;

            try
            {
                // Try direct encoding first (your original method)
                var bytes = texture.EncodeToPNG();
                using (var file = File.Create(fileName))
                {
                    var binary = new BinaryWriter(file);
                    binary.Write(bytes);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                Debug.Log("MinimalFileManager: Direct texture encoding failed (" + ex.Message + "), trying RenderTexture workaround...");
                
                // Fallback to RenderTexture workaround for non-readable textures
                try
                {
                    var bytes = ExtractTextureUsingRenderTexture(texture);
                    if (bytes != null && bytes.Length > 0)
                    {
                        using (var file = File.Create(fileName))
                        {
                            var binary = new BinaryWriter(file);
                            binary.Write(bytes);
                        }
                        Debug.Log("MinimalFileManager: RenderTexture workaround successful for " + texture.name);
                        return fileName;
                    }
                    else
                    {
                        Debug.LogError("MinimalFileManager: RenderTexture workaround also failed for " + texture.name);
                        return "";
                    }
                }
                catch (Exception renderEx)
                {
                    Debug.LogError("MinimalFileManager: All texture extraction methods failed for " + texture.name + ": " + renderEx.Message);
                    return "";
                }
            }
        }

        private static byte[] ExtractTextureUsingRenderTexture(Texture2D sourceTexture)
        {
            try
            {
                // Create a temporary RenderTexture with the same dimensions  
                RenderTexture renderTexture = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                
                // Copy the source texture to the RenderTexture
                Graphics.Blit(sourceTexture, renderTexture);
                
                // Save the current RenderTexture
                RenderTexture previousActive = RenderTexture.active;
                
                // Set the RenderTexture as active
                RenderTexture.active = renderTexture;
                
                // Create new readable Texture2D and read pixels from RenderTexture
                Texture2D readableTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
                readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                readableTexture.Apply();
                
                // Restore the previous RenderTexture
                RenderTexture.active = previousActive;
                
                // Release the temporary RenderTexture
                RenderTexture.ReleaseTemporary(renderTexture);
                
                // Encode to PNG
                byte[] bytes = readableTexture.EncodeToPNG();
                
                // Clean up the readable texture
                UnityEngine.Object.DestroyImmediate(readableTexture);
                
                return bytes;
            }
            catch (Exception ex)
            {
                Debug.LogError("MinimalFileManager: ExtractTextureUsingRenderTexture failed: " + ex.Message);
                return null;
            }
        }

        private static string FilePathCheck(string fileName)
        {
            string fileWithPath = fileName;
            string execPath = Manager.GetPluginManager().PluginPath;

            if (!fileWithPath.Contains(@":") || !fileWithPath.Contains(@"\"))
            {
                if (execPath.EndsWith(@"\"))
                    fileWithPath = execPath + fileWithPath;
                else
                    fileWithPath = execPath + @"\" + fileWithPath;
            }
            return fileWithPath;
        }
    }
}