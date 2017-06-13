using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CloudMagic.Helpers;
using System.IO;
using System;
using System.Data;
using System.Linq;
using System.Threading;

namespace CloudMagic.Rotation.DeathKnight.DK

{
    public class HolyPalFmFlex : CombatRoutine
    {

        // TALENT PARAMETERS
        private bool isTalentBestowFaith = true;
        private bool isTalentJoL = true;

        // HEALING USAGE PARAMETERS
        private static int minHealingHP = 90;
        private static int minTankPriorityHP = 30;
        private static int minSelfPriorityHP = 20;

        private static int FlashofLightHp = 70;
        private static int HolyLightHp = 90;
        private static int HolyShockHp = 85;
        private static int LightoftheMartyrHp = 40;
        private static int BlessingofProtectionHp = 40;
        private static int BlessingofSacrificeHp = 40;
        private static int LayonHandsHp = 20;
        private static int BestowFaithHp = 80;
        private static int BeaconOfVertueHp = 50;

        // DO NOT TOUCH THESE PARAMETERS
        private int targetPartyID = -1;
        private int targetHealth = minHealingHP;
        private int tankPartyID = -1;
        private int tankPartyID2 = -1;
        private int tankTankingID = -1;
        private int TankRaidCurrentBOL = -1;
        private int partyMemberLow = 0;
        private int partyMemberReallyLow = 0;
        private bool isInRaid = false;
        private int groupSize = 0;
        private int currentMana = -1;
        private int lastCastedID = -1;

        public override string Name
        {
            get
            {
                return "Holy Pal by FmFlex";
            }
        }
        public override string Class
        {
            get
            {
                return "Paladin";
            }
        }
        public override Form SettingsForm { get; set; }

        public override void Initialize()
        {
            Log.Write("Welcome to Holy Pal V2.0 by FmFlex", Color.Green);

        }



        public override void Stop()
        {
        }

