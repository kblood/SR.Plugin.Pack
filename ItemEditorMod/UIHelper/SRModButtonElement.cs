using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ItemEditorMod.UIHelper
{
    public class SRModButtonElement
    {
        public string ButtonText;
        public string Description;
        public UnityAction Action;
        public Button Button;
        public Text Text;
        public Text DescriptionText;
        public Transform Container;

        public SRModButtonElement(string buttonText, UnityAction action, string description)
        {
            ButtonText = buttonText;
            Action = action;
            Description = description;
            //Container = container;
            //Button.onClick.AddListener(action);
        }
    }
}
