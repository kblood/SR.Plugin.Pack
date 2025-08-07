using SRMod.Services;
using SatelliteReignModdingTools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SatelliteReignModdingTools.Controls;

namespace SatelliteReignModdingTools
{
    public partial class QuestBrowser : Form
    {
        private List<Quest> quests = new List<Quest>();
        private List<Quest> filteredQuests = new List<Quest>();
        private List<SRMod.DTOs.SerializableItemData> availableItems = new List<SRMod.DTOs.SerializableItemData>();
        private List<Translation> translations = new List<Translation>();
        private Quest activeQuest = null;
        private bool isEditing = false;
        private bool hasUnsavedChanges = false;
        private QuestReward selectedReward = null;
        private int selectedRewardIndex = -1;
        private const string _questDataFileName = "questDefinitions.xml";
        private const string _translationDataFileName = "translations.xml";

        private SharedToolbar _toolbar;

        public QuestBrowser()
        {
            InitializeComponent();
            InitializeEditor();
            LoadAllData();
            filteredQuests = new List<Quest>(quests);
            UpdateQuestList();
            SetupFormTitle();

            // Subscribe to form closing event
            this.FormClosing += QuestBrowser_FormClosing;

            // Subscribe to text change events
            this.txtQuestTitle.TextChanged += txtQuestTitle_TextChanged;
            this.numLocationID.ValueChanged += numLocationID_ValueChanged;

            // Add shared toolbar and wire events
            _toolbar = new SharedToolbar();
            _toolbar.ReloadClicked += () => { LoadAllData(); filteredQuests = new List<Quest>(quests); UpdateQuestList(); };
            _toolbar.SaveClicked += () => btnSaveQuest_Click(this, EventArgs.Empty);
            _toolbar.SaveWithDiffClicked += () => SaveWithDiff();
            _toolbar.ValidateClicked += () => ShowValidationResults();
            _toolbar.SearchTextChanged += (text) => ApplySearch(text);
            Controls.Add(_toolbar);
            _toolbar.Dock = DockStyle.None;
            _toolbar.Location = new Point(0, 0);
            _toolbar.Width = this.ClientSize.Width;
            _toolbar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // Shift existing controls down by toolbar height
            foreach (Control c in this.Controls)
            {
                if (c == _toolbar) continue;
                c.Top += _toolbar.Height;
            }
            _toolbar.BringToFront();
        }

        private void InitializeEditor()
        {
            // Initialize dropdown controls with validation data
            PopulateDistrictDropdown();
            SetupEditorControls();
            EnableEditMode(false);
        }

        private void SetupFormTitle()
        {
            this.Text = "Quest Editor - Satellite Reign Modding Tools";
        }

        private void ApplySearch(string text)
        {
            text = text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                filteredQuests = new List<Quest>(quests);
            }
            else
            {
                var lower = text.ToLowerInvariant();
                filteredQuests = quests.Where(q =>
                    ($"{q.ID}".Contains(lower)) ||
                    (q.Title?.ToLowerInvariant().Contains(lower) ?? false) ||
                    (q.DisplayName?.ToLowerInvariant().Contains(lower) ?? false) ||
                    (q.TitleKey?.ToLowerInvariant().Contains(lower) ?? false)
                ).ToList();
            }
            UpdateQuestList();
        }

