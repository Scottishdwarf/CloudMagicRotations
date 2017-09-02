// winifix@gmail.com
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertPropertyToExpressionBody

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CloudMagic.Helpers;

namespace CloudMagic.Rotation
{
    public class ProtNil : CombatRoutine
    {
        public static bool autointerrupt;
        public Stopwatch CombatWatch = new Stopwatch();

        public Stopwatch interruptwatch = new Stopwatch();

        public override string Name
        {
            get { return "Prot Warrior"; }
        }

        public override string Class
        {
            get { return "Warrior"; }
        }

        public override Form SettingsForm { get; set; }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        public override void Initialize()
        {
            Log.Write("Welcome to Protection Warrior", Color.Green);
            Log.Write("Suggested build: 1213312", Color.Green);
            Log.Write("LEFT CTRL - Heroic Leap (please make @Cursor macro for it)", Color.Black);
            Log.Write("LEFT ALT - Shockwave + Neltharion's Fury if not on CD", Color.Black);
            //Log.Write("Suggested build: 1213312", Color.Black);
        }

        public override void Stop()
        {
        }


        public override void Pulse()
        {
            if (interruptwatch.ElapsedMilliseconds == 0)
            {
                interruptwatch.Start();
                return;
            }


            if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_F3) < 0)
            {
                if (interruptwatch.ElapsedMilliseconds > 1000)
                {
                    autointerrupt = !autointerrupt;
                    Log.Write("Interrupt " + (autointerrupt ? "On" : "Off"));
                    Log.WriteCloudMagic("Auto interrupt " + (autointerrupt ? "ON" : "OFF"), Color.Red);
                    interruptwatch.Restart();
                }
            }

            if (WoW.PlayerHasBuff("Mount")) return;

            if (WoW.IsInCombat && WoW.HealthPercent < 35 && WoW.CanCast("Last Stand") && !WoW.IsSpellOnCooldown("Last Stand"))
            {
                WoW.CastSpell("Last Stand");
                return;
            }
            if (WoW.IsInCombat && WoW.HealthPercent < 20 && WoW.CanCast("Shield Wall") && !WoW.IsSpellOnCooldown("Shield Wall"))
            {
                WoW.CastSpell("Shield Wall");
                return;
            }

            if (autointerrupt && WoW.IsInCombat && !WoW.IsSpellOnCooldown("Pummel") && WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsSpellInRange("Shield Slam") && WoW.TargetIsCasting &&
                WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 80)
            {
                WoW.CastSpell("Pummel");
                return;
            }


