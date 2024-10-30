using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SRMod.DTOs;
using SRMod.Services;

namespace SatelliteReignModdingTools
{
    public partial class EnemyBrowser : Form
    {
        private static List<SerializableEnemyEntry> enemyDTOs = new List<SerializableEnemyEntry>();
        private static SerializableEnemyEntry activeEnemyData = new SerializableEnemyEntry();
        private static SerializableModifierData activeModifier;
        private const string _enemyDataFileName = "enemyentries.xml";
        // Add to the field declarations at the top of EnemyBrowser.cs
        private ListBox ItemListBox;
        private Button AddItemButton;
        private Button RemoveItemButton;
        // Add to field declarations at top of EnemyBrowser.cs
        private DataGridView CountKeysGrid;
        private Button AddCountKeyButton;
        private Button DeleteCountKeyButton;

        public EnemyBrowser()
        {
            InitializeComponent();
            // Add to constructor after InitializeComponent()
            if (ItemBrowser.itemDTOs == null || !ItemBrowser.itemDTOs.Any())
            {
                ItemBrowser.itemDTOs = FileManager.LoadItemDataXML("itemDefinitions.xml", FileManager.ExecPath).OrderBy(i => i.m_ID).ToList();
            }

            EnemyListBox.ClearSelected();

            // Load enemy definitions from XML file
            enemyDTOs = FileManager.LoadEnemyDataXML(_enemyDataFileName, FileManager.ExecPath).OrderBy(i => i.EnemyUID).ToList();
            UpdateEnemyInfo();

            // Initialize dropdown data sources
            GroupIDDropDown.DataSource = Enum.GetValues(typeof(GroupID));
            ModifierDropdown.DataSource = Enum.GetValues(typeof(ModifierType)).Cast<ModifierType>().ToList().Skip(1).ToList();
            WeaponTypeDropDown.DataSource = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();
            WardrobeTypeDropDown.DataSource = Enum.GetValues(typeof(WardrobeManager.WardrobeType));
        }

        private void UpdateItemList()
        {
            // Update equipped items list

            var equippedItems = (activeEnemyData.ItemIds ?? new List<int>())
                .Select((itemId, index) =>
                {
                    var item = ItemBrowser.itemDTOs.FirstOrDefault(i => i.m_ID == itemId);
                    return new
                    {
                        Index = index,
                        Key = $"{index + 1}: {item?.m_ID}: {item?.m_FriendlyName ?? "Unknown Item"}",
                        Value = itemId
                    };
                })
                .ToList();

            ItemListBox.DataSource = null;
            ItemListBox.DataSource = equippedItems;
            ItemListBox.DisplayMember = "Key";
            ItemListBox.ValueMember = "Value";

            // Update available items dropdown
            var availableItems = ItemBrowser.itemDTOs
                .OrderBy(i => i.m_ID)
                .Select(item => new
                {
                    Key = $"{item.m_ID}: {item.m_FriendlyName}",
                    Value = item
                })
                .ToList();

            ItemDropDown.DataSource = null;
            ItemDropDown.DataSource = availableItems;
            ItemDropDown.DisplayMember = "Key";
            ItemDropDown.ValueMember = "Value";
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            if (ItemDropDown.SelectedItem != null)
            {
                var selectedItem = (ItemDropDown.SelectedItem as dynamic).Value as SerializableItemData;
                if (activeEnemyData.ItemIds == null)
                    activeEnemyData.ItemIds = new List<int>();

                activeEnemyData.ItemIds.Add(selectedItem.m_ID);
                UpdateItemList();
            }
        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {
                int index = (ItemListBox.SelectedItem as dynamic).Index;
                if (activeEnemyData.ItemIds != null && index >= 0 && index < activeEnemyData.ItemIds.Count)
                {
                    activeEnemyData.ItemIds.RemoveAt(index);
                    UpdateItemList();
                }
            }
        }

        private void MoveItemUpButton_Click(object sender, EventArgs e)
        {
            if (ItemListBox.SelectedItem != null)
            {
                int index = (ItemListBox.SelectedItem as dynamic).Index;
                if (activeEnemyData.ItemIds != null && index > 0)
                {
                    // Swap with previous item
                    int temp = activeEnemyData.ItemIds[index];
                    activeEnemyData.ItemIds[index] = activeEnemyData.ItemIds[index - 1];
                    activeEnemyData.ItemIds[index - 1] = temp;

                    UpdateItemList();
                    ItemListBox.SelectedIndex = index - 1; // Keep the moved item selected
                }
            }
        }

