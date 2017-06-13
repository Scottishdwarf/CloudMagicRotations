//Changelog
// v1.0 First release
// v1.1 many little bugs got fixed
// v1.2 removed Levelchecks, cause its buggy


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
    public class DruidGuardiansmartie : CombatRoutine
    {
        private static readonly Stopwatch coolDownStopWatch = new Stopwatch();
        public override string Name
        {
            get { return "Guardian Druid"; }
        }

        public override string Class
        {
            get { return "Druid"; }
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
                var cooldownKey = ConfigFile.ReadValue("DruidGuardian", "cooldownKey").Trim();
                if (cooldownKey != "")
                {
                    return Convert.ToInt32(cooldownKey);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "cooldownKey", value.ToString()); }
        }

        public static int cooldownModifier
        {
            get
            {
                var cooldownModifier = ConfigFile.ReadValue("DruidGuardian", "cooldownModifier").Trim();
                if (cooldownModifier != "")
                {
                    return Convert.ToInt32(cooldownModifier);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "cooldownModifier", value.ToString()); }
        }

        public static string cooldownHotKeyString
        {
            get
            {
                var cooldownHotKeyString = ConfigFile.ReadValue("DruidGuardian", "cooldownHotKeyString").Trim();

                if (cooldownHotKeyString != "")
                {
                    return cooldownHotKeyString;
                }

                return "Click to Set";
            }
            set { ConfigFile.WriteValue("DruidGuardian", "cooldownHotKeyString", value); }
        }

        public static bool ischeckHotkeysGuardianROTS
        {
            get
            {
                var ischeckHotkeysGuardianROTS = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianROTS").Trim();

                if (ischeckHotkeysGuardianROTS != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianROTS);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianROTS", value.ToString()); }
        }

        public static int GuardianROTSPercent
        {
            get
            {
                var GuardianROTSPercent = ConfigFile.ReadValue("DruidGuardian", "GuardianROTSPercent").Trim();
                if (GuardianROTSPercent != "")
                {
                    return Convert.ToInt32(GuardianROTSPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "GuardianROTSPercent", value.ToString()); }
        }

        public static bool ischeckHotkeysGuardianLunarBeam
        {
            get
            {
                var ischeckHotkeysGuardianLunarBeam = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianLunarBeam").Trim();

                if (ischeckHotkeysGuardianLunarBeam != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianLunarBeam);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianLunarBeam", value.ToString()); }
        }

        public static int GuardianLunarBeamPercent
        {
            get
            {
                var GuardianLunarBeamPercent = ConfigFile.ReadValue("DruidGuardian", "GuardianLunarBeamPercent").Trim();
                if (GuardianLunarBeamPercent != "")
                {
                    return Convert.ToInt32(GuardianLunarBeamPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "GuardianLunarBeamPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysGuardianBarkskin
        {
            get
            {
                var ischeckHotkeysGuardianBarkskin = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianBarkskin").Trim();

                if (ischeckHotkeysGuardianBarkskin != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianBarkskin);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianBarkskin", value.ToString()); }
        }

        public static int GuardianBarkskinPercent
        {
            get
            {
                var GuardianBarkskinPercent = ConfigFile.ReadValue("DruidGuardian", "GuardianBarkskinPercent").Trim();
                if (GuardianBarkskinPercent != "")
                {
                    return Convert.ToInt32(GuardianBarkskinPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "GuardianBarkskinPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysGuardianSI
        {
            get
            {
                var ischeckHotkeysGuardianSI = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianSI").Trim();

                if (ischeckHotkeysGuardianSI != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianSI);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianSI", value.ToString()); }
        }

        public static int GuardianSIPercent
        {
            get
            {
                var GuardianSIPercent = ConfigFile.ReadValue("DruidGuardian", "GuardianSIPercent").Trim();
                if (GuardianSIPercent != "")
                {
                    return Convert.ToInt32(GuardianSIPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "GuardianSIPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysGuardianFReg
        {
            get
            {
                var ischeckHotkeysGuardianFReg = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianFReg").Trim();

                if (ischeckHotkeysGuardianFReg != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianFReg);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianFReg", value.ToString()); }
        }

        public static int GuardianFRegPercent
        {
            get
            {
                var GuardianFRegPercent = ConfigFile.ReadValue("DruidGuardian", "GuardianFRegPercent").Trim();
                if (GuardianFRegPercent != "")
                {
                    return Convert.ToInt32(GuardianFRegPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "GuardianFRegPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysGuardianBristlingFur
        {
            get
            {
                var ischeckHotkeysGuardianBristlingFur = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianBristlingFur").Trim();

                if (ischeckHotkeysGuardianBristlingFur != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianBristlingFur);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianBristlingFur", value.ToString()); }
        }

        public static int GuardianBristlingFurPercent
        {
            get
            {
                var GuardianBristlingFurPercent = ConfigFile.ReadValue("DruidGuardian", "GuardianBristlingFurPercent").Trim();
                if (GuardianBristlingFurPercent != "")
                {
                    return Convert.ToInt32(GuardianBristlingFurPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "GuardianBristlingFurPercent", value.ToString()); }
        }
		
        public static bool ischeckHotkeysGuardianPulverize
        {
            get
            {
                var ischeckHotkeysGuardianPulverize = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianPulverize").Trim();

                if (ischeckHotkeysGuardianPulverize != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianPulverize);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianPulverize", value.ToString()); }
        }

        public static int GuardianPulverizePercent
        {
            get
            {
                var GuardianPulverizePercent = ConfigFile.ReadValue("DruidGuardian", "GuardianPulverizePercent").Trim();
                if (GuardianPulverizePercent != "")
                {
                    return Convert.ToInt32(GuardianPulverizePercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "GuardianPulverizePercent", value.ToString()); }
        }


        public static bool ischeckHotkeysGuardianIncarnation
        {
            get
            {
                var ischeckHotkeysGuardianIncarnation = ConfigFile.ReadValue("DruidGuardian", "ischeckHotkeysGuardianIncarnation").Trim();

                if (ischeckHotkeysGuardianIncarnation != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysGuardianIncarnation);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DruidGuardian", "ischeckHotkeysGuardianIncarnation", value.ToString()); }
        }

        public override void Initialize()
        {
            Log.Write("Welcome to the Guardian Druid v1.2 by smartie", Color.Green);
	        Log.Write("All Talents supported and auto detected", Color.Green);		
            SettingsFormDFF = new SettingsFormDFF();
            SettingsForm = SettingsFormDFF;

            SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.Text = cooldownHotKeyString;

            SettingsFormDFF.checkHotkeysGuardianIncarnation.Checked = ischeckHotkeysGuardianIncarnation;
            SettingsFormDFF.checkHotkeysGuardianLunarBeam.Checked = ischeckHotkeysGuardianLunarBeam;
            SettingsFormDFF.checkHotkeysGuardianLunarBeamPercent.Text = GuardianLunarBeamPercent.ToString();
            SettingsFormDFF.checkHotkeysGuardianROTS.Checked = ischeckHotkeysGuardianROTS;
            SettingsFormDFF.checkHotkeysGuardianROTSPercent.Text = GuardianROTSPercent.ToString();
            SettingsFormDFF.checkHotkeysGuardianBarkskin.Checked = ischeckHotkeysGuardianBarkskin;
            SettingsFormDFF.checkHotkeysGuardianBarkskinPercent.Text = GuardianBarkskinPercent.ToString();
            SettingsFormDFF.checkHotkeysGuardianSI.Checked = ischeckHotkeysGuardianSI;
            SettingsFormDFF.checkHotkeysGuardianSIPercent.Text = GuardianSIPercent.ToString();
            SettingsFormDFF.checkHotkeysGuardianFReg.Checked = ischeckHotkeysGuardianFReg;
            SettingsFormDFF.checkHotkeysGuardianFRegPercent.Text = GuardianFRegPercent.ToString();
            SettingsFormDFF.checkHotkeysGuardianBristlingFur.Checked = ischeckHotkeysGuardianBristlingFur;
            SettingsFormDFF.checkHotkeysGuardianBristlingFurPercent.Text = GuardianBristlingFurPercent.ToString();
            SettingsFormDFF.checkHotkeysGuardianPulverize.Checked = ischeckHotkeysGuardianPulverize;
            SettingsFormDFF.checkHotkeysGuardianPulverizePercent.Text = GuardianPulverizePercent.ToString();

			try
			{
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            SettingsFormDFF.checkHotkeysGuardianLunarBeam.CheckedChanged += ischeckHotkeysGuardianLunarBeam_Click;
            SettingsFormDFF.checkHotkeysGuardianLunarBeamPercent.TextChanged += ischeckHotkeysGuardianLunarBeamPercent_Click;
            SettingsFormDFF.checkHotkeysGuardianROTS.CheckedChanged += ischeckHotkeysGuardianROTS_Click;
            SettingsFormDFF.checkHotkeysGuardianROTSPercent.TextChanged += ischeckHotkeysGuardianROTSPercent_Click;
            SettingsFormDFF.checkHotkeysGuardianBarkskin.CheckedChanged += ischeckHotkeysGuardianBarkskin_Click;
            SettingsFormDFF.checkHotkeysGuardianBarkskinPercent.TextChanged += ischeckHotkeysGuardianBarkskinPercent_Click;
            SettingsFormDFF.checkHotkeysGuardianSI.CheckedChanged += ischeckHotkeysGuardianSI_Click;
            SettingsFormDFF.checkHotkeysGuardianSIPercent.TextChanged += ischeckHotkeysGuardianSIPercent_Click;
            SettingsFormDFF.checkHotkeysGuardianFReg.CheckedChanged += ischeckHotkeysGuardianFReg_Click;
            SettingsFormDFF.checkHotkeysGuardianFRegPercent.TextChanged += ischeckHotkeysGuardianFRegPercent_Click;
            SettingsFormDFF.checkHotkeysGuardianBristlingFur.CheckedChanged += ischeckHotkeysGuardianBristlingFur_Click;
            SettingsFormDFF.checkHotkeysGuardianBristlingFurPercent.TextChanged += ischeckHotkeysGuardianBristlingFurPercent_Click;
            SettingsFormDFF.checkHotkeysGuardianPulverize.CheckedChanged += ischeckHotkeysGuardianPulverize_Click;
            SettingsFormDFF.checkHotkeysGuardianPulverizePercent.TextChanged += ischeckHotkeysGuardianPulverizePercent_Click;
            SettingsFormDFF.checkHotkeysGuardianIncarnation.CheckedChanged += ischeckHotkeysGuardianIncarnation_Click;
            SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.KeyDown += KeyDown;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
                return;
            SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.Text = "Hotkey : ";
            if (e.Shift)
            {
                cooldownModifier = (int)Keys.ShiftKey;
                SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.Text += Keys.Shift + " + ";
            }
            else if (e.Alt)
            {
                cooldownModifier = (int)Keys.Menu;
                SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.Text += Keys.Alt + " + ";
            }
            else if (e.Control)
            {
                cooldownModifier = (int)Keys.ControlKey;
                SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.Text += Keys.Control + " + ";
            }
            else cooldownModifier = -1;
            cooldownKey = (int)e.KeyCode;
            SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.Text += e.KeyCode;
            cooldownHotKeyString = SettingsFormDFF.btnHotkeysGuardianOffensiveCooldowns.Text;
            SettingsFormDFF.checkHotkeysGuardianROTSPercentLabel.Focus();
        }

        private void ischeckHotkeysGuardianROTS_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianROTS = SettingsFormDFF.checkHotkeysGuardianROTS.Checked;
        }

        private void ischeckHotkeysGuardianROTSPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysGuardianROTSPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                GuardianROTSPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysGuardianROTSPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void ischeckHotkeysGuardianLunarBeam_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianLunarBeam = SettingsFormDFF.checkHotkeysGuardianLunarBeam.Checked;
        }

        private void ischeckHotkeysGuardianLunarBeamPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysGuardianLunarBeamPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                GuardianLunarBeamPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysGuardianLunarBeamPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysGuardianBarkskin_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianBarkskin = SettingsFormDFF.checkHotkeysGuardianBarkskin.Checked;
        }

        private void ischeckHotkeysGuardianBarkskinPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysGuardianBarkskinPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                GuardianBarkskinPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysGuardianBarkskinPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysGuardianSI_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianSI = SettingsFormDFF.checkHotkeysGuardianSI.Checked;
        }

        private void ischeckHotkeysGuardianSIPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysGuardianSIPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                GuardianSIPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysGuardianSIPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysGuardianFReg_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianFReg = SettingsFormDFF.checkHotkeysGuardianFReg.Checked;
        }

        private void ischeckHotkeysGuardianFRegPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysGuardianFRegPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                GuardianFRegPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysGuardianFRegPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysGuardianBristlingFur_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianBristlingFur = SettingsFormDFF.checkHotkeysGuardianBristlingFur.Checked;
        }

        private void ischeckHotkeysGuardianBristlingFurPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysGuardianBristlingFurPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                GuardianBristlingFurPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysGuardianBristlingFurPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }
		
        private void ischeckHotkeysGuardianPulverize_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianPulverize = SettingsFormDFF.checkHotkeysGuardianPulverize.Checked;
        }

        private void ischeckHotkeysGuardianPulverizePercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysGuardianPulverizePercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                GuardianPulverizePercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysGuardianPulverizePercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }

        private void ischeckHotkeysGuardianIncarnation_Click(object sender, EventArgs e)
        {
            ischeckHotkeysGuardianIncarnation = SettingsFormDFF.checkHotkeysGuardianIncarnation.Checked;
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
            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted && !WoW.PlayerHasBuff("Travel Form"))
            {
                if (!WoW.PlayerHasBuff("Bear Form"))
                {
                    WoW.CastSpell("Bear Form");
                }
				//DEF CDS
				if (!WoW.IsSpellOnCooldown("Lunar Beam") && WoW.HealthPercent <= GuardianLunarBeamPercent && ischeckHotkeysGuardianLunarBeam && WoW.Talent(7) == 2)
                {
                    WoW.CastSpell("Lunar Beam");
                }
				if (!WoW.IsSpellOnCooldown("Rage of the Sleeper") && WoW.HealthPercent <= GuardianROTSPercent && ischeckHotkeysGuardianROTS)
                {
                    WoW.CastSpell("Rage of the Sleeper");
                }
				if (!WoW.IsSpellOnCooldown("Barkskin") && WoW.HealthPercent <= GuardianBarkskinPercent && ischeckHotkeysGuardianBarkskin)
                {
                    WoW.CastSpell("Barkskin");
                }
				if (!WoW.IsSpellOnCooldown("Survival Instincts") && WoW.HealthPercent <= GuardianSIPercent && ischeckHotkeysGuardianSI && !WoW.PlayerHasBuff("Survival Instincts"))
                {
                    WoW.CastSpell("Survival Instincts");
                }
				if (!WoW.IsSpellOnCooldown("Frenzied Regeneration") && WoW.HealthPercent <= GuardianFRegPercent && ischeckHotkeysGuardianFReg && !WoW.PlayerHasBuff("Frenzied Regeneration") && !WoW.PlayerHasBuff("Natural Defenses") && WoW.Rage >=10)
                {
                    WoW.CastSpell("Frenzied Regeneration");
                }
				if (!WoW.IsSpellOnCooldown("Frenzied Regeneration") && WoW.HealthPercent <= GuardianFRegPercent && ischeckHotkeysGuardianFReg && !WoW.PlayerHasBuff("Frenzied Regeneration") && WoW.PlayerHasBuff("Natural Defenses") && WoW.Rage >=5)
                {
                    WoW.CastSpell("Frenzied Regeneration");
                }
				if (!WoW.IsSpellOnCooldown("Bristling Fur") && WoW.HealthPercent <= GuardianBristlingFurPercent && ischeckHotkeysGuardianBristlingFur && WoW.Talent(1) == 2)
                {
                    WoW.CastSpell("Bristling Fur");
                }
				if (!WoW.IsSpellOnCooldown("Pulverize") && WoW.HealthPercent <= GuardianPulverizePercent && ischeckHotkeysGuardianPulverize && WoW.Talent(7) == 3 && WoW.TargetDebuffStacks("Thrash") >=2)
                {
                    WoW.CastSpell("Pulverize");
                }
                if (WoW.CanCast("Ironfur") && WoW.HealthPercent <= 90 && WoW.Rage >= 45)
                {
					WoW.CastSpell("Ironfur");
                    return;
                }

				if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave || combatRoutine.Type == RotationType.AOE)
				{
					if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted && !WoW.PlayerHasBuff("Travel Form"))
					{
                        if (combatRoutine.UseCooldowns && WoW.CanCast("Incarnation") && ischeckHotkeysGuardianIncarnation && WoW.Talent(5) == 2 && WoW.TargetHasDebuff("Moonfire"))
                        {
                            WoW.CastSpell("Incarnation");
                            return;
                        }
						if (WoW.IsSpellInRange("Moonfire") && WoW.CanCast("Moonfire") && WoW.PlayerHasBuff("Galactic Guardian"))
                        {
                            WoW.CastSpell("Moonfire");
                            return;
                        }
                        if (WoW.IsSpellInRange("Moonfire") && WoW.CanCast("Moonfire") && WoW.TargetHasDebuff("Moonfire") && WoW.TargetDebuffTimeRemaining("Moonfire") <= 300 || !WoW.TargetHasDebuff("Moonfire"))
                        {
                            WoW.CastSpell("Moonfire");
                            return;
                        }
                        if (WoW.CanCast("Thrash") && WoW.TargetHasDebuff("Moonfire") && !WoW.PlayerHasBuff("Incarnation"))
                        {
                            WoW.CastSpell("Thrash");
                            return;
                        }
                        if (WoW.CanCast("Thrash") && WoW.TargetHasDebuff("Moonfire") && WoW.PlayerHasBuff("Incarnation") && !WoW.PlayerHasBuff("Gore"))
                        {
                            WoW.CastSpell("Thrash");
                            return;
                        }
                        if (WoW.CanCast("Mangle") && WoW.TargetHasDebuff("Moonfire"))
                        {
                            WoW.CastSpell("Mangle");
							return;
                        }
                        if (WoW.CanCast("Mangle") && WoW.TargetHasDebuff("Moonfire") && WoW.PlayerHasBuff("Incarnation") && WoW.PlayerHasBuff("Gore"))
                        {
                            WoW.CastSpell("Mangle");
							return;
                        }
                        if (WoW.CanCast("Swipe") && WoW.TargetHasDebuff("Moonfire") && WoW.IsSpellOnCooldown("Thrash") && WoW.IsSpellOnCooldown("Mangle"))
                        {
                            WoW.CastSpell("Swipe");
							return;
                        }
                        if (WoW.CanCast("Maul") && WoW.Rage >= 80 && WoW.TargetHasDebuff("Moonfire") && WoW.HealthPercent >= 90)
                        {
                            WoW.CastSpell("Maul");
                            return;
                        }
					}
				}
			}
		}
	}
	

    public class SettingsFormDFF : Form
    {
        public Button btnaddspell;
        public Button btnHotkeysGuardianOffensiveCooldowns;
        public Button btnremovespell;
        public CheckBox checkHotkeysGuardianLunarBeam;
        public TextBox checkHotkeysGuardianLunarBeamPercent;
        public Label checkHotkeysGuardianLunarBeamPercentLabel;
        public CheckBox checkHotkeysGuardianROTS;
        public TextBox checkHotkeysGuardianROTSPercent;
        public Label checkHotkeysGuardianROTSPercentLabel;
        public CheckBox checkHotkeysGuardianBarkskin;
        public TextBox checkHotkeysGuardianBarkskinPercent;
        public Label checkHotkeysGuardianBarkskinPercentLabel;
        public CheckBox checkHotkeysGuardianSI;
        public TextBox checkHotkeysGuardianSIPercent;
        public Label checkHotkeysGuardianSIPercentLabel;
        public CheckBox checkHotkeysGuardianFReg;
        public TextBox checkHotkeysGuardianFRegPercent;
        public Label checkHotkeysGuardianFRegPercentLabel;
        public CheckBox checkHotkeysGuardianBristlingFur;
        public TextBox checkHotkeysGuardianBristlingFurPercent;
        public Label checkHotkeysGuardianBristlingFurPercentLabel;
        public CheckBox checkHotkeysGuardianPulverize;
        public TextBox checkHotkeysGuardianPulverizePercent;
        public Label checkHotkeysGuardianPulverizePercentLabel;
        public CheckBox checkHotkeysGuardianIncarnation;


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
            this.checkHotkeysGuardianLunarBeam = new System.Windows.Forms.CheckBox();
            this.checkHotkeysGuardianLunarBeamPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysGuardianLunarBeamPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysGuardianROTS = new System.Windows.Forms.CheckBox();
            this.checkHotkeysGuardianROTSPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysGuardianROTSPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysGuardianBarkskin = new System.Windows.Forms.CheckBox();
            this.checkHotkeysGuardianBarkskinPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysGuardianBarkskinPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysGuardianSI = new System.Windows.Forms.CheckBox();
            this.checkHotkeysGuardianSIPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysGuardianSIPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysGuardianFReg = new System.Windows.Forms.CheckBox();
            this.checkHotkeysGuardianFRegPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysGuardianFRegPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysGuardianBristlingFur = new System.Windows.Forms.CheckBox();
            this.checkHotkeysGuardianBristlingFurPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysGuardianBristlingFurPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysGuardianPulverize = new System.Windows.Forms.CheckBox();
            this.checkHotkeysGuardianPulverizePercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysGuardianPulverizePercentLabel = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysGuardianIncarnation = new System.Windows.Forms.CheckBox();
            this.btnHotkeysGuardianOffensiveCooldowns = new System.Windows.Forms.Button();
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
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianLunarBeam);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianLunarBeamPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianLunarBeamPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianROTS);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianROTSPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianROTSPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianBarkskin);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianBarkskinPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianBarkskinPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianSI);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianSIPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianSIPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianFReg);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianFRegPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianFRegPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianBristlingFur);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianBristlingFurPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianBristlingFurPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianPulverize);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianPulverizePercent);
            this.groupBox12.Controls.Add(this.checkHotkeysGuardianPulverizePercentLabel);

            this.groupBox12.Location = new System.Drawing.Point(8, 100);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(556, 190);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Defensive Cooldowns";
            // 
            // checkHotkeysGuardianLunarBeam
            // 
            this.checkHotkeysGuardianLunarBeam.AutoSize = true;
            this.checkHotkeysGuardianLunarBeam.Location = new System.Drawing.Point(151, 28);
            this.checkHotkeysGuardianLunarBeam.Name = "checkHotkeysGuardianLunarBeam";
            this.checkHotkeysGuardianLunarBeam.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysGuardianLunarBeam.TabIndex = 8;
            this.checkHotkeysGuardianLunarBeam.Text = "Lunar Beam";
            this.checkHotkeysGuardianLunarBeam.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysGuardianLunarBeamPercent
            // 
            this.checkHotkeysGuardianLunarBeamPercent.AutoSize = true;
            this.checkHotkeysGuardianLunarBeamPercent.Location = new System.Drawing.Point(300, 28);
            this.checkHotkeysGuardianLunarBeamPercent.Name = "checkHotkeysGuardianLunarBeamPercent";
            this.checkHotkeysGuardianLunarBeamPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianLunarBeamPercent.TabIndex = 9;
            this.checkHotkeysGuardianLunarBeamPercent.Text = "50";
            // 
            // checkHotkeysGuardianLunarBeamPercentLabel
            // 
            this.checkHotkeysGuardianLunarBeamPercentLabel.AutoSize = true;
            this.checkHotkeysGuardianLunarBeamPercentLabel.Location = new System.Drawing.Point(321, 30);
            this.checkHotkeysGuardianLunarBeamPercentLabel.Name = "checkHotkeysGuardianLunarBeamPercentLabel";
            this.checkHotkeysGuardianLunarBeamPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianLunarBeamPercentLabel.TabIndex = 9;
            this.checkHotkeysGuardianLunarBeamPercentLabel.Text = "% HP";
            // 
            // checkHotkeysGuardianROTS
            // 
            this.checkHotkeysGuardianROTS.AutoSize = true;
            this.checkHotkeysGuardianROTS.Location = new System.Drawing.Point(151, 50);
            this.checkHotkeysGuardianROTS.Name = "checkHotkeysGuardianROTS";
            this.checkHotkeysGuardianROTS.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysGuardianROTS.TabIndex = 9;
            this.checkHotkeysGuardianROTS.Text = "Rage of the Sleeper";
            this.checkHotkeysGuardianROTS.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysGuardianROTSPercent
            // 
            this.checkHotkeysGuardianROTSPercent.AutoSize = true;
            this.checkHotkeysGuardianROTSPercent.Location = new System.Drawing.Point(300, 50);
            this.checkHotkeysGuardianROTSPercent.Name = "checkHotkeysGuardianROTSPercent";
            this.checkHotkeysGuardianROTSPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianROTSPercent.TabIndex = 9;
            this.checkHotkeysGuardianROTSPercent.Text = "50";
            // 
            // checkHotkeysGuardianROTSPercentLabel
            // 
            this.checkHotkeysGuardianROTSPercentLabel.AutoSize = true;
            this.checkHotkeysGuardianROTSPercentLabel.Location = new System.Drawing.Point(321, 52);
            this.checkHotkeysGuardianROTSPercentLabel.Name = "checkHotkeysGuardianROTSPercentLabel";
            this.checkHotkeysGuardianROTSPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianROTSPercentLabel.TabIndex = 9;
            this.checkHotkeysGuardianROTSPercentLabel.Text = "% HP";
            // 
            // checkHotkeysGuardianBarkskin
            // 
            this.checkHotkeysGuardianBarkskin.AutoSize = true;
            this.checkHotkeysGuardianBarkskin.Location = new System.Drawing.Point(151, 72);
            this.checkHotkeysGuardianBarkskin.Name = "checkHotkeysGuardianBarkskin";
            this.checkHotkeysGuardianBarkskin.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysGuardianBarkskin.TabIndex = 8;
            this.checkHotkeysGuardianBarkskin.Text = "Barkskin";
            this.checkHotkeysGuardianBarkskin.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysGuardianBarkskinPercent
            // 
            this.checkHotkeysGuardianBarkskinPercent.AutoSize = true;
            this.checkHotkeysGuardianBarkskinPercent.Location = new System.Drawing.Point(300, 72);
            this.checkHotkeysGuardianBarkskinPercent.Name = "checkHotkeysGuardianBarkskinPercent";
            this.checkHotkeysGuardianBarkskinPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianBarkskinPercent.TabIndex = 9;
            this.checkHotkeysGuardianBarkskinPercent.Text = "50";
            // 
            // checkHotkeysGuardianBarkskinPercentLabel
            // 
            this.checkHotkeysGuardianBarkskinPercentLabel.AutoSize = true;
            this.checkHotkeysGuardianBarkskinPercentLabel.Location = new System.Drawing.Point(321, 74);
            this.checkHotkeysGuardianBarkskinPercentLabel.Name = "checkHotkeysGuardianBarkskinPercentLabel";
            this.checkHotkeysGuardianBarkskinPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianBarkskinPercentLabel.TabIndex = 9;
            this.checkHotkeysGuardianBarkskinPercentLabel.Text = "% HP";
            // 
            // checkHotkeysGuardianSI
            // 
            this.checkHotkeysGuardianSI.AutoSize = true;
            this.checkHotkeysGuardianSI.Location = new System.Drawing.Point(151, 94);
            this.checkHotkeysGuardianSI.Name = "checkHotkeysGuardianSI";
            this.checkHotkeysGuardianSI.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysGuardianSI.TabIndex = 8;
            this.checkHotkeysGuardianSI.Text = "Survival Instincts";
            this.checkHotkeysGuardianSI.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysGuardianSIPercent
            // 
            this.checkHotkeysGuardianSIPercent.AutoSize = true;
            this.checkHotkeysGuardianSIPercent.Location = new System.Drawing.Point(300, 94);
            this.checkHotkeysGuardianSIPercent.Name = "checkHotkeysGuardianSIPercent";
            this.checkHotkeysGuardianSIPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianSIPercent.TabIndex = 9;
            this.checkHotkeysGuardianSIPercent.Text = "50";
            // 
            // checkHotkeysGuardianSIPercentLabel
            // 
            this.checkHotkeysGuardianSIPercentLabel.AutoSize = true;
            this.checkHotkeysGuardianSIPercentLabel.Location = new System.Drawing.Point(321, 96);
            this.checkHotkeysGuardianSIPercentLabel.Name = "checkHotkeysGuardianSIPercentLabel";
            this.checkHotkeysGuardianSIPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianSIPercentLabel.TabIndex = 9;
            this.checkHotkeysGuardianSIPercentLabel.Text = "% HP";
            // 
            // checkHotkeysGuardianFReg
            // 
            this.checkHotkeysGuardianFReg.AutoSize = true;
            this.checkHotkeysGuardianFReg.Location = new System.Drawing.Point(151, 116);
            this.checkHotkeysGuardianFReg.Name = "checkHotkeysGuardianFReg";
            this.checkHotkeysGuardianFReg.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysGuardianFReg.TabIndex = 8;
            this.checkHotkeysGuardianFReg.Text = "Frenzied Regeneration";
            this.checkHotkeysGuardianFReg.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysGuardianFRegPercent
            // 
            this.checkHotkeysGuardianFRegPercent.AutoSize = true;
            this.checkHotkeysGuardianFRegPercent.Location = new System.Drawing.Point(300, 116);
            this.checkHotkeysGuardianFRegPercent.Name = "checkHotkeysGuardianFRegPercent";
            this.checkHotkeysGuardianFRegPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianFRegPercent.TabIndex = 9;
            this.checkHotkeysGuardianFRegPercent.Text = "50";
            // 
            // checkHotkeysGuardianFRegPercentLabel
            // 
            this.checkHotkeysGuardianFRegPercentLabel.AutoSize = true;
            this.checkHotkeysGuardianFRegPercentLabel.Location = new System.Drawing.Point(321, 118);
            this.checkHotkeysGuardianFRegPercentLabel.Name = "checkHotkeysGuardianFRegPercentLabel";
            this.checkHotkeysGuardianFRegPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianFRegPercentLabel.TabIndex = 9;
            this.checkHotkeysGuardianFRegPercentLabel.Text = "% HP";
            //
            // checkHotkeysGuardianBristlingFur
            // 
            this.checkHotkeysGuardianBristlingFur.AutoSize = true;
            this.checkHotkeysGuardianBristlingFur.Location = new System.Drawing.Point(151, 138);
            this.checkHotkeysGuardianBristlingFur.Name = "checkHotkeysGuardianBristlingFur";
            this.checkHotkeysGuardianBristlingFur.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysGuardianBristlingFur.TabIndex = 8;
            this.checkHotkeysGuardianBristlingFur.Text = "Bristling Fur";
            this.checkHotkeysGuardianBristlingFur.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysGuardianBristlingFurPercent
            // 
            this.checkHotkeysGuardianBristlingFurPercent.AutoSize = true;
            this.checkHotkeysGuardianBristlingFurPercent.Location = new System.Drawing.Point(300, 138);
            this.checkHotkeysGuardianBristlingFurPercent.Name = "checkHotkeysGuardianBristlingFurPercent";
            this.checkHotkeysGuardianBristlingFurPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianBristlingFurPercent.TabIndex = 9;
            this.checkHotkeysGuardianBristlingFurPercent.Text = "50";
            // 
            // checkHotkeysGuardianBristlingFurPercentLabel
            // 
            this.checkHotkeysGuardianBristlingFurPercentLabel.AutoSize = true;
            this.checkHotkeysGuardianBristlingFurPercentLabel.Location = new System.Drawing.Point(321, 140);
            this.checkHotkeysGuardianBristlingFurPercentLabel.Name = "checkHotkeysGuardianBristlingFurPercentLabel";
            this.checkHotkeysGuardianBristlingFurPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianBristlingFurPercentLabel.TabIndex = 9;
            this.checkHotkeysGuardianBristlingFurPercentLabel.Text = "% HP";
            //
            // checkHotkeysGuardianPulverize
            // 
            this.checkHotkeysGuardianPulverize.AutoSize = true;
            this.checkHotkeysGuardianPulverize.Location = new System.Drawing.Point(151, 160);
            this.checkHotkeysGuardianPulverize.Name = "checkHotkeysGuardianPulverize";
            this.checkHotkeysGuardianPulverize.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysGuardianPulverize.TabIndex = 8;
            this.checkHotkeysGuardianPulverize.Text = "Pulverize";
            this.checkHotkeysGuardianPulverize.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysGuardianPulverizePercent
            // 
            this.checkHotkeysGuardianPulverizePercent.AutoSize = true;
            this.checkHotkeysGuardianPulverizePercent.Location = new System.Drawing.Point(300, 160);
            this.checkHotkeysGuardianPulverizePercent.Name = "checkHotkeysGuardianPulverizePercent";
            this.checkHotkeysGuardianPulverizePercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianPulverizePercent.TabIndex = 9;
            this.checkHotkeysGuardianPulverizePercent.Text = "50";
            // 
            // checkHotkeysGuardianPulverizePercentLabel
            // 
            this.checkHotkeysGuardianPulverizePercentLabel.AutoSize = true;
            this.checkHotkeysGuardianPulverizePercentLabel.Location = new System.Drawing.Point(321, 162);
            this.checkHotkeysGuardianPulverizePercentLabel.Name = "checkHotkeysGuardianPulverizePercentLabel";
            this.checkHotkeysGuardianPulverizePercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysGuardianPulverizePercentLabel.TabIndex = 9;
            this.checkHotkeysGuardianPulverizePercentLabel.Text = "% HP";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.checkHotkeysGuardianIncarnation);
            this.groupBox13.Controls.Add(this.btnHotkeysGuardianOffensiveCooldowns);
            this.groupBox13.Location = new System.Drawing.Point(8, 8);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(556, 90);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Offensive Cooldowns";
            // 
            // checkHotkeysGuardianIncarnation
            // 
            this.checkHotkeysGuardianIncarnation.AutoSize = true;
            this.checkHotkeysGuardianIncarnation.Location = new System.Drawing.Point(151, 32);
            this.checkHotkeysGuardianIncarnation.Name = "checkHotkeysGuardianIncarnation";
            this.checkHotkeysGuardianIncarnation.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysGuardianIncarnation.TabIndex = 2;
            this.checkHotkeysGuardianIncarnation.Text = "Incarnation";
            this.checkHotkeysGuardianIncarnation.UseVisualStyleBackColor = true;		
            // 
            // btnHotkeysGuardianOffensiveCooldowns
            // 
            this.btnHotkeysGuardianOffensiveCooldowns.Location = new System.Drawing.Point(18, 28);
            this.btnHotkeysGuardianOffensiveCooldowns.Name = "btnHotkeysGuardianOffensiveCooldowns";
            this.btnHotkeysGuardianOffensiveCooldowns.Size = new System.Drawing.Size(113, 23);
            this.btnHotkeysGuardianOffensiveCooldowns.TabIndex = 1;
            this.btnHotkeysGuardianOffensiveCooldowns.Text = "Click to Set";
            this.btnHotkeysGuardianOffensiveCooldowns.UseVisualStyleBackColor = true;
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
Spell,8921,Moonfire,D2
Spell,213771,Swipe,D3
Spell,77758,Thrash,D4
Spell,33917,Mangle,D5
Spell,6807,Maul,D6
Spell,192081,Ironfur,NumPad5
Spell,5487,Bear Form,NumPad9
Spell,204066,Lunar Beam,D8
Spell,200851,Rage of the Sleeper,D7
Spell,22812,Barkskin,NumPad1
Spell,61336,Survival Instincts,NumPad2
Spell,22842,Frenzied Regeneration,NumPad6
Spell,155835,Bristling Fur,NumPad7
Spell,80313,Pulverize,NumPad8
Spell,102558,Incarnation,D9
Aura,164812,Moonfire
Aura,192090,Thrash
Aura,93622,Gore
Aura,213708,Galactic Guardian
Aura,5487,Bear Form
Aura,213680,Guardian of Elune
Aura,211160,Natural Defenses
Aura,61336,Survival Instincts
Aura,22812,Barkskin
Aura,192081,Ironfur
Aura,22842,Frenzied Regeneration
Aura,102558,Incarnation
Aura,155835,Bristling Fur
Aura,158792,Pulverize
Aura,783,Travel Form
*/
