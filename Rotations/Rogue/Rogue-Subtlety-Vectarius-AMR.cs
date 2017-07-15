using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using CloudMagic.Helpers;
using System.Threading;

namespace CloudMagic.Rotation
{
    public class RogueSubtlety : CombatRoutine
    {
        private bool BL
        {
            get
            {
				if (WoW.PlayerHasBuff("Bloodlust") || WoW.PlayerHasBuff("Ancient Hysteria") || WoW.PlayerHasBuff("Netherwinds") || WoW.PlayerHasBuff("Drums") || WoW.PlayerHasBuff("Heroism") || WoW.PlayerHasBuff("Time Warp"))
				{
					return true;
				}
				else
					return false;
            }
        }
		//variable,name=dsh_dfa,value=talent.death_from_above.enabled&talent.dark_shadow.enabled&spell_targets.death_from_above<4
        private bool dsh_dfa
        {
            get
            {
				if (WoW.Talent(7) ==3 && WoW.Talent(6)==1 )
				{
					return true;
				}
				else
					return false;
            }
        }		
		
        private bool stealth
        {
            get
            {
				if (WoW.PlayerHasBuff("Stealth") || WoW.PlayerHasBuff("ShadowDance") || WoW.PlayerHasBuff("Vanish") || WoW.PlayerHasBuff("Subterfuge") )
				{
					return true;
				}
				else
					return false;
            }
        }	
		
		private NumericUpDown nudBlurPercentValue;
		
		private static readonly Stopwatch coolDownStopWatch = new Stopwatch();

        public override string Name { get { return "Subtlety by Chriser"; } }

        public override string Class { get { return "Rogue"; } }

        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
			Log.Write("The recommended build for Sub is 1213111", Color.Red);			     			
        }

        private void CmdSave_Click(object sender, EventArgs e)
        {
        }

        public override void Stop()
        {
        }
		public override int CLEAVE { get { return 99; } } //please Set between 1-99 NpC in range for AOE  if not desired set to 99
        public override int AOE { get { return 2; } }//please Set between 1-99 NpC in range for Cleave if not desired set to 99
        public override int SINGLE {get { return 1; } }//please Set between 1-99 NpC in range for ST if not desired set to 99   
		
        public override void Pulse()
        {
		if (DetectKeyPress.GetKeyState(0x6A) < 0)
            {
                UseCooldowns = !UseCooldowns;
                Thread.Sleep(150);
            }			
			
			if (WoW.TargetIsCasting) // Kick
                {
			    if (WoW.CanCast("Kick") && WoW.IsSpellInRange("Kick") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 55 && !WoW.IsSpellOnCooldown("Kick") && !WoW.PlayerIsChanneling && !WoW.WasLastCasted("Kick"))
					{
						WoW.CastSpell("Kick");						
						return;
					}	
			    }			       
            if (combatRoutine.Type == RotationType.SingleTarget|| combatRoutine.Type == RotationType.SingleTargetCleave) // Do Single Target Stuff here
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted)
                {
// Spell ShadowDance if TimerSecRemaining(DeathFromAbove) > 0 and HasTalent(DarkShadow)					
				if (WoW.CanCast("ShadowDance")  && WoW.Talent(6)==1 && WoW.LastSpell == "DeathFromAbove" )
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}					
					// Cooldowns Spell ShadowBlades if AlternatePowerToMax >= 2
					if (combatRoutine.UseCooldowns)
					{
						if (!stealth)
						{
//StealthCooldowns Spell ShadowDance if ChargesRemaining(ShadowDance) = SpellCharges(ShadowDance)
						if (WoW.CanCast("ShadowDance") && WoW.PlayerSpellCharges("ShadowDance") >=2)
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}	
//StealthCooldowns Spell Vanish if HasTalent(Subterfuge) and not HasBuff(MasterAssassinsInitiative)
							if (WoW.CanCast("Vanish") && WoW.IsSpellInRange("NightBlade") && WoW.Talent(2)==2 && !WoW.PlayerHasBuff("MasterAssassinsInitiative") && (!WoW.PlayerHasBuff("Stealth") || !WoW.PlayerHasBuff("ShadowDance")))
							{
								WoW.CastSpell("Vanish");
								return;
							}						
						}
							if (WoW.CanCast("ShadowBlades") && WoW.IsSpellInRange("NightBlade") && WoW.CurrentComboPoints <= 4)
							{
								WoW.CastSpell("ShadowBlades");
								return;
							}	
//Cooldowns Spell GoremawsBite if AlternatePowerToMax >= 3 and not HasBuff(Stealth) and not HasBuff(ShadowDance) and not HasBuff(Subterfuge) and ChargesRemaining(ShadowDance) < SpellCharges(ShadowDance)	
						if (WoW.CanCast("GoremawsBite") && WoW.IsSpellInRange("NightBlade") && WoW.CurrentComboPoints <=3 && !stealth && WoW.PlayerSpellCharges("ShadowDance") < 2)
						{
							WoW.CastSpell("GoremawsBite");
							return;
						}
							
					}
							if (WoW.CanCast("Berserking") && !WoW.IsSpellOnCooldown ("Berserking") && WoW.PlayerRace == "Troll")
							{
								WoW.CastSpell("Berserking");
								return;
							}	
							if (WoW.CanCast("Arcane Torrent") && !WoW.IsSpellOnCooldown ("Arcane Torrent") && WoW.PlayerRace == "BloodElf" && WoW.Energy <= 20)
							{
								WoW.CastSpell("Arcane Torrent");
								return;
							}	
							if (WoW.CanCast("Blood Fury") && !WoW.IsSpellOnCooldown ("Blood Fury") && WoW.PlayerRace == "Orc")
							{
								WoW.CastSpell("Blood Fury");
								return;
							}	

