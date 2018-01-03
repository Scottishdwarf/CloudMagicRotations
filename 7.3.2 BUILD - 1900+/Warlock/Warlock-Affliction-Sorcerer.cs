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
    public class WarlockAffliction : CombatRoutine
    {
        public override string Name
        {get{return "Affliction Warlock";}}

        public override string Class
        {get{return "Warlock";}}

        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
        }

        public override void Stop()
        {
        }

        public override void Pulse() // Updated for Legion (tested and working for single target)
        {
            if (combatRoutine.Type == RotationType.SingleTarget) // Do Single Target Stuff here
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerIsCasting && !WoW.IsMounted)
                {
                    //Agony
                    if ((!WoW.TargetHasDebuff("Agony") || WoW.TargetDebuffTimeRemaining("Agony") <= 540)
                        && WoW.CanCast("Agony")
                        && WoW.IsSpellInRange("Agony"))
                    {
                        WoW.CastSpell("Agony");
                        return;
                    }

                    //Corruption
                    if ((!WoW.TargetHasDebuff("Corruption") || (WoW.TargetDebuffTimeRemaining("Corruption") <= 420 && WoW.TargetDebuffTimeRemaining("Corruption") > 0))
                        && WoW.CanCast("Corruption")
                        && WoW.IsSpellInRange("Agony"))
                    {
                        WoW.CastSpell("Corruption");
                        return;
                    }

                    //Siphon Life
                    if ((!WoW.TargetHasDebuff("Siphon Life") || WoW.TargetDebuffTimeRemaining("Siphon Life") <= 420)
                        && (!WoW.PlayerIsChanneling || WoW.TargetDebuffTimeRemaining("Siphon Life") <= 150)
                        && WoW.Talent(7) == 2
                        && WoW.CanCast("Siphon Life")
                        && WoW.IsSpellInRange("Agony"))
                    {
                        WoW.CastSpell("Siphon Life");  
                        return;
                    }

                    //Phantom Singularity
                    if (WoW.CanCast("Phantom Singularity")
						&& WoW.Talent(4) == 1
                        && WoW.IsSpellInRange("Agony"))
                    {
                        WoW.CastSpell("Phantom Singularity");
                        return;
                    }

                    //Unstable Affliction
                    if ((WoW.CurrentSoulShards >= 3 || (WoW.CurrentSoulShards >= 2 && WoW.WasLastCasted("Unstable Affliction")))
                        && !WoW.IsMoving
                        && WoW.CanCast("Unstable Affliction")
                        && WoW.IsSpellInRange("Agony"))
                    {
                        WoW.CastSpell("Unstable Affliction");
                        Thread.Sleep(1000);
                        return;
                    }

                    //Reap Souls
                    if ((WoW.TargetHasDebuff("Unstable Affliction1") && WoW.TargetHasDebuff("Unstable Affliction2")
                        || WoW.TargetHasDebuff("Unstable Affliction1") && WoW.TargetHasDebuff("Unstable Affliction3")
                        || WoW.TargetHasDebuff("Unstable Affliction1") && WoW.TargetHasDebuff("Unstable Affliction4")
                        || WoW.TargetHasDebuff("Unstable Affliction1") && WoW.TargetHasDebuff("Unstable Affliction5")
                        || WoW.TargetHasDebuff("Unstable Affliction2") && WoW.TargetHasDebuff("Unstable Affliction3")
                        || WoW.TargetHasDebuff("Unstable Affliction2") && WoW.TargetHasDebuff("Unstable Affliction4")
                        || WoW.TargetHasDebuff("Unstable Affliction2") && WoW.TargetHasDebuff("Unstable Affliction5")
                        || WoW.TargetHasDebuff("Unstable Affliction3") && WoW.TargetHasDebuff("Unstable Affliction4")
                        || WoW.TargetHasDebuff("Unstable Affliction3") && WoW.TargetHasDebuff("Unstable Affliction5")
                        || WoW.TargetHasDebuff("Unstable Affliction4") && WoW.TargetHasDebuff("Unstable Affliction5")
                        || WoW.PlayerBuffStacks("Tormented Souls") >= 5)
                        && !WoW.PlayerIsCasting
                        && WoW.CanCast("Reap Souls")
                        && !WoW.PlayerHasBuff("Deadwind Harvester")
                        && WoW.PlayerHasBuff("Tormented Souls"))
                    {
                        WoW.CastSpell("Reap Souls");
                        return;
                    }

                    if (WoW.CanCast("Life Tap") && !WoW.PlayerIsChanneling && WoW.Talent(2) == 3 && !WoW.PlayerHasBuff("Empowered Life Tap"))
                    {
                        WoW.CastSpell("Life Tap");
                        return;
                    }
                    

                    if (WoW.CanCast("Felhunter") && WoW.Talent(6) == 2 && !WoW.IsSpellOnCooldown("Felhunter") && WoW.IsSpellInRange("Agony") && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting)
                    {
                        WoW.CastSpell("Felhunter");
                        return;
                    }					

                    if (WoW.Mana < 40 && WoW.HealthPercent > 70 && WoW.CanCast("Life Tap"))
                    {
                        WoW.CastSpell("Life Tap");
                        return;
                    }

                    if (WoW.CanCast("Haunt") && WoW.Talent(1) == 1 && !WoW.IsSpellOnCooldown("Haunt") && WoW.IsSpellInRange("Agony") && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Haunt");
                        return;
                    }

                    if (WoW.CanCast("Drain Soul") && WoW.IsSpellInRange("Agony") && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Drain Soul");
                        return;
                    }
                }
            }

            if (combatRoutine.Type == RotationType.AOE)
            {
                //Phantom Singularity
                if (WoW.CanCast("Phantom Singularity")
                    && WoW.Talent(4) == 1
                    && WoW.IsSpellInRange("Agony"))
                {
                    WoW.CastSpell("Phantom Singularity");
                    return;
                }

                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting && !WoW.IsMounted) // Do AOE stuff here
                {                    

                    if (WoW.CanCast("Seed of Corruption") && WoW.IsSpellInRange("Agony") && !WoW.TargetHasDebuff("Seed of Corruption") && !WoW.IsMoving && WoW.CurrentSoulShards >= 1)
                    {
                        WoW.CastSpell("Seed of Corruption");
                        return;
                    }

                    if (WoW.CanCast("Agony") && WoW.IsSpellInRange("Agony") && WoW.TargetHasDebuff("Seed of Corruption") && (!WoW.TargetHasDebuff("Agony") || (WoW.TargetDebuffTimeRemaining("Agony") <= 540)))
                    {
                        WoW.CastSpell("Agony");
                        return;
                    }

                    if (WoW.CanCast("Corruption") && WoW.IsSpellInRange("Agony") && WoW.TargetHasDebuff("Seed of Corruption") && (!WoW.TargetHasDebuff("Corruption") || (WoW.TargetDebuffTimeRemaining("Corruption") <= 420)))
                    {
                        WoW.CastSpell("Corruption");
                        return;
                    }
                }
            }
            if (combatRoutine.Type == RotationType.SingleTargetCleave)
            {
                // Do Single Target Cleave stuff here if applicable else ignore this one
            }
        }



        #region Functions

        public override int Interrupt_Ability_Id
        {
            get
            {
                return 119910;
            }
        }

        private static bool lastNamePlate = true;
        public override int AoE_Range
        {
            get
            {
                return 40;
            }
        }

        public override int SINGLE { get { return 1; } }   //please Set between 1-99 NpC in range for ST if not desired set to 99
        public override int CLEAVE { get { return 2; } }   //please Set between 1-99 NpC in range for AOE  if not desired set to 99
        public override int AOE { get { return 4; } }      //please Set between 1-99 NpC in range for Cleave if not desired set to 99

        #endregion 

    }
}


/*
[AddonDetails.db]
AddonAuthor=Sorcerer
AddonName=Quartz
WoWVersion=Legion - 72000
[SpellBook.db]
Spell,980,Agony,NumPad1
Spell,63106,Siphon Life,NumPad2
Spell,172,Corruption,NumPad3
Spell,30108,Unstable Affliction,NumPad4
Spell,216698,Reap Souls,NumPad5
Spell,1454,Life Tap,NumPad7
Spell,48181,Haunt,NumPad8
Spell,198590,Drain Soul,Add
Spell,27243,Seed of Corruption,NumPad0
Spell,111897,Felhunter,NumPad9
Spell,205179,Phantom Singularity,NumPad8
Aura,980,Agony
Aura,27243,Seed of Corruption
Aura,146739,Corruption
Aura,63106,Siphon Life
Aura,233490,Unstable Affliction1
Aura,233496,Unstable Affliction2
Aura,233497,Unstable Affliction3
Aura,233498,Unstable Affliction4
Aura,233499,Unstable Affliction5
Aura,216708,Deadwind Harvester
Aura,216695,Tormented Souls
Aura,235156,Empowered Life Tap
Aura,127271,Mount
*/
