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
		private float MongooseBiteRecharge
        {
            get
            {
                    return 12000f / (1f + (WoW.HastePercent / 100f));
            }
        }		
		 private float GCD
        {
            get
            {

                    return (150f / (1f + (WoW.HastePercent / 100f)));

            }
        }
		 private float bitegcd
        {
            get
            {

                    return (MongooseBiteRecharge/10 - GCD*2);

            }
        }		
private float FocusRegen
{
     get
     {
         return (10f* (1f + (WoW.HastePercent / 100f)));
     }
}
private float FocusRegenAotW
{
     get
     {
         return ((10f* (1f + (WoW.HastePercent / 100f)))+10);
     }
}		
private float FocusTimetoMax
{
     get
     {
         return ((120f - WoW.Focus) /(10f* (1f + (WoW.HastePercent / 100f)))) *100f;
     }
}

private float FocusTimetoMaxAotW
{
     get
     {
         return ((120f - WoW.Focus) /((10f* (1f + (WoW.HastePercent / 100f)))+10)) *100f;
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
if(WoW.PlayerSpec == "Beast Mastery")
{
					
					
					
                    if (!WoW.HasPet && WoW.CanCast("Wolf"))
					{
						WoW.CastSpell("Wolf") ;
						return;
					}
					if (WoW.PetHealthPercent <= 0 && WoW.CanCast("Phoenix"))
					{
						WoW.CastSpell("Phoenix") ;
						return;
					}
				
					if (WoW.PetHealthPercent <= 90 
						&& WoW.Level >= 14&& !WoW.PetHasBuff("Heal Pet")
						&& HealPet						
						&& WoW.CanCast("Revive Pet") 
						&& !WoW.IsMoving)
					{
						WoW.CastSpell("Heal Pet") ;
						return;
					}					
					if (WoW.PetHealthPercent <= 0 
						&& WoW.IsSpellOnCooldown("Phoenix") 
						&& WoW.CanCast("Revive Pet") 
						&& !WoW.IsMoving)
					{
						WoW.CastSpell("Revive Pet") ;
						return;
					}	

					if (WoW.TargetIsCasting)
                    {
                        if (WoW.CanCast("Counter Shot") && WoW.Level >= 32
							&& Kick
							&& WoW.TargetIsCastingAndSpellIsInterruptible 
							&& WoW.TargetPercentCast >= ConfigFile.ReadValue<int>("Hunter", "Kick Percent") 
							&& !WoW.IsSpellOnCooldown("Counter Shot") 
							&& !WoW.PlayerIsChanneling 
							&& !WoW.WasLastCasted("Counter Shot"))
                        {
                            WoW.CastSpell("Counter Shot");						
                            return;
                        }	
                        if (WoW.CanCast("Intimidation") && WoW.Level >= 32 && WoW.PlayerSpec == "BeastMastery"
						
							&& Intimidation
							&& WoW.TargetIsCastingAndSpellIsInterruptible 
							&& WoW.TargetPercentCast <= ConfigFile.ReadValue<int>("Hunter", "Intimidation Percent") 
							&& !WoW.IsSpellOnCooldown("Intimidation") 
							&& !WoW.PlayerIsChanneling 
							&& !WoW.WasLastCasted("Intimidation"))
                        {
                            WoW.CastSpell("Intimidation");						
                            return;
                        }						
					}
					if (WoW.CanCast("Kil'jaeden's Burning Wish") && KilJaeden && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish") && !WoW.IsSpellOnCooldown("Kil'jaeden's Burning Wish"))  
                    {
                        WoW.CastSpell("Kil'jaeden's Burning Wish");
                        return;
                    }	
					
					if (WoW.CanCast("Berserking") 
						
						&& !WoW.IsSpellOnCooldown ("Berserking")
						&& WoW.PlayerRace == "Troll")
                    {
                        WoW.CastSpell("Berserking");
                        return;
                    }	
					if (WoW.CanCast("Arcane Torrent")  
						&& WoW.PlayerRace == "BloodElf"
						&& WoW.Focus <= 85)
                    {
                        WoW.CastSpell("Arcane Torrent");
                        return;
                    }	
					if (WoW.CanCast("Blood Fury") 
						
						&& !WoW.IsSpellOnCooldown ("Blood Fury")
						&& WoW.PlayerRace == "Orc")
                    {
                        WoW.CastSpell("Blood Fury");
                        return;
                    }	
				
				
			if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave)  
            {

			if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted)
                {	
					if (WoW.CanCast("Chimaera Shot") 
						&& WoW.Focus <90
						&& WoW.IsSpellOnCooldown("Dire Frenzy")
						&& WoW.IsSpellOnCooldown("Kill Command")						
						&& WoW.CanCast("Chimaera Shot")
						&& WoW.Talent(2) == 3)
						{
                        WoW.CastSpell("Chimaera Shot");
                        return;
						}				
					if (WoW.CanCast("Volley") 
						&& !WoW.PlayerHasBuff("Volley")
						&& WoW.Talent(6) == 3)
                    {
                        WoW.CastSpell("Volley");
                        return;
                    }			
					if (WoW.CanCast("A Murder of Crows") 
						&& WoW.Talent(6) == 1
						&& WoW.Focus >= 25
						&& WoW.IsSpellInRange("Cobra Shot")
						&& !WoW.IsSpellOnCooldown("A Murder of Crows")	)
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }
//	stampede,if=buff.bloodlust.up|buff.bestial_wrath.up|cooldown.bestial_wrath.remains<=2|target.time_to_die<=14
					if (WoW.CanCast("Stampede") && WoW.Talent(7) == 1 && WoW.IsSpellInRange("Cobra Shot") && ((WoW.PlayerHasBuff("Bestial Wrath")) || (WoW.SpellCooldownTimeRemaining("Bestial Wrath") <=200))						
						&& !WoW.PlayerHasBuff("AspectoftheTurtle")
						&& !WoW.IsSpellOnCooldown("Stampede")) 
                    {
                        WoW.CastSpell("Stampede");

                        return;
                    }				
//dire_beast,if=cooldown.bestial_wrath.remains>3	
					if (WoW.CanCast("Dire Beast") && WoW.Level >= 12 && WoW.Talent(2) != 2 && !WoW.IsSpellOnCooldown ("Dire Beast") && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 300 && WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Dire Beast");
                        return;
                    }									
//dire_frenzy,if=(pet.cat.buff.dire_frenzy.remains<=gcd.max*1.2)|(charges_fractional>=1.8)|target.time_to_die<9
					if (WoW.CanCast("Dire Frenzy") && WoW.Talent(2) == 2 && WoW.IsSpellInRange("Cobra Shot") && !WoW.IsSpellOnCooldown("Dire Frenzy"))
					{
						if (WoW.PetBuffTimeRemaining("Dire Frenzy") <= (GCD*1.8)) 
						{
                        WoW.CastSpell("Dire Frenzy");
						Log.Write("Dire 1"  , Color.Red);
                        return;
						}
						if (WoW.PlayerSpellCharges("Dire Frenzy") >=2)  
						{
                        WoW.CastSpell("Dire Frenzy");
												Log.Write("Dire 2"  , Color.Red);
                        return;
						}						
					}
					



//aspect_of_the_wild,if=buff.bestial_wrath.up|target.time_to_die<12	
					if (WoW.CanCast("Aspect of the Wild")&& WoW.Level >= 26&& UseCooldowns && WoW.PlayerHasBuff("Bestial Wrath") && WoW.IsSpellInRange("Cobra Shot") && !WoW.IsSpellOnCooldown("Aspect of the Wild"))
                    {
                        WoW.CastSpell("Aspect of the Wild");
                        return;
                    }	
//titans_thunder,if=															(talent.dire_frenzy.enabled&(buff.bestial_wrath.up        |   cooldown.bestial_wrath.remains>35))                  |cooldown.dire_beast.remains>=3                      |(buff.bestial_wrath.up&pet.dire_beast.active)	
					if (WoW.CanCast("Titan's Thunder") && WoW.IsSpellInRange("Cobra Shot")&& !WoW.IsSpellOnCooldown("Titan's Thunder")&& WoW.Level >= 110&& (WoW.Talent(2) == 2 && ( WoW.PlayerHasBuff("Bestial Wrath") || WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 350)))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }					
					if (WoW.CanCast("Titan's Thunder") && WoW.IsSpellInRange("Cobra Shot")&& !WoW.IsSpellOnCooldown("Titan's Thunder")&& WoW.Level >= 110 && WoW.SpellCooldownTimeRemaining("Dire Beast") >=300 || (WoW.PlayerHasBuff ("Bestial Wrath") && WoW.PetHasBuff("Dire Beast")))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }	
//bestial_wrath
					if (WoW.CanCast("Bestial Wrath") && WoW.Level >= 40&&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        return;
                    }
//kill_command
					if (WoW.CanCast("Kill Command") && (WoW.Focus >= 30 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 25)) && WoW.Level >= 10&&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }					