            if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_LCONTROL) < 0)
            {
                if (WoW.IsInCombat && !WoW.IsSpellOnCooldown("HeroicLeap"))
                {
                    WoW.CastSpell("HeroicLeap");
                    return;
                }
                return;
            }
            if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_1) < 0)
            {
                if (WoW.IsInCombat && !WoW.IsSpellOnCooldown("Thunder Clap"))
                {
                    WoW.CastSpell("Thunder Clap");
                    return;
                }
                return;
            }
            if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_LMENU) < 0)
            {
                if (WoW.IsInCombat && !WoW.IsSpellOnCooldown("Shockwave"))
                {
                    WoW.CastSpell("Shockwave");
                    return;
                }
                return;
            }

            if (combatRoutine.Type == RotationType.SingleTarget || combatRoutine.Type == RotationType.AOE) // Do Single Target Stuff here
            {
                if (CombatWatch.IsRunning && !WoW.IsInCombat)
                {
                    CombatWatch.Reset();
                }
                if (!CombatWatch.IsRunning && WoW.IsInCombat)
                {
                    CombatWatch.Start();
                }

                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerIsChanneling)
                {
                    if (!WoW.TargetHasDebuff("ShockWavestun") && WoW.IsInCombat)

                    {
                        if (WoW.CanCast("Shield Block") && WoW.Rage >= 15 && !WoW.PlayerIsChanneling &&
                            (WoW.PlayerSpellCharges("Shield Block") == 2 ||
                             (WoW.PlayerSpellCharges("Shield Block") >= 1 && WoW.HealthPercent < 100 && WoW.PlayerBuffTimeRemaining("ShieldBlockAura") <= 2) ||
                             WoW.PlayerSpellCharges("Shield Block") == 1 && WoW.HealthPercent < 100))
                        {
                            WoW.CastSpell("Shield Block");
                            return;
                        }

                        if (WoW.IsSpellInRange("Shield Slam") && WoW.CanCast("Thunder Clap") && !WoW.IsSpellOnCooldown("Thunder Clap") && CombatWatch.ElapsedMilliseconds > 1000 &&
                            CombatWatch.ElapsedMilliseconds < 5000)
                        {
                            WoW.CastSpell("Thunder Clap");
                            return;
                        }

                        /* ------------------ IGNORE PAIN MANAGEMENT----------------------*/

                        if (WoW.CanCast("Ignore Pain") && WoW.PlayerHasBuff("Vengeance: Ignore Pain") && WoW.Rage >= 39)
                        {
                            WoW.CastSpell("Ignore Pain");
                            return;
                        }

                        if (WoW.CanCast("Ignore Pain") && WoW.Rage > 30 && WoW.HealthPercent < 100 && (!WoW.PlayerHasBuff("Ignore Pain") || WoW.PlayerBuffTimeRemaining("Ignore Pain") <= 2) &&
                            !WoW.PlayerHasBuff("Vengeance: Ignore Pain") && !WoW.PlayerHasBuff("Vengeance: Focused Rage"))
                        {
                            WoW.CastSpell("Ignore Pain");
                            return;
                        }

                        /* ------------------ END IGNORE PAIN MANAGEMENT-------------------*/

                        if (WoW.TargetIsCasting && WoW.CanCast("SpellReflect") && !WoW.IsSpellOnCooldown("SpellReflect"))
                        {
                            WoW.CastSpell("SpellReflect");
                        }
                        if (WoW.IsSpellInRange("Shield Slam") && WoW.CanCast("Battle Cry") && !WoW.IsSpellOnCooldown("Battle Cry"))
                        {
                            WoW.CastSpell("Battle Cry");
                            return;
                        }
                        if (WoW.IsSpellInRange("Shield Slam") && WoW.CanCast("Shield Slam") && !WoW.IsSpellOnCooldown("Shield Slam") && WoW.PlayerHasBuff("Legendary"))
                        {
                            WoW.CastSpell("Shield Slam");
                            return;
                        }
                        if (WoW.IsSpellInRange("Shield Slam") && WoW.CanCast("Thunder Clap") && !WoW.IsSpellOnCooldown("Thunder Clap"))
                        {
                            WoW.CastSpell("Thunder Clap");
                            return;
                        }
                        if (WoW.CanCast("Revenge") && !WoW.IsSpellOnCooldown("Revenge") && WoW.IsSpellInRange("Shield Slam") && WoW.IsSpellOverlayed("Revenge") &&
                            !WoW.PlayerHasBuff("Vengeance: Ignore Pain"))
                        {
                            WoW.CastSpell("Revenge");
                            return;
                        }
                        if (WoW.CanCast("Revenge") && !WoW.IsSpellOnCooldown("Revenge") && WoW.IsSpellInRange("Shield Slam") && WoW.PlayerHasBuff("Vengeance: Focused Rage") && WoW.Rage > 59)
                        {
                            WoW.CastSpell("Revenge");
                            return;
                        }
                        if (WoW.CanCast("Revenge") && !WoW.IsSpellOnCooldown("Revenge") && WoW.IsSpellInRange("Shield Slam") && !WoW.PlayerHasBuff("Ignore Pain") && WoW.Rage > 35 &&
                            WoW.HealthPercent < 100)
                        {
                            WoW.CastSpell("Revenge");
                            return;
                        }
                        if (WoW.CanCast("Revenge") && !WoW.IsSpellOnCooldown("Revenge") && WoW.IsSpellInRange("Shield Slam") && WoW.PlayerHasBuff("Ignore Pain") &&
                            WoW.PlayerBuffTimeRemaining("Ignore Pain") <= 3 && WoW.Rage > 40 && WoW.HealthPercent < 100)
                        {
                            WoW.CastSpell("Revenge");
                            return;
                        }
                        if (WoW.CanCast("Revenge") && !WoW.IsSpellOnCooldown("Revenge") && WoW.IsSpellInRange("Shield Slam") && !WoW.PlayerHasBuff("Vengeance: Focused Rage") &&
                            !WoW.PlayerHasBuff("Vengeance: Ignore Pain") && WoW.Rage > 69)
                        {
                            WoW.CastSpell("Revenge");
                            return;
                        }
                        if (WoW.CanCast("Victory Rush") && !WoW.IsSpellOnCooldown("Victory Rush") && WoW.IsSpellInRange("Shield Slam") && WoW.HealthPercent < 90 &&
                            WoW.PlayerHasBuff("VictoryRush"))
                        {
                            WoW.CastSpell("Victory Rush");
                            return;
                        }
                        if (WoW.IsSpellInRange("Devastate") && WoW.CanCast("Devastate"))
                        {
                            WoW.CastSpell("Devastate");
                            return;
                        }
                    }
                    if (WoW.CanCast("Neltharion's Fury") && WoW.TargetHasDebuff("ShockWavestun"))
                    {
                        WoW.CastSpell("Neltharion's Fury");
                        return;
                    }

                    /* actions.prot=spell_reflection,if=incoming_damage_2500ms>health.max*0.20
							actions.prot+=/demoralizing_shout,if=incoming_damage_2500ms>health.max*0.20&!talent.booming_voice.enabled
							actions.prot+=/last_stand,if=incoming_damage_2500ms>health.max*0.40
							actions.prot+=/shield_wall,if=incoming_damage_2500ms>health.max*0.40&!cooldown.last_stand.remains=0
							actions.prot+=/potion,name=unbending_potion,if=(incoming_damage_2500ms>health.max*0.15&!buff.potion.up)|target.time_to_die<=25
							actions.prot+=/battle_cry,if=cooldown.shield_slam.remains=0
							actions.prot+=/demoralizing_shout,if=talent.booming_voice.enabled&buff.battle_cry.up
							actions.prot+=/ravager,if=talent.ravager.enabled&buff.battle_cry.up
							actions.prot+=/neltharions_fury,if=!buff.shield_block.up&cooldown.shield_block.remains>3&((cooldown.shield_slam.remains>3&talent.heavy_repercussions.enabled)|(!talent.heavy_repercussions.enabled))
							actions.prot+=/shield_block,if=!buff.neltharions_fury.up&((cooldown.shield_slam.remains=0&talent.heavy_repercussions.enabled)|action.shield_block.charges=2|!talent.heavy_repercussions.enabled)
							actions.prot+=/ignore_pain,if=(rage>=60&!talent.vengeance.enabled)|(buff.vengeance_ignore_pain.up&rage>=39)|(talent.vengeance.enabled&!buff.vengeance_ignore_pain.up&!buff.vengeance_revenge.up&rage<30&!buff.revenge.react)
							actions.prot+=/shield_slam,if=(!(cooldown.shield_block.remains<=gcd.max*2&!buff.shield_block.up)&talent.heavy_repercussions.enabled)|!talent.heavy_repercussions.enabled
							actions.prot+=/thunder_clap
							actions.prot+=/revenge,if=(talent.vengeance.enabled&buff.revenge.react&!buff.vengeance_ignore_pain.up)|(buff.vengeance_revenge.up&rage>=59)|(talent.vengeance.enabled&!buff.vengeance_ignore_pain.up&!buff.vengeance_revenge.up&rage>=69)|(!talent.vengeance.enabled&buff.revenge.react)
							actions.prot+=/devastate */
                }
            }
            if (combatRoutine.Type == RotationType.AOE)
            {
                // Do AOE Stuff here
            }
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=WiNiFiX
AddonName=badddger
WoWVersion=Legion - 70000
[SpellBook.db]
Spell,6343,Thunder Clap,D1
Spell,23922,Shield Slam,D2
Spell,6572,Revenge,D3
Spell,20243,Devastate,D4
Spell,34428,Victory Rush,D5
Spell,204488,Focused Rage,D6
Spell,203526,Neltharion's Fury,D7
Spell,46968,Shockwave,D8
Spell,871,Shield Wall,F5
Spell,12975,Last Stand,F6
Spell,6552,Pummel,F7
Spell,2565,Shield Block,F8
Spell,190456,Ignore Pain,F9
Spell,1719,Battle Cry,F10
Spell,6544,HeroicLeap,F11
Spell,23920,SpellReflect,F12
Aura,132168,ShockWavestun
Aura,202573,Vengeance: Focused Rage
Aura,202574,Vengeance: Ignore Pain
Aura,190456,Ignore Pain
Aura,132404,ShieldBlockAura
Aura,32216,VictoryRush
Aura,207844,Legendary
Aura,186305,Mount
*/