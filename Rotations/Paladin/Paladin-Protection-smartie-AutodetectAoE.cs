//Changelog
// v1.0 First release


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
    public class ProtPalasmartie : CombatRoutine
    {
        public override string Name
        {
            get { return "Protection Paladin by smartie"; }
        }

        public override string Class
        {
            get { return "Paladin"; }
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
				return 99; 
			} 
		}
        public override Form SettingsForm { get; set; }
        public SettingsFormDFF SettingsFormDFF { get; set; }

        public static int cooldownKey
        {
            get
            {
                var cooldownKey = ConfigFile.ReadValue("ProtPala", "cooldownKey").Trim();
                if (cooldownKey != "")
                {
                    return Convert.ToInt32(cooldownKey);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "cooldownKey", value.ToString()); }
        }

        public static int cooldownModifier
        {
            get
            {
                var cooldownModifier = ConfigFile.ReadValue("ProtPala", "cooldownModifier").Trim();
                if (cooldownModifier != "")
                {
                    return Convert.ToInt32(cooldownModifier);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "cooldownModifier", value.ToString()); }
        }

        public static string cooldownHotKeyString
        {
            get
            {
                var cooldownHotKeyString = ConfigFile.ReadValue("ProtPala", "cooldownHotKeyString").Trim();

                if (cooldownHotKeyString != "")
                {
                    return cooldownHotKeyString;
                }

                return "Click to Set";
            }
            set { ConfigFile.WriteValue("ProtPala", "cooldownHotKeyString", value); }
        }

        public static bool ischeckHotkeysProtRacials
        {
            get
            {
                var ischeckHotkeysProtRacials = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtRacials").Trim();

                if (ischeckHotkeysProtRacials != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtRacials);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtRacials", value.ToString()); }
        }
		
        public static bool ischeckHotkeysProtKick
        {
            get
            {
                var ischeckHotkeysProtKick = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtKick").Trim();

                if (ischeckHotkeysProtKick != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtKick);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtKick", value.ToString()); }
        }
		
        public static int ProtKickPercent
        {
            get
            {
                var ProtKickPercent = ConfigFile.ReadValue("ProtPala", "ProtKickPercent").Trim();
                if (ProtKickPercent != "")
                {
                    return Convert.ToInt32(ProtKickPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "ProtKickPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysProtLotP
        {
            get
            {
                var ischeckHotkeysProtLotP = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtLotP").Trim();

                if (ischeckHotkeysProtLotP != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtLotP);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtLotP", value.ToString()); }
        }

        public static int ProtLotPPercent
        {
            get
            {
                var ProtLotPPercent = ConfigFile.ReadValue("ProtPala", "ProtLotPPercent").Trim();
                if (ProtLotPPercent != "")
                {
                    return Convert.ToInt32(ProtLotPPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "ProtLotPPercent", value.ToString()); }
        }

        public static bool ischeckHotkeysProtLoH
        {
            get
            {
                var ischeckHotkeysProtLoH = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtLoH").Trim();

                if (ischeckHotkeysProtLoH != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtLoH);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtLoH", value.ToString()); }
        }
		
        public static int ProtLoHPercent
        {
            get
            {
                var ProtLoHPercent = ConfigFile.ReadValue("ProtPala", "ProtLoHPercent").Trim();
                if (ProtLoHPercent != "")
                {
                    return Convert.ToInt32(ProtLoHPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "ProtLoHPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysProtAD
        {
            get
            {
                var ischeckHotkeysProtAD = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtAD").Trim();

                if (ischeckHotkeysProtAD != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtAD);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtAD", value.ToString()); }
        }

        public static int ProtADPercent
        {
            get
            {
                var ProtADPercent = ConfigFile.ReadValue("ProtPala", "ProtADPercent").Trim();
                if (ProtADPercent != "")
                {
                    return Convert.ToInt32(ProtADPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "ProtADPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysProtGotAK
        {
            get
            {
                var ischeckHotkeysProtGotAK = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtGotAK").Trim();

                if (ischeckHotkeysProtGotAK != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtGotAK);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtGotAK", value.ToString()); }
        }

        public static int ProtGotAKPercent
        {
            get
            {
                var ProtGotAKPercent = ConfigFile.ReadValue("ProtPala", "ProtGotAKPercent").Trim();
                if (ProtGotAKPercent != "")
                {
                    return Convert.ToInt32(ProtGotAKPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "ProtGotAKPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysProtEoT
        {
            get
            {
                var ischeckHotkeysProtEoT = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtEoT").Trim();

                if (ischeckHotkeysProtEoT != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtEoT);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtEoT", value.ToString()); }
        }

        public static int ProtEoTPercent
        {
            get
            {
                var ProtEoTPercent = ConfigFile.ReadValue("ProtPala", "ProtEoTPercent").Trim();
                if (ProtEoTPercent != "")
                {
                    return Convert.ToInt32(ProtEoTPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("ProtPala", "ProtEoTPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysProtAW
        {
            get
            {
                var ischeckHotkeysProtAW = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtAW").Trim();

                if (ischeckHotkeysProtAW != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtAW);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtAW", value.ToString()); }
        }

        public static bool ischeckHotkeysProtBOL
        {
            get
            {
                var ischeckHotkeysProtBOL = ConfigFile.ReadValue("ProtPala", "ischeckHotkeysProtBOL").Trim();

                if (ischeckHotkeysProtBOL != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysProtBOL);
                }

                return true;
            }
            set { ConfigFile.WriteValue("ProtPala", "ischeckHotkeysProtBOL", value.ToString()); }
        }

        public override void Initialize()
        {
            Log.Write("Welcome to the Protection Paladin v1.0 by smartie", Color.Green);
	        Log.Write("All Talents supported and auto detected", Color.Green);		
            SettingsFormDFF = new SettingsFormDFF();
            SettingsForm = SettingsFormDFF;

            SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.Text = cooldownHotKeyString;
            SettingsFormDFF.checkHotkeysProtAW.Checked = ischeckHotkeysProtAW;
            SettingsFormDFF.checkHotkeysProtBOL.Checked = ischeckHotkeysProtBOL;
            SettingsFormDFF.checkHotkeysProtLotP.Checked = ischeckHotkeysProtLotP;
            SettingsFormDFF.checkHotkeysProtLotPPercent.Text = ProtLotPPercent.ToString();
            SettingsFormDFF.checkHotkeysProtLoH.Checked = ischeckHotkeysProtLoH;
            SettingsFormDFF.checkHotkeysProtLoHPercent.Text = ProtLoHPercent.ToString();
            SettingsFormDFF.checkHotkeysProtAD.Checked = ischeckHotkeysProtAD;
            SettingsFormDFF.checkHotkeysProtADPercent.Text = ProtADPercent.ToString();
            SettingsFormDFF.checkHotkeysProtGotAK.Checked = ischeckHotkeysProtGotAK;
            SettingsFormDFF.checkHotkeysProtGotAKPercent.Text = ProtGotAKPercent.ToString();
            SettingsFormDFF.checkHotkeysProtEoT.Checked = ischeckHotkeysProtEoT;
            SettingsFormDFF.checkHotkeysProtEoTPercent.Text = ProtEoTPercent.ToString();
            SettingsFormDFF.checkHotkeysProtRacials.Checked = ischeckHotkeysProtRacials;
            SettingsFormDFF.checkHotkeysProtKick.Checked = ischeckHotkeysProtKick;
            SettingsFormDFF.checkHotkeysProtKickPercent.Text = ProtKickPercent.ToString();
			
            SettingsFormDFF.checkHotkeysProtRacials.CheckedChanged += ischeckHotkeysProtRacials_Click;
            SettingsFormDFF.checkHotkeysProtKick.CheckedChanged += ischeckHotkeysProtKick_Click;
            SettingsFormDFF.checkHotkeysProtKickPercent.TextChanged += ischeckHotkeysProtKickPercent_Click;
            SettingsFormDFF.checkHotkeysProtLotP.CheckedChanged += ischeckHotkeysProtLotP_Click;
            SettingsFormDFF.checkHotkeysProtLotPPercent.TextChanged += ischeckHotkeysProtLotPPercent_Click;
            SettingsFormDFF.checkHotkeysProtLoH.CheckedChanged += ischeckHotkeysProtLoH_Click;
            SettingsFormDFF.checkHotkeysProtLoHPercent.TextChanged += ischeckHotkeysProtLoHPercent_Click;
            SettingsFormDFF.checkHotkeysProtAD.CheckedChanged += ischeckHotkeysProtAD_Click;
            SettingsFormDFF.checkHotkeysProtADPercent.TextChanged += ischeckHotkeysProtADPercent_Click;
            SettingsFormDFF.checkHotkeysProtGotAK.CheckedChanged += ischeckHotkeysProtGotAK_Click;
            SettingsFormDFF.checkHotkeysProtGotAKPercent.TextChanged += ischeckHotkeysProtGotAKPercent_Click;
            SettingsFormDFF.checkHotkeysProtEoT.CheckedChanged += ischeckHotkeysProtEoT_Click;
            SettingsFormDFF.checkHotkeysProtEoTPercent.TextChanged += ischeckHotkeysProtEoTPercent_Click;
            SettingsFormDFF.checkHotkeysProtAW.CheckedChanged += ischeckHotkeysProtAW_Click;
            SettingsFormDFF.checkHotkeysProtBOL.CheckedChanged += ischeckHotkeysProtBOL_Click;
            SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.KeyDown += KeyDown;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
                return;
            SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.Text = "Hotkey : ";
            if (e.Shift)
            {
                cooldownModifier = (int)Keys.ShiftKey;
                SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.Text += Keys.Shift + " + ";
            }
            else if (e.Alt)
            {
                cooldownModifier = (int)Keys.Menu;
                SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.Text += Keys.Alt + " + ";
            }
            else if (e.Control)
            {
                cooldownModifier = (int)Keys.ControlKey;
                SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.Text += Keys.Control + " + ";
            }
            else cooldownModifier = -1;
            cooldownKey = (int)e.KeyCode;
            SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.Text += e.KeyCode;
            cooldownHotKeyString = SettingsFormDFF.btnHotkeysProtOffensiveCooldowns.Text;
            SettingsFormDFF.checkHotkeysProtLotPPercentLabel.Focus();
        }
		
        private void ischeckHotkeysProtLotP_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtLotP = SettingsFormDFF.checkHotkeysProtLotP.Checked;
        }

        private void ischeckHotkeysProtLotPPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysProtLotPPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                ProtLotPPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysProtLotPPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysProtLoH_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtLoH = SettingsFormDFF.checkHotkeysProtLoH.Checked;
        }
		
        private void ischeckHotkeysProtLoHPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysProtLoHPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                ProtLoHPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysProtLoHPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysProtAD_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtAD = SettingsFormDFF.checkHotkeysProtAD.Checked;
        }

        private void ischeckHotkeysProtADPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysProtADPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                ProtADPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysProtADPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysProtGotAK_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtGotAK = SettingsFormDFF.checkHotkeysProtGotAK.Checked;
        }

        private void ischeckHotkeysProtGotAKPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysProtGotAKPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                ProtGotAKPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysProtGotAKPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysProtEoT_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtEoT = SettingsFormDFF.checkHotkeysProtEoT.Checked;
        }

        private void ischeckHotkeysProtEoTPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysProtEoTPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                ProtEoTPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysProtEoTPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysProtRacials_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtRacials = SettingsFormDFF.checkHotkeysProtRacials.Checked;
        }
		
        private void ischeckHotkeysProtKick_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtKick = SettingsFormDFF.checkHotkeysProtKick.Checked;
        }
		
        private void ischeckHotkeysProtKickPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysProtKickPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                ProtKickPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysProtKickPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void ischeckHotkeysProtAW_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtAW = SettingsFormDFF.checkHotkeysProtAW.Checked;
        }

        private void ischeckHotkeysProtBOL_Click(object sender, EventArgs e)
        {
            ischeckHotkeysProtBOL = SettingsFormDFF.checkHotkeysProtBOL.Checked;
        }

        public override void Stop()
        {
        }

        public override void Pulse()
        {
            if (DetectKeyPress.GetKeyState(cooldownKey) < 0 && (cooldownModifier == -1 || cooldownModifier != -1 && DetectKeyPress.GetKeyState(cooldownModifier) < 0))
			{
				UseCooldowns = !UseCooldowns;
				Thread.Sleep(150);
			}
            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && (!WoW.IsMounted || WoW.PlayerHasBuff("Divine Steed")))
            {
				//Offensive Cooldowns
				if (WoW.CanCast("Avenging Wrath") && ischeckHotkeysProtAW && UseCooldowns && WoW.PlayerHasBuff("Seraphim") && WoW.Talent (7) == 2 && WoW.IsSpellInRange("Rebuke"))
				{
					WoW.CastSpell("Avenging Wrath");
					return;
				}
				if (WoW.CanCast("Avenging Wrath") && ischeckHotkeysProtAW && UseCooldowns && WoW.Talent (7) != 2 && WoW.IsSpellInRange("Rebuke"))
				{
					WoW.CastSpell("Avenging Wrath");
					return;
				}
                if (WoW.CanCast("Rebuke") && ischeckHotkeysProtKick && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= ProtKickPercent && !WoW.IsSpellOnCooldown("Rebuke") && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Rebuke"))
                {
                    WoW.CastSpell("Rebuke");						
                    return;
                }
				if (WoW.CanCast("Bastion of Light") && ischeckHotkeysProtBOL && UseCooldowns && WoW.Talent (2) == 2 && WoW.PlayerSpellCharges("Shield of the Righteous") == 0 && WoW.IsSpellInRange("Rebuke"))
				{
					WoW.CastSpell("Bastion of Light");
					return;
				}
				// Defensive Cooldowns
                if (WoW.CanCast("Eye of Tyr") && ischeckHotkeysProtEoT && !WoW.IsSpellOnCooldown("Eye of Tyr") && WoW.HealthPercent <= ProtEoTPercent && WoW.IsSpellInRange("Rebuke"))
                {
                    WoW.CastSpell("Eye of Tyr");
                    return;
                }
                if (WoW.CanCast("Ardent Defender") && ischeckHotkeysProtAD && !WoW.IsSpellOnCooldown("Ardent Defender") && WoW.HealthPercent <= ProtADPercent)
                {
                   	WoW.CastSpell("Ardent Defender");
                    return;
                }
                if (WoW.CanCast("Guardian of the Ancient Kings") && ischeckHotkeysProtGotAK && !WoW.IsSpellOnCooldown("Guardian of the Ancient Kings") && WoW.HealthPercent <= ProtGotAKPercent)
                {
                   	WoW.CastSpell("Guardian of the Ancient Kings");
                   	return;
                }
                if (WoW.CanCast("Lay on Hands") && ischeckHotkeysProtLoH && !WoW.IsSpellOnCooldown("Lay on Hands") && WoW.HealthPercent <= ProtLoHPercent)
                {
                   	WoW.CastSpell("Lay on Hands");
                    return;
                }
                if (WoW.CanCast("Hand of the Protector") && ischeckHotkeysProtLotP && !WoW.IsSpellOnCooldown("Hand of the Protector") && WoW.HealthPercent <= ProtLotPPercent && WoW.Talent(5) == 1)
                {
                    WoW.CastSpell("Hand of the Protector");
                    return;
                }
                if (WoW.CanCast("Light of the Protector") && ischeckHotkeysProtLotP && !WoW.IsSpellOnCooldown("Light of the Protector") && WoW.HealthPercent <= ProtLotPPercent && WoW.Talent(5) != 1)
                {
                    WoW.CastSpell("Light of the Protector");
                    return;
                }
				//Rotation
				if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave || combatRoutine.Type == RotationType.AOE)
				{
					if (!WoW.IsSpellOnCooldown("Seraphim") && WoW.PlayerSpellCharges("Shield of the Righteous") >= 2 && WoW.HealthPercent >= 80 && WoW.IsSpellInRange("Rebuke") && WoW.Talent (7) == 2)
					{
						WoW.CastSpell("Seraphim");
						return;
					}
					if (WoW.PlayerSpellCharges("Shield of the Righteous") == 3 && WoW.IsSpellInRange("Rebuke"))
					{
						WoW.CastSpell("Shield of the Righteous");
						return;
					}
					if (WoW.PlayerHasBuff("Shield of the Righteous") && WoW.PlayerBuffTimeRemaining("Shield of the Righteous") <= 1.5 && WoW.IsSpellInRange("Rebuke"))
					{
						WoW.CastSpell("Shield of the Righteous");
						return;
					}
					if (WoW.PlayerSpellCharges("Shield of the Righteous") >= 1 && WoW.HealthPercent <= 80 && (!WoW.PlayerHasBuff("Shield of the Righteous") || WoW.PlayerBuffTimeRemaining("Shield of the Righteous") <= 1.5) && WoW.IsSpellInRange("Rebuke"))
					{
						WoW.CastSpell("Shield of the Righteous");
						return;
					}
                    if (WoW.Talent(1) == 2 && WoW.CanCast("Blessed Hammer") && WoW.PlayerSpellCharges("Blessed Hammer") >= 3 && !WoW.IsSpellOnGCD("Blessed Hammer") && WoW.IsSpellInRange("Rebuke"))
                    {
                       	WoW.CastSpell("Blessed Hammer");
                        return;
                    }
                    if (WoW.Talent(1) != 2 && WoW.CanCast("Hammer of the Righteous") && WoW.PlayerSpellCharges("Hammer of the Righteous") >= 1 && !WoW.IsSpellOnGCD("Hammer of the Righteous") && WoW.IsSpellInRange("Rebuke"))
                    {
                       	WoW.CastSpell("Hammer of the Righteous");
                        return;
                    }
                    if (WoW.CanCast("Judgement") && !WoW.IsSpellOnGCD("Judgement"))
                    {
                       	WoW.CastSpell("Judgement");
                        return;
                    }
                    if (WoW.CanCast("Consecration") && !WoW.IsSpellOnGCD("Consecration") && !WoW.TargetHasDebuff("Consecration") && WoW.IsSpellInRange("Rebuke"))
                    {
                       	WoW.CastSpell("Consecration");
                        return;
                    }
                    if (WoW.CanCast("Avenger's Shield") && !WoW.IsSpellOnGCD("Avenger's Shield"))
                    {
                        WoW.CastSpell("Avenger's Shield");
                        return;
                    }
                    if (WoW.Talent(1) == 2 && WoW.CanCast("Blessed Hammer") && WoW.PlayerSpellCharges("Blessed Hammer") >= 1 && !WoW.IsSpellOnGCD("Blessed Hammer") && !WoW.TargetHasDebuff("Blessed Hammer") && WoW.IsSpellInRange("Rebuke"))
                    {
                       	WoW.CastSpell("Blessed Hammer");
                        return;
                   	}
				}
			}
		}
	}
	

    public class SettingsFormDFF : Form
    {
        public Button btnHotkeysProtOffensiveCooldowns;
        public CheckBox checkHotkeysProtLotP;
        public TextBox checkHotkeysProtLotPPercent;
        public Label checkHotkeysProtLotPPercentLabel;
        public CheckBox checkHotkeysProtLoH;
        public TextBox checkHotkeysProtLoHPercent;
        private readonly Label checkHotkeysProtLoHPercentLabel;
        public CheckBox checkHotkeysProtAD;
        public TextBox checkHotkeysProtADPercent;
        public Label checkHotkeysProtADPercentLabel;
        public CheckBox checkHotkeysProtGotAK;
        public TextBox checkHotkeysProtGotAKPercent;
        public Label checkHotkeysProtGotAKPercentLabel;
        public CheckBox checkHotkeysProtEoT;
        public TextBox checkHotkeysProtEoTPercent;
        public Label checkHotkeysProtEoTPercentLabel;
        public CheckBox checkHotkeysProtRacials;
        public CheckBox checkHotkeysProtKick;
        public TextBox checkHotkeysProtKickPercent;
        public Label checkHotkeysProtKickPercentLabel;
        public CheckBox checkHotkeysProtAW;
        public CheckBox checkHotkeysProtBOL;

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
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysProtRacials = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtKick = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtKickPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysProtKickPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysProtLotP = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtLotPPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysProtLotPPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysProtLoH = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtLoHPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysProtLoHPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysProtAD = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtADPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysProtADPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysProtGotAK = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtGotAKPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysProtGotAKPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysProtEoT = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtEoTPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysProtEoTPercentLabel = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysProtAW = new System.Windows.Forms.CheckBox();
            this.checkHotkeysProtBOL = new System.Windows.Forms.CheckBox();
            this.btnHotkeysProtOffensiveCooldowns = new System.Windows.Forms.Button();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage5.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.tabControl3.SuspendLayout();

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
            this.groupBox12.Controls.Add(this.checkHotkeysProtLotP);
            this.groupBox12.Controls.Add(this.checkHotkeysProtLotPPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysProtLotPPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysProtLoH);
            this.groupBox12.Controls.Add(this.checkHotkeysProtLoHPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysProtLoHPercentLabel);	
            this.groupBox12.Controls.Add(this.checkHotkeysProtAD);
            this.groupBox12.Controls.Add(this.checkHotkeysProtADPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysProtADPercentLabel);			
            this.groupBox12.Controls.Add(this.checkHotkeysProtGotAK);
            this.groupBox12.Controls.Add(this.checkHotkeysProtGotAKPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysProtGotAKPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysProtEoT);
            this.groupBox12.Controls.Add(this.checkHotkeysProtEoTPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysProtEoTPercentLabel);
            this.groupBox12.Location = new System.Drawing.Point(8, 100);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(561, 110);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Defensive Cooldowns";
			
            // 
            // checkHotkeysProtLotP
            // 
            this.checkHotkeysProtLotP.AutoSize = true;
            this.checkHotkeysProtLotP.Location = new System.Drawing.Point(20, 28);
            this.checkHotkeysProtLotP.Name = "checkHotkeysProtLotP";
            this.checkHotkeysProtLotP.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysProtLotP.TabIndex = 8;
            this.checkHotkeysProtLotP.Text = "Light/Hand of the Protector";
            this.checkHotkeysProtLotP.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysProtLotPPercent
            // 
            this.checkHotkeysProtLotPPercent.AutoSize = true;
            this.checkHotkeysProtLotPPercent.Location = new System.Drawing.Point(190, 28);
            this.checkHotkeysProtLotPPercent.Name = "checkHotkeysProtLotPPercent";
            this.checkHotkeysProtLotPPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtLotPPercent.TabIndex = 9;
            this.checkHotkeysProtLotPPercent.Text = "50";
            // 
            // checkHotkeysProtLotPPercentLabel
            // 
            this.checkHotkeysProtLotPPercentLabel.AutoSize = true;
            this.checkHotkeysProtLotPPercentLabel.Location = new System.Drawing.Point(211, 30);
            this.checkHotkeysProtLotPPercentLabel.Name = "checkHotkeysProtLotPPercentLabel";
            this.checkHotkeysProtLotPPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtLotPPercentLabel.TabIndex = 9;
            this.checkHotkeysProtLotPPercentLabel.Text = "% HP";
            // 
            // checkHotkeysProtLoH
            // 
            this.checkHotkeysProtLoH.AutoSize = true;
            this.checkHotkeysProtLoH.Location = new System.Drawing.Point(20, 50);
            this.checkHotkeysProtLoH.Name = "checkHotkeysProtLoH";
            this.checkHotkeysProtLoH.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysProtLoH.TabIndex = 9;
            this.checkHotkeysProtLoH.Text = "Lay on Hands";
            this.checkHotkeysProtLoH.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysProtLoHPercent
            // 
            this.checkHotkeysProtLoHPercent.AutoSize = true;
            this.checkHotkeysProtLoHPercent.Location = new System.Drawing.Point(190, 50);
            this.checkHotkeysProtLoHPercent.Name = "checkHotkeysProtLoHPercent";
            this.checkHotkeysProtLoHPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtLoHPercent.TabIndex = 9;
            this.checkHotkeysProtLoHPercent.Text = "50";
            // 
            // checkHotkeysProtLoHPercentLabel
            // 
            this.checkHotkeysProtLoHPercentLabel.AutoSize = true;
            this.checkHotkeysProtLoHPercentLabel.Location = new System.Drawing.Point(211, 52);
            this.checkHotkeysProtLoHPercentLabel.Name = "checkHotkeysProtLoHPercentLabel";
            this.checkHotkeysProtLoHPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtLoHPercentLabel.TabIndex = 9;
            this.checkHotkeysProtLoHPercentLabel.Text = "% HP";
			
            // 
            // checkHotkeysProtAD
            // 
            this.checkHotkeysProtAD.AutoSize = true;
            this.checkHotkeysProtAD.Location = new System.Drawing.Point(20, 72);
            this.checkHotkeysProtAD.Name = "checkHotkeysProtAD";
            this.checkHotkeysProtAD.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysProtAD.TabIndex = 8;
            this.checkHotkeysProtAD.Text = "Ardent Defender";
            this.checkHotkeysProtAD.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysProtADPercent
            // 
            this.checkHotkeysProtADPercent.AutoSize = true;
            this.checkHotkeysProtADPercent.Location = new System.Drawing.Point(190, 72);
            this.checkHotkeysProtADPercent.Name = "checkHotkeysProtADPercent";
            this.checkHotkeysProtADPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtADPercent.TabIndex = 9;
            this.checkHotkeysProtADPercent.Text = "50";
            // 
            // checkHotkeysProtADPercentLabel
            // 
            this.checkHotkeysProtADPercentLabel.AutoSize = true;
            this.checkHotkeysProtADPercentLabel.Location = new System.Drawing.Point(211, 74);
            this.checkHotkeysProtADPercentLabel.Name = "checkHotkeysProtADPercentLabel";
            this.checkHotkeysProtADPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtADPercentLabel.TabIndex = 9;
            this.checkHotkeysProtADPercentLabel.Text = "% HP";
			
            // 
            // checkHotkeysProtGotAK
            // 
            this.checkHotkeysProtGotAK.AutoSize = true;
            this.checkHotkeysProtGotAK.Location = new System.Drawing.Point(300, 28);
            this.checkHotkeysProtGotAK.Name = "checkHotkeysProtGotAK";
            this.checkHotkeysProtGotAK.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysProtGotAK.TabIndex = 8;
            this.checkHotkeysProtGotAK.Text = "Guardian of the Ancient Kings";
            this.checkHotkeysProtGotAK.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysProtGotAKPercent
            // 
            this.checkHotkeysProtGotAKPercent.AutoSize = true;
            this.checkHotkeysProtGotAKPercent.Location = new System.Drawing.Point(470, 28);
            this.checkHotkeysProtGotAKPercent.Name = "checkHotkeysProtGotAKPercent";
            this.checkHotkeysProtGotAKPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtGotAKPercent.TabIndex = 9;
            this.checkHotkeysProtGotAKPercent.Text = "50";
            // 
            // checkHotkeysProtGotAKPercentLabel
            // 
            this.checkHotkeysProtGotAKPercentLabel.AutoSize = true;
            this.checkHotkeysProtGotAKPercentLabel.Location = new System.Drawing.Point(491, 30);
            this.checkHotkeysProtGotAKPercentLabel.Name = "checkHotkeysProtGotAKPercentLabel";
            this.checkHotkeysProtGotAKPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtGotAKPercentLabel.TabIndex = 9;
            this.checkHotkeysProtGotAKPercentLabel.Text = "% HP";
			
            // 
            // checkHotkeysProtEoT
            // 
            this.checkHotkeysProtEoT.AutoSize = true;
            this.checkHotkeysProtEoT.Location = new System.Drawing.Point(300, 50);
            this.checkHotkeysProtEoT.Name = "checkHotkeysProtEoT";
            this.checkHotkeysProtEoT.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysProtEoT.TabIndex = 8;
            this.checkHotkeysProtEoT.Text = "Eye of Tyr";
            this.checkHotkeysProtEoT.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysProtEoTPercent
            // 
            this.checkHotkeysProtEoTPercent.AutoSize = true;
            this.checkHotkeysProtEoTPercent.Location = new System.Drawing.Point(470, 50);
            this.checkHotkeysProtEoTPercent.Name = "checkHotkeysProtEoTPercent";
            this.checkHotkeysProtEoTPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtEoTPercent.TabIndex = 9;
            this.checkHotkeysProtEoTPercent.Text = "50";
            // 
            // checkHotkeysProtEoTPercentLabel
            // 
            this.checkHotkeysProtEoTPercentLabel.AutoSize = true;
            this.checkHotkeysProtEoTPercentLabel.Location = new System.Drawing.Point(491, 52);
            this.checkHotkeysProtEoTPercentLabel.Name = "checkHotkeysProtEoTPercentLabel";
            this.checkHotkeysProtEoTPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtEoTPercentLabel.TabIndex = 9;
            this.checkHotkeysProtEoTPercentLabel.Text = "% HP";


            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.checkHotkeysProtBOL);
            this.groupBox13.Controls.Add(this.checkHotkeysProtAW);
            this.groupBox13.Controls.Add(this.btnHotkeysProtOffensiveCooldowns);
            this.groupBox13.Controls.Add(this.checkHotkeysProtRacials);
            this.groupBox13.Controls.Add(this.checkHotkeysProtKick);
            this.groupBox13.Controls.Add(this.checkHotkeysProtKickPercent);
            this.groupBox13.Controls.Add(this.checkHotkeysProtKickPercentLabel);
            this.groupBox13.Location = new System.Drawing.Point(8, 8);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(561, 90);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Offensive Cooldowns";
			
            // 
            // checkHotkeysProtAW
            // 
            this.checkHotkeysProtAW.AutoSize = true;
            this.checkHotkeysProtAW.Location = new System.Drawing.Point(151, 32);
            this.checkHotkeysProtAW.Name = "checkHotkeysProtAW";
            this.checkHotkeysProtAW.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysProtAW.TabIndex = 2;
            this.checkHotkeysProtAW.Text = "Avenging Wrath";
            this.checkHotkeysProtAW.UseVisualStyleBackColor = true;

            // 
            // checkHotkeysProtBOL
            // 
            this.checkHotkeysProtBOL.AutoSize = true;
            this.checkHotkeysProtBOL.Location = new System.Drawing.Point(151, 60);
            this.checkHotkeysProtBOL.Name = "checkHotkeysProtBOL";
            this.checkHotkeysProtBOL.Size = new System.Drawing.Size(48, 17);
            this.checkHotkeysProtBOL.TabIndex = 3;
            this.checkHotkeysProtBOL.Text = "Bastion of Light";
            this.checkHotkeysProtBOL.UseVisualStyleBackColor = true;

            // 
            // checkHotkeysProtRacials
            // 
            this.checkHotkeysProtRacials.AutoSize = true;
            this.checkHotkeysProtRacials.Location = new System.Drawing.Point(300, 32);
            this.checkHotkeysProtRacials.Name = "checkHotkeysProtRacials";
            this.checkHotkeysProtRacials.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysProtRacials.TabIndex = 4;
            this.checkHotkeysProtRacials.Text = "Racials";
            this.checkHotkeysProtRacials.UseVisualStyleBackColor = true;
			
            // 
            // checkHotkeysProtKick
            // 
            this.checkHotkeysProtKick.AutoSize = true;
            this.checkHotkeysProtKick.Location = new System.Drawing.Point(300, 60);
            this.checkHotkeysProtKick.Name = "checkHotkeysProtKick";
            this.checkHotkeysProtKick.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysProtKick.TabIndex = 5;
            this.checkHotkeysProtKick.Text = "Interrupt @";
            this.checkHotkeysProtKick.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysProtKickPercent
            // 
            this.checkHotkeysProtKickPercent.AutoSize = true;
            this.checkHotkeysProtKickPercent.Location = new System.Drawing.Point(390, 60);
            this.checkHotkeysProtKickPercent.Name = "checkHotkeysProtKickPercent";
            this.checkHotkeysProtKickPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtKickPercent.TabIndex = 9;
            this.checkHotkeysProtKickPercent.Text = "50";
            // 
            // checkHotkeysProtKickPercentLabel
            // 
            this.checkHotkeysProtKickPercentLabel.AutoSize = true;
            this.checkHotkeysProtKickPercentLabel.Location = new System.Drawing.Point(411, 62);
            this.checkHotkeysProtKickPercentLabel.Name = "checkHotkeysProtKickPercentLabel";
            this.checkHotkeysProtKickPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysProtKickPercentLabel.TabIndex = 9;
            this.checkHotkeysProtKickPercentLabel.Text = "% of cast";
			
            // 
            // btnHotkeysProtOffensiveCooldowns
            // 
            this.btnHotkeysProtOffensiveCooldowns.Location = new System.Drawing.Point(18, 28);
            this.btnHotkeysProtOffensiveCooldowns.Name = "btnHotkeysProtOffensiveCooldowns";
            this.btnHotkeysProtOffensiveCooldowns.Size = new System.Drawing.Size(113, 23);
            this.btnHotkeysProtOffensiveCooldowns.TabIndex = 1;
            this.btnHotkeysProtOffensiveCooldowns.Text = "Click to Set";
            this.btnHotkeysProtOffensiveCooldowns.UseVisualStyleBackColor = true;
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
            this.Text = "Protection Paladin by smartie";
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
Spell,20271,Judgement,D4
Spell,204019,Blessed Hammer,D5
Spell,53595,Hammer of the Righteous,D5
Spell,53600,Shield of the Righteous,D6
Spell,31935,Avenger's Shield,D2
Spell,31884,Avenging Wrath,D7
Spell,26573,Consecration,D3
Spell,96231,Rebuke,NumPad4
Spell,184092,Light of the Protector,NumPad2
Spell,152262,Seraphim,D8
Spell,213652,Hand of the Protector,NumPad2
Spell,209202,Eye of Tyr,D9
Spell,633,Lay on Hands,NumPad3
Spell,86659,Guardian of the Ancient Kings,NumPad1
Spell,31850,Ardent Defender,NumPad5
Spell,204035,Bastion of Light,D0
Spell,80483,Arcane Torrent,NumPad6
Aura,132403,Shield of the Righteous
Aura,204301,Blessed Hammer
Aura,204242,Consecration
Aura,152262,Seraphim
Aura,221886,Divine Steed
*/
