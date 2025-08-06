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

        private void btnQuestEditor_Click(object sender, EventArgs e)
        {
            var questBrowser = new QuestBrowser();
            questBrowser.Show();
        }

        private void btnTranslationsBrowser_Click(object sender, EventArgs e)
        {
            var translationsBrowser = new TranslationsBrowser();
            translationsBrowser.Show();
        }
    }
}