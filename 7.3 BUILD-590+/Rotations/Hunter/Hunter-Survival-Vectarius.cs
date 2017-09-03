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
		private NumericUpDown nudHealPetPercentValue;
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


private float FocusTimetoMax
{
     get
     {
         return ((120f - WoW.Focus) /(10f* (1f + (WoW.HastePercent / 100f)))) *100f;
     }
}



//Pet Control	
		private CheckBox HealPetBox;
		private CheckBox IntimidationBox;
		// Items
		private CheckBox KilJaedenBox;
		private CheckBox RacialsBox;		
		private CheckBox PotionBox;
		private CheckBox PotBox;
		private CheckBox FrizzosBox; 
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
                var Pot = ConfigFile.ReadValue("HunterSurvival", "Pot").Trim();

                return Pot != "" && Convert.ToBoolean(Pot);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "Pot", value.ToString()); }
        }
		
		private static bool Racials
        {
            get
            {
                var Racial = ConfigFile.ReadValue("HunterSurvival", "Racial").Trim();

                return Racial != "" && Convert.ToBoolean(Racial);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "Racial", value.ToString()); }
        }		
		
		private static bool Potion
        {
            get
            {
                var Potion = ConfigFile.ReadValue("HunterSurvival", "Potion").Trim();

                return Potion != "" && Convert.ToBoolean(Potion);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "Potion", value.ToString()); }
        }
		
		private static bool KilJaeden
        {
            get
            {
                var KilJaeden = ConfigFile.ReadValue("HunterSurvival", "KilJaeden").Trim();

                return KilJaeden != "" && Convert.ToBoolean(KilJaeden);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "KilJaeden", value.ToString()); }
        }

		private static bool ConvergenceOfFates
        {
            get
            {
                var ConvergenceOfFates = ConfigFile.ReadValue("HunterSurvival", "ConvergenceOfFates").Trim();

                return ConvergenceOfFates != "" && Convert.ToBoolean(ConvergenceOfFates);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "ConvergenceOfFates", value.ToString()); }
        }

		private static bool Frizzos
        {
            get
            {
                var Frizzos = ConfigFile.ReadValue("HunterSurvival", "Frizzos").Trim();

                return Frizzos != "" && Convert.ToBoolean(Frizzos);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "Frizzos", value.ToString()); }
        }		
		
        private static bool HealPet
        {
            get
            {
                var HealPet = ConfigFile.ReadValue("HunterSurvival", "HealPet").Trim();

                return HealPet != "" && Convert.ToBoolean(HealPet);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "HealPet", value.ToString()); }
        }
		
        private static bool Intimidation
        {
            get
            {
                var Intimidation = ConfigFile.ReadValue("HunterSurvival", "Intimidation").Trim();

                return Intimidation != "" && Convert.ToBoolean(Intimidation);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "Intimidation", value.ToString()); }
        }		
		
		

		
        private static bool Kick
        {
            get
            {
                var Kick = ConfigFile.ReadValue("HunterSurvival", "Kick").Trim();

                return Kick != "" && Convert.ToBoolean(Kick);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "Kick", value.ToString()); }
        }	
		
        private static bool Exhilaration
        {
            get
            {
                var Exhilaration = ConfigFile.ReadValue("HunterSurvival", "Exhilaration").Trim();

                return Exhilaration != "" && Convert.ToBoolean(Exhilaration);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "Exhilaration", value.ToString()); }
        }	
		
        private static bool FeignDeath
        {
            get
            {
                var FeignDeath = ConfigFile.ReadValue("HunterSurvival", "FeignDeath").Trim();

                return FeignDeath != "" && Convert.ToBoolean(FeignDeath);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "FeignDeath", value.ToString()); }
        }	

        private static bool AspectoftheTurtle
        {
            get
            {
                var AspectoftheTurtle = ConfigFile.ReadValue("HunterSurvival", "AspectoftheTurtle").Trim();

                return AspectoftheTurtle != "" && Convert.ToBoolean(AspectoftheTurtle);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "AspectoftheTurtle", value.ToString()); }
        }				
		
        private static bool CallOfTheWild
        {
            get
            {
                var CallOfTheWild = ConfigFile.ReadValue("HunterSurvival", "CallOfTheWild").Trim();

                return CallOfTheWild != "" && Convert.ToBoolean(CallOfTheWild);
            }
            set { ConfigFile.WriteValue("HunterSurvival", "CallOfTheWild", value.ToString()); }
        }		
		

		

        
      public override string Name
        {
            get { return "Hunter Survival"; }
        }

        
		 public override string Class
        {
            get { return "Hunter"; }
        }

        public override Form SettingsForm { get; set; }
		
        public override void Initialize()
        {
			Log.Write("Auto AoE optimized for WQs", Color.Green);	
           
			if (ConfigFile.ReadValue("HunterSurvival", "AspectoftheTurtle Percent") == "")
            {
                ConfigFile.WriteValue("HunterSurvival", "AspectoftheTurtle Percent", "15");
            }
			if (ConfigFile.ReadValue("HunterSurvival", "HealPet Percent") == "")
            {
                ConfigFile.WriteValue("HunterSurvival", "HealPet Percent", "80");
            }			
			if (ConfigFile.ReadValue("HunterSurvival", "FeignDeath Percent") == "")
            {
                ConfigFile.WriteValue("HunterSurvival", "FeignDeath Percent", "5");
            }
			if (ConfigFile.ReadValue("HunterSurvival", "Exhilaration Percent") == "")
            {
                ConfigFile.WriteValue("HunterSurvival", "Exhilaration Percent", "45");
            }
			if (ConfigFile.ReadValue("HunterSurvival", "Kick Percent") == "")
            {
                ConfigFile.WriteValue("HunterSurvival", "Kick Percent", "65");
            }	
			if (ConfigFile.ReadValue("HunterSurvival", "Intimidation Percent") == "")
            {
                ConfigFile.WriteValue("HunterSurvival", "Intimidation Percent", "80");
            }	
			if (ConfigFile.ReadValue("HunterSurvival", "Potion Percent") == "")
            {
                ConfigFile.WriteValue("HunterSurvival", "Potion Percent", "30");
            }			
		   
SettingsForm = new Form {Text = "Survival Hunter", StartPosition = FormStartPosition.CenterScreen, Width = 400, Height = 650, ShowIcon = false};

            nudAspectoftheTurtlePercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("HunterSurvival", "AspectoftheTurtle Percent"), 
			Left = 215, 
			Top = 172,
			Size = new Size (40, 10)
			}; 
			SettingsForm.Controls.Add(nudAspectoftheTurtlePercentValue);
			
            nudHealPetPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("HunterSurvival", "HealPet Percent"), 
			Left = 215, 
			Top = 247,
			Size = new Size (40, 10)
			}; 
			SettingsForm.Controls.Add(nudHealPetPercentValue);		
		

            nudExhilarationPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("HunterSurvival", "Exhilaration Percent"), 
			Left = 215, 
			Top = 122,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudExhilarationPercentValue);
			
			
		

            nudFeignDeathPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("HunterSurvival", "FeignDeath Percent"), 
			Left = 215, 
			Top =147,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudFeignDeathPercentValue);

            nudKickPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("HunterSurvival", "Kick Percent"), 
			Left = 215, 
			Top =100,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudKickPercentValue);			

            nudIntimidationPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("HunterSurvival", "Intimidation Percent"), 
			Left = 215, 
			Top =272,
			Size = new Size (40, 10)
			};
			SettingsForm.Controls.Add(nudIntimidationPercentValue);			

            nudPotionPercentValue = new NumericUpDown 
			{Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("HunterSurvival", "Potion Percent"), 
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
                    "Heal Pet @",
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
                    "Healthstone @",
                Size = new Size(270, 15),
                Left = 100,
                Top = 350
            };
			
			lblPotionBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblPotionBox);				
		   
			var lblPotBox = new Label
            {
                Text =
                    "Potion of p.Power with BL",
                Size = new Size(270, 15),
                Left = 100,
                Top = 375
            };
			
			lblPotBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblPotBox);			

			var lblFrizzosBox = new Label
            {
                Text =
                    "Frizzos Fingertrap",
                Size = new Size(270, 15),
                Left = 100,
                Top = 400
            };
			
			lblFrizzosBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblFrizzosBox);	

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

			var lblRacialsBox = new Label
            {
                Text =
                    "Use Racials",
                Size = new Size(270, 15),
                Left = 100,
                Top = 200
            };
			
			lblRacialsBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblRacialsBox);			
		   
			
			
			
			
			var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 5, Top = 520, Size = new Size(120, 31)};
			
			var cmdReadme = new Button {Text = "Macros! Use Them", Width = 65, Height = 25, Left = 125, Top = 520, Size = new Size(120, 31)};
			
 

