// winifix@gmail.com
// ReSharper disable UnusedMember.Global


using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using CloudMagic.Helpers;

namespace CloudMagic.Rotation
{
    public class Enhancement : CombatRoutine
    {
		private static readonly Stopwatch coolDownStopWatch = new Stopwatch();
		
		private CheckBox fourt19Box;
		
		public override string Name 
		{
			get
			{				
				return "Enhancement(Tempest) by Jedix";		
			}
		}

        public override string Class 
		{
			get
			{				
				return "Shaman";		
			}
		}

        public override Form SettingsForm { get; set; }
		
		public static bool fourt19
        {
            get
            {
                var fourt19 = ConfigFile.ReadValue("Shaman Enhancement", "fourt19").Trim();

                return fourt19 != "" && Convert.ToBoolean(fourt19);
            }
            set { ConfigFile.WriteValue("Shaman Enhancement", "fourt19", value.ToString()); }
        }

        public override void Initialize()
        {
            Log.Write("Welcome to Enhancement Shaman by Jedix", Color.Green);
            Log.Write("Suggested build: 3212112", Color.Green);
			Log.Write("If you got atleast 2pieces of t19, use this rotation for most dps. Numpad * controls cooldowns on/off", Color.Green);
			Log.Write("If you got 4pieces of t19 set checkbox in options", Color.Green);
			
			SettingsForm = new Form {Text = "Settings", StartPosition = FormStartPosition.CenterScreen, Width = 480, Height = 300, ShowIcon = false};

            var fourt19Text = new Label // Four set label
            {Text = "4pieces t19", Size = new Size(81, 13), Left = 12, Top = 129};
            SettingsForm.Controls.Add(fourt19Text); //Four set TEXT
			
			fourt19Box = new CheckBox {Checked = fourt19, TabIndex = 4, Size = new Size(15, 14), Left = 115, Top = 129};
            SettingsForm.Controls.Add(fourt19Box); // Four set BOX
			
			var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 332, Top = 190, Size = new Size(120, 31)};
			
            fourt19Box.Checked = fourt19;


            cmdSave.Click += CmdSave_Click;
            fourt19Box.CheckedChanged += fourt19_Click;
            

            SettingsForm.Controls.Add(cmdSave);
            fourt19Text.BringToFront();
        }
		
		 private void CmdSave_Click(object sender, EventArgs e)
        {
            fourt19 = fourt19Box.Checked;
            MessageBox.Show("Settings saved", "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsForm.Close();
        }


        private void fourt19_Click(object sender, EventArgs e)
        {
            fourt19 = fourt19Box.Checked;
        }

        public override void Stop()
        {
        }

        public override void Pulse()
        {
			if (!coolDownStopWatch.IsRunning || coolDownStopWatch.ElapsedMilliseconds > 60000)
							coolDownStopWatch.Restart();
					if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_MULTIPLY) < 0)  //Use cooldowns manage by *numButton
					{
						if (coolDownStopWatch.ElapsedMilliseconds > 1000)
						{
							combatRoutine.UseCooldowns = !combatRoutine.UseCooldowns;

							coolDownStopWatch.Restart();
						}
					}
					
