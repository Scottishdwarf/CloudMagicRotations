// cheers to winifix@gmail.com !
// ReSharper disable UnusedMember.Global
// To do, add buttons to set in options (for eye and cd's), blade dance wait for 1 sec gcd - will be better dps or no


using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using CloudMagic.Helpers;

namespace CloudMagic.Rotation
{
    public class DemonHunterHavocJe : CombatRoutine
    {
        private NumericUpDown nudBlurPercentValue;
		
	private static readonly Stopwatch coolDownStopWatch = new Stopwatch();

        public override string Name { get { return "Havoc dual by Jedix"; } }

        public override string Class { get { return "Demon Hunter"; } }

        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
            //Log.Write("Welcome to CloudMagic Havoc by Jedix");

            if (ConfigFile.ReadValue("DemonHunter", "Blur Usage Percent") == "")
            {
                ConfigFile.WriteValue("Demonhunter", "Blur Usage Percent", "45");
            }
            Log.Write("Welcome to Havoc dual by Jedix", Color.Green);
            Log.Write("Suggested builds:single target - 2223311, aoe/cleave(good in myth+) - 1223112", Color.Green);
            Log.Write("IMPORTANT!", Color.Red);
            Log.Write("When you want to use single target build, use only single target rotation, when u want to use aoe build - use aoe/cleave rotations", Color.Black);
            Log.Write("When you use single target rotation, hold Z key to use eye beam in priority list automatically - to cleave, when needed. Numpad * key controls cooldown's use (on/off)", Color.Black);
            Log.Write("When you use aoe/cleave rotation, use your retreat and rush manually, to not fail (same for Meta burst)", Color.Black);

            SettingsForm = new Form {Text = "Settings", StartPosition = FormStartPosition.CenterScreen, Width = 800, Height = 490, ShowIcon = false};
            var lblBlurPercent = new Label {Text = "Blur Health %", Left = 12, Top = 150};
            SettingsForm.Controls.Add(lblBlurPercent);

            nudBlurPercentValue = new NumericUpDown {Minimum = 0, Maximum = 100, Value = ConfigFile.ReadValue<decimal>("Demonhunter", "Blur Usage Percent"), Left = 108, Top = 145};
            SettingsForm.Controls.Add(nudBlurPercentValue);

            var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 662, Top = 408, Size = new Size(108, 31)};
            cmdSave.Click += CmdSave_Click;

