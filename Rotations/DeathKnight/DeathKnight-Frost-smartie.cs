//Changelog
// v2.0 fmflex rotation
// v2.1 fixed for latest pm, removed kick settings, removed double reload
// v2.2 added hotkey for Sindragosa's Fury
// v2.3 added support for Legendary Ring
// v2.4 Auto Talent detection added
// v2.5 Bugfixes
// v2.6 added Levelcheck, so ppl can use rota for leveling
// v2.7 removed Levelcheck cause it buggy
// v2.8 cleanup and added Racials
// v2.9 Racial update
// v3.0 added Deathstrike

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
    public class DKFrostMGFmflex : CombatRoutine
    {
        private bool haveCoF = true;
        private bool haveHRW = true;

        private static readonly Stopwatch coolDownStopWatch = new Stopwatch();
        private bool useNextHRWCharge = false;
		private bool legyringtest = true;

        public override string Name
        {
            get { return "Frost DK"; }
        }

        public override string Class
        {
            get { return "Deathknight"; }
        }

        public override Form SettingsForm { get; set; }
        public SettingsFormDFF SettingsFormDFF { get; set; }

        public static int cooldownKey
        {
            get
            {
                var cooldownKey = ConfigFile.ReadValue("DKFrost", "cooldownKey").Trim();
                if (cooldownKey != "")
                {
                    return Convert.ToInt32(cooldownKey);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DkFrost", "cooldownKey", value.ToString()); }
        }

        public static int cooldownModifier
        {
            get
            {
                var cooldownModifier = ConfigFile.ReadValue("DKFrost", "cooldownModifier").Trim();
                if (cooldownModifier != "")
                {
                    return Convert.ToInt32(cooldownModifier);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DkFrost", "cooldownModifier", value.ToString()); }
        }

        public static string cooldownHotKeyString
        {
            get
            {
                var cooldownHotKeyString = ConfigFile.ReadValue("DkFrost", "cooldownHotKeyString").Trim();

                if (cooldownHotKeyString != "")
                {
                    return cooldownHotKeyString;
                }

                return "Click to Set";
            }
            set { ConfigFile.WriteValue("DkFrost", "cooldownHotKeyString", value); }
        }
		
        public static bool isCheckHotkeyslegyring
        {
            get
            {
                var isCheckHotkeyslegyring = ConfigFile.ReadValue("DkFrost", "isCheckHotkeyslegyring").Trim();

                if (isCheckHotkeyslegyring != "")
                {
                    return Convert.ToBoolean(isCheckHotkeyslegyring);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeyslegyring", value.ToString()); }
        }
		
        public static bool isCheckHotkeysRacials
        {
            get
            {
                var isCheckHotkeysRacials = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysRacials").Trim();

                if (isCheckHotkeysRacials != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysRacials);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysRacials", value.ToString()); }
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

        public static bool isCheckHotkeysFrostIceboundFortitude
        {
            get
            {
                var isCheckHotkeysFrostIceboundFortitude = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostIceboundFortitude").Trim();

                if (isCheckHotkeysFrostIceboundFortitude != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostIceboundFortitude);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostIceboundFortitude", value.ToString()); }
        }

        public static int FrostIceboundHPPercent
        {
            get
            {
                var FrostIceboundHPPercent = ConfigFile.ReadValue("DKFrost", "FrostIceboundHPPercent").Trim();
                if (FrostIceboundHPPercent != "")
                {
                    return Convert.ToInt32(FrostIceboundHPPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DkFrost", "FrostIceboundHPPercent", value.ToString()); }
        }

        public static bool isCheckHotkeysFrostAntiMagicShield
        {
            get
            {
                var isCheckHotkeysFrostAntiMagicShield = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostAntiMagicShield").Trim();

                if (isCheckHotkeysFrostAntiMagicShield != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostAntiMagicShield);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostAntiMagicShield", value.ToString()); }
        }

        public static int FrostAMSHPPercent
        {
            get
            {
                var FrostAMSHPPercent = ConfigFile.ReadValue("DKFrost", "FrostAMSHPPercent").Trim();
                if (FrostAMSHPPercent != "")
                {
                    return Convert.ToInt32(FrostAMSHPPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DkFrost", "FrostAMSHPPercent", value.ToString()); }
        }

        public static bool isCheckHotkeysFrostOffensiveErW
        {
            get
            {
                var isCheckHotkeysFrostOffensiveErW = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostOffensiveErW").Trim();

                if (isCheckHotkeysFrostOffensiveErW != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostOffensiveErW);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostOffensiveErW", value.ToString()); }
        }

        public static bool isCheckHotkeysFrostOffensivePillarofFrost
        {
            get
            {
                var isCheckHotkeysFrostOffensivePillarofFrost = ConfigFile.ReadValue("DkFrost", "isCheckHotkeysFrostOffensivePillarofFrost").Trim();

                if (isCheckHotkeysFrostOffensivePillarofFrost != "")
                {
                    return Convert.ToBoolean(isCheckHotkeysFrostOffensivePillarofFrost);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DkFrost", "isCheckHotkeysFrostOffensivePillarofFrost", value.ToString()); }
        }

        public override void Initialize()
        {
            Log.Write("Welcome to the Frost DK v3.0", Color.Green);
	        Log.Write("All Talents supported and auto detected", Color.Green);		
			Log.Write("Hold down Z key (Y for US) for Sindragosa's Fury", Color.Red);
            SettingsFormDFF = new SettingsFormDFF();
            SettingsForm = SettingsFormDFF;

            SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.Text = cooldownHotKeyString;
            SettingsFormDFF.checkHotkeysFrostOffensiveErW.Checked = isCheckHotkeysFrostOffensiveErW;
            SettingsFormDFF.checkHotkeysFrostOffensivePillarofFrost.Checked = isCheckHotkeysFrostOffensivePillarofFrost;
            SettingsFormDFF.checkHotkeysFrostAntiMagicShield.Checked = isCheckHotkeysFrostAntiMagicShield;
            SettingsFormDFF.checkHotkeysFrostAMSPercent.Text = FrostAMSHPPercent.ToString();
            SettingsFormDFF.checkHotkeysFrostIceboundFortitude.Checked = isCheckHotkeysFrostIceboundFortitude;
            SettingsFormDFF.checkHotkeysFrostIFPercent.Text = FrostIceboundHPPercent.ToString();
            SettingsFormDFF.checkHotkeysFrostDeathstrike.Checked = isCheckHotkeysFrostDeathstrike;
            SettingsFormDFF.checkHotkeysFrostDeathstrikePercent.Text = FrostDeathstrikeHPPercent.ToString();
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrike.Checked = isCheckHotkeysFrostFreeDeathstrike;
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrikePercent.Text = FrostFreeDeathstrikeHPPercent.ToString();
            SettingsFormDFF.checkHotkeysFrostKick.Checked = isCheckHotkeysFrostKick;
            SettingsFormDFF.checkHotkeysFrostKickPercent.Text = FrostKickPercent.ToString();
            SettingsFormDFF.checkHotkeyslegyring.Checked = isCheckHotkeyslegyring;
            SettingsFormDFF.checkHotkeysRacials.Checked = isCheckHotkeysRacials;
			
            SettingsFormDFF.checkHotkeysFrostIceboundFortitude.CheckedChanged += isCheckHotkeysFrostIceboundFortitude_Click;
            SettingsFormDFF.checkHotkeysFrostIFPercent.TextChanged += isCheckHotkeysFrostIFPercent_Click;	
            SettingsFormDFF.checkHotkeysFrostDeathstrike.CheckedChanged += isCheckHotkeysFrostDeathstrike_Click;
            SettingsFormDFF.checkHotkeysFrostDeathstrikePercent.TextChanged += isCheckHotkeysFrostDeathstrikePercent_Click;
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrike.CheckedChanged += isCheckHotkeysFrostFreeDeathstrike_Click;
            SettingsFormDFF.checkHotkeysFrostFreeDeathstrikePercent.TextChanged += isCheckHotkeysFrostFreeDeathstrikePercent_Click;
            SettingsFormDFF.checkHotkeysFrostKick.CheckedChanged += isCheckHotkeysFrostKick_Click;
            SettingsFormDFF.checkHotkeysFrostKickPercent.TextChanged += isCheckHotkeysFrostKickPercent_Click;
            SettingsFormDFF.checkHotkeysFrostAntiMagicShield.CheckedChanged += isCheckHotkeysFrostAntiMagicShield_Click;
            SettingsFormDFF.checkHotkeysFrostAMSPercent.TextChanged += isCheckHotkeysFrostAMSPercent_Click;
            SettingsFormDFF.checkHotkeyslegyring.CheckedChanged += isCheckHotkeyslegyring_Click;
            SettingsFormDFF.checkHotkeysRacials.CheckedChanged += isCheckHotkeysRacials_Click;
            SettingsFormDFF.checkHotkeysFrostOffensivePillarofFrost.CheckedChanged += isCheckHotkeysFrostOffensivePillarofFrost_Click;
            SettingsFormDFF.checkHotkeysFrostOffensiveErW.CheckedChanged += isCheckHotkeysFrostOffensiveErW_Click;
            SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.KeyDown += KeyDown;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
                return;
            SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.Text = "Hotkey : ";
            if (e.Shift)
            {
                cooldownModifier = (int)Keys.ShiftKey;
                SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.Text += Keys.Shift + " + ";
            }
            else if (e.Alt)
            {
                cooldownModifier = (int)Keys.Menu;
                SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.Text += Keys.Alt + " + ";
            }
            else if (e.Control)
            {
                cooldownModifier = (int)Keys.ControlKey;
                SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.Text += Keys.Control + " + ";
            }
            else cooldownModifier = -1;
            cooldownKey = (int)e.KeyCode;
            SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.Text += e.KeyCode;
            cooldownHotKeyString = SettingsFormDFF.btnHotkeysFrostOffensiveCooldowns.Text;
            SettingsFormDFF.checkHotkeysFrostIFPercentLabel.Focus();
        }

        private void isCheckHotkeysFrostIceboundFortitude_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostIceboundFortitude = SettingsFormDFF.checkHotkeysFrostIceboundFortitude.Checked;
        }
		
        private void isCheckHotkeysFrostIFPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysFrostIFPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                FrostIceboundHPPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysFrostIFPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
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
		
        private void isCheckHotkeysFrostAntiMagicShield_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostAntiMagicShield = SettingsFormDFF.checkHotkeysFrostAntiMagicShield.Checked;
        }

        private void isCheckHotkeysFrostAMSPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysFrostAMSPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                FrostAMSHPPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysFrostAMSPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void isCheckHotkeyslegyring_Click(object sender, EventArgs e)
        {
            isCheckHotkeyslegyring = SettingsFormDFF.checkHotkeyslegyring.Checked;
        }
		
        private void isCheckHotkeysRacials_Click(object sender, EventArgs e)
        {
            isCheckHotkeysRacials = SettingsFormDFF.checkHotkeysRacials.Checked;
        }
		
        private void isCheckHotkeysFrostOffensivePillarofFrost_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostOffensivePillarofFrost = SettingsFormDFF.checkHotkeysFrostOffensivePillarofFrost.Checked;
        }

        private void isCheckHotkeysFrostOffensiveErW_Click(object sender, EventArgs e)
        {
            isCheckHotkeysFrostOffensiveErW = SettingsFormDFF.checkHotkeysFrostOffensiveErW.Checked;
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
            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted)
            {
                if (WoW.CanCast("Sindragosa's Fury") && DetectKeyPress.GetKeyState(0x5A) < 0 )
                {																
                    WoW.CastSpell("Sindragosa's Fury");
                    return;
                }
                if (WoW.CanCast("Mind Freeze") && isCheckHotkeysFrostKick && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= FrostKickPercent && !WoW.IsSpellOnCooldown("Mind Freeze") && !WoW.PlayerIsChanneling)
                {
                    WoW.CastSpell("Mind Freeze");						
                    return;
                }
                if (!WoW.IsSpellOnCooldown("Anti-Magic Shell") && WoW.HealthPercent <= FrostAMSHPPercent && !WoW.IsSpellOnCooldown("Anti-Magic Shell") && isCheckHotkeysFrostAntiMagicShield)
                {
                    WoW.CastSpell("Anti-Magic Shell");
                }
                if (!WoW.IsSpellOnCooldown("Icebound Fortitude") && WoW.HealthPercent < FrostIceboundHPPercent && !WoW.IsSpellOnCooldown("Icebound Fortitude") && isCheckHotkeysFrostIceboundFortitude)
                {
                    WoW.CastSpell("Icebound Fortitude");
                }
				if (WoW.CanCast("Obliterate", false, false, true, false, false) && isCheckHotkeysFrostFreeDeathstrike && WoW.HealthPercent <= FrostFreeDeathstrikeHPPercent && WoW.PlayerHasBuff("Free DeathStrike"))
				{
					WoW.CastSpell("Death Strike");
					return;
				}
				if (WoW.CanCast("Obliterate", false, false, true, false, false) && isCheckHotkeysFrostDeathstrike && WoW.HealthPercent <= FrostDeathstrikeHPPercent && WoW.RunicPower >=45)
				{
					WoW.CastSpell("Death Strike");
					return;
				}
            }
			if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave || combatRoutine.Type == RotationType.AOE)
			{
				if (WoW.Talent(7) == 2 && WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted)
				{
					if (combatRoutine.UseCooldowns && isCheckHotkeysFrostOffensivePillarofFrost && !WoW.IsSpellOnCooldown("PillarofFrost") && WoW.PlayerHasBuff("Breath"))
					{
						WoW.CastSpell("PillarofFrost");
					}
					if (combatRoutine.UseCooldowns && isCheckHotkeysFrostOffensivePillarofFrost && !WoW.IsSpellOnCooldown("PillarofFrost") && !WoW.PlayerHasBuff("Breath") && WoW.SpellCooldownTimeRemaining("Breath") >= 5900)
					{
						WoW.CastSpell("PillarofFrost");
					}
					if ((haveCoF || useNextHRWCharge) && isCheckHotkeyslegyring && haveHRW && WoW.RunicPower <= 30 && isCheckHotkeysFrostOffensiveErW && combatRoutine.UseCooldowns && !WoW.PlayerHasBuff("HEmpower Rune") && !WoW.IsSpellOnCooldown("HEmpower Rune") && WoW.PlayerHasBuff("Breath") && legyringtest == true )
					{
						useNextHRWCharge = false;
						WoW.CastSpell("HEmpower Rune");
						legyringtest = false;
					}
					if ((haveCoF || useNextHRWCharge) && !isCheckHotkeyslegyring && haveHRW && WoW.RunicPower <= 30 && isCheckHotkeysFrostOffensiveErW && combatRoutine.UseCooldowns && !WoW.PlayerHasBuff("HEmpower Rune") && !WoW.IsSpellOnCooldown("HEmpower Rune") && WoW.PlayerHasBuff("Breath"))
					{
						useNextHRWCharge = false;
						WoW.CastSpell("HEmpower Rune");
					}
					if (isCheckHotkeyslegyring && !WoW.PlayerHasBuff("Breath"))
					{
						legyringtest = true;
					}
					if (WoW.CanCast("Arcane Torrent") && WoW.PlayerRace == "BloodElf" && WoW.RunicPower <= 25 && combatRoutine.UseCooldowns && isCheckHotkeysRacials && WoW.PlayerHasBuff("Breath") && !WoW.PlayerHasBuff("HEmpower Rune") && WoW.CanCast("Obliterate", false, false, true, false, false))
                    {
                        WoW.CastSpell("Arcane Torrent");
                        return;
                    }
					if (WoW.CanCast("Blood Fury") && combatRoutine.UseCooldowns && isCheckHotkeysRacials && !WoW.IsSpellOnCooldown ("Blood Fury") && WoW.PlayerRace == "Orc" && WoW.PlayerHasBuff("Breath") && WoW.CanCast("Obliterate", false, false, true, false, false))
                    {
                        WoW.CastSpell("Blood Fury");
                        return;
                    }
					if (WoW.CanCast("Berserking") && combatRoutine.UseCooldowns && isCheckHotkeysRacials && !WoW.IsSpellOnCooldown ("Berserking") && WoW.PlayerRace == "Troll" && WoW.PlayerHasBuff("Breath") && WoW.CanCast("Obliterate", false, false, true, false, false))
                    {
                        WoW.CastSpell("Berserking");
                        return;
                    }
					if ((haveCoF || useNextHRWCharge) && !haveHRW && WoW.RunicPower <= 50 && WoW.CurrentRunes <=1 && isCheckHotkeysFrostOffensiveErW && combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("HEmpower Rune") && WoW.PlayerHasBuff("Breath"))
					{
						WoW.CastSpell("Empower Rune");
					}
					if (combatRoutine.UseCooldowns && WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 2 && WoW.RunicPower >= 70 && !WoW.IsSpellOnCooldown("Breath"))
					{
						WoW.CastSpell("Breath");
						useNextHRWCharge = true;
						return;
					}
					if (!WoW.TargetHasDebuff("Frost Fever") && WoW.CurrentRunes >= 1 && !WoW.PlayerHasBuff("Breath") && WoW.CanCast("Howling Blast", false, false, true, false, false) && !WoW.IsSpellOnCooldown("Howling Blast"))
					{
						WoW.CastSpell("Howling Blast");
						return;
					}
					if (WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 1 && ((WoW.RunicPower >= 48 && WoW.PlayerHasBuff("Breath")) || !WoW.PlayerHasBuff("Breath")) && (!combatRoutine.UseCooldowns || (combatRoutine.UseCooldowns && WoW.SpellCooldownTimeRemaining("Breath") >= 1500)) && !WoW.IsSpellOnCooldown("Remorseless Winter"))
					{
						WoW.CastSpell("Remorseless Winter");
						return;
					}
					if (((WoW.RunicPower >= 46 && WoW.PlayerHasBuff("Breath")) || !WoW.PlayerHasBuff("Breath")) && WoW.CanCast("Howling Blast", false, false, true, false, false) && WoW.PlayerHasBuff("Rime"))
					{
						WoW.CastSpell("Howling Blast");
						return;
					}
					if (WoW.Talent(6) != 1 && WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 2 && !WoW.PlayerHasBuff("Breath") && WoW.PlayerHasBuff("Gathering Storm"))
					{
						WoW.CastSpell("Obliterate");
						return;
					}
					if (WoW.RunicPower >= 70 && !WoW.PlayerHasBuff("Breath") && WoW.CanCast("Frost Strike", false, false, true, false, false))
					{
						WoW.CastSpell("Frost Strike");
						return;
					}
					if (WoW.Talent(6) == 1 && WoW.CanCast("Frost Strike", false, false, true, false, false) && WoW.CurrentRunes >= 1 && WoW.PlayerHasBuff("Killing Machine") && !WoW.PlayerHasBuff("Breath"))
					{
						WoW.CastSpell("Frostscythe");
						return;
					}
					if (WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 2 && (!WoW.PlayerHasBuff("Breath") || (WoW.PlayerHasBuff("Breath") && (WoW.RunicPower <= 70 || WoW.PlayerHasBuff("Breath") && WoW.CurrentRunes > 3))))
					{
						WoW.CastSpell("Obliterate");
						return;
					}
					if (WoW.RunicPower >= 25 && WoW.CanCast("Frost Strike", false, false, true, false, false) && !WoW.PlayerHasBuff("Breath") && (!combatRoutine.UseCooldowns || (combatRoutine.UseCooldowns && WoW.SpellCooldownTimeRemaining("Breath") >= 1500)))
					{
						WoW.CastSpell("Frost Strike");
						return;
					}
					if (WoW.Talent(2) == 3 && WoW.CurrentRunes <= 4 && WoW.RunicPower <= 70 && !WoW.IsSpellOnCooldown("Horn") && !WoW.PlayerHasBuff("HEmpower Rune") && (WoW.PlayerHasBuff("Breath") || (!WoW.PlayerHasBuff("Breath") && WoW.SpellCooldownTimeRemaining("Breath") >= 1500)))
					{
						WoW.CastSpell("Horn");
					}
				}
				if (WoW.Talent(7) != 2 && WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted)
				{
					if (isCheckHotkeysFrostOffensivePillarofFrost && WoW.CanCast("Obliterate", false, false, true, false, false) && combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("PillarofFrost"))
					{
						WoW.CastSpell("PillarofFrost");
					}
					if (combatRoutine.UseCooldowns && isCheckHotkeysFrostOffensiveErW && WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes == 0 && WoW.PlayerHasBuff("PillarofFrost") && !WoW.IsSpellOnCooldown("Empower Rune"))
					{
						WoW.CastSpell("Empower Rune");
					}
					if (WoW.CanCast("Arcane Torrent") && combatRoutine.UseCooldowns && isCheckHotkeysRacials && WoW.PlayerRace == "BloodElf" && WoW.RunicPower <= 30 && WoW.PlayerHasBuff("PillarofFrost") && WoW.CanCast("Obliterate", false, false, true, false, false))
                    {
                        WoW.CastSpell("Arcane Torrent");
                        return;
                    }
					if (WoW.CanCast("Blood Fury") && combatRoutine.UseCooldowns && isCheckHotkeysRacials && !WoW.IsSpellOnCooldown ("Blood Fury") && WoW.PlayerRace == "Orc" && WoW.PlayerHasBuff("PillarofFrost") && WoW.CanCast("Obliterate", false, false, true, false, false))
                    {
                        WoW.CastSpell("Blood Fury");
                        return;
                    }
					if (WoW.CanCast("Berserking") && combatRoutine.UseCooldowns && isCheckHotkeysRacials && !WoW.IsSpellOnCooldown ("Berserking") && WoW.PlayerRace == "Troll" && WoW.PlayerHasBuff("PillarofFrost") && WoW.CanCast("Obliterate", false, false, true, false, false))
                    {
                        WoW.CastSpell("Berserking");
                        return;
                    }
					if (WoW.CanCast("Frost Strike", false, false, true, false, false) && (!WoW.PlayerHasBuff("Icy Talons") || WoW.PlayerBuffTimeRemaining("Icy Talons") <= 200) && WoW.RunicPower >= 25 && !(combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("Obliteration") && WoW.Talent(7) == 1) && (WoW.Talent(7) != 1 || (WoW.Talent(7) == 1 && !WoW.PlayerHasBuff("Obliteration"))))
					{
						Log.Write("Hasbuff " + WoW.PlayerHasBuff("Icy Talons") + " Remaining " + WoW.PlayerBuffTimeRemaining("Icy Talons"));
						WoW.CastSpell("Frost Strike");
						return;
					}
					if (WoW.CanCast("Howling Blast", false, false, true, false, false) && !WoW.IsSpellOnCooldown("Howling Blast") && !WoW.TargetHasDebuff("Frost Fever") && WoW.CurrentRunes >= 1 && (WoW.Talent(7) != 1 || (WoW.Talent(7) == 1 && !WoW.PlayerHasBuff("Obliteration"))))
					{
						WoW.CastSpell("Howling Blast");
						return;
					}
					if (WoW.Talent(6) == 1 && WoW.RunicPower >= 80 && WoW.CanCast("Frost Strike", false, false, true, false, false))
					{
						WoW.CastSpell("Frost Strike");
						return;
					}
					if (WoW.CanCast("Howling Blast", false, false, true, false, false) && WoW.PlayerHasBuff("Rime") && (WoW.Talent(7) != 1 || (WoW.Talent(7) == 1 && !WoW.PlayerHasBuff("Obliteration"))))
					{
						WoW.CastSpell("Howling Blast");
						return;
					}
					if (combatRoutine.UseCooldowns && WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 2 && WoW.RunicPower >= 25 && WoW.Talent(7) == 1 && !WoW.IsSpellOnCooldown("Obliteration"))
					{
						WoW.CastSpell("Obliteration");
						return;
					}
					if (WoW.Talent(7) == 1 && WoW.RunicPower >= 25 && WoW.CanCast("Frost Strike", false, false, true, false, false) && WoW.PlayerHasBuff("Obliteration") && !WoW.PlayerHasBuff("Killing Machine"))
					{
						WoW.CastSpell("Frost Strike");
						return;
					}
					if (WoW.Talent(7) == 1 && WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 1 && WoW.PlayerHasBuff("Killing Machine") && WoW.PlayerHasBuff("Obliteration"))
					{
						WoW.CastSpell("Obliterate");
						return;
					}
					if (WoW.Talent(6) == 1 && WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 1 && WoW.PlayerHasBuff("Killing Machine"))
					{
						WoW.CastSpell("Frostscythe");
						return;
					}
					if (WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 2)
					{
						WoW.CastSpell("Obliterate");
						return;
					}
					if (WoW.Talent(7) == 3 && WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 1 && !WoW.IsSpellOnCooldown("Glacial Advance"))
					{
						WoW.CastSpell("Glacial Advance");
						return;
					}
					if (WoW.Talent(7) == 1 && WoW.RunicPower >= 40 && WoW.CanCast("Frost Strike", false, false, true, false, false) && !(combatRoutine.UseCooldowns && !WoW.IsSpellOnCooldown("Obliteration")) && WoW.Talent(7) == 1 && !WoW.PlayerHasBuff("Obliteration"))
					{
						WoW.CastSpell("Frost Strike");
						return;
					}
					if (WoW.CanCast("Obliterate", false, false, true, false, false) && WoW.CurrentRunes >= 1 && !WoW.IsSpellOnCooldown("Remorseless Winter") && (WoW.Talent(7) != 1 || (WoW.Talent(7) == 1 && !WoW.PlayerHasBuff("Obliteration"))))
					{
						WoW.CastSpell("Remorseless Winter");
						return;
					}
				}
            }
        }
    }

    public class SettingsFormDFF : Form
    {
        public Button btnHotkeysFrostOffensiveCooldowns;
        public TextBox checkHotkeysFrostAMSPercent;
        private readonly Label checkHotkeysFrostAMSPercentLabel;
        public CheckBox checkHotkeysFrostAntiMagicShield;
        public CheckBox checkHotkeysFrostIceboundFortitude;
        public TextBox checkHotkeysFrostIFPercent;
        public Label checkHotkeysFrostIFPercentLabel;
        public CheckBox checkHotkeysFrostDeathstrike;
        public TextBox checkHotkeysFrostDeathstrikePercent;
        public Label checkHotkeysFrostDeathstrikePercentLabel;
        public CheckBox checkHotkeysFrostFreeDeathstrike;
        public TextBox checkHotkeysFrostFreeDeathstrikePercent;
        public Label checkHotkeysFrostFreeDeathstrikePercentLabel;
        public CheckBox checkHotkeysFrostKick;
        public TextBox checkHotkeysFrostKickPercent;
        public Label checkHotkeysFrostKickPercentLabel;
        public CheckBox checkHotkeyslegyring;
        public CheckBox checkHotkeysRacials;
        public CheckBox checkHotkeysFrostOffensiveErW;
        public CheckBox checkHotkeysFrostOffensivePillarofFrost;


        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private IContainer components = null;

        private readonly GroupBox groupBox12;
        private readonly GroupBox groupBox13;
        private readonly GroupBox groupBox22;
        private readonly Label spellIdLabel;
        private readonly TabControl tabControl3;
        private readonly TabPage tabPage5;


        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        public SettingsFormDFF()
        {
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysFrostIceboundFortitude = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostIFPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostIFPercentLabel = new System.Windows.Forms.Label();	
            this.checkHotkeysFrostDeathstrike = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostDeathstrikePercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostDeathstrikePercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysFrostFreeDeathstrike = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostFreeDeathstrikePercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostFreeDeathstrikePercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysFrostKick = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostKickPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostKickPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysFrostAntiMagicShield = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostAMSPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysFrostAMSPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeyslegyring = new System.Windows.Forms.CheckBox();
            this.checkHotkeysRacials = new System.Windows.Forms.CheckBox();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysFrostOffensiveErW = new System.Windows.Forms.CheckBox();
            this.checkHotkeysFrostOffensivePillarofFrost = new System.Windows.Forms.CheckBox();
            this.btnHotkeysFrostOffensiveCooldowns = new System.Windows.Forms.Button();
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
            this.groupBox12.Controls.Add(this.checkHotkeysFrostIceboundFortitude);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostIFPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostIFPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostDeathstrike);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostDeathstrikePercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostDeathstrikePercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostFreeDeathstrike);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostFreeDeathstrikePercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostFreeDeathstrikePercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostKick);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostKickPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostKickPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostAntiMagicShield);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostAMSPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysFrostAMSPercentLabel);

            this.groupBox12.Location = new System.Drawing.Point(8, 100);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(561, 140);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Defensive Cooldowns";
            // 
            // checkHotkeysFrostIceboundFortitude
            // 
            this.checkHotkeysFrostIceboundFortitude.AutoSize = true;
            this.checkHotkeysFrostIceboundFortitude.Location = new System.Drawing.Point(151, 28);
            this.checkHotkeysFrostIceboundFortitude.Name = "checkHotkeysFrostIceboundFortitude";
            this.checkHotkeysFrostIceboundFortitude.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysFrostIceboundFortitude.TabIndex = 9;
            this.checkHotkeysFrostIceboundFortitude.Text = "Icebound Fortitude";
            this.checkHotkeysFrostIceboundFortitude.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysFrostIFPercent
            // 
            this.checkHotkeysFrostIFPercent.AutoSize = true;
            this.checkHotkeysFrostIFPercent.Location = new System.Drawing.Point(300, 28);
            this.checkHotkeysFrostIFPercent.Name = "checkHotkeysFrostIFPercent";
            this.checkHotkeysFrostIFPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostIFPercent.TabIndex = 9;
            this.checkHotkeysFrostIFPercent.Text = "50";
            // 
            // checkHotkeysFrostIFPercentLabel
            // 
            this.checkHotkeysFrostIFPercentLabel.AutoSize = true;
            this.checkHotkeysFrostIFPercentLabel.Location = new System.Drawing.Point(321, 30);
            this.checkHotkeysFrostIFPercentLabel.Name = "checkHotkeysFrostIFPercentLabel";
            this.checkHotkeysFrostIFPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostIFPercentLabel.TabIndex = 9;
            this.checkHotkeysFrostIFPercentLabel.Text = "% HP";

            // 
            // checkHotkeysFrostAntiMagicShield
            // 
            this.checkHotkeysFrostAntiMagicShield.AutoSize = true;
            this.checkHotkeysFrostAntiMagicShield.Location = new System.Drawing.Point(151, 50);
            this.checkHotkeysFrostAntiMagicShield.Name = "checkHotkeysFrostAntiMagicShield";
            this.checkHotkeysFrostAntiMagicShield.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysFrostAntiMagicShield.TabIndex = 8;
            this.checkHotkeysFrostAntiMagicShield.Text = "Anti-Magic Shield";
            this.checkHotkeysFrostAntiMagicShield.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysFrostAMSPercent
            // 
            this.checkHotkeysFrostAMSPercent.AutoSize = true;
            this.checkHotkeysFrostAMSPercent.Location = new System.Drawing.Point(300, 50);
            this.checkHotkeysFrostAMSPercent.Name = "checkHotkeysFrostIFPercent";
            this.checkHotkeysFrostAMSPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostAMSPercent.TabIndex = 9;
            this.checkHotkeysFrostAMSPercent.Text = "50";
            // 
            // checkHotkeysFrostAMSPercentLabel
            // 
            this.checkHotkeysFrostAMSPercentLabel.AutoSize = true;
            this.checkHotkeysFrostAMSPercentLabel.Location = new System.Drawing.Point(321, 52);
            this.checkHotkeysFrostAMSPercentLabel.Name = "checkHotkeysFrostAMSPercentLabel";
            this.checkHotkeysFrostAMSPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysFrostAMSPercentLabel.TabIndex = 9;
            this.checkHotkeysFrostAMSPercentLabel.Text = "% HP";
			
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
            this.groupBox13.Controls.Add(this.checkHotkeysFrostOffensiveErW);
            this.groupBox13.Controls.Add(this.checkHotkeysFrostOffensivePillarofFrost);
            this.groupBox13.Controls.Add(this.btnHotkeysFrostOffensiveCooldowns);
            this.groupBox13.Controls.Add(this.checkHotkeyslegyring);
            this.groupBox13.Controls.Add(this.checkHotkeysRacials);
            this.groupBox13.Location = new System.Drawing.Point(8, 8);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(561, 90);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Offensive Cooldowns";

            // 
            // checkHotkeysFrostOffensiveErW
            // 
            this.checkHotkeysFrostOffensiveErW.AutoSize = true;
            this.checkHotkeysFrostOffensiveErW.Location = new System.Drawing.Point(151, 60);
            this.checkHotkeysFrostOffensiveErW.Name = "checkHotkeysFrostOffensiveErW";
            this.checkHotkeysFrostOffensiveErW.Size = new System.Drawing.Size(48, 17);
            this.checkHotkeysFrostOffensiveErW.TabIndex = 3;
            this.checkHotkeysFrostOffensiveErW.Text = "ErW / HrW";
            this.checkHotkeysFrostOffensiveErW.UseVisualStyleBackColor = true;

            // 
            // checkHotkeysFrostOffensivePillarofFrost
            // 
            this.checkHotkeysFrostOffensivePillarofFrost.AutoSize = true;
            this.checkHotkeysFrostOffensivePillarofFrost.Location = new System.Drawing.Point(151, 32);
            this.checkHotkeysFrostOffensivePillarofFrost.Name = "checkHotkeysFrostOffensivePillarofFrost";
            this.checkHotkeysFrostOffensivePillarofFrost.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysFrostOffensivePillarofFrost.TabIndex = 2;
            this.checkHotkeysFrostOffensivePillarofFrost.Text = "Pillar of Frost";
            this.checkHotkeysFrostOffensivePillarofFrost.UseVisualStyleBackColor = true;

            // 
            // checkHotkeyslegyring
            // 
            this.checkHotkeyslegyring.AutoSize = true;
            this.checkHotkeyslegyring.Location = new System.Drawing.Point(300, 32);
            this.checkHotkeyslegyring.Name = "checkHotkeyslegyring";
            this.checkHotkeyslegyring.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeyslegyring.TabIndex = 4;
            this.checkHotkeyslegyring.Text = "Legendary Ring";
            this.checkHotkeyslegyring.UseVisualStyleBackColor = true;
			
			// 
            // checkHotkeysRacials
            // 
            this.checkHotkeysRacials.AutoSize = true;
            this.checkHotkeysRacials.Location = new System.Drawing.Point(300, 60);
            this.checkHotkeysRacials.Name = "checkHotkeysRacials";
            this.checkHotkeysRacials.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysRacials.TabIndex = 5;
            this.checkHotkeysRacials.Text = "Use Racials";
            this.checkHotkeysRacials.UseVisualStyleBackColor = true;
			
            // 
            // btnHotkeysFrostOffensiveCooldowns
            // 
            this.btnHotkeysFrostOffensiveCooldowns.Location = new System.Drawing.Point(18, 28);
            this.btnHotkeysFrostOffensiveCooldowns.Name = "btnHotkeysFrostOffensiveCooldowns";
            this.btnHotkeysFrostOffensiveCooldowns.Size = new System.Drawing.Size(113, 23);
            this.btnHotkeysFrostOffensiveCooldowns.TabIndex = 1;
            this.btnHotkeysFrostOffensiveCooldowns.Text = "Click to Set";
            this.btnHotkeysFrostOffensiveCooldowns.UseVisualStyleBackColor = true;
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
Spell,49143,Frost Strike,D4
Spell,207230,Frostscythe,NumPad8
Spell,49184,Howling Blast,D1
Spell,49020,Obliterate,D3
Spell,196770,Remorseless Winter,D7
Spell,194913,Glacial Advance,NumPad7
Spell,49998,Death Strike,NumPad1
Spell,48707,Anti-Magic Shell,NumPad2
Spell,48792,Icebound Fortitude,NumPad3
Spell,51271,PillarofFrost,D6
Spell,47568,Empower Rune,D9
Spell,207127,HEmpower Rune,D9
Spell,207256,Obliteration,OemOpenBrackets
Spell,152279,Breath,D5
Spell,57330,Horn,D0
Spell,190778,Sindragosa's Fury,D8
Spell,26297,Berserking,F9
Spell,80483,Arcane Torrent,F9
Spell,20572,Blood Fury,F9
Spell,47528,Mind Freeze,NumPad4
Aura,51124,Killing Machine
Aura,194879,Icy Talons
Aura,55095,Frost Fever
Aura,59057,Rime
Aura,101568,Free DeathStrike
Aura,51271,PillarofFrost
Aura,45524,ChainofIce
Aura,207256,Obliteration
Aura,152279,Breath
Aura,207127,HEmpower Rune
Aura,211805,Gathering Storm
*/
