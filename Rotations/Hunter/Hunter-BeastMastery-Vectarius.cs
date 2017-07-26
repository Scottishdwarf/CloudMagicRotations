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
		

		private readonly Stopwatch pullwatch = new Stopwatch();

		

public int DireBeastCount
        {
            get
            {
                string[] idBuffs = { "Dire Beast", "DireFrenzy1" };
                var buffs = 0;
                for (var i = 0; i < idBuffs.Length; i++)
                    if (WoW.PlayerBuffStacks(idBuffs[i])>0)
                        buffs = WoW.PlayerBuffStacks(idBuffs[i]);
                return buffs;
            }
            
        }

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
		private CheckBox QaplaBox; 
		private CheckBox ConvergenceOfFatesBox;

		// DEF cds
		private CheckBox ExhilarationBox;
		private CheckBox FeignDeathBox;
		private CheckBox AspectoftheTurtleBox;	

		private CheckBox KickBox;		
		
		//dps cds
		private CheckBox CallOfTheWildBox;
		
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

		private static bool ConvergenceOfFates
        {
            get
            {
                var ConvergenceOfFates = ConfigFile.ReadValue("HunterBeastmastery", "ConvergenceOfFates").Trim();

                return ConvergenceOfFates != "" && Convert.ToBoolean(ConvergenceOfFates);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "ConvergenceOfFates", value.ToString()); }
        }

		private static bool Qapla
        {
            get
            {
                var Qapla = ConfigFile.ReadValue("HunterBeastmastery", "Qapla").Trim();

                return Qapla != "" && Convert.ToBoolean(Qapla);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Qapla", value.ToString()); }
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
		
        private static bool CallOfTheWild
        {
            get
            {
                var CallOfTheWild = ConfigFile.ReadValue("HunterBeastmastery", "CallOfTheWild").Trim();

                return CallOfTheWild != "" && Convert.ToBoolean(CallOfTheWild);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "CallOfTheWild", value.ToString()); }
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
			Log.Write("Y - Key for 4+ Targets on AoE", Color.Red);
           
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
		   
SettingsForm = new Form {Text = "Beast Mastery Hunter", StartPosition = FormStartPosition.CenterScreen, Width = 400, Height = 650, ShowIcon = false};

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

			 
			var lblCallOfTheWildBox = new Label
            {
                Text =
                    "Call Of The Wild",
                Size = new Size(270, 15),
                Left = 100,
                Top = 450
            };
			
			lblCallOfTheWildBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblCallOfTheWildBox);			
           
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

			var lblQaplaBox = new Label
            {
                Text =
                    "Qa'pla, Eredun War Order",
                Size = new Size(270, 15),
                Left = 100,
                Top = 400
            };
			
			lblQaplaBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblQaplaBox);	

			var lblConvergenceOfFatesBox = new Label
            {
                Text =
                    "Convergence of Fates",
                Size = new Size(270, 15),
                Left = 100,
                Top = 425
            };
			
			lblConvergenceOfFatesBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblConvergenceOfFatesBox);				
		   
			
			
			
			
			var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 5, Top = 500, Size = new Size(120, 31)};
			
			var cmdReadme = new Button {Text = "Macros! Use Them", Width = 65, Height = 25, Left = 125, Top = 500, Size = new Size(120, 31)};
			
 