//items
            KilJaedenBox = new CheckBox {Checked = KilJaeden, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 325};	
			SettingsForm.Controls.Add(KilJaedenBox);
            PotionBox = new CheckBox {Checked = Potion, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 350};				
            SettingsForm.Controls.Add(PotionBox);
            PotBox = new CheckBox {Checked = Pot, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 375};				
            SettingsForm.Controls.Add(PotBox);	
            FrizzosBox = new CheckBox {Checked = Frizzos, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 400};				
            SettingsForm.Controls.Add(FrizzosBox);	
            ConvergenceOfFatesBox = new CheckBox {Checked = ConvergenceOfFates, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 425};				
            SettingsForm.Controls.Add(ConvergenceOfFatesBox);	
            RacialsBox = new CheckBox {Checked = Racials, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 200};				
            SettingsForm.Controls.Add(RacialsBox);			
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
            FrizzosBox.CheckedChanged += Frizzos_Click;  
			RacialsBox.CheckedChanged += Racials_Click; 			
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
			nudHealPetPercentValue.BringToFront();
			nudAspectoftheTurtlePercentValue.BringToFront();
			nudFeignDeathPercentValue.BringToFront();		
			
            KilJaedenBox.BringToFront();
			RacialsBox.BringToFront();
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
			Racials = RacialsBox.Checked;
            Frizzos = FrizzosBox.Checked;
            ConvergenceOfFates = ConvergenceOfFatesBox.Checked;			
			
            Potion = PotionBox.Checked;	
            Pot = PotBox.Checked;				
            HealPet = HealPetBox.Checked;			
            Intimidation = IntimidationBox.Checked;				
			
            CallOfTheWild = CallOfTheWildBox.Checked;		
            Kick = KickBox.Checked;	
            Exhilaration = ExhilarationBox.Checked;
            FeignDeath = FeignDeathBox.Checked;
            AspectoftheTurtle = AspectoftheTurtleBox.Checked;			
			
            ConfigFile.WriteValue("HunterSurvival", "AspectoftheTurtle Percent", nudAspectoftheTurtlePercentValue.Value.ToString());
			ConfigFile.WriteValue("HunterSurvival", "HealPet Percent", nudHealPetPercentValue.Value.ToString());
	        ConfigFile.WriteValue("HunterSurvival", "FeignDeath Percent", nudFeignDeathPercentValue.Value.ToString());		
            ConfigFile.WriteValue("HunterSurvival", "Exhilaration Percent", nudExhilarationPercentValue.Value.ToString());		
			ConfigFile.WriteValue("HunterSurvival", "Intimidation Percent", nudIntimidationPercentValue.Value.ToString());		
			ConfigFile.WriteValue("HunterSurvival", "Kick Percent", nudKickPercentValue.Value.ToString());	
			ConfigFile.WriteValue("HunterSurvival", "Potion Percent", nudPotionPercentValue.Value.ToString());	
		
	
			
            MessageBox.Show("Settings saved.", "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsForm.Close();
        }
		private void CmdReadme_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                " make sure you make macros of Kill Command and Dire Frenzy/Beast /petattack",
                "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
	

		
