using dto = SRMod.DTOs;
using SRMod.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TextManager;
using SatelliteReignModdingTools.Services;
using SatelliteReignModdingTools.Controls;
using System.Xml.Serialization;

namespace SatelliteReignModdingTools
{
    public partial class ItemBrowser : Form
    {
        public static List<dto.TranslationElementDTO> _translations = new List<dto.TranslationElementDTO>();
        public static List<dto.SerializableItemData> itemDTOs = new List<dto.SerializableItemData>();
        static List<Models.Ability> abilities = new List<Models.Ability>();
        static dto.SerializableItemData activeItemData = new dto.SerializableItemData();
        static dto.SerializableModifierData activeModifier;
        private const string _itemDataFileName = "itemDefinitions.xml";

        public static int activeLanguage = 2;

        // Shared toolbar instance
        private SharedToolbar _toolbar;
        // Keep an unfiltered copy for search
        private List<dto.SerializableItemData> _allItems = new List<dto.SerializableItemData>();

        public ItemBrowser()
        {
            try
            {
                // Initialize language and cultures
                SRMapper.LanguageMapper();

                InitializeComponent();

                // Insert shared toolbar at the top (programmatic, non-designer)
                _toolbar = new SharedToolbar();
                _toolbar.ReloadClicked += () => loadAllToolStripMenuItem_Click(this, EventArgs.Empty);
                _toolbar.SaveClicked += () => saveToolStripMenuItem_Click(this, EventArgs.Empty);
                _toolbar.SaveWithDiffClicked += () => saveWithDiffToolStripMenuItem_Click(this, EventArgs.Empty);
                _toolbar.ValidateClicked += () => ShowValidationResults();
                _toolbar.SearchTextChanged += text => ApplySearch(text);
                Controls.Add(_toolbar);
                // Place toolbar just under the menu to avoid overlapping designed controls
                _toolbar.Dock = DockStyle.None;
                _toolbar.Location = new Point(0, menuStrip1.Bottom);
                _toolbar.Width = this.ClientSize.Width;
                _toolbar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                // Shift existing controls down by toolbar height (except menu and toolbar)
                foreach (Control c in this.Controls)
                {
                    if (c == _toolbar || c == menuStrip1) continue;
                    c.Top += _toolbar.Height;
                }
                _toolbar.BringToFront();

                // Add tooltip to indicate the icon is clickable
                var iconTooltip = new System.Windows.Forms.ToolTip();
                iconTooltip.SetToolTip(ItemIconImageBox, "Click to change item icon");

                ItemListBox.ClearSelected();
                _translations = FileManager.LoadTranslationsXML("Translations.xml", FileManager.ExecPath).ToList();
                itemDTOs = FileManager.LoadItemDataXML(_itemDataFileName, FileManager.ExecPath).OrderBy(i => i.m_ID).ToList();
                _allItems = new List<dto.SerializableItemData>(itemDTOs);
                UpdateItemInfo();

                ItemSlotTypeDropDown.DataSource = Enum.GetValues(typeof(ItemSlotTypes)).Cast<ItemSlotTypes>().ToList().Take(8).ToList();
                GearSubTypeDropDown.DataSource = Enum.GetValues(typeof(ItemSubCategories));
                WeaponTypeDropDown.DataSource = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList().Take(29).ToList();

                // Build abilities list from translations
                abilities = _translations
                    .Where(t => t.Key.StartsWith("ABILITY_") && t.Key.EndsWith("_NAME"))
                    .Select(a =>
                    {
                        int id = int.Parse(a.Key.Replace("ABILITY_", string.Empty).Replace("_NAME", string.Empty));
                        LocElement name = a.Element;
                        LocElement desc = _translations.FirstOrDefault(t => t.Key == $"ABILITY_{id}_DESC")?.Element;
                    return new Models.Ability
                    {
                        Id = id,
                        LocName = name,
                        LocDesc = desc
                    };
                })
                .OrderBy(x => x.Name)
                .ToList();

                AbilityDropdown.DataSource = abilities;
                AbilityDropdown.DisplayMember = "Name";
                AbilityDropdown.ValueMember = "This";
                ModifierDropdown.DataSource = Enum.GetValues(typeof(ModifierType)).Cast<ModifierType>().ToList().Skip(1).ToList();

                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"ItemBrowser initialization failed:\n\n{ex.GetType().Name}: {ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                    "ItemBrowser Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw;
            }
        }

        private void ApplySearch(string text)
        {
            try
            {
                text = text ?? string.Empty;
                IEnumerable<dto.SerializableItemData> filtered = _allItems;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var lower = text.ToLowerInvariant();
                    filtered = _allItems.Where(i =>
                        ($"{i.m_ID}".Contains(lower)) ||
                        (_translations.FirstOrDefault(t => t.Key == $"ITEM_{i.m_ID}_NAME")?.Element?.m_Translations[activeLanguage]?.ToLowerInvariant()?.Contains(lower) == true)
                    );
                }
                itemDTOs = filtered.OrderBy(i => i.m_ID).ToList();
                UpdateItemInfo();
                UpdateUI();
            }
            catch { }
        }

