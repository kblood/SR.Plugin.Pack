using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace SyndicateMod.DTOs
{
    public class Sprite
    {
        [XmlArray("verticesArray")]
        public Vector2[] vertices { get; set; }
        public Vector4 border { get; set; }
        public Vector2 pivot { get; set; }
        //public SpritePackingRotation packingRotation { get; set; }
        //public SpritePackingMode packingMode { get; set; }
        public bool packed { get; set; }
        public Vector2 textureRectOffset { get; set; }
        public Rect textureRect { get; set; }
        public string associatedAlphaSplitTextureName { get; set; }
        public string textureName { get; set; }
        public float pixelsPerUnit { get; set; }
        public Rect rect { get; set; }
        public Bounds bounds { get; set; }
        [XmlArray("trianglesArray")]
        public ushort[] triangles { get; set; }
        [XmlArray("uvArray")]
        public Vector2[] uv { get; set; }
    }
}