        private void ShowValidationResults()
        {
            var (errors, warnings) = ValidateQuests();
            var sb = new StringBuilder();
            if (errors.Count == 0 && warnings.Count == 0)
            {
                MessageBox.Show(this, "No issues found.", "Validate Quests", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (errors.Count > 0)
            {
                sb.AppendLine("Errors:");
                foreach (var e in errors.Take(100)) sb.AppendLine(" - " + e);
                if (errors.Count > 100) sb.AppendLine($"(+{errors.Count - 100} more)");
                sb.AppendLine();
            }
            if (warnings.Count > 0)
            {
                sb.AppendLine("Warnings:");
                foreach (var w in warnings.Take(200)) sb.AppendLine(" - " + w);
                if (warnings.Count > 200) sb.AppendLine($"(+{warnings.Count - 200} more)");
            }
            MessageBox.Show(this, sb.ToString(), "Validate Quests", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private (List<string> errors, List<string> warnings) ValidateQuests()
        {
            var errors = new List<string>();
            var warnings = new List<string>();
            try
            {
                var idSet = new HashSet<int>();
                foreach (var q in quests)
                {
                    if (!idSet.Add(q.ID)) errors.Add($"Duplicate quest ID: {q.ID}");
                    if (string.IsNullOrWhiteSpace(q.TitleKey)) warnings.Add($"Quest {q.ID} missing TitleKey");
                    if (q.SubQuests != null)
                    {
                        foreach (var sub in q.SubQuests)
                        {
                            if (sub == q.ID) errors.Add($"Quest {q.ID} has a self-referential subquest");
                            if (!quests.Any(qq => qq.ID == sub)) warnings.Add($"Quest {q.ID} subquest {sub} not found");
                        }
                    }
                    if (q.Location != null && (q.Location.LocationID < -1 || q.Location.LocationID > 500))
                        warnings.Add($"Quest {q.ID} location out of bounds: {q.Location.LocationID}");
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            return (errors, warnings);
        }

        private void SaveWithDiff()
        {
            try
            {
                var basePath = FileManager.ExecPath;
                var questPath = Path.Combine(basePath, _questDataFileName);
                var oldXml = File.Exists(questPath) ? File.ReadAllText(questPath) : string.Empty;

                // Serialize current quests to XML string (matching FileManager.SaveQuestDefinitionsXML)
                var defs = new SatelliteReignModdingTools.Models.QuestDefinitions { Quests = quests };
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SatelliteReignModdingTools.Models.QuestDefinitions));
                var ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);
                string newXml;
                using (var sw = new StringWriter())
                {
                    serializer.Serialize(sw, defs, ns);
                    newXml = sw.ToString();
                }

                var diff = Services.XmlDiffUtil.Diff(oldXml, newXml);
                var dlg = DiffViewerForm.ShowDiff(this, "Confirm Save", _questDataFileName + "\n" + diff);
                if (dlg != DialogResult.OK) return;

                FileManager.SaveQuestDefinitionsXML(quests, _questDataFileName, basePath + @"\");
                MessageBox.Show(this, "Saved.", "Save (with diff)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save (with diff) failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllData()
        {
            LoadQuestData();
            LoadTranslationData();
            LoadItemData();
        }

        private void LoadQuestData()
        {
            try
            {
                string questFilePath = System.IO.Path.Combine(FileManager.ExecPath, _questDataFileName);
                quests = FileManager.LoadQuestDefinitionsXML(_questDataFileName, FileManager.ExecPath) ?? new List<Quest>();
                
                // Debug check for Quest ID 8 rewards
                var quest8 = quests.FirstOrDefault(q => q.ID == 8);
                if (quest8 != null)
                {
                    System.Diagnostics.Debug.WriteLine($"\n=== QUEST ID 8 DEBUG ===");
                    System.Diagnostics.Debug.WriteLine($"Quest ID 8 - Initial Rewards: {quest8.Rewards?.Count ?? 0}, Descriptions: {quest8.Descriptions?.Count ?? 0}");
                    if (quest8.Descriptions != null)
                    {
                        foreach (var desc in quest8.Descriptions)
                        {
                            System.Diagnostics.Debug.WriteLine($"Quest 8 Description: '{desc.LocTitle}' - '{desc.Translation}'");
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"Available Items Count: {availableItems?.Count ?? 0}");
                    if (availableItems?.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"First 10 items:");
                        var sampleItems = availableItems.Take(10).ToList();
                        foreach (var item in sampleItems)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - '{item.m_FriendlyName}' (ID: {item.m_ID})");
                        }
                        
                        // Look for items that might match "boom" or similar
                        var potentialMatches = availableItems.Where(i => 
                            i.m_FriendlyName.ToLower().Contains("boom") ||
                            i.m_FriendlyName.ToLower().Contains("stick") ||
                            i.m_FriendlyName.ToLower().Contains("prototype") ||
                            i.m_FriendlyName.ToLower().Contains("weapon")).Take(5).ToList();
                        
                        if (potentialMatches.Any())
                        {
                            System.Diagnostics.Debug.WriteLine($"Potential matches for common terms:");
                            foreach (var item in potentialMatches)
                            {
                                System.Diagnostics.Debug.WriteLine($"  * '{item.m_FriendlyName}' (ID: {item.m_ID})");
                            }
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"=== END QUEST ID 8 DEBUG ===\n");
                }
                
                // Convert description-based rewards to actual rewards for better editing
                MigrateRewardDescriptionsToRewards();
                
                // Debug check Quest ID 8 after migration
                quest8 = quests.FirstOrDefault(q => q.ID == 8);
                if (quest8 != null)
                {
                    System.Diagnostics.Debug.WriteLine($"\n=== QUEST ID 8 AFTER MIGRATION ===");
                    System.Diagnostics.Debug.WriteLine($"Quest ID 8 - Final Rewards: {quest8.Rewards?.Count ?? 0}");
                    if (quest8.Rewards != null)
                    {
                        for (int i = 0; i < quest8.Rewards.Count; i++)
                        {
                            var reward = quest8.Rewards[i];
                            System.Diagnostics.Debug.WriteLine($"Reward {i}: Type={reward.Type}, ItemId={reward.ItemId}, ItemName='{reward.ItemName}', PrototypeInfo='{reward.PrototypeInfo}', DisplayText='{reward.DisplayText}'");
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"=== END QUEST ID 8 AFTER MIGRATION ===\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading quest data: {ex.Message}\n\nMake sure you've exported quest data from the game using the LoadCustomData mod (press F4 or Delete).", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                quests = new List<Quest>();
            }
        }
        
        private void MigrateRewardDescriptionsToRewards()
        {
            // Instead of parsing descriptions, look for actual QAGiveItem actions in the quest data
            // This requires accessing the exported quest data in the correct format
            try
            {
                var questDataPath = System.IO.Path.Combine(FileManager.ExecPath, "questDefinitions.xml");
                if (System.IO.File.Exists(questDataPath))
                {
                    ExtractRewardsFromQuestActions();
                }
                else
                {
                    // Fallback to description parsing if quest data not available
                    System.Diagnostics.Debug.WriteLine("Quest actions data not available, falling back to description parsing");
                    MigrateRewardDescriptionsToRewardsFallback();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in reward migration: {ex.Message}");
                MigrateRewardDescriptionsToRewardsFallback();
            }
        }
        
        private void ExtractRewardsFromQuestActions()
        {
            // This would need to parse the actual quest actions XML data
            // For now, implement the fallback since we don't have the full quest actions structure
            System.Diagnostics.Debug.WriteLine("ExtractRewardsFromQuestActions: Not yet implemented, using fallback");
            MigrateRewardDescriptionsToRewardsFallback();
        }
        
        private void MigrateRewardDescriptionsToRewardsFallback()
        {
            foreach (var quest in quests)
            {
                if (quest.Descriptions != null && quest.Descriptions.Count > 0)
                {
                    // Look for formal reward descriptions first (LocTitle = "Rewards")
                    var formalRewardDescs = quest.Descriptions.Where(d => 
                        d.LocTitle.ToLower().Contains("reward")).ToList();
                    
                    // Look for informal reward mentions in descriptions
                    var informalRewardDescs = quest.Descriptions.Where(d => 
                        !d.LocTitle.ToLower().Contains("reward") && (
                        d.Translation.ToLower().Contains("prototype weapon") ||
                        d.Translation.ToLower().Contains("prototype") ||
                        d.Translation.ToLower().Contains("blueprint") ||
                        d.Translation.ToLower().Contains("credits") ||
                        d.Translation.ToLower().Contains("district pass"))).ToList();
                    
                    // Process formal rewards with structured parsing
                    foreach (var rewardDesc in formalRewardDescs)
                    {
                        var rewards = ParseStructuredRewards(rewardDesc.Translation);
                        foreach (var reward in rewards)
                        {
                            reward.Description = rewardDesc.Translation;
                            if (quest.Rewards == null)
                                quest.Rewards = new List<QuestReward>();
                            quest.Rewards.Add(reward);
                            System.Diagnostics.Debug.WriteLine($"\u2713 Parsed structured reward in Quest {quest.ID}: {reward.DisplayText}");
                        }
                    }
                    
                    // Process informal reward mentions
                    foreach (var rewardDesc in informalRewardDescs)
                    {
                        var rewards = ParseInformalRewards(rewardDesc.Translation, quest.ID);
                        foreach (var reward in rewards)
                        {
                            reward.Description = rewardDesc.Translation;
                            if (quest.Rewards == null)
                                quest.Rewards = new List<QuestReward>();
                            quest.Rewards.Add(reward);
                            System.Diagnostics.Debug.WriteLine($"\u2713 Parsed informal reward in Quest {quest.ID}: {reward.DisplayText}");
                        }
                    }
                }
            }
        }
        
        private QuestReward CreateRewardFromDescription(string description)
        {
            var reward = new QuestReward();
            var lowerDesc = description.ToLower();
            
            System.Diagnostics.Debug.WriteLine($"\n=== Creating reward from description: '{description}' ===");
            
            // Try to match with actual items first
            var matchedItem = FindMatchingItem(description);
            if (matchedItem != null)
            {
                reward.Type = lowerDesc.Contains("prototype") ? QuestRewardType.Prototype : QuestRewardType.Item;
                reward.ItemId = matchedItem.m_ID;
                reward.ItemName = matchedItem.m_FriendlyName;
                if (lowerDesc.Contains("prototype"))
                {
                    reward.PrototypeInfo = matchedItem.m_FriendlyName;
                }
                System.Diagnostics.Debug.WriteLine($"✓ SUCCESS: Matched item - ID={reward.ItemId}, Name='{reward.ItemName}', Type={reward.Type}");
            }
            // Detect reward type and extract specific information
            else if (lowerDesc.Contains("district pass"))
            {
                reward.Type = QuestRewardType.DistrictPass;
                reward.DistrictName = ExtractDistrictNameFromDescription(description);
                reward.ItemName = $"District Pass ({reward.DistrictName})";
                System.Diagnostics.Debug.WriteLine($"✓ District Pass: {reward.ItemName}");
            }
            else if (lowerDesc.Contains("credit"))
            {
                reward.Type = QuestRewardType.Money;
                reward.ItemName = "Credits";
                reward.Quantity = ExtractQuantityFromDescription(description);
                System.Diagnostics.Debug.WriteLine($"✓ Credits: {reward.Quantity}");
            }
            else if (lowerDesc.Contains("prototype"))
            {
                reward.Type = QuestRewardType.Prototype;
                reward.PrototypeInfo = ExtractBetterPrototypeInfo(description);
                reward.ItemName = $"Prototype: {reward.PrototypeInfo}";
                System.Diagnostics.Debug.WriteLine($"✓ Generic prototype: {reward.PrototypeInfo}");
            }
            else
            {
                reward.Type = QuestRewardType.Item;
                reward.ItemName = ExtractBetterItemName(description);
                System.Diagnostics.Debug.WriteLine($"✓ Generic item: {reward.ItemName}");
            }
            
            if (reward.Quantity <= 0)
                reward.Quantity = ExtractQuantityFromDescription(description);
                
            System.Diagnostics.Debug.WriteLine($"=== Final reward: Type={reward.Type}, ItemId={reward.ItemId}, Name='{reward.ItemName}', Quantity={reward.Quantity} ===\n");
            return reward;
        }
        
        private string ExtractBetterItemName(string description)
        {
            // Extract a better item name from description
            var cleanDesc = description.Replace("Reward:", "").Replace("reward:", "").Trim();
            
            // Look for quoted or bracketed item names
            var quotedMatch = System.Text.RegularExpressions.Regex.Match(cleanDesc, @"['""']([^'""]+)['""']|\[([^\]]+)\]|\(([^)]+)\)");
            if (quotedMatch.Success)
            {
                var extractedName = quotedMatch.Groups[1].Value ?? quotedMatch.Groups[2].Value ?? quotedMatch.Groups[3].Value;
                if (!string.IsNullOrWhiteSpace(extractedName) && extractedName.Length > 2)
                {
                    return extractedName.Trim();
                }
            }
            
            // Look for capitalized words (likely item names)
            var words = cleanDesc.Split(new char[] { ' ', ',', '.', '!', '?', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var capitalizedWords = words.Where(w => w.Length > 2 && char.IsUpper(w[0]) && 
                !IsCommonWord(w) && !w.ToLower().Contains("reward")).ToList();
            
            if (capitalizedWords.Count > 0)
            {
                return string.Join(" ", capitalizedWords.Take(3)); // Take up to 3 words
            }
            
            // Fallback to first few meaningful words
            var meaningfulWords = words.Where(w => w.Length > 3 && !IsCommonWord(w)).Take(2);
            if (meaningfulWords.Any())
            {
                return string.Join(" ", meaningfulWords);
            }
            
            return "Unknown Item";
        }
        
        private string ExtractBetterPrototypeInfo(string description)
        {
            // Better prototype extraction
            var lowerDesc = description.ToLower();
            var cleanDesc = description.Replace("Reward:", "").Replace("reward:", "").Trim();
            
            // Look for "prototype" followed by item name
            var prototypeMatch = System.Text.RegularExpressions.Regex.Match(cleanDesc, @"prototype\s+([a-zA-Z\s]+?)(?:\.|,|!|\?|$)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (prototypeMatch.Success)
            {
                var prototypeName = prototypeMatch.Groups[1].Value.Trim();
                if (prototypeName.Length > 2)
                {
                    return prototypeName;
                }
            }
            
            // Look for specific prototype types
            if (lowerDesc.Contains("weapon") || lowerDesc.Contains("gun") || lowerDesc.Contains("rifle") || lowerDesc.Contains("pistol"))
                return "Weapon Prototype";
            if (lowerDesc.Contains("armor") || lowerDesc.Contains("suit") || lowerDesc.Contains("vest"))
                return "Armor Prototype";
            if (lowerDesc.Contains("augment") || lowerDesc.Contains("implant") || lowerDesc.Contains("enhancement"))
                return "Augment Prototype";
            if (lowerDesc.Contains("tool") || lowerDesc.Contains("device") || lowerDesc.Contains("gadget"))
                return "Tool Prototype";
            
            // Extract any capitalized words after "prototype"
            var words = cleanDesc.Split(' ');
            for (int i = 0; i < words.Length - 1; i++)
            {
                if (words[i].ToLower() == "prototype" && i + 1 < words.Length)
                {
                    var followingWords = words.Skip(i + 1).Take(3).Where(w => w.Length > 2).ToArray();
                    if (followingWords.Length > 0)
                    {
                        return string.Join(" ", followingWords).Trim(',', '.', '!', '?');
                    }
                }
            }
            
            return "Unknown Prototype";
        }
        
        private SRMod.DTOs.SerializableItemData FindMatchingItem(string description)
        {
            if (availableItems == null || availableItems.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"No available items to match against (Count: {availableItems?.Count ?? 0})");
                return null;
            }
                
            var lowerDesc = description.ToLower();
            System.Diagnostics.Debug.WriteLine($"\n--- Attempting to match item from: '{description}'");
            System.Diagnostics.Debug.WriteLine($"Available items count: {availableItems.Count}");
            
            // Clean the description for better matching
            var cleanDesc = lowerDesc.Replace("prototype", "").Replace("reward", "").Replace(":", "").Trim();
            var words = cleanDesc.Split(new char[] { ' ', ',', '.', '!', '?', ':', ';', '(', ')', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Try exact name matches first (case insensitive)
            foreach (var item in availableItems)
            {
                var itemNameLower = item.m_FriendlyName.ToLower();
                if (lowerDesc.Contains(itemNameLower) && itemNameLower.Length > 3)
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Found exact match: '{item.m_FriendlyName}' (ID: {item.m_ID})");
                    return item;
                }
            }
            
            // Special case handling for common item name variations
            var specialCases = new Dictionary<string, string[]>
            {
                { "boom stick", new[] { "boomstick", "boom", "stick" } },
                { "shock baton", new[] { "shock", "baton", "stun" } },
                { "riot gun", new[] { "riot", "shotgun" } },
                { "sniper rifle", new[] { "sniper", "rifle" } },
                { "assault rifle", new[] { "assault", "rifle" } },
                { "body armor", new[] { "body", "armor", "vest" } },
                { "kevlar", new[] { "kevlar", "armor" } }
            };
            
            // Check special cases first
            foreach (var specialCase in specialCases)
            {
                foreach (var variant in specialCase.Value)
                {
                    if (lowerDesc.Contains(variant))
                    {
                        foreach (var item in availableItems)
                        {
                            var itemNameLower = item.m_FriendlyName.ToLower();
                            if (itemNameLower.Contains(specialCase.Key) || 
                                specialCase.Value.Any(v => itemNameLower.Contains(v)))
                            {
                                System.Diagnostics.Debug.WriteLine($"✓ Found special case match: '{item.m_FriendlyName}' (ID: {item.m_ID}) for '{variant}'");
                                return item;
                            }
                        }
                    }
                }
            }
            
            // Try reverse - check if item name contains any significant words from description
            var significantWords = words.Where(w => w.Length > 3 && 
                !IsCommonWord(w)).ToArray();
            
            foreach (var item in availableItems)
            {
                var itemNameLower = item.m_FriendlyName.ToLower();
                var itemWords = itemNameLower.Split(' ');
                
                foreach (var word in significantWords)
                {
                    if (itemWords.Any(iw => iw.Equals(word, StringComparison.OrdinalIgnoreCase)))
                    {
                        System.Diagnostics.Debug.WriteLine($"✓ Found word match: '{item.m_FriendlyName}' (ID: {item.m_ID}) matched word '{word}'");
                        return item;
                    }
                }
            }
            
            // Try partial matches with individual significant words
            foreach (var word in significantWords)
            {
                foreach (var item in availableItems)
                {
                    var itemNameLower = item.m_FriendlyName.ToLower();
                    if (itemNameLower.Contains(word) && word.Length > 2)
                    {
                        System.Diagnostics.Debug.WriteLine($"✓ Found partial match: '{item.m_FriendlyName}' (ID: {item.m_ID}) contains word '{word}'");
                        return item;
                    }
                }
            }
            
            // Debug what we're trying to match
            System.Diagnostics.Debug.WriteLine($"✗ No match found. Significant words: [{string.Join(", ", significantWords)}]");
            System.Diagnostics.Debug.WriteLine($"Sample available items:");
            foreach (var item in availableItems.Take(10))
            {
                System.Diagnostics.Debug.WriteLine($"  - '{item.m_FriendlyName}' (ID: {item.m_ID})");
            }
            
            return null;
        }
        
        private bool IsCommonWord(string word)
        {
            var commonWords = new[] { "the", "and", "for", "you", "are", "has", "have", "this", "that", "with", "from", "they", "been", "will", "what", "when", "where", "item", "gear", "new" };
            return commonWords.Contains(word.ToLower());
        }
        
        private List<QuestReward> ParseStructuredRewards(string rewardText)
        {
            var rewards = new List<QuestReward>();
            
            try
            {
                // Remove HTML color tags and normalize text
                var cleanText = System.Text.RegularExpressions.Regex.Replace(rewardText, @"<[^>]+>", "");
                var lines = cleanText.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
                
                System.Diagnostics.Debug.WriteLine($"Parsing structured rewards from {lines.Length} lines");
                
                QuestRewardType currentType = QuestRewardType.Item;
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    // Detect reward category
                    if (trimmedLine.ToLower().Contains("blueprint"))
                    {
                        currentType = QuestRewardType.Blueprint;
                        continue;
                    }
                    else if (trimmedLine.ToLower().Contains("prototype"))
                    {
                        currentType = QuestRewardType.Prototype;
                        continue;
                    }
                    else if (trimmedLine.ToLower().Contains("credit"))
                    {
                        currentType = QuestRewardType.Money;
                        continue;
                    }
                    
                    // Parse specific reward items (lines starting with " > " or "- >")
                    if (trimmedLine.StartsWith("> ") || trimmedLine.StartsWith("- >"))
                    {
                        var itemText = trimmedLine.Substring(trimmedLine.IndexOf('>') + 1).Trim();
                        
                        // Split by ":" to separate name from status
                        var parts = itemText.Split(new[] { ':' }, 2);
                        var itemName = parts[0].Trim();
                        var status = parts.Length > 1 ? parts[1].Trim() : "";
                        
                        var reward = new QuestReward
                        {
                            Type = currentType,
                            ItemName = itemName,
                            Quantity = 1,
                            DropChance = status.ToUpper().Contains("ACQUIRED") ? 100 : 100,
                            IsGuaranteed = true
                        };
                        
                        // Try to match with actual item database
                        var matchedItem = FindMatchingItem(itemName);
                        if (matchedItem != null)
                        {
                            reward.ItemId = matchedItem.m_ID;
                            reward.ItemName = matchedItem.m_FriendlyName;
                            System.Diagnostics.Debug.WriteLine($"\u2713 Matched structured reward '{itemName}' to '{matchedItem.m_FriendlyName}' (ID: {matchedItem.m_ID})");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"\u2717 No match found for structured reward '{itemName}'");
                        }
                        
                        if (currentType == QuestRewardType.Prototype)
                        {
                            reward.PrototypeInfo = itemName;
                        }
                        
                        rewards.Add(reward);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing structured rewards: {ex.Message}");
            }
            
            return rewards;
        }
        
        private List<QuestReward> ParseInformalRewards(string descriptionText, int questId)
        {
            var rewards = new List<QuestReward>();
            
            try
            {
                var lowerText = descriptionText.ToLower();
                
                // Special handling for Quest ID 8 (Rescue Infiltrator)
                if (questId == 8 && lowerText.Contains("prototype weapon"))
                {
                    var reward = new QuestReward
                    {
                        Type = QuestRewardType.Prototype,
                        ItemName = "Prototype Weapon",
                        PrototypeInfo = "Weapon obtained from rescue mission",
                        Quantity = 1,
                        DropChance = 100,
                        IsGuaranteed = true
                    };
                    
                    // Try to identify the specific weapon type
                    // Based on the game, this is likely a "Boom Stick" or similar weapon
                    var potentialWeapons = new[] { "boom stick", "boomstick", "shotgun", "riot gun" };
                    foreach (var weaponName in potentialWeapons)
                    {
                        var matchedWeapon = availableItems?.FirstOrDefault(i => 
                            i.m_FriendlyName.ToLower().Contains(weaponName) || 
                            i.m_FriendlyName.ToLower().Replace(" ", "").Contains(weaponName.Replace(" ", "")));
                        
                        if (matchedWeapon != null)
                        {
                            reward.ItemId = matchedWeapon.m_ID;
                            reward.ItemName = matchedWeapon.m_FriendlyName;
                            reward.PrototypeInfo = matchedWeapon.m_FriendlyName;
                            System.Diagnostics.Debug.WriteLine($"\u2713 Quest {questId}: Matched prototype weapon to '{matchedWeapon.m_FriendlyName}' (ID: {matchedWeapon.m_ID})");
                            break;
                        }
                    }
                    
                    rewards.Add(reward);
                }
                
                // Look for other prototype mentions
                else if (lowerText.Contains("prototype") && !lowerText.Contains("weapon"))
                {
                    var reward = new QuestReward
                    {
                        Type = QuestRewardType.Prototype,
                        ItemName = "Prototype Item",
                        PrototypeInfo = ExtractBetterPrototypeInfo(descriptionText),
                        Quantity = 1,
                        DropChance = 100,
                        IsGuaranteed = true
                    };
                    rewards.Add(reward);
                }
                
                // Look for credit amounts
                var creditMatch = System.Text.RegularExpressions.Regex.Match(descriptionText, @"(\d+)\s*credit", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (creditMatch.Success)
                {
                    var creditAmount = int.Parse(creditMatch.Groups[1].Value);
                    var reward = new QuestReward
                    {
                        Type = QuestRewardType.Money,
                        ItemName = "Credits",
                        Quantity = creditAmount,
                        DropChance = 100,
                        IsGuaranteed = true
                    };
                    rewards.Add(reward);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing informal rewards: {ex.Message}");
            }
            
            return rewards;
        }
        
        private string ExtractPrototypeInfoFromDescription(string description)
        {
            // Extract prototype information from description
            var lowerDesc = description.ToLower();
            var words = description.Split(' ');
            
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].ToLower() == "prototype" && i + 1 < words.Length)
                {
                    // Get the next few words as prototype info
                    var prototypeWords = words.Skip(i + 1).Take(3).ToArray();
                    return string.Join(" ", prototypeWords).Trim(',', '.', '!', '?');
                }
            }
            
            // Fallback to extracting from common patterns
            if (lowerDesc.Contains("weapon prototype"))
                return "Weapon";
            if (lowerDesc.Contains("armor prototype"))
                return "Armor";
            if (lowerDesc.Contains("augment prototype"))
                return "Augment";
            
            return "Unknown Prototype";
        }
        
        private string ExtractDistrictNameFromDescription(string description)
        {
            var lowerDesc = description.ToLower();
            
            if (lowerDesc.Contains("red light"))
                return "Red Light District";
            if (lowerDesc.Contains("industrial"))
                return "Industrial District";
            if (lowerDesc.Contains("grid"))
                return "Grid District";
            if (lowerDesc.Contains("cbd"))
                return "CBD District";
            if (lowerDesc.Contains("downtown"))
                return "Downtown District";
                
            return "Unknown District";
        }
        
        private int ExtractQuantityFromDescription(string description)
        {
            // Try to extract quantity from description
            var words = description.Split(' ');
            foreach (var word in words)
            {
                if (int.TryParse(word, out int quantity))
                    return quantity;
            }
            return 1;
        }

        private void LoadTranslationData()
        {
            try
            {
                string translationFilePath = System.IO.Path.Combine(FileManager.ExecPath, _translationDataFileName);
                translations = FileManager.LoadTranslationDefinitionsXML(_translationDataFileName, FileManager.ExecPath) ?? new List<Translation>();
            }
            catch (Exception ex)
            {
                // Translations are optional - don't show error, just use empty list
                System.Diagnostics.Debug.WriteLine($"No translations file found: {ex.Message}");
                translations = new List<Translation>();
            }
        }
        
        private void LoadItemData()
        {
            try
            {
                string itemFilePath = System.IO.Path.Combine(FileManager.ExecPath, "itemDefinitions.xml");
                System.Diagnostics.Debug.WriteLine($"Loading item data from: {itemFilePath}");
                
                if (System.IO.File.Exists(itemFilePath))
                {
                    availableItems = FileManager.LoadItemDataXML("itemDefinitions.xml", FileManager.ExecPath) ?? new List<SRMod.DTOs.SerializableItemData>();
                    System.Diagnostics.Debug.WriteLine($"Loaded {availableItems.Count} items from item definitions");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Item definitions file not found at: {itemFilePath}");
                    availableItems = new List<SRMod.DTOs.SerializableItemData>();
                }
            }
            catch (Exception ex)
            {
                // Items are optional for rewards - don't show error, just use empty list
                System.Diagnostics.Debug.WriteLine($"Error loading item definitions: {ex.Message}");
                availableItems = new List<SRMod.DTOs.SerializableItemData>();
            }
        }

        private void PopulateDistrictDropdown()
        {
            cmbDistrict.Items.Clear();
            foreach (QuestDistrict district in Enum.GetValues(typeof(QuestDistrict)))
            {
                cmbDistrict.Items.Add(new { 
                    Text = district.ToString(), 
                    Value = district 
                });
            }
            cmbDistrict.DisplayMember = "Text";
            cmbDistrict.ValueMember = "Value";
        }

        private void SetupEditorControls()
        {
            // Populate translation key dropdown
            PopulateTitleKeyDropdown();
            
            // Set up numeric control for wake locations
            numWakeOnLocation.Minimum = -1;
            numWakeOnLocation.Maximum = 500;
            numWakeOnLocation.Value = -1;
            
            // Populate available quests dropdown
            PopulateAvailableQuestsDropdown();
            
            // Set up reward controls
            SetupRewardControls();
        }

        private void PopulateTitleKeyDropdown()
        {
            cmbTitleKey.Items.Clear();
            var allTitleKeys = new HashSet<string>();
            
            // Add existing translation keys from translations
            var translationKeys = GetTranslationKeys();
            foreach (string key in translationKeys)
            {
                allTitleKeys.Add(key);
            }
            
            // Add title keys from all loaded quests
            if (quests?.Count > 0)
            {
                foreach (var quest in quests)
                {
                    if (!string.IsNullOrEmpty(quest.TitleKey))
                    {
                        allTitleKeys.Add(quest.TitleKey);
                    }
                }
            }
            
            // Add all unique keys to dropdown
            var sortedKeys = allTitleKeys.OrderBy(k => k).ToList();
            foreach (string key in sortedKeys)
            {
                cmbTitleKey.Items.Add(key);
            }
            
            // Always add some common prefixes for new keys
            cmbTitleKey.Items.Add("CUSTOM_QUEST_TITLE_001");
            cmbTitleKey.Items.Add("CUSTOM_QUEST_TITLE_002"); 
            cmbTitleKey.Items.Add("Q_GEN_CUSTOM_TITLE_01");
            cmbTitleKey.Items.Add("Q_RL_CUSTOM_TITLE_01");
            cmbTitleKey.Items.Add("Q_IND_CUSTOM_TITLE_01");
            cmbTitleKey.Items.Add("Q_GRID_CUSTOM_TITLE_01");
            cmbTitleKey.Items.Add("Q_CBD_CUSTOM_TITLE_01");
            
            System.Diagnostics.Debug.WriteLine($"Populated title key dropdown with {cmbTitleKey.Items.Count} items including quest keys");
        }

        private void PopulateAvailableQuestsDropdown()
        {
            cmbAvailableQuests.Items.Clear();
            
            if (quests?.Count > 0)
            {
                var questItems = quests.Select(quest => new
                {
                    Text = quest.DisplayName,
                    Value = quest.ID
                }).OrderBy(x => x.Text).ToList();

                foreach (var item in questItems)
                {
                    cmbAvailableQuests.Items.Add(item);
                }
                
                cmbAvailableQuests.DisplayMember = "Text";
                cmbAvailableQuests.ValueMember = "Value";
            }
        }
        
        private void PopulateAvailableQuestsDropdownForEditing()
        {
            cmbAvailableQuests.Items.Clear();
            System.Diagnostics.Debug.WriteLine($"PopulateAvailableQuestsDropdownForEditing called - quests: {quests?.Count}, activeQuest: {activeQuest?.ID}");
            
            if (quests?.Count > 0 && activeQuest != null)
            {
                // Filter out current quest and existing sub-quests
                var excludedIds = new HashSet<int> { activeQuest.ID };
                if (activeQuest.SubQuests?.Count > 0)
                {
                    foreach (int subQuestId in activeQuest.SubQuests)
                        excludedIds.Add(subQuestId);
                }
                
                var questItems = quests
                    .Where(quest => !excludedIds.Contains(quest.ID))
                    .Select(quest => new
                    {
                        Text = quest.DisplayName,
                        Value = quest.ID
                    }).OrderBy(x => x.Text).ToList();

                foreach (var item in questItems)
                {
                    cmbAvailableQuests.Items.Add(item);
                }
                
                cmbAvailableQuests.DisplayMember = "Text";
                cmbAvailableQuests.ValueMember = "Value";
                
                // Reset selection to no item selected
                cmbAvailableQuests.SelectedIndex = -1;
                
                System.Diagnostics.Debug.WriteLine($"Populated available quests dropdown with {cmbAvailableQuests.Items.Count} items");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Cannot populate available quests - no quests loaded or no active quest");
            }
        }
        
        private void SetupRewardControls()
        {
            // Set up reward type dropdown
            PopulateRewardTypeDropdown();
            
            // Set up item dropdown 
            PopulateRewardItemDropdown();
            
            // Set up numeric controls
            numRewardQuantity.Minimum = 1;
            numRewardQuantity.Maximum = 999;
            numRewardQuantity.Value = 1;
            
            numDropChance.Minimum = 0;
            numDropChance.Maximum = 100;
            numDropChance.DecimalPlaces = 1;
            numDropChance.Value = 100;
        }
        
        private void PopulateRewardTypeDropdown()
        {
            cmbRewardType.Items.Clear();
            foreach (QuestRewardType rewardType in Enum.GetValues(typeof(QuestRewardType)))
            {
                cmbRewardType.Items.Add(new { 
                    Text = rewardType.ToString(), 
                    Value = rewardType 
                });
            }
            cmbRewardType.DisplayMember = "Text";
            cmbRewardType.ValueMember = "Value";
            
            // Select Item by default
            cmbRewardType.SelectedIndex = 0;
        }
        
        private void PopulateRewardItemDropdown()
        {
            cmbRewardItem.Items.Clear();
            
            if (availableItems?.Count > 0)
            {
                var itemList = availableItems.Select(item => new
                {
                    Text = $"{item.m_FriendlyName} (ID: {item.m_ID})",
                    Value = item.m_ID,
                    ItemName = item.m_FriendlyName
                }).OrderBy(x => x.Text).ToList();

                foreach (var item in itemList)
                {
                    cmbRewardItem.Items.Add(item);
                }
                
                cmbRewardItem.DisplayMember = "Text";
                cmbRewardItem.ValueMember = "Value";
            }
            
            // Add default options if no items loaded
            if (cmbRewardItem.Items.Count == 0)
            {
                cmbRewardItem.Items.Add(new { Text = "Credits (ID: 0)", Value = 0, ItemName = "Credits" });
                cmbRewardItem.Items.Add(new { Text = "Experience (ID: -1)", Value = -1, ItemName = "Experience" });
                cmbRewardItem.DisplayMember = "Text";
                cmbRewardItem.ValueMember = "Value";
            }
        }
        
        private void InitializeRewardControlsForEditing()
        {
            // Refresh reward dropdowns to ensure they have current data
            PopulateRewardTypeDropdown();
            PopulateRewardItemDropdown();
            
            // Reset reward form controls to default state when entering edit mode
            if (cmbRewardType.Items.Count > 0)
                cmbRewardType.SelectedIndex = 0; // Select "Item" by default
            
            cmbRewardItem.SelectedIndex = -1; // No item selected by default
            numRewardQuantity.Value = 1;
            numDropChance.Value = 100;
            chkGuaranteed.Checked = true;
            
            System.Diagnostics.Debug.WriteLine($"Initialized reward controls - RewardType: {cmbRewardType.Items.Count} items, RewardItem: {cmbRewardItem.Items.Count} items");
        }

        private void EnableEditMode(bool enabled)
        {
            isEditing = enabled;
            
            // Toggle visibility of edit controls vs read-only controls
            cmbDistrict.Visible = enabled;
            txtDistrict.Visible = !enabled;
            
            cmbTitleKey.Visible = enabled;
            txtTitleKey.Visible = !enabled;
            
            numWakeOnLocation.Visible = enabled;
            txtWakeOnLocation.Visible = !enabled;
            
            numLocationID.Visible = enabled;
            txtLocation.Visible = !enabled;
            
            // Toggle edit button visibility
            btnEditQuest.Visible = !enabled;
            btnSaveQuest.Visible = enabled;
            btnCancelEdit.Visible = enabled;
            
            // Enable/disable text fields
            txtQuestTitle.ReadOnly = !enabled;
            
            // Enable/disable checkboxes
            chkHidden.Enabled = enabled;
            chkShowDebrief.Enabled = enabled;
            
            // Toggle description editing buttons
            btnAddDescription.Visible = enabled;
            btnEditDescription.Visible = enabled;
            btnEditTitle.Visible = enabled;
            btnDeleteDescription.Visible = enabled;
            
            // Toggle sub-quest editing controls
            lstSubQuests.Visible = enabled;
            txtSubQuests.Visible = !enabled;
            cmbAvailableQuests.Visible = enabled;
            btnAddSubQuest.Visible = enabled;
            btnRemoveSubQuest.Visible = enabled;
            
            // Toggle reward editing controls
            lstRewards.Visible = enabled;
            cmbRewardType.Visible = enabled;
            cmbRewardItem.Visible = enabled;
            numRewardQuantity.Visible = enabled;
            numDropChance.Visible = enabled;
            chkGuaranteed.Visible = enabled;
            btnAddReward.Visible = enabled;
            btnEditReward.Visible = enabled;
            btnDeleteReward.Visible = enabled;
            btnBrowseItems.Visible = enabled;
            lblRewards.Visible = enabled;
            lblRewardType.Visible = enabled;
            lblRewardItem.Visible = enabled;
            lblQuantity.Visible = enabled;
            lblDropChance.Visible = enabled;
            
            if (enabled)
            {
                // Reset reward selection state when enabling edit mode
                selectedReward = null;
                selectedRewardIndex = -1;
                btnEditReward.Text = "Update";
                btnEditReward.Enabled = false;
                
                // Set default values for reward controls
                if (cmbRewardType.Items.Count > 0)
                    cmbRewardType.SelectedIndex = 0;
                if (cmbRewardItem.Items.Count > 0)
                    cmbRewardItem.SelectedIndex = -1;
                numRewardQuantity.Value = 1;
                numDropChance.Value = 100;
                chkGuaranteed.Checked = true;
                
                System.Diagnostics.Debug.WriteLine("Enabled reward editing controls with defaults");
            }
            else
            {
                selectedReward = null;
                selectedRewardIndex = -1;
                btnEditReward.Text = "Edit";
                btnEditReward.Enabled = false;
            }
            
            // Update form title to indicate edit mode
            if (enabled)
            {
                this.Text = "Quest Editor - EDITING MODE - Satellite Reign Modding Tools";
            }
            else
            {
                this.Text = "Quest Editor - Satellite Reign Modding Tools";
            }
            
            // If entering edit mode, make sure dropdowns are properly set
            if (enabled && activeQuest != null)
            {
                // Trigger update to set dropdown selections correctly
                UpdateQuestDetails();
            }
        }

        private void UpdateQuestList()
        {
            try
            {
                var questItems = filteredQuests.Select(quest => new
                {
                    Key = quest.DisplayName,
                    Value = quest
                }).ToList();

                QuestListBox.DataSource = questItems;
                QuestListBox.DisplayMember = "Key";
                QuestListBox.ValueMember = "Value";

                lblQuestCount.Text = $"Showing {filteredQuests.Count} of {quests.Count} quests";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quest list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateQuestDetails()
        {
            if (activeQuest == null)
            {
                ClearQuestDetails();
                return;
            }

            try
            {
                // Basic quest info
                txtQuestID.Text = activeQuest.ID.ToString();
                txtQuestTitle.Text = activeQuest.Title ?? "";
                txtTitleKey.Text = activeQuest.TitleKey ?? "";
                txtDistrict.Text = activeQuest.DistrictText;
                txtStatus.Text = activeQuest.StatusText;
                txtLocation.Text = activeQuest.LocationText;
                
                chkHidden.Checked = activeQuest.Hidden;
                chkShowDebrief.Checked = activeQuest.ShowDebrief;

                // Dropdown controls are now initialized by InitializeDropdownsForEditing() after edit mode is enabled

                // Quest description
                txtDescription.Text = activeQuest.DescriptionText;

                // Sub-quests
                txtSubQuests.Text = activeQuest.SubQuestText;
                if (activeQuest.SubQuests?.Count > 0)
                {
                    var subQuestDetails = string.Join(", ", activeQuest.SubQuests.Select(id => 
                    {
                        var subQuest = quests.FirstOrDefault(q => q.ID == id);
                        return subQuest != null ? $"{id}: {subQuest.Title}" : id.ToString();
                    }));
                    txtSubQuests.Text += $"\nDetails: {subQuestDetails}";
                }
                
                // Update sub-quest list for edit mode
                if (isEditing)
                {
                    UpdateSubQuestsList();
                    UpdateRewardsList();
                }

                // Wake on location info
                if (activeQuest.WakeOnLocation > 0)
                {
                    txtWakeOnLocation.Text = $"Location ID: {activeQuest.WakeOnLocation}";
                }
                else
                {
                    txtWakeOnLocation.Text = "No wake location";
                }

                // Update descriptions list
                lstDescriptions.Items.Clear();
                if (activeQuest.Descriptions?.Count > 0)
                {
                    foreach (var desc in activeQuest.Descriptions)
                    {
                        lstDescriptions.Items.Add($"{desc.LocTitle}: {desc.Translation.Substring(0, Math.Min(100, desc.Translation.Length))}...");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quest details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearQuestDetails()
        {
            txtQuestID.Text = "";
            txtQuestTitle.Text = "";
            txtTitleKey.Text = "";
            txtDistrict.Text = "";
            txtStatus.Text = "";
            txtLocation.Text = "";
            txtDescription.Text = "";
            txtSubQuests.Text = "";
            txtWakeOnLocation.Text = "";
            chkHidden.Checked = false;
            chkShowDebrief.Checked = false;
            lstDescriptions.Items.Clear();
        }

        private void QuestListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (QuestListBox.SelectedValue is Quest selectedQuest)
                {
                    activeQuest = selectedQuest;
                    UpdateQuestDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting quest: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to discard them and reload?", 
                    "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }

            LoadAllData();
            UpdateQuestList();
            hasUnsavedChanges = false;
            EnableEditMode(false);
            
            string message = $"Reloaded {quests.Count} quests";
            if (translations.Count > 0)
                message += $" and {translations.Count} translations";
            
            MessageBox.Show(message, "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string filterText = txtFilter.Text.ToLower();
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    UpdateQuestList();
                    return;
                }

                var filteredQuests = quests.Where(q => 
                    q.Title.ToLower().Contains(filterText) ||
                    q.ID.ToString().Contains(filterText) ||
                    q.District.ToLower().Contains(filterText) ||
                    (q.DescriptionText?.ToLower().Contains(filterText) ?? false)
                ).ToList();

                var questItems = filteredQuests.Select(quest => new
                {
                    Key = quest.DisplayName,
                    Value = quest
                }).ToList();

                QuestListBox.DataSource = questItems;
                QuestListBox.DisplayMember = "Key";
                QuestListBox.ValueMember = "Value";

                lblQuestCount.Text = $"Showing {filteredQuests.Count} of {quests.Count} quests";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering quests: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(sender, e);
                e.Handled = true;
            }
        }

        // New editing methods
        private void btnEditQuest_Click(object sender, EventArgs e)
        {
            if (activeQuest == null)
            {
                MessageBox.Show("Please select a quest to edit.", "No Quest Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EnableEditMode(true);
            InitializeDropdownsForEditing();
            MessageBox.Show("Quest editing enabled. You can now modify quest properties using the dropdown controls and text fields.", 
                "Edit Mode Enabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaveQuest_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null)
                return;

            try
            {
                // Save changes to the active quest
                SaveCurrentQuestChanges();

                // Save the entire quest collection to file
                string questFilePath = System.IO.Path.Combine(FileManager.ExecPath, "questDefinitions_modified.xml");
                FileManager.SaveQuestDefinitionsXML(quests, "questDefinitions_modified.xml", FileManager.ExecPath);

                hasUnsavedChanges = false;
                EnableEditMode(false);
                UpdateQuestList();

                MessageBox.Show($"Quest changes saved to questDefinitions_modified.xml", "Save Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving quest: {ex.Message}", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewQuest_Click(object sender, EventArgs e)
        {
            try
            {
                // Create new quest with next available ID
                int newId = quests.Count > 0 ? quests.Max(q => q.ID) + 1 : 1;
                
                var newQuest = new Quest
                {
                    ID = newId,
                    Title = "New Quest",
                    TitleKey = $"CUSTOM_QUEST_TITLE_{newId:000}",
                    District = "NONE",
                    Hidden = false,
                    ShowDebrief = true,
                    SubQuests = new List<int>(),
                    Descriptions = new List<QuestDescription>()
                };

                quests.Add(newQuest);
                activeQuest = newQuest;
                hasUnsavedChanges = true;
                
                UpdateQuestList();
                
                // Select the new quest in the list
                for (int i = 0; i < QuestListBox.Items.Count; i++)
                {
                    var item = QuestListBox.Items[i];
                    if (item.GetType().GetProperty("Value")?.GetValue(item) as Quest == newQuest)
                    {
                        QuestListBox.SelectedIndex = i;
                        break;
                    }
                }

                EnableEditMode(true);
                InitializeDropdownsForEditing();
                MessageBox.Show($"New quest created with ID {newId}. You can now edit its properties.", 
                    "New Quest Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new quest: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCurrentQuestChanges()
        {
            if (activeQuest == null)
                return;

            // This method will be expanded when we add the proper editing controls
            // For now, we'll prepare the framework
            hasUnsavedChanges = true;
        }

        private List<string> GetTranslationKeys()
        {
            return translations.Select(t => t.Key).OrderBy(k => k).ToList();
        }

        private string GetTranslationText(string key)
        {
            var translation = translations.FirstOrDefault(t => t.Key == key);
            return translation?.Element?.PrimaryTranslation ?? "";
        }

        private void OnQuestPropertyChanged()
        {
            hasUnsavedChanges = true;
        }

        private void QuestBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?", 
                    "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    btnSaveQuest_Click(this, EventArgs.Empty);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        // New event handlers for editing controls
        private void cmbDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null && cmbDistrict.SelectedItem != null)
            {
                var selectedDistrict = ((dynamic)cmbDistrict.SelectedItem).Value;
                activeQuest.District = selectedDistrict.ToString();
                OnQuestPropertyChanged();
            }
        }

        private void cmbTitleKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null && cmbTitleKey.SelectedItem != null)
            {
                string selectedKey = cmbTitleKey.SelectedItem.ToString();
                activeQuest.TitleKey = selectedKey;
                
                // Auto-populate title from translation if available
                string translationText = GetTranslationText(selectedKey);
                if (!string.IsNullOrEmpty(translationText))
                {
                    activeQuest.Title = translationText;
                    txtQuestTitle.Text = translationText;
                }
                
                OnQuestPropertyChanged();
            }
        }

        private void numWakeOnLocation_ValueChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null)
            {
                activeQuest.WakeOnLocation = (int)numWakeOnLocation.Value;
                OnQuestPropertyChanged();
                
                // Update display text
                if (activeQuest.WakeOnLocation == -1)
                {
                    txtWakeOnLocation.Text = "No wake location";
                }
                else
                {
                    string locationName = QuestValidation.GetLocationName(activeQuest.WakeOnLocation);
                    txtWakeOnLocation.Text = $"Location ID: {activeQuest.WakeOnLocation} ({locationName})";
                }
            }
        }

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show("Are you sure you want to cancel editing? All changes will be lost.", 
                    "Cancel Editing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }

            // Reload the quest data to revert changes
            if (activeQuest != null)
            {
                var originalQuest = quests.FirstOrDefault(q => q.ID == activeQuest.ID);
                if (originalQuest != null)
                {
                    activeQuest = originalQuest;
                    UpdateQuestDetails();
                }
            }

            hasUnsavedChanges = false;
            EnableEditMode(false);
        }

        private void btnOpenTranslations_Click(object sender, EventArgs e)
        {
            try
            {
                var translationsBrowser = new TranslationsBrowser();
                translationsBrowser.Show();
                
                // Optionally notify user about the connection
                MessageBox.Show("Translations Browser opened. Any changes made there can be referenced in Quest Editor using translation keys.",
                    "Translations Browser", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Translations Browser: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtQuestTitle_TextChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null)
            {
                activeQuest.Title = txtQuestTitle.Text;
                OnQuestPropertyChanged();
                
                // Update quest list display since DisplayName includes the title
                UpdateQuestList();
            }
        }

        private void btnEditTitle_Click(object sender, EventArgs e)
        {
            if (activeQuest == null || !isEditing) return;

            try
            {
                var loadedTranslations = FileManager.LoadTranslationDefinitionsXML("translationDefinitions.xml", FileManager.ExecPath);
                var currentTitleKey = activeQuest.TitleKey;
                
                var dialog = new TranslationEditDialog(currentTitleKey, loadedTranslations);
                dialog.Text = "Edit Quest Title Translation";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Update the quest with the new or existing translation key
                    activeQuest.TitleKey = dialog.TranslationKey;
                    activeQuest.Title = dialog.TranslationText;
                    
                    // If this is a new translation, add it to our translations list
                    if (dialog.IsNewTranslation)
                    {
                        var newTranslation = new Translation
                        {
                            Key = dialog.TranslationKey,
                            Element = new TranslationElement
                            {
                                PrimaryTranslation = dialog.TranslationText
                            }
                        };
                        translations.Add(newTranslation);
                    }
                    else
                    {
                        // Update existing translation in our local list
                        var existing = translations.FirstOrDefault(t => t.Key == dialog.TranslationKey);
                        if (existing != null)
                        {
                            existing.Element.PrimaryTranslation = dialog.TranslationText;
                        }
                    }
                    
                    // Save translations and update UI
                    SaveTranslationsToFile();
                    UpdateQuestDetails();
                    OnQuestPropertyChanged();
                    UpdateQuestList();
                    
                    MessageBox.Show($"Quest title updated successfully with translation key: {dialog.TranslationKey}",
                        "Title Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing quest title translation: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void numLocationID_ValueChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null)
            {
                if (activeQuest.Location == null)
                    activeQuest.Location = new QuestLocation();
                
                activeQuest.Location.LocationID = (int)numLocationID.Value;
                OnQuestPropertyChanged();
                
                // Update display text
                if (activeQuest.Location.LocationID == -1)
                {
                    txtLocation.Text = "No Location";
                }
                else
                {
                    string locationName = QuestValidation.GetLocationName(activeQuest.Location.LocationID);
                    txtLocation.Text = $"Location ID: {activeQuest.Location.LocationID} ({locationName})";
                }
            }
        }

        // Quest Description CRUD methods
        private void btnAddDescription_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null)
                return;

            try
            {
                // Open Translation Edit Dialog for new description
                var translationEditDialog = new TranslationEditDialog(null, translations);
                
                if (translationEditDialog.ShowDialog() == DialogResult.OK)
                {
                    // Create new description with user-defined translation
                    var newDescription = new QuestDescription
                    {
                        LocTitle = translationEditDialog.TranslationKey,
                        Translation = translationEditDialog.TranslationText,
                        IsNew = translationEditDialog.IsNew,
                        HasBeenSeen = translationEditDialog.HasBeenSeen
                    };

                    if (activeQuest.Descriptions == null)
                        activeQuest.Descriptions = new List<QuestDescription>();

                    activeQuest.Descriptions.Add(newDescription);
                    
                    // Add to translations collection if it's a new key
                    if (translationEditDialog.IsNewTranslation)
                    {
                        var newTranslation = new Translation 
                        {
                            Key = translationEditDialog.TranslationKey,
                            Element = new TranslationElement 
                            {
                                Token = translationEditDialog.TranslationKey,
                                Translations = new List<string> { translationEditDialog.TranslationText }
                            }
                        };
                        translations.Add(newTranslation);
                        
                        // Save the updated translations
                        SaveTranslationsToFile();
                    }
                    
                    OnQuestPropertyChanged();
                    UpdateQuestDetails(); // Refresh the descriptions list

                    MessageBox.Show("New description created and added to quest!", 
                        "Description Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding description: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditDescription_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null || lstDescriptions.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a description to edit.", "No Description Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int selectedIndex = lstDescriptions.SelectedIndex;
                if (selectedIndex < activeQuest.Descriptions?.Count)
                {
                    var selectedDescription = activeQuest.Descriptions[selectedIndex];
                    
                    // Open Translation Edit Dialog with existing description
                    var translationEditDialog = new TranslationEditDialog(selectedDescription.LocTitle, translations);
                    translationEditDialog.IsNew = selectedDescription.IsNew;
                    translationEditDialog.HasBeenSeen = selectedDescription.HasBeenSeen;
                    
                    if (translationEditDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Update the description with new/modified translation
                        selectedDescription.LocTitle = translationEditDialog.TranslationKey;
                        selectedDescription.Translation = translationEditDialog.TranslationText;
                        selectedDescription.IsNew = translationEditDialog.IsNew;
                        selectedDescription.HasBeenSeen = translationEditDialog.HasBeenSeen;
                        
                        // Add to translations collection if it's a new key
                        if (translationEditDialog.IsNewTranslation)
                        {
                            var newTranslation = new Translation 
                            {
                                Key = translationEditDialog.TranslationKey,
                                Element = new TranslationElement 
                                {
                                    Token = translationEditDialog.TranslationKey,
                                    Translations = new List<string> { translationEditDialog.TranslationText }
                                }
                            };
                            translations.Add(newTranslation);
                            
                            // Save the updated translations
                            SaveTranslationsToFile();
                        }
                        else
                        {
                            // Update existing translation
                            var existingTranslation = translations.FirstOrDefault(t => t.Key == translationEditDialog.TranslationKey);
                            if (existingTranslation != null)
                            {
                                if (existingTranslation.Element == null)
                                    existingTranslation.Element = new TranslationElement();
                                
                                existingTranslation.Element.Token = translationEditDialog.TranslationKey;
                                if (existingTranslation.Element.Translations == null)
                                    existingTranslation.Element.Translations = new List<string>();
                                
                                if (existingTranslation.Element.Translations.Count > 0)
                                    existingTranslation.Element.Translations[0] = translationEditDialog.TranslationText;
                                else
                                    existingTranslation.Element.Translations.Add(translationEditDialog.TranslationText);
                                
                                // Save the updated translations
                                SaveTranslationsToFile();
                            }
                        }
                        
                        OnQuestPropertyChanged();
                        UpdateQuestDetails(); // Refresh the descriptions list
                        
                        MessageBox.Show("Description updated successfully!", "Description Updated", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing description: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteDescription_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null || lstDescriptions.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a description to delete.", "No Description Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int selectedIndex = lstDescriptions.SelectedIndex;
                if (selectedIndex < activeQuest.Descriptions?.Count)
                {
                    var selectedDescription = activeQuest.Descriptions[selectedIndex];
                    
                    var result = MessageBox.Show($"Are you sure you want to delete this description?\n\n\"{selectedDescription.Translation}\"", 
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        activeQuest.Descriptions.RemoveAt(selectedIndex);
                        OnQuestPropertyChanged();
                        UpdateQuestDetails(); // Refresh the descriptions list
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting description: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sub-Quest management methods
        private void UpdateSubQuestsList()
        {
            lstSubQuests.Items.Clear();
            
            if (activeQuest?.SubQuests?.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Updating sub-quests list with {activeQuest.SubQuests.Count} sub-quests");
                foreach (int subQuestId in activeQuest.SubQuests)
                {
                    var subQuest = quests.FirstOrDefault(q => q.ID == subQuestId);
                    string displayText = subQuest != null ? $"{subQuestId}: {subQuest.Title}" : $"{subQuestId}: (Quest not found)";
                    lstSubQuests.Items.Add(new { Text = displayText, Value = subQuestId });
                    System.Diagnostics.Debug.WriteLine($"Added sub-quest: {displayText}");
                }
                
                lstSubQuests.DisplayMember = "Text";
                lstSubQuests.ValueMember = "Value";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No sub-quests to display - quest.SubQuests is null or empty");
            }
        }

        private void btnAddSubQuest_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnAddSubQuest_Click called");
            
            if (!isEditing || activeQuest == null || cmbAvailableQuests.SelectedItem == null)
            {
                System.Diagnostics.Debug.WriteLine($"btnAddSubQuest_Click failed checks - isEditing: {isEditing}, activeQuest: {activeQuest != null}, selectedItem: {cmbAvailableQuests.SelectedItem != null}");
                MessageBox.Show("Please select a quest to add as a sub-quest.", "No Quest Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int selectedQuestId = ((dynamic)cmbAvailableQuests.SelectedItem).Value;
                
                // Don't allow adding self as sub-quest
                if (selectedQuestId == activeQuest.ID)
                {
                    MessageBox.Show("A quest cannot be a sub-quest of itself.", "Invalid Sub-Quest", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Check if already added
                if (activeQuest.SubQuests?.Contains(selectedQuestId) == true)
                {
                    MessageBox.Show("This quest is already a sub-quest.", "Duplicate Sub-Quest", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Check for circular dependency
                if (WouldCreateCircularDependency(selectedQuestId, activeQuest.ID))
                {
                    MessageBox.Show("Adding this sub-quest would create a circular dependency.", "Circular Dependency", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Add the sub-quest
                if (activeQuest.SubQuests == null)
                    activeQuest.SubQuests = new List<int>();
                
                activeQuest.SubQuests.Add(selectedQuestId);
                OnQuestPropertyChanged();
                UpdateSubQuestsList();
                UpdateQuestDetails(); // Refresh the read-only display too
                
                // Reset the dropdown
                cmbAvailableQuests.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding sub-quest: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveSubQuest_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null || lstSubQuests.SelectedItem == null)
            {
                MessageBox.Show("Please select a sub-quest to remove.", "No Sub-Quest Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int selectedSubQuestId = ((dynamic)lstSubQuests.SelectedItem).Value;
                
                var subQuest = quests.FirstOrDefault(q => q.ID == selectedSubQuestId);
                string questName = subQuest?.Title ?? "Unknown Quest";
                
                var result = MessageBox.Show($"Are you sure you want to remove sub-quest:\n\n\"{selectedSubQuestId}: {questName}\"?", 
                    "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    activeQuest.SubQuests.Remove(selectedSubQuestId);
                    OnQuestPropertyChanged();
                    UpdateSubQuestsList();
                    UpdateQuestDetails(); // Refresh the read-only display too
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing sub-quest: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool WouldCreateCircularDependency(int newSubQuestId, int parentQuestId)
        {
            // Simple circular dependency check
            var newSubQuest = quests.FirstOrDefault(q => q.ID == newSubQuestId);
            if (newSubQuest?.SubQuests?.Contains(parentQuestId) == true)
            {
                return true; // Direct circular dependency
            }
            
            // Check deeper levels (one level deep for now)
            if (newSubQuest?.SubQuests?.Count > 0)
            {
                foreach (int subSubQuestId in newSubQuest.SubQuests)
                {
                    var subSubQuest = quests.FirstOrDefault(q => q.ID == subSubQuestId);
                    if (subSubQuest?.SubQuests?.Contains(parentQuestId) == true)
                    {
                        return true; // Indirect circular dependency
                    }
                }
            }
            
            return false;
        }
        
        // Initialize all dropdown controls with current quest values when entering edit mode
        private void InitializeDropdownsForEditing()
        {
            if (activeQuest == null) return;

            try
            {
                // Set district dropdown - compare enum values, not text
                cmbDistrict.SelectedIndex = -1;
                for (int i = 0; i < cmbDistrict.Items.Count; i++)
                {
                    var item = (dynamic)cmbDistrict.Items[i];
                    if (item.Value.ToString() == activeQuest.District)
                    {
                        cmbDistrict.SelectedIndex = i;
                        break;
                    }
                }
                
                // Refresh title key dropdown to ensure it includes all current quest title keys
                PopulateTitleKeyDropdown();
                
                // Set title key dropdown
                cmbTitleKey.SelectedIndex = -1;
                if (!string.IsNullOrEmpty(activeQuest.TitleKey))
                {
                    for (int i = 0; i < cmbTitleKey.Items.Count; i++)
                    {
                        string item = cmbTitleKey.Items[i].ToString();
                        if (item == activeQuest.TitleKey)
                        {
                            cmbTitleKey.SelectedIndex = i;
                            System.Diagnostics.Debug.WriteLine($"Found title key {activeQuest.TitleKey} at index {i}");
                            break;
                        }
                    }
                    
                    if (cmbTitleKey.SelectedIndex == -1)
                    {
                        System.Diagnostics.Debug.WriteLine($"Title key {activeQuest.TitleKey} not found in dropdown");
                    }
                }
                
                // Set wake on location numeric control
                numWakeOnLocation.Value = Math.Max(-1, Math.Min(500, activeQuest.WakeOnLocation));
                
                // Set location ID numeric control
                if (activeQuest.Location != null && activeQuest.Location.LocationID > 0)
                {
                    numLocationID.Value = Math.Max(-1, Math.Min(500, activeQuest.Location.LocationID));
                }
                else
                {
                    numLocationID.Value = -1;
                }
                
                // Update available quests dropdown to exclude current quest and existing sub-quests
                PopulateAvailableQuestsDropdownForEditing();
                
                // Initialize reward controls to default state
                InitializeRewardControlsForEditing();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing dropdowns: {ex.Message}");
            }
        }
        
        // Translation management helper methods
        private void SaveTranslationsToFile()
        {
            try
            {
                FileManager.SaveTranslationDefinitionsXML(translations, "translations_modified.xml", FileManager.ExecPath);
                System.Diagnostics.Debug.WriteLine("Translations saved to translations_modified.xml");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving translations: {ex.Message}");
                // Don't show error to user as this is background operation
            }
        }
        
        // Quest Rewards management methods
        private void UpdateRewardsList()
        {
            lstRewards.Items.Clear();
            
            if (activeQuest?.Rewards?.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Updating rewards list with {activeQuest.Rewards.Count} rewards");
                foreach (var reward in activeQuest.Rewards)
                {
                    lstRewards.Items.Add(new { Text = reward.DisplayText, Value = reward });
                    System.Diagnostics.Debug.WriteLine($"Added reward: {reward.DisplayText}");
                }
                
                lstRewards.DisplayMember = "Text";
                lstRewards.ValueMember = "Value";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No rewards to display - quest.Rewards is null or empty");
            }
        }
        
        private void btnAddReward_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnAddReward_Click called");
            
            if (!isEditing || activeQuest == null)
            {
                System.Diagnostics.Debug.WriteLine($"btnAddReward_Click failed checks - isEditing: {isEditing}, activeQuest: {activeQuest != null}");
                return;
            }

            try
            {
                var selectedRewardType = cmbRewardType.SelectedItem as dynamic;
                var selectedItem = cmbRewardItem.SelectedItem as dynamic;
                
                if (selectedRewardType == null || selectedItem == null)
                {
                    MessageBox.Show("Please select both reward type and item.", "Missing Selection", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                var newReward = new QuestReward
                {
                    Type = (QuestRewardType)selectedRewardType.Value,
                    ItemId = (int)selectedItem.Value,
                    ItemName = selectedItem.ItemName ?? selectedItem.Text,
                    Quantity = (int)numRewardQuantity.Value,
                    DropChance = (float)numDropChance.Value,
                    IsGuaranteed = chkGuaranteed.Checked,
                    Description = $"{selectedRewardType.Text}: {selectedItem.ItemName}"
                };

                if (activeQuest.Rewards == null)
                    activeQuest.Rewards = new List<QuestReward>();

                activeQuest.Rewards.Add(newReward);
                OnQuestPropertyChanged();
                UpdateRewardsList();

                // Reset form
                cmbRewardType.SelectedIndex = 0;
                cmbRewardItem.SelectedIndex = -1;
                numRewardQuantity.Value = 1;
                numDropChance.Value = 100;
                chkGuaranteed.Checked = true;

                MessageBox.Show($"Added reward: {newReward.DisplayText}", "Reward Added", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding reward: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnEditReward_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null || selectedReward == null || selectedRewardIndex < 0)
            {
                MessageBox.Show("Please select a reward to update.", "No Reward Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"\n=== UPDATING REWARD ===");
                System.Diagnostics.Debug.WriteLine($"Original reward: {selectedReward.DisplayText}");
                
                // Get values from UI controls
                var newType = QuestRewardType.Item;
                if (cmbRewardType.SelectedItem != null)
                {
                    newType = (QuestRewardType)((dynamic)cmbRewardType.SelectedItem).Value;
                }
                
                var newItemId = 0;
                var newItemName = "Unknown Item";
                if (cmbRewardItem.SelectedItem != null && cmbRewardItem.Visible)
                {
                    var selectedItemData = (dynamic)cmbRewardItem.SelectedItem;
                    newItemId = (int)selectedItemData.Value;
                    newItemName = selectedItemData.ItemName ?? selectedItemData.Text;
                }
                
                var newQuantity = (int)numRewardQuantity.Value;
                var newDropChance = (float)numDropChance.Value;
                var newIsGuaranteed = chkGuaranteed.Checked;
                
                // Update the reward object
                selectedReward.Type = newType;
                selectedReward.ItemId = newItemId;
                selectedReward.ItemName = newItemName;
                selectedReward.Quantity = newQuantity;
                selectedReward.DropChance = newDropChance;
                selectedReward.IsGuaranteed = newIsGuaranteed;
                
                // Update type-specific properties
                if (newType == QuestRewardType.Prototype)
                {
                    selectedReward.PrototypeInfo = newItemName;
                }
                else if (newType == QuestRewardType.DistrictPass)
                {
                    selectedReward.DistrictName = "Unknown District";
                    selectedReward.ItemName = $"District Pass ({selectedReward.DistrictName})";
                }
                
                System.Diagnostics.Debug.WriteLine($"Updated reward: {selectedReward.DisplayText}");
                
                // Refresh the rewards list to show the updated values
                UpdateRewardsList();
                
                // Reselect the updated item
                if (selectedRewardIndex < lstRewards.Items.Count)
                {
                    lstRewards.SelectedIndex = selectedRewardIndex;
                }
                
                // Mark as having unsaved changes
                hasUnsavedChanges = true;
                
                MessageBox.Show($"Reward updated successfully!\n\nNew values:\n" +
                    $"Type: {selectedReward.Type}\n" +
                    $"Item: {selectedReward.ItemName}\n" +
                    $"Quantity: {selectedReward.Quantity}\n" +
                    $"Drop Chance: {selectedReward.DropChance}%\n" +
                    $"Guaranteed: {selectedReward.IsGuaranteed}", 
                    "Reward Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                System.Diagnostics.Debug.WriteLine($"=== REWARD UPDATE COMPLETE ===\n");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error updating reward: {ex.Message}");
                MessageBox.Show($"Error updating reward: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnDeleteReward_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null || lstRewards.SelectedItem == null)
            {
                MessageBox.Show("Please select a reward to delete.", "No Reward Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var selectedReward = ((dynamic)lstRewards.SelectedItem).Value as QuestReward;
                
                var result = MessageBox.Show($"Are you sure you want to delete this reward?\n\n\"{selectedReward.DisplayText}\"", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    activeQuest.Rewards.Remove(selectedReward);
                    OnQuestPropertyChanged();
                    UpdateRewardsList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting reward: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnBrowseItems_Click(object sender, EventArgs e)
        {
            try
            {
                // Open the existing ItemBrowser for item selection
                var itemBrowser = new ItemBrowser();
                if (itemBrowser.ShowDialog() == DialogResult.OK)
                {
                    // Refresh item dropdown to include any new items
                    PopulateRewardItemDropdown();
                    MessageBox.Show("Item Browser closed. Item dropdown has been refreshed.", 
                        "Items Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Item Browser: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void lstRewards_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When a reward is selected in the list, populate the UI controls
            if (lstRewards.SelectedItem != null && isEditing)
            {
                try
                {
                    selectedReward = ((dynamic)lstRewards.SelectedItem).Value as QuestReward;
                    selectedRewardIndex = lstRewards.SelectedIndex;
                    
                    if (selectedReward != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"\n=== REWARD SELECTION ===");
                        System.Diagnostics.Debug.WriteLine($"Selected reward: {selectedReward.DisplayText}");
                        System.Diagnostics.Debug.WriteLine($"Index: {selectedRewardIndex}, Type: {selectedReward.Type}, ItemId: {selectedReward.ItemId}");
                        
                        // Temporarily disable event handlers to prevent recursion
                        cmbRewardType.SelectedIndexChanged -= cmbRewardType_SelectedIndexChanged;
                        
                        // Set reward type dropdown
                        bool typeSet = false;
                        for (int i = 0; i < cmbRewardType.Items.Count; i++)
                        {
                            var item = (dynamic)cmbRewardType.Items[i];
                            if ((QuestRewardType)item.Value == selectedReward.Type)
                            {
                                cmbRewardType.SelectedIndex = i;
                                typeSet = true;
                                System.Diagnostics.Debug.WriteLine($"\u2713 Set reward type to: {selectedReward.Type} (index {i})");
                                break;
                            }
                        }
                        if (!typeSet)
                        {
                            System.Diagnostics.Debug.WriteLine($"\u2717 Failed to set reward type: {selectedReward.Type}");
                        }
                        
                        // Re-enable type handler and trigger it to update UI visibility
                        cmbRewardType.SelectedIndexChanged += cmbRewardType_SelectedIndexChanged;
                        cmbRewardType_SelectedIndexChanged(cmbRewardType, EventArgs.Empty);
                        
                        // Set reward item if it's an item/prototype type
                        if ((selectedReward.Type == QuestRewardType.Item || 
                             selectedReward.Type == QuestRewardType.Prototype || 
                             selectedReward.Type == QuestRewardType.Blueprint) && 
                            selectedReward.ItemId > 0)
                        {
                            bool itemSet = false;
                            for (int i = 0; i < cmbRewardItem.Items.Count; i++)
                            {
                                var item = (dynamic)cmbRewardItem.Items[i];
                                if ((int)item.Value == selectedReward.ItemId)
                                {
                                    cmbRewardItem.SelectedIndex = i;
                                    itemSet = true;
                                    System.Diagnostics.Debug.WriteLine($"✓ Set item dropdown to: {item.Text} (ID: {selectedReward.ItemId})");
                                    break;
                                }
                            }
                            if (!itemSet)
                            {
                                System.Diagnostics.Debug.WriteLine($"✗ Failed to find item with ID: {selectedReward.ItemId}");
                                // Clear the selection if item not found
                                cmbRewardItem.SelectedIndex = -1;
                            }
                        }
                        else
                        {
                            cmbRewardItem.SelectedIndex = -1;
                            System.Diagnostics.Debug.WriteLine($"Cleared item dropdown (Type: {selectedReward.Type}, ItemId: {selectedReward.ItemId})");
                        }
                        
                        // Set numeric values
                        numRewardQuantity.Value = Math.Max(1, Math.Min(999, selectedReward.Quantity));
                        numDropChance.Value = Math.Max(0, Math.Min(100, (decimal)selectedReward.DropChance));
                        chkGuaranteed.Checked = selectedReward.IsGuaranteed;
                        
                        // Enable the update button
                        btnEditReward.Text = "Update";
                        btnEditReward.Enabled = true;
                        
                        System.Diagnostics.Debug.WriteLine($"✓ Populated all UI controls:");
                        System.Diagnostics.Debug.WriteLine($"  - Type: {selectedReward.Type}");
                        System.Diagnostics.Debug.WriteLine($"  - Item: {selectedReward.ItemName} (ID: {selectedReward.ItemId})");
                        System.Diagnostics.Debug.WriteLine($"  - Quantity: {selectedReward.Quantity}");
                        System.Diagnostics.Debug.WriteLine($"  - Drop Chance: {selectedReward.DropChance}%");
                        System.Diagnostics.Debug.WriteLine($"  - Guaranteed: {selectedReward.IsGuaranteed}");
                        System.Diagnostics.Debug.WriteLine($"=== END REWARD SELECTION ===\n");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"✗ Error selecting reward: {ex.Message}");
                    selectedReward = null;
                    selectedRewardIndex = -1;
                    btnEditReward.Text = "Edit";
                    btnEditReward.Enabled = false;
                }
            }
            else
            {
                // Clear selection
                selectedReward = null;
                selectedRewardIndex = -1;
                btnEditReward.Text = "Edit";
                btnEditReward.Enabled = false;
                System.Diagnostics.Debug.WriteLine("Cleared reward selection");
            }
        }
        
        private void cmbRewardType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update UI controls based on reward type selection
            if (cmbRewardType.SelectedItem != null)
            {
                var selectedType = (QuestRewardType)((dynamic)cmbRewardType.SelectedItem).Value;
                
                // Show/hide controls based on reward type
                bool showItemDropdown = selectedType == QuestRewardType.Item || 
                                       selectedType == QuestRewardType.Blueprint || 
                                       selectedType == QuestRewardType.Prototype;
                bool showQuantity = selectedType != QuestRewardType.DistrictPass;
                
                cmbRewardItem.Visible = showItemDropdown;
                lblRewardItem.Visible = showItemDropdown;
                btnBrowseItems.Visible = showItemDropdown;
                
                numRewardQuantity.Visible = showQuantity;
                lblQuantity.Visible = showQuantity;
                
                // Clear item selection when switching types unless we're loading an existing reward
                if (selectedReward == null && showItemDropdown && cmbRewardItem.Items.Count > 0)
                {
                    cmbRewardItem.SelectedIndex = -1;
                }
                
                // Adjust quantity defaults based on type
                if (selectedReward == null)
                {
                    switch (selectedType)
                    {
                        case QuestRewardType.Money:
                            numRewardQuantity.Value = 1000;
                            break;
                        case QuestRewardType.Experience:
                            numRewardQuantity.Value = 100;
                            break;
                        case QuestRewardType.DistrictPass:
                            numRewardQuantity.Value = 1;
                            break;
                        default:
                            numRewardQuantity.Value = 1;
                            break;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Reward type changed to: {selectedType}, Item controls visible: {showItemDropdown}, Quantity visible: {showQuantity}");
            }
        }
    }
}