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
    public class Assassination : CombatRoutine
    {
        public override string Name
        {
            get { return "Rogue-Assassination"; }
        }

        public override string Class
        {
            get { return "Rogue"; }
        }

        public override Form SettingsForm { get; set; }


        public override void Initialize()
        {
            Log.Write("Welcome to Rogue-Assassination", Color.Green);
            Log.Write("Suggested build: 1133111");
        }

        public override void Stop()
        {
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
				return 4; 
			} 
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
       private readonly Stopwatch stopwatch = new Stopwatch();
		private  bool lastNamePlate = false;
        public  void SelectRotation(int aoe, int cleave, int single)
        {
            int count = WoW.CountEnemyNPCsInRange;
            if(lastNamePlate)
            {
                combatRoutine.ChangeType(RotationType.SingleTarget);
                lastNamePlate =false;
            }
            lastNamePlate = WoW.Nameplates;
            if (count >= 3)
                combatRoutine.ChangeType(RotationType.AOE);
            if (count == 2)
                combatRoutine.ChangeType(RotationType.SingleTargetCleave);
            if (count <= 1)
                combatRoutine.ChangeType(RotationType.SingleTarget);

        }
       public override void Pulse()
        {		
            {
            if (combatRoutine.Type == RotationType.SingleTarget)
            {				
				if (WoW.IsSpellInRange("Rupture") && WoW.IsInCombat && !WoW.IsMounted)				
				{
					if ( UseCooldowns &&
                        WoW.CanCast("Kingsbane") && WoW.CurrentComboPoints <= 4 && WoW.Energy >= 35 && WoW.PlayerHasBuff("Envenom") && WoW.PlayerBuffTimeRemaining("Envenom") >= 150 &&
                        !WoW.IsSpellOnCooldown("Kingsbane") && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") >= 1400 &&
                            (
                            WoW.SpellCooldownTimeRemaining("Vendetta") >= 1000 ||
                            WoW.TargetHasDebuff("Vendetta")
                            ))
                         {
                        WoW.CastSpell("Kingsbane");
                        return;
                    }			
					if (!WoW.PlayerHasBuff("Vanish") && WoW.CanCast("Toxic Blade") && WoW.Energy >= 20 && !WoW.IsSpellOnCooldown("Toxic Blade") && WoW.CurrentComboPoints <= 4 && WoW.IsSpellInRange("Garrote") && WoW.Talent (6) == 1)
					{
						WoW.CastSpell("Toxic Blade");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && WoW.CanCast("Exsanguinate") && WoW.Energy >= 25 && !WoW.IsSpellOnCooldown("Exsanguinate") && WoW.IsSpellInRange("Garrote") && WoW.Talent (6) == 3)
					{
						WoW.CastSpell("Exsanguinate");
						return;
					}					
					if (!WoW.PlayerHasBuff("Vanish") && WoW.CanCast("Garrote") && WoW.Energy >= 45 && !WoW.TargetHasDebuff("Garrote") && !WoW.IsSpellOnCooldown("Garrote") && WoW.CurrentComboPoints <= 4 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Garrote");
						return;
					}
					if (WoW.TargetHasDebuff("Vendetta") && WoW.CanCast("Fan Of Knives") && WoW.Energy >= 35 && WoW.PlayerHasBuff("FoK") && WoW.PlayerBuffStacks("FoK") == 30 && WoW.CurrentComboPoints <= 4)
					{
						WoW.CastSpell("Fan Of Knives");
						return;	
					}
					if (!WoW.PlayerHasBuff("Vanish") && WoW.CanCast("Garrote") && WoW.Energy >= 45 && WoW.TargetHasDebuff("Garrote") && WoW.TargetDebuffTimeRemaining("Garrote") <= 300 && WoW.CurrentComboPoints <= 4 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Garrote");
						return;
					}
					if (WoW.CurrentComboPoints == 4 && WoW.Energy >= 25 && WoW.CanCast("Rupture") && !WoW.TargetHasDebuff("Rupture") && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Rupture");
						return;
					}
					if (WoW.CurrentComboPoints == 4 && WoW.Energy >= 25 && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") <= 600 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Rupture");
						return;
					}
					if (WoW.CurrentComboPoints == 5 && WoW.Energy >= 25 && WoW.CanCast("Rupture") && !WoW.TargetHasDebuff("Rupture") && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Rupture");
						return;
					}
					if (WoW.CurrentComboPoints == 5 && WoW.Energy >= 25 && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") <= 600 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Rupture");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && WoW.TargetHasDebuff("Toxins") && WoW.TargetDebuffTimeRemaining("Toxins") <= 150 && WoW.Energy >= 35 && WoW.CurrentComboPoints == 4 && WoW.CanCast("Envenom") && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") > 600 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && WoW.TargetHasDebuff("Toxins") && WoW.TargetDebuffTimeRemaining("Toxins") <= 150 && WoW.Energy >= 35 && WoW.CurrentComboPoints == 5 && WoW.CanCast("Envenom") && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") > 600 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (WoW.TargetHasDebuff("Kingsbane") && WoW.TargetDebuffTimeRemaining("Rupture") >= 400 && WoW.Energy >= 35 && WoW.CurrentComboPoints >= 2 && WoW.PlayerHasBuff("Envenom") && WoW.PlayerBuffTimeRemaining("Envenom") <= 150)
					{
						WoW.CastSpell("Envenom");
						Log.Write("Extend Envenom remaining");
						return;
					}
					if (WoW.TargetHasDebuff("Kingsbane") && WoW.TargetDebuffTimeRemaining("Rupture") >= 400 && WoW.Energy >= 35 && WoW.CurrentComboPoints >= 2 && !WoW.PlayerHasBuff("Envenom"))
					{
						WoW.CastSpell("Envenom");
						Log.Write("Getting Envenom up for kingsbane");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && !WoW.TargetHasDebuff("Toxins") && WoW.Energy >= 35 && WoW.CurrentComboPoints == 4 && WoW.CanCast("Envenom") && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") > 600 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && !WoW.TargetHasDebuff("Toxins") && WoW.Energy >= 35 && WoW.CurrentComboPoints == 5 && WoW.CanCast("Envenom") && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") > 600 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && WoW.TargetHasDebuff("Toxins") && WoW.TargetHasDebuff("Vendetta") && WoW.Energy >= 140 && WoW.CurrentComboPoints >= 4 && WoW.CanCast("Envenom") && WoW.TargetHasDebuff("Rupture") && WoW.TargetDebuffTimeRemaining("Rupture") > 600 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (WoW.CanCast("Mutilate") && WoW.Energy >= 55 && WoW.CurrentComboPoints <= 3 )
					{
						WoW.CastSpell("Mutilate");
						return;
					}					
 					if (UseCooldowns && WoW.CanCast("Vendetta") &&
                        !WoW.IsSpellOnCooldown("Vendetta") &&
                        WoW.Energy <= 50
                        ) {
                        WoW.CastSpell("Vendetta");
                        return;
                    }
					/*if (UseCooldowns &&
                        WoW.CanCast("Vanish") &&
                        WoW.CurrentComboPoints >= 5 &&
                        WoW.Energy >= 25 && WoW.TargetHasDebuff("Vendetta") &&
                        (
                            WoW.TargetHasDebuff("Rupture") &&
                            WoW.TargetDebuffTimeRemaining("Rupture") < 10
                        )
                    ) {
                        WoW.CastSpell("Vanish");
                        return;
                    }*/
					if (!WoW.PlayerHasBuff("Critbuff") && UseCooldowns && WoW.CurrentComboPoints == 5 && !WoW.IsSpellOnCooldown("Vanish") && WoW.Energy >= 35 && WoW.TargetHasDebuff("Vendetta") )
					{
						WoW.CastSpell("Vanish");
						return;
					}
					if (UseCooldowns && WoW.CurrentComboPoints == 5 && WoW.PlayerHasBuff("Vanish") && WoW.Energy >= 35 && WoW.TargetHasDebuff("Vendetta"))
					{
						WoW.CastSpell("Rupture");
						return;
					}
					if (UseCooldowns && WoW.CanCast("Berserk") && !WoW.IsSpellOnCooldown("Berserk") && WoW.TargetHasDebuff ("Vendetta") && WoW.PlayerRace == "Troll")
                    {
                        WoW.CastSpell("Berserk");
                        return;
                    }
					
				}
                
            }
            

            if (combatRoutine.Type == RotationType.AOE || combatRoutine.Type == RotationType.SingleTargetCleave) // Do AoE Target Stuff here
            {
                if (WoW.HasTarget && WoW.IsSpellInRange("Rupture") && WoW.IsInCombat && !WoW.IsMounted)
				{
					if (WoW.Energy >= 35 && WoW.CurrentComboPoints <= 4 && WoW.CanCast("Fan Of Knives"))
					{
						WoW.CastSpell("Fan Of Knives");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && WoW.CanCast("Toxic Blade") && WoW.Energy >= 20 && !WoW.IsSpellOnCooldown("Toxic Blade") && WoW.CurrentComboPoints <= 4 && WoW.IsSpellInRange("Garrote") && WoW.Talent (6) == 1)
					{
						WoW.CastSpell("Toxic Blade");
						return;
					}
					if (!WoW.PlayerHasBuff("Vanish") && WoW.CanCast("Exsanguinate") && WoW.Energy >= 25 && !WoW.IsSpellOnCooldown("Exsanguinate") && WoW.IsSpellInRange("Garrote") && WoW.Talent (6) == 3)
					{
						WoW.CastSpell("Exsanguinate");
						return;
					}
					if (WoW.Energy >= 35 && WoW.CurrentComboPoints == 4 && WoW.TargetHealthPercent <= 35 && WoW.CanCast("Envenom"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (WoW.Energy >= 35 && WoW.CurrentComboPoints == 5 && WoW.TargetHealthPercent <= 35 && WoW.CanCast("Envenom"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (WoW.Energy >= 25 && WoW.CurrentComboPoints == 4 && WoW.TargetHealthPercent >= 36 && WoW.CanCast("Rupture") && !WoW.TargetHasDebuff("Rupture"))
					{
						WoW.CastSpell("Rupture");
						return;
					}
					if (WoW.Energy >= 25 && WoW.CurrentComboPoints == 5 && WoW.TargetHealthPercent >= 36 && WoW.CanCast("Rupture") && !WoW.TargetHasDebuff("Rupture"))
					{
						WoW.CastSpell("Rupture");
						return;
					}
					if (WoW.Energy >= 35 && WoW.CurrentComboPoints >= 4 && WoW.TargetHealthPercent >= 36 && WoW.TargetHasDebuff("Rupture") && WoW.CanCast("Envenom"))
					{
						WoW.CastSpell("Envenom");
						return;
					}
					if (WoW.CanCast("Garrote") && WoW.Energy >= 45 && !WoW.TargetHasDebuff("Garrote") && !WoW.IsSpellOnCooldown("Garrote") && WoW.CurrentComboPoints <= 4 && WoW.IsSpellInRange("Garrote"))
					{
						WoW.CastSpell("Garrote");
						return;
					}
					if (UseCooldowns && WoW.CanCast("Berserk") && !WoW.IsSpellOnCooldown("Berserk") )
                    {
                           WoW.CastSpell("Berserk");
                           return;
                    }
				}
            }
        }
    }
}
}
/*
[AddonDetails.db]
AddonAuthor=Creepyjoker
AddonName=smartie
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,1943,Rupture,D2
Spell,79140,Vendetta,D5
Spell,1856,Vanish,V
Spell,1329,Mutilate,D1
Spell,703,Garrote,R
Spell,192759,Kingsbane,D3
Spell,32645,Envenom,Q
Spell,51723,Fan Of Knives,X
Spell,26297,Berserk,D0
Spell,245388,Toxic Blade,D0
Spell,200806,Exsanguinate,D0
Aura,1943,Rupture
Aura,1784,Stealth
Aura,703,Garrote
Aura,235027,Critbuff
Aura,208693,FoK
Aura,1856,Vanish
Aura,192425,Toxins
Aura,32645,Envenom
Aura,192759,Kingsbane
Aura,200802,Agonizing Poison
Aura,193641,Elaborate Planning
Aura,79140,Vendetta
*/
