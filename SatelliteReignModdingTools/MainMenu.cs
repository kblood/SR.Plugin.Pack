using System;
using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            try
            {
                InitializeComponent();

                // Test loading a game type to verify DLL loading works
                var test = typeof(ItemManager);

                // Continue with initialization...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}\n\n" +
                    "Please verify that the game path is correct.",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void btnItemBrowser_Click(object sender, EventArgs e)
        {
            var itemBrowser = new ItemBrowser();
            itemBrowser.Show();
        }

        private void btnEnemyBrowser_Click(object sender, EventArgs e)
        {
            //open enemy browser
            var itemBrowser = new EnemyBrowser();
            itemBrowser.Show();
        }

        private void btnWeaponBrowser_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Weapon Browser not yet implemented", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnMissionBrowser_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Mission Browser not yet implemented", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}