//cobra_shot,if=(cooldown.kill_command.remains>focus.time_to_max&cooldown.bestial_wrath.remains>focus.time_to_max)|(buff.bestial_wrath.up&focus.regen*cooldown.kill_command.remains>action.kill_command.cost)|target.time_to_die<cooldown.kill_command.remains|(equipped.parsels_tongue&buff.parsels_tongue.remains<=gcd.max*2)				
					if (WoW.CanCast("Cobra Shot") && WoW.IsSpellOnCooldown("Kill Command") && (WoW.Focus > 32 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 25))&& WoW.IsSpellInRange("Cobra Shot"))
				{
						if (WoW.Level <= 39)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
						if ((WoW.SpellCooldownTimeRemaining("Kill Command") > (FocusTimetoMax)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (FocusTimetoMax)))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }
						if (WoW.PlayerHasBuff("Bestial Wrath") && ((FocusRegen*WoW.SpellCooldownTimeRemaining("Kill Command")) > 300))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
						if ((WoW.SpellCooldownTimeRemaining("Kill Command") > (FocusTimetoMaxAotW)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (FocusTimetoMaxAotW))&& WoW.PlayerHasBuff("Aspect of the Wild"))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
						if (WoW.Legendary(1) == 5 && WoW.PlayerHasBuff("Parsels Tongue") && WoW.PlayerBuffTimeRemaining("Parsels Tongue") <= GCD*2)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }						
				}
					
                }
            }

            if (combatRoutine.Type == RotationType.AOE)
            {
	
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {
																					
				
                    if (WoW.CanCast("Barrage") 
						&& WoW.Talent(6) == 2 
						&& !WoW.IsSpellOnCooldown("Barrage") 
						&& WoW.IsSpellInRange("Cobra Shot") 
						&& WoW.Focus >= 60)
                    {
                        WoW.CastSpell("Barrage");
                        return;
                    }	
					
					if (WoW.CanCast("A Murder of Crows") 
						&& WoW.Talent(6) == 1
						&& WoW.Focus >= 46-FocusRegen 
						&& WoW.PetBuffTimeRemaining("Beast Cleave") > GCD
						&& WoW.IsSpellInRange("Cobra Shot")
						&& !WoW.IsSpellOnCooldown("A Murder of Crows")	)
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }
					if (WoW.CanCast("Multi-Shot") && WoW.Level >= 50
						&& (WoW.Focus >= 40 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 34))
						&& !WoW.PetHasBuff("Beast Cleave") 
						&& WoW.IsSpellInRange("Multi-Shot"))
                    {
                        WoW.CastSpell("Multi-Shot");                        
                        return;
                    }
                    if (WoW.CanCast("Multi-Shot") && WoW.Level >= 50
						&& (WoW.Focus >= 40 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 34)) 
						&& WoW.PetHasBuff("Beast Cleave") 
						&& WoW.PetBuffTimeRemaining("Beast Cleave") <= 70
						&& WoW.IsSpellInRange("Multi-Shot"))
                    {
                        WoW.CastSpell("Multi-Shot");                        
                        return;
                    }					
//	stampede,if=buff.bloodlust.up|buff.bestial_wrath.up|cooldown.bestial_wrath.remains<=2|target.time_to_die<=14	
					if (WoW.CanCast("Stampede") && WoW.Talent(7) == 1 && WoW.IsSpellInRange("Cobra Shot") && ((WoW.PlayerHasBuff("Bestial Wrath")) || (WoW.SpellCooldownTimeRemaining("Bestial Wrath") <=2))						
						&& !WoW.PlayerHasBuff("AspectoftheTurtle")
						&& !WoW.IsSpellOnCooldown("Stampede")) 
                    {
                        WoW.CastSpell("Stampede");

                        return;
                    }				
//dire_beast,if=cooldown.bestial_wrath.remains>3	
					if (WoW.CanCast("Dire Beast") && WoW.Level >= 12&& WoW.Talent(2) != 2 && !WoW.IsSpellOnCooldown ("Dire Beast") && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 300 && WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Dire Beast");
                        return;
                    }									
