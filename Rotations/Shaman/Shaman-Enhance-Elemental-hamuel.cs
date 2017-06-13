/* * #ShowToolTip   /cast [@cursor] Earthquake *  * #ShowToolTip/cast [@cursor] Liquid Magma Totem
 * 
 * 
 * 
 */
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
    public struct totem_data
    {
        public bool Active;
        public int Duration;
        public totem_data(bool p1, int p2)
        {
            Active = p1;
            Duration = p2;
        }

    }
    public struct Tank_data
    {
        public bool Active;
        public int Hp;
        public int Location;
        public bool Riptide;
        public double Dur;
        public bool Range;
        public Tank_data(bool p1, int p2, int p3, bool p4, double p5, bool p6)
        {
            Active = p1;
            Hp = p2;
            Location = p3;
            Riptide = p4;
            Dur = p5;
            Range = p6;
        }
    }
    public struct Lowest_data
    {
        public int Hp;
        public int Location;
        public bool Riptide;
        public double Dur;
        public bool Range;
        public Lowest_data(int p2, int p3, bool p4, double p5, bool p6)
        {
            Hp = p2;
            Location = p3;
            Riptide = p4;
            Dur = p5;
            Range = p6;

        }
    }
    public struct Below_data
    {
        public int Chain;
        public int Tide;
        public int Stream;
        public int Link;
        public Below_data(int p1, int p2, int p3, int p4)
        {
            Chain = p1;
            Tide = p2;
            Stream = p3;
            Link = p4;
        }
    }
    public struct Rip_data
    {
        public bool Active;
        public int Location;
        public int Count;
        public double Dur;

        public Rip_data(bool p1, int p2, int p3, double p4)
        {
            Active = p1;
            Location = p2;
            Count = p3;
            Dur = p4;
        }
    }
    public struct Party_data
    {
        public bool Range;
        public int Roll;
        public int Hp;
        public bool Riptide;
        public double Dur;

        public Party_data(bool p1, int p2, int p3, bool p4, double p5)
        {
            Range = p1;
            Roll = p2;
            Hp = p3;
            Riptide = p4;
            Dur = p5;
        }
    }
    public class Enhancement : CombatRoutine
    {
        public override int CLEAVE { get { return 2; } } //please Set between 1-99 NpC in range for AOE  if not desired set to 99
        public override int AOE { get { return 3; } }//please Set between 1-99 NpC in range for Cleave if not desired set to 99
        public override int SINGLE { get { return 1; } }//please Set between 1-99 NpC in range for ST if not desired set to 99
        private static double Revision = 2.3;
        private static int EnhLowHp = 40;
        private static int interMin = 50;
        private static int interMax = 90;
        private static int TankNum = 5, RipNum = 4;
        private static int RiptidePct = 90;
        private static int Chainheal = 50, HTide = 60, HStream = 90, SLink = 25;
        private static int ChainCnt = 3, HTideCnt = 3, HStreamCnt = 1, SLinkCnt = 2;
        private static int HSurge = 75, HWave = 99, TankLow = 15;
        private static int LocationPlayer = 31;
        public Stopwatch Healstream = new Stopwatch();
        private Rip_data[] PartyRip = new Rip_data[RipNum];
        private Tank_data[] TanksInfo = new Tank_data[TankNum];
        private Party_data[] PartyInfo = new Party_data[31];
        private Lowest_data LowInfo = new Lowest_data();
        public Below_data Below = new Below_data();
        private totem_data Totem = new totem_data();
        public static int Alt = 1, Ctrl = 2, Shift = 3, None = 4;
        private static DataTable dtColorHelper;
        private static readonly Random getrandom = new Random();
        public Stopwatch Crash = new Stopwatch();
        public Stopwatch Pets = new Stopwatch();
        public Stopwatch Rotation = new Stopwatch();

        public override string Name
        {
            get { return "Enhancement Hamuel:SimC"; }
        }
        public override string Class
        {
            get { return "Shaman"; }
        }
        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
            Log.Write("Welcome to Enhancement Shaman by Hamuel", Color.Green);
            Log.Write("Detect talents/race/spec/ST/AOE/Interrupt", Color.Green);
            Log.Write("version " + Revision, Color.Green);
            dtColorHelper = new DataTable();
            dtColorHelper.Columns.Add("Percent");
            dtColorHelper.Columns.Add("Unrounded");
            dtColorHelper.Columns.Add("Rounded");
            dtColorHelper.Columns.Add("Value");
            for (var i = 0; i <= 99; i++)
            {
                var drNew = dtColorHelper.NewRow();
                drNew["Percent"] = i < 10 ? "0.0" + i : "0." + i;
                drNew["Unrounded"] = double.Parse(drNew["Percent"].ToString()) * 255;
                drNew["Rounded"] = Math.Round(double.Parse(drNew["Percent"].ToString()) * 255, 0);
                drNew["Value"] = i;
                dtColorHelper.Rows.Add(drNew);
            }
            {
                var drNew = dtColorHelper.NewRow();
                drNew["Percent"] = "1.00";
                drNew["Unrounded"] = "255";
                drNew["Rounded"] = "255";
                drNew["Value"] = 100;
                dtColorHelper.Rows.Add(drNew);
            }
            {
                var drNew = dtColorHelper.NewRow();
                drNew["Percent"] = "1.00";
                drNew["Unrounded"] = "255";
                drNew["Rounded"] = "77"; // Manually added from testing this color sometimes shows up 
                drNew["Value"] = 30;
                dtColorHelper.Rows.Add(drNew);
            }
            {
                var drNew = dtColorHelper.NewRow();
                drNew["Percent"] = "1.00";
                drNew["Unrounded"] = "255";
                drNew["Rounded"] = "179"; // Manually added from testing this color sometimes shows up 
                drNew["Value"] = 70;
                dtColorHelper.Rows.Add(drNew);
            }
        }
        private async void AsyncPulse()
        {
            Task[] tasks = new Task[3];
            tasks[0] = Healing_Update();
            tasks[1] = Who_Low();
            tasks[2] = TotemInfo();
            await Task.WhenAll(tasks);
        }
        private async Task Healing_Update()
        {
            await Task.Run(() =>
            {
                Color pixelColor = Color.FromArgb(0);
                for (int i = 1; i <= RaidSize; i++)
                {
                    if (RaidSize >= 21 && i >= 21)
                        pixelColor = WoW.GetBlockColor(i - 20, 23);
                    else
                        pixelColor = WoW.GetBlockColor(i, 22);

                    PartyInfo[i].Hp = Convert.ToInt32(pixelColor.R) * 100 / 255;
                    PartyInfo[i].Roll = Convert.ToInt32(pixelColor.B) * 100 / 255;
                    if (Convert.ToInt32(pixelColor.G) / 255 == 1)
                        PartyInfo[i].Range = true;
                    else
                        PartyInfo[i].Range = false;
                    //Log.Write("Party Member" + i + "hp :" + PartyInfo[i].Hp);
                }
            });
        }
        private async Task Who_Low()
        {
            await Task.Run(() =>
            {
                LowInfo.Hp = 100;
                LowInfo.Location = 1;
                LowInfo.Riptide = false;
                LowInfo.Dur = 0.0;
                LowInfo.Range = false;
                Below.Chain = 0;
                Below.Stream = 0;
                Below.Link = 0;
                Below.Tide = 0;

                for (int i = 1; i < TankNum; i++)
                {
                    TanksInfo[i].Hp = 0;
                    TanksInfo[i].Location = 0;
                    TanksInfo[i].Active = false;
                    TanksInfo[i].Riptide = false;
                    TanksInfo[i].Range = false;

                }
                for (int i = 1; i <= RaidSize; i++)
                {
                    for (int a = 1; a < TankNum; a++)
                        if (PartyInfo[i].Range && PartyInfo[i].Roll == 1 && !TanksInfo[a].Active)
                        {
                            TanksInfo[a].Hp = PartyInfo[i].Hp;
                            TanksInfo[a].Location = i;
                            TanksInfo[a].Active = true;
                            TanksInfo[a].Range = true;
                            for (int b = 1; b < RipNum; b++)
                                if (PartyRip[b].Location == i)
                                {
                                    TanksInfo[a].Riptide = true;
                                    TanksInfo[a].Dur = PartyRip[b].Dur;
                                }

                        }

                    if (LowInfo.Hp >= PartyInfo[i].Hp && PartyInfo[i].Hp != 0 && PartyInfo[i].Range)
                    {
                        LowInfo.Location = i;
                        LowInfo.Hp = PartyInfo[i].Hp;
                        LowInfo.Range = true;
                        for (int b = 1; b < RipNum; b++)
                            if (PartyRip[b].Location == i)
                            {
                                LowInfo.Riptide = true;
                                LowInfo.Dur = PartyRip[b].Dur;
                            }

                    }
                    if (PartyInfo[i].Hp < Chainheal)
                        Below.Chain++;
                    if (PartyInfo[i].Hp < SLink)
                        Below.Link++;
                    if (PartyInfo[i].Hp < HStream)
                        Below.Stream++;
                    if (PartyInfo[i].Hp < HTide)
                        Below.Tide++;
                }
            });

        }



        private async Task TotemInfo()
        {
            await Task.Run(() =>
            {
                Color pixelColor = Color.FromArgb(0);
                pixelColor = WoW.GetBlockColor(7, 24);
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) / 255)) == 1)
                {
                    Totem.Active = true;
                    Totem.Duration = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) * 100 / 255));
                }
                else
                {
                    Totem.Active = false;
                    Totem.Duration = 0;
                }
                //Log.Write("Totems active : " + Totem.Active +" Duration :" +Totem.Duration);
            });
        }

        private void Totems()
        {
            if (WoW.CanCast("Healing Stream Totem") && WoW.PlayerSpellCharges("Healing Stream Totem") >= 2 && !WoW.IsSpellOnCooldown("Healing Stream Totem")) //drop healing stream if in combat
            {
                WoW.CastSpell("Healing Stream Totem");
                Healstream.Start();
                return;
            }
            if (WoW.CanCast("Healing Stream Totem") && !WoW.IsSpellOnCooldown("Healing Stream Totem") && WoW.PlayerSpellCharges("Healing Stream Totem") == 1 && (WoW.HealthPercent < 75 || Below.Stream > HStreamCnt) && (Healstream.Elapsed.Seconds >= 14 || Healstream.Elapsed.Seconds == 0)) //drop healing stream if in combat
            {
                Healstream.Start();
                WoW.CastSpell("Healing Stream Totem");
                return;
            }

            if (WoW.Talent(6) == 2 && WoW.CanCast("Cloudburst Totem") && !WoW.PlayerIsChanneling) //drop cloudburst in combat
            {
                WoW.CastSpell("Cloudburst Totem");
                return;
            }
        }
        private void CastByLocation(int location, int mod)
        {

            switch (location)
            {
                case 1:
                    WoW.CastSpell("raid1");
                    break;
                case 2:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid1");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid1");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    break;
                case 4:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.CastSpell("raid1");
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 5:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid1");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 6:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid1");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 7:
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid1");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    break;
                case 8:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid1");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;

                case 9:
                    WoW.CastSpell("raid2");
                    break;
                case 10:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid2");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 11:
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid2");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    break;
                case 12:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.CastSpell("raid2");
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 13:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid2");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 14:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid2");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 15:
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid2");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    break;
                case 16:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid2");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 17:
                    WoW.CastSpell("raid3");
                    break;
                case 18:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid3");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 19:
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid3");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    break;
                case 20:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.CastSpell("raid3");
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 21:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid3");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 22:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid3");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 23:
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid3");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    break;
                case 24:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid3");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 25:
                    WoW.CastSpell("raid4");
                    break;
                case 26:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid4");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 27:
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid4");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    break;
                case 28:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.CastSpell("raid4");
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                case 29:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LControlKey);
                    WoW.CastSpell("raid4");
                    WoW.KeyUp(WoW.Keys.LControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 30:
                    WoW.KeyDown(WoW.Keys.LShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.CastSpell("raid4");
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.LShiftKey);
                    break;
                default:
                    break;
            }


        }
        private void Riptide_Update()
        {
            Color pixelColor = Color.FromArgb(0);
            for (int i = 0; i < RipNum; i++)
            {
                pixelColor = WoW.GetBlockColor(12 + i, 23);
                if ((Convert.ToDouble(pixelColor.R) * 100 / 255) <= 30)
                {
                    PartyRip[i].Active = true;
                    PartyRip[i].Location = Convert.ToInt32(pixelColor.R) * 100 / 255;
                    PartyRip[i].Count = Convert.ToInt32(pixelColor.G) * 100 / 255;
                    PartyRip[i].Dur = Convert.ToDouble(pixelColor.B) * 100 / 255;
                }
                else
                {
                    PartyRip[i].Active = true;
                    PartyRip[i].Location = 0;
                    PartyRip[i].Count = 0;
                    PartyRip[i].Dur = 0;
                }

            }
        }
        private void RipTideCast()
        {
            if (!WoW.IsSpellOnCooldown("Riptide"))
            {
                if (WoW.HealthPercent + TankLow < RiptidePct && (WoW.PlayerHasBuff("Riptide") || WoW.PlayerBuffTimeRemaining("Riptide") < 420))
                {
                    CastByLocation(LocationPlayer, Alt);
                    WoW.CastSpell("Riptide");
                    return;
                }


                for (int b = 0; b < TankNum; b++)
                {
                    if (TanksInfo[b].Active && TanksInfo[b].Hp < RiptidePct && (TanksInfo[b].Riptide && TanksInfo[b].Dur < 420))
                    {
                        Log.Write("Player Cast riptide On tank" + b);
                        CastByLocation(TanksInfo[b].Location, Alt);
                        WoW.CastSpell("Riptide");
                        return;
                    }

                }
                if (!LowInfo.Riptide && LowInfo.Range || LowInfo.Range && (LowInfo.Dur < 4.2 && LowInfo.Dur == 0))
                {
                    Log.Write("Player Cast riptide On Lowest" + LowInfo.Location);
                    CastByLocation(LowInfo.Location, Alt);
                    WoW.CastSpell("Riptide");
                    return;
                }
            }
        }

        private void PartyHealing()
        {
            if (!WoW.IsSpellOnCooldown("Healing Surge"))
            {
                if (WoW.HealthPercent < HSurge && Below.Chain >= ChainCnt)
                {
                    CastByLocation(LocationPlayer, None);
                    WoW.CastSpell("Healing Surge");
                    return;
                }

                for (int b = 0; b < TankNum; b++)
                {
                    if (TanksInfo[b].Active && TanksInfo[b].Hp < HSurge && ((LowInfo.Hp + TankLow) < TanksInfo[b].Hp))
                    {
                        Log.Write("Player Cast Surge On Tank" + LowInfo.Location);
                        CastByLocation(TanksInfo[b].Location, Ctrl);
                        WoW.CastSpell("Healing Surge");
                        return;
                    }

                }
                if (LowInfo.Range && LowInfo.Hp < HSurge)
                {
                    Log.Write("Player Cast Surge On Lowest" + LowInfo.Location);
                    CastByLocation(LowInfo.Location, Ctrl);
                    WoW.CastSpell("Healing Surge");
                    return;
                }
            }


            if (!WoW.IsSpellOnCooldown("Chain Heal"))
            {
                if (WoW.HealthPercent + TankLow < HWave && Below.Chain >= ChainCnt)
                {
                    CastByLocation(LocationPlayer, Ctrl);
                    WoW.CastSpell("Chain Heal");
                    return;
                }

                for (int b = 0; b < TankNum; b++)
                {
                    if (TanksInfo[b].Active && TanksInfo[b].Hp < HWave && Below.Chain >= ChainCnt && ((LowInfo.Hp + TankLow) < TanksInfo[b].Hp))
                    {
                        Log.Write("Player Cast Chain On Tank");
                        CastByLocation(TanksInfo[b].Location, Ctrl);
                        WoW.CastSpell("Chain Heal");
                        return;
                    }

                }

                if (LowInfo.Range && LowInfo.Hp < HWave && Below.Chain >= ChainCnt)
                {
                    Log.Write("Player Cast Wave On Lowest" + LowInfo.Location);
                    CastByLocation(LowInfo.Location, Ctrl);
                    WoW.CastSpell("Chain Heal");
                    return;
                }
            }



            if (!WoW.IsSpellOnCooldown("Healing Wave"))
            {
                if (WoW.HealthPercent + TankLow <= HWave)
                {
                    CastByLocation(LocationPlayer, None);
                    WoW.CastSpell("Healing Wave");
                    return;
                }
                for (int b = 0; b < TankNum; b++)
                {
                    if (TanksInfo[b].Active && TanksInfo[b].Hp < HWave && ((LowInfo.Hp + TankLow) < TanksInfo[b].Hp))
                    {
                        Log.Write("Player Cast Wave On Tank");
                        CastByLocation(TanksInfo[b].Location, None);
                        WoW.CastSpell("Healing Wave");

                        return;
                    }

                }
                if (LowInfo.Range && LowInfo.Hp < HWave)
                {
                    Log.Write("Player Cast Wave On Lowest" + LowInfo.Location);
                    CastByLocation(LowInfo.Location, None);
                    WoW.CastSpell("Healing Wave");
                    return;
                }
            }

            if (WoW.CanCast("Earthen Shield Totem") && (WoW.TargetHealthPercent >= 40 | WoW.TargetHealthPercent <= 70) && !WoW.IsSpellOnCooldown("Earthen Shield Totem"))
            {
                WoW.CastSpell("Earthen Shield Totem");
                return;
            }

        }
        private void RestoDMG()
        {
            if (WoW.CanCast("Flame Shock") && !WoW.IsSpellOnCooldown("Flame Shock") && (!WoW.PlayerHasBuff("Flame Shock") || WoW.TargetDebuffTimeRemaining("Flame Shock") <= GCD)) //interupt every spell, not a boss.
            {
                WoW.CastSpell("Flame Shock");
                return;
            }
            if (WoW.CanCast("Lava Burst") && !WoW.IsSpellOnCooldown("Lava Burst") && (WoW.PlayerHasBuff("Flame Shock") || WoW.TargetDebuffTimeRemaining("Flame Shock") <= GCD)) //interupt every spell, not a boss.
            {
                WoW.CastSpell("Lava Burst");
                return;
            }
            if (WoW.CanCast("Lightning Bolt") && !WoW.IsSpellOnCooldown("Lightning Bolt") && (WoW.PlayerHasBuff("Flame Shock") || WoW.TargetDebuffTimeRemaining("Flame Shock") <= GCD)) //interupt every spell, not a boss.
            {
                WoW.CastSpell("Lightning Bolt");
                return;
            }

        }

        private async Task RestoRotation()
        {
            await Task.Run(() =>
            {
                if (WoW.PlayerSpec == "Restoration") // Do Single Target Stuff here
                {

                    if (WoW.IsInCombat && !WoW.PlayerIsChanneling) //if in combat then...
                        Totems();
                    if (WoW.IsInCombat && WoW.HasTarget && WoW.TargetIsFriend && !WoW.PlayerIsChanneling && !WoW.PlayerIsCasting)
                    {
                        RipTideCast();
                        PartyHealing();
                    }

                    if (WoW.IsInCombat && WoW.HasTarget && WoW.TargetIsEnemy)
                        RestoDMG();

                }
            });
        }

        private float GCD
        {
            get
            {
                if (Convert.ToSingle(10/ (1 + (WoW.HastePercent / 100f)))> 75f)
                {
                    return Convert.ToSingle(150f / (1 + (WoW.HastePercent / 100f)));
                }
                else
                {
                    return 75f;
                }
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
                if (WoW.PlayerRace == "BloodElf" && WoW.CanCast("Arcane Torrent", true, true, false, false, true) && !WoW.IsSpellOnCooldown("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Stormstrike")) //interupt every spell, not a boss.
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
        }

        private void DBMPrePull()
        {
            if (dbmOn && dbmTimer <= 18 && dbmTimer > 0 && WoW.HasTarget)
            {
                if (!WoW.ItemOnCooldown("Prolonged Power"))
                {
                    WoW.CastSpell("Prolonged Power");
                    return;
                }
                if (WoW.CanCast("Feral Spirit", true, true, false, false, true)) //feral spirit on boss - normally cast manually
                {
                    Pets.Start();
                    Log.Write("Using Feral Spirit", Color.Red);
                    WoW.CastSpell("Feral Spirit");
                    return;
                }
                if (WoW.Talent(2) == 2 && WoW.CanCast("Feral Lunge", true, true, false, false, true) && dbmTimer < 10)
                {
                    WoW.CastSpell("Feral Lunge");
                    return;
                }
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
        private void Stuns()
        {
            if (!WoW.PlayerIsCasting)
            {
                if (WoW.CanCast("Lightning Surge") && !WoW.IsSpellOnCooldown("Lightning Surge") && WoW.Talent(3) == 1)
                {
                    WoW.CastSpell("Lightning Surge");
                    return;
                }
                if (WoW.PlayerRace == "Tauren​" && !WoW.IsMoving && WoW.CanCast("War Stomp") && !WoW.IsSpellOnCooldown("War Stomp") && (WoW.Talent(3) != 1 || (WoW.IsSpellOnCooldown("Lightning Surge") && WoW.Talent(3) == 1)))
                {
                    WoW.CastSpell("War Stomp");
                    return;
                }
            }
        }
        private void DPSRacial()
        {
            if (!WoW.PlayerIsCasting)
            {
                // actions +=/ berserking,if= buff.ascendance.up | !talent.ascendance.enabled | level < 100
                // actions +=/ blood_fury
                if (WoW.PlayerRace == "Troll" && WoW.CanCast("Berserking") && !WoW.IsSpellOnCooldown("Berserking") && (WoW.Talent(7) != 1 || WoW.PlayerHasBuff("Ascendance")))
                {
                    WoW.CastSpell("Berserking");
                    return;
                }
                // actions +=/ blood_fury,if= !talent.ascendance.enabled | buff.ascendance.up | cooldown.ascendance.remains > 50
                if (WoW.PlayerRace == "Orc" && WoW.CanCast("Blood Fury") && !WoW.IsSpellOnCooldown("Blood Fury")
               && (WoW.Talent(7) != 1 || WoW.PlayerHasBuff("Ascendance") | WoW.SpellCooldownTimeRemaining("Ascendance") > 5000))
                {
                    WoW.CastSpell("Blood Fury");
                    return;
                }
            }

        }
        //actions +=/ potion,name = prolonged_power,if= feral_spirit.remains > 5 | target.time_to_die <= 60
        private void UsePotion()
        {
            if (Pets.Elapsed.Seconds < 5 && !WoW.ItemOnCooldown("Prolonged Power"))
            {
                WoW.CastSpell("Prolonged Power");
                return;
            }
        }
        public override void Stop()
        {
        }
        private void TimerReset()
        {

            if (Healstream.Elapsed.Seconds > 15)
            {
                Healstream.Reset();
            }
            if (Pets.Elapsed.Seconds >= 15)
            {
                Pets.Reset();
                Crash.Reset();
            }
        }

        /// <summary>
        /// elemental rotation stuff
        /// </summary>
        /// <returns></returns>
        private async Task ElementalConsistantUse()
        {
            if (WoW.PlayerSpec == "Elemental")
            {
                //Tailwind Totem,210659,
                //  actions +=/ totem_mastery,if= buff.resonance_totem.remains < 2
                //Log.Write("Totem Mastery :" + WoW.WildImpsCount);
                /*if (WoW.WildImpsCount == 0 && WoW.CanCast("Totem Mastery", true, true, false, false, true))
                {
                    WoW.CastSpell("Totem Mastery");
                    return;
                }*/
                if (WoW.Talent(1) == 3 && WoW.CanCast("Totem Mastery") && (!Totem.Active || Totem.Duration < 2))
                {
                    WoW.CastSpell("Totem Mastery");
                    return;
                }
                //actions +=/ fire_elemental
                if (WoW.Talent(6) != 2 && WoW.CanCast("Fire Elemental", true, true, false, false, true))
                {
                    WoW.CastSpell("Fire Elemental");
                    return;
                }
                //ations +=/ storm_elemental
                if (WoW.Talent(6) == 2 && WoW.CanCast("Storm Elemental", true, true, false, false, true))
                {
                    WoW.CastSpell("Storm Elemental");
                    return;
                }
                //
                if (WoW.Talent(4) == 3 && WoW.CanCast("Elemental Mastery", true, true, false, false, true))
                {
                    WoW.CastSpell("Elemental Mastery");
                    return;
                }
                Elemental_AOE();
                Task[] RotationType = new Task[3];
                RotationType[0] = Elemental_MeatBall();
                RotationType[1] = Elemental_Icefury();
                RotationType[2] = Elemental_LightRod();
                await Task.WhenAll(RotationType);

            }
        }
        private async Task Elemental_Icefury()
        {
            await Task.Run(() =>
            {

                if (WoW.Talent(7) == 3)
                {
                    //Log.Write("Frost Shock :  " + WoW.IsSpellOnCooldown("Frost Shock"));
                    //var c = WoW.GetBlockColor(3, 4);
                    //Log.Write("Red : " + c.R * 100 / 255 + " Green : " + c.G * 100 / 255 + " Blue : " + c.B * 100 / 255);

                    //actions.single_if=flame_shock,if=!ticking
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true) && !WoW.TargetHasDebuff("Flame Shock"))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //actions.single_if+=/earthquake,if=buff.echoes_of_the_great_sundering.up&maelstrom>=86
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.PlayerHasBuff("Echoes of the Great")
                    && WoW.Maelstrom >= 86 && mouseTarget)
                    {
                        WoW.CastSpell("Earthquake");
                        return;
                    }
                    //actions.single_if+=/frost_shock,if=buff.Icefury.up&maelstrom>=86
                    if (WoW.CanCast("Frost Shock") && WoW.PlayerHasBuff("Icefury") && WoW.Maelstrom >= 86)
                    {
                        Log.Write("Frost shock 1");
                        WoW.CastSpell("Frost Shock");
                        return;
                    }
                    //actions.single_if+=/earth_shock,if=maelstrom>=92
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.Maelstrom >= 92)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                    //actions.single_if +=/ stormkeeper,if= raid_event.adds.count < 3 | raid_event.adds.in> 50
                    if (WoW.CanCast("Stormkeeper", true, true, false, false, true) && !WoW.IsMoving)

                    {
                        WoW.CastSpell("Stormkeeper");
                        return;
                    }

                    //actions.single_if+=/elemental_blast
                    if (WoW.CanCast("Elemental Blast", true, true, true, false, true) && WoW.Talent(5) == 3 && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Elemental Blast");
                        return;
                    }
                    //actions.single_if+=/Icefury,if=raid_event.movement.in<5|maelstrom<=76
                    if (WoW.CanCast("Icefury", true, true, true, false, true) && WoW.Talent(7) == 3 && WoW.Maelstrom <= 76 && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Icefury");
                        return;
                    }
                    //actions.single_if+=/liquid_magma_totem,if=raid_event.adds.count<3|raid_event.adds.in>50
                    if (WoW.CanCast("Liquid Magma", true, true, false, false, true) && WoW.Talent(6) == 1 && mouseTarget)
                    {
                        WoW.CastSpell("Liquid Magma");
                        return;
                    }
                    //actions.single_if+=/lightning_bolt,if=buff.power_of_the_maelstrom.up&buff.stormkeeper.up&spell_targets.chain_lightning<3
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && (WoW.PlayerHasBuff("Power of the Maelstrom")
                    && WoW.PlayerHasBuff("Stormkeeper")) && combatRoutine.Type != RotationType.AOE)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_if+=/lava_burst,if=dot.flame_shock.remains>cast_time&cooldown_react
                    if (WoW.CanCast("Lava Burst", true, true, true, false, true) && WoW.TargetDebuffTimeRemaining("Flame Shock") > 200f / (1 + (WoW.HastePercent / 100f)) && (!WoW.IsMoving || WoW.IsMoving && WoW.PlayerHasBuff("Lava Surge")))
                    {
                        WoW.CastSpell("Lava Burst");
                        return;
                    }
                    //actions.single_if+=/frost_shock,
                    //if=buff.Icefury.up&( (maelstrom>=20&raid_event.movement.in>buff.Icefury.remains)
                    //|buff.Icefury.remains<(1.5*spell_haste*buff.Icefury.stack+1))
                    //FIX
                    if (WoW.CanCast("Frost Shock", true, true, true, false, true)
                    && ((WoW.PlayerHasBuff("Icefury") && WoW.Maelstrom >= 20 && WoW.IsMoving) || WoW.PlayerHasBuff("Icefury") && WoW.PlayerBuffTimeRemaining("Icefury") < GCD * (WoW.PlayerSpellCharges("Icefury") + 2)))
                    {
                        Log.Write("Frost shock 2");
                        WoW.CastSpell("Frost Shock");
                        return;
                    }
                    //actions.single_if+=/flame_shock,if=maelstrom>=20&buff.elemental_focus.up,target_if=refreshable
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true) && WoW.Maelstrom >= 20 && WoW.TargetDebuffTimeRemaining("Flame Shock") < 450 && WoW.PlayerHasBuff("Elemental Focus"))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //actions.single_if+=/frost_shock,moving=1,if=buff.Icefury.up
                    if (WoW.CanCast("Frost Shock", true, true, true, false, true)
                    && (WoW.PlayerHasBuff("Icefury") && WoW.PlayerHasBuff("Icefury") && WoW.IsMoving))
                    {
                        Log.Write("Frost shock 3");
                        WoW.CastSpell("Frost Shock");
                        return;
                    }
                    //actions.single_if+=/earth_shock,if=maelstrom>=86
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true)
                     && WoW.Maelstrom >= 86)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                    //actions.single_if+=/totem_mastery,if=buff.resonance_totem.remains<10
                    //actions.single_asc+=/totem_mastery,if=buff.resonance_totem.remains<10|(buff.resonance_totem.remains<(buff.ascendance.duration+cooldown.ascendance.remains)&cooldown.ascendance.remains<15)
                    if (WoW.Talent(1) == 3 && WoW.CanCast("Totem Mastery") && (!Totem.Active || Totem.Duration < 10))
                    {
                        WoW.CastSpell("Totem Mastery");
                        return;
                    }
                    //actions.single_if+=/earthquake,if=buff.echoes_of_the_great_sundering.up
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.PlayerHasBuff("Echoes of the Great") && mouseTarget)
                    {
                        WoW.CastSpell("Earthquake");
                        return;
                    }
                    //actions.single_if+=/chain_lightning,if=active_enemies>1&spell_targets.chain_lightning>1
                    if (WoW.CanCast("Chain Lightning", true, true, true, false, true) && combatRoutine.Type != RotationType.SingleTarget
                     && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Chain Lightning");
                        return;
                    }
                    // actions.single_if +=/ lightning_bolt
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_if+=/flame_shock,moving=1,target_if=refreshable
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true)
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") < 450 || !WoW.TargetHasDebuff("Flame Shock")) && WoW.IsMoving)
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    if (WoW.CanCast("Earth Shock", true, true, false, false, true) && WoW.IsMoving && WoW.Maelstrom >= 10)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                }
            });
        }
        private async Task Elemental_LightRod()
        {
            await Task.Run(() =>
            {
                if (WoW.Talent(7) == 2)
                {
                    //actions.single_lr+=/flame_shock,if=maelstrom>=20
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true)
                    && !WoW.TargetHasDebuff("Flame Shock"))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //actions.single_lr+=/earthquake,if=buff.echoes_of_the_great_sundering.up&maelstrom>86
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.TargetHasDebuff("Echoes of the Great") && WoW.Maelstrom >= 86 && mouseTarget)
                    {
                        WoW.CastSpell("Earthquake");
                        return;
                    }
                    //actions.single_lr+=/earth_shock,if=maelstrom>=92
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.Maelstrom >= 92)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                    //actions.single_LR +=/ stormkeeper,if= raid_event.adds.count < 3 | raid_event.adds.in> 50
                    if (WoW.CanCast("Stormkeeper", true, true, false, false, true))
                    {
                        WoW.CastSpell("Stormkeeper");
                        return;
                    }
                    //actions.single_asc+=/elemental_blast
                    if (WoW.CanCast("Elemental Blast", true, true, true, false, true) && WoW.Talent(5) == 3 && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Elemental Blast");
                        return;
                    }
                    //actions.single_Lr+=/liquid_magma_totem,if=raid_event.adds.count<3|raid_event.adds.in>50
                    if (WoW.CanCast("Liquid Magma", true, true, false, false, true) && WoW.Talent(6) == 1 && mouseTarget)
                    {
                        WoW.CastSpell("Liquid Magma");
                        return;
                    }
                    //actions.single_lr+=/lava_burst,if=dot.flame_shock.remains>cast_time&cooldown_react
                    if (WoW.CanCast("Lava Burst", true, true, true, false, true) && !WoW.IsMoving
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") > 200f / (1 + (WoW.HastePercent / 100f))))
                    {
                        WoW.CastSpell("Lava Burst");
                        return;
                    }
                    //actions.single_Lr+=/flame_shock,if=maelstrom>=20&buff.elemental_focus.up,target_if=refreshable
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true) && WoW.Maelstrom >= 20
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") < GCD || !WoW.TargetHasDebuff("Flame Shock")) && WoW.PlayerHasBuff("Elemental Focus"))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //actions.single_lr+=/earth_shock,if=maelstrom>=86
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.Maelstrom >= 86)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }

                    //actions.single_asc+=/lightning_bolt,if=buff.power_of_the_maelstrom.up&buff.stormkeeper.up&spell_targets.chain_lightning<3
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && WoW.PlayerHasBuff("Power of the Maelstrom")
                    && WoW.PlayerHasBuff("Stormkeeper"))
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_asc+=/lava_burst,if=dot.flame_shock.remains>cast_time
                    if (WoW.CanCast("Lava Burst", true, true, true, false, true)
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") > 200f / (1 + (WoW.HastePercent / 100f))))
                    {
                        WoW.CastSpell("Lava Burst");
                        return;
                    }
                    //actions.single_asc+=/earth_shock,if=maelstrom>=86
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.Maelstrom >= 86)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                    //actions.single_asc+=/totem_mastery,if=buff.resonance_totem.remains<10|(buff.resonance_totem.remains<(buff.ascendance.duration+cooldown.ascendance.remains)&cooldown.ascendance.remains<15)
                    if (WoW.Talent(1) == 3 && WoW.CanCast("Totem Mastery") && (!Totem.Active || Totem.Duration < 10))
                    {
                        WoW.CastSpell("Totem Mastery");
                        return;
                    }


                    //actions.single_if+=/earthquake,if=buff.echoes_of_the_great_sundering.up
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.TargetHasDebuff("Echoes of the Great") && mouseTarget)
                    {
                        WoW.CastSpell("Earthquake");
                        return;
                    }
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && WoW.PlayerHasBuff("Power of the Maelstrom")
                    && combatRoutine.Type != RotationType.AOE && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }

                    //actions.single_lr+=/lightning_bolt,if=buff.power_of_the_maelstrom.up&spell_targets.chain_lightning<3,target_if=debuff.lightning_rod.down
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && WoW.PlayerHasBuff("Power of the Maelstrom")
                    && combatRoutine.Type != RotationType.AOE && !WoW.IsMoving)
                    { //change target
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_if+=/lightning_bolt,if=buff.power_of_the_maelstrom.up&spell_targets.chain_lightning<3
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && WoW.PlayerHasBuff("Power of the Maelstrom")
                    && combatRoutine.Type != RotationType.AOE && !WoW.IsMoving && combatRoutine.Type != RotationType.AOE)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_lr+=/chain_lightning,if=active_enemies>1&spell_targets.chain_lightning>1,target_if=debuff.lightning_rod.down
                    if (WoW.CanCast("Chain Lightning", true, true, true, false, true) && combatRoutine.Type != RotationType.SingleTarget
                        && !WoW.TargetHasDebuff("Lightning Rod") && !WoW.IsMoving)
                    {
                        //change target need to add
                    }
                    //actions.single_lr+=/chain_lightning,if=active_enemies>1&spell_targets.chain_lightning>1
                    if (WoW.CanCast("Chain Lightning", true, true, true, false, true) && combatRoutine.Type != RotationType.SingleTarget
                    && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Chain Lightning");
                        return;
                    }

                    //actions.single_lr+=/lightning_bolt,target_if=debuff.lightning_rod.down
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && !WoW.TargetHasBuff("Lightning Rod")
                    && !WoW.IsMoving)
                    {
                        //change target
                    }
                    // actions.single_lr +=/ lightning_bolt
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }

                    //actions.single_lr+=/flame_shock,moving=1,target_if=refreshable
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true)
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") < GCD || !WoW.TargetHasDebuff("Flame Shock")) && WoW.IsMoving)
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }

                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.Maelstrom >= 50 && WoW.IsMoving)

                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                }
            });
        }
        private async Task Elemental_MeatBall()
        {
            await Task.Run(() =>
            {
                if (WoW.Talent(7) == 1)
                {
                    //actions.single_asc=ascendance,if=dot.flame_shock.remains>buff.ascendance.duration
                    //&(time >= 60 | buff.bloodlust.up) 
                    // & cooldown.lava_burst.remains > 0 & !buff.stormkeeper.up
                    if (WoW.CanCast("AscendanceEle", true, true, true, false, true)
                     && (WoW.TargetDebuffTimeRemaining("Flame Shock") > 1500)
                     && (WoW.IsSpellOnCooldown("Lava Burst") && !WoW.PlayerHasBuff("Stormkeeper")))
                    {
                        WoW.CastSpell("AscendanceEle");
                        return;
                    }
                    //actions.single_if=flame_shock,if=!ticking
                    //actions.single_asc+=/flame_shock,if=maelstrom>=20

                    if (WoW.CanCast("Flame Shock", true, true, true, false, true)
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") < GCD || !WoW.TargetHasDebuff("Flame Shock")) && WoW.Maelstrom >= 20)
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //&remains<=buff.ascendance.duration&cooldown.ascendance.remains+buff.ascendance.duration<=duration
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true) && WoW.Maelstrom >= 20
                        && (WoW.TargetDebuffTimeRemaining("Flame Shock") < WoW.PlayerBuffTimeRemaining("AscendanceEle") && WoW.PlayerHasBuff("AscendanceEle")
                        || WoW.PlayerBuffTimeRemaining("AscendanceEle") + WoW.SpellCooldownTimeRemaining("AscendanceEle") <= WoW.TargetDebuffTimeRemaining("Flame Shock")))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //actions.single_asc+=/earth_shock,if=maelstrom>=92&!buff.ascendance.up
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.Maelstrom >= 92 && !WoW.PlayerHasBuff("AscendanceEle"))
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                    //actions.single_if +=/ stormkeeper,if= raid_event.adds.count < 3 | raid_event.adds.in> 50
                    if (WoW.CanCast("Stormkeeper", true, true, false, false, true) && !WoW.PlayerHasBuff("AscendanceEle") && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Stormkeeper");
                        return;
                    }
                    //actions.single_asc+=/elemental_blast
                    if (WoW.CanCast("Elemental Blast", true, true, true, false, true) && WoW.Talent(5) == 3 && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Elemental Blast");
                        return;
                    }
                    //actions.single_asc+=/liquid_magma_totem,if=raid_event.adds.count<3|raid_event.adds.in>50
                    if (WoW.CanCast("Liquid Magma", true, true, false, false, true) && WoW.Talent(6) == 1 && mouseTarget)
                    {
                        WoW.CastSpell("Liquid Magma");
                        return;
                    }
                    //actions.single_asc+=/lightning_bolt,if=buff.power_of_the_maelstrom.up&buff.stormkeeper.up&spell_targets.chain_lightning<3
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && WoW.PlayerHasBuff("Power of the Maelstrom")
                    && WoW.PlayerHasBuff("Stormkeeper") && combatRoutine.Type != RotationType.AOE && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_asc+=/lava_burst,if=dot.flame_shock.remains>cast_time
                    if (WoW.CanCast("Lava Burst", true, true, true, false, true)
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") > 200f / (1 + (WoW.HastePercent / 100f))))
                    {
                        WoW.CastSpell("Lava Burst");
                        return;
                    }
                    //actions.single_if+=/flame_shock,if=maelstrom>=20&buff.elemental_focus.up,target_if=refreshable
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true) && WoW.Maelstrom >= 20 && WoW.PlayerHasBuff("Elemental Focus") && (WoW.TargetDebuffTimeRemaining("Flame Shock") < GCD || !WoW.TargetHasDebuff("Flame Shock")))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //actions.single_asc+=/earth_shock,if=maelstrom>=86
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.Maelstrom >= 86)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                    //actions.single_asc+=/totem_mastery,if=buff.resonance_totem.remains<10|(buff.resonance_totem.remains<(buff.ascendance.duration+cooldown.ascendance.remains)&cooldown.ascendance.remains<15)
                    if (WoW.Talent(1) == 3 && WoW.CanCast("Totem Mastery") && (!Totem.Active || Totem.Duration < 10))
                    {
                        WoW.CanCast("Totem Mastery");
                        return;
                    }


                    //actions.single_if+=/earthquake,if=buff.echoes_of_the_great_sundering.up
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.TargetHasDebuff("Echoes of the Great") && mouseTarget)
                    {
                        WoW.CastSpell("Earthquake");
                        return;
                    }
                    //actions.single_asc+=/lava_beam,if=active_enemies>1&spell_targets.lava_beam>1
                    if (WoW.CanCast("Lava Beam", true, true, true, false, true) && combatRoutine.Type != RotationType.SingleTarget
                    && WoW.PlayerHasBuff("AscendanceEle"))
                    {
                        WoW.CastSpell("Lava Beam");
                        return;
                    }

                    //actions.single_if+=/lightning_bolt,if=buff.power_of_the_maelstrom.up&spell_targets.chain_lightning<3
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && WoW.PlayerHasBuff("Power of the Maelstrom")
                    && combatRoutine.Type != RotationType.AOE && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_if+=/chain_lightning,if=active_enemies>1&spell_targets.chain_lightning>1
                    if (WoW.CanCast("Chain Lightning", true, true, true, false, true) && combatRoutine.Type != RotationType.SingleTarget
                    && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Chain Lightning");
                        return;
                    }
                    // actions.single_if +=/ lightning_bolt
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true) && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    //actions.single_if+=/flame_shock,moving=1,target_if=refreshable
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true)
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") < GCD || !WoW.TargetHasDebuff("Flame Shock")))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    if (WoW.CanCast("Earth Shock", true, true, true, false, true) && WoW.IsMoving)
                    {
                        WoW.CastSpell("Earth Shock");
                        return;
                    }
                }
            });
        }
        private void Elemental_AOE()
        {
            if (combatRoutine.Type == RotationType.AOE)
            {
                if (!WoW.IsMoving)
                {
                    //actions.aoe = stormkeeper
                    if (WoW.CanCast("Stormkeeper", true, true, false, false, true))
                    {
                        WoW.CastSpell("Stormkeeper");
                        return;
                    }
                    // actions.aoe = Ascendance
                    if (WoW.Talent(7) == 1 && WoW.CanCast("AscendanceEle", true, true, false, false, true))
                    {
                        WoW.CastSpell("AscendanceEle");
                        return;
                    }
                    //actions.aoe +=/ liquid_magma_totem
                    if (WoW.Talent(6) == 1 && WoW.CanCast("Liquid Magma", true, true, false, false, true) && mouseTarget)
                    {
                        WoW.CastSpell("Liquid Magma");
                        return;
                    }
                    // actions.aoe +=/ flame_shock,if= spell_targets.chain_lightning < 4 & maelstrom >= 20 
                    //& !talent.lightning_rod.enabled,target_if = refreshable
                    if (WoW.Talent(7) != 2 && WoW.CanCast("Flame Shock", true, true, true, false, true)
                     && combatRoutine.Type == RotationType.AOE && WoW.Maelstrom >= 20
                     && (WoW.TargetDebuffTimeRemaining("Flame Shock") < 450 || !WoW.TargetHasDebuff("Flame Shock")))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                    //actions.aoe +=/ earthquake
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.Maelstrom >= 50)
                    {
                        WoW.CastSpell("Earthquake");
                        return;
                    }
                    // actions.aoe +=/ lava_burst,if= dot.flame_shock.remains > cast_time 
                    //& buff.lava_surge.up & !talent.lightning_rod.enabled & spell_targets.chain_lightning < 4
                    if (WoW.CanCast("Lava Burst", true, true, true, false, true)
                    && (WoW.TargetDebuffTimeRemaining("Flame Shock") > 200f / (1 + (WoW.HastePercent / 100f)) || !WoW.TargetHasDebuff("Flame Shock"))
                    && WoW.PlayerHasBuff("Lava Surge") && WoW.Talent(7) != 2 && combatRoutine.Type == RotationType.AOE)

                    {
                        WoW.CastSpell("Lava Burst");
                        return;
                    }
                    //actions.aoe +=/ elemental_blast,if= !talent.lightning_rod.enabled & spell_targets.chain_lightning < 5
                    if (WoW.CanCast("Elemental Blast", true, true, true, false, true)
                    && WoW.Talent(7) != 7 && WoW.Talent(5) == 3 && combatRoutine.Type == RotationType.AOE && WoW.CountEnemyNPCsInRange < 5 && !WoW.IsMoving)
                    {
                        WoW.CastSpell("Elemental Blast");
                        return;
                    }
                    // actions.aoe +=/ lava_beam
                    if (WoW.CanCast("Lava Beam", true, true, true, false, true) && WoW.PlayerHasBuff("AscendanceEle"))
                    {
                        WoW.CastSpell("Lava Beam");
                        return;
                    }
                    //actions.aoe +=/ chain_lightning,target_if = debuff.lightning_rod.down
                    //actions.aoe +=/ chain_lightning
                    if (WoW.CanCast("Chain Lightning", true, true, true, false, true) && !WoW.IsMoving
                    && (WoW.Talent(7) == 2 && WoW.TargetHasDebuff("Lightning Rod")))
                    {
                        WoW.CastSpell("Chain Lightning");
                        return;
                    }
                    if (WoW.CanCast("Chain Lightning", true, true, true, false, true) && !WoW.IsMoving
                   && WoW.Talent(7) != 2)
                    {
                        WoW.CastSpell("Chain Lightning");
                        return;
                    }
                }
                if (WoW.IsMoving)
                {
                    //actions.aoe +=/ lava_burst,moving = 1
                    if (WoW.CanCast("Lava Burst", true, true, true, false, true)
                       && WoW.PlayerHasBuff("Lava Surge"))
                    {
                        WoW.CastSpell("Lava Burst");
                        return;
                    }
                    // actions.aoe +=/ flame_shock,moving = 1,target_if = refreshable
                    if (WoW.CanCast("Flame Shock", true, true, true, false, true)
                        && (WoW.TargetDebuffTimeRemaining("Flame Shock") < 450 || !WoW.TargetHasDebuff("Flame Shock")))
                    {
                        WoW.CastSpell("Flame Shock");
                        return;
                    }
                }
            }
        }
        private void Feral_spirit()
        {
            if (WoW.CanCast("Feral Spirit", true, true, false, false, true) && WoW.Maelstrom >= 20 && (WoW.CanCast("Crash Lightning", true, true, false, false, true) || WoW.SpellCooldownTimeRemaining("Crash Lightning") < GCD)) //feral spirit on boss - normally cast manually
            {
                Pets.Start();
                Log.Write("Using Feral Spirit", Color.Red);
                WoW.CastSpell("Feral Spirit");
                return;
            }
        }
        private async Task EnhancementDps()
        {
            await Task.Run(() =>
            {
                if (WoW.PlayerSpec == "Enhancement")
                {
                    Feral_spirit();
                    //actions +=/ crash_lightning,if= artifact.alpha_wolf.rank & prev_gcd.1.feral_spirit
                    if (WoW.CanCast("Crash Lightning", true, true, false, false, false) && WoW.IsSpellInRange("Stormstrike")
                        //&& WoW.WildImpsCount > 1
                        && Pets.IsRunning
                        && WoW.Maelstrom >= 20
                        && (Crash.Elapsed.Seconds <= 10 && Crash.IsRunning && Crash.Elapsed.Seconds >= 6 || !Crash.IsRunning))
                    {
                        if (!Crash.IsRunning)
                            Crash.Start();
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }

                    // a    ctions +=/ berserking,if= buff.ascendance.up | !talent.ascendance.enabled | level < 100
                    // actions +=/ blood_fury
                    DPSRacial();
                    //actions +=/ potion,name = prolonged_power,if= feral_spirit.remains > 5 | target.time_to_die <= 60
                    //x UsePotion();
                    /* if (WoW.CanCast("Stormstrike", true, true, true, false, true)
                     && (WoW.Maelstrom >= 135 || WoW.Maelstrom >= 100 && (WoW.PlayerHasBuff("Doom Winds") || Pets.IsRunning)))
                     {
                              Log.Write("Maelstrom overflow protection", Color.Blue);
                              WoW.CastSpell("Stormstrike");
                              return;
                     }*/

                    if (WoW.CanCast("Lava Lash", true, true, true, false, true) && WoW.Maelstrom >= 40 && WoW.TargetDebuffStacks("Legionfall") > 90)
                    {
                        Log.Write("Maelstrom overflow protection", Color.Blue);
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
                    // Log.Write("Buff Time :" +  WoW.PlayerBuffTimeRemaining("Flametongue") + " GCD: " + GCD + "Has buff :" + WoW.PlayerHasBuff("Flametongue"));
                    // actions +=/ boulderfist,if= buff.boulderfist.remains < gcd | (maelstrom <= 50 & active_enemies >= 3)
                    if (WoW.Talent(1) == 3 && WoW.CanCast("Boulderfist", true, true, true, false, true)
                      && ((WoW.PlayerBuffTimeRemaining("Boulderfist") <= GCD || !WoW.PlayerHasBuff("Boulderfist"))
                   || (WoW.Maelstrom <= 50 && combatRoutine.Type == RotationType.AOE)))
                    {
                        Log.Write("Buff Time :" + WoW.PlayerBuffTimeRemaining("Boulderfist") + " GCD: " + GCD + "Has buff :" + WoW.PlayerHasBuff("Boulderfist"));
                        WoW.CastSpell("Boulderfist"); //boulderfist it to not waste a charge
                        return;
                    }
                    //actions +=/ boulderfist,if= buff.boulderfist.remains < gcd | (charges_fractional > 1.75 & maelstrom <= 100 & active_enemies <= 2)
                    if (WoW.Talent(1) == 3 && WoW.CanCast("Boulderfist", true, true, true, false, true)
                    && WoW.Maelstrom <= 100 && (WoW.PlayerSpellCharges("Boulderfist") >= 2
                    || WoW.PlayerSpellCharges("Boulderfist") + (((600 / (1 + WoW.HastePercent / 100)) - (WoW.SpellCooldownTimeRemaining("Boulderfist"))) / (600 / (1 + WoW.HastePercent / 100))) > 1.75)
                    && combatRoutine.Type != RotationType.AOE)
                    {
                        WoW.CastSpell("Boulderfist"); //boulderfist it to not waste a charge
                        return;
                    }
                    //actions +=/ rockbiter,if=talent.landslide.enabled & buff.landslide.remains < gcd
                    if (WoW.Talent(1) != 3 && WoW.Talent(7) == 2 && (!WoW.PlayerHasBuff("Landslide") || WoW.PlayerBuffTimeRemaining("Landslide") < GCD)
                     && WoW.CanCast("Rockbiter", true, true, true, false, true))
                    {
                        WoW.CastSpell("Rockbiter");
                        return;
                    }
                    //actions +=/ fury_of_air,if= !ticking & maelstrom > 22
                    if (WoW.Talent(6) == 2 && WoW.Maelstrom >= 22 && !WoW.PlayerHasBuff("Fury Air") && WoW.CanCast("Fury Air", true, true, false, false, true))
                    {
                        WoW.CastSpell("Fury Air");
                        return;
                    }
                    //actions +=/ frostbrand,if= talent.hailstorm.enabled & buff.frostbrand.remains < gcd & ((!talent.fury_of_air.enabled) | (talent.fury_of_air.enabled & maelstrom > 25))
                    if (WoW.Talent(4) == 3 && WoW.Maelstrom >= 20 && WoW.CanCast("Hailstorm", true, true, true, false, true)
                        && (!WoW.PlayerHasBuff("Frostbrand") || WoW.PlayerBuffTimeRemaining("Frostbrand") < GCD)&&WoW.Talent(6) !=2)
                    {
                        WoW.CastSpell("Hailstorm");
                        return;
                    }

                    // actions +=/ flametongue,if= buff.flametongue.remains < gcd | (cooldown.doom_winds.remains < 6 & buff.flametongue.remains < 4)
                    if (((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") < GCD) || (WoW.PlayerBuffTimeRemaining("Flametongue") < 400 && WoW.IsSpellOnCooldown("Doom Winds") && WoW.SpellCooldownTimeRemaining("Doom Winds") < 600))
                         && WoW.CanCast("Flametongue", true, true, true, false, true))
                    {
                        WoW.CastSpell("Flametongue");
                        return;
                    }
                    //actions +=/ doom_winds
                    if (WoW.CanCast("Doom Winds", true, true, false, false, true) && WoW.IsSpellInRange("Stormstrike") && WoW.PlayerHasBuff("Flametongue"))
                    {
                        WoW.CastSpell("Doom Winds");
                        return;
                    }
                    //actions +=/ crash_lightning,if= talent.crashing_storm.enabled & active_enemies >= 3 
                    //&& (!talent.hailstorm.enabled | buff.frostbrand.remains > gcd)
                    if (WoW.Talent(6) == 3 && combatRoutine.Type == RotationType.AOE
                        && (WoW.Talent(4) != 3 || WoW.PlayerBuffTimeRemaining("Frostbrand") > GCD)
                        && WoW.IsSpellInRange("Stormstrike") && WoW.CanCast("Crash Lightning", true, true, false, false, true) && WoW.Maelstrom > 20)

                    {

                        WoW.CastSpell("Crash Lightning");
                        return;
                    }

                    //actions +=/ earthen_spike
                    if (WoW.Talent(7) == 3 && WoW.CanCast("Earthen Spike", true, true, false, false, true))
                    {
                        WoW.CastSpell("Earthen Spike");
                        return;
                    }

                    //if= (talent.overcharge.enabled & maelstrom >= 40 & !talent.fury_of_air.enabled) 
                    //| (talent.overcharge.enabled & talent.fury_of_air.enabled & maelstrom > 46)
                    if (WoW.CanCast("Lightning Bolt", true, true, true, false, true)
                    && ((WoW.Talent(5) == 2 && WoW.Maelstrom >= 40 && WoW.Talent(6) != 2)
                    || (WoW.Talent(5) == 2 && WoW.Maelstrom >= 46 && WoW.Talent(6) == 2)))
                    {
                        WoW.CastSpell("Lightning Bolt");
                        return;
                    }
                    // actions +=/ crash_lightning,if= buff.crash_lightning.remains < gcd & active_enemies >= 2
                    if ((!WoW.PlayerHasBuff("Crash Lightning") || WoW.PlayerBuffTimeRemaining("Crash Lightning") < GCD)
                        && (combatRoutine.Type == RotationType.AOE || combatRoutine.Type == RotationType.SingleTargetCleave)
                        && WoW.IsSpellInRange("Stormstrike") && WoW.CanCast("Crash Lightning", true, true, false, false, true) && WoW.Maelstrom > 20)
                    {
                        Log.Write("Aoe/Cleave Rotation");
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
                    // actions +=/ windsong
                    if (WoW.Talent(1) == 1 && !WoW.PlayerHasBuff("Windsong") && WoW.CanCast("Windsong", true, true, false, false, true))
                    {
                        WoW.CastSpell("Windsong");
                        return;
                    }// actions +=/ Ascendance
                    if (WoW.Talent(7) == 1 && !WoW.PlayerHasBuff("Ascendance") && WoW.CanCast("Ascendance", true, true, false, false, true))
                    {
                        WoW.CastSpell("Ascendance");
                        return;
                    }
                    /* Log.Write("Temptation deuff: " + WoW.PlayerHasDebuff("Temptation"));
                     if (!WoW.PlayerHasDebuff("Temptation"))
                         WoW.CastSpell("Collapsing Futures");*/
                    // actions +=/ stormstrike,if= buff.stormbringer.react & ((talent.fury_of_air.enabled & maelstrom >= 26) | (!talent.fury_of_air.enabled))
                    if (WoW.CanCast("Stormstrike", true, true, true, false, true) && WoW.PlayerHasBuff("Stormbringer")
                        && (WoW.Talent(6) == 2 && WoW.Maelstrom >= 26 || WoW.Talent(6) != 2 && WoW.Maelstrom >= 20))
                    {
                        // Log.Write("Stormstrike react Current Maelstrom :" + WoW.Maelstrom + " Stormbringer ?:" + WoW.PlayerHasBuff("Stormbringer"));
                        WoW.CastSpell("Stormstrike");
                        return;

                    }
                    //actions +=/ frostbrand,if= equipped.137084 & talent.hot_hand.enabled & buff.hot_hand.react & !buff.frostbrand.up & ((!talent.fury_of_air.enabled) | (talent.fury_of_air.enabled & maelstrom > 25))
                    if (WoW.Legendary(1) == 7 && WoW.Legendary(2) == 7 && WoW.Talent(4) == 3 && (WoW.Talent(6) != 2 && WoW.Maelstrom >= 20 || WoW.Maelstrom > 25) && WoW.CanCast("Hailstorm", true, true, true, false, true)
                        && WoW.PlayerHasBuff("Hot hands") && (!WoW.PlayerHasBuff("Frostbrand") || WoW.PlayerBuffTimeRemaining("Frostbrand") < GCD) && WoW.Talent(6) != 2)
                    {
                        WoW.CastSpell("Hailstorm");
                        return;
                    }
                    //actions +=/ lava_lash,if= talent.hot_hand.enabled & buff.hot_hand.react
                    if (WoW.PlayerHasBuff("Hot hands") && WoW.CanCast("Lava Lash", true, true, true, false, true))
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
                    //actions +=/ crash_lightning,if= active_enemies >= 3
                    if (combatRoutine.Type == RotationType.AOE && WoW.IsSpellInRange("Stormstrike") && WoW.CanCast("Crash Lightning", true, true, false, false, true) && WoW.Maelstrom >= 20)
                    {
                        Log.Write("Aoe Rotation");
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
                    //actions +=/ stormstrike,if= talent.overcharge.enabled & cooldown.lightning_bolt.remains < gcd & maelstrom > 80
                    if (WoW.CanCast("Stormstrike", true, true, true, false, true)
                        && WoW.Talent(5) == 2 && WoW.SpellCooldownTimeRemaining("Lightning Bolt") < GCD && WoW.Maelstrom >= 80)
                    {
                        Log.Write("Stormstrike OC Current Maelstrom :" + WoW.Maelstrom + " Stormbringer ?:" + WoW.PlayerHasBuff("Stormbringer"));
                        WoW.CastSpell("Stormstrike");
                        return;
                    }

                    // actions +=/ stormstrike,if= talent.fury_of_air.enabled & maelstrom > 46 & (cooldown.lightning_bolt.remains > gcd | !talent.overcharge.enabled)
                    if (WoW.CanCast("Stormstrike", true, true, true, false, true)
                     && WoW.Talent(6) == 2 && WoW.Maelstrom >= 46
                     && (WoW.SpellCooldownTimeRemaining("Lightning Bolt") < GCD || WoW.Talent(5) != 2))
                    {
                        Log.Write("Storm strike no OC basic Current Maelstrom :" + WoW.Maelstrom + " Stormbringer ?:" + WoW.PlayerHasBuff("Stormbringer"));
                        WoW.CastSpell("Stormstrike");
                        return;
                    }
                    //actions +=/ stormstrike,if= !talent.overchage.enabled & !talent.fury_of_air.enabled
                    if (WoW.CanCast("Stormstrike", true, true, true, false, true)
                    && WoW.Talent(5) != 2 && WoW.Talent(6) != 2
                    && (WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 20 || WoW.Maelstrom >= 40))
                    {
                        // Log.Write("Storm strike no OC/FoA Current Maelstrom :" + WoW.Maelstrom + " Stormbringer ?:" + WoW.PlayerHasBuff("Stormbringer"));

                        WoW.CastSpell("Stormstrike");
                        return;
                    }
                    //actions +=/ stormstrike,if= active_enemies >= 3 & !talent.hailstorm.enabled
                    if (WoW.CanCast("Stormstrike", true, true, true, false, true)
                    && (WoW.Maelstrom >= 40 || WoW.PlayerHasBuff("Stormbringer") && WoW.Maelstrom >= 20)
                    && combatRoutine.Type == RotationType.AOE & WoW.Talent(4) != 3)
                    {

                        Log.Write("Storm strike basic Current Maelstrom :" + WoW.Maelstrom + " Stormbringer ?:" + WoW.PlayerHasBuff("Stormbringer"));

                        WoW.CastSpell("Stormstrike");
                        return;
                    }

                    //if= ((active_enemies > 1 
                    //| talent.crashing_storm.enabled 
                    //| talent.boulderfist.enabled) & !set_bonus.tier19_4pc) | feral_spirit.remains > 5
                    if (WoW.CanCast("Crash Lightning", true, true, true, false, true) && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 20
                    && ((combatRoutine.Type == RotationType.AOE || combatRoutine.Type == RotationType.SingleTargetCleave || WoW.Talent(6) == 1 || WoW.Talent(1) == 3) && WoW.SetBonus(19) < 4 || Pets.IsRunning && Pets.Elapsed.Seconds > 0 && Pets.Elapsed.Seconds <= 10))
                    {
                        Log.Write("This is the problem");
                        //Log.Write("T6=1 : " + WoW.Talent(6) + "or T1 ==3: " + WoW.Talent(1) + " or Pet timer : " + Pets.Elapsed.Seconds);
                        WoW.CastSpell("Crash Lightning");
                        return;
                    }
                    //   actions +=/ frostbrand,if= talent.hailstorm.enabled & buff.frostbrand.remains < 4.8

                    if (WoW.Talent(4) == 3 && (!WoW.PlayerHasBuff("Frostbrand") || WoW.PlayerBuffTimeRemaining("Frostbrand") < 480)
                      && WoW.CanCast("Hailstorm", true, true, true, false, true) && WoW.Maelstrom >= 20)
                    {
                        WoW.CastSpell("Hailstorm");
                        return;
                    }
                    // actions +=/ flametongue,if= buff.flametongue.remains < 4.8
                    if ((!WoW.PlayerHasBuff("Flametongue") || WoW.PlayerBuffTimeRemaining("Flametongue") <= 480)
                         && WoW.CanCast("Flametongue", true, true, true, false, true))
                    {
                        WoW.CastSpell("Flametongue");
                        return;
                    }
                    //actions +=/ lava_lash,if= talent.fury_of_air.enabled & talent.overcharge.enabled & (set_bonus.tier19_4pc & maelstrom >= 80)
                    if (WoW.CanCast("Lava Lash", true, true, true, false, true)
                        && (WoW.Talent(6) == 2 && WoW.Talent(5) == 2 && WoW.SetBonus(19) >= 4 && WoW.Maelstrom >= 80))
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }

                    //actions +=/ lava_lash,if= talent.fury_of_air.enabled & !talent.overcharge.enabled & (set_bonus.tier19_4pc & maelstrom >= 53)
                    if (WoW.CanCast("Lava Lash", true, true, true, false, true)
                    && (WoW.Talent(6) == 2 && WoW.Talent(5) != 2 && WoW.SetBonus(19) >= 4 && WoW.Maelstrom >= 53))
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
                    //actions +=/ lava_lash,if= (!set_bonus.tier19_4pc & maelstrom >= 120) | (!talent.fury_of_air.enabled & set_bonus.tier19_4pc & maelstrom >= 40)
                    if (WoW.CanCast("Lava Lash", true, true, true, false, true)
                    && (WoW.SetBonus(19) < 4 && WoW.Maelstrom >= 120 || WoW.Talent(6) != 2 && WoW.Maelstrom >= 40 && WoW.SetBonus(19) >= 4))
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
                    //   actions +=/ sundering
                    if (WoW.Talent(6) == 3 && WoW.CanCast("Sundering", true, true, false, false, true) && WoW.IsSpellInRange("Stormstrike") && WoW.Maelstrom >= 60)
                    {
                        WoW.CastSpell("Sundering");
                        return;
                    }

                    // actions +=/ rockbiter
                    if (WoW.Talent(1) != 3 && WoW.CanCast("Rockbiter", true, true, true, false, true))
                    {
                        WoW.CastSpell("Rockbiter");
                        return;
                    }
                    //actions +=/ flametongue

                    if (WoW.CanCast("Flametongue", true, true, true, false, true))
                    {
                        Log.Write("Nothing to due");
                        WoW.CastSpell("Flametongue");
                        return;
                    }
                    //actions +=/ boulderfist
                    if (WoW.Talent(1) == 3 && WoW.CanCast("Boulderfist", true, true, true, false, true))
                    {
                        Log.Write("Nothing to due");
                        WoW.CastSpell("Boulderfist");
                        return;
                    }
                    if (WoW.CanCast("Ghost Wolf", true, true, false, false, true) && WoW.IsMoving && !WoW.PlayerHasBuff("Ghost Wolf") && !WoW.IsSpellInRange("Stormstrike"))
                    {
                        WoW.CastSpell("Ghost Wolf");
                        return;
                    }
                }
            });
        }
        /// <summary>
        /// addon edditting stuff 
        /// </summary>
        private async void DpsRotationSelection()
        {
            Task[] RotationType = new Task[4];
            RotationType[0] = ChangeTarget();
            RotationType[1] = ElementalConsistantUse();
            RotationType[2] = EnhancementDps();
            RotationType[3] = RestoRotation();
            await Task.WhenAll(RotationType);

        }
        private async Task ChangeTarget()
        {
            await Task.Run(() => {
                if (WoW.PlayerSpec == "Mage" && !targetMelee)
                    WoW.KeyPressRelease(WoW.Keys.Tab);
                if (WoW.PlayerSpec == "Elemental " && !mouseTarget)
                    WoW.KeyPressRelease(WoW.Keys.Tab);
            });
        }
        public override void Pulse()
        {
            AsyncPulse();
            //DBMPrePull();
            TimerReset();
            //var c = WoW.GetBlockColor(4, 24);
            //Log.Write("R : " + c.R + " G: " + c.G + " B: " + c.B);

            //Log.Write("Spec  : " + WoW.PlayerSpec + " Legendary1 : " + WoW.Legendary(1) + " legendary2 :" + WoW.Legendary(2));
            if (WoW.IsInCombat && !WoW.IsMounted)
            {
                SelectRotation(3, 2, 1);
                interruptcast();
                //Stuns();
                Defensive();
                DpsRotationSelection();
            }
        }
        /// <summary>
        /// Cooldown function/mount/Buff/stack/Remaining time
        /// </summary>
        /// 

        private static int RaidSize
        {
            get
            {
                var c = WoW.GetBlockColor(11, 23);
                try
                {
                    int players = 0;
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R)) * 100 / 255) > 0)
                        players = (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R)) * 100 / 255));
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R)) * 100 / 255) == 100)
                        players = 1;
                    if (players > 30)
                        players = 30;
                    if (players <= 5)
                        players = players - 1;
                    return players;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Players  Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return 1;
            }
        }
        private static bool dbmOn
        {
            get
            {
                var c = WoW.GetBlockColor(7, 24);
                try
                {
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R) / 255)) == 1)
                        return true;
                    else
                        return false;

                }
                catch (Exception ex)
                {
                    Log.Write("Error in DBMON  Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return false;
            }
        }
        private static int dbmTimer
        {
            get
            {
                var c = WoW.GetBlockColor(7, 24);
                try
                {
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R) / 255)) == 1)
                        return Convert.ToInt32(Math.Round(Convert.ToSingle(c.G) * 100 / 255));
                    else
                        return 0;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Dbm Timer Green = " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return 0;
            }
        }
        private static bool lastNamePlate = true;
        public void SelectRotation(int aoe, int cleave, int single)
        {
            int count = WoW.CountEnemyNPCsInRange;
            if (!lastNamePlate)
            {
                combatRoutine.ChangeType(RotationType.SingleTarget);
                lastNamePlate = true;
            }
            lastNamePlate = WoW.Nameplates;
            if (count >= 4)
                combatRoutine.ChangeType(RotationType.AOE);
            if (count == 2)
                combatRoutine.ChangeType(RotationType.SingleTargetCleave);
            if (count <= 1)
                combatRoutine.ChangeType(RotationType.SingleTarget);

        }
        private static bool targetMelee
        {
            get
            {
                var c = WoW.GetBlockColor(9, 24);
                try
                {
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.R) / 255)) == 1)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Target Melee Red= " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return true;
            }
        }
        private static bool mouseTarget
        {
            get
            {
                var c = WoW.GetBlockColor(9, 24);
                try
                {
                    if (Convert.ToInt32(Math.Round(Convert.ToSingle(c.G) / 255)) == 1)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Target Melee Red= " + c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return true;
            }
        }

        /// <summary>
        /// Lua strings for varies functions above
        /// </summary>
        private const string CastUpdate = @"
        local function updatePlayerIsCasting(self, event)
		spell, _, _, _, startTime, endTime, _, castID, _ = UnitCastingInfo(""player"")
		name, _, _, _, _, _, _, _ = UnitChannelInfo(""player"")

				
			if castID ~= nil then
		
				if GetTime() + timeDiff <= endTime/1000  then
					--print(""Cast time :"", timeDiff, ""Time "", GetTime())
					--print(""Cast time :"", endTime/1000, ""Time "", GetTime()+ timeDiff)
					playerIsCastingFrame.t:SetColorTexture(1, 0, 0, alphaColor)
				else
					--print(""OffCast time :"", endTime/1000 - timeDiff, ""Time "", GetTime())
					playerIsCastingFrame.t:SetColorTexture(1, 1, 1, alphaColor)

                end
            end
		
			if castID == nil then

                playerIsCastingFrame.t:SetColorTexture(1, 1, 1, alphaColor)

            end	
		

		if name ~= nil then
			if text ~= lastChanneling then

            playerIsCastingFrame.t:SetColorTexture(0, 1, 0, alphaColor)
			--   print(text)

            lastChanneling = text
            end

		else
			if text ~= lastChanneling then

            playerIsCastingFrame.t:SetColorTexture(1, 1, 1, alphaColor)

            lastChanneling = text
            end


        end
    end
";

    }


}

