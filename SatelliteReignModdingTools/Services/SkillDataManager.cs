using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SRMod.Services; // for SRInfoHelper logging

namespace SatelliteReignModdingTools.Services
{
    // Minimal manager to wire into future UI. Read-only friendly; Save writes a snapshot.
    public sealed class SkillDataManager : IGameDataManager
    {
        public DTOs.SerializableSkillTree SkillTree { get; private set; } = new DTOs.SerializableSkillTree();
        public DTOs.SerializableProgressionCurve Progression { get; private set; } = new DTOs.SerializableProgressionCurve();

        public const string SkillTreeFile = "skillTree.xml"; // placeholder until LoadCustomData exports final filename
        public const string ProgressionFile = "progressionData.xml";

        public bool Load(string basePath)
        {
            try
            {
                var loadedAny = false;
                var treePath = Path.Combine(basePath ?? string.Empty, SkillTreeFile);
                if (File.Exists(treePath))
                {
                    SkillTree = Deserialize<DTOs.SerializableSkillTree>(treePath) ?? new DTOs.SerializableSkillTree();
                    loadedAny = true;
                }

                var progPath = Path.Combine(basePath ?? string.Empty, ProgressionFile);
                if (File.Exists(progPath))
                {
                    Progression = Deserialize<DTOs.SerializableProgressionCurve>(progPath) ?? new DTOs.SerializableProgressionCurve();
                    loadedAny = true;
                }

                if (!loadedAny)
                {
                    SRInfoHelper.Log("SkillDataManager: No skill/progression XML found; UI will show placeholders.");
                }
                return true;
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("SkillDataManager.Load failed: " + ex);
                return false;
            }
        }

        public bool Save(string basePath)
        {
            return Save(basePath, backupExisting: false);
        }

        public bool Save(string basePath, bool backupExisting)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(basePath)) basePath = ".";
                Directory.CreateDirectory(basePath);

                var treePath = Path.Combine(basePath, SkillTreeFile);
                var progPath = Path.Combine(basePath, ProgressionFile);

                if (backupExisting)
                {
                    BackupIfExists(treePath);
                    BackupIfExists(progPath);
                }

                Serialize(treePath, SkillTree);
                Serialize(progPath, Progression);
                return true;
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("SkillDataManager.Save failed: " + ex);
                return false;
            }
        }

        public string GetSkillTreeXml()
        {
            try
            {
                return SerializeToString(SkillTree);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("SkillDataManager.GetSkillTreeXml failed: " + ex);
                return string.Empty;
            }
        }

        public string GetProgressionXml()
        {
            try
            {
                return SerializeToString(Progression);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("SkillDataManager.GetProgressionXml failed: " + ex);
                return string.Empty;
            }
        }

        public ValidationResult Validate()
        {
            try
            {
                var errors = new System.Collections.Generic.List<string>();
                var warnings = new System.Collections.Generic.List<string>();

                // Basic checks: duplicate IDs, prerequisite cycles (shallow), XP monotonicity
                var skills = SkillTree.Skills ?? Array.Empty<DTOs.SerializableSkillData>();
                var ids = skills.Select(s => s.Id).ToArray();
                if (ids.Length != ids.Distinct().Count())
                    errors.Add("Duplicate skill IDs detected.");

                var prereqSet = new System.Collections.Generic.HashSet<int>(skills.SelectMany(s => s.Prerequisites ?? Array.Empty<int>()));
                foreach (var p in prereqSet)
                {
                    if (!ids.Contains(p)) warnings.Add("Prerequisite references missing skill ID: " + p);
                }

                var points = Progression.Points ?? Array.Empty<DTOs.ProgressionPoint>();
                var lastXP = -1;
                foreach (var pt in points.OrderBy(p => p.Level))
                {
                    if (pt.XP < lastXP)
                        errors.Add("Progression curve is not monotonic increasing at level " + pt.Level);
                    lastXP = pt.XP;
                }

                return new ValidationResult
                {
                    IsValid = errors.Count == 0,
                    Errors = errors.ToArray(),
                    Warnings = warnings.ToArray()
                };
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("SkillDataManager.Validate failed: " + ex);
                return ValidationResult.From("Validation threw an exception; see logs.");
            }
        }

        private static T Deserialize<T>(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var xs = new XmlSerializer(typeof(T));
                return (T)xs.Deserialize(fs);
            }
        }

        private static void Serialize<T>(string path, T data)
        {
            using (var fs = File.Create(path + ".tmp"))
            {
                var xs = new XmlSerializer(typeof(T));
                xs.Serialize(fs, data);
            }
            if (File.Exists(path)) File.Replace(path + ".tmp", path, null);
            else File.Move(path + ".tmp", path);
        }

        private static string SerializeToString<T>(T data)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, data);
                return sw.ToString();
            }
        }

        private static void BackupIfExists(string path)
        {
            try
            {
                if (!File.Exists(path)) return;
                var bak = path + ".bak." + DateTime.Now.ToString("yyyyMMddHHmmss");
                File.Copy(path, bak, overwrite: false);
            }
            catch { }
        }
    }
}