// Spell Backstab Once if AlternatePower < 5 and FightDurationSec - FightSecRemaining < 2		
					if (WoW.CanCast("Backstab") && WoW.CurrentComboPoints < 6 && WoW.Energy >= 45 && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("Backstab");
							return;
						}
//Spell Vanish if not HasTalent(Subterfuge) and not HasBuff(ShadowDance) and not HasBuff(MasterAssassinsInitiative) and Power >= 40 and ChargesRemaining(ShadowDance) < SpellCharges(ShadowDance) and not IsOnGcd and (AlternatePowerToMax >= 3 or (AlternatePowerToMax >= 2 and not HasBuff(ShadowBlades)))						
							if (WoW.CanCast("Vanish") &&!stealth&& WoW.IsSpellInRange("NightBlade") && WoW.Talent(2)!=2 && !WoW.PlayerHasBuff("ShadowDance") &&!WoW.PlayerHasBuff("MasterAssassinsInitiative") && WoW.Energy >=40 && WoW.PlayerSpellCharges("ShadowDance") <2 && (WoW.CurrentComboPoints >=3 || (WoW.CurrentComboPoints >=2 && !WoW.PlayerHasBuff("ShadowBlades"))))
							{
								WoW.CastSpell("Vanish");
								return;
							}
//AmrRogueSubtletyDefault2 Spell SymbolsOfDeath if ((PowerToMax >= 40 and (not HasBuff(ShadowDance) or BuffRemainingSec(ShadowDance) >= BuffDurationSec(ShadowDance) - 1)) or 
						if(WoW.CanCast("Symbols of Death") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 &&  WoW.Energy <=60 && (WoW.PlayerHasBuff("ShadowDance") || WoW.PlayerBuffTimeRemaining("ShadowDance") >= 300))
						{
							WoW.CastSpell("Symbols of Death");
							return;
						}
//(HasBuff(ShadowDance) and FightDurationSec - FightSecRemaining < 10) or 

//(HasItem(TheFirstOfTheDead) and AlternatePowerToMax >= 5 and Power >= 40)) and not IsOnGcd		
// Finisher Spell Nightblade if CanRefreshDot(Nightblade) and TargetSecRemaining > 6 and (not HasBuff(MasterAssassinsInitiative) or DotRemainingSec(Nightblade) < BuffRemainingSec(MasterAssassinsInitiative))	
						if (WoW.CanCast("NightBlade") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 25 && (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) &&((!WoW.TargetHasDebuff("MasterAssassinsInitiative")) || (WoW.TargetDebuffTimeRemaining("Nightblade") < WoW.PlayerBuffTimeRemaining("MasterAssassinsInitiative"))) && WoW.IsSpellInRange("NightBlade"))
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
 //Finisher Spell DeathFromAbove if HasBuff(SymbolsOfDeath) or CooldownSecRemaining(SymbolsOfDeath) > 10	
						if (WoW.CanCast("DeathFromAbove") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 25 && (WoW.PlayerHasBuff("SymbolsOfDeath") || WoW.SpellCooldownTimeRemaining("SymbolsOfDeath") > 10000) && WoW.IsSpellInRange("NightBlade"))
						{
                            WoW.CastSpell("DeathFromAbove");
                            return;
						} 
//Finisher Spell Eviscerate
						if (WoW.CanCast("Eviscerate") && WoW.CurrentComboPoints >=5 && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.TargetHasDebuff("NightBlade"))
						{	
							WoW.CastSpell("Eviscerate");
							return;
						}
//InsideStealth Spell Shadowstrike
						if (WoW.CanCast("ShadowStrike") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=40 && WoW.CurrentComboPoints <=5 && stealth )
						{
							WoW.CastSpell("ShadowStrike");
							return;
						}	
// Spell ShadowDance if TimerSecRemaining(DeathFromAbove) > 0 and HasTalent(DarkShadow)
				if (WoW.CanCast("ShadowDance")  && WoW.Talent(6)==1 && WoW.LastSpell == "DeathFromAbove" )
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}	
//StealthCooldowns Spell ShadowDance
				if (WoW.CanCast("ShadowDance") )
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}
//Builder Spell Backstab	
                if (WoW.CanCast("Backstab") && WoW.CurrentComboPoints < 6 && WoW.Energy >= 45 && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("Backstab");
							return;
						}					
				}
            }

            if (combatRoutine.Type == RotationType.AOE) // Do AoE Stuff here
            {
                 
            }
        }
    }
}


/*
[AddonDetails.db]
AddonAuthor=Chriser
AddonName=Recount
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,152150,DeathFromAbove,D0
Spell,185311,CrimsonVial,F3
Spell,212283,Symbols of Death,D2
Spell,185438,ShadowStrike,D1
Spell,185313,ShadowDance,H
Spell,195452,NightBlade,D3
Spell,196819,Eviscerate,D4
Spell,209782,GoremawsBite,T
Spell,1856,Vanish,O
Spell,121471,ShadowBlades,G
Spell,1766,Kick,D6
Spell,53,Backstab,D1
Spell,197835,ShurikenStorm,D8
Spell,115191,Stealth,R
Spell,26297,Berserking,F2
Spell,202719,Arcane Torrent,F2
Spell,20572,Blood Fury,F2
Aura,212283,SymbolsOfDeath
Aura,115191,Stealth
Aura,115192,Subterfuge
Aura,185422,ShadowDance
Aura,195452,NightBlade
Aura,11327,Vanish
*/