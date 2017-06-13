// ReSharper disable UnusedMember.Global


using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CloudMagic.Helpers;

namespace CloudMagic.Rotation
{
    public class DemonologyWarlock : CombatRoutine
    {
        private static bool empowered;
        private static bool threeimps;




        //Boss bool (easier for testing)



        private readonly Stopwatch CombatTime = new Stopwatch();
        private readonly Stopwatch DreadstalkersTime = new Stopwatch();
        private readonly Stopwatch ImpTime = new Stopwatch();
		

        //Settings


        private readonly string readme = "Vectarius Demonology Rotation" + Environment.NewLine + "";

        //Talents

				private float OneFiveCast
        {
            get
            {
                    return 150f / (1f + (WoW.HastePercent / 100f));
            }
        }
		private float TwoSecondCast
        {
            get
            {
                    return 200f / (1f + (WoW.HastePercent / 100f));
            }
        }		
		 private float GCD
        {
            get
            {

                    return (150f / (1f + (WoW.HastePercent / 100f)));

            }
        }







        public override Form SettingsForm { get; set; }

      public override string Name
        {
            get { return "Warlock Demonology"; }
        }

        
		 public override string Class
        {
            get { return "Warlock"; }
        }
        private static bool lastNamePlate = true;
		public override int CLEAVE { get { return 99; } } //please Set between 1-99 NpC in range for AOE  if not desired set to 99
        public override int AOE { get { return 3; } }//please Set between 1-99 NpC in range for Cleave if not desired set to 99
        public override int SINGLE {get { return 1; } }//please Set between 1-99 NpC in range for ST if not desired set to 99  
        public override void Initialize()
        {
 
            Log.Write("Vectarius Demo Warlock");

            SettingsForm = new Form {Text = "Vectarius Demo Warlock Rotation - Settings", StartPosition = FormStartPosition.CenterScreen, Width = 480, Height = 318, ShowIcon = false};







            var cmdSave = new Button {Text = "Save", Width = 65, Height = 25, Left = 332, Top = 243, Size = new Size(120, 31)};
            var cmdReadme = new Button {Text = "Read Me", Width = 65, Height = 25, Left = 332, Top = 213, Size = new Size(120, 31)};





            cmdSave.Click += CmdSave_Click;
            cmdReadme.Click += CmdReadme_Click;



            SettingsForm.Controls.Add(cmdSave);
            SettingsForm.Controls.Add(cmdReadme);

        }

