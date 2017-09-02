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
    public class WarlockDemonology : CombatRoutine
    {
        public override string Name
        { get { return "Demonology Warlock"; } }

        public override string Class
        { get { return "Warlock"; } }

        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
            Log.Write("Welcome to the Demonology Warlock rotation", Color.Purple);
            Log.Write("Use Scroll Lock key to toggle ST/AOE/CLEAVE auto detection", Color.Blue);
            Log.Write("If Scroll Lock LED is ON ST/AOE/CLEAVE auto detection is ENABLED", Color.Blue);
            Log.Write("If Scroll Lock LED is OFF ST/AOE/CLEAVE auto detection is DISABLED use the manual mode to select ST/AOE/CLEAVE (Default: ALT+S, ALT+A)", Color.Blue);
        }

        public override void Stop()
        {
        }

        public override void Pulse() // Updated for Legion (tested and working for single target)
        {
            if (WoW.IsInCombat && Control.IsKeyLocked(Keys.Scroll) && !WoW.TargetIsPlayer && !WoW.IsMounted)
            {
                SelectRotation(4, 9999, 1);
            }

            //Dark Pact
            if (WoW.CanCast("Dark Pact")
                && WoW.Talent(5) == 3
                && WoW.HealthPercent <= 30
                && !WoW.IsMounted)
            {
                WoW.CastSpell("Dark Pact");
                return;
            }

            //Shadowfury
            if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_LMENU) < 0
                && WoW.Talent(3) == 3
                && !WoW.IsMoving
                && WoW.CanCast("Shadowfury"))
            {
                WoW.CastSpell("Shadowfury");
                return;
            }

            if (UseCooldowns)
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerIsChanneling && WoW.IsInCombat && !WoW.PlayerIsCasting && !WoW.IsMounted)
                {
                    //Doomguard
                    if (WoW.CanCast("Doomguard")
                        && (WoW.Talent(6) == 0 || WoW.Talent(6) == 2 || WoW.Talent(6) == 3)
                        && WoW.CurrentSoulShards >= 1
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Doomguard");
                        return;
                    }

                    //Grimoire of Service
                    if (WoW.CanCast("Grimoire: Felguard")
                        && WoW.Talent(6) == 2
                        && WoW.CurrentSoulShards >= 1
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Grimoire: Felguard");
                        return;
                    }

                    //Soul Harvest
                    if (WoW.CanCast("Soul Harvest")
                        && WoW.Talent(4) == 3
                        && !WoW.IsMoving
                        && WoW.IsSpellInRange("Doom")
                        && (WoW.PlayerHasBuff("Bloodlust") || WoW.PlayerHasBuff("Time Warp") || WoW.PlayerHasBuff("Netherwinds") || WoW.PlayerHasBuff("Drums of War") || WoW.PlayerHasBuff("Heroism")))
                    {
                        WoW.CastSpell("Soul Harvest");
                        return;
                    }
                }
            }

            if (combatRoutine.Type == RotationType.SingleTarget) // Do Single Target Stuff here
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerIsChanneling && WoW.IsInCombat && !WoW.PlayerIsCasting && !WoW.IsMounted)
                {
                    if ((!WoW.TargetHasDebuff("Doom") || WoW.TargetDebuffTimeRemaining("Doom") <= 150)
                        && WoW.CanCast("Doom")
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Doom");
                        return;
                    }

                    if (WoW.CanCast("Darkglare")
                        && WoW.Talent(7) == 1
                        && WoW.CurrentSoulShards >= 1
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Darkglare");
                        return;
                    }

                    if (WoW.CanCast("Call Dreadstalkers")
                        && (WoW.CurrentSoulShards >= 2 || WoW.TargetHasDebuff("Demonic Calling"))
                        && WoW.IsSpellInRange("Doom")
                        && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Call Dreadstalkers");
                        return;
                    }

                    if (WoW.CanCast("Hand of Guldan")
                        && WoW.CurrentSoulShards >= 4
                        && WoW.IsSpellInRange("Doom")
                        && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Hand of Guldan");
                        return;
                    }

                    if (WoW.CanCast("Demonic Empowerment")
                        && !WoW.IsMoving
                        && !WoW.WasLastCasted("Demonic Empowerment")
                        && (!WoW.PetHasBuff("Demonic Empowerment") || WoW.PetBuffTimeRemaining("Demonic Empowerment") <= 1.5
                        || WoW.WasLastCasted("Call Dreadstalkers") || WoW.WasLastCasted("Grimoire: Felguard") || WoW.WasLastCasted("Doomguard") || WoW.WasLastCasted("Hand of Guldan")))
                    {
                        WoW.CastSpell("Demonic Empowerment");
                        Thread.Sleep(1000);
                        return;
                    }

                    if (WoW.CanCast("Talkiels Consumption")
                        && WoW.PetHasBuff("Demonic Empowerment")
                        && WoW.PetBuffTimeRemaining("Demonic Empowerment") >= 2
                        && WoW.DreadstalkersCount >= 1
                        && WoW.IsSpellInRange("Doom")
                        && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Talkiels Consumption");
                        return;
                    }

                    if (WoW.CanCast("Felstorm")
                        && WoW.PetHasBuff("Demonic Empowerment")
                        && WoW.PetBuffTimeRemaining("Demonic Empowerment") >= 6
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Felstorm");
                        return;
                    }

                    if (WoW.CanCast("Shadowflame")
                        && WoW.Talent(1) == 2
                        && !WoW.TargetHasDebuff("Shadowflame")
                        && WoW.CanCast("Shadowflame")
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Shadowflame");
                        return;
                    }

                    if (WoW.CanCast("Life Tap")
                        && WoW.Mana < 60
                        && WoW.HealthPercent > 50)
                    {
                        WoW.CastSpell("Life Tap");
                        return;
                    }

                    if (WoW.CanCast("Demonwrath")
                        && WoW.Mana > 60
                        && WoW.IsMoving)
                    {
                        WoW.CastSpell("Demonwrath");
                        return;
                    }

                    if ((WoW.CanCast("Shadow Bolt") || WoW.CanCast("Demonbolt"))
                        && WoW.IsSpellInRange("Doom")
                        && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Shadow Bolt");
                        WoW.CastSpell("Demonbolt");
                        return;
                    }
                }
            }
            if (combatRoutine.Type == RotationType.AOE)
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerIsChanneling && WoW.IsInCombat && !WoW.PlayerIsCasting && !WoW.IsMounted)
                {
                    if (WoW.CanCast("Hand of Guldan")
                        && WoW.CurrentSoulShards >= 4
                        && WoW.IsSpellInRange("Doom")
                        && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Hand of Guldan");
                        return;
                    }

                    if (WoW.CanCast("Implosion")
                        && WoW.Talent(2) == 3
                        && WoW.WildImpsCount >= 1
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Implosion");
                        return;
                    }

                    if (WoW.CanCast("Darkglare")
                        && WoW.Talent(7) == 1
                        && WoW.CurrentSoulShards >= 1
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Darkglare");
                        return;
                    }

                    if (WoW.CanCast("Demonic Empowerment")
                       && WoW.CanCast("Felstorm")
                       && !WoW.IsMoving
                       && !WoW.WasLastCasted("Demonic Empowerment")
                       && (!WoW.PetHasBuff("Demonic Empowerment") || WoW.PetBuffTimeRemaining("Demonic Empowerment") <= 6))
                    {
                        WoW.CastSpell("Demonic Empowerment");
                        Thread.Sleep(2000);
                        return;
                    }

                    if (WoW.CanCast("Felstorm")
                        && WoW.PetHasBuff("Demonic Empowerment")
                        && WoW.PetBuffTimeRemaining("Demonic Empowerment") >= 6
                        && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Felstorm");
                        return;
                    }

                    if (WoW.CanCast("Life Tap")
                        && WoW.Mana < 60
                        && WoW.HealthPercent > 50)
                    {
                        WoW.CastSpell("Life Tap");
                        return;
                    }

                    if (WoW.CanCast("Demonwrath")
                        && WoW.Mana > 60)
                    {
                        WoW.CastSpell("Demonwrath");
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

        public override int CLEAVE { get { return 99; } }   //please Set between 1-99 NpC in range for AOE  if not desired set to 99
        public override int AOE { get { return 99; } }      //please Set between 1-99 NpC in range for Cleave if not desired set to 99
        public override int SINGLE { get { return 99; } }   //please Set between 1-99 NpC in range for ST if not desired set to 99        

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
Spell,686,Shadow Bolt,NumPad1
Spell,157695,Demonbolt,NumPad1
Spell,104316,Call Dreadstalkers,NumPad2
Spell,105174,Hand of Guldan,NumPad3
Spell,193396,Demonic Empowerment,NumPad4
Spell,603,Doom,NumPad5
Spell,193440,Demonwrath,NumPad6
Spell,1454,Life Tap,NumPad7
Spell,205180,Darkglare,NumPad8
Spell,111897,Grimoire: Felguard,NumPad9
Spell,211714,Talkiels Consumption,Add
Spell,205181,Shadowflame,NumPad0
Spell,18540,Doomguard,Decimal
Spell,119914,Felstorm,D4
Spell,196098,Soul Harvest,D0
Spell,196277,Implosion,D7
Spell,30283,Shadowfury,D3
Spell,108416,Dark Pact,Multiply
Aura,2825,Bloodlust
Aura,32182,Heroism
Aura,80353,Time Warp
Aura,160452,Netherwinds
Aura,230935,Drums of War
Aura,603,Doom
Aura,193396,Demonic Empowerment
Aura,205146,Demonic Calling
Aura,205181,Shadowflame
Aura,127271,Mount
*/
