using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Forms;
using CloudMagic.Helpers;
using CloudMagic.GUI;

namespace CloudMagic.Rotation
{
    public class RetributionPaladin : CombatRoutine
    {
        private readonly Stopwatch BeastCleave = new Stopwatch();

        public override string Name
        {get{return "Retribution Paladin";}}

        public override string Class
        {get{return "Paladin";}}

        public override Form SettingsForm { get; set; }        

        public override void Initialize()
        {
            Log.Write("Welcome to Retribution Paladin", Color.Green);
            Log.Write("Use Scroll Lock key to toggle ST/AOE/CLEAVE auto detection", Color.Blue);
            Log.Write("If Scroll Lock LED is ON ST/AOE/CLEAVE auto detection is ENABLED", Color.Blue);
            Log.Write("If Scroll Lock LED is OFF ST/AOE/CLEAVE auto detection is DISABLED use the manual mode to select ST/AOE/CLEAVE (Default: ALT+S, ALT+A)", Color.Blue);
        }

        public override void Stop()
        {
        }

        public override void Pulse()        // Updated for Legion (tested and working for single target)
        {
            if (WoW.IsInCombat && Control.IsKeyLocked(Keys.Scroll) && !WoW.TargetIsPlayer && !WoW.IsMounted)
            {
                SelectRotation(4, 9999, 1);                
            }

            //Healthstone - Potion
            if ((WoW.CanCast("Healthstone") || WoW.CanCast("Potion"))
                && (WoW.ItemCount("Healthstone") >= 1 || WoW.ItemCount("Potion") >= 1)
                && (!WoW.ItemOnCooldown("Healthstone") || !WoW.ItemOnCooldown("Potion"))                
                && WoW.HealthPercent <= 30
                && !WoW.IsMounted)
            {
                WoW.CastSpell("Healthstone");
                WoW.CastSpell("Potion");
                return;
            }

            //Shield of Vengeance
            if (WoW.CanCast("Shield of Vengeance")
                && WoW.HealthPercent <= 40
                && !WoW.IsMounted)
            {
                WoW.CastSpell("Shield of Vengeance");
                return;
            }

            //Lay on Hands
            if (WoW.CanCast("Lay on Hands")
                && WoW.HealthPercent <= 20
                && !WoW.IsMounted)
            {
                WoW.CastSpell("Lay on Hands");
                return;
            }

            //Divine Steed
            if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_4) < 0
                && WoW.CanCast("Divine Steed")
                && !WoW.PlayerHasBuff("Divine Steed"))
            {
                WoW.CastSpell("Divine Steed");
                return;
            }

