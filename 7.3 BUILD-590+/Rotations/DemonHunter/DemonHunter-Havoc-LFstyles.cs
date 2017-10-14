using CloudMagic.Helpers;
using System.Diagnostics;
using System.Drawing;
using System;
using System.Threading;
using System.Windows.Forms;
using CloudMagic.GUI;

namespace CloudMagic.Rotation
{
    public class DemonHunterHavoc : CombatRoutine
    {
        public override string Name 
		{
			get
			{
				return "CloudMagic DemonHunter";
			}
		}
        public override string Class 
		{
			get
			{
				return "DemonHunter";
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
				return 183752; /*Consume Magic ID*/ 
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
            Log.WriteCloudMagic("Welcome to LFstyles Havoc Demon Hunter ", Color.Yellow);
			Log.Write("Suggested build: 2231311", Color.Yellow);
			Log.Write("Must Use this Macro to manually use Metamorphosis: #showtooltip /cast [@cursor] Metamorphosis", Color.Yellow);
		}
        public override void Stop()
        {
        }

        public override void Pulse()
        {
			if (WoW.IsMounted) return;
		
            // if (WoW.TargetIsCasting && interruptwatch.ElapsedMilliseconds > 1200)
                    // {
                        // if (!WoW.IsSpellOnCooldown("Sigil of Silence") && WoW.WasLastCasted("Arcane Torrent"))
                        // {
                            
                            // WoW.CastSpell("Sigil of Silence");
                            // interruptwatch.Reset();
                            // interruptwatch.Start();
                            // return;
                        // }

                        // if (!WoW.IsSpellOnCooldown("Arcane Torrent") && WoW.WasLastCasted("Sigil of Silence"))
                        // {
                            
                            // WoW.CastSpell("Arcane Torrent");
                            // interruptwatch.Reset();
                            // interruptwatch.Start();
                            // return;
                        // }
                    // }
			if (UseCooldowns)
            {
            }
			
            if (combatRoutine.Type != RotationType.SingleTarget && combatRoutine.Type != RotationType.AOE) return;

            if (!WoW.HasTarget || !WoW.TargetIsEnemy) return;
			
			if (WoW.HasBossTarget && !WoW.IsSpellOnCooldown("FOTI") && WoW.IsSpellInRange("Chaos Strike"))
            {
                WoW.CastSpell("FOTI");
                return;
            }

            if (WoW.CanCast("FOTI") && !WoW.IsSpellOnCooldown("FOTI") && WoW.IsSpellInRange("Chaos Strike") &&
                WoW.CountEnemyNPCsInRange >= 2 && !WoW.IsMoving)
            {
                WoW.CastSpell("FOTI");
                return;
            }

            if (WoW.CanCast("Eye Beam") && WoW.Fury > 50 && WoW.CountEnemyNPCsInRange >= 2
                && !WoW.IsMoving)
            {
                WoW.CastSpell("Eye Beam");
                return;
            }
			{
                if (WoW.PlayerHasBuff("Metamorphosis"))
                {
                if (WoW.CanCast("Nemesis") && !WoW.IsSpellOnCooldown("Nemesis"))
                {
                    WoW.CastSpell("Nemesis");
                }

                if (WoW.CanCast("Chaos Blades") && (!WoW.IsSpellOnCooldown("Chaos Blades") && (!WoW.PlayerHasBuff("Chaos Blades Buff"))))
                {
                    WoW.CastSpell("Chaos Blades");
                }

                if (WoW.CanCast("Death Sweep") && !WoW.IsSpellOnCooldown("Death Sweep") && WoW.IsSpellInRange("Annihilation") &&
                    WoW.Fury > 15)
                {
                    WoW.CastSpell("Death Sweep");
                    return;
                }

                if (WoW.CanCast("Annihilation") && !WoW.IsSpellOnCooldown("Annihilation") && WoW.IsSpellInRange("Annihilation") &&
                    WoW.Fury > 40)
                {
                    WoW.CastSpell("Annihilation");
                    return;
                }

                if (WoW.CanCast("Felblade") && WoW.IsSpellInRange("Annihilation"))
                {
                    WoW.CastSpell("Felblade");
                    return;
                }
						if (WoW.CanCast("Throw Glaive") && !WoW.TargetHasDebuff("Bloodlet"))
						{
							WoW.CastSpell("Throw Glaive");
							return;
						}
				}
				if (!WoW.PlayerHasBuff("Metamorphosis"))
				{
						if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Eye Beam");
							return;
						}
						if (WoW.CanCast("Throw Glaive") && !WoW.TargetHasDebuff("Bloodlet"))
						{
							WoW.CastSpell("Throw Glaive");
							return;
						}
						if (WoW.CanCast("Felblade")) // Fury Generator
						{
							WoW.CastSpell("Felblade");
							return;
						}
                if (WoW.CanCast("Chaos Blades") && (!WoW.IsSpellOnCooldown("Chaos Blades") && (!WoW.PlayerHasBuff("Chaos Blades Buff"))))
                {
                    WoW.CastSpell("Chaos Blades");
                }
						if (WoW.CanCast("Nemesis") && !WoW.IsSpellOnCooldown("Nemesis") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Nemesis");
							return;
						}
						if (WoW.CanCast("FOTI") && WoW.IsSpellInRange("Chaos Strike") && !WoW.IsSpellOnCooldown("FOTI") && !WoW.IsMoving)
						{
							WoW.CastSpell("FOTI");
							return;
						}
						if (WoW.CanCast("Eye Beam") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 50)
						{
							WoW.CastSpell("Eye Beam");
							return;
						}
						if (WoW.CanCast("Blade Dance") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Blade Dance");
							return;
						}
						if (WoW.CanCast("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") && WoW.IsSpellOnCooldown("Eye Beam") && (WoW.Fury >= 40 || WoW.Fury >= 70))
						{
							WoW.CastSpell("Chaos Strike");
							return;
				}
				}
				if (WoW.CanCast("Blade Dance") && !WoW.IsSpellOnCooldown("Blade Dance") && WoW.IsSpellInRange("Chaos Strike") &&
                    WoW.Fury > 15)
                {
                    WoW.CastSpell("Blade Dance");
                    return;
                }

                if (WoW.CanCast("Felblade") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury < 110)
                {
                    WoW.CastSpell("Felblade");
                    return;
                }

                if (WoW.CanCast("Chaos Strike") && !WoW.IsSpellOnCooldown("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") &&
                    WoW.Fury > 40)
                {
                    WoW.CastSpell("Chaos Strike");
                    return;
                }

                if (WoW.CanCast("Felblade") && WoW.IsSpellInRange("Chaos Strike"))
                {
                    WoW.CastSpell("Felblade");
                    return;
                }
				}
                //if (WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70) // Fury Generator
                //{
                //    WoW.CastSpell("Demons Bite");
                //}
            }
        }
	}
/*
[AddonDetails.db]
AddonAuthor=LFstyles
AddonName=_914
WoWVersion=70300
[SpellBook.db]
Spell,198013,Eye Beam,D5
Spell,195072,Fel Rush,D2
Spell,162243,Demons Bite,None
Spell,162794,Chaos Strike,D4
Spell,185123,Throw Glaive,D8
Spell,188499,Blade Dance,D3
Spell,198793,Vengeful Retreat,R
Spell,179057,Chaos Nova,T
Spell,206491,Nemesis,F9
Spell,201427,Annihilation,D4
Spell,210152,Death Sweep,D3
Spell,191427,Metamorphosis,D1
Spell,198589,Blur,F2
Spell,211053,Fel Barrage,None
Spell,201467,FOTI,D7
Spell,247983,Chaos Blades,D0
Spell,232893,Felblade,D6
Aura,162264,Metamorphosis
Aura,208628,Momentum
Aura,207690,Bloodlet
Aura,247938,Chaos Blades Buff
*/
