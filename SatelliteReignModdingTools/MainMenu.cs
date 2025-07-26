using System;
using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
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
            var missionBrowser = new MissionBrowser();
            missionBrowser.Show();
        }
    }
}