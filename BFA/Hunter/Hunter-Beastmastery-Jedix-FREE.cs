using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using FoxCore.WoWChecks;
using FoxCore.Logs;
using FoxCore.ControlMethod;

namespace FoxCore.Rotation
{
    public class BeastMaster : CombatRoutine
    {
        public override string Name { get { return "BeastMaster"; } }

        public override string Class { get { return "Hunter"; } }

        public override Form SettingsForm { get; set; }
		public override string RoutineName { get { return "BeastMaster"; } } //only for paid routines
        public override string Premium { get { return "false"; } }
        public override string Recurring { get { return "false"; } }
		
		public override int CLEAVE { get { return 99; } } //please Set between 1-99 if not desired set to 99
        public override int AOE { get { return 99; } }//please Set between 1-99 if not desired set to 99
        public override int SINGLE { get { return 99; } }//please Set between 1-99 if not desired set to 99
		public override int AoE_Range  
		{
			get 
			{ 
				return 40; 
			} 
		}
		public override int Interrupt_Ability_Id 
		{ 
			get 
			{ 
				return 147362; 
			}
		}
		
		private int GCD
        {
            get
            {
                if ((150 / (1 + (WoW.HastePercent / 100)) > 75) && !WoW.PlayerHasBuff("AotW"))
                {
                    return 150 / (1 + (WoW.HastePercent / 100));
                }
				
				if ((((150 / (1 + (WoW.HastePercent / 100))) - 20) > 75) && WoW.PlayerHasBuff("AotW"))
                {
                    return ((150 / (1 + (WoW.HastePercent / 100))) - 20);
                }
				
                else
                {
                    return 75;
                }
            }
        }
		
		private static readonly Stopwatch frenzywatch = new Stopwatch();
		

        public override void Initialize()
        {
            Log.Write("Welcome to BeastMaster Hunter by Jedix", Color.Green);
			Log.Write("For single/aoe - disable/enable nameplates", Color.Red);
			Log.Write("Default pet - with Claw attack", Color.Red);
			Log.Write("To do more dps by pet check file and replace WoW.CanCast(Claw) and WoW.CastSpell(Claw) to your family pet spell, e.g Bite or Smack", Color.Red);
        }	

        public override void Stop()
        {					
        }

