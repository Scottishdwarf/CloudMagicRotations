//test@test.com
// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CloudMagic.Helpers;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace CloudMagic.Rotation
{
	
	public class SubRogueVectarius : CombatRoutine
    {
		
		
		private NumericUpDown nudCloakOfShadowsPercentValue;
		private NumericUpDown nudEvasionPercentValue;
		private NumericUpDown nudPlaceholder1PercentValue;
		private NumericUpDown nudKickPercentValue;
		private NumericUpDown nudPlaceholder2PercentValue;	
		private NumericUpDown nudPotionPercentValue;		
		

		private readonly Stopwatch pullwatch = new Stopwatch();
		private readonly Stopwatch DfAwatch = new Stopwatch();

		



        private bool BL
        {
            get
            {
				if (WoW.PlayerHasBuff("Bloodlust") || WoW.PlayerHasBuff("Ancient Hysteria") || WoW.PlayerHasBuff("Netherwinds") || WoW.PlayerHasBuff("Drums") || WoW.PlayerHasBuff("Heroism") || WoW.PlayerHasBuff("Time Warp"))
				{
					return true;
				}
				else
					return false;
            }
        }
	
		
        private bool stealth
        {
            get
            {
				if (WoW.PlayerHasBuff("Stealth") || WoW.PlayerHasBuff("ShadowDance") || WoW.PlayerHasBuff("Vanish") || WoW.PlayerHasBuff("Subterfuge") )
				{
					return true;
				}
				else
					return false;
            }
        }	
//variable,name=dsh_dfa,value=talent.death_from_above.enabled&talent.dark_shadow.enabled&spell_targets.death_from_above<4
        private bool dshdfa
        {
            get
            {
				if (WoW.Talent(7) == 3 && WoW.Talent(6) ==1)
				{
					return true;
				}
				else
					return false;
            }
        }	
//MantleDuration
        private float MantleDuration
        {
            get
            {
				return (WoW.PlayerBuffTimeRemaining("MasterAssassinsInitiative"));


            }
        }
	
		 private float GCD
        {
            get
            {

                    return (150f / (1f + (WoW.HastePercent / 100f)));

            }
        }
		
        private bool DfAready
        {
            get
            {
				if (WoW.CanCast("DeathFromAbove")&& WoW.IsSpellInRange("NightBlade")   && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") >= 1000))
				{
					return true;
				}
				else
					return false;
            }
        }	

		//Pet Control	
		private CheckBox MantleoftheMasterBox;
		private CheckBox Placeholder2Box;
		// Items
		private CheckBox KilJaedenBox;			
		private CheckBox PotionBox;
		private CheckBox PotBox;		

		// DEF cds
		private CheckBox CloakOfShadowsBox;
		private CheckBox Placeholder1Box;
		private CheckBox EvasionBox;	

		private CheckBox KickBox;		
		
		//dps cds
		private CheckBox T204pcBox;
		
		private static bool Pot
        {
            get
            {
                var Pot = ConfigFile.ReadValue("RogueSubtlety", "Pot").Trim();

                return Pot != "" && Convert.ToBoolean(Pot);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "Pot", value.ToString()); }
        }
		
		private static bool Potion
        {
            get
            {
                var Potion = ConfigFile.ReadValue("RogueSubtlety", "Potion").Trim();

                return Potion != "" && Convert.ToBoolean(Potion);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "Potion", value.ToString()); }
        }
		
		private static bool KilJaeden
        {
            get
            {
                var KilJaeden = ConfigFile.ReadValue("RogueSubtlety", "KilJaeden").Trim();

                return KilJaeden != "" && Convert.ToBoolean(KilJaeden);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "KilJaeden", value.ToString()); }
        }	
		
        private static bool MantleoftheMaster
        {
            get
            {
                var MantleoftheMaster= ConfigFile.ReadValue("RogueSubtlety", "MantleoftheMaster").Trim();

                return MantleoftheMaster != "" && Convert.ToBoolean(MantleoftheMaster);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "MantleoftheMaster", value.ToString()); }
        }
		
        private static bool Placeholder2
        {
            get
            {
                var Placeholder2 = ConfigFile.ReadValue("RogueSubtlety", "Placeholder2").Trim();

                return Placeholder2 != "" && Convert.ToBoolean(Placeholder2);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "Placeholder2", value.ToString()); }
        }		
		
		

		
        private static bool Kick
        {
            get
            {
                var Kick = ConfigFile.ReadValue("RogueSubtlety", "Kick").Trim();

                return Kick != "" && Convert.ToBoolean(Kick);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "Kick", value.ToString()); }
        }	
		
        private static bool CloakOfShadows
        {
            get
            {
                var CloakOfShadows = ConfigFile.ReadValue("RogueSubtlety", "CloakOfShadows").Trim();

                return CloakOfShadows != "" && Convert.ToBoolean(CloakOfShadows);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "CloakOfShadows", value.ToString()); }
        }	
		
        private static bool Placeholder1
        {
            get
            {
                var Placeholder1 = ConfigFile.ReadValue("RogueSubtlety", "Placeholder1").Trim();

                return Placeholder1 != "" && Convert.ToBoolean(Placeholder1);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "Placeholder1", value.ToString()); }
        }	

        private static bool Evasion
        {
            get
            {
                var Evasion = ConfigFile.ReadValue("RogueSubtlety", "Evasion").Trim();

                return Evasion != "" && Convert.ToBoolean(Evasion);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "Evasion", value.ToString()); }
        }				
		
        private static bool T204pc
        {
            get
            {
                var T204pc = ConfigFile.ReadValue("RogueSubtlety", "T204pc").Trim();

                return T204pc != "" && Convert.ToBoolean(T204pc);
            }
            set { ConfigFile.WriteValue("RogueSubtlety", "T204pc", value.ToString()); }
        }		
		

		

        
      public override string Name
        {
            get { return "Rogue Subtlety"; }
        }

        
		 public override string Class
        {
            get { return "Rogue"; }
        }

        public override Form SettingsForm { get; set; }
		
		
        public override void Initialize()
        {
			Log.Write("Talents: 1/1/1/3/2/1/3", Color.Green);

           
			if (ConfigFile.ReadValue("Rogue", "Evasion Percent") == "")
            {
                ConfigFile.WriteValue("Rogue", "Evasion Percent", "15");
            }
			if (ConfigFile.ReadValue("Rogue", "Placeholder1 Percent") == "")
            {
                ConfigFile.WriteValue("Rogue", "Placeholder1 Percent", "5");
            }
			if (ConfigFile.ReadValue("Rogue", "CloakOfShadows Percent") == "")
            {
                ConfigFile.WriteValue("Rogue", "CloakOfShadows Percent", "45");
            }
			if (ConfigFile.ReadValue("Rogue", "Kick Percent") == "")
            {
                ConfigFile.WriteValue("Rogue", "Kick Percent", "65");
            }	
			if (ConfigFile.ReadValue("Rogue", "Placeholder2 Percent") == "")
            {
                ConfigFile.WriteValue("Rogue", "Placeholder2 Percent", "80");
            }	
			if (ConfigFile.ReadValue("Rogue", "Potion Percent") == "")
            {
                ConfigFile.WriteValue("Rogue", "Potion Percent", "30");
            }			
		   
SettingsForm = new Form {Text = "Subtlety Rogue", StartPosition = FormStartPosition.CenterScreen, Width = 400, Height = 500, ShowIcon = false};

            nudEvasionPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Rogue", "Evasion Percent"), 
			Left = 215, 
			Top = 172,
			Size = new Size (40, 10)
			}; 
			SettingsForm.Controls.Add(nudEvasionPercentValue);
			
		

            nudCloakOfShadowsPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Rogue", "CloakOfShadows Percent"), 
			Left = 215, 
			Top = 122,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudCloakOfShadowsPercentValue);
			
			
		

            nudPlaceholder1PercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Rogue", "Placeholder1 Percent"), 
			Left = 215, 
			Top =147,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudPlaceholder1PercentValue);

            nudKickPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Rogue", "Kick Percent"), 
			Left = 215, 
			Top =100,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudKickPercentValue);			

            nudPlaceholder2PercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Rogue", "Placeholder2 Percent"), 
			Left = 215, 
			Top =272,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudPlaceholder2PercentValue);			

            nudPotionPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Rogue", "Potion Percent"), 
			Left = 215, 
			Top =347,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudPotionPercentValue);					
			
			


			
			var lblTitle = new Label
            {
                Text =
                    "Sub Rogue by Vectarius",
                Size = new Size(270, 14),
                Left = 61,
                Top = 1
	       };
			lblTitle.ForeColor = Color.Black;
			Font myFont = new Font(lblTitle.Font,FontStyle.Bold|FontStyle.Underline);
			lblTitle.Font = myFont;
            SettingsForm.Controls.Add(lblTitle);
			
			

			

			
			var lblTextBox3 = new Label
            {
                Text =
                    "Cooldowns",
                Size = new Size(200, 17),
                Left = 70,
                Top = 50
            };
			lblTextBox3.ForeColor = Color.Black;
			 SettingsForm.Controls.Add(lblTextBox3);

			 
			var lblT204pcBox = new Label
            {
                Text =
                    "T20 4pc",
                Size = new Size(270, 15),
                Left = 100,
                Top = 75
            };
			
			lblT204pcBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblT204pcBox);			
           
			var lblKickBox = new Label
            {
                Text =
                    "Kick @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 100
            };
			
			lblKickBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblKickBox);	

			var lblCloakOfShadowsBox = new Label
            {
                Text =
                    "Cloak of Shadows @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 125
            };
			
			lblCloakOfShadowsBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblCloakOfShadowsBox);	

			var lblEvasionBox = new Label
            {
                Text =
                    "Evasion @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 175
            };
			
			lblEvasionBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblEvasionBox);	

			var lblPlaceholder1Box = new Label
            {
                Text =
                    "Placeholder1 @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 150
            };
			
			lblPlaceholder1Box.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblPlaceholder1Box);		



					
			 
			var lblTextBox5 = new Label
            {
                Text =
                    "Pet Control",
                Size = new Size(200, 17),
                Left = 70,
                Top = 225
            };
			lblTextBox5.ForeColor = Color.Black;
			 SettingsForm.Controls.Add(lblTextBox5);			 

						var lblTextBox6 = new Label
            {
                Text =
                    "Items",
                Size = new Size(200, 17),
                Left = 70,
                Top = 300
            };
			lblTextBox6.ForeColor = Color.Black;
			 SettingsForm.Controls.Add(lblTextBox6);
			 



	

			var lblMantleoftheMasterBox = new Label
            {
                Text =
                    "MantleoftheMaster",
                Size = new Size(270, 15),
                Left = 100,
                Top = 250
            };
			
			lblMantleoftheMasterBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblMantleoftheMasterBox);	
			
			var lblPlaceholder2Box = new Label
            {
                Text =
                    "Placeholder2 @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 275
            };
			
			lblPlaceholder2Box.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblPlaceholder2Box);				

			var lblKilJaedenBox = new Label
            {
                Text =
                    "Kil'Jaeden's Burning Wish",
                Size = new Size(270, 15),
                Left = 100,
                Top = 325
            };
			
			lblKilJaedenBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblKilJaedenBox);	

			var lblPotionBox = new Label
            {
                Text =
                    "Healthstone/Potion @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 350
            };
			
			lblPotionBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblPotionBox);				
		   
			var lblPotBox = new Label
            {
                Text =
                    "Potion of Prolonged Power with Bloodlust",
                Size = new Size(270, 15),
                Left = 100,
                Top = 375
            };
			
			lblPotBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblPotBox);				
		   
			
			
			
			
			var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 5, Top = 400, Size = new Size(120, 31)};
			
			var cmdReadme = new Button {Text = "Macros! Use Them", Width = 65, Height = 25, Left = 125, Top = 400, Size = new Size(120, 31)};
			
 

