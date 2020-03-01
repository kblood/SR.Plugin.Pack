using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace LoadCustomDataMod.DTOs
{
    [Serializable]
    public class ItemData
    {

        /*
        //Token: 0x1700038B RID: 907
        // (get) Token: 0x06002485 RID: 9349 RVA: 0x001161D8 File Offset: 0x001143D8
        public string m_LocName
        {
            get
            {
                return TextManager.GetLoc("ITEM_" + this.m_ID + "_NAME", true, false);
            }
        }

        // Token: 0x1700038E RID: 910
        // (get) Token: 0x06002491 RID: 9361 RVA: 0x001163E8 File Offset: 0x001145E8
        public string m_Description
        {
            get
            {
                return TextManager.GetLoc("ITEM_" + this.m_ID + "_DESCRIPTION", true, false);
            }
        }
        */

        /*
        // Token: 0x060024B0 RID: 9392 RVA: 0x00116680 File Offset: 0x00114880
        public string GetDescription(InfoManager iManager = null, bool prototypeWarning = true)
        {
            if (iManager == null)
            {
                iManager = Manager.GetInfoManager();
            }
            string text = string.Empty;
            string text2 = "\n\n";
            switch (this.m_Slot)
            {
                case ItemSlotTypes.AugmentationHead:
                    text = text + TextManager.GetLoc("AUGMENTATION_DESC_TYPE_PREFIX", true, false) + TextManager.FormatString(TextManager.GetLoc("AUGMENTATION_DESC_TYPE_HEAD", true, false), TextManager.FormatType.questTextHighlight) + text2;
                    break;
                case ItemSlotTypes.AugmentationBody:
                    text = text + TextManager.GetLoc("AUGMENTATION_DESC_TYPE_PREFIX", true, false) + TextManager.FormatString(TextManager.GetLoc("AUGMENTATION_DESC_TYPE_BODY", true, false), TextManager.FormatType.questTextHighlight) + text2;
                    break;
                case ItemSlotTypes.AugmentationArms:
                    text = text + TextManager.GetLoc("AUGMENTATION_DESC_TYPE_PREFIX", true, false) + TextManager.FormatString(TextManager.GetLoc("AUGMENTATION_DESC_TYPE_ARMS", true, false), TextManager.FormatType.questTextHighlight) + text2;
                    break;
                case ItemSlotTypes.AugmentationLegs:
                    text = text + TextManager.GetLoc("AUGMENTATION_DESC_TYPE_PREFIX", true, false) + TextManager.FormatString(TextManager.GetLoc("AUGMENTATION_DESC_TYPE_LEGS", true, false), TextManager.FormatType.questTextHighlight) + text2;
                    break;
            }
            text += this.m_Description;
            text += "\n";
            foreach (int id in this.m_AbilityIDs)
            {
                string description = Manager.GetAbilityManager().GetDescription(id);
                if (description.Length > 0)
                {
                    text = text + "\n" + description;
                }
            }
            string weaponStatDescription = Weapon.GetWeaponStatDescription(this);
            if (!string.IsNullOrEmpty(weaponStatDescription))
            {
                text = text + "\n" + weaponStatDescription;
            }
            if (this.IsPrototype() && prototypeWarning)
            {
                string text3 = text;
                text = string.Concat(new string[]
                {
                    text3,
                    "\n\n",
                    TextManager.GetFormatOpen(TextManager.FormatType.warning),
                    TextManager.GetLoc("WARNING_PREFIX", true, false),
                    TextManager.GetLoc("WARNING_EQUIP_PROTOTYPE_DESCRIPTION", true, false),
                    TextManager.GetFormatClose(TextManager.FormatType.warning)
                });
            }
            if (this.m_Modifiers != null && this.m_Modifiers.Length > 0)
            {
                text += "\n\n";
                for (int i = 0; i < this.m_Modifiers.Length; i++)
                {
                    text = text + "\n* " + iManager.GetDescription(this.m_Modifiers[i].m_Type, this.m_Modifiers[i].m_Ammount);
                }
            }
            text += "\n\n";
            text += string.Format(TextManager.Get().CurrentCultureInfo, TextManager.GetLoc("DESC_RESPAWN_TIME_ADDITION", true, false) + "\n", new object[]
            {
                this.GetCost(Manager.GetItemManager()) * 0.11f / 34f
            });
            text += string.Format(TextManager.Get().CurrentCultureInfo, TextManager.GetLoc("DESC_RESPAWN_COST_ADDITION", true, false) + "\n", new object[]
            {
                this.GetCost(Manager.GetItemManager()) * 0.11f
            });
            if (this.m_Slot == ItemSlotTypes.WeaponAugmentation)
            {
                ItemManager itemManager = Manager.GetItemManager();
                text = text + "\n\n" + TextManager.GetLoc("ITEMS_VALID_FOR", true, false) + "\n";
                for (int j = 0; j < 29; j++)
                {
                    if ((this.m_ValidWeaponAugmentationWeaponMask & 1 << j) > 0)
                    {
                        int itemIDFromWeaponType = itemManager.GetItemIDFromWeaponType((WeaponType)j);
                        ItemManager.ItemData itemData = itemManager.GetItemData(itemIDFromWeaponType);
                        text = text + " - " + ((!itemData.PlayerHasPrototype && !itemData.PlayerHasBlueprints) ? "???" : itemManager.GetItemData(itemIDFromWeaponType).m_LocName) + "\n";
                    }
                }
            }
            return TextManager.GenericFiltering(text);
        }

        */


        // Token: 0x04002012 RID: 8210
        public int m_ID { get; set; }

        // Token: 0x04002013 RID: 8211
        public int m_PrereqID { get; set; }

        // Token: 0x04002014 RID: 8212
        public ItemSlotTypes m_Slot{ get; set; }

        // Token: 0x04002015 RID: 8213
        public ItemSubCategories m_GearSubCategory{ get; set; }

        //// Token: 0x04002016 RID: 8214
        //public Sprite m_UIIcon{ get; set; }

        // Token: 0x04002017 RID: 8215
        public WeaponType m_WeaponType{ get; set; }

        // Token: 0x04002018 RID: 8216
        public int m_ValidWeaponAugmentationWeaponMask{ get; set; }

        // Token: 0x04002019 RID: 8217
        public int m_PrototypeProgressionValue { get; set; } = 10;

        // Token: 0x0400201A RID: 8218
        public int m_BlueprintProgressionValue { get; set; } = 20;

        // Token: 0x0400201B RID: 8219
        public float m_StealthVsCombat{ get; set; }

        // Token: 0x0400201C RID: 8220
        [XmlArray("Modifiers")]
        public ModifierData5L[] m_Modifiers{ get; set; }

        // Token: 0x0400201D RID: 8221
        [XmlArray("Abilities")]
        public List<int> m_AbilityIDs{ get; set; }

        [XmlArray("AbilityMasks")]
        // Token: 0x0400201E RID: 8222
        public List<int> m_AbilityMasks{ get; set; }

        // Token: 0x0400201F RID: 8223
        public bool m_AvailableToPlayer { get; set; } = true;

        // Token: 0x04002020 RID: 8224
        public bool m_AvailableFor_ALPHA_BETA_EARLYACCESS{ get; set; }

        // Token: 0x04002021 RID: 8225
        public bool m_PlayerStartsWithBlueprints{ get; set; }

        // Token: 0x04002022 RID: 8226
        public bool m_PlayerStartsWithPrototype{ get; set; }

        // Token: 0x04002023 RID: 8227
        public bool m_PlayerCanResearchFromStart{ get; set; }

        // Token: 0x04002024 RID: 8228
        public bool m_ResearchStarted{ get; set; }

        // Token: 0x04002025 RID: 8229
        public float m_BlueprintCost { get; set; } = 1f;

        // Token: 0x04002026 RID: 8230
        public float m_PrototypeCost { get; set; } = 1f;

        // Token: 0x04002027 RID: 8231
        public float m_FindBlueprintCost { get; set; } = 1f;

        // Token: 0x04002028 RID: 8232
        public float m_FindPrototypeCost { get; set; } = 1f;

        // Token: 0x04002029 RID: 8233
        public float m_Cost { get; set; } = 1f;

        // Token: 0x0400202A RID: 8234
        public bool m_ItemHasBeenLocated{ get; set; }

        // Token: 0x0400202B RID: 8235
        public int m_BlueprintRandomReleaseStage{ get; set; }

        // Token: 0x0400202C RID: 8236
        public int m_PrototypeRandomReleaseStage{ get; set; }

        // Token: 0x0400202D RID: 8237
        public float m_ResearchCost { get; set; } = 3000f;

        // Token: 0x0400202E RID: 8238
        protected float m_TotalResearchTime { get; set; } = -1f;

        // Token: 0x0400202F RID: 8239
        public float m_Progression { get; set; }

        // Token: 0x04002030 RID: 8240
        public int m_MinResearchersRequired { get; set; }

        // Token: 0x04002031 RID: 8241
        public int m_Count{ get; set; }

        // Token: 0x04002033 RID: 8243
        public bool m_PlayerHasPrototype{ get; set; }

        // Token: 0x04002034 RID: 8244
        public bool m_PlayerHasBlueprints{ get; set; }

        // Token: 0x04002035 RID: 8245
        public float m_ResearchProgress{ get; set; }

        // Token: 0x04002036 RID: 8246
        internal float m_ResearchTimeToDate{ get; set; }

        // Token: 0x04002037 RID: 8247
        internal float m_ResearchCostToDate{ get; set; }

        // Token: 0x04002038 RID: 8248
        public bool m_PrototypeIsInTheWorld{ get; set; }

        // Token: 0x04002039 RID: 8249
        public int m_InHouseResearchersResearching{ get; set; }

        // Token: 0x0400203A RID: 8250
        public int m_ExternalResearchersResearching{ get; set; }

        // Token: 0x0400203B RID: 8251
        public float m_ResearchProgressionPerSecond{ get; set; }

        // Token: 0x0400203C RID: 8252
        public float m_CurrentResearchCost{ get; set; }

        // Token: 0x0400203D RID: 8253
        public bool m_Expanded{ get; set; }

        // Token: 0x0400203E RID: 8254
        public int m_OverrideAmmo{ get; set; }

        public DTOs.Sprite m_UIIcon { get; set; }
    }

    [Serializable]
    public class ModifierData5L
    {
        // Token: 0x04001C3D RID: 7229
        public ModifierType m_Type { get; set; }

        // Token: 0x04001C3E RID: 7230
        public float m_Ammount { get; set; }

        // Token: 0x04001C3F RID: 7231
        public float m_TimeOut { get; set; }

        // Token: 0x04001C40 RID: 7232
        public ModifierType m_AmountModifier { get; set; }

        //// Token: 0x020003D8 RID: 984
        //public struct _ModifierData5L
        //{
        //    // Token: 0x04001C41 RID: 7233
        //    public byte m_Type;

        //    // Token: 0x04001C42 RID: 7234
        //    public byte m_AmountModifier;

        //    // Token: 0x04001C43 RID: 7235
        //    public float m_Amount;

        //    // Token: 0x04001C44 RID: 7236
        //    public byte[] m_TimeoutBytes;
        //}
    }
}
