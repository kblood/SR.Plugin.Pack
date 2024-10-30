using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteReignModdingTools.Models
{
    public class Ability
    {
        public int Id { get; set; }
        //public int MaskId { get; set; }
        public string Name { get { return LocName?.m_Translations[ItemBrowser.activeLanguage]; } }
        public string Desc
        {
            get
            {
                if (LocDesc == null)
                    return "";
                else
                    return LocDesc.m_Translations[ItemBrowser.activeLanguage];
            }
        }
        public Ability This { get { return this; } }
        public TextManager.LocElement LocName { get; set; }
        public TextManager.LocElement LocDesc { get; set; }
    }
}
