using dto = SyndicateMod.DTOs;
using SyndicateMod.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TextManager;

namespace SatelliteReignModdingTools
{
    public partial class ItemBrowser : Form
    {
        public static List<dto.TranslationElementDTO> _translations = new List<dto.TranslationElementDTO>();
        public static List<dto.ItemData> itemDTOs = new List<dto.ItemData>();
        static List<Models.Ability> abilities = new List<Models.Ability>();
        static dto.ItemData activeItemData = new dto.ItemData();
        static dto.ModifierData5L activeModifier;

        public static int activeLanguage = 2;

        public ItemBrowser()
        {
            SRMapper.LanguageMapper();

            InitializeComponent();
            ItemListBox.ClearSelected();
            _translations = FileManager.LoadTranslationsXML("Translations.xml", FileManager.ExecPath).ToList();
            itemDTOs = FileManager.LoadItemDataXML("ItemData.xml", FileManager.ExecPath).OrderBy(i => i.m_ID).ToList();
            UpdateItemInfo();

            ItemSlotTypeDropDown.DataSource = Enum.GetValues(typeof(ItemSlotTypes)).Cast<ItemSlotTypes>().ToList().Take(8).ToList();
            GearSubTypeDropDown.DataSource = Enum.GetValues(typeof(ItemSubCategories)); //.Cast<ItemSubCategories>().ToList().Take(8).ToList();
            WeaponTypeDropDown.DataSource = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList().Take(29).ToList();
            
            abilities = _translations.Where(t => t.Key.StartsWith("ABILITY_") && t.Key.EndsWith("_NAME")).Select(a =>
            {
                int id = int.Parse(a.Key.Replace("ABILITY_", "").Replace("_NAME", ""));
                LocElement desc = null;
                if (_translations.Where(t => t.Key == "ABILITY_" + id + "_DESC").Any())
                    desc = _translations.Where(t => t.Key == "ABILITY_" + id + "_DESC").First().Element;
                return new Models.Ability()
                {
                    Id = id,
                    LocName = a.Element,
                    LocDesc = desc
                };
            }).ToList();

            AbilityDropdown.DataSource = abilities;
            AbilityDropdown.DisplayMember = "Name";
            AbilityDropdown.ValueMember = "Desc";
            ModifierDropdown.DataSource = Enum.GetValues(typeof(ModifierType)).Cast<ModifierType>().ToList().Skip(1).ToList();
            //MultiplierTypeDropDown.DataSource = Enum.GetValues(typeof(ModifierType)).Cast<ModifierType>().ToList();
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
            if(activeItemData.m_UIIcon != null)
            {
                ItemIconImageBox.Image = FileManager.LoadImageFromFile(activeItemData.m_UIIcon.textureName);
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
                abilityId => new
                {
                    //Key = abilityId + ": " + _translations.Where(t => t.Key == "ABILITY_" + abilityId + "_NAME").FirstOrDefault()?.Element?.m_Translations[activeLanguage],
                    //Name = _translations.Where(t => t.Key == "ABILITY_" + abilityId + "_NAME").FirstOrDefault()?.Element?.m_Translations[activeLanguage]
                    Key = abilityId + ": " + abilities.Where(a => a.Id == abilityId).FirstOrDefault()?.Name,
                    Name = abilities.Where(a => a.Id == abilityId).FirstOrDefault()?.Name,
                    Value = abilities.Where(a => a.Id == abilityId).FirstOrDefault()
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
            using (Bitmap bmp = new Bitmap(FileManager.LoadImageFromFile(activeItemData.m_UIIcon.textureName)))
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
                activeItemData = (dto.ItemData)ItemListBox.SelectedItem.GetMemberValue("Value");
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
                ExtraNameTextBox.Text = (string)AbilityListBox.SelectedItem.GetMemberValue("Name");
                ExtraDescriptionTextBox.Text = ((Models.Ability)AbilityListBox.SelectedItem.GetMemberValue("Value"))?.Desc;
                AbilityDropdown.SelectedItem = ((Models.Ability)AbilityListBox.SelectedItem.GetMemberValue("Value"));
                //UpdateUI();
            }
        }

        private void ModifierListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModifierListBox.SelectedItem != null)
            {
                ExtraNameTextBox.Text = ModifierListBox.SelectedItem.GetMemberValue("Key")?.ToString();
                ExtraDescriptionTextBox.Text = ModifierListBox.SelectedItem.GetMemberValue("Description")?.ToString();
                ModifierDropdown.SelectedItem = (ModifierType)ModifierListBox.SelectedItem.GetMemberValue("Enum");
                var modifier = (SyndicateMod.DTOs.ModifierData5L)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                AmountTextBox.Text = modifier.m_Ammount.ToString();
                //TimeOutTextBox.Text = modifier.m_TimeOut.ToString();
                //MultiplierTypeDropDown.SelectedItem = ModifierDropdown.SelectedItem = modifier.m_AmountModifier;
                //modifier.

                // Modifier
                //UpdateUI();
            }
        }

