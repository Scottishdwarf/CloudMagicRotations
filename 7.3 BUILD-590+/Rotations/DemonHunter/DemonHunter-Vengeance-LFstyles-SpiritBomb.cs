using CloudMagic.Helpers;
using System.Diagnostics;
using System.Drawing;
using System;
using System.Threading;
using System.Windows.Forms;
using CloudMagic.GUI;


namespace CloudMagic.Rotation
{
    public class DemonHunterVeng : CombatRoutine
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
            Log.WriteCloudMagic("Welcome to LFstyles Vengeance Demon Hunter ", Color.Black);
            Log.Write("Suggested build: 1222331", Color.Green);
			Log.Write("Must Use this Macro to manually use infernal strike: #showtooltip /cast [@cursor] Infernal strike", Color.Purple);
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

			if (WoW.HealthPercent < 30 && !WoW.IsSpellOnCooldown("Metamorphosis"))
            {
                Log.Write("Health low < 40% using CDs...", Color.Red);
                WoW.CastSpell("Metamorphosis"); // Off the GCD no return needed
            }
			{
			if (!WoW.PlayerHasBuff("Metamorphosis"))
			{
				if (!WoW.PlayerIsCasting && WoW.ItemCount("Healthstone") >= 1 && !WoW.ItemOnCooldown("Healthstone") && WoW.HealthPercent < 25)
				{
					WoW.CastSpell("HealthstoneKeybind");
				}
				if (WoW.CanCast("Throw Glaive") && !WoW.IsSpellOnCooldown("Throw Glaive") && !WoW.IsSpellInRange("Soul Carver"))
				{
					WoW.CastSpell("Throw Glaive");
					return; 
				}
				if (WoW.CanCast("Immolation Aura") && WoW.IsSpellInRange("Soul Carver"))  
				{
					WoW.CastSpell("Immolation Aura");
					return;
				}
				if (WoW.CanCast("Soul Carver") && WoW.IsSpellInRange("Soul Carver"))
				{
					WoW.CastSpell("Soul Carver");
					return;
				}
				if (WoW.CanCast("Fracture") && WoW.IsSpellInRange("Soul Carver") && (WoW.Pain > 30 && (WoW.PlayerBuffStacks("Soul Fragments") < 3)))
				{
					WoW.CastSpell("Fracture");
					return;
				}
				if (WoW.CanCast("Spirit Bomb") && !WoW.IsSpellOnCooldown("Spirit Bomb") && (WoW.PlayerHasBuff("Soul Fragments") && !WoW.TargetHasDebuff("Frailty")))
				{	
					WoW.CastSpell("Spirit Bomb");
					return;
				}
				if (WoW.CanCast("Spirit Bomb") && !WoW.IsSpellOnCooldown("Spirit Bomb") && (WoW.PlayerHasBuff("Soul Fragments") && (WoW.PlayerBuffStacks("Soul Fragments") >= 3)))
				{
					WoW.CastSpell("Spirit Bomb");
					return;
				}
				if (WoW.CanCast("Fiery Brand") && !WoW.TargetHasDebuff("Fiery Demise"))
				{
					WoW.CastSpell("Fiery Brand");
				}
			    if (WoW.CanCast("Soul Cleave") && !WoW.IsSpellOnCooldown("Soul Cleave") && WoW.IsSpellInRange("Soul Carver") && WoW.Pain > 100)
				{
					WoW.CastSpell("Soul Cleave");
					return;
				}
				// if (WoW.CanCast("Demon Spikes") && !WoW.PlayerHasBuff("Demon Spikes") && !WoW.IsSpellOnCooldown("Demon Spikes") && (WoW.Pain == 80 || WoW.HealthPercent < 80 && WoW.Pain > 20 )) // to not waste cd and pain
                // {
					// WoW.CastSpell("Demon Spikes");
                // }

				if (WoW.CanCast("Shear") && WoW.IsSpellInRange("Soul Carver") && WoW.Pain < 30 || WoW.Pain < 100 && !WoW.PlayerHasBuff("Soul Fragments")) // Pain Generator
				{
					WoW.CastSpell("Shear");
					return;
				}
				if (WoW.CanCast("Demon Spikes") && !WoW.PlayerHasBuff("Demon Spikes") && (WoW.Pain >= 90 || WoW.HealthPercent < 80 && WoW.Pain >= 20)) // to not waste CD and Pain
				{
					WoW.CastSpell("Demon Spikes");
				}
				if (WoW.CanCast("Sigil of Flame") && (!WoW.TargetHasDebuff("Sigil of Flame") && WoW.IsSpellInRange("Soul Carver")))
				{
					WoW.CastSpell("Sigil of Flame");  // Must have "Concentrated Sigil's" talent and macro set up
					return;
				}
				// if (WoW.ItemCount("Trinket") == 1 && !WoW.ItemOnCooldown("Trinket") && WoW.IsSpellInRange("Soul Carver"))
				// {
					// WoW.CastSpell("TrinketKeybind");
					// return;
				// }
			}
			if (WoW.PlayerHasBuff("Metamorphosis"))
			{
					if (WoW.CanCast("Soul Carver") && WoW.IsSpellInRange("Soul Carver"))
					{
						WoW.CastSpell("Soul Carver");
						return;
					}
					if (WoW.CanCast("Sever") && WoW.IsSpellInRange("Soul Carver") && WoW.PlayerBuffStacks("Soul Fragments") <= 3)
					{
						WoW.CastSpell("Sever");
						return;
					}
					if (!WoW.PlayerIsCasting && WoW.ItemCount("Healthstone") >= 1 && !WoW.ItemOnCooldown("Healthstone") && WoW.HealthPercent < 30)
					{
						WoW.CastSpell("HealthstoneKeybind");
					}
					if (WoW.CanCast("Throw Glaive") && !WoW.IsSpellOnCooldown("Throw Glaive") && !WoW.IsSpellInRange("Soul Carver"))
					{
						WoW.CastSpell("Throw Glaive");
						return; 
					}
					if (WoW.CanCast("Immolation Aura") && WoW.IsSpellInRange("Soul Carver"))  
					{
						WoW.CastSpell("Immolation Aura");
						return;
					}
					if (WoW.CanCast("Soul Cleave") && !WoW.IsSpellOnCooldown("Soul Cleave") && WoW.IsSpellInRange("Soul Carver") && (WoW.Pain > 50 && WoW.PlayerBuffStacks("Soul Fragments") >= 3))
					{
						WoW.CastSpell("Soul Cleave");
						return;
					}
					// if (WoW.CanCast("Spirit Bomb") && !WoW.IsSpellOnCooldown("Spirit Bomb") && (WoW.PlayerHasBuff("Soul Fragments") && (WoW.PlayerBuffStacks("Soul Fragments") >= 5)))
					// {
						// WoW.CastSpell("Spirit Bomb");
						// return;
					// }
				 // if (WoW.CanCast("Sever") && !WoW.CanCast("Soul Carver") && WoW.PlayerBuffStacks("Soul Fragments") < 5)
					// {
						// WoW.CastSpell("Sever");
						// return;
					// }
					if (WoW.CanCast("Sigil of Flame") && (!WoW.TargetHasDebuff("Sigil of Flame") && WoW.IsSpellInRange("Soul Carver")))
					{
						WoW.CastSpell("Sigil of Flame");  // NB must have "Concentrated Sigil's" talent
						return;
					}

					// if (WoW.ItemCount("Trinket") == 1 && !WoW.ItemOnCooldown("Trinket") && WoW.IsSpellInRange("Soul Carver"))
					// {
						// WoW.CastSpell("TrinketKeybind");
						// return;
					// }
			}
			// if (WoW.TargetCastingSpellID == 233441)
				// {
				// }
			// if (WoW.CanCast("Fiery Brand") && !WoW.TargetHasDebuff("Fiery Demise") && WoW.PlayerHasBuff("Spirit of the Darkness Flame") && (WoW.PlayerBuffStacks("Spirit of the Darkness Flame") >= 7))
            // {
                // WoW.CastSpell("Fiery Brand");
            // }
            // if (WoW.CanCast("Demon Spikes") && !WoW.PlayerHasBuff("Demon Spikes") && (WoW.Pain >= 90 || WoW.HealthPercent < 90 && WoW.Pain >= 20)) // to not waste cd and pain
            // {
                // WoW.CastSpell("Demon Spikes");
            // }
            // if (WoW.CanCast("Fel Devastation") && WoW.IsSpellInRange("Soul Carver") && WoW.Pain >= 30)
            // {
                // WoW.CastSpell("Fel Devastation");
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
			// if (WoW.CanCast("Fracture") && WoW.IsSpellInRange("Soul Carver") && ((WoW.Pain >= 60 && WoW.HealthPercent >= 40 && WoW.PlayerHasBuff("Demon Spikes")) || (WoW.Pain >= 80 && WoW.HealthPercent >= 40 && !WoW.CanCast("Demon Spikes"))))
			// {
				// WoW.CastSpell("Fracture");
				// return;
			// }
			//if (WoW.CanCast("Felblade"))  // Pain Generator
            //{
            //    WoW.CastSpell("Felblade");
			//	return;
            //}
			}
		}
	}
}


/*
[AddonDetails.db]
AddonAuthor=LFstyles
AddonName=
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,203782,Shear,D2
Spell,235964,Sever,D2
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
Spell,209795,Fracture,F7
Spell,183752,Consume Magic,J
Spell,6603,Auto Attack,I
Spell,247454,Spirit Bomb,F2
Spell,0,HealthstoneKeybind,F9
Spell,1,TrinketKeybind,F3
Aura,203819,Demon Spikes
Aura,235543,Spirit of the Darkness Flame
Aura,212818,Fiery Demise
Aura,41252,Mount
Aura,207472,Magnum Opus
Aura,204598,Sigil of Flame
Aura,187827,Metamorphosis
Aura,203981,Soul Fragments
Aura,247456,Frailty
Item,5512,Healthstone
Item,142168,Trinket
*/
