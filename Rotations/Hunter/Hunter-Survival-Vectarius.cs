// winifix@gmail.com
// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CloudMagic.Helpers;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace CloudMagic.Rotation
{
    public class SVHunterVectarius : CombatRoutine
    {
		private NumericUpDown nudExhilarationPercentValue;
		private NumericUpDown nudAspectoftheTurtlePercentValue;
		private NumericUpDown nudFeignDeathPercentValue;		
		
		private readonly Stopwatch tacticswatch = new Stopwatch();
		private readonly Stopwatch pullwatch = new Stopwatch();
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
private float FocusTimetoMax
{
     get
     {
         return ((120f - WoW.Focus) /(10f* (1f + (WoW.HastePercent / 100f)))) *100f;
     }
}

			//Pet Control	
		private CheckBox HealPetBox;
		// Items
		private CheckBox KilJaedenBox;			


		// DEF cds
		private CheckBox ExhilarationBox;
		private CheckBox FeignDeathBox;
		private CheckBox AspectoftheTurtleBox;	

		private CheckBox MuzzleBox;		
		
		//dps cds
		private CheckBox AspectoftheEagleBox;
		
		

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
		
		

		
        private static bool Muzzle
        {
            get
            {
                var Muzzle = ConfigFile.ReadValue("HunterBeastmastery", "Muzzle").Trim();

                return Muzzle != "" && Convert.ToBoolean(Muzzle);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "Muzzle", value.ToString()); }
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
		
        private static bool AspectoftheEagle
        {
            get
            {
                var AspectoftheEagle = ConfigFile.ReadValue("HunterBeastmastery", "AspectoftheEagle").Trim();

                return AspectoftheEagle != "" && Convert.ToBoolean(AspectoftheEagle);
            }
            set { ConfigFile.WriteValue("HunterBeastmastery", "AspectoftheEagle", value.ToString()); }
        }		
		

		

        
      public override string Name
        {
            get { return "Hunter SV"; }
        }

        
		 public override string Class
        {
            get { return "Hunter"; }
        }

        public override Form SettingsForm { get; set; }
		
		
        public override void Initialize()
        {
           
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
		   
SettingsForm = new Form {Text = "SV Hunter", StartPosition = FormStartPosition.CenterScreen, Width = 400, Height = 500, ShowIcon = false};

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
			
			


			
			var lblTitle = new Label
            {
                Text =
                    "SV Hunter by Vectarius",
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

			 
			var lblAspectoftheEagleBox = new Label
            {
                Text =
                    "AspectoftheEagle",
                Size = new Size(270, 15),
                Left = 100,
                Top = 75
            };
			
			lblAspectoftheEagleBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblAspectoftheEagleBox);			
           
			var lblMuzzleBox = new Label
            {
                Text =
                    "Muzzle",
                Size = new Size(270, 15),
                Left = 100,
                Top = 100
            };
			
			lblMuzzleBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblMuzzleBox);	

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
                Top = 275
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

			var lblKilJaedenBox = new Label
            {
                Text =
                    "Kil'Jaeden's Burning Wish",
                Size = new Size(270, 15),
                Left = 100,
                Top = 300
            };
			
			lblKilJaedenBox.ForeColor = Color.Black;
            SettingsForm.Controls.Add(lblKilJaedenBox);			
		   

			
			
			
			
			var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 5, Top = 375, Size = new Size(120, 31)};
			
			var cmdReadme = new Button {Text = "Macros! Use Them", Width = 65, Height = 25, Left = 125, Top = 375, Size = new Size(120, 31)};
			
 

//items
            KilJaedenBox = new CheckBox {Checked = KilJaeden, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 300};		
            SettingsForm.Controls.Add(KilJaedenBox);
//pet control			
			HealPetBox = new CheckBox {Checked = HealPet, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 250};			
            SettingsForm.Controls.Add(HealPetBox);
			
			// Checkboxes
            MuzzleBox = new CheckBox {Checked = Muzzle, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 100};		
            SettingsForm.Controls.Add(MuzzleBox);
			ExhilarationBox = new CheckBox {Checked = Exhilaration, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 125};			
            SettingsForm.Controls.Add(ExhilarationBox);
			FeignDeathBox = new CheckBox {Checked = FeignDeath, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 150};
            SettingsForm.Controls.Add(FeignDeathBox);
			
			AspectoftheTurtleBox = new CheckBox {Checked = AspectoftheTurtle, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 175};			
			            SettingsForm.Controls.Add(AspectoftheTurtleBox);		
			//dps cooldowns
            AspectoftheEagleBox = new CheckBox {Checked = AspectoftheEagle, TabIndex = 8, Size = new Size(14, 14), Left = 70, Top = 75};
            SettingsForm.Controls.Add(AspectoftheEagleBox);			

			
			
			MuzzleBox.Checked = Muzzle;	
			ExhilarationBox.Checked = Exhilaration;	
			FeignDeathBox.Checked = FeignDeath;	
			AspectoftheTurtleBox.Checked = AspectoftheTurtle;	
			
			AspectoftheEagleBox.Checked = AspectoftheEagle;

			

		
			
			//cmdSave

			
            KilJaedenBox.CheckedChanged += KilJaeden_Click;    
            HealPetBox.CheckedChanged += HealPet_Click;				
			
            AspectoftheEagleBox.CheckedChanged += AspectoftheEagle_Click;    
            ExhilarationBox.CheckedChanged += Exhilaration_Click; 
            MuzzleBox.CheckedChanged += Muzzle_Click;
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
            HealPetBox.BringToFront();				
			
            AspectoftheEagleBox.BringToFront();	
            MuzzleBox.BringToFront();	
            ExhilarationBox.BringToFront();
            FeignDeathBox.BringToFront();
            AspectoftheTurtleBox.BringToFront();				
			

			
			
		}
			
			private void CmdSave_Click(object sender, EventArgs e)
        {


            KilJaeden = KilJaedenBox.Checked;		
            HealPet = HealPetBox.Checked;				
			
            AspectoftheEagle = AspectoftheEagleBox.Checked;		
            Muzzle = MuzzleBox.Checked;	
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
			
//pet control			
		private void HealPet_Click(object sender, EventArgs e)
        {
            HealPet = HealPetBox.Checked;
        }	

			
		private void Muzzle_Click(object sender, EventArgs e)
        {
            Muzzle = MuzzleBox.Checked;
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
        private void AspectoftheEagle_Click(object sender, EventArgs e)
        {
            AspectoftheEagle = AspectoftheEagleBox.Checked;
        }			

		
		
        public override void Stop()
        {
			
			
        }

        private static bool lastNamePlate = true;
        public void SelectRotation(int aoe, int cleave, int single)
        {
            int count = WoW.CountEnemyNPCsInRange;
            if (!lastNamePlate)
            {
                combatRoutine.ChangeType(RotationType.SingleTarget);
                lastNamePlate = true;
            }
            lastNamePlate = WoW.Nameplates;
            if (count >= aoe)
                combatRoutine.ChangeType(RotationType.AOE);
            if (count == cleave)
                combatRoutine.ChangeType(RotationType.SingleTargetCleave);
            if (count <= single)
                combatRoutine.ChangeType(RotationType.SingleTarget);

        }       

        public override void Pulse()
        {
			if (WoW.IsInCombat && Control.IsKeyLocked(Keys.Scroll) && !WoW.TargetIsPlayer && !WoW.IsMounted)
			{	
			SelectRotation(2, 100, 1 );
			}
		if (DetectKeyPress.GetKeyState(0x6A) < 0)
            {
                UseCooldowns = !UseCooldowns;
                Thread.Sleep(150);
            }			
            if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave )  // Do Single Target Stuff here
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
	
                    if (WoW.CanCast("Feign Death") && WoW.HealthPercent < 10 && FeignDeath && !WoW.IsSpellOnCooldown("Feign Death") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Feign Death");
                        return;
                    }
                    if (WoW.CanCast("Exhilaration") && WoW.HealthPercent < 50 && Exhilaration && !WoW.IsSpellOnCooldown("Exhilaration") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Exhilaration");
                        return;
                    }	
					if (WoW.CanCast("Aspect of the Turtle") && WoW.HealthPercent < 20 && AspectoftheTurtle && !WoW.IsSpellOnCooldown("Aspect of the Turtle") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Aspect of the Turtle");
                        return;
                    }

											
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {	

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
				if(pullwatch.ElapsedMilliseconds < 10000)
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
					if (WoW.CanCast("Muzzle") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 60 && !WoW.IsSpellOnCooldown("Muzzle")&& !WoW.PlayerIsChanneling && !WoW.WasLastCasted("Muzzle"))
                    {
                            WoW.CastSpell("Muzzle");						
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
						if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") < GCD && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
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
                    if (((WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && UseCooldowns && WoW.PlayerBuffStacks("tactics") >= 2) || WoW.Talent(1) != 3)&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
                    {
						if (WoW.CanCast("Arcane Torrent")  && WoW.PlayerHasBuff("Aspect of the Eagle")&& !WoW.IsSpellOnCooldown ("Arcane Torrent")&& WoW.PlayerRace == "BloodElf"&& WoW.Focus <= 85)
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
						if (WoW.CanCast("Snake Hunter")&& WoW.Talent(2) == 3&& WoW.PlayerSpellCharges("Mongoose Bite") <= 0 && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") >= 300 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
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
					if(!WoW.PlayerHasBuff("Mongoose Fury"))
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
						if (WoW.CanCast("Raptor Strike") && !WoW.PlayerHasBuff("tactics") && !WoW.TargetHasDebuff("Serpent Sting")&& WoW.Talent(6) == 3 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();
                        return;
						}
//0.00	steel_trap
                    if (WoW.CanCast("Steel Trap") && !WoW.IsMoving && WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting && WoW.Talent(4) == 3&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Steel Trap");
                        return;
						}
//0.00	a_murder_of_crows
                    if (WoW.CanCast("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1	&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Murder of Crows");
                        return;
						}
//0.00	dragonsfire_grenade
                    if (WoW.CanCast("Dragonsfire Grenade")  && !WoW.IsMoving&& WoW.IsSpellInRange("Raptor Strike")&& !WoW.PlayerIsCasting&& WoW.Talent(6) == 2&& !WoW.PlayerIsChanneling)
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
						if (WoW.CanCast("Caltrops") && !WoW.IsMoving&& !WoW.TargetHasDebuff("Caltrops")&& WoW.Talent(4) == 1&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
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
						if (WoW.CanCast ("Fury of the Eagle") && (WoW.Talent(1) != 3 || WoW.PlayerBuffTimeRemaining("tactics") > (GCD*(8%3))) && WoW.PlayerBuffStacks ("Mongoose Fury") >= 6 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Fury of the Eagle");
                        return;
						}
//	64.01	mongoose_bite,if=charges>=2&cooldown.mongoose_bite.remains<gcd*2
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerSpellCharges("Mongoose Bite") >=2 && WoW.SpellCooldownTimeRemaining("Mongoose Bite") <GCD*2 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Mongoose Bite");
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
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerHasBuff("Mongoose Fury")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
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
					
					//0.00	spitting_cobra
						if (WoW.CanCast("Spitting Cobra")&& WoW.Focus >= 30&& WoW.Talent(7) == 1&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
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
						if (WoW.CanCast("Raptor Strike")&& !WoW.TargetHasDebuff("Serpent Sting") && WoW.Talent(6) == 3&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();
                        return;
						}
//0.00	steel_trap
						if (WoW.CanCast("Steel Trap")&& !WoW.IsMoving && WoW.IsSpellInRange("Raptor Strike")&& WoW.Talent(4) == 3&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling )
						{
                        WoW.CastSpell("Steel Trap");
                        return;
						}
//0.00	a_murder_of_crows
						if (WoW.CanCast("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
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
//FILLERS

//0.00	carve,if=active_enemies>1&talent.serpent_sting.enabled&!dot.serpent_sting.ticking
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
						if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics")&& WoW.PlayerBuffTimeRemaining("tactics")< (GCD*2) && WoW.Talent(1) == 3&& !WoW.PlayerIsCasting  && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}
//0.41	raptor_strike,if=focus>((25-focus.regen*gcd)+55)
						if (WoW.CanCast("Raptor Strike") && WoW.CanCast("Raptor Strike")&& WoW.Focus>((25-FocusRegen*GCD)+55)&& !WoW.PlayerIsCasting&& !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Raptor Strike");
						Log.Write("Too much focus! RAPTOR", Color.Red);
						
						tacticswatch.Reset();
						tacticswatch.Start();						
                        return;
						}					
				}
			}
			if (combatRoutine.Type == RotationType.AOE )  // Do Single Target Stuff here
			{
if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {	
			                    if (WoW.CanCast("Feign Death") && WoW.HealthPercent < 10 && FeignDeath && !WoW.IsSpellOnCooldown("Feign Death") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Feign Death");
                        return;
                    }
                    if (WoW.CanCast("Exhilaration") && WoW.HealthPercent < 50 && Exhilaration && !WoW.IsSpellOnCooldown("Exhilaration") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Exhilaration");
                        return;
                    }	
					if (WoW.CanCast("Aspect of the Turtle") && WoW.HealthPercent < 20 && AspectoftheTurtle && !WoW.IsSpellOnCooldown("Aspect of the Turtle") && WoW.HealthPercent != 0)
                    {
                        WoW.CastSpell("Aspect of the Turtle");
                        return;
                    }
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
									if(pullwatch.ElapsedMilliseconds < 10000)
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
						if (WoW.CanCast("Raptor Strike") && WoW.PlayerHasBuff("tactics") && WoW.PlayerBuffTimeRemaining("tactics") < GCD && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
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
                    if (((WoW.Talent(1) == 3 && WoW.PlayerHasBuff("tactics") && UseCooldowns) || WoW.Talent(1) != 3) && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
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
						if (WoW.CanCast("Snake Hunter")&& WoW.Talent(2) == 3&& WoW.PlayerSpellCharges("Mongoose Bite") <= 0 && WoW.PlayerHasBuff("Mongoose Fury") && WoW.PlayerBuffTimeRemaining("Mongoose Fury") >= 300 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Snake Hunter");
                        return;
						}	
					
                    }					
//0.00	call_action_list,name=preBitePhase,if=!buff.mongoose_fury.up
					if(!WoW.PlayerHasBuff("Mongoose Fury"))
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
                    if (WoW.CanCast("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1	&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
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
						if (WoW.CanCast ("Fury of the Eagle") && (WoW.Talent(1) != 3 || WoW.PlayerBuffTimeRemaining("tactics") > (GCD*(8%3)))  && WoW.PlayerBuffStacks ("Mongoose Fury") >= 6 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Fury of the Eagle");
                        return;
						}
//	64.01	mongoose_bite,if=charges>=2&cooldown.mongoose_bite.remains<gcd*2
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerSpellCharges("Mongoose Bite") >=2 && WoW.SpellCooldownTimeRemaining("Mongoose Bite") <GCD*2 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
						{
                        WoW.CastSpell("Mongoose Bite");
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
						if (WoW.CanCast("Mongoose Bite") && WoW.PlayerHasBuff("Mongoose Fury")&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.IsSpellInRange("Raptor Strike"))
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
					
					//0.00	spitting_cobra
						if (WoW.CanCast("Spitting Cobra")&& WoW.Focus >= 30&& WoW.Talent(7) == 1&& !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
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
						if (WoW.CanCast("Murder of Crows") && WoW.Focus >= 30&& WoW.Talent(2) == 1 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
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
						if (WoW.CanCast("Butchery") && WoW.Talent(6) == 1&& WoW.IsSpellInRange("Raptor Strike") && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
						{
                        WoW.CastSpell("Butchery");
                        return;
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


	



	

/*
[AddonDetails.db]
AddonAuthor=Vectarius
AddonName=PixelMagic
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,190928,Mongoose Bite,D1
Spell,83244,Wolf,F4
Spell,202800,Flanking Strike,D2
Spell,185855,Lacerate,D3
Spell,186270,Raptor Strike,D4
Spell,194277,Caltrops,D5
Spell,191433,Explosive Trap,D6
Spell,194855,Dragonsfire Grenade,D7
Spell,200163,Throwing Axes,D8
Spell,203415,Fury of the Eagle,D9
Spell,206505,Murder of Crows,D0
Spell,186289,Aspect of the Eagle,F1
Spell,212436,Butchery,C
Spell,187707,Muzzle,F
Spell,194407,Spitting Cobra,C
Spell,187708,Carve,F2
Spell,190925,Harpoon,G
Spell,162488,Steel Trap,D5
Spell,26297,Berserking,F9
Spell,20572,Blood Fury,F9
Spell,80483,Arcane Torrent,F9
Spell,201078,Snake Hunter,D0
Spell,109304,Exhilaration,F5
Spell,186265,Aspect of the Turtle,F6
Spell,5384,Feign Death,F7
Spell,55709,Phoenix,none
Spell,982,Revive Pet,X
Spell,136,Heal Pet,X
Aura,190931,Mongoose Fury
Aura,87935,Serpent Sting
Aura,185855,Lacerate
Aura,186289,Aspect of the Eagle
Aura,194277,Caltrops
Aura,201081,tactics
*/