//items
            KilJaedenBox = new CheckBox {Checked = KilJaeden, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 325};	
			SettingsForm.Controls.Add(KilJaedenBox);
            PotionBox = new CheckBox {Checked = Potion, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 350};				
            SettingsForm.Controls.Add(PotionBox);
            PotBox = new CheckBox {Checked = Pot, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 375};				
            SettingsForm.Controls.Add(PotBox);	
            QaplaBox = new CheckBox {Checked = Qapla, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 400};				
            SettingsForm.Controls.Add(QaplaBox);	
            ConvergenceOfFatesBox = new CheckBox {Checked = ConvergenceOfFates, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 425};				
            SettingsForm.Controls.Add(ConvergenceOfFatesBox);				
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
            CallOfTheWildBox = new CheckBox {Checked = CallOfTheWild, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 450};
            SettingsForm.Controls.Add(CallOfTheWildBox);			

			
			
			KickBox.Checked = Kick;	
			ExhilarationBox.Checked = Exhilaration;	
			FeignDeathBox.Checked = FeignDeath;	
			AspectoftheTurtleBox.Checked = AspectoftheTurtle;	
			
			CallOfTheWildBox.Checked = CallOfTheWild;

			

		
			
			//cmdSave

			
            KilJaedenBox.CheckedChanged += KilJaeden_Click;    
            QaplaBox.CheckedChanged += Qapla_Click;   
            ConvergenceOfFatesBox.CheckedChanged += ConvergenceOfFates_Click;   			
			PotionBox.CheckedChanged += Potion_Click;  
			PotBox.CheckedChanged += Pot_Click; 			
            HealPetBox.CheckedChanged += HealPet_Click;		
            IntimidationBox.CheckedChanged += Intimidation_Click;				
			
            CallOfTheWildBox.CheckedChanged += CallOfTheWild_Click;    
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
			
            CallOfTheWildBox.BringToFront();	
            KickBox.BringToFront();	
            ExhilarationBox.BringToFront();
            FeignDeathBox.BringToFront();
            AspectoftheTurtleBox.BringToFront();				
			

			
			
		}
			
			private void CmdSave_Click(object sender, EventArgs e)
        {


            KilJaeden = KilJaedenBox.Checked;	
            Qapla = QaplaBox.Checked;
            ConvergenceOfFates = ConvergenceOfFatesBox.Checked;			
			
            Potion = PotionBox.Checked;	
            Pot = PotionBox.Checked;				
            HealPet = HealPetBox.Checked;			
            Intimidation = IntimidationBox.Checked;				
			
            CallOfTheWild = CallOfTheWildBox.Checked;		
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
		private void Qapla_Click(object sender, EventArgs e)
        {
            Qapla = QaplaBox.Checked;
        }
		private void ConvergenceOfFates_Click(object sender, EventArgs e)
        {
            ConvergenceOfFates = ConvergenceOfFatesBox.Checked;
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
        private void CallOfTheWild_Click(object sender, EventArgs e)
        {
            CallOfTheWild = CallOfTheWildBox.Checked;
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
//Log.Write("FocusRegen*SpellCooldownTimeRemainingKill Command" + FocusRegen*WoW.SpellCooldownTimeRemaining("Kill Command")  , Color.Red);
//Log.Write("focusregdire" + (FocusRegen+DireBeastCount*1.5) , Color.Red);
//Log.Write("focusreg" + (FocusRegen)  , Color.Red);

//Log.Write("timetomax" + (((120f - WoW.Focus) /(FocusRegen+DireBeastCount*1.5)) *100) , Color.Red);
//Log.Write("timetomax" + (((120f - WoW.Focus) /((10f* (1f + (WoW.HastePercent / 100f)))+WoW.PlayerBuffStacks("Dire Beast")*1.5)) *100f) , Color.Red);




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
                        if (WoW.CanCast("Counter Shot") && WoW.Level >= 32 && Kick
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
					if (WoW.CanCast("Kil'jaeden's Burning Wish")&& UseCooldowns && KilJaeden && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish") && !WoW.IsSpellOnCooldown("Kil'jaeden's Burning Wish"))  
                    {
                        WoW.CastSpell("Kil'jaeden's Burning Wish");
                        return;
                    }	
					
					if (WoW.CanCast("Berserking") 
						&& WoW.PlayerHasBuff("Bestial Wrath")
						&& WoW.PlayerBuffTimeRemaining("Bestial Wrath") > 700
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
						&& WoW.PlayerHasBuff("Bestial Wrath")
						&& WoW.PlayerBuffTimeRemaining("Bestial Wrath") > 700
						&& !WoW.IsSpellOnCooldown ("Blood Fury")
						&& WoW.PlayerRace == "Orc")
                    {
                        WoW.CastSpell("Blood Fury");
                        return;
                    }	

				
			if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave)  
            {

			if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted && !WoW.PlayerHasBuff("AspectoftheTurtle"))
                {	
						if (WoW.PlayerHasBuff("Parsels Tongue") && WoW.PlayerBuffTimeRemaining("Parsels Tongue") <= GCD*2)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }		
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
//	a_murder_of_crows,if=cooldown.bestial_wrath.remains<3|cooldown.bestial_wrath.remains>30|target.time_to_die<16					
					if (WoW.CanCast("A Murder of Crows") 
						&& WoW.Talent(6) == 1 && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") < 300 || WoW.SpellCooldownTimeRemaining ("Bestial Wrath") > 3000)
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
//bestial_wrath if=!buff.bestial_wrath.up
					if (WoW.CanCast("Bestial Wrath") && !WoW.PlayerHasBuff("Bestial Wrath") && WoW.Level >= 40&&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        return;
                    }					
//aspect_of_the_wild,if=(equipped.call_of_the_wild&equipped.convergence_of_fates&talent.one_with_the_pack.enabled)|buff.bestial_wrath.remains>7|target.time_to_die<12	
					if (UseCooldowns && WoW.CanCast("Aspect of the Wild") && ((ConvergenceOfFates && CallOfTheWild && WoW.Talent(4)==1) || WoW.PlayerBuffTimeRemaining("Bestial Wrath") >700) && WoW.IsSpellInRange("Cobra Shot") && !WoW.IsSpellOnCooldown("Aspect of the Wild"))
                    {
                        WoW.CastSpell("Aspect of the Wild");
                        return;
                    }					
//kill_command,if=equipped.qapla_eredun_war_order
					if (WoW.CanCast("Kill Command") && Qapla && (WoW.Focus >= 30 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 25)) && WoW.Level >= 10&&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }					
//dire_beast,if=((!equipped.qapla_eredun_war_order|cooldown.kill_command.remains>=3)&(set_bonus.tier19_2pc|!buff.bestial_wrath.up))|full_recharge_time<gcd.max|cooldown.titans_thunder.up|spell_targets>1	
					if (WoW.CanCast("Dire Beast") && !WoW.PlayerHasBuff("Bestial Wrath")&& WoW.Level >= 12 && WoW.Talent(2) != 2 &&(((!Qapla || WoW.SpellCooldownTimeRemaining("Kill Command")>=100)&& (WoW.SetBonus(19) >= 2 || !WoW.PlayerHasBuff("Bestial Wrath"))) || (WoW.PlayerSpellCharges("Dire Beast") >=2) || (WoW.IsSpellOnCooldown("Titan's Thunder"))) &&WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Dire Beast");
                        return;
                    }
					if (WoW.CanCast("Dire Beast") && WoW.PlayerHasBuff("Bestial Wrath") && WoW.SpellCooldownTimeRemaining("Kill Command") > GCD && WoW.Level >= 12 && WoW.Talent(2) != 2 &&(((!Qapla || WoW.SpellCooldownTimeRemaining("Kill Command")>=100)&& (WoW.SetBonus(19) >= 2 || !WoW.PlayerHasBuff("Bestial Wrath"))) || (WoW.PlayerSpellCharges("Dire Beast") >=2) || (WoW.IsSpellOnCooldown("Titan's Thunder"))) &&WoW.IsSpellInRange("Cobra Shot"))
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
					



	
//titans_thunder,if=																																		(talent.dire_frenzy.enabled&(buff.bestial_wrath.up|cooldown.bestial_wrath.remains>35))|buff.bestial_wrath.up	
					if (WoW.CanCast("Titan's Thunder") && WoW.IsSpellInRange("Cobra Shot")&& !WoW.IsSpellOnCooldown("Titan's Thunder")&& WoW.Level >= 110&& ((WoW.Talent(2) == 2 && ( WoW.PlayerHasBuff("Bestial Wrath") || WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 3500)) || WoW.PlayerHasBuff("Bestial Wrath")))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }					
	

	    
//kill_command
					if (WoW.CanCast("Kill Command") && (WoW.Focus >= 30 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 25)) && WoW.Level >= 10&&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }					


				if(WoW.CanCast("Cobra Shot") && ((WoW.SpellCooldownTimeRemaining("Kill Command") > GCD && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > GCD) || (WoW.PlayerHasBuff("Bestial Wrath"))) && WoW.IsSpellOnCooldown("Kill Command") && (WoW.Focus > 32 || (WoW.PlayerHasBuff("Roar of the Seven Lions") && WoW.Focus >= 25))&& WoW.IsSpellInRange("Cobra Shot"))
				{
//(cooldown.kill_command.remains>focus.time_to_max&cooldown.bestial_wrath.remains>focus.time_to_max)					
						if ( !WoW.PlayerHasBuff("Aspect of the Wild")&&(WoW.SpellCooldownTimeRemaining("Kill Command") > (((120f - WoW.Focus) /(FocusRegen+DireBeastCount*1.5)) *100)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (((120f - WoW.Focus) /(FocusRegen+DireBeastCount*1.5)) *100)))
                    {
							
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }
					//with AotW
						if ((WoW.SpellCooldownTimeRemaining("Kill Command") > (((120f - WoW.Focus) /((FocusRegen+DireBeastCount*1.5)+10)) *100)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (((120f - WoW.Focus) /((FocusRegen+DireBeastCount*1.5)+10)) *100))&& WoW.PlayerHasBuff("Aspect of the Wild"))
                    {						
                        WoW.CastSpell("Cobra Shot");
                        return;			
                    }	
// focus.regen*cooldown.kill_command.remains>action.kill_command.cost))						
			/*			if (WoW.PlayerHasBuff("Bestial Wrath") && (((((FocusRegen+DireBeastCount*1.5)*WoW.SpellCooldownTimeRemaining("Kill Command")) > 2550) && WoW.PlayerHasBuff("Roar of the Seven Lions")) || ((((FocusRegen+DireBeastCount*1.5)*WoW.SpellCooldownTimeRemaining("Kill Command")) > 2550) && !WoW.PlayerHasBuff("Roar of the Seven Lions"))))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }
					*/
					//(buff.bestial_wrath.up&(spell_targets.multishot=1
						if (WoW.PlayerHasBuff("Bestial Wrath") && WoW.IsSpellOnCooldown("Kill Command"))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }					
	
						if (WoW.PlayerHasBuff("Parsels Tongue") && WoW.PlayerBuffTimeRemaining("Parsels Tongue") <= GCD*2)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }						
					
				}	
                }
            }

            if (combatRoutine.Type == RotationType.AOE)
            {
	
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerHasBuff("AspectoftheTurtle"))
                {
																					
						if (WoW.PlayerHasBuff("Parsels Tongue") && WoW.PlayerBuffTimeRemaining("Parsels Tongue") <= GCD*2)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }				
                    if (WoW.CanCast("Barrage") 
						&& WoW.Talent(6) == 2 && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") >= GCD
						&& !WoW.IsSpellOnCooldown("Barrage") 
						&& WoW.IsSpellInRange("Cobra Shot") 
						&& WoW.Focus >= 60)
                    {
                        WoW.CastSpell("Barrage");
                        return;
                    }	
					
					if (WoW.CanCast("A Murder of Crows") 
						&& WoW.Talent(6) == 1
						&& WoW.Focus >= 46-FocusRegen && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") < 300 || WoW.SpellCooldownTimeRemaining ("Bestial Wrath") > 3000)
						&& WoW.PetBuffTimeRemaining("Beast Cleave") > GCD
						&& WoW.IsSpellInRange("Cobra Shot")
						&& !WoW.IsSpellOnCooldown("A Murder of Crows")	)
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }
					