            if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting && WoW.HealthPercent != 0)
            {
                //Crusade
                if (WoW.CanCast("Crusade")
                    && WoW.Talent(7) == 2
                    && WoW.CurrentHolyPower >= 3
                    && WoW.IsSpellInRange("Templar Verdict")
                    && (WoW.PlayerHasBuff("Bloodlust") || WoW.PlayerHasBuff("Time Warp") || WoW.PlayerHasBuff("Netherwinds") || WoW.PlayerHasBuff("Drums of War")))
                {
                    WoW.CastSpell("Crusade");
                    return;
                }

                //Avenging Wrath
                if (WoW.CanCast("Avenging Wrath")
                    && WoW.CurrentHolyPower >= 3
                    && WoW.IsSpellInRange("Templar Verdict")
                    && (WoW.PlayerHasBuff("Bloodlust") || WoW.PlayerHasBuff("Time Warp") || WoW.PlayerHasBuff("Netherwinds") || WoW.PlayerHasBuff("Drums of War")))
                {
                    WoW.CastSpell("Avenging Wrath");
                    return;
                }

                //Hammer of Justice
                if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_ADD) < 0
                   && WoW.CanCast("Hammer of Justice")
                    )
                {
                    WoW.CastSpell("Hammer of Justice");
                    return;
                }

                //Holy Wrath
                if (WoW.CanCast("Holy Wrath")
                    && WoW.Talent(7) == 3
                    && WoW.HealthPercent <= 40
                    && WoW.IsSpellInRange("Templar Verdict"))
                {
                    WoW.CastSpell("Holy Wrath");
                    return;
                }

                //Single Target Rotation

                //Execution Sentence
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.CanCast("Execution Sentence")                    
                    && WoW.TargetHasDebuff("Judgement")
                    && WoW.TargetDebuffTimeRemaining("Judgement") >= 650
                    && WoW.Talent(1) == 2)
                {
                    WoW.CastSpell("Execution Sentence");
                    return;
                }

                //Justicar's Vengeance
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.CanCast("Justicars Vengeance")                    
                    && WoW.PlayerHasBuff("Divine Purpose")
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.Talent(5) == 1)
                {
                    WoW.CastSpell("Justicars Vengeance");
                    return;
                }

                //Templar's Verdict
                if (combatRoutine.Type == RotationType.SingleTarget
                    && (WoW.CurrentHolyPower >= 3 || WoW.PlayerHasBuff("Divine Purpose") || (WoW.CurrentHolyPower >= 2 && WoW.PlayerHasBuff("The Fires of Justice")))
                    && WoW.CanCast("Templar Verdict")
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.TargetHasDebuff("Judgement")
                    && WoW.TargetDebuffTimeRemaining("Judgement") >= 50)
                {
                    WoW.CastSpell("Templar Verdict");
                    return;
                }

                //Judgement
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.CanCast("Judgement")
                    && WoW.CurrentHolyPower >= 3)
                {
                    WoW.CastSpell("Judgement");
                    return;
                }

                //Wake of Ashes
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.CurrentHolyPower == 0
                    && WoW.CanCast("Wake of Ashes")
                    && WoW.IsSpellInRange("Templar Verdict"))
                {
                    WoW.CastSpell("Wake of Ashes");
                    return;
                }

                //Blade of Justice
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.CanCast("Blade of Justice")
                    && WoW.IsSpellInRange("Blade of Justice")
                    && WoW.CurrentHolyPower <= 3
                    && WoW.Talent(4) != 3)
                {
                    WoW.CastSpell("Blade of Justice");
                    return;
                }

                //Divine Hammer
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.CanCast("Divine Hammer")
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.CurrentHolyPower <= 3
                    && WoW.Talent(4) == 3)
                {
                    WoW.CastSpell("Divine Hammer");
                    return;
                }

                //Crusader Strike
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.CurrentHolyPower < 5
                    && WoW.PlayerSpellCharges("Crusader Strike") >= 1
                    && WoW.CanCast("Crusader Strike")
                    && WoW.Talent(2) != 2)
                {
                    WoW.CastSpell("Crusader Strike");
                    return;
                }

                //Zeal
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.CurrentHolyPower < 5
                    && WoW.PlayerSpellCharges("Zeal") >= 1
                    && WoW.CanCast("Zeal")
                    && WoW.Talent(2) == 2)
                {
                    WoW.CastSpell("Zeal");
                    return;
                }

                //Consecration
                if (combatRoutine.Type == RotationType.SingleTarget
                    && WoW.CanCast("Consecration")
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.Talent(1) == 3)
                {
                    WoW.CastSpell("Consecration");
                    return;
                }

                //AoE Rotation = 3+ Targets

                //Divine Storm
                if (combatRoutine.Type == RotationType.AOE
                    && (WoW.CurrentHolyPower >= 3 || WoW.PlayerHasBuff("Divine Purpose") || (WoW.CurrentHolyPower >= 2 && WoW.PlayerHasBuff("The Fires of Justice")))
                    && WoW.CanCast("Divine Storm")
                    && WoW.IsSpellInRange("Templar Verdict"))
                {
                    WoW.CastSpell("Divine Storm");
                    return;
                }

                //Judgement
                if (combatRoutine.Type == RotationType.AOE
                    && WoW.CanCast("Judgement"))
                {
                    WoW.CastSpell("Judgement");
                    return;
                }

                //Wake of Ashes
                if (combatRoutine.Type == RotationType.AOE
                    && WoW.CurrentHolyPower == 0
                    && WoW.CanCast("Wake of Ashes")
                    && WoW.IsSpellInRange("Templar Verdict"))
                {
                    WoW.CastSpell("Wake of Ashes");
                    return;
                }

                //Consecration
                if (combatRoutine.Type == RotationType.AOE
                    && WoW.CanCast("Consecration")
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.Talent(1) == 3)
                {
                    WoW.CastSpell("Consecration");
                    return;
                }

                //Blade of Justice
                if (combatRoutine.Type == RotationType.AOE
                    && WoW.CanCast("Blade of Justice")
                    && WoW.IsSpellInRange("Blade of Justice")
                    && WoW.CurrentHolyPower <= 3
                    && WoW.Talent(4) != 3)
                {
                    WoW.CastSpell("Blade of Justice");
                    return;
                }

                //Divine Hammer
                if (combatRoutine.Type == RotationType.AOE
                    && WoW.CanCast("Divine Hammer")
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.CurrentHolyPower <= 3
                    && WoW.Talent(4) == 3)
                {
                    WoW.CastSpell("Divine Hammer");
                    return;
                }

                //Crusader Strike
                if (combatRoutine.Type == RotationType.AOE
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.CurrentHolyPower < 5
                    && WoW.PlayerSpellCharges("Crusader Strike") >= 1
                    && WoW.CanCast("Crusader Strike")
                    && WoW.Talent(2) != 2)
                {
                    WoW.CastSpell("Crusader Strike");
                    return;
                }

                //Zeal
                if (combatRoutine.Type == RotationType.AOE
                    && WoW.IsSpellInRange("Templar Verdict")
                    && WoW.CurrentHolyPower < 5
                    && WoW.PlayerSpellCharges("Zeal") >= 1
                    && WoW.CanCast("Zeal")
                    && WoW.Talent(2) == 2)
                {
                    WoW.CastSpell("Zeal");
                    return;
                }


                if (combatRoutine.Type == RotationType.SingleTargetCleave) //Cleave rotation = 2 targets
                {
                    // Do Single Target Cleave stuff here if applicable else ignore this one
                }

            }
        }

        #region Functions

        private float GCD
        {
            get
            {
                if (Convert.ToSingle(150f / (1 + (WoW.HastePercent / 100f))) > 75f)
                {
                    return Convert.ToSingle(150f / (1 + (WoW.HastePercent / 100f)));
                }
                else
                {
                    return 75f;
                }
            }
        }

        private void interruptcast()
        {
            Random random = new Random();
            int randomNumber = random.Next(60, 80);
            if (WoW.TargetPercentCast > randomNumber && WoW.TargetIsCastingAndSpellIsInterruptible)
            {
                if (WoW.PlayerRace == "BloodElf" && WoW.CanCast("Arcane Torrent", true, true, false, false, true) && !WoW.IsSpellOnCooldown("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible) //interupt every spell, not a boss.
                {
                    WoW.CastSpell("Arcane Torrent");
                    return;
                }
                if (WoW.PlayerRace == "Pandaren" && WoW.CanCast("Quaking palm", true, true, true, false, true)) //interupt every spell, not a boss.
                {
                    WoW.CastSpell("Quaking palm");
                    return;
                }
            }
        }

        private void DBMPrePull()
        {
            if (dbmOn && dbmTimer <= 18 && dbmTimer > 0 && WoW.HasTarget)
            {
                if (!WoW.ItemOnCooldown("Prolonged Power"))
                {
                    WoW.CastSpell("Prolonged Power");
                    return;
                }
            }
        }

        private void Defensive()
        {
            if (WoW.PlayerRace == "Dreanei" && WoW.HealthPercent < 80 && !WoW.IsSpellOnCooldown("Gift Naaru"))
            {
                WoW.CastSpell("Gift Naaru");
            }
        }

        private void Stuns()
        {
            if (!WoW.PlayerIsCasting)
            {
                if (WoW.PlayerRace == "Tauren​" && !WoW.IsMoving && WoW.CanCast("War Stomp") && !WoW.IsSpellOnCooldown("War Stomp"))
                {
                    WoW.CastSpell("War Stomp");
                    return;
                }
            }
        }

        private void DPSRacial()
        {
            if (!WoW.PlayerIsCasting)
            {
                if (WoW.PlayerRace == "Troll" && WoW.CanCast("Berserking") && !WoW.IsSpellOnCooldown("Berserking"))
                {
                    WoW.CastSpell("Berserking");
                    return;
                }

                if (WoW.PlayerRace == "Orc" && WoW.CanCast("Blood Fury") && !WoW.IsSpellOnCooldown("Blood Fury"))
                {
                    WoW.CastSpell("Blood Fury");
                    return;
                }
            }

        }

        private void UsePotion()
        {
            if (!WoW.ItemOnCooldown("Prolonged Power"))
            {
                WoW.CastSpell("Prolonged Power");
                return;
            }
        }

        private static bool lastNamePlate = true;
        public void SelectRotation(int aoe, int cleave, int single)
        {
            int count = WoW.CountEnemyNPCsInRange;
            if (!lastNamePlate)
            {
                combatRoutine.ChangeType(RotationType.SingleTarget);
                lastNamePlate = true;
            }
            lastNamePlate = WoW.Nameplates;
            if (count >= aoe)
                combatRoutine.ChangeType(RotationType.AOE);
            if (count == cleave)
                combatRoutine.ChangeType(RotationType.SingleTargetCleave);
            if (count <= single)
                combatRoutine.ChangeType(RotationType.SingleTarget);

        }

        /*  private void ChangeTarget()
          {
              await Task.Run(() =>
              {
                  if (PlayerSpec == "Enhancement" && !TargetInfo.Melee)
                      WoW.KeyPressRelease(WoW.Keys.Tab);
                  if (PlayerSpec == "Elemental " && !TargetInfo.Range)
                      WoW.KeyPressRelease(WoW.Keys.Tab);
              });
          }*/

        private static bool setBonus2Pc
        {
            get
            {
                var control = WoW.GetBlockColor(9, 24);
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(control.G))) == 0 && Convert.ToInt32(Math.Round(Convert.ToSingle(control.R) * 100 / 255)) >= 2)
                {

                    return true;
                }
                else
                    return false;
            }
        }

        private static bool setBonus4Pc
        {
            get
            {
                var control = WoW.GetBlockColor(9, 24);
                //Log.Write("Bonus location: " + Convert.ToInt32(Math.Round(Convert.ToSingle(control.R) * 100 / 255)));
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(control.G))) == 0 && Convert.ToInt32(Math.Round(Convert.ToSingle(control.R) * 100 / 255)) >= 4)
                {

                    return true;
                }
                else
                    return false;
            }
        }

        private static string PlayerSpec
        {
            get
            {
                var c = WoW.GetBlockColor(10, 24);
                try
                {
                    if (c.B == 0) return "none";
                    string[] Spec = new string[] { "None", "Blood", "Frost", "Unholy", "Havoc", "Vengeance", "Balance", "Feral", "Guardian", "Restoration", "Beast Mastery", "Marksmanship", "Survival", "Arcane", "Fire", "Frost", "Brewmaster", "Mistweaver", "Windwalker", "Holy", "Protection", "Retribution", "Discipline", "HolyPriest", "Shadow", "Assassination", "Outlaw", "Subtlety", "Elemental", "Enhancement", "RestorationShaman", "Affliction", "Arms", "Fury", "Protection", "Demonology", "Destruction", "none" };
                    var race = Convert.ToInt32(Math.Round(Convert.ToSingle(c.G))) * 100 / 255;
                    var spec = Spec[race];

                    return spec;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Spec  Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return "none";
            }
        }

        private static int npcCount
        {
            get
            {
                var c = WoW.GetBlockColor(11, 23);
                try
                {
                    return Convert.ToInt32(Math.Round(Convert.ToSingle(c.G) * 100 / 255));
                }
                catch (Exception ex)
                {
                    Log.Write("Error in NamePlate  Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return 1;
            }
        }

        private static bool Nameplates
        {
            get
            {
                var c = WoW.GetBlockColor(11, 23);
                try
                {
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.B) / 255)) == 1)
                        return true;
                    else
                        return false;

                }
                catch (Exception ex)
                {
                    Log.Write("Error in NamePlate  Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return false;
            }
        }

        private static int RaidSize
        {
            get
            {
                var c = WoW.GetBlockColor(11, 23);
                try
                {
                    int players = 0;
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R)) * 100 / 255) > 0)
                        players = (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R)) * 100 / 255));
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R)) * 100 / 255) == 100)
                        players = 1;
                    if (players > 30)
                        players = 30;
                    if (players <= 5)
                        players = players - 1;
                    return players;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Players  Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return 1;
            }
        }

        private static bool dbmOn
        {
            get
            {
                Color pixelColor = Color.FromArgb(0);
                var c = WoW.GetBlockColor(6, 24);
                try
                {
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) / 255)) == 1)
                        return true;
                    else
                        return false;

                }
                catch (Exception ex)
                {
                    Log.Write("Error in DBMON  Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return false;
            }
        }

        private static int dbmTimer
        {
            get
            {
                Color pixelColor = Color.FromArgb(0);
                var c = WoW.GetBlockColor(6, 24);
                try
                {
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) / 255)) == 1)
                        return Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255));
                    else
                        return 0;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Dbm Timer Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return 0;
            }
        }

        #endregion

    }
}