            SettingsForm.Controls.Add(cmdSave);
            nudBlurPercentValue.BringToFront();
        }

        private void CmdSave_Click(object sender, EventArgs e)
        {
            ConfigFile.WriteValue("Demonhunter", "Blur Usage Percent", nudBlurPercentValue.Value.ToString());
        }

        public override void Stop()
        {
        }

        public override void Pulse()
        {
            if (combatRoutine.Type == RotationType.SingleTarget) // Do Single Target Stuff here
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
                if (WoW.HasTarget && !WoW.PlayerIsChanneling && WoW.TargetIsEnemy && WoW.IsInCombat)
                {
                    if (WoW.PlayerHasBuff("Metamorphosis"))
                    {
						if (combatRoutine.UseCooldowns)
						{
							if (WoW.CanCast("Nemesis") && WoW.IsSpellInRange("Chaos Strike"))
							{
								WoW.CastSpell("Nemesis");
								return;
							}
							if (WoW.CanCast("Chaos Blades") && WoW.IsSpellInRange("Chaos Strike"))
							{
								WoW.CastSpell("Chaos Blades");
								return;
							}
						
							if (WoW.CanCast("FOTI") && WoW.IsSpellInRange("Chaos Strike"))
							{
								WoW.CastSpell("FOTI");
								return;
							}
						}
                        if (WoW.CanCast("Death Sweep") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                        {
                            WoW.CastSpell("Death Sweep");
                            return;
                        }
                        if (WoW.CanCast("Felblade") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury < 100) 
                        {
                            WoW.CastSpell("Felblade"); //Felblade only at melee range to not make worse (if you need to gtfo)
                            return;
                        }
                        if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike") && (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_Z) < 0))
                        {																
                            WoW.CastSpell("Eye Beam"); //Use Eyebeam by Z press
                            return;
                        }
                        if (WoW.CanCast("Annihilation") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 55)
                        {
                            WoW.CastSpell("Annihilation");
                            return;
                        }
                        //if (WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70)  // Fury Generator
                        //{
                        //    WoW.CastSpell("Demons Bite");
                        //    return;
                        //}
                        if (WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }
                    }
					if (combatRoutine.UseCooldowns)
					{
						if (WoW.CanCast("Nemesis") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Nemesis");
							return;
						}
						if (WoW.CanCast("Chaos Blades") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("Chaos Blades");
							return;
						}
					
						if (WoW.CanCast("FOTI") && WoW.IsSpellInRange("Chaos Strike"))
						{
							WoW.CastSpell("FOTI");
							return;
						}
					}
                    if (WoW.CanCast("Blade Dance") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                    {
                        WoW.CastSpell("Blade Dance");
                        return;
                    }
                    if (WoW.CanCast("Felblade") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury < 100) 
                    {
                        WoW.CastSpell("Felblade"); //Felblade only at melee range to not make worse (if you need to gtfo)
                        return;
                    }
                    if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.IsSpellInRange("Chaos Strike") && (DetectKeyPress.GetKeyState(DetectKeyPress.VK_KEY_Z) < 0))
                    {
                       WoW.CastSpell("Eye Beam"); //Use Eyebeam by Z press
                        return;
                    }
                    if (WoW.CanCast("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") &&
                        (WoW.Fury >= 70 || (WoW.Fury >= 55 && (WoW.PlayerHasBuff("Chaos Blades") || WoW.SpellCooldownTimeRemaining("Nemesis") >= 60))))
                    {
                        WoW.CastSpell("Chaos Strike"); //If we got damage buffs - spent fury on CS instantly (15 save for Blade Dance)
                        return;
                    }
                    if (WoW.CanCast("Blur") && WoW.IsInCombat && WoW.HealthPercent <= ConfigFile.ReadValue<int>("Demonhunter", "Blur Usage Percent"))
                    {
                        WoW.CastSpell("Blur");
                        return;
                    }
                    //if (WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70)  // Fury Generator
                    //{
                    //    WoW.CastSpell("Demons Bite");
                    //   return;
                    //}
                    if (WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive"))
                    {
                        WoW.CastSpell("Throw Glaive");
                        return;
                    }
                }
            }
            if (combatRoutine.Type == RotationType.AOE || combatRoutine.Type == RotationType.SingleTargetCleave)
            {
                // Do AOE Stuff here
                if (WoW.HasTarget && !WoW.PlayerIsChanneling && WoW.TargetIsEnemy && WoW.IsInCombat)
                {
                    if (WoW.PlayerHasBuff("Metamorphosis"))
                    {
                        if (WoW.CanCast("FOTI") && WoW.PlayerHasBuff("Momentum") && WoW.IsSpellInRange("Chaos Strike"))
                        {
                            WoW.CastSpell("FOTI");
                            return;
                        }
                        if (WoW.CanCast("Fel Barrage") && WoW.PlayerSpellCharges("Fel Barrage") == 5 && WoW.PlayerHasBuff("Momentum") && (WoW.PlayerBuffTimeRemaining("Momentum") >= 1) &&
                            WoW.IsSpellInRange("Fel Barrage"))
                        {
                            WoW.CastSpell("Fel Barrage");
                            return;
                        }
                        if (WoW.CanCast("Death Sweep") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                        {
                            WoW.CastSpell("Death Sweep");
                            return;
                        }
                        if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.PlayerHasBuff("Momentum") && (WoW.PlayerBuffTimeRemaining("Momentum") >= 1) && WoW.IsSpellInRange("Chaos Strike"))
                        {
                            WoW.CastSpell("Eye Beam");
                            return;
                        }
                        if (WoW.CanCast("Annihilation") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 40 && WoW.PlayerHasBuff("Momentum") || WoW.Fury >= 70))
                        {
                            WoW.CastSpell("Annihilation");
                            return;
                        }
                        if (WoW.CanCast("Fel Barrage") && WoW.PlayerSpellCharges("Fel Barrage") >= 4 && WoW.PlayerHasBuff("Momentum") && (WoW.PlayerBuffTimeRemaining("Momentum") >= 1) &&
                            WoW.IsSpellInRange("Fel Barrage"))
                        {
                            WoW.CastSpell("Fel Barrage");
                            return;
                        }
                        //if (WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70)  // Fury Generator
                        //{
                        //    WoW.CastSpell("Demons Bite");
                        //    return;
                        //}
                        if (WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive") && WoW.PlayerHasBuff("Momentum"))
                        {
                            WoW.CastSpell("Throw Glaive");
                            return;
                        }
                    }
                    if (WoW.CanCast("FOTI") && WoW.PlayerHasBuff("Momentum") && WoW.IsSpellInRange("Chaos Strike"))
                    {
                        WoW.CastSpell("FOTI");
                        return;
                    }
                    if (WoW.CanCast("Fel Barrage") && WoW.PlayerSpellCharges("Fel Barrage") == 5 && WoW.PlayerHasBuff("Momentum") && (WoW.PlayerBuffTimeRemaining("Momentum") >= 1) &&
                        WoW.IsSpellInRange("Fel Barrage"))
                    {
                        WoW.CastSpell("Fel Barrage");
                        return;
                    }
                    if (WoW.CanCast("Blade Dance") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury >= 15)
                    {
                        WoW.CastSpell("Blade Dance");
                        return;
                    }
                    if (WoW.CanCast("Eye Beam") && WoW.Fury >= 50 && WoW.PlayerHasBuff("Momentum") && (WoW.PlayerBuffTimeRemaining("Momentum") > 1) && WoW.IsSpellInRange("Chaos Strike"))
                    {
                        WoW.CastSpell("Eye Beam");
                        return;
                    }
                    if (WoW.CanCast("Chaos Strike") && WoW.IsSpellInRange("Chaos Strike") && (WoW.Fury >= 40 && WoW.PlayerHasBuff("Momentum") || WoW.Fury >= 70)) // Fury Spender
                    {
                        WoW.CastSpell("Chaos Strike");
                        return;
                    }
                    if (WoW.CanCast("Fel Barrage") && WoW.PlayerSpellCharges("Fel Barrage") >= 4 && WoW.PlayerHasBuff("Momentum") && (WoW.PlayerBuffTimeRemaining("Momentum") >= 1) &&
                        WoW.IsSpellInRange("Fel Barrage"))
                    {
                        WoW.CastSpell("Fel Barrage");
                        return;
                    }
                    if (WoW.CanCast("Blur") && WoW.IsInCombat && WoW.HealthPercent <= ConfigFile.ReadValue<int>("Demonhunter", "Blur Usage Percent"))
                    {
                        WoW.CastSpell("Blur");
                        return;
                    }
                    //if (WoW.CanCast("Demons Bite") && WoW.IsSpellInRange("Chaos Strike") && WoW.Fury <= 70)  // Fury Generator
                    //{
                    //    WoW.CastSpell("Demons Bite");
                    //   return;
                    //}
                    if (WoW.CanCast("Throw Glaive") && WoW.IsSpellInRange("Throw Glaive") && WoW.PlayerHasBuff("Momentum"))
                    {
                        WoW.CastSpell("Throw Glaive");
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
Spell,198013,Eye Beam,D2
Spell,195072,Fel Rush,MButton
Spell,206491,Nemesis,G
Spell,162794,Chaos Strike,E
Spell,185123,Throw Glaive,D3
Spell,188499,Blade Dance,Q
Spell,198793,Vengeful Retreat,D1
Spell,201427,Annihilation,E
Spell,210152,Death Sweep,Q
Spell,191427,Metamorphosis,NumPad6
Spell,198589,Blur,D4
Spell,211053,Fel Barrage,NumPad1
Spell,232893,Felblade,NumPad1
Spell,201467,FOTI,NumPad5
Spell,211048,Chaos Blades,NumPad2
Aura,162264,Metamorphosis
Aura,211048,Chaos Blades
Aura,208628,Momentum
*/
