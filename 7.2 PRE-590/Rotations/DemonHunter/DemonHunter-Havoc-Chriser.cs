using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using PixelMagic.Helpers;

namespace PixelMagic.Rotation
{
    public class DemonHunterHavoc : CombatRoutine
    {
        private NumericUpDown nudBlurPercentValue;
		
	private static readonly Stopwatch coolDownStopWatch = new Stopwatch();

        public override string Name { get { return "Havoc by Chriser"; } }

        public override string Class { get { return "Demon Hunter"; } }

        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {

            if (ConfigFile.ReadValue("DemonHunter", "Blur Usage Percent") == "")
            {
                ConfigFile.WriteValue("Demonhunter", "Blur Usage Percent", "45");
            }	
			Log.Write("For Single Target I Recommend Talents 2223311", Color.Red);							      
			
            SettingsForm = new Form {Text = "Settings", StartPosition = FormStartPosition.CenterScreen, Width = 800, Height = 490, ShowIcon = false};
            var lblBlurPercent = new Label {Text = "Blur Health %", Left = 12, Top = 150};
            SettingsForm.Controls.Add(lblBlurPercent);

            nudBlurPercentValue = new NumericUpDown {Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Demonhunter", "Blur Usage Percent"), Left = 108, Top = 145};
            SettingsForm.Controls.Add(nudBlurPercentValue);

            var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 662, Top = 408, Size = new Size(108, 31)};
            cmdSave.Click += CmdSave_Click;

            SettingsForm.Controls.Add(cmdSave);
            nudBlurPercentValue.BringToFront();
        }

        private void CmdSave_Click(object sender, EventArgs e)
        {
            ConfigFile.WriteValue("Demonhunter", "Blur Usage Percent", nudBlurPercentValue.Value.ToString());
        }

        public override void Stop()
        {
        }

        public override void Pulse()
        {
            if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave) //Single Target 
																
