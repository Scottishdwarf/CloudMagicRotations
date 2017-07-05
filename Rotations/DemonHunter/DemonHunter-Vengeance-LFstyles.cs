// winifix@gmail.com
// ReSharper disable UnusedMember.Global

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CloudMagic.GUI;
using CloudMagic.Helpers;

namespace CloudMagic.Rotation
{
    public class DemonHunterVeng : CombatRoutine
    {
        private readonly Stopwatch interruptwatch = new Stopwatch();
		
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
            Log.WriteCloudMagic("Welcome to LFstyles Vengeance Demon Hunter ", Color.Black);
            Log.Write("Suggested build: 1222331", Color.Green);			
        }

        public override void Stop()
        {
        }

        public override void Pulse()
        {		
			if (WoW.IsMounted) return;
		
            if (WoW.IsInCombat && interruptwatch.ElapsedMilliseconds == 0)
            {
				Log.Write("Starting interrupt timer", Color.Blue);
                interruptwatch.Start();
            }

            if (UseCooldowns)
            {
            }
            
            //if (WoW.PlayerHasBuff("Mount")) return;

            if (combatRoutine.Type != RotationType.SingleTarget && combatRoutine.Type != RotationType.AOE) return;

            if (!WoW.HasTarget || !WoW.TargetIsEnemy) return;
			
			if (WoW.CanCast("Auto Attack") && !WoW.AutoAtacking)
				{
					WoW.CastSpell("Auto Attack");
				}
			if (WoW.HealthPercent < 50 && !WoW.IsSpellOnCooldown("Metamorphosis"))
            {
                Log.Write("Health low < 50% using CDs...", Color.Red);
                WoW.CastSpell("Metamorphosis"); // Off the GCD no return needed
            }
            if (WoW.CanCast("Throw Glaive") && !WoW.IsSpellOnCooldown("Throw Glaive") && !WoW.IsSpellInRange("Soul Carver"))
		    {
				WoW.CastSpell("Throw Glaive");
                return; 
            }
			if (WoW.CanCast("Spirit Bomb") && WoW.IsSpellInRange("Spirit Bomb") && !WoW.IsSpellOnCooldown("Spirit Bomb") && WoW.PlayerHasBuff("Soul Fragments") && !WoW.TargetHasDebuff("Frailty"))
            {
                WoW.CastSpell("Spirit Bomb");
                return;
            }
			// if (WoW.CanCast("Throw Glaive") && !WoW.IsSpellOnCooldown("Throw Glaive") && !WoW.IsSpellInRange("Soul Carver") && !WoW.TargetCastingSpellID("233441"))
		    // {
				// WoW.CastSpell("Throw Glaive");
                // return; 
            // }
			if (WoW.CanCast("Fiery Brand") && !WoW.TargetHasDebuff("Fiery Demise"))
            {
                WoW.CastSpell("Fiery Brand");
            }
			if (WoW.CanCast("Sever") && WoW.IsSpellInRange("Soul Carver") && WoW.PlayerHasBuff("Metamorphosis")) // Pain Generator
            {
                WoW.CastSpell("Sever");
                return;
            }
			if (WoW.CanCast("Immolation Aura") && WoW.IsSpellInRange("Soul Carver"))  
            {
                WoW.CastSpell("Immolation Aura");
                return;
            }
           if (!WoW.PlayerIsCasting && !WoW.PlayerIsChanneling && WoW.ItemCount("Healthstone") >= 1 && !WoW.ItemOnCooldown("Healthstone") && WoW.HealthPercent < 40)
                {
                    WoW.CastSpell("HealthstoneKeybind");
                    return;
                }
			// if (WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 10 && (WoW.TargetCastingSpellID == 234423 || WoW.TargetCastingSpellID == 2234678) && WoW.CanCast("Consume Magic") && WoW.IsSpellInRange("Consume Magic")) //interupt every spell with all we got
                    // {
                        // WoW.CastSpell("Consume Magic");
                    // }
                    
                    // if (WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 60 && (WoW.TargetCastingSpellID == 24079 || WoW.TargetCastingSpellID == 0) && WoW.CanCast("Sigil of Silence") && WoW.IsSpellInRange("Soul Carver"))
                    // {
                        // WoW.CastSpell("Sigil of Silence");
                        // return;
                    // }
                    
                    // if (WoW.SpellCooldownTimeRemaining("Sigil of Silence") <= 45 && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.TargetPercentCast >= 60 && (WoW.TargetCastingSpellID == 24079 || WoW.TargetCastingSpellID == 0) && WoW.CanCast("Sigil of Misery") && WoW.IsSpellInRange("Soul Carver"))
                    // {
                        // WoW.CastSpell("Sigil of Misery");
                        // return;
                    // }
            if (WoW.CanCast("Soul Carver") && WoW.IsSpellInRange("Soul Carver") && WoW.SpellCooldownTimeRemaining("Fiery Brand") >= 5)
            {
                WoW.CastSpell("Soul Carver");
                return;
            }
			if (WoW.IsSpellInRange("Soul Carver"))
				{
				if (WoW.CanCast("Soul Cleave") && ((WoW.Pain >= 25 && WoW.HealthPercent < 25) || (WoW.Pain >= 80 && WoW.HealthPercent <= 50 && !WoW.CanCast("Demon Spikes"))
						|| (WoW.Pain >= 50 && WoW.HealthPercent < 50 && WoW.PlayerHasBuff("Demon Spikes"))))
					{
						WoW.CastSpell("Soul Cleave");
						return;
					}
				}
			if (WoW.PlayerHasBuff("Metamorphosis") && !WoW.IsSpellOnCooldown("Spirit Bomb") && WoW.PlayerHasBuff("Soul Fragments") && !WoW.TargetHasDebuff("Frailty"))
			{
				WoW.CastSpell("Spirit Bomb");
				return;
			}
            
			// if (!WoW.IsSpellInRange("Soul Carver")) // If we are out of melee range return
                // return;
            // if (WoW.TargetIsCasting && interruptwatch.ElapsedMilliseconds > 1200)            {                if (!WoW.IsSpellOnCooldown("Sigil of Silence") && WoW.WasLastCasted("Arcane Torrent"))                {                    WoW.CastSpell("Sigil of Silence");                    interruptwatch.Reset();                    interruptwatch.Start();
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
            // if (WoW.CanCast("Fiery Brand") && !WoW.TargetHasDebuff("Fiery Demise") && WoW.PlayerHasBuff("Spirit of the Darkness Flame") && (WoW.PlayerBuffStacks("Spirit of the Darkness Flame") >= 7))
            // {
                // WoW.CastSpell("Fiery Brand");
            // }
			
            if (WoW.CanCast("Demon Spikes") && !WoW.PlayerHasBuff("Demon Spikes") && (WoW.Pain >= 90 || WoW.HealthPercent < 90 && WoW.Pain >= 20)) // to not waste cd and pain
            {
                WoW.CastSpell("Demon Spikes");
            }
 
            // if (WoW.CanCast("Fel Devastation") && WoW.IsSpellInRange("Soul Carver") && WoW.Pain >= 30)
            // {
                // WoW.CastSpell("Fel Devastation");
            // }
            if (WoW.CanCast("Soul Cleave") && WoW.PlayerHasBuff("Soul Fragments") && WoW.Pain >= 100) 
            {
                WoW.CastSpell("Soul Cleave");
                return;
            }
			
			// if (WoW.CanCast("Demon Spikes") && !WoW.PlayerHasBuff("Demon Spikes") && (WoW.Pain == 100 || WoW.HealthPercent < 80 && WoW.Pain >= 20 )) // to not waste cd and pain
                    // {
                        // WoW.CastSpell("Demon Spikes");
                    // }
					
					// if (WoW.CanCast("Soul Cleave") && WoW.Pain >= 30 && WoW.HealthPercent < 25) // Extra save, when we are on too low hp
                    // {
                        // WoW.CastSpell("Soul Cleave");
                        // return;
                    // }
					
					// if (WoW.CanCast("Soul Cleave") && WoW.Pain >= 80 && WoW.HealthPercent < 50 && !WoW.CanCast("Demon Spikes"))
                    // {
                        // WoW.CastSpell("Soul Cleave");
                        // return;
                    // }
					
					// if (WoW.CanCast("Soul Cleave") && WoW.Pain >= 60 && WoW.HealthPercent < 50 && WoW.PlayerHasBuff("Demon Spikes"))
                    // {
                        // WoW.CastSpell("Soul Cleave");
                        // return;
                    // }
            
			if (WoW.CanCast("Immolation Aura") && WoW.IsSpellInRange("Soul Carver"))  
            {
                WoW.CastSpell("Immolation Aura");
                return;
            }
											
			if (WoW.CanCast("Fracture") && WoW.IsSpellInRange("Soul Carver") && ((WoW.Pain >= 60 && WoW.HealthPercent >= 50 && WoW.PlayerHasBuff("Demon Spikes")) || (WoW.Pain >= 80 && WoW.HealthPercent >= 50 && !WoW.CanCast("Demon Spikes"))))
			{
				WoW.CastSpell("Fracture");
				return;
			}
            
			if (WoW.CanCast("Spirit Bomb") && !WoW.IsSpellOnCooldown("Spirit Bomb") && WoW.PlayerHasBuff("Soul Fragments") && !WoW.TargetHasDebuff("Frailty"))
            {
                WoW.CastSpell("Spirit Bomb");
                return;
            }
            
			if (WoW.CanCast("Sigil of Flame") && (!WoW.TargetHasDebuff("Sigil of Flame")))
            {
                //WoW.CastSpellOnMe("Sigil of Flame");  // Use this if you not using "Concentrated Sigil's" talent - this is a little buggy!!!
                WoW.CastSpell("Sigil of Flame");  // NB must have "Concentrated Sigil's" talent
				return;
            }
			//if (WoW.CanCast("Felblade"))  // Pain Generator
            //{
            //    WoW.CastSpell("Felblade");
			//	return;
            //}
            if (WoW.CanCast("Sever") && WoW.IsSpellInRange("Soul Carver") && WoW.PlayerHasBuff("Metamorphosis")) // Pain Generator
            {
                WoW.CastSpell("Sever");
                return;
            }

			if (WoW.CanCast("Shear") && WoW.IsSpellInRange("Soul Carver") && !WoW.PlayerHasBuff("Metamorphosis")) // Pain Generator
            {
                WoW.CastSpell("Shear");
                return;
            }
  		}
    }
}