        private void CopyItemButton_Click(object sender, EventArgs e)
        {
            activeItemData = SREditor.CopyItem(activeItemData);
            UpdateItemInfo();
            ItemListBox.SelectedIndex = itemDTOs.IndexOf(activeItemData);
            //UpdateUI();
        }

        private void SaveItemButton_Click(object sender, EventArgs e)
        {
            var name =_translations.Where(t => t.Key == "ITEM_" + activeItemData.m_ID + "_NAME").First();
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
            if(a == ',')
                AmountTextBox.Text = Regex.Replace(AmountTextBox.Text, "[^-,0-9]", "");
            else
                AmountTextBox.Text = Regex.Replace(AmountTextBox.Text, "[^-.0-9]", "");

            if (Char.IsDigit(AmountTextBox.Text.First()) && Char.IsDigit(AmountTextBox.Text.Last()))
                if (float.TryParse(AmountTextBox.Text, out float amount))
                {
                    AmountTextBox.Text = amount.ToString();
                    oldAmountValue = amount;
                }
                else
                {
                    AmountTextBox.Text = oldAmountValue.ToString();
                }
            //AmountTextBox.Text = Regex.Replace(AmountTextBox.Text, "[^,.0-9]", "");
        }

        private void AddModiferButton_Click(object sender, EventArgs e)
        {
            var type = (ModifierType)ModifierDropdown.SelectedItem;
            if(!activeItemData.m_Modifiers.Select(m => m.m_Type).Contains(type))
            {
                var modifiers = activeItemData.m_Modifiers.ToList();
                var newMod = new SyndicateMod.DTOs.ModifierData5L() { m_Type = type };
                modifiers.Add(newMod);
                activeItemData.m_Modifiers = modifiers.ToArray();
                //UpdateUI();
                UpdateModifierInfo();

                ModifierListBox.SelectedIndex = activeItemData.m_Modifiers.Count() - 1;
                //ModifierListBox.SetSelected(ModifierListBox.SelectedIndex, true);
            }
        }

        private void SaveModifierButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(AmountTextBox.Text, out float amount))
            {
                AmountTextBox.Text = amount.ToString();
                oldAmountValue = amount;
                var modifier = (SyndicateMod.DTOs.ModifierData5L)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                modifier.m_Ammount = amount;
            }
            else
            {
                MessageBox.Show(oldAmountValue.ToString() + " is not a number", "Cannot save modifiers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //AmountTextBox.Text = oldAmountValue.ToString();
            }
        }

        private void DeleteModifierButton_Click(object sender, EventArgs e)
        {
            if(ModifierListBox.SelectedItem != null)
            {
                var modifier = (dto.ModifierData5L)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                var modifiers = activeItemData.m_Modifiers.ToList();
                modifiers.Remove(modifier);
                activeItemData.m_Modifiers = modifiers.ToArray();
                UpdateModifierInfo();
            }
            else
            {
                MessageBox.Show("No modifier selected that can be deleted", "Cannot delete modifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteAbilityButton_Click(object sender, EventArgs e)
        {
            if(AbilityListBox.SelectedItem != null)
            {
                var abil = ((Models.Ability)AbilityListBox.SelectedItem.GetMemberValue("Value"));

                if(abil != null)
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
            if(AbilityDropdown.SelectedItem != null)
            {
                Models.Ability ability = (Models.Ability)AbilityDropdown.SelectedItem;
                if(!activeItemData.m_AbilityIDs.Contains(ability.Id))
                {
                    activeItemData.m_AbilityIDs.Add(ability.Id);
                    
                    if(ability.Id == 1407)
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
            if(activeItemData.m_ID < 145)
            {
                messageText += @"
This item is one of the games original items, deleting it might cause problems for the game if you try to play the game without it.";
            }

            var result = MessageBox.Show(messageText, "Are you sure you want to delete this item?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

            if(result == DialogResult.Yes)
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
                    // Can use dialog.FileName
                    //using (Stream stream = dialog.OpenFile())
                    //{
                    //    // Save data
                    //}
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
            //dto.TranslationsDTO translationList = new dto.TranslationsDTO() { Translations = _translations };
            //FileManager.SaveAsXML(translationList, "Translations.xml", FileManager.ExecPath+ @"\");
            FileManager.SaveAsXML(_translations, "Translations.xml", FileManager.ExecPath + @"\");

            //dto.ItemDataList itemList = new dto.ItemDataList() { Items = itemDTOs };
            //FileManager.SaveAsXML(itemList, "ItemData.xml", FileManager.ExecPath + @"\");
            FileManager.SaveAsXML(itemDTOs, "ItemData.xml", FileManager.ExecPath + @"\");

            //var result = MessageBox.Show(FileManager.ExecPath, FileManager.ExecPath, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void loadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _translations = FileManager.LoadTranslationsXML("Translations.xml", FileManager.ExecPath + @"\");
            itemDTOs = FileManager.LoadItemDataXML("ItemData.xml", FileManager.ExecPath + @"\");
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