//dire_frenzy,if=(pet.cat.buff.dire_frenzy.remains<=gcd.max*1.2)|(charges_fractional>=1.8)|target.time_to_die<9
					if (WoW.CanCast("Dire Frenzy") && WoW.Talent(2) == 2 && WoW.IsSpellInRange("Cobra Shot") && !WoW.IsSpellOnCooldown("Dire Frenzy") && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD)
					{
						if (WoW.PetBuffTimeRemaining("Dire Frenzy") <= (GCD*1.8)) 
						{
                        WoW.CastSpell("Dire Frenzy");
						Log.Write("Dire 1"  , Color.Red);
                        return;
						}
						if (WoW.PlayerSpellCharges("Dire Frenzy") >=2)  
						{
                        WoW.CastSpell("Dire Frenzy");
												Log.Write("Dire 2"  , Color.Red);
                        return;
						}						
					}
	
//aspect_of_the_wild,if=buff.bestial_wrath.up|target.time_to_die<12
					if (WoW.CanCast("Aspect of the Wild") && !WoW.IsSpellOnCooldown("Aspect of the WIld") && WoW.Level >= 26&& UseCooldowns && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD && WoW.PlayerHasBuff("Bestial Wrath") && WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Aspect of the Wild");
                        return;
                    }	
//titans_thunder,if=															(talent.dire_frenzy.enabled&(buff.bestial_wrath.up        |   cooldown.bestial_wrath.remains>35))                  |cooldown.dire_beast.remains>=3                      |(buff.bestial_wrath.up                 &pet.dire_beast.active)		
					if (WoW.CanCast("Titan's Thunder")&& WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD && WoW.IsSpellInRange("Cobra Shot")&& !WoW.IsSpellOnCooldown("Titan's Thunder")&& WoW.Level >= 110&& (WoW.Talent(2) == 2 && ( WoW.PlayerHasBuff("Bestial Wrath") || WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 350)))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }					
					if (WoW.CanCast("Titan's Thunder")&& WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD && WoW.IsSpellInRange("Cobra Shot")&& !WoW.IsSpellOnCooldown("Titan's Thunder")&& WoW.Level >= 110 && WoW.SpellCooldownTimeRemaining("Dire Beast") >=300 || (WoW.PlayerHasBuff ("Bestial Wrath") && WoW.PetHasBuff("Dire Beast")))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }	
//bestial_wrath
					if (WoW.CanCast("Bestial Wrath") && !WoW.IsSpellOnCooldown("Bestial Wrath")&& WoW.Level >= 40&& WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD  &&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        return;
                    }
//kill_command
					if (WoW.CanCast("Kill Command") && !WoW.IsSpellOnCooldown("Kill Command") && WoW.Level >= 10&& ((WoW.Focus >= 70-FocusRegen) || (WoW.PlayerHasBuff("Roar of the Seven Lions") && (WoW.Focus >= 59-FocusRegen))) && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD &&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }					
//cobra_shot,if=(cooldown.kill_command.remains>focus.time_to_max&cooldown.bestial_wrath.remains>focus.time_to_max)|(buff.bestial_wrath.up&focus.regen*cooldown.kill_command.remains>action.kill_command.cost)|target.time_to_die<cooldown.kill_command.remains|(equipped.parsels_tongue&buff.parsels_tongue.remains<=gcd.max*2)								
						if (WoW.CanCast("Cobra Shot") && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD&& WoW.IsSpellOnCooldown("Kill Command") && (WoW.Focus > 32 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 25))&& WoW.IsSpellInRange("Cobra Shot"))
				{
						if (WoW.Level <= 39)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
						if ((WoW.SpellCooldownTimeRemaining("Kill Command") > (FocusTimetoMax)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (FocusTimetoMax)))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }
						if (WoW.PlayerHasBuff("Bestial Wrath") && ((FocusRegen*WoW.SpellCooldownTimeRemaining("Kill Command")) > 300))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
						if ((WoW.SpellCooldownTimeRemaining("Kill Command") > (FocusTimetoMaxAotW)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (FocusTimetoMaxAotW))&& WoW.PlayerHasBuff("Aspect of the Wild"))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
						if (WoW.Legendary(1) == 5 && WoW.PlayerHasBuff("Parsels Tongue") && WoW.PlayerBuffTimeRemaining("Parsels Tongue") <= GCD*2)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }						
				}
				}
            }
		}
}
			
// - Survival Hunter	
		if (WoW.PlayerSpec == "Survival")
		{		
            if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave )  // Do Single Target Stuff here
            {
		
					if ((!WoW.IsInCombat || WoW.IsInCombat) && tacticswatch.ElapsedMilliseconds > 10000)
					{
					tacticswatch.Reset();
					Log.Write("Leaving Combat, Resetting tacticswatch.", Color.Red);
					
					}
					if (!WoW.IsInCombat && mongoosebitewatch.ElapsedMilliseconds > 10000)
					{
					mongoosebitewatch.Reset();
					Log.Write("Mongoose Bite watch reset", Color.Red);
                    
					}	
					if (WoW.IsInCombat && mongoosebitewatch.ElapsedMilliseconds > MongooseBiteRecharge)
					{
					mongoosebitewatch.Reset();	
					mongoosebitewatch.Start();
					Log.Write("Mongoose Bite watch reset", Color.Red);
					Log.Write("Mongoose Bite watch start", Color.Red);
                    
					}					
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
	

											
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {

			
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffStacks ("Mongoose Fury") >= 6 && WoW.IsSpellOnCooldown("Fury of the Eagle") &&  !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling
							&& WoW.PlayerSpellCharges("Mongoose Bite") >=1 && WoW.IsSpellInRange("Raptor Strike") && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)))
						{
                        WoW.CastSpell("Mongoose Bite");
                        return;
						}					

//3	0.00	summon_pet
                    if (!WoW.HasPet && WoW.CanCast("Wolf"))
					{
						WoW.CastSpell("Wolf") ;
						return;
					}
					if (WoW.PetHealthPercent <= 0 && WoW.CanCast("Phoenix"))
					{
						WoW.CastSpell("Phoenix") ;
						return;
					}
				
					if (WoW.PetHealthPercent <= 90 
						&& WoW.Level >= 14&& !WoW.PetHasBuff("Heal Pet")
						&& HealPet						
						&& WoW.CanCast("Revive Pet") 
						&& !WoW.IsMoving)
					{
						WoW.CastSpell("Heal Pet") ;
						return;
					}					
					if (WoW.PetHealthPercent <= 0 
						&& WoW.IsSpellOnCooldown("Phoenix") 
						&& WoW.CanCast("Revive Pet") 
						&& !WoW.IsMoving)
					{
						WoW.CastSpell("Revive Pet") ;
						return;
					}