//items
		private void KilJaeden_Click(object sender, EventArgs e)
        {
            KilJaeden = KilJaedenBox.Checked;
        }
		private void Frizzos_Click(object sender, EventArgs e)
        {
            Frizzos = FrizzosBox.Checked;
        }
		private void ConvergenceOfFates_Click(object sender, EventArgs e)
        {
            ConvergenceOfFates = ConvergenceOfFatesBox.Checked;
        }
		private void Racials_Click(object sender, EventArgs e)
        {
            Racials = RacialsBox.Checked;
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


		public override int AoE_Range  
        {
            get 
            { 
                return 8; 
            } 
        }
        public override int Interrupt_Ability_Id 
        { 
            get 
            { 
                return 187707; 
            }
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
				     if (WoW.CanCast("Feign Death") && WoW.Level >= 28&& WoW.HealthPercent <= ConfigFile.ReadValue<int>("HunterSurvival", "FeignDeath Percent") && FeignDeath && !WoW.IsSpellOnCooldown("Feign Death") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Feign Death");
                        return;
                    }
                    if (WoW.CanCast("Exhilaration") && WoW.Level >= 24 && WoW.HealthPercent <= ConfigFile.ReadValue<int>("HunterSurvival", "Exhilaration Percent") && Exhilaration && !WoW.IsSpellOnCooldown("Exhilaration") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Exhilaration");
                        return;
                    }	
					if (WoW.CanCast("Aspect of the Turtle") && WoW.Level >= 70&& WoW.HealthPercent <= ConfigFile.ReadValue<int>("HunterSurvival", "Aspect of the Turtle Percent") && AspectoftheTurtle && !WoW.IsSpellOnCooldown("Aspect of the Turtle") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Aspect of the Turtle");
                        return;
                    }

                    if (KilJaeden && WoW.CanCast("Kil'jaeden's Burning Wish") && !WoW.ItemOnCooldown("Kil'jaeden's Burning Wish"))
					{
						WoW.CastSpell("Kil'jaeden's Burning Wish") ;
						return;
					}					

					if (Potion && WoW.IsInCombat && WoW.HealthPercent < ConfigFile.ReadValue<int>("HunterSurvival", "Potion Percent") && WoW.ItemCount("Healthstone") >= 1 && !WoW.ItemOnCooldown("Healthstone") && WoW.ItemCount("HealthPotion") == 0)
					{
                
                    WoW.CastSpell("Healthstone");
                    return;
					}				
                    if (Pot && BL && WoW.CanCast("Pot") && !WoW.PlayerHasBuff("Pot") && WoW.ItemCount("Pot") >= 1 && !WoW.ItemOnCooldown("Pot"))
					{
						WoW.CastSpell("Pot") ;
						return;
					}
									

// - Survival Hunter	
		
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
Log.Write("recharge time"+ mongoosebitewatch.ElapsedMilliseconds, Color.Red);	

											
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
//snake_hunter,if=cooldown.mongoose_bite.charges=0&buff.mongoose_fury.remains>3*gcd&buff.aspect_of_the_eagle.down					
						if (WoW.CanCast("Snake Hunter")&& WoW.Talent(2) == 3&& WoW.PlayerSpellCharges("Mongoose Bite") <= 0 && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") >= 3*GCD && !WoW.PlayerHasBuff("Aspect of the Eagle") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
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
//snake_hunter,if=cooldown.mongoose_bite.charges=0&buff.mongoose_fury.remains>3*gcd&buff.aspect_of_the_eagle.down					
						if (WoW.CanCast("Snake Hunter")&& WoW.Talent(2) == 3&& WoW.PlayerSpellCharges("Mongoose Bite") <= 0 && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") >= 3*GCD && !WoW.PlayerHasBuff("Aspect of the Eagle") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
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
Spell,206505,Murder of Crows,D5
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
Spell,80483,Arcane Torrent,F3
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
Aura,190931,Mongoose Fury
Aura,118253,Serpent Sting
Aura,185855,Lacerate
Aura,186289,Aspect of the Eagle
Aura,194277,Caltrops
Aura,201081,tactics
Item,144259,Kil'jaeden's Burning Wish
Item,142117,Pot
Item,5512,Healthstone
Item,127834,Potion
*/