            if (combatRoutine.Type == RotationType.SingleTarget) // Do Single Target Stuff here
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerHasBuff("Ghost Wolf") && WoW.IsInCombat) //First things go first but break if we wanna run in Wolf
                {
                    if (WoW.CanCast("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Wind Shear")) //interupt every spell
                    {
                        WoW.CastSpell("Wind Shear");
                        return;
                    }

                    if (WoW.CanCast("Feral Spirit") && WoW.IsSpellInRange("Stormstrike") && combatRoutine.UseCooldowns && WoW.Maelstrom >= 20) //Wolves in melee range
                    {
                        Log.Write("Using Feral Spirit", Color.Red);
                        WoW.CastSpell("Feral Spirit");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.SpellCooldownTimeRemaining("Feral Spirit") >= 115) //Crash lightning for alpha wolf
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }	

                    if (WoW.CanCast("Boulderfist") && (!WoW.PlayerHasBuff("Landslide") || WoW.PlayerBuffTimeRemaining("Landslide") <= 1) && WoW.IsSpellInRange("Flametongue")) //REFRESH LANDSLIDE
                    {
                        Log.Write("Reseting Landslide", Color.Red);
                        WoW.CastSpell("Boulderfist");
                        return;
                    }
					
					if (WoW.CanCast("Boulderfist") && WoW.IsSpellInRange("Flametongue") && WoW.PlayerSpellCharges("Boulderfist") == 2 && WoW.Maelstrom <= 100) 
                    {    
                        WoW.CastSpell("Boulderfist"); //boulderfist it to not waste a charge
                        return;
                    }
					
                    if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue") && ((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") <= 1)) || (WoW.SpellCooldownTimeRemaining("Doom Winds") <= 6 && WoW.PlayerBuffTimeRemaining("Flametongue") <= 4)) //REFRESH FLAMETONGUE
                    {
                        Log.Write("Reseting Flametongue buff", Color.Red);
                        WoW.CastSpell("Flametongue");
                        return;
                    }								
					
					if (WoW.CanCast("Doom Winds") && combatRoutine.UseCooldowns && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Flametongue"))
                    {
                        WoW.CastSpell("Doom Winds");
                        return;
                    }
					
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 20)
                    {
                        WoW.CastSpell("Stormstrike");
                        return;
                    }
				
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 40)
					{
						WoW.CastSpell("Stormstrike");
						return;
					}

					if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && (WoW.Maelstrom >= 80 || (fourt19 && WoW.Maelstrom >= 40)))
					{
						WoW.CastSpell("Lava Lash"); //Buffer Lava
						return;
					}
					
					if (WoW.CanCast("Flametongue") && WoW.PlayerBuffTimeRemaining("Flametongue") <= 5 && WoW.IsSpellInRange("Flametongue"))
					{
						WoW.CastSpell("Flametongue"); //REFRESH FLAMETONGUE PANDEMIC
						return;
					}
					
					if (WoW.CanCast("Boulderfist") && WoW.IsSpellInRange("Flametongue")) 
					{
						WoW.CastSpell("Boulderfist"); //Nothing to do
						return;
					}
						
					if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue")) 
					{
						WoW.CastSpell("Flametongue"); //Nothing to do 
						return;
					}
					
					if (WoW.CanCast("Feral Lunge") && !WoW.IsSpellInRange("Flametongue") && WoW.IsSpellInRange("Feral Lunge")) //out of range of flametongue 10y and in range of feral lunge 8-25y 
                    {
                        WoW.CastSpell("Feral Lunge");
                        return;
                    }
					
					if (WoW.CanCast("Lightning Bolt") && !WoW.IsSpellInRange("Flametongue")) //out of range cast LB if we are 10y away and cannot jump by Feral Lunge
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }

                    if (WoW.CanCast("Astral Shift") && WoW.HealthPercent < 60) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
                    {
                        WoW.CastSpell("Astral Shift");
                        return;
                    }
                }
            }
            if (combatRoutine.Type == RotationType.SingleTargetCleave)
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerHasBuff("Ghost Wolf") && WoW.IsInCombat) //First things go first
                {
                    if (WoW.CanCast("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Wind Shear")) //interupt every spell
                    {
                        WoW.CastSpell("Wind Shear");
                        return;
                    }

                    if (WoW.CanCast("Feral Spirit") && WoW.IsSpellInRange("Stormstrike") && combatRoutine.UseCooldowns && WoW.Maelstrom >= 20) //Wolves in melee range
                    {
                        Log.Write("Using Feral Spirit", Color.Red);
                        WoW.CastSpell("Feral Spirit");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.SpellCooldownTimeRemaining("Feral Spirit") >= 115) //Crash lightning for alpha wolf
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }	

                    if (WoW.CanCast("Boulderfist") && (!WoW.PlayerHasBuff("Landslide") || WoW.PlayerBuffTimeRemaining("Landslide") <= 1) && WoW.IsSpellInRange("Flametongue")) //REFRESH LANDSLIDE
                    {
                        Log.Write("Reseting Landslide", Color.Red);
                        WoW.CastSpell("Boulderfist");
                        return;
                    }
					
					if (WoW.CanCast("Boulderfist") && WoW.IsSpellInRange("Flametongue") && WoW.PlayerSpellCharges("Boulderfist") == 2 && WoW.Maelstrom <= 100) 
                    {    
                        WoW.CastSpell("Boulderfist"); //boulderfist it to not waste a charge
                        return;
                    }
					
                    if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue") && ((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") <= 1)) || (WoW.SpellCooldownTimeRemaining("Doom Winds") <= 6 && WoW.PlayerBuffTimeRemaining("Flametongue") <= 4)) //REFRESH FLAMETONGUE
                    {
                        Log.Write("Reseting Flametongue buff", Color.Red);
                        WoW.CastSpell("Flametongue");
                        return;
                    }								
					
					if (WoW.CanCast("Doom Winds") && combatRoutine.UseCooldowns && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Flametongue"))
                    {
                        WoW.CastSpell("Doom Winds");
                        return;
                    }
					
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 20)
                    {
                        WoW.CastSpell("Stormstrike");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Stormstrike") && !WoW.PlayerHasBuff("Crash Lightning")) //Crash lightning for cleave to enable storm
                    {
						Log.Write("Reseting Crashing buff", Color.Blue);
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
				
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 40)
					{
						WoW.CastSpell("Stormstrike");
						return;
					}
					
					if (WoW.CanCast("Crash Lightning") && !WoW.PlayerHasBuff("Stormbringer") && !fourt19 && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 20 && WoW.SpellCooldownTimeRemaining("Feral Spirit") >= 6) //Crash lightning for cleave proc stormbringer without 4t19
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }

					if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && (WoW.Maelstrom >= 80 || (fourt19 && WoW.Maelstrom >= 40)))
					{
						WoW.CastSpell("Lava Lash"); //Buffer Lava
						return;
					}
					
					if (WoW.CanCast("Flametongue") && WoW.PlayerBuffTimeRemaining("Flametongue") <= 5 && WoW.IsSpellInRange("Flametongue"))
					{
						WoW.CastSpell("Flametongue"); //REFRESH FLAMETONGUE PANDEMIC
						return;
					}
					
					if (WoW.CanCast("Boulderfist") && WoW.IsSpellInRange("Flametongue")) 
					{
						WoW.CastSpell("Boulderfist"); //Nothing to do
						return;
					}
						
					if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue")) 
					{
						WoW.CastSpell("Flametongue"); //Nothing to do 
						return;
					}
					
					if (WoW.CanCast("Feral Lunge") && !WoW.IsSpellInRange("Flametongue") && WoW.IsSpellInRange("Feral Lunge")) //out of range of flametongue 10y and in range of feral lunge 8-25y 
                    {
                        WoW.CastSpell("Feral Lunge");
                        return;
                    }
					
					if (WoW.CanCast("Lightning Bolt") && !WoW.IsSpellInRange("Flametongue")) //out of range cast LB if we are 10y away and cannot jump by Feral Lunge
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }

                    if (WoW.CanCast("Astral Shift") && WoW.HealthPercent < 60) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
                    {
                        WoW.CastSpell("Astral Shift");
                        return;
                    }
                }
            }
            if (combatRoutine.Type == RotationType.AOE)
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerHasBuff("Ghost Wolf") && WoW.IsInCombat) //First things go first
                {
                    if (WoW.CanCast("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Wind Shear")) //interupt every spell
                    {
                        WoW.CastSpell("Wind Shear");
                        return;
                    }

                    if (WoW.CanCast("Feral Spirit") && WoW.IsSpellInRange("Stormstrike") && combatRoutine.UseCooldowns && WoW.Maelstrom >= 20) //Wolves in melee range
                    {
                        Log.Write("Using Feral Spirit", Color.Red);
                        WoW.CastSpell("Feral Spirit");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.SpellCooldownTimeRemaining("Feral Spirit") >= 115) //Crash lightning for alpha wolf
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }	

                    if (WoW.CanCast("Boulderfist") && (!WoW.PlayerHasBuff("Landslide") || WoW.PlayerBuffTimeRemaining("Landslide") <= 1) && WoW.IsSpellInRange("Flametongue")) //REFRESH LANDSLIDE
                    {
                        Log.Write("Reseting Landslide", Color.Red);
                        WoW.CastSpell("Boulderfist");
                        return;
                    }
					
					if (WoW.CanCast("Boulderfist") && WoW.IsSpellInRange("Flametongue") && WoW.PlayerSpellCharges("Boulderfist") == 2 && WoW.Maelstrom <= 100) 
                    {    
                        WoW.CastSpell("Boulderfist"); //boulderfist it to not waste a charge
                        return;
                    }
					
                    if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue") && ((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") <= 1)) || (WoW.SpellCooldownTimeRemaining("Doom Winds") <= 6 && WoW.PlayerBuffTimeRemaining("Flametongue") <= 4)) //REFRESH FLAMETONGUE
                    {
                        Log.Write("Reseting Flametongue buff", Color.Red);
                        WoW.CastSpell("Flametongue");
                        return;
                    }								
					
					if (WoW.CanCast("Doom Winds") && combatRoutine.UseCooldowns && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Flametongue"))
                    {
                        WoW.CastSpell("Doom Winds");
                        return;
                    }
					
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 20)
                    {
                        WoW.CastSpell("Stormstrike");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Stormstrike")) //Crash lightning priority on aoe
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
				
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 40)
					{
						WoW.CastSpell("Stormstrike");
						return;
					}

					if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && (WoW.Maelstrom >= 80 || (fourt19 && WoW.Maelstrom >= 40)))
					{
						WoW.CastSpell("Lava Lash"); //Buffer Lava
						return;
					}
					
					if (WoW.CanCast("Flametongue") && WoW.PlayerBuffTimeRemaining("Flametongue") <= 5 && WoW.IsSpellInRange("Flametongue"))
					{
						WoW.CastSpell("Flametongue"); //REFRESH FLAMETONGUE PANDEMIC
						return;
					}
					
					if (WoW.CanCast("Boulderfist") && WoW.IsSpellInRange("Flametongue")) 
					{
						WoW.CastSpell("Boulderfist"); //Nothing to do
						return;
					}
						
					if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue")) 
					{
						WoW.CastSpell("Flametongue"); //Nothing to do 
						return;
					}
					
					if (WoW.CanCast("Feral Lunge") && !WoW.IsSpellInRange("Flametongue") && WoW.IsSpellInRange("Feral Lunge")) //out of range of flametongue 10y and in range of feral lunge 8-25y 
                    {
                        WoW.CastSpell("Feral Lunge");
                        return;
                    }
					
					if (WoW.CanCast("Lightning Bolt") && !WoW.IsSpellInRange("Flametongue")) //out of range cast LB if we are 10y away and cannot jump by Feral Lunge
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }

                    if (WoW.CanCast("Astral Shift") && WoW.HealthPercent < 60) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
                    {
                        WoW.CastSpell("Astral Shift");
                        return;
                    }
                }
            }
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Jedix
AddonName=RGB
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,57994,Wind Shear,NumPad9
Spell,196884,Feral Lunge,NumPad6
Spell,51533,Feral Spirit,NumPad2
Spell,204945,Doom Winds,NumPad3
Spell,187874,Crash Lightning,D3
Spell,193796,Flametongue,D1
Spell,108271,Astral Shift,D5
Spell,201897,Boulderfist,G
Spell,197211,Fury of Air,D6
Spell,60103,Lava Lash,E
Spell,17364,Stormstrike,Q
Spell,187837,Lightning Bolt,NumPad5
Spell,188070,Healing Surge,NumPad4
Aura,194084,Flametongue
Aura,197211,Fury of Air
Aura,187878,Crash Lightning
Aura,218825,Boulderfist
Aura,202004,Landslide
Aura,201846,Stormbringer
Aura,204945,Doom Winds
Aura,2645,Ghost Wolf
*/
