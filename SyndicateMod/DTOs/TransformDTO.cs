using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace SyndicateMod.DTOs
{
    public class TransformDTO
    {
        public Vector3 localPosition { get; set; }
        public Vector3 eulerAngles { get; set; }
        public Vector3 localEulerAngles { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 localScale { get; set; }
        //public Transform parent { get; set; }
        public Quaternion localRotation { get; set; }

        [XmlArray("ChildrenList")]
        public List<GameObjectDTO> Children { get; set; }
        [XmlArray("ComponentList")]
        public List<Component> Components { get; set; }

    }
}
