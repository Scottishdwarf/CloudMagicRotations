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
    public class DestroWLsmartie : CombatRoutine
    {
        public override string Name
        {
            get 
			{ 
				return "Destruction"; 
			}
        }
        public override string Class
        {
            get 
			{ 
				return "Warlock"; 
			}
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
				return 4; 
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
                var cooldownKey = ConfigFile.ReadValue("DestroWL", "cooldownKey").Trim();
                if (cooldownKey != "")
                {
                    return Convert.ToInt32(cooldownKey);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DestroWL", "cooldownKey", value.ToString()); }
        }
        public static int cooldownModifier
        {
            get
            {
                var cooldownModifier = ConfigFile.ReadValue("DestroWL", "cooldownModifier").Trim();
                if (cooldownModifier != "")
                {
                    return Convert.ToInt32(cooldownModifier);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DestroWL", "cooldownModifier", value.ToString()); }
        }
        public static string cooldownHotKeyString
        {
            get
            {
                var cooldownHotKeyString = ConfigFile.ReadValue("DestroWL", "cooldownHotKeyString").Trim();

                if (cooldownHotKeyString != "")
                {
                    return cooldownHotKeyString;
                }

                return "Set Havoc Key";
            }
            set { ConfigFile.WriteValue("DestroWL", "cooldownHotKeyString", value); }
        }
        public static bool ischeckHotkeysDestroHS
        {
            get
            {
                var ischeckHotkeysDestroHS = ConfigFile.ReadValue("DestroWL", "ischeckHotkeysDestroHS").Trim();

                if (ischeckHotkeysDestroHS != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysDestroHS);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DestroWL", "ischeckHotkeysDestroHS", value.ToString()); }
        }
        public static bool ischeckHotkeyslegy1
        {
            get
            {
                var ischeckHotkeyslegy1 = ConfigFile.ReadValue("DestroWL", "ischeckHotkeyslegy1").Trim();

                if (ischeckHotkeyslegy1 != "")
                {
                    return Convert.ToBoolean(ischeckHotkeyslegy1);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DestroWL", "ischeckHotkeyslegy1", value.ToString()); }
        }		
        public static bool ischeckHotkeyslegy2
        {
            get
            {
                var ischeckHotkeyslegy2 = ConfigFile.ReadValue("DestroWL", "ischeckHotkeyslegy2").Trim();

                if (ischeckHotkeyslegy2 != "")
                {
                    return Convert.ToBoolean(ischeckHotkeyslegy2);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DestroWL", "ischeckHotkeyslegy2", value.ToString()); }
        }
        public static int DestroHSPercent
        {
            get
            {
                var DestroHSPercent = ConfigFile.ReadValue("DestroWL", "DestroHSPercent").Trim();
                if (DestroHSPercent != "")
                {
                    return Convert.ToInt32(DestroHSPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DestroWL", "DestroHSPercent", value.ToString()); }
        }
        public static bool ischeckHotkeysDestroUnRE
        {
            get
            {
                var ischeckHotkeysDestroUnRE = ConfigFile.ReadValue("DestroWL", "ischeckHotkeysDestroUnRE").Trim();

                if (ischeckHotkeysDestroUnRE != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysDestroUnRE);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DestroWL", "ischeckHotkeysDestroUnRE", value.ToString()); }
        }
        public static int DestroUnREPercent
        {
            get
            {
                var DestroUnREPercent = ConfigFile.ReadValue("DestroWL", "DestroUnREPercent").Trim();
                if (DestroUnREPercent != "")
                {
                    return Convert.ToInt32(DestroUnREPercent);
                }

                return -1;
            }
            set { ConfigFile.WriteValue("DestroWL", "DestroUnREPercent", value.ToString()); }
        }	
        public static bool ischeckHotkeysDestroBigPet
        {
            get
            {
                var ischeckHotkeysDestroBigPet = ConfigFile.ReadValue("DestroWL", "ischeckHotkeysDestroBigPet").Trim();

                if (ischeckHotkeysDestroBigPet != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysDestroBigPet);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DestroWL", "ischeckHotkeysDestroBigPet", value.ToString()); }
        }
        public static bool ischeckHotkeysDestroService
        {
            get
            {
                var ischeckHotkeysDestroService = ConfigFile.ReadValue("DestroWL", "ischeckHotkeysDestroService").Trim();

                if (ischeckHotkeysDestroService != "")
                {
                    return Convert.ToBoolean(ischeckHotkeysDestroService);
                }

                return true;
            }
            set { ConfigFile.WriteValue("DestroWL", "ischeckHotkeysDestroService", value.ToString()); }
        }
        public override void Initialize()
        {
            Log.Write("Welcome to the Destro Warlock v1.0 by smartie", Color.Green);
	        Log.Write("All Talents supported and auto detected", Color.Green);
	        Log.Write("Supported Legendarys: Shoulders, Boots and Trinket - if you need any more added pls pm me", Color.Yellow);	
	        Log.Write("Set your Havoc Key in the Setting", Color.Red);
	        Log.Write("You have to make a mouseover macro for Havoc to work and bind this macro in Keybinds", Color.Red);
			Log.Write("#showtooltip Havoc /cast [@mouseover,harm] Havoc; [harm] Havoc", Color.Red);
            SettingsFormDFF = new SettingsFormDFF();
            SettingsForm = SettingsFormDFF;

            SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.Text = cooldownHotKeyString;
            SettingsFormDFF.checkHotkeysDestroBigPet.Checked = ischeckHotkeysDestroBigPet;
            SettingsFormDFF.checkHotkeysDestroService.Checked = ischeckHotkeysDestroService;
            SettingsFormDFF.checkHotkeysDestroUnRE.Checked = ischeckHotkeysDestroUnRE;
            SettingsFormDFF.checkHotkeysDestroUnREPercent.Text = DestroUnREPercent.ToString();
            SettingsFormDFF.checkHotkeysDestroHS.Checked = ischeckHotkeysDestroHS;
            SettingsFormDFF.checkHotkeysDestroHSPercent.Text = DestroHSPercent.ToString();
            SettingsFormDFF.checkHotkeyslegy1.Checked = ischeckHotkeyslegy1;
            SettingsFormDFF.checkHotkeyslegy2.Checked = ischeckHotkeyslegy2;
            SettingsFormDFF.checkHotkeyslegy1.CheckedChanged += ischeckHotkeyslegy1_Click;
            SettingsFormDFF.checkHotkeyslegy2.CheckedChanged += ischeckHotkeyslegy2_Click;
            SettingsFormDFF.checkHotkeysDestroUnRE.CheckedChanged += ischeckHotkeysDestroUnRE_Click;
            SettingsFormDFF.checkHotkeysDestroUnREPercent.TextChanged += ischeckHotkeysDestroUnREPercent_Click;
            SettingsFormDFF.checkHotkeysDestroHS.CheckedChanged += ischeckHotkeysDestroHS_Click;
            SettingsFormDFF.checkHotkeysDestroHSPercent.TextChanged += ischeckHotkeysDestroHSPercent_Click;
            SettingsFormDFF.checkHotkeysDestroBigPet.CheckedChanged += ischeckHotkeysDestroBigPet_Click;
            SettingsFormDFF.checkHotkeysDestroService.CheckedChanged += ischeckHotkeysDestroService_Click;
            SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.KeyDown += KeyDown;
        }
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey)
                return;
            SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.Text = "Havoc : ";
            if (e.Shift)
            {
                cooldownModifier = (int)Keys.ShiftKey;
                SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.Text += Keys.Shift + " + ";
            }
            else if (e.Alt)
            {
                cooldownModifier = (int)Keys.Menu;
                SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.Text += Keys.Alt + " + ";
            }
            else if (e.Control)
            {
                cooldownModifier = (int)Keys.ControlKey;
                SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.Text += Keys.Control + " + ";
            }
            else cooldownModifier = -1;
            cooldownKey = (int)e.KeyCode;
            SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.Text += e.KeyCode;
            cooldownHotKeyString = SettingsFormDFF.btnHotkeysDestroOffensiveCooldowns.Text;
            SettingsFormDFF.checkHotkeysDestroHSPercentLabel.Focus();
        }	
        private void ischeckHotkeysDestroUnRE_Click(object sender, EventArgs e)
        {
            ischeckHotkeysDestroUnRE = SettingsFormDFF.checkHotkeysDestroUnRE.Checked;
        }
        private void ischeckHotkeysDestroUnREPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysDestroUnREPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                DestroUnREPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysDestroUnREPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }	
        private void ischeckHotkeysDestroHS_Click(object sender, EventArgs e)
        {
            ischeckHotkeysDestroHS = SettingsFormDFF.checkHotkeysDestroHS.Checked;
        }	
        private void ischeckHotkeysDestroHSPercent_Click(object sender, EventArgs e)
        {
            int userVal;
            if (int.TryParse(SettingsFormDFF.checkHotkeysDestroHSPercent.Text, out userVal) && userVal >= 0 && userVal <= 100)
            {
                DestroHSPercent = userVal;
            }
            else
            {
                SettingsFormDFF.checkHotkeysDestroHSPercent.Text = "";
                Log.Write("Enter a number between 0 and 100 in the text box", Color.DarkRed);
            }
        }	
        private void ischeckHotkeyslegy1_Click(object sender, EventArgs e)
        {
            ischeckHotkeyslegy1 = SettingsFormDFF.checkHotkeyslegy1.Checked;
        }	
        private void ischeckHotkeyslegy2_Click(object sender, EventArgs e)
        {
            ischeckHotkeyslegy2 = SettingsFormDFF.checkHotkeyslegy2.Checked;
        }
        private void ischeckHotkeysDestroBigPet_Click(object sender, EventArgs e)
        {
            ischeckHotkeysDestroBigPet = SettingsFormDFF.checkHotkeysDestroBigPet.Checked;
        }
        private void ischeckHotkeysDestroService_Click(object sender, EventArgs e)
        {
            ischeckHotkeysDestroService = SettingsFormDFF.checkHotkeysDestroService.Checked;
        }
        public override void Stop()
        {
        }
        public override void Pulse()
        {
			//Log.Write("Mana :" + WoW.Mana, Color.Green);
			if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting && WoW.TargetIsVisible && !WoW.IsMounted)
            {
				if (WoW.CanCast("UnendingResolve") && ischeckHotkeysDestroUnRE && WoW.HealthPercent <= DestroUnREPercent && WoW.HealthPercent != 0)
				{
					WoW.CastSpell("UnendingResolve");
					return;
				}
				if (WoW.CanCast("Healthstone") && ischeckHotkeysDestroHS && WoW.ItemCount("Healthstone") >= 1 && !WoW.ItemOnCooldown("Healthstone") && WoW.HealthPercent <= DestroHSPercent && WoW.HealthPercent != 0)
				{
					WoW.CastSpell("Healthstone");
					return;
				}
				if (WoW.CanCast("DoomGuard") && ischeckHotkeysDestroBigPet && !WoW.IsSpellOnCooldown("DoomGuard") && WoW.Talent (6) != 1 && WoW.PlayerHasDebuff("Lord of Flames") && (WoW.PlayerHasBuff("Soul Harvest") && WoW.Talent (4) == 3 || WoW.Talent (4) != 3) && UseCooldowns)
                {
                    WoW.CastSpell("DoomGuard");
                    return;
                }
				if (WoW.CanCast("Infernal") && ischeckHotkeysDestroBigPet && !WoW.IsSpellOnCooldown("Infernal") && WoW.Talent (6) != 1 && !WoW.PlayerHasDebuff("Lord of Flames") && (WoW.PlayerHasBuff("Soul Harvest") && WoW.Talent (4) == 3 || WoW.Talent (4) != 3) && UseCooldowns)
                {
                    WoW.CastSpell("Infernal");
                    return;
                }
				if (WoW.CanCast("Soul Harvest") && !WoW.IsMoving && UseCooldowns && !WoW.IsSpellOnCooldown("Soul Harvest") && WoW.CurrentSoulShards >= 4 && WoW.Talent (4) == 3)
				{
					WoW.CastSpell("Soul Harvest");
                    return;
                }
				if (WoW.CanCast("Berserk") && UseCooldowns && !WoW.IsSpellOnCooldown ("Berserk") && WoW.PlayerRace == "Troll")
                {
                    WoW.CastSpell("Berserk");
                    return;
                }
				if (WoW.CanCast("Havoc") && !WoW.IsSpellOnCooldown("Havoc") && DetectKeyPress.GetKeyState(cooldownKey) < 0 && (cooldownModifier == -1 || cooldownModifier != -1 && DetectKeyPress.GetKeyState(cooldownModifier) < 0)) // you will need a mouseover macro
				{
					WoW.CastSpell("Havoc");
					return;
				}
				if (UseCooldowns && !WoW.ItemOnCooldown("KBW") && ischeckHotkeyslegy2)
				{
					WoW.CastSpell("KBW") ;
					return;
				}
				if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave)
				{
					if (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon's Foresight"))
					{
						if (WoW.CanCast("Life Tap") && !WoW.PlayerHasBuff("Life Tap") && WoW.HealthPercent >= 40 && WoW.Talent (2) == 3)
						{
							WoW.CastSpell("Life Tap");
							return;						
						}
						if (WoW.CanCast("Life Tap") && WoW.Mana <= 40 && WoW.HealthPercent >= 40 && WoW.Talent (2) != 3)
						{
							WoW.CastSpell("Life Tap");
							return;						
						}
						if (!WoW.WasLastCasted("Immolate") && WoW.CanCast("Immolate") && (!WoW.TargetHasDebuff("Immolate") || WoW.TargetDebuffTimeRemaining("Immolate") <= 420))
						{
							WoW.CastSpell("Immolate");
							return;
						}
						if (WoW.CanCast("DimRift") && !ischeckHotkeyslegy1 && WoW.PlayerSpellCharges("DimRift") == 3)
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("DimRift") && ischeckHotkeyslegy1 && WoW.PlayerSpellCharges("DimRift") >= 1 && !WoW.PlayerHasBuff("Lessons of Space-Time"))
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("Channel Demonfire") && !WoW.IsSpellOnCooldown("Channel Demonfire") && WoW.TargetHasDebuff("Immolate") && WoW.TargetDebuffTimeRemaining("Immolate") >= 500 && WoW.Talent (7) == 2 )
						{
							WoW.CastSpell("Channel Demonfire");
							return;	
						}
						if (WoW.TargetDebuffTimeRemaining("Immolate") >= 1000 && WoW.CanCast("Conflagrate"))
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
						if (WoW.TargetDebuffTimeRemaining("Immolate") >= 1000 && WoW.PlayerSpellCharges("Conflagrate") == 1 && WoW.WasLastCasted("Conflagrate") && WoW.CanCast("Conflagrate"))
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
						if (WoW.PlayerHasBuff("Conflagrate") && WoW.TargetHasDebuff("ChaosBolt") && WoW.CanCast("Conflagrate") && WoW.CurrentSoulShards <= 4 && WoW.CanCast("Conflagrate"))
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
						if (WoW.CanCast("Conflagrate") && WoW.PlayerSpellCharges("Conflagrate") == 2 && !WoW.WasLastCasted("Immolate") && WoW.CurrentSoulShards <= 4)
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
						if (WoW.CanCast("ServiceImp") && ischeckHotkeysDestroService && WoW.CurrentSoulShards >= 1 && WoW.Talent (6) == 2 && (UseCooldowns && WoW.PlayerHasBuff("Soul Harvest") && WoW.Talent (4) == 3 || !UseCooldowns || WoW.Talent (4) != 3 && UseCooldowns))
						{
							WoW.CastSpell("ServiceImp");
							return;
						}
						if (WoW.CanCast("ChaosBolt") && WoW.CurrentSoulShards > 3 && (UseCooldowns && WoW.SpellCooldownTimeRemaining("Soul Harvest") >= 500 && WoW.Talent (4) == 3 || UseCooldowns && WoW.PlayerHasBuff("Soul Harvest") && WoW.Talent (4) == 3 || !UseCooldowns || WoW.Talent (4) != 3 && UseCooldowns))
						{
							WoW.CastSpell("ChaosBolt");
							return;
						}
						if (WoW.CanCast("DimRift") && !ischeckHotkeyslegy1 && !WoW.IsSpellOnCooldown("DimRift") && WoW.PlayerSpellCharges("DimRift") <= 2)
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("ChaosBolt") && WoW.CurrentSoulShards >= 2 && (UseCooldowns && WoW.SpellCooldownTimeRemaining("Soul Harvest") >= 500 && WoW.Talent (4) == 3 || UseCooldowns && WoW.PlayerHasBuff("Soul Harvest") && WoW.Talent (4) == 3 || !UseCooldowns || WoW.Talent (4) != 3 && UseCooldowns))
						{
							WoW.CastSpell("ChaosBolt");
							return;
						}
						if (WoW.CanCast("Incinerate") && WoW.CurrentSoulShards <= 1 || (UseCooldowns && WoW.SpellCooldownTimeRemaining("Soul Harvest") <= 500 && WoW.CurrentSoulShards < 4 && WoW.Talent (4) == 3))
						{
							WoW.CastSpell("Incinerate");
							return;
						}
						if (WoW.CanCast("Incinerate") && WoW.TargetHasDebuff("ChaosBolt") && WoW.TargetDebuffTimeRemaining("ChaosBolt") >= 200 && WoW.CurrentSoulShards <= 3)
						{
							WoW.CastSpell("Incinerate");
							return;
						}
					}
					if (WoW.IsMoving && !WoW.PlayerHasBuff("Norgannon's Foresight"))
					{
						if (WoW.CanCast("DimRift") && !ischeckHotkeyslegy1)
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("Conflagrate"))
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
					}
                }
				if (combatRoutine.Type == RotationType.AOE)
				{
					if (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon's Foresight"))
					{
						if (WoW.CanCast("Life Tap") && !WoW.PlayerHasBuff("Life Tap") && WoW.HealthPercent >= 40 && WoW.Talent (2) == 3)
						{
							WoW.CastSpell("Life Tap");
							return;						
						}
						if (WoW.CanCast("Life Tap") && WoW.Mana <= 40 && WoW.HealthPercent >= 40 && WoW.Talent (2) != 3)
						{
							WoW.CastSpell("Life Tap");
							return;						
						}
						if ((!WoW.TargetHasDebuff("Immolate") || WoW.TargetDebuffTimeRemaining("Immolate") <= 420) && !WoW.WasLastCasted("Immolate") && WoW.CanCast("Immolate"))
						{
							WoW.CastSpell("Immolate");
							return;
						}
						if (WoW.CanCast("DimRift") && !ischeckHotkeyslegy1 && WoW.PlayerSpellCharges("DimRift") == 3)
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("DimRift") && ischeckHotkeyslegy1 && WoW.PlayerSpellCharges("DimRift") >= 1 && !WoW.PlayerHasBuff("Lessons of Space-Time"))
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("Channel Demonfire") && !WoW.IsSpellOnCooldown("Channel Demonfire") && WoW.TargetHasDebuff("Immolate") && WoW.TargetDebuffTimeRemaining("Immolate") >= 500 && WoW.Talent (7) == 2 )
						{
							WoW.CastSpell("Channel Demonfire");
							return;	
						}
						if (WoW.CanCast("Rain of Fire") && WoW.CurrentSoulShards >= 3)
						{
							WoW.CastSpell("Rain of Fire");
							return;
						}
						if (WoW.TargetDebuffTimeRemaining("Immolate") >= 1000 && WoW.CanCast("Conflagrate"))
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
						if (WoW.TargetDebuffTimeRemaining("Immolate") >= 1000 && WoW.PlayerSpellCharges("Conflagrate") == 1 && WoW.WasLastCasted("Conflagrate") && WoW.CanCast("Conflagrate"))
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
						if (WoW.CanCast("Conflagrate") && WoW.PlayerSpellCharges("Conflagrate") == 2 && !WoW.WasLastCasted("Immolate") && WoW.CurrentSoulShards <= 4)
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
						if (WoW.CanCast("ServiceImp") && ischeckHotkeysDestroService && WoW.CurrentSoulShards >= 1 && WoW.Talent (6) == 2 && (UseCooldowns && WoW.PlayerHasBuff("Soul Harvest") && WoW.Talent (4) == 3 || !UseCooldowns || WoW.Talent (4) != 3 && UseCooldowns))
						{
							WoW.CastSpell("ServiceImp");
							return;
						}
						if (WoW.CanCast("DimRift") && !ischeckHotkeyslegy1 && !WoW.IsSpellOnCooldown("DimRift") && WoW.PlayerSpellCharges("DimRift") <= 2)
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("Incinerate") && WoW.CurrentSoulShards <= 1 || (UseCooldowns && WoW.SpellCooldownTimeRemaining("Soul Harvest") <= 500 && WoW.CurrentSoulShards < 4 && WoW.Talent (4) == 3))
						{
							WoW.CastSpell("Incinerate");
							return;
						}
						if (WoW.CanCast("Incinerate") && WoW.CurrentSoulShards <= 3)
						{
							WoW.CastSpell("Incinerate");
							return;
						}
					}
					if (WoW.IsMoving && !WoW.PlayerHasBuff("Norgannon's Foresight"))
					{
						if (WoW.CanCast("Rain of Fire") && WoW.CurrentSoulShards >= 3)
						{
							WoW.CastSpell("Rain of Fire");
							return;
						}
						if (WoW.CanCast("DimRift") && !ischeckHotkeyslegy1)
						{
							WoW.CastSpell("DimRift");
							return;
						}
						if (WoW.CanCast("Conflagrate"))
						{
							WoW.CastSpell("Conflagrate");
							return;
						}
					}	
				}
            }
		}
	}
    public class SettingsFormDFF : Form
    {
        public Button btnHotkeysDestroOffensiveCooldowns;
        public CheckBox checkHotkeysDestroUnRE;
        public TextBox checkHotkeysDestroUnREPercent;
        private readonly Label checkHotkeysDestroUnREPercentLabel;
        public CheckBox checkHotkeysDestroHS;
        public TextBox checkHotkeysDestroHSPercent;
        public Label checkHotkeysDestroHSPercentLabel;
        public CheckBox checkHotkeyslegy1;
        public CheckBox checkHotkeyslegy2;
        public CheckBox checkHotkeysDestroBigPet;
        public CheckBox checkHotkeysDestroService;
		
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
            this.checkHotkeyslegy1 = new System.Windows.Forms.CheckBox();
            this.checkHotkeyslegy2 = new System.Windows.Forms.CheckBox();
            this.checkHotkeysDestroUnRE = new System.Windows.Forms.CheckBox();
            this.checkHotkeysDestroUnREPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysDestroUnREPercentLabel = new System.Windows.Forms.Label();
            this.checkHotkeysDestroHS = new System.Windows.Forms.CheckBox();
            this.checkHotkeysDestroHSPercent = new System.Windows.Forms.TextBox();
            this.checkHotkeysDestroHSPercentLabel = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.checkHotkeysDestroBigPet = new System.Windows.Forms.CheckBox();
            this.checkHotkeysDestroService = new System.Windows.Forms.CheckBox();
            this.btnHotkeysDestroOffensiveCooldowns = new System.Windows.Forms.Button();
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
            this.tabPage5.Text = "Settings";
            this.tabPage5.UseVisualStyleBackColor = true;

            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.checkHotkeysDestroUnRE);
            this.groupBox12.Controls.Add(this.checkHotkeysDestroUnREPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysDestroUnREPercentLabel);
            this.groupBox12.Controls.Add(this.checkHotkeysDestroHS);
            this.groupBox12.Controls.Add(this.checkHotkeysDestroHSPercent);
            this.groupBox12.Controls.Add(this.checkHotkeysDestroHSPercentLabel);
            this.groupBox12.Location = new System.Drawing.Point(8, 100);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(561, 80);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Defensive Cooldowns";
			
            // 
            // checkHotkeysDestroUnRE
            // 
            this.checkHotkeysDestroUnRE.AutoSize = true;
            this.checkHotkeysDestroUnRE.Location = new System.Drawing.Point(151, 50);
            this.checkHotkeysDestroUnRE.Name = "checkHotkeysDestroUnRE";
            this.checkHotkeysDestroUnRE.Size = new System.Drawing.Size(104, 17);
            this.checkHotkeysDestroUnRE.TabIndex = 8;
            this.checkHotkeysDestroUnRE.Text = "Unending Resolve";
            this.checkHotkeysDestroUnRE.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysDestroUnREPercent
            // 
            this.checkHotkeysDestroUnREPercent.AutoSize = true;
            this.checkHotkeysDestroUnREPercent.Location = new System.Drawing.Point(300, 50);
            this.checkHotkeysDestroUnREPercent.Name = "checkHotkeysDestroUnREPercent";
            this.checkHotkeysDestroUnREPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysDestroUnREPercent.TabIndex = 9;
            this.checkHotkeysDestroUnREPercent.Text = "50";
            // 
            // checkHotkeysDestroUnREPercentLabel
            // 
            this.checkHotkeysDestroUnREPercentLabel.AutoSize = true;
            this.checkHotkeysDestroUnREPercentLabel.Location = new System.Drawing.Point(321, 52);
            this.checkHotkeysDestroUnREPercentLabel.Name = "checkHotkeysDestroUnREPercentLabel";
            this.checkHotkeysDestroUnREPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysDestroUnREPercentLabel.TabIndex = 9;
            this.checkHotkeysDestroUnREPercentLabel.Text = "% HP";
            // 
            // checkHotkeysDestroHS
            // 
            this.checkHotkeysDestroHS.AutoSize = true;
            this.checkHotkeysDestroHS.Location = new System.Drawing.Point(151, 28);
            this.checkHotkeysDestroHS.Name = "checkHotkeysDestroHS";
            this.checkHotkeysDestroHS.Size = new System.Drawing.Size(100, 28);
            this.checkHotkeysDestroHS.TabIndex = 9;
            this.checkHotkeysDestroHS.Text = "Healthstone";
            this.checkHotkeysDestroHS.UseVisualStyleBackColor = true;
            // 
            // checkHotkeysDestroHSPercent
            // 
            this.checkHotkeysDestroHSPercent.AutoSize = true;
            this.checkHotkeysDestroHSPercent.Location = new System.Drawing.Point(300, 28);
            this.checkHotkeysDestroHSPercent.Name = "checkHotkeysDestroHSPercent";
            this.checkHotkeysDestroHSPercent.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysDestroHSPercent.TabIndex = 9;
            this.checkHotkeysDestroHSPercent.Text = "50";
            // 
            // checkHotkeysDestroHSPercentLabel
            // 
            this.checkHotkeysDestroHSPercentLabel.AutoSize = true;
            this.checkHotkeysDestroHSPercentLabel.Location = new System.Drawing.Point(321, 30);
            this.checkHotkeysDestroHSPercentLabel.Name = "checkHotkeysDestroHSPercentLabel";
            this.checkHotkeysDestroHSPercentLabel.Size = new System.Drawing.Size(20, 28);
            this.checkHotkeysDestroHSPercentLabel.TabIndex = 9;
            this.checkHotkeysDestroHSPercentLabel.Text = "% HP";

            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.checkHotkeysDestroService);
            this.groupBox13.Controls.Add(this.checkHotkeysDestroBigPet);
            this.groupBox13.Controls.Add(this.btnHotkeysDestroOffensiveCooldowns);
            this.groupBox13.Controls.Add(this.checkHotkeyslegy1);
            this.groupBox13.Controls.Add(this.checkHotkeyslegy2);
            this.groupBox13.Location = new System.Drawing.Point(8, 8);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(556, 90);
            this.groupBox13.TabIndex = 3;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Offensive Cooldowns";
			
            // 
            // checkHotkeysDestroBigPet
            // 
            this.checkHotkeysDestroBigPet.AutoSize = true;
            this.checkHotkeysDestroBigPet.Location = new System.Drawing.Point(151, 32);
            this.checkHotkeysDestroBigPet.Name = "checkHotkeysDestroBigPet";
            this.checkHotkeysDestroBigPet.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeysDestroBigPet.TabIndex = 2;
            this.checkHotkeysDestroBigPet.Text = "DoomGuard/Infernal";
            this.checkHotkeysDestroBigPet.UseVisualStyleBackColor = true;

            // 
            // checkHotkeysDestroService
            // 
            this.checkHotkeysDestroService.AutoSize = true;
            this.checkHotkeysDestroService.Location = new System.Drawing.Point(151, 60);
            this.checkHotkeysDestroService.Name = "checkHotkeysDestroService";
            this.checkHotkeysDestroService.Size = new System.Drawing.Size(48, 17);
            this.checkHotkeysDestroService.TabIndex = 3;
            this.checkHotkeysDestroService.Text = "Call Service Imp";
            this.checkHotkeysDestroService.UseVisualStyleBackColor = true;

            // 
            // checkHotkeyslegy1
            // 
            this.checkHotkeyslegy1.AutoSize = true;
            this.checkHotkeyslegy1.Location = new System.Drawing.Point(300, 32);
            this.checkHotkeyslegy1.Name = "checkHotkeyslegy1";
            this.checkHotkeyslegy1.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeyslegy1.TabIndex = 4;
            this.checkHotkeyslegy1.Text = "Legendary Shoulders";
            this.checkHotkeyslegy1.UseVisualStyleBackColor = true;
			
            // 
            // checkHotkeyslegy2
            // 
            this.checkHotkeyslegy2.AutoSize = true;
            this.checkHotkeyslegy2.Location = new System.Drawing.Point(300, 60);
            this.checkHotkeyslegy2.Name = "checkHotkeyslegy2";
            this.checkHotkeyslegy2.Size = new System.Drawing.Size(99, 17);
            this.checkHotkeyslegy2.TabIndex = 5;
            this.checkHotkeyslegy2.Text = "Legendary Trinket";
            this.checkHotkeyslegy2.UseVisualStyleBackColor = true;
			
            // 
            // btnHotkeysDestroOffensiveCooldowns
            // 
            this.btnHotkeysDestroOffensiveCooldowns.Location = new System.Drawing.Point(18, 28);
            this.btnHotkeysDestroOffensiveCooldowns.Name = "btnHotkeysDestroOffensiveCooldowns";
            this.btnHotkeysDestroOffensiveCooldowns.Size = new System.Drawing.Size(113, 23);
            this.btnHotkeysDestroOffensiveCooldowns.TabIndex = 1;
            this.btnHotkeysDestroOffensiveCooldowns.Text = "Set Havoc Key";
            this.btnHotkeysDestroOffensiveCooldowns.UseVisualStyleBackColor = true;
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
            this.Text = "Destro WL by smartie";
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
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,29722,Incinerate,D1
Spell,348,Immolate,D3
Spell,17962,Conflagrate,D4
Spell,116858,ChaosBolt,D2
Spell,18540,DoomGuard,F1
Spell,196586,DimRift,F2
Spell,111859,ServiceImp,F3
Spell,104773,UnendingResolve,F5
Spell,80240,Havoc,F5
Spell,234153,DrainLife,F2
Spell,26297,Berserk,D0
Spell,5512,Healthstone,NumPad1
Spell,196098,Soul Harvest,D9
Spell,196447,Channel Demonfire,D7
Spell,1454,Life Tap,F6
Spell,5740,Rain of Fire,D6
Spell,235991,KBW,NumPad3
Spell,1122,Infernal,F11
Aura,235156,Life Tap
Aura,196098,Soul Harvest
Aura,157736,Immolate
Aura,196414,ChaosBolt
Aura,196546,Conflagrate
Aura,236176,Lessons of Space-Time
Aura,208215,Norgannon's Foresight
Aura,226802,Lord of Flames
Item,5512,Healthstone
Item,144259,KBW
*/