        private void ShowValidationResults()
        {
            var results = ValidateItems();
            var sb = new StringBuilder();
            if (results.Errors.Count == 0 && results.Warnings.Count == 0)
            {
                MessageBox.Show(this, "No issues found.", "Validate Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (results.Errors.Count > 0)
            {
                sb.AppendLine("Errors:");
                foreach (var e in results.Errors.Take(100)) sb.AppendLine(" - " + e);
                if (results.Errors.Count > 100) sb.AppendLine($"(+{results.Errors.Count - 100} more)");
                sb.AppendLine();
            }
            if (results.Warnings.Count > 0)
            {
                sb.AppendLine("Warnings:");
                foreach (var w in results.Warnings.Take(200)) sb.AppendLine(" - " + w);
                if (results.Warnings.Count > 200) sb.AppendLine($"(+{results.Warnings.Count - 200} more)");
            }
            MessageBox.Show(this, sb.ToString(), "Validate Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private (List<string> Errors, List<string> Warnings) ValidateItems()
        {
            var errors = new List<string>();
            var warnings = new List<string>();
            try
            {
                var nameKeyPrefix = "ITEM_";
                foreach (var item in _allItems)
                {
                    // Translation keys
                    var nameKey = $"{nameKeyPrefix}{item.m_ID}_NAME";
                    var descKey = $"{nameKeyPrefix}{item.m_ID}_DESCRIPTION";
                    var nameLoc = _translations.FirstOrDefault(t => t.Key == nameKey)?.Element?.m_Translations;
                    var descLoc = _translations.FirstOrDefault(t => t.Key == descKey)?.Element?.m_Translations;
                    if (nameLoc == null || string.IsNullOrWhiteSpace(nameLoc.ElementAtOrDefault(activeLanguage)))
                        errors.Add($"Missing name translation: {nameKey}");
                    if (descLoc == null || string.IsNullOrWhiteSpace(descLoc.ElementAtOrDefault(activeLanguage)))
                        warnings.Add($"Missing description translation: {descKey}");

                    // Icon file presence
                    if (!string.IsNullOrWhiteSpace(item.m_UIIconName))
                    {
                        var iconPath = Path.Combine(FileManager.GetIconsPath(), item.m_UIIconName + ".png");
                        if (!File.Exists(iconPath))
                            warnings.Add($"Icon missing: {iconPath}");
                    }

                    // Ability translation presence
                    if (item.m_AbilityIDs != null)
                    {
                        foreach (var abilityId in item.m_AbilityIDs)
                        {
                            var aNameKey = $"ABILITY_{abilityId}_NAME";
                            var aDescKey = $"ABILITY_{abilityId}_DESC";
                            var aName = _translations.FirstOrDefault(t => t.Key == aNameKey)?.Element?.m_Translations?.ElementAtOrDefault(activeLanguage);
                            if (string.IsNullOrWhiteSpace(aName))
                                warnings.Add($"Ability name missing: {aNameKey}");
                            var aDesc = _translations.FirstOrDefault(t => t.Key == aDescKey)?.Element?.m_Translations?.ElementAtOrDefault(activeLanguage);
                            if (string.IsNullOrWhiteSpace(aDesc))
                                warnings.Add($"Ability desc missing: {aDescKey}");
                        }
                    }

                    // Modifier sanity checks
                    if (item.m_Modifiers != null)
                    {
                        foreach (var m in item.m_Modifiers)
                        {
                            if (m.m_TimeOut < 0) warnings.Add($"Item {item.m_ID} modifier {m.m_Type}: negative timeout");
                            if (Math.Abs(m.m_Ammount) > 1_000_000) warnings.Add($"Item {item.m_ID} modifier {m.m_Type}: extreme amount {m.m_Ammount}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            return (errors, warnings);
        }

        private void UpdateUI()
        {
            string description = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_DESCRIPTION").FirstOrDefault()?.Element.m_Translations[activeLanguage];
            string name = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_NAME").FirstOrDefault()?.Element.m_Translations[activeLanguage];

            DescriptionTextBox.Text = description;
            ItemNameTextBox.Text = name;
            ItemSlotTypeDropDown.SelectedIndex = (int)activeItemData.m_Slot;
            GearSubTypeDropDown.SelectedIndex = (int)activeItemData.m_GearSubCategory;
            WeaponTypeDropDown.SelectedIndex = (int)activeItemData.m_WeaponType;
            if (activeItemData.m_UIIconName != null)
            {
                var loadedImage = FileManager.LoadImageFromFile(activeItemData.m_UIIconName);
                if (loadedImage != null)
                {
                    ItemIconImageBox.Image = loadedImage;
                    ChangeImageColor(Color.White, Color.Aquamarine);
                }
                else
                {
                    ItemIconImageBox.Image = null;
                }
            }

            // Update Combat tab fields
            UpdateCombatFields();

            // Update Research tab fields
            UpdateResearchFields();

            // Update Economic tab fields
            UpdateEconomicFields();

            UpdateAbilityInfo();
            UpdateModifierInfo();
        }

        private void UpdateCombatFields()
        {
            if (stealthVsCombatTextBox != null)
                stealthVsCombatTextBox.Text = activeItemData.m_StealthVsCombat.ToString();
            if (weaponAugMaskTextBox != null)
                weaponAugMaskTextBox.Text = activeItemData.m_ValidWeaponAugmentationWeaponMask.ToString();
            if (overrideAmmoTextBox != null)
                overrideAmmoTextBox.Text = activeItemData.m_OverrideAmmo.ToString();
        }

        private void UpdateResearchFields()
        {
            if (prereqIdTextBox != null)
                prereqIdTextBox.Text = activeItemData.m_PrereqID.ToString();
            if (prototypeProgressTextBox != null)
                prototypeProgressTextBox.Text = activeItemData.m_PrototypeProgressionValue.ToString();
            if (blueprintProgressTextBox != null)
                blueprintProgressTextBox.Text = activeItemData.m_BlueprintProgressionValue.ToString();
            if (minResearchersTextBox != null)
                minResearchersTextBox.Text = activeItemData.m_MinResearchersRequired.ToString();

            if (availableToPlayerCheckBox != null)
                availableToPlayerCheckBox.Checked = activeItemData.m_AvailableToPlayer;
            if (playerStartsBlueprintsCheckBox != null)
                playerStartsBlueprintsCheckBox.Checked = activeItemData.m_PlayerStartsWithBlueprints;
            if (playerStartsPrototypeCheckBox != null)
                playerStartsPrototypeCheckBox.Checked = activeItemData.m_PlayerStartsWithPrototype;
            if (playerCanResearchCheckBox != null)
                playerCanResearchCheckBox.Checked = activeItemData.m_PlayerCanResearchFromStart;
        }

        private void UpdateEconomicFields()
        {
            if (itemCostTextBox != null)
                itemCostTextBox.Text = activeItemData.m_Cost.ToString();
            if (blueprintCostTextBox != null)
                blueprintCostTextBox.Text = activeItemData.m_BlueprintCost.ToString();
            if (prototypeCostTextBox != null)
                prototypeCostTextBox.Text = activeItemData.m_PrototypeCost.ToString();
            if (findBlueprintCostTextBox != null)
                findBlueprintCostTextBox.Text = activeItemData.m_FindBlueprintCost.ToString();
            if (findPrototypeCostTextBox != null)
                findPrototypeCostTextBox.Text = activeItemData.m_FindPrototypeCost.ToString();
        }

        private void UpdateItemInfo()
        {
            var items = itemDTOs.Select
            (
                item => new
                {
                    Key = item.m_ID + ": " + _translations.Where(t => t.Key == "ITEM_" + item.m_ID + "_NAME").FirstOrDefault()?.Element.m_Translations[activeLanguage],
                    Value = item
                }
            ).ToList();
            ItemListBox.SelectedIndexChanged -= ItemListBox_SelectedIndexChanged;
            ItemListBox.DataSource = items;
            ItemListBox.DisplayMember = "Key";
            ItemListBox.ValueMember = "Value";
            ItemListBox.ClearSelected();
            ItemListBox.SelectedIndexChanged += ItemListBox_SelectedIndexChanged;

            UpdateAbilityInfo();

            UpdateModifierInfo();
        }

        private void UpdateModifierInfo()
        {
            var modifiers = activeItemData.m_Modifiers?.Select
            (
                modifier => new
                {
                    Key = modifier.m_Type.ToString(),
                    Description = $@"Amount: {modifier.m_Ammount}
AmountModifier: {modifier.m_AmountModifier}
Timeout: {modifier.m_TimeOut}",
                    Enum = modifier.m_Type,
                    Modifier = modifier
                }
            )?.ToList();

            ModifierListBox.SelectedIndexChanged -= ModifierListBox_SelectedIndexChanged;
            ModifierListBox.DataSource = modifiers;
            ModifierListBox.DisplayMember = "Key";
            ModifierListBox.ValueMember = "Description";
            ModifierListBox.ClearSelected();
            ModifierListBox.SelectedIndexChanged += ModifierListBox_SelectedIndexChanged;
        }

        private void UpdateAbilityInfo()
        {
            var itemAbilities = activeItemData.m_AbilityIDs?.Select
            (
                abilityId =>
                {
                    var ability = abilities.Where(a => a.Id == abilityId).FirstOrDefault();
                    if (ability == null)
                        ability = new Models.Ability() { Id = abilityId };
                    return new
                    {
                        Key = abilityId + ": " + ability?.Name,
                        Name = ability?.Name,
                        Value = ability
                    };
                }
            )?.ToList();

            AbilityListBox.SelectedIndexChanged -= AbilityListBox_SelectedIndexChanged;
            AbilityListBox.DataSource = itemAbilities;
            AbilityListBox.DisplayMember = "Key";
            AbilityListBox.ValueMember = "Value";
            AbilityListBox.ClearSelected();
            AbilityListBox.SelectedIndexChanged += AbilityListBox_SelectedIndexChanged;
        }

        private void ChangeImageColor(Color oldColor, Color newColor)
        {
            try
            {
                // Check if the image exists before trying to change its color
                if (ItemIconImageBox.Image == null)
                {
                    SRInfoHelper.Log("Cannot change color: ItemIconImageBox.Image is null");
                    return;
                }

                var sourceImage = FileManager.LoadImageFromFile(activeItemData.m_UIIconName);
                if (sourceImage == null)
                {
                    SRInfoHelper.Log($"Cannot change color: Source image '{activeItemData.m_UIIconName}' could not be loaded");
                    return;
                }

                using (Graphics g = Graphics.FromImage(ItemIconImageBox.Image))
                using (Bitmap bmp = new Bitmap(sourceImage))
                {
                    // Set the image attribute's color mappings
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = oldColor;
                    colorMap[0].NewColor = newColor;
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    // Draw using the color map
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    g.DrawImage(bmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Error changing image color: {ex.Message}");
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Bitmap bmp = new Bitmap("myImage.png"))
            {
                // Set the image attribute's color mappings
                ColorMap[] colorMap = new ColorMap[1];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.Black;
                colorMap[0].NewColor = Color.Blue;
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                // Draw using the color map
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                g.DrawImage(bmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
            }
        }

        private void ItemListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {
                activeItemData = (dto.SerializableItemData)ItemListBox.SelectedItem.GetMemberValue("Value");
                ExtraNameTextBox.Clear();
                ExtraDescriptionTextBox.Clear();
                UpdateUI();
            }
        }

        private void ChangeLanguage(Language language)
        {
            activeLanguage = (int)language;
            UpdateUI();
            int selectedItem = ItemListBox.SelectedIndex;
            UpdateItemInfo();
            ItemListBox.SelectedIndex = selectedItem;
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage(Language.EN);
        }

        private void frenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage(Language.FR);
        }

        private void czechToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage(Language.CZ);
        }

        private void germanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage(Language.GE);
        }

        private void italianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage(Language.IT);
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage(Language.RU);
        }

        private void spanishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage(Language.SP);
        }

        private void AbilityListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AbilityListBox.SelectedItem != null)
            {
                var ability = (Models.Ability)AbilityListBox.SelectedItem.GetMemberValue("Value");
                ExtraNameTextBox.Text = (string)AbilityListBox.SelectedItem.GetMemberValue("Name");
                ExtraDescriptionTextBox.Text = ability?.Desc;
                AbilityDropdown.SelectedItem = ability;
                int index = activeItemData.m_AbilityIDs.IndexOf(ability.Id);
                if (index >= 0 && index < activeItemData.m_AbilityMasks.Count)
                    AmountTextBox.Text = activeItemData.m_AbilityMasks[index].ToString();
            }
        }

        private void ModifierListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModifierListBox.SelectedItem != null)
            {
                ExtraNameTextBox.Text = ModifierListBox.SelectedItem.GetMemberValue("Key")?.ToString();
                ExtraDescriptionTextBox.Text = ModifierListBox.SelectedItem.GetMemberValue("Description")?.ToString();
                ModifierDropdown.SelectedItem = (ModifierType)ModifierListBox.SelectedItem.GetMemberValue("Enum");
                var modifier = (SRMod.DTOs.SerializableModifierData)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                AmountTextBox.Text = modifier.m_Ammount.ToString();
            }
        }

        private void CopyItemButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate preconditions
                if (activeItemData == null)
                {
                    MessageBox.Show("Please select an item to copy first.", "No Item Selected", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (itemDTOs == null)
                {
                    MessageBox.Show("Item list is not initialized.", "System Error", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (activeItemData.m_ID <= 0)
                {
                    MessageBox.Show("Invalid item ID. Cannot copy item.", "Invalid Item", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Disable UI during copy operation
                CopyItemButton.Enabled = false;
                Cursor = Cursors.WaitCursor;

                SRInfoHelper.Log($"User initiated copy of item ID: {activeItemData.m_ID}");

                // Perform the copy using our safe method
                var copiedItem = SREditor.CopyItemSafe(activeItemData, itemDTOs, _translations);
                
                // Add to collections
                itemDTOs.Add(copiedItem);
                _allItems.Add(copiedItem);

                SRInfoHelper.Log($"Item added to collections, refreshing UI");

                // Update UI to show new item
                RefreshUIAndFocusNewItem(copiedItem);

                MessageBox.Show($"Item copied successfully!\n\nOriginal: {activeItemData.m_ID}\nNew Copy: {copiedItem.m_ID}", 
                               "Copy Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy item: {ex.Message}\n\nPlease check the logs for more details.", 
                               "Copy Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SRInfoHelper.Log($"CopyItem failed: {ex}");
            }
            finally
            {
                // Re-enable UI
                CopyItemButton.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void SaveItemButton_Click(object sender, EventArgs e)
        {
            try
            {
                SRInfoHelper.Log($"Starting save process for item {activeItemData.m_ID}...");

                // Save basic item information
                var name = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_NAME").First();
                name.Element.m_Translations[activeLanguage] = ItemNameTextBox.Text;
                var desc = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_DESCRIPTION").First();
                desc.Element.m_Translations[activeLanguage] = DescriptionTextBox.Text;

                Enum.TryParse<ItemSubCategories>(GearSubTypeDropDown.SelectedValue.ToString(), out var m_GearSubCategory);
                activeItemData.m_GearSubCategory = m_GearSubCategory;
                Enum.TryParse<WeaponType>(WeaponTypeDropDown.SelectedValue.ToString(), out var m_WeaponType);
                activeItemData.m_WeaponType = m_WeaponType;
                Enum.TryParse<ItemSlotTypes>(ItemSlotTypeDropDown.SelectedValue.ToString(), out var slottype);
                activeItemData.m_Slot = slottype;

                // Save ALL tab fields with detailed logging
                SaveCombatFields();
                SaveResearchFields();
                SaveEconomicFields();

                // Update the item in both collections to ensure consistency
                UpdateItemInCollections();

                // DIRECTLY SAVE TO XML FILES
                SRInfoHelper.Log("Saving to XML files...");

                // Save item data to XML
                FileManager.SaveAsXML(itemDTOs, _itemDataFileName, FileManager.ExecPath + @"\");
                SRInfoHelper.Log($"Item data saved to {_itemDataFileName}");

                // Save translations to XML
                FileManager.SaveAsXML(_translations, "Translations.xml", FileManager.ExecPath + @"\");
                SRInfoHelper.Log("Translations saved to Translations.xml");

                SRInfoHelper.Log($"Item {activeItemData.m_ID} successfully saved to XML files!");
                MessageBox.Show($"Item {activeItemData.m_ID} saved successfully to XML files!\n\nBoth item data and translations have been persisted.", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Error saving item to XML: {ex}");
                MessageBox.Show($"Error saving item to XML: {ex.Message}\n\nCheck the logs for more details.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCombatFields()
        {
            SRInfoHelper.Log("Saving Combat tab fields...");

            if (stealthVsCombatTextBox != null && float.TryParse(stealthVsCombatTextBox.Text, out float stealthVsCombat))
            {
                activeItemData.m_StealthVsCombat = stealthVsCombat;
                SRInfoHelper.Log($"  StealthVsCombat: {stealthVsCombat}");
            }

            if (weaponAugMaskTextBox != null && int.TryParse(weaponAugMaskTextBox.Text, out int weaponAugMask))
            {
                activeItemData.m_ValidWeaponAugmentationWeaponMask = weaponAugMask;
                SRInfoHelper.Log($"  WeaponAugmentationMask: {weaponAugMask}");
            }

            if (overrideAmmoTextBox != null && int.TryParse(overrideAmmoTextBox.Text, out int overrideAmmo))
            {
                activeItemData.m_OverrideAmmo = overrideAmmo;
                SRInfoHelper.Log($"  OverrideAmmo: {overrideAmmo}");
            }

            SRInfoHelper.Log("Combat fields saved successfully");
        }

        private void SaveResearchFields()
        {
            SRInfoHelper.Log("Saving Research tab fields...");

            if (prereqIdTextBox != null && int.TryParse(prereqIdTextBox.Text, out int prereqId))
            {
                activeItemData.m_PrereqID = prereqId;
                SRInfoHelper.Log($"  PrereqID: {prereqId}");
            }

            if (prototypeProgressTextBox != null && int.TryParse(prototypeProgressTextBox.Text, out int prototypeProgress))
            {
                activeItemData.m_PrototypeProgressionValue = prototypeProgress;
                SRInfoHelper.Log($"  PrototypeProgressionValue: {prototypeProgress}");
            }

            if (blueprintProgressTextBox != null && int.TryParse(blueprintProgressTextBox.Text, out int blueprintProgress))
            {
                activeItemData.m_BlueprintProgressionValue = blueprintProgress;
                SRInfoHelper.Log($"  BlueprintProgressionValue: {blueprintProgress}");
            }

            if (minResearchersTextBox != null && int.TryParse(minResearchersTextBox.Text, out int minResearchers))
            {
                activeItemData.m_MinResearchersRequired = minResearchers;
                SRInfoHelper.Log($"  MinResearchersRequired: {minResearchers}");
            }

            // IMPORTANT: Checkbox states - these are the critical settings the user requested
            if (availableToPlayerCheckBox != null)
            {
                activeItemData.m_AvailableToPlayer = availableToPlayerCheckBox.Checked;
                SRInfoHelper.Log($"  AvailableToPlayer (CHECKBOX): {availableToPlayerCheckBox.Checked}");
            }

            if (playerStartsBlueprintsCheckBox != null)
            {
                activeItemData.m_PlayerStartsWithBlueprints = playerStartsBlueprintsCheckBox.Checked;
                SRInfoHelper.Log($"  PlayerStartsWithBlueprints (CHECKBOX): {playerStartsBlueprintsCheckBox.Checked}");
            }

            if (playerStartsPrototypeCheckBox != null)
            {
                activeItemData.m_PlayerStartsWithPrototype = playerStartsPrototypeCheckBox.Checked;
                SRInfoHelper.Log($"  PlayerStartsWithPrototype (CHECKBOX): {playerStartsPrototypeCheckBox.Checked}");
            }

            if (playerCanResearchCheckBox != null)
            {
                activeItemData.m_PlayerCanResearchFromStart = playerCanResearchCheckBox.Checked;
                SRInfoHelper.Log($"  PlayerCanResearchFromStart (CHECKBOX): {playerCanResearchCheckBox.Checked}");
            }

            SRInfoHelper.Log("Research fields (including checkboxes) saved successfully");
        }

        private void SaveEconomicFields()
        {
            SRInfoHelper.Log("Saving Economic tab fields...");

            if (itemCostTextBox != null && float.TryParse(itemCostTextBox.Text, out float itemCost))
            {
                activeItemData.m_Cost = itemCost;
                SRInfoHelper.Log($"  Cost: {itemCost}");
            }

            if (blueprintCostTextBox != null && float.TryParse(blueprintCostTextBox.Text, out float blueprintCost))
            {
                activeItemData.m_BlueprintCost = blueprintCost;
                SRInfoHelper.Log($"  BlueprintCost: {blueprintCost}");
            }

            if (prototypeCostTextBox != null && float.TryParse(prototypeCostTextBox.Text, out float prototypeCost))
            {
                activeItemData.m_PrototypeCost = prototypeCost;
                SRInfoHelper.Log($"  PrototypeCost: {prototypeCost}");
            }

            if (findBlueprintCostTextBox != null && float.TryParse(findBlueprintCostTextBox.Text, out float findBlueprintCost))
            {
                activeItemData.m_FindBlueprintCost = findBlueprintCost;
                SRInfoHelper.Log($"  FindBlueprintCost: {findBlueprintCost}");
            }

            if (findPrototypeCostTextBox != null && float.TryParse(findPrototypeCostTextBox.Text, out float findPrototypeCost))
            {
                activeItemData.m_FindPrototypeCost = findPrototypeCost;
                SRInfoHelper.Log($"  FindPrototypeCost: {findPrototypeCost}");
            }

            SRInfoHelper.Log("Economic fields saved successfully");
        }

        float oldAmountValue;

        private void AmountTextBox_TextChanged(object sender, EventArgs e)
        {
            char a = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (a == ',')
                AmountTextBox.Text = Regex.Replace(AmountTextBox.Text, "[^-,0-9]", "");
            else
                AmountTextBox.Text = Regex.Replace(AmountTextBox.Text, "[^-.0-9]", "");

            if (!string.IsNullOrEmpty(AmountTextBox.Text) && char.IsDigit(AmountTextBox.Text.First()) && char.IsDigit(AmountTextBox.Text.Last()))
                if (float.TryParse(AmountTextBox.Text, out float amount))
                {
                    AmountTextBox.Text = amount.ToString();
                    oldAmountValue = amount;
                }
                else
                {
                    AmountTextBox.Text = oldAmountValue.ToString();
                }
        }

        private void AddModiferButton_Click(object sender, EventArgs e)
        {
            var type = (ModifierType)ModifierDropdown.SelectedItem;
            if (!activeItemData.m_Modifiers.Select(m => m.m_Type).Contains(type))
            {
                var modifiers = activeItemData.m_Modifiers.ToList();
                var newMod = new SRMod.DTOs.SerializableModifierData() { m_Type = type };
                modifiers.Add(newMod);
                activeItemData.m_Modifiers = modifiers;
                UpdateModifierInfo();

                ModifierListBox.SelectedIndex = activeItemData.m_Modifiers.Count() - 1;
            }
        }

        private void SaveModifierButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(AmountTextBox.Text, out float amount))
            {
                AmountTextBox.Text = amount.ToString();
                oldAmountValue = amount;
                var modifier = (SRMod.DTOs.SerializableModifierData)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                modifier.m_Ammount = amount;
            }
            else
            {
                MessageBox.Show(oldAmountValue.ToString() + " is not a number", "Cannot save modifiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteModifierButton_Click(object sender, EventArgs e)
        {
            if (ModifierListBox.SelectedItem != null)
            {
                var modifier = (dto.SerializableModifierData)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                var modifiers = activeItemData.m_Modifiers.ToList();
                modifiers.Remove(modifier);
                activeItemData.m_Modifiers = modifiers;
                UpdateModifierInfo();
            }
            else
            {
                MessageBox.Show("No modifier selected that can be deleted", "Cannot delete modifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteAbilityButton_Click(object sender, EventArgs e)
        {
            if (AbilityListBox.SelectedItem != null)
            {
                var abil = ((Models.Ability)AbilityListBox.SelectedItem.GetMemberValue("Value"));

                if (abil != null)
                {
                    int index = activeItemData.m_AbilityIDs.IndexOf(abil.Id);
                    activeItemData.m_AbilityIDs.RemoveAt(index);
                    activeItemData.m_AbilityMasks.RemoveAt(index);
                    if (abil.Id == 1407 & activeItemData.m_AbilityIDs.Any() && activeItemData.m_AbilityIDs.Contains(1369))
                    {
                        index = activeItemData.m_AbilityIDs.IndexOf(1369);
                        activeItemData.m_AbilityIDs.RemoveAt(index);
                        activeItemData.m_AbilityMasks.RemoveAt(index);
                    }
                    UpdateAbilityInfo();
                }
                else if (activeItemData.m_AbilityIDs.Any() && activeItemData.m_AbilityIDs.Contains(1369))
                {
                    int index = activeItemData.m_AbilityIDs.IndexOf(1369);
                    activeItemData.m_AbilityIDs.RemoveAt(index);
                    activeItemData.m_AbilityMasks.RemoveAt(index);
                    index = activeItemData.m_AbilityIDs.IndexOf(1407);
                    activeItemData.m_AbilityIDs.RemoveAt(index);
                    activeItemData.m_AbilityMasks.RemoveAt(index);
                    UpdateAbilityInfo();
                }
                else
                {
                    MessageBox.Show($"Could not delete this type of ability ()", "Ability deletion error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No ability to delete", "No abililty selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddAbility_Click(object sender, EventArgs e)
        {
            if (AbilityDropdown.SelectedItem != null)
            {
                Models.Ability ability = (Models.Ability)AbilityDropdown.SelectedItem;
                if (!activeItemData.m_AbilityIDs.Contains(ability.Id))
                {
                    activeItemData.m_AbilityIDs.Add(ability.Id);

                    if (ability.Id == 1407)
                    {
                        activeItemData.m_AbilityMasks.Add(-2);
                        activeItemData.m_AbilityIDs.Add(1369);
                        activeItemData.m_AbilityMasks.Add(-2);
                    }
                    else
                        activeItemData.m_AbilityMasks.Add(-1);
                    UpdateAbilityInfo();
                }
                else
                {
                    MessageBox.Show("Cannot add same ability twice", "Ability already on item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteItemButton_Click(object sender, EventArgs e)
        {
            string messageText = "Are you sure you want to delete this item? It will not be saved to the data files before you also click save in the file menu.";
            if (activeItemData.m_ID < 145)
            {
                messageText += @"
This item is one of the games original items, deleting it might cause problems for the game if you try to play the game without it.";
            }

            var result = MessageBox.Show(messageText, "Are you sure you want to delete this item?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

            if (result == DialogResult.Yes)
            {
                var itemID = activeItemData.m_ID;
                var name = _translations.Where(t => t.Key == "ITEM_" + itemID + "_NAME").FirstOrDefault();
                var desc = _translations.Where(t => t.Key == "ITEM_" + itemID + "_DESCRIPTION").FirstOrDefault();

                if (name != null)
                    _translations.Remove(name);
                if (desc != null)
                    _translations.Remove(desc);

                // Remove from main collection by ID (not by reference)
                var itemToRemove = itemDTOs.FirstOrDefault(i => i.m_ID == itemID);
                if (itemToRemove != null)
                {
                    itemDTOs.Remove(itemToRemove);
                    SRInfoHelper.Log($"Removed item {itemID} from itemDTOs");
                }

                // Remove from filtered collection by ID (not by reference)
                var filteredItemToRemove = _allItems.FirstOrDefault(i => i.m_ID == itemID);
                if (filteredItemToRemove != null)
                {
                    _allItems.Remove(filteredItemToRemove);
                    SRInfoHelper.Log($"Removed item {itemID} from _allItems");
                }

                UpdateItemInfo();
                UpdateUI();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Example: save items to chosen path
                    try
                    {
                        FileManager.SaveAsXML(itemDTOs, Path.GetFileName(dialog.FileName), Path.GetDirectoryName(dialog.FileName) + @"\");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Save Items As failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    itemDTOs = FileManager.LoadItemDataXML(dialog.FileName);
                    _allItems = new List<dto.SerializableItemData>(itemDTOs);
                    UpdateItemInfo();
                    UpdateUI();
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SRInfoHelper.Log("Starting main Save operation...");

                // If we have an active item, save it first before saving collections
                if (activeItemData != null)
                {
                    SRInfoHelper.Log($"Saving current item {activeItemData.m_ID} before saving all data...");

                    // Save basic item information from UI
                    var name = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_NAME").FirstOrDefault();
                    if (name != null)
                        name.Element.m_Translations[activeLanguage] = ItemNameTextBox.Text;

                    var desc = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_DESCRIPTION").FirstOrDefault();
                    if (desc != null)
                        desc.Element.m_Translations[activeLanguage] = DescriptionTextBox.Text;

                    if (GearSubTypeDropDown.SelectedValue != null)
                    {
                        Enum.TryParse<ItemSubCategories>(GearSubTypeDropDown.SelectedValue.ToString(), out var m_GearSubCategory);
                        activeItemData.m_GearSubCategory = m_GearSubCategory;
                    }

                    if (WeaponTypeDropDown.SelectedValue != null)
                    {
                        Enum.TryParse<WeaponType>(WeaponTypeDropDown.SelectedValue.ToString(), out var m_WeaponType);
                        activeItemData.m_WeaponType = m_WeaponType;
                    }

                    if (ItemSlotTypeDropDown.SelectedValue != null)
                    {
                        Enum.TryParse<ItemSlotTypes>(ItemSlotTypeDropDown.SelectedValue.ToString(), out var slottype);
                        activeItemData.m_Slot = slottype;
                    }

                    // Save ALL tab fields with detailed logging
                    SaveCombatFields();
                    SaveResearchFields();
                    SaveEconomicFields();

                    // Update the item in both collections to ensure consistency
                    UpdateItemInCollections();
                }

                // Now save all data to XML files
                SRInfoHelper.Log("Saving all data to XML files...");
                FileManager.SaveAsXML(_translations, "Translations.xml", FileManager.ExecPath + @"\");
                FileManager.SaveAsXML(itemDTOs, _itemDataFileName, FileManager.ExecPath + @"\");

                SRInfoHelper.Log("Main Save operation completed successfully!");
                MessageBox.Show("All data saved successfully to XML files!\n\nBoth item data and translations have been persisted.", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Error in main Save operation: {ex}");
                MessageBox.Show($"Error saving data to XML: {ex.Message}\n\nCheck the logs for more details.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveWithDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var basePath = FileManager.ExecPath;
                var itemsPath = Path.Combine(basePath, _itemDataFileName);
                var transPath = Path.Combine(basePath, "Translations.xml");

                // Serialize current data to XML text
                var itemsXml = SerializeToXmlString(itemDTOs);
                var transXml = SerializeToXmlString(new SRMod.DTOs.TranslationsDTO { Translations = _translations });

                // Read on-disk versions
                var oldItems = XmlDiffUtil.ReadAllTextOrEmpty(itemsPath);
                var oldTrans = XmlDiffUtil.ReadAllTextOrEmpty(transPath);

                // Compute diffs
                var diffItems = XmlDiffUtil.Diff(oldItems, itemsXml);
                var diffTrans = XmlDiffUtil.Diff(oldTrans, transXml);

                var msg = _itemDataFileName + "\n" + diffItems + "\n\nTranslations.xml\n" + diffTrans;
                var dlg = DiffViewerForm.ShowDiff(this, "Confirm Save", msg);
                if (dlg != DialogResult.OK) return;

                // Backup existing files
                BackupIfExists(itemsPath);
                BackupIfExists(transPath);

                // Persist to disk
                FileManager.SaveAsXML(itemDTOs, _itemDataFileName, basePath + @"\");
                FileManager.SaveAsXML(_translations, "Translations.xml", basePath + @"\");

                MessageBox.Show(this, "Saved.", "Save (with diff)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save (with diff) failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void BackupIfExists(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var dir = Path.GetDirectoryName(path) ?? "";
                    var name = Path.GetFileName(path);
                    var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var backup = Path.Combine(dir, name + ".bak_" + stamp);
                    File.Copy(path, backup, overwrite: false);
                }
            }
            catch { }
        }

        private static string SerializeToXmlString<T>(T data)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, data, ns);
                return sw.ToString();
            }
        }

        private void loadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _translations = FileManager.LoadTranslationsXML("Translations.xml", FileManager.ExecPath + @"\");
            itemDTOs = FileManager.LoadItemDataXML(_itemDataFileName, FileManager.ExecPath + @"\");
            _allItems = new List<dto.SerializableItemData>(itemDTOs);
            UpdateItemInfo();
            UpdateUI();
        }

        private void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _translations = FileManager.LoadTranslationsXML(dialog.FileName);
                    UpdateItemInfo();
                    UpdateUI();
                }
            }
        }

        /// <summary>
        /// Refreshes UI and focuses on the newly copied item
        /// </summary>
        /// <param name="newItem">The newly copied item to focus on</param>
        private void RefreshUIAndFocusNewItem(dto.SerializableItemData newItem)
        {
            try
            {
                SRInfoHelper.Log($"Refreshing UI and focusing on item {newItem.m_ID}");

                // Clear any search filters to ensure new item is visible
                if (_toolbar?.SearchText != null && !string.IsNullOrEmpty(_toolbar.SearchText))
                {
                    SRInfoHelper.Log("Clearing search filter to show new item");
                    _toolbar.SearchText = "";
                    ApplySearch("");
                }

                // Refresh the item list
                UpdateItemInfo();

                // Find and select the new item
                FocusOnItem(newItem);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"RefreshUIAndFocusNewItem failed: {ex.Message}");
                // At minimum, ensure we have the right active item
                activeItemData = newItem;
                UpdateUI();
            }
        }

        /// <summary>
        /// Focuses on a specific item in the list
        /// </summary>
        /// <param name="targetItem">Item to focus on</param>
        private void FocusOnItem(dto.SerializableItemData targetItem)
        {
            try
            {
                SRInfoHelper.Log($"Attempting to focus on item {targetItem.m_ID}");

                // Get the current display list from the ListBox
                var displayItems = ItemListBox.DataSource as List<object>;
                if (displayItems != null)
                {
                    for (int i = 0; i < displayItems.Count; i++)
                    {
                        var listItem = displayItems[i];
                        var itemData = listItem.GetMemberValue("Value") as dto.SerializableItemData;
                        
                        if (itemData?.m_ID == targetItem.m_ID)
                        {
                            SRInfoHelper.Log($"Found item {targetItem.m_ID} at index {i}, selecting it");
                            
                            // Temporarily disable the selection event to prevent recursion
                            ItemListBox.SelectedIndexChanged -= ItemListBox_SelectedIndexChanged;
                            
                            // Select and scroll to the item
                            ItemListBox.SelectedIndex = i;
                            ItemListBox.TopIndex = Math.Max(0, i - 3); // Scroll to show item with context
                            
                            // Re-enable the selection event
                            ItemListBox.SelectedIndexChanged += ItemListBox_SelectedIndexChanged;
                            
                            // Update the active item and UI
                            activeItemData = targetItem;
                            UpdateUI();
                            
                            SRInfoHelper.Log($"Successfully focused on item {targetItem.m_ID}");
                            return;
                        }
                    }
                }
                
                // Fallback: if we couldn't find it in the list, still update activeItemData
                SRInfoHelper.Log($"Could not find item {targetItem.m_ID} in list, updating activeItemData anyway");
                activeItemData = targetItem;
                UpdateUI();
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"FocusOnItem failed: {ex.Message}");
                // At minimum, ensure we have the right active item
                activeItemData = targetItem;
                UpdateUI();
            }
        }

        /// <summary>
        /// Validates item data integrity
        /// </summary>
        /// <param name="item">Item to validate</param>
        /// <returns>True if item is valid</returns>
        private bool ValidateItemData(dto.SerializableItemData item)
        {
            if (item == null) 
            {
                SRInfoHelper.Log("Item validation failed: item is null");
                return false;
            }
            
            if (item.m_ID <= 0) 
            {
                SRInfoHelper.Log($"Item validation failed: invalid ID {item.m_ID}");
                return false;
            }
            
            // Check for duplicate IDs
            var existingItem = itemDTOs.FirstOrDefault(i => i != item && i.m_ID == item.m_ID);
            if (existingItem != null)
            {
                SRInfoHelper.Log($"Item validation failed: duplicate ID {item.m_ID}");
                return false;
            }
            
            SRInfoHelper.Log($"Item {item.m_ID} validation passed");
            return true;
        }

        private void ItemIconImageBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (activeItemData == null || activeItemData.m_ID <= 0)
                {
                    MessageBox.Show("Please select an item first.", "No Item Selected", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SRInfoHelper.Log($"Opening icon picker for item {activeItemData.m_ID}");

                // Open icon picker dialog
                using (var iconPicker = new IconPickerDialog(activeItemData.m_UIIconName))
                {
                    if (iconPicker.ShowDialog(this) == DialogResult.OK)
                    {
                        string newIconName = iconPicker.SelectedIconName;
                        
                        if (!string.IsNullOrEmpty(newIconName) && newIconName != activeItemData.m_UIIconName)
                        {
                            // Update the item's icon
                            activeItemData.m_UIIconName = newIconName;
                            
                            // Refresh the icon display
                            RefreshItemIcon();
                            
                            // Update the item in the collections
                            UpdateItemInCollections();
                            
                            SRInfoHelper.Log($"Item {activeItemData.m_ID} icon changed to: {newIconName}");
                            
                            // Show success message
                            MessageBox.Show($"Icon updated successfully!\n\nNew icon: {newIconName}", 
                                           "Icon Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to change icon: {ex.Message}\n\nPlease check the logs for more details.", 
                               "Icon Change Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SRInfoHelper.Log($"Icon change failed: {ex}");
            }
        }

        private void RefreshItemIcon()
        {
            try
            {
                if (activeItemData != null && !string.IsNullOrEmpty(activeItemData.m_UIIconName))
                {
                    var loadedImage = FileManager.LoadImageFromFile(activeItemData.m_UIIconName);
                    if (loadedImage != null)
                    {
                        ItemIconImageBox.Image = loadedImage;
                        ChangeImageColor(Color.White, Color.Aquamarine);
                    }
                    else
                    {
                        SRInfoHelper.Log($"Warning: Icon file '{activeItemData.m_UIIconName}' not found or could not be loaded");
                        ItemIconImageBox.Image = null;
                    }
                }
                else
                {
                    ItemIconImageBox.Image = null;
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Failed to refresh icon: {ex.Message}");
                ItemIconImageBox.Image = null;
            }
        }

        private void UpdateItemInCollections()
        {
            try
            {
                // Find and update the item in the main collection
                var existingItem = itemDTOs.FirstOrDefault(i => i.m_ID == activeItemData.m_ID);
                if (existingItem != null)
                {
                    CopyItemData(activeItemData, existingItem);
                }

                // Find and update in the filtered collection
                var existingFilteredItem = _allItems.FirstOrDefault(i => i.m_ID == activeItemData.m_ID);
                if (existingFilteredItem != null)
                {
                    CopyItemData(activeItemData, existingFilteredItem);
                }

                SRInfoHelper.Log($"Updated item {activeItemData.m_ID} in all collections with complete data");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Failed to update item in collections: {ex.Message}");
            }
        }

        /// <summary>
        /// Copies all data from source item to target item
        /// </summary>
        private void CopyItemData(dto.SerializableItemData source, dto.SerializableItemData target)
        {
            // Basic properties
            target.m_FriendlyName = source.m_FriendlyName;
            target.m_UIIconName = source.m_UIIconName;
            target.m_Slot = source.m_Slot;
            target.m_GearSubCategory = source.m_GearSubCategory;
            target.m_WeaponType = source.m_WeaponType;

            // Combat properties
            target.m_StealthVsCombat = source.m_StealthVsCombat;
            target.m_ValidWeaponAugmentationWeaponMask = source.m_ValidWeaponAugmentationWeaponMask;
            target.m_OverrideAmmo = source.m_OverrideAmmo;

            // Research properties
            target.m_PrereqID = source.m_PrereqID;
            target.m_PrototypeProgressionValue = source.m_PrototypeProgressionValue;
            target.m_BlueprintProgressionValue = source.m_BlueprintProgressionValue;
            target.m_MinResearchersRequired = source.m_MinResearchersRequired;
            target.m_AvailableToPlayer = source.m_AvailableToPlayer;
            target.m_PlayerStartsWithBlueprints = source.m_PlayerStartsWithBlueprints;
            target.m_PlayerStartsWithPrototype = source.m_PlayerStartsWithPrototype;
            target.m_PlayerCanResearchFromStart = source.m_PlayerCanResearchFromStart;

            // Economic properties
            target.m_Cost = source.m_Cost;
            target.m_BlueprintCost = source.m_BlueprintCost;
            target.m_PrototypeCost = source.m_PrototypeCost;
            target.m_FindBlueprintCost = source.m_FindBlueprintCost;
            target.m_FindPrototypeCost = source.m_FindPrototypeCost;

            // Copy collections
            target.m_Modifiers = source.m_Modifiers != null ?
                new List<dto.SerializableModifierData>(source.m_Modifiers) :
                new List<dto.SerializableModifierData>();
            target.m_AbilityIDs = source.m_AbilityIDs != null ?
                new List<int>(source.m_AbilityIDs) :
                new List<int>();
            target.m_AbilityMasks = source.m_AbilityMasks != null ?
                new List<int>(source.m_AbilityMasks) :
                new List<int>();
        }
    }
}