        public override void Pulse()
        {
			if (!WoW.IsInCombat && frenzywatch.IsRunning)
			{
				frenzywatch.Reset();
			}
			
			if (WoW.HasTarget && WoW.TargetIsVisible && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting)
			{	
				
				//if (WoW.CanCast("CounterShot") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 75 && WoW.IsSpellInRange("CobraShot")) //interupt every spell
				//{
				//	WoW.CastSpell("CounterShot");
				//}
				
				if (WoW.CanCast("Claw"))
				{
					WoW.CastSpell("Claw");
				}
				
				if (WoW.CanCast("BarbedShot") && WoW.IsSpellInRange("CobraShot") && ((frenzywatch.ElapsedMilliseconds >= (7950-(GCD*10)) && frenzywatch.ElapsedMilliseconds < 8000) || (WoW.PetBuffTimeRemaining("Frenzy") <= GCD && WoW.PetHasBuff("Frenzy"))))
				{
					WoW.CastSpell("BarbedShot");
					frenzywatch.Restart();
					Log.Write("UpdateFrenzy", Color.Red);
					return;	
				}
				
				if (WoW.CanCast("BarbedShot") && !WoW.CanCast("AotW") && WoW.IsSpellInRange("CobraShot") && WoW.PlayerSpellCharges("BarbedShot") >= 2)
				{
					WoW.CastSpell("BarbedShot");
					frenzywatch.Restart();
					return;	
				}
				
				if (WoW.CanCast("AotW")&& (WoW.SpellCooldownTimeRemaining("BestialWrath") <= 500 || (WoW.PlayerHasBuff("BestialWrath") && WoW.PlayerBuffTimeRemaining("BestialWrath") >= 10))
				&& WoW.IsSpellInRange("CobraShot") && !UseCooldowns && ((WoW.PetBuffTimeRemaining("Frenzy") >= GCD && WoW.PetHasBuff("Frenzy")) || !WoW.PetHasBuff("Frenzy")))
				{
					WoW.CastSpell("AotW");
				}
				
				if (WoW.Talent(4) == 3 && WoW.CanCast("MurderofCrows") && !UseCooldowns
				&& WoW.CountEnemyNPCsInRange <= 1 && WoW.Focus >= 30 && ((WoW.PetBuffTimeRemaining("Frenzy") >= GCD && WoW.PetHasBuff("Frenzy")) || !WoW.PetHasBuff("Frenzy")))
				{
					WoW.CastSpell("MurderofCrows");
				}
				
				if (WoW.CanCast("BestialWrath") && !UseCooldowns && WoW.IsSpellInRange("CobraShot") && ((WoW.PetBuffTimeRemaining("Frenzy") >= GCD && WoW.PetHasBuff("Frenzy")) || !WoW.PetHasBuff("Frenzy")))
				{
					WoW.CastSpell("BestialWrath");
				}
				
				if (WoW.CanPreCast("KillCommand") && WoW.IsSpellInRange("KillCommand") && WoW.Focus >= 30 && ((WoW.PetBuffTimeRemaining("Frenzy") >= GCD && WoW.PetHasBuff("Frenzy")) || !WoW.PetHasBuff("Frenzy")))
				{
					WoW.CastSpell("KillCommand");
					return;
				}
				
				if (WoW.Talent(2) == 3 && ((WoW.PetBuffTimeRemaining("Frenzy") >= GCD && WoW.PetHasBuff("Frenzy")) || !WoW.PetHasBuff("Frenzy")) && WoW.CanCast("ChimaeraShot") && WoW.IsSpellInRange("CobraShot") && (WoW.Focus <= 110 || WoW.CountEnemyNPCsInRange >= 2))
				{
					WoW.CastSpell("ChimaeraShot");
					return;
				}

				if (WoW.Talent(1) == 3 && WoW.CanCast("DireBeast") && WoW.Focus >= 25)
				{
					WoW.CastSpell("DireBeast");
					return;
				}

				if (WoW.CanCast("BarbedShot") && WoW.IsSpellInRange("CobraShot") && WoW.PlayerSpellCharges("BarbedShot") >= 1
				&& ((WoW.SpellChargeRemaining("BarbedShot") < 7 && !WoW.PetHasBuff("Frenzy")) || (WoW.SpellChargeRemaining("BarbedShot") < 2 && WoW.PetHasBuff("Frenzy"))))
				{
					WoW.CastSpell("BarbedShot");
					frenzywatch.Restart();
					return;	
				}

				if (WoW.CanCast("MultiShot") && WoW.IsSpellInRange("CobraShot") && WoW.Focus >= 40 && WoW.CountEnemyNPCsInRange >= 2
				&& (!WoW.PlayerHasBuff("BeastCleave") || (WoW.PlayerHasBuff("BeastCleave") && WoW.PlayerBuffTimeRemaining("BeastCleave") <= GCD)))
				{
					WoW.CastSpell("MultiShot");
					return;
				}

				if (WoW.CanCast("CobraShot") && ((WoW.PetBuffTimeRemaining("Frenzy") >= GCD && WoW.PetHasBuff("Frenzy")) || !WoW.PetHasBuff("Frenzy")) && WoW.IsSpellInRange("CobraShot") && (WoW.Focus >= 110 || (WoW.PlayerHasBuff("BestialWrath") && WoW.Focus >= 45
				&& WoW.SpellCooldownTimeRemaining("KillCommand") >= GCD) || (WoW.Focus >= 35 && WoW.SpellCooldownTimeRemaining("KillCommand") > GCD
				&& WoW.PlayerHasBuff("BestialWrath") && WoW.PlayerBuffTimeRemaining("BestialWrath") <= GCD)))
				{
					WoW.CastSpell("CobraShot");
					return;
				}
			}
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Jedix
AddonName=IdTip
WoWVersion=80000
[SpellBook.db]
Spell,193455,CobraShot,D1
Spell,217200,BarbedShot,D2
Spell,34026,KillCommand,D3
Spell,2643,MultiShot,D4
Spell,53209,ChimaeraShot,D5
Spell,19574,BestialWrath,D6
Spell,193530,AotW,D7
Spell,147362,CounterShot,D8
Spell,131894,MurderofCrows,D9
Spell,120679,DireBeast,D10
Spell,16827,Claw,F1
Spell,17253,Bite,F1
Spell,49966,Smack,F1
Buff,19574,BestialWrath
PetBuff,272790,Frenzy
Buff,268877,BeastCleave
Buff,193530,AotW
Charge,217200,BarbedShot
Range,193455,CobraShot
Range,34026,KillCommand
*/
