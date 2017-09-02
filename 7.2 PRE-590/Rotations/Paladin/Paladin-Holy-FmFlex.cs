// Email: xxx@gmail.com
using System.Drawing;
using System.Windows.Forms;
using CloudMagic.Helpers;
using System;
using System.Threading;

namespace CloudMagic.Rotation
{
    public class HolyPalFmFlex : CombatRoutine
    {
        private bool AddonEdited = true;
        private bool DEBUG = false;

        private bool useBeaconOfLight = true;
        private static class SPELL
        {
            public const string BEACON_OF_LIGHT = "Beacon of Light";
            public const string BEACON_OF_VIRTUE = "Beacon of Virtue";
            public const string BEACON_OF_FAITH = "Beacon of Faith";
            public const string HOLY_LIGHT = "Holy Light";
            public const string HOLY_SHOCK = "Holy Shock";
            public const string DIVINE_SHIELD = "Divine Shield";
            public const string DIVINE_PROTECTION = "Divine Protection";
            public const string LIGHT_OF_THE_MARTYR = "Light of the Martyr";
            public const string BESTOW_FAITH = "Bestow Faith";
            public const string JUDGMENT = "Judgment";
            public const string FORBEARANCE = "Forbearance";
            public const string LAY_ON_HANDS = "Lay on Hands";
            public const string BLESSING_OF_SACRIFICE = "Blessing of Sacrifice";
            public const string FLASH_OF_LIGHT = "Flash of Light";
            public const string CRUSADER_STRIKE = "Crusader Strike";
            public const string CONSECRATION = "Consecration";
            public const string TYRS_DELIVERANCE = "Tyrs Deliverance";
            public const string BLESSING_OF_PROTECTION = "Blessing of Protection";
            public const string LIGHT_OF_DAWN = "Light of Dawn";
            public const string AURA_MASTERY = "Aura Mastery";
            public const string STOP_CASTING = "stopcasting";
            public const string CLEANSE = "cleanse";
        }

        // Settings Default PARAMETERS
        private const int BlessingofProtectionDefaultHp = 40;
        private const int BlessingofProtectionHp = 40;

        private const int minHealingDefaultHP = 90;
        private const int minTankPriorityDefaultHP = 30;
        private const int minSelfPriorityDefaultHP = 20;

        private const int FlashofLightDefaultHp = 70;
        private const int HolyLightDefaultHp = 80;
        private const int HolyShockDefaultHp = 90;
        private const int LightoftheMartyrDefaultHp = 40;
        private const int DivineShieldDefaultHp = 40;

        private const int BlessingofSacrificeDefaultHp = 40;
        private const int LayonHandsDefaultHp = 20;
        private const int BestowFaithDefaultHp = 80;
        private const int TyrsDeliveranceDefaultHP = 80;
        private const int BeaconofVirtueDefaultHP = 80;
        private const int AuraMasterDefaultHP = 60;
        private const int LightofDawnDefaultHP = 80;
        private const int BeaconofVirtueDefaultLMR = 8;
        private const int AuraMasterDefaultLMR = 10;
        private const int LightofDawnDefaultLMR = 8;
        private const int BeaconofVirtueDefaultLMP = 3;
        private const int AuraMasteryDefaultLMP = 4;
        private const int LightofDawnDefaultLMP = 3;


