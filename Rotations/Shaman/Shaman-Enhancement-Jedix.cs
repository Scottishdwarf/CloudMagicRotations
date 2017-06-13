// winifix@gmail.com
// ReSharper disable UnusedMember.Global


using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using CloudMagic.Helpers;

namespace CloudMagic.Rotation
{
    public class Enhancement : CombatRoutine
    {
		private static readonly Stopwatch coolDownStopWatch = new Stopwatch();
	
	public override string Name { get { return "Enhancement(Overcharge) by Jedix"; } }

        public override string Class { get { return "Shaman"; } }

        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
            Log.Write("Welcome to Enhancement Shaman by Jedix", Color.Green);
            Log.Write("Suggested build: 2212222 (HotHand+Overcharge+FoA)", Color.Green);
			Log.Write("If you got no set t19, use this rotation for most dps. Numpad * controls cooldowns on/off", Color.Green);
            //Log.Write("CloudMagic Enhancement");
        }

        public override void Stop()
        {
        }

        public override void Pulse()
        {
			if (!coolDownStopWatch.IsRunning || coolDownStopWatch.ElapsedMilliseconds > 60000)
							coolDownStopWatch.Restart();
					if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_MULTIPLY) < 0)  //Use cooldowns manage by *numButton
					{
						if (coolDownStopWatch.ElapsedMilliseconds > 1000)
						{
							combatRoutine.UseCooldowns = !combatRoutine.UseCooldowns;
							//Log.Write("Cooldowns " + (combatRoutine.UseCooldowns ? "On" : "Off"));
							coolDownStopWatch.Restart();
						}
					}
					
            if (combatRoutine.Type == RotationType.SingleTarget) // Do Single Target Stuff here
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerHasBuff("Ghost Wolf") && WoW.IsInCombat) //First things go first but break if we wanna run in Wolf
                {
					if (WoW.CanCast("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Wind Shear")) //interupt every spell
                    {
                        WoW.CastSpell("Wind Shear");
                        return;
                    }
					
					if (WoW.CanCast("Feral Spirit") && WoW.IsSpellInRange("Stormstrike") && combatRoutine.UseCooldowns && WoW.Maelstrom >= 20) //Wolves in melee range
                    {
                        Log.Write("Using Feral Spirit", Color.Red);
                        WoW.CastSpell("Feral Spirit");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.SpellCooldownTimeRemaining("Feral Spirit") >= 115) //Crash lightning for alpha wolf
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
					
                    if (WoW.CanCast("Rockbiter") && (!WoW.PlayerHasBuff("Landslide") || WoW.PlayerBuffTimeRemaining("Landslide") <= 1) && WoW.IsSpellInRange("Flametongue")) //REFRESH LANDSLIDE
                    {
                        Log.Write("Reseting Landslide", Color.Red);
                        WoW.CastSpell("Rockbiter");
                        return;
                    }

                    if (WoW.CanCast("Fury of Air") && !WoW.PlayerHasBuff("Fury of Air") && WoW.IsSpellInRange("Flametongue") && WoW.Maelstrom >= 22) //REFRESH Fury of Air
                    {
                        Log.Write("Reseting Fury of Air", Color.Red);
                        WoW.CastSpell("Fury of Air");
                        return;
                    }

                    if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue") && ((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") <=1)) || (WoW.SpellCooldownTimeRemaining("Doom Winds") <= 6 && WoW.PlayerBuffTimeRemaining("Flametongue") <= 4)) //REFRESH FLAMETONGUE
                    {
                        Log.Write("Reseting Flametongue buff", Color.Red);
                        WoW.CastSpell("Flametongue");
                        return;
                    }
					
					if (WoW.CanCast("Doom Winds") && combatRoutine.UseCooldowns && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Flametongue"))
                    {
                        WoW.CastSpell("Doom Winds");
                        return;
                    }
					
					if (WoW.CanCast("Lightning Bolt") && WoW.IsSpellInRange("Lightning Bolt") && WoW.Maelstrom >= 46) //Overcharge
                    {
                        WoW.CastSpell("Lightning Bolt"); 
                        return;
                    }
					
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 26)
                    {
                        WoW.CastSpell("Stormstrike");
                        return;
                    }
					
					if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Hot Hand")) //Hot Hand
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
					
					if (WoW.SpellCooldownTimeRemaining("Lightning Bolt") >= 1)  // to get not GCD locked on Bolt
						{
						if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 80)
							{
								WoW.CastSpell("Stormstrike");
								return;
							}

						if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 120)
							{
								WoW.CastSpell("Lava Lash"); //Buffer Lava
								return;
							}

						if (WoW.CanCast("Flametongue") && WoW.PlayerBuffTimeRemaining("Flametongue") <= 5 && WoW.IsSpellInRange("Flametongue"))
							{
								WoW.CastSpell("Flametongue"); //REFRESH FLAMETONGUE PANDEMIC
								return;
							}
						
						if (WoW.CanCast("Rockbiter") && WoW.IsSpellInRange("Flametongue")) 
							{
								WoW.CastSpell("Rockbiter"); //Nothing to do
								return;
							}
						if (WoW.CanCast("Feral Lunge") && !WoW.IsSpellInRange("Flametongue") && WoW.IsSpellInRange("Feral Lunge")) //out of range of flametongue 10y and in range of feral lunge 8-25y 
							{
								WoW.CastSpell("Feral Lunge");
								return;
							}
						}					

                    if (WoW.CanCast("Astral Shift") && WoW.HealthPercent < 60) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
                    {
                        WoW.CastSpell("Astral Shift");
                        return;
                    }
                }
            }
            if (combatRoutine.Type == RotationType.SingleTargetCleave)
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerHasBuff("Ghost Wolf") && WoW.IsInCombat) //First things go first
                {
                    if (WoW.CanCast("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Wind Shear")) //interupt every spell
                    {
                        WoW.CastSpell("Wind Shear");
                        return;
                    }
					
					if (WoW.CanCast("Feral Spirit") && WoW.IsSpellInRange("Stormstrike") && combatRoutine.UseCooldowns && WoW.Maelstrom >= 20) //Wolves in melee range
                    {
                        Log.Write("Using Feral Spirit", Color.Red);
                        WoW.CastSpell("Feral Spirit");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.SpellCooldownTimeRemaining("Feral Spirit") >= 115) //Crash lightning for alpha wolf
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
					
                    if (WoW.CanCast("Rockbiter") && (!WoW.PlayerHasBuff("Landslide") || WoW.PlayerBuffTimeRemaining("Landslide") <= 1) && WoW.IsSpellInRange("Flametongue")) //REFRESH LANDSLIDE
                    {
                        Log.Write("Reseting Landslide", Color.Red);
                        WoW.CastSpell("Rockbiter");
                        return;
                    }

                    if (WoW.CanCast("Fury of Air") && !WoW.PlayerHasBuff("Fury of Air") && WoW.IsSpellInRange("Flametongue") && WoW.Maelstrom >= 22) //REFRESH Fury of Air
                    {
                        Log.Write("Reseting Fury of Air", Color.Red);
                        WoW.CastSpell("Fury of Air");
                        return;
                    }

                    if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue") && ((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") <=1)) || (WoW.SpellCooldownTimeRemaining("Doom Winds") <= 6 && WoW.PlayerBuffTimeRemaining("Flametongue") <= 4)) //REFRESH FLAMETONGUE
                    {
                        Log.Write("Reseting Flametongue buff", Color.Red);
                        WoW.CastSpell("Flametongue");
                        return;
                    }
					
					if (WoW.CanCast("Doom Winds") && combatRoutine.UseCooldowns && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Flametongue"))
                    {
                        WoW.CastSpell("Doom Winds");
                        return;
                    }
					
					if (WoW.CanCast("Lightning Bolt") && WoW.IsSpellInRange("Lightning Bolt") && WoW.Maelstrom >= 46) //Overcharge
                    {
                        WoW.CastSpell("Lightning Bolt"); 
                        return;
                    }
					
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 26)
                    {
                        WoW.CastSpell("Stormstrike");
                        return;
                    }
					
					if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Hot Hand")) //Hot Hand
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 22 && WoW.IsSpellInRange("Stormstrike") && !WoW.PlayerHasBuff("Crash Lightning")) //Crash lightning for cleave to enable storm
                    {
						Log.Write("Reseting Crashing buff", Color.Blue);
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
					
					if (WoW.SpellCooldownTimeRemaining("Lightning Bolt") >= 1)  // to get not GCD locked on Bolt
						{
						if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 80)
							{
								WoW.CastSpell("Stormstrike");
								return;
							}

						if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 120)
							{
								WoW.CastSpell("Lava Lash"); //Buffer Lava
								return;
							}

						if (WoW.CanCast("Flametongue") && WoW.PlayerBuffTimeRemaining("Flametongue") <= 5 && WoW.IsSpellInRange("Flametongue"))
							{
								WoW.CastSpell("Flametongue"); //REFRESH FLAMETONGUE PANDEMIC
								return;
							}
						
						if (WoW.CanCast("Rockbiter") && WoW.IsSpellInRange("Flametongue")) 
							{
								WoW.CastSpell("Rockbiter"); //Nothing to do
								return;
							}
						if (WoW.CanCast("Feral Lunge") && !WoW.IsSpellInRange("Flametongue") && WoW.IsSpellInRange("Feral Lunge")) //out of range of flametongue 10y and in range of feral lunge 8-25y 
							{
								WoW.CastSpell("Feral Lunge");
								return;
							}
						}					

                    if (WoW.CanCast("Astral Shift") && WoW.HealthPercent < 60) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
                    {
                        WoW.CastSpell("Astral Shift");
                        return;
                    }
                }
            }
            if (combatRoutine.Type == RotationType.AOE)
            {
                if (WoW.HasTarget && WoW.TargetIsEnemy && !WoW.PlayerHasBuff("Ghost Wolf") && WoW.IsInCombat) //First things go first
                {
                    if (WoW.CanCast("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Wind Shear")) //interupt every spell
                    {
                        WoW.CastSpell("Wind Shear");
                        return;
                    }
					
					if (WoW.CanCast("Feral Spirit") && WoW.IsSpellInRange("Stormstrike") && combatRoutine.UseCooldowns && WoW.Maelstrom >= 20) //Wolves in melee range
                    {
                        Log.Write("Using Feral Spirit", Color.Red);
                        WoW.CastSpell("Feral Spirit");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 20 && WoW.SpellCooldownTimeRemaining("Feral Spirit") >= 115) //Crash lightning for alpha wolf
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
					
                    if (WoW.CanCast("Rockbiter") && (!WoW.PlayerHasBuff("Landslide") || WoW.PlayerBuffTimeRemaining("Landslide") <= 1) && WoW.IsSpellInRange("Flametongue")) //REFRESH LANDSLIDE
                    {
                        Log.Write("Reseting Landslide", Color.Red);
                        WoW.CastSpell("Rockbiter");
                        return;
                    }

                    if (WoW.CanCast("Fury of Air") && !WoW.PlayerHasBuff("Fury of Air") && WoW.IsSpellInRange("Flametongue") && WoW.Maelstrom >= 22) //REFRESH Fury of Air
                    {
                        Log.Write("Reseting Fury of Air", Color.Red);
                        WoW.CastSpell("Fury of Air");
                        return;
                    }

                    if (WoW.CanCast("Flametongue") && WoW.IsSpellInRange("Flametongue") && ((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") <=1)) || (WoW.SpellCooldownTimeRemaining("Doom Winds") <= 6 && WoW.PlayerBuffTimeRemaining("Flametongue") <= 4)) //REFRESH FLAMETONGUE
                    {
                        Log.Write("Reseting Flametongue buff", Color.Red);
                        WoW.CastSpell("Flametongue");
                        return;
                    }
					
					if (WoW.CanCast("Doom Winds") && combatRoutine.UseCooldowns && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Flametongue"))
                    {
                        WoW.CastSpell("Doom Winds");
                        return;
                    }
					
					if (WoW.CanCast("Lightning Bolt") && WoW.IsSpellInRange("Lightning Bolt") && WoW.Maelstrom >= 46) //Overcharge
                    {
                        WoW.CastSpell("Lightning Bolt"); 
                        return;
                    }
					
					if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 26)
                    {
                        WoW.CastSpell("Stormstrike");
                        return;
                    }
					
					if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Hot Hand")) //Hot Hand
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
					
					if (WoW.CanCast("Crash Lightning") && WoW.Maelstrom >= 22 && WoW.IsSpellInRange("Stormstrike")) //Crash lightning priority for aoe
                    {
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
					
					if (WoW.SpellCooldownTimeRemaining("Lightning Bolt") >= 1)  // to get not GCD locked on Bolt
						{
						if (WoW.CanCast("Stormstrike") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 80)
							{
								WoW.CastSpell("Stormstrike");
								return;
							}

						if (WoW.CanCast("Lava Lash") && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 120)
							{
								WoW.CastSpell("Lava Lash"); //Buffer Lava
								return;
							}

						if (WoW.CanCast("Flametongue") && WoW.PlayerBuffTimeRemaining("Flametongue") <= 5 && WoW.IsSpellInRange("Flametongue"))
							{
								WoW.CastSpell("Flametongue"); //REFRESH FLAMETONGUE PANDEMIC
								return;
							}
						
						if (WoW.CanCast("Rockbiter") && WoW.IsSpellInRange("Flametongue")) 
							{
								WoW.CastSpell("Rockbiter"); //Nothing to do
								return;
							}
						if (WoW.CanCast("Feral Lunge") && !WoW.IsSpellInRange("Flametongue") && WoW.IsSpellInRange("Feral Lunge")) //out of range of flametongue 10y and in range of feral lunge 8-25y 
							{
								WoW.CastSpell("Feral Lunge");
								return;
							}
						}					

                    if (WoW.CanCast("Astral Shift") && WoW.HealthPercent < 60) //ASTRAL SHIFT - DMG REDUCTION if we are below 60% of HP
                    {
                        WoW.CastSpell("Astral Shift");
                        return;
                    }
                }
            }
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Jedix
AddonName=Pawn
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,57994,Wind Shear,NumPad9
Spell,196884,Feral Lunge,NumPad6
Spell,51533,Feral Spirit,NumPad2
Spell,204945,Doom Winds,NumPad3
Spell,187874,Crash Lightning,D3
Spell,193796,Flametongue,D1
Spell,108271,Astral Shift,D5
Spell,193786,Rockbiter,G
Spell,197211,Fury of Air,D6
Spell,60103,Lava Lash,E
Spell,17364,Stormstrike,Q
Spell,187837,Lightning Bolt,NumPad5
Aura,194084,Flametongue
Aura,197211,Fury of Air
Aura,187878,Crash Lightning
Aura,202004,Landslide
Aura,215785,Hot Hand
Aura,201846,Stormbringer
Aura,204945,Doom Winds
Aura,2645,Ghost Wolf
*/