//items
            KilJaedenBox = new CheckBox {Checked = KilJaeden, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 325};	
			SettingsForm.Controls.Add(KilJaedenBox);
            PotionBox = new CheckBox {Checked = Potion, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 350};				
            SettingsForm.Controls.Add(PotionBox);
            PotBox = new CheckBox {Checked = Pot, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 375};				
            SettingsForm.Controls.Add(PotBox);			
//pet control			
			MantleoftheMasterBox = new CheckBox {Checked = MantleoftheMaster, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 250};		
			Placeholder2Box = new CheckBox {Checked = Placeholder2, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 275};				
            SettingsForm.Controls.Add(MantleoftheMasterBox);
            SettingsForm.Controls.Add(Placeholder2Box);			
			
			// Checkboxes
            KickBox = new CheckBox {Checked = Kick, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 100};		
            SettingsForm.Controls.Add(KickBox);
			CloakOfShadowsBox = new CheckBox {Checked = CloakOfShadows, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 125};			
            SettingsForm.Controls.Add(CloakOfShadowsBox);
			Placeholder1Box = new CheckBox {Checked = Placeholder1, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 150};
            SettingsForm.Controls.Add(Placeholder1Box);
			
			EvasionBox = new CheckBox {Checked = Evasion, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 175};			
			            SettingsForm.Controls.Add(EvasionBox);		
			//dps cooldowns
            T204pcBox = new CheckBox {Checked = T204pc, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 75};
            SettingsForm.Controls.Add(T204pcBox);			

			
			
			KickBox.Checked = Kick;	
			CloakOfShadowsBox.Checked = CloakOfShadows;	
			Placeholder1Box.Checked = Placeholder1;	
			EvasionBox.Checked = Evasion;	
			
			T204pcBox.Checked = T204pc;

			

		
			
			//cmdSave

			
            KilJaedenBox.CheckedChanged += KilJaeden_Click;    
			PotionBox.CheckedChanged += Potion_Click;  
			PotBox.CheckedChanged += Pot_Click; 			
            MantleoftheMasterBox.CheckedChanged += MantleoftheMaster_Click;		
            Placeholder2Box.CheckedChanged += Placeholder2_Click;				
			
            T204pcBox.CheckedChanged += T204pc_Click;    
            CloakOfShadowsBox.CheckedChanged += CloakOfShadows_Click; 
            KickBox.CheckedChanged += Kick_Click;
            Placeholder1Box.CheckedChanged += Placeholder1_Click;
            EvasionBox.CheckedChanged += Evasion_Click;	
			
			
			cmdSave.Click += CmdSave_Click;
			cmdReadme.Click += CmdReadme_Click;
 
			
			SettingsForm.Controls.Add(cmdSave);
			SettingsForm.Controls.Add(cmdReadme);
		
			lblTextBox5.BringToFront();		
			lblTextBox6.BringToFront();				
			lblTitle.BringToFront();

			nudCloakOfShadowsPercentValue.BringToFront();
			nudEvasionPercentValue.BringToFront();
			nudPlaceholder1PercentValue.BringToFront();		
			
            KilJaedenBox.BringToFront();
            PotionBox.BringToFront();
            PotBox.BringToFront();			
            MantleoftheMasterBox.BringToFront();	
            Placeholder2Box.BringToFront();				
			
            T204pcBox.BringToFront();	
            KickBox.BringToFront();	
            CloakOfShadowsBox.BringToFront();
            Placeholder1Box.BringToFront();
            EvasionBox.BringToFront();				
			

			
			
		}
			
			private void CmdSave_Click(object sender, EventArgs e)
        {


            KilJaeden = KilJaedenBox.Checked;	
            Potion = PotionBox.Checked;	
            Pot = PotionBox.Checked;				
            MantleoftheMaster = MantleoftheMasterBox.Checked;			
            Placeholder2 = Placeholder2Box.Checked;				
			
            T204pc = T204pcBox.Checked;		
            Kick = KickBox.Checked;	
            CloakOfShadows = CloakOfShadowsBox.Checked;
            Placeholder1 = Placeholder1Box.Checked;
            Evasion = EvasionBox.Checked;			
			
            ConfigFile.WriteValue("Rogue", "Evasion Percent", nudEvasionPercentValue.Value.ToString());
	        ConfigFile.WriteValue("Rogue", "Placeholder1 Percent", nudPlaceholder1PercentValue.Value.ToString());		
            ConfigFile.WriteValue("Rogue", "CloakOfShadows Percent", nudCloakOfShadowsPercentValue.Value.ToString());			
			
			
            MessageBox.Show("Settings saved.", "PixelMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsForm.Close();
        }
		private void CmdReadme_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                " make sure you make macros of Kill Command and Dire Frenzy/Beast /petattack",
                "PixelMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
	

		
//items
		private void KilJaeden_Click(object sender, EventArgs e)
        {
            KilJaeden = KilJaedenBox.Checked;
        }			

		private void Potion_Click(object sender, EventArgs e)
        {
            Potion = PotionBox.Checked;
        }	
		private void Pot_Click(object sender, EventArgs e)
        {
            Pot = PotBox.Checked;
        }		
//pet control			
		private void MantleoftheMaster_Click(object sender, EventArgs e)
        {
            MantleoftheMaster = MantleoftheMasterBox.Checked;
        }	
		private void Placeholder2_Click(object sender, EventArgs e)
        {
            Placeholder2 = Placeholder2Box.Checked;
        }			

			
		private void Kick_Click(object sender, EventArgs e)
        {
            Kick = KickBox.Checked;
        }			
			
        private void CloakOfShadows_Click(object sender, EventArgs e)
        {
            CloakOfShadows = CloakOfShadowsBox.Checked;
        }
        private void Placeholder1_Click(object sender, EventArgs e)
        {
            Placeholder1 = Placeholder1Box.Checked;
        }
        private void Evasion_Click(object sender, EventArgs e)
        {
            Evasion = EvasionBox.Checked;
        }			
			//dpscooldown
        private void T204pc_Click(object sender, EventArgs e)
        {
            T204pc = T204pcBox.Checked;
        }			

		
		
        public override void Stop()
        {
			
			
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
                return 1766; 
            }
        }
        private static bool lastNamePlate = true;
		public override int CLEAVE { get { return 99; } } //please Set between 1-99 NpC in range for AOE  if not desired set to 99
        public override int AOE { get { return 3; } }//please Set between 1-99 NpC in range for Cleave if not desired set to 99
        public override int SINGLE {get { return 1; } }//please Set between 1-99 NpC in range for ST if not desired set to 99    

        public override void Pulse()
        {

		
		if (DetectKeyPress.GetKeyState(0x6A) < 0)
            {
                UseCooldowns = !UseCooldowns;
                Thread.Sleep(150);
            }			
				/*if(WoW.IsInCombat && !WoW.HasTarget)
				{
				WoW.KeyPressRelease(WoW.Keys.Tab);
				return;
				}*/
					if (WoW.IsInCombat && !pullwatch.IsRunning)
					{
					pullwatch.Start();
					Log.Write("Starting Combat, Starting Pullwatch.", Color.Red);
                    
					}
					if (!WoW.IsInCombat && pullwatch.ElapsedMilliseconds > 10000)
					{
					pullwatch.Reset();
					Log.Write("Leaving Combat, Resetting Stopwatches.", Color.Red);
					
					}							
					if (WoW.IsInCombat && DfAwatch.ElapsedMilliseconds > 3000)
					{
					DfAwatch.Reset();
					Log.Write("DfA watch reset", Color.Red);
                    
					}									






			if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave)  
            {

                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted)
                {
					if((DfAwatch.IsRunning && DfAwatch.ElapsedMilliseconds < 2000) || WoW.PlayerSpellCharges("ShadowDance") == 0)
					{
						if (WoW.CanCast("ShadowDance"))
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}	
					}
						if(DfAwatch.ElapsedMilliseconds > 2000 || !DfAwatch.IsRunning)
					{
						
/* //shadow_dance,if=talent.dark_shadow.enabled&!stealthed.all&buff.death_from_above.up&buff.death_from_above.remains<=0.15			
				if (WoW.CanCast("ShadowDance") && (WoW.PlayerHasBuff("DeathFromAbove")|| WoW.LastSpell == "DeathFromAbove")&& !stealth && WoW.Talent(6)==1 )
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}
						*/
//shadow_dance,if=charges_fractional>=variable.shd_fractional|target.time_to_die<cooldown.symbols_of_death.remains		
				if (WoW.CanCast("ShadowDance") && WoW.PlayerSpellCharges("ShadowDance") >= 2 && !stealth && WoW.LastSpell != "ShadowDance" )
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}

                        if (WoW.CanCast("Kick") && WoW.Level >= 32 && Kick
							&& WoW.TargetIsCastingAndSpellIsInterruptible 
							&& WoW.TargetPercentCast >= ConfigFile.ReadValue<int>("Rogue", "Kick Percent") 
							&& !WoW.IsSpellOnCooldown("Kick") 
							&& !WoW.PlayerIsChanneling)
                        {
                            WoW.CastSpell("Kick");						
                            return;
                        }
						if (WoW.CanCast("Kil'jaeden's Burning Wish") && KilJaeden && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish") && !WoW.IsSpellOnCooldown("Kil'jaeden's Burning Wish"))  
						{
							WoW.CastSpell("Kil'jaeden's Burning Wish");
							return;
						}
						if (WoW.CanCast("CloakOfShadows") && WoW.Level >= 24 && WoW.HealthPercent <= ConfigFile.ReadValue<int>("Rogue", "CloakOfShadows Percent") && CloakOfShadows && !WoW.IsSpellOnCooldown("CloakOfShadows") && WoW.HealthPercent != 0)
						{
							WoW.CastSpell("CloakOfShadows");
							return;
						}	
						if (WoW.CanCast("Evasion") && WoW.Level >= 70&& WoW.HealthPercent <= ConfigFile.ReadValue<int>("Rogue", "Evasion Percent") && Evasion && !WoW.IsSpellOnCooldown("Evasion") && WoW.HealthPercent != 0)
						{
							WoW.CastSpell("Evasion");
							return;
						}						
//	death_from_above,if=!talent.dark_shadow.enabled|spell_targets>=4&buff.shadow_dance.up|spell_targets<4&!buff.shadow_dance.up&(buff.symbols_of_death.up|cooldown.symbols_of_death.remains>=10+set_bonus.tier20_4pc*5)
						if (WoW.CanCast("DeathFromAbove")&& WoW.Talent(7)==3&& WoW.IsSpellInRange("NightBlade") && (WoW.Talent(6) !=1 ||  !WoW.PlayerHasBuff("ShadowDance"))  && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") >= 1000))
						{
                            WoW.CastSpell("DeathFromAbove");
							DfAwatch.Restart();
							DfAwatch.Start();
							Log.Write("DfA watch started", Color.Red);							
                            return;
						} 						
//	call_action_list,name=cds	
						if (combatRoutine.UseCooldowns)
						{
//	use_item,name=vial_of_ceaseless_toxins,if=(!talent.dark_shadow.enabled|buff.shadow_dance.up)&(buff.symbols_of_death.up|(!talent.death_from_above.enabled&((mantle_duration>=3|!equipped.mantle_of_the_master_assassin)|cooldown.vanish.remains>=60)))
/*							if (WoW.CanCast("VOCT") && !WoW.IsSpellOnCooldown ("VOCT") && !stealth && !WoW.ItemOnCooldown ("VOCT") && (WoW.Talent(6)!=1 || WoW.PlayerHasBuff("ShadowDance")) && (WoW.PlayerHasBuff("SymbolsOfDeath") || (WoW.Talent(7) !=3 &&((MantleDuration >=300 || !MantleoftheMaster) || WoW.SpellCooldownTimeRemaining("Vanish") >=60))))
							{
								WoW.CastSpell("VOCT");
								return;
							}
*/							
							if (WoW.CanCast("Berserking") && !WoW.IsSpellOnCooldown ("Berserking") && WoW.PlayerRace == "Troll" && stealth)
							{
								WoW.CastSpell("Berserking");
								return;
							}	
							if (WoW.CanCast("Arcane Torrent") && !WoW.IsSpellOnCooldown ("Arcane Torrent") && WoW.PlayerRace == "BloodElf" && WoW.Energy <= 20)
							{
								WoW.CastSpell("Arcane Torrent");
								return;
							}	
							if (WoW.CanCast("Blood Fury") && stealth&& !WoW.IsSpellOnCooldown ("Blood Fury") && WoW.PlayerRace == "Orc")
							{
								WoW.CastSpell("Blood Fury");
								return;
							}							
//symbols_of_death,if=!talent.death_from_above.enabled&((time>10&energy.deficit>=40-stealthed.all*30)|(time<10&dot.nightblade.ticking))
							if(WoW.CanCast("SymbolsOfDeath") && WoW.Talent(6) !=1 && stealth && ((pullwatch.ElapsedMilliseconds > 10000 && WoW.Energy >=40) ||pullwatch.ElapsedMilliseconds < 10000 && WoW.TargetHasDebuff("NightBlade") ))
							{
							WoW.CastSpell("SymbolsOfDeath");
							return;
							}							
//symbols_of_death,if=(talent.death_from_above.enabled&cooldown.death_from_above.remains<=3&(dot.nightblade.remains>=cooldown.death_from_above.remains+3|target.time_to_die-dot.nightblade.remains<=6)&(time>=3|set_bonus.tier20_4pc))						
							if(WoW.CanCast("SymbolsOfDeath") && WoW.Talent(7) ==3 && stealth && WoW.SpellCooldownTimeRemaining("DeathFromAbove") <= 300 &&(WoW.TargetDebuffTimeRemaining("NightBlade") >= (WoW.SpellCooldownTimeRemaining("DeathFromAbove")+300)) && ((pullwatch.ElapsedMilliseconds >= 3000) || T204pc)  && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 )
							{
							WoW.CastSpell("SymbolsOfDeath");
							return;
							}
//	marked_for_death,target_if=min:target.time_to_die,if=target.time_to_die<combo_points.deficit
							
//shadow_blades,if=(time>10&combo_points.deficit>=2+stealthed.all-equipped.mantle_of_the_master_assassin)|(time<10&(!talent.marked_for_death.enabled|combo_points.deficit>=3|dot.nightblade.ticking))
							if (WoW.CanCast("ShadowBlades") && ((pullwatch.ElapsedMilliseconds > 10000 && WoW.CurrentComboPoints <=4 && stealth && MantleoftheMaster) || (pullwatch.ElapsedMilliseconds < 10000 &&(WoW.Talent(7) != 2 || WoW.CurrentComboPoints <=3 || WoW.TargetHasDebuff("Nightblade")))) && WoW.IsSpellInRange("NightBlade"))
							{
								WoW.CastSpell("ShadowBlades");
								return;
							}	
//goremaws_bite,if=!stealthed.all&cooldown.shadow_dance.charges_fractional<=variable.shd_fractional&((combo_points.deficit>=4-(time<10)*2&energy.deficit>50+talent.vigor.enabled*25-(time>=10)*15)|(combo_points.deficit>=1&target.time_to_die<8))	
							if (WoW.CanCast("GoremawsBite") && WoW.IsSpellInRange("NightBlade") && WoW.PlayerSpellCharges("ShadowDance") < 2&& !stealth && WoW.CurrentComboPoints <=2 && WoW.Energy <= 50)
							{
							WoW.CastSpell("GoremawsBite");
							return;
							}
//Marked for Death <= 1 Combo Point
							if (WoW.CanCast("MarkedForDeath") && WoW.Talent(7) ==2 && !WoW.IsSpellOnCooldown ("MarkedForDeath") && WoW.CurrentComboPoints <=1)
							{
								WoW.CastSpell("MarkedForDeath");
								return;
							}
//vanish,if=energy>=55-talent.shadow_focus.enabled*10&variable.dsh_dfa&(!equipped.mantle_of_the_master_assassin|buff.symbols_of_death.up)&cooldown.shadow_dance.charges_fractional<=variable.shd_fractional				&!buff.shadow_dance.up&!buff.stealth.up&mantle_duration=0&(dot.nightblade.remains>=cooldown.death_from_above.remains+6|target.time_to_die-dot.nightblade.remains<=6)&cooldown.death_from_above.remains<=1&(time<10|combo_points>=3)|target.time_to_die<=7						
							if (WoW.CanCast("Vanish") && WoW.IsSpellInRange("NightBlade") && (WoW.Energy >=50 || (WoW.Talent(2) ==3 && WoW.Energy >=45)) && dshdfa &&(!MantleoftheMaster|| WoW.PlayerHasBuff("SymbolsOfDeath")) && WoW.PlayerSpellCharges("ShadowDance") < 2 && !stealth && (WoW.TargetDebuffTimeRemaining("NightBlade") >= (WoW.SpellCooldownTimeRemaining("DeathFromAbove")+600)) && WoW.SpellCooldownTimeRemaining("DeathFromAbove") <=100 &&((pullwatch.ElapsedMilliseconds < 10000) || WoW.CurrentComboPoints>=3))
							{
								WoW.CastSpell("Vanish");
								return;
							}
						}							
						if (stealth)
					{
//combo_points>=5&(spell_targets.shuriken_storm>=3+equipped.shadow_satyrs_walk|(mantle_duration<=1.3&mantle_duration-gcd.remains>=0.3))
					/*	if (WoW.CurrentComboPoints >=5 )
						{
//nightblade,if=(!talent.dark_shadow.enabled|!buff.shadow_dance.up)&target.time_to_die-remains>6&(mantle_duration=0|remains<=mantle_duration)&((refreshable&(!finality|buff.finality_nightblade.up))|remains<tick_time*2)
						if (WoW.CanCast("NightBlade") && WoW.IsSpellInRange("NightBlade")&& WoW.Energy >= 25 && (WoW.Talent(6) !=1 || !WoW.PlayerHasBuff("ShadowDance")) && (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) ) 
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
//	death_from_above,if=!talent.dark_shadow.enabled|spell_targets>=4&buff.shadow_dance.up|spell_targets<4&!buff.shadow_dance.up&(buff.symbols_of_death.up|cooldown.symbols_of_death.remains>=10+set_bonus.tier20_4pc*5)
						if (WoW.CanCast("DeathFromAbove")&& WoW.IsSpellInRange("NightBlade") && WoW.Talent(6) !=1 && WoW.PlayerHasBuff("ShadowDance") && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") >= 1000))
						{
                            WoW.CastSpell("DeathFromAbove");
                            return;
						} 
//eviscerate
						if (WoW.CanCast("Eviscerate")  && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.TargetHasDebuff("NightBlade"))
						{	
							WoW.CastSpell("Eviscerate");
							return;
						}
						*/
//shadowstrike
						if (WoW.CanCast("ShadowStrike") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=40 && WoW.CurrentComboPoints <=5 )
						{
							WoW.CastSpell("ShadowStrike");
							return;
						}	
					}	
//	nightblade,if=target.time_to_die>6&remains<gcd.max&combo_points>=4-(time<10)*2
						if (WoW.CanCast("NightBlade") && WoW.Energy >= 25 && WoW.TargetDebuffTimeRemaining("NightBlade") < GCD && WoW.CurrentComboPoints >=4 && pullwatch.ElapsedMilliseconds < 10000) 
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
//finish,if=combo_points>=5+(talent.deeper_stratagem.enabled&!buff.shadow_blades.up&(mantle_duration=0|set_bonus.tier20_4pc))|(combo_points>=4&combo_points.deficit<=2&spell_targets.shuriken_storm>=3&spell_targets.shuriken_storm<=4)|(target.time_to_die<=1&combo_points>=3)						
						if((WoW.CurrentComboPoints >=5    &&(MantleDuration==0 || T204pc)))
					{
//nightblade,if=(!talent.dark_shadow.enabled|!buff.shadow_dance.up)&target.time_to_die-remains>6&(mantle_duration=0|remains<=mantle_duration)&((refreshable&(!finality|buff.finality_nightblade.up))|remains<tick_time*2)
						if (WoW.CanCast("NightBlade") && WoW.IsSpellInRange("NightBlade")&& WoW.Energy >= 25 && (WoW.Talent(6) !=1 || !WoW.PlayerHasBuff("ShadowDance")) && (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) ) 
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
//	death_from_above,if=!talent.dark_shadow.enabled|spell_targets>=4&buff.shadow_dance.up|spell_targets<4&!buff.shadow_dance.up&(buff.symbols_of_death.up|cooldown.symbols_of_death.remains>=10+set_bonus.tier20_4pc*5)
						if (WoW.CanCast("DeathFromAbove")&& WoW.Talent(7)==3&& WoW.Energy >=25 && WoW.IsSpellInRange("NightBlade") && (WoW.Talent(6) !=1 || !WoW.PlayerHasBuff("ShadowDance"))  && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") >= 1000))
						{
                            WoW.CastSpell("DeathFromAbove");
							DfAwatch.Restart();
							DfAwatch.Start();
							Log.Write("DfA watch started", Color.Red);									
							return;
						} 
//eviscerate
						if (WoW.CanCast("Eviscerate") && !DfAready && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.TargetHasDebuff("NightBlade"))
						{	
							WoW.CastSpell("Eviscerate");
							return;
						}
					}						
//backstab
					if (WoW.CanCast("Backstab") && WoW.CurrentComboPoints < 6 && WoW.Energy >= 35 && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("Backstab");
							return;
						}							
				}
			}

					}
				
            if (combatRoutine.Type == RotationType.AOE)
            {
	
                if (WoW.HasTarget && WoW.TargetIsEnemy  && !WoW.IsMounted)
                {
//shadow_dance,if=talent.dark_shadow.enabled&!stealthed.all&buff.death_from_above.up&buff.death_from_above.remains<=0.15			
				if (WoW.CanCast("ShadowDance") && (WoW.PlayerHasBuff("DeathFromAbove")|| WoW.LastSpell == "DeathFromAbove")&& !stealth && WoW.Talent(6)==1 )
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}
//shadow_dance,if=charges_fractional>=variable.shd_fractional|target.time_to_die<cooldown.symbols_of_death.remains		
				if (WoW.CanCast("ShadowDance") && WoW.PlayerSpellCharges("ShadowDance") <= 2 && !stealth && WoW.LastSpell != "ShadowDance" )
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}
//	death_from_above,if=!talent.dark_shadow.enabled|spell_targets>=4&buff.shadow_dance.up|spell_targets<4&!buff.shadow_dance.up&(buff.symbols_of_death.up|cooldown.symbols_of_death.remains>=10+set_bonus.tier20_4pc*5)
						if (WoW.CanCast("DeathFromAbove") && WoW.Talent(7)==3 && WoW.Energy >=25 && WoW.IsSpellInRange("NightBlade") && (WoW.Talent(6) !=1 || !WoW.PlayerHasBuff("ShadowDance"))  && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") >= 1000))
						{
                            WoW.CastSpell("DeathFromAbove");
							
							WoW.CastSpell("ShadowDance");
								if (WoW.CanCast("ShadowDance") && !WoW.IsSpellOnCooldown("ShadowDance") && !stealth )
								{
								WoW.CastSpell("ShadowDance");
									if (WoW.CanCast("ShadowDance") && !WoW.IsSpellOnCooldown("ShadowDance") && !stealth )
									{
									WoW.CastSpell("ShadowDance");
									
									}
								}
								return;
						}  						
//	call_action_list,name=cds	
						if (combatRoutine.UseCooldowns)
						{
//	use_item,name=vial_of_ceaseless_toxins,if=(!talent.dark_shadow.enabled|buff.shadow_dance.up)&(buff.symbols_of_death.up|(!talent.death_from_above.enabled&((mantle_duration>=3|!equipped.mantle_of_the_master_assassin)|cooldown.vanish.remains>=60)))
/*							if (WoW.CanCast("VOCT") && !WoW.IsSpellOnCooldown ("VOCT") && !stealth && !WoW.ItemOnCooldown ("VOCT") && (WoW.Talent(6)!=1 || WoW.PlayerHasBuff("ShadowDance")) && (WoW.PlayerHasBuff("SymbolsOfDeath") || (WoW.Talent(7) !=3 &&((MantleDuration >=300 || !MantleoftheMaster) || WoW.SpellCooldownTimeRemaining("Vanish") >=60))))
							{
								WoW.CastSpell("VOCT");
								return;
							}
*/							
							if (WoW.CanCast("Berserking") && !WoW.IsSpellOnCooldown ("Berserking") && WoW.PlayerRace == "Troll" && stealth)
							{
								WoW.CastSpell("Berserking");
								return;
							}	
							if (WoW.CanCast("Arcane Torrent") && !WoW.IsSpellOnCooldown ("Arcane Torrent") && WoW.PlayerRace == "BloodElf" && WoW.Energy <= 20)
							{
								WoW.CastSpell("Arcane Torrent");
								return;
							}	
							if (WoW.CanCast("Blood Fury") && stealth&& !WoW.IsSpellOnCooldown ("Blood Fury") && WoW.PlayerRace == "Orc")
							{
								WoW.CastSpell("Blood Fury");
								return;
							}
//symbols_of_death,if=!talent.death_from_above.enabled&((time>10&energy.deficit>=40-stealthed.all*30)|(time<10&dot.nightblade.ticking))
							if(WoW.CanCast("SymbolsOfDeath") && WoW.Talent(6) !=1 && stealth && ((pullwatch.ElapsedMilliseconds > 10000 && WoW.Energy >=40) ||pullwatch.ElapsedMilliseconds < 10000 && WoW.TargetHasDebuff("NightBlade") ))
							{
							WoW.CastSpell("SymbolsOfDeath");
							return;
							}								
//symbols_of_death,if=(talent.death_from_above.enabled&cooldown.death_from_above.remains<=3&(dot.nightblade.remains>=cooldown.death_from_above.remains+3|target.time_to_die-dot.nightblade.remains<=6)&(time>=3|set_bonus.tier20_4pc))						
							if(WoW.CanCast("SymbolsOfDeath") && WoW.Talent(7) ==3 && stealth && WoW.SpellCooldownTimeRemaining("DeathFromAbove") <= 300 &&(WoW.TargetDebuffTimeRemaining("NightBlade") >= (WoW.SpellCooldownTimeRemaining("DeathFromAbove")+300)) && ((pullwatch.ElapsedMilliseconds >= 3000) || T204pc)  && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 )
							{
							WoW.CastSpell("SymbolsOfDeath");
							return;
							}
//shadow_blades,if=(time>10&combo_points.deficit>=2+stealthed.all-equipped.mantle_of_the_master_assassin)|(time<10&(!talent.marked_for_death.enabled|combo_points.deficit>=3|dot.nightblade.ticking))
							if (WoW.CanCast("ShadowBlades") && ((pullwatch.ElapsedMilliseconds > 10000 && WoW.CurrentComboPoints <=4 && stealth && MantleoftheMaster) || (pullwatch.ElapsedMilliseconds < 10000 &&(WoW.Talent(7) != 2 || WoW.CurrentComboPoints <=3 || WoW.TargetHasDebuff("Nightblade")))) && WoW.IsSpellInRange("NightBlade"))
							{
								WoW.CastSpell("ShadowBlades");
								return;
							}	
//goremaws_bite,if=!stealthed.all&cooldown.shadow_dance.charges_fractional<=variable.shd_fractional&((combo_points.deficit>=4-(time<10)*2&energy.deficit>50+talent.vigor.enabled*25-(time>=10)*15)|(combo_points.deficit>=1&target.time_to_die<8))	
							if (WoW.CanCast("GoremawsBite") && WoW.IsSpellInRange("NightBlade") && WoW.PlayerSpellCharges("ShadowDance") < 2&& !stealth && WoW.CurrentComboPoints <=2 && WoW.Energy <= 50)
							{
							WoW.CastSpell("GoremawsBite");
							return;
							}
//Marked for Death <= 1 Combo Point
							if (WoW.CanCast("MarkedForDeath") && WoW.Talent(7) ==2 && !WoW.IsSpellOnCooldown ("MarkedForDeath") && WoW.CurrentComboPoints <=1)
							{
								WoW.CastSpell("MarkedForDeath");
								return;
							}							
//vanish,if=energy>=55-talent.shadow_focus.enabled*10&variable.dsh_dfa&(!equipped.mantle_of_the_master_assassin|buff.symbols_of_death.up)&cooldown.shadow_dance.charges_fractional<=variable.shd_fractional				&!buff.shadow_dance.up&!buff.stealth.up&mantle_duration=0&(dot.nightblade.remains>=cooldown.death_from_above.remains+6|target.time_to_die-dot.nightblade.remains<=6)&cooldown.death_from_above.remains<=1&(time<10|combo_points>=3)|target.time_to_die<=7						
							if (WoW.CanCast("Vanish") && WoW.IsSpellInRange("NightBlade") && (WoW.Energy >=50 || (WoW.Talent(2) ==3 && WoW.Energy >=45)) && dshdfa &&(!MantleoftheMaster|| WoW.PlayerHasBuff("SymbolsOfDeath")) && WoW.PlayerSpellCharges("ShadowDance") < 2 && !stealth && (WoW.TargetDebuffTimeRemaining("NightBlade") >= (WoW.SpellCooldownTimeRemaining("DeathFromAbove")+600)) && WoW.SpellCooldownTimeRemaining("DeathFromAbove") <=100 &&((pullwatch.ElapsedMilliseconds < 10000) || WoW.CurrentComboPoints>=3))
							{
								WoW.CastSpell("Vanish");
								return;
							}
						}							
						if (stealth)
					{
//combo_points>=5&(spell_targets.shuriken_storm>=3+equipped.shadow_satyrs_walk|(mantle_duration<=1.3&mantle_duration-gcd.remains>=0.3))
					/*	if (WoW.CurrentComboPoints >=5 )
						{
//nightblade,if=(!talent.dark_shadow.enabled|!buff.shadow_dance.up)&target.time_to_die-remains>6&(mantle_duration=0|remains<=mantle_duration)&((refreshable&(!finality|buff.finality_nightblade.up))|remains<tick_time*2)
						if (WoW.CanCast("NightBlade") && WoW.IsSpellInRange("NightBlade")&& WoW.Energy >= 25 && (WoW.Talent(6) !=1 || !WoW.PlayerHasBuff("ShadowDance")) && (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) ) 
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
//	death_from_above,if=!talent.dark_shadow.enabled|spell_targets>=4&buff.shadow_dance.up|spell_targets<4&!buff.shadow_dance.up&(buff.symbols_of_death.up|cooldown.symbols_of_death.remains>=10+set_bonus.tier20_4pc*5)
						if (WoW.CanCast("DeathFromAbove")&& WoW.IsSpellInRange("NightBlade") && WoW.Talent(6) !=1 && WoW.PlayerHasBuff("ShadowDance") && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") >= 1000))
						{
                            WoW.CastSpell("DeathFromAbove");
                            return;
						} 
//eviscerate
						if (WoW.CanCast("Eviscerate")  && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.TargetHasDebuff("NightBlade"))
						{	
							WoW.CastSpell("Eviscerate");
							return;
						}
						*/
//shadowstrike
						if (WoW.CanCast("ShurikenStorm") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=40 && WoW.CurrentComboPoints <=5 )
						{
							WoW.CastSpell("ShurikenStorm");
							return;
						}	
					}	
//	nightblade,if=target.time_to_die>6&remains<gcd.max&combo_points>=4-(time<10)*2
						if (WoW.CanCast("NightBlade") && WoW.Energy >= 25 && WoW.TargetDebuffTimeRemaining("NightBlade") < GCD && WoW.CurrentComboPoints >=4 && pullwatch.ElapsedMilliseconds < 10000) 
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
//finish,if=combo_points>=5+(talent.deeper_stratagem.enabled&!buff.shadow_blades.up&(mantle_duration=0|set_bonus.tier20_4pc))|(combo_points>=4&combo_points.deficit<=2&spell_targets.shuriken_storm>=3&spell_targets.shuriken_storm<=4)|(target.time_to_die<=1&combo_points>=3)						
						if((WoW.CurrentComboPoints >=5    &&(MantleDuration==0 || T204pc)))
					{
//nightblade,if=(!talent.dark_shadow.enabled|!buff.shadow_dance.up)&target.time_to_die-remains>6&(mantle_duration=0|remains<=mantle_duration)&((refreshable&(!finality|buff.finality_nightblade.up))|remains<tick_time*2)
						if (WoW.CanCast("NightBlade") && WoW.IsSpellInRange("NightBlade")&& WoW.Energy >= 25 && (WoW.Talent(6) !=1 || !WoW.PlayerHasBuff("ShadowDance")) && (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) ) 
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
//	death_from_above,if=!talent.dark_shadow.enabled|spell_targets>=4&buff.shadow_dance.up|spell_targets<4&!buff.shadow_dance.up&(buff.symbols_of_death.up|cooldown.symbols_of_death.remains>=10+set_bonus.tier20_4pc*5)
						if (WoW.CanCast("DeathFromAbove")&& WoW.Talent(7)==3&& WoW.Energy >=25 && WoW.IsSpellInRange("NightBlade") && (WoW.Talent(6) !=1  || !WoW.PlayerHasBuff("ShadowDance"))  && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") >= 1000))
						{
                            WoW.CastSpell("DeathFromAbove");
							
							WoW.CastSpell("ShadowDance");
								if (WoW.CanCast("ShadowDance") && !WoW.IsSpellOnCooldown("ShadowDance") && !stealth )
								{
								WoW.CastSpell("ShadowDance");
									if (WoW.CanCast("ShadowDance") && !WoW.IsSpellOnCooldown("ShadowDance") && !stealth )
									{
									WoW.CastSpell("ShadowDance");
									
									}
								}
								return;
						} 
//eviscerate
						if (WoW.CanCast("Eviscerate") && !DfAready && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.TargetHasDebuff("NightBlade"))
						{	
							WoW.CastSpell("Eviscerate");
							return;
						}
					}						
//backstab
					if (WoW.CanCast("ShurikenStorm") && WoW.CurrentComboPoints < 6 && WoW.Energy >= 35 && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("ShurikenStorm");
							return;
						}							
				}
				
            }
		}
	}

}




	

