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
	
	public class BMHunterVectarius : CombatRoutine
    {
		
		
		private NumericUpDown nudExhilarationPercentValue;
		private NumericUpDown nudAspectoftheTurtlePercentValue;
		private NumericUpDown nudFeignDeathPercentValue;
		private NumericUpDown nudKickPercentValue;
		private NumericUpDown nudIntimidationPercentValue;	
		private NumericUpDown nudPotionPercentValue;		
		
		private readonly Stopwatch tacticswatch = new Stopwatch();
		private readonly Stopwatch mongoosebitewatch = new Stopwatch();
		private readonly Stopwatch pullwatch = new Stopwatch();
		private readonly Stopwatch Direfrenzywatch = new Stopwatch();
		



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
		private float AimedShotCastTime
        {
            get
            {
                    return 200f / (1f + (WoW.HastePercent / 100f));
            }
        }
		
		 private float GCD
        {
            get
            {

                    return (150f / (1f + (WoW.HastePercent / 100f)));

            }
        }
			
private float FocusRegen
{
     get
     {
         return (10f* (1f + (WoW.HastePercent / 100f)));
     }
}



		//Pet Control	
		private CheckBox HealPetBox;
		private CheckBox IntimidationBox;
		// Items
		private CheckBox KilJaedenBox;			
		private CheckBox PotionBox;
		private CheckBox PotBox;		

		// DEF cds
		private CheckBox ExhilarationBox;
		private CheckBox FeignDeathBox;
		private CheckBox AspectoftheTurtleBox;	

		private CheckBox KickBox;		
		
		//dps cds
		private CheckBox FrizzosBox;
		
		private static bool Pot
        {
            get
            {
                var Pot = ConfigFile.ReadValue("HunterBeastmastery", "Pot").Trim();

                return Pot != "" && Convert.ToBoolean(Pot);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Pot", value.ToString()); }
        }
		
		private static bool Potion
        {
            get
            {
                var Potion = ConfigFile.ReadValue("HunterBeastmastery", "Potion").Trim();

                return Potion != "" && Convert.ToBoolean(Potion);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Potion", value.ToString()); }
        }
		
		private static bool KilJaeden
        {
            get
            {
                var KilJaeden = ConfigFile.ReadValue("HunterBeastmastery", "KilJaeden").Trim();

                return KilJaeden != "" && Convert.ToBoolean(KilJaeden);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "KilJaeden", value.ToString()); }
        }	
		
        private static bool HealPet
        {
            get
            {
                var HealPet = ConfigFile.ReadValue("HunterBeastmastery", "HealPet").Trim();

                return HealPet != "" && Convert.ToBoolean(HealPet);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "HealPet", value.ToString()); }
        }
		
        private static bool Intimidation
        {
            get
            {
                var Intimidation = ConfigFile.ReadValue("HunterBeastmastery", "Intimidation").Trim();

                return Intimidation != "" && Convert.ToBoolean(Intimidation);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Intimidation", value.ToString()); }
        }		
		
		

		
        private static bool Kick
        {
            get
            {
                var Kick = ConfigFile.ReadValue("HunterBeastmastery", "Kick").Trim();

                return Kick != "" && Convert.ToBoolean(Kick);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Kick", value.ToString()); }
        }	
		
        private static bool Exhilaration
        {
            get
            {
                var Exhilaration = ConfigFile.ReadValue("HunterBeastmastery", "Exhilaration").Trim();

                return Exhilaration != "" && Convert.ToBoolean(Exhilaration);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Exhilaration", value.ToString()); }
        }	
		
        private static bool FeignDeath
        {
            get
            {
                var FeignDeath = ConfigFile.ReadValue("HunterBeastmastery", "FeignDeath").Trim();

                return FeignDeath != "" && Convert.ToBoolean(FeignDeath);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "FeignDeath", value.ToString()); }
        }	

        private static bool AspectoftheTurtle
        {
            get
            {
                var AspectoftheTurtle = ConfigFile.ReadValue("HunterBeastmastery", "AspectoftheTurtle").Trim();

                return AspectoftheTurtle != "" && Convert.ToBoolean(AspectoftheTurtle);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "AspectoftheTurtle", value.ToString()); }
        }				
		
        private static bool Frizzos
        {
            get
            {
                var Frizzos = ConfigFile.ReadValue("HunterBeastmastery", "Frizzos").Trim();

                return Frizzos != "" && Convert.ToBoolean(Frizzos);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Frizzos", value.ToString()); }
        }		
		

		

        
      public override string Name
        {
            get { return "Hunter Beast Mastery"; }
        }

        
		 public override string Class
        {
            get { return "Hunter"; }
        }

        public override Form SettingsForm { get; set; }
		
		
        public override void Initialize()
        {
			Log.Write("Auto AoE optimized for WQs", Color.Green);	
           
			if (ConfigFile.ReadValue("Hunter", "AspectoftheTurtle Percent") == "")
            {
                ConfigFile.WriteValue("Hunter", "AspectoftheTurtle Percent", "15");
            }
			if (ConfigFile.ReadValue("Hunter", "FeignDeath Percent") == "")
            {
                ConfigFile.WriteValue("Hunter", "FeignDeath Percent", "5");
            }
			if (ConfigFile.ReadValue("Hunter", "Exhilaration Percent") == "")
            {
                ConfigFile.WriteValue("Hunter", "Exhilaration Percent", "45");
            }
			if (ConfigFile.ReadValue("Hunter", "Kick Percent") == "")
            {
                ConfigFile.WriteValue("Hunter", "Kick Percent", "65");
            }	
			if (ConfigFile.ReadValue("Hunter", "Intimidation Percent") == "")
            {
                ConfigFile.WriteValue("Hunter", "Intimidation Percent", "80");
            }	
			if (ConfigFile.ReadValue("Hunter", "Potion Percent") == "")
            {
                ConfigFile.WriteValue("Hunter", "Potion Percent", "30");
            }			
		   
SettingsForm = new Form {Text = "Beast Mastery Hunter", StartPosition = FormStartPosition.CenterScreen, Width = 400, Height = 500, ShowIcon = false};

            nudAspectoftheTurtlePercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Hunter", "AspectoftheTurtle Percent"), 
			Left = 215, 
			Top = 172,
			Size = new Size (40, 10)
			}; 
			SettingsForm.Controls.Add(nudAspectoftheTurtlePercentValue);
			
		

            nudExhilarationPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Hunter", "Exhilaration Percent"), 
			Left = 215, 
			Top = 122,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudExhilarationPercentValue);
			
			
		

            nudFeignDeathPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Hunter", "FeignDeath Percent"), 
			Left = 215, 
			Top =147,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudFeignDeathPercentValue);

            nudKickPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Hunter", "Kick Percent"), 
			Left = 215, 
			Top =100,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudKickPercentValue);			

            nudIntimidationPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Hunter", "Intimidation Percent"), 
			Left = 215, 
			Top =272,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudIntimidationPercentValue);			

            nudPotionPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Hunter", "Potion Percent"), 
			Left = 215, 
			Top =347,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudPotionPercentValue);					
			
			


			
			var lblTitle = new Label
            {
                Text =
                    "BM Hunter by Vectarius",
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

			 
			var lblFrizzosBox = new Label
            {
                Text =
                    "Frizzo's Fingertrap",
                Size = new Size(270, 15),
                Left = 100,
                Top = 75
            };
			
			lblFrizzosBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblFrizzosBox);			
           
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

			var lblExhilarationBox = new Label
            {
                Text =
                    "Exhilaration @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 125
            };
			
			lblExhilarationBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblExhilarationBox);	

			var lblAspectoftheTurtleBox = new Label
            {
                Text =
                    "Aspect of the Turtle @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 175
            };
			
			lblAspectoftheTurtleBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblAspectoftheTurtleBox);	

			var lblFeignDeathBox = new Label
            {
                Text =
                    "Feign Death @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 150
            };
			
			lblFeignDeathBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblFeignDeathBox);		



					
			 
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
			 



	

			var lblHealPetBox = new Label
            {
                Text =
                    "Heal Pet",
                Size = new Size(270, 15),
                Left = 100,
                Top = 250
            };
			
			lblHealPetBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblHealPetBox);	
			
			var lblIntimidationBox = new Label
            {
                Text =
                    "Intimidation @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 275
            };
			
			lblIntimidationBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblIntimidationBox);				

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
			HealPetBox = new CheckBox {Checked = HealPet, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 250};		
			IntimidationBox = new CheckBox {Checked = Intimidation, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 275};				
            SettingsForm.Controls.Add(HealPetBox);
            SettingsForm.Controls.Add(IntimidationBox);			
			
			// Checkboxes
            KickBox = new CheckBox {Checked = Kick, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 100};		
            SettingsForm.Controls.Add(KickBox);
			ExhilarationBox = new CheckBox {Checked = Exhilaration, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 125};			
            SettingsForm.Controls.Add(ExhilarationBox);
			FeignDeathBox = new CheckBox {Checked = FeignDeath, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 150};
            SettingsForm.Controls.Add(FeignDeathBox);
			
			AspectoftheTurtleBox = new CheckBox {Checked = AspectoftheTurtle, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 175};			
			            SettingsForm.Controls.Add(AspectoftheTurtleBox);		
			//dps cooldowns
            FrizzosBox = new CheckBox {Checked = Frizzos, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 75};
            SettingsForm.Controls.Add(FrizzosBox);			

			
			
			KickBox.Checked = Kick;	
			ExhilarationBox.Checked = Exhilaration;	
			FeignDeathBox.Checked = FeignDeath;	
			AspectoftheTurtleBox.Checked = AspectoftheTurtle;	
			
			FrizzosBox.Checked = Frizzos;

			

		
			
			//cmdSave

			
            KilJaedenBox.CheckedChanged += KilJaeden_Click;    
			PotionBox.CheckedChanged += Potion_Click;  
			PotBox.CheckedChanged += Pot_Click; 			
            HealPetBox.CheckedChanged += HealPet_Click;		
            IntimidationBox.CheckedChanged += Intimidation_Click;				
			
            FrizzosBox.CheckedChanged += Frizzos_Click;    
            ExhilarationBox.CheckedChanged += Exhilaration_Click; 
            KickBox.CheckedChanged += Kick_Click;
            FeignDeathBox.CheckedChanged += FeignDeath_Click;
            AspectoftheTurtleBox.CheckedChanged += AspectoftheTurtle_Click;	
			
			
			cmdSave.Click += CmdSave_Click;
			cmdReadme.Click += CmdReadme_Click;
 
			
			SettingsForm.Controls.Add(cmdSave);
			SettingsForm.Controls.Add(cmdReadme);
		
			lblTextBox5.BringToFront();		
			lblTextBox6.BringToFront();				
			lblTitle.BringToFront();

			nudExhilarationPercentValue.BringToFront();
			nudAspectoftheTurtlePercentValue.BringToFront();
			nudFeignDeathPercentValue.BringToFront();		
			
            KilJaedenBox.BringToFront();
            PotionBox.BringToFront();
            PotBox.BringToFront();			
            HealPetBox.BringToFront();	
            IntimidationBox.BringToFront();				
			
            FrizzosBox.BringToFront();	
            KickBox.BringToFront();	
            ExhilarationBox.BringToFront();
            FeignDeathBox.BringToFront();
            AspectoftheTurtleBox.BringToFront();				
			

			
			
		}
			
			private void CmdSave_Click(object sender, EventArgs e)
        {


            KilJaeden = KilJaedenBox.Checked;	
            Potion = PotionBox.Checked;	
            Pot = PotionBox.Checked;				
            HealPet = HealPetBox.Checked;			
            Intimidation = IntimidationBox.Checked;				
			
            Frizzos = FrizzosBox.Checked;		
            Kick = KickBox.Checked;	
            Exhilaration = ExhilarationBox.Checked;
            FeignDeath = FeignDeathBox.Checked;
            AspectoftheTurtle = AspectoftheTurtleBox.Checked;			
			
            ConfigFile.WriteValue("Hunter", "AspectoftheTurtle Percent", nudAspectoftheTurtlePercentValue.Value.ToString());
	        ConfigFile.WriteValue("Hunter", "FeignDeath Percent", nudFeignDeathPercentValue.Value.ToString());		
            ConfigFile.WriteValue("Hunter", "Exhilaration Percent", nudExhilarationPercentValue.Value.ToString());			
			
			
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
		private void HealPet_Click(object sender, EventArgs e)
        {
            HealPet = HealPetBox.Checked;
        }	
		private void Intimidation_Click(object sender, EventArgs e)
        {
            Intimidation = IntimidationBox.Checked;
        }			

			
		private void Kick_Click(object sender, EventArgs e)
        {
            Kick = KickBox.Checked;
        }			
			
        private void Exhilaration_Click(object sender, EventArgs e)
        {
            Exhilaration = ExhilarationBox.Checked;
        }
        private void FeignDeath_Click(object sender, EventArgs e)
        {
            FeignDeath = FeignDeathBox.Checked;
        }
        private void AspectoftheTurtle_Click(object sender, EventArgs e)
        {
            AspectoftheTurtle = AspectoftheTurtleBox.Checked;
        }			
			//dpscooldown
        private void Frizzos_Click(object sender, EventArgs e)
        {
            Frizzos = FrizzosBox.Checked;
        }			

		
		
        public override void Stop()
        {
			
			
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
				
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted)
				{
				     if (WoW.CanCast("Feign Death") && WoW.Level >= 28&& WoW.HealthPercent <= ConfigFile.ReadValue<int>("Hunter", "FeignDeath Percent") && FeignDeath && !WoW.IsSpellOnCooldown("Feign Death") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Feign Death");
                        return;
                    }
                    if (WoW.CanCast("Exhilaration") && WoW.Level >= 24 && WoW.HealthPercent <= ConfigFile.ReadValue<int>("Hunter", "Exhilaration Percent") && Exhilaration && !WoW.IsSpellOnCooldown("Exhilaration") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Exhilaration");
                        return;
                    }	
					if (WoW.CanCast("Aspect of the Turtle") && WoW.Level >= 70&& WoW.HealthPercent <= ConfigFile.ReadValue<int>("Hunter", "Aspect of the Turtle Percent") && AspectoftheTurtle && !WoW.IsSpellOnCooldown("Aspect of the Turtle") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Aspect of the Turtle");
                        return;
                    }

                    if (KilJaeden && WoW.CanCast("Kil'jaeden's Burning Wish") && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish"))
					{
						WoW.CastSpell("Kil'jaeden's Burning Wish") ;
						return;
					}					

					if (Potion && WoW.IsInCombat && WoW.HealthPercent < ConfigFile.ReadValue<int>("Hunter", "Potion Percent") && WoW.ItemCount("Healthstone") >= 1 && !WoW.ItemOnCooldown("Healthstone") && WoW.ItemCount("HealthPotion") == 0)
					{
                
                    WoW.CastSpell("Healthstone");
                    return;
					}				
                    if (Pot && BL && WoW.CanCast("Pot") && !WoW.PlayerHasBuff("Pot") && WoW.ItemCount("Pot") >= 1 && !WoW.ItemOnCooldown("Pot"))
					{
						WoW.CastSpell("Pot") ;
						return;
					}
if (WoW.PlayerSpec == "Marksmanship")
			{			
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted)
                {			

					if (WoW.CanCast("Counter Shot") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.TargetIsCasting
						&& WoW.TargetIsCastingAndSpellIsInterruptible 
							&& WoW.TargetPercentCast >= ConfigFile.ReadValue<int>("Hunter", "Kick Percent") 
						&& Kick)
                    {
                        WoW.CastSpell("Counter Shot");
                        return;
                    }				
					if (WoW.CanCast("Volley") 
						&& !WoW.PlayerHasBuff("Volley")
						&& WoW.Talent(6) == 3)
                    {
                        WoW.CastSpell("Volley");
                        return;
                    }	
					if (KilJaeden && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish"))  
                    {
                        WoW.CastSpell("Kil'jaeden's Burning Wish");
                        return;
                    }							
					if (WoW.CanCast("Arcane Torrent") 
						
						&& !WoW.IsSpellOnCooldown ("Arcane Torrent")
						&& WoW.PlayerHasBuff("Trueshot")
						&& WoW.PlayerRace == "BloodElf"

						&& WoW.Focus <= 85)
                    {
                        WoW.CastSpell("Arcane Torrent");
                        return;
                    }
					if (WoW.CanCast("Berserking") 
						
						&& !WoW.IsSpellOnCooldown ("Berserking")
						&& WoW.PlayerHasBuff("Trueshot")
						&& WoW.PlayerRace == "Troll")
                    {
                        WoW.CastSpell("Berserking");
                        return;
                    }					
					if (WoW.CanCast("Blood Fury") 
						&& WoW.PlayerHasBuff("Trueshot")
						&& !WoW.IsSpellOnCooldown ("Blood Fury")
						&& WoW.PlayerRace == "Orc")
                    {
                        WoW.CastSpell("Blood Fury");
                        return;
                    }
				}					
            if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave)  // Do Single Target Stuff here
            {
	
                   
											
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {	

					if (WoW.CanCast("Trueshot")
						&& UseCooldowns
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)
                    {
                        WoW.CastSpell("Trueshot");
                        return;
                    }	
                    if (WoW.CanCast("A Murder of Crows")  // AMurderOfCrows if (not HasBuff(Vulnerable) 
						&& WoW.Talent(6) == 1 
						&& (WoW.Focus >= 30) 
						&& !WoW.TargetHasDebuff("Vulnerable")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst"))
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }					
                    if (WoW.CanCast("A Murder of Crows")  // or (BuffRemainingSec(Vulnerable) < SpellCastTimeSec(AimedShot) and not HasBuff(LockAndLoad)))
						&& WoW.Talent(6) == 1 
						&& (WoW.Focus >= 30) 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") < AimedShotCastTime
						&& !WoW.PlayerHasBuff("Lock and Load")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst"))
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }	
						if (WoW.CanCast("Piercing Shot")  //  PiercingShot if HasBuff(Vulnerable) and PowerToMax < 20
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Talent(7) == 2
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.Focus >=100
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)							
                    {
                        WoW.CastSpell("Piercing Shot");
                        return;
                    }					
						if (WoW.CanCast("Arcane Shot")  //  ArcaneShot if WasLastCast(MarkedShot) and HasTalent(PatientSniper)
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Talent(4) == 3
						&& WoW.WasLastCasted("Marked Shot")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)							
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }	
						
						if (WoW.TargetHasDebuff("Vulnerable")   // AimedShot if HasBuff(LockAndLoad) and HasBuff(Vulnerable)
						&& WoW.PlayerHasBuff("Lock and Load")
						&& WoW.CanCast("AS") 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)					
                    {
                        WoW.CastSpell("AS");
						if (WoW.TargetHasDebuff("Vulnerable")   // AimedShot if HasBuff(LockAndLoad) and HasBuff(Vulnerable)
						&& WoW.PlayerHasBuff("Lock and Load")
						&& WoW.CanCast("AS") 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)					
                    {
                        WoW.CastSpell("AS");						
						return;
                    }	
					}					
						if (WoW.Focus >=50  // AimedShot if (WasLastCast(Windburst)
						&& WoW.WasLastCasted("Windburst")
						&& WoW.CanCast("AS") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting	)				
                    {
                        WoW.CastSpell("AS");
						if (WoW.Focus >=50  // AimedShot if (WasLastCast(Windburst)
					
						&& WoW.CanCast("AS") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting	)				
                    {
                        WoW.CastSpell("AS");	
						if (WoW.Focus >=50  // AimedShot if (WasLastCast(Windburst)
					
						&& WoW.CanCast("AS") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting	)				
                    {
                        WoW.CastSpell("AS");							
						return;
                    }
					}	
					}					
						if (WoW.Focus >=50  /* SpellCastTimeSec(AimedShot) < BuffRemainingSec(Vulnerable)) and (not HasTalent(PiercingShot)*/
						&& WoW.CanCast("AS") 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime	
						&& WoW.Talent(7) == 3
						&& WoW.IsSpellInRange("Windburst") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting		)			
                    {
                        WoW.CastSpell("AS");
						if (WoW.Focus >=50  /* SpellCastTimeSec(AimedShot) < BuffRemainingSec(Vulnerable)) and (not HasTalent(PiercingShot)*/
						&& WoW.CanCast("AS") 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime	
						&& WoW.Talent(7) == 3
						&& WoW.IsSpellInRange("Windburst") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting		)			
                    {
                        WoW.CastSpell("AS");	
						if (WoW.Focus >=50  /* SpellCastTimeSec(AimedShot) < BuffRemainingSec(Vulnerable)) and (not HasTalent(PiercingShot)*/
						&& WoW.CanCast("AS") 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime	
						&& WoW.Talent(7) == 3
						&& WoW.IsSpellInRange("Windburst") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting		)			
                    {
                        WoW.CastSpell("AS");						
						return;
                    }		
					}
					}
						if (WoW.Focus >=50  // CooldownSecRemaining(PiercingShot) > BuffRemainingSec(Vulnerable))

						&& WoW.CanCast("AS") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") > WoW.SpellCooldownTimeRemaining("Piercing Shot")	
						&& WoW.Talent(7) == 2
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting	)				
                    {
                        WoW.CastSpell("AS");
						if (WoW.Focus >=50  // CooldownSecRemaining(PiercingShot) > BuffRemainingSec(Vulnerable))

						&& WoW.CanCast("AS") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") > WoW.SpellCooldownTimeRemaining("Piercing Shot")	
						&& WoW.Talent(7) == 2
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting	)				
                    {
                        WoW.CastSpell("AS");
						if (WoW.Focus >=50  // CooldownSecRemaining(PiercingShot) > BuffRemainingSec(Vulnerable))

						&& WoW.CanCast("AS") 
						&& (!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization")) 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") > WoW.SpellCooldownTimeRemaining("Piercing Shot")	
						&& WoW.Talent(7) == 2
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting	)				
                    {
                        WoW.CastSpell("AS");						
						return;
                    }
					}					
					}
					if (WoW.CanCast("Windburst") // Windburst
						&& !WoW.IsMoving
						&& WoW.Focus >= 20 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting															
						&& WoW.IsSpellInRange("Windburst"))
                    {
                        WoW.CastSpell("Windburst");
                        return;
                    }							
						if (WoW.CanCast("Arcane Shot")  // ArcaneShot if (HasBuff(MarkingTargets)
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.PlayerHasBuff("Marking Targets")
						&& !WoW.PlayerHasBuff("Hunters Mark")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)							
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }					
					/*	if (WoW.CanCast("Arcane Shot")  //HasBuff(Trueshot)) and not HasBuff(HuntersMark)
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=94
						&& WoW.PlayerHasBuff("Trueshot")
						&& !WoW.PlayerHasBuff("Hunters Mark")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)							
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }	*/					
					if (WoW.CanCast("Marked Shot")   // MarkedShot if not HasTalent(PatientSniper)
						&& (WoW.Focus >= 25) 
						&& WoW.TargetHasDebuff("Hunters Mark") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst")
						&& (WoW.Talent(4) == 1 || WoW.Talent(4) == 2))
					{	
					    WoW.CastSpell("Marked Shot");
                        return;
					}					
					if (WoW.CanCast("Marked Shot")   // (BuffRemainingSec(Vulnerable) < SpellCastTimeSec(AimedShot) and (Power > SpellPowerCost(MarkedShot) + SpellPowerCost(AimedShot) 
						&& (WoW.Focus >= 70) 
						&& WoW.TargetHasDebuff("Hunters Mark") 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") < AimedShotCastTime							
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst"))
					{	
					    WoW.CastSpell("Marked Shot");
                        return;
					}					
					if (WoW.CanCast("Marked Shot")   //  not HasTalent(TrueAim)
						&& (WoW.Focus >= 70) 
						&& WoW.TargetHasDebuff("Hunters Mark") 
						&& (WoW.Talent(1) == 2 || WoW.Talent(1) == 1)
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst"))
					{	
					    WoW.CastSpell("Marked Shot");
                        return;
					}		
						if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // AimedShot if PowerToMax < 25
						&& WoW.Focus >= 95 
						&& WoW.CanCast("AS") && WoW.IsSpellInRange("Windburst") && !WoW.TargetHasDebuff("Vulnerable")
						&& (WoW.Talent(7) == 3 || WoW.Talent(7) == 1)						
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)
                    {
                        WoW.CastSpell("AS");
						
                        return;
                    }
						if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // AimedShot if PowerToMax < 25
						&& WoW.Focus >= 95 
						&& WoW.CanCast("AS") && WoW.IsSpellInRange("Windburst") && !WoW.TargetHasDebuff("Vulnerable")
						&& WoW.Talent(7) == 2		
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.SpellCooldownTimeRemaining("Piercing Shot") > 300
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)
                    {
                        WoW.CastSpell("AS");
						
                        return;
                    }						
						if (WoW.CanCast("Arcane Shot")  //  added: less than 50 focus
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=49
						&& WoW.Talent(1) == 1
						&& !WoW.PlayerHasBuff("Lock and Load")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)							
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }		
						if (WoW.CanCast("Arcane Shot")  // arcane shot
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=99
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.Talent(7) == 2)
						
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }					
						if (WoW.CanCast("Arcane Shot")  // arcane shot
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=94
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& !WoW.TargetHasDebuff("Vulnerable"))
						
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }					
					
						
				} 
			}
            if (combatRoutine.Type == RotationType.AOE)
            {
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.PlayerHasBuff("Trueshot") && !WoW.IsMounted)
				
				{
					if (WoW.CanCast("Multi-Shot") 
						&& WoW.Focus >=40
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.TargetHasDebuff("Hunters Mark")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)					
						
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }			
					
					if (WoW.CanCast("Marked Shot") 
						&& (WoW.Focus >= 25) 
						&& WoW.TargetHasDebuff("Hunters Mark") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst")
						&& (WoW.TargetHasDebuff("Vulnerable")||!WoW.TargetHasDebuff("Vulnerable")))
					{	
					    WoW.CastSpell("Marked Shot");
                        return;
					}	
					
				}
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerHasBuff("Trueshot") && !WoW.IsMounted)
                {	
					if (WoW.CanCast("Counter Shot") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.TargetIsCasting
						&& WoW.TargetIsCastingAndSpellIsInterruptible 
							&& WoW.TargetPercentCast >= ConfigFile.ReadValue<int>("Hunter", "Kick Percent") 
						&& Kick)
                    {
                        WoW.CastSpell("Counter Shot");
                        return;
                    }				
					if (WoW.CanCast("Volley") 
						&& !WoW.PlayerHasBuff("Volley")
						&& WoW.Talent(6) == 3)
                    {
                        WoW.CastSpell("Volley");
                        return;
                    }		
					if (KilJaeden && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish"))  
                    {
                        WoW.CastSpell("Kil'jaeden's Burning Wish");
                        return;
                    }						
					if (WoW.CanCast("Arcane Torrent") 
						
						&& !WoW.IsSpellOnCooldown ("Arcane Torrent")
						&& WoW.PlayerHasBuff("Trueshot")
						&& WoW.PlayerRace == "BloodElf"
						&& WoW.Focus <= 85)
                    {
                        WoW.CastSpell("Arcane Torrent");
                        return;
                    }
					if (WoW.CanCast("Berserking") 
						
						&& !WoW.IsSpellOnCooldown ("Berserking")
						&& WoW.PlayerHasBuff("Trueshot")
						&& WoW.PlayerRace == "Troll")
                    {
                        WoW.CastSpell("Berserking");
                        return;
                    }					
					if (WoW.CanCast("Blood Fury") 
						&& WoW.PlayerHasBuff("Trueshot")
						&& !WoW.IsSpellOnCooldown ("Blood Fury")
						&& WoW.PlayerRace == "Orc")
                    {
                        WoW.CastSpell("Blood Fury");
                        return;
                    }	
					if (WoW.CanCast("Trueshot")
						&& UseCooldowns
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)
                    {
                        WoW.CastSpell("Trueshot");
                        return;
                    }	
					if (WoW.CanCast("Piercing Shot") 
						&& WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.Focus >= 100 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting			
						&& WoW.Talent(7) == 2
						&& WoW.IsSpellInRange("Windburst"))
                    {
                        WoW.CastSpell("Piercing Shot");
                        return;
                    }		
					
                    if (WoW.CanCast("A Murder of Crows") 
						&& WoW.Talent(6) == 1 
						&& (WoW.Focus >= 30) 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst"))
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }
					
					if (WoW.CanCast("Marked Shot") 
						&& (WoW.Focus >= 25) 
						&& WoW.TargetHasDebuff("Hunters Mark") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst")
						&& WoW.TargetHasDebuff("Vulnerable")						
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") <= 100))
					{	
					    WoW.CastSpell("Marked Shot");
                        return;
					}	
					if (WoW.CanCast("Marked Shot") 
						&& (WoW.Focus >= 25) 
						&& WoW.TargetHasDebuff("Hunters Mark") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& WoW.IsSpellInRange("Windburst")
						&& !WoW.TargetHasDebuff("Vulnerable"))						

					{	
					    WoW.CastSpell("Marked Shot");
                        return;
					}						
/* 					if (WoW.CanCast("Windburst") 						&& !WoW.IsMoving						&& WoW.Focus >= 20 						&& WoW.TargetHasDebuff("Vulnerable") 						&& !WoW.PlayerIsChanneling						&& !WoW.PlayerIsCasting												&& (WoW.TargetDebuffTimeRemaining("Vulnerable") <= 1)												&& WoW.IsSpellInRange("Windburst"))                    {                        WoW.CastSpell("Windburst");
                        return;
                    }
					if (WoW.CanCast("Windburst") 
						&& !WoW.IsMoving
						&& WoW.Focus >= 20 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting						
						&& !WoW.TargetHasDebuff("Vulnerable") 										
						&& WoW.IsSpellInRange("Windburst"))
                    {
                        WoW.CastSpell("Windburst");
                        return;
                    }	
					*/
					
														
                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // with piercing shot
						&& WoW.Focus >= 50 
						&& WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") 
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime) 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.Talent(7) == 2
						&& (WoW.SpellCooldownTimeRemaining ("Piercing Shot") >300 || WoW.Focus >100))						
                    {
                        WoW.CastSpell("AS");
						
						                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // with piercing shot
						&& WoW.Focus >= 50 
						&& WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") 
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime) 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.Talent(7) == 2
						&& (WoW.SpellCooldownTimeRemaining ("Piercing Shot") >300 || WoW.Focus >100))						
                    {
                        WoW.CastSpell("AS");
						
						                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // with piercing shot
						&& WoW.Focus >= 50 
						&& WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") 
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime) 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.Talent(7) == 2
						&& (WoW.SpellCooldownTimeRemaining ("Piercing Shot") >300 || WoW.Focus >100))						
                    {
                        WoW.CastSpell("AS");
						return;
                    }	
					}
					}
                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))  
						&& WoW.Focus >= 95 
						&& !WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") && WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.Talent(7) == 2
						&& (WoW.SpellCooldownTimeRemaining ("Piercing Shot") >300 || WoW.Focus >100))
                    {
                        WoW.CastSpell("AS");
						
						                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))  
						&& WoW.Focus >= 95 
						&& !WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") && WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.Talent(7) == 2
						&& (WoW.SpellCooldownTimeRemaining ("Piercing Shot") >300 || WoW.Focus >100))
                    {
                        WoW.CastSpell("AS");
						
						                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))  
						&& WoW.Focus >= 95 
						&& !WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") && WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.Talent(7) == 2
						&& (WoW.SpellCooldownTimeRemaining ("Piercing Shot") >300 || WoW.Focus >100))
                    {
                        WoW.CastSpell("AS");
						
                        return;
                    }
					}
					}
                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // without piercing shot
						&& WoW.Focus >= 50 
						&& WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") 
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime) 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Talent(7) == 3
						&& !WoW.PlayerIsChanneling					
						&& !WoW.PlayerIsCasting)
					
                    {
                        WoW.CastSpell("AS");
						
						                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // without piercing shot
						&& WoW.Focus >= 50 
						&& WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") 
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime) 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Talent(7) == 3
						&& !WoW.PlayerIsChanneling					
						&& !WoW.PlayerIsCasting)
					
                    {
                        WoW.CastSpell("AS");
						
						                    if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // without piercing shot
						&& WoW.Focus >= 50 
						&& WoW.TargetHasDebuff("Vulnerable") 
						&& WoW.CanCast("AS") 
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") > AimedShotCastTime) 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Talent(7) == 3
						&& !WoW.PlayerIsChanneling					
						&& !WoW.PlayerIsCasting)
					
                    {
                        WoW.CastSpell("AS");
						
                        return;
                    }	
					}
					}
                    				
                    if (WoW.PlayerHasBuff ("Lock and Load") 
						&& WoW.CanCast("AS") 
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting		
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.IsSpellInRange("Windburst")) 
                    {
                        WoW.CastSpell("AS");
                        return;
                    }	
						if (WoW.CanCast("Multi-Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=94
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.TargetDebuffTimeRemaining("Vulnerable") < AimedShotCastTime						
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }					
						if (WoW.CanCast("Multi-Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.PlayerHasBuff("Marking Targets")
						&& !WoW.TargetHasDebuff("Hunters Mark")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }
					if (WoW.CanCast("Multi-Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& !WoW.PlayerHasBuff("Marking Targets")
						&& !WoW.TargetHasDebuff("Vulnerable")
						&& !WoW.TargetHasDebuff("Hunters Mark")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }
					if (WoW.CanCast("Multi-Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=99
						&& WoW.Talent(7) == 2
						&& !WoW.IsSpellOnCooldown("Piercing Shot")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }	
						if (WoW.CanCast("Multi-Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=99
						&& WoW.Talent(7) == 2
						&& WoW.IsSpellOnCooldown("Piercing Shot")
						&& WoW.SpellCooldownTimeRemaining ("Piercing Shot") <3						
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }						
						if (WoW.CanCast("Multi-Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.TargetHasDebuff("Vulnerable")
						&& WoW.Focus <=49
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }						
                   /* if (WoW.CanCast("Arcane Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=94
						&& !WoW.TargetHasDebuff("Vulnerable")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }						
                    if (WoW.CanCast("Arcane Shot") 
						&& WoW.IsSpellInRange("Windburst") 
						&& WoW.Focus <=94
						&& WoW.TargetHasDebuff("Vulnerable")
						&& (WoW.TargetDebuffTimeRemaining("Vulnerable") <= 1.9 ||	WoW.TargetDebuffTimeRemaining("Vulnerable") >= 5)						
						&& WoW.IsSpellOnCooldown("Windburst")
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)						
						
                    {
                        WoW.CastSpell("Arcane Shot");
                        return;
                    }
*/					
				}
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
Spell,83245,Wolf,F1
Spell,2643,Multi-Shot,D4
Spell,131894,A Murder of Crows,D5
Spell,120360,Barrage,D6
Spell,147362,Counter Shot,D0
Spell,20572,Blood Fury,F3
Spell,5116,Concussive,None
Spell,109304,Exhilaration,V
Spell,186265,Aspect of the Turtle,G
Spell,5384,Feign Death,F2
Spell,127834,Potion,NumPad2
Spell,143940,Silkweave Bandage,None
Spell,55709,Phoenix,F6
Spell,5512,Healthstone,NumPad2
Spell,982,Revive Pet,X
Spell,136,Heal Pet,X
Spell,144259,Kil'jaeden's Burning Wish,NumPad4
Spell,194386,Volley,F
Spell,80483,Arcane Torrent,F3
Spell,53209,Chimaera Shot,F8
Spell,26297,Berserking,F3
Spell,201430,Stampede,C
Spell,24394,Intimidation,None
Spell,142117,Pot,NumPad1
Spell,204147,Windburst,D2
Spell,19434,AS,D4
Spell,185358,Arcane Shot,D5
Spell,185901,Marked Shot,D6
Spell,186387,Bursting Shot,D7
Spell,198670,Piercing Shot,D1
Spell,193526,Trueshot,C
Aura,246153,Precision
Aura,242243,Critical Aimed
Aura,186265,AspectoftheTurtle
Aura,136,Heal Pet
Aura,11196,Bandaged
Aura,234143,Temptation
Aura,2825,Bloodlust
Aura,80353,Time Warp
Aura,90355,Ancient Hysteria
Aura,160452,Netherwinds
Aura,146613,Drums
Aura,32182,Heroism
Aura,229206,Pot
Aura,194386,Volley
Aura,223138,Marking Targets
Aura,185365,Hunters Mark
Aura,194594,Lock and Load
Aura,187131,Vulnerable
Aura,193526,Trueshot
Aura,235712,Gyroscopic Stabilization
Item,144259,Kil'jaeden's Burning Wish
Item,142117,Pot
Item,5512,Healthstone
Item,127834,Potion
*/
