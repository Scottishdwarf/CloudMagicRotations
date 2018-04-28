//smartie@kissmyass.com
//Changelog
// v1.0 First release
// v1.1 added bear rota
// v1.2 some small fixes
// v1.3 added Fiery Red Maimers legendary and T21 4 set Bonus
// v1.4 added levelchecks
// v1.5 recode of AoE rota and some minor bug fixes
// v1.6 added Ashamane`s Frenzy to use on cd (checkbox)
// v1.7 T21 4set fix
// v1.8 some small fixes
// v1.85 bite fix
// v1.9 opener fix - Lunar Inspirtation Fix
// v2.0 massive cleanup
// v2.1 Rip Fix
// v2.2 Sabertooth Talent fix and some other Bug Fixes
// v2.3 added cleave rotation and fixed a stuck problem
// v2.4 fixed rip hopefully
// v2.5 massive cleanup and improving bleed and savage roar uptimes
// v2.6 T21 4set Fix Bite Fix
// v2.7 improved Tigers fury usage and Regroth fix
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using CloudMagic.WoWChecks;
using CloudMagic.Logs;
using CloudMagic.ControlMethod;

namespace CloudMagic.Rotation
{
    public class smarties_Kitty : CombatRoutine
    {
        public override int SINGLE 
		{
			get 
			{ 
				return 1; 
			} 
		}
		public override int CLEAVE 
		{ 
			get 
			{ 
				return 2;
			} 
		}
        public override int AOE 
		{ 
			get 
			{ 
				return 3; 
			} 
		}
		public override int AoE_Range  
		{
			get 
			{ 
				return 10; 
			} 
		}
        public override int Interrupt_Ability_Id 
		{ 
			get 
			{ 
				return 106839; 
			}
		}
		private float GCD
        {
            get
            {

                return (150f / (1f + (WoW.HastePercent / 100f)));

            }
        }
        private static readonly Stopwatch pullwatch = new Stopwatch();
        private static readonly Stopwatch ripunbuffed = new Stopwatch();
        private static readonly Stopwatch ripbuffed = new Stopwatch();
        public override string Name
        {
            get
            {
                return "Kitty Rotation";
            }
        }
        public override string Class
        {
            get
            {
                return "Druid";
            }
        }
        public override Form SettingsForm 
		{ 
			get; 
			set; 
		}
        public SettingsFormDFF SettingsFormDFF 
		{ 
			get; 
			set; 
		}
        public static int cooldownKey
        {
            get
            {
                var cooldownKey = ConfigFile.ReadValue("KittyDrood", "cooldownKey").Trim();
                if (cooldownKey != "")
                {
                    return Convert.ToInt32(cooldownKey);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("KittyDrood", "cooldownKey", value.ToString()); }
        }
        public static int cooldownModifier
        {
            get
            {
                var cooldownModifier = ConfigFile.ReadValue("KittyDrood", "cooldownModifier").Trim();
                if (cooldownModifier != "")
                {
                    return Convert.ToInt32(cooldownModifier);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("KittyDrood", "cooldownModifier", value.ToString()); }
        }
        public static string cooldownHotKeyString
        {
            get
            {
                var cooldownHotKeyString = ConfigFile.ReadValue("KittyDrood", "cooldownHotKeyString").Trim();

                if (cooldownHotKeyString != "")
                {
                    return cooldownHotKeyString;
                }

                return "Set Roar Key";
            }
            set { ConfigFile.WriteValue("KittyDrood", "cooldownHotKeyString", value); }
        }
        public static bool ischeckHotkeysKittyRegrowth
        {
            get
            {
                var ischeckHotkeysKittyRegrowth = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittyRegrowth").Trim();

                if (ischeckHotkeysKittyRegrowth != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittyRegrowth);
                }

                return true;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittyRegrowth", value.ToString()); }
        }
        public static int KittyRegrowthPercent
        {
            get
            {
                var KittyRegrowthPercent = ConfigFile.ReadValue("KittyDrood", "KittyRegrowthPercent").Trim();
                if (KittyRegrowthPercent != "")
                {
                    return Convert.ToInt32(KittyRegrowthPercent);
                }

                return 70;
            }
            set { ConfigFile.WriteValue("KittyDrood", "KittyRegrowthPercent", value.ToString()); }
        }
        public static bool ischeckHotkeysKittySurvivalInstincts
        {
            get
            {
                var ischeckHotkeysKittySurvivalInstincts = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittySurvivalInstincts").Trim();

                if (ischeckHotkeysKittySurvivalInstincts != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittySurvivalInstincts);
                }

                return true;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittySurvivalInstincts", value.ToString()); }
        }
        public static int KittySurvivalInstinctsPercent
        {
            get
            {
                var KittySurvivalInstinctsPercent = ConfigFile.ReadValue("KittyDrood", "KittySurvivalInstinctsPercent").Trim();
                if (KittySurvivalInstinctsPercent != "")
                {
                    return Convert.ToInt32(KittySurvivalInstinctsPercent);
                }

                return 20;
            }
            set { ConfigFile.WriteValue("KittyDrood", "KittySurvivalInstinctsPercent", value.ToString()); }
        }
        public static bool ischeckHotkeysKittyRenewal
        {
            get
            {
                var ischeckHotkeysKittyRenewal = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittyRenewal").Trim();

                if (ischeckHotkeysKittyRenewal != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittyRenewal);
                }

                return true;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittyRenewal", value.ToString()); }
        }
        public static int KittyRenewalPercent
        {
            get
            {
                var KittyRenewalPercent = ConfigFile.ReadValue("KittyDrood", "KittyRenewalPercent").Trim();
                if (KittyRenewalPercent != "")
                {
                    return Convert.ToInt32(KittyRenewalPercent);
                }

                return 35;
            }
            set { ConfigFile.WriteValue("KittyDrood", "KittyRenewalPercent", value.ToString()); }
        }
        public static bool ischeckHotkeysKittyHealthstone
        {
            get
            {
                var ischeckHotkeysKittyHealthstone = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittyHealthstone").Trim();

                if (ischeckHotkeysKittyHealthstone != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittyHealthstone);
                }

                return true;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittyHealthstone", value.ToString()); }
        }
        public static int KittyHealthstonePercent
        {
            get
            {
                var KittyHealthstonePercent = ConfigFile.ReadValue("KittyDrood", "KittyHealthstonePercent").Trim();
                if (KittyHealthstonePercent != "")
                {
                    return Convert.ToInt32(KittyHealthstonePercent);
                }

                return 40;
            }
            set { ConfigFile.WriteValue("KittyDrood", "KittyHealthstonePercent", value.ToString()); }
        }
        public static bool ischeckHotkeysKittyLuffaWrappings
        {
            get
            {
                var ischeckHotkeysKittyLuffaWrappings = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittyLuffaWrappings").Trim();

                if (ischeckHotkeysKittyLuffaWrappings != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittyLuffaWrappings);
                }

                return false;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittyLuffaWrappings", value.ToString()); }
        }	
        public static bool ischeckHotkeysKittyKick
        {
            get
            {
                var ischeckHotkeysKittyKick = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittyKick").Trim();

                if (ischeckHotkeysKittyKick != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittyKick);
                }

                return true;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittyKick", value.ToString()); }
        }
        public static int KittyKickPercent
        {
            get
            {
                var KittyKickPercent = ConfigFile.ReadValue("KittyDrood", "KittyKickPercent").Trim();
                if (KittyKickPercent != "")
                {
                    return Convert.ToInt32(KittyKickPercent);
                }

                return 65;
            }
            set { ConfigFile.WriteValue("KittyDrood", "KittyKickPercent", value.ToString()); }
        }
        public static bool ischeckHotkeysKittyIncarnation
        {
            get
            {
                var ischeckHotkeysKittyIncarnation = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittyIncarnation").Trim();

                if (ischeckHotkeysKittyIncarnation != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittyIncarnation);
                }

                return true;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittyIncarnation", value.ToString()); }
        }
        public static bool ischeckHotkeysKittyAshamane
        {
            get
            {
                var ischeckHotkeysKittyAshamane = ConfigFile.ReadValue("KittyDrood", "ischeckHotkeysKittyAshamane").Trim();

                if (ischeckHotkeysKittyAshamane != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysKittyAshamane);
                }

                return true;
            }
            set { ConfigFile.WriteValue("KittyDrood", "ischeckHotkeysKittyAshamane", value.ToString()); }
        }
        public override void Initialize()
        {
            Log.Write("Welcome to the Kitty Druid Rota v2.7 by smartie", Color.Green);
	        Log.Write("All Talents supported and auto detected", Color.Green);
	        Log.Write("Set your Roar Key in the Settings and hold it down if you want to cast Roar", Color.Red);			
            SettingsFormDFF = new SettingsFormDFF();
            SettingsForm = SettingsFormDFF;

            SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.Text = cooldownHotKeyString;
            SettingsFormDFF.checkHotkeysKittyIncarnation.Checked = ischeckHotkeysKittyIncarnation;
            SettingsFormDFF.checkHotkeysKittyAshamane.Checked = ischeckHotkeysKittyAshamane;
            SettingsFormDFF.checkHotkeysKittyRegrowth.Checked = ischeckHotkeysKittyRegrowth;
            SettingsFormDFF.checkHotkeysKittyRegrowthPercent.Text = KittyRegrowthPercent.ToString();
            SettingsFormDFF.checkHotkeysKittySurvivalInstincts.Checked = ischeckHotkeysKittySurvivalInstincts;
            SettingsFormDFF.checkHotkeysKittySurvivalInstinctsPercent.Text = KittySurvivalInstinctsPercent.ToString();
            SettingsFormDFF.checkHotkeysKittyRenewal.Checked = ischeckHotkeysKittyRenewal;
            SettingsFormDFF.checkHotkeysKittyRenewalPercent.Text = KittyRenewalPercent.ToString();
            SettingsFormDFF.checkHotkeysKittyHealthstone.Checked = ischeckHotkeysKittyHealthstone;
            SettingsFormDFF.checkHotkeysKittyHealthstonePercent.Text = KittyHealthstonePercent.ToString();
            SettingsFormDFF.checkHotkeysKittyLuffaWrappings.Checked = ischeckHotkeysKittyLuffaWrappings;
            SettingsFormDFF.checkHotkeysKittyKick.Checked = ischeckHotkeysKittyKick;
            SettingsFormDFF.checkHotkeysKittyKickPercent.Text = KittyKickPercent.ToString();
            SettingsFormDFF.checkHotkeysKittyLuffaWrappings.CheckedChanged += ischeckHotkeysKittyLuffaWrappings_Click;
            SettingsFormDFF.checkHotkeysKittyKick.CheckedChanged += ischeckHotkeysKittyKick_Click;
            SettingsFormDFF.checkHotkeysKittyKickPercent.TextChanged += isCheckHotkeysKittyKickPercent_Click;
            SettingsFormDFF.checkHotkeysKittyRegrowth.CheckedChanged += ischeckHotkeysKittyRegrowth_Click;
            SettingsFormDFF.checkHotkeysKittyRegrowthPercent.TextChanged += ischeckHotkeysKittyRegrowthPercent_Click;
            SettingsFormDFF.checkHotkeysKittySurvivalInstincts.CheckedChanged += ischeckHotkeysKittySurvivalInstincts_Click;
            SettingsFormDFF.checkHotkeysKittySurvivalInstinctsPercent.TextChanged += ischeckHotkeysKittySurvivalInstinctsPercent_Click;
            SettingsFormDFF.checkHotkeysKittyRenewal.CheckedChanged += ischeckHotkeysKittyRenewal_Click;
            SettingsFormDFF.checkHotkeysKittyRenewalPercent.TextChanged += ischeckHotkeysKittyRenewalPercent_Click;
            SettingsFormDFF.checkHotkeysKittyHealthstone.CheckedChanged += ischeckHotkeysKittyHealthstone_Click;
            SettingsFormDFF.checkHotkeysKittyHealthstonePercent.TextChanged += ischeckHotkeysKittyHealthstonePercent_Click;
            SettingsFormDFF.checkHotkeysKittyIncarnation.CheckedChanged += ischeckHotkeysKittyIncarnation_Click;
			SettingsFormDFF.checkHotkeysKittyAshamane.CheckedChanged += ischeckHotkeysKittyAshamane_Click;
            SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.KeyDown += KeyDown;
        }
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
                return;
            SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.Text = "Roar key : ";
            if (e.Shift)
            {
                cooldownModifier = (int)Keys.ShiftKey;
                SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.Text += Keys.Shift + " + ";
            }
            else if (e.Alt)
            {
                cooldownModifier = (int)Keys.Menu;
                SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.Text += Keys.Alt + " + ";
            }
            else if (e.Control)
            {
                cooldownModifier = (int)Keys.ControlKey;
                SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.Text += Keys.Control + " + ";
            }
            else cooldownModifier = -1;
            cooldownKey = (int)e.KeyCode;
            SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.Text += e.KeyCode;
            cooldownHotKeyString = SettingsFormDFF.btnHotkeysKittyOffensiveCooldowns.Text;
            SettingsFormDFF.checkHotkeysKittySurvivalInstinctsPercentLabel.Focus();
        }	
        private void ischeckHotkeysKittyRegrowth_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittyRegrowth = SettingsFormDFF.checkHotkeysKittyRegrowth.Checked;
        }
        private void ischeckHotkeysKittyRegrowthPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysKittyRegrowthPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                KittyRegrowthPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysKittyRegrowthPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }	
        private void ischeckHotkeysKittySurvivalInstincts_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittySurvivalInstincts = SettingsFormDFF.checkHotkeysKittySurvivalInstincts.Checked;
        }	
        private void ischeckHotkeysKittySurvivalInstinctsPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysKittySurvivalInstinctsPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                KittySurvivalInstinctsPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysKittySurvivalInstinctsPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void ischeckHotkeysKittyRenewal_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittyRenewal = SettingsFormDFF.checkHotkeysKittyRenewal.Checked;
        }
        private void ischeckHotkeysKittyRenewalPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysKittyRenewalPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                KittyRenewalPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysKittyRenewalPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void ischeckHotkeysKittyHealthstone_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittyHealthstone = SettingsFormDFF.checkHotkeysKittyHealthstone.Checked;
        }
        private void ischeckHotkeysKittyHealthstonePercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysKittyHealthstonePercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                KittyHealthstonePercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysKittyHealthstonePercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }	
        private void ischeckHotkeysKittyLuffaWrappings_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittyLuffaWrappings = SettingsFormDFF.checkHotkeysKittyLuffaWrappings.Checked;
        }	
        private void ischeckHotkeysKittyKick_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittyKick = SettingsFormDFF.checkHotkeysKittyKick.Checked;
        }
        private void isCheckHotkeysKittyKickPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysKittyKickPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                KittyKickPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysKittyKickPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
        private void ischeckHotkeysKittyIncarnation_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittyIncarnation = SettingsFormDFF.checkHotkeysKittyIncarnation.Checked;
        }
        private void ischeckHotkeysKittyAshamane_Click(object sender, EventArgs e)
        {
            ischeckHotkeysKittyAshamane = SettingsFormDFF.checkHotkeysKittyAshamane.Checked;
        }
        public override void Stop()
        {
        }
        public override void Pulse()
        {
			// Stopwatch START
			if (WoW.IsInCombat && !pullwatch.IsRunning)
			{
				pullwatch.Start();
				Log.Write("Entering Combat, Starting opener timer.", Color.Red);
			}
			// Stopwatch stop
			if (!WoW.IsInCombat && pullwatch.ElapsedMilliseconds >= 1000)
			{
				pullwatch.Reset();
				ripunbuffed.Reset();
				ripbuffed.Reset();
				Log.Write("Leaving Combat, Resetting opener timer.", Color.Red);
			}
            if (WoW.HasTarget && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted && !WoW.PlayerHasBuff("Travel Form") && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting )
            {
                if (!WoW.PlayerHasBuff("Cat Form") && !WoW.PlayerHasBuff("Bear Form"))
                {
                    WoW.CastSpell("Cat Form");
                }
				// OPEN COMBAT WITH SHADOWMELD RAKE
				if (WoW.PlayerHasBuff("Cat Form") && (WoW.PlayerHasBuff("Prowl") || WoW.PlayerHasBuff("Shadowmeld")))
				{
					if (WoW.IsSpellInRange("Rake") && WoW.Level >= 12 && WoW.CanPreCast("Rake"))
					{
						WoW.CastSpell("Rake");
						return;
					}
				}
				if (WoW.CanPreCast("Stampeding Roar") && WoW.Level >= 50 && DetectKeyPress.GetAsyncKeyState(cooldownKey) < 0 && (cooldownModifier == -1 || cooldownModifier != -1 && DetectKeyPress.GetAsyncKeyState(cooldownModifier) < 0))
				{																
					WoW.CastSpell("Stampeding Roar");
					return;
				}
				//Def Cooldowns + kick
				if (WoW.CanPreCast("Skull Bash") && WoW.Level >= 70 && WoW.IsSpellInRange("Skull Bash") && WoW.TargetIsCastingAndSpellIsInterruptible && ischeckHotkeysKittyKick && WoW.TargetPercentCast >= KittyKickPercent && !WoW.IsSpellOnCooldown("Skull Bash"))
				{
					WoW.CastSpell("Skull Bash");						
					return;
				}	
				if (WoW.PlayerHasBuff("PredatorySwiftness") && WoW.Level >= 5 && WoW.LastSpell != "Regrowth" && WoW.PlayerBuffTimeRemaining ("PredatorySwiftness") >= GCD*2 && ischeckHotkeysKittyRegrowth && WoW.HealthPercent <= KittyRegrowthPercent)
				{
					WoW.CastSpell("Regrowth");
					return;
				}
				if (!WoW.IsSpellOnCooldown("Survival Instincts") && WoW.Level >= 36 && ischeckHotkeysKittySurvivalInstincts && WoW.HealthPercent <= KittySurvivalInstinctsPercent && !WoW.PlayerHasBuff("Survival Instincts"))
				{
					WoW.CastSpell("Survival Instincts");
				}
				if (!WoW.IsSpellOnCooldown("Renewal") && WoW.Level >= 30 && ischeckHotkeysKittyRenewal && WoW.HealthPercent <= KittyRenewalPercent && WoW.Talent (2) == 1)
				{
					WoW.CastSpell("Renewal");
				}
				if (WoW.CanPreCast("Healthstone") && WoW.ItemCount("Healthstone") >= 1 && !WoW.ItemOnCooldown("Healthstone") && ischeckHotkeysKittyHealthstone && WoW.HealthPercent <= KittyHealthstonePercent && WoW.HealthPercent != 0)
				{
					WoW.CastSpell("Healthstone");
					return;
				}
				if (WoW.PlayerHasBuff("Bear Form"))
				{
					if (!WoW.IsSpellOnCooldown("Frenzied Regeneration") && WoW.Level >= 40 && WoW.HealthPercent <= 80 && !WoW.PlayerHasBuff("Frenzied Regeneration") && WoW.Rage >= 10 && WoW.Talent (3) == 2)
					{
						WoW.CastSpell("Frenzied Regeneration");
					}
					if (WoW.CanCast("MoonfireBear") && (!WoW.TargetHasDebuff("MoonfireBear") || WoW.TargetHasDebuff("MoonfireBear") && WoW.TargetDebuffTimeRemaining("MoonfireBear") <= 200))
					{
						WoW.CastSpell("MoonfireBear");
						return;
					}
					if (WoW.CanCast("Ironfur") && WoW.Level >= 20 && WoW.HealthPercent <= 90 && WoW.Rage >= 45 && WoW.Talent (3) == 2)
					{
						WoW.CastSpell("Ironfur");
						return;
					}
					if (WoW.CanCast("ThrashBear") && WoW.Level >= 12 && !WoW.IsSpellOnCooldown("ThrashBear") && WoW.IsSpellInRange("Skull Bash"))
					{
                        WoW.CastSpell("ThrashBear");
                        return;
                    }
                    if (WoW.CanCast("Mangle") && WoW.Level >= 10 && WoW.IsSpellInRange("Mangle") && WoW.Talent (3) == 2)
                    {
                        WoW.CastSpell("Mangle");
						return;
                    }	
				}
				if (WoW.PlayerHasBuff("Cat Form"))
				{					
					// COOLDOWNS
					if (WoW.CanPreCast("TigersFury") && WoW.Level >= 13 && WoW.IsSpellInRange("Rake") && 
					(WoW.Talent (6) != 3 && WoW.Talent (1) != 1 && WoW.Energy <= 40 || 
					WoW.Talent (6) != 3 && WoW.Talent (1) == 1 && (!WoW.PlayerHasBuff("TigersFury") || WoW.PlayerHasBuff("TigersFury") && WoW.Energy <= 40) ||
					WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.Talent (1) != 1 ||
					WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.Talent (1) == 1 && (WoW.PlayerHasBuff("TigersFury") && WoW.Energy <= 40 || !WoW.PlayerHasBuff("TigersFury"))))
					{
						WoW.CastSpell("TigersFury");
						return;
					}
					if (WoW.Talent (7) == 2 && WoW.CanCast("Regrowth") && WoW.PlayerHasBuff("PredatorySwiftness") && WoW.PlayerBuffTimeRemaining ("PredatorySwiftness") >= GCD && WoW.LastSpell != "Regrowth")
					{
						if (WoW.CurrentComboPoints >= 2 && WoW.Level >= 5 && !WoW.PlayerHasBuff("SavageRoar"))
						{
							WoW.CastSpell("Regrowth");
							return;
						}
						if (WoW.CurrentComboPoints >= 5 && WoW.Level >= 5)
						{
							WoW.CastSpell("Regrowth");
							return;
						}
						if (WoW.PlayerBuffTimeRemaining("PredatorySwiftness") < 150 && WoW.Level >= 5)
						{
							WoW.CastSpell("Regrowth");
							return;
						}
						if (WoW.CurrentComboPoints == 2 && WoW.Level >= 5 && !WoW.PlayerHasBuff("Bloodtalons") && WoW.SpellCooldownTimeRemaining("Ashamane") <= 100)
						{
							WoW.CastSpell("Regrowth");
							return;
						}
					}
					if (WoW.Talent (7) == 3 && WoW.CanCast("Regrowth") && WoW.PlayerHasBuff("PredatorySwiftness") && WoW.PlayerBuffTimeRemaining ("PredatorySwiftness") >= GCD && WoW.LastSpell != "Regrowth")
					{
						if (WoW.SpellCooldownTimeRemaining("ElunesGuidance") <= 100 && WoW.Level >= 5 && WoW.CurrentComboPoints == 0)
						{
							WoW.CastSpell("Regrowth");
							return;
						}
						if (WoW.PlayerHasBuff("ElunesGuidance") && WoW.Level >= 5 && WoW.CurrentComboPoints >= 4)
						{
							WoW.CastSpell("Regrowth");
							return;
						}
					}
					if (UseCooldowns && WoW.IsSpellInRange("Rake") && (pullwatch.ElapsedMilliseconds > 10000 && combatRoutine.Type != RotationType.AOE || combatRoutine.Type == RotationType.AOE))
					{
						if (WoW.Talent (5) != 2 && WoW.Level >= 40 && WoW.CanPreCast("Berserk") && ischeckHotkeysKittyIncarnation && (WoW.PlayerHasBuff("TigersFury") || WoW.SpellCooldownTimeRemaining("TigersFury") < 200) && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
						{
							WoW.CastSpell("Berserk");
							return;
						}
						if (WoW.Talent (5) == 2 && WoW.Level >= 75 && WoW.CanPreCast("Incarnation") && ischeckHotkeysKittyIncarnation && (WoW.PlayerHasBuff("TigersFury") || WoW.SpellCooldownTimeRemaining("TigersFury") < 200) && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
						{
							WoW.CastSpell("Incarnation");
							return;
						}
						if (WoW.IsSpellInRange("Ashamane") && WoW.Level >= 101 && WoW.CanPreCast("Ashamane") && WoW.CurrentComboPoints <= 2 && WoW.PlayerHasBuff("TigersFury") && WoW.PlayerBuffTimeRemaining("TigersFury") >= 400 && (WoW.Talent (7) != 2 || WoW.Talent (7) == 2 && WoW.PlayerHasBuff("Bloodtalons")))
						{
							WoW.CastSpell("Ashamane");
							return;
						}
						if (WoW.PlayerRace == "NightElf" && WoW.CanPreCast("Shadowmeld") && WoW.CurrentComboPoints < 5 && WoW.Energy >= 35 && WoW.PlayerHasBuff("TigersFury"))
						{
							WoW.CastSpell("Shadowmeld");
							return;
						}
					}
					if (combatRoutine.Type == RotationType.SingleTarget)
					{
						// OPENER
						if (pullwatch.ElapsedMilliseconds < 10000 && UseCooldowns && WoW.IsSpellInRange("Rake"))
						{
							if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && !WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints >= 2)
							{
								WoW.CastSpell("SavageRoar");
								return;
							}
							if (WoW.CanPreCast("TigersFury") && WoW.Level >= 13 && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
							{
								WoW.CastSpell("TigersFury");
								return;
							}
							if (WoW.Talent (5) != 2 && WoW.Level >= 40 && ischeckHotkeysKittyIncarnation && WoW.CanPreCast("Berserk") && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
							{
								WoW.CastSpell("Berserk");
								return;
							}
							if (WoW.Talent (5) == 2 && WoW.Level >= 75 && ischeckHotkeysKittyIncarnation && WoW.CanPreCast("Incarnation") && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
							{
								WoW.CastSpell("Incarnation");
								return;
							}
							if (WoW.CanPreCast("Rake") && WoW.Level >= 12 && (!WoW.TargetHasDebuff("Rake") || WoW.TargetDebuffTimeRemaining("Rake") < 300) && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 35 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 17))
							{
								WoW.CastSpell("Rake");
								return;
							}
							if (WoW.Talent (1) == 3 && WoW.Level >= 3 && !WoW.TargetHasDebuff("Moonfire") && WoW.LastSpell != "Moonfire" && WoW.IsSpellInRange("Skull Bash") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.CurrentComboPoints < 5)
							{
								WoW.CastSpell("Moonfire");
								return;
							}
							if (WoW.CanPreCast("Ashamane") && WoW.Level >= 101 && WoW.PlayerHasBuff("TigersFury") && WoW.PlayerBuffTimeRemaining("TigersFury") >= 400)
							{
								WoW.CastSpell("Ashamane");
								return;
							}
							if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && ripbuffed.IsRunning && WoW.TargetHasDebuff("Rip") && (WoW.Talent(6) == 3 && WoW.PlayerHasBuff("SavageRoar") || WoW.Talent(6) != 3))
							{
								WoW.CastSpell("FerociousBite");
								return;
							}
							if (WoW.CanPreCast("Rip") && WoW.Level >= 20 && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3) && WoW.CurrentComboPoints == 5 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip"))
							{
								WoW.CastSpell("Rip");
								ripbuffed.Start();
								ripunbuffed.Reset();
								Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
								return;
							}
							if (WoW.CanPreCast("Shred") && WoW.Level >= 10 && WoW.CurrentComboPoints < 5 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20 || WoW.PlayerHasBuff("Clearcasting")))
							{
								WoW.CastSpell("Shred");
								return;
							}
						}
						if (WoW.IsSpellInRange("Rake") && ischeckHotkeysKittyLuffaWrappings && WoW.CanPreCast("Thrash") && (!WoW.TargetHasDebuff("Thrash") || WoW.TargetDebuffTimeRemaining ("Thrash") <= 300) && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 45 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 22 || WoW.PlayerHasBuff("Clearcasting")))
						{
							WoW.CastSpell("Thrash");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && ischeckHotkeysKittyAshamane && WoW.Level >= 101 && WoW.CanPreCast("Ashamane") && !WoW.IsSpellOnCooldown("Ashamane") && WoW.CurrentComboPoints < 4 && WoW.PlayerHasBuff("TigersFury") && WoW.PlayerBuffTimeRemaining("TigersFury") >= 400 && (WoW.Talent (7) != 2 || WoW.Talent (7) == 2 && WoW.PlayerHasBuff("Bloodtalons")))
						{
							WoW.CastSpell("Ashamane");
							return;
						}
						// Keep Rip from falling off during execute range
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 63 && WoW.CanCast("Maim") && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("FreeMaim"))
						{
							WoW.CastSpell("Maim");
							return;
						}
						// SavageRoar if player dont have buff SavageRoar and is at 5 combo points.
						if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && !WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("SavageRoar");
							return;
						}
						// SavageRoar if player have buff SavageRoar and is under 4 seconds remaining and is at 5 combo points.
						if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && WoW.PlayerBuffTimeRemaining("SavageRoar") <= 400 && WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("SavageRoar");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && WoW.CanPreCast("Rake") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 35 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 17 ))
						{
							// TODO : Code in bleed multiplier
							if (WoW.CurrentComboPoints < 5 && WoW.Level >= 12 && WoW.Talent (7) == 2 && WoW.PlayerHasBuff("Bloodtalons") && WoW.TargetDebuffTimeRemaining("Rake") <= 500)
							{
								WoW.CastSpell("Rake");
								return;
							}
							if (WoW.CurrentComboPoints < 5 && WoW.Level >= 12 && (!WoW.TargetHasDebuff("Rake") || WoW.TargetDebuffTimeRemaining("Rake") < 300))
							{
								WoW.CastSpell("Rake");
								return;
							}
						}
						if (WoW.Talent (1) == 3 && WoW.Level >= 3 && WoW.LastSpell != "Moonfire" && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3) && (!WoW.TargetHasDebuff("Moonfire") || WoW.TargetDebuffTimeRemaining("Moonfire") <= 300) && WoW.IsSpellInRange("Skull Bash") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15 ) && WoW.CurrentComboPoints < 5)
						{
							WoW.CastSpell("Moonfire");
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && WoW.PlayerHasBuff("FreeBite") && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints >= 1 && WoW.TargetHasDebuff("Rip") && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						// Ferocious Bite if HP under 25% and Rip Remaining under 3 seconds. (will extend Rip)
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints > 1 && WoW.TargetDebuffTimeRemaining("Rip") < 300 && WoW.TargetHealthPercent < 25 && WoW.TargetHasDebuff("Rip"))
						{
							WoW.CastSpell("FerociousBite");
							Log.Write("Refresh Rip. Target under 25%", Color.Red);
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 12) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && WoW.TargetHasDebuff("Rip") && WoW.Talent(6) == 1)
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						// Refresh Rip if not on target and tigersfury or berserk
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip") && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("TigersFury"))
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip for stronger Rip
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && ripunbuffed.IsRunning && WoW.PlayerHasBuff("TigersFury") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip at 8 seconds if have Tigers fury or Berserk buff.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.Talent (6) != 1 && WoW.TargetDebuffTimeRemaining("Rip") < 800 && WoW.TargetHealthPercent > 25 && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("TigersFury"))
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip at 8 seconds if no buff and no sabertooth talent and non-buffed rip is running.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.Talent (6) != 1 && WoW.TargetDebuffTimeRemaining("Rip") < 800 && WoW.TargetHealthPercent > 25 && WoW.CurrentComboPoints == 5 && !ripbuffed.IsRunning)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. Under 8 secs remaining on Unbuffed Rip.", Color.Red);
							return;
						}
						// Refresh Rip if not on target.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip")  && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. No Rip on target or about to expire.", Color.Red);
							return;
						}
						// Refresh Rip if under 4 seconds remaining.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.TargetDebuffTimeRemaining("Rip") < 400 && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. No Rip under 4secs remaining.", Color.Red);
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && WoW.TargetHasDebuff("Rip") && (WoW.Talent(6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent(6) != 3))
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						if (WoW.Talent (7) == 3 && WoW.Level >= 100 && WoW.CanPreCast("ElunesGuidance") && WoW.CurrentComboPoints == 0 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15))
						{  
							WoW.CastSpell("ElunesGuidance");
							return;
						}
						if (WoW.Talent (6) == 2 && WoW.Level >= 90 && WoW.IsSpellInRange("Rake") && WoW.CanPreCast("BrutalSlash") && WoW.PlayerSpellCharges("BrutalSlash") == 3 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 10 || WoW.PlayerHasBuff("Clearcasting")))
						{
							WoW.CastSpell("BrutalSlash");
							return;
						}
						if (WoW.IsSpellInRange("Shred") && WoW.Level >= 10 && WoW.CanPreCast("Shred") && WoW.CurrentComboPoints < 5 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20 || WoW.PlayerHasBuff("Clearcasting")))
						{
							WoW.CastSpell("Shred");
							return;
						}   					
					}
					// Cleave Rotation
					if (combatRoutine.Type == RotationType.SingleTargetCleave)
					{
						// OPENER
						if (pullwatch.ElapsedMilliseconds < 10000 && UseCooldowns && WoW.IsSpellInRange("Rake"))
						{
							if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && !WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints >= 2)
							{
								WoW.CastSpell("SavageRoar");
								return;
							}
							if (WoW.CanPreCast("TigersFury") && WoW.Level >= 13 && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
							{
								WoW.CastSpell("TigersFury");
								return;
							}
							if (WoW.Talent (5) != 2 && WoW.Level >= 40 && ischeckHotkeysKittyIncarnation && WoW.CanPreCast("Berserk") && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
							{
								WoW.CastSpell("Berserk");
								return;
							}
							if (WoW.Talent (5) == 2 && WoW.Level >= 75 && ischeckHotkeysKittyIncarnation && WoW.CanPreCast("Incarnation") && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
							{
								WoW.CastSpell("Incarnation");
								return;
							}
							if (WoW.CanPreCast("Rake") && WoW.Level >= 12 && (!WoW.TargetHasDebuff("Rake") || WoW.TargetDebuffTimeRemaining("Rake") < 300) && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 35 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 17))
							{
								WoW.CastSpell("Rake");
								return;
							}
							if (WoW.Talent (1) == 3 && WoW.Level >= 3 && !WoW.TargetHasDebuff("Moonfire") && WoW.LastSpell != "Moonfire" && WoW.IsSpellInRange("Skull Bash") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.CurrentComboPoints < 5)
							{
								WoW.CastSpell("Moonfire");
								return;
							}
							if (WoW.CanPreCast("Ashamane") && WoW.Level >= 101 && WoW.PlayerHasBuff("TigersFury") && WoW.PlayerBuffTimeRemaining("TigersFury") >= 400)
							{
								WoW.CastSpell("Ashamane");
								return;
							}
							if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && ripbuffed.IsRunning && WoW.TargetHasDebuff("Rip") && (WoW.Talent(6) == 3 && WoW.PlayerHasBuff("SavageRoar") || WoW.Talent(6) != 3))
							{
								WoW.CastSpell("FerociousBite");
								return;
							}
							if (WoW.CanPreCast("Rip") && WoW.Level >= 20 && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3) && WoW.CurrentComboPoints == 5 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip"))
							{
								WoW.CastSpell("Rip");
								ripbuffed.Start();
								ripunbuffed.Reset();
								Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
								return;
							}
							if (WoW.CanPreCast("Shred") && WoW.Level >= 10 && WoW.CurrentComboPoints < 5 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20 || WoW.PlayerHasBuff("Clearcasting")))
							{
								WoW.CastSpell("Shred");
								return;
							}
						}
						if (WoW.IsSpellInRange("Rake") && WoW.CanPreCast("Thrash") && (!WoW.TargetHasDebuff("Thrash") || WoW.TargetDebuffTimeRemaining ("Thrash") <= 300) && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 45 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 22 || WoW.PlayerHasBuff("Clearcasting")))
						{
							WoW.CastSpell("Thrash");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && ischeckHotkeysKittyAshamane && WoW.Level >= 101 && WoW.CanPreCast("Ashamane") && !WoW.IsSpellOnCooldown("Ashamane") && WoW.CurrentComboPoints < 4 && WoW.PlayerHasBuff("TigersFury") && WoW.PlayerBuffTimeRemaining("TigersFury") >= 400 && (WoW.Talent (7) != 2 || WoW.Talent (7) == 2 && WoW.PlayerHasBuff("Bloodtalons")))
						{
							WoW.CastSpell("Ashamane");
							return;
						}
						// Keep Rip from falling off during execute range
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 63 && WoW.CanCast("Maim") && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("FreeMaim"))
						{
							WoW.CastSpell("Maim");
							return;
						}
						// SavageRoar if player dont have buff SavageRoar and is at 5 combo points.
						if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && !WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("SavageRoar");
							return;
						}
						// SavageRoar if player have buff SavageRoar and is under 4 seconds remaining and is at 5 combo points.
						if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && WoW.PlayerBuffTimeRemaining("SavageRoar") <= 400 && WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("SavageRoar");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && WoW.CanPreCast("Rake") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 35 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 17 ))
						{
							// TODO : Code in bleed multiplier
							if (WoW.CurrentComboPoints < 5 && WoW.Level >= 12 && WoW.Talent (7) == 2 && WoW.PlayerHasBuff("Bloodtalons") && WoW.TargetDebuffTimeRemaining("Rake") <= 500)
							{
								WoW.CastSpell("Rake");
								return;
							}
							if (WoW.CurrentComboPoints < 5 && WoW.Level >= 12 && (!WoW.TargetHasDebuff("Rake") || WoW.TargetDebuffTimeRemaining("Rake") < 300))
							{
								WoW.CastSpell("Rake");
								return;
							}
						}
						if (WoW.Talent (1) == 3 && WoW.Level >= 3 && WoW.LastSpell != "Moonfire" && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3) && (!WoW.TargetHasDebuff("Moonfire") || WoW.TargetDebuffTimeRemaining("Moonfire") <= 300) && WoW.IsSpellInRange("Skull Bash") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15 ) && WoW.CurrentComboPoints < 5)
						{
							WoW.CastSpell("Moonfire");
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && WoW.PlayerHasBuff("FreeBite") && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints >= 1 && WoW.TargetHasDebuff("Rip") && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						// Ferocious Bite if HP under 25% and Rip Remaining under 3 seconds. (will extend Rip)
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints > 1 && WoW.TargetDebuffTimeRemaining("Rip") < 300 && WoW.TargetHealthPercent < 25 && WoW.TargetHasDebuff("Rip"))
						{
							WoW.CastSpell("FerociousBite");
							Log.Write("Refresh Rip. Target under 25%", Color.Red);
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 12) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && WoW.TargetHasDebuff("Rip") && WoW.Talent(6) == 1)
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						// Refresh Rip if not on target and tigersfury or berserk
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip") && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("TigersFury"))
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip for stronger Rip
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && ripunbuffed.IsRunning && WoW.PlayerHasBuff("TigersFury") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip at 8 seconds if have Tigers fury or Berserk buff.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.Talent (6) != 1 && WoW.TargetDebuffTimeRemaining("Rip") < 800 && WoW.TargetHealthPercent > 25 && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("TigersFury"))
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip at 8 seconds if no buff and no sabertooth talent and non-buffed rip is running.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.Talent (6) != 1 && WoW.TargetDebuffTimeRemaining("Rip") < 800 && WoW.TargetHealthPercent > 25 && WoW.CurrentComboPoints == 5 && !ripbuffed.IsRunning)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. Under 8 secs remaining on Unbuffed Rip.", Color.Red);
							return;
						}
						// Refresh Rip if not on target.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip")  && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. No Rip on target or about to expire.", Color.Red);
							return;
						}
						// Refresh Rip if under 4 seconds remaining.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.TargetDebuffTimeRemaining("Rip") < 400 && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. No Rip under 4secs remaining.", Color.Red);
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && WoW.TargetHasDebuff("Rip") && (WoW.Talent(6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent(6) != 3))
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						if (WoW.Talent (7) == 3 && WoW.Level >= 100 && WoW.CanPreCast("ElunesGuidance") && WoW.CurrentComboPoints == 0 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15))
						{  
							WoW.CastSpell("ElunesGuidance");
							return;
						}
						if (WoW.Talent (6) == 2 && WoW.Level >= 90 && WoW.IsSpellInRange("Rake") && WoW.CanPreCast("BrutalSlash") && WoW.PlayerSpellCharges("BrutalSlash") >= 2 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 10 || WoW.PlayerHasBuff("Clearcasting")))
						{
							WoW.CastSpell("BrutalSlash");
							return;
						}
						if (WoW.IsSpellInRange("Shred") && WoW.Level >= 10 && WoW.CanPreCast("Shred") && WoW.CurrentComboPoints < 5 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20 || WoW.PlayerHasBuff("Clearcasting")))
						{
							WoW.CastSpell("Shred");
							return;
						}   					
					}
					//  AOE ROTATION
					if (combatRoutine.Type == RotationType.AOE)
					{
						if (WoW.IsSpellInRange("Rake") && ischeckHotkeysKittyAshamane && WoW.Level >= 101 && WoW.CanPreCast("Ashamane") && !WoW.IsSpellOnCooldown("Ashamane") && WoW.CurrentComboPoints < 4 && WoW.PlayerHasBuff("TigersFury") && WoW.PlayerBuffTimeRemaining("TigersFury") >= 400 && (WoW.Talent (7) != 2 || WoW.Talent (7) == 2 && WoW.PlayerHasBuff("Bloodtalons")))
						{
							WoW.CastSpell("Ashamane");
							return;
						}
						// Keep Rip from falling off during execute range
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 63 && WoW.CanCast("Maim") && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("FreeMaim"))
						{
							WoW.CastSpell("Maim");
							return;
						}
						// SavageRoar if player dont have buff SavageRoar and is at 5 combo points.
						if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && !WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("SavageRoar");
							return;
						}
						// SavageRoar if player have buff SavageRoar and is under 4 seconds remaining and is at 5 combo points.
						if (WoW.Talent (6) == 3 && WoW.Level >= 90 && WoW.CanPreCast("SavageRoar") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 40 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20) && WoW.PlayerBuffTimeRemaining("SavageRoar") <= 400 && WoW.PlayerHasBuff("SavageRoar") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("SavageRoar");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && WoW.CanPreCast("Rake") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 35 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 17 ))
						{
							// TODO : Code in bleed multiplier
							if (WoW.CurrentComboPoints < 5 && WoW.Level >= 12 && WoW.Talent (7) == 2 && WoW.PlayerHasBuff("Bloodtalons") && WoW.TargetDebuffTimeRemaining("Rake") <= 500)
							{
								WoW.CastSpell("Rake");
								return;
							}
							if (WoW.CurrentComboPoints < 5 && WoW.Level >= 12 && (!WoW.TargetHasDebuff("Rake") || WoW.TargetDebuffTimeRemaining("Rake") < 300))
							{
								WoW.CastSpell("Rake");
								return;
							}
						}
						if (WoW.Talent (1) == 3 && WoW.Level >= 3 && WoW.LastSpell != "Moonfire" && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3) && (!WoW.TargetHasDebuff("Moonfire") || WoW.TargetDebuffTimeRemaining("Moonfire") <= 300) && WoW.IsSpellInRange("Skull Bash") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15 ) && WoW.CurrentComboPoints < 5)
						{
							WoW.CastSpell("Moonfire");
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && WoW.PlayerHasBuff("FreeBite") && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints >= 1 && WoW.TargetHasDebuff("Rip") && (WoW.Talent (6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent (6) != 3))
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						// Ferocious Bite if HP under 25% and Rip Remaining under 3 seconds. (will extend Rip)
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints > 1 && WoW.TargetDebuffTimeRemaining("Rip") < 300 && WoW.TargetHealthPercent < 25 && WoW.TargetHasDebuff("Rip"))
						{
							WoW.CastSpell("FerociousBite");
							Log.Write("Refresh Rip. Target under 25%", Color.Red);
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 12) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && WoW.TargetHasDebuff("Rip") && WoW.Talent(6) == 1)
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						// Refresh Rip if not on target and tigersfury or berserk
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip") && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("TigersFury"))
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip for stronger Rip
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && ripunbuffed.IsRunning && WoW.PlayerHasBuff("TigersFury") && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip at 8 seconds if have Tigers fury or Berserk buff.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.Talent (6) != 1 && WoW.TargetDebuffTimeRemaining("Rip") < 800 && WoW.TargetHealthPercent > 25 && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("TigersFury"))
						{
							WoW.CastSpell("Rip");
							ripbuffed.Start();
							ripunbuffed.Reset();
							Log.Write("Buffed Rip. Tigers Fury buff.", Color.Red);
							return;
						}
						// Refresh Rip at 8 seconds if no buff and no sabertooth talent and non-buffed rip is running.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.Talent (6) != 1 && WoW.TargetDebuffTimeRemaining("Rip") < 800 && WoW.TargetHealthPercent > 25 && WoW.CurrentComboPoints == 5 && !ripbuffed.IsRunning)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. Under 8 secs remaining on Unbuffed Rip.", Color.Red);
							return;
						}
						// Refresh Rip if not on target.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && !WoW.TargetHasDebuff("Rip")  && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. No Rip on target or about to expire.", Color.Red);
							return;
						}
						// Refresh Rip if under 4 seconds remaining.
						if (WoW.IsSpellInRange("Rip") && WoW.Level >= 20 && WoW.CanPreCast("Rip") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15) && WoW.TargetDebuffTimeRemaining("Rip") < 400 && WoW.CurrentComboPoints == 5)
						{
							WoW.CastSpell("Rip");
							ripunbuffed.Start();
							ripbuffed.Reset();
							Log.Write("Unbuffed Rip. No Rip under 4secs remaining.", Color.Red);
							return;
						}
						if (WoW.IsSpellInRange("FerociousBite") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 50 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 25 || WoW.PlayerHasBuff("FreeBite")) && WoW.Level >= 10 && WoW.CanPreCast("FerociousBite") && WoW.CurrentComboPoints == 5 && WoW.TargetHasDebuff("Rip") && (WoW.Talent(6) == 3 && WoW.PlayerHasBuff("SavageRoar") && WoW.PlayerBuffTimeRemaining("SavageRoar") >= 400 || WoW.Talent(6) != 3))
						{
							WoW.CastSpell("FerociousBite");
							return;
						}
						if (WoW.Talent (7) == 3 && WoW.Level >= 100 && WoW.CanPreCast("ElunesGuidance") && WoW.CurrentComboPoints == 0 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 30 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 15))
						{  
							WoW.CastSpell("ElunesGuidance");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 90 && WoW.Talent (6) == 2 && WoW.CurrentComboPoints < 5 && WoW.CanPreCast("BrutalSlash") && WoW.PlayerSpellCharges("BrutalSlash") >= 1 && WoW.TargetHasDebuff("Thrash") && (WoW.PlayerHasBuff("Clearcasting") || !WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 20 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 10))
						{
							WoW.CastSpell("BrutalSlash");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 12 && WoW.CanPreCast("Thrash") && WoW.CurrentComboPoints < 5 && !WoW.TargetHasDebuff("Thrash") && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 45 || WoW.PlayerHasBuff("Clearcasting") || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 23))
						{
							WoW.CastSpell("Thrash");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 32 && WoW.Talent (6) != 2 && WoW.CanPreCast("Swipe") && WoW.TargetHasDebuff("Thrash") && WoW.CurrentComboPoints < 5 && (!WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 45 || WoW.PlayerHasBuff("Clearcasting") || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 23))
						{
							WoW.CastSpell("Swipe");
							return;
						}
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 12 && WoW.CanPreCast("Thrash") && WoW.CurrentComboPoints < 5 && WoW.PlayerHasBuff("Clearcasting"))
						{
							WoW.CastSpell("Thrash");
							return;
						} 
						if (WoW.IsSpellInRange("Rake") && WoW.Level >= 12 && WoW.CanPreCast("Thrash") && WoW.CurrentComboPoints < 5 && !WoW.PlayerHasBuff("Clearcasting") && (WoW.Energy >= 70 || WoW.PlayerHasBuff("Incarnation") && WoW.Energy >= 35))
						{
							WoW.CastSpell("Thrash");
							return;
						} 
					}
				}
			}
		}
	}
	

    public class SettingsFormDFF : Form
    {
        public Button btnHotkeysKittyOffensiveCooldowns;
        public CheckBox checkHotkeysKittyRegrowth;
        public TextBox checkHotkeysKittyRegrowthPercent;
        private readonly Label checkHotkeysKittyRegrowthPercentLabel;
        public CheckBox checkHotkeysKittySurvivalInstincts;
        public TextBox checkHotkeysKittySurvivalInstinctsPercent;
        public Label checkHotkeysKittySurvivalInstinctsPercentLabel;
        public CheckBox checkHotkeysKittyRenewal;
        public TextBox checkHotkeysKittyRenewalPercent;
        public Label checkHotkeysKittyRenewalPercentLabel;
        public CheckBox checkHotkeysKittyHealthstone;
        public TextBox checkHotkeysKittyHealthstonePercent;
        public Label checkHotkeysKittyHealthstonePercentLabel;
        public CheckBox checkHotkeysKittyLuffaWrappings;
        public CheckBox checkHotkeysKittyKick;
        public TextBox checkHotkeysKittyKickPercent;
        public Label checkHotkeysKittyKickPercentLabel;
        public CheckBox checkHotkeysKittyIncarnation;
        public CheckBox checkHotkeysKittyAshamane;
        private readonly GroupBox groupBox12;
        private readonly GroupBox groupBox13;
        private readonly GroupBox groupBox14;
        private readonly Label spellIdLabel;
        public ListBox spellList;
        public TextBox spellText;
        private readonly TabControl tabControl3;
        private readonly TabPage tabPage5;

        #region Windows Form Designer generated code

        public SettingsFormDFF()
        {
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysKittyLuffaWrappings = new System.Windows.Forms.CheckBox();
            this.checkHotkeysKittyKick = new System.Windows.Forms.CheckBox();
            this.checkHotkeysKittyKickPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysKittyKickPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysKittyRegrowth = new System.Windows.Forms.CheckBox();
            this.checkHotkeysKittyRegrowthPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysKittyRegrowthPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysKittySurvivalInstincts = new System.Windows.Forms.CheckBox();
            this.checkHotkeysKittySurvivalInstinctsPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysKittySurvivalInstinctsPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysKittyRenewal = new System.Windows.Forms.CheckBox();
            this.checkHotkeysKittyRenewalPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysKittyRenewalPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysKittyHealthstone = new System.Windows.Forms.CheckBox();
            this.checkHotkeysKittyHealthstonePercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysKittyHealthstonePercentLabel = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysKittyIncarnation = new System.Windows.Forms.CheckBox();
            this.checkHotkeysKittyAshamane = new System.Windows.Forms.CheckBox();
            this.btnHotkeysKittyOffensiveCooldowns = new System.Windows.Forms.Button();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage5.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.tabControl3.SuspendLayout();
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox12);
            this.tabPage5.Controls.Add(this.groupBox13);
            this.tabPage5.Controls.Add(this.groupBox14);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(582, 406);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Kitty Settings";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.checkHotkeysKittyRegrowth);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyRegrowthPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyRegrowthPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysKittySurvivalInstincts);
            this.groupBox12.Controls.Add(this.checkHotkeysKittySurvivalInstinctsPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysKittySurvivalInstinctsPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyRenewal);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyRenewalPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyRenewalPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyHealthstone);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyHealthstonePercent);
            this.groupBox12.Controls.Add(this.checkHotkeysKittyHealthstonePercentLabel);
            this.groupBox12.Location = new System.Drawing.Point(8, 100);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(556, 120);
            this.groupBox12.TabIndex = 4;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Defensive Cooldowns";			
            // 
            // checkHotkeysKittyRegrowth
            // 
            this.checkHotkeysKittyRegrowth.AutoSize = true;
            this.checkHotkeysKittyRegrowth.Location = new System.Drawing.Point(151, 28);
            this.checkHotkeysKittyRegrowth.Name = "checkHotkeysKittyRegrowth";
            this.checkHotkeysKittyRegrowth.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysKittyRegrowth.TabIndex = 8;
            this.checkHotkeysKittyRegrowth.Text = "Regrowth";
            this.checkHotkeysKittyRegrowth.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysKittyRegrowthPercent
            // 
            this.checkHotkeysKittyRegrowthPercent.AutoSize = true;
            this.checkHotkeysKittyRegrowthPercent.Location = new System.Drawing.Point(300, 28);
            this.checkHotkeysKittyRegrowthPercent.Name = "checkHotkeysKittyRegrowthPercent";
            this.checkHotkeysKittyRegrowthPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyRegrowthPercent.TabIndex = 9;
            this.checkHotkeysKittyRegrowthPercent.Text = "50";
            // 
            // checkHotkeysKittyRegrowthPercentLabel
            // 
            this.checkHotkeysKittyRegrowthPercentLabel.AutoSize = true;
            this.checkHotkeysKittyRegrowthPercentLabel.Location = new System.Drawing.Point(321, 30);
            this.checkHotkeysKittyRegrowthPercentLabel.Name = "checkHotkeysKittyRegrowthPercentLabel";
            this.checkHotkeysKittyRegrowthPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyRegrowthPercentLabel.TabIndex = 9;
            this.checkHotkeysKittyRegrowthPercentLabel.Text = "% HP";
            // 
            // checkHotkeysKittySurvivalInstincts
            // 
            this.checkHotkeysKittySurvivalInstincts.AutoSize = true;
            this.checkHotkeysKittySurvivalInstincts.Location = new System.Drawing.Point(151, 50);
            this.checkHotkeysKittySurvivalInstincts.Name = "checkHotkeysKittySurvivalInstincts";
            this.checkHotkeysKittySurvivalInstincts.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysKittySurvivalInstincts.TabIndex = 9;
            this.checkHotkeysKittySurvivalInstincts.Text = "Survival Instincts";
            this.checkHotkeysKittySurvivalInstincts.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysKittySurvivalInstinctsPercent
            // 
            this.checkHotkeysKittySurvivalInstinctsPercent.AutoSize = true;
            this.checkHotkeysKittySurvivalInstinctsPercent.Location = new System.Drawing.Point(300, 50);
            this.checkHotkeysKittySurvivalInstinctsPercent.Name = "checkHotkeysKittySurvivalInstinctsPercent";
            this.checkHotkeysKittySurvivalInstinctsPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittySurvivalInstinctsPercent.TabIndex = 9;
            this.checkHotkeysKittySurvivalInstinctsPercent.Text = "50";
            // 
            // checkHotkeysKittySurvivalInstinctsPercentLabel
            // 
            this.checkHotkeysKittySurvivalInstinctsPercentLabel.AutoSize = true;
            this.checkHotkeysKittySurvivalInstinctsPercentLabel.Location = new System.Drawing.Point(321, 52);
            this.checkHotkeysKittySurvivalInstinctsPercentLabel.Name = "checkHotkeysKittySurvivalInstinctsPercentLabel";
            this.checkHotkeysKittySurvivalInstinctsPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittySurvivalInstinctsPercentLabel.TabIndex = 9;
            this.checkHotkeysKittySurvivalInstinctsPercentLabel.Text = "% HP";
            // 
            // checkHotkeysKittyRenewal
            // 
            this.checkHotkeysKittyRenewal.AutoSize = true;
            this.checkHotkeysKittyRenewal.Location = new System.Drawing.Point(151, 72);
            this.checkHotkeysKittyRenewal.Name = "checkHotkeysKittyRenewal";
            this.checkHotkeysKittyRenewal.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysKittyRenewal.TabIndex = 8;
            this.checkHotkeysKittyRenewal.Text = "Renewal";
            this.checkHotkeysKittyRenewal.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysKittyRenewalPercent
            // 
            this.checkHotkeysKittyRenewalPercent.AutoSize = true;
            this.checkHotkeysKittyRenewalPercent.Location = new System.Drawing.Point(300, 72);
            this.checkHotkeysKittyRenewalPercent.Name = "checkHotkeysKittyRenewalPercent";
            this.checkHotkeysKittyRenewalPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyRenewalPercent.TabIndex = 9;
            this.checkHotkeysKittyRenewalPercent.Text = "50";
            // 
            // checkHotkeysKittyRenewalPercentLabel
            // 
            this.checkHotkeysKittyRenewalPercentLabel.AutoSize = true;
            this.checkHotkeysKittyRenewalPercentLabel.Location = new System.Drawing.Point(321, 74);
            this.checkHotkeysKittyRenewalPercentLabel.Name = "checkHotkeysKittyRenewalPercentLabel";
            this.checkHotkeysKittyRenewalPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyRenewalPercentLabel.TabIndex = 9;
            this.checkHotkeysKittyRenewalPercentLabel.Text = "% HP";
            // 
            // checkHotkeysKittyHealthstone
            // 
            this.checkHotkeysKittyHealthstone.AutoSize = true;
            this.checkHotkeysKittyHealthstone.Location = new System.Drawing.Point(151, 94);
            this.checkHotkeysKittyHealthstone.Name = "checkHotkeysKittyHealthstone";
            this.checkHotkeysKittyHealthstone.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysKittyHealthstone.TabIndex = 8;
            this.checkHotkeysKittyHealthstone.Text = "Healthstone";
            this.checkHotkeysKittyHealthstone.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysKittyHealthstonePercent
            // 
            this.checkHotkeysKittyHealthstonePercent.AutoSize = true;
            this.checkHotkeysKittyHealthstonePercent.Location = new System.Drawing.Point(300, 94);
            this.checkHotkeysKittyHealthstonePercent.Name = "checkHotkeysKittyHealthstonePercent";
            this.checkHotkeysKittyHealthstonePercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyHealthstonePercent.TabIndex = 9;
            this.checkHotkeysKittyHealthstonePercent.Text = "50";
            // 
            // checkHotkeysKittyHealthstonePercentLabel
            // 
            this.checkHotkeysKittyHealthstonePercentLabel.AutoSize = true;
            this.checkHotkeysKittyHealthstonePercentLabel.Location = new System.Drawing.Point(321, 96);
            this.checkHotkeysKittyHealthstonePercentLabel.Name = "checkHotkeysKittyHealthstonePercentLabel";
            this.checkHotkeysKittyHealthstonePercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyHealthstonePercentLabel.TabIndex = 9;
            this.checkHotkeysKittyHealthstonePercentLabel.Text = "% HP";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.checkHotkeysKittyIncarnation);
            this.groupBox13.Controls.Add(this.checkHotkeysKittyAshamane);
            this.groupBox13.Controls.Add(this.btnHotkeysKittyOffensiveCooldowns);
            this.groupBox13.Controls.Add(this.checkHotkeysKittyLuffaWrappings);
            this.groupBox13.Controls.Add(this.checkHotkeysKittyKick);
            this.groupBox13.Controls.Add(this.checkHotkeysKittyKickPercent);
            this.groupBox13.Controls.Add(this.checkHotkeysKittyKickPercentLabel);
            this.groupBox13.Location = new System.Drawing.Point(8, 8);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(556, 90);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Toggles";		
            // 
            // checkHotkeysKittyIncarnation
            // 
            this.checkHotkeysKittyIncarnation.AutoSize = true;
            this.checkHotkeysKittyIncarnation.Location = new System.Drawing.Point(151, 32);
            this.checkHotkeysKittyIncarnation.Name = "checkHotkeysKittyIncarnation";
            this.checkHotkeysKittyIncarnation.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysKittyIncarnation.TabIndex = 2;
            this.checkHotkeysKittyIncarnation.Text = "Use Incarnation/Berserk";
            this.checkHotkeysKittyIncarnation.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysKittyAshamane
            // 
            this.checkHotkeysKittyAshamane.AutoSize = true;
            this.checkHotkeysKittyAshamane.Location = new System.Drawing.Point(151, 60);
            this.checkHotkeysKittyAshamane.Name = "checkHotkeysKittyIncarnation";
            this.checkHotkeysKittyAshamane.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysKittyAshamane.TabIndex = 2;
            this.checkHotkeysKittyAshamane.Text = "Always use Ashamane`s Frenzy";
            this.checkHotkeysKittyAshamane.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysKittyLuffaWrappings
            // 
            this.checkHotkeysKittyLuffaWrappings.AutoSize = true;
            this.checkHotkeysKittyLuffaWrappings.Location = new System.Drawing.Point(350, 32);
            this.checkHotkeysKittyLuffaWrappings.Name = "checkHotkeysKittyLuffaWrappings";
            this.checkHotkeysKittyLuffaWrappings.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysKittyLuffaWrappings.TabIndex = 4;
            this.checkHotkeysKittyLuffaWrappings.Text = "Luffa Wrappings Legendary";
            this.checkHotkeysKittyLuffaWrappings.UseVisualStyleBackColor = true;			
            // 
            // checkHotkeysKittyKick
            // 
            this.checkHotkeysKittyKick.AutoSize = true;
            this.checkHotkeysKittyKick.Location = new System.Drawing.Point(350, 60);
            this.checkHotkeysKittyKick.Name = "checkHotkeysKittyKick";
            this.checkHotkeysKittyKick.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysKittyKick.TabIndex = 5;
            this.checkHotkeysKittyKick.Text = "Skull Bash @";
            this.checkHotkeysKittyKick.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysKittyKickPercent
            // 
            this.checkHotkeysKittyKickPercent.AutoSize = true;
            this.checkHotkeysKittyKickPercent.Location = new System.Drawing.Point(450, 60);
            this.checkHotkeysKittyKickPercent.Name = "checkHotkeysKittyKickPercent";
            this.checkHotkeysKittyKickPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyKickPercent.TabIndex = 9;
            this.checkHotkeysKittyKickPercent.Text = "50";
            // 
            // checkHotkeysKittyKickLabel
            // 
            this.checkHotkeysKittyKickPercentLabel.AutoSize = true;
            this.checkHotkeysKittyKickPercentLabel.Location = new System.Drawing.Point(471, 62);
            this.checkHotkeysKittyKickPercentLabel.Name = "checkHotkeysKittyKickPercentLabel";
            this.checkHotkeysKittyKickPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysKittyKickPercentLabel.TabIndex = 9;
            this.checkHotkeysKittyKickPercentLabel.Text = "% of Cast";			
            // 
            // btnHotkeysKittyOffensiveCooldowns
            // 
            this.btnHotkeysKittyOffensiveCooldowns.Location = new System.Drawing.Point(18, 28);
            this.btnHotkeysKittyOffensiveCooldowns.Name = "btnHotkeysKittyOffensiveCooldowns";
            this.btnHotkeysKittyOffensiveCooldowns.Size = new System.Drawing.Size(113, 23);
            this.btnHotkeysKittyOffensiveCooldowns.TabIndex = 1;
            this.btnHotkeysKittyOffensiveCooldowns.Text = "Set Roar Key";
            this.btnHotkeysKittyOffensiveCooldowns.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            this.groupBox14.Location = new System.Drawing.Point(8, 220);
            this.groupBox14.Name = "groupBox13";
            this.groupBox14.Size = new System.Drawing.Size(556, 120);
            this.groupBox14.TabIndex = 3;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Bear Rotation (Needs Guardian Affinity) - coming soon";	
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage5);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(590, 432);
            this.tabControl3.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 432);
            this.Controls.Add(this.tabControl3);
            this.Name = "SettingsForm";
            this.Text = "smartie`s Kitty rota";
            this.tabPage5.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.tabControl3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
	}
}
/*
[AddonDetails.db]
AddonAuthor=smartie
AddonName=smartie
WoWVersion=70300
[SpellBook.db]
Spell,768,Cat Form,NumPad1
Spell,1079,Rip,D6
Spell,1822,Rake,D2
Spell,5215,Prowl,NumPad2
Spell,5217,TigersFury,D9
Spell,5221,Shred,D3
Spell,8936,Regrowth,F3
Spell,22568,FerociousBite,D7
Spell,52610,SavageRoar,D8
Spell,58984,Shadowmeld,NumPad4
Spell,106830,Thrash,D4
Spell,106785,Swipe,D5
Spell,202028,BrutalSlash,D5
Spell,155625,Moonfire,NumPad3
Spell,210722,Ashamane,OemOpenBrackets
Spell,106951,Berserk,D0
Spell,102543,Incarnation,D0
Spell,202060,ElunesGuidance,Oem6
Spell,106839,Skull Bash,F11
Spell,61336,Survival Instincts,F1
Spell,108238,Renewal,F4
Spell,5512,Healthstone,F5
Spell,106898,Stampeding Roar,F6
Spell,22570,Maim,F9
Spell,1,----Bear Stuff---,Z
Spell,5487,Bear Form,NumPad5
Spell,77758,ThrashBear,D4
Spell,33917,Mangle,D3
Spell,22842,Frenzied Regeneration,F2
Spell,192081,Ironfur,D5
Spell,8921,MoonfireBear,NumPad3
Buff,5215,Prowl
Buff,768,Cat Form
Buff,5487,Bear Form
Buff,5217,TigersFury
Buff,52610,SavageRoar
Buff,58984,Shadowmeld
Buff,69369,PredatorySwiftness
Buff,106951,Berserk
Buff,145152,Bloodtalons
Buff,155580,LunarinspirationTalent
Buff,155672,BloodtalonsTalent
Buff,202031,Sabertooth
Buff,202032,JaggedwoundsTalent
Buff,135700,Clearcasting
Buff,61336,Survival Instincts
Buff,202060,ElunesGuidance
Buff,783,Travel Form
Buff,192081,Ironfur
Buff,22842,Frenzied Regeneration
Buff,236757,FreeMaim
Buff,102543,Incarnation
Buff,252752,FreeBite
Debuff,1079,Rip
Debuff,106830,Thrash
Debuff,192090,ThrashBear
Debuff,155722,Rake
Debuff,155625,Moonfire
Debuff,164812,MoonfireBear
Range,106839,Skull Bash
Range,1822,Rake
Range,22568,FerociousBite
Range,1079,Rip
Range,5221,Shred
Range,210722,Ashamane
Charge,202028,BrutalSlash
Charge,22842,Frenzied Regeneration
Charge,61336,Survival Instincts
Item,5512,Healthstone
*/