        public override void Pulse()
        {
            Log.Write("group size: " + GroupSize() + " is in raid: " + isInRaid);
            tankTankingID = -1;
            groupSize = GroupSize();
            currentMana = WoW.Mana;
            if (isInRaid)
            {
                if (!WoW.PlayerIsCasting && !WoW.PlayerIsChanneling)
                {
                    raidTargetSelector();
                    Log.Write("tankid: " + tankPartyID + " tankid2: " + tankPartyID2 + " tank tanking: " + tankTankingID);
                    if (targetPartyID != -1 && !(tankTankingID != -1 &&TankRaidCurrentBOL != tankTankingID && IsInHealRangeParty(tankTankingID)))
                        TargetOnRaid(targetPartyID);
                    else if (tankTankingID != -1 && IsInHealRangeParty(tankTankingID))
                    {
                        TargetOnRaid(tankTankingID);
                        if (TankRaidCurrentBOL != tankTankingID)
                        {
                            if (!WoW.TargetHasBuff("Beacon of Light") && WoW.CanCast("Beacon of Light", true, true, false, false, false))
                            {
                                WoW.CastSpell("Beacon of Light");
                                TankRaidCurrentBOL = tankTankingID;
                                return;
                            }
                        }
                        if (WoW.CanCast("Holy Shock", true, true, false, false, false) && currentMana >= 40)
                        {
                            WoW.CastSpell("Holy Shock");
                            return;
                        }
                        if (WoW.PlayerHasBuff("Divine Shield") && WoW.CanCast("Light of the Martyr", true, true, false, false, false))
                        {
                            WoW.CastSpell("Light of the Martyr");
                            return;
                        }
                        if (isTalentBestowFaith && WoW.CanCast("Bestow Faith", true, true, false, false, false) && currentMana >= 40)
                        {
                            WoW.CastSpell("Bestow Faith");
                            return;
                        }
                        if (isTalentJoL && isInJudgmentRange() && WoW.CanCast("Judgment", false, true, false, false, false))
                        {
                            WoW.CastSpell("Judgment");
                            return;
                        }
                        if (WoW.CanCast("Holy Light", true, true, false, false, false) && currentMana >= 70)
                        {
                            WoW.CastSpell("Holy Light");
                            lastCastedID = tankTankingID;
                            return;
                        }
                    }
                    if (targetPartyID != -1 && WoW.HasTarget && !WoW.TargetIsEnemy)
                    {
                        lastCastedID = -1;
                        if (targetPartyID == tankTankingID && !WoW.TargetHasBuff("Beacon of Light") && WoW.CanCast("Beacon of Light", true, true, false, false, false))
                        {
                            WoW.CastSpell("Beacon of Light");
                            TankRaidCurrentBOL = tankTankingID;
                            return;
                        }
                        if (targetHealth <= LayonHandsHp && !WoW.TargetHasDebuff("Forbearance") && WoW.CanCast("Lay on Hands", true, true, false, false, false))
                        {
                            WoW.CastSpell("Lay on Hands");
                            return;
                        }
                        if ((targetPartyID == tankPartyID || targetPartyID == tankPartyID2) && targetHealth <= BlessingofSacrificeHp && targetPartyID != 0 && WoW.HealthPercent >= 90 && WoW.CanCast("Blessing of Sacrifice", true, true, false, false, false))
                        {
                            WoW.CastSpell("Blessing of Sacrifice");
                            return;
                        }
                        if (targetHealth <= HolyShockHp && WoW.CanCast("Holy Shock", true, true, false, false, false))
                        {
                            WoW.CastSpell("Holy Shock");
                            return;
                        }
                        if (((targetHealth <= LightoftheMartyrHp && currentMana >= 50 && WoW.HealthPercent >= 85)|| WoW.PlayerHasBuff("Divine Shield")) && WoW.CanCast("Light of the Martyr", true, true, false, false, false))
                        {
                            WoW.CastSpell("Light of the Martyr");
                            return;
                        }
                        
                        if (isTalentBestowFaith && targetPartyID == tankTankingID && WoW.CanCast("Bestow Faith", true, true, false, false, false))
                        {
                            WoW.CastSpell("Bestow Faith");
                            return;
                        }
                        if (isTalentJoL && isInJudgmentRange() && WoW.CanCast("Judgment", false, true, false, false, false))
                        {
                            WoW.CastSpell("Judgment");
                            return;
                        }
                        if (targetHealth <= FlashofLightHp && (partyMemberLow >=3 || targetHealth <= 50) && currentMana >= 30 && WoW.CanCast("Flash of Light", true, true, false, false, false))
                        {
                            WoW.CastSpell("Flash of Light");
                            lastCastedID = targetPartyID; 
                            return;
                        }
                        if (targetHealth <= HolyLightHp && WoW.CanCast("Holy Light", true, true, false, false, false))
                        {
                            WoW.CastSpell("Holy Light");
                            lastCastedID = targetPartyID;
                            return;
                        }
                    }
                    if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                    {
                        if (WoW.CanCast("Judgment", true, true, true, false, true))
                        {
                            WoW.CastSpell("Judgment");
                            return;
                        }
                        if (WoW.CanCast("Holy Shock", true, true, true, false, true))
                        {
                            WoW.CastSpell("Holy Shock");
                            return;
                        }
                    }
                }
                else
                {
                    if(WoW.TargetHealthPercent >= 95 && lastCastedID != tankPartyID && lastCastedID != tankPartyID2)
                    {
                        WoW.CastSpell("stopcasting");
                        return;
                    }
                }
            }
            else
            {
                if (combatRoutine.Type == RotationType.SingleTarget && !WoW.PlayerIsCasting && !WoW.PlayerIsChanneling) // Do Single Target Stuff here
                {
                    partyTargetSelector();
                    if (tankPartyID != -1)
                    {
                        if (!PartyHasBuff("Beacon of Light", tankPartyID) && IsInHealRangeParty(tankPartyID) && WoW.CanCast("Beacon of Light", true, true, false, false, false))
                        {
                            castSpellOnGrp("Beacon of Light", tankPartyID);
                            return;
                        }
                    }
                    if (targetPartyID != -1)
                    {
                        if (targetHealth <= LayonHandsHp && !PartyHasDeBuff("Forbearance", targetPartyID) && WoW.CanCast("Lay on Hands", true, true, false, false, false))
                        {
                            castSpellOnGrp("Lay on Hands", targetPartyID);
                            return;
                        }
                        /*if (targetHealth <= BlessingofProtectionHp && targetPartyID != tankPartyID && !PartyHasDeBuff("Forbearance", targetPartyID) && WoW.CanCast("Blessing of Protection", true, true, false, false, false))
                        {
                            castSpellOnGrp("Blessing of Protection", targetPartyID);
                            return;
                        }*/
                        if (targetPartyID == tankPartyID && targetHealth <= BlessingofSacrificeHp && targetPartyID != 0 && WoW.HealthPercent >= 90 && WoW.CanCast("Blessing of Sacrifice", true, true, false, false, false))
                        {
                            castSpellOnGrp("Blessing of Sacrifice", targetPartyID);
                            return;
                        }
                        /* if (partyMemberReallyLow >= 3 && WoW.CanCast("Tyrs Deliverance", true, true, false, false, false))
                         {
                             WoW.CastSpell("Tyrs Deliverance");
                             return;
                         }*/
                        /* if (partyMemberLow >= 3 && WoW.CanCast("Light of Dawn", true, true, false, false, false))
                         {
                             WoW.CastSpell("Light of Dawn");
                             return;
                         }*/
                        if (targetHealth <= HolyShockHp && WoW.CanCast("Holy Shock", true, true, false, false, false))
                        {
                            castSpellOnGrp("Holy Shock", targetPartyID);
                            return;
                        }
                        if (targetHealth <= LightoftheMartyrHp && WoW.HealthPercent >= 85 && targetPartyID != 0 && WoW.CanCast("Light of the Martyr", true, true, false, false, false))
                        {
                            castSpellOnGrp("Light of the Martyr", targetPartyID);
                            return;
                        }
                        if (isTalentBestowFaith && targetHealth <= BestowFaithHp && WoW.CanCast("Bestow Faith", true, true, false, false, false))
                        {
                            castSpellOnGrp("Bestow Faith", targetPartyID);
                            return;
                        }
                        if (targetHealth <= FlashofLightHp && WoW.CanCast("Flash of Light", true, true, false, false, false))
                        {
                            castSpellOnGrp("Flash of Light", targetPartyID);
                            return;
                        }
                        if (targetHealth <= HolyLightHp && WoW.CanCast("Holy Light", true, true, false, false, false) && WoW.IsInCombat)
                        {
                            castSpellOnGrp("Holy Light", targetPartyID);
                            return;
                        }
                    }
                    if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                    {
                        if (WoW.CanCast("Judgment", true, true, true, false, true))
                        {
                            WoW.CastSpell("Judgment");
                            return;
                        }
                        if (WoW.CanCast("Holy Shock", true, true, true, false, true))
                        {
                            WoW.CastSpell("Holy Shock");
                            return;
                        }
                        if (WoW.CanCast("Crusader Strike", true, true, true, true, true))
                        {
                            WoW.CastSpell("Crusader Strike");
                            return;
                        }
                        if (WoW.CanCast("Consecration", false, true, false, false, false) && WoW.CanCast("Crusader Strike", true, false, true, false, true))
                        {
                            WoW.CastSpell("Consecration");
                            return;
                        }
                    }
                    if (WoW.HasTarget && !WoW.TargetIsEnemy && WoW.IsInCombat)
                    {
                        if (WoW.TargetHealthPercent <= LayonHandsHp && !WoW.TargetHasDebuff("Forbearance") && WoW.CanCast("Lay on Hands", true, true, false, false, false))
                        {
                            WoW.CastSpell("Lay on Hands");
                            return;
                        }
                        if (WoW.TargetHealthPercent <= HolyShockHp && WoW.CanCast("Holy Shock", true, true, false, false, false))
                        {
                            WoW.CastSpell("Holy Shock");
                            return;
                        }
                        if (WoW.TargetHealthPercent <= LightoftheMartyrHp && WoW.HealthPercent >= 85 && WoW.CanCast("Light of the Martyr", true, true, false, false, false))
                        {
                            WoW.CastSpell("Light of the Martyr");
                            return;
                        }
                        if (isTalentBestowFaith && WoW.TargetHealthPercent <= BestowFaithHp && WoW.CanCast("Bestow Faith", true, true, false, false, false))
                        {
                            WoW.CastSpell("Bestow Faith");
                            return;
                        }
                        if (WoW.TargetHealthPercent <= FlashofLightHp && WoW.CanCast("Flash of Light", true, true, false, false, false))
                        {
                            WoW.CastSpell("Flash of Light");
                            return;
                        }
                        if (WoW.TargetHealthPercent <= HolyLightHp && WoW.CanCast("Holy Light", true, true, false, false, false))
                        {
                            WoW.CastSpell("Holy Light");
                            return;
                        }
                    }

                    /*if (tankPartyID != -1 && WoW.IsInCombat && IsInHealRangeParty(tankPartyID) && WoW.CanCast("Holy Light", true, true, false, false, false))
                  {
                      castSpellOnGrp("Holy Light", tankPartyID);
                      return;
                  }*/

                }
                if (combatRoutine.Type == RotationType.SingleTargetCleave) // Do Single Target Stuff here
                {
                    partyTargetSelector();
                    if (tankPartyID != -1)
                    {
                        if (!PartyHasBuff("Beacon of Light", tankPartyID) && IsInHealRangeParty(tankPartyID) && WoW.CanCast("Beacon of Light", true, true, false, false, false))
                        {
                            castSpellOnGrp("Beacon of Light", tankPartyID);
                            return;
                        }
                    }
                    if (targetPartyID != -1)
                    {
                        if (targetHealth <= LayonHandsHp && !PartyHasDeBuff("Forbearance", targetPartyID) && WoW.CanCast("Lay on Hands", true, true, false, false, false))
                        {
                            castSpellOnGrp("Lay on Hands", targetPartyID);
                            return;
                        }
                        if (targetHealth <= BlessingofProtectionHp && !PartyHasDeBuff("Forbearance", targetPartyID) && targetPartyID != tankPartyID && WoW.CanCast("Blessing of Protection", true, true, false, false, false))
                        {
                            castSpellOnGrp("Blessing of Protection", targetPartyID);
                            return;
                        }
                        if (targetHealth <= BlessingofSacrificeHp && !PartyHasDeBuff("Forbearance", targetPartyID) && targetPartyID != 0 && WoW.HealthPercent >= 90 && WoW.CanCast("Blessing of Sacrifice", true, true, false, false, false))
                        {
                            castSpellOnGrp("Blessing of Sacrifice", targetPartyID);
                            return;
                        }
                        /*if (partyMemberReallyLow >= 3 && WoW.CanCast("Tyrs Deliverance", true, true, false, false, false))
                        {
                            WoW.CastSpell("Tyrs Deliverance");
                            return;
                        }*/
                        /*if (partyMemberLow >= 3 && WoW.CanCast("Light of Dawn", true, true, false, false, false))
                        {
                            WoW.CastSpell("Light of Dawn");
                            return;
                        }*/
                        if (targetHealth <= HolyShockHp && WoW.CanCast("Holy Shock", true, true, false, false, false))
                        {
                            castSpellOnGrp("Holy Shock", targetPartyID);
                            return;
                        }
                        if (targetHealth <= LightoftheMartyrHp && WoW.HealthPercent >= 85 && targetPartyID != 0 && WoW.CanCast("Light of the Martyr", true, true, false, false, false))
                        {
                            castSpellOnGrp("Light of the Martyr", targetPartyID);
                            return;
                        }
                        if (isTalentBestowFaith && targetHealth <= BestowFaithHp && WoW.CanCast("Bestow Faith", true, true, false, false, false))
                        {
                            castSpellOnGrp("Bestow Faith", targetPartyID);
                            return;
                        }
                        if (targetHealth <= HolyLightHp && WoW.CanCast("Flash of Light", true, true, false, false, false))
                        {
                            castSpellOnGrp("Flash of Light", targetPartyID);
                            return;
                        }
                    }
                    if (tankPartyID != -1 && WoW.IsInCombat && IsInHealRangeParty(tankPartyID) && WoW.CanCast("Holy Light", true, true, false, false, false))
                    {
                        castSpellOnGrp("Holy Light", tankPartyID);
                        return;
                    }
                }
                if (combatRoutine.Type == RotationType.AOE)
                {
                    if (WoW.HasTarget && WoW.TargetIsEnemy && WoW.IsInCombat)
                    {
                        if (WoW.CanCast("Judgment", true, true, true, false, true))
                        {
                            WoW.CastSpell("Judgment");
                            return;
                        }
                        if (WoW.CanCast("Holy Shock", true, true, true, false, true))
                        {
                            WoW.CastSpell("Holy Shock");
                            return;
                        }
                        if (WoW.CanCast("Crusader Strike", true, true, true, true, true))
                        {
                            WoW.CastSpell("Crusader Strike");
                            return;
                        }
                        if (WoW.CanCast("Consecration", false, true, false, false, false) && WoW.CanCast("Crusader Strike", true, false, true, false, true))
                        {
                            WoW.CastSpell("Consecration");
                            return;
                        }

                    }
                    partyTargetSelector();
                    if (targetPartyID != -1)
                    {
                        /*if (partyMemberLow >= 3 && WoW.CanCast("Light of Dawn", true, true, false, false, false))
                        {
                            WoW.CastSpell("Light of Dawn");
                            return;
                        }*/
                        if (targetHealth <= LightoftheMartyrHp && WoW.HealthPercent >= 85 && targetPartyID != 0 && WoW.CanCast("Light of the Martyr", true, true, false, false, false))
                        {
                            castSpellOnGrp("Light of the Martyr", targetPartyID);
                            return;
                        }
                        if (isTalentBestowFaith && targetPartyID != -1 && targetHealth <= BestowFaithHp && WoW.CanCast("Bestow Faith", true, true, false, false, false))
                        {
                            castSpellOnGrp("Bestow Faith", targetPartyID);
                            return;
                        }
                    }
                }
            }
        }
        private void partyTargetSelector()
        {
            targetPartyID = -1;
            targetHealth = minHealingHP;
            partyMemberLow = 0;
            partyMemberReallyLow = 0;
            var tankHealh = 100;
            tankPartyID = -1;
            var selfHealth = 100;
            if (groupSize == 0)
                groupSize = 1;
            for (int i = 0; i < groupSize; i++)
            {
                if (IsTankParty(i))
                {
                    tankPartyID = i;
                }
                var currentTargetHP = HealthPercentParty(i);
                var isInHealRange = IsInHealRangeParty(i);
                if (currentTargetHP <= 80 && isInHealRange)
                {
                    partyMemberLow++;
                }
                if (currentTargetHP <= 60 && isInHealRange)
                {
                    partyMemberLow++;
                    partyMemberReallyLow++;
                }
                if (i == tankPartyID && isInHealRange)
                {
                    tankHealh = currentTargetHP;
                }
                if (i == 0 && isInHealRange)
                {
                    selfHealth = currentTargetHP;
                }
                if (currentTargetHP <= targetHealth && targetHealth != 0 && isInHealRange && !(PartyHasBuff("Bestow Faith", i) && (currentTargetHP + 10) >= targetHealth))
                {
                    targetHealth = currentTargetHP;
                    targetPartyID = i;
                }
            }
            if (tankHealh <= minTankPriorityHP)
            {
                targetHealth = tankHealh;
                targetPartyID = tankPartyID;
            }
            if (selfHealth <= minSelfPriorityHP)
            {
                targetHealth = selfHealth;
                targetPartyID = 0;
            }
        }
        private void raidTargetSelector()
        {
            targetPartyID = -1;
            targetHealth = minHealingHP;
            partyMemberLow = 0;
            partyMemberReallyLow = 0;
            var tankHealh = 100;
            tankPartyID = -1;
            var tankHealh2 = 100;
            tankPartyID2 = -1;
            for (int i = 1; i <= groupSize; i++)
            {
                if (IsTankParty(i))
                {
                    if (tankPartyID == -1)
                        tankPartyID = i;
                    else
                        tankPartyID2 = i;
                }
                var currentTargetHP = HealthPercentParty(i);
                var isInHealRange = IsInHealRangeParty(i);
                if (currentTargetHP <= 80 && isInHealRange)
                {
                    partyMemberLow++;
                }
                if (currentTargetHP <= 60 && isInHealRange)
                {
                    partyMemberLow++;
                    partyMemberReallyLow++;
                }
                if (i == tankPartyID && isInHealRange)
                {
                    tankHealh = currentTargetHP;
                }
                if (i == tankPartyID2 && isInHealRange)
                {
                    tankHealh2 = currentTargetHP;
                }
                if (currentTargetHP <= targetHealth && targetHealth != 0 && isInHealRange)
                {
                    targetHealth = currentTargetHP;
                    targetPartyID = i;
                }
            }
            if (tankHealh <= minTankPriorityHP)
            {
                targetHealth = tankHealh;
                targetPartyID = tankPartyID;
            }
            if (tankHealh2 <= minTankPriorityHP)
            {
                targetHealth = tankHealh2;
                targetPartyID = tankPartyID2;
            }
        }


