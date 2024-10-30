using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRMod.DTOs
{
    public class GameObjectDTO
    {
        public string name { get; set; }
        public HideFlags hideFlags { get; set; }
        public string tag { get; set; }
        public bool isStatic { get; set; }
        public bool activeInHierarchy { get; set; }
        public bool activeSelf { get; set; }
        [Obsolete("GameObject.active is obsolete. Use GameObject.SetActive(), GameObject.activeSelf or GameObject.activeInHierarchy.")]
        public bool active { get; set; }
        public int layer { get; set; }
        public TransformDTO transform { get; set; }
        public string sceneName { get; set; }
        //public GameObject gameObject { get; }
    }
}