//9	0.00	harpoon
				if(pullwatch.ElapsedMilliseconds < 10000 && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)))
				{
                    if (WoW.CanCast("Explosive Trap") 
						&& !WoW.IsMoving 
						&& WoW.IsSpellInRange("Raptor Strike")
						&& !WoW.PlayerIsCasting 
						&& !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Explosive Trap");
                        return;
                    }
//7	0.00	steel_trap
                    if (WoW.CanCast("Steel Trap") 
						&& !WoW.IsMoving 
						&& WoW.IsSpellInRange("Raptor Strike")
						&& !WoW.PlayerIsCasting 
						&& WoW.Talent(4) == 3
						&& !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Steel Trap");
                        return;
                    }
//8	0.00	dragonsfire_grenade
                    if (WoW.CanCast("Dragonsfire Grenade") 
						&& !WoW.IsMoving 
						&& WoW.IsSpellInRange("Raptor Strike")
						&& !WoW.PlayerIsCasting 
						&& WoW.Talent(6) == 2
						&& !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Dragonsfire Grenade");
                        return;
                    }
//9	0.00	harpoon
                    if (WoW.CanCast("Harpoon") 
						&& WoW.IsSpellInRange("Harpoon")
						&& !WoW.PlayerIsCasting 
						&& !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Harpoon");
                        return;
                    }
				}
 //0.00	muzzle,if=target.debuff.casting.react
					if (WoW.CanCast("Muzzle") && Kick && WoW.TargetIsCastingAndSpellIsInterruptible 
							&& WoW.TargetPercentCast >= ConfigFile.ReadValue<int>("Hunter", "Kick Percent")  && !WoW.IsSpellOnCooldown("Muzzle")&& !WoW.PlayerIsChanneling && !WoW.WasLastCasted("Muzzle"))
                    {
                            WoW.CastSpell("Muzzle");						
                            return;
                        }	
 //	0.00	call_action_list,name=mokMaintain,if=talent.way_of_the_moknathal.enabled
				    if(WoW.Talent(1) == 3)
					{
						if (WoW.CanCast("Raptor Strike") && !WoW.PlayerHasBuff("tactics") && WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
Log.Write("Raptor 1", Color.Red);							
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();
                        return;
						}
						if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") < GCD  && WoW.IsSpellInRange("Raptor Strike"))
						{
							
                        WoW.CastSpell("Raptor Strike");
Log.Write("Raptor 2", Color.Red);						
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
					    if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics") && WoW.Talent(1) == 3&& WoW.PlayerBuffStacks("tactics") < 2&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");
Log.Write("Raptor 3", Color.Red);						
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
					}	
//	0.00	call_action_list,name=CDs,if=buff.moknathal_tactics.stack>=2|!talent.way_of_the_moknathal.enabled
                    if (((WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1 && WoW.PlayerBuffStacks("tactics") >= 2) || WoW.Talent(1) != 3)&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && UseCooldowns)
                    {
						if (WoW.CanCast("Arcane Torrent")  && WoW.PlayerHasBuff("Aspect of the Eagle")&& !WoW.IsSpellOnCooldown ("Arcane Torrent")&& WoW.PlayerRace == "BloodElf"&& WoW.Focus <= 30 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)
						{
                        WoW.CastSpell("Arcane Torrent");
                        return;
						}
						if (WoW.CanCast("Berserking") && WoW.PlayerHasBuff("Aspect of the Eagle")&& !WoW.IsSpellOnCooldown ("Berserking")&& WoW.PlayerRace == "Troll" && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling )
						{
                        WoW.CastSpell("Berserking");
                        return;
						}					
						if (WoW.CanCast("Blood Fury") && WoW.PlayerHasBuff("Aspect of the Eagle")	&& !WoW.IsSpellOnCooldown ("Blood Fury")&& WoW.PlayerRace == "Orc" && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling )
						{
                        WoW.CastSpell("Blood Fury");
                        return;
						}
//	2.82	snake_hunter,if=cooldown.mongoose_bite.charges=0&buff.mongoose_fury.remains>3*gcd						
						if (WoW.CanCast("Snake Hunter")&& WoW.Talent(2) == 3&& WoW.PlayerSpellCharges("Mongoose Bite") <= 0 && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") >= 300*GCD && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Snake Hunter");
                        return;
						}	
//	2.51	Aspect_of_the_eagle,if=(buff.mongoose_fury.remains<=11&buff.mongoose_fury.up)&(cooldown.fury_of_the_eagle.remains>buff.mongoose_fury.remains)
						if (WoW.CanCast("Aspect of the Eagle")&& WoW.PlayerHasBuff("Mongoose Fury")&& WoW.PlayerBuffTimeRemaining("Mongoose Fury") <=1100
							&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Aspect of the Eagle");
                        return;
						}
//	3.97	Aspect_of_the_eagle,if=(buff.mongoose_fury.remains<=7&buff.mongoose_fury.up)
						if (WoW.CanCast("Aspect of the Eagle")&& WoW.PlayerHasBuff("Mongoose Fury")&& WoW.PlayerBuffTimeRemaining("Mongoose Fury") <=700&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Aspect of the Eagle");
                        return;
						}						
                    }					
