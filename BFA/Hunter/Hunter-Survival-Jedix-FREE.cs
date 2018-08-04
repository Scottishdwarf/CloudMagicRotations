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
    public class Survival : CombatRoutine
    {
        public override string Name { get { return "Survival"; } }

        public override string Class { get { return "Hunter"; } }

        public override Form SettingsForm { get; set; }
		public override string RoutineName { get { return "Survival"; } } //only for paid routines
        public override string Premium { get { return "false"; } }
        public override string Recurring { get { return "false"; } }
		
		public override int CLEAVE { get { return 99; } } //please Set between 1-99 if not desired set to 99
        public override int AOE { get { return 99; } }//please Set between 1-99 if not desired set to 99
        public override int SINGLE { get { return 99; } }//please Set between 1-99 if not desired set to 99
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
				return 187707; 
			}
		}
		
		private int GCD
        {
            get
            {
                if (150 / (1 + (WoW.HastePercent / 100)) > 75)
                {
                    return 150 / (1 + (WoW.HastePercent / 100));
                }
                else
                {
                    return 75;
                }
            }
        }

        public override void Initialize()
        {
            Log.Write("Welcome to Survival Hunter by Jedix", Color.Green);

			
        }	

        public override void Stop()
        {					
        }

        public override void Pulse()
        {
			if (WoW.HasTarget && WoW.TargetIsVisible && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting)
			{	
				
				//if (WoW.CanCast("Muzzle") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 75 && WoW.IsSpellInRange("RaptorStrike")) //interupt every spell
				//{
				//	WoW.CastSpell("Muzzle");
				//}
				
				if (WoW.CanCast("Claw"))
				{
					WoW.CastSpell("Claw");
				}
				
				if (WoW.CanCast("CoordinatedAssault") && WoW.IsSpellInRange("RaptorStrike") && WoW.TargetHasDebuff("WBomb") && WoW.TargetHasDebuff("SSting") && !UseCooldowns)
				{
					WoW.CastSpell("CoordinatedAssault");
					return;
				}
				
				if (WoW.CanCast("SerprentSting") && WoW.IsSpellInRange("SerprentSting") && WoW.Talent(1) == 1 && WoW.PlayerHasBuff("ViperVenom"))
				{
					WoW.CastSpell("SerprentSting");
					return;	
				}
				
				if (WoW.CanCast("Carve") && WoW.Talent(7) == 1 && WoW.PlayerHasBuff("Coordinated") && WoW.PlayerBuffTimeRemaining("Coordinated") <= GCD && WoW.Focus >= 50 && WoW.CountEnemyNPCsInRange >= 3 && WoW.IsSpellInRange("RaptorStrike"))
				{
					WoW.CastSpell("Carve");
					return;
				}
				
				if (WoW.CanCast("RaptorStrike") && WoW.Talent(7) == 1 && WoW.PlayerHasBuff("Coordinated") && WoW.PlayerBuffTimeRemaining("Coordinated") <= GCD && WoW.Focus >= 50 && WoW.IsSpellInRange("RaptorStrike"))
				{
					WoW.CastSpell("RaptorStrike");
					return;
				}
				
				if (WoW.CanCast("KillCommand") && WoW.IsSpellInRange("KillCommand") && WoW.Focus <= 80
					&& (WoW.Talent(6) != 1 || (WoW.Talent(6) == 1 && (!WoW.PlayerHasBuff("ToS") || (WoW.PlayerHasBuff("ToS") && WoW.PlayerBuffStacks("ToS") <= 2)))))
				{
					WoW.CastSpell("KillCommand");
					return;
				}
				
				if (WoW.CanCast("FlankingStrike") && WoW.IsSpellInRange("FlankingStrike") && (WoW.Focus <= 70 || !WoW.IsSpellInRange("RaptorStrike"))
					&& WoW.Talent(6) == 3)
				{
					WoW.CastSpell("FlankingStrike");
					return;
				}
				
				if (WoW.CanCast("WildfireBomb") && WoW.IsSpellInRange("SerprentSting")  && (WoW.PlayerSpellCharges("WildfireBomb") >= 2 || !WoW.TargetHasDebuff("WBomb") || (WoW.TargetHasDebuff("WBomb")
					&& WoW.TargetDebuffTimeRemaining("WBomb") < 300)))
				{
					WoW.CastSpell("WildfireBomb");
					Log.Write("UpdateWildfireBomb", Color.Red);
					return;	
				}
				
				if (WoW.CanCast("SerprentSting") && WoW.IsSpellInRange("SerprentSting") && ((WoW.TargetHasDebuff("SSting") && WoW.TargetDebuffTimeRemaining("SSting") < 300 && WoW.Focus >=20) || (!WoW.TargetHasDebuff("SSting") && WoW.Focus >=20)))
				{
					WoW.CastSpell("SerprentSting");
					Log.Write("UpdateSerprentSting", Color.Red);
					return;	
				}
				
				if (WoW.CanCast("Carve") && WoW.Focus >= 35 && WoW.CountEnemyNPCsInRange >= 3 && WoW.IsSpellInRange("RaptorStrike"))
				{
					WoW.CastSpell("Carve");
					return;
				}

				if (WoW.CanCast("RaptorStrike") && WoW.Focus >= 30 && WoW.IsSpellInRange("RaptorStrike"))
				{
					WoW.CastSpell("RaptorStrike");
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
Spell,187707,Muzzle,D1
Spell,186270,RaptorStrike,D2
Spell,259489,KillCommand,D3
Spell,259491,SerprentSting,D4
Spell,259495,WildfireBomb,D5
Spell,187708,Carve,D6
Spell,266779,CoordinatedAssault,D7
Spell,269751,FlankingStrike,D8
Spell,16827,Claw,F1
Spell,17253,Bite,F1
Spell,49966,Smack,F1
Buff,266779,Coordinated
Buff,268552,ViperVenom
Buff,260286,ToS
Debuff,269747,WBomb
Debuff,259491,SSting
Debuff,259277,KCommand
Charge,259495,WildfireBomb
Range,186270,RaptorStrike
Range,259495,SerprentSting
Range,259489,KillCommand
Range,269751,FlankingStrike
*/