//	stampede,if=buff.bloodlust.up|buff.bestial_wrath.up|cooldown.bestial_wrath.remains<=2|target.time_to_die<=14	
					if (WoW.CanCast("Stampede") && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") >= GCD&& WoW.Talent(7) == 1 && WoW.IsSpellInRange("Cobra Shot") && ((WoW.PlayerHasBuff("Bestial Wrath")) || (WoW.SpellCooldownTimeRemaining("Bestial Wrath") <=2))						
						&& !WoW.PlayerHasBuff("AspectoftheTurtle")
						&& !WoW.IsSpellOnCooldown("Stampede")) 
                    {
                        WoW.CastSpell("Stampede");
						return;
                    }				
//bestial_wrath if=!buff.bestial_wrath.up
					if (WoW.CanCast("Bestial Wrath") && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") >= GCD&& !WoW.PlayerHasBuff("Bestial Wrath") && WoW.Level >= 40&&  WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        return;
                    }					
//aspect_of_the_wild,if=(equipped.call_of_the_wild&equipped.convergence_of_fates&talent.one_with_the_pack.enabled)|buff.bestial_wrath.remains>7|target.time_to_die<12	
					if (UseCooldowns && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") >= GCD&& WoW.CanCast("Aspect of the Wild") && ((ConvergenceOfFates && CallOfTheWild && WoW.Talent(4)==1) || WoW.PlayerBuffTimeRemaining("Bestial Wrath") >700) && WoW.IsSpellInRange("Cobra Shot") && !WoW.IsSpellOnCooldown("Aspect of the Wild"))
                    {
                        WoW.CastSpell("Aspect of the Wild");
                        return;
                    }					
//kill_command,if=equipped.qapla_eredun_war_order
					if (WoW.CanCast("Kill Command") && Qapla && ((WoW.Focus >= (70 - (FocusRegen+DireBeastCount*1.5))) || (WoW.PlayerHasBuff("Roar of the Seven Lions") && (WoW.Focus >= (59-(FocusRegen+DireBeastCount*1.5))))) && WoW.Level >= 10&&  WoW.IsSpellInRange("Cobra Shot") && ((WoW.PetHasBuff("Beast Cleave") && DetectKeyPress.GetKeyState(0x59) == 0) || (DetectKeyPress.GetKeyState(0x59) < 0 && WoW.PetBuffTimeRemaining("Beast Cleave") >GCD)))
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }					
//dire_beast,if=((!equipped.qapla_eredun_war_order|cooldown.kill_command.remains>=3)&(set_bonus.tier19_2pc|!buff.bestial_wrath.up))|full_recharge_time<gcd.max|cooldown.titans_thunder.up|spell_targets>1	
					if (WoW.CanCast("Dire Beast") && WoW.PetHasBuff("Beast Cleave") 
						&& WoW.PetBuffTimeRemaining("Beast Cleave") >= GCD && WoW.Level >= 12 && WoW.Talent(2) != 2 &&(((!Qapla || WoW.SpellCooldownTimeRemaining("Kill Command")>=100)&& (WoW.SetBonus(19) >= 2 || !WoW.PlayerHasBuff("Bestial Wrath"))) || (WoW.PlayerSpellCharges("Dire Beast") >=2) || (WoW.IsSpellOnCooldown("Titan's Thunder"))) &&WoW.IsSpellInRange("Cobra Shot"))
                    {
                        WoW.CastSpell("Dire Beast");
                        return;
                    }
