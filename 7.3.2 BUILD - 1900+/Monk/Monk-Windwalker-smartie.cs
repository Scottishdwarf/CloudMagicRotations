using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using CloudMagic.WoWChecks;
using CloudMagic.Logs;
using CloudMagic.ControlMethod;

namespace CloudMagic.Rotation
{
    public class WindwalkerMonk : CombatRoutine
    {
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
				return 3; 
			} 
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
				return 116705; 
			}
		}
        public override Form SettingsForm { get; set; }

        public override string Name 
		{
			get
			{
				return "Windwalker Monk";
			}
		}

        public override string Class 
		{
			get		
			{
				return "Monk";
			}
		}

        public override void Initialize()
        {
            Log.DrawHorizontalLine();
            Log.Write("Welcome to smartie`s Windwalker Rotation", Color.SpringGreen);
           
        }


        public override void Stop()
        {
        }

        public override void Pulse()
        {
            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.PlayerIsChanneling && !WoW.IsMounted)
            {
				//Healing Stuff
				if (WoW.CanCast("Healing Elixier") && WoW.PlayerSpellCharges("Healing Elixier") > 0 && !WoW.IsSpellOnCooldown("Healing Elixier") && WoW.HealthPercent <= 40 )
				{
					WoW.CastSpell("Healing Elixier");
					return;
				}
				if (WoW.CanCast("Überschwung") && !WoW.IsSpellOnCooldown("Überschwung") && WoW.HealthPercent <= 10 && WoW.Energy >= 30)
				{
					WoW.CastSpell("Überschwung");
					return;
				}	
				if (WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 60 && WoW.CanCast("Spear Hand Strike") && !WoW.IsSpellOnCooldown("Spear Hand Strike") && WoW.IsSpellInRange("Spear Hand Strike"))
				{
					WoW.CastSpell("Spear Hand Strike");
					return;
				}				
				//Cooldowns
				if (UseCooldowns)
				{
					if (WoW.CanCast("Touch of Death") && !WoW.IsSpellOnCooldown("Touch of Death") && WoW.IsSpellInRange("Tiger Palm"))
					{
						WoW.CastSpell("Touch of Death");
						return;
					}
					if (WoW.CanCast("Serenity") && !WoW.IsSpellOnCooldown("Serenity") && WoW.Talent (7) == 3 && WoW.IsSpellInRange("Tiger Palm") && !WoW.IsSpellOnCooldown("Fist of Fury"))
					{
						WoW.CastSpell("Serenity");
						return;
					}
					if (!WoW.ItemOnCooldown("Vial of Ceaseless Toxins") && !WoW.IsSpellOnCooldown("Vial of Ceaseless Toxins"))
					{
						WoW.CastSpell("Vial of Ceaseless Toxins") ;
						return;
					}
					/*if (WoW.CanCast("EWF") && !WoW.IsSpellOnCooldown("EWF") && WoW.Talent (7) != 3 && WoW.IsSpellInRange("Tiger Palm"))
					{
						WoW.CastSpell("EWF");
						return;
					}*/
				}				
				// Single Target Rotation
				if (combatRoutine.Type == RotationType.SingleTarget)
				{
                    if (WoW.CanCast("Energizing Elixir") && WoW.IsSpellInRange("Tiger Palm") && WoW.Energy < 50 && WoW.CurrentChi <= 1 && (WoW.IsSpellOnCooldown("Serenity") && !WoW.PlayerHasBuff("Serenity") && WoW.SpellCooldownTimeRemaining("Serenity") > 300 && UseCooldowns || !UseCooldowns))
                    {
                        WoW.CastSpell("Energizing Elixir");
                        return;
                    }					
					if (WoW.CanCast("Fist of Fury") && !WoW.IsSpellOnCooldown("Fist of Fury") && WoW.IsSpellInRange("Tiger Palm") && (WoW.SpellCooldownTimeRemaining("Serenity") > 1000 && UseCooldowns || !UseCooldowns) && (WoW.CurrentChi >=3 || WoW.PlayerHasBuff("Serenity")))
					{	
						WoW.CastSpell("Fist of Fury");
						return;
					}
					if (WoW.CanCast("Strike of the Windlord") && !WoW.IsSpellOnCooldown("Strike of the Windlord") && WoW.IsSpellInRange("Tiger Palm") && (WoW.CurrentChi >= 2 || WoW.PlayerHasBuff("Serenity")) && (WoW.IsSpellOnCooldown("Fist of Fury") && WoW.SpellCooldownTimeRemaining("Fist of Fury") >= 200 || WoW.CurrentChi == 5 || WoW.SpellCooldownTimeRemaining("Serenity") < 1000 && WoW.IsSpellOnCooldown("Serenity") && UseCooldowns))
					{
						WoW.CastSpell("Strike of the Windlord");
						return;
					}
					if (WoW.CanCast("Tiger Palm") && WoW.CurrentChi <= 3 && WoW.IsSpellInRange("Tiger Palm") && WoW.Energy >= 110 && !WoW.PlayerHasBuff("Serenity"))
					{
						WoW.CastSpell("Tiger Palm");
						return;
					}
					if (WoW.CanCast("Rising Sun Kick") && WoW.IsSpellInRange("Tiger Palm") && (WoW.CurrentChi >= 2 || WoW.PlayerHasBuff("Serenity")) && (WoW.IsSpellOnCooldown("Fist of Fury") && WoW.SpellCooldownTimeRemaining("Fist of Fury") >= 200 || WoW.CurrentChi == 5 || WoW.SpellCooldownTimeRemaining("Serenity") < 1000 && WoW.IsSpellOnCooldown("Serenity") && UseCooldowns))
					{
						WoW.CastSpell("Rising Sun Kick");
						return;
					}
					if (WoW.CanCast("Blackout Kick") && WoW.IsSpellInRange("Tiger Palm") && (WoW.CurrentChi >= 1 || WoW.PlayerHasBuff("Free Blackout Kick") || WoW.PlayerHasBuff("Serenity")) && (WoW.IsSpellOnCooldown("Fist of Fury") && WoW.SpellCooldownTimeRemaining("Fist of Fury") >= 200 || WoW.PlayerHasBuff("Free Blackout Kick") || WoW.SpellCooldownTimeRemaining("Serenity") < 1000 && WoW.IsSpellOnCooldown("Serenity") && UseCooldowns))
					{
						WoW.CastSpell("Blackout Kick");
						return;
					}
					if (WoW.CanCast("Tiger Palm") && WoW.IsSpellInRange("Tiger Palm") && WoW.Energy >= 50 )
					{
						WoW.CastSpell("Tiger Palm");
						return;
					}				
				}

				// AoE Rotation
				if (combatRoutine.Type == RotationType.AOE)
				{
                    if (WoW.CanCast("Energizing Elixir") && WoW.IsSpellInRange("Tiger Palm") && WoW.Energy < 50 && WoW.CurrentChi <= 1 && (WoW.IsSpellOnCooldown("Serenity") && !WoW.PlayerHasBuff("Serenity") && WoW.SpellCooldownTimeRemaining("Serenity") > 300 && UseCooldowns || !UseCooldowns))
                    {
                        WoW.CastSpell("Energizing Elixir");
                        return;
                    }					
					if (WoW.CanCast("Fist of Fury") && !WoW.IsSpellOnCooldown("Fist of Fury") && WoW.IsSpellInRange("Tiger Palm") && (WoW.SpellCooldownTimeRemaining("Serenity") > 1000 && UseCooldowns || !UseCooldowns) && (WoW.CurrentChi >=3 || WoW.PlayerHasBuff("Serenity")))
					{	
						WoW.CastSpell("Fist of Fury");
						return;
					}
					if (WoW.CanCast("Strike of the Windlord") && !WoW.IsSpellOnCooldown("Strike of the Windlord") && WoW.IsSpellInRange("Tiger Palm") && (WoW.CurrentChi >= 2 || WoW.PlayerHasBuff("Serenity")) && (WoW.IsSpellOnCooldown("Fist of Fury") && WoW.SpellCooldownTimeRemaining("Fist of Fury") >= 200 || WoW.CurrentChi == 5 || WoW.SpellCooldownTimeRemaining("Serenity") < 1000 && WoW.IsSpellOnCooldown("Serenity") && UseCooldowns))
					{
						WoW.CastSpell("Strike of the Windlord");
						return;
					}
					if (WoW.CanCast("Tiger Palm") && WoW.CurrentChi <= 3 && WoW.IsSpellInRange("Tiger Palm") && WoW.Energy >= 110 && !WoW.PlayerHasBuff("Serenity"))
					{
						WoW.CastSpell("Tiger Palm");
						return;
					}
					if (WoW.CanCast("Spinning Crane Kick") && WoW.IsSpellInRange("Tiger Palm") && (WoW.CurrentChi >= 3 || WoW.PlayerHasBuff("Serenity")) && (WoW.IsSpellOnCooldown("Fist of Fury") && WoW.SpellCooldownTimeRemaining("Fist of Fury") >= 200 || WoW.SpellCooldownTimeRemaining("Serenity") < 1000 && WoW.IsSpellOnCooldown("Serenity") && UseCooldowns))
					{
						WoW.CastSpell("Spinning Crane Kick");
						return;
					}
					if (WoW.CanCast("Blackout Kick") && WoW.IsSpellInRange("Tiger Palm") && WoW.PlayerHasBuff("Free Blackout Kick"))
					{
						WoW.CastSpell("Blackout Kick");
						return;
					}
					if (WoW.CanCast("Tiger Palm") && WoW.IsSpellInRange("Tiger Palm") && WoW.Energy >= 50 )
					{
						WoW.CastSpell("Tiger Palm");
						return;
					}				
				}
			}
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=smartie
AddonName=smartie
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,115080,Touch of Death,D0
Spell,113656,Fist of Fury,D5
Spell,205320,Strike of the Windlord,D9
Spell,100780,Tiger Palm,D2
Spell,107428,Rising Sun Kick,D6
Spell,115098,Chi Wave,D7
Spell,100784,Blackout Kick,D4
Spell,101546,Spinning Crane Kick,D3
Spell,152173,Serenity,T
Spell,152175,WhirlingDragonPunch,F7
Spell,122281,Healing Elixier,F2
Spell,116694,Überschwung,F3
Spell,115288,Energizing Elixir,D8
Spell,116705,Spear Hand Strike,F12
Spell,147011,Vial of Ceaseless Toxins,D7
Aura,152173,Serenity
Aura,116768,Free Blackout Kick
Aura,228287,MotC
Aura,220358,SCK
Aura,115080,TouchOfDeath
Item,147011,Vial of Ceaseless Toxins
*/