            {
				
                if (WoW.TargetIsCasting) //Kick
                    {
                        if (WoW.CanCast("Consume Magic") 
							&& WoW.TargetIsCastingAndSpellIsInterruptible 
							&& WoW.TargetPercentCast >= 55
							&& !WoW.IsSpellOnCooldown("Consume Magic") 
							&& !WoW.PlayerIsChanneling 
							&& !WoW.WasLastCasted("Consume Magic"))
                        {
                            WoW.CastSpell("Consume Magic");						
                            return;
                        }	
					}
				if (WoW.HasTarget && !WoW.PlayerIsChanneling && WoW.TargetIsEnemy && WoW.IsInCombat)
                {    
                    if (WoW.PlayerHasBuff("Metamorphosis"))
                    {
						if (combatRoutine.UseCooldowns)
						{
							if (WoW.Talent(5) == 3 && WoW.CanCast("Nemesis") && WoW.IsSpellInRange("Chaos Strike"))
							{
								WoW.CastSpell("Nemesis");
								return;
							}
							if (WoW.Talent(7) == 1 && WoW.CanCast("Chaos Blades") && WoW.IsSpellInRange("Chaos Strike"))
							{
								WoW.CastSpell("Chaos Blades");
								return;
							}
						
							if (WoW.CanCast("FOTI") && WoW.IsSpellInRange("Chaos Strike"))
							{
								WoW.CastSpell("FOTI");
								return;
							}
						}
						/*if (WoW.CanCast("Fel Rush") && WoW.PlayerSpellCharges("Fel Rush")>=1 && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <=100)
						{
							WoW.CastSpell("Fel Rush");
							return;
						}	*/ // Use at own Risk may end up dead 
						if (WoW.Talent (7) == 2 && WoW.CanCast("FelBarrage") && WoW.IsSpellInRange("Chaos Strike") && WoW.PlayerSpellCharges("FelBarrage")>=5)  
						{	
							WoW.CastSpell("FelBarrage");
							return;
						}
						if (WoW.Talent(7)== 3 && WoW.Talent(1) == 3 && WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Eye Beam"); 
							return;
						}
						if (WoW.Talent(1) == 3 && WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Eye Beam"); 
							return;
						}
						if (WoW.Talent(7) == 3 && WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Eye Beam"); 
							return;
						}
						if (WoW.Talent(5) == 2 && WoW.CanCast("FelEruption") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 10)
						{
							WoW.CastSpell("FelEruption");
							return;
						}
						if (WoW.Talent(3) == 3 && WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }	
					    if (WoW.Talent(1) == 2 && WoW.CanCast("Felblade") && WoW.Fury < 100 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Felblade");
							return;
						}
						if (WoW.Talent (3) == 2 && WoW.CanCast("Death Sweep") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                        {
                            WoW.CastSpell("Death Sweep");
                            return;
                        }	
						if (WoW.CanCast("Annihilation") && WoW.IsSpellInRange("Chaos Strike") && WoW.PlayerHasBuff("Chaos Blades") && (WoW.Fury >= 40 ))
                        {
                            WoW.CastSpell("Annihilation");
                            return;
                        }
						if (WoW.CanCast("Annihilation") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 55 ))
                        {
                            WoW.CastSpell("Annihilation");
                            return;
                        }												
						if (WoW.Talent(2) == 3 && WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70) 
                        {
                            WoW.CastSpell("Demons Bite");
                            return;
                        }
						if (WoW.Talent(2) == 1 && WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70)  
                        {
                            WoW.CastSpell("Demons Bite");
                            return;
                        }											
						if (WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }
						if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike") && (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_Y) < 0))
                        {																
                            WoW.CastSpell("Eye Beam"); 
                            return;
                        }

                    }
					if (combatRoutine.UseCooldowns)
					{				
						if (WoW.Talent(5) == 3 && WoW.CanCast("Nemesis") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Nemesis");
							return;
						}
						if (WoW.CanCast("Metamorphosis") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Metamorphosis");
							return;
						}
						if (WoW.Talent(7) == 1 && WoW.CanCast("Chaos Blades") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Chaos Blades");
							return;
						}
					
						if (WoW.CanCast("FOTI") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("FOTI");
							return;
						}
						if (WoW.CanCast("Arcane Torrent") && !WoW.IsSpellOnCooldown ("Arcane Torrent") && WoW.PlayerRace == "BloodElf" && WoW.Fury <=50)
						{
							WoW.CastSpell("Arcane Torrent");
							return;
						}	
					}					
						/*if (WoW.CanCast("Fel Rush") && WoW.PlayerSpellCharges("Fel Rush")>=1 && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <=100)
						{
							WoW.CastSpell("Fel Rush");
							return;
						}	*/ //Use at own Risk may end up dead
						if (WoW.Talent (7) == 2 && WoW.CanCast("FelBarrage") && WoW.IsSpellInRange("Chaos Strike") && WoW.PlayerSpellCharges("FelBarrage")>=5)  
						{	
							WoW.CastSpell("FelBarrage");
							return;
						}
						if (WoW.Talent(7)== 3 && WoW.Talent(1) == 3 && WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Eye Beam"); 
							return;
						}
						if (WoW.Talent(1) == 3 && WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Eye Beam"); 
							return;
						}
						if (WoW.Talent(7) == 3 && WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Eye Beam"); 
							return;
						}
						if (WoW.Talent(5) == 2 && WoW.CanCast("FelEruption") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 10)
						{
							WoW.CastSpell("FelEruption");
							return;
						}
						if (WoW.Talent(3) == 3 && WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }	
					    if (WoW.Talent(1) == 2 && WoW.CanCast("Felblade") && WoW.Fury < 100 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Felblade");
							return;
						}
						if (WoW.Talent (3) == 2 && WoW.CanCast("Blade Dance") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                        {
                            WoW.CastSpell("Blade Dance");
                            return;
                        }	
						if (WoW.CanCast("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") && WoW.PlayerHasBuff("Chaos Blades") && (WoW.Fury >= 40 ))
                        {
                            WoW.CastSpell("Chaos Strike");
                            return;
                        }
						if (WoW.CanCast("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 55 ))
                        {
                            WoW.CastSpell("Chaos Strike");
                            return;
                        }							
						if (WoW.Talent(2) == 3 && WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70) 
                        {
                            WoW.CastSpell("Demons Bite");
                            return;
                        }
						if (WoW.Talent(2) == 1 && WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70)  
                        {
                            WoW.CastSpell("Demons Bite");
                            return;
                        }											
						if (WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }
						if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike") && (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_Y) < 0))
                        {																
                            WoW.CastSpell("Eye Beam"); 
                            return;
                        }
                }
            }
           if (combatRoutine.Type == RotationType.AOE)  // AOE
            {
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                {
						if (WoW.CanCast("Vengeful Retreat") && WoW.Talent(5)==1 && WoW.Talent(2)==1 && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >=55)
						{
							WoW.CastSpell("Vengeful Retreat");
							return;
						}
						if (WoW.CanCast("Vengeful Retreat") && WoW.Talent(5)==1 && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >=85)
						{
							WoW.CastSpell("Vengeful Retreat");
							return;
						}
						if (WoW.CanCast("Fel Rush")&& WoW.IsSpellInRange("Fel Blade") && WoW.Fury <=100)
						{
							WoW.CastSpell("Fel Rush");
							return;
						}
						if (WoW.Talent (7) == 2 && WoW.CanCast("FelBarrage") && WoW.IsSpellInRange("Chaos Strike") && WoW.PlayerSpellCharges("FelBarrage")>=5 && WoW.PlayerHasBuff ("Momentum"))  
						{	
							WoW.CastSpell("FelBarrage");
							return;
						}
						if (WoW.CanCast("FOTI") && WoW.IsSpellInRange("Chaos Strike") && WoW.PlayerHasBuff("Momentum"))
						{
							WoW.CastSpell("FOTI");
							return;
						}
						if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike") && WoW.PlayerHasBuff("Momentum"))
						{
							WoW.CastSpell("Eye Beam"); 
							return;
						}
						if (WoW.Talent(3)== 2 && WoW.CanCast("Blade Dance") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                        {
                            WoW.CastSpell("Blade Dance");
                            return;
                        }
						if (WoW.Talent(3)== 2 &&WoW.CanCast("Death Sweep") && WoW.PlayerHasBuff("Metamorphosis") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                        {
                            WoW.CastSpell("Death Sweep");
                            return;
                        }
						if (WoW.CanCast("Blade Dance") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 35)
                        {
                            WoW.CastSpell("Blade Dance");
                            return;
                        }
						if (WoW.CanCast("Death Sweep") && WoW.PlayerHasBuff("Metamorphosis") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 35)
                        {
                            WoW.CastSpell("Death Sweep");
                            return;
                        }
						if (WoW.Talent(3) == 3 && WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }
						if ((WoW.Talent(3)==1 && WoW.CanCast("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 65 || (WoW.Fury >= 55 && (WoW.PlayerHasBuff("Chaos Blades") || WoW.TargetHasDebuff("Nemesis") )))))
                        {
                            WoW.CastSpell("Chaos Strike");
                            return;
                        }
						if ((WoW.Talent(3)==1 && WoW.CanCast("Annihilation") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 65 || (WoW.Fury >= 55 && (WoW.PlayerHasBuff("Chaos Blades") || WoW.TargetHasDebuff("Nemesis") )))))
                        {
                            WoW.CastSpell("Annihilation");
                            return;
                        }
						if (WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }
						if ((WoW.CanCast("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 65 || (WoW.Fury >= 55 && (WoW.PlayerHasBuff("Chaos Blades") || WoW.TargetHasDebuff("Nemesis") )))))
                        {
                            WoW.CastSpell("Chaos Strike");
                            return;
                        }
						if ((WoW.CanCast("Annihilation") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 65 || (WoW.Fury >= 55 && (WoW.PlayerHasBuff("Chaos Blades") || WoW.TargetHasDebuff("Nemesis") )))))
                        {
                            WoW.CastSpell("Annihilation");
                            return;
                        }
						if (WoW.Talent(2) == 1 && WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70)  
                        {
                            WoW.CastSpell("Demons Bite");
                            return;
                        }
						if (WoW.Talent(2) == 3 && WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70) 
                        {
                            WoW.CastSpell("Demons Bite");
                            return;
                        }
						if (WoW.Talent(6) == 2 && WoW.CanCast("Chaos Nova") && WoW.IsSpellInRange("Chaos Blade"))
						{
							WoW.CastSpell("Chaos Nova");
							return;
						}
																																																						
				}
																				
			}
				
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Chriser
AddonName=Recount
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,198013,Eye Beam,D4
Spell,211053,FelBarrage,D6
Spell,195072,Fel Rush,D1
Spell,162794,Chaos Strike,D3
Spell,185123,Throw Glaive,D8
Spell,188499,Blade Dance,D2
Spell,198793,Vengeful Retreat,F
Spell,183752,Consume Magic,O
Spell,179057,Chaos Nova,T
Spell,206491,Nemesis,H
Spell,201427,Annihilation,D3
Spell,210152,Death Sweep,D2
Spell,191427,Metamorphosis,D7
Spell,198589,Blur,F2
Spell,201467,FOTI,D5
Spell,211048,Chaos Blades,G
Spell,232893,Felblade,R
Spell,162243,Demons Bite,D9
Spell,211881,FelEruption,F6
Spell,80483,Arcane Torrent,F3
Aura,162264,Metamorphosis
Aura,211048,Chaos Blades
Aura,208628,Momentum
*/
