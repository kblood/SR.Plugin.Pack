using System;
using System.Collections.Generic;
using SRMod.DTOs;

namespace ItemEditorMod.Models
{
    /// <summary>
    /// EditorState - Tracks the current state of the editor session
    /// </summary>
    public class EditorState
    {
        public SerializableItemData CurrentItem { get; set; }
        public string ActiveTab { get; set; }
        public string SearchTerm { get; set; }
        public int SelectedItemId { get; set; }
        public bool IsDirty { get; set; }
        public ValidationResult LastValidation { get; set; }

        public EditorState()
        {
            ActiveTab = "Combat";
            SearchTerm = "";
            SelectedItemId = -1;
            IsDirty = false;
            LastValidation = new ValidationResult();
        }
    }
}
