using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using CloudMagic.Helpers;

namespace CloudMagic.Rotation
{
    public class RogueSubtlety : CombatRoutine
    {
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

        public override void Pulse()
        {
		if (DetectKeyPress.GetKeyState(0x6A) < 0)
            {
                UseCooldowns = !UseCooldowns;
                Thread.Sleep(150);
            }			
			if (!WoW.IsInCombat && !WoW.PlayerHasBuff("Stealth") && !WoW.IsMounted && WoW.CanCast("Stealth"))
			{
				WoW.CastSpell("Stealth");
				return;
			}
			if (WoW.CanCast ("Symbols of Death") && WoW.PlayerHasBuff("Stealth") && !WoW.IsInCombat  && !WoW.IsMounted)
			{
				WoW.CastSpell("Symbols of Death");
				return;
			}
			if (WoW.CanCast("CrimsonVial") && WoW.HealthPercent <= 40 && WoW.Energy >=35)
            {
                WoW.CastSpell("CrimsonVial");
                return;
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
                    if (combatRoutine.UseCooldowns)
						{							
							if (WoW.CanCast("ShadowBlades") && WoW.PlayerHasBuff("Symbols of Death") && WoW.IsSpellInRange("NightBlade"))
							{
								WoW.CastSpell("ShadowBlades");
								return;
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
							if (WoW.CanCast("Vanish") && WoW.IsSpellInRange("NightBlade") && WoW.TargetHasDebuff("NightBlade") && (!WoW.PlayerHasBuff("Stealth") || !WoW.PlayerHasBuff("ShadowDance")))
							{
								WoW.CastSpell("Vanish");
								return;
							}							
						}																								
					if (WoW.PlayerHasBuff("Stealth") || WoW.PlayerHasBuff("Subterfuge") || WoW.PlayerHasBuff("ShadowDance"))
                    {
						if(WoW.CanCast("Symbols of Death") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.PlayerBuffTimeRemaining("Symbols of Death") <= 500)
						{
							WoW.CastSpell("Symbols of Death");
							return;
						}
						if (WoW.CanCast("ShadowStrike") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=40 && WoW.CurrentComboPoints <=5 )
						{
							WoW.CastSpell("ShadowStrike");
							return;
						}
						if (WoW.CanCast("NightBlade") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 25 &&
                            (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) && WoW.IsSpellInRange("NightBlade"))
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
                        if (WoW.CanCast("GoremawsBite") && WoW.IsSpellInRange("NightBlade") && WoW.CurrentComboPoints <=3)
						{
							WoW.CastSpell("GoremawsBite");
							return;
						}
						if (WoW.CanCast("Eviscerate") && WoW.CurrentComboPoints >=5 && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.TargetHasDebuff("NightBlade"))
						{	
							WoW.CastSpell("Eviscerate");
							return;
						}					
						
					}
				if (WoW.CanCast("ShadowDance") && (!WoW.PlayerHasBuff("Stealth") || !WoW.PlayerHasBuff("ShadowDance") || !WoW.PlayerHasBuff("Vanish") || !WoW.PlayerHasBuff("Subterfuge")) && WoW.Energy >= 55 &&
                        (WoW.PlayerSpellCharges("ShadowDance") >=2 && WoW.CurrentComboPoints <= 3 && WoW.IsSpellInRange("NightBlade")))
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}
				if (WoW.CanCast("NightBlade") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 25 &&
                            (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) && WoW.IsSpellInRange("NightBlade"))
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
				if (WoW.CanCast("GoremawsBite") && WoW.IsSpellInRange("NightBlade") && WoW.CurrentComboPoints <=3)
						{
							WoW.CastSpell("GoremawsBite");
							return;
						}
				if (WoW.CanCast("Eviscerate") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 35 && WoW.TargetHasDebuff("NightBlade") && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("Eviscerate");
							return;
						}
                if (WoW.CanCast("Backstab") && WoW.CurrentComboPoints < 6 && WoW.Energy >= 45 && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("Backstab");
							return;
						}
                }
            }

            if (combatRoutine.Type == RotationType.AOE) // Do AoE Stuff here
            {
                  if (WoW.PlayerHasBuff("Stealth") || WoW.PlayerHasBuff("Subterfuge") || WoW.PlayerHasBuff("ShadowDance"))
                    {
						if(WoW.CanCast("Symbols of Death") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.PlayerBuffTimeRemaining("Symbols of Death") <= 500)
						{
							WoW.CastSpell("Symbols of Death");
							return;
						}
						if (WoW.CanCast("ShurikenStorm") && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=40 && WoW.CurrentComboPoints <=5 )
						{
							WoW.CastSpell("ShurikenStorm");
							return;
						}
						if (WoW.CanCast("NightBlade") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 25 &&
                            (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) && WoW.IsSpellInRange("NightBlade"))
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
                        if (WoW.CanCast("GoremawsBite") && WoW.IsSpellInRange("NightBlade") && WoW.CurrentComboPoints <=3)
						{
							WoW.CastSpell("GoremawsBite");
							return;
						}
						if (WoW.CanCast("Eviscerate") && WoW.CurrentComboPoints >=5 && WoW.IsSpellInRange("NightBlade") && WoW.Energy >=35 && WoW.TargetHasDebuff("NightBlade"))
						{	
							WoW.CastSpell("Eviscerate");
							return;
						}					
						
					}
					if (WoW.CanCast("ShadowDance") && (!WoW.PlayerHasBuff("Stealth") || !WoW.PlayerHasBuff("ShadowDance") || !WoW.PlayerHasBuff("Vanish") || !WoW.PlayerHasBuff("Subterfuge")) && WoW.Energy >= 55 &&
                        (WoW.PlayerSpellCharges("ShadowDance") >=2 && WoW.CurrentComboPoints <= 3 && WoW.IsSpellInRange("NightBlade")))
						{
                        WoW.CastSpell("ShadowDance");
                        return;
						}
					if (WoW.CanCast("NightBlade") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 25 &&
                            (!WoW.TargetHasDebuff("NightBlade") || WoW.TargetDebuffTimeRemaining("NightBlade") <= 600) && WoW.IsSpellInRange("NightBlade"))
						{
                            WoW.CastSpell("NightBlade");
                            return;
						}
					if (WoW.CanCast("GoremawsBite") && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("GoremawsBite");
							return;
						}
					if (WoW.CanCast("Eviscerate") && WoW.CurrentComboPoints >= 5 && WoW.Energy >= 35 && WoW.TargetHasDebuff("NightBlade") && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("Eviscerate");
							return;
						}
					if (WoW.CanCast("ShurikenStorm") && WoW.CurrentComboPoints < 6 && WoW.Energy >= 35 && WoW.IsSpellInRange("NightBlade"))
						{
							WoW.CastSpell("ShurikenStorm");
							return;
						}
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
Aura,212283,Symbols of Death
Aura,115191,Stealth
Aura,115192,Subterfuge
Aura,185422,ShadowDance
Aura,195452,NightBlade
Aura,11327,Vanish
*/