        private void TargetOnRaid(int id)
        {
            Log.Write("target raid member" + id + " modulo: " + id % 8 + "div: " + (id - 1) / 8);
            switch (id % 8)
            {
                case 0:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 5:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 6:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 7:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
            }
            WoW.CastSpell("target" + ((id - 1) / 8));
            switch (id % 8)
            {
                case 0:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 5:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 6:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 7:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
            }
        }
        private void castSpellOnGrp(string spell, int id)
        {
            Log.Write("target raid member" + id);
            switch (id)
            {
                case 0:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 1:
                    WoW.KeyDown(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    WoW.KeyDown(WoW.Keys.LMenu);
                    break;
            }
            WoW.CastSpell(spell);
            switch (id)
            {
                case 0:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 1:
                    WoW.KeyUp(WoW.Keys.ShiftKey);
                    break;
                case 2:
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
                case 3:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    break;
                case 4:
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    WoW.KeyUp(WoW.Keys.LMenu);
                    break;
            }
        }

        private static int HealthPercentParty(int partyID)
        {
            if (partyID == 0)
                return WoW.HealthPercent;
            var c = WoW.GetBlockColor(partyID + 1, 21);
            try
            {
                if (c.R == 0)
                    return 100;
                int health = Convert.ToInt32((double)c.R * 100 / 255);
                return health;
            }
            catch (Exception ex)
            {
                Log.Write($"[Health] Red = {c.R}");
                Log.Write(ex.Message, Color.Red);
                return 100;
            }
        }
        private int GroupSize()
        {
            var c = WoW.GetBlockColor(1, 21);
            try
            {
                if (c.R == 0)
                    return 0;
                isInRaid = c.G != 255;
                int size = Convert.ToInt32((double)c.R * 100 / 255);
                if (size == 100)
                    return 0;
                return size;
            }
            catch (Exception ex)
            {
                Log.Write($"[Health] Red = {c.R}");
                Log.Write(ex.Message, Color.Red);
                return 100;
            }
        }
        private bool isInJudgmentRange()
        {
            var c = WoW.GetBlockColor(1, 21);
            try
            {
                return c.B == 0;
            }
            catch (Exception ex)
            {
                Log.Write($"[Health] Red = {c.B}");
                Log.Write(ex.Message, Color.Red);
                return false;
            }
        }
        private static bool IsInHealRangeParty(int partyID)
        {
            if (partyID == 0)
                return true;
            var c = WoW.GetBlockColor(partyID + 1, 21);
            try
            {
                return c.G != 255;
            }
            catch (Exception ex)
            {
                Log.Write($"[Health] Green = {c.G}");
                Log.Write(ex.Message, Color.Red);
                return false;
            }
        }
        private bool IsTankParty(int partyID)
        {
            if (WoW.HealthPercent != 0)
            {
                if (partyID == 0)
                    return false;
                var c = WoW.GetBlockColor(partyID + 1, 21);
                if (c.B <= 80 && c.B !=0)
                { 
                    if(tankTankingID == -1)
                    tankTankingID = partyID;
                }

                try
                {
                    return c.B <= 80;
                }
                catch (Exception ex)
                {
                    Log.Write($"[Health] Blue = {c.B}");
                    Log.Write(ex.Message, Color.Red);
                    return false;
                }
            }
            return false;
        }

        private static bool PartyHasBuff(string buffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerHasBuff(buffName);
            Aura aura = null;
            for (var i = 0; i < SpellBook.Auras.Count; i++)
            {
                if (SpellBook.Auras[i].AuraName == buffName)
                    aura = SpellBook.Auras[i];
            }
            if (aura == null)
            {
                Log.Write($"[Hasbuff] Fant ikke debuff '{buffName}' in Spell Book");
                return false;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 12 + partyID);
            return (c.R != 255) && (c.G != 255) && (c.B != 255);
        }

        private static bool PartyHasDeBuff(string debuffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerHasDebuff(debuffName);
            Aura aura = null;
            for (var i = 0; i < SpellBook.Auras.Count; i++)
            {
                if (SpellBook.Auras[i].AuraName == debuffName)
                    aura = SpellBook.Auras[i];
            }
            if (aura == null)
            {
                Log.Write($"[HasDebuff] Fant ikke debuff '{debuffName}' in Spell Book");
                return false;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 16 + partyID);
            return (c.R != 255) && (c.G != 255) && (c.B != 255);
        }

        private static int GetPartyBuffTimeRemaining(string buffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerBuffTimeRemaining(buffName);
            Aura aura = null;
            for (var i = 0; i < SpellBook.Auras.Count; i++)
            {
                if (SpellBook.Auras[i].AuraName == buffName)
                    aura = SpellBook.Auras[i];
            }
            if (aura == null)
            {
                Log.Write($"[HasDebuff] Fant ikke debuff '{buffName}' in Spell Book");
                return 0;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 12 + partyID);

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                if (c.R == 0)
                    return 0;
                return Convert.ToInt32((double)c.R * 100 / 255);
            }
            catch (Exception ex)
            {
                Log.Write("Failed to find debuff stacks for color G = " + c.B, Color.Red);
                Log.Write("Error: " + ex.Message, Color.Red);
            }

            return 0;
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Tyalieva
AddonName=tyahelper
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,19750,Flash of Light,D1
Spell,82326,Holy Light,D2
Spell,183998,Light of the Martyr,D4
Spell,20473,Holy Shock,D3
Spell,223306,Bestow Faith,D5
Spell,200025,Beacon of Virtue,F10
Spell,53563,Beacon of Light,F10
Spell,633,Lay on Hands,D9
Spell,20271,Judgment,D0
Spell,35395,Crusader Strike,F8
Spell,26573,Consecration,F12
Spell,85222,Light of Dawn,A
Spell,1022,Blessing of Protection,F3
Spell,6940,Blessing of Sacrifice,F7
Spell,200652,Tyrs Deliverance,F9 
Spell,0,target0,T
Spell,1,target1,U
Spell,2,target2,I
Spell,3,stopcasting,NumPad5
Aura,642,Divine Shield
Aura,200025,Beacon of Virtue
Aura,214222,Judgment
Aura,223306,Bestow Faith
Aura,25771,Forbearance
Aura,53563,Beacon of Light
*/
