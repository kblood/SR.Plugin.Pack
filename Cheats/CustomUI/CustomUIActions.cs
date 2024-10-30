using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Cheats.CustomUI
{
    
    public delegate bool Action();
    
    public class CustomUIActions
    {
        public CustomUIActions()
        {
        }

        public CustomUIActions(string name, Action action)
        {
            Name = name;
            Action = action;
            Active = false;
        }

        public string Name { get; set; }
        public Action Action { get; set; }
        public bool Active { get; set; }

    }
}