        // TALENT PARAMETERS
        #region Talent Parameters
        private static bool isTalentBestowFaith
        {
            get
            {
                var isTalentBestowFaith = ConfigFile.ReadValue("Hpaladin", "isTalentBestowFaith").Trim();

                if (isTalentBestowFaith != "")
                {
                    return Convert.ToBoolean(isTalentBestowFaith);
                }

                return false;
            }
            set { ConfigFile.WriteValue("Hpaladin", "isTalentBestowFaith", value.ToString()); }
        }
        private static bool isTalentJoL
        {
            get
            {
                var isTalentJoL = ConfigFile.ReadValue("Hpaladin", "isTalentJoL").Trim();

                if (isTalentJoL != "")
                {
                    return Convert.ToBoolean(isTalentJoL);
                }

                return false;
            }
            set { ConfigFile.WriteValue("Hpaladin", "isTalentJoL", value.ToString()); }
        }
        private static bool isTalentBoF
        {
            get
            {
                var isTalentBoF = ConfigFile.ReadValue("Hpaladin", "isTalentBoF").Trim();

                if (isTalentBoF != "")
                {
                    return Convert.ToBoolean(isTalentBoF);
                }

                return false;
            }
            set { ConfigFile.WriteValue("Hpaladin", "isTalentBoF", value.ToString()); }
        }
        private static bool isTalentBoV
        {
            get
            {
                var isTalentBoV = ConfigFile.ReadValue("Hpaladin", "isTalentBoV").Trim();

                if (isTalentBoV != "")
                {
                    return Convert.ToBoolean(isTalentBoV);
                }

                return false;
            }
            set { ConfigFile.WriteValue("Hpaladin", "isTalentBoV", value.ToString()); }
        }
        #endregion
        // HEALING PARAMETERS
        #region Healing Parameters
        private static int minHealingHP
        {
            get
            {
                var minHealingHP = ConfigFile.ReadValue("Hpaladin", "minHealingHP").Trim();
                if (minHealingHP != "")
                {
                    return Convert.ToInt32(minHealingHP);
                }

                return minHealingDefaultHP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "minHealingHP", value.ToString()); }
        }
        private static int minTankPriorityHP
        {
            get
            {
                var minTankPriorityHP = ConfigFile.ReadValue("Hpaladin", "minTankPriorityHP").Trim();
                if (minTankPriorityHP != "")
                {
                    return Convert.ToInt32(minTankPriorityHP);
                }

                return minTankPriorityDefaultHP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "minTankPriorityHP", value.ToString()); }
        }
        private static int minSelfPriorityHP
        {
            get
            {
                var minSelfPriorityHP = ConfigFile.ReadValue("Hpaladin", "minSelfPriorityHP").Trim();
                if (minSelfPriorityHP != "")
                {
                    return Convert.ToInt32(minSelfPriorityHP);
                }

                return minSelfPriorityDefaultHP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "minSelfPriorityHP", value.ToString()); }
        }
        private static int FlashofLightHp
        {
            get
            {
                var FlashofLightHp = ConfigFile.ReadValue("Hpaladin", "FlashofLightHp").Trim();
                if (FlashofLightHp != "")
                {
                    return Convert.ToInt32(FlashofLightHp);
                }

                return FlashofLightDefaultHp;
            }
            set { ConfigFile.WriteValue("Hpaladin", "FlashofLightHp", value.ToString()); }
        }
        private static int HolyLightHp
        {
            get
            {
                var HolyLightHp = ConfigFile.ReadValue("Hpaladin", "HolyLightHp").Trim();
                if (HolyLightHp != "")
                {
                    return Convert.ToInt32(HolyLightHp);
                }

                return HolyLightDefaultHp;
            }
            set { ConfigFile.WriteValue("Hpaladin", "HolyLightHp", value.ToString()); }
        }
        private static int HolyShockHp
        {
            get
            {
                var HolyShockHp = ConfigFile.ReadValue("Hpaladin", "HolyShockHp").Trim();
                if (HolyShockHp != "")
                {
                    return Convert.ToInt32(HolyShockHp);
                }

                return HolyShockDefaultHp;
            }
            set { ConfigFile.WriteValue("Hpaladin", "HolyShockHp", value.ToString()); }
        }
        private static int LightoftheMartyrHp
        {
            get
            {
                var LightoftheMartyrHp = ConfigFile.ReadValue("Hpaladin", "LightoftheMartyrHp").Trim();
                if (LightoftheMartyrHp != "")
                {
                    return Convert.ToInt32(LightoftheMartyrHp);
                }

                return LightoftheMartyrDefaultHp;
            }
            set { ConfigFile.WriteValue("Hpaladin", "LightoftheMartyrHp", value.ToString()); }
        }
        private static int BestowFaithHp
        {
            get
            {
                var BestowFaithHp = ConfigFile.ReadValue("Hpaladin", "BestowFaithHp").Trim();
                if (BestowFaithHp != "")
                {
                    return Convert.ToInt32(BestowFaithHp);
                }

                return BestowFaithDefaultHp;
            }
            set { ConfigFile.WriteValue("Hpaladin", "BestowFaithHp", value.ToString()); }
        }

        private static int BlessingofSacrificeHp
        {
            get
            {
                var BlessingofSacrificeHp = ConfigFile.ReadValue("Hpaladin", "BlessingofSacrificeHp").Trim();
                if (BlessingofSacrificeHp != "")
                {
                    return Convert.ToInt32(BlessingofSacrificeHp);
                }

                return BlessingofSacrificeDefaultHp;
            }
            set { ConfigFile.WriteValue("Hpaladin", "BlessingofSacrificeHp", value.ToString()); }
        }
        private static int LayonHandsHp
        {
            get
            {
                var LayonHandsHp = ConfigFile.ReadValue("Hpaladin", "LayonHandsHp").Trim();
                if (LayonHandsHp != "")
                {
                    return Convert.ToInt32(LayonHandsHp);
                }

                return LayonHandsDefaultHp;
            }
            set { ConfigFile.WriteValue("Hpaladin", "LayonHandsHp", value.ToString()); }
        }
        private static int BeaconOfVirtueHp
        {
            get
            {
                var BeaconOfVirtueHp = ConfigFile.ReadValue("Hpaladin", "BeaconOfVirtueHp").Trim();
                if (BeaconOfVirtueHp != "")
                {
                    return Convert.ToInt32(BeaconOfVirtueHp);
                }

                return BeaconofVirtueDefaultHP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "BeaconOfVirtueHp", value.ToString()); }
        }
        private static int TyrsDeliveranceHP
        {
            get
            {
                var TyrsDeliveranceHP = ConfigFile.ReadValue("Hpaladin", "TyrsDeliveranceHP").Trim();
                if (TyrsDeliveranceHP != "")
                {
                    return Convert.ToInt32(TyrsDeliveranceHP);
                }

                return TyrsDeliveranceDefaultHP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "TyrsDeliveranceHP", value.ToString()); }
        }
        private static int AuraMasteryHP
        {
            get
            {
                var AuraMasteryHP = ConfigFile.ReadValue("Hpaladin", "AuraMasteryHP").Trim();
                if (AuraMasteryHP != "")
                {
                    return Convert.ToInt32(AuraMasteryHP);
                }

                return AuraMasterDefaultHP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "AuraMasteryHP", value.ToString()); }
        }
        private static int LightofDawnHP
        {
            get
            {
                var LightofDawnHP = ConfigFile.ReadValue("Hpaladin", "LightofDawnHP").Trim();
                if (LightofDawnHP != "")
                {
                    return Convert.ToInt32(LightofDawnHP);
                }

                return LightofDawnDefaultHP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "LightofDawnLMR", value.ToString()); }
        }
        private static int BeaconofVirtueLMR
        {
            get
            {
                var BeaconofVirtueLMR = ConfigFile.ReadValue("Hpaladin", "BeaconofVirtueLMR").Trim();
                if (BeaconofVirtueLMR != "")
                {
                    return Convert.ToInt32(BeaconofVirtueLMR);
                }

                return BeaconofVirtueDefaultLMR;
            }
            set { ConfigFile.WriteValue("Hpaladin", "BeaconofVirtueLMR", value.ToString()); }
        }
        private static int TyrsDeliveranceLMR
        {
            get
            {
                var TyrsDeliveranceLMR = ConfigFile.ReadValue("Hpaladin", "TyrsDeliveranceLMR").Trim();
                if (TyrsDeliveranceLMR != "")
                {
                    return Convert.ToInt32(TyrsDeliveranceLMR);
                }

                return BeaconofVirtueDefaultLMR;
            }
            set { ConfigFile.WriteValue("Hpaladin", "TyrsDeliveranceLMR", value.ToString()); }
        }
        private static int AuraMasteryLMR
        {
            get
            {
                var AuraMasteryLMR = ConfigFile.ReadValue("Hpaladin", "AuraMasteryLMR").Trim();
                if (AuraMasteryLMR != "")
                {
                    return Convert.ToInt32(AuraMasteryLMR);
                }

                return AuraMasterDefaultLMR;
            }
            set { ConfigFile.WriteValue("Hpaladin", "AuraMasteryLMR", value.ToString()); }
        }
        private static int LightofDawnLMR
        {
            get
            {
                var LightofDawnLMR = ConfigFile.ReadValue("Hpaladin", "LightofDawnLMR").Trim();
                if (LightofDawnLMR != "")
                {
                    return Convert.ToInt32(LightofDawnLMR);
                }

                return LightofDawnDefaultLMR;
            }
            set { ConfigFile.WriteValue("Hpaladin", "LightofDawnLMR", value.ToString()); }
        }
        private static int BeaconofVirtueLMP
        {
            get
            {
                var BeaconofVirtueLMP = ConfigFile.ReadValue("Hpaladin", "BeaconofVirtueLMP").Trim();
                if (BeaconofVirtueLMP != "")
                {
                    return Convert.ToInt32(BeaconofVirtueLMP);
                }

                return BeaconofVirtueDefaultLMP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "BeaconofVirtueLMP", value.ToString()); }
        }
        private static int TyrsDeliveranceLMP
        {
            get
            {
                var TyrsDeliveranceLMP = ConfigFile.ReadValue("Hpaladin", "TyrsDeliveranceLMP").Trim();
                if (TyrsDeliveranceLMP != "")
                {
                    return Convert.ToInt32(TyrsDeliveranceLMP);
                }

                return BeaconofVirtueDefaultLMP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "TyrsDeliveranceLMP", value.ToString()); }
        }
        private static int AuraMasteryLMP
        {
            get
            {
                var AuraMasteryLMP = ConfigFile.ReadValue("Hpaladin", "AuraMasteryLMP").Trim();
                if (AuraMasteryLMP != "")
                {
                    return Convert.ToInt32(AuraMasteryLMP);
                }

                return AuraMasteryDefaultLMP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "AuraMasteryLMP", value.ToString()); }
        }
        private static int LightofDawnLMP
        {
            get
            {
                var LightofDawnLMP = ConfigFile.ReadValue("Hpaladin", "LightofDawnLMP").Trim();
                if (LightofDawnLMP != "")
                {
                    return Convert.ToInt32(LightofDawnLMP);
                }

                return LightofDawnDefaultLMP;
            }
            set { ConfigFile.WriteValue("Hpaladin", "LightofDawnLMP", value.ToString()); }
        }
        private static bool TyrsDeliveranceEnabled
        {
            get
            {
                var TyrsDeliveranceEnabled = ConfigFile.ReadValue("Hpaladin", "TyrsDeliveranceEnabled").Trim();

                if (TyrsDeliveranceEnabled != "")
                {
                    return Convert.ToBoolean(TyrsDeliveranceEnabled);
                }

                return false;
            }
            set { ConfigFile.WriteValue("Hpaladin", "TyrsDeliveranceEnabled", value.ToString()); }
        }
        private static bool AuraMasteryEnabled
        {
            get
            {
                var AuraMasteryEnabled = ConfigFile.ReadValue("Hpaladin", "AuraMasteryEnabled").Trim();

                if (AuraMasteryEnabled != "")
                {
                    return Convert.ToBoolean(AuraMasteryEnabled);
                }

                return false;
            }
            set { ConfigFile.WriteValue("Hpaladin", "AuraMasteryEnabled", value.ToString()); }
        }
        private static bool LightofDawnEnabled
        {
            get
            {
                var LightofDawnEnabled = ConfigFile.ReadValue("Hpaladin", "LightofDawnEnabled").Trim();

                if (LightofDawnEnabled != "")
                {
                    return Convert.ToBoolean(LightofDawnEnabled);
                }

                return false;
            }
            set { ConfigFile.WriteValue("Hpaladin", "LightofDawnEnabled", value.ToString()); }
        }
        #endregion
        // DO NOT TOUCH THESE PARAMETERS
        private int targetPartyID = -1;
        private int targetHealth = minHealingHP;
        private int tankPartyID = -1;
        private int tankPartyID2 = -1;
        private int playerRaidID = -1;
        private int currentHealth = -1;
        private int tankTankingID = -1;
        private int TankRaidCurrentBOL = -1;
        private int partyMemberLow = 0;
        private int partyMemberReallyLow = 0;
        private bool isInRaid = false;
        private int groupSize = 0;
        private int currentMana = -1;
        private int lastCastedID = -1;
        private int targetDispelPartyID = -1;

        public override string Name
        {
            get
            {
                return "Holy Pal by FmFlex";
            }
        }
        public override string Class
        {
            get
            {
                return "Paladin";
            }
        }
        public override Form SettingsForm { get; set; }
        public SettingsFormHPF SettingsFormHPF { get; set; }

        public override void Initialize()
        {
            Log.Write("Welcome to Holy Pal V2.0 by FmFlex", Color.Green);
            Log.Write("If you want to offer me a beer, Here is my Paypal donation link", Color.Green);
            Log.Write(" https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=SH8GVLUVFD6M4&lc=BE&item_name=FmFlex%20Rotation&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted", Color.Green);
            #region GU variable setting
            SettingsFormHPF = new SettingsFormHPF();
            SettingsForm = SettingsFormHPF;

            SettingsFormHPF.BestowFaith.Checked = isTalentBestowFaith;
            SettingsFormHPF.BestowFaith.CheckedChanged += BestowFaith_CheckedChanged;
            SettingsFormHPF.JudgmentOfLight.Checked = isTalentJoL;
            SettingsFormHPF.JudgmentOfLight.CheckedChanged += JudgmentOfLight_CheckedChanged;
            SettingsFormHPF.BeaconofFaith.Checked = isTalentBoF;
            SettingsFormHPF.BeaconofFaith.CheckedChanged += BeaconofFaith_CheckedChanged;
            SettingsFormHPF.BeaconofVirtue.Checked = isTalentBoV;
            SettingsFormHPF.BeaconofVirtue.CheckedChanged += BeaconofVirtue_CheckedChanged;

            SettingsFormHPF.minHealingHPValue.Text = minHealingHP.ToString();
            SettingsFormHPF.minHealingHPValue.TextChanged += minHealingHPValue_TextChanged;
            SettingsFormHPF.minTankPriorityHPValue.Text = minTankPriorityHP.ToString();
            SettingsFormHPF.minTankPriorityHPValue.TextChanged += minTankPriorityHPValue_TextChanged;
            SettingsFormHPF.minSelfPriorityHPValue.Text = minSelfPriorityHP.ToString();
            SettingsFormHPF.minSelfPriorityHPValue.TextChanged += minSelfPriorityHPValue_TextChanged;

            SettingsFormHPF.FlashofLightHpValue.Text = FlashofLightHp.ToString();
            SettingsFormHPF.FlashofLightHpValue.TextChanged += FlashofLightHpValue_TextChanged;
            SettingsFormHPF.HolyLightHpValue.Text = HolyLightHp.ToString();
            SettingsFormHPF.HolyLightHpValue.TextChanged += HolyLightHpValue_TextChanged;
            SettingsFormHPF.HolyShockHpValue.Text = HolyShockHp.ToString();
            SettingsFormHPF.HolyShockHpValue.TextChanged += HolyShockHpValue_TextChanged;
            SettingsFormHPF.LightoftheMartyrHpValue.Text = LightoftheMartyrHp.ToString();
            SettingsFormHPF.LightoftheMartyrHpValue.TextChanged += LightoftheMartyrHpValue_TextChanged;
            SettingsFormHPF.BestowFaithHpValue.Text = BestowFaithHp.ToString();
            SettingsFormHPF.BestowFaithHpValue.TextChanged += BestowFaithHpValue_TextChanged;

            SettingsFormHPF.BlessingofSacrificeHpValue.Text = BlessingofProtectionHp.ToString();
            SettingsFormHPF.BlessingofSacrificeHpValue.TextChanged += BlessingofSacrificeHpValue_TextChanged;
            SettingsFormHPF.LayonHandsHpValue.Text = LayonHandsHp.ToString();
            SettingsFormHPF.LayonHandsHpValue.TextChanged += LayonHandsHpValue_TextChanged;
            SettingsFormHPF.BeaconOfVirtueHPValue.Text = BeaconOfVirtueHp.ToString() ;
            SettingsFormHPF.BeaconOfVirtueHPValue.TextChanged += BeaconOfVirtueHpValue_TextChanged;
            SettingsFormHPF.TyrsDeliveranceHPValue.Text = TyrsDeliveranceHP.ToString();
            SettingsFormHPF.TyrsDeliveranceHPValue.TextChanged += TyrsDeliveranceHPValue_TextChanged;
            SettingsFormHPF.AuraMasteryHPValue.Text = AuraMasteryHP.ToString();
            SettingsFormHPF.AuraMasteryHPValue.TextChanged += AuraMasteryHPValue_TextChanged;
            SettingsFormHPF.LightofDawnHPValue.Text = LightofDawnHP.ToString();
            SettingsFormHPF.LightofDawnHPValue.TextChanged += LightofDawnHPValue_TextChanged;

            SettingsFormHPF.BeaconOfVirtueLMR.Text = BeaconofVirtueLMR.ToString();
            SettingsFormHPF.BeaconOfVirtueLMR.TextChanged += BeaconOfVirtueLMR_TextChanged;
            SettingsFormHPF.TyrsDeliveranceLMR.Text = TyrsDeliveranceLMR.ToString();
            SettingsFormHPF.TyrsDeliveranceLMR.TextChanged += TyrsDeliveranceLMR_TextChanged;
            SettingsFormHPF.AuraMasteryLMR.Text = AuraMasteryLMR.ToString();
            SettingsFormHPF.AuraMasteryLMR.TextChanged += AuraMasteryLMR_TextChanged;
            SettingsFormHPF.LightofDawnLMR.Text = LightofDawnLMR.ToString();
            SettingsFormHPF.LightofDawnLMR.TextChanged += LightofDawnLMR_TextChanged;

            SettingsFormHPF.BeaconOfVirtueLMP.Text = BeaconofVirtueLMP.ToString();
            SettingsFormHPF.BeaconOfVirtueLMP.TextChanged += BeaconOfVirtueLMP_TextChanged;
            SettingsFormHPF.TyrsDeliveranceLMP.Text = TyrsDeliveranceLMP.ToString();
            SettingsFormHPF.TyrsDeliveranceLMP.TextChanged += TyrsDeliveranceLMP_TextChanged;
            SettingsFormHPF.AuraMasteryLMP.Text = AuraMasteryLMP.ToString();
            SettingsFormHPF.AuraMasteryLMP.TextChanged += AuraMasteryLMP_TextChanged;
            SettingsFormHPF.LightofDawnLMP.Text = LightofDawnLMP.ToString();
            SettingsFormHPF.LightofDawnLMP.TextChanged += LightofDawnLMP_TextChanged;

            SettingsFormHPF.TyrsDeliveranceEnable.Checked = TyrsDeliveranceEnabled;
            SettingsFormHPF.TyrsDeliveranceEnable.CheckedChanged += TyrsDeliveranceEnable_CheckedChanged;
            SettingsFormHPF.AuraMasteryEnable.Checked = AuraMasteryEnabled;
            SettingsFormHPF.AuraMasteryEnable.CheckedChanged += AuraMasteryEnable_CheckedChanged;
            SettingsFormHPF.LightofDawnEnable.Checked = LightofDawnEnabled;
            SettingsFormHPF.LightofDawnEnable.CheckedChanged += LightofDawnEnable_CheckedChanged;
            #endregion

        }
        #region GUI handler
        private void BeaconofVirtue_CheckedChanged(object sender, EventArgs e)
        {
            isTalentBoV = SettingsFormHPF.BeaconofVirtue.Checked;
        }

        private void BestowFaith_CheckedChanged(object sender, EventArgs e)
        {
            isTalentBestowFaith = SettingsFormHPF.BestowFaith.Checked;
        }

        private void BeaconofFaith_CheckedChanged(object sender, EventArgs e)
        {
            isTalentBoF = SettingsFormHPF.BeaconofFaith.Checked;
        }

        private void JudgmentOfLight_CheckedChanged(object sender, EventArgs e)
        {
            isTalentJoL = SettingsFormHPF.JudgmentOfLight.Checked;
        }

        private void minHealingHPValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.minHealingHPValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                minHealingHP = userVal;
            }
            else
            {
                SettingsFormHPF.minHealingHPValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void minTankPriorityHPValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.minTankPriorityHPValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                minTankPriorityHP = userVal;
            }
            else
            {
                SettingsFormHPF.minTankPriorityHPValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void FlashofLightHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.FlashofLightHpValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                FlashofLightHp = userVal;
            }
            else
            {
                SettingsFormHPF.FlashofLightHpValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void minSelfPriorityHPValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.minSelfPriorityHPValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                minSelfPriorityHP = userVal;
            }
            else
            {
                SettingsFormHPF.minSelfPriorityHPValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void HolyLightHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.HolyLightHpValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                HolyLightHp = userVal;
            }
            else
            {
                SettingsFormHPF.HolyLightHpValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void LightoftheMartyrHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.LightoftheMartyrHpValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                LightoftheMartyrHp = userVal;
            }
            else
            {
                SettingsFormHPF.LightoftheMartyrHpValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void BestowFaithHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.BestowFaithHpValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                BestowFaithHp = userVal;
            }
            else
            {
                SettingsFormHPF.BestowFaithHpValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void BlessingofSacrificeHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.BlessingofSacrificeHpValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                BlessingofSacrificeHp = userVal;
            }
            else
            {
                SettingsFormHPF.BlessingofSacrificeHpValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void LayonHandsHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.LayonHandsHpValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                LayonHandsHp = userVal;
            }
            else
            {
                SettingsFormHPF.LayonHandsHpValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void HolyShockHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.HolyShockHpValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                HolyShockHp = userVal;
            }
            else
            {
                SettingsFormHPF.HolyShockHpValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void BeaconOfVirtueHpValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.BeaconOfVirtueHPValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                BeaconOfVirtueHp = userVal;
            }
            else
            {
                SettingsFormHPF.BeaconOfVirtueHPValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void TyrsDeliveranceHPValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.TyrsDeliveranceHPValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                TyrsDeliveranceHP = userVal;
            }
            else
            {
                SettingsFormHPF.TyrsDeliveranceHPValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void AuraMasteryHPValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.AuraMasteryHPValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                AuraMasteryHP = userVal;
            }
            else
            {
                SettingsFormHPF.AuraMasteryHPValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void LightofDawnHPValue_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.LightofDawnHPValue.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                LightofDawnHP = userVal;
            }
            else
            {
                SettingsFormHPF.LightofDawnHPValue.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void BeaconOfVirtueLMR_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.BeaconOfVirtueLMR.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                BeaconofVirtueLMR = userVal;
            }
            else
            {
                SettingsFormHPF.BeaconOfVirtueLMR.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void TyrsDeliveranceLMR_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.TyrsDeliveranceLMR.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                TyrsDeliveranceLMR = userVal;
            }
            else
            {
                SettingsFormHPF.TyrsDeliveranceLMR.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void AuraMasteryLMR_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.AuraMasteryLMR.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                AuraMasteryLMR = userVal;
            }
            else
            {
                SettingsFormHPF.AuraMasteryLMR.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void LightofDawnLMR_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.LightofDawnLMR.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                LightofDawnLMR = userVal;
            }
            else
            {
                SettingsFormHPF.LightofDawnLMR.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void BeaconOfVirtueLMP_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.BeaconOfVirtueLMP.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                BeaconofVirtueLMP = userVal;
            }
            else
            {
                SettingsFormHPF.BeaconOfVirtueLMP.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void TyrsDeliveranceLMP_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.TyrsDeliveranceLMP.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                TyrsDeliveranceLMP = userVal;
            }
            else
            {
                SettingsFormHPF.TyrsDeliveranceLMP.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void AuraMasteryLMP_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.AuraMasteryLMP.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                AuraMasteryLMP = userVal;
            }
            else
            {
                SettingsFormHPF.AuraMasteryLMP.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void LightofDawnLMP_TextChanged(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormHPF.LightofDawnLMP.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                LightofDawnLMP = userVal;
            }
            else
            {
                SettingsFormHPF.LightofDawnLMP.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void TyrsDeliveranceEnable_CheckedChanged(object sender, EventArgs e)
        {
            TyrsDeliveranceEnabled = SettingsFormHPF.TyrsDeliveranceEnable.Checked;
        }
        private void AuraMasteryEnable_CheckedChanged(object sender, EventArgs e)
        {
            AuraMasteryEnabled = SettingsFormHPF.AuraMasteryEnable.Checked;
        }
        private void LightofDawnEnable_CheckedChanged(object sender, EventArgs e)
        {
            LightofDawnEnabled = SettingsFormHPF.LightofDawnEnable.Checked;
        }


        #endregion
        public override void Stop()
        {
        }

        public override void Pulse()
        {            
            if(DEBUG)
            Log.Write("group size: " + GroupSize() + " is in raid: " + isInRaid);
            tankTankingID = -1;
            groupSize = GroupSize();
            currentMana = WoW.Mana;
            currentHealth = WoW.HealthPercent;
            if (isInRaid)
            {
                RaidPulse();
                return;
            }
            else
            {
                PartyPulse();
                return;
            }
        }

        private void RaidPulse()
        {
            if (!WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
            {
                raidTargetSelector();
                if (DEBUG)
                    Log.Write("tankid: " + tankPartyID + " tankid2: " + tankPartyID2 + " tank tanking: " + tankTankingID);
                if (targetDispelPartyID != -1 && targetHealth > LayonHandsHp &&  WoW.CanCast(SPELL.CLEANSE, false, true, false, false, false))
                {
                    TargetOnRaid(targetDispelPartyID);
                    WoW.CastSpell(SPELL.CLEANSE);
                    return;
                }
                if (targetPartyID != -1 && !(tankTankingID != -1 && useBeaconOfLight && !isTalentBoF && !isTalentBoV && TankRaidCurrentBOL != tankTankingID && IsInHealRangeParty(tankTankingID)))
                    TargetOnRaid(targetPartyID);
                else if (tankTankingID != -1 && IsInHealRangeParty(tankTankingID))
                {
                    TargetOnRaid(tankTankingID);
                    if (TankRaidCurrentBOL != tankTankingID)
                    {
                        if (useBeaconOfLight && !isTalentBoF && !isTalentBoV && !WoW.TargetHasBuff(SPELL.BEACON_OF_LIGHT) && WoW.CanCast(SPELL.BEACON_OF_LIGHT, false, true, false, false, false))
                        {
                            WoW.CastSpell(SPELL.BEACON_OF_LIGHT);
                            TankRaidCurrentBOL = tankTankingID;
                            return;
                        }
                    }
                    if (WoW.CanCast(SPELL.HOLY_SHOCK, false, true, false, false, false) && currentMana >= 40)
                    {
                        WoW.CastSpell(SPELL.HOLY_SHOCK);
                        return;
                    }
                    if ((WoW.PlayerHasBuff(SPELL.DIVINE_SHIELD) || (currentHealth >= 60 && WoW.PlayerHasBuff(SPELL.DIVINE_PROTECTION))) && WoW.CanCast(SPELL.LIGHT_OF_THE_MARTYR, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_THE_MARTYR);
                        return;
                    }
                    if (isTalentBestowFaith && WoW.CanCast(SPELL.BESTOW_FAITH, true, true, false, false, false) && currentMana >= 40)
                    {
                        WoW.CastSpell(SPELL.BESTOW_FAITH);
                        return;
                    }
                    if (isTalentJoL && isInJudgmentRange() && WoW.CanCast(SPELL.JUDGMENT, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.JUDGMENT);
                        return;
                    }
                    /*if (WoW.CanCast(SPELL.HOLY_LIGHT, false, true, false, false, false) && currentMana >= 70)
                    {
                        WoW.CastSpell(SPELL.HOLY_LIGHT);
                        lastCastedID = tankTankingID;
                        return;
                    }*/
                }
                if (targetPartyID != -1 && WoW.HasTarget && !WoW.TargetIsEnemy)
                {
                    lastCastedID = -1;
                    if (useBeaconOfLight && !isTalentBoF && !isTalentBoV && targetPartyID == tankTankingID && !WoW.TargetHasBuff(SPELL.BEACON_OF_LIGHT) && WoW.CanCast(SPELL.BEACON_OF_LIGHT, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.BEACON_OF_LIGHT);
                        TankRaidCurrentBOL = tankTankingID;
                        return;
                    }
                    if (isTalentBoV && partyMemberHP(BeaconOfVirtueHp) >= BeaconofVirtueLMR && WoW.CanCast(SPELL.BEACON_OF_VIRTUE, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.BEACON_OF_VIRTUE);
                        return;
                    }
                    if (targetHealth <= LayonHandsHp && !WoW.TargetHasDebuff(SPELL.FORBEARANCE) && WoW.CanCast(SPELL.LAY_ON_HANDS, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LAY_ON_HANDS);
                        return;
                    }
                    if ((targetPartyID == tankPartyID || targetPartyID == tankPartyID2) && targetHealth <= BlessingofSacrificeHp && targetPartyID != 0 && currentHealth >= 90 && WoW.CanCast(SPELL.BLESSING_OF_SACRIFICE, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.BLESSING_OF_SACRIFICE);
                        return;
                    }
                    if (AuraMasteryEnabled && partyMemberHP(AuraMasteryHP) >= AuraMasteryLMR && WoW.CanCast(SPELL.AURA_MASTERY, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.AURA_MASTERY);
                        return;
                    }
                    if (LightofDawnEnabled && partyMemberHP(LightofDawnHP)>=LightofDawnLMR && WoW.CanCast(SPELL.LIGHT_OF_DAWN, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_DAWN);
                        return;
                    }
                    if (targetHealth <= HolyShockHp && WoW.CanCast(SPELL.HOLY_SHOCK, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.HOLY_SHOCK);
                        return;
                    }
                    if (targetPartyID != playerRaidID && ((targetHealth <= LightoftheMartyrHp && currentHealth >= 60) || (WoW.PlayerHasBuff(SPELL.DIVINE_SHIELD) || (currentHealth >= 60 && WoW.PlayerHasBuff(SPELL.DIVINE_PROTECTION)))) && WoW.CanCast(SPELL.LIGHT_OF_THE_MARTYR, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_THE_MARTYR);
                        return;
                    }

                    if (isTalentBestowFaith && targetPartyID == tankTankingID && WoW.CanCast(SPELL.BESTOW_FAITH, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.BESTOW_FAITH);
                        return;
                    }
                    if (isTalentJoL && isInJudgmentRange() && WoW.CanCast(SPELL.JUDGMENT, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.JUDGMENT);
                        return;
                    }
                    if (TyrsDeliveranceEnabled && partyMemberHP(TyrsDeliveranceHP) >= TyrsDeliveranceLMR && WoW.CanCast(SPELL.TYRS_DELIVERANCE, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.TYRS_DELIVERANCE);
                        return;
                    }
                    if (targetHealth <= FlashofLightHp && (partyMemberLow >= 3 || targetHealth <= 50) && currentMana >= 30 && WoW.CanCast(SPELL.FLASH_OF_LIGHT, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.FLASH_OF_LIGHT);
                        lastCastedID = targetPartyID;
                        return;
                    }
                    if (targetPartyID != playerRaidID && targetHealth >=85 && currentHealth >= 80 && WoW.CanCast(SPELL.LIGHT_OF_THE_MARTYR, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_THE_MARTYR);
                        return;
                    }
                    if (targetHealth <= HolyLightHp && WoW.CanCast(SPELL.HOLY_LIGHT, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.HOLY_LIGHT);
                        lastCastedID = targetPartyID;
                        return;
                    }

                }
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {
                    if (WoW.CanCast(SPELL.JUDGMENT, true, true, true, false, true))
                    {
                        WoW.CastSpell(SPELL.JUDGMENT);
                        return;
                    }
                    if (WoW.CanCast(SPELL.HOLY_SHOCK, true, true, true, false, true))
                    {
                        WoW.CastSpell(SPELL.HOLY_SHOCK);
                        return;
                    }
                }
            }
            else
            {
                if (WoW.TargetHealthPercent >= 95 && lastCastedID != -1 && lastCastedID != tankPartyID && lastCastedID != tankPartyID2)
                {
                    WoW.CastSpell(SPELL.STOP_CASTING);
                    return;
                }
            }
        }
        private void PartyPulse()
        {
            if (combatRoutine.Type == RotationType.SingleTarget && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling) // Do Single Target Stuff here
            {
                partyTargetSelector();
                if (tankPartyID != -1)
                {
                    if (useBeaconOfLight && !isTalentBoF && !isTalentBoV && !PartyHasBuff(SPELL.BEACON_OF_LIGHT, tankPartyID) && IsInHealRangeParty(tankPartyID) && WoW.CanCast(SPELL.BEACON_OF_LIGHT, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BEACON_OF_LIGHT, tankPartyID);
                        return;
                    }
                }
                if (targetPartyID != -1)
                {
                    if (targetHealth <= LayonHandsHp && !PartyHasDeBuff(SPELL.FORBEARANCE, targetPartyID) && WoW.CanCast(SPELL.LAY_ON_HANDS, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.LAY_ON_HANDS, targetPartyID);
                        return;
                    }
                    if (isTalentBoV && partyMemberHP(BeaconOfVirtueHp) >= BeaconofVirtueLMP && WoW.CanCast(SPELL.BEACON_OF_VIRTUE, false, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BEACON_OF_VIRTUE,targetPartyID);
                        return;
                    }
                    /*if (targetHealth <= BlessingofProtectionHp && targetPartyID != tankPartyID && !PartyHasDeBuff(SPELL.FORBEARANCE, targetPartyID) && WoW.CanCast(SPELL.BLESSING_OF_PROTECTION, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BLESSING_OF_PROTECTION, targetPartyID);
                        return;
                    }*/
                    if (targetPartyID == tankPartyID && targetHealth <= BlessingofSacrificeHp && targetPartyID != 0 && currentHealth >= 90 && WoW.CanCast(SPELL.BLESSING_OF_SACRIFICE, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BLESSING_OF_SACRIFICE, targetPartyID);
                        return;
                    }
                    if (AuraMasteryEnabled && partyMemberHP(AuraMasteryHP) >= AuraMasteryLMP && WoW.CanCast(SPELL.AURA_MASTERY, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.AURA_MASTERY);
                        return;
                    }
                    if (LightofDawnEnabled && partyMemberHP(LightofDawnHP) >= LightofDawnLMP && WoW.CanCast(SPELL.LIGHT_OF_DAWN, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_DAWN);
                        return;
                    }
                    if (targetHealth <= HolyShockHp && WoW.CanCast(SPELL.HOLY_SHOCK, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.HOLY_SHOCK, targetPartyID);
                        return;
                    }
                    if (targetDispelPartyID != -1 && WoW.CanCast(SPELL.CLEANSE, false, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.CLEANSE, targetDispelPartyID);
                        return;
                    }
                    if (targetPartyID != 0 && ((targetHealth <= LightoftheMartyrHp && currentHealth >= 60) || (WoW.PlayerHasBuff(SPELL.DIVINE_SHIELD) || (currentHealth >= 60 && WoW.PlayerHasBuff(SPELL.DIVINE_PROTECTION)))) && WoW.CanCast(SPELL.LIGHT_OF_THE_MARTYR, false, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.LIGHT_OF_THE_MARTYR, targetPartyID);
                        return;
                    }
                    if (isTalentBestowFaith && targetHealth <= BestowFaithHp && WoW.CanCast(SPELL.BESTOW_FAITH, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BESTOW_FAITH, targetPartyID);
                        return;
                    }
                    if (TyrsDeliveranceEnabled && partyMemberHP(TyrsDeliveranceHP) >= TyrsDeliveranceLMR && WoW.CanCast(SPELL.TYRS_DELIVERANCE, false, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.TYRS_DELIVERANCE);
                        return;
                    }
                    if (targetHealth <= FlashofLightHp && WoW.CanCast(SPELL.FLASH_OF_LIGHT, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.FLASH_OF_LIGHT, targetPartyID);
                        return;
                    }
                    if (targetHealth <= HolyLightHp && WoW.CanCast(SPELL.HOLY_LIGHT, true, true, false, false, false) && WoW.IsInCombat)
                    {
                        castSpellOnGrp(SPELL.HOLY_LIGHT, targetPartyID);
                        return;
                    }
                }
                else if (targetDispelPartyID != -1 && WoW.CanCast(SPELL.CLEANSE, false, true, false, false, false))
                {
                    castSpellOnGrp(SPELL.CLEANSE, targetDispelPartyID);
                    return;
                }
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {
                    if (WoW.CanCast(SPELL.JUDGMENT, true, true, true, false, true))
                    {
                        WoW.CastSpell(SPELL.JUDGMENT);
                        return;
                    }
                    if (WoW.CanCast(SPELL.HOLY_SHOCK, true, true, true, false, true))
                    {
                        WoW.CastSpell(SPELL.HOLY_SHOCK);
                        return;
                    }
                    if (WoW.CanCast(SPELL.CRUSADER_STRIKE, true, true, true, true, true))
                    {
                        WoW.CastSpell(SPELL.CRUSADER_STRIKE);
                        return;
                    }
                    if (WoW.CanCast(SPELL.CONSECRATION, false, true, false, false, false) && WoW.CanCast(SPELL.CRUSADER_STRIKE, true, false, true, false, true))
                    {
                        WoW.CastSpell(SPELL.CONSECRATION);
                        return;
                    }
                }
                if (WoW.HasTarget && !WoW.TargetIsEnemy && WoW.IsInCombat)
                {
                    if (WoW.TargetHealthPercent <= LayonHandsHp && !WoW.TargetHasDebuff(SPELL.FORBEARANCE) && WoW.CanCast(SPELL.LAY_ON_HANDS, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LAY_ON_HANDS);
                        return;
                    }
                    if (WoW.TargetHealthPercent <= HolyShockHp && WoW.CanCast(SPELL.HOLY_SHOCK, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.HOLY_SHOCK);
                        return;
                    }
                    if (WoW.TargetHealthPercent <= LightoftheMartyrHp && currentHealth >= 85 && WoW.CanCast(SPELL.LIGHT_OF_THE_MARTYR, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_THE_MARTYR);
                        return;
                    }
                    if (isTalentBestowFaith && WoW.TargetHealthPercent <= BestowFaithHp && WoW.CanCast(SPELL.BESTOW_FAITH, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.BESTOW_FAITH);
                        return;
                    }
                    if (WoW.TargetHealthPercent <= FlashofLightHp && WoW.CanCast(SPELL.FLASH_OF_LIGHT, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.FLASH_OF_LIGHT);
                        return;
                    }
                    if (WoW.TargetHealthPercent <= HolyLightHp && WoW.CanCast(SPELL.HOLY_LIGHT, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.HOLY_LIGHT);
                        return;
                    }
                }

                /*if (tankPartyID != -1 && WoW.IsInCombat && IsInHealRangeParty(tankPartyID) && WoW.CanCast(SPELL.HOLY_LIGHT, true, true, false, false, false))
              {
                  castSpellOnGrp(SPELL.HOLY_LIGHT, tankPartyID);
                  return;
              }*/

            }
            if (combatRoutine.Type == RotationType.SingleTargetCleave) // Do Single Target Stuff here
            {
                partyTargetSelector();
                if (tankPartyID != -1)
                {
                    if (useBeaconOfLight && !isTalentBoF && !isTalentBoV && !PartyHasBuff(SPELL.BEACON_OF_LIGHT, tankPartyID) && IsInHealRangeParty(tankPartyID) && WoW.CanCast(SPELL.BEACON_OF_LIGHT, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BEACON_OF_LIGHT, tankPartyID);
                        return;
                    }
                }
                if (targetPartyID != -1)
                {
                    if (targetHealth <= LayonHandsHp && !PartyHasDeBuff(SPELL.FORBEARANCE, targetPartyID) && WoW.CanCast(SPELL.LAY_ON_HANDS, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.LAY_ON_HANDS, targetPartyID);
                        return;
                    }
                    if (targetHealth <= BlessingofProtectionHp && !PartyHasDeBuff(SPELL.FORBEARANCE, targetPartyID) && targetPartyID != tankPartyID && WoW.CanCast(SPELL.BLESSING_OF_PROTECTION, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BLESSING_OF_PROTECTION, targetPartyID);
                        return;
                    }
                    if (targetHealth <= BlessingofSacrificeHp && !PartyHasDeBuff(SPELL.FORBEARANCE, targetPartyID) && targetPartyID != 0 && currentHealth >= 90 && WoW.CanCast(SPELL.BLESSING_OF_SACRIFICE, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BLESSING_OF_SACRIFICE, targetPartyID);
                        return;
                    }
                    /*if (partyMemberReallyLow >= 3 && WoW.CanCast(SPELL.TYRS_DELIVERANCE, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.TYRS_DELIVERANCE);
                        return;
                    }*/
                    /*if (partyMemberLow >= 3 && WoW.CanCast(SPELL.LIGHT_OF_DAWN, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_DAWN);
                        return;
                    }*/
                    if (targetHealth <= HolyShockHp && WoW.CanCast(SPELL.HOLY_SHOCK, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.HOLY_SHOCK, targetPartyID);
                        return;
                    }
                    if (targetHealth <= LightoftheMartyrHp && currentHealth >= 85 && targetPartyID != 0 && WoW.CanCast(SPELL.LIGHT_OF_THE_MARTYR, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.LIGHT_OF_THE_MARTYR, targetPartyID);
                        return;
                    }
                    if (isTalentBestowFaith && targetHealth <= BestowFaithHp && WoW.CanCast(SPELL.BESTOW_FAITH, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BESTOW_FAITH, targetPartyID);
                        return;
                    }
                    if (targetHealth <= HolyLightHp && WoW.CanCast(SPELL.FLASH_OF_LIGHT, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.FLASH_OF_LIGHT, targetPartyID);
                        return;
                    }
                }
                if (tankPartyID != -1 && WoW.IsInCombat && IsInHealRangeParty(tankPartyID) && WoW.CanCast(SPELL.HOLY_LIGHT, true, true, false, false, false))
                {
                    castSpellOnGrp(SPELL.HOLY_LIGHT, tankPartyID);
                    return;
                }
            }
            if (combatRoutine.Type == RotationType.AOE)
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {
                    if (WoW.CanCast(SPELL.JUDGMENT, true, true, true, false, true))
                    {
                        WoW.CastSpell(SPELL.JUDGMENT);
                        return;
                    }
                    if (WoW.CanCast(SPELL.HOLY_SHOCK, true, true, true, false, true))
                    {
                        WoW.CastSpell(SPELL.HOLY_SHOCK);
                        return;
                    }
                    if (WoW.CanCast(SPELL.CRUSADER_STRIKE, true, true, true, true, true))
                    {
                        WoW.CastSpell(SPELL.CRUSADER_STRIKE);
                        return;
                    }
                    if (WoW.CanCast(SPELL.CONSECRATION, false, true, false, false, false) && WoW.CanCast(SPELL.CRUSADER_STRIKE, true, false, true, false, true))
                    {
                        WoW.CastSpell(SPELL.CONSECRATION);
                        return;
                    }

                }
                partyTargetSelector();
                if (targetPartyID != -1)
                {
                    /*if (partyMemberLow >= 3 && WoW.CanCast(SPELL.LIGHT_OF_DAWN, true, true, false, false, false))
                    {
                        WoW.CastSpell(SPELL.LIGHT_OF_DAWN);
                        return;
                    }*/
                    if (targetHealth <= LightoftheMartyrHp && currentHealth >= 85 && targetPartyID != 0 && WoW.CanCast(SPELL.LIGHT_OF_THE_MARTYR, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.LIGHT_OF_THE_MARTYR, targetPartyID);
                        return;
                    }
                    if (isTalentBestowFaith && targetPartyID != -1 && targetHealth <= BestowFaithHp && WoW.CanCast(SPELL.BESTOW_FAITH, true, true, false, false, false))
                    {
                        castSpellOnGrp(SPELL.BESTOW_FAITH, targetPartyID);
                        return;
                    }
                }
            }
        }
        private int partyMemberHP(int HpSetting)
        {
            if(80 == HpSetting)
            {
                return partyMemberLow;
            }
            return partyMemberReallyLow;
        }
        private void partyTargetSelector()
        {
            targetPartyID = -1;
            targetHealth = minHealingHP;
            partyMemberLow = 0;
            partyMemberReallyLow = 0;
            var tankHealh = 100;

            tankPartyID = -1;
            var selfHealth = 100;
            var tankDispel = false;
            var tankDispelPriority = false;
            var selfDispel = false;
            var selfDispelPriority = false;
            targetDispelPartyID = -1;
            bool isDispelPriorityTarget = false;
            if (groupSize == 0)
                groupSize = 1;
            for (int i = 0; i < groupSize; i++)
            {
                if (IsTankParty(i))
                {
                    tankPartyID = i;
                }
                var currentTargetHP = HealthPercentParty(i);
                var isDispelNeededParty = isDispelNeeded(i);
                var isDispelPriorityParty = isDispelPriority(i);
                if (isDispelNeededParty)
                    Log.Write("Party member: " + i + " isDispelNeeded: " + isDispelNeededParty + " IsDispelPriority " + isDispelPriorityParty);
                var isInHealRange = IsInHealRangeParty(i);
                if (currentTargetHP <= 80 && isInHealRange)
                {
                    partyMemberLow++;
                }
                if (currentTargetHP <= 60 && isInHealRange)
                {
                    partyMemberLow++;
                    partyMemberReallyLow++;
                }
                if (i == tankPartyID && isInHealRange)
                {
                    tankHealh = currentTargetHP;
                    tankDispel = isDispelNeededParty;
                    tankDispelPriority = isDispelPriorityParty;
                }
                if (i == 0 && isInHealRange)
                {
                    selfHealth = currentTargetHP;
                    selfDispel = isDispelNeededParty;
                    selfDispelPriority = isDispelPriorityParty;
                }
                if (currentTargetHP <= targetHealth && targetHealth != 0 && isInHealRange && !(PartyHasBuff(SPELL.BESTOW_FAITH, i) && (currentTargetHP + 10) >= targetHealth))
                {
                    targetHealth = currentTargetHP;
                    targetPartyID = i;
                }
                if (isDispelNeededParty && isInHealRange && !isDispelPriorityTarget)
                {
                    targetDispelPartyID = i;
                    isDispelPriorityTarget = isDispelPriorityParty;
                }
            }
            if (tankHealh <= minTankPriorityHP)
            {
                targetHealth = tankHealh;
                targetPartyID = tankPartyID;
            }
            if (selfHealth <= minSelfPriorityHP)
            {
                targetHealth = selfHealth;
                targetPartyID = 0;
            }
            if(tankDispel && (!isDispelPriorityTarget || tankDispelPriority))
            {
                targetDispelPartyID = tankPartyID;
            }
            if (selfDispel && (!isDispelPriorityTarget || selfDispelPriority))
            {
                targetDispelPartyID = 0;
            }
            
        }
        private void raidTargetSelector()
        {
            targetPartyID = -1;
            playerRaidID = -1;
            targetHealth = minHealingHP;
            partyMemberLow = 0;
            partyMemberReallyLow = 0;
            var tankHealh = 100;
            tankPartyID = -1;
            var tankHealh2 = 100;
            tankPartyID2 = -1;
            var tankDispel = false;
            var tankDispelPriority = false;
            var tankDispel2 = false;
            var tankDispelPriority2 = false;
            var selfDispel = false;
            var selfDispelPriority = false;
            var playerHealth = -1;
            targetDispelPartyID = -1;
            bool isDispelPriorityTarget = false;
            for (int i = 1; i <= groupSize; i++)
            {
                if (IsTankParty(i))
                {
                    if (tankPartyID == -1)
                        tankPartyID = i;
                    else
                        tankPartyID2 = i;
                }
                var currentTargetHP = HealthPercentParty(i);
                var isInHealRange = IsInHealRangeParty(i);
                var isDispelNeededParty = isDispelNeeded(i);
                var isDispelPriorityParty = isDispelPriority(i);
                if (isDispelNeededParty)
                    Log.Write("Raid member: " + i + " isDispelNeeded: " + isDispelNeededParty + " IsDispelPriority " + isDispelPriorityParty);
                if (currentTargetHP <= 80 && isInHealRange)
                {
                    partyMemberLow++;
                }
                if (currentTargetHP <= 60 && isInHealRange)
                {
                    partyMemberLow++;
                    partyMemberReallyLow++;
                }
                if (i == tankPartyID && isInHealRange)
                {
                    tankHealh = currentTargetHP;
                    tankDispel = isDispelNeededParty;
                    tankDispelPriority = isDispelPriorityParty;
                }
                if (i == tankPartyID2 && isInHealRange)
                {
                    tankHealh2 = currentTargetHP;
                    tankDispel2 = isDispelNeededParty;
                    tankDispelPriority2 = isDispelPriorityParty;
                }
                if (i == playerRaidID && isInHealRange)
                {
                    playerHealth = currentTargetHP;
                    selfDispel = isDispelNeededParty;
                    selfDispelPriority = isDispelPriorityParty;
                }
                if (currentTargetHP <= targetHealth && targetHealth != 0 && isInHealRange)
                {
                    targetHealth = currentTargetHP;
                    targetPartyID = i;
                }
                if (isDispelNeededParty && isInHealRange && !isDispelPriorityTarget)
                {
                    targetDispelPartyID = i;
                    isDispelPriorityTarget = isDispelPriorityParty;
                }
            }
            if (tankHealh <= minTankPriorityHP)
            {
                targetHealth = tankHealh;
                targetPartyID = tankPartyID;

            }
            if (tankHealh2 <= minTankPriorityHP)
            {
                targetHealth = tankHealh2;
                targetPartyID = tankPartyID2;
            }
            if (playerHealth <= minSelfPriorityHP)
            {
                targetHealth = playerHealth;
                targetPartyID = playerRaidID;
            }
            if (tankDispel && (!isDispelPriorityTarget || tankDispelPriority))
            {
                targetDispelPartyID = tankPartyID;
            }
            if (tankDispel2 && (!isDispelPriorityTarget || tankDispelPriority2))
            {
                targetDispelPartyID = tankPartyID2;
            }
            if (selfDispel && (!isDispelPriorityTarget || selfDispelPriority))
            {
                targetDispelPartyID = 0;
            }

        }

        private bool isInJudgmentRange()
        {
            var c = WoW.GetBlockColor(1, 21);
            try
            {
                return c.B != 255;
            }
            catch (Exception ex)
            {
                Log.Write("[Health] Red = " + c.B);
                Log.Write(ex.Message, Color.Red);
                return false;
            }
        }

        private void TargetOnRaid(int id)
        {
            if(DEBUG)
            Log.Write("target raid member" + id + " modulo: " + id % 8 + "div: " + (id - 1) / 8);
            switch (id % 8)
            {
                case 0:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 5:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 6:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 7:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
            }
            WoW.CastSpell("target" + ((id - 1) / 8));
            switch (id % 8)
            {
                case 0:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 5:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 6:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 7:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
            }
        }
        private void castSpellOnGrp(string spell, int id)
        {
            if(DEBUG)
            Log.Write("target raid member" + id);
            switch (id)
            {
                case 0:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 1:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
            }
            WoW.CastSpell(spell);
            switch (id)
            {
                case 0:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 1:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
            }
        }
        private static int HealthPercentParty(int partyID)
        {
            if (partyID == 0)
                return WoW.HealthPercent;
            var c = WoW.GetBlockColor(partyID + 1, 21);
            try
            {
                if (c.R == 0)
                    return 100;
                int health = Convert.ToInt32((double)c.R * 100 / 255);
                return health;
            }
            catch (Exception ex)
            {
                Log.Write("[Health] Red = " + c.R);
                Log.Write(ex.Message, Color.Red);
                return 100;
            }
        }
        private static bool isDispelNeeded(int partyID)
        {
            var c = WoW.GetBlockColor(partyID + 1, 22);
            try
            {
                return c.R == 255;
            }
            catch (Exception ex)
            {
                Log.Write("[DispelNeeded] Red = " + c.R);
                Log.Write(ex.Message, Color.Red);
                return false;
            }
        }
        private static bool isDispelPriority(int partyID)
        {
            var c = WoW.GetBlockColor(partyID + 1, 22);
            try
            {
                return c.G == 255;
            }
            catch (Exception ex)
            {
                Log.Write("[DispelNeeded] Red = " + c.R);
                Log.Write(ex.Message, Color.Red);
                return false;
            }
        }
        private int GroupSize()
        {
            var c = WoW.GetBlockColor(1, 21);
            try
            {
                if (c.R == 0)
                    return 0;
                isInRaid = c.G != 255;
                int size = Convert.ToInt32((double)c.R * 100 / 255);
                if (size == 100)
                    return 0;
                return size;
            }
            catch (Exception ex)
            {
                Log.Write("[Health] Red = " + c.R);
                Log.Write(ex.Message, Color.Red);
                return 100;
            }
        }
        private bool IsInHealRangeParty(int partyID)
        {
            if (partyID == 0)
                return true;
            var c = WoW.GetBlockColor(partyID + 1, 21);
            if (c.G != 255 && c.G != 0)
                playerRaidID = partyID;
            try
            {
                return c.G != 255;
            }
            catch (Exception ex)
            {
                Log.Write("[Health] Green = " + c.G);
                Log.Write(ex.Message, Color.Red);
                return false;
            }
        }
        private bool IsTankParty(int partyID)
        {
            if (WoW.HealthPercent != 0)
            {
                if (partyID == 0)
                    return false;
                var c = WoW.GetBlockColor(partyID + 1, 21);
                if (c.B <= 80 && c.B != 0)
                {
                    if (tankTankingID == -1)
                        tankTankingID = partyID;
                }

                try
                {
                    return c.B <= 80;
                }
                catch (Exception ex)
                {
                    Log.Write("[Health] Blue = " + c.B);
                    Log.Write(ex.Message, Color.Red);
                    return false;
                }
            }
            return false;
        }
        private static bool PartyHasBuff(string buffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerHasBuff(buffName);
            Aura aura = null;
            for (var i = 0; i < SpellBook.Auras.Count; i++)
            {
                if (SpellBook.Auras[i].AuraName == buffName)
                    aura = SpellBook.Auras[i];
            }
            if (aura == null)
            {
                Log.Write("[Hasbuff] Fant ikke debuff '" + buffName + "' in Spell Book");
                return false;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 12 + partyID);
            return (c.R != 255) && (c.G != 255) && (c.B != 255);
        }
        private static bool PartyHasDeBuff(string debuffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerHasDebuff(debuffName);
            Aura aura = null;
            for (var i = 0; i < SpellBook.Auras.Count; i++)
            {
                if (SpellBook.Auras[i].AuraName == debuffName)
                    aura = SpellBook.Auras[i];
            }
            if (aura == null)
            {
                Log.Write("[HasDebuff] Fant ikke debuff '" + debuffName + "' in Spell Book");
                return false;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 16 + partyID);
            return (c.R != 255) && (c.G != 255) && (c.B != 255);
        }
        private static int GetPartyBuffTimeRemaining(string buffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerBuffTimeRemaining(buffName);
            Aura aura = null;
            for (var i = 0; i < SpellBook.Auras.Count; i++)
            {
                if (SpellBook.Auras[i].AuraName == buffName)
                    aura = SpellBook.Auras[i];
            }
            if (aura == null)
            {
                Log.Write("[HasDebuff] Fant ikke buff '" + buffName + "' in Spell Book");
                return 0;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 12 + partyID);

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                if (c.R == 0)
                    return 0;
                return Convert.ToInt32((double)c.R * 100 / 255);
            }
            catch (Exception ex)
            {
                Log.Write("Failed to find debuff stacks for color G = " + c.B, Color.Red);
                Log.Write("Error: " + ex.Message, Color.Red);
            }

            return 0;
        }       
    }


    public class SettingsFormHPF : Form
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        /// 
        public SettingsFormHPF()
        {
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Hello");
            this.TABControl = new System.Windows.Forms.TabControl();
            this.HealSettingTab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TyrsDeliveranceLMP = new System.Windows.Forms.TextBox();
            this.BeaconOfVirtueLMP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TyrsDeliveranceEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TyrsDeliveranceLMR = new System.Windows.Forms.TextBox();
            this.BeaconOfVirtueLMR = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BeaconOfVirtueHp = new System.Windows.Forms.Label();
            this.TyrsDeliveranceHP = new System.Windows.Forms.Label();
            this.STHSpell = new System.Windows.Forms.GroupBox();
            this.FlashofLightHp = new System.Windows.Forms.Label();
            this.FlashofLightHpValue = new System.Windows.Forms.TextBox();
            this.HolyLightHpValue = new System.Windows.Forms.TextBox();
            this.HolyShockHpValue = new System.Windows.Forms.TextBox();
            this.HolyLightHp = new System.Windows.Forms.Label();
            this.HolyShockHp = new System.Windows.Forms.Label();
            this.LightoftheMartyrHpValue = new System.Windows.Forms.TextBox();
            this.LightoftheMartyrHp = new System.Windows.Forms.Label();
            this.BestowFaithHp = new System.Windows.Forms.Label();
            this.BestowFaithHpValue = new System.Windows.Forms.TextBox();
            this.BlessingofSacrificeHp = new System.Windows.Forms.Label();
            this.LayonHandsHpValue = new System.Windows.Forms.TextBox();
            this.BlessingofSacrificeHpValue = new System.Windows.Forms.TextBox();
            this.LayonHandsHp = new System.Windows.Forms.Label();
            this.groupBoxGeneralHealing = new System.Windows.Forms.GroupBox();
            this.minHealingHP = new System.Windows.Forms.Label();
            this.minHealingHPValue = new System.Windows.Forms.TextBox();
            this.minTankPriorityHPValue = new System.Windows.Forms.TextBox();
            this.minSelfPriorityHPValue = new System.Windows.Forms.TextBox();
            this.minTankPriorityHP = new System.Windows.Forms.Label();
            this.minSelfPriorityHP = new System.Windows.Forms.Label();
            this.TalentTab = new System.Windows.Forms.TabPage();
            this.BeaconofVirtue = new System.Windows.Forms.CheckBox();
            this.BeaconofFaith = new System.Windows.Forms.CheckBox();
            this.JudgmentOfLight = new System.Windows.Forms.CheckBox();
            this.BestowFaith = new System.Windows.Forms.CheckBox();
            this.AuraMasteryLMP = new System.Windows.Forms.TextBox();
            this.AuraMasteryEnable = new System.Windows.Forms.CheckBox();
            this.AuraMasteryLMR = new System.Windows.Forms.TextBox();
            this.labelAuraMastery = new System.Windows.Forms.Label();
            this.LightofDawnLMP = new System.Windows.Forms.TextBox();
            this.LightofDawnEnable = new System.Windows.Forms.CheckBox();
            this.LightofDawnLMR = new System.Windows.Forms.TextBox();
            this.LightofDawn = new System.Windows.Forms.Label();
            this.DispelTAB = new System.Windows.Forms.TabPage();
            this.generalDispelSettings = new System.Windows.Forms.GroupBox();
            this.dispelwaittime = new System.Windows.Forms.Label();
            this.dispelrandomwait = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.dispelmode = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SpellID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.dispelPLUS = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.DispeSpelllistgroup = new System.Windows.Forms.GroupBox();
            this.groupBoxInstruction = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.BeaconOfVirtueHPValue = new System.Windows.Forms.ComboBox();
            this.TyrsDeliveranceHPValue = new System.Windows.Forms.ComboBox();
            this.AuraMasteryHPValue = new System.Windows.Forms.ComboBox();
            this.LightofDawnHPValue = new System.Windows.Forms.ComboBox();
            this.TABControl.SuspendLayout();
            this.HealSettingTab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.STHSpell.SuspendLayout();
            this.groupBoxGeneralHealing.SuspendLayout();
            this.TalentTab.SuspendLayout();
            this.DispelTAB.SuspendLayout();
            this.generalDispelSettings.SuspendLayout();
            this.DispeSpelllistgroup.SuspendLayout();
            this.groupBoxInstruction.SuspendLayout();
            this.SuspendLayout();
            // 
            // TABControl
            // 
            this.TABControl.Controls.Add(this.HealSettingTab);
            this.TABControl.Controls.Add(this.TalentTab);
            this.TABControl.Controls.Add(this.DispelTAB);
            this.TABControl.Font = new System.Drawing.Font("Lucida Sans", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TABControl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TABControl.Location = new System.Drawing.Point(2, 1);
            this.TABControl.Name = "TABControl";
            this.TABControl.SelectedIndex = 0;
            this.TABControl.Size = new System.Drawing.Size(721, 486);
            this.TABControl.TabIndex = 0;
            // 
            // HealSettingTab
            // 
            this.HealSettingTab.BackColor = System.Drawing.Color.White;
            this.HealSettingTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.HealSettingTab.Controls.Add(this.groupBoxInstruction);
            this.HealSettingTab.Controls.Add(this.groupBox1);
            this.HealSettingTab.Controls.Add(this.STHSpell);
            this.HealSettingTab.Controls.Add(this.groupBoxGeneralHealing);
            this.HealSettingTab.Location = new System.Drawing.Point(4, 26);
            this.HealSettingTab.Name = "HealSettingTab";
            this.HealSettingTab.Padding = new System.Windows.Forms.Padding(3);
            this.HealSettingTab.Size = new System.Drawing.Size(713, 456);
            this.HealSettingTab.TabIndex = 0;
            this.HealSettingTab.Text = "Heal Settings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LightofDawnHPValue);
            this.groupBox1.Controls.Add(this.AuraMasteryHPValue);
            this.groupBox1.Controls.Add(this.TyrsDeliveranceHPValue);
            this.groupBox1.Controls.Add(this.BeaconOfVirtueHPValue);
            this.groupBox1.Controls.Add(this.LightofDawnLMP);
            this.groupBox1.Controls.Add(this.AuraMasteryLMP);
            this.groupBox1.Controls.Add(this.LightofDawnEnable);
            this.groupBox1.Controls.Add(this.AuraMasteryEnable);
            this.groupBox1.Controls.Add(this.LightofDawnLMR);
            this.groupBox1.Controls.Add(this.AuraMasteryLMR);
            this.groupBox1.Controls.Add(this.LightofDawn);
            this.groupBox1.Controls.Add(this.labelAuraMastery);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TyrsDeliveranceLMP);
            this.groupBox1.Controls.Add(this.BeaconOfVirtueLMP);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TyrsDeliveranceEnable);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TyrsDeliveranceLMR);
            this.groupBox1.Controls.Add(this.BeaconOfVirtueLMR);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.BeaconOfVirtueHp);
            this.groupBox1.Controls.Add(this.TyrsDeliveranceHP);
            this.groupBox1.Location = new System.Drawing.Point(294, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 201);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Multi Target Healing Spell";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(297, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 17);
            this.label4.TabIndex = 38;
            this.label4.Text = "LMP";
            // 
            // TyrsDeliveranceLMP
            // 
            this.TyrsDeliveranceLMP.Location = new System.Drawing.Point(293, 90);
            this.TyrsDeliveranceLMP.Name = "TyrsDeliveranceLMP";
            this.TyrsDeliveranceLMP.Size = new System.Drawing.Size(43, 25);
            this.TyrsDeliveranceLMP.TabIndex = 37;
            // 
            // BeaconOfVirtueLMP
            // 
            this.BeaconOfVirtueLMP.Location = new System.Drawing.Point(293, 57);
            this.BeaconOfVirtueLMP.Name = "BeaconOfVirtueLMP";
            this.BeaconOfVirtueLMP.Size = new System.Drawing.Size(43, 25);
            this.BeaconOfVirtueLMP.TabIndex = 36;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(334, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 17);
            this.label3.TabIndex = 33;
            this.label3.Text = "Enabled";
            // 
            // TyrsDeliveranceEnable
            // 
            this.TyrsDeliveranceEnable.AutoSize = true;
            this.TyrsDeliveranceEnable.Location = new System.Drawing.Point(361, 95);
            this.TyrsDeliveranceEnable.Name = "TyrsDeliveranceEnable";
            this.TyrsDeliveranceEnable.Size = new System.Drawing.Size(15, 14);
            this.TyrsDeliveranceEnable.TabIndex = 31;
            this.TyrsDeliveranceEnable.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 17);
            this.label2.TabIndex = 29;
            this.label2.Text = "LMR";
            // 
            // TyrsDeliveranceLMR
            // 
            this.TyrsDeliveranceLMR.Location = new System.Drawing.Point(243, 90);
            this.TyrsDeliveranceLMR.Name = "TyrsDeliveranceLMR";
            this.TyrsDeliveranceLMR.Size = new System.Drawing.Size(43, 25);
            this.TyrsDeliveranceLMR.TabIndex = 28;
            // 
            // BeaconOfVirtueLMR
            // 
            this.BeaconOfVirtueLMR.Location = new System.Drawing.Point(243, 57);
            this.BeaconOfVirtueLMR.Name = "BeaconOfVirtueLMR";
            this.BeaconOfVirtueLMR.Size = new System.Drawing.Size(43, 25);
            this.BeaconOfVirtueLMR.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(199, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "HP";
            // 
            // BeaconOfVirtueHp
            // 
            this.BeaconOfVirtueHp.AutoSize = true;
            this.BeaconOfVirtueHp.Location = new System.Drawing.Point(6, 60);
            this.BeaconOfVirtueHp.Name = "BeaconOfVirtueHp";
            this.BeaconOfVirtueHp.Size = new System.Drawing.Size(131, 17);
            this.BeaconOfVirtueHp.TabIndex = 18;
            this.BeaconOfVirtueHp.Text = "Beacon of Virtue";
            // 
            // TyrsDeliveranceHP
            // 
            this.TyrsDeliveranceHP.AutoSize = true;
            this.TyrsDeliveranceHP.Location = new System.Drawing.Point(6, 93);
            this.TyrsDeliveranceHP.Name = "TyrsDeliveranceHP";
            this.TyrsDeliveranceHP.Size = new System.Drawing.Size(133, 17);
            this.TyrsDeliveranceHP.TabIndex = 19;
            this.TyrsDeliveranceHP.Text = "Tyr\'s Deliverance";
            // 
            // STHSpell
            // 
            this.STHSpell.Controls.Add(this.FlashofLightHp);
            this.STHSpell.Controls.Add(this.FlashofLightHpValue);
            this.STHSpell.Controls.Add(this.HolyLightHpValue);
            this.STHSpell.Controls.Add(this.HolyShockHpValue);
            this.STHSpell.Controls.Add(this.HolyLightHp);
            this.STHSpell.Controls.Add(this.HolyShockHp);
            this.STHSpell.Controls.Add(this.LightoftheMartyrHpValue);
            this.STHSpell.Controls.Add(this.LightoftheMartyrHp);
            this.STHSpell.Controls.Add(this.BestowFaithHp);
            this.STHSpell.Controls.Add(this.BestowFaithHpValue);
            this.STHSpell.Controls.Add(this.BlessingofSacrificeHp);
            this.STHSpell.Controls.Add(this.LayonHandsHpValue);
            this.STHSpell.Controls.Add(this.BlessingofSacrificeHpValue);
            this.STHSpell.Controls.Add(this.LayonHandsHp);
            this.STHSpell.Location = new System.Drawing.Point(9, 171);
            this.STHSpell.Name = "STHSpell";
            this.STHSpell.Size = new System.Drawing.Size(265, 245);
            this.STHSpell.TabIndex = 25;
            this.STHSpell.TabStop = false;
            this.STHSpell.Text = "Single Target Healing Spell";
            // 
            // FlashofLightHp
            // 
            this.FlashofLightHp.AutoSize = true;
            this.FlashofLightHp.Location = new System.Drawing.Point(6, 31);
            this.FlashofLightHp.Name = "FlashofLightHp";
            this.FlashofLightHp.Size = new System.Drawing.Size(133, 17);
            this.FlashofLightHp.TabIndex = 9;
            this.FlashofLightHp.Text = "Flash of Light HP";
            // 
            // FlashofLightHpValue
            // 
            this.FlashofLightHpValue.Location = new System.Drawing.Point(211, 28);
            this.FlashofLightHpValue.Name = "FlashofLightHpValue";
            this.FlashofLightHpValue.Size = new System.Drawing.Size(43, 25);
            this.FlashofLightHpValue.TabIndex = 6;
            // 
            // HolyLightHpValue
            // 
            this.HolyLightHpValue.Location = new System.Drawing.Point(211, 57);
            this.HolyLightHpValue.Name = "HolyLightHpValue";
            this.HolyLightHpValue.Size = new System.Drawing.Size(43, 25);
            this.HolyLightHpValue.TabIndex = 7;
            // 
            // HolyShockHpValue
            // 
            this.HolyShockHpValue.Location = new System.Drawing.Point(211, 86);
            this.HolyShockHpValue.Name = "HolyShockHpValue";
            this.HolyShockHpValue.Size = new System.Drawing.Size(43, 25);
            this.HolyShockHpValue.TabIndex = 8;
            // 
            // HolyLightHp
            // 
            this.HolyLightHp.AutoSize = true;
            this.HolyLightHp.Location = new System.Drawing.Point(6, 60);
            this.HolyLightHp.Name = "HolyLightHp";
            this.HolyLightHp.Size = new System.Drawing.Size(111, 17);
            this.HolyLightHp.TabIndex = 10;
            this.HolyLightHp.Text = "Holy Light HP";
            // 
            // HolyShockHp
            // 
            this.HolyShockHp.AutoSize = true;
            this.HolyShockHp.Location = new System.Drawing.Point(6, 89);
            this.HolyShockHp.Name = "HolyShockHp";
            this.HolyShockHp.Size = new System.Drawing.Size(120, 17);
            this.HolyShockHp.TabIndex = 11;
            this.HolyShockHp.Text = "Holy Shock HP";
            // 
            // LightoftheMartyrHpValue
            // 
            this.LightoftheMartyrHpValue.Location = new System.Drawing.Point(211, 117);
            this.LightoftheMartyrHpValue.Name = "LightoftheMartyrHpValue";
            this.LightoftheMartyrHpValue.Size = new System.Drawing.Size(43, 25);
            this.LightoftheMartyrHpValue.TabIndex = 12;
            // 
            // LightoftheMartyrHp
            // 
            this.LightoftheMartyrHp.AutoSize = true;
            this.LightoftheMartyrHp.Location = new System.Drawing.Point(6, 120);
            this.LightoftheMartyrHp.Name = "LightoftheMartyrHp";
            this.LightoftheMartyrHp.Size = new System.Drawing.Size(168, 17);
            this.LightoftheMartyrHp.TabIndex = 13;
            this.LightoftheMartyrHp.Text = "Light of the Martyr HP";
            // 
            // BestowFaithHp
            // 
            this.BestowFaithHp.AutoSize = true;
            this.BestowFaithHp.Location = new System.Drawing.Point(6, 151);
            this.BestowFaithHp.Name = "BestowFaithHp";
            this.BestowFaithHp.Size = new System.Drawing.Size(127, 17);
            this.BestowFaithHp.TabIndex = 14;
            this.BestowFaithHp.Text = "Bestow Faith HP";
            // 
            // BestowFaithHpValue
            // 
            this.BestowFaithHpValue.Location = new System.Drawing.Point(211, 148);
            this.BestowFaithHpValue.Name = "BestowFaithHpValue";
            this.BestowFaithHpValue.Size = new System.Drawing.Size(43, 25);
            this.BestowFaithHpValue.TabIndex = 15;
            // 
            // BlessingofSacrificeHp
            // 
            this.BlessingofSacrificeHp.AutoSize = true;
            this.BlessingofSacrificeHp.Location = new System.Drawing.Point(6, 179);
            this.BlessingofSacrificeHp.Name = "BlessingofSacrificeHp";
            this.BlessingofSacrificeHp.Size = new System.Drawing.Size(182, 17);
            this.BlessingofSacrificeHp.TabIndex = 16;
            this.BlessingofSacrificeHp.Text = "Blessing of Sacrifice HP";
            // 
            // LayonHandsHpValue
            // 
            this.LayonHandsHpValue.Location = new System.Drawing.Point(211, 210);
            this.LayonHandsHpValue.Name = "LayonHandsHpValue";
            this.LayonHandsHpValue.Size = new System.Drawing.Size(43, 25);
            this.LayonHandsHpValue.TabIndex = 21;
            // 
            // BlessingofSacrificeHpValue
            // 
            this.BlessingofSacrificeHpValue.Location = new System.Drawing.Point(211, 179);
            this.BlessingofSacrificeHpValue.Name = "BlessingofSacrificeHpValue";
            this.BlessingofSacrificeHpValue.Size = new System.Drawing.Size(43, 25);
            this.BlessingofSacrificeHpValue.TabIndex = 20;
            // 
            // LayonHandsHp
            // 
            this.LayonHandsHp.AutoSize = true;
            this.LayonHandsHp.Location = new System.Drawing.Point(6, 210);
            this.LayonHandsHp.Name = "LayonHandsHp";
            this.LayonHandsHp.Size = new System.Drawing.Size(133, 17);
            this.LayonHandsHp.TabIndex = 17;
            this.LayonHandsHp.Text = "Lay on Hands HP";
            // 
            // groupBoxGeneralHealing
            // 
            this.groupBoxGeneralHealing.Controls.Add(this.minHealingHP);
            this.groupBoxGeneralHealing.Controls.Add(this.minHealingHPValue);
            this.groupBoxGeneralHealing.Controls.Add(this.minTankPriorityHPValue);
            this.groupBoxGeneralHealing.Controls.Add(this.minSelfPriorityHPValue);
            this.groupBoxGeneralHealing.Controls.Add(this.minTankPriorityHP);
            this.groupBoxGeneralHealing.Controls.Add(this.minSelfPriorityHP);
            this.groupBoxGeneralHealing.Location = new System.Drawing.Point(9, 19);
            this.groupBoxGeneralHealing.Name = "groupBoxGeneralHealing";
            this.groupBoxGeneralHealing.Size = new System.Drawing.Size(265, 126);
            this.groupBoxGeneralHealing.TabIndex = 24;
            this.groupBoxGeneralHealing.TabStop = false;
            this.groupBoxGeneralHealing.Text = "General Healing Settings";
            // 
            // minHealingHP
            // 
            this.minHealingHP.AutoSize = true;
            this.minHealingHP.Location = new System.Drawing.Point(6, 32);
            this.minHealingHP.Name = "minHealingHP";
            this.minHealingHP.Size = new System.Drawing.Size(163, 17);
            this.minHealingHP.TabIndex = 3;
            this.minHealingHP.Text = "Minimum Healing HP";
            // 
            // minHealingHPValue
            // 
            this.minHealingHPValue.Location = new System.Drawing.Point(211, 29);
            this.minHealingHPValue.Name = "minHealingHPValue";
            this.minHealingHPValue.Size = new System.Drawing.Size(43, 25);
            this.minHealingHPValue.TabIndex = 0;
            // 
            // minTankPriorityHPValue
            // 
            this.minTankPriorityHPValue.Location = new System.Drawing.Point(211, 61);
            this.minTankPriorityHPValue.Name = "minTankPriorityHPValue";
            this.minTankPriorityHPValue.Size = new System.Drawing.Size(43, 25);
            this.minTankPriorityHPValue.TabIndex = 1;
            // 
            // minSelfPriorityHPValue
            // 
            this.minSelfPriorityHPValue.Location = new System.Drawing.Point(211, 92);
            this.minSelfPriorityHPValue.Name = "minSelfPriorityHPValue";
            this.minSelfPriorityHPValue.Size = new System.Drawing.Size(43, 25);
            this.minSelfPriorityHPValue.TabIndex = 2;
            // 
            // minTankPriorityHP
            // 
            this.minTankPriorityHP.AutoSize = true;
            this.minTankPriorityHP.Location = new System.Drawing.Point(6, 64);
            this.minTankPriorityHP.Name = "minTankPriorityHP";
            this.minTankPriorityHP.Size = new System.Drawing.Size(199, 17);
            this.minTankPriorityHP.TabIndex = 4;
            this.minTankPriorityHP.Text = "Minimum Tank Priority HP";
            // 
            // minSelfPriorityHP
            // 
            this.minSelfPriorityHP.AutoSize = true;
            this.minSelfPriorityHP.Location = new System.Drawing.Point(6, 95);
            this.minSelfPriorityHP.Name = "minSelfPriorityHP";
            this.minSelfPriorityHP.Size = new System.Drawing.Size(192, 17);
            this.minSelfPriorityHP.TabIndex = 5;
            this.minSelfPriorityHP.Text = "Minimum Self Priority HP";
            // 
            // TalentTab
            // 
            this.TalentTab.BackColor = System.Drawing.Color.White;
            this.TalentTab.Controls.Add(this.BeaconofVirtue);
            this.TalentTab.Controls.Add(this.BeaconofFaith);
            this.TalentTab.Controls.Add(this.JudgmentOfLight);
            this.TalentTab.Controls.Add(this.BestowFaith);
            this.TalentTab.Location = new System.Drawing.Point(4, 26);
            this.TalentTab.Name = "TalentTab";
            this.TalentTab.Padding = new System.Windows.Forms.Padding(3);
            this.TalentTab.Size = new System.Drawing.Size(713, 456);
            this.TalentTab.TabIndex = 1;
            this.TalentTab.Text = "Talents";
            // 
            // BeaconofVirtue
            // 
            this.BeaconofVirtue.AutoSize = true;
            this.BeaconofVirtue.BackColor = System.Drawing.Color.White;
            this.BeaconofVirtue.Location = new System.Drawing.Point(180, 75);
            this.BeaconofVirtue.Name = "BeaconofVirtue";
            this.BeaconofVirtue.Size = new System.Drawing.Size(150, 21);
            this.BeaconofVirtue.TabIndex = 3;
            this.BeaconofVirtue.Text = "Beacon of Virtue";
            this.BeaconofVirtue.UseVisualStyleBackColor = false;
            // 
            // BeaconofFaith
            // 
            this.BeaconofFaith.AutoSize = true;
            this.BeaconofFaith.BackColor = System.Drawing.Color.White;
            this.BeaconofFaith.Location = new System.Drawing.Point(20, 75);
            this.BeaconofFaith.Name = "BeaconofFaith";
            this.BeaconofFaith.Size = new System.Drawing.Size(142, 21);
            this.BeaconofFaith.TabIndex = 2;
            this.BeaconofFaith.Text = "Beacon of Faith";
            this.BeaconofFaith.UseVisualStyleBackColor = false;
            // 
            // JudgmentOfLight
            // 
            this.JudgmentOfLight.AutoSize = true;
            this.JudgmentOfLight.BackColor = System.Drawing.Color.White;
            this.JudgmentOfLight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.JudgmentOfLight.Location = new System.Drawing.Point(20, 52);
            this.JudgmentOfLight.Name = "JudgmentOfLight";
            this.JudgmentOfLight.Size = new System.Drawing.Size(158, 21);
            this.JudgmentOfLight.TabIndex = 1;
            this.JudgmentOfLight.Text = "Judgment Of Light";
            this.JudgmentOfLight.UseVisualStyleBackColor = false;
            // 
            // BestowFaith
            // 
            this.BestowFaith.AutoSize = true;
            this.BestowFaith.BackColor = System.Drawing.Color.White;
            this.BestowFaith.Location = new System.Drawing.Point(20, 29);
            this.BestowFaith.Name = "BestowFaith";
            this.BestowFaith.Size = new System.Drawing.Size(120, 21);
            this.BestowFaith.TabIndex = 0;
            this.BestowFaith.Text = "Bestow Faith";
            this.BestowFaith.UseVisualStyleBackColor = false;
            // 
            // AuraMasteryLMP
            // 
            this.AuraMasteryLMP.Location = new System.Drawing.Point(293, 124);
            this.AuraMasteryLMP.Name = "AuraMasteryLMP";
            this.AuraMasteryLMP.Size = new System.Drawing.Size(43, 25);
            this.AuraMasteryLMP.TabIndex = 43;
            // 
            // AuraMasteryEnable
            // 
            this.AuraMasteryEnable.AutoSize = true;
            this.AuraMasteryEnable.Location = new System.Drawing.Point(361, 129);
            this.AuraMasteryEnable.Name = "AuraMasteryEnable";
            this.AuraMasteryEnable.Size = new System.Drawing.Size(15, 14);
            this.AuraMasteryEnable.TabIndex = 42;
            this.AuraMasteryEnable.UseVisualStyleBackColor = true;
            // 
            // AuraMasteryLMR
            // 
            this.AuraMasteryLMR.Location = new System.Drawing.Point(243, 124);
            this.AuraMasteryLMR.Name = "AuraMasteryLMR";
            this.AuraMasteryLMR.Size = new System.Drawing.Size(43, 25);
            this.AuraMasteryLMR.TabIndex = 41;
            // 
            // labelAuraMastery
            // 
            this.labelAuraMastery.AutoSize = true;
            this.labelAuraMastery.Location = new System.Drawing.Point(6, 127);
            this.labelAuraMastery.Name = "labelAuraMastery";
            this.labelAuraMastery.Size = new System.Drawing.Size(102, 17);
            this.labelAuraMastery.TabIndex = 39;
            this.labelAuraMastery.Text = "Aura Mastery";
            // 
            // LightofDawnLMP
            // 
            this.LightofDawnLMP.Location = new System.Drawing.Point(293, 157);
            this.LightofDawnLMP.Name = "LightofDawnLMP";
            this.LightofDawnLMP.Size = new System.Drawing.Size(43, 25);
            this.LightofDawnLMP.TabIndex = 42;
            // 
            // LightofDawnEnable
            // 
            this.LightofDawnEnable.AutoSize = true;
            this.LightofDawnEnable.Location = new System.Drawing.Point(361, 162);
            this.LightofDawnEnable.Name = "LightofDawnEnable";
            this.LightofDawnEnable.Size = new System.Drawing.Size(15, 14);
            this.LightofDawnEnable.TabIndex = 41;
            this.LightofDawnEnable.UseVisualStyleBackColor = true;
            // 
            // LightofDawnLMR
            // 
            this.LightofDawnLMR.Location = new System.Drawing.Point(243, 157);
            this.LightofDawnLMR.Name = "LightofDawnLMR";
            this.LightofDawnLMR.Size = new System.Drawing.Size(43, 25);
            this.LightofDawnLMR.TabIndex = 40;
            // 
            // LightofDawn
            // 
            this.LightofDawn.AutoSize = true;
            this.LightofDawn.Location = new System.Drawing.Point(6, 160);
            this.LightofDawn.Name = "LightofDawn";
            this.LightofDawn.Size = new System.Drawing.Size(108, 17);
            this.LightofDawn.TabIndex = 38;
            this.LightofDawn.Text = "Light of Dawn";
            // 
            // DispelTAB
            // 
            this.DispelTAB.Controls.Add(this.DispeSpelllistgroup);
            this.DispelTAB.Controls.Add(this.generalDispelSettings);
            this.DispelTAB.Location = new System.Drawing.Point(4, 26);
            this.DispelTAB.Name = "DispelTAB";
            this.DispelTAB.Padding = new System.Windows.Forms.Padding(3);
            this.DispelTAB.Size = new System.Drawing.Size(713, 456);
            this.DispelTAB.TabIndex = 2;
            this.DispelTAB.Text = "Dispel Settings";
            this.DispelTAB.UseVisualStyleBackColor = true;
            // 
            // generalDispelSettings
            // 
            this.generalDispelSettings.Controls.Add(this.dispelmode);
            this.generalDispelSettings.Controls.Add(this.textBox8);
            this.generalDispelSettings.Controls.Add(this.textBox7);
            this.generalDispelSettings.Controls.Add(this.comboBox1);
            this.generalDispelSettings.Controls.Add(this.dispelrandomwait);
            this.generalDispelSettings.Controls.Add(this.dispelwaittime);
            this.generalDispelSettings.Location = new System.Drawing.Point(20, 18);
            this.generalDispelSettings.Name = "generalDispelSettings";
            this.generalDispelSettings.Size = new System.Drawing.Size(672, 103);
            this.generalDispelSettings.TabIndex = 0;
            this.generalDispelSettings.TabStop = false;
            this.generalDispelSettings.Text = "General Dispel Settings";
            // 
            // dispelwaittime
            // 
            this.dispelwaittime.AutoSize = true;
            this.dispelwaittime.Location = new System.Drawing.Point(7, 39);
            this.dispelwaittime.Name = "dispelwaittime";
            this.dispelwaittime.Size = new System.Drawing.Size(186, 17);
            this.dispelwaittime.TabIndex = 0;
            this.dispelwaittime.Text = "Minimum Wait Time (ms)";
            // 
            // dispelrandomwait
            // 
            this.dispelrandomwait.AutoSize = true;
            this.dispelrandomwait.Location = new System.Drawing.Point(7, 70);
            this.dispelrandomwait.Name = "dispelrandomwait";
            this.dispelrandomwait.Size = new System.Drawing.Size(179, 17);
            this.dispelrandomwait.TabIndex = 1;
            this.dispelrandomwait.Text = "Random Wait Time (ms)";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Disabled",
            "Dispel All",
            "Dispel From List"});
            this.comboBox1.Location = new System.Drawing.Point(520, 39);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 25);
            this.comboBox1.TabIndex = 2;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(200, 39);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(60, 25);
            this.textBox7.TabIndex = 3;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(200, 70);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(60, 25);
            this.textBox8.TabIndex = 4;
            // 
            // dispelmode
            // 
            this.dispelmode.AutoSize = true;
            this.dispelmode.Location = new System.Drawing.Point(414, 42);
            this.dispelmode.Name = "dispelmode";
            this.dispelmode.Size = new System.Drawing.Size(100, 17);
            this.dispelmode.TabIndex = 5;
            this.dispelmode.Text = "Dispel Mode";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.SpellID});
            this.listView1.GridLines = true;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2});
            this.listView1.Location = new System.Drawing.Point(16, 50);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(235, 257);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Spell Name";
            this.columnHeader1.Width = 150;
            // 
            // SpellID
            // 
            this.SpellID.Text = "SpellID";
            this.SpellID.Width = 80;
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(16, 24);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(152, 25);
            this.textBox9.TabIndex = 2;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(169, 24);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(80, 25);
            this.textBox10.TabIndex = 3;
            // 
            // dispelPLUS
            // 
            this.dispelPLUS.Font = new System.Drawing.Font("Lucida Sans", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dispelPLUS.Location = new System.Drawing.Point(252, 24);
            this.dispelPLUS.Name = "dispelPLUS";
            this.dispelPLUS.Size = new System.Drawing.Size(59, 25);
            this.dispelPLUS.TabIndex = 4;
            this.dispelPLUS.Text = "Add";
            this.dispelPLUS.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.dispelPLUS.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Lucida Sans", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(252, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 25);
            this.button1.TabIndex = 5;
            this.button1.Text = "Del";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // DispeSpelllistgroup
            // 
            this.DispeSpelllistgroup.Controls.Add(this.textBox10);
            this.DispeSpelllistgroup.Controls.Add(this.button1);
            this.DispeSpelllistgroup.Controls.Add(this.listView1);
            this.DispeSpelllistgroup.Controls.Add(this.dispelPLUS);
            this.DispeSpelllistgroup.Controls.Add(this.textBox9);
            this.DispeSpelllistgroup.Location = new System.Drawing.Point(20, 127);
            this.DispeSpelllistgroup.Name = "DispeSpelllistgroup";
            this.DispeSpelllistgroup.Size = new System.Drawing.Size(328, 321);
            this.DispeSpelllistgroup.TabIndex = 6;
            this.DispeSpelllistgroup.TabStop = false;
            this.DispeSpelllistgroup.Text = "Spell List";
            // 
            // groupBoxInstruction
            // 
            this.groupBoxInstruction.Controls.Add(this.richTextBox1);
            this.groupBoxInstruction.Location = new System.Drawing.Point(294, 257);
            this.groupBoxInstruction.Name = "groupBoxInstruction";
            this.groupBoxInstruction.Size = new System.Drawing.Size(411, 159);
            this.groupBoxInstruction.TabIndex = 27;
            this.groupBoxInstruction.TabStop = false;
            this.groupBoxInstruction.Text = "Instructions";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(9, 24);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(390, 125);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "- LMR : number of low member in raid\n- LMP : number of low member in party";
            // 
            // BeaconOfVirtueHPValue
            // 
            this.BeaconOfVirtueHPValue.FormattingEnabled = true;
            this.BeaconOfVirtueHPValue.Items.AddRange(new object[] {
            "80",
            "60"});
            this.BeaconOfVirtueHPValue.Location = new System.Drawing.Point(170, 57);
            this.BeaconOfVirtueHPValue.Name = "BeaconOfVirtueHPValue";
            this.BeaconOfVirtueHPValue.Size = new System.Drawing.Size(67, 25);
            this.BeaconOfVirtueHPValue.TabIndex = 44;
            // 
            // TyrsDeliveranceHPValue
            // 
            this.TyrsDeliveranceHPValue.FormattingEnabled = true;
            this.TyrsDeliveranceHPValue.Items.AddRange(new object[] {
            "80",
            "60"});
            this.TyrsDeliveranceHPValue.Location = new System.Drawing.Point(170, 88);
            this.TyrsDeliveranceHPValue.Name = "TyrsDeliveranceHPValue";
            this.TyrsDeliveranceHPValue.Size = new System.Drawing.Size(67, 25);
            this.TyrsDeliveranceHPValue.TabIndex = 45;
            // 
            // AuraMasteryHPValue
            // 
            this.AuraMasteryHPValue.FormattingEnabled = true;
            this.AuraMasteryHPValue.Items.AddRange(new object[] {
            "80",
            "60"});
            this.AuraMasteryHPValue.Location = new System.Drawing.Point(170, 124);
            this.AuraMasteryHPValue.Name = "AuraMasteryHPValue";
            this.AuraMasteryHPValue.Size = new System.Drawing.Size(67, 25);
            this.AuraMasteryHPValue.TabIndex = 46;
            // 
            // LightofDawnHPValue
            // 
            this.LightofDawnHPValue.FormattingEnabled = true;
            this.LightofDawnHPValue.Items.AddRange(new object[] {
            "80",
            "60"});
            this.LightofDawnHPValue.Location = new System.Drawing.Point(170, 155);
            this.LightofDawnHPValue.Name = "LightofDawnHPValue";
            this.LightofDawnHPValue.Size = new System.Drawing.Size(67, 25);
            this.LightofDawnHPValue.TabIndex = 47;
            // 
            // fmflexHpalSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(723, 487);
            this.Controls.Add(this.TABControl);
            this.Name = "fmflexHpalSettings";
            this.Text = "FmFlex Holy Paladin Settings";
            this.TABControl.ResumeLayout(false);
            this.HealSettingTab.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.STHSpell.ResumeLayout(false);
            this.STHSpell.PerformLayout();
            this.groupBoxGeneralHealing.ResumeLayout(false);
            this.groupBoxGeneralHealing.PerformLayout();
            this.TalentTab.ResumeLayout(false);
            this.TalentTab.PerformLayout();
            this.DispelTAB.ResumeLayout(false);
            this.generalDispelSettings.ResumeLayout(false);
            this.generalDispelSettings.PerformLayout();
            this.DispeSpelllistgroup.ResumeLayout(false);
            this.DispeSpelllistgroup.PerformLayout();
            this.groupBoxInstruction.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TABControl;
        private System.Windows.Forms.TabPage HealSettingTab;
        private System.Windows.Forms.Label minSelfPriorityHP;
        private System.Windows.Forms.Label minTankPriorityHP;
        private System.Windows.Forms.Label minHealingHP;
        private System.Windows.Forms.Label HolyShockHp;
        private System.Windows.Forms.Label HolyLightHp;
        private System.Windows.Forms.Label FlashofLightHp;
        private System.Windows.Forms.Label BestowFaithHp;
        private System.Windows.Forms.Label LightoftheMartyrHp;
        private System.Windows.Forms.Label TyrsDeliveranceHP;
        private System.Windows.Forms.Label BeaconOfVirtueHp;
        private System.Windows.Forms.Label LayonHandsHp;
        private System.Windows.Forms.Label BlessingofSacrificeHp;
        private System.Windows.Forms.GroupBox STHSpell;
        private System.Windows.Forms.GroupBox groupBoxGeneralHealing;
        public System.Windows.Forms.TabPage TalentTab;
        public System.Windows.Forms.CheckBox BeaconofVirtue;
        public System.Windows.Forms.CheckBox BeaconofFaith;
        public System.Windows.Forms.CheckBox JudgmentOfLight;
        public System.Windows.Forms.CheckBox BestowFaith;
        public System.Windows.Forms.TextBox minSelfPriorityHPValue;
        public System.Windows.Forms.TextBox minTankPriorityHPValue;
        public System.Windows.Forms.TextBox minHealingHPValue;
        public System.Windows.Forms.TextBox HolyShockHpValue;
        public System.Windows.Forms.TextBox HolyLightHpValue;
        public System.Windows.Forms.TextBox FlashofLightHpValue;
        public System.Windows.Forms.TextBox BestowFaithHpValue;
        public System.Windows.Forms.TextBox LightoftheMartyrHpValue;
        public System.Windows.Forms.TextBox LayonHandsHpValue;
        public System.Windows.Forms.TextBox BlessingofSacrificeHpValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox TyrsDeliveranceEnable;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox TyrsDeliveranceLMR;
        public System.Windows.Forms.TextBox BeaconOfVirtueLMR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox TyrsDeliveranceLMP;
        public System.Windows.Forms.TextBox BeaconOfVirtueLMP;
        public System.Windows.Forms.TextBox LightofDawnLMP;
        public System.Windows.Forms.TextBox AuraMasteryLMP;
        public System.Windows.Forms.CheckBox LightofDawnEnable;
        public System.Windows.Forms.CheckBox AuraMasteryEnable;
        public System.Windows.Forms.TextBox LightofDawnLMR;
        public System.Windows.Forms.TextBox AuraMasteryLMR;
        private System.Windows.Forms.Label LightofDawn;
        private System.Windows.Forms.Label labelAuraMastery;
        private System.Windows.Forms.TabPage DispelTAB;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button dispelPLUS;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.ListView listView1;
        public System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader SpellID;
        private System.Windows.Forms.GroupBox generalDispelSettings;
        private System.Windows.Forms.Label dispelmode;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label dispelrandomwait;
        private System.Windows.Forms.Label dispelwaittime;
        private System.Windows.Forms.GroupBox DispeSpelllistgroup;
        private System.Windows.Forms.GroupBox groupBoxInstruction;
        private System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.ComboBox LightofDawnHPValue;
        public System.Windows.Forms.ComboBox AuraMasteryHPValue;
        public System.Windows.Forms.ComboBox TyrsDeliveranceHPValue;
        public System.Windows.Forms.ComboBox BeaconOfVirtueHPValue;

    }
}

/*
[AddonDetails.db]
AddonAuthor=Tyalieva
AddonName=RGB
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,19750,Flash of Light,D1
Spell,82326,Holy Light,D2
Spell,183998,Light of the Martyr,D4
Spell,20473,Holy Shock,D3
Spell,223306,Bestow Faith,D5
Spell,200025,Beacon of Virtue,F10
Spell,53563,Beacon of Light,F10
Spell,633,Lay on Hands,D9
Spell,20271,Judgment,D0
Spell,35395,Crusader Strike,F8
Spell,26573,Consecration,F12
Spell,85222,Light of Dawn,A
Spell,1022,Blessing of Protection,F3
Spell,6940,Blessing of Sacrifice,F7
Spell,200652,Tyrs Deliverance,F9 
Spell,31821,Aura Mastery,F4
Spell,4987,cleanse,OemOpenBrackets
Spell,0,target0,T
Spell,1,target1,U
Spell,2,target2,I
Spell,3,stopcasting,NumPad5
Aura,642,Divine Shield
Aura,498,Divine Protection
Aura,200025,Beacon of Virtue
Aura,214222,Judgment
Aura,223306,Bestow Faith
Aura,25771,Forbearance
Aura,53563,Beacon of Light
*/
