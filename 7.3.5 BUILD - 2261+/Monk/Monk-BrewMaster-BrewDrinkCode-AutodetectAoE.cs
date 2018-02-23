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
    public class BrewMaster : CombatRoutine
    {
        private static bool _ironSkinFired;

		public override string Name 
		{
			get		
			{
				return "BrewMaster Rotation";
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
		public override int AoE_Range  
		{
			get 
			{ 
				return 100; 
			} 
		}
        public override int Interrupt_Ability_Id 
		{ 
			get 
			{ 
				return 116705; 
			}
		}
		public override string Class 
		{
			get		
			{
				return "Monk";
			}
		}

        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {            
            Log.Write("Welcome to Monk BM rotation", Color.Green);
        }

        public override void Stop()
        {
        }


        public override void Pulse()
        {
			/*if (!WoW.IsInCombat && WoW.HealthPercent <= 75 & WoW.Energy >= 30 && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && !WoW.IsMounted)
			{
				WoW.CastSpell("Effuse");
				return;
			}*/
            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && WoW.TargetIsVisible && !WoW.IsMounted)
            {
				if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.SingleTargetCleave || combatRoutine.Type == RotationType.AOE)
				{
					//Cooldown saves
					if (WoW.HealthPercent <= 85 && WoW.PlayerSpellCharges("Healing Elixir") > 0 && !WoW.IsSpellOnCooldown("Healing Elixir"))
					{
						WoW.CastSpell("Healing Elixir");
					return;
					}
					if (WoW.CanCast("Rushing Jadewind") && !WoW.IsSpellOnCooldown("Rushing Jadewind"))
					{
						WoW.CastSpell("Rushing Jadewind");
						return;
					}
					//Interrupts or Damage negation
					if (WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 60 && WoW.CanCast("Spear Hand Strike") && !WoW.IsSpellOnCooldown("Spear Hand Strike") && WoW.IsSpellInRange("Spear Hand Strike"))
					{
						WoW.CastSpell("Spear Hand Strike");
						return;
					}
					//Leg Sweep to open, or mitigate damage
					if (WoW.CanCast("Leg Sweep") && !WoW.IsSpellOnCooldown("Leg Sweep") && WoW.IsSpellInRange("Tiger Palm"))
					{
						WoW.CastSpell("Leg Sweep");
						return;
					}
					/*//Look for Expel Harm Charges.. if 3 and health low then hit
					if (WoW.CanCast("Expel Harm") && WoW.PlayerSpellCharges("Expel Harm") >= 3 && WoW.HealthPercent <= 75 && WoW.Energy >= 15)
					{
						WoW.CastSpell("Expel Harm");
						return;
					}
					//If Target is almost dead, and we have Expel Harm charges use up -- we don't want to leave it 
					if (WoW.CanCast("Expel Harm") && WoW.TargetHealthPercent <= 10 && WoW.PlayerSpellCharges("Expel Harm") != 100 && WoW.CanCast("Expel Harm") && !WoW.IsSpellOnCooldown("Expel Harm") && WoW.Energy >= 15)
					{
						Log.Write(string.Format("Expel Harm Count {0}", WoW.PlayerSpellCharges("Expel Harm")));
						WoW.CastSpell("Expel Harm");
						return;
					}*/
					//Maintain Eye of the Tiger
					if (!WoW.PlayerHasBuff("Eye of the Tiger") && !WoW.IsSpellOnCooldown("Tiger Palm") && WoW.IsSpellInRange("Tiger Palm"))
					{
						WoW.CastSpell("Tiger Palm");
						return;
					}
					//Breath of fire if Vulnerable
					if (WoW.TargetHasDebuff("Keg Smash") && !WoW.TargetHasDebuff("Breath of Fire") && WoW.CanCast("Breath of Fire") && !WoW.IsSpellOnCooldown("Breath of Fire"))
					{
						WoW.CastSpell("Breath of Fire");
						return;
					}
					if (!WoW.TargetHasDebuff("Keg Smash") && WoW.CanCast("Keg Smash") && !WoW.IsSpellOnCooldown("Keg Smash") && WoW.Energy >= 40)
					{
						WoW.CastSpell("Keg Smash");
						return;
					}
					//Energy dump if High
					if (WoW.Energy >= 65 && WoW.CanCast("Tiger Palm") && !WoW.IsSpellOnCooldown("Tiger Palm") && WoW.IsSpellInRange("Tiger Palm"))
					{
						WoW.CastSpell("Tiger Palm");
						return;
					}
					//if Ironskin count = 3 and in Melee range (or we cast too early as we approach)
					if (WoW.PlayerSpellCharges("Ironskin Brew") >= 2 && !WoW.PlayerHasBuff("Ironskin Brew") && WoW.IsSpellInRange("Tiger Palm"))
					{
						_ironSkinFired = true;
						WoW.CastSpell("Ironskin Brew");
						return;
					}
					if (_ironSkinFired && !WoW.PlayerHasBuff("Ironskin Brew"))
					{
						_ironSkinFired = false;
						if (WoW.PlayerSpellCharges("Purifying Brew") >= 1)
						{
							WoW.CastSpell("Purifying Brew");
							return;
						}
					}
					//if We Can Cast Exploding Keg and in Melee range then do so
					if (WoW.CanCast("Exploding Keg") && !WoW.IsSpellOnCooldown("Exploding Keg") && WoW.IsSpellInRange("Tiger Palm"))
					{
						WoW.CastSpell("Exploding Keg");
						return;
					}
					if (WoW.CanCast("Blackout Strike") && !WoW.IsSpellOnCooldown("Blackout Strike") && WoW.IsSpellInRange("Tiger Palm"))
					{
						WoW.CastSpell("Blackout Strike");
					}
					if (WoW.CanCast("Tiger Palm") && !WoW.IsSpellOnCooldown("Tiger Palm") && WoW.IsSpellInRange("Tiger Palm") && WoW.TargetHasDebuff("Keg Smash"))
					{
						WoW.CastSpell("Tiger Palm");
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
Spell,121253,Keg Smash,D1
Spell,115181,Breath of Fire,D2
Spell,100780,Tiger Palm,D3
Spell,205523,Blackout Strike,D4
Spell,115308,Ironskin Brew,D5
Spell,119582,Purifying Brew,D6
Spell,115399,Black Ox Brew,D7
Spell,122281,Healing Elixir,D8
Spell,116705,Spear Hand Strike,D9
Spell,119381,Leg Sweep,D0
Spell,115072,Expel Harm,F1
Spell,214326,Exploding Keg,F2
Spell,116694,Effuse,F1
Spell,133585,Trinket,D0
Spell,116847,Rushing Jadewind,D3
Debuff,121253,Keg Smash
Debuff,115181,Breath of Fire
Buff,115308,Ironskin Brew
Buff,196607,Eye of the Tiger
Range,100780,Tiger Palm
Range,116705,Spear Hand Strike
Charge,122281,Healing Elixir
Charge,115072,Expel Harm
Charge,115308,Ironskin Brew
Charge,119582,Purifying Brew
*/
