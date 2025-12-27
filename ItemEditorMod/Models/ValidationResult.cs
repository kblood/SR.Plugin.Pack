using System;
using System.Collections.Generic;

namespace ItemEditorMod.Models
{
    /// <summary>
    /// ValidationResult - Contains validation errors and warnings
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }

        public ValidationResult()
        {
            IsValid = true;
            Errors = new List<string>();
            Warnings = new List<string>();
        }
    }
}