/*
[AddonDetails.db]
AddonAuthor=WiNiFiX
AddonName=_AXCM
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,203782,Shear,D2
Spell,228477,Soul Cleave,D3
Spell,207407,Soul Carver,D4
Spell,212084,Fel Devastation,F2
Spell,178740,Immolation Aura,D5
Spell,204596,Sigil of Flame,D9
Spell,204157,Throw Glaive,D6
Spell,207682,Sigil of Silence,D0
Spell,202719,Arcane Torrent,H
Spell,207684,Sigil of Misery,D7
Spell,187827,Metamorphosis,D8
Spell,204021,Fiery Brand,T
Spell,203720,Demon Spikes,G
Spell,232893,Felblade,D7
Spell,235964,Sever,D2
Spell,209795,Fracture,F7
Spell,183752,Consume Magic,J
Spell,6603,Auto Attack,I
Spell,0,HealthstoneKeybind,F10
Spell,247454,Spirit Bomb,F2
Aura,203819,Demon Spikes
Aura,235543,Spirit of the Darkness Flame
Aura,212818,Fiery Demise
Aura,41252,Mount
Aura,207472,Magnum Opus
Aura,204598,Sigil of Flame
Aura,187827,Metamorphosis
Aura,203981,Soul Fragments
Aura,247456,Frailty
Item,80610,Mana
Item,5512,Healthstone
*/