        private void UpdateUI()
        {
            // Existing UI updates...
            EnemyNameTextBox.Text = activeEnemyData.EnemyName;
            GroupIDDropDown.SelectedItem = activeEnemyData.GroupId;
            WardrobeTypeDropDown.SelectedItem = activeEnemyData.WardrobeOverride;
            WeaponTypeDropDown.SelectedItem = activeEnemyData.WeaponOverrides?.FirstOrDefault() ?? WeaponType.None;

            SpawnableCheckbox.Checked = activeEnemyData.Spawnable;
            SoloCheckbox.Checked = activeEnemyData.Solo;
            UseWardrobeOverrideCheckbox.Checked = activeEnemyData.UseWardrobeOverride;

            UpdateItemList();
            UpdateModifierInfo();
            UpdateCountKeysGrid();
        }

        private void AddCountKeyButton_Click(object sender, EventArgs e)
        {
            if (activeEnemyData.m_CountKeys == null)
                activeEnemyData.m_CountKeys = new List<EnemyCountKey>();

            activeEnemyData.m_CountKeys.Add(new EnemyCountKey
            {
                m_Count = 1,
                m_Progression = 0.0f
            });

            UpdateCountKeysGrid();
        }

        private void DeleteCountKeyButton_Click(object sender, EventArgs e)
        {
            if (CountKeysGrid.CurrentRow != null &&
                activeEnemyData.m_CountKeys != null &&
                activeEnemyData.m_CountKeys.Count > 0 &&
                CountKeysGrid.CurrentRow.Index < activeEnemyData.m_CountKeys.Count)
            {
                try
                {
                    int index = CountKeysGrid.CurrentRow.Index;
                    activeEnemyData.m_CountKeys.RemoveAt(index);
                    UpdateCountKeysGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting count key: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CountKeysGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (activeEnemyData.m_CountKeys == null || CountKeysGrid.CurrentRow == null)
                return;

            int index = e.RowIndex;
            string newValue = e.FormattedValue.ToString();

            if (e.ColumnIndex == 0) // Count column
            {
                if (int.TryParse(newValue, out int count))
                {
                    activeEnemyData.m_CountKeys[index].m_Count = count;
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show("Please enter a valid number for Count", "Invalid Value",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (e.ColumnIndex == 1) // Progression column
            {
                if (float.TryParse(newValue, out float progression))
                {
                    activeEnemyData.m_CountKeys[index].m_Progression = progression;
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show("Please enter a valid number for Progression", "Invalid Value",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void SaveEnemyButton_Click(object sender, EventArgs e)
        {
            activeEnemyData.EnemyName = EnemyNameTextBox.Text;
            activeEnemyData.GroupId = (GroupID)GroupIDDropDown.SelectedItem;
            activeEnemyData.WardrobeOverride = (WardrobeManager.WardrobeType)WardrobeTypeDropDown.SelectedItem;
            activeEnemyData.Spawnable = SpawnableCheckbox.Checked;
            activeEnemyData.Solo = SoloCheckbox.Checked;
            activeEnemyData.UseWardrobeOverride = UseWardrobeOverrideCheckbox.Checked;

            var selectedWeaponType = (WeaponType)WeaponTypeDropDown.SelectedItem;
            if (selectedWeaponType != WeaponType.None)
            {
                if (activeEnemyData.WeaponOverrides == null)
                    activeEnemyData.WeaponOverrides = new List<WeaponType>();
                if (!activeEnemyData.WeaponOverrides.Contains(selectedWeaponType))
                {
                    activeEnemyData.WeaponOverrides.Clear();
                    activeEnemyData.WeaponOverrides.Add(selectedWeaponType);
                }
            }

            // County Keys are automatically saved as they're edited in the grid
            // The grid updates the actual objects in m_CountKeys directly

            UpdateEnemyInfo();
            int currentIndex = EnemyListBox.SelectedIndex;
            UpdateUI();
            EnemyListBox.SelectedIndex = currentIndex;
        }

        private void UpdateEnemyInfo()
        {
            var enemies = enemyDTOs.Select(
                enemy => new
                {
                    Key = $"{enemy.EnemyUID}: {enemy.EnemyName}",
                    Value = enemy
                }
            ).ToList();

            EnemyListBox.SelectedIndexChanged -= EnemyListBox_SelectedIndexChanged;
            EnemyListBox.DataSource = enemies;
            EnemyListBox.DisplayMember = "Key";
            EnemyListBox.ValueMember = "Value";
            EnemyListBox.ClearSelected();
            EnemyListBox.SelectedIndexChanged += EnemyListBox_SelectedIndexChanged;

            UpdateModifierInfo();
        }

        private void UpdateModifierInfo()
        {
            var modifiers = activeEnemyData.Modifiers?.Select(
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

        private void EnemyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EnemyListBox.SelectedItem != null)
            {
                activeEnemyData = (SerializableEnemyEntry)EnemyListBox.SelectedItem.GetMemberValue("Value");
                UpdateUI();
            }
        }

        private void ModifierListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModifierListBox.SelectedItem != null)
            {
                ModifierNameTextBox.Text = ModifierListBox.SelectedItem.GetMemberValue("Key")?.ToString();
                ModifierDescriptionTextBox.Text = ModifierListBox.SelectedItem.GetMemberValue("Description")?.ToString();
                ModifierDropdown.SelectedItem = (ModifierType)ModifierListBox.SelectedItem.GetMemberValue("Enum");
                activeModifier = (SerializableModifierData)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                AmountTextBox.Text = activeModifier.m_Ammount.ToString();
            }
        }

        private void CopyEnemyButton_Click(object sender, EventArgs e)
        {
            var newEnemy = new SerializableEnemyEntry();
            newEnemy.EnemyUID = enemyDTOs.Max(nmy => nmy.EnemyUID) + 1;
            newEnemy.EnemyName = activeEnemyData.EnemyName + " (Copy)";
            newEnemy.GroupId = activeEnemyData.GroupId;
            newEnemy.Spawnable = activeEnemyData.Spawnable;
            newEnemy.Solo = activeEnemyData.Solo;
            newEnemy.UseWardrobeOverride = activeEnemyData.UseWardrobeOverride;
            newEnemy.WardrobeOverride = activeEnemyData.WardrobeOverride;
            newEnemy.WeaponOverrides = new List<WeaponType>(activeEnemyData.WeaponOverrides ?? new List<WeaponType>());
            newEnemy.Modifiers = activeEnemyData.Modifiers?.Select(m => new SerializableModifierData
            {
                m_Type = m.m_Type,
                m_Ammount = m.m_Ammount,
                m_AmountModifier = m.m_AmountModifier,
                m_TimeOut = m.m_TimeOut
            }).ToList() ?? new List<SerializableModifierData>();

            enemyDTOs.Add(newEnemy);
            UpdateEnemyInfo();
            EnemyListBox.SelectedIndex = enemyDTOs.Count - 1;
        }

        private void DeleteEnemyButton_Click(object sender, EventArgs e)
        {
            if (activeEnemyData != null)
            {
                string message = $"Are you sure you want to delete enemy {activeEnemyData.EnemyName} (UID: {activeEnemyData.EnemyUID})?";
                var result = MessageBox.Show(message, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    enemyDTOs.Remove(activeEnemyData);
                    UpdateEnemyInfo();
                    if (enemyDTOs.Any())
                        EnemyListBox.SelectedIndex = 0;
                }
            }
        }

        private void AddModifierButton_Click(object sender, EventArgs e)
        {
            var type = (ModifierType)ModifierDropdown.SelectedItem;
            if (activeEnemyData.Modifiers == null)
                activeEnemyData.Modifiers = new List<SerializableModifierData>();

            if (!activeEnemyData.Modifiers.Any(m => m.m_Type == type))
            {
                var newMod = new SerializableModifierData { m_Type = type };
                activeEnemyData.Modifiers.Add(newMod);
                UpdateModifierInfo();
                ModifierListBox.SelectedIndex = activeEnemyData.Modifiers.Count - 1;
            }
        }

        private void SaveModifierButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(AmountTextBox.Text, out float amount))
            {
                activeModifier.m_Ammount = amount;
                UpdateModifierInfo();
            }
            else
            {
                MessageBox.Show("Invalid amount value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteModifierButton_Click(object sender, EventArgs e)
        {
            if (ModifierListBox.SelectedItem != null)
            {
                var modifier = (SerializableModifierData)ModifierListBox.SelectedItem.GetMemberValue("Modifier");
                activeEnemyData.Modifiers.Remove(modifier);
                UpdateModifierInfo();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileManager.SaveAsXML(enemyDTOs, _enemyDataFileName, FileManager.ExecPath + @"\");
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    enemyDTOs = FileManager.LoadEnemyDataXML(dialog.FileName);
                    UpdateEnemyInfo();
                    UpdateUI();
                }
            }
        }

        private class CountKeyDisplay
        {
            public int Count { get; set; }
            public float Progression { get; set; }
        }

        private void UpdateCountKeysGrid()
        {
            var countKeys = activeEnemyData.m_CountKeys?
                .Select(ck => new CountKeyDisplay
                {
                    Count = ck.m_Count,
                    Progression = ck.m_Progression
                })
                .ToList() ?? new List<CountKeyDisplay>();

            CountKeysGrid.DataSource = null;
            CountKeysGrid.DataSource = countKeys;
        }
    }
}