        private void CmdReadme_Click(object sender, EventArgs e)
        {
            MessageBox.Show(readme, "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CmdSave_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Settings saved.", "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsForm.Close();
        }

        // Checkboxes



        public override void Stop()
        {
        }

        public override void Pulse()
        {
            //Combat Time
            if (CombatTime.IsRunning && !WoW.IsInCombat)
            {
                CombatTime.Reset();
            }
            if (!CombatTime.IsRunning && WoW.IsInCombat)
            {
                CombatTime.Start();
            }

            //Dreadstalkers Time Remaining (12000 ms) (EXPERIMENTAL)
            if (DreadstalkersTime.IsRunning && WoW.DreadstalkersCount == 0)
            {
                DreadstalkersTime.Reset();
            }
            if (!DreadstalkersTime.IsRunning && WoW.DreadstalkersCount >= 1)
            {
                DreadstalkersTime.Start();
            }
            if (DreadstalkersTime.IsRunning && DreadstalkersTime.ElapsedMilliseconds > 12000)
            {
                DreadstalkersTime.Reset();
            }			

            //Imp Time Remaining (12000 ms) (EXPERIMENTAL)
            if (ImpTime.IsRunning && WoW.WildImpsCount == 0)
            {
                ImpTime.Reset();
            }
            if (!ImpTime.IsRunning && WoW.WildImpsCount >= 1)
            {
                ImpTime.Start();
            }

            var DreadstalkersRemainingDuration = Convert.ToSingle((12000f - DreadstalkersTime.ElapsedMilliseconds)/10f);
            var ImpsRemainingDuration = Convert.ToSingle((12000f - ImpTime.ElapsedMilliseconds)/10f);

            // Single Target Rotation
            if (combatRoutine.Type == RotationType.SingleTarget)
            {
				
                // Normal Rotation
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerIsCasting)
                {
                    Log.Write("Imp Timer running: " + ImpsRemainingDuration, Color.DarkViolet);
					Log.Write("Imp Time: " + ImpTime.ElapsedMilliseconds, Color.DarkViolet);
					Log.Write("Imp Count: " + WoW.WildImpsCount, Color.Green);
                    Log.Write("Dread Timer running: " + DreadstalkersTime.ElapsedMilliseconds, Color.DarkViolet);
					Log.Write("Dread Time: " + DreadstalkersRemainingDuration, Color.DarkViolet);
                    //Implosion (if talent)
                    if (WoW.CanCast("Implosion") && WoW.Talent(2) == 3 && WoW.IsSpellInRange("Doom"))
                    {
 //                       if (ImpsRemainingDuration <= GCD && WoW.PlayerHasBuff("DemonicSynergy"))

                        if (WoW.WildImpsCount == 1 && WoW.PlayerHasBuff("DemonicSynergy") && ImpsRemainingDuration >= GCD)
                        {
                            WoW.CastSpell("Implosion");
                            return;
                        }
                    }

                    //Shadowflame (if talent)
                    if (WoW.CanCast("Shadowflame") && WoW.Talent(1)==2 && WoW.IsSpellInRange("Doom") && WoW.TargetHasDebuff("Shadowflame") &&
                        WoW.TargetDebuffTimeRemaining("Shadowflame") < TwoSecondCast + 2)
                    {
                        WoW.CastSpell("Shadowflame");
                        return;
                    }

                    //Service Pet (if talent)
                    if (WoW.CanCast("GrimoireFelguard") && UseCooldowns && WoW.Talent(6)==2 && WoW.IsSpellInRange("Doom") && WoW.CurrentSoulShards >= 1 && UseCooldowns)
                    {
                        WoW.CastSpell("GrimoireFelguard");
                        empowered = false;
                        return;
                    }

                    // Doomguard
                    if (WoW.CanCast("Doomguard") && WoW.CurrentSoulShards >= 1 && UseCooldowns && WoW.IsSpellInRange("Doom"))
                    {
                        WoW.CastSpell("Doomguard");
                        empowered = false;
                        return;
                    }

                    //Felstorm
                    if (WoW.CanCast("Felstorm") && WoW.HasPet && WoW.PetHasBuff("DemonicEmpowerment"))
                    {
                        WoW.CastSpell("Felstorm");
                        return;
                    }

                    //Call Dreadstalkers (if NOT talent Summon Darkglare)
                    if (WoW.CanCast("CallDreadstalkers") && WoW.Talent (7)!=1 && (WoW.CurrentSoulShards >= 2 || WoW.PlayerHasBuff("DemonicCalling")) && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("CallDreadstalkers");
                        empowered = false;
                        DreadstalkersTime.Restart();
                        return;
                    }

                    //Hand Of Guldan (if NOT talent Summon Darkglare)
                    if (WoW.CanCast("HandOfGuldan") && WoW.Talent (7)!=1 && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 4 && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        empowered = false;
                        threeimps = false;
                        ImpTime.Restart();
                        return;
                    }

                    //Summon Darkglare (if talent)
                    if (WoW.CanCast("SummonDarkglare") && WoW.Talent (7)==1 && WoW.CurrentSoulShards >= 1 && WoW.TargetHasDebuff("Doom") && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        if (WoW.LastSpell == "HandOfGuldan" || WoW.LastSpell == "CallDreadstalkers")
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("1", Color.Red);
                            empowered = false;
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("CallDreadstalkers") > 5 && WoW.CurrentSoulShards < 3)
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("2", Color.Red);
                            empowered = false;
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("CallDreadstalkers") >= GCD && WoW.CurrentSoulShards >= 3)
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("3", Color.Red);
                            empowered = false;
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("CallDreadstalkers") >= GCD && WoW.CurrentSoulShards >= 1 && WoW.PlayerHasBuff("DemonicCalling"))
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("4", Color.Red);
                            empowered = false;
                            return;
                        }
                    }

                    //Call Dreadstalkers (if talent Summon Darkglare)
                    if (WoW.CanCast("CallDreadstalkers") && WoW.Talent (7)==1 && (WoW.CurrentSoulShards >= 2 || WoW.PlayerHasBuff("DemonicCalling")) && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        if (WoW.SpellCooldownTimeRemaining("SummonDarkglare") <= TwoSecondCast && WoW.CurrentSoulShards >= 3)
                        {
                            WoW.CastSpell("CallDreadstalkers");
                            empowered = false;
                            DreadstalkersTime.Restart();
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("SummonDarkglare") <= TwoSecondCast && WoW.CurrentSoulShards >= 1 && WoW.PlayerHasBuff("DemonicCalling"))
                        {
                            WoW.CastSpell("CallDreadstalkers");
                            empowered = false;
                            DreadstalkersTime.Restart();
                            return;
                        }
                    }