/*
[AddonDetails.db]
AddonAuthor=Hamuel
AddonName=Hamuel
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,57994,Wind Shear,NumPad1
Spell,196884,Feral Lunge,F9
Spell,51533,Feral Spirit,NumPad0
Spell,196834,Hailstorm,NumPad2
Spell,204945,Doom Winds,F1
Spell,187874,Crash Lightning,NumPad4
Spell,193796,Flametongue,NumPad3
Spell,108271,Astral Shift,F2
Spell,193786,Rockbiter,NumPad5
Spell,201897,Boulderfist,NumPad5
Spell,60103,Lava Lash,NumPad6
Spell,17364,Stormstrike,NumPad7
Spell,187837,Lightning Bolt,NumPad8
Spell,188070,Healing Surge,NumPad9
Spell,215864,Rainfall,F8
Spell,188089,Earthen Spike,F4
Spell,201898,Windsong,F5
Spell,197217,Sundering,F6
Spell,114051,Ascendance,Add
Spell,114050,AscendanceEle,Add
Spell,197211,Fury Air,Subtract
Spell,59544,Gift Naaru,F10
Spell,192058,Lightning Surge,F7
Spell,26297,Berserking,F10
Spell,33697,Blood Fury,F10
Spell,20549,War Stomp,F10
Spell,155145,Arcane Torrent,F10
Spell,107079,Quaking palm,F10
Spell,142117,Prolonged Power,F11
Spell,188389,Flame Shock,NumPad2
Spell,198067,Fire Elemental,NumPad6
Spell,51505,Lava Burst,NumPad3
Spell,198103,Earth Elemental,NumPad7    
Spell,114074,Lava Beam,NumPad4
Spell,188443,Chain Lightning,NumPad4
Spell,16166,Elemental Mastery,NumPad0
Spell,61882,Earthquake,Subtract
Spell,210714,Icefury,F6
Spell,117014,Elemental Blast,F8
Spell,192222,Liquid Magma,F9
Spell,108281,Ancestral Guidance,D7
Spell,205495,Stormkeeper,F5
Spell,210643,Totem Mastery,F4
Spell,8042,Earth Shock,NumPad5
Spell,196840,Frost Shock,F10
Spell,5394,Healing Stream Totem,NumPad6
Spell,157153,Cloudburst Totem,F5
Spell,198838,Earthen Shield Totem,F6
Spell,61295,Riptide,NumPad5
Spell,77472,Healing Wave,NumPad3
Spell,1064,Chain Heal,NumPad4
Spell,207778,Queen Heal,NumPad0
Spell,2645,Ghost Wolf,E
Spell,3,raid3,U
Spell,2,raid2,Y
Spell,1,raid1,T
Spell,4,raid4,I
Spell,142173,Collapsing Futures,F12
Aura,61295,Riptide
Aura,210689,Lightning Rod
Aura,188389,Flame Shock
Aura,210659,Totem Mastery
Aura,114050,AscendanceEle
Aura,77762,Lava Surge
Aura,205495,Stormkeeper
Aura,194084,Flametongue
Aura,196834,Frostbrand
Aura,187878,Crashing Storm
Aura,187874,Crash Lightning
Aura,218825,Boulderfist
Aura,202004,Landslide
Aura,201846,Stormbringer
Aura,204945,Doom Winds
Aura,127271,Mount
Aura,215864,Rainfall
Aura,114051,Ascendance
Aura,201898,Windsong
Aura,201900,Hot hands
Aura,197211,Fury Air
Aura,191861,Power of the Maelstrom
Aura,137074,Echoes of the Great
Aura,16164,Elemental Focus
Aura,210714,Icefury
Aura,2645,Ghost Wolf
Aura,240842,Legionfall
Aura,234143,Temptation
Item,142117,Prolonged Power
Item,142173,Collapsing Futures
[Dispell.db]
*/