//titans_thunder,if=																																		(talent.dire_frenzy.enabled&(buff.bestial_wrath.up|cooldown.bestial_wrath.remains>35))|buff.bestial_wrath.up	
					if (WoW.CanCast("Titan's Thunder") && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") >= GCD&& WoW.IsSpellInRange("Cobra Shot")&& !WoW.IsSpellOnCooldown("Titan's Thunder")&& WoW.Level >= 110&& ((WoW.Talent(2) == 2 && ( WoW.PlayerHasBuff("Bestial Wrath") || WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 3500)) || WoW.PlayerHasBuff("Bestial Wrath")))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }						
				
//kill_command
					if (WoW.CanCast("Kill Command") && ((WoW.Focus >= (70 - (FocusRegen+DireBeastCount*1.5))) || (WoW.PlayerHasBuff("Roar of the Seven Lions") && (WoW.Focus >= (59-(FocusRegen+DireBeastCount*1.5))))) && WoW.Level >= 10&&  WoW.IsSpellInRange("Cobra Shot") && ((WoW.PetHasBuff("Beast Cleave") && DetectKeyPress.GetKeyState(0x59) == 0) || (DetectKeyPress.GetKeyState(0x59) < 0 && WoW.PetBuffTimeRemaining("Beast Cleave") >GCD)))
                    {
                        WoW.CastSpell("Kill Command");
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

				if(WoW.CanCast("Cobra Shot") &&((WoW.SpellCooldownTimeRemaining("Kill Command") > GCD && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > GCD) || (WoW.PlayerHasBuff("Bestial Wrath"))) && WoW.PetHasBuff("Beast Cleave") && WoW.PetBuffTimeRemaining("Beast Cleave") > GCD && WoW.IsSpellOnCooldown("Kill Command") && ((WoW.Focus >= (72 - (FocusRegen+DireBeastCount*1.5))) || (WoW.PlayerHasBuff("Roar of the Seven Lions") && (WoW.Focus >= (61-(FocusRegen+DireBeastCount*1.5)))))&& WoW.IsSpellInRange("Cobra Shot"))
				{
//(cooldown.kill_command.remains>focus.time_to_max&cooldown.bestial_wrath.remains>focus.time_to_max)					
						if (!WoW.PlayerHasBuff("Aspect of the Wild")&&(WoW.SpellCooldownTimeRemaining("Kill Command") > (((120f - WoW.Focus) /(FocusRegen+DireBeastCount*1.5)) *100)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (((120f - WoW.Focus) /(FocusRegen+DireBeastCount*1.5)) *100)))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }
					//with AotW
						if ((WoW.SpellCooldownTimeRemaining("Kill Command") > (((120f - WoW.Focus) /((FocusRegen+DireBeastCount*1.5)+10)) *100)) && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > (((120f - WoW.Focus) /((FocusRegen+DireBeastCount*1.5)+10)) *100))&& WoW.PlayerHasBuff("Aspect of the Wild"))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
// focus.regen*cooldown.kill_command.remains>action.kill_command.cost))						
				/*		if (WoW.PlayerHasBuff("Bestial Wrath") && (((((FocusRegen+DireBeastCount*1.5)*WoW.SpellCooldownTimeRemaining("Kill Command")) > 2550) && WoW.PlayerHasBuff("Roar of the Seven Lions")) || ((((FocusRegen+DireBeastCount*1.5)*WoW.SpellCooldownTimeRemaining("Kill Command")) > 2550) && !WoW.PlayerHasBuff("Roar of the Seven Lions"))))
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }	
*/					
	
						if (WoW.PlayerHasBuff("Parsels Tongue") && WoW.PlayerBuffTimeRemaining("Parsels Tongue") <= GCD*2)
                    {			
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }						
					
				}
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

Aura,120694,Dire Beast
Aura,248085,Parsels Tongue
Aura,217200,Dire Frenzy
Aura,246152,DireFrenzy1
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

Item,144259,Kil'jaeden's Burning Wish
Item,142117,Pot
Item,5512,Healthstone
Item,127834,Potion
*/