//0.00	call_action_list,name=preBitePhase,if=!buff.mongoose_fury.up
					if(!WoW.PlayerHasBuff("Mongoose Fury") && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)) && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
					{
	//16.61	flanking_strike
						if (WoW.CanCast("Flanking Strike") && WoW.Focus >= 50 && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Flanking Strike");
                        return;
						}
//0.00	spitting_cobra
						if (WoW.CanCast("Spitting Cobra") && WoW.Focus >= 30&& WoW.Talent(7) == 1)
						{
                        WoW.CastSpell("Spitting Cobra");
                        return;
						}
//6.77	lacerate,if=!dot.lacerate.ticking
						if (WoW.CanCast("Lacerate") && WoW.Focus >= 35 && !WoW.TargetHasDebuff("Lacerate") && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Lacerate");
                        return;
						}
//0.00	raptor_strike,if=active_enemies=1&talent.serpent_sting.enabled&!dot.serpent_sting.ticking
						if (WoW.CanCast("Raptor Strike") && !WoW.PlayerHasBuff("tactics") && !WoW.TargetHasDebuff("Serpent Sting")&& WoW.Talent(6) == 3 && WoW.IsSpellInRange("Raptor Strike"))
						{
Log.Write("Raptor 4", Color.Red);							
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();
                        return;
						}
//0.00	steel_trap
                    if (WoW.CanCast("Steel Trap") && !WoW.IsMoving && WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && WoW.Talent(4) == 3)
						{
                        WoW.CastSpell("Steel Trap");
                        return;
						}
//0.00	a_murder_of_crows
                    if (WoW.CanCast("Murder of Crows") && !WoW.IsSpellOnCooldown ("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1)
						{
                        WoW.CastSpell("Murder of Crows");
                        return;
						}
//0.00	dragonsfire_grenade
                    if (WoW.CanCast("Dragonsfire Grenade")  && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting&& WoW.Talent(6) == 2)
						{
                        WoW.CastSpell("Dragonsfire Grenade");
                        return;
						}
//	6.52	explosive_trap
                    if (WoW.CanCast("Explosive Trap") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Explosive Trap");
                        return;
						}
// 11.01	caltrops,if=!dot.caltrops.ticking
						if (WoW.CanCast("Caltrops") && !WoW.IsMoving&& !WoW.TargetHasDebuff("Caltrops")&& WoW.Talent(4) == 1&& WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Caltrops");
                        return;
						}	
//	butchery,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
                   /* 	if (WoW.CanCast("Butchery") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Butchery");
                        return;
						}
					*/
//0.00	carve,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
                   /*	 if (WoW.CanCast("Carve") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
					*/
//	3.40	lacerate,if=dot.lacerate.remains<3.6
						if (WoW.CanCast("Lacerate") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Lacerate");
                        return;
						}
					}
//call_action_list,name=aoe,if=active_enemies>=3
//if(WoW.CountEnemyNPCsInRange >3	

//actions.bitePhase
//	5.42	fury_of_the_eagle,if=(!talent.way_of_the_moknathal.enabled|buff.moknathal_tactics.remains>(gcd*(8%3)))&buff.mongoose_fury.stack=6,interrupt_if=(talent.way_of_the_moknathal.enabled&buff.moknathal_tactics.remains<=tick_time)
						if ( (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)) &&!WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
					{
						if (WoW.CanCast ("Fury of the Eagle") && (WoW.Talent(1) != 3 || WoW.PlayerBuffTimeRemaining("tactics") > (GCD*(8%3))) && WoW.PlayerBuffStacks ("Mongoose Fury") >= 6)
						{
                        WoW.CastSpell("Fury of the Eagle");
                        return;
						}
//	64.01	mongoose_bite,if=charges>=2&cooldown.mongoose_bite.remains<gcd*2
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerSpellCharges("Mongoose Bite") >=2 && mongoosebitewatch.ElapsedMilliseconds > bitegcd*10 && WoW.IsSpellInRange("Raptor Strike") )
						{
Log.Write("Bite >=2" , Color.Red);	

                        WoW.CastSpell("Mongoose Bite");

                        return;
						}
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerSpellCharges("Mongoose Bite") >=3 && WoW.IsSpellInRange("Raptor Strike") )
						{
Log.Write("Bite >=3" , Color.Red);
Log.Write("Start bite watch" , Color.Red);	

                        WoW.CastSpell("Mongoose Bite");
						mongoosebitewatch.Reset();
						mongoosebitewatch.Start();
                        return;
						}						
//	24.07	flanking_strike,if=((buff.mongoose_fury.remains>(gcd*(cooldown.mongoose_bite.charges+2)))&cooldown.mongoose_bite.charges<=1)&!buff.Aspect_of_the_eagle.up
						if (WoW.CanCast("Flanking Strike") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") > (GCD*(WoW.SpellCooldownTimeRemaining("Mongoose Bite")+200)) && WoW.PlayerSpellCharges("Mongoose Bite") <=1 && !WoW.PlayerHasBuff("Aspect of the Eagle")
						&& WoW.Focus >= 50 && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Flanking Strike");
                        return;
						}
					
//	53.63	mongoose_bite,if=buff.mongoose_fury.up
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerHasBuff("Mongoose Fury") && WoW.IsSpellInRange("Raptor Strike") && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics"))))
						{
                        WoW.CastSpell("Mongoose Bite");
                        return;
						}
//	7.26	flanking_strike
						if (WoW.CanCast("Flanking Strike") && WoW.Focus >= 50 && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Flanking Strike");
                        return;
						}
					}	