/*
[AddonDetails.db]
AddonAuthor=Vectarius
AddonName=myspellpriority
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,152150,DeathFromAbove,D5
Spell,31224,CloakOfShadows,D0
Spell,5277,Evasion,F
Spell,147011,VOCT,D0
Spell,185311,CrimsonVial,F3
Spell,212283,SymbolsOfDeath,D2
Spell,185438,ShadowStrike,D1
Spell,185313,ShadowDance,H
Spell,195452,NightBlade,D3
Spell,196819,Eviscerate,D4
Spell,209782,GoremawsBite,T
Spell,1856,Vanish,O
Spell,121471,ShadowBlades,G
Spell,1766,Kick,D6
Spell,53,Backstab,D1
Spell,197835,ShurikenStorm,D8
Spell,115191,Stealth,R
Spell,26297,Berserking,F2
Spell,25046,Arcane Torrent,F2
Spell,20572,Blood Fury,F2
Spell,137619,MarkedForDeath,D6
Spell,144259,Kil'jaeden's Burning Wish,NumPad4 
Aura,197496,Eviscerate
Aura,121471,ShadowBlades
Aura,212283,SymbolsOfDeath
Aura,152150,DeathFromAbove
Aura,235027,MasterAssassinsInitiative
Aura,115191,Stealth
Aura,115192,Subterfuge
Aura,185422,ShadowDance
Aura,195452,NightBlade
Aura,11327,Vanish
Aura,5277,Evasion
Aura,31224,CloakOfShadows
Item,147011,VOCT
*/
