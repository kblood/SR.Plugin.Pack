using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using SatelliteReignModdingTools.Controls;
using SRMod.DTOs;

namespace SatelliteReignModdingTools
{
    public sealed class EconomyBrowser : Form
    {
        private readonly SplitContainer _split = new SplitContainer();
        private readonly ListBox _economyList = new ListBox();
        private readonly PropertyGrid _grid = new PropertyGrid();
        private SharedToolbar _toolbar;
        
        // Weapon data support
        private List<SerializableWeaponData> _weapons = new List<SerializableWeaponData>();
        private List<SerializableWeaponData> _filteredWeapons = new List<SerializableWeaponData>();
        private const string _weaponDataFileName = "weapons.xml";

        public EconomyBrowser()
        {
            Text = "Economy & Weapons";
            Width = 1000;
            Height = 700;
            BackColor = Color.Black;
            ForeColor = Color.Aquamarine;

            // Insert shared toolbar at the top
            _toolbar = new SharedToolbar();
            _toolbar.ReloadClicked += () => ReloadData();
            _toolbar.SaveClicked += () => SaveData(false);
            _toolbar.SaveWithDiffClicked += () => SaveData(true);
            _toolbar.ValidateClicked += () => ShowValidationResults();
            _toolbar.SearchTextChanged += text => ApplySearch(text);
            Controls.Add(_toolbar);

            _split.Dock = DockStyle.Fill;
            _split.Orientation = Orientation.Vertical;
            _split.SplitterDistance = 300;

            _economyList.BackColor = Color.SeaGreen;
            _economyList.ForeColor = Color.Aquamarine;
            _economyList.Dock = DockStyle.Fill;

            _grid.Dock = DockStyle.Fill;
            _grid.HelpVisible = false;

            var rightPanel = new Panel { Dock = DockStyle.Fill };
            rightPanel.Controls.Add(_grid);

            _split.Panel1.Controls.Add(_economyList);
            _split.Panel2.Controls.Add(rightPanel);
            Controls.Add(_split);

            Load += OnLoad;
            _economyList.SelectedIndexChanged += OnWeaponSelectionChanged;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void OnWeaponSelectionChanged(object sender, EventArgs e)
        {
            var selectedItem = _economyList.SelectedItem as WeaponListItem;
            if (selectedItem?.Weapon != null)
            {
                _grid.SelectedObject = selectedItem.Weapon;
            }
            else
            {
                _grid.SelectedObject = null;
            }
        }

        private void ReloadData()
        {
            try
            {
                LoadWeapons();
                UpdateWeaponList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWeapons()
        {
            _weapons.Clear();
            
            var filePath = Path.Combine(Environment.CurrentDirectory, _weaponDataFileName);
            if (!File.Exists(filePath))
            {
                _economyList.Items.Clear();
                _economyList.Items.Add($"No {_weaponDataFileName} found");
                _economyList.Items.Add($"Expected path: {filePath}");
                _economyList.Items.Add("Create weapons.xml from LoadCustomData export");
                return;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(List<SerializableWeaponData>));
                using (var reader = new FileStream(filePath, FileMode.Open))
                {
                    var loadedWeapons = (List<SerializableWeaponData>)serializer.Deserialize(reader);
                    if (loadedWeapons != null)
                    {
                        _weapons.AddRange(loadedWeapons);
                    }
                }
            }
            catch (Exception ex)
            {
                _economyList.Items.Clear();
                _economyList.Items.Add($"Error loading {_weaponDataFileName}: {ex.Message}");
            }
        }

        private void UpdateWeaponList()
        {
            _economyList.Items.Clear();
            _filteredWeapons = _weapons.ToList();
            
            if (_filteredWeapons.Count == 0)
            {
                _economyList.Items.Add("No weapons loaded");
                return;
            }

            foreach (var weapon in _filteredWeapons)
            {
                var displayName = $"Weapon {weapon.m_WeaponType}: {weapon.m_Name}";
                _economyList.Items.Add(new WeaponListItem { Weapon = weapon, DisplayName = displayName });
            }
            
            _economyList.DisplayMember = "DisplayName";
        }

        private class WeaponListItem
        {
            public SerializableWeaponData Weapon { get; set; }
            public string DisplayName { get; set; }
        }

        private void SaveData(bool showDiff)
        {
            try
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, _weaponDataFileName);
                var serializer = new XmlSerializer(typeof(List<SerializableWeaponData>));
                
                using (var writer = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(writer, _weapons);
                }
                
                MessageBox.Show(this, $"Weapons data saved to {_weaponDataFileName}", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                if (showDiff)
                {
                    MessageBox.Show(this, "Diff functionality not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error saving weapons data: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowValidationResults()
        {
            var validationResults = ValidateWeapons();
            MessageBox.Show(this, validationResults, "Weapon Validation Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string ValidateWeapons()
        {
            if (_weapons.Count == 0)
                return "No weapons loaded to validate.";

            var results = new List<string>();
            
            foreach (var weapon in _weapons)
            {
                var issues = new List<string>();
                
                if (string.IsNullOrEmpty(weapon.m_Name))
                    issues.Add("Missing name");
                    
                if (weapon.m_WeaponType < 0 || weapon.m_WeaponType > 30)
                    issues.Add($"Invalid weapon type: {weapon.m_WeaponType}");
                
                if (weapon.m_Ammo != null && weapon.m_Ammo.Count > 0)
                {
                    var firstAmmo = weapon.m_Ammo[0];
                    if (firstAmmo.m_damage_min < 0 || firstAmmo.m_damage_max < firstAmmo.m_damage_min)
                        issues.Add("Invalid damage values");
                        
                    if (firstAmmo.m_max_ammo <= 0)
                        issues.Add("Invalid magazine size");
                }

                if (issues.Count > 0)
                    results.Add($"Weapon {weapon.m_WeaponType} ({weapon.m_Name}): {string.Join(", ", issues)}");
            }
            
            return results.Count > 0 
                ? string.Join("\n", results) 
                : $"All {_weapons.Count} weapons passed validation.";
        }

        private void ApplySearch(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                UpdateWeaponList();
                return;
            }

            _economyList.Items.Clear();
            _filteredWeapons = _weapons.Where(w => 
                w.m_Name?.ToLower().Contains(text.ToLower()) == true ||
                w.m_WeaponType.ToString().Contains(text)).ToList();

            if (_filteredWeapons.Count == 0)
            {
                _economyList.Items.Add($"No weapons found matching '{text}'");
                return;
            }

            foreach (var weapon in _filteredWeapons)
            {
                var displayName = $"Weapon {weapon.m_WeaponType}: {weapon.m_Name}";
                _economyList.Items.Add(new WeaponListItem { Weapon = weapon, DisplayName = displayName });
            }
            
            _economyList.DisplayMember = "DisplayName";
        }
    }
}