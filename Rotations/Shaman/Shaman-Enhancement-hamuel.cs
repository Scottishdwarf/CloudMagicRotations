using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Text;
using System.Threading;
using CloudMagic.Helpers;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace CloudMagic.Rotation
{//Data tables
    public class Enhancement : CombatRoutine
    {
        public override string Name
        {
            get { return "Enhancement Hamuel:SimC"; }
        }
        public override string Class
        {
            get { return "Shaman"; }
        }
        public override Form SettingsForm { get; set; }
        public override int CLEAVE { get { return 2; } } //please Set between 1-99 if not desired set to 99
        public override int AOE { get { return 3; } }//please Set between 1-99 if not desired set to 99
        public override int SINGLE { get { return 1; } }//please Set between 1-99 if not desired set to 99
        private static readonly Random getrandom = new Random();
        private static int Revision = 1;
        public Stopwatch Crash = new Stopwatch();
        public Stopwatch Pets = new Stopwatch();
        private static int interMin = 50;
        private static int interMax = 90;
        public Stopwatch Rotation = new Stopwatch();
        public override void Initialize()
        {
            Log.Write("Welcome to Enhancement Shaman by Hamuel", Color.Green);
            Log.Write("version " + Revision, Color.Green);
        }
        //actions+=/variable,name=hailstormCheck,value=((talent.hailstorm.enabled&!buff.frostbrand.up)|!talent.hailstorm.enabled)
        private bool hailstormCheck
        {
            get
            {
                if (WoW.Talent(4) == 3 && !WoW.PlayerHasBuff("Frostbrand") || WoW.Talent(4) != 3)
                {
                    return true;
                }
                return false;
            }
        }
        //actions+=/variable,name=furyCheck80,value=(!talent.fury_of_air.enabled|(talent.fury_of_air.enabled&maelstrom>80))
        private bool furyCheck80
        {
            get
            {
                if (WoW.Talent(6) != 2 || WoW.Talent(6) == 2 && WoW.Maelstrom > 80)
                {
                    return true;
                }
                return false;
            }
        }
        //actions+=/variable,name=furyCheck70,value=(!talent.fury_of_air.enabled|(talent.fury_of_air.enabled&maelstrom>70))
        private bool furyCheck70
        {
            get
            {
                if (WoW.Talent(6) != 2 || WoW.Talent(6) == 2 && WoW.Maelstrom > 70)
                {
                    return true;
                }
                return false;
            }
        }

        //actions+=/variable,name=furyCheck45,value=(!talent.fury_of_air.enabled|(talent.fury_of_air.enabled&maelstrom>45))
        private bool furyCheck45
        {
            get
            {
                if (WoW.Talent(6) != 2 || WoW.Talent(6) == 2 && WoW.Maelstrom > 45)
                {
                    return true;
                }
                return false;
            }
        }

        //actions+=/variable,name=furyCheck25,value=(!talent.fury_of_air.enabled|(talent.fury_of_air.enabled&maelstrom>25))
        private bool furyCheck25
        {
            get
            {
                if (WoW.Talent(6) != 2 || WoW.Talent(6) == 2 && WoW.Maelstrom >= 25)
                {
                    return true;
                }
                return false;
            }
        }

        //actions+=/variable,name=OCPool70,value=(!talent.overcharge.enabled|(talent.overcharge.enabled&maelstrom>70))
        private bool OCPool70
        {
            get
            {
                if (WoW.Talent(5) != 2 || WoW.Talent(5) == 2 && WoW.Maelstrom > 70)
                {
                    return true;
                }
                return false;
            }

        }
        //actions+=/variable,name=OCPool60,value=(!talent.overcharge.enabled|(talent.overcharge.enabled&maelstrom>60))
        private bool OCPool60
        {
            get
            {
                if (WoW.Talent(5) != 2 || WoW.Talent(5) == 2 && WoW.Maelstrom > 60)
                {
                    return true;
                }
                return false;
            }

        }
        //actions+=/variable,name=heartEquipped,value=(equipped.151819)

        //actions+=/variable,name=akainuEquipped,value=(equipped.137084)

        private static bool akainuEquip
        {
            get
            {
                if (WoW.Legendary(1) == 9 || WoW.Legendary(2) == 9)
                {
                    return true;
                }
                return false;
            }

        }
        //actions+=/variable,name=akainuAS,value=(variable.akainuEquipped&buff.hot_hand.react&!buff.frostbrand.up)
        private bool akainus
        {
            get
            {
                if (akainuEquip && WoW.PlayerHasBuff("Hot Hands") && !WoW.PlayerHasBuff("Frostbrand"))
                {
                    return true;
                }
                return false;
            }
        }
        //actions+=/variable,name=LightningCrashNotUp,value=(!buff.lightning_crash.up&set_bonus.tier20_2pc)
        private bool LightningCrashNotUp
        {
            get
            {
                if (!WoW.PlayerHasBuff("Lightning crash") && WoW.SetBonus(20) == 2)
                {
                    return true;
                }
                return false;
            }
        }
        //actions+=/variable,name=alphaWolfCheck,value=((pet.frost_wolf.buff.alpha_wolf.remains<2&pet.fiery_wolf.buff.alpha_wolf.remains<2&pet.lightning_wolf.buff.alpha_wolf.remains<2)&feral_spirit.remains>4)

        private bool alphaWolfCheck
        {

            get
            {
                if (Pets.IsRunning && Crash.IsRunning && Crash.ElapsedMilliseconds < 8000)
                {
                    return true;
                }
                return false;
            }
        }
        public void EnhancementCD()
        {
            if (UseCooldowns)
            {

                //actions.CDs = bloodlust,if= target.health.pct < 25 | time > 0.500

                //actions.CDs +=/ berserking,if= buff.ascendance.up | (feral_spirit.remains > 5)
                if (WoW.PlayerRace == "Troll" && WoW.CanCast("Berserking") && !WoW.IsSpellOnCooldown("Berserking") && ((WoW.Talent(7) != 1 || WoW.PlayerHasBuff("Ascendance")) || Pets.ElapsedMilliseconds < 10000))
                {
                    WoW.CastSpell("Berserking");
                    return;
                }
                //actions.CDs +=/ blood_fury,if= buff.ascendance.up | (feral_spirit.remains > 5) | level < 100
                if (WoW.PlayerRace == "Orc" && WoW.CanCast("Blood Fury")
                && ((WoW.Talent(7) != 1 || WoW.PlayerHasBuff("Ascendance")) || Pets.ElapsedMilliseconds < 10000))
                {
                    WoW.CastSpell("Blood Fury");
                    return;
                }
                //actions.CDs +=/ feral_spirit
                if (WoW.CanCast("Feral Spirit", true, true, false, false, true) && WoW.IsSpellInRange("Rockbiter") && WoW.Maelstrom >= 20 && (WoW.CanCast("Crash lightning", true, true, false, false, true)  || WoW.SpellCooldownTimeRemaining("Crash lightning") < GCD)) //feral spirit on boss - normally cast manually
                {
                    Pets.Start();
                    WoW.CastSpell("Feral Spirit");
                    return;
                }
                //actions.CDs +=/ potion,if= buff.ascendance.up | !talent.ascendance.enabled & feral_spirit.remains > 5 | target.time_to_die <= 60

                //actions.CDs +=/ doom_winds,if= debuff.earthen_spike.up & talent.earthen_spike.enabled | !talent.earthen_spike.enabled
                if (WoW.CanCast("Doom Winds") && WoW.IsSpellInRange("Rockbiter") && (WoW.Talent(7) == 3 && WoW.TargetHasDebuff("Earthen spike") || WoW.Talent(7) != 3))
                {
                    WoW.CastSpell("Doom Winds");
                    return;
                }
                //actions.CDs +=/ ascendance,if= buff.doom_winds.up
                if (WoW.CanCast("Ascendance") && WoW.PlayerHasBuff("Doom Winds"))
                {
                    WoW.CastSpell("Ascendance");
                    return;
                }
            }
        }
        private float GCD
        {
            get
            {
                if (Convert.ToSingle(150 / (1 + (WoW.HastePercent / 100f))) > 75f)
                {
                    return Convert.ToSingle(150f / (1 + (WoW.HastePercent / 100f)));
                }
                else
                {
                    return 75f;
                }
            }
        }
        private void EnhancementBuffs()
        {
            //actions.buffs = rockbiter,if= talent.landslide.enabled & !buff.landslide.up
            if(WoW.CanCast("Rockbiter", true, true, true) &&WoW.Talent(1) == 3 && !WoW.PlayerHasBuff("Landslide"))
            {
                WoW.CastSpell("Rockbiter", "Buff spell");
                return;
            }
            //actions.buffs +=/ fury_of_air,if= buff.ascendance.up | (feral_spirit.remains > 5) | level < 100
            if (WoW.CanCast("FoA") && WoW.Maelstrom >= 5 && WoW.Talent(6) == 2 && !WoW.PlayerHasBuff("FoA")&&(WoW.PlayerHasBuff("Ascendance") && Pets.IsRunning))
            {
                WoW.CastSpell("FoA", "Buff spell");
                return;
            }
            //actions.buffs +=/ crash_lightning,if= artifact.alpha_wolf.rank & prev_gcd.1.feral_spirit
            if (WoW.CanCast("Crash lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Rockbiter") && Pets.IsRunning && !Crash.IsRunning)
            {
                Crash.Restart();
                WoW.CastSpell("Crash lightning", "Buff spell");
                return;
            }
            //actions.buffs +=/ flametongue,if= !buff.flametongue.up
            if (!WoW.PlayerHasBuff("Flametongue") && WoW.CanCast("Flametongue", true, true, true))
            {
                WoW.CastSpell("Flametongue", "Buff spell");
                return;
            }
            //actions.buffs +=/ frostbrand,if= talent.hailstorm.enabled & !buff.frostbrand.up & variable.furyCheck45
            if(WoW.CanCast("Frostbrand", true, true, true) && WoW.Maelstrom >= 20 && WoW.Talent(4) == 3 && !WoW.PlayerHasBuff("Frostbrand")&& furyCheck45)
            {
                WoW.CastSpell("Frostbrand", "Buff spell");
                return;
            }
            //actions.buffs +=/ flametongue,if= buff.flametongue.remains < 6 + gcd & cooldown.doom_winds.remains < gcd * 2
            if (WoW.CanCast("Flametongue", true, true, true) &&WoW.PlayerBuffTimeRemaining("Flametongue")< 600+GCD && WoW.SpellCooldownTimeRemaining("Doom Winds") < GCD *2)
            {
                WoW.CastSpell("Flametongue", "Buff spell");
                return;
            }
            //actions.buffs +=/ frostbrand,if= talent.hailstorm.enabled & buff.frostbrand.remains < 6 + gcd & cooldown.doom_winds.remains < gcd * 2
            if (WoW.CanCast("Frostbrand", true, true, true) && WoW.Maelstrom >= 20 && WoW.PlayerBuffTimeRemaining("Frostbrand") < 600 + GCD && WoW.SpellCooldownTimeRemaining("Doom Winds") < GCD * 2)
            {
                WoW.CastSpell("Frostbrand", "Buff spell");
                return;
            }
        }
        private void EnhancementCore()
        {

            if (WoW.CanCast("lava lash", true, true, true, false, true) && WoW.Maelstrom >= 40 && WoW.TargetDebuffStacks("Legionfall") > 90)
            {
                Log.Write("Maelstrom overflow protection", Color.Blue);
                WoW.CastSpell("lava lash");
                return;
            }
            //actions.core = earthen_spike,if= variable.furyCheck25
            if (WoW.CanCast("Earthen spike") && WoW.Maelstrom >= 20 && WoW.Talent(7)== 3 && furyCheck25)
            {
                WoW.CastSpell("Earthen spike", "Core spell");
                return;
            }
            //actions.core +=/ crash_lightning,if= !buff.crash_lightning.up & active_enemies >= 2
            if(WoW.CanCast("Crash lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Rockbiter")&& !WoW.PlayerHasBuff("Crash lightning") && combatRoutine.Type != RotationType.SingleTarget)
            {
                WoW.CastSpell("Crash lightning", "Core spell");
                return;
            }
            //actions.core +=/ windsong
            if(WoW.CanCast("Windsong", true, true, true) && WoW.Talent(1)== 1)
            {
                WoW.CastSpell("Windsong", "Core spell");
                return;
            }
            //actions.core +=/ crash_lightning,if= active_enemies >= 8 | (active_enemies >= 6 & talent.crashing_storm.enabled)
            if(WoW.CanCast("Crash lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Rockbiter") && combatRoutine.Type == RotationType.AOE && (WoW.CountEnemyNPCsInRange >= 8 || WoW.CountEnemyNPCsInRange >=6 && WoW.Talent(6) ==1))
            {
                WoW.CastSpell("Crash lightning", "Core spell");
                return;

            }
            //actions.core +=/ windstrike
            if(WoW.CanCast("Stormstrike", true, true, true) && WoW.Maelstrom >= 8 && WoW.PlayerHasBuff("Ascendance"))
            {
                WoW.CastSpell("Stormstrike", "Core spell");
                return;
            }
            //actions.core +=/ Stormstrike,if= buff.stormbringer.up & variable.furyCheck25
            if (WoW.CanCast("Stormstrike", true, true, true) && WoW.Maelstrom >= 20 && WoW.PlayerHasBuff("Stormbringer") && furyCheck25)
            {
                WoW.CastSpell("Stormstrike", "Core: Stormbriner react spell");
                return;
            }
            //actions.core +=/ crash_lightning,if= active_enemies >= 4 | (active_enemies >= 2 & talent.crashing_storm.enabled)
            if(WoW.CanCast("Crash lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Rockbiter") && combatRoutine.Type != RotationType.SingleTarget && (WoW.CountEnemyNPCsInRange >=4 || WoW.CountEnemyNPCsInRange>2 && WoW.Talent(6)==1 ))
            {
                WoW.CastSpell("Crash lightning", "Core: Aoe with talent spell");
                return;
            }
            //actions.core +=/ lightning_bolt,if= talent.overcharge.enabled & variable.furyCheck45 & maelstrom >= 40
            if (WoW.CanCast("Lightning bolt") && WoW.Talent(5) == 2 && furyCheck45 && WoW.Maelstrom >= 40)
            {
                WoW.CastSpell("Lightning bolt", "Core spell");
                return;
            }

            //actions.core +=/ Stormstrike,if= (!talent.overcharge.enabled & variable.furyCheck45) | (talent.overcharge.enabled & variable.furyCheck80)
            if (WoW.CanCast("Stormstrike",true,true,true) && WoW.Maelstrom > 40 && (WoW.Talent(5) != 2 && furyCheck45|| WoW.Talent(5)==2 &&furyCheck80))
            {
                WoW.CastSpell("Stormstrike", "Core spell");
                return;
            }
            //actions.core +=/ frostbrand,if= variable.akainuAS
            if (WoW.CanCast("Frostbrand", true, true, true) && WoW.Maelstrom >= 20 && akainus)
            {
                WoW.CastSpell("Frostbrand","Core, With Akainus spell");
                return;
            }
            
            //actions.core +=/ lava_lash,if= buff.hot_hand.react & ((variable.akainuEquipped & buff.frostbrand.up) | !variable.akainuEquipped)
            if (WoW.CanCast("lava lash",true,true,true,false,true) && WoW.PlayerHasBuff("Hot Hands") && (akainuEquip && WoW.PlayerHasBuff("Frostbrand") ||!akainuEquip))
            {
               WoW.CastSpell("lava lash", "Core, Hot Hand re-react spell");
               return;
            }
            //actions.core +=/ sundering,if= active_enemies >= 3
            if(WoW.CanCast("Sundering") && WoW.Maelstrom >= 20 && WoW.Talent(6) ==3 && combatRoutine.Type == RotationType.AOE)
            {
                WoW.CastSpell("Sundering", "Core: Aoe spell");
                return;
            }
            //actions.core +=/ crash_lightning,if= active_enemies >= 3 | variable.LightningCrashNotUp | variable.alphaWolfCheck
            if(WoW.CanCast("Crash lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Rockbiter") && (combatRoutine.Type == RotationType.AOE || LightningCrashNotUp || alphaWolfCheck))
            {
                Crash.Restart();
                WoW.CastSpell("Crash lightning", "Core: Wolves or t20 crit buff");
                return;
            }
        }
        private void EnhancementFiller()
        {
            //actions.filler = rockbiter,if= maelstrom < 120
            if(WoW.CanCast("Rockbiter", true, true, true) && WoW.Maelstrom <120)
            {
                WoW.CastSpell("Rockbiter", "Filler spell");
                return;
            }
            //actions.filler +=/ flametongue,if= buff.flametongue.remains < 4.8
            if(WoW.CanCast("Flametongue", true, true, true) && (!WoW.PlayerHasBuff("Flametongue")||WoW.PlayerBuffTimeRemaining("Flametongue")<480))
            {
                WoW.CastSpell("Flametongue", "Filler: Refresh buff spell");
                return;
            }
            //actions.filler +=/ rockbiter,if= maelstrom <= 40
            if (WoW.CanCast("Rockbiter", true, true, true) && WoW.Maelstrom < 40)
            {
                WoW.CastSpell("Rockbiter", "Filler spell");
                return;
            }
            //actions.filler +=/ crash_lightning,if= (talent.crashing_storm.enabled | active_enemies >= 2) & debuff.earthen_spike.up & maelstrom >= 40 & variable.OCPool60
            if (WoW.CanCast("Crash lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Rockbiter") && (WoW.Talent(6) == 1 && combatRoutine.Type != RotationType.SingleTarget) && WoW.TargetHasDebuff("Earthen spike")&&WoW.Maelstrom >= 40 && OCPool60)
            {
                Crash.Restart();
                WoW.CastSpell("Crash lightning", "Filler spell");
                return;
            }
            //actions.filler +=/ frostbrand,if= talent.hailstorm.enabled & buff.frostbrand.remains < 4.8 & maelstrom > 40
            if (WoW.CanCast("Frostbrand", true, true, true) && WoW.Maelstrom >= 20 && hailstormCheck && (!WoW.PlayerHasBuff("Frostbrand") || WoW.PlayerBuffTimeRemaining("Frostbrand") < 480 && WoW.Maelstrom >= 40))
            {
                WoW.CastSpell("Frostbrand", "Hailstorm check");
                return;
            }

            //actions.filler +=/ frostbrand,if= variable.akainuEquipped & !buff.frostbrand.up & maelstrom >= 75
            if (WoW.CanCast("Frostbrand", true, true, true) && WoW.Maelstrom >= 20 && akainuEquip && (!WoW.PlayerHasBuff("Frostbrand") && WoW.Maelstrom >= 75))
            {
                WoW.CastSpell("Frostbrand", "Filler spell");
                return;
            }

            //actions.filler +=/ sundering
            if (WoW.CanCast("Sundering") && WoW.Maelstrom >= 20 && WoW.Talent(6) == 3)
            {
                WoW.CastSpell("Sundering", "Filler spell");
                return;
            }
            //actions.filler +=/ lava_lash,if= maelstrom >= 50 & variable.OCPool70 & variable.furyCheck80
            if (WoW.CanCast("lava lash", true, true, true) && WoW.Maelstrom >50 && OCPool70 && furyCheck80)
            {
                WoW.CastSpell("lava lash", "Filler spell");
                return;
            }
            //actions.filler +=/ rockbiter
            if (WoW.CanCast("Rockbiter", true, true, true))
            {
                WoW.CastSpell("Rockbiter", "Filler spell");
                return;
            }

            //actions.filler +=/ crash_lightning,if= (maelstrom >= 65 | talent.crashing_storm.enabled | active_enemies >= 2) & variable.OCPool60 & variable.furyCheck45
            if (WoW.CanCast("Crash lightning") && WoW.Maelstrom >= 20 && WoW.IsSpellInRange("Rockbiter") && (WoW.Maelstrom > 65 | WoW.Talent(6) == 1 && combatRoutine.Type != RotationType.SingleTarget) && OCPool60 && furyCheck45)
            {
                Crash.Restart();
                WoW.CastSpell("Crash lightning", "Filler spell");
                return;
            }
            //actions.filler +=/ flametongue
            if (WoW.CanCast("Flametongue", true, true, true))
            {
                WoW.CastSpell("Flametongue", "Filler spell");
                return;
            }
        }
        private void TimerReset()
        {
            if(Crash.Elapsed.Seconds >=8)
                Crash.Reset();
            if (Pets.Elapsed.Seconds >= 15)
                Pets.Reset();
        }
    
        public override void Pulse()
        {
            TimerReset();
            if (WoW.IsInCombat && !WoW.IsMounted)
            {
                SelectRotation(3, 2, 1);
                interruptcast();
                //Stuns();
                Defensive();

                //actions +=/ call_action_list,name = buffs
                EnhancementBuffs();
                //actions +=/ call_action_list,name = CDs
                EnhancementCD();
                //actions +=/ call_action_list,name = core
                EnhancementCore();
                //actions +=/ call_action_list,name = filler
                EnhancementFiller();
            }
        }
        private void Defensive()
        {
            if (WoW.Talent(2) == 1 && WoW.CanCast("Rainfall") && !WoW.PlayerHasBuff("Rainfall") && !WoW.IsSpellOnCooldown("Rainfall")) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
            {
                WoW.CastSpell("Rainfall");
                return;
            }
            /* if (CharInfo.Mana > 21 && WoW.Maelstrom > 20 && WoW.CanCast("Healing Surge") && WoW.HealthPercent < EnhLowHp && !WoW.IsSpellOnCooldown("Healing Surge")) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
             {
                 WoW.CastSpell("Healing Surge");
                 return;
             }*/
            if (WoW.CanCast("Astral Shift") && WoW.HealthPercent < 60 && !WoW.IsSpellOnCooldown("Astral Shift")) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
            {
                WoW.CastSpell("Astral Shift");
                return;
            }
            if (WoW.PlayerRace == "Dreanei" && WoW.HealthPercent < 80 && !WoW.IsSpellOnCooldown("Gift Naaru"))
            {
                WoW.CastSpell("Gift Naaru");
            }
        }
        private void interruptcast()
        {
            Random random = new Random();
            int randomNumber = random.Next(interMin, interMax);

            if (WoW.TargetPercentCast > randomNumber && WoW.TargetIsCastingAndSpellIsInterruptible)
            {

                if (WoW.CanCast("Wind Shear") && !WoW.IsSpellOnCooldown("Wind Shear") && WoW.TargetIsCasting && WoW.IsSpellInRange("Wind Shear")) //interupt every spell, not a boss.
                {
                    WoW.CastSpell("Wind Shear");
                    return;
                }
                if (WoW.PlayerRace == "BloodElf" && WoW.CanCast("Arcane Torrent", true, true, false, false, true) && !WoW.IsSpellOnCooldown("Wind Shear") && WoW.IsSpellInRange("Stormstrike")) //interupt every spell, not a boss.
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
            /*  for (int i = 1; i < 5; i++)
                  if (WoW.BossPercentCast(i) > randomNumber && WoW.BossIsCastingAndSpellIsInterruptible(i))
                  {

                      if (WoW.CanCast("Wind Shear")) //interupt every spell, not a boss.
                      {
                          WoW.CastSpell("Wind Shear");
                          return;
                      }
                  }*/
        }
        public override void Stop()
        {
        }
    }
}
/*
[AddonDetails.db]
AddonAuthor=Hamuel
AddonName=Hamuel
WoWVersion=Legion - 72500
[SpellBook.db]
Spell,57994,Wind Shear,NumPad1
Spell,196884,Feral Lunge,F9
Spell,51533,Feral Spirit,NumPad0
Spell,196834,Frostbrand,NumPad2
Spell,204945,Doom Winds,F1
Spell,187874,Crash lightning,NumPad4
Spell,193796,Flametongue,NumPad3
Spell,108271,Astral Shift,F2
Spell,193786,Rockbiter,NumPad5
Spell,60103,lava lash,NumPad6
Spell,17364,Stormstrike,NumPad7
Spell,187837,Lightning bolt,NumPad8
Spell,188070,Healing Surge,NumPad9
Spell,215864,Rainfall,F8
Spell,188089,Earthen spike,F4
Spell,201898,Windsong,F5
Spell,197217,Sundering,F6
Spell,114051,Ascendance,Add
Spell,197211,FoA,Subtract
Spell,59544,Gift Naaru,F10
Spell,192058,Lightning Surge,F7
Spell,26297,Berserking,F10
Spell,33697,Blood Fury,F10
Spell,20549,War Stomp,F10
Spell,155145,Arcane Torrent,F10
Spell,107079,Quaking palm,F10
Spell,142117,Prolonged Power,F11
Spell,2645,Ghost Wolf,E
Spell,3,raid3,U
Spell,2,raid2,Y
Spell,1,raid1,T
Spell,4,raid4,I
Spell,142173,Collapsing Futures,F12
Aura,194084,Flametongue
Aura,196834,Frostbrand
Aura,187878,Crashing Storm
Aura,187874,Crash lightning
Aura,201846,Stormbringer
Aura,202004,Landslide
Aura,204945,Doom Winds
Aura,215864,Rainfall
Aura,114051,Ascendance
Aura,201898,Windsong
Aura,201900,Hot Hands
Aura,197211,FoA
Aura,2645,Ghost Wolf
Aura,240842,Legionfall
Aura,234143,Temptation
Aura,242283,Lightning crash
Aura,188089,Earthen spike
Item,142117,Prolonged Power
Item,142173,Collapsing Futures
*/