/*
[AddonDetails.db]
AddonAuthor=Sorcerer
AddonName=Quartz
WoWVersion=Legion - 72000
[SpellBook.db]
Spell,217020,Zeal,NumPad1
Spell,215661,Justicars Vengeance,D8
Spell,184662,Shield of Vengeance,NumPad0
Spell,853,Hammer of Justice,OemMinus
Spell,213757,Execution Sentence,D9
Spell,633,Lay on Hands,D6
Spell,205273,Wake of Ashes,NumPad5
Spell,53385,Divine Storm,NumPad6
Spell,184575,Blade of Justice,NumPad4
Spell,198034,Divine Hammer,NumPad4
Spell,35395,Crusader Strike,NumPad1
Spell,85256,Templar Verdict,NumPad2
Spell,20271,Judgement,NumPad3
Spell,224668,Crusade,Subtract
Spell,31884,Avenging Wrath,Subtract
Spell,19750,Flash of Light,D1
Spell,210220,Holy Wrath,D8
Spell,205228,Consecration,D8
Spell,5512,Healthstone,D1
Spell,127834,Potion,D1
Spell,190784,Divine Steed,D8
Aura,197277,Judgement
Aura,223819,Divine Purpose
Aura,209785,The Fires of Justice
Aura,2825,Bloodlust
Aura,80353,Time Warp
Aura,160452,Netherwinds
Aura,230935,Drums of War
Aura,127271,Mount
Aura,25771,Forbearance
Aura,190784,Divine Steed
Item,5512,Healthstone
Item,127834,Potion
*/
