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
using PixelMagic.Helpers;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PixelMagic.Rotation
{//Data tables
    public struct targetinfo_data
    {
        public bool Melee;
        public bool Range;
        public targetinfo_data(bool p1, bool p2)
        {
            Melee = p1;
            Range = p2;
        }
    }
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
    public struct dbm_data
    {
        public int Timer;
        public bool On;

        private dbm_data(int p1, bool p2)
        {
            Timer = p1;
            On = p2;
        }
    }

    public class Enhancement : CombatRoutine
    {
        private static double Revision = 2.3;
        private int npcCount, players;
        private static int EnhLowHp = 40;
        private static int interMin = 50;
        private static int interMax = 90;
        private bool Nameplates = false;
        private static int TankNum = 5, RipNum = 4;
        private static int RiptidePct = 90;
        private static int Chainheal = 50, HTide = 60, HStream = 90, SLink = 25;
        private static int ChainCnt = 3, HTideCnt = 3, HStreamCnt = 1, SLinkCnt = 2;
        private static int HSurge = 75, HWave = 99, TankLow = 15;
        private static int LocationPlayer = 31;
        public Stopwatch Healstream = new Stopwatch();
        public dbm_data DBM = new dbm_data();
        private Rip_data[] PartyRip = new Rip_data[RipNum];
        private Tank_data[] TanksInfo = new Tank_data[TankNum];
        private Party_data[] PartyInfo = new Party_data[31];
        private Lowest_data LowInfo = new Lowest_data();
        public Below_data Below = new Below_data();
        private targetinfo_data TargetInfo = new targetinfo_data();
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
            Task[] tasks = new Task[6];
            tasks[0] = Healing_Update();
            tasks[1] = Who_Low();
            tasks[2] = RaidSize();
            tasks[3] = DBMTimer();
            tasks[4] = TotemInfo();
            tasks[5] = targetInfo();
            await Task.WhenAll(tasks);
        }
        private async Task Healing_Update()
        {
            await Task.Run(() =>
            {
                Color pixelColor = Color.FromArgb(0);
                for (int i = 1; i <= players; i++)
                {
                    if (players >= 21 && i >= 21)
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
                for (int i = 1; i <= players; i++)
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

        private async Task RaidSize()
        {
            await Task.Run(() =>
            {
                Color pixelColor = Color.FromArgb(0);
                pixelColor = WoW.GetBlockColor(11, 23);
                players = 1;

                if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R)) * 100 / 255) > 0)
                    players = (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R)) * 100 / 255));
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R)) * 100 / 255) == 100)
                    players = 1;
                if (players > 30)
                    players = 30;
                if (players <= 5)
                    players = players - 1;

                npcCount = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255));
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.B) / 255)) == 1)
                    Nameplates = true;
                else
                    Nameplates = false;
                //Log.Write("Name plate location : "+ Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.B) * 100 / 255)));
                //Log.Write("npc count :" + npcCount);
                //Log.Write("npc count : " + npcCount + " Party size: " + players + " Name Plates on: " + Nameplates);
            });
        }
        private async Task DBMTimer()
        {
            await Task.Run(() =>
            {
                Color pixelColor = Color.FromArgb(0);
                pixelColor = WoW.GetBlockColor(6, 24);
                //Log.Write("Block 6,21 R: "+Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R ) / 255)) +" block 7,21 G " + Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255)));
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) / 255)) == 1)
                {
                    DBM.On = true;
                    DBM.Timer = Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) * 100 / 255));
                    Log.Write("DBM.On True");
                }
                else
                {
                    //Log.Write("DBM.On false");
                    DBM.On = false;
                }
                //Log.Write("Is DBM  on: " + DBM.On + " Time : " + DBM.Timer);

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
        private async Task targetInfo()
        {
            await Task.Run(() =>
            {
                Color pixelColor = Color.FromArgb(0);
                pixelColor = WoW.GetBlockColor(8, 24);
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.R) / 255)) == 1)
                    TargetInfo.Melee = true;
                else
                    TargetInfo.Melee = false;
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(pixelColor.G) / 255)) == 1)
                    TargetInfo.Range = true;
                else
                    TargetInfo.Range = false;
                //Log.Write("Target Melee : " + TargetInfo.Melee + "Mouse over :" + TargetInfo.Range);



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
                if (PlayerSpec == "Restoration") // Do Single Target Stuff here
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
                if (Convert.ToSingle(150f / (1 + (WoW.HastePercent/ 100f))) > 75f)
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
                if (WoW.PlayerRace== "BloodElf" && WoW.CanCast("Arcane Torrent", true, true, false, false, true) && !WoW.IsSpellOnCooldown("Wind Shear") && WoW.TargetIsCastingAndSpellIsInterruptible && WoW.IsSpellInRange("Stormstrike")) //interupt every spell, not a boss.
                {
                    WoW.CastSpell("Arcane Torrent");
                    return;
                }
                if (WoW.PlayerRace== "Pandaren" && WoW.CanCast("Quaking palm", true, true, true, false, true)) //interupt every spell, not a boss.
                {
                    WoW.CastSpell("Quaking palm");
                    return;
                }
            }
        }

        private void DBMPrePull()
        {
            if (DBM.On && DBM.Timer <= 18 && DBM.Timer > 0 && WoW.HasTarget)
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
                if (WoW.Talent(2) == 2 && WoW.CanCast("Feral Lunge", true, true, false, false, true) && DBM.Timer < 1)
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
            if (WoW.PlayerRace== "Dreanei" && WoW.HealthPercent < 80 && !WoW.IsSpellOnCooldown("Gift Naaru"))
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
                if (WoW.PlayerRace== "Tauren​" && !WoW.IsMoving && WoW.CanCast("War Stomp") && !WoW.IsSpellOnCooldown("War Stomp") && (WoW.Talent(3) != 1 || (WoW.IsSpellOnCooldown("Lightning Surge") && WoW.Talent(3) == 1)))
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
                if (WoW.PlayerRace== "Troll" && WoW.CanCast("Berserking") && !WoW.IsSpellOnCooldown("Berserking") && (WoW.Talent(7) != 1 || WoW.PlayerHasBuff("Ascendance")))
                {
                    WoW.CastSpell("Berserking");
                    return;
                }
                // actions +=/ blood_fury,if= !talent.ascendance.enabled | buff.ascendance.up | cooldown.ascendance.remains > 50
                if (WoW.PlayerRace== "Orc" && WoW.CanCast("Blood Fury") && !WoW.IsSpellOnCooldown("Blood Fury")
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
        private void SelectRotation()
        {
            if (Nameplates)
            {
                if (npcCount >= 3)
                    combatRoutine.ChangeType(RotationType.AOE);
                if (npcCount == 2)
                    combatRoutine.ChangeType(RotationType.SingleTargetCleave);
                if (npcCount <= 1)
                    combatRoutine.ChangeType(RotationType.SingleTarget);
            }
        }
        /// <summary>
        /// elemental rotation stuff
        /// </summary>
        /// <returns></returns>
        private async Task ElementalConsistantUse()
        {
            if (PlayerSpec == "Elemental")
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
                if (WoW.Talent(6) != 2 && WoW.CanCast("Fire Elemental", true, true, false, false, true) )
                {
                    WoW.CastSpell("Fire Elemental");
                    return;
                }
                //ations +=/ storm_elemental
                if (WoW.Talent(6) == 2 && WoW.CanCast("Storm Elemental", true, true, false, false, true) )
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
                    && WoW.Maelstrom >= 86 && TargetInfo.Range)
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
                    if (WoW.CanCast("Liquid Magma", true, true, false, false, true) && WoW.Talent(6) == 1 && TargetInfo.Range)
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
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.PlayerHasBuff("Echoes of the Great") && TargetInfo.Range)
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
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.TargetHasDebuff("Echoes of the Great") && WoW.Maelstrom >= 86 && TargetInfo.Range)
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
                    if (WoW.CanCast("Liquid Magma", true, true, false, false, true) && WoW.Talent(6) == 1 && TargetInfo.Range)
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
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.TargetHasDebuff("Echoes of the Great") && TargetInfo.Range)
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
                     && (WoW.IsSpellOnCooldown("Lava Burst") && !WoW.PlayerHasBuff("Stormkeeper")) )
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
                    if (WoW.CanCast("Liquid Magma", true, true, false, false, true) && WoW.Talent(6) == 1 && TargetInfo.Range)
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
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.TargetHasDebuff("Echoes of the Great") && TargetInfo.Range)
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
                    if (WoW.Talent(7) == 1 && WoW.CanCast("AscendanceEle", true, true, false, false, true) )
                    {
                        WoW.CastSpell("AscendanceEle");
                        return;
                    }
                    //actions.aoe +=/ liquid_magma_totem
                    if (WoW.Talent(6) == 1 && WoW.CanCast("Liquid Magma", true, true, false, false, true) && TargetInfo.Range)
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
                    if (WoW.CanCast("Earthquake", true, true, false, false, true) && WoW.Maelstrom >= 50 )
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
                    && WoW.Talent(7) != 7 && WoW.Talent(5) == 3 && combatRoutine.Type == RotationType.AOE && npcCount < 5 && !WoW.IsMoving)
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
                if (PlayerSpec == "Enhancement")
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
                      && (WoW.PlayerBuffTimeRemaining("Boulderfist") <= GCD || !WoW.PlayerHasBuff("Boulderfist"))
                   || (WoW.Maelstrom <= 50 && combatRoutine.Type == RotationType.AOE))
                    {
                        Log.Write("Buff Time :" + WoW.PlayerBuffTimeRemaining("Boulderfist") + " GCD: " + GCD + "Has buff :" + WoW.PlayerHasBuff("Boulderfist"));
                        WoW.CastSpell("Boulderfist"); //boulderfist it to not waste a charge
                        return;
                    }
                    //actions +=/ boulderfist,if= buff.boulderfist.remains < gcd | (charges_fractional > 1.75 & maelstrom <= 100 & active_enemies <= 2)
                    if (WoW.Talent(1) == 3 && WoW.CanCast("Boulderfist", true, true, true, false, true)
                    && WoW.Maelstrom <= 100 && (WoW.PlayerSpellCharges("Boulderfist") >= 2
                    || WoW.PlayerSpellCharges("Boulderfist") + (((600 / (1 + WoW.HastePercent/ 100)) - (WoW.SpellCooldownTimeRemaining("Boulderfist"))) / (600 / (1 + WoW.HastePercent / 100))) > 1.75)
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
                    //actions +=/ frostbrand,if= talent.hailstorm.enabled & buff.frostbrand.remains < gcd
                    if (WoW.Talent(4) == 3 && WoW.Maelstrom >= 20 && WoW.CanCast("Hailstorm", true, true, true, false, true)
                        && (!WoW.PlayerHasBuff("Frostbrand") || WoW.PlayerBuffTimeRemaining("Frostbrand") < GCD))
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
                    && ((combatRoutine.Type == RotationType.AOE || combatRoutine.Type == RotationType.SingleTargetCleave || WoW.Talent(6) == 1 || WoW.Talent(1) == 3) && !setBonus4Pc || Pets.IsRunning && Pets.Elapsed.Seconds > 0 && Pets.Elapsed.Seconds <= 10))
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
                    //actions +=/ lava_lash,if= talent.fury_of_air.enabled & talent.overcharge.enabled & (set_bonus.tier19_4pc & maelstrom >= 80)
                    if (WoW.CanCast("Lava Lash", true, true, true, false, true)
                        && (WoW.Talent(6) == 2 && WoW.Talent(5) == 2 && setBonus4Pc && WoW.Maelstrom >= 80))
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }

                    //actions +=/ lava_lash,if= talent.fury_of_air.enabled & !talent.overcharge.enabled & (set_bonus.tier19_4pc & maelstrom >= 53)
                    if (WoW.CanCast("Lava Lash", true, true, true, false, true)
                    && (WoW.Talent(6) == 2 && WoW.Talent(5) != 2 && setBonus4Pc && WoW.Maelstrom >= 53))
                    {
                        WoW.CastSpell("Lava Lash");
                        return;
                    }
                    //actions +=/ lava_lash,if= (!set_bonus.tier19_4pc & maelstrom >= 120) | (!talent.fury_of_air.enabled & set_bonus.tier19_4pc & maelstrom >= 40)
                    //actions +=/ lava_lash,if= (!set_bonus.tier19_4pc & maelstrom >= 120) | (!talent.fury_of_air.enabled & set_bonus.tier19_4pc & maelstrom >= 40)
                    if (WoW.CanCast("Lava Lash", true, true, true, false, true)
                    && (!setBonus4Pc && WoW.Maelstrom >= 120 || WoW.Talent(6) != 2 && WoW.Maelstrom >= 40 && setBonus4Pc))
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
                if (PlayerSpec == "Enhancement" && !TargetInfo.Melee)
                    WoW.KeyPressRelease(WoW.Keys.Tab);
                if (PlayerSpec == "Elemental " && !TargetInfo.Range)
                    WoW.KeyPressRelease(WoW.Keys.Tab);
            });
        }
        public override void Pulse()
        {
            AsyncPulse();
                       //DBMPrePull();
                TimerReset();
                if (WoW.IsInCombat && !WoW.IsMounted)
                {
                    for (int i = 1; i < 2; i++)
                    {
                        SelectRotation();
                        interruptcast();
                        //Stuns();
                        Defensive();
                        DpsRotationSelection();
                        Thread.Sleep((int.Parse(ConfigFile.Pulse.ToString()) / 3));
                    }
                }
        }
        /// <summary>
        /// Cooldown function/mount/Buff/stack/Remaining time
        /// </summary>
        /// 
        /// 
        private static bool setBonus2Pc
        {
            get
            {
                var control = WoW.GetBlockColor(9, 24);
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(control.G))) == 0 && Convert.ToInt32(Math.Round(Convert.ToSingle(control.R) * 100 / 255)) >= 2)
                {

                    return true;
                }
                else
                    return false;
            }
        }
        private static bool setBonus4Pc
        {
            get
            {
                var control = WoW.GetBlockColor(9, 24);
                //Log.Write("Bonus location: " + Convert.ToInt32(Math.Round(Convert.ToSingle(control.R) * 100 / 255)));
                if (Convert.ToInt32(Math.Round(Convert.ToSingle(control.G))) == 0 && Convert.ToInt32(Math.Round(Convert.ToSingle(control.R) * 100 / 255)) >= 4)
                {

                    return true;
                }
                else
                    return false;
            }
        }

        public static string PlayerSpec
        {
            get
            {
                var c = WoW.GetBlockColor(9, 24);
               
                try
                {

                    if (c.B != 0) return "none";
                     string[] Spec = new string[]
                    {"None","Blood", "Frost", "Unholy", "Havoc", "Vengeance", "Balance", "Feral", "Guardian", "Restoration", "Beast Mastery", "Marksmanship", "Survival", "Arcane", "Fire", "Frost", "Brewmaster", "Mistweaver", "Windwalker", "Holy", "Protection", "Retribution", "Discipline", "HolyPriest", "Shadow", "Assassination", "Outlaw", "Subtlety", "Elemental", "Enhancement", "RestorationShaman", "Affliction", "Arms", "Fury", "Protection","Demonology","Destruction","none"};
                    Log.WriteDirectlyToLogFile("Green = "+ c.G);
                    var race = Convert.ToInt32(Math.Round(Convert.ToSingle(c.G)));
                    var spec = Spec[race];

                    return spec;
                }
                catch (Exception ex)
                {
                    Log.Write("Error in Spec  Green = "+c.G);

                    Log.Write(ex.Message, Color.Red);
                }
                return "none";
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
        private const string partyvarable = @"local sentTime =0 
local timeDiff =0
local PartyBuffs = {}
do
for i = 1, 4 do
PartyBuffs[i] ={Buffs = {} ,LastStateBuff = {} ,debuffs = {},LaststateDebuff = {} }
end
end
";
        private const string partybuffdebuff = @"local function updatePartyBuffs()
	for _, auraId in pairs(buffs) do
           local auraName = GetSpellInfo(auraId)
    for i = 1, 4 do
        if auraName == nil then
            if ( PartyBuffs[z].BuffLast[auraId] ~= ""BuffOff"") then
                 PartyBuffs[z].BuffLast[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                 PartyBuffs[z].BuffLast[auraId].t:SetAllPoints(false)
                 PartyBuffs[z].BuffLast[auraId] = ""BuffOff""          
            end    
            return
        end

        local name, _,_, count, _, duration, expires, _, _, _, spellID, _, _, _, _, _, _, _, _ = UnitBuff(""party""..i, auraName)

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list
            local getTime = GetTime()

            local remainingTime = math.floor(expires - getTime + timeDiff)

			if (PartyBuffs[z].BuffLast[auraId] ~= ""BuffOn"" .. count..remainingTime) then
                    local green = 0
					local blue = 0

                    local strcount = ""0.0""..count;
                    local strbluecount = ""0.0""..remainingTime;
						
				if(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then
                    blue = 0
                    strbluecount = 0
				end

				if (count >= 10) then
                    strcount = ""0.""..count;
                end

				if(remainingTime >= 10) then
                   strbluecount = ""0.""..remainingTime;
                end

                green = tonumber(strcount)
                blue = tonumber(strbluecount)


                PartyBuffs[z].Buff[auraId].t:SetColorTexture(0, green, blue, alphaColor)
                PartyBuffs[z].BuffLast[auraId].t:SetAllPoints(false)

                PartyBuffs[z].BuffLast[auraId] = ""BuffOn"" .. count..remainingTime
            end
        else
            if (PartyBuffs[z].BuffLast[auraId] ~= ""BuffOff"") then
                PartyBuffs[z].Buff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                PartyBuffs[z].Buff[auraId].t:SetAllPoints(false)
                PartyBuffs[z].BuffLast[auraId] = ""BuffOff""              
            end
        end
    end
    end
end

local function updatePartyDebuffs(self, event)
    
	for _, debuffId in pairs(debuffs) do
    for i =1, 4 do
        local auraName = GetSpellInfo(auraId)
   
        if auraName == nil then
            if (PartyBuffs[z].debuffs[debuffId] ~= ""DebuffOff"") then
                 PartyBuffs[z].debuffs[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)
                 PartyBuffs[z].debuffs[debuffId].t:SetAllPoints(false)
                PartyBuffs[z].debuffs[debuffId] = ""DebuffOff""               
            end
    
            return
        end
        
		--print(""Getting debuff for Id = "" .. auraName)


        local name, _, _, _, _, _, expirationTime, _, _, _, spellId, _, _, _, _, _ = UnitDebuff(""party""..i, auraName, nil, ""PLAYER|HARMFUL"")

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list
            local getTime = GetTime()
            local remainingTime = math.floor(expirationTime - getTime + 0.5)

			if (PartyBuffs[z].debuffs[debuffId] ~= ""DebuffOn"" .. count..remainingTime) then
                local green = 0
                local blue = 0
                local strcount = ""0.0""..count;
                local strbluecount = ""0.0""..remainingTime;
                
                if (count >= 10) then
                    strcount = ""0.""..count;
                end

                if(remainingTime >= 10) then
                   strbluecount = ""0.""..remainingTime
                end

                green = tonumber(strcount)
                blue = tonumber(strbluecount)

                PartyBuffs[z].debuffs[debuffId].t:SetColorTexture(0, green, blue, alphaColor)

                PartyBuffs[z].debuffs[debuffId].t:SetAllPoints(false)
                --print(""["" .. buff.. ""] "" .. auraName.. "" "" .. count.. "" Green: "" .. green.. "" Blue: "" .. blue)
               PartyBuffs[z].debuffslast[debuffId] = ""DebuffOn"" .. count..remainingTime
            end
        else
            if ( PartyBuffs[z].debuffslast[debuffId] ~= ""DebuffOff"") then
                PartyBuffs[z].debuffs[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)
                PartyBuffs[z].debuffs[debuffId].t:SetAllPoints(false)
                PartyBuffs[z].debuffslast[debuffId] = ""DebuffOff""
                --print(""["" .. buff.. ""] "" .. auraName.. "" Off"")
            end
        end
    end
    end
end

";
        private const string partybuff = @"	raidHealthFrame[i]:SetScript(""OnUpdate"", updateRaidHealth)
	end
	
	for z =1, 4  do
			i=0
		for _, buffId in pairs(buffs) do

			 PartyBuffs[z].Buffs[buffId] = CreateFrame(""frame"","""", parent)
			 PartyBuffs[z].Buffs[buffId]:SetSize(size, size)
	         PartyBuffs[z].Buffs[buffId]:SetPoint(""TOPLEFT"", i * size, -size * (11+z))                            -- column 13 [Target Buffs]
			 PartyBuffs[z].Buffs[buffId].t = PartyBuffs[z].Buffs[buffId]:CreateTexture()
	         PartyBuffs[z].Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)
		     PartyBuffs[z].Buffs[buffId].t:SetAllPoints(  PartyBuffs[z].Buffs[buffId])
			PartyBuffs[z].Buffs[buffId]:Show()
			PartyBuffs[z].Buffs[buffId]:SetScript(""OnUpdate"", updatePartyBuffs)
        i=i+1
		end
	end
    i=0
	for _, debuffId in pairs(debuffs) do
		for z=1, 4 do 
			PartyBuffs[z].debuffs[debuffId] = CreateFrame(""frame"","""", parent)
			PartyBuffs[z].debuffs[debuffId]:SetSize(size, size)
			PartyBuffs[z].debuffs[debuffId]:SetPoint(""TOPLEFT"", i * size, -size * (15+z))         -- row 4, column 1+ [Spell In Range]
			PartyBuffs[z].debuffs[debuffId].t = PartyBuffs[z].debuffs[debuffId]:CreateTexture()        
			PartyBuffs[z].debuffs[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)
			PartyBuffs[z].debuffs[debuffId].t:SetAllPoints(PartyBuffs[z].debuffs[debuffId])
			PartyBuffs[z].debuffs[debuffId]:Show()		               
			PartyBuffs[z].debuffs[debuffId]:SetScript(""OnUpdate"", updatePartyDebuffs)
		end
        i=i+1
	end
end
";

        private const string CustomLua = @"local Healingbuffs =  ""Riptide""
local Spec = {
	[250] = 0.01,
	[251] = 0.02,
	[252] =0.03,
	[577] = 0.04,
	[581] =0.05,
	[102] = 0.06,
	[103] =0.07,
	[104] =0.08,
	[105] =0.09,
	[253] = 0.10,
	[254] =0.11,
	[255] = 0.12,
	[62] =0.13,
	[63] = 0.14,
	[64] =0.15,
	[268] = 0.16,
	[269] =0.17,
	[270] = 0.18,
	[65] = 0.19,
	[66] = 0.20,
	[70] =0.21,
	[256] = 0.22,
	[257]=.23,
	[258] =0.24,
	[259] =0.25,
	[260] =0.26,
	[261] = 0.27,
	[262] = 0.28,
    [263] = 0.29,
	[264] = 0.30,
	[265] = 0.31,
	[71] = 0.32,
	[72] = 0.33,
	[73] = 0.34,
	[266] = 0.35,
    [267] = 0.36,
}

local activeUnitPlates = {}
local RaidBuffFrame = {}
local raidSizeFrame = nil
local PlayerStatFrame = {}
local targetInfoFrame = nil
local timerDBMFrames = {}
local PlatesOn = 0
local party_units = {}
local raid_units = {}
local raidheal_cache = {}
local raidHealthFrame = {}
local RaidRole = {}
local RaidRange = {}
local lasthp = {}
local castPCT = 0
local charUnit = {}
local lasthaste = 0
local raidBuff = {}
local raidBufftime = {}
local partySize = 0
local setBonusFrame = nil
local debufftarget = {locX = 0,locY = 30, debufftargetframes = {},dispellType = {
	[250] = {type1= 'Magic'},
	[251] = {type1= 'Magic'},
	[252] = {type1= 'Magic'},
	[577] = {type1= 'Magic'},
	[581] = {type1= 'Magic'},
	[102] = {type1= 'Magic'},
	[103] = {type1= 'Magic'},
	[104] = {type1= 'Magic'},
	[105] = {type1= 'Magic'},
	[253] =  {type1= 'Magic'},
	[254] = {type1= 'Magic'},
	[255] =  {type1= 'Magic'},
	[62] = {type1= 'Magic'},
	[63] =  {type1= 'Magic'},
	[64] = {type1= 'Magic'},
	[268] = {type1= 'Magic'},
	[269] = {type1= 'Magic'},
	[270] = {type1= 'Magic'},
	[65] = {type1= 'Magic'},
	[66] = {type1= 'Magic'},
	[70] = {type1= 'Magic'},
	[256] = {type1= 'Magic'},
	[257]= {type1= 'Magic'},
	[257] = {type1= 'Magic'},
	[259] = {type1= 'Magic'},
	[260] = {type1= 'Magic'},
	[261] =  {type1= 'Magic'},
	[262] =  {type1= 'Magic'},
    [263] = {type1= 'Magic'},
	[264] = {type1= 'Magic'},
	[265] =  {type1= 'Magic'},
	[71] = {type1= 'Magic'},
	[72] = {type1= 'Magic'},
	[73] = {type1= 'Magic'},
	[266] = {type1= 'Magic'},
    [267] = {type1= 'Magic'}},
	debuff = {targetdebuff}}
local debuffraid = {locX = 0,locY = 30, debuffraidframes = {},
dispellType = {
	[250] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[251] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[252] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[577] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[581] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[102] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[103] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[104] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[105] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[253] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[254] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[255] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[62] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[63] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[64] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[268] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[269] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[270] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[65] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[66] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[70] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[256] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[257]= {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[257] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[259] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[260] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[261] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[262] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
    [263] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[264] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[265] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[71] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[72] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[73] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[266] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
    [267] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""}},
	debuff = {}}



local frame = CreateFrame(""frame"", """", parent)
frame:RegisterEvent(""NAME_PLATE_UNIT_ADDED"")
frame:RegisterEvent(""UNIT_HEALTH_FREQUENT"")
frame:RegisterEvent(""RAID_ROSTER_UPDATE"")
frame:RegisterEvent(""GROUP_ROSTER_UPDATE"")
frame:RegisterUnitEvent(""UNIT_SPELL_HASTE"",""player"")
frame:RegisterUnitEvent(""UNIT_POWER"",""player"")
frame:RegisterEvent(""PLAYER_REGEN_DISABLED"")
frame:RegisterEvent(""PLAYER_REGEN_ENABLED"")
frame:RegisterEvent(""PLAYER_ENTERING_WORLD"")
frame:RegisterEvent(""CHAT_MSG_ADDON"")
frame:RegisterUnitEvent(""UNIT_HEALTH"",""player"")
frame:RegisterEvent(""PLAYER_EQUIPMENT_CHANGED"")
frame:RegisterEvent(""PLAYER_TARGET_CHANGED"")
frame:RegisterEvent(""PLAYER_ENTER_COMBAT"")
frame:RegisterEvent(""PLAYER_LEAVE_COMBAT"")
frame:RegisterEvent(""PLAYER_CONTROL_LOST"")
frame:RegisterEvent(""PLAYER_CONTROL_GAINED"")
frame:RegisterEvent(""ACTIONBAR_UPDATE_STATE"")
frame:RegisterUnitEvent(""UNIT_SPELLCAST_START"",""player"")
frame:RegisterEvent(""CURRENT_SPELL_CAST_CHANGED"")
frame:RegisterUnitEvent(""UNIT_SPELLCAST_SUCCEEDED"",""player"")

local function updateFlag(self, event)
	if event == ""PLAYER_CONTROL_GAINED"" then
		flagFrame.t:SetColorTexture(0,0,0,alphaColor)
	end
	if event == ""PLAYER_CONTROL_LOST"" then
		flagFrame.t:SetColorTexture(1,0,0,alphaColor)
	end
end

local function UpdateMana()
		charUnit[1] = UnitPower(""player"",0) / UnitPowerMax(""player"",0)
        PlayerStatFrame[1].t:SetColorTexture(0, charUnit[2],charUnit[1], alphaColor)
end



local function CharRaceUpdate()
 	  charUnit[2] = Spec[select(1, GetSpecializationInfo(GetSpecialization()))]
end


local function UnitIsPartyUnit(unit)
	--print(""checking :"", unit)
	for _, v in next, party_units do
		if unit == v then return true end
	end
end

local function UnitIsRaidUnit(unit)
	for _, v in next, raid_units do
		if unit == v then return true end
	end
end

local function HealthChangedEvent(unit)
	local h = UnitHealth(unit)
	if h==lasthp[unit] then return end
	lasthp[unit]=h
	local m = UnitHealthMax(unit);
	h = (h / m)
	raidheal_cache[unit] = h
end

local function RangeCheck(unit)
	if LibStub(""SpellRange-1.0"").IsSpellInRange(""Healing Wave"", unit) == 1 then
		RaidRange[unit] = 1;
	else
		RaidRange[unit] = .5;
	end
end

local function RaidRoleCheck(unit)
	if UnitGroupRolesAssigned(unit) == ""TANK"" then
		RaidRole[unit] = 1;
	elseif UnitGroupRolesAssigned(unit) == ""HEALER"" then
		RaidRole[unit] = .5;
	else
		RaidRole[unit] = 0;
	end
end

local function UpdateRaidIndicators(unit)
	if UnitIsRaidUnit(unit) or UnitIsPartyUnit(unit) then
		if UnitInParty (""player"") and not UnitInRaid (""player"") then
		--print(unit,""needs heals"")
			for i, key in pairs(party_units) do
				if key == unit then
					HealthChangedEvent(unit)
					RangeCheck(unit)
					RaidRoleCheck(unit)
					--print(unit, ""is at :"", raidheal_cache[unit])
					raidHealthFrame[i].t:SetColorTexture(raidheal_cache[unit], RaidRange[unit], RaidRole[unit], alphaColor)
				end	
			end
		end
		if UnitInRaid (""player"") then 
			for i, key in pairs(raid_units) do
				if key == unit then
					HealthChangedEvent(unit)
					RangeCheck(unit)
					RaidRoleCheck(unit)
					--print(unit, ""is at :"", raidheal_cache[unit], "" and At : "", i)
					raidHealthFrame[i].t:SetColorTexture(raidheal_cache[unit], RaidRange[unit], RaidRole[unit], alphaColor)
				end	
			end
		end
		if not UnitInRaid (""player"") and not UnitInParty (""player"") then
			for i = 1, 30 do
				raidHealthFrame[i].t:SetColorTexture(1, 0, 0, alphaColor)
			end
		end
	end
end

local function updateTotemsFrame()
	Totemsframe = 0
	TotemDuration = 0 
	for i = 1, 4 do
		haveTotem, name, startTime, duration, icon = GetTotemInfo(i)
		local Quesatime =  startTime + duration
		if haveTotem then
			if (name == ""Spirit Wolf"" or name == ""Totem Mastery"")
			and(startTime + duration - GetTime() > 1.5 ) then
				Totemsframe = 1;
				TotemDuration = startTime + duration - GetTime()
			end
		end
	end
	totemsFrame.t:SetColorTexture(Totemsframe, TotemDuration,0, alphaColor)
	
end


local function UpdateRaidBuffIndicators(unit)
		if select(7, UnitBuff(unit, Healingbuffs, ""player"")) == nil  then return end
		UpdateRaidBuffslot(unit,expires)
end

local function UpdateRaidBuffslot(unit,expires)
	for i = 1, 4 do 
		if raidBuff[i] == 0 then
			local slot = string.match (unit, ""%d+"")
			UpdateBuffTime(unit,expires,i)
			if i >= 10 then
				raidBuff[i] = tonumber(""0."" .. slot)
			else 
				raidBuff[i] = tonumber(""0.0"" .. slot)
			end
		end
	end
end

local function UpdateBuffTime(unit,expires,location)
	local remainingTime = math.floor(expires -  GetTime() + 0.5)
	if(remainingTime >= 10) then
		raidBufftime[i] = tonumber(""0.""..remainingTime);
	else
		raidBufftime[i] = tonumber(""0.0""..remainingTime);
	end
end

local function updateRaidBuff(self, event)
	if not UnitInRaid (""player"") and  UnitInParty (""player"") then
		for key, _ in pairs(party_units) do
			UpdateRaidBuffIndicators(key)
		end
	end	
	if UnitInRaid (""player"") then
		for key, _ in pairs(raid_units) do
			UpdateRaidBuffIndicators(key)
		end
	end
	for i=1, 4 do 
		if raidBuff[i] ~= nil then
			RaidBuffFrame[i].t:SetColorTexture(raidBuff[i], 1, raidBufftime[i], alphaColor)
			RaidBuffFrame[i].t:SetAllPoints(false)
		else
			RaidBuffFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
			RaidBuffFrame[i].t:SetAllPoints(false)
			raidBuff[i] = nil
			raidBufftime[i] = nil
		end
	end
end
local function TurnOnPlates()
	if GetCVar(""nameplateShowEnemies"") == ""1"" then
		PlatesOn = 1
	else
		PlatesOn = 0
	end
end
local enemiesPlate = 0
local function activeEnemies()
    enemiesPlate = 0
    for k, v in pairs(activeUnitPlates) do
        if v ~= nil then
            if UnitCanAttack(""player"", k) 
            and (LibStub(""SpellRange-1.0"").IsSpellInRange(""Death Strike"", k) == 1 and charUnit[3] == .01 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Frost Strike"", k) == 1 and charUnit[3] == .02 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Festering Strike"", k) == 1 and charUnit[3] == .03 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Demon's Bite"", k) == 1 and charUnit[3] == .04 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Shear"", k) == 1 and charUnit[3] == .05 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Solar Wrath"", k) == 1 and charUnit[3] == .06 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Shred"", k) == 1 and charUnit[3] == .07 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Mangle"", k) == 1 and charUnit[3] == .08 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Solar Wrath"", k) == 1 and charUnit[3] == .09 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Counter Shot"", k) == 1 and charUnit[3] == .10 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Counter Shot"", k) == 1 and charUnit[3] == .11 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Mongoose Bite"", k) == 1 and charUnit[3] == .12 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Arcane Blast"", k) == 1 and charUnit[3] == .13 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Fireball"", k) == 1 and charUnit[3] == .14 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Frostbolt"", k) == 1 and charUnit[3] == .15 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Blackout Strike"", k) == 1 and charUnit[3] == .16 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Rising Sun Kick"", k) == 1 and charUnit[3] == .17 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Rising Sun Kick"", k) == 1 and charUnit[3] == .18 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Crusader Strike"", k) == 1 and charUnit[3] == .19 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Shield of the Righteous"", k) == 1 and charUnit[3] == .20 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Crusader Strike"", k) == 1 and charUnit[3] == .21 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Penance"", k) == 1 and charUnit[3] == .22 or
			
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Smite"", k) == 1 and charUnit[3] == .23 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Mind Blast"", k) == 1 and charUnit[3] == .24 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Mutilate"", k) == 1 and charUnit[3] == .25 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Saber Slash"", k) == 1 and charUnit[3] == .26 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Backstab"", k) == 1 and charUnit[3] == .27 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Lightning Bolt"", k) == 1 and charUnit[3] == .28 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Rockbiter"", k) == 1 and charUnit[3] == .29 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Lightning Bolt"", k) == 1 and charUnit[3] == .30 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Agony"", k) == 1 and charUnit[3] == .31 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Mortal Strike"", k) == 1 and charUnit[3] == .32 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Bloodthirst"", k) == 1 and charUnit[3] == .33 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Devastate"", k) == 1 and charUnit[3] == .34 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Doom"", k) == 1 and charUnit[3] == .35 or
			LibStub(""SpellRange-1.0"").IsSpellInRange(""Incinerate"", k) == 1 and charUnit[3] == .36) and
            UnitIsDead(k) == false and UnitAffectingCombat(k) then
                enemiesPlate = enemiesPlate + 1
            --print(""what is k:"",k,""Is "",v,"" In range "",LibStub(""SpellRange-1.0"").IsSpellInRange(""Mongoose Bite"", v),"" Is unit dead :"",UnitIsDead(v))
        
            end
        end
    end
    enemiesPlate = enemiesPlate/100
    
end
local function NameplateFrameUPDATE()
	activeEnemies()
	TurnOnPlates()
	--TurnOnPlates()
	--print(""Npc counT"",enemiesPlate)
	raidSizeFrame.t:SetColorTexture(partySize, enemiesPlate, PlatesOn, alphaColor)
end

local function AddNameplate(unitID)
	local GUID = false
	for k,v in pairs(activeUnitPlates)do
		 activeUnitPlates[k] = nil
	end	
	for i=1, 30 do
		if	UnitGUID (""nameplate""..i) ~= nil and not UnitIsPlayer(""nameplate"".. i) then
			for k, v in pairs(activeUnitPlates) do
			--print(""Nameplate :"",""nameplate""..i,""GUID"",UnitGUID (""nameplate""..i)  )
				if v == UnitGUID (""nameplate""..i) then
					GUID = true
				end
			end
			if GUID == false then
				--print(""SET Nameplate :"",""nameplate""..i,""GUID"",UnitGUID (""nameplate""..i)  )
				activeUnitPlates[""nameplate""..i] = UnitGUID (""nameplate""..i)
				GUID = false
			end
		end
		GUID = false
	end
end
local function ClearNamePlates()
	for k,v in pairs(activeUnitPlates)do
		if  k ~=UnitGUID(""target"") then 
			activeUnitPlates[k] = nil
		end 
	end	
end


local function register_unit(tbl, unit)
		table.insert(tbl, unit)
end
do
	for i = 1, 5 do
		register_unit(party_units, (""party%d""):format(i))
	end
	for	i = 1,30 do
		register_unit(raid_units, (""raid%d""):format(i))
	end
end
local invSlots = {
1,3,5,7,10,15
}
local setItems = {
	--shaman
	138343, 138348, 138372, 138346, 138341, 138345
}
local function updateSetBonus()
	count = 0
	for _, v in pairs(invSlots) do
		itemID = GetInventoryItemID(""player"", v);
		for _,z in pairs(setItems) do
			if(itemID == z)then
			--print(v, "" "", z)
				count= count +1
			end
		end

	end
	setBonusFrame.t:SetColorTexture(count/100, 0, 0, alphaColor)
end

local function UdateRaidSizeFrame(self, event)
	partySize = GetNumGroupMembers() ;
	TurnOnPlates()
	partySize = partySize /100
	--print(""Party Size: "",partySize)
	if partySize > .30 then
		partySize = .30
	end
	--print(""Partyupdate :"",partySize)
	--print(""Name plates :"", PlatesOn)
	raidSizeFrame.t:SetColorTexture(partySize, enemiesPlate, PlatesOn, alphaColor)
end
local DBMTIMER = {}

local function updateDBMFrames(elapsed)
	if(pullvalue and tickerdbm >= 0) then
		tickerdbm = tickerdbm - elapsed
		timerDBMFrames.t:SetColorTexture( 1,tickerdbm/10,0, alphaColor)
	else
		timerDBMFrames.t:SetColorTexture( 0,0,0, alphaColor)
	end
end
local timer 
local function DBMPull(prefix,msg,sender)
	_, _,_,_,_, _,_, instanceMapID, _ = GetInstanceInfo()
	print(""DBM Time : "",msg)
	if prefix == ""D4"" and select(1,strsplit(""\t"", msg)) == ""PT""
    and (UnitInRaid(Ambiguate(sender, ""short"")) or UnitInParty ( Ambiguate (sender, ""short"") ) )
   	and tonumber ( select(3, strsplit(""\t"", msg) ) ) == instanceMapID  then
		local time = select(2,strsplit(""\t"", msg) )
		time =  tonumber(time)


		if time ~= 0 then
			print(""DBM pull timer"")
			tickerdbm = time
			pullvalue = true
		end
		if time == 0 then
			tickerdbm = time
			pullvalue = false
		end

	end
end
local function updatetargetInfoFrame()
	targetexist = 0
	rangetargetexist = 0
	if UnitExists(""target"") then
		if	LibStub(""SpellRange-1.0"").IsSpellInRange(""Rockbiter"", ""target"") == 1 and charUnit[3] == .29  then
			targetexist = 1
		end
		if LibStub(""SpellRange-1.0"").IsSpellInRange(""Lightning Bolt"", ""target"") == 1 and charUnit[3] == .28 then
			targetexist = 1
		end
	end
	if UnitExists(""mouseover"") and UnitAffectingCombat(""mouseover"") and LibStub(""SpellRange-1.0"").IsSpellInRange(""Lightning Bolt"", ""mouseover"") == 1 then
			rangetargetexist = 1
	end
	
	--print(""Info "", targetexist, "" "", rangetargetexist )
	targetInfoFrame.t:SetColorTexture(targetexist,rangetargetexist, 0 ,alphaColor)
end

local function InitializeFour()
	for i = 1, 4 do 
		raidBuff[i] = 1
		raidBufftime[i] =1
	end
		--print (""Initialising raid Health Frames"")
	for i = 1, 20 do	
		raidHealthFrame[i] = CreateFrame(""frame"", """", parent)
		raidHealthFrame[i]:SetSize(size, size)
		raidHealthFrame[i]:SetPoint(""TOPLEFT"", size*(i-1), -size *21 )   --  row 1-20,  column 19
		raidHealthFrame[i].t = raidHealthFrame[i]:CreateTexture()        
		raidHealthFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		raidHealthFrame[i].t:SetAllPoints(raidHealthFrame[i])
		raidHealthFrame[i]:Show()
	end
	for i = 21, 30 do		
		raidHealthFrame[i] = CreateFrame(""frame"", """", parent)
		raidHealthFrame[i]:SetSize(size, size)
		raidHealthFrame[i]:SetPoint(""TOPLEFT"", size*(i-21), -size *22 )   --  row 1-10,  column 20
		raidHealthFrame[i].t = raidHealthFrame[i]:CreateTexture()        
		raidHealthFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		raidHealthFrame[i].t:SetAllPoints(raidHealthFrame[i])
		raidHealthFrame[i]:Show()
	end
		raidSizeFrame = CreateFrame(""frame"", """", parent)
		raidSizeFrame:SetSize(size, size)
		raidSizeFrame:SetPoint(""TOPLEFT"", size*(10), -size *22 )   --  row 11,  column 20
		raidSizeFrame.t = raidSizeFrame:CreateTexture()        
		raidSizeFrame.t:SetColorTexture(1, 1, 1, alphaColor)
		raidSizeFrame.t:SetAllPoints(raidSizeFrame)
		raidSizeFrame:Show()
		raidSizeFrame:SetScript(""OnUpdate"",NameplateFrameUPDATE)
		
	for i = 1, 4 do		
		RaidBuffFrame[i] = CreateFrame(""frame"", """", parent)
		RaidBuffFrame[i]:SetSize(size, size)
		RaidBuffFrame[i]:SetPoint(""TOPLEFT"", size*(10 + i), -size *22 )   --  row 12-15,  column 20
		RaidBuffFrame[i].t = RaidBuffFrame[i]:CreateTexture()        
		RaidBuffFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		RaidBuffFrame[i].t:SetAllPoints(RaidBuffFrame[i])
		RaidBuffFrame[i]:Show()
		
	end
		timerDBMFrames = CreateFrame(""frame"", """", parent)
		timerDBMFrames:SetSize(size, size);
		timerDBMFrames:SetPoint(""TOPLEFT"", size * 5, -(size * 23))           -- column 6 row 21
		timerDBMFrames.t = timerDBMFrames:CreateTexture()        
		timerDBMFrames.t:SetColorTexture(0, 0, 0, alphaColor)
		timerDBMFrames.t:SetAllPoints(timerDBMFrames)
		timerDBMFrames:Show()	


		totemsFrame = CreateFrame(""frame"", """", parent)
		totemsFrame:SetSize(size, size);
		totemsFrame:SetPoint(""TOPLEFT"", size * 6, -(size * 23))           -- column 7 row 21
		totemsFrame.t = totemsFrame:CreateTexture()        
		totemsFrame.t:SetColorTexture(0, 0, 0, alphaColor)
		totemsFrame.t:SetAllPoints(totemsFrame)
		totemsFrame:Show()

		targetInfoFrame = CreateFrame(""frame"", """", parent)
		targetInfoFrame:SetSize(size, size);
		targetInfoFrame:SetPoint(""TOPLEFT"", size * 7, -(size * 23))           -- column 8 row 21
		targetInfoFrame.t = targetInfoFrame:CreateTexture()        
		targetInfoFrame.t:SetColorTexture(0, 0, 0, alphaColor)
		targetInfoFrame.t:SetAllPoints(targetInfoFrame)
		targetInfoFrame:Show()

		setBonusFrame = CreateFrame(""frame"", """", parent)
		setBonusFrame:SetSize(size, size);
		setBonusFrame:SetPoint(""TOPLEFT"", size * 8, -(size * 23))           -- column 9 row 21
		setBonusFrame.t = setBonusFrame:CreateTexture()        
		setBonusFrame.t:SetColorTexture(0, 0, 0, alphaColor)
		setBonusFrame.t:SetAllPoints(setBonusFrame)
		setBonusFrame:Show()
	
		PlayerStatFrame[1] = CreateFrame(""frame"", """", parent)
		PlayerStatFrame[1]:SetSize(size, size)
		PlayerStatFrame[1]:SetPoint(""TOPLEFT"", size*(9), -size *23 )   --  row 1-4,  column 21
		PlayerStatFrame[1].t =PlayerStatFrame[i]:CreateTexture()        
		PlayerStatFrame[1].t:SetColorTexture(1, 1, 1, alphaColor)
		PlayerStatFrame[1].t:SetAllPoints(PlayerStatFrame[1])
		PlayerStatFrame[1]:Show()

end

is_casting = false
local function HealinEventHandler(self,event, ...)
    if event == ""NAME_PLATE_UNIT_ADDED"" then
		    AddNameplate(select(1,...))
			activeEnemies()
    end
	if event == ""UNIT_HEALTH_FREQUENT"" then
	
		if (select(1,...) ~= ""player"") then
			UpdateRaidIndicators(select(1,...))
		end
	end
    if event == ""UNIT_SPELLCAST_SUCCEEDED"" then
		if not is_casting then
			for _, spellId in pairs(cooldowns) do
				if spellId == select(5,...) then
                    timeDiff = GetTime() - sendTime
                    timeDiff = timeDiff > select(4, GetNetStats())/ 500  and timeDiff or select(4, GetNetStats())/ 500
				end
            end
        end
    is_casting = false
	end
	if event == ""UNIT_SPELLCAST_START"" then
		if not is_casting then
			for _, spellId in pairs(cooldowns) do
				if spellId == select(5,...) then
                    timeDiff = GetTime() - sendTime
                    timeDiff = timeDiff > select(4, GetNetStats())/ 500  and timeDiff or select(4, GetNetStats())/ 500
				end
            end
        end
    is_casting = false
	end
	if event == ""CURRENT_SPELL_CAST_CHANGED"" then
        sendTime = GetTime()
	end
    if event == ""UNIT_SPELLCAST_FAILED"" then
        is_casting = false
	end
	if event == ""RAID_ROSTER_UPDATE"" or event == ""GROUP_ROSTER_UPDATE"" then
	   UdateRaidSizeFrame()
    end
	if event == ""UNIT_POWER"" then
		UpdateMana()
	end
	if event == ""PLAYER_ENTERING_WORLD""then
        CharRaceUpdate()
		TurnOnPlates()
        updateSetBonus()
	end
	if event == ""PLAYER_REGEN_DISABLED""then 
	    UpdateMana()
		ClearNamePlates()
		CharRaceUpdate()
		updateSetBonus()
	end
	if event == ""PLAYER_REGEN_ENABLED""then 
		ClearNamePlates()
		activeEnemies()
	end
	if event == ""PLAYER_EQUIPMENT_CHANGED"" then
		updateSetBonus()
	end
	if event == ""CHAT_MSG_ADDON""then 
		--select(1,...) --prefix
		--select(2,...) --msg
		--select(4,...) sender
		DBMPull(select(1,...),select(2,...),select(4,...))
	end

	if event == ""PLAYER_TARGET_CHANGED"" then
		updatetargetInfoFrame()
	end
if event == ""PLAYER_ENTER_COMBAT"" or event == ""PLAYER_LEAVE_COMBAT"" then
		updatetargetInfoFrame()
	end

end

local GlobalTimer = 0
local function onUpDateFunction(self,elapsed)
	
			updateRaidBuff()
			updateDBMFrames(elapsed)
            updateSpellInRangeFrames()
			if (classIndex == 6 or                                  -- DeathKnight   
				classIndex == 3 or                                  -- Hunter
				classIndex == 9 or                                  -- Warlock
				classIndex == 8 or                                  -- Mage
				classIndex == 7)                                    -- Shaman (Enh. Needs it for Wolves)
				then
					updateTotemsFrame()
			end
			
end	
frame:SetScript(""OnEvent"",HealinEventHandler)
frame:SetScript(""OnUpdate"",onUpDateFunction)";

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
Aura,239042,Legionfall
Aura,234143,Temptation
Item,142117,Prolonged Power
Item,142173,Collapsing Futures
[Dispell.db]
*/