//biteFill	
					if(WoW.PlayerHasBuff("Mongoose Fury")&& WoW.PlayerSpellCharges("Mongoose Bite") ==0&&(WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)) && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
					{					
					//0.00	spitting_cobra
						if (WoW.CanCast("Spitting Cobra")&& WoW.Focus >= 30&& WoW.Talent(7) == 1)
						{
                        WoW.CastSpell("Spitting Cobra");
                        return;
						}
//0.00	butchery,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
						/*if (WoW.CanCast("Butchery") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Butchery");
                        return;
						}
						*/
//0.00	carve,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
						/*if (WoW.CanCast("Carve") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
						*/
//	10.26	lacerate,if=dot.lacerate.remains<3.6
						if (WoW.CanCast("Lacerate") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Lacerate");
                        return;
						}
//0.00	raptor_strike,if=active_enemies=1&talent.serpent_sting.enabled&!dot.serpent_sting.ticking
						if (WoW.CanCast("Raptor Strike")&& !WoW.TargetHasDebuff("Serpent Sting") && WoW.Talent(6) == 3 && WoW.IsSpellInRange("Raptor Strike"))
						{
Log.Write("Raptor 5", Color.Red);							
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();
                        return;
						}
//0.00	steel_trap
						if (WoW.CanCast("Steel Trap")&& !WoW.IsMoving && WoW.IsSpellInRange("Raptor Strike")&& WoW.Talent(4) == 3)
						{
                        WoW.CastSpell("Steel Trap");
                        return;
						}
//0.00	a_murder_of_crows
                    if (WoW.CanCast("Murder of Crows") && !WoW.IsSpellOnCooldown ("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1	&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD)
						{
                        WoW.CastSpell("Murder of Crows");
                        return;
						}
// 0.00	dragonsfire_grenade
						if (WoW.CanCast("Dragonsfire Grenade") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike") && WoW.Talent(6) == 2&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Dragonsfire Grenade");
                        return;
						}
//	3.60	explosive_trap
						if (WoW.CanCast("Explosive Trap") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Explosive Trap");
                        return;
						}
//	4.10	caltrops,if=!dot.caltrops.ticking
						if (WoW.CanCast("Caltrops") && !WoW.IsMoving&& !WoW.TargetHasDebuff("Caltrops")&& WoW.Talent(4) == 1&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Caltrops");
                        return;
						}
					}
//FILLERS

//0.00	carve,if=active_enemies>1&talent.serpent_sting.enabled&!dot.serpent_sting.ticking
					if((WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)))
					{
						if (WoW.CanCast("Carve")&& WoW.CountEnemyNPCsInRange >1	&& 	WoW.Talent(6) == 3	&& WoW.Focus >= 40 && !WoW.TargetHasDebuff("Serpent Sting")&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting&& !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
//0.00	throwing_axes
						if (WoW.CanCast("Throwing Axes")&& WoW.Focus >= 15 && WoW.Talent(1) == 2&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Throwing Axes");
                        return;
						}
//0.00	carve,if=active_enemies>2
						if (WoW.CanCast("Carve")&& WoW.CountEnemyNPCsInRange >2&& WoW.Focus >= 40 && WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
//10.69	raptor_strike,if=(talent.way_of_the_moknathal.enabled&buff.moknathal_tactics.remains<gcd*4)
						if (WoW.CanCast("Raptor Strike") &&  WoW.PlayerHasBuff("tactics")&& WoW.PlayerBuffTimeRemaining("tactics")< (GCD*4) && WoW.Talent(1) == 3&& !WoW.PlayerIsCasting  && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
Log.Write("Raptor 6", Color.Red);							
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
//0.41	raptor_strike,if=focus>((25-focus.regen*gcd)+55)
						if (WoW.CanCast("Raptor Strike") && WoW.CanCast("Raptor Strike")&& WoW.Focus>((25-FocusRegen*(GCD/100))+55)&& !WoW.PlayerIsCasting&& !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
Log.Write("Raptor 7", Color.Red);							
                        WoW.CastSpell("Raptor Strike");
						Log.Write("Too much focus! RAPTOR", Color.Red);

						
						
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
					}						
				}
			}
			if (combatRoutine.Type == RotationType.AOE )  // Do Single Target Stuff here
			{
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {	
			

					if ((!WoW.IsInCombat || WoW.IsInCombat) && tacticswatch.ElapsedMilliseconds > 10000)
					{
					tacticswatch.Reset();
					Log.Write("Leaving Combat, Resetting tacticswatch.", Color.Red);
					
					}
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
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffStacks ("Mongoose Fury") >= 6 && WoW.IsSpellOnCooldown("Fury of the Eagle") &&  !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling
							&& WoW.PlayerSpellCharges("Mongoose Bite") >=1 && WoW.SpellCooldownTimeRemaining("Mongoose Bite") <GCD*2 && WoW.IsSpellInRange("Raptor Strike") && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)))
						{
                        WoW.CastSpell("Mongoose Bite");
                        return;
						}					
					if(pullwatch.ElapsedMilliseconds < 10000 && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)))
				{
                    if (WoW.CanCast("Explosive Trap") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Explosive Trap");
                        return;
                    }
//7	0.00	steel_trap
                    if (WoW.CanCast("Steel Trap") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && WoW.Talent(4) == 3&& !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Steel Trap");
                        return;
                    }
//8	0.00	dragonsfire_grenade
                    if (WoW.CanCast("Dragonsfire Grenade")&& !WoW.IsMoving && WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && WoW.Talent(6) == 2&& !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Dragonsfire Grenade");
                        return;
                    }
//9	0.00	harpoon
                    if (WoW.CanCast("Harpoon") && WoW.IsSpellInRange("Harpoon")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Harpoon");
                        return;
                    }
				}
//3	0.00	summon_pet
					if (!WoW.HasPet && WoW.CanCast("Wolf") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
					{
						WoW.CastSpell("Wolf") ;
						return;
					}

//9	0.00	harpoon

 //0.00	muzzle,if=target.debuff.casting.react
					if (WoW.CanCast("Muzzle") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 60 && !WoW.IsSpellOnCooldown("Muzzle")&& !WoW.PlayerIsChanneling && !WoW.WasLastCasted("Muzzle"))
						{
                            WoW.CastSpell("Muzzle");						
                            return;
                        }
						if (!Frizzos && WoW.CanCast("Butchery")&& WoW.Focus >=40 && !WoW.IsSpellOnCooldown("Butchery") && WoW.Talent(6) == 1&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Butchery");
                        return;
						}						
 //	0.00	call_action_list,name=mokMaintain,if=talent.way_of_the_moknathal.enabled
				    if(WoW.Talent(1) == 3)
					{
						if (WoW.CanCast("Raptor Strike") && !WoW.PlayerHasBuff("tactics")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();
                        return;
						}
						if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") < GCD  && WoW.IsSpellInRange("Raptor Strike"))
						{
							
                        WoW.CastSpell("Raptor Strike");	
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
					    if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics") && WoW.Talent(1) == 3&& WoW.PlayerBuffStacks("tactics") < 2&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");		
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
					}	
//	0.00	call_action_list,name=CDs,if=buff.moknathal_tactics.stack>=2|!talent.way_of_the_moknathal.enabled
                    if (((WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffStacks("tactics") >= 2 && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1) || WoW.Talent(1) != 3) && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && UseCooldowns)
                    {
//	2.51	Aspect_of_the_eagle,if=(buff.mongoose_fury.remains<=11&buff.mongoose_fury.up)&(cooldown.fury_of_the_eagle.remains>buff.mongoose_fury.remains)
						if (WoW.CanCast("Aspect of the Eagle")&& WoW.PlayerHasBuff("Mongoose Fury")&& WoW.PlayerBuffTimeRemaining("Mongoose Fury") <=1100
							&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Aspect of the Eagle");
                        return;
						}
//	3.97	Aspect_of_the_eagle,if=(buff.mongoose_fury.remains<=7&buff.mongoose_fury.up)
						if (WoW.CanCast("Aspect of the Eagle")&& WoW.PlayerHasBuff("Mongoose Fury")&& WoW.PlayerBuffTimeRemaining("Mongoose Fury") <=700&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Aspect of the Eagle");
                        return;
						}	
						if (WoW.CanCast("Arcane Torrent")  && WoW.PlayerHasBuff("Aspect of the Eagle")&& !WoW.IsSpellOnCooldown ("Arcane Torrent")&& WoW.PlayerRace == "BloodElf" && WoW.Focus <= 85)
						{
                        WoW.CastSpell("Arcane Torrent");
                        return;
						}
						if (WoW.CanCast("Berserking") && WoW.PlayerHasBuff("Aspect of the Eagle")&& !WoW.IsSpellOnCooldown ("Berserking")&& WoW.PlayerRace == "Troll" && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Berserking");
                        return;
						}					
						if (WoW.CanCast("Blood Fury") && WoW.PlayerHasBuff("Aspect of the Eagle")	&& !WoW.IsSpellOnCooldown ("Blood Fury")&& WoW.PlayerRace == "Orc" && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Blood Fury");
                        return;
						}
//	2.82	snake_hunter,if=cooldown.mongoose_bite.charges=0&buff.mongoose_fury.remains>3*gcd						
						if (WoW.CanCast("Snake Hunter")&& WoW.Talent(2) == 3&& WoW.PlayerSpellCharges("Mongoose Bite") <= 0 && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") >= 300*GCD && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Snake Hunter");
                        return;
						}	
					
                    }					