                    //Hand Of Guldan
                    if (WoW.CanCast("HandOfGuldan") && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 3 && WoW.LastSpell == "CallDreadstalkers" && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        empowered = false;
                        threeimps = true;
                        ImpTime.Restart();
                        return;
                    }

                    //Hand Of Guldan (if talent Summon Darkglare)
                    if (WoW.CanCast("HandOfGuldan") && WoW.Talent (7)==1 && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 5 &&
                        WoW.SpellCooldownTimeRemaining("SummonDarkglare") <= OneFiveCast && WoW.IsSpellInRange("Doom") && (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        threeimps = false;
                        empowered = false;
                        ImpTime.Restart();
                        return;
                    }

                    //Hand Of Guldan (if talent Summon Darkglare)
                    if (WoW.CanCast("HandOfGuldan") && WoW.Talent (7)==1 && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 4 && WoW.SpellCooldownTimeRemaining("SummonDarkglare") > 2 &&
                        WoW.IsSpellInRange("Doom") && (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        threeimps = false;
                        empowered = false;
                        ImpTime.Restart();
                        return;
                    }

                    //Demonic Empowerment (if last spell was Hand Of Guldan)
                    if (WoW.CanCast("DemonicEmpowerment") && WoW.LastSpell == "HandOfGuldan" && WoW.LastSpell != "DemonicEmpowerment" && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("DemonicEmpowerment");
                        empowered = true;
                        return;
                    }

                    //Demonic Empowerment
                    if (WoW.CanCast("DemonicEmpowerment") && (!empowered || !WoW.PetHasBuff("DemonicEmpowerment")) && WoW.LastSpell != "DemonicEmpowerment" && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("DemonicEmpowerment");
                        empowered = true;
                        return;
                    }
/*
					//Doom (if NOT talent Hand of Doom)
					if(WoW.CanCast("Doom") && (!WoW.TargetHasDebuff("Doom") || WoW.TargetDebuffTimeRemaining("Doom") < 5) && WoW.IsSpellInRange("Doom"))
					{
						WoW.CastSpell("Doom");
						return;
					}					

					//Soul Harvest
					if(WoW.CanCast("SoulHarvest") && WoW.IsSpellInRange("Doom"))
					{
						WoW.CastSpell("SoulHarvest");
						return;
					}
*/
                    //Shadowflame (if talent)
                    if (WoW.CanCast("Shadowflame") && WoW.Talent(1)==2 && WoW.IsSpellInRange("Doom") && WoW.PlayerSpellCharges("Shadowflame") == 2)
                    {
                        WoW.CastSpell("Shadowflame");
                        return;
                    }

                    //Thal'kiel's Consumption
                   if (WoW.CanCast("TK") && WoW.DreadstalkersCount >= 1 && DreadstalkersRemainingDuration > TwoSecondCast && ((WoW.WildImpsCount >= 1 && !threeimps) || WoW.WildImpsCount >= 2) &&
                        ImpsRemainingDuration > TwoSecondCast && WoW.IsSpellInRange("Doom") && (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("TK");
                        return;
                    }


					
                    //Life Tap
                    if (WoW.CanCast("LifeTap") && WoW.Mana <= 30)
                    {
                        WoW.CastSpell("LifeTap");
                        return;
                    }

                    //Demonwrath (if moving)
                    if (WoW.CanCast("Demonwrath") && WoW.IsMoving && !WoW.PlayerHasBuff("Norgannon"))
                    {
                        WoW.CastSpell("Demonwrath");
                        return;
                    }

                    //Demonbolt (if talent)
                    if (WoW.CanCast("Demonbolt") && WoW.Talent(7)==2 && WoW.IsSpellInRange("Doom") && (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("Demonbolt");
                        return;
                    }

                    //Shadowbolt (if NOT Demonbolt talent)
                    if (WoW.CanCast("Shadowbolt") && WoW.Talent(7)!=2 && WoW.IsSpellInRange("Doom") && (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("Demonbolt");
                        return;
                    }

                    //Life Tap
                    if (WoW.CanCast("LifeTap") && WoW.Mana < 100)
                    {
                        WoW.CastSpell("LifeTap");
                        return;
                    }
                }
            }

            // AoE Rotation
            if (combatRoutine.Type == RotationType.AOE)
            {
                Log.Write("Imp Time: " + ImpsRemainingDuration, Color.DarkViolet);
                // AoE Rotation
                if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat && !WoW.PlayerIsCasting)
                {
					
					
                    //Implosion (if talent)
                    if (WoW.CanCast("Implosion") && WoW.Talent (2)==3 && WoW.IsSpellInRange("Doom"))
                    {
                        if (ImpsRemainingDuration <= GCD && WoW.PlayerHasBuff("DemonicSynergy"))

                        {
                            WoW.CastSpell("Implosion");
                            return;
                        }
                        if (WoW.LastSpell == "HandOfGuldan" && WoW.WildImpsCount == 1 && WoW.PlayerHasBuff("DemonicSynergy"))
                        {
                            WoW.CastSpell("Implosion");
                            return;
                        }
                        if (WoW.WildImpsCount == 1 && ImpsRemainingDuration <= GCD)
                        {
                            WoW.CastSpell("Implosion");
                            return;
                        }
  //                      if (WoW.LastSpell == "HandOfGuldan" && WoW.WildImpsCount == 1)
						if (WoW.WasLastCasted("HandOfGuldan") && WoW.WildImpsCount == 1)
                        {
                            WoW.CastSpell("Implosion");
                            return;
                        }
                    }

                    //Shadowflame (if talent)
                    if (WoW.CanCast("Shadowflame") && WoW.Talent(1)==2 && WoW.IsSpellInRange("Doom") && WoW.TargetHasDebuff("Shadowflame") &&
                        WoW.TargetDebuffTimeRemaining("Shadowflame") < TwoSecondCast + 2)
                    {
                        WoW.CastSpell("Shadowflame");
                        return;
                    }

                    //Felstorm
                    if (WoW.CanCast("Felstorm") && WoW.HasPet && WoW.PetHasBuff("DemonicEmpowerment"))
                    {
                        WoW.CastSpell("Felstorm");
                        return;
                    }

                    //Call Dreadstalkers (if NOT talent Summon Darkglare or Implosion)
                    if (WoW.CanCast("CallDreadstalkers") && WoW.Talent (7)!=1 && (WoW.CurrentSoulShards >= 2 || WoW.PlayerHasBuff("DemonicCalling")) && WoW.Talent (2)!=3 && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("CallDreadstalkers");
                        empowered = false;
                        DreadstalkersTime.Restart();
                        return;
                    }

                    //Hand Of Guldan (if NOT talent Summon Darkglare)
                    if (WoW.CanCast("HandOfGuldan") && WoW.Talent (7)!=1 && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 4 && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        empowered = false;
                        threeimps = false;
                        ImpTime.Restart();
                        return;
                    }

                    //Summon Darkglare (if talent)
                    if (WoW.CanCast("SummonDarkglare") && WoW.Talent (7)==1 && WoW.CurrentSoulShards >= 1 && WoW.TargetHasDebuff("Doom") && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        if (WoW.LastSpell == "HandOfGuldan" || WoW.LastSpell == "CallDreadstalkers")
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("1", Color.Red);
                            empowered = false;
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("CallDreadstalkers") > 500 && WoW.CurrentSoulShards < 3)
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("2", Color.Red);
                            empowered = false;
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("CallDreadstalkers") >= GCD && WoW.CurrentSoulShards >= 3)
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("3", Color.Red);
                            empowered = false;
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("CallDreadstalkers") >= GCD && WoW.CurrentSoulShards >= 1 && WoW.PlayerHasBuff("DemonicCalling"))
                        {
                            WoW.CastSpell("SummonDarkglare");
                            Log.Write("4", Color.Red);
                            empowered = false;
                            return;
                        }
                    }

                    //Call Dreadstalkers (if talent Summon Darkglare and not Implosion)
                    if (WoW.CanCast("CallDreadstalkers") && WoW.Talent (7)==1 && (WoW.CurrentSoulShards >= 2 || WoW.PlayerHasBuff("DemonicCalling")) && WoW.Talent (2)!=3 && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        if (WoW.SpellCooldownTimeRemaining("SummonDarkglare") <= TwoSecondCast && WoW.CurrentSoulShards >= 3)
                        {
                            WoW.CastSpell("CallDreadstalkers");
                            empowered = false;
                            DreadstalkersTime.Restart();
                            return;
                        }
                        if (WoW.SpellCooldownTimeRemaining("SummonDarkglare") <= TwoSecondCast && WoW.CurrentSoulShards >= 1 && WoW.PlayerHasBuff("DemonicCalling"))
                        {
                            WoW.CastSpell("CallDreadstalkers");
                            empowered = false;
                            DreadstalkersTime.Restart();
                            return;
                        }
                    }

                    //Hand Of Guldan
                    if (WoW.CanCast("HandOfGuldan") && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 3 && WoW.LastSpell == "CallDreadstalkers" && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        empowered = false;
                        threeimps = true;
                        ImpTime.Restart();
                        return;
                    }

                    //Hand Of Guldan (if talent Summon Darkglare)
                    if (WoW.CanCast("HandOfGuldan") && WoW.Talent (7)==1 && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 5 &&
                        WoW.SpellCooldownTimeRemaining("SummonDarkglare") <= OneFiveCast && WoW.IsSpellInRange("Doom") && (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        threeimps = false;
                        empowered = false;
                        ImpTime.Restart();
                        return;
                    }

                    //Hand Of Guldan (if talent Summon Darkglare)
                    if (WoW.CanCast("HandOfGuldan") && WoW.Talent (7)==1 && WoW.LastSpell != "HandOfGuldan" && WoW.CurrentSoulShards >= 4 && WoW.SpellCooldownTimeRemaining("SummonDarkglare") > 2 &&
                        WoW.IsSpellInRange("Doom") && (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("HandOfGuldan");
                        threeimps = false;
                        empowered = false;
                        ImpTime.Restart();
                        return;
                    }

                    //Demonic Empowerment (if last spell was Hand Of Guldan)
                    if (WoW.CanCast("DemonicEmpowerment") && WoW.LastSpell == "HandOfGuldan" && WoW.LastSpell != "DemonicEmpowerment" && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("DemonicEmpowerment");
                        empowered = true;
                        return;
                    }

                    //Demonic Empowerment
                    if (WoW.CanCast("DemonicEmpowerment") && (!empowered || !WoW.PetHasBuff("DemonicEmpowerment")) && WoW.LastSpell != "DemonicEmpowerment" && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("DemonicEmpowerment");
                        empowered = true;
                        return;
                    }
/*
					//Doom (if NOT talent Hand of Doom)
					if(WoW.CanCast("Doom") && (!WoW.TargetHasDebuff("Doom") || WoW.TargetDebuffTimeRemaining("Doom") < 5) && WoW.IsSpellInRange("Doom"))
					{
						WoW.CastSpell("Doom");
						return;
					}					

					//Soul Harvest
					if(WoW.CanCast("SoulHarvest") && WoW.IsSpellInRange("Doom"))
					{
						WoW.CastSpell("SoulHarvest");
						return;
					}
*/
                    //Shadowflame (if talent)
                    if (WoW.CanCast("Shadowflame") && WoW.Talent(1)==2 && WoW.IsSpellInRange("Doom") && WoW.PlayerSpellCharges("Shadowflame") == 2)
                    {
                        WoW.CastSpell("Shadowflame");
                        return;
                    }

                    //Thal'kiel's Consumption
                   if (WoW.CanCast("TK") && ((WoW.DreadstalkersCount >= 1 && DreadstalkersRemainingDuration > TwoSecondCast) || WoW.Talent (2)==3) &&
                        ((WoW.WildImpsCount >= 1 && !threeimps) || WoW.WildImpsCount >= 2) && ImpsRemainingDuration > TwoSecondCast && WoW.IsSpellInRange("Doom") &&
                        (!WoW.IsMoving || WoW.PlayerHasBuff("Norgannon")))
                    {
                        WoW.CastSpell("TK");
                        return;
                    }


                    //Life Tap
                    if (WoW.CanCast("LifeTap") && WoW.Mana <= 30)
                    {
                        WoW.CastSpell("LifeTap");
                        return;
                    }

                    //Demonwrath
                    if (WoW.CanCast("Demonwrath") && !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("Demonwrath");
                        return;
                    }

                    //Life Tap
                    if (WoW.CanCast("LifeTap") && WoW.Mana < 100 && !WoW.PlayerIsChanneling)
                    {
                        WoW.CastSpell("LifeTap");
                    }
                }
            }
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Vectarius
AddonName=myspellpriority
WoWVersion=Legion - 70200
[SpellBook.db]
Spell,157695,Demonbolt,F6
Spell,686,Shadowbolt,F6
Spell,193440,Demonwrath,F12
Spell,1454,LifeTap,R
Spell,205181,Shadowflame,F5
Spell,603,Doom,F4
Spell,105174,HandOfGuldan,F1
Spell,104316,CallDreadstalkers,F2
Spell,193396,DemonicEmpowerment,Q
Spell,196277,Implosion,C
Spell,119914,Felstorm,F
Spell,211714,TK,F3
Spell,5512,Healthstone,F10
Spell,18540,Doomguard,T
Spell,111898,GrimoireFelguard,E
Spell,196098,SoulHarvest,F11
Spell,205180,SummonDarkglare,F7
Aura,603,Doom
Aura,205181,Shadowflame
Aura,205146,DemonicCalling
Aura,193396,DemonicEmpowerment
Aura,171982,DemonicSynergy
Aura,193440,Demonwrath
Aura,208215,Norgannon
*/