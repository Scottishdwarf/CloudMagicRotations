//test@test.com
// ReSharper disable UnusedMember.Global



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
    public class RogueOutlaw : CombatRoutine
    {
		
		public override string Name 
		{
			get
			{
				return "CloudMagic Rogue";
			}
		}

        public override string Class 
		{
			get
			{
				return "Rogue";
			}
		}
				
		// AoE_Range
        //  5,6,8,10,15,20,25,30,35,40,45,50,60,70,80,90,100 Will default to 5 if set incorrectly
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
				return 1766; /*Kick ID*/ 
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
				return 99; 
			} 
		}
		public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
            Log.DrawHorizontalLine(); 			          
            Log.Write("Please use this Macro for Cancel Blade Flurry:",Color.Red); 
			Log.Write("#showtooltip Stealth /stopcasting /cancelaura Blade Flurry", Color.Black);		
          
        }

        public override void Stop()
        {
        }

        public override void Pulse()
    {		
			if (UseCooldowns) 
				{					
				}
				
            if (combatRoutine.Type == RotationType.SingleTarget) 
			{	
				if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted && !WoW.PlayerIsChanneling)
				{								
				if (WoW.CanPreCast("Kick") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 60 && WoW.IsSpellInRange("Ghostly Strike") && !WoW.IsSpellOnCooldown("Kick"))
				{
					WoW.CastSpell("Kick");						
					return;
				}
				if (WoW.CanPreCast("Blade Flurry") && WoW.IsSpellInRange("Ghostly Strike") && WoW.CountEnemyNPCsInRange >=2 && !WoW.PlayerHasBuff("Blade Flurry"))
                    {
                        WoW.CastSpell("Blade Flurry");
                        return;
                    }
				if (WoW.CanPreCast("Cancel Blade Flurry") && WoW.IsSpellInRange("Ghostly Strike") && WoW.PlayerHasBuff("Blade Flurry") && WoW.CountEnemyNPCsInRange <=1)
					{
						WoW.CastSpell("Cancel Blade Flurry");
						return;
					}				
				if (WoW.CanPreCast("Ambush") && WoW.Energy >=60 && WoW.IsSpellInRange("Ghostly Strike") && WoW.CurrentComboPoints >=4 && (WoW.PlayerHasBuff("Stealth") || WoW.PlayerHasBuff("Vanish")))
					{
						WoW.CastSpell("Ambush");
						return;
					}
				if (WoW.CanCast("Slice and Dice") && WoW.Energy >=21 && WoW.IsSpellInRange("Ghostly Strike") && WoW.Talent(7) == 1 && WoW.CurrentComboPoints >=4  && WoW.PlayerHasBuff("Loaded Dice"))
					{
						WoW.CastSpell("Slice and Dice");
						return;
					}
				if (WoW.CanCast("Slice and Dice") && WoW.Energy >=21 && WoW.IsSpellInRange("Ghostly Strike") && !WoW.PlayerHasBuff("Slice and Dice") && WoW.PlayerBuffTimeRemaining ("Slice and Dice") <=500 && WoW.CurrentComboPoints >=4 && WoW.Talent(7) == 1)
					{
						WoW.CastSpell("Slice and Dice");
						return;
					}
				if (WoW.CanCast("Roll the Bones") && WoW.Energy >=21 && WoW.IsSpellInRange("Ghostly Strike") && WoW.Talent(7) == 2 && WoW.PlayerHasBuff("Loaded Dice") && WoW.CurrentComboPoints >=4 )
					{
						WoW.CastSpell("Roll the Bones");
						return;
					}	
				if (WoW.CanCast("Roll the Bones") && WoW.Talent (7) == 2 && WoW.Energy >=21 && WoW.IsSpellInRange("Ghostly Strike") && WoW.CurrentComboPoints >=4 
				&& !WoW.PlayerHasBuff("Buried Treasure") && !WoW.PlayerHasBuff("Shark Infested Waters") && !WoW.PlayerHasBuff("True Bearing") 
				&& !WoW.PlayerHasBuff("Jolly Roger") && !WoW.PlayerHasBuff("Broadsides"))
					{
						WoW.CastSpell("Roll the Bones");
						return;
					}	
				if (WoW.CanPreCast("Ghostly Strike") && WoW.IsSpellInRange("Ghostly Strike") && WoW.Energy >=30 && WoW.CurrentComboPoints <=5 && !WoW.TargetHasDebuff("Ghostly Strike") && WoW.TargetDebuffTimeRemaining("Ghostly Strike") <=200 && WoW.Talent(1) == 1)
					{
						WoW.CastSpell("Ghostly Strike");
						return;
					}
				if (WoW.CanPreCast("Adrenaline Rush") && UseCooldowns && WoW.IsSpellInRange("Ghostly Strike"))
					{
						WoW.CastSpell("Adrenaline Rush");
						return;
					}
				if (WoW.CanPreCast("Vanish") && UseCooldowns && WoW.IsSpellInRange("Ghostly Strike") && WoW.Energy >=60 && !WoW.PlayerHasBuff("Adrenaline Rush"))
					{
						WoW.CastSpell("Vanish");
						return;
					}		
				if (WoW.CanPreCast("Arcane Torrent") && UseCooldowns && WoW.IsSpellInRange("Ghostly Strike") && !WoW.PlayerHasBuff("Adrenaline Rush") && WoW.Energy <=20 && WoW.PlayerRace == "BloodElf")
					{
						WoW.CastSpell("Arcane Torrent");
						return;
					}
				if (WoW.CanPreCast("Marked for Death") && WoW.IsSpellInRange("Ghostly Strike") && WoW.CurrentComboPoints <=1 && WoW.Talent(7) == 2)
					{
						WoW.CastSpell("Marked for Death");
						return;
					}		
				if (WoW.CanPreCast("Curse of the Dreadblades") && UseCooldowns && WoW.IsSpellInRange("Ghostly Strike"))
					{
						WoW.CastSpell("Curse of the Dreadblades");
						return;
					}				
				if (WoW.CanCast("Between the Eyes") && WoW.CurrentComboPoints >=5 && WoW.Energy >=31 && WoW.IsSpellInRange("Ghostly Strike"))
					{
						WoW.CastSpell("Between the Eyes");
						return;
					}				
				if (WoW.CanCast("Run Through") && WoW.CurrentComboPoints ==6 && WoW.Energy >=31 && WoW.IsSpellInRange("Ghostly Strike") || (WoW.PlayerHasBuff("Broadsides") && WoW.CurrentComboPoints >=5 && WoW.Energy >=31))
					{
						WoW.CastSpell("Run Through");
						return;
					}
				if (WoW.CanPreCast("Pistol Shot") && WoW.IsSpellInRange("Ghostly Strike") && WoW.PlayerHasBuff("Opportunity") && WoW.CurrentComboPoints <=5)
					{
						WoW.CastSpell("Pistol Shot");
						return;
					}
				if (WoW.CanPreCast("Saber Slash") && WoW.CurrentComboPoints <=5 && WoW.Energy >=50 && WoW.IsSpellInRange("Ghostly Strike"))
					{
						WoW.CastSpell("Saber Slash");
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
AddonName=
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,13877,Blade Flurry,D6
Spell,1784,Cancel Blade Flurry,D9
Spell,5171,Slice and Dice,T
Spell,196937,Ghostly Strike,D4
Spell,13750,Adrenaline Rush,G
Spell,202665,Curse of the Dreadblades,H
Spell,199804,Between the Eyes,D5
Spell,2098,Run Through,D3
Spell,185763,Pistol Shot,D2
Spell,193315,Saber Slash,D1
Spell,1766,Kick,D7
Spell,25046,Arcane Torrent,O
Spell,137619,Marked for Death,D0
Spell,193316,Roll the Bones,T
Spell,8676,Ambush,D1
Spell,1856,Vanish,F2

Buff,13877,Blade Flurry
Buff,5171,Slice and Dice
Debuff,196937,Ghostly Strike
Buff,195627,Opportunity
Buff,13750,Adrenaline Rush
Buff,240837,Loaded Dice
Buff,1784,Stealth
Buff,11327,Vanish

Buff,199600,Buried Treasure
Buff,193357,Shark Infested Waters
Buff,193359,True Bearing
Buff,193358,Grand Melee
Buff,199603,Jolly Roger
Buff,193356,Broadsides

Range,196937,Ghostly Strike

*/
