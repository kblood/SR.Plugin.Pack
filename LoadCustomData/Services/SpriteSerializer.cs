using UnityEngine;
using System.IO;
using SRMod.Services;

public class SpriteSerializer
{
    // Method to serialize and save sprite texture to disk
    public static void SaveSpriteToDisk(Sprite sprite, string filePath, bool asPng = true)
    {
        if (sprite == null)
        {
            SRInfoHelper.Log("Sprite is null, cannot save!");
            return;
        }

        // Get the texture from the sprite
        Texture2D texture = sprite.texture;

        // Create a temporary texture if we need to handle only a portion of the texture
        Rect spriteRect = sprite.textureRect;
        Texture2D newTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);

        // Copy the pixels from the original texture to the new texture
        Color[] pixels = texture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            (int)spriteRect.width,
            (int)spriteRect.height);

        newTexture.SetPixels(pixels);
        newTexture.Apply();  // Apply changes to the texture

        // Encode texture into PNG or JPEG format
        byte[] fileData;
        if (asPng)
        {
            fileData = newTexture.EncodeToPNG();  // PNG format
        }
        else
        {
            fileData = newTexture.EncodeToJPG();  // JPG format
        }

        // Save the file to disk
        File.WriteAllBytes(filePath, fileData);
        SRInfoHelper.Log($"Sprite saved to {filePath}");
    }

    // Method to load a texture from disk and create a sprite
    public static Sprite UpdateSprite(Texture2D texture, Sprite existingSprite)
    {
        // Handle null existing sprite for new items - use default sprite properties
        if (existingSprite == null)
        {
            // Create sprite with default properties for new items
            Sprite newSprite = Sprite.Create(texture,
                                             new Rect(0, 0, texture.width, texture.height), // Use full texture rect
                                             new Vector2(0.5f, 0.5f),                       // Center pivot
                                             100.0f                                          // Default pixels per unit
                                             );
            return newSprite;
        }
        else
        {
            // Use existing sprite's properties for updating existing items
            Sprite newSprite = Sprite.Create(texture,
                                             existingSprite.rect,      // Use the original sprite's rect
                                             existingSprite.pivot,     // Use the original sprite's pivot
                                             existingSprite.pixelsPerUnit
                                             ); // Use the original sprite's pixels per unit
            return newSprite;
        }
    }
}
