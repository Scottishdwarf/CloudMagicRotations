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
    public class HunterBeastmastery : CombatRoutine
    {
        public override string Name
        {get{return "Hunter Beastmastery";}}

        public override string Class
        {get{return "Hunter";}}

        public override Form SettingsForm { get; set; }        

        public override void Initialize()
        {	
			Log.Write("Welcome to Beastmastery Hunter", Color.Green);
			Log.Write("The rotation assumes you have Tier 19 4p bonus, no particular legendary and 4/4 Slithering Serpents weapon trait for optimal DPS but will work in any case", Color.Red);
			Log.Write("All talents are supported and auto detected on any loading screen or when entering combat", Color.Red);
            Log.Write("Potion/Healthstone will be used at 20% health or less, Exhilaration at 30% or less", Color.Red);
			Log.Write("Counter Shot will be used on +75% of the spell's cast time", Color.Red);
			Log.Write("To use Binding Shot and Tarps at cursor location you need macros: /cast [@cursor] Binding Shot, /cast [@cursor] Freezing Trap and /cast [@cursor] Tar Trap", Color.Red);
			Log.Write("Stampede, if chosen, will be cast if Left Ctrl key is pressed", Color.Red);
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
                    SelectRotation(4, 2, 1);
                    SelectRotation(4, 3, 1);
                }

                //Healthstone - Potion
                if ((WoW.CanCast("Healthstone") || WoW.CanCast("Potion"))
                    && (WoW.ItemCount("Healthstone") >= 1 || WoW.ItemCount("Potion") >= 1)
                    && (!WoW.ItemOnCooldown("Healthstone") || !WoW.ItemOnCooldown("Potion"))
                    && !WoW.PlayerHasBuff("Feign Death")
                    && WoW.HealthPercent <= 15
                    && !WoW.IsMounted
                    && WoW.IsInCombat
                    && WoW.HealthPercent > 1)
                {
                    Thread.Sleep(500);
                    WoW.CastSpell("Healthstone");
                    WoW.CastSpell("Potion");
                    return;
                }

                //Exhilaration
                if (WoW.CanCast("Exhilaration")
                    && WoW.HealthPercent <= 25
                    && !WoW.IsMounted
                    && !WoW.PlayerHasBuff("Feign Death")
                    && WoW.HealthPercent > 1)
                {
                    WoW.CastSpell("Exhilaration");
                    return;
                }

                //Counter Shot
                if (WoW.CanCast("Counter Shot")
                    && (WoW.TargetIsCastingAndSpellIsInterruptible)
                    && !WoW.IsSpellOnCooldown("Counter Shot")
                    && WoW.TargetIsCasting
                    && WoW.IsSpellInRange("Counter Shot")
                    && WoW.TargetPercentCast > 75)
                {
                    WoW.CastSpell("Counter Shot");
                    return;
                }

                //Mend Pet
                if (WoW.HasPet
                    && WoW.CanCast("Mend Pet")
                    && WoW.HealthPercent > 1
                    && WoW.PetHealthPercent <= 40
                    && !WoW.PlayerHasBuff("Feign Death"))
                {
                    WoW.CastSpell("Mend Pet");
                    return;
                }

                //Revive Pet Call pet
                if (!WoW.HasPet
                    && !WoW.IsMounted
                    && WoW.HealthPercent > 1
                    && !WoW.PlayerHasBuff("Feign Death"))
                {                    
                    WoW.CastSpell("Call Pet");
                    Thread.Sleep(500);
                    WoW.CastSpell("Heart of the Phoenix");
                    WoW.CastSpell("Revive Pet");
                    return;
                }

                //Voley
                if (WoW.CanCast("Voley")
                    && !WoW.PlayerHasBuff("Feign Death")
                    && !WoW.PlayerHasBuff("Voley")
                    && WoW.HealthPercent != 0
                    && WoW.Talent(6) == 3)
                {
                    WoW.CastSpell("Voley");
                    return;
                }

                //Intimidation //Binding Shot
                if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_Z) < 0
                    && ((WoW.CanCast("Intimidation") && WoW.Talent(5) == 3) || (WoW.CanCast("Binding Shot") && WoW.Talent(5) == 1)))
                {
                    WoW.CastSpell("Binding Shot");
                    WoW.CastSpell("Intimidation");
                    return;
                }

                //Freezing Trap
                if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_X) < 0
                    && WoW.CanCast("Freezing Trap")
                    && !WoW.IsMounted)
                {
                    WoW.CastSpell("Freezing Trap");
                    return;
                }

				//Tar Trap
                if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_C) < 0
                    && WoW.CanCast("Tar Trap")
                     && !WoW.IsMounted)
                {
                    WoW.CastSpell("Tar Trap");
                    return;
                }

                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.IsMounted && !WoW.PlayerIsChanneling && !WoW.PlayerHasBuff("Feign Death") && WoW.HealthPercent > 1 && !WoW.TargetHasBuff("Nether Gale"))
                {
                    //Cooldowns
                    if (UseCooldowns)
                    {
                    }

                    //Stampede
                    if (DetectKeyPress.GetKeyState(DetectKeyPress.VK_LCONTROL) < 0
                        && WoW.CanCast("Stampede")
                        && WoW.Talent(7) == 1)
                    {
                        WoW.CastSpell("Stampede");
                        return;
                    }                    

                    //SINGLE TARGET
                    //A Murder of Crows
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && WoW.CanCast("A Murder of Crows")
                        && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") < 300 || WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 3000)
                        //&& WoW.PlayerHasBuff("Bestial Wrath")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(6) == 1
                        && WoW.Focus >= 30)
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }

                    //Bestial Wrath
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && WoW.CanCast("Bestial Wrath")
                        && !WoW.PlayerHasBuff("Aspect of the Turtle")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && (WoW.Focus >= 110 || (WoW.Focus >= 90 && WoW.CanCast("Aspect of the Wild"))))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        WoW.CastSpell("Aspect of the Wild");
                        WoW.CastSpell("Kill Command");
                    }

                    //Aspect of the Wild
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && WoW.CanCast("Aspect of the Wild")
                        && !WoW.PlayerHasBuff("Aspect of the Turtle")
                        //&& WoW.PlayerHasBuff("Bestial Wrath")
                        //&& WoW.PlayerBuffTimeRemaining("Bestial Wrath") >= 700
                        )
                    {
                        WoW.CastSpell("Aspect of the Wild");
                    }
                                        
                    //Dire Frenzy
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && (WoW.CanCast("Dire Frenzy") || WoW.SpellCooldownTimeRemaining("Dire Frenzy") <= 100)
                        && WoW.Talent(2) == 2
                        && ((WoW.PetHasBuff("Dire Frenzy") && WoW.PetBuffTimeRemaining("Dire Frenzy") <= 200) || !WoW.PetHasBuff("Dire Frenzy") || WoW.PlayerSpellCharges("Dire Frenzy") >= 2)                        )
                    {
                        WoW.CastSpell("Titan's Thunder");
                        WoW.CastSpell("Dire Frenzy");
                        return;
                    }

                    // Dire beast
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && WoW.CanCast("Dire Beast")
                        && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 300 || WoW.PlayerSpellCharges("Dire Beast") >= 2)
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(2) != 2)
                    {
                        WoW.CastSpell("Dire Beast");
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }
                    
                    //Kill Command
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && (WoW.SpellCooldownTimeRemaining("Kill Command") <= 100 || WoW.CanCast("Kill Command"))
                        && WoW.Focus >= 30)
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }

                    //Chimaera Shot
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && WoW.CanCast("Chimaera Shot")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(2) == 3
                        && WoW.Focus < 80)
                    {
                        WoW.CastSpell("Chimaera Shot");
                        return;
                    }

                    //Cobra Shot
                    if (combatRoutine.Type == RotationType.SingleTarget
                        && ((WoW.Focus >= 90) || (WoW.PlayerHasBuff("Bestial Wrath") && WoW.Focus >= 32))
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.CanCast("Cobra Shot")
                        && !WoW.CanCast("Bestial Wrath"))
                    {
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }

                    //AOE
                    //Bestial Wrath
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Bestial Wrath")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && !WoW.PlayerHasBuff("Aspect of the Turtle"))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        return;
                    }

                    // Dire beast
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Dire Beast")
                        && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 300
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(2) != 2)
                    {
                        WoW.CastSpell("Dire Beast");
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }

                    //Dire Frenzy
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Dire Frenzy")
                        && WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 600
                        && WoW.Talent(2) == 2)
                    {
						WoW.CastSpell("Titan's Thunder");
                        WoW.CastSpell("Dire Frenzy");                        
                        return;
                    }

                    //Barrage
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Barrage")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(6) == 2
                        && (WoW.Focus >= 60))
                    {
                        WoW.CastSpell("Barrage");
                        return;
                    }

                    //Multishot
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Multi-Shot")
                        && WoW.IsSpellInRange("Multi-Shot")
                        && WoW.Focus >= 40)
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }

                    //Chimaera Shot
                    if (combatRoutine.Type == RotationType.AOE
                        && WoW.CanCast("Chimaera Shot")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(2) == 3
                        && WoW.Focus < 80)
                    {
                        WoW.CastSpell("Chimaera Shot");
                        return;
                    }

                    //CLEAVE 
                    //A Murder of Crows
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("A Murder of Crows")
                        && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") < 300 || WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 3000)
                        //&& WoW.PlayerHasBuff("Bestial Wrath")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(6) == 1
                        && WoW.Focus >= 30)
                    {
                        WoW.CastSpell("A Murder of Crows");
                        return;
                    }

                    //Bestial Wrath
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Bestial Wrath")
                        && !WoW.PlayerHasBuff("Aspect of the Turtle")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && (WoW.Focus >= 110 || (WoW.Focus >= 90 && WoW.CanCast("Aspect of the Wild"))))
                    {
                        WoW.CastSpell("Bestial Wrath");
                        WoW.CastSpell("Aspect of the Wild");
                        WoW.CastSpell("Kill Command");
                    }

                    //Aspect of the Wild
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Aspect of the Wild")
                        && !WoW.PlayerHasBuff("Aspect of the Turtle")
                        //&& WoW.PlayerHasBuff("Bestial Wrath")
                        //&& WoW.PlayerBuffTimeRemaining("Bestial Wrath") >= 700
                        )
                    {
                        WoW.CastSpell("Aspect of the Wild");
                    }

                    //Dire Frenzy
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && (WoW.CanCast("Dire Frenzy") || WoW.SpellCooldownTimeRemaining("Dire Frenzy") <= 100)
                        && WoW.Talent(2) == 2
                        && ((WoW.PetHasBuff("Dire Frenzy") && WoW.PetBuffTimeRemaining("Dire Frenzy") <= 200) || !WoW.PetHasBuff("Dire Frenzy") || WoW.PlayerSpellCharges("Dire Frenzy") >= 2))
                    {
                        WoW.CastSpell("Titan's Thunder");
                        WoW.CastSpell("Dire Frenzy");
                        return;
                    }

                    // Dire beast
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Dire Beast")
                        && (WoW.SpellCooldownTimeRemaining("Bestial Wrath") > 300 || WoW.PlayerSpellCharges("Dire Beast") >= 2)
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(2) != 2)
                    {
                        WoW.CastSpell("Dire Beast");
                        WoW.CastSpell("Titan's Thunder");
                        return;
                    }

                    //Barrage
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Barrage")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(6) == 2
                        && (WoW.Focus >= 60))
                    {
                        WoW.CastSpell("Barrage");
                        return;
                    }

                    //Kill Command
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && (WoW.SpellCooldownTimeRemaining("Kill Command") <= 100 || WoW.CanCast("Kill Command"))
                        && WoW.Focus >= 30)
                    {
                        WoW.CastSpell("Kill Command");
                        return;
                    }

                    //Multishot - Beast Cleave uptime
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Multi-Shot")
                        && (!WoW.PetHasBuff("BeastCleave") || WoW.PetBuffTimeRemaining("BeastCleave") < 0.5)
                        && WoW.IsSpellInRange("Multi-Shot")
                        && !WoW.CanCast("Bestial Wrath")
                        && WoW.Focus >= 40)
                    {
                        WoW.CastSpell("Multi-Shot");
                        return;
                    }

                    //Chimaera Shot
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && WoW.CanCast("Chimaera Shot")
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.Talent(2) == 3
                        && WoW.Focus < 80)
                    {
                        WoW.CastSpell("Chimaera Shot");
                        return;
                    }

                    //Cobra Shot
                    if (combatRoutine.Type == RotationType.SingleTargetCleave
                        && ((WoW.Focus >= 90) || (WoW.PlayerHasBuff("Bestial Wrath") && (WoW.Focus >= 32)))
                        && WoW.IsSpellInRange("Cobra Shot")
                        && WoW.CanCast("Cobra Shot")
                        && !WoW.CanCast("Bestial Wrath"))
                    {
                        WoW.CastSpell("Cobra Shot");
                        return;
                    }

                    //Mend Pet
                    if (WoW.HasPet
                        && WoW.CanCast("Mend Pet")
                        && WoW.HealthPercent != 0
                        && WoW.PetHealthPercent <= 75
                        && !WoW.PlayerHasBuff("Feign Death"))
                    {
                        WoW.CastSpell("Mend Pet");
                        return;
                    }
                }            
        }

        #region Functions

        public override int Interrupt_Ability_Id
        {
            get
            {
                return 6552;
            }
        }

        private static bool lastNamePlate = true;
        public override int AoE_Range
        {
            get
            {
                return 8;
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
Spell,136,Mend Pet,D0
Spell,982,Revive Pet,D0
Spell,883,Call Pet,OemMinus
Spell,5512,Healthstone,D1
Spell,127834,Potion,D1
Spell,19577,Intimidation,D7
Spell,109248,Binding Shot,D7
Spell,193455,Cobra Shot,NumPad2
Spell,109304,Exhilaration,NumPad7
Spell,120679,Dire Beast,NumPad1
Spell,217200,Dire Frenzy,NumPad1
Spell,34026,Kill Command,NumPad3
Spell,131894,A Murder of Crows,NumPad4
Spell,120360,Barrage,NumPad4
Spell,194386,Voley,NumPad4
Spell,2643,Multi-Shot,NumPad5
Spell,207068,Titan's Thunder,Add
Spell,19574,Bestial Wrath,NumPad9
Spell,55709,Heart of the Phoenix,D0
Spell,193530,Aspect of the Wild,Divide
Spell,53209,Chimaera Shot,D0
Spell,201430,Stampede,NumPad6
Spell,147362,Counter Shot,Decimal
Spell,187650,Freezing Trap,D8
Spell,187698,Tar Trap,D9
Aura,244834,Nether Gale
Aura,120694,Dire Beast
Aura,5384,Feign Death
Aura,19574,Bestial Wrath
Aura,127271,Mount
Aura,186265,Aspect of the Turtle
Aura,118455,BeastCleave
Aura,194386,Voley
Aura,217200,Dire Frenzy
Item,5512,Healthstone
Item,127834,Potion
*/
