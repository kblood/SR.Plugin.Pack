using System;

namespace SatelliteReignModdingTools.Services
{
    // Unifies load/save/validate across all data managers without breaking existing services.
    public interface IGameDataManager
    {
        // Load data from disk into memory. Should not throw; return false on failure and log via SRInfoHelper.
        bool Load(string basePath);

        // Save in-memory data to disk. Non-destructive by default (use backups, write to temp and swap).
        bool Save(string basePath);

        // Validate in-memory data and return a simple summary (no exceptions).
        ValidationResult Validate();
    }

    // Lightweight validation container to avoid pulling a dependency tree yet.
    public sealed class ValidationResult
    {
        public bool IsValid { get; set; }
        public string[] Warnings { get; set; } = Array.Empty<string>();
        public string[] Errors { get; set; } = Array.Empty<string>();

        public static ValidationResult Ok()
        {
            return new ValidationResult { IsValid = true };
        }

        public static ValidationResult From(params string[] errors)
        {
            return new ValidationResult { IsValid = errors == null || errors.Length == 0, Errors = errors ?? Array.Empty<string>() };
        }
    }
}
