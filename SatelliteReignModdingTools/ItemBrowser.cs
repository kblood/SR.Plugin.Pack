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
                        var iconPath = Path.Combine(FileManager.ExecPath, "icons", item.m_UIIconName + ".png");
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
                ItemIconImageBox.Image = FileManager.LoadImageFromFile(activeItemData.m_UIIconName);
                ChangeImageColor(Color.White, Color.Aquamarine);
            }

            UpdateAbilityInfo();
            UpdateModifierInfo();
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
            using (Graphics g = Graphics.FromImage(ItemIconImageBox.Image))
            using (Bitmap bmp = new Bitmap(FileManager.LoadImageFromFile(activeItemData.m_UIIconName)))
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
            activeItemData = new dto.SerializableItemData(SREditor.CopyItem(activeItemData.m_ID));
            UpdateItemInfo();
            ItemListBox.SelectedIndex = itemDTOs.IndexOf(activeItemData);
        }

        private void SaveItemButton_Click(object sender, EventArgs e)
        {
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
                var name = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_NAME").First();
                var desc = _translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_DESCRIPTION").First();
                _translations.Remove(name);
                _translations.Remove(desc);
                itemDTOs.Remove(activeItemData);
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
            FileManager.SaveAsXML(_translations, "Translations.xml", FileManager.ExecPath + @"\");
            FileManager.SaveAsXML(itemDTOs, _itemDataFileName, FileManager.ExecPath + @"\");
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
    }
}