//0.00	call_action_list,name=preBitePhase,if=!buff.mongoose_fury.up
					if(!WoW.PlayerHasBuff("Mongoose Fury") && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1))&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
					{
	//16.61	flanking_strike
						if (WoW.CanCast("Flanking Strike") && WoW.Focus >= 50 && WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Flanking Strike");
                        return;
						}
//0.00	spitting_cobra
						if (WoW.CanCast("Spitting Cobra") && WoW.Focus >= 30&& WoW.Talent(7) == 1&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Spitting Cobra");
                        return;
						}
//6.77	lacerate,if=!dot.lacerate.ticking
						if (WoW.CanCast("Lacerate") && WoW.Focus >= 35 && !WoW.TargetHasDebuff("Lacerate") && WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Lacerate");
                        return;
						}
//0.00	raptor_strike,if=active_enemies=1&talent.serpent_sting.enabled&!dot.serpent_sting.ticking
						if (WoW.CanCast("Carve") && !WoW.PlayerHasBuff("tactics") && !WoW.TargetHasDebuff("Serpent Sting")&& WoW.Talent(6) == 3 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
//0.00	steel_trap
                    if (WoW.CanCast("Steel Trap") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && WoW.Talent(4) == 3&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Steel Trap");
                        return;
						}
//0.00	a_murder_of_crows
                    if (WoW.CanCast("Murder of Crows") && !WoW.IsSpellOnCooldown ("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1	&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD)
						{
                        WoW.CastSpell("Murder of Crows");
                        return;
						}
//0.00	dragonsfire_grenade
                    if (WoW.CanCast("Dragonsfire Grenade")&& !WoW.IsMoving && WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting&& WoW.Talent(6) == 2&& !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Dragonsfire Grenade");
                        return;
						}
//	6.52	explosive_trap
                    if (WoW.CanCast("Explosive Trap") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Explosive Trap");
                        return;
						}
// 11.01	caltrops,if=!dot.caltrops.ticking
						if (WoW.CanCast("Caltrops")&& !WoW.IsMoving && !WoW.TargetHasDebuff("Caltrops")&& WoW.Talent(4) == 1&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Caltrops");
                        return;
						}	
//	butchery,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
                    	if (Frizzos && WoW.CanCast("Butchery") && WoW.Focus >= 40 && WoW.Talent(6) == 1 && WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Butchery");
                        return;
						}
					
//0.00	carve,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
                   	 if (Frizzos && WoW.CanCast("Carve") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
					
//	3.40	lacerate,if=dot.lacerate.remains<3.6
						if (WoW.CanCast("Lacerate") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting&& !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Lacerate");
                        return;
						}
					}
//call_action_list,name=aoe,if=active_enemies>=3
//if(WoW.CountEnemyNPCsInRange >3	



//actions.bitePhase
//	5.42	fury_of_the_eagle,if=(!talent.way_of_the_moknathal.enabled|buff.moknathal_tactics.remains>(gcd*(8%3)))&buff.mongoose_fury.stack=6,interrupt_if=(talent.way_of_the_moknathal.enabled&buff.moknathal_tactics.remains<=tick_time)
					if((WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)))
					{			
						if (WoW.CanCast ("Fury of the Eagle") && (WoW.Talent(1) != 3 || WoW.PlayerBuffTimeRemaining("tactics") > (GCD*(8%3)))  && WoW.PlayerBuffStacks ("Mongoose Fury") >= 6 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Fury of the Eagle");
                        return;
						}
//	64.01	mongoose_bite,if=charges>=2&cooldown.mongoose_bite.remains<gcd*2
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerSpellCharges("Mongoose Bite") >=2 && mongoosebitewatch.ElapsedMilliseconds > bitegcd*10 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike") && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics"))))
						{
                        WoW.CastSpell("Mongoose Bite");
					
                        return;
						}
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerSpellCharges("Mongoose Bite") >=3 && WoW.IsSpellInRange("Raptor Strike") )
						{
Log.Write("Bite >=3" , Color.Red);	

                        WoW.CastSpell("Mongoose Bite");
						mongoosebitewatch.Reset();
						mongoosebitewatch.Start();
                        return;
						}						
//	24.07	flanking_strike,if=((buff.mongoose_fury.remains>(gcd*(cooldown.mongoose_bite.charges+2)))&cooldown.mongoose_bite.charges<=1)&!buff.Aspect_of_the_eagle.up
						if (WoW.CanCast("Flanking Strike") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") > (GCD*(WoW.SpellCooldownTimeRemaining("Mongoose Bite"))) && WoW.PlayerSpellCharges("Mongoose Bite") <=1 && !WoW.PlayerHasBuff("Aspect of the Eagle")
						&& WoW.Focus >= 50 && WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting&& !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Flanking Strike");
                        return;
						}
					
//	53.63	mongoose_bite,if=buff.mongoose_fury.up
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerHasBuff("Mongoose Fury")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike") && (WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics"))))
						{
                        WoW.CastSpell("Mongoose Bite");
                        return;
						}
//	7.26	flanking_strike
						if (WoW.CanCast("Flanking Strike") && WoW.Focus >= 50 && WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Flanking Strike");
                        return;
						}
