//Changelog
// v1.0 First release
// v1.1 alot of Bugfixes
// v1.2 massive cleanup
// v1.3 Soul Reaper is now supported and working
// v1.4 added Deathstrike and Mind Freeze


using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CloudMagic.Helpers;
using System.Threading;

namespace CloudMagic.Rotation
{
    public class DKUnholysmartie : CombatRoutine
    {
        private static readonly Stopwatch coolDownStopWatch = new Stopwatch();
        public override string Name
        {
            get { return "Unholy DK"; }
        }

        public override string Class
        {
            get { return "Deathknight"; }
        }
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
				return 99;
			} 
		}
        public override int AOE 
		{ 
			get 
			{ 
				return 3; 
			} 
		}
        public override Form SettingsForm { get; set; }
        public SettingsFormDFF SettingsFormDFF { get; set; }

        public static int cooldownKey
        {
            get
            {
                var cooldownKey = ConfigFile.ReadValue("DKUnholy", "cooldownKey").Trim();
                if (cooldownKey != "")
                {
                    return Convert.ToInt32(cooldownKey);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DKUnholy", "cooldownKey", value.ToString()); }
        }

        public static int cooldownModifier
        {
            get
            {
                var cooldownModifier = ConfigFile.ReadValue("DKUnholy", "cooldownModifier").Trim();
                if (cooldownModifier != "")
                {
                    return Convert.ToInt32(cooldownModifier);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DKUnholy", "cooldownModifier", value.ToString()); }
        }

        public static string cooldownHotKeyString
        {
            get
            {
                var cooldownHotKeyString = ConfigFile.ReadValue("DKUnholy", "cooldownHotKeyString").Trim();

                if (cooldownHotKeyString != "")
                {
                    return cooldownHotKeyString;
                }

                return "Click to Set";
            }
            set { ConfigFile.WriteValue("DKUnholy", "cooldownHotKeyString", value); }
        }

        public static bool isCheckHotkeysUnholyIceboundFortitude
        {
            get
            {
                var isCheckHotkeysUnholyIceboundFortitude = ConfigFile.ReadValue("DKUnholy", "isCheckHotkeysUnholyIceboundFortitude").Trim();

                if (isCheckHotkeysUnholyIceboundFortitude != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysUnholyIceboundFortitude);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DKUnholy", "isCheckHotkeysUnholyIceboundFortitude", value.ToString()); }
        }

        public static bool isCheckHotkeyslegyshoulder
        {
            get
            {
                var isCheckHotkeyslegyshoulder = ConfigFile.ReadValue("DKUnholy", "isCheckHotkeyslegyshoulder").Trim();

                if (isCheckHotkeyslegyshoulder != "")
                {
                    return Convert.ToBoolean(isCheckHotkeyslegyshoulder);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DKUnholy", "isCheckHotkeyslegyshoulder", value.ToString()); }
        }
		
        public static bool isCheckHotkeyslegytrinket
        {
            get
            {
                var isCheckHotkeyslegytrinket = ConfigFile.ReadValue("DKUnholy", "isCheckHotkeyslegytrinket").Trim();

                if (isCheckHotkeyslegytrinket != "")
                {
                    return Convert.ToBoolean(isCheckHotkeyslegytrinket);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DKUnholy", "isCheckHotkeyslegytrinket", value.ToString()); }
        }

		
        public static bool isCheckHotkeysFrostDeathstrike
        {
            get
            {
                var isCheckHotkeysFrostDeathstrike = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostDeathstrike").Trim();

                if (isCheckHotkeysFrostDeathstrike != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostDeathstrike);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostDeathstrike", value.ToString()); }
        }

        public static int FrostDeathstrikeHPPercent
        {
            get
            {
                var FrostDeathstrikeHPPercent = ConfigFile.ReadValue("DKFrost", "FrostDeathstrikeHPPercent").Trim();
                if (FrostDeathstrikeHPPercent != "")
                {
                    return Convert.ToInt32(FrostDeathstrikeHPPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DkFrost", "FrostDeathstrikeHPPercent", value.ToString()); }
        }
		
        public static bool isCheckHotkeysFrostKick
        {
            get
            {
                var isCheckHotkeysFrostKick = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostKick").Trim();

                if (isCheckHotkeysFrostKick != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostKick);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostKick", value.ToString()); }
        }

        public static int FrostKickPercent
        {
            get
            {
                var FrostKickPercent = ConfigFile.ReadValue("DKFrost", "FrostKickPercent").Trim();
                if (FrostKickPercent != "")
                {
                    return Convert.ToInt32(FrostKickPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DkFrost", "FrostKickPercent", value.ToString()); }
        }
		
        public static bool isCheckHotkeysFrostFreeDeathstrike
        {
            get
            {
                var isCheckHotkeysFrostFreeDeathstrike = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostFreeDeathstrike").Trim();

                if (isCheckHotkeysFrostFreeDeathstrike != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostFreeDeathstrike);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostFreeDeathstrike", value.ToString()); }
        }

        public static int FrostFreeDeathstrikeHPPercent
        {
            get
            {
                var FrostFreeDeathstrikeHPPercent = ConfigFile.ReadValue("DKFrost", "FrostFreeDeathstrikeHPPercent").Trim();
                if (FrostFreeDeathstrikeHPPercent != "")
                {
                    return Convert.ToInt32(FrostFreeDeathstrikeHPPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DkFrost", "FrostFreeDeathstrikeHPPercent", value.ToString()); }
        }
		
		
        public static int UnholyIceboundHPPercent
        {
            get
            {
                var UnholyIceboundHPPercent = ConfigFile.ReadValue("DKUnholy", "UnholyIceboundHPPercent").Trim();
                if (UnholyIceboundHPPercent != "")
                {
                    return Convert.ToInt32(UnholyIceboundHPPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DKUnholy", "UnholyIceboundHPPercent", value.ToString()); }
        }

        public static bool isCheckHotkeysUnholyAntiMagicShield
        {
            get
            {
                var isCheckHotkeysUnholyAntiMagicShield = ConfigFile.ReadValue("DKUnholy", "isCheckHotkeysUnholyAntiMagicShield").Trim();

                if (isCheckHotkeysUnholyAntiMagicShield != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysUnholyAntiMagicShield);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DKUnholy", "isCheckHotkeysUnholyAntiMagicShield", value.ToString()); }
        }

        public static int UnholyAMSHPPercent
        {
            get
            {
                var UnholyAMSHPPercent = ConfigFile.ReadValue("DKUnholy", "UnholyAMSHPPercent").Trim();
                if (UnholyAMSHPPercent != "")
                {
                    return Convert.ToInt32(UnholyAMSHPPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DKUnholy", "UnholyAMSHPPercent", value.ToString()); }
        }

        public static bool isCheckHotkeysUnholyOffensiveApocalypse
        {
            get
            {
                var isCheckHotkeysUnholyOffensiveApocalypse = ConfigFile.ReadValue("DKUnholy", "isCheckHotkeysUnholyOffensiveApocalypse").Trim();

                if (isCheckHotkeysUnholyOffensiveApocalypse != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysUnholyOffensiveApocalypse);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DKUnholy", "isCheckHotkeysUnholyOffensiveApocalypse", value.ToString()); }
        }

        public static bool isCheckHotkeysUnholyOffensiveSummonGargoyle
        {
            get
            {
                var isCheckHotkeysUnholyOffensiveSummonGargoyle = ConfigFile.ReadValue("DKUnholy", "isCheckHotkeysUnholyOffensiveSummonGargoyle").Trim();

                if (isCheckHotkeysUnholyOffensiveSummonGargoyle != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysUnholyOffensiveSummonGargoyle);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DKUnholy", "isCheckHotkeysUnholyOffensiveSummonGargoyle", value.ToString()); }
        }

        public override void Initialize()
        {
            Log.Write("Welcome to the Unholy DK v1.4 by smartie", Color.Green);
	        Log.Write("All Talents supported and auto detected", Color.Green);		
            SettingsFormDFF = new SettingsFormDFF();
            SettingsForm = SettingsFormDFF;

            SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.Text = cooldownHotKeyString;

            SettingsFormDFF.checkHotkeysUnholyOffensiveApocalypse.Checked = isCheckHotkeysUnholyOffensiveApocalypse;
            SettingsFormDFF.checkHotkeysUnholyOffensiveSummonGargoyle.Checked = isCheckHotkeysUnholyOffensiveSummonGargoyle;
            SettingsFormDFF.checkHotkeysFrostDeathstrike.Checked = isCheckHotkeysFrostDeathstrike;
            SettingsFormDFF.checkHotkeysFrostDeathstrikePercent.Text = FrostDeathstrikeHPPercent.ToString();
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrike.Checked = isCheckHotkeysFrostFreeDeathstrike;
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrikePercent.Text = FrostFreeDeathstrikeHPPercent.ToString();
            SettingsFormDFF.checkHotkeysFrostKick.Checked = isCheckHotkeysFrostKick;
            SettingsFormDFF.checkHotkeysFrostKickPercent.Text = FrostKickPercent.ToString();
            SettingsFormDFF.checkHotkeysUnholyAntiMagicShield.Checked = isCheckHotkeysUnholyAntiMagicShield;
            SettingsFormDFF.checkHotkeysUnholyIceboundFortitude.Checked = isCheckHotkeysUnholyIceboundFortitude;
            SettingsFormDFF.checkHotkeyslegyshoulder.Checked = isCheckHotkeyslegyshoulder;
            SettingsFormDFF.checkHotkeyslegytrinket.Checked = isCheckHotkeyslegytrinket;
            SettingsFormDFF.checkHotkeysUnholyIFPercent.Text = UnholyIceboundHPPercent.ToString();
            SettingsFormDFF.checkHotkeysUnholyAMSPercent.Text = UnholyAMSHPPercent.ToString();

            SettingsFormDFF.checkHotkeysUnholyIceboundFortitude.CheckedChanged += isCheckHotkeysUnholyIceboundFortitude_Click;
            SettingsFormDFF.checkHotkeyslegyshoulder.CheckedChanged += isCheckHotkeyslegyshoulder_Click;
            SettingsFormDFF.checkHotkeyslegytrinket.CheckedChanged += isCheckHotkeyslegytrinket_Click;
            SettingsFormDFF.checkHotkeysFrostDeathstrike.CheckedChanged += isCheckHotkeysFrostDeathstrike_Click;
            SettingsFormDFF.checkHotkeysFrostDeathstrikePercent.TextChanged += isCheckHotkeysFrostDeathstrikePercent_Click;
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrike.CheckedChanged += isCheckHotkeysFrostFreeDeathstrike_Click;
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrikePercent.TextChanged += isCheckHotkeysFrostFreeDeathstrikePercent_Click;
            SettingsFormDFF.checkHotkeysFrostKick.CheckedChanged += isCheckHotkeysFrostKick_Click;
            SettingsFormDFF.checkHotkeysFrostKickPercent.TextChanged += isCheckHotkeysFrostKickPercent_Click;
            SettingsFormDFF.checkHotkeysUnholyIFPercent.TextChanged += isCheckHotkeysUnholyIFPercent_Click;
            SettingsFormDFF.checkHotkeysUnholyAntiMagicShield.CheckedChanged += isCheckHotkeysUnholyAntiMagicShield_Click;
            SettingsFormDFF.checkHotkeysUnholyAMSPercent.TextChanged += isCheckHotkeysUnholyAMSPercent_Click;
            SettingsFormDFF.checkHotkeysUnholyOffensiveSummonGargoyle.CheckedChanged += isCheckHotkeysUnholyOffensiveSummonGargoyle_Click;
            SettingsFormDFF.checkHotkeysUnholyOffensiveApocalypse.CheckedChanged += isCheckHotkeysUnholyOffensiveApocalypse_Click;
            SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.KeyDown += KeyDown;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
                return;
            SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.Text = "Hotkey : ";
            if (e.Shift)
            {
                cooldownModifier = (int)Keys.ShiftKey;
                SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.Text += Keys.Shift + " + ";
            }
            else if (e.Alt)
            {
                cooldownModifier = (int)Keys.Menu;
                SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.Text += Keys.Alt + " + ";
            }
            else if (e.Control)
            {
                cooldownModifier = (int)Keys.ControlKey;
                SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.Text += Keys.Control + " + ";
            }
            else cooldownModifier = -1;
            cooldownKey = (int)e.KeyCode;
            SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.Text += e.KeyCode;
            cooldownHotKeyString = SettingsFormDFF.btnHotkeysUnholyOffensiveCooldowns.Text;
            SettingsFormDFF.checkHotkeysUnholyIFPercentLabel.Focus();
        }

        private void isCheckHotkeysUnholyIceboundFortitude_Click(object sender, EventArgs e)
        {
            isCheckHotkeysUnholyIceboundFortitude = SettingsFormDFF.checkHotkeysUnholyIceboundFortitude.Checked;
        }
		
        private void isCheckHotkeyslegyshoulder_Click(object sender, EventArgs e)
        {
            isCheckHotkeyslegyshoulder = SettingsFormDFF.checkHotkeyslegyshoulder.Checked;
        }
		
        private void isCheckHotkeyslegytrinket_Click(object sender, EventArgs e)
        {
            isCheckHotkeyslegytrinket = SettingsFormDFF.checkHotkeyslegytrinket.Checked;
        }

		
        private void isCheckHotkeysFrostDeathstrike_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostDeathstrike = SettingsFormDFF.checkHotkeysFrostDeathstrike.Checked;
        }
		
        private void isCheckHotkeysFrostDeathstrikePercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysFrostDeathstrikePercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                FrostDeathstrikeHPPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysFrostDeathstrikePercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void isCheckHotkeysFrostFreeDeathstrike_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostFreeDeathstrike = SettingsFormDFF.checkHotkeysFrostFreeDeathstrike.Checked;
        }
		
        private void isCheckHotkeysFrostFreeDeathstrikePercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysFrostFreeDeathstrikePercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                FrostFreeDeathstrikeHPPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysFrostFreeDeathstrikePercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void isCheckHotkeysFrostKick_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostKick = SettingsFormDFF.checkHotkeysFrostKick.Checked;
        }
		
        private void isCheckHotkeysFrostKickPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysFrostKickPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                FrostKickPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysFrostKickPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void isCheckHotkeysUnholyIFPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysUnholyIFPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                UnholyIceboundHPPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysUnholyIFPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void isCheckHotkeysUnholyAntiMagicShield_Click(object sender, EventArgs e)
        {
            isCheckHotkeysUnholyAntiMagicShield = SettingsFormDFF.checkHotkeysUnholyAntiMagicShield.Checked;
        }

        private void isCheckHotkeysUnholyAMSPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysUnholyAMSPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                UnholyAMSHPPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysUnholyAMSPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void isCheckHotkeysUnholyOffensiveSummonGargoyle_Click(object sender, EventArgs e)
        {
            isCheckHotkeysUnholyOffensiveSummonGargoyle = SettingsFormDFF.checkHotkeysUnholyOffensiveSummonGargoyle.Checked;
        }

        private void isCheckHotkeysUnholyOffensiveApocalypse_Click(object sender, EventArgs e)
        {
            isCheckHotkeysUnholyOffensiveApocalypse = SettingsFormDFF.checkHotkeysUnholyOffensiveApocalypse.Checked;
        }


        public override void Stop()
        {
        }

        public override void Pulse()
        {
            if (!coolDownStopWatch.IsRunning || coolDownStopWatch.ElapsedMilliseconds > 60000)
                coolDownStopWatch.Restart();
            if (DetectKeyPress.GetKeyState(cooldownKey) < 0 && (cooldownModifier == -1 || cooldownModifier != -1 && DetectKeyPress.GetKeyState(cooldownModifier) < 0))
            {
                if (coolDownStopWatch.ElapsedMilliseconds > 1000)
                {
                    combatRoutine.UseCooldowns = !combatRoutine.UseCooldowns;
                    coolDownStopWatch.Restart();
                }
            }
			if (!WoW.IsInCombat && !WoW.HasPet && WoW.CanCast("Raise Dead") && !WoW.PlayerHasDebuff("Ignoble Sacrifice") && !WoW.IsMounted)
            {
				WoW.CastSpell("Raise Dead") ;
				return;
			}
            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted)
            {
                if (!WoW.IsSpellOnCooldown("Anti-Magic Shell") && WoW.HealthPercent <= UnholyAMSHPPercent &&isCheckHotkeysUnholyAntiMagicShield && WoW.Level >= 57)
                {
                    WoW.CastSpell("Anti-Magic Shell");
                }
                if (!WoW.IsSpellOnCooldown("Icebound Fortitude") && WoW.HealthPercent <= UnholyIceboundHPPercent  && isCheckHotkeysUnholyIceboundFortitude && WoW.Level >= 65)
                {
                    WoW.CastSpell("Icebound Fortitude");
                }
				if (!WoW.HasPet && WoW.CanCast("Raise Dead") && !WoW.PlayerHasDebuff("Ignoble Sacrifice"))
				{
					WoW.CastSpell("Raise Dead") ;
					return;
				}
				if (WoW.CanCast("Death Coil", false, false, true, false, false) && isCheckHotkeysFrostFreeDeathstrike && WoW.HealthPercent <= FrostFreeDeathstrikeHPPercent && WoW.PlayerHasBuff("Free DeathStrike"))
				{
					WoW.CastSpell("Death Strike");
					return;
				}
				if (WoW.CanCast("Death Coil", false, false, true, false, false) && isCheckHotkeysFrostDeathstrike && WoW.HealthPercent <= FrostDeathstrikeHPPercent && WoW.RunicPower >=45)
				{
					WoW.CastSpell("Death Strike");
					return;
				}
                if (WoW.CanCast("Mind Freeze") && isCheckHotkeysFrostKick && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= FrostKickPercent && !WoW.IsSpellOnCooldown("Mind Freeze") && !WoW.PlayerIsChanneling)
                {
                    WoW.CastSpell("Mind Freeze");						
                    return;
                }

				if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave) // Do Single Target Stuff here
				{
					if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted)
					{
						//Dark Arbiter Rotation
                		if (WoW.Talent(7) == 1)
                		{
							//Cooldowns with Legendary Shoulders
							if (combatRoutine.UseCooldowns && WoW.CanCast("Dark Arbiter") && isCheckHotkeyslegyshoulder && WoW.RunicPower <=70 && WoW.SpellCooldownTimeRemaining("Dark Transformation") >= 200)
							{
								WoW.CastSpell("Dark Arbiter") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 16500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) != 1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 5500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) == 1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 3500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							//Cooldowns without Legendary Shoulders
							if (combatRoutine.UseCooldowns && !WoW.ItemOnCooldown("KBW") && isCheckHotkeyslegytrinket)
							{
								WoW.CastSpell("KBW") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Dark Arbiter") && !isCheckHotkeyslegyshoulder && WoW.TargetDebuffStacks("Festering Wound") >=6 && WoW.RunicPower >=70)
							{
								WoW.CastSpell("Dark Arbiter") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Apocalypse") && WoW.TargetDebuffStacks("Festering Wound") >=6)
							{
								WoW.CastSpell("Apocalypse") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.PlayerRace == "BloodElf" && WoW.CanCast("Arcane Torrent") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >=10500 && WoW.Talent(7) == 1 && WoW.RunicPower <35)
							{
								WoW.CastSpell("Arcane Torrent");
								return;
							}
							//Single Target Rotation
							if (WoW.CurrentRunes >= 1 && WoW.CanCast("Outbreak") && (!WoW.TargetHasDebuff("Virulent Plague") || WoW.TargetDebuffTimeRemaining("Virulent Plague") <= 200))
							{
								WoW.CastSpell("Outbreak") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.RunicPower >=35 && WoW.RunicPower != 119 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 500 && WoW.PlayerHasBuff("Sudden Doom") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.RunicPower != 119 && WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes >= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >=10500 && WoW.RunicPower >=35)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.RunicPower <=35 && WoW.CurrentRunes >= 2 && !WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 600 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.RunicPower <=87 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 3 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <= 200 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >=10500 && WoW.RunicPower <=35 && !WoW.CanCast("Arcane Torrent") && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
                        	{
                           		WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) == 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
                        	{
                           		WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) != 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Blighted Rune Weapon") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(2) == 3 && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Blighted Rune Weapon") ;
								return;
							}
						}
						//Soul Reaper Rotation
                		if (WoW.Talent(7) == 3)
                		{
							//Cooldowns with Legendary Shoulders
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3 && WoW.SpellCooldownTimeRemaining("Dark Transformation") >= 1000 && WoW.Talent(7) != 1)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(7) != 1 && WoW.Talent(6) != 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 5500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(7) != 1 && WoW.Talent(6) == 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 3500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							//Cooldowns without Legendary Shoulders
							if (combatRoutine.UseCooldowns && !WoW.ItemOnCooldown("KBW") && isCheckHotkeyslegytrinket)
							{
								WoW.CastSpell("KBW") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3 && WoW.Talent(7) != 1)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Soul Reaper") && WoW.TargetDebuffStacks("Festering Wound") >=6 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 400 && WoW.CurrentRunes >= 4)
							{
								WoW.CastSpell("Soul Reaper") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Apocalypse") && WoW.TargetDebuffStacks("Festering Wound") >=6)
							{
								WoW.CastSpell("Apocalypse") ;
								return;
							}
							//Single Target Rotation
							if (WoW.CurrentRunes >= 1 && WoW.CanCast("Outbreak") && (!WoW.TargetHasDebuff("Virulent Plague") || WoW.TargetDebuffTimeRemaining("Virulent Plague") <= 200))
							{
								WoW.CastSpell("Outbreak") ;
								return;
							}
							if (!combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && WoW.CanCast("Soul Reaper") && WoW.TargetDebuffStacks("Festering Wound") >=3 && WoW.CurrentRunes >= 4)
							{
								WoW.CastSpell("Soul Reaper") ;
								return;
							}
							if (WoW.TargetHasDebuff("Soul Reaper2"))
							{
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
							}
							if (WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 1 && WoW.SpellCooldownTimeRemaining("Soul Reaper") <= 500)
							{
								if (WoW.CanCast("Festering Strike"))
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
							}
							if (!WoW.TargetHasDebuff("Soul Reaper2") && WoW.SpellCooldownTimeRemaining("Soul Reaper") >= 500)
							{
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Necrosis") && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 1 && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 1 && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Dark Transformation") && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 1 && WoW.Talent(7) != 1 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.RunicPower <=35 && WoW.CurrentRunes >= 2 && !WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 600 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.RunicPower <=87 && WoW.CurrentRunes >= 2)
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Blighted Rune Weapon") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(2) == 3 && WoW.CurrentRunes >= 3)
								{
									WoW.CastSpell("Blighted Rune Weapon") ;
									return;
								}
							}
							if (!WoW.TargetHasDebuff("Soul Reaper2") && WoW.SpellCooldownTimeRemaining("Soul Reaper") <= 500)
							{		
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Necrosis") && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 1 && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 1 && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Dark Transformation") && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 1 && WoW.Talent(7) != 1 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
							}
						}
						//Defile Rotation
                		if (WoW.Talent(7) == 2)
                		{
							//Cooldowns with Legendary Shoulders
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3 && WoW.SpellCooldownTimeRemaining("Dark Transformation") >= 1000)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) != 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 5500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) == 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 3500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							//Cooldowns without Legendary Shoulders
							if (combatRoutine.UseCooldowns && !WoW.ItemOnCooldown("KBW") && isCheckHotkeyslegytrinket)
							{
								WoW.CastSpell("KBW") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Apocalypse") && WoW.TargetDebuffStacks("Festering Wound") >=6)
							{
								WoW.CastSpell("Apocalypse") ;
								return;
							}
							//Single Target Rotation
							if (WoW.CurrentRunes >= 1 && WoW.CanCast("Outbreak") && (!WoW.TargetHasDebuff("Virulent Plague") || WoW.TargetDebuffTimeRemaining("Virulent Plague") <= 200))
							{
								WoW.CastSpell("Outbreak") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && !WoW.PlayerHasBuff("Necrosis") && WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 1 && WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}					
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) == 1 && !WoW.PlayerHasBuff("Dark Transformation") && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 1 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 3 && WoW.RunicPower <=35 && WoW.CurrentRunes >= 2 && !WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 3 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 600 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.RunicPower <=87 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) == 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
							{
                           		WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}				
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
                        	{
                            	WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}										
							if (WoW.CanCast("Defile") && WoW.TargetHasDebuff("Virulent Plague") && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Defile") ;
								return;
							}
							if (WoW.CanCast("Defile") && WoW.TargetHasDebuff("Virulent Plague") && WoW.CurrentRunes <= 1 && (WoW.RunicPower >=35 || WoW.PlayerHasBuff("Sudden Doom")))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Blighted Rune Weapon") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(2) == 3 && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Blighted Rune Weapon") ;
								return;
							}
						}
					}
				}
				if (combatRoutine.Type == RotationType.AOE) // AoE Rotation
				{
					if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted)
					{
						//Dark Arbiter Rotation
                		if (WoW.Talent(7) == 1)
                		{
							//Cooldowns with Legendary Shoulders
							if (combatRoutine.UseCooldowns && WoW.CanCast("Dark Arbiter") && isCheckHotkeyslegyshoulder && WoW.RunicPower <=70 && WoW.SpellCooldownTimeRemaining("Dark Transformation") >= 200)
							{
								WoW.CastSpell("Dark Arbiter") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 16500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) != 1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 5500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) == 1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 3500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							//Cooldowns without Legendary Shoulders
							if (combatRoutine.UseCooldowns && !WoW.ItemOnCooldown("KBW") && isCheckHotkeyslegytrinket)
							{
								WoW.CastSpell("KBW") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Dark Arbiter") && !isCheckHotkeyslegyshoulder && WoW.TargetDebuffStacks("Festering Wound") >=6 && WoW.RunicPower >=70)
							{
								WoW.CastSpell("Dark Arbiter") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Apocalypse") && WoW.TargetDebuffStacks("Festering Wound") >=6)
							{
								WoW.CastSpell("Apocalypse") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.PlayerRace == "BloodElf" && WoW.CanCast("Arcane Torrent") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >=10500 && WoW.Talent(7) == 1 && WoW.RunicPower <35)
							{
								WoW.CastSpell("Arcane Torrent");
								return;
							}
							//Multi Target Rotation
							if (WoW.CurrentRunes >= 1 && WoW.CanCast("Outbreak") && (!WoW.TargetHasDebuff("Virulent Plague") || WoW.TargetDebuffTimeRemaining("Virulent Plague") <= 200))
							{
								WoW.CastSpell("Outbreak") ;
								return;
							}
							if (WoW.CanCast("Death and Decay") && WoW.TargetHasDebuff("Virulent Plague") && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Death and Decay") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.RunicPower >=35 && WoW.RunicPower != 119 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >= 500 && WoW.PlayerHasBuff("Sudden Doom") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.RunicPower != 119 && WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes >= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >=10500 && WoW.RunicPower >=35)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(6) != 3 && WoW.RunicPower <=35 && WoW.CurrentRunes >= 2 && !WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(6) != 3 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 600 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(6) != 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.RunicPower <=87 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(6) == 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Festering Strike") && !WoW.CanCast("Death and Decay") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 3 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <= 200 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.SpellCooldownTimeRemaining("Dark Arbiter") >=10500 && WoW.RunicPower <=35 && !WoW.CanCast("Arcane Torrent") && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
                        	{
                           		WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) == 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
                        	{
                           		WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.SpellCooldownTimeRemaining("Dark Arbiter") <=10500 && WoW.Talent(7) == 1 && WoW.Talent(3) != 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Blighted Rune Weapon") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) == 3 && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Blighted Rune Weapon") ;
								return;
							}
							if (WoW.CanCast("Epidemic") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) == 1 && WoW.TargetDebuffStacks("Festering Wound") >=3 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Epidemic") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(2) == 1 && WoW.CanCast("Epidemic") && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(3) != 3 && !WoW.CanCast("Epidemic") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) == 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(3) == 3 && !WoW.CanCast("Epidemic") && !WoW.CanCast("Death and Decay") && WoW.Talent(2) == 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Death and Decay") && WoW.TargetHasDebuff("Virulent Plague") && WoW.CurrentRunes <= 1 && (WoW.RunicPower >=35 || WoW.PlayerHasBuff("Sudden Doom")))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
						}
						//Soul Reaper Rotation
                		if (WoW.Talent(7) == 3)
                		{
							//Cooldowns with Legendary Shoulders
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3 && WoW.SpellCooldownTimeRemaining("Dark Transformation") >= 1000 && WoW.Talent(7) != 1)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(7) != 1 && WoW.Talent(6) != 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 5500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(7) != 1 && WoW.Talent(6) == 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 3500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							//Cooldowns without Legendary Shoulders
							if (combatRoutine.UseCooldowns && !WoW.ItemOnCooldown("KBW") && isCheckHotkeyslegytrinket)
							{
								WoW.CastSpell("KBW") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3 && WoW.Talent(7) != 1)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Soul Reaper") && WoW.TargetDebuffStacks("Festering Wound") >=6 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 400 && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Soul Reaper") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Apocalypse") && WoW.TargetDebuffStacks("Festering Wound") >=6)
							{
								WoW.CastSpell("Apocalypse") ;
								return;
							}
							//Multi Target Rotation
							if (WoW.CurrentRunes >= 1 && WoW.CanCast("Outbreak") && (!WoW.TargetHasDebuff("Virulent Plague") || WoW.TargetDebuffTimeRemaining("Virulent Plague") <= 200))
							{
								WoW.CastSpell("Outbreak") ;
								return;
							}
							if (!combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && WoW.CanCast("Soul Reaper") && WoW.TargetDebuffStacks("Festering Wound") >=3 && WoW.CurrentRunes >= 4)
							{
								WoW.CastSpell("Soul Reaper") ;
								return;
							}
							if (WoW.TargetHasDebuff("Soul Reaper2"))
							{
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
							}
							if (WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 1 && WoW.SpellCooldownTimeRemaining("Soul Reaper") <= 500)
							{
								if (WoW.CanCast("Festering Strike"))
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
							}
							if (!WoW.TargetHasDebuff("Soul Reaper2") && WoW.SpellCooldownTimeRemaining("Soul Reaper") >= 500)
							{
								if (WoW.CanCast("Death and Decay") && WoW.TargetHasDebuff("Virulent Plague") && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Death and Decay") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Necrosis") && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 1 && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 1 && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Dark Transformation") && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 1 && WoW.Talent(7) != 1 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.RunicPower <=35 && WoW.CurrentRunes >= 2 && !WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 600 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.RunicPower <=87 && WoW.CurrentRunes >= 2)
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
								{
									WoW.CastSpell("Festering Strike") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Clawing Shadows") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
								{
									WoW.CastSpell("Scourge Strike") ;
									return;
								}
								if (WoW.CanCast("Blighted Rune Weapon") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(2) == 3 && WoW.CurrentRunes >= 3)
								{
									WoW.CastSpell("Blighted Rune Weapon") ;
									return;
								}
							}
							if (!WoW.TargetHasDebuff("Soul Reaper2") && WoW.SpellCooldownTimeRemaining("Soul Reaper") <= 500)
							{		
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Necrosis") && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(7) != 1 && WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 1 && WoW.PlayerHasBuff("Sudden Doom"))
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) == 1 && WoW.Talent(7) != 1 && !WoW.PlayerHasBuff("Dark Transformation") && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
								if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(6) != 1 && WoW.Talent(7) != 1 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
								{
									WoW.CastSpell("Death Coil") ;
									return;
								}
							}
						}
						//Defile Rotation
                		if (WoW.Talent(7) == 2)
                		{
							//Cooldowns with Legendary Shoulders
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3 && WoW.SpellCooldownTimeRemaining("Dark Transformation") >= 1000)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) != 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 5500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.Talent(6) == 1 && WoW.SpellCooldownTimeRemaining("Summon Gargoyle") >= 3500)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							//Cooldowns without Legendary Shoulders
							if (combatRoutine.UseCooldowns && !WoW.ItemOnCooldown("KBW") && isCheckHotkeyslegytrinket)
							{
								WoW.CastSpell("KBW") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && isCheckHotkeysUnholyOffensiveSummonGargoyle && WoW.CanCast("Summon Gargoyle") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Summon Gargoyle") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.TargetHasDebuff("Virulent Plague") && !isCheckHotkeyslegyshoulder && WoW.CanCast("Dark Transformation") && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Dark Transformation") ;
								return;
							}
							if (combatRoutine.UseCooldowns && WoW.CanCast("Apocalypse") && WoW.TargetDebuffStacks("Festering Wound") >=6)
							{
								WoW.CastSpell("Apocalypse") ;
								return;
							}
							//Multi Target Rotation
							if (WoW.CurrentRunes >= 1 && WoW.CanCast("Outbreak") && (!WoW.TargetHasDebuff("Virulent Plague") || WoW.TargetDebuffTimeRemaining("Virulent Plague") <= 200))
							{
								WoW.CastSpell("Outbreak") ;
								return;
							}
							if (WoW.CanCast("Defile") && WoW.TargetHasDebuff("Virulent Plague") && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Defile") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.RunicPower >=90 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && !WoW.PlayerHasBuff("Necrosis") && WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 1 && WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("Necrosis") && WoW.CurrentRunes <= 3 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}					
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) == 1 && !WoW.PlayerHasBuff("Dark Transformation") && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Death Coil") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 1 && WoW.RunicPower >=35 && WoW.RunicPower != 119)
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 3 && WoW.RunicPower <=35 && WoW.CurrentRunes >= 2 && !WoW.PlayerHasBuff("Sudden Doom"))
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 3 && WoW.SpellCooldownTimeRemaining("Apocalypse") >= 600 && WoW.TargetDebuffStacks("Festering Wound") <=6 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) != 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.RunicPower <=87 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(6) == 3 && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.CurrentRunes >= 1)
							{
                           		WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) != 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}				
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.CurrentRunes >= 1)
                        	{
                            	WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Necrosis") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.PlayerHasBuff("Unholy Strength") && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) != 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.Talent(3) == 3 && WoW.RunicPower <=89 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}										
							if (WoW.CanCast("Defile") && WoW.TargetHasDebuff("Virulent Plague") && WoW.CurrentRunes <= 1 && (WoW.RunicPower >=35 || WoW.PlayerHasBuff("Sudden Doom")))
							{
								WoW.CastSpell("Death Coil") ;
								return;
							}
							if (WoW.CanCast("Blighted Rune Weapon") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(2) == 3 && WoW.CurrentRunes >= 3)
							{
								WoW.CastSpell("Blighted Rune Weapon") ;
								return;
							}
							if (WoW.CanCast("Epidemic") && WoW.TargetHasDebuff("Virulent Plague") && !WoW.CanCast("Defile") && WoW.Talent(2) == 1 && WoW.TargetDebuffStacks("Festering Wound") >=3 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Epidemic") ;
								return;
							}
							if (WoW.CanCast("Festering Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(2) == 1 && WoW.CanCast("Epidemic") && WoW.TargetDebuffStacks("Festering Wound") <=3 && WoW.CurrentRunes >= 2)
							{
								WoW.CastSpell("Festering Strike") ;
								return;
							}
							if (WoW.CanCast("Scourge Strike") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(3) != 3 && !WoW.CanCast("Epidemic") && !WoW.CanCast("Defile") && WoW.Talent(2) == 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Scourge Strike") ;
								return;
							}
							if (WoW.CanCast("Clawing Shadows") && WoW.TargetHasDebuff("Virulent Plague") && WoW.Talent(3) == 3 && !WoW.CanCast("Epidemic") && !WoW.CanCast("Defile") && WoW.Talent(2) == 1 && WoW.TargetDebuffStacks("Festering Wound") >=1 && WoW.CurrentRunes >= 1)
							{
								WoW.CastSpell("Clawing Shadows") ;
								return;
							}
						}
					}
				}
			}
		}
	}
	

    public class SettingsFormDFF : Form
    {
        public Button btnaddspell;
        public Button btnHotkeysUnholyOffensiveCooldowns;
        public Button btnremovespell;
		public CheckBox checkHotkeysFrostDeathstrike;
        public TextBox checkHotkeysFrostDeathstrikePercent;
        public Label checkHotkeysFrostDeathstrikePercentLabel;
        public CheckBox checkHotkeysFrostFreeDeathstrike;
        public TextBox checkHotkeysFrostFreeDeathstrikePercent;
        public Label checkHotkeysFrostFreeDeathstrikePercentLabel;
        public CheckBox checkHotkeysFrostKick;
        public TextBox checkHotkeysFrostKickPercent;
        public Label checkHotkeysFrostKickPercentLabel;
        public TextBox checkHotkeysUnholyAMSPercent;
        private readonly Label checkHotkeysUnholyAMSPercentLabel;
        public CheckBox checkHotkeysUnholyAntiMagicShield;
        public CheckBox checkHotkeysUnholyIceboundFortitude;
        public CheckBox checkHotkeyslegyshoulder;
        public CheckBox checkHotkeyslegytrinket;
        public TextBox checkHotkeysUnholyIFPercent;
        public Label checkHotkeysUnholyIFPercentLabel;
        public CheckBox checkHotkeysUnholyOffensiveApocalypse;
        public CheckBox checkHotkeysUnholyOffensiveSummonGargoyle;


        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private IContainer components = null;

        private readonly GroupBox groupBox12;
        private readonly GroupBox groupBox13;
        private readonly GroupBox groupBox22;
        private readonly Label spellIdLabel;
        public ListBox spellList;
        public TextBox spellText;
        private readonly TabControl tabControl3;
        private readonly TabPage tabPage5;


        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        public SettingsFormDFF()
        {
            this.spellList = new System.Windows.Forms.ListBox();
            this.spellText = new System.Windows.Forms.TextBox();
            this.spellIdLabel = new System.Windows.Forms.Label();
            this.btnaddspell = new System.Windows.Forms.Button();
            this.btnremovespell = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysFrostDeathstrike = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostDeathstrikePercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostDeathstrikePercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysFrostFreeDeathstrike = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostFreeDeathstrikePercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostFreeDeathstrikePercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysFrostKick = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostKickPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostKickPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysUnholyIceboundFortitude = new System.Windows.Forms.CheckBox();
            this.checkHotkeyslegyshoulder = new System.Windows.Forms.CheckBox();
            this.checkHotkeyslegytrinket = new System.Windows.Forms.CheckBox();
            this.checkHotkeysUnholyAntiMagicShield = new System.Windows.Forms.CheckBox();
            this.checkHotkeysUnholyIFPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysUnholyIFPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysUnholyAMSPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysUnholyAMSPercentLabel = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysUnholyOffensiveApocalypse = new System.Windows.Forms.CheckBox();
            this.checkHotkeysUnholyOffensiveSummonGargoyle = new System.Windows.Forms.CheckBox();
            this.btnHotkeysUnholyOffensiveCooldowns = new System.Windows.Forms.Button();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage5.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.tabControl3.SuspendLayout();
            /*
            this.spellIdLabel = new System.Windows.Forms.Label();
            this.btnaddspell = new System.Windows.Forms.Button();
            this.btnremovespell
            */
            // 
            // btnaddspell
            // 
            this.btnaddspell.Location = new System.Drawing.Point(110, 50);
            this.btnaddspell.Name = "btnaddspell";
            this.btnaddspell.Size = new System.Drawing.Size(28, 48);
            this.btnaddspell.TabIndex = 1;
            this.btnaddspell.Text = "+";
            this.btnaddspell.UseVisualStyleBackColor = true;

            // 
            // btnremovespell
            // 
            this.btnremovespell.Location = new System.Drawing.Point(110, 100);
            this.btnremovespell.Name = "btnremovespell";
            this.btnremovespell.Size = new System.Drawing.Size(28, 48);
            this.btnremovespell.TabIndex = 1;
            this.btnremovespell.Text = "-";
            this.btnremovespell.UseVisualStyleBackColor = true;
            // 
            // spellIdLabel
            // 
            this.spellIdLabel.AutoSize = true;
            this.spellIdLabel.Location = new System.Drawing.Point(28, 28);
            this.spellIdLabel.Name = "spellIdLabel";
            this.spellIdLabel.Size = new System.Drawing.Size(28, 28);
            this.spellIdLabel.TabIndex = 9;
            this.spellIdLabel.Text = "Spell ID:";
            // 
            // spellText
            // 
            this.spellText.AutoSize = true;
            this.spellText.Location = new System.Drawing.Point(28, 50);
            this.spellText.Name = "spellText";
            this.spellText.Size = new System.Drawing.Size(80, 28);
            this.spellText.TabIndex = 9;
            this.spellText.Text = "";

            // 
            // spellList
            // 
            this.spellList.AutoSize = false;
            this.spellList.Location = new System.Drawing.Point(28, 75);
            this.spellList.Name = "spellList";
            this.spellList.Size = new System.Drawing.Size(80, 290);
            this.spellList.TabIndex = 9;

            this.spellList.Text = "spellList";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox12);
            this.tabPage5.Controls.Add(this.groupBox13);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(582, 406);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Hotkeys";
            this.tabPage5.UseVisualStyleBackColor = true;

            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.checkHotkeysFrostDeathstrike);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostDeathstrikePercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostDeathstrikePercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostFreeDeathstrike);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostFreeDeathstrikePercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostFreeDeathstrikePercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostKick);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostKickPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostKickPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysUnholyIceboundFortitude);
            this.groupBox12.Controls.Add(this.checkHotkeysUnholyIFPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysUnholyIFPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysUnholyAntiMagicShield);
            this.groupBox12.Controls.Add(this.checkHotkeysUnholyAMSPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysUnholyAMSPercentLabel);

            this.groupBox12.Location = new System.Drawing.Point(8, 100);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(561, 140);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Defensive Cooldowns";
            // 
            // checkHotkeysUnholyIceboundFortitude
            // 
            this.checkHotkeysUnholyIceboundFortitude.AutoSize = true;
            this.checkHotkeysUnholyIceboundFortitude.Location = new System.Drawing.Point(151, 28);
            this.checkHotkeysUnholyIceboundFortitude.Name = "checkHotkeysUnholyIceboundFortitude";
            this.checkHotkeysUnholyIceboundFortitude.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysUnholyIceboundFortitude.TabIndex = 9;
            this.checkHotkeysUnholyIceboundFortitude.Text = "Icebound Fortitude";
            this.checkHotkeysUnholyIceboundFortitude.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysUnholyIFPercent
            // 
            this.checkHotkeysUnholyIFPercent.AutoSize = true;
            this.checkHotkeysUnholyIFPercent.Location = new System.Drawing.Point(300, 28);
            this.checkHotkeysUnholyIFPercent.Name = "checkHotkeysUnholyIFPercent";
            this.checkHotkeysUnholyIFPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysUnholyIFPercent.TabIndex = 9;
            this.checkHotkeysUnholyIFPercent.Text = "50";
            // 
            // checkHotkeysUnholyIFPercent
            // 
            this.checkHotkeysUnholyIFPercentLabel.AutoSize = true;
            this.checkHotkeysUnholyIFPercentLabel.Location = new System.Drawing.Point(321, 30);
            this.checkHotkeysUnholyIFPercentLabel.Name = "checkHotkeysUnholyIFPercentLabel";
            this.checkHotkeysUnholyIFPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysUnholyIFPercentLabel.TabIndex = 9;
            this.checkHotkeysUnholyIFPercentLabel.Text = "% HP";

            // 
            // checkHotkeysUnholyAntiMagicShield
            // 
            this.checkHotkeysUnholyAntiMagicShield.AutoSize = true;
            this.checkHotkeysUnholyAntiMagicShield.Location = new System.Drawing.Point(151, 50);
            this.checkHotkeysUnholyAntiMagicShield.Name = "checkHotkeysUnholyAntiMagicShield";
            this.checkHotkeysUnholyAntiMagicShield.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysUnholyAntiMagicShield.TabIndex = 8;
            this.checkHotkeysUnholyAntiMagicShield.Text = "Anti-Magic Shield";
            this.checkHotkeysUnholyAntiMagicShield.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysUnholyAMSPercent
            // 
            this.checkHotkeysUnholyAMSPercent.AutoSize = true;
            this.checkHotkeysUnholyAMSPercent.Location = new System.Drawing.Point(300, 50);
            this.checkHotkeysUnholyAMSPercent.Name = "checkHotkeysUnholyIFPercent";
            this.checkHotkeysUnholyAMSPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysUnholyAMSPercent.TabIndex = 9;
            this.checkHotkeysUnholyAMSPercent.Text = "50";
            // 
            // checkHotkeysUnholyAMSPercentLabel
            // 
            this.checkHotkeysUnholyAMSPercentLabel.AutoSize = true;
            this.checkHotkeysUnholyAMSPercentLabel.Location = new System.Drawing.Point(321, 52);
            this.checkHotkeysUnholyAMSPercentLabel.Name = "checkHotkeysUnholyAMSPercentLabel";
            this.checkHotkeysUnholyAMSPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysUnholyAMSPercentLabel.TabIndex = 9;
            this.checkHotkeysUnholyAMSPercentLabel.Text = "% HP";
           // 
            // checkHotkeysFrostDeathstrike
            // 
            this.checkHotkeysFrostDeathstrike.AutoSize = true;
            this.checkHotkeysFrostDeathstrike.Location = new System.Drawing.Point(151, 72);
            this.checkHotkeysFrostDeathstrike.Name = "checkHotkeysFrostDeathstrike";
            this.checkHotkeysFrostDeathstrike.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysFrostDeathstrike.TabIndex = 9;
            this.checkHotkeysFrostDeathstrike.Text = "Deathstrike";
            this.checkHotkeysFrostDeathstrike.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysFrostDeathstrikePercent
            // 
            this.checkHotkeysFrostDeathstrikePercent.AutoSize = true;
            this.checkHotkeysFrostDeathstrikePercent.Location = new System.Drawing.Point(300, 72);
            this.checkHotkeysFrostDeathstrikePercent.Name = "checkHotkeysFrostDeathstrikePercent";
            this.checkHotkeysFrostDeathstrikePercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostDeathstrikePercent.TabIndex = 9;
            this.checkHotkeysFrostDeathstrikePercent.Text = "50";
            // 
            // checkHotkeysFrostDeathstrikeLabel
            // 
            this.checkHotkeysFrostDeathstrikePercentLabel.AutoSize = true;
            this.checkHotkeysFrostDeathstrikePercentLabel.Location = new System.Drawing.Point(321, 74);
            this.checkHotkeysFrostDeathstrikePercentLabel.Name = "checkHotkeysFrostDeathstrikePercentLabel";
            this.checkHotkeysFrostDeathstrikePercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostDeathstrikePercentLabel.TabIndex = 9;
            this.checkHotkeysFrostDeathstrikePercentLabel.Text = "% HP";
			
            // 
            // checkHotkeysFrostFreeDeathstrike
            // 
            this.checkHotkeysFrostFreeDeathstrike.AutoSize = true;
            this.checkHotkeysFrostFreeDeathstrike.Location = new System.Drawing.Point(151, 94);
            this.checkHotkeysFrostFreeDeathstrike.Name = "checkHotkeysFrostFreeDeathstrike";
            this.checkHotkeysFrostFreeDeathstrike.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysFrostFreeDeathstrike.TabIndex = 9;
            this.checkHotkeysFrostFreeDeathstrike.Text = "Free Deathstrike";
            this.checkHotkeysFrostFreeDeathstrike.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysFrostFreeDeathstrikePercent
            // 
            this.checkHotkeysFrostFreeDeathstrikePercent.AutoSize = true;
            this.checkHotkeysFrostFreeDeathstrikePercent.Location = new System.Drawing.Point(300, 94);
            this.checkHotkeysFrostFreeDeathstrikePercent.Name = "checkHotkeysFrostFreeDeathstrikePercent";
            this.checkHotkeysFrostFreeDeathstrikePercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostFreeDeathstrikePercent.TabIndex = 9;
            this.checkHotkeysFrostFreeDeathstrikePercent.Text = "50";
            // 
            // checkHotkeysFrostFreeDeathstrikeLabel
            // 
            this.checkHotkeysFrostFreeDeathstrikePercentLabel.AutoSize = true;
            this.checkHotkeysFrostFreeDeathstrikePercentLabel.Location = new System.Drawing.Point(321, 96);
            this.checkHotkeysFrostFreeDeathstrikePercentLabel.Name = "checkHotkeysFrostFreeDeathstrikePercentLabel";
            this.checkHotkeysFrostFreeDeathstrikePercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostFreeDeathstrikePercentLabel.TabIndex = 9;
            this.checkHotkeysFrostFreeDeathstrikePercentLabel.Text = "% HP";
			
            // 
            // checkHotkeysFrostKick
            // 
            this.checkHotkeysFrostKick.AutoSize = true;
            this.checkHotkeysFrostKick.Location = new System.Drawing.Point(151, 116);
            this.checkHotkeysFrostKick.Name = "checkHotkeysFrostKick";
            this.checkHotkeysFrostKick.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysFrostKick.TabIndex = 9;
            this.checkHotkeysFrostKick.Text = "Mind Freeze";
            this.checkHotkeysFrostKick.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysFrostKickPercent
            // 
            this.checkHotkeysFrostKickPercent.AutoSize = true;
            this.checkHotkeysFrostKickPercent.Location = new System.Drawing.Point(300, 116);
            this.checkHotkeysFrostKickPercent.Name = "checkHotkeysFrostKickPercent";
            this.checkHotkeysFrostKickPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostKickPercent.TabIndex = 9;
            this.checkHotkeysFrostKickPercent.Text = "50";
            // 
            // checkHotkeysFrostKickLabel
            // 
            this.checkHotkeysFrostKickPercentLabel.AutoSize = true;
            this.checkHotkeysFrostKickPercentLabel.Location = new System.Drawing.Point(321, 118);
            this.checkHotkeysFrostKickPercentLabel.Name = "checkHotkeysFrostKickPercentLabel";
            this.checkHotkeysFrostKickPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostKickPercentLabel.TabIndex = 9;
            this.checkHotkeysFrostKickPercentLabel.Text = "% of Cast";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.checkHotkeysUnholyOffensiveApocalypse);
            this.groupBox13.Controls.Add(this.checkHotkeysUnholyOffensiveSummonGargoyle);
            this.groupBox13.Controls.Add(this.btnHotkeysUnholyOffensiveCooldowns);
            this.groupBox13.Controls.Add(this.checkHotkeyslegyshoulder);
            this.groupBox13.Controls.Add(this.checkHotkeyslegytrinket);
            this.groupBox13.Location = new System.Drawing.Point(8, 8);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(556, 90);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Offensive Cooldowns";

            // 
            // checkHotkeysUnholyOffensiveApocalypse
            // 
            this.checkHotkeysUnholyOffensiveApocalypse.AutoSize = true;
            this.checkHotkeysUnholyOffensiveApocalypse.Location = new System.Drawing.Point(151, 60);
            this.checkHotkeysUnholyOffensiveApocalypse.Name = "checkHotkeysUnholyOffensiveApocalypse";
            this.checkHotkeysUnholyOffensiveApocalypse.Size = new System.Drawing.Size(48, 17);
            this.checkHotkeysUnholyOffensiveApocalypse.TabIndex = 3;
            this.checkHotkeysUnholyOffensiveApocalypse.Text = "Apocalypse";
            this.checkHotkeysUnholyOffensiveApocalypse.UseVisualStyleBackColor = true;

            // 
            // checkHotkeysUnholyOffensiveSummonGargoyle
            // 
            this.checkHotkeysUnholyOffensiveSummonGargoyle.AutoSize = true;
            this.checkHotkeysUnholyOffensiveSummonGargoyle.Location = new System.Drawing.Point(151, 32);
            this.checkHotkeysUnholyOffensiveSummonGargoyle.Name = "checkHotkeysUnholyOffensiveSummonGargoyle";
            this.checkHotkeysUnholyOffensiveSummonGargoyle.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysUnholyOffensiveSummonGargoyle.TabIndex = 2;
            this.checkHotkeysUnholyOffensiveSummonGargoyle.Text = "Summon Gargoyle";
            this.checkHotkeysUnholyOffensiveSummonGargoyle.UseVisualStyleBackColor = true;

            // 
            // checkHotkeyslegyshoulder
            // 
            this.checkHotkeyslegyshoulder.AutoSize = true;
            this.checkHotkeyslegyshoulder.Location = new System.Drawing.Point(300, 32);
            this.checkHotkeyslegyshoulder.Name = "checkHotkeyslegyshoulder";
            this.checkHotkeyslegyshoulder.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeyslegyshoulder.TabIndex = 4;
            this.checkHotkeyslegyshoulder.Text = "Legendary Shoulder";
            this.checkHotkeyslegyshoulder.UseVisualStyleBackColor = true;
			
            // 
            // checkHotkeyslegytrinket
            // 
            this.checkHotkeyslegytrinket.AutoSize = true;
            this.checkHotkeyslegytrinket.Location = new System.Drawing.Point(300, 60);
            this.checkHotkeyslegytrinket.Name = "checkHotkeyslegytrinket";
            this.checkHotkeyslegytrinket.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeyslegytrinket.TabIndex = 5;
            this.checkHotkeyslegytrinket.Text = "Legendary Trinket";
            this.checkHotkeyslegytrinket.UseVisualStyleBackColor = true;
			
            // 
            // btnHotkeysUnholyOffensiveCooldowns
            // 
            this.btnHotkeysUnholyOffensiveCooldowns.Location = new System.Drawing.Point(18, 28);
            this.btnHotkeysUnholyOffensiveCooldowns.Name = "btnHotkeysUnholyOffensiveCooldowns";
            this.btnHotkeysUnholyOffensiveCooldowns.Size = new System.Drawing.Size(113, 23);
            this.btnHotkeysUnholyOffensiveCooldowns.TabIndex = 1;
            this.btnHotkeysUnholyOffensiveCooldowns.Text = "Click to Set";
            this.btnHotkeysUnholyOffensiveCooldowns.UseVisualStyleBackColor = true;
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
            this.Text = "SettingsForm";
            this.tabPage5.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
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
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,77575,Outbreak,D1
Spell,63560,Dark Transformation,OemOpenBrackets
Spell,85948,Festering Strike,D3
Spell,220143,Apocalypse,D0
Spell,130736,Soul Reaper,D9
Spell,55090,Scourge Strike,D4
Spell,207311,Clawing Shadows,D4
Spell,47541,Death Coil,D5
Spell,43265,Death and Decay,D6
Spell,152280,Defile,D6
Spell,207317,Epidemic,D7
Spell,49206,Summon Gargoyle,D8
Spell,207349,Dark Arbiter,D8
Spell,48707,Anti-Magic Shell,NumPad2
Spell,48792,Icebound Fortitude,NumPad3
Spell,46584,Raise Dead,NumPad5
Spell,194918,Blighted Rune Weapon,NumPad6
Spell,235991,KBW,T
Spell,49998,Death Strike,NumPad1
Spell,47528,Mind Freeze,NumPad4
Spell,80483,Arcane Torrent,F3
Aura,101568,Free DeathStrike
Aura,191587,Virulent Plague
Aura,194310,Festering Wound
Aura,207305,Castigator
Aura,215711,Soul Reaper
Aura,130736,Soul Reaper2
Aura,212756,Ignoble Sacrifice
Aura,63560,Dark Transformation
Aura,81340,Sudden Doom
Aura,170761,Necrosis
Aura,53365,Unholy Strength
Item,144259,KBW
*/