//biteFill	
					
					
					if(WoW.PlayerHasBuff("Mongoose Fury")&& WoW.PlayerSpellCharges("Mongoose Bite") ==0&&(WoW.Talent(1) != 3 || (WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD*1.1)) && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
					{
//0.00	spitting_cobra
						if (WoW.CanCast("Spitting Cobra")&& WoW.Focus >= 30&& WoW.Talent(7) == 1&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Spitting Cobra");
                        return;
						}
//0.00	butchery,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
						if (Frizzos && WoW.CanCast("Butchery") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Butchery");
                        return;
						}
						
//0.00	carve,if=equipped.frizzos_fingertrap&dot.lacerate.remains<3.6
						if (Frizzos && WoW.CanCast("Carve") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
						
//	10.26	lacerate,if=dot.lacerate.remains<3.6
						if (WoW.CanCast("Lacerate") && WoW.Focus >= 40 && WoW.Talent(6) == 1&& WoW.TargetDebuffTimeRemaining("Lacerate") <360&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Lacerate");
                        return;
						}
//0.00	raptor_strike,if=active_enemies=1&talent.serpent_sting.enabled&!dot.serpent_sting.ticking
						if (WoW.CanCast("Carve")&& !WoW.TargetHasDebuff("Serpent Sting") && WoW.Talent(6) == 3&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
//0.00	steel_trap
						if (WoW.CanCast("Steel Trap")&& !WoW.IsMoving && WoW.IsSpellInRange("Raptor Strike")&& WoW.Talent(4) == 3&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Steel Trap");
                        return;
						}
//0.00	a_murder_of_crows
						if (WoW.CanCast("Murder of Crows") && !WoW.IsSpellOnCooldown ("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1	&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") > GCD)
						{
                        WoW.CastSpell("Murder of Crows");
                        return;
						}
// 0.00	dragonsfire_grenade
						if (WoW.CanCast("Dragonsfire Grenade") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike") && WoW.Talent(6) == 2&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Dragonsfire Grenade");
                        return;
						}
//	3.60	explosive_trap
						if (WoW.CanCast("Explosive Trap") && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Explosive Trap");
                        return;
						}
//	4.10	caltrops,if=!dot.caltrops.ticking
						if (WoW.CanCast("Caltrops") && !WoW.IsMoving&& !WoW.TargetHasDebuff("Caltrops")&& WoW.Talent(4) == 1&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Caltrops");
                        return;
						}
						if (WoW.CanCast("Butchery")&& WoW.Focus >=40 && !WoW.IsSpellOnCooldown("Butchery") && WoW.Talent(6) == 1&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Butchery");
                        return;
						}
					}
						
//FILLERS		

//0.00	carve,if=active_enemies>1&talent.serpent_sting.enabled&!dot.serpent_sting.ticking
	
						if (WoW.CanCast("Carve")	&& 	WoW.Talent(6) == 3	&& WoW.Focus >= 40 && !WoW.TargetHasDebuff("Serpent Sting")&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting&& !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
//0.00	throwing_axes
						if (WoW.CanCast("Throwing Axes")&& WoW.Focus >= 15 && WoW.Talent(1) == 2&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Throwing Axes");
                        return;
						}
//0.00	carve,if=active_enemies>2
						if (WoW.CanCast("Carve")&& WoW.CountEnemyNPCsInRange >2&& WoW.Focus >= 40 && WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");
                        return;
						}
//10.69	raptor_strike,if=(talent.way_of_the_moknathal.enabled&buff.moknathal_tactics.remains<gcd*4)
						if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics")&& WoW.PlayerBuffTimeRemaining("tactics") <(GCD*4) && WoW.Talent(1) == 3&& !WoW.PlayerIsCasting  && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
//0.41	raptor_strike,if=focus>((25-focus.regen*gcd)+55)
						if (WoW.CanCast("Carve") && WoW.CanCast("Raptor Strike")&& WoW.Focus >= 80&& !WoW.PlayerIsCasting&& !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Carve");						
                        return;
						}	
					}			
				}							
			}
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
						return;
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
						return;
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
						return;
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
						return;
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
						&& (WoW.Focus >= 75) 
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
						&& (WoW.Focus >= 25) 
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
						&& WoW.CanCast("AS") && WoW.IsSpellInRange("Windburst") 
						&& (WoW.Talent(7) == 3 || WoW.Talent(7) == 1)						
						&& !WoW.PlayerIsChanneling
						&& !WoW.PlayerIsCasting)
                    {
                        WoW.CastSpell("AS");
						
                        return;
                    }
						if ((!WoW.IsMoving || WoW.PlayerHasBuff("Gyroscopic Stabilization"))   // AimedShot if PowerToMax < 25
						&& WoW.Focus >= 95 
						&& WoW.CanCast("AS") && WoW.IsSpellInRange("Windburst") 
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
/* 					if (WoW.CanCast("Windburst") 						&& !WoW.IsMoving						&& WoW.Focus >= 20 						&& WoW.TargetHasDebuff("Vulnerable") 						&& !WoW.PlayerIsChanneling						&& !WoW.PlayerIsCasting												&& (WoW.TargetDebuffTimeRemaining("Vulnerable") <= 1)												&& WoW.IsSpellInRange("Windburst"))                    {
                        WoW.CastSpell("Windburst");
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


	

/*
[AddonDetails.db]
AddonAuthor=Vectarius
AddonName=myspellpriority
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,83245,Wolf,F1
Spell,120679,Dire Beast,D1
Spell,217200,Dire Frenzy,D1
Spell,193455,Cobra Shot,D3
Spell,2643,Multi-Shot,D4
Spell,34026,Kill Command,D2
Spell,19574,Bestial Wrath,D8
Spell,131894,A Murder of Crows,D5
Spell,206505,Murder of Crows,D5
Spell,120360,Barrage,D6
Spell,147362,Counter Shot,D0
Spell,193530,Aspect of the Wild,D9
Spell,20572,Blood Fury,F3
Spell,207068,Titan's Thunder,D7
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
Spell,190928,Mongoose Bite,D1
Spell,202800,Flanking Strike,D2
Spell,185855,Lacerate,D3
Spell,186270,Raptor Strike,D4
Spell,194277,Caltrops,D5
Spell,191433,Explosive Trap,D6
Spell,194855,Dragonsfire Grenade,D7
Spell,200163,Throwing Axes,D8
Spell,203415,Fury of the Eagle,D9
Spell,186289,Aspect of the Eagle,F4
Spell,212436,Butchery,C
Spell,187707,Muzzle,F
Spell,194407,Spitting Cobra,C
Spell,187708,Carve,F7
Spell,190925,Harpoon,NumPad5
Spell,162488,Steel Trap,D5
Spell,201078,Snake Hunter,D0
Spell,204147,Windburst,D2
Spell,19434,AS,D4
Spell,185358,Arcane Shot,D5
Spell,185901,Marked Shot,D6
Spell,186387,Bursting Shot,D7
Spell,198670,Piercing Shot,D1
Spell,193526,Trueshot,C
Aura,120679,Dire Beast
Aura,151805,Parsels Tongue
Aura,217200,Dire Frenzy
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
Aura,19574,Bestial Wrath
Aura,118455,Beast Cleave
Aura,193530,Aspect of the Wild
Aura,194386,Volley
Aura,137080,Roar of the Seven Lions
Aura,190931,Mongoose Fury
Aura,118253,Serpent Sting
Aura,185855,Lacerate
Aura,186289,Aspect of the Eagle
Aura,194277,Caltrops
Aura,201081,tactics
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
