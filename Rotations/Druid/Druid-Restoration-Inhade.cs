// winifix@gmail.com
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertPropertyToExpressionBody


/*

Macros:
Rotation will create them automatically, however if it fails, copy paste in WoW.

"Cenarion Ward"
#showtooltip Cenarion Ward
/cast [nomod,@player] Cenarion Ward
/cast [mod:ctrl,mod:alt,@party4] Cenarion Ward
/cast [mod:shift,@party1] Cenarion Ward
/cast [mod:alt,@party2] Cenarion Ward
/cast [mod:ctrl,@party3] Cenarion Ward

"Efflorescence"
#showtooltip Efflorescence
/cast [@cursor] Efflorescence

"Healing Touch"
#showtooltip Healing Touch
/cast [nomod,@player] Healing Touch
/cast [mod:ctrl,mod:alt,@party4] Healing Touch
/cast [mod:shift,@party1] Healing Touch
/cast [mod:alt,@party2] Healing Touch
/cast [mod:ctrl,@party3] Healing Touch

"InnervateSelf"
#showtooltip Innervate
/cast [@player] Innervate

"Inspect"
/script InspectUnit("party1"); InspectUnit("party2"); InspectUnit("party3"); InspectUnit("party4");

"Ironbark"
#showtooltip Ironbark
/cast [nomod,@player] Ironbark
/cast [mod:ctrl,mod:alt,@party4] Ironbark
/cast [mod:shift,@party1] Ironbark
/cast [mod:alt,@party2] Ironbark
/cast [mod:ctrl,@party3] Ironbark

"Lifebloom"
#showtooltip Lifebloom
/cast [nomod,@player] Lifebloom
/cast [mod:ctrl,mod:alt,@party4] Lifebloom
/cast [mod:shift,@party1] Lifebloom
/cast [mod:alt,@party2] Lifebloom
/cast [mod:ctrl,@party3] Lifebloom

"Rebirth"
#showtooltip Rebirth
/cast [nomod,@player] Rebirth
/cast [mod:ctrl,mod:alt,@party4] Rebirth
/cast [mod:shift,@party1] Rebirth
/cast [mod:alt,@party2] Rebirth
/cast [mod:ctrl,@party3] Rebirth

"Regrowth"
#showtooltip Regrowth
/cast [nomod,@player] Regrowth
/cast [mod:ctrl,mod:alt,@party4] Regrowth
/cast [mod:shift,@party1] Regrowth
/cast [mod:alt,@party2] Regrowth
/cast [mod:ctrl,@party3] Regrowth

"Rejuvenation"
#showtooltip Rejuvenation
/cast [nomod,@player] Rejuvenation
/cast [mod:ctrl,mod:alt,@party4] Rejuvenation
/cast [mod:shift,@party1] Rejuvenation
/cast [mod:alt,@party2] Rejuvenation
/cast [mod:ctrl,@party3] Rejuvenation

"Swiftmend"
#showtooltip Swiftmend
/cast [nomod,@player] Swiftmend
/cast [mod:ctrl,mod:alt,@party4] Swiftmend
/cast [mod:shift,@party1] Swiftmend
/cast [mod:alt,@party2] Swiftmend
/cast [mod:ctrl,@party3] Swiftmend

KNOWN BUGS: Keybinds box does not refresh after restart, however your keybinds are there.

0.1
Initial internal Release

0.1.1
Automated addon change for TargetDebuff to show on Friendly

0.2
Switched to FMFlex Heal Lua - no more target switching!

0.3
Added high and low healing intensity rotations
Added HPROC calculations

0.4
Added offspec rotations
Added overlay and health calculators from guardian

0.5
New GUI
Improved tank detection
Included lua in rotation

0.6 public beta
Added support for all talents
Added customizable mana/heal ratio
Added auto macro creation
Added tank healing and high healing rotation

Outstanding:
Make a help file

Please report all issues to #druid tagging me!
Will be buggy, lots of experimantal stuff.


*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using CloudMagic.Helpers;
using Timer = System.Timers.Timer;
#pragma warning disable 414

namespace CloudMagic.Rotation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Restoration : CombatRoutine
    {
        // Variables

        // Overlay text
        public static string DisplayText;
        private static readonly string AddonName = ConfigFile.ReadValue("CloudMagic", "AddonName");
        private static bool AddonEdited;
        private static bool Initialized;
        private static int Self = 0;
        private static int Tank = -1;
        private static int Dps1 = -1;
        private static int Dps2 = -1;
        private static int Dps3 = -1;
        private static int InitializeStep;
        private static bool IsFirstRun = true;

        // Sleep variables
        private static int PulseSleep;

        // HP rate of change for party members
        private static float SelfHPRateOfChange;
        private static float TankHPRateOfChange;
        private static float Dps1HPRateOfChange;
        private static float Dps2HPRateOfChange;
        private static float Dps3HPRateOfChange;

        // Variables for HP timers
        private static int PreviousSelfHP;
        private static int CurrentSelfHP;
        private static int PreviousTankHP;
        private static int CurrentTankHP;
        private static int PreviousDps1HP;
        private static int CurrentDps1HP;
        private static int PreviousDps2HP;
        private static int CurrentDps2HP;
        private static int PreviousDps3HP;
        private static int CurrentDps3HP;

        // HP Rate Of Change booleans
        private static bool SelfHPROC1sAny;
        private static bool SelfHPROC5sAny;
        private static bool SelfHPROC10sAny;
        private static bool TankHPROC1sAny;
        private static bool TankHPROC5sAny;
        private static bool TankHPROC10sAny;
        private static bool Dps1HPROC1sAny;
        private static bool Dps1HPROC5sAny;
        private static bool Dps1HPROC10sAny;
        private static bool Dps2HPROC1sAny;
        private static bool Dps2HPROC5sAny;
        private static bool Dps2HPROC10sAny;
        private static bool Dps3HPROC1sAny;
        private static bool Dps3HPROC5sAny;
        private static bool Dps3HPROC10sAny;
        
        // Healing efficiency variables
        private static string HealingIntensity = "Normal";
        private static readonly int HealingTouchSleep = Convert.ToInt32(2.5f/(1 + WoW.HastePercent/100f) > 1)*1000;

        // Party variables
        private static int PartyRejuv;
        private static int PartyGermination;
        private static int PartyTotalHOTs;
        private static int PartyHealth;
        private static int PartyLowestHealth;
        private static readonly List<int> PartyHealthList = new List<int>();
        private static int PartyLowestHealthMember;
        private static bool PartyLowestHealthMemberInRange;
        private static bool IsDeadParty;

        // User selection booleans
        private static bool DPSMode;
        private static bool CoolDowns;
        private static bool CoolDownsTriggeredByInnervate = true;
        private static bool TankHealing;

        // Selections stopwatch
        private readonly Stopwatch CoolDownTimer = new Stopwatch();
        private readonly Queue<float> Dps1Queue10s = new Queue<float>();
        private readonly Queue<float> Dps1Queue1s = new Queue<float>();
        private readonly Queue<float> Dps1Queue5s = new Queue<float>();
        private readonly Queue<float> Dps2Queue10s = new Queue<float>();
        private readonly Queue<float> Dps2Queue1s = new Queue<float>();
        private readonly Queue<float> Dps2Queue5s = new Queue<float>();
        private readonly Queue<float> Dps3Queue10s = new Queue<float>();
        private readonly Queue<float> Dps3Queue1s = new Queue<float>();
        private readonly Queue<float> Dps3Queue5s = new Queue<float>();
        private readonly Stopwatch DPSTimer = new Stopwatch();

        // Efflorescence stopwatch
        private readonly Stopwatch EfflorescenceTimer = new Stopwatch();

        // Frenzied Regen stopwatch
        private readonly Stopwatch FrenziedTimer = new Stopwatch();

        // Healing Intensity stopwatch
        private readonly Stopwatch HealingIntensityTimer = new Stopwatch();

        // Overlay display stopwatch
        private readonly Stopwatch OverlayTimer = new Stopwatch();
        private readonly Queue<float> SelfQueue10s = new Queue<float>();

        // HP Rate Of Change Queues 
        private readonly Queue<float> SelfQueue1s = new Queue<float>();
        private readonly Queue<float> SelfQueue5s = new Queue<float>();
        private readonly Stopwatch TankHealingTimer = new Stopwatch();
        private readonly Queue<float> TankQueue10s = new Queue<float>();
        private readonly Queue<float> TankQueue1s = new Queue<float>();
        private readonly Queue<float> TankQueue5s = new Queue<float>();

        // Tier 4 stopwatch
        private readonly Stopwatch Tier4Timer = new Stopwatch();
        private Label AccountLabel;
        private TextBox AccountNameTextBox;
        private CheckBox AreKeybindsEnabledBox;
        private CheckBox CenarionWardBox;
        private Label CharacterLabel;
        private TextBox CharacterNameTextBox;
        private CheckBox CheckIfAddonEditedBox;
        private CheckBox CoolDownBox;
        private ComboBox CoolDownKeyCombo;
        private ComboBox CoolDownModCombo;
        private Button CreateMacrosButton;
        private Button DefaultsButton;
        private CheckBox DisplayCDBox;
        private CheckBox DisplayOffSpecBox;
        private Button DisplayOverlayButton;
        private CheckBox DisplayTankHealingBox;
        private Timer Dps1Timer;
        private Timer Dps2Timer;
        private Timer Dps3Timer;
        private ComboBox DPSKeyCombo;
        private ComboBox DPSModCombo;
        private CheckBox DPSModeBox;
        private CheckBox EfflorescenceBox;
        private CheckBox EssenceBox;
        private NumericUpDown EssenceNum;
        private CheckBox FlourishBox;
        private NumericUpDown FlourishNum;
        private CheckBox GerminationBox;
        private NumericUpDown GerminationNum;
        private Button HighHealButton;
        private Label HP1Label;
        private Label HP2Label;
        private Label HP3Label;
        private Label HP4Label;
        private Label HP5Label;
        private Label HP6Label;
        private Label HP7Label;
        private Label HP8Label;
        private Label HPROCLabel;
        private Label HPROCTextLabel;
        private TrackBar HPROCTrackBar;
        private CheckBox IncarnateBox;
        private NumericUpDown IncarnateNum;
        private CheckBox InitializeWithSendsBox;
        private CheckBox InnervateBox;
        private NumericUpDown InnervateNum;
        private CheckBox LoadExternalAddonBox;
        private Label ManaModifierLabel;
        private TrackBar ManaModifierTrackBar;
        private GroupBox MiscGroup;
        private Label NameOfMountLabel;
        private TextBox NameOfMountTextBox;
        private CheckedListBox OffSpecListBox;
        private Label OverlayLabel;
        private CheckBox ProwlOOCBox;
        private Label RealmLabel;
        private TextBox RealmNameTextBox;
        private Button RegisterMountButton;
        private Label RejuvenationLabel;
        private NumericUpDown RejuvenationNum;
        private CheckBox RenewalBox;
        private NumericUpDown RenewalNum;
        private Button SaveButton;
        private CheckBox SaveEssenceBox;
        private CheckBox SaveFlourishBox;
        private Label SaveForCDsLabel;
        private CheckBox SaveIncarnateBox;
        private CheckBox SaveInnervateBox;
        private CheckBox SaveRenewalBox;
        private CheckBox SaveTranquilityBox;
        private Label SelectAffinityLabel;

        // HP timer
        private Timer SelfTimer;
        private GroupBox SettingsGroup;
        private CheckBox SoulOfTheForestBox;
        private CheckBox SpringBlossomsBox;

        // SettingsForm variables
        private GroupBox TalentsGroup;
        private CheckBox TankHealingBox;
        private ComboBox TankHealingKeyCombo;
        private ComboBox TankHealingModCombo;
        private Timer TankTimer;
        private CheckBox Tier4Box;
        private CheckBox TranquilityBox;
        private NumericUpDown TranquilityNum;

        // Initialization variables		
        private static string CustomLua
        {
            get
            {
                if (LoadExternalAddon)
                {
                    var customLua = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "CustomHeal.lua");
                    return customLua;
                }
                else
                {
                    var customLua = AddonEmbedded;
                    return customLua;
                }
            }
        }

        // Get spell timers
        private static float GCD
        {
            get
            {
                return Convert.ToSingle(1.5f/(1 + WoW.HastePercent/100f)) > 1 ? Convert.ToSingle(1.5f/(1 + WoW.HastePercent/100f)) : 1;
            }
        }

        // Savable variables
        private static bool Germination
        {
            get
            {
                var germination = ConfigFile.ReadValue("Restoration", "Germination").Trim();
                return germination != "" && Convert.ToBoolean(germination);
            }
            set { ConfigFile.WriteValue("Restoration", "Germination", value.ToString()); }
        }

        private static bool SoulOfTheForest
        {
            get
            {
                var soulOfTheForest = ConfigFile.ReadValue("Restoration", "SoulOfTheForest").Trim();
                return soulOfTheForest != "" && Convert.ToBoolean(soulOfTheForest);
            }
            set { ConfigFile.WriteValue("Restoration", "SoulOfTheForest", value.ToString()); }
        }

        private static bool SpringBlossoms
        {
            get
            {
                var springBlossoms = ConfigFile.ReadValue("Restoration", "SpringBlossoms").Trim();
                return springBlossoms != "" && Convert.ToBoolean(springBlossoms);
            }
            set { ConfigFile.WriteValue("Restoration", "SpringBlossoms", value.ToString()); }
        }

        private static bool CenarionWard
        {
            get
            {
                var cenarionWard = ConfigFile.ReadValue("Restoration", "CenarionWard").Trim();
                return cenarionWard != "" && Convert.ToBoolean(cenarionWard);
            }
            set { ConfigFile.WriteValue("Restoration", "CenarionWard", value.ToString()); }
        }

        private static bool UseTranquility
        {
            get
            {
                var useTranquility = ConfigFile.ReadValue("Restoration", "UseTranquility").Trim();
                return useTranquility != "" && Convert.ToBoolean(useTranquility);
            }
            set { ConfigFile.WriteValue("Restoration", "UseTranquility", value.ToString()); }
        }

        private static bool SaveTranquility
        {
            get
            {
                var saveTranquility = ConfigFile.ReadValue("Restoration", "SaveTranquility").Trim();
                return saveTranquility != "" && Convert.ToBoolean(saveTranquility);
            }
            set { ConfigFile.WriteValue("Restoration", "SaveTranquility", value.ToString()); }
        }

        private static bool UseEssence
        {
            get
            {
                var useTranquility = ConfigFile.ReadValue("Restoration", "UseTranquility").Trim();
                return useTranquility != "" && Convert.ToBoolean(useTranquility);
            }
            set { ConfigFile.WriteValue("Restoration", "UseTranquility", value.ToString()); }
        }

        private static bool SaveEssence
        {
            get
            {
                var saveEssence = ConfigFile.ReadValue("Restoration", "SaveEssence").Trim();
                return saveEssence != "" && Convert.ToBoolean(saveEssence);
            }
            set { ConfigFile.WriteValue("Restoration", "SaveEssence", value.ToString()); }
        }

        private static bool UseFlourish
        {
            get
            {
                var useFlourish = ConfigFile.ReadValue("Restoration", "UseFlourish").Trim();
                return useFlourish != "" && Convert.ToBoolean(useFlourish);
            }
            set { ConfigFile.WriteValue("Restoration", "UseFlourish", value.ToString()); }
        }

        private static bool SaveFlourish
        {
            get
            {
                var saveFlourish = ConfigFile.ReadValue("Restoration", "SaveFlourish").Trim();
                return saveFlourish != "" && Convert.ToBoolean(saveFlourish);
            }
            set { ConfigFile.WriteValue("Restoration", "SaveFlourish", value.ToString()); }
        }

        private static bool UseInnervate
        {
            get
            {
                var useInnervate = ConfigFile.ReadValue("Restoration", "UseInnervate").Trim();
                return useInnervate != "" && Convert.ToBoolean(useInnervate);
            }
            set { ConfigFile.WriteValue("Restoration", "UseInnervate", value.ToString()); }
        }

        private static bool SaveInnervate
        {
            get
            {
                var saveInnervate = ConfigFile.ReadValue("Restoration", "SaveInnervate").Trim();
                return saveInnervate != "" && Convert.ToBoolean(saveInnervate);
            }
            set { ConfigFile.WriteValue("Restoration", "SaveInnervate", value.ToString()); }
        }

        private static bool UseEfflorescence
        {
            get
            {
                var useEfflorescence = ConfigFile.ReadValue("Restoration", "UseEfflorescence").Trim();
                return useEfflorescence != "" && Convert.ToBoolean(useEfflorescence);
            }
            set { ConfigFile.WriteValue("Restoration", "UseEfflorescence", value.ToString()); }
        }

        private static bool UseRenewal
        {
            get
            {
                var useRenewal = ConfigFile.ReadValue("Restoration", "UseRenewal").Trim();
                return useRenewal != "" && Convert.ToBoolean(useRenewal);
            }
            set { ConfigFile.WriteValue("Restoration", "UseRenewal", value.ToString()); }
        }

        private static bool SaveRenewal
        {
            get
            {
                var saveRenewal = ConfigFile.ReadValue("Restoration", "SaveRenewal").Trim();
                return saveRenewal != "" && Convert.ToBoolean(saveRenewal);
            }
            set { ConfigFile.WriteValue("Restoration", "SaveRenewal", value.ToString()); }
        }

        private static bool UseIncarnate
        {
            get
            {
                var useIncarnate = ConfigFile.ReadValue("Restoration", "UseIncarnate").Trim();
                return useIncarnate != "" && Convert.ToBoolean(useIncarnate);
            }
            set { ConfigFile.WriteValue("Restoration", "UseIncarnate", value.ToString()); }
        }

        private static bool SaveIncarnate
        {
            get
            {
                var saveIncarnate = ConfigFile.ReadValue("Restoration", "SaveIncarnate").Trim();
                return saveIncarnate != "" && Convert.ToBoolean(saveIncarnate);
            }
            set { ConfigFile.WriteValue("Restoration", "SaveIncarnate", value.ToString()); }
        }

        private static bool UseTier4
        {
            get
            {
                var useTier4 = ConfigFile.ReadValue("Restoration", "UseTier4").Trim();
                return useTier4 != "" && Convert.ToBoolean(useTier4);
            }
            set { ConfigFile.WriteValue("Restoration", "UseTier4", value.ToString()); }
        }

        private static bool InitializeWithSends
        {
            get
            {
                var initializeWithSends = ConfigFile.ReadValue("Restoration", "InitializeWithSends").Trim();
                return initializeWithSends != "" && Convert.ToBoolean(initializeWithSends);
            }
            set { ConfigFile.WriteValue("Restoration", "InitializeWithSends", value.ToString()); }
        }

        private static bool ForceAddonReload
        {
            get
            {
                var forceAddonReload = ConfigFile.ReadValue("Restoration", "ForceAddonReload").Trim();
                return forceAddonReload != "" && Convert.ToBoolean(forceAddonReload);
            }
            set { ConfigFile.WriteValue("Restoration", "ForceAddonReload", value.ToString()); }
        }

        private static bool LoadExternalAddon
        {
            get
            {
                var loadExternalAddon = ConfigFile.ReadValue("Restoration", "LoadExternalAddon").Trim();
                return loadExternalAddon != "" && Convert.ToBoolean(loadExternalAddon);
            }
            set { ConfigFile.WriteValue("Restoration", "LoadExternalAddon", value.ToString()); }
        }

        private static bool UseDPSMode
        {
            get
            {
                var useDPSMode = ConfigFile.ReadValue("Restoration", "UseDPSMode").Trim();
                return useDPSMode != "" && Convert.ToBoolean(useDPSMode);
            }
            set { ConfigFile.WriteValue("Restoration", "UseDPSMode", value.ToString()); }
        }

        private static bool UseCoolDowns
        {
            get
            {
                var useCoolDowns = ConfigFile.ReadValue("Restoration", "UseCoolDowns").Trim();
                return useCoolDowns != "" && Convert.ToBoolean(useCoolDowns);
            }
            set { ConfigFile.WriteValue("Restoration", "UseCoolDowns", value.ToString()); }
        }

        private static bool UseTankHealing
        {
            get
            {
                var useTankHealing = ConfigFile.ReadValue("Restoration", "UseTankHealing").Trim();
                return useTankHealing != "" && Convert.ToBoolean(useTankHealing);
            }
            set { ConfigFile.WriteValue("Restoration", "UseTankHealing", value.ToString()); }
        }

        private static bool AreKeybindsEnabled
        {
            get
            {
                var areKeybindsEnabled = ConfigFile.ReadValue("Restoration", "AreKeybindsEnabled").Trim();
                return areKeybindsEnabled != "" && Convert.ToBoolean(areKeybindsEnabled);
            }
            set { ConfigFile.WriteValue("Restoration", "AreKeybindsEnabled", value.ToString()); }
        }

        private static bool DisplayDPS
        {
            get
            {
                var displayDPS = ConfigFile.ReadValue("Restoration", "DisplayDPS").Trim();
                return displayDPS != "" && Convert.ToBoolean(displayDPS);
            }
            set { ConfigFile.WriteValue("Restoration", "DisplayDPS", value.ToString()); }
        }

        private static bool DisplayCD
        {
            get
            {
                var displayCD = ConfigFile.ReadValue("Restoration", "DisplayCD").Trim();
                return displayCD != "" && Convert.ToBoolean(displayCD);
            }
            set { ConfigFile.WriteValue("Restoration", "DisplayCD", value.ToString()); }
        }

        private static bool DisplayTank
        {
            get
            {
                var displayTank = ConfigFile.ReadValue("Restoration", "DisplayTank").Trim();
                return displayTank != "" && Convert.ToBoolean(displayTank);
            }
            set { ConfigFile.WriteValue("Restoration", "DisplayTank", value.ToString()); }
        }

        private static bool ProwlOOC
        {
            get
            {
                var prowlOOC = ConfigFile.ReadValue("Restoration", "ProwlOOC").Trim();
                return prowlOOC != "" && Convert.ToBoolean(prowlOOC);
            }
            set { ConfigFile.WriteValue("Restoration", "ProwlOOC", value.ToString()); }
        }

        private static int HPROC
        {
            get
            {
                var readValue = ConfigFile.ReadValue("Restoration", "HPROC");
                try
                {
                    return Convert.ToInt32(readValue);
                }
                catch (FormatException)
                {
                    return 10;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "HPROC", value.ToString()); }
        }

        private static string MountName
        {
            get
            {
                var mountName = ConfigFile.ReadValue("Restoration", "MountName");
                try
                {
                    return mountName;
                }
                catch (FormatException)
                {
                    return "Corrupted Fire Hawk";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "MountName", value); }
        }

        private static string CoolDownKey
        {
            get
            {
                var coolDownKey = ConfigFile.ReadValue("Restoration", "CoolDownKey");
                try
                {
                    return coolDownKey;
                }
                catch (FormatException)
                {
                    return "None";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "CoolDownKey", value); }
        }

        private static string CoolDownMod
        {
            get
            {
                var coolDownMod = ConfigFile.ReadValue("Restoration", "CoolDownMod");
                try
                {
                    return coolDownMod;
                }
                catch (FormatException)
                {
                    return "None";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "CoolDownMod", value); }
        }

        private static string DPSModeKey
        {
            get
            {
                var dPSModeKey = ConfigFile.ReadValue("Restoration", "DPSModeKey");
                try
                {
                    return dPSModeKey;
                }
                catch (FormatException)
                {
                    return "None";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "DPSModeKey", value); }
        }

        private static string TankHealingMod
        {
            get
            {
                var tankHealingMod = ConfigFile.ReadValue("Restoration", "TankHealingMod");
                try
                {
                    return tankHealingMod;
                }
                catch (FormatException)
                {
                    return "None";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "TankHealingMod", value); }
        }

        private static string TankHealingKey
        {
            get
            {
                var tankHealingKey = ConfigFile.ReadValue("Restoration", "TankHealingKey");
                try
                {
                    return tankHealingKey;
                }
                catch (FormatException)
                {
                    return "None";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "TankHealingKey", value); }
        }

        private static string DPSModeMod
        {
            get
            {
                var dPSModeMod = ConfigFile.ReadValue("Restoration", "DPSModeMod");
                try
                {
                    return dPSModeMod;
                }
                catch (FormatException)
                {
                    return "None";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "DPSModeMod", value); }
        }

        private static int CoolKey
        {
            get
            {
                var coolKey = ConfigFile.ReadValue("Restoration", "CoolKey");
                try
                {
                    return Convert.ToInt32(coolKey);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "CoolKey", value.ToString()); }
        }

        private static int CoolMod
        {
            get
            {
                var coolMod = ConfigFile.ReadValue("Restoration", "CoolMod");
                try
                {
                    return Convert.ToInt32(coolMod);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "CoolMod", value.ToString()); }
        }

        private static int DPSKey
        {
            get
            {
                var dPSKey = ConfigFile.ReadValue("Restoration", "DPSKey");
                try
                {
                    return Convert.ToInt32(dPSKey);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "DPSKey", value.ToString()); }
        }

        private static int DPSMod
        {
            get
            {
                var dPSMod = ConfigFile.ReadValue("Restoration", "DPSMod");
                try
                {
                    return Convert.ToInt32(dPSMod);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "DPSMod", value.ToString()); }
        }

        private static int TankKey
        {
            get
            {
                var tankKey = ConfigFile.ReadValue("Restoration", "TankKey");
                try
                {
                    return Convert.ToInt32(tankKey);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "TankKey", value.ToString()); }
        }

        private static int TankMod
        {
            get
            {
                var tankMod = ConfigFile.ReadValue("Restoration", "TankMod");
                try
                {
                    return Convert.ToInt32(tankMod);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "TankMod", value.ToString()); }
        }

        private static int EfflorescenceHP
        {
            get
            {
                var efflorescenceHP = ConfigFile.ReadValue("Restoration", "EfflorescenceHP");
                try
                {
                    return Convert.ToInt32(efflorescenceHP);
                }
                catch (FormatException)
                {
                    return 80;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "EfflorescenceHP", value.ToString()); }
        }

        private static int InnervateHP
        {
            get
            {
                var innervateHP = ConfigFile.ReadValue("Restoration", "InnervateHP");
                try
                {
                    return Convert.ToInt32(innervateHP);
                }
                catch (FormatException)
                {
                    return 65;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "InnervateHP", value.ToString()); }
        }

        private static int FlourishHP
        {
            get
            {
                var flourishHP = ConfigFile.ReadValue("Restoration", "FlourishHP");
                try
                {
                    return Convert.ToInt32(flourishHP);
                }
                catch (FormatException)
                {
                    return 70;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "FlourishHP", value.ToString()); }
        }

        private static int WildGrowthHP
        {
            get
            {
                var wildGrowthHP = ConfigFile.ReadValue("Restoration", "WildGrowthHP");
                try
                {
                    return Convert.ToInt32(wildGrowthHP);
                }
                catch (FormatException)
                {
                    return 80;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "WildGrowthHP", value.ToString()); }
        }

        private static int EssenceHP
        {
            get
            {
                var essenceHP = ConfigFile.ReadValue("Restoration", "EssenceHP");
                try
                {
                    return Convert.ToInt32(essenceHP);
                }
                catch (FormatException)
                {
                    return 80;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "EssenceHP", value.ToString()); }
        }

        private static int TranquilityHP
        {
            get
            {
                var essenceHP = ConfigFile.ReadValue("Restoration", "EssenceHP");
                try
                {
                    return Convert.ToInt32(essenceHP);
                }
                catch (FormatException)
                {
                    return 65;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "EssenceHP", value.ToString()); }
        }

        private static int RegrowthHP
        {
            get
            {
                var regrowthHP = ConfigFile.ReadValue("Restoration", "RegrowthHP");
                try
                {
                    return Convert.ToInt32(regrowthHP);
                }
                catch (FormatException)
                {
                    return 50;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "RegrowthHP", value.ToString()); }
        }

        private static int HealingTouchHP
        {
            get
            {
                var healingTouchHP = ConfigFile.ReadValue("Restoration", "HealingTouchHP");
                try
                {
                    return Convert.ToInt32(healingTouchHP);
                }
                catch (FormatException)
                {
                    return 40;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "HealingTouchHP", value.ToString()); }
        }

        private static int RejuvenationHP
        {
            get
            {
                var rejuvenationHP = ConfigFile.ReadValue("Restoration", "RejuvenationHP");
                try
                {
                    return Convert.ToInt32(rejuvenationHP);
                }
                catch (FormatException)
                {
                    return 80;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "RejuvenationHP", value.ToString()); }
        }

        private static int GerminationHP
        {
            get
            {
                var germinationHP = ConfigFile.ReadValue("Restoration", "GerminationHP");
                try
                {
                    return Convert.ToInt32(germinationHP);
                }
                catch (FormatException)
                {
                    return 60;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "GerminationHP", value.ToString()); }
        }

        private static int SwiftmendHP
        {
            get
            {
                var swiftmendHP = ConfigFile.ReadValue("Restoration", "SwiftmendHP");
                try
                {
                    return Convert.ToInt32(swiftmendHP);
                }
                catch (FormatException)
                {
                    return 40;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "SwiftmendHP", value.ToString()); }
        }

        private static int IronbarkHP
        {
            get
            {
                var ironbarkHP = ConfigFile.ReadValue("Restoration", "IronbarkHP");
                try
                {
                    return Convert.ToInt32(ironbarkHP);
                }
                catch (FormatException)
                {
                    return 40;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "IronbarkHP", value.ToString()); }
        }

        private static int RenewalHP
        {
            get
            {
                var renewalHP = ConfigFile.ReadValue("Restoration", "RenewalHP");
                try
                {
                    return Convert.ToInt32(renewalHP);
                }
                catch (FormatException)
                {
                    return 40;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "RenewalHP", value.ToString()); }
        }

        private static int IncarnateHP
        {
            get
            {
                var incarnateHP = ConfigFile.ReadValue("Restoration", "IncarnateHP");
                try
                {
                    return Convert.ToInt32(incarnateHP);
                }
                catch (FormatException)
                {
                    return 50;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "IncarnateHP", value.ToString()); }
        }

        private static int ManaModifier
        {
            get
            {
                var manaModifier = ConfigFile.ReadValue("Restoration", "ManaModifier");
                try
                {
                    return Convert.ToInt32(manaModifier);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
            set { ConfigFile.WriteValue("Restoration", "ManaModifier", value.ToString()); }
        }

        private static string Affinity
        {
            get
            {
                var affinity = ConfigFile.ReadValue("Guardian", "Affinity");
                try
                {
                    return affinity;
                }
                catch (FormatException)
                {
                    return "Guardian";
                }
            }
            set { ConfigFile.WriteValue("Guardian", "Affinity", value); }
        }

        private static int AffinityIndex
        {
            get
            {
                var affinityIndex = ConfigFile.ReadValue("Guardian", "AffinityIndex");
                try
                {
                    return Convert.ToInt32(affinityIndex);
                }
                catch (FormatException)
                {
                    return 2;
                }
            }
            set { ConfigFile.WriteValue("Guardian", "AffinityIndex", value.ToString()); }
        }

        private static string AccountName
        {
            get
            {
                var accountName = ConfigFile.ReadValue("Restoration", "AccountName");
                try
                {
                    return accountName;
                }
                catch (FormatException)
                {
                    return "Account";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "AccountName", value); }
        }

        private static string RealmName
        {
            get
            {
                var realmName = ConfigFile.ReadValue("Restoration", "RealmName");
                try
                {
                    return realmName;
                }
                catch (FormatException)
                {
                    return "Realm";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "RealmName", value); }
        }

        private static string CharacterName
        {
            get
            {
                var characterName = ConfigFile.ReadValue("Restoration", "CharacterName");
                try
                {
                    return characterName;
                }
                catch (FormatException)
                {
                    return "Character";
                }
            }
            set { ConfigFile.WriteValue("Restoration", "CharacterName", value); }
        }


        // Common Methods

        public override string Name => "Restoration";

        public override string Class => "Druid";

        // Forms and PM Inialization

        public override Form SettingsForm { get; set; }


        private static Image BackroundLogo
        {
            get
            {
                const string base64String = "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b29sAAAAAABDbnRDYm9vbAAAAAAATGJsc2Jvb2wAAAAAAE5ndHZib29sAAAAAABFbWxEYm9vbAAAAAAASW50cmJvb2wAAAAAAEJja2dPYmpjAAAAAQAAAAAAAFJHQkMAAAADAAAAAFJkICBkb3ViQG/gAAAAAAAAAAAAR3JuIGRvdWJAb+AAAAAAAAAAAABCbCAgZG91YkBv4AAAAAAAAAAAAEJyZFRVbnRGI1JsdAAAAAAAAAAAAAAAAEJsZCBVbnRGI1JsdAAAAAAAAAAAAAAAAFJzbHRVbnRGI1B4bEBSAAAAAAAAAAAACnZlY3RvckRhdGFib29sAQAAAABQZ1BzZW51bQAAAABQZ1BzAAAAAFBnUEMAAAAATGVmdFVudEYjUmx0AAAAAAAAAAAAAAAAVG9wIFVudEYjUmx0AAAAAAAAAAAAAAAAU2NsIFVudEYjUHJjQFkAAAAAAAAAAAAQY3JvcFdoZW5QcmludGluZ2Jvb2wAAAAADmNyb3BSZWN0Qm90dG9tbG9uZwAAAAAAAAAMY3JvcFJlY3RMZWZ0bG9uZwAAAAAAAAANY3JvcFJlY3RSaWdodGxvbmcAAAAAAAAAC2Nyb3BSZWN0VG9wbG9uZwAAAAAAOEJJTQPtAAAAAAAQAEgAAAABAAEASAAAAAEAAThCSU0EJgAAAAAADgAAAAAAAAAAAAA/gAAAOEJJTQQNAAAAAAAEAAAAHjhCSU0EGQAAAAAABAAAAB44QklNA/MAAAAAAAkAAAAAAAAAAAEAOEJJTScQAAAAAAAKAAEAAAAAAAAAAThCSU0D9QAAAAAASAAvZmYAAQBsZmYABgAAAAAAAQAvZmYAAQChmZoABgAAAAAAAQAyAAAAAQBaAAAABgAAAAAAAQA1AAAAAQAtAAAABgAAAAAAAThCSU0D+AAAAAAAcAAA/////////////////////////////wPoAAAAAP////////////////////////////8D6AAAAAD/////////////////////////////A+gAAAAA/////////////////////////////wPoAAA4QklNBAAAAAAAAAIAADhCSU0EAgAAAAAAAgAAOEJJTQQwAAAAAAABAQA4QklNBC0AAAAAAAYAAQAAAAM4QklNBAgAAAAAABAAAAABAAACQAAAAkAAAAAA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czpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChXaW5kb3dzKSIgeG1wOkNyZWF0ZURhdGU9IjIwMTctMDEtMDNUMDM6MDk6MDMrMDI6MDAiIHhtcDpNb2RpZnlEYXRlPSIyMDE3LTAxLTAzVDAzOjM1OjM2KzAyOjAwIiB4bXA6TWV0YWRhdGFEYXRlPSIyMDE3LTAxLTAzVDAzOjM1OjM2KzAyOjAwIiBkYzpmb3JtYXQ9ImltYWdlL2pwZWciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo4NzJFM0M3NjUxRDFFNjExODUxNkJDRTM1ODBBQzIxQyIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDo4NjJFM0M3NjUxRDFFNjExODUxNkJDRTM1ODBBQzIxQyIgeG1wTU06T3JpZ2luYWxEb2N1bWVudElEPSJ4bXAuZGlkOjg2MkUzQzc2NTFEMUU2MTE4NTE2QkNFMzU4MEFDMjFDIj4gPHhtcE1NOkhpc3Rvcnk+IDxyZGY6U2VxPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0iY3JlYXRlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDo4NjJFM0M3NjUxRDFFNjExODUxNkJDRTM1ODBBQzIxQyIgc3RFdnQ6d2hlbj0iMjAxNy0wMS0wM1QwMzowOTowMyswMjowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiLz4gPHJkZjpsaSBzdEV2dDphY3Rpb249ImNvbnZlcnRlZCIgc3RFdnQ6cGFyYW1ldGVycz0iZnJvbSBpbWFnZS9wbmcgdG8gaW1hZ2UvanBlZyIvPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0ic2F2ZWQiIHN0RXZ0Omluc3RhbmNlSUQ9InhtcC5paWQ6ODcyRTNDNzY1MUQxRTYxMTg1MTZCQ0UzNTgwQUMyMUMiIHN0RXZ0OndoZW49IjIwMTctMDEtMDNUMDM6MzU6MzYrMDI6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCBDUzYgKFdpbmRvd3MpIiBzdEV2dDpjaGFuZ2VkPSIvIi8+IDwvcmRmOlNlcT4gPC94bXBNTTpIaXN0b3J5PiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8P3hwYWNrZXQgZW5kPSJ3Ij8+/+IMWElDQ19QUk9GSUxFAAEBAAAMSExpbm8CEAAAbW50clJHQiBYWVogB84AAgAJAAYAMQAAYWNzcE1TRlQAAAAASUVDIHNSR0IAAAAAAAAAAAAAAAEAAPbWAAEAAAAA0y1IUCAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAARY3BydAAAAVAAAAAzZGVzYwAAAYQAAABsd3RwdAAAAfAA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";
                var imageBytes = Convert.FromBase64String(base64String);
                var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }

        private static string AddonEmbedded
        {
            get
            {
                return "local healthFrameparty1 = nil\r\nlocal healthFrameparty2 = nil\r\nlocal healthFrameparty3 = nil\r\nlocal healthFrameparty4 = nil\r\nlocal lastHealthparty1 = 0\r\nlocal lastHealthparty2 = 0\r\nlocal lastHealthparty3 = 0\r\nlocal lastHealthparty4 = 0\r\nlocal Party1Buffs = { }\r\nlocal Party2Buffs = { }\r\nlocal Party3Buffs = { }\r\nlocal Party4Buffs = { }\r\nlocal buffLastStateParty1  = { }\r\nlocal buffLastStateParty2  = { }\r\nlocal buffLastStateParty3  = { }\r\nlocal buffLastStateParty4  = { }\r\nlocal Party1Debuff = {}\r\nlocal Party2Debuff = {}\r\nlocal Party3Debuff = {}\r\nlocal Party4Debuff = {}\r\nlocal debuffLastStateParty1  = { }\r\nlocal debuffLastStateParty2  = { }\r\nlocal debuffLastStateParty3  = { }\r\nlocal debuffLastStateParty4  = { }\r\nlocal lastInRangeparty1 = 0\r\nlocal lastInRangeparty2 = 0\r\nlocal lastInRangeparty3 = 0\r\nlocal lastInRangeparty4 = 0\r\n\r\nlocal function updateHealthparty1(self, event)  \r\n\tlocal health = UnitHealth(\"party1\");\t\t\r\n\tlocal maxHealth = UnitHealthMax(\"party1\");\r\n\tlocal partydead = UnitIsDead(\"party1\");\r\n\tlocal percHealth = ceil((health / maxHealth) * 100)\r\n\tlocal inRange = 0\r\n\tlocal isTank = \tGetSpecializationRoleByID(GetInspectSpecialization(\"party1\"))\r\n\tlocal blue = 1\r\n\tif isTank == \"TANK\" then\r\n\tblue = 0\r\n\tend \r\n\tinRange = UnitInRange(\"party1\")\t\r\n\tif (percHealth ~= lastHealthparty1 or inRange ~= lastInRangeparty1) then\r\n\t\tif partydead then\r\n\t\t\t--print(\"party1 Dead\")\r\n\t\t\thealthFrameparty1.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\treturn\r\n\t\tend\r\n\t\tlocal red = 0             \r\n\t\tlocal strHealth = \"0.0\" .. percHealth;\r\n\t\t\t\t\r\n\t\tif (percHealth >= 10) then\r\n\t\t\tstrHealth = \"0.\" .. percHealth;\r\n\t\tend\r\n\t\tred = tonumber(strHealth)\r\n\t\tif(red == nil) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty1.t:SetColorTexture(0, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty1.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\t\treturn\r\n\t\tend\r\n\t\tif (percHealth == 100) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty1.t:SetColorTexture(255, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty1.t:SetColorTexture(255, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\telse\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty1.t:SetColorTexture(red, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty1.t:SetColorTexture(red, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\tend\r\n\r\n\t\t--print (\"Health = \" .. percHealth .. \" strHealth = \".. strHealth)\r\n\t\t\t\r\n\t\tlastHealthparty1 = percHealth\r\n\t\tlastInRangeparty1 = inRange\r\n\tend\r\nend\r\n\r\nlocal function updateHealthparty2(self, event)  \r\n\tlocal health = UnitHealth(\"party2\");\t\t\r\n\tlocal maxHealth = UnitHealthMax(\"party2\");\r\n\tlocal partydead = UnitIsDead(\"party2\");\r\n\tlocal percHealth = ceil((health / maxHealth) * 100)\r\n\tlocal inRange = 0\t\t\r\n\tinRange = UnitInRange(\"party2\")  -- \'0\' if out of range, \'1\' if in range, or \'nil\' if the unit is invalid.\t\t\t\t\t\t\r\n\tlocal isTank = \tGetSpecializationRoleByID(GetInspectSpecialization(\"party2\"))\r\n\tlocal blue = 1\r\n\tif isTank == \"TANK\" then\r\n\tblue = 0\r\n\tend \r\n\tif (percHealth ~= lastHealthparty2 or inRange ~= lastInRangeparty2) then\r\n\t\tif partydead then\r\n\t\t\t--print(\"party2 Dead\")\r\n\t\t\thealthFrameparty2.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\treturn\r\n\t\tend\r\n\t\tlocal red = 0             \r\n\t\tlocal strHealth = \"0.0\" .. percHealth;\r\n\t\t\t\t\r\n\t\tif (percHealth >= 10) then\r\n\t\t\tstrHealth = \"0.\" .. percHealth;\r\n\t\tend\r\n\t\tred = tonumber(strHealth)\r\n\t\tif(red == nil) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty2.t:SetColorTexture(0, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty2.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\t\treturn\r\n\t\tend\r\n\t\tif (percHealth == 100) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty2.t:SetColorTexture(255, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty2.t:SetColorTexture(255, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\telse\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty2.t:SetColorTexture(red, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty2.t:SetColorTexture(red, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\tend\r\n\r\n\t\t--print (\"Health = \" .. percHealth .. \" strHealth = \".. strHealth)\r\n\t\t\t\r\n\t\tlastHealthparty2 = percHealth\r\n\t\tlastInRangeparty2 = inRange\r\n\tend\r\nend\r\n\r\nlocal function updateHealthparty3(self, event)  \r\n\tlocal health = UnitHealth(\"party3\");\t\t\r\n\tlocal maxHealth = UnitHealthMax(\"party3\");\r\n\tlocal partydead = UnitIsDead(\"party3\");\r\n\tlocal percHealth = ceil((health / maxHealth) * 100)\r\n\tlocal inRange = 0\r\n\tinRange = UnitInRange(\"party3\")\t\r\n\tlocal isTank = \tGetSpecializationRoleByID(GetInspectSpecialization(\"party3\"))\r\n\tlocal blue = 1\r\n\tif isTank == \"TANK\" then\r\n\tblue = 0\r\n\tend \r\n\tif (percHealth ~= lastHealthparty3 or inRange ~= lastInRangeparty3) then\r\n\t\tif partydead then\r\n\t\t\t--print(\"party3 Dead\")\r\n\t\t\thealthFrameparty3.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\treturn\r\n\t\tend\r\n\t\tlocal red = 0             \r\n\t\tlocal strHealth = \"0.0\" .. percHealth;\r\n\t\t\t\t\r\n\t\tif (percHealth >= 10) then\r\n\t\t\tstrHealth = \"0.\" .. percHealth;\r\n\t\tend\r\n\t\tred = tonumber(strHealth)\r\n\t\tif(red == nil) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty3.t:SetColorTexture(0, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty3.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\t\treturn\r\n\t\tend\r\n\t\tif (percHealth == 100) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty3.t:SetColorTexture(255, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty3.t:SetColorTexture(255, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\telse\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty3.t:SetColorTexture(red, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty3.t:SetColorTexture(red, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\tend\r\n\r\n\t\t--print (\"Health = \" .. percHealth .. \" strHealth = \".. strHealth)\r\n\t\t\t\r\n\t\tlastHealthparty3 = percHealth\r\n\t\tlastInRangeparty3 = inRange\r\n\tend\r\nend\r\n\r\nlocal function updateHealthparty4(self, event)  \r\n\tlocal health = UnitHealth(\"party4\");\t\t\r\n\tlocal maxHealth = UnitHealthMax(\"party4\");\r\n\tlocal partydead = UnitIsDead(\"party4\");\r\n\tlocal percHealth = ceil((health / maxHealth) * 100)\r\n\tlocal inRange = 0\r\n\tinRange = UnitInRange(\"party4\")\t\r\n\tlocal isTank = \tGetSpecializationRoleByID(GetInspectSpecialization(\"party4\"))\r\n\tlocal blue = 1\r\n\tif isTank == \"TANK\" then\r\n\tblue = 0\r\n\tend \r\n\tif (percHealth ~= lastHealthparty4 or inRange ~= lastInRangeparty4) then\r\n\t\tif partydead then\r\n\t\t\t--print(\"party4 Dead\")\r\n\t\t\thealthFrameparty4.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\treturn\r\n\t\tend\r\n\t\tlocal red = 0             \r\n\t\tlocal strHealth = \"0.0\" .. percHealth;\r\n\t\t\t\t\r\n\t\tif (percHealth >= 10) then\r\n\t\t\tstrHealth = \"0.\" .. percHealth;\r\n\t\tend\r\n\t\tred = tonumber(strHealth)\r\n\t\tif(red == nil) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty4.t:SetColorTexture(0, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty4.t:SetColorTexture(0, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\t\treturn\r\n\t\tend\r\n\t\tif (percHealth == 100) then\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty4.t:SetColorTexture(255, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty4.t:SetColorTexture(255, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\telse\r\n\t\t\tif (inRange) then\r\n\t\t\t\thealthFrameparty4.t:SetColorTexture(red, 0, blue, alphaColor)\r\n\t\t\telse\r\n\t\t\t\thealthFrameparty4.t:SetColorTexture(red, 1, blue, alphaColor)\r\n\t\t\tend \r\n\t\tend\r\n\r\n\t\t--print (\"Health = \" .. percHealth .. \" strHealth = \".. strHealth)\r\n\t\t\t\r\n\t\tlastHealthparty4 = percHealth\r\n\t\tlastInRangeparty4 = inRange\r\n\tend\r\nend\r\n\r\nlocal function updateParty1Buffs()\r\n\tfor _, auraId in pairs(buffs) do\r\n        local buff = \"Unitbuff\";\r\n        local auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (buffLastStateParty1[auraId] ~= \"BuffOff\") then\r\n                Party1Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party1Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty1[auraId] = \"BuffOff\"          \r\n            end    \r\n            return\r\n        end\r\n\r\n\t\tlocal name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff(\"party1\", auraName)\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\r\n\t\t\tlocal getTime = GetTime()\r\n\t\t\tlocal remainingTime = math.floor(expires - getTime + 0.5) \t\r\n\r\n\t\t\tif (buffLastStateParty1[auraId] ~= \"BuffOn\" .. count..remainingTime) then\r\n\t\t\t\tlocal green = 0\r\n\t\t\t\t\tlocal blue = 0\r\n\t\t\t\t\tlocal strcount = \"0.0\"..count;\r\n\t\t\t\tlocal strbluecount = \"0.0\"..remainingTime;\r\n\t\t\t\t\t\t\r\n\t\t\t\tif(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then \r\n\t\t\t\t\tblue = 0\r\n\t\t\t\t\tstrbluecount = 0\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif (count >= 10) then\r\n\t\t\t\t\tstrcount = \"0.\"..count;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif(remainingTime >= 10) then\r\n\t\t\t\t   strbluecount = \"0.\"..remainingTime;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tgreen = tonumber(strcount)\r\n\t\t\t\tblue = tonumber(strbluecount)\r\n\r\n\t\t\t\tParty1Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty1Buffs[auraId].t:SetAllPoints(false)\r\n\t\t\t\tbuffLastStateParty1[auraId] = \"BuffOn\" .. count..remainingTime\r\n            end\r\n        else\r\n            if (buffLastStateParty1[auraId] ~= \"BuffOff\") then\r\n                Party1Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party1Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty1[auraId] = \"BuffOff\"              \r\n            end\r\n        end\r\n    end\r\nend\r\n\r\n\r\nlocal function updateParty2Buffs()\r\n\tfor _, auraId in pairs(buffs) do\r\n        local buff = \"Unitbuff\";\r\n        local auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (buffLastStateParty2[auraId] ~= \"BuffOff\") then\r\n                Party2Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party2Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty2[auraId] = \"BuffOff\"          \r\n            end    \r\n            return\r\n        end\r\n\r\n\t\tlocal name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff(\"party2\", auraName)\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\r\n\t\t\tlocal getTime = GetTime()\r\n\t\t\tlocal remainingTime = math.floor(expires - getTime + 0.5) \t\r\n\r\n\t\t\tif (buffLastStateParty2[auraId] ~= \"BuffOn\" .. count..remainingTime) then\r\n\t\t\t\tlocal green = 0\r\n\t\t\t\t\tlocal blue = 0\r\n\t\t\t\t\tlocal strcount = \"0.0\"..count;\r\n\t\t\t\tlocal strbluecount = \"0.0\"..remainingTime;\r\n\t\t\t\t\t\t\r\n\t\t\t\tif(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then \r\n\t\t\t\t\tblue = 0\r\n\t\t\t\t\tstrbluecount = 0\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif (count >= 10) then\r\n\t\t\t\t\tstrcount = \"0.\"..count;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif(remainingTime >= 10) then\r\n\t\t\t\t   strbluecount = \"0.\"..remainingTime;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tgreen = tonumber(strcount)\r\n\t\t\t\tblue = tonumber(strbluecount)\r\n\r\n\t\t\t\tParty2Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty2Buffs[auraId].t:SetAllPoints(false)\r\n\t\t\t\tbuffLastStateParty2[auraId] = \"BuffOn\" .. count..remainingTime\r\n            end\r\n        else\r\n            if (buffLastStateParty2[auraId] ~= \"BuffOff\") then\r\n                Party2Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party2Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty2[auraId] = \"BuffOff\"              \r\n            end\r\n        end\r\n    end\r\nend\r\n\r\nlocal function updateParty3Buffs()\r\n\tfor _, auraId in pairs(buffs) do\r\n        local buff = \"Unitbuff\";\r\n        local auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (buffLastStateParty3[auraId] ~= \"BuffOff\") then\r\n                Party3Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party3Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty3[auraId] = \"BuffOff\"          \r\n            end    \r\n            return\r\n        end\r\n\r\n\t\tlocal name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff(\"party3\", auraName)\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\r\n\t\t\tlocal getTime = GetTime()\r\n\t\t\tlocal remainingTime = math.floor(expires - getTime + 0.5) \t\r\n\r\n\t\t\tif (buffLastStateParty3[auraId] ~= \"BuffOn\" .. count..remainingTime) then\r\n\t\t\t\tlocal green = 0\r\n\t\t\t\t\tlocal blue = 0\r\n\t\t\t\t\tlocal strcount = \"0.0\"..count;\r\n\t\t\t\tlocal strbluecount = \"0.0\"..remainingTime;\r\n\t\t\t\t\t\t\r\n\t\t\t\tif(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then \r\n\t\t\t\t\tblue = 0\r\n\t\t\t\t\tstrbluecount = 0\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif (count >= 10) then\r\n\t\t\t\t\tstrcount = \"0.\"..count;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif(remainingTime >= 10) then\r\n\t\t\t\t   strbluecount = \"0.\"..remainingTime;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tgreen = tonumber(strcount)\r\n\t\t\t\tblue = tonumber(strbluecount)\r\n\r\n\t\t\t\tParty3Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty3Buffs[auraId].t:SetAllPoints(false)\r\n\t\t\t\tbuffLastStateParty3[auraId] = \"BuffOn\" .. count..remainingTime\r\n            end\r\n        else\r\n            if (buffLastStateParty3[auraId] ~= \"BuffOff\") then\r\n                Party3Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party3Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty3[auraId] = \"BuffOff\"              \r\n            end\r\n        end\r\n    end\r\nend\r\n\r\nlocal function updateParty4Buffs()\r\n\tfor _, auraId in pairs(buffs) do\r\n        local buff = \"Unitbuff\";\r\n        local auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (buffLastStateParty4[auraId] ~= \"BuffOff\") then\r\n                Party4Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party4Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty4[auraId] = \"BuffOff\"          \r\n            end    \r\n            return\r\n        end\r\n\r\n\t\tlocal name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff(\"party4\", auraName)\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\r\n\t\t\tlocal getTime = GetTime()\r\n\t\t\tlocal remainingTime = math.floor(expires - getTime + 0.5) \t\r\n\r\n\t\t\tif (buffLastStateParty4[auraId] ~= \"BuffOn\" .. count..remainingTime) then\r\n\t\t\t\tlocal green = 0\r\n\t\t\t\t\tlocal blue = 0\r\n\t\t\t\t\tlocal strcount = \"0.0\"..count;\r\n\t\t\t\tlocal strbluecount = \"0.0\"..remainingTime;\r\n\t\t\t\t\t\t\r\n\t\t\t\tif(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then \r\n\t\t\t\t\tblue = 0\r\n\t\t\t\t\tstrbluecount = 0\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif (count >= 10) then\r\n\t\t\t\t\tstrcount = \"0.\"..count;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tif(remainingTime >= 10) then\r\n\t\t\t\t   strbluecount = \"0.\"..remainingTime;\r\n\t\t\t\tend\r\n\r\n\t\t\t\tgreen = tonumber(strcount)\r\n\t\t\t\tblue = tonumber(strbluecount)\r\n\r\n\t\t\t\tParty4Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty4Buffs[auraId].t:SetAllPoints(false)\r\n\t\t\t\tbuffLastStateParty4[auraId] = \"BuffOn\" .. count..remainingTime\r\n            end\r\n        else\r\n            if (buffLastStateParty4[auraId] ~= \"BuffOff\") then\r\n                Party4Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party4Buffs[auraId].t:SetAllPoints(false)\r\n                buffLastStateParty4[auraId] = \"BuffOff\"              \r\n            end\r\n        end\r\n    end\r\nend\r\n\r\nlocal function updateParty1Debuffs(self, event)\r\n    \r\n\tfor _, auraId in pairs(debuffs) do\r\n        local buff = \"UnitDebuff\";\r\n\t\tlocal auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (debuffLastStateParty1[auraId] ~= \"DebuffOff\") then\r\n                Party1Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party1Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty1[auraId] = \"DebuffOff\"               \r\n            end\r\n    \r\n            return\r\n        end\r\n        \r\n\t\t--print(\"Getting debuff for Id = \" .. auraName)\r\n\t\t\r\n        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff(\"party1\", auraName, nil, \"PLAYER|HARMFUL\")\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\t\t\t\t\t\r\n            local getTime = GetTime()\r\n            local remainingTime = math.floor(expirationTime - getTime + 0.5) \t\r\n\r\n\t\t\tif (debuffLastStateParty1[auraId] ~= \"DebuffOn\" .. count .. remainingTime) then\r\n                local green = 0\r\n                local blue = 0             \r\n                local strcount = \"0.0\" .. count;\r\n                local strbluecount = \"0.0\" .. remainingTime;\r\n                \r\n                if (count >= 10) then\r\n                    strcount = \"0.\" .. count;\r\n                end\r\n\r\n                if(remainingTime >= 10) then\r\n                   strbluecount = \"0.\" .. remainingTime\r\n                end\r\n\r\n                green = tonumber(strcount)\r\n                blue = tonumber(strbluecount)\r\n\r\n                Party1Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty1Debuff[auraId].t:SetAllPoints(false)\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" \" .. count .. \" Green: \" .. green .. \" Blue: \" .. blue)\r\n                debuffLastStateParty1[auraId] = \"DebuffOn\" .. count .. remainingTime\r\n            end\r\n        else\r\n            if (debuffLastStateParty1[auraId] ~= \"DebuffOff\") then\r\n                Party1Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party1Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty1[auraId] = \"DebuffOff\"\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" Off\")\r\n            end\r\n        end\r\n\r\n    end\r\nend\r\n\r\nlocal function updateParty2Debuffs(self, event)\r\n    \r\n\tfor _, auraId in pairs(debuffs) do\r\n        local buff = \"UnitDebuff\";\r\n\t\tlocal auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (debuffLastStateParty2[auraId] ~= \"DebuffOff\") then\r\n                Party2Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party2Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty2[auraId] = \"DebuffOff\"               \r\n            end\r\n    \r\n            return\r\n        end\r\n        \r\n\t\t--print(\"Getting debuff for Id = \" .. auraName)\r\n\t\t\r\n        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff(\"party2\", auraName, nil, \"PLAYER|HARMFUL\")\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\t\t\t\t\t\r\n            local getTime = GetTime()\r\n            local remainingTime = math.floor(expirationTime - getTime + 0.5) \t\r\n\r\n\t\t\tif (debuffLastStateParty2[auraId] ~= \"DebuffOn\" .. count .. remainingTime) then\r\n                local green = 0\r\n                local blue = 0             \r\n                local strcount = \"0.0\" .. count;\r\n                local strbluecount = \"0.0\" .. remainingTime;\r\n                \r\n                if (count >= 10) then\r\n                    strcount = \"0.\" .. count;\r\n                end\r\n\r\n                if(remainingTime >= 10) then\r\n                   strbluecount = \"0.\" .. remainingTime\r\n                end\r\n\r\n                green = tonumber(strcount)\r\n                blue = tonumber(strbluecount)\r\n\r\n                Party2Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty2Debuff[auraId].t:SetAllPoints(false)\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" \" .. count .. \" Green: \" .. green .. \" Blue: \" .. blue)\r\n                debuffLastStateParty2[auraId] = \"DebuffOn\" .. count .. remainingTime\r\n            end\r\n        else\r\n            if (debuffLastStateParty2[auraId] ~= \"DebuffOff\") then\r\n                Party2Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party2Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty2[auraId] = \"DebuffOff\"\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" Off\")\r\n            end\r\n        end\r\n\r\n    end\r\nend\r\n\r\nlocal function updateParty3Debuffs(self, event)\r\n    \r\n\tfor _, auraId in pairs(debuffs) do\r\n        local buff = \"UnitDebuff\";\r\n\t\tlocal auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (debuffLastStateParty3[auraId] ~= \"DebuffOff\") then\r\n                Party3Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party3Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty3[auraId] = \"DebuffOff\"               \r\n            end\r\n    \r\n            return\r\n        end\r\n        \r\n\t\t--print(\"Getting debuff for Id = \" .. auraName)\r\n\t\t\r\n        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff(\"party3\", auraName, nil, \"PLAYER|HARMFUL\")\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\t\t\t\t\t\r\n            local getTime = GetTime()\r\n            local remainingTime = math.floor(expirationTime - getTime + 0.5) \t\r\n\r\n\t\t\tif (debuffLastStateParty3[auraId] ~= \"DebuffOn\" .. count .. remainingTime) then\r\n                local green = 0\r\n                local blue = 0             \r\n                local strcount = \"0.0\" .. count;\r\n                local strbluecount = \"0.0\" .. remainingTime;\r\n                \r\n                if (count >= 10) then\r\n                    strcount = \"0.\" .. count;\r\n                end\r\n\r\n                if(remainingTime >= 10) then\r\n                   strbluecount = \"0.\" .. remainingTime\r\n                end\r\n\r\n                green = tonumber(strcount)\r\n                blue = tonumber(strbluecount)\r\n\r\n                Party3Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty3Debuff[auraId].t:SetAllPoints(false)\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" \" .. count .. \" Green: \" .. green .. \" Blue: \" .. blue)\r\n                debuffLastStateParty3[auraId] = \"DebuffOn\" .. count .. remainingTime\r\n            end\r\n        else\r\n            if (debuffLastStateParty3[auraId] ~= \"DebuffOff\") then\r\n                Party3Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party3Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty3[auraId] = \"DebuffOff\"\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" Off\")\r\n            end\r\n        end\r\n\r\n    end\r\nend\r\n\r\nlocal function updateParty4Debuffs(self, event)\r\n    \r\n\tfor _, auraId in pairs(debuffs) do\r\n        local buff = \"UnitDebuff\";\r\n\t\tlocal auraName = GetSpellInfo(auraId)\r\n\r\n        if auraName == nil then\r\n            if (debuffLastStateParty4[auraId] ~= \"DebuffOff\") then\r\n                Party4Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party4Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty4[auraId] = \"DebuffOff\"               \r\n            end\r\n    \r\n            return\r\n        end\r\n        \r\n\t\t--print(\"Getting debuff for Id = \" .. auraName)\r\n\t\t\r\n        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff(\"party4\", auraName, nil, \"PLAYER|HARMFUL\")\r\n\r\n\t\tif (name == auraName) then -- We have Aura up and Aura ID is matching our list\t\t\t\t\t\r\n            local getTime = GetTime()\r\n            local remainingTime = math.floor(expirationTime - getTime + 0.5) \t\r\n\r\n\t\t\tif (debuffLastStateParty4[auraId] ~= \"DebuffOn\" .. count .. remainingTime) then\r\n                local green = 0\r\n                local blue = 0             \r\n                local strcount = \"0.0\" .. count;\r\n                local strbluecount = \"0.0\" .. remainingTime;\r\n                \r\n                if (count >= 10) then\r\n                    strcount = \"0.\" .. count;\r\n                end\r\n\r\n                if(remainingTime >= 10) then\r\n                   strbluecount = \"0.\" .. remainingTime\r\n                end\r\n\r\n                green = tonumber(strcount)\r\n                blue = tonumber(strbluecount)\r\n\r\n                Party4Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)\r\n\t\t\t\tParty4Debuff[auraId].t:SetAllPoints(false)\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" \" .. count .. \" Green: \" .. green .. \" Blue: \" .. blue)\r\n                debuffLastStateParty4[auraId] = \"DebuffOn\" .. count .. remainingTime\r\n            end\r\n        else\r\n            if (debuffLastStateParty4[auraId] ~= \"DebuffOff\") then\r\n                Party4Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n                Party4Debuff[auraId].t:SetAllPoints(false)\r\n                debuffLastStateParty4[auraId] = \"DebuffOff\"\r\n                --print(\"[\" .. buff .. \"] \" .. auraName.. \" Off\")\r\n            end\r\n        end\r\n\r\n    end\r\nend\r\n\r\n\r\nlocal function InitializeThree()\r\n\tlocal i = 0\r\n\t--print (\"Initialising healthFrameparty1\")\t\r\n\thealthFrameparty1 = CreateFrame(\"frame\", \"\", parent)\r\n\thealthFrameparty1:SetSize(size, size)\r\n\thealthFrameparty1:SetPoint(\"TOPLEFT\", 0, -size * 11)                         -- row 1, column 1 [Player Health]\r\n\r\n\thealthFrameparty1.t = healthFrameparty1:CreateTexture()        \r\n\thealthFrameparty1.t:SetColorTexture(1, 1, 1, alphaColor)\r\n\thealthFrameparty1.t:SetAllPoints(healthFrameparty1)\t\r\n\thealthFrameparty1:Show()\r\n\thealthFrameparty1:SetScript(\"OnUpdate\", updateHealthparty1)\r\n\t\r\n\t\t--print (\"Initialising healthFrameparty2\")\t\r\n\thealthFrameparty2 = CreateFrame(\"frame\", \"\", parent)\r\n\thealthFrameparty2:SetSize(size, size)\r\n\thealthFrameparty2:SetPoint(\"TOPLEFT\", 1, -size * 11)                         -- row 1, column 1 [Player Health]\r\n\r\n\thealthFrameparty2.t = healthFrameparty2:CreateTexture()        \r\n\thealthFrameparty2.t:SetColorTexture(1, 1, 1, alphaColor)\r\n\thealthFrameparty2.t:SetAllPoints(healthFrameparty2)\t\r\n\thealthFrameparty2:Show()\r\n\thealthFrameparty2:SetScript(\"OnUpdate\", updateHealthparty2)\r\n\t\r\n\t\t\t--print (\"Initialising healthFrameparty3\")\t\r\n\thealthFrameparty3 = CreateFrame(\"frame\", \"\", parent)\r\n\thealthFrameparty3:SetSize(size, size)\r\n\thealthFrameparty3:SetPoint(\"TOPLEFT\",2, -size * 11)                         -- row 1, column 1 [Player Health]\r\n\r\n\thealthFrameparty3.t = healthFrameparty3:CreateTexture()        \r\n\thealthFrameparty3.t:SetColorTexture(1, 1, 1, alphaColor)\r\n\thealthFrameparty3.t:SetAllPoints(healthFrameparty3)\t\r\n\thealthFrameparty3:Show()\r\n\thealthFrameparty3:SetScript(\"OnUpdate\", updateHealthparty3)\r\n\t\r\n\t\t\t--print (\"Initialising healthFrameparty4\")\t\r\n\thealthFrameparty4 = CreateFrame(\"frame\", \"\", parent)\r\n\thealthFrameparty4:SetSize(size, size)\r\n\thealthFrameparty4:SetPoint(\"TOPLEFT\", 3, -size * 11)                         -- row 1, column 1 [Player Health]\r\n\r\n\thealthFrameparty4.t = healthFrameparty4:CreateTexture()        \r\n\thealthFrameparty4.t:SetColorTexture(1, 1, 1, alphaColor)\r\n\thealthFrameparty4.t:SetAllPoints(healthFrameparty4)\t\r\n\thealthFrameparty4:Show()\r\n\thealthFrameparty4:SetScript(\"OnUpdate\", updateHealthparty4)\r\n\t\r\n\ti = 0\r\n\tfor _, buffId in pairs(buffs) do\r\n\t\tParty1Buffs[buffId] = CreateFrame(\"frame\",\"\", parent)\r\n        Party1Buffs[buffId]:SetSize(size, size)\r\n        Party1Buffs[buffId]:SetPoint(\"TOPLEFT\", i * size, -size * 12)                            -- column 13 [Target Buffs]\r\n\t\tParty1Buffs[buffId].t = Party1Buffs[buffId]:CreateTexture()\r\n        Party1Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n        Party1Buffs[buffId].t:SetAllPoints(Party1Buffs[buffId])\r\n        Party1Buffs[buffId]:Show()\r\n        Party1Buffs[buffId]:SetScript(\"OnUpdate\", updateParty1Buffs)\r\n        i = i + 1\r\n\tend\r\n\t\r\n\ti = 0\r\n\tfor _, buffId in pairs(buffs) do\r\n\t\tParty2Buffs[buffId] = CreateFrame(\"frame\",\"\", parent)\r\n        Party2Buffs[buffId]:SetSize(size, size)\r\n        Party2Buffs[buffId]:SetPoint(\"TOPLEFT\", i * size, -size * 13)                            -- column 14 [Target Buffs]\r\n\t\tParty2Buffs[buffId].t = Party2Buffs[buffId]:CreateTexture()\r\n        Party2Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n        Party2Buffs[buffId].t:SetAllPoints(Party2Buffs[buffId])\r\n        Party2Buffs[buffId]:Show()\r\n        Party2Buffs[buffId]:SetScript(\"OnUpdate\", updateParty2Buffs)\r\n        i = i + 1\r\n\tend\r\n\t\r\n\ti = 0\r\n\tfor _, buffId in pairs(buffs) do\r\n\t\tParty3Buffs[buffId] = CreateFrame(\"frame\",\"\", parent)\r\n        Party3Buffs[buffId]:SetSize(size, size)\r\n        Party3Buffs[buffId]:SetPoint(\"TOPLEFT\", i * size, -size * 14)                            -- column 15 [Target Buffs]\r\n\t\tParty3Buffs[buffId].t = Party3Buffs[buffId]:CreateTexture()\r\n        Party3Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n        Party3Buffs[buffId].t:SetAllPoints(Party3Buffs[buffId])\r\n        Party3Buffs[buffId]:Show()\r\n        Party3Buffs[buffId]:SetScript(\"OnUpdate\", updateParty3Buffs)\r\n        i = i + 1\r\n\tend\r\n\t\r\n\ti = 0\r\n\tfor _, buffId in pairs(buffs) do\r\n\t\tParty4Buffs[buffId] = CreateFrame(\"frame\",\"\", parent)\r\n        Party4Buffs[buffId]:SetSize(size, size)\r\n        Party4Buffs[buffId]:SetPoint(\"TOPLEFT\", i * size, -size * 15)                            -- column 13 [Target Buffs]\r\n\t\tParty4Buffs[buffId].t = Party4Buffs[buffId]:CreateTexture()\r\n        Party4Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n        Party4Buffs[buffId].t:SetAllPoints(Party4Buffs[buffId])\r\n        Party4Buffs[buffId]:Show()\r\n        Party4Buffs[buffId]:SetScript(\"OnUpdate\", updateParty4Buffs)\r\n        i = i + 1\r\n\tend\r\n\t\r\n\ti = 0\r\n\tfor _, debuffId in pairs(debuffs) do\r\n\t\tParty1Debuff[debuffId] = CreateFrame(\"frame\",\"\", parent)\r\n\t\tParty1Debuff[debuffId]:SetSize(size, size)\r\n\t\tParty1Debuff[debuffId]:SetPoint(\"TOPLEFT\", i * size, -size * 16)         -- row 4, column 1+ [Spell In Range]\r\n\t\tParty1Debuff[debuffId].t = Party1Debuff[debuffId]:CreateTexture()        \r\n\t\tParty1Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n\t\tParty1Debuff[debuffId].t:SetAllPoints(Party1Debuff[debuffId])\r\n\t\tParty1Debuff[debuffId]:Show()\t\t               \r\n\t\tParty1Debuff[debuffId]:SetScript(\"OnUpdate\", updateParty1Debuffs)\r\n\t\ti = i + 1\r\n\tend\r\n\t\r\n\ti = 0\r\n\tfor _, debuffId in pairs(debuffs) do\r\n\t\tParty2Debuff[debuffId] = CreateFrame(\"frame\",\"\", parent)\r\n\t\tParty2Debuff[debuffId]:SetSize(size, size)\r\n\t\tParty2Debuff[debuffId]:SetPoint(\"TOPLEFT\", i * size, -size * 17)         -- row 4, column 1+ [Spell In Range]\r\n\t\tParty2Debuff[debuffId].t = Party2Debuff[debuffId]:CreateTexture()        \r\n\t\tParty2Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n\t\tParty2Debuff[debuffId].t:SetAllPoints(Party2Debuff[debuffId])\r\n\t\tParty2Debuff[debuffId]:Show()\t\t               \r\n\t\tParty2Debuff[debuffId]:SetScript(\"OnUpdate\", updateParty2Debuffs)\r\n\t\ti = i + 1\r\n\tend\r\n\t\r\n\ti = 0\r\n\tfor _, debuffId in pairs(debuffs) do\r\n\t\tParty3Debuff[debuffId] = CreateFrame(\"frame\",\"\", parent)\r\n\t\tParty3Debuff[debuffId]:SetSize(size, size)\r\n\t\tParty3Debuff[debuffId]:SetPoint(\"TOPLEFT\", i * size, -size * 18)         -- row 4, column 1+ [Spell In Range]\r\n\t\tParty3Debuff[debuffId].t = Party3Debuff[debuffId]:CreateTexture()        \r\n\t\tParty3Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n\t\tParty3Debuff[debuffId].t:SetAllPoints(Party3Debuff[debuffId])\r\n\t\tParty3Debuff[debuffId]:Show()\t\t               \r\n\t\tParty3Debuff[debuffId]:SetScript(\"OnUpdate\", updateParty3Debuffs)\r\n\t\ti = i + 1\r\n\tend\r\n\t\r\n\ti = 0\r\n\tfor _, debuffId in pairs(debuffs) do\r\n\t\tParty4Debuff[debuffId] = CreateFrame(\"frame\",\"\", parent)\r\n\t\tParty4Debuff[debuffId]:SetSize(size, size)\r\n\t\tParty4Debuff[debuffId]:SetPoint(\"TOPLEFT\", i * size, -size * 19)         -- row 4, column 1+ [Spell In Range]\r\n\t\tParty4Debuff[debuffId].t = Party4Debuff[debuffId]:CreateTexture()        \r\n\t\tParty4Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)\r\n\t\tParty4Debuff[debuffId].t:SetAllPoints(Party4Debuff[debuffId])\r\n\t\tParty4Debuff[debuffId]:Show()\t\t               \r\n\t\tParty4Debuff[debuffId]:SetScript(\"OnUpdate\", updateParty4Debuffs)\r\n\t\ti = i + 1\r\n\tend\r\nend";
            }
        }

        private static string Macros
        {
            get
            {
                return "\r\nVER 3 0000000000000041 \"Cenarion WardGrp\" \"ABILITY_DRUID_NATURALPERFECTION\"\r\n#showtooltip Cenarion Ward\r\n/cast [nomod,@player] Cenarion Ward\r\n/cast [mod:ctrl,mod:alt,@party4] Cenarion Ward\r\n/cast [mod:shift,@party1] Cenarion Ward\r\n/cast [mod:alt,@party2] Cenarion Ward\r\n/cast [mod:ctrl,@party3] Cenarion Ward\r\nEND\r\nVER 3 0000000000000049 \"EfflorescenceCrs\" \"INV_MISC_HERB_TALANDRASROSE\"\r\n#showtooltip Efflorescence\r\n/cast [@cursor] Efflorescence\r\nEND\r\nVER 3 000000000000003F \"Healing TouchGrp\" \"SPELL_NATURE_HEALINGTOUCH\"\r\n#showtooltip Healing Touch\r\n/cast [nomod,@player] Healing Touch\r\n/cast [mod:ctrl,mod:alt,@party4] Healing Touch\r\n/cast [mod:shift,@party1] Healing Touch\r\n/cast [mod:alt,@party2] Healing Touch\r\n/cast [mod:ctrl,@party3] Healing Touch\r\nEND\r\nVER 3 0000000000000045 \"InnervateSelf\" \"SPELL_NATURE_LIGHTNING\"\r\n#showtooltip Innervate\r\n/cast [@player] Innervate\r\nEND\r\nVER 3 0000000000000047 \"Inpsctxrun\" \"INV_MISC_QUESTIONMARK\"\r\n/script InspectUnit(\"party1\"); InspectUnit(\"party2\"); InspectUnit(\"party3\"); InspectUnit(\"party4\");\r\nEND\r\nVER 3 0000000000000044 \"IronbarkGrp\" \"SPELL_DRUID_IRONBARK\"\r\n#showtooltip Ironbark\r\n/cast [nomod,@player] Ironbark\r\n/cast [mod:ctrl,mod:alt,@party4] Ironbark\r\n/cast [mod:shift,@party1] Ironbark\r\n/cast [mod:alt,@party2] Ironbark\r\n/cast [mod:ctrl,@party3] Ironbark\r\nEND\r\nVER 3 000000000000003E \"LifebloomGrp\" \"INV_MISC_HERB_FELBLOSSOM\"\r\n#showtooltip Lifebloom\r\n/cast [nomod,@player] Lifebloom\r\n/cast [mod:ctrl,mod:alt,@party4] Lifebloom\r\n/cast [mod:shift,@party1] Lifebloom\r\n/cast [mod:alt,@party2] Lifebloom\r\n/cast [mod:ctrl,@party3] Lifebloom\r\nEND\r\nVER 3 0000000000000043 \"RebirthGrp\" \"SPELL_NATURE_REINCARNATION\"\r\n#showtooltip Rebirth\r\n/cast [nomod,@player] Rebirth\r\n/cast [mod:ctrl,mod:alt,@party4] Rebirth\r\n/cast [mod:shift,@party1] Rebirth\r\n/cast [mod:alt,@party2] Rebirth\r\n/cast [mod:ctrl,@party3] Rebirth\r\nEND\r\nVER 3 0000000000000040 \"RegrowthGrp\" \"SPELL_NATURE_RESISTNATURE\"\r\n#showtooltip Regrowth\r\n/cast [nomod,@player] Regrowth\r\n/cast [mod:ctrl,mod:alt,@party4] Regrowth\r\n/cast [mod:shift,@party1] Regrowth\r\n/cast [mod:alt,@party2] Regrowth\r\n/cast [mod:ctrl,@party3] Regrowth\r\nEND\r\nVER 3 000000000000003D \"RejuvenationGrp\" \"SPELL_NATURE_REJUVENATION\"\r\n#showtooltip Rejuvenation\r\n/cast [nomod,@player] Rejuvenation\r\n/cast [mod:ctrl,mod:alt,@party4] Rejuvenation\r\n/cast [mod:shift,@party1] Rejuvenation\r\n/cast [mod:alt,@party2] Rejuvenation\r\n/cast [mod:ctrl,@party3] Rejuvenation\r\nEND\r\nVER 3 0000000000000042 \"SwiftmendGrp\" \"INV_RELICS_IDOLOFREJUVENATION\"\r\n#showtooltip Swiftmend\r\n/cast [nomod,@player] Swiftmend\r\n/cast [mod:ctrl,mod:alt,@party4] Swiftmend\r\n/cast [mod:shift,@party1] Swiftmend\r\n/cast [mod:alt,@party2] Swiftmend\r\n/cast [mod:ctrl,@party3] Swiftmend\r\nEND\r\n";
            }
        }

        public override void Stop()
        {
            Initialized = true;
        }

        public override void Initialize()
        {
            Log.DrawHorizontalLine();
            Log.Write("Welcome to CloudMagic Restoration");
            Log.WriteCloudMagic("Welcome to CloudMagic Restoration Rotation v0.6 beta by Inhade", Color.Black);
            Log.Write("Supports all talent choices. However, recommended talents are 11313312 and 11313313", Color.Green);
            Log.Write("Make sure you have \"Force Addon Reload\" checked the first time you start the rotation");
            Log.Write("This comes with a built in addon, however you can put yours in PM folder and name it CustomHeal.lua");
            Log.Write("Enter your character account, realm and name and press create macros to create your set of macros.");
            Log.Write("This is needed to be done once.");
            Log.Write("If it fails, you can find the macros needed on top of the rotation to copy paste them");
            Log.Write("Entering your mount name and pressing Register will create a rotation file with your specific mount");
            Log.Write("so that it pauses when you are mounted. You can create multiple versions for multiple mounts.");
            Log.Write("For this, the rotation needs to be in the appropriate druid folder.");
            Log.Write("If you find that automatic tank detection is buggy for you you can target them manually during start.");
            Log.Write("Known issue: on rotation reload your keybinds seem to be reverted to none.");
            Log.Write("They are not, it is just the GUI that doesnt display them, you can use your saved keybinds");
            Log.DrawHorizontalLine();

            SettingsForm = new Form();
            TalentsGroup = new GroupBox();
            HPROCLabel = new Label();
            HPROCTrackBar = new TrackBar();
            HPROCTextLabel = new Label();
            SaveForCDsLabel = new Label();
            SpringBlossomsBox = new CheckBox();
            HP8Label = new Label();
            IncarnateNum = new NumericUpDown();
            IncarnateBox = new CheckBox();
            SoulOfTheForestBox = new CheckBox();
            HP7Label = new Label();
            RenewalNum = new NumericUpDown();
            RenewalBox = new CheckBox();
            HP6Label = new Label();
            TranquilityNum = new NumericUpDown();
            TranquilityBox = new CheckBox();
            HP5Label = new Label();
            InnervateNum = new NumericUpDown();
            InnervateBox = new CheckBox();
            HP4Label = new Label();
            FlourishNum = new NumericUpDown();
            FlourishBox = new CheckBox();
            HP3Label = new Label();
            EssenceNum = new NumericUpDown();
            EssenceBox = new CheckBox();
            RejuvenationLabel = new Label();
            HP1Label = new Label();
            RejuvenationNum = new NumericUpDown();
            HP2Label = new Label();
            GerminationNum = new NumericUpDown();
            GerminationBox = new CheckBox();
            EfflorescenceBox = new CheckBox();
            CenarionWardBox = new CheckBox();
            MiscGroup = new GroupBox();
            HighHealButton = new Button();
            ManaModifierTrackBar = new TrackBar();
            DisplayOverlayButton = new Button();
            DisplayOffSpecBox = new CheckBox();
            ManaModifierLabel = new Label();
            DisplayCDBox = new CheckBox();
            OverlayLabel = new Label();
            SelectAffinityLabel = new Label();
            OffSpecListBox = new CheckedListBox();
            Tier4Box = new CheckBox();
            ProwlOOCBox = new CheckBox();
            SettingsGroup = new GroupBox();
            CoolDownKeyCombo = new ComboBox();
            CoolDownModCombo = new ComboBox();
            CoolDownBox = new CheckBox();
            DPSKeyCombo = new ComboBox();
            DPSModCombo = new ComboBox();
            DPSModeBox = new CheckBox();
            AreKeybindsEnabledBox = new CheckBox();
            CheckIfAddonEditedBox = new CheckBox();
            InitializeWithSendsBox = new CheckBox();
            LoadExternalAddonBox = new CheckBox();
            SaveButton = new Button();
            DefaultsButton = new Button();
            SaveTranquilityBox = new CheckBox();
            SaveFlourishBox = new CheckBox();
            SaveEssenceBox = new CheckBox();
            SaveInnervateBox = new CheckBox();
            SaveRenewalBox = new CheckBox();
            SaveIncarnateBox = new CheckBox();
            AccountLabel = new Label();
            RealmLabel = new Label();
            CharacterLabel = new Label();
            AccountNameTextBox = new TextBox();
            RealmNameTextBox = new TextBox();
            CharacterNameTextBox = new TextBox();
            CreateMacrosButton = new Button();
            TankHealingKeyCombo = new ComboBox();
            TankHealingModCombo = new ComboBox();
            TankHealingBox = new CheckBox();
            NameOfMountLabel = new Label();
            NameOfMountTextBox = new TextBox();
            RegisterMountButton = new Button();
            NameOfMountLabel = new Label();
            NameOfMountTextBox = new TextBox();
            RegisterMountButton = new Button();
            DisplayTankHealingBox = new CheckBox();
            TalentsGroup.SuspendLayout();
            ((ISupportInitialize) HPROCTrackBar).BeginInit();
            ((ISupportInitialize) IncarnateNum).BeginInit();
            ((ISupportInitialize) RenewalNum).BeginInit();
            ((ISupportInitialize) TranquilityNum).BeginInit();
            ((ISupportInitialize) InnervateNum).BeginInit();
            ((ISupportInitialize) FlourishNum).BeginInit();
            ((ISupportInitialize) EssenceNum).BeginInit();
            ((ISupportInitialize) RejuvenationNum).BeginInit();
            ((ISupportInitialize) GerminationNum).BeginInit();
            MiscGroup.SuspendLayout();
            ((ISupportInitialize) ManaModifierTrackBar).BeginInit();
            SettingsGroup.SuspendLayout();
            SettingsForm.SuspendLayout();
            // 
            // TalentsGroup
            // 
            TalentsGroup.BackColor = Color.Transparent;
            TalentsGroup.Controls.Add(SaveIncarnateBox);
            TalentsGroup.Controls.Add(SaveRenewalBox);
            TalentsGroup.Controls.Add(SaveInnervateBox);
            TalentsGroup.Controls.Add(SaveEssenceBox);
            TalentsGroup.Controls.Add(SaveFlourishBox);
            TalentsGroup.Controls.Add(SaveTranquilityBox);
            TalentsGroup.Controls.Add(HPROCLabel);
            TalentsGroup.Controls.Add(HPROCTrackBar);
            TalentsGroup.Controls.Add(HPROCTextLabel);
            TalentsGroup.Controls.Add(SaveForCDsLabel);
            TalentsGroup.Controls.Add(SpringBlossomsBox);
            TalentsGroup.Controls.Add(HP8Label);
            TalentsGroup.Controls.Add(IncarnateNum);
            TalentsGroup.Controls.Add(IncarnateBox);
            TalentsGroup.Controls.Add(SoulOfTheForestBox);
            TalentsGroup.Controls.Add(HP7Label);
            TalentsGroup.Controls.Add(RenewalNum);
            TalentsGroup.Controls.Add(RenewalBox);
            TalentsGroup.Controls.Add(HP6Label);
            TalentsGroup.Controls.Add(TranquilityNum);
            TalentsGroup.Controls.Add(TranquilityBox);
            TalentsGroup.Controls.Add(HP5Label);
            TalentsGroup.Controls.Add(InnervateNum);
            TalentsGroup.Controls.Add(InnervateBox);
            TalentsGroup.Controls.Add(HP4Label);
            TalentsGroup.Controls.Add(FlourishNum);
            TalentsGroup.Controls.Add(FlourishBox);
            TalentsGroup.Controls.Add(HP3Label);
            TalentsGroup.Controls.Add(EssenceNum);
            TalentsGroup.Controls.Add(EssenceBox);
            TalentsGroup.Controls.Add(RejuvenationLabel);
            TalentsGroup.Controls.Add(HP1Label);
            TalentsGroup.Controls.Add(RejuvenationNum);
            TalentsGroup.Controls.Add(HP2Label);
            TalentsGroup.Controls.Add(GerminationNum);
            TalentsGroup.Controls.Add(GerminationBox);
            TalentsGroup.Controls.Add(EfflorescenceBox);
            TalentsGroup.Controls.Add(CenarionWardBox);
            TalentsGroup.Location = new Point(12, 12);
            TalentsGroup.Name = "TalentsGroup";
            TalentsGroup.Size = new Size(267, 588);
            TalentsGroup.TabIndex = 0;
            TalentsGroup.TabStop = false;
            TalentsGroup.Text = "Talents and Spells";
            // 
            // HPROCLabel
            // 
            HPROCLabel.AutoSize = true;
            HPROCLabel.Location = new Point(236, 531);
            HPROCLabel.Name = "HPROCLabel";
            HPROCLabel.Size = new Size(19, 13);
            HPROCLabel.TabIndex = 39;
            HPROCLabel.Text = Convert.ToString(HPROC);
            // 
            // HPROCTrackBar
            // 
            HPROCTrackBar.LargeChange = 2;
            HPROCTrackBar.Location = new Point(5, 528);
            HPROCTrackBar.Maximum = 20;
            HPROCTrackBar.Minimum = 1;
            HPROCTrackBar.Name = "HPROCTrackBar";
            HPROCTrackBar.Size = new Size(225, 45);
            HPROCTrackBar.TabIndex = 38;
            HPROCTrackBar.Value = HPROC;
            HPROCTrackBar.Scroll += HPROCTrackBar_Scroll;
            // 
            // HPROCTextLabel
            // 
            HPROCTextLabel.AutoSize = true;
            HPROCTextLabel.Location = new Point(21, 510);
            HPROCTextLabel.Name = "HPROCTextLabel";
            HPROCTextLabel.Size = new Size(193, 13);
            HPROCTextLabel.TabIndex = 36;
            HPROCTextLabel.Text = "Sensitivity of sudden HP drop detection";
            // 
            // SaveForCDsLabel
            // 
            SaveForCDsLabel.AutoSize = true;
            SaveForCDsLabel.Location = new Point(5, 331);
            SaveForCDsLabel.Name = "SaveForCDsLabel";
            SaveForCDsLabel.Size = new Size(186, 13);
            SaveForCDsLabel.TabIndex = 30;
            SaveForCDsLabel.Text = "Save for use on CoolDown activation:";
            // 
            // SpringBlossomsBox
            // 
            SpringBlossomsBox.AutoSize = true;
            SpringBlossomsBox.Location = new Point(7, 283);
            SpringBlossomsBox.Name = "SpringBlossomsBox";
            SpringBlossomsBox.Size = new Size(103, 17);
            SpringBlossomsBox.TabIndex = 28;
            SpringBlossomsBox.Text = "Spring Blossoms";
            SpringBlossomsBox.UseVisualStyleBackColor = true;
            SpringBlossomsBox.Checked = SpringBlossoms;
            SpringBlossomsBox.CheckedChanged += SpringBlossoms_CheckedChanged;
            // 
            // HP8Label
            // 
            HP8Label.AutoSize = true;
            HP8Label.Location = new Point(189, 261);
            HP8Label.Name = "HP8Label";
            HP8Label.Size = new Size(22, 13);
            HP8Label.TabIndex = 27;
            HP8Label.Text = "HP";
            // 
            // IncarnateNum
            // 
            IncarnateNum.Location = new Point(126, 259);
            IncarnateNum.Name = "IncarnateNum";
            IncarnateNum.Size = new Size(57, 20);
            IncarnateNum.TabIndex = 26;
            IncarnateNum.TextAlign = HorizontalAlignment.Right;
            IncarnateNum.Value = IncarnateHP;
            IncarnateNum.ValueChanged += IncarnateNum_ValueChanged;
            // 
            // IncarnateBox
            // 
            IncarnateBox.AutoSize = true;
            IncarnateBox.Location = new Point(7, 260);
            IncarnateBox.Name = "IncarnateBox";
            IncarnateBox.Size = new Size(79, 17);
            IncarnateBox.TabIndex = 25;
            IncarnateBox.Text = "Incarnation";
            IncarnateBox.UseVisualStyleBackColor = true;
            IncarnateBox.Checked = UseIncarnate;
            IncarnateBox.CheckedChanged += Incarnate_CheckedChanged;
            // 
            // SoulOfTheForestBox
            // 
            SoulOfTheForestBox.AutoSize = true;
            SoulOfTheForestBox.Location = new Point(7, 237);
            SoulOfTheForestBox.Name = "SoulOfTheForestBox";
            SoulOfTheForestBox.Size = new Size(109, 17);
            SoulOfTheForestBox.TabIndex = 24;
            SoulOfTheForestBox.Text = "Soul of the Forest";
            SoulOfTheForestBox.UseVisualStyleBackColor = true;
            SoulOfTheForestBox.Checked = SoulOfTheForest;
            SoulOfTheForestBox.CheckedChanged += SoulOfTheForest_CheckedChanged;
            // 
            // HP7Label
            // 
            HP7Label.AutoSize = true;
            HP7Label.Location = new Point(189, 215);
            HP7Label.Name = "HP7Label";
            HP7Label.Size = new Size(22, 13);
            HP7Label.TabIndex = 21;
            HP7Label.Text = "HP";
            // 
            // RenewalNum
            // 
            RenewalNum.Location = new Point(126, 213);
            RenewalNum.Name = "RenewalNum";
            RenewalNum.Size = new Size(57, 20);
            RenewalNum.TabIndex = 20;
            RenewalNum.TextAlign = HorizontalAlignment.Right;
            RenewalNum.Value = RenewalHP;
            RenewalNum.ValueChanged += RenewalNum_ValueChanged;
            // 
            // RenewalBox
            // 
            RenewalBox.AutoSize = true;
            RenewalBox.Location = new Point(7, 214);
            RenewalBox.Name = "RenewalBox";
            RenewalBox.Size = new Size(68, 17);
            RenewalBox.TabIndex = 19;
            RenewalBox.Text = "Renewal";
            RenewalBox.UseVisualStyleBackColor = true;
            RenewalBox.Checked = UseRenewal;
            RenewalBox.CheckedChanged += Renewal_CheckedChanged;
            // 
            // HP6Label
            // 
            HP6Label.AutoSize = true;
            HP6Label.Location = new Point(189, 192);
            HP6Label.Name = "HP6Label";
            HP6Label.Size = new Size(22, 13);
            HP6Label.TabIndex = 18;
            HP6Label.Text = "HP";
            // 
            // TranquilityNum
            // 
            TranquilityNum.Location = new Point(126, 190);
            TranquilityNum.Name = "TranquilityNum";
            TranquilityNum.Size = new Size(57, 20);
            TranquilityNum.TabIndex = 17;
            TranquilityNum.TextAlign = HorizontalAlignment.Right;
            TranquilityNum.Value = TranquilityHP;
            TranquilityNum.ValueChanged += TranquilityNum_ValueChanged;
            // 
            // TranquilityBox
            // 
            TranquilityBox.AutoSize = true;
            TranquilityBox.Location = new Point(7, 191);
            TranquilityBox.Name = "TranquilityBox";
            TranquilityBox.Size = new Size(74, 17);
            TranquilityBox.TabIndex = 16;
            TranquilityBox.Text = "Tranquility";
            TranquilityBox.UseVisualStyleBackColor = true;
            TranquilityBox.Checked = UseTranquility;
            TranquilityBox.CheckedChanged += Tranquility_CheckedChanged;
            // 
            // HP5Label
            // 
            HP5Label.AutoSize = true;
            HP5Label.Location = new Point(189, 169);
            HP5Label.Name = "HP5Label";
            HP5Label.Size = new Size(22, 13);
            HP5Label.TabIndex = 15;
            HP5Label.Text = "HP";
            // 
            // InnervateNum
            // 
            InnervateNum.Location = new Point(126, 167);
            InnervateNum.Name = "InnervateNum";
            InnervateNum.Size = new Size(57, 20);
            InnervateNum.TabIndex = 14;
            InnervateNum.TextAlign = HorizontalAlignment.Right;
            InnervateNum.Value = InnervateHP;
            InnervateNum.ValueChanged += InnervateNum_ValueChanged;
            // 
            // InnervateBox
            // 
            InnervateBox.AutoSize = true;
            InnervateBox.Location = new Point(7, 168);
            InnervateBox.Name = "InnervateBox";
            InnervateBox.Size = new Size(71, 17);
            InnervateBox.TabIndex = 13;
            InnervateBox.Text = "Innervate";
            InnervateBox.UseVisualStyleBackColor = true;
            InnervateBox.Checked = UseInnervate;
            InnervateBox.CheckedChanged += Innervate_CheckedChanged;
            // 
            // HP4Label
            // 
            HP4Label.AutoSize = true;
            HP4Label.Location = new Point(189, 146);
            HP4Label.Name = "HP4Label";
            HP4Label.Size = new Size(22, 13);
            HP4Label.TabIndex = 12;
            HP4Label.Text = "HP";
            // 
            // FlourishNum
            // 
            FlourishNum.Location = new Point(126, 143);
            FlourishNum.Name = "FlourishNum";
            FlourishNum.Size = new Size(57, 20);
            FlourishNum.TabIndex = 11;
            FlourishNum.TextAlign = HorizontalAlignment.Right;
            FlourishNum.Value = FlourishHP;
            FlourishNum.ValueChanged += FlourishNum_ValueChanged;
            // 
            // FlourishBox
            // 
            FlourishBox.AutoSize = true;
            FlourishBox.Location = new Point(7, 145);
            FlourishBox.Name = "FlourishBox";
            FlourishBox.Size = new Size(62, 17);
            FlourishBox.TabIndex = 10;
            FlourishBox.Text = "Flourish";
            FlourishBox.UseVisualStyleBackColor = true;
            FlourishBox.Checked = UseFlourish;
            FlourishBox.CheckedChanged += Flourish_CheckedChanged;
            // 
            // HP3Label
            // 
            HP3Label.AutoSize = true;
            HP3Label.Location = new Point(189, 123);
            HP3Label.Name = "HP3Label";
            HP3Label.Size = new Size(22, 13);
            HP3Label.TabIndex = 9;
            HP3Label.Text = "HP";
            // 
            // EssenceNum
            // 
            EssenceNum.Location = new Point(126, 121);
            EssenceNum.Name = "EssenceNum";
            EssenceNum.Size = new Size(57, 20);
            EssenceNum.TabIndex = 8;
            EssenceNum.TextAlign = HorizontalAlignment.Right;
            EssenceNum.Value = EssenceHP;
            EssenceNum.ValueChanged += EssenceNum_ValueChanged;
            // 
            // EssenceBox
            // 
            EssenceBox.AutoSize = true;
            EssenceBox.Location = new Point(7, 122);
            EssenceBox.Name = "EssenceBox";
            EssenceBox.Size = new Size(117, 17);
            EssenceBox.TabIndex = 7;
            EssenceBox.Text = "Essence of G\'Hanir";
            EssenceBox.UseVisualStyleBackColor = true;
            EssenceBox.Checked = UseEssence;
            EssenceBox.CheckedChanged += Essence_CheckedChanged;
            // 
            // RejuvenationLabel
            // 
            RejuvenationLabel.AutoSize = true;
            RejuvenationLabel.Location = new Point(22, 77);
            RejuvenationLabel.Name = "RejuvenationLabel";
            RejuvenationLabel.Size = new Size(82, 13);
            RejuvenationLabel.TabIndex = 0;
            RejuvenationLabel.Text = "Rejuvenation at";
            // 
            // HP1Label
            // 
            HP1Label.AutoSize = true;
            HP1Label.Location = new Point(189, 75);
            HP1Label.Name = "HP1Label";
            HP1Label.Size = new Size(22, 13);
            HP1Label.TabIndex = 6;
            HP1Label.Text = "HP";
            // 
            // RejuvenationNum
            // 
            RejuvenationNum.Location = new Point(126, 73);
            RejuvenationNum.Name = "RejuvenationNum";
            RejuvenationNum.Size = new Size(57, 20);
            RejuvenationNum.TabIndex = 4;
            RejuvenationNum.TextAlign = HorizontalAlignment.Right;
            RejuvenationNum.Value = RejuvenationHP;
            RejuvenationNum.ValueChanged += RejuvenationNum_ValueChanged;
            // 
            // HP2Label
            // 
            HP2Label.AutoSize = true;
            HP2Label.Location = new Point(189, 98);
            HP2Label.Name = "HP2Label";
            HP2Label.Size = new Size(22, 13);
            HP2Label.TabIndex = 3;
            HP2Label.Text = "HP";
            // 
            // GerminationNum
            // 
            GerminationNum.Location = new Point(126, 96);
            GerminationNum.Name = "GerminationNum";
            GerminationNum.Size = new Size(57, 20);
            GerminationNum.TabIndex = 0;
            GerminationNum.TextAlign = HorizontalAlignment.Right;
            GerminationNum.Value = GerminationHP;
            GerminationNum.ValueChanged += GerminationNum_ValueChanged;
            // 
            // GerminationBox
            // 
            GerminationBox.AutoSize = true;
            GerminationBox.Location = new Point(7, 99);
            GerminationBox.Name = "GerminationBox";
            GerminationBox.Size = new Size(94, 17);
            GerminationBox.TabIndex = 2;
            GerminationBox.Text = "Germination at";
            GerminationBox.UseVisualStyleBackColor = true;
            GerminationBox.Checked = Germination;
            GerminationBox.CheckedChanged += Germination_CheckedChanged;
            // 
            // EfflorescenceBox
            // 
            EfflorescenceBox.AutoSize = true;
            EfflorescenceBox.Location = new Point(7, 53);
            EfflorescenceBox.Name = "EfflorescenceBox";
            EfflorescenceBox.Size = new Size(91, 17);
            EfflorescenceBox.TabIndex = 1;
            EfflorescenceBox.Text = "Efflorescence";
            EfflorescenceBox.UseVisualStyleBackColor = true;
            EfflorescenceBox.Checked = UseEfflorescence;
            EfflorescenceBox.CheckedChanged += Efflorescence_CheckedChanged;
            // 
            // CenarionWardBox
            // 
            CenarionWardBox.AutoSize = true;
            CenarionWardBox.Location = new Point(7, 30);
            CenarionWardBox.Name = "CenarionWardBox";
            CenarionWardBox.Size = new Size(97, 17);
            CenarionWardBox.TabIndex = 0;
            CenarionWardBox.Text = "Cenarion Ward";
            CenarionWardBox.UseVisualStyleBackColor = true;
            CenarionWardBox.Checked = CenarionWard;
            CenarionWardBox.CheckedChanged += CenarionWard_CheckedChanged;
            // 
            // MiscGroup
            // 
            MiscGroup.BackColor = Color.Transparent;
            MiscGroup.Controls.Add(HighHealButton);
            MiscGroup.Controls.Add(ManaModifierTrackBar);
            MiscGroup.Controls.Add(DisplayOverlayButton);
            MiscGroup.Controls.Add(DisplayOffSpecBox);
            MiscGroup.Controls.Add(ManaModifierLabel);
            MiscGroup.Controls.Add(DisplayCDBox);
            MiscGroup.Controls.Add(OverlayLabel);
            MiscGroup.Controls.Add(SelectAffinityLabel);
            MiscGroup.Controls.Add(OffSpecListBox);
            MiscGroup.Controls.Add(Tier4Box);
            MiscGroup.Controls.Add(ProwlOOCBox);
            MiscGroup.Controls.Add(DisplayTankHealingBox);
            MiscGroup.Location = new Point(558, 12);
            MiscGroup.Name = "MiscGroup";
            MiscGroup.Size = new Size(267, 559);
            MiscGroup.TabIndex = 1;
            MiscGroup.TabStop = false;
            MiscGroup.Text = "Miscellaneous";
            // 
            // HighHealButton
            // 
            HighHealButton.BackColor = Color.Thistle;
            HighHealButton.FlatStyle = FlatStyle.Popup;
            HighHealButton.Location = new Point(75, 517);
            HighHealButton.Name = "HighHealButton";
            HighHealButton.Size = new Size(111, 23);
            HighHealButton.TabIndex = 40;
            HighHealButton.Text = "Boost Healing";
            HighHealButton.UseVisualStyleBackColor = false;
            HighHealButton.Click += HighHealButton_Click;
            // 
            // ManaModifierTrackBar
            // 
            ManaModifierTrackBar.Location = new Point(18, 466);
            ManaModifierTrackBar.Name = "ManaModifierTrackBar";
            ManaModifierTrackBar.Size = new Size(225, 45);
            ManaModifierTrackBar.TabIndex = 39;
            ManaModifierTrackBar.Maximum = 10;
            ManaModifierTrackBar.Minimum = -10;
            ManaModifierTrackBar.Value = ManaModifier;
            ManaModifierTrackBar.Scroll += ManaModifierTrackBar_Scroll;
            // 
            // DisplayOverlayButton
            // 
            DisplayOverlayButton.Location = new Point(6, 238);
            DisplayOverlayButton.Name = "DisplayOverlayButton";
            DisplayOverlayButton.Size = new Size(111, 23);
            DisplayOverlayButton.TabIndex = 5;
            DisplayOverlayButton.Text = "Display Overlay";
            DisplayOverlayButton.UseVisualStyleBackColor = true;
            DisplayOverlayButton.Click += DisplayOverlayButton_Click;
            // 
            // DisplayOffSpecBox
            // 
            DisplayOffSpecBox.AutoSize = true;
            DisplayOffSpecBox.Location = new Point(6, 167);
            DisplayOffSpecBox.Name = "DisplayOffSpecBox";
            DisplayOffSpecBox.Size = new Size(130, 17);
            DisplayOffSpecBox.TabIndex = 34;
            DisplayOffSpecBox.Text = "Display off spec mode";
            DisplayOffSpecBox.UseVisualStyleBackColor = true;
            DisplayOffSpecBox.Checked = DisplayDPS;
            DisplayOffSpecBox.CheckedChanged += DisplayOffSpec_CheckedChanged;
            // 
            // ManaModifierLabel
            // 
            ManaModifierLabel.AutoSize = true;
            ManaModifierLabel.Location = new Point(18, 447);
            ManaModifierLabel.Name = "ManaModifierLabel";
            ManaModifierLabel.Size = new Size(228, 13);
            ManaModifierLabel.TabIndex = 37;
            ManaModifierLabel.Text = "Less Mana/Less Heal - More Mana/More Heal";
            // 
            // DisplayCDBox
            // 
            DisplayCDBox.AutoSize = true;
            DisplayCDBox.Location = new Point(6, 190);
            DisplayCDBox.Name = "DisplayCDBox";
            DisplayCDBox.Size = new Size(107, 17);
            DisplayCDBox.TabIndex = 35;
            DisplayCDBox.Text = "Display CD mode";
            DisplayCDBox.UseVisualStyleBackColor = true;
            DisplayCDBox.Checked = DisplayCD;
            DisplayCDBox.CheckedChanged += DisplayCD_CheckedChanged;
            // 
            // DisplayTankHealingBox
            // 
            DisplayTankHealingBox.AutoSize = true;
            DisplayTankHealingBox.Location = new Point(6, 215);
            DisplayTankHealingBox.Name = "DisplayTankHealingBox";
            DisplayTankHealingBox.Size = new Size(156, 17);
            DisplayTankHealingBox.TabIndex = 41;
            DisplayTankHealingBox.Text = "Display Tank Healing mode";
            DisplayTankHealingBox.UseVisualStyleBackColor = true;
            DisplayTankHealingBox.Checked = DisplayTank;
            DisplayTankHealingBox.CheckedChanged += DisplayTank_CheckedChanged;
            // 
            // OverlayLabel
            // 
            OverlayLabel.AutoSize = true;
            OverlayLabel.Location = new Point(6, 145);
            OverlayLabel.Name = "OverlayLabel";
            OverlayLabel.Size = new Size(80, 13);
            OverlayLabel.TabIndex = 33;
            OverlayLabel.Text = "Overlay Display";
            // 
            // SelectAffinityLabel
            // 
            SelectAffinityLabel.AutoSize = true;
            SelectAffinityLabel.Location = new Point(6, 91);
            SelectAffinityLabel.Name = "SelectAffinityLabel";
            SelectAffinityLabel.Size = new Size(127, 13);
            SelectAffinityLabel.TabIndex = 32;
            SelectAffinityLabel.Text = "Select Affinity to off spec:";
            // 
            // OffSpecListBox
            // 
            OffSpecListBox.BorderStyle = BorderStyle.None;
            OffSpecListBox.FormattingEnabled = true;
            OffSpecListBox.Items.AddRange(new object[] {"Balance", "Feral", "Guardian"});
            OffSpecListBox.Location = new Point(155, 75);
            OffSpecListBox.Name = "OffSpecListBox";
            OffSpecListBox.Size = new Size(106, 45);
            OffSpecListBox.TabIndex = 31;
            OffSpecListBox.SelectionMode = SelectionMode.One;
            OffSpecListBox.SelectedIndexChanged += OffSpecListBox_CheckSelected;
            OffSpecListBox.SelectedIndex = AffinityIndex;
            OffSpecListBox.SelectedIndexChanged -= OffSpecListBox_CheckSelected;
            OffSpecListBox.CheckOnClick = true;
            OffSpecListBox.SelectedIndexChanged += OffSpecListBox_SelectedIndexChanged;
            OffSpecListBox.ItemCheck += OffSpecListBox_ItemCheck;
            // 
            // Tier4Box
            // 
            Tier4Box.AutoSize = true;
            Tier4Box.Location = new Point(6, 30);
            Tier4Box.Name = "Tier4Box";
            Tier4Box.Size = new Size(217, 17);
            Tier4Box.TabIndex = 22;
            Tier4Box.Text = "Mighty Bash/Entangling Roots/Typhoon";
            Tier4Box.UseVisualStyleBackColor = true;
            Tier4Box.Checked = UseTier4;
            Tier4Box.CheckedChanged += Tier4_CheckedChanged;
            // 
            // ProwlOOCBox
            // 
            ProwlOOCBox.AutoSize = true;
            ProwlOOCBox.Location = new Point(6, 53);
            ProwlOOCBox.Name = "ProwlOOCBox";
            ProwlOOCBox.Size = new Size(149, 17);
            ProwlOOCBox.TabIndex = 23;
            ProwlOOCBox.Text = "Prowl when out of combat";
            ProwlOOCBox.UseVisualStyleBackColor = true;
            ProwlOOCBox.Checked = ProwlOOC;
            ProwlOOCBox.CheckedChanged += ProwlOOC_CheckedChanged;
            // 
            // SettingsGroup
            // 
            SettingsGroup.SuspendLayout();
            SettingsGroup.BackColor = Color.Transparent;
            SettingsGroup.Controls.Add(CoolDownKeyCombo);
            SettingsGroup.Controls.Add(CoolDownModCombo);
            SettingsGroup.Controls.Add(CoolDownBox);
            SettingsGroup.Controls.Add(DPSKeyCombo);
            SettingsGroup.Controls.Add(DPSModCombo);
            SettingsGroup.Controls.Add(DPSModeBox);
            SettingsGroup.Controls.Add(AreKeybindsEnabledBox);
            SettingsGroup.Controls.Add(CheckIfAddonEditedBox);
            SettingsGroup.Controls.Add(InitializeWithSendsBox);
            SettingsGroup.Controls.Add(LoadExternalAddonBox);
            SettingsGroup.Controls.Add(CreateMacrosButton);
            SettingsGroup.Controls.Add(CharacterNameTextBox);
            SettingsGroup.Controls.Add(RealmNameTextBox);
            SettingsGroup.Controls.Add(AccountNameTextBox);
            SettingsGroup.Controls.Add(CharacterLabel);
            SettingsGroup.Controls.Add(RealmLabel);
            SettingsGroup.Controls.Add(AccountLabel);
            SettingsGroup.Controls.Add(TankHealingKeyCombo);
            SettingsGroup.Controls.Add(TankHealingModCombo);
            SettingsGroup.Controls.Add(TankHealingBox);
            SettingsGroup.Controls.Add(RegisterMountButton);
            SettingsGroup.Controls.Add(NameOfMountTextBox);
            SettingsGroup.Controls.Add(NameOfMountLabel);
            SettingsGroup.FlatStyle = FlatStyle.Popup;
            SettingsGroup.Location = new Point(285, 12);
            SettingsGroup.Name = "SettingsGroup";
            SettingsGroup.Size = new Size(267, 588);
            SettingsGroup.TabIndex = 2;
            SettingsGroup.TabStop = false;
            SettingsGroup.Text = "Settings";
            SettingsGroup.ResumeLayout(false);
            // 
            // CoolDownKeyCombo
            // 
            CoolDownKeyCombo.FormattingEnabled = false;
            CoolDownKeyCombo.Location = new Point(180, 163);
            CoolDownKeyCombo.Name = "CoolDownKeyCombo";
            CoolDownKeyCombo.Size = new Size(81, 21);
            CoolDownKeyCombo.TabIndex = 40;
            // this.CoolDownKeyCombo.SuspendLayout();
            CoolDownKeyCombo.DataSource = Enum.GetValues(typeof(WoW.Keys));
            CoolDownKeyCombo.SelectedIndexChanged += CoolDownKeyCombo_SelectedIndexChanged;
            CoolDownKeyCombo.SelectedIndex = CoolDownKeyCombo.FindString(CoolDownKey);
            // this.CoolDownKeyCombo.ResumeLayout();
            // this.CoolDownKeyCombo.SelectedText = CoolDownKey;
            // 
            // CoolDownModCombo
            // 
            CoolDownModCombo.FormattingEnabled = true;
            CoolDownModCombo.Location = new Point(93, 163);
            CoolDownModCombo.Name = "CoolDownModCombo";
            CoolDownModCombo.Size = new Size(81, 21);
            CoolDownModCombo.TabIndex = 39;
            CoolDownModCombo.DataSource = Enum.GetValues(typeof(ModifierKeys));
            CoolDownModCombo.SelectedIndex = CoolDownModCombo.FindString(CoolDownMod);
            CoolDownModCombo.SelectedIndexChanged += CoolDownModCombo_SelectedIndexChanged;
            // 
            // CoolDownBox
            // 
            CoolDownBox.AutoSize = true;
            CoolDownBox.Location = new Point(6, 165);
            CoolDownBox.Name = "CoolDownBox";
            CoolDownBox.Size = new Size(78, 17);
            CoolDownBox.TabIndex = 38;
            CoolDownBox.Text = "Cooldowns";
            CoolDownBox.UseVisualStyleBackColor = true;
            CoolDownBox.Checked = UseCoolDowns;
            CoolDownBox.CheckedChanged += CoolDown_CheckedChanged;
            // 
            // DPSKeyCombo
            // 
            DPSKeyCombo.FormattingEnabled = true;
            DPSKeyCombo.Location = new Point(180, 140);
            DPSKeyCombo.Name = "DPSKeyCombo";
            DPSKeyCombo.Size = new Size(81, 21);
            DPSKeyCombo.TabIndex = 37;
            DPSKeyCombo.DataSource = Enum.GetValues(typeof(WoW.Keys));
            DPSKeyCombo.SelectedIndex = DPSKeyCombo.FindString(DPSModeKey);
            DPSKeyCombo.SelectedIndexChanged += DPSKeyCombo_SelectedIndexChanged;
            DPSKeyCombo.SelectedText = DPSModeKey;
            // 
            // DPSModCombo
            // 
            DPSModCombo.FormattingEnabled = true;
            DPSModCombo.Location = new Point(93, 140);
            DPSModCombo.Name = "DPSModCombo";
            DPSModCombo.Size = new Size(81, 21);
            DPSModCombo.TabIndex = 36;
            DPSModCombo.DataSource = Enum.GetValues(typeof(ModifierKeys));
            DPSModCombo.SelectedIndex = DPSModCombo.FindString(DPSModeMod);
            DPSModCombo.SelectedIndexChanged += DPSModCombo_SelectedIndexChanged;
            // 
            // DPSModeBox
            // 
            DPSModeBox.AutoSize = true;
            DPSModeBox.Location = new Point(6, 142);
            DPSModeBox.Name = "DPSModeBox";
            DPSModeBox.Size = new Size(66, 17);
            DPSModeBox.TabIndex = 35;
            DPSModeBox.Text = "Off spec";
            DPSModeBox.UseVisualStyleBackColor = true;
            DPSModeBox.Checked = UseDPSMode;
            DPSModeBox.CheckedChanged += DPSMode_CheckedChanged;
            // 
            // TankHealingKeyCombo
            // 
            TankHealingKeyCombo.FormattingEnabled = true;
            TankHealingKeyCombo.Location = new Point(180, 185);
            TankHealingKeyCombo.Name = "TankHealingKeyCombo";
            TankHealingKeyCombo.Size = new Size(81, 21);
            TankHealingKeyCombo.TabIndex = 54;
            TankHealingKeyCombo.DataSource = Enum.GetValues(typeof(WoW.Keys));
            TankHealingKeyCombo.SelectedIndex = TankHealingKeyCombo.FindString(TankHealingKey);
            TankHealingKeyCombo.SelectedIndexChanged += TankHealingKeyCombo_SelectedIndexChanged;
            // 
            // TankHealingModCombo
            // 
            TankHealingModCombo.FormattingEnabled = true;
            TankHealingModCombo.Location = new Point(93, 185);
            TankHealingModCombo.Name = "TankHealingModCombo";
            TankHealingModCombo.Size = new Size(81, 21);
            TankHealingModCombo.TabIndex = 53;
            TankHealingModCombo.DataSource = Enum.GetValues(typeof(ModifierKeys));
            TankHealingModCombo.SelectedIndex = TankHealingModCombo.FindString(TankHealingMod);
            TankHealingModCombo.SelectedIndexChanged += TankHealingModCombo_SelectedIndexChanged;
            // 
            // TankHealingBox
            // 
            TankHealingBox.AutoSize = true;
            TankHealingBox.Location = new Point(6, 187);
            TankHealingBox.Name = "TankHealingBox";
            TankHealingBox.Size = new Size(90, 17);
            TankHealingBox.TabIndex = 52;
            TankHealingBox.Text = "Tank Healing";
            TankHealingBox.UseVisualStyleBackColor = true;
            TankHealingBox.Checked = UseTankHealing;
            TankHealingBox.CheckedChanged += TankHealing_CheckedChanged;
            // 
            // AreKeybindsEnabledBox
            // 
            AreKeybindsEnabledBox.AutoSize = true;
            AreKeybindsEnabledBox.Location = new Point(6, 119);
            AreKeybindsEnabledBox.Name = "AreKeybindsEnabledBox";
            AreKeybindsEnabledBox.Size = new Size(91, 17);
            AreKeybindsEnabledBox.TabIndex = 33;
            AreKeybindsEnabledBox.Text = "Use Keybinds";
            AreKeybindsEnabledBox.UseVisualStyleBackColor = true;
            AreKeybindsEnabledBox.Checked = AreKeybindsEnabled;
            AreKeybindsEnabledBox.CheckedChanged += AreKeybindsEnabled_CheckedChanged;
            // 
            // CheckIfAddonEditedBox
            // 
            CheckIfAddonEditedBox.AutoSize = true;
            CheckIfAddonEditedBox.Location = new Point(6, 76);
            CheckIfAddonEditedBox.Name = "CheckIfAddonEditedBox";
            CheckIfAddonEditedBox.Size = new Size(216, 17);
            CheckIfAddonEditedBox.TabIndex = 32;
            CheckIfAddonEditedBox.Text = "Force Reload of Addon on Each Restart";
            CheckIfAddonEditedBox.UseVisualStyleBackColor = true;
            CheckIfAddonEditedBox.Checked = ForceAddonReload;
            CheckIfAddonEditedBox.CheckedChanged += CheckIfAddonEdited_CheckedChanged;
            // 
            // InitializeWithSendsBox
            // 
            InitializeWithSendsBox.AutoSize = true;
            InitializeWithSendsBox.Location = new Point(6, 53);
            InitializeWithSendsBox.Name = "InitializeWithSendsBox";
            InitializeWithSendsBox.Size = new Size(122, 17);
            InitializeWithSendsBox.TabIndex = 31;
            InitializeWithSendsBox.Text = "Manual Tank Select";
            InitializeWithSendsBox.UseVisualStyleBackColor = true;
            InitializeWithSendsBox.Checked = InitializeWithSends;
            InitializeWithSendsBox.CheckedChanged += InitializeWithSends_CheckedChanged;
            // 
            // LoadExternalAddonBox
            // 
            LoadExternalAddonBox.AutoSize = true;
            LoadExternalAddonBox.Location = new Point(6, 30);
            LoadExternalAddonBox.Name = "LoadExternalAddonBox";
            LoadExternalAddonBox.Size = new Size(125, 17);
            LoadExternalAddonBox.TabIndex = 0;
            LoadExternalAddonBox.Text = "Load External Addon";
            LoadExternalAddonBox.UseVisualStyleBackColor = true;
            LoadExternalAddonBox.Checked = LoadExternalAddon;
            LoadExternalAddonBox.CheckedChanged += LoadExternalAddon_CheckedChanged;
            // 
            // NameOfMountLabel
            // 
            NameOfMountLabel.AutoSize = true;
            NameOfMountLabel.Location = new Point(7, 312);
            NameOfMountLabel.Name = "NameOfMountLabel";
            NameOfMountLabel.Size = new Size(82, 13);
            NameOfMountLabel.TabIndex = 55;
            NameOfMountLabel.Text = "Name of mount:";
            // 
            // NameOfMountTextBox
            // 
            NameOfMountTextBox.Location = new Point(93, 309);
            NameOfMountTextBox.Name = "NameOfMountTextBox";
            NameOfMountTextBox.Size = new Size(79, 20);
            NameOfMountTextBox.TabIndex = 56;
            NameOfMountTextBox.Text = MountName;
            NameOfMountTextBox.TextAlign = HorizontalAlignment.Right;
            NameOfMountTextBox.TextChanged += NameOfMountTextBox_TextChanged;
            // 
            // RegisterMountButton
            // 
            RegisterMountButton.Location = new Point(181, 308);
            RegisterMountButton.Name = "RegisterMountButton";
            RegisterMountButton.Size = new Size(78, 23);
            RegisterMountButton.TabIndex = 57;
            RegisterMountButton.Text = "Register";
            RegisterMountButton.UseVisualStyleBackColor = true;
            RegisterMountButton.Click += RegisterMountButton_Click;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(750, 577);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 3;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // DefaultsButton
            // 
            DefaultsButton.Location = new Point(655, 577);
            DefaultsButton.Name = "DefaultsButton";
            DefaultsButton.Size = new Size(89, 23);
            DefaultsButton.TabIndex = 4;
            DefaultsButton.Text = "Load Defaults";
            DefaultsButton.UseVisualStyleBackColor = true;
            DefaultsButton.UseVisualStyleBackColor = true;
            DefaultsButton.Click += DefaultsButton_Click;
            // 
            // SaveTranquilityBox
            // 
            SaveTranquilityBox.AutoSize = true;
            SaveTranquilityBox.Location = new Point(7, 352);
            SaveTranquilityBox.Name = "SaveTranquilityBox";
            SaveTranquilityBox.Size = new Size(74, 17);
            SaveTranquilityBox.TabIndex = 40;
            SaveTranquilityBox.Text = "Tranquility";
            SaveTranquilityBox.UseVisualStyleBackColor = true;
            SaveTranquilityBox.Checked = SaveTranquility;
            SaveTranquilityBox.CheckedChanged += SaveTranquility_CheckedChanged;
            // 
            // SaveFlourishBox
            // 
            SaveFlourishBox.AutoSize = true;
            SaveFlourishBox.Location = new Point(7, 373);
            SaveFlourishBox.Name = "SaveFlourishBox";
            SaveFlourishBox.Size = new Size(62, 17);
            SaveFlourishBox.TabIndex = 41;
            SaveFlourishBox.Text = "Flourish";
            SaveFlourishBox.UseVisualStyleBackColor = true;
            SaveFlourishBox.Checked = SaveFlourish;
            SaveFlourishBox.CheckedChanged += SaveFlourish_CheckedChanged;
            // 
            // SaveEssenceBox
            // 
            SaveEssenceBox.AutoSize = true;
            SaveEssenceBox.Location = new Point(7, 396);
            SaveEssenceBox.Name = "SaveEssenceBox";
            SaveEssenceBox.Size = new Size(117, 17);
            SaveEssenceBox.TabIndex = 42;
            SaveEssenceBox.Text = "Essence of G\'Hanir";
            SaveEssenceBox.UseVisualStyleBackColor = true;
            SaveEssenceBox.Checked = SaveEssence;
            SaveEssenceBox.CheckedChanged += SaveEssence_CheckedChanged;
            // 
            // SaveInnervateBox
            // 
            SaveInnervateBox.AutoSize = true;
            SaveInnervateBox.Location = new Point(7, 419);
            SaveInnervateBox.Name = "SaveInnervateBox";
            SaveInnervateBox.Size = new Size(71, 17);
            SaveInnervateBox.TabIndex = 43;
            SaveInnervateBox.Text = "Innervate";
            SaveInnervateBox.UseVisualStyleBackColor = true;
            SaveInnervateBox.Checked = SaveInnervate;
            SaveInnervateBox.CheckedChanged += SaveInnervate_CheckedChanged;
            // 
            // SaveRenewalBox
            // 
            SaveRenewalBox.AutoSize = true;
            SaveRenewalBox.Location = new Point(7, 442);
            SaveRenewalBox.Name = "SaveRenewalBox";
            SaveRenewalBox.Size = new Size(68, 17);
            SaveRenewalBox.TabIndex = 44;
            SaveRenewalBox.Text = "Renewal";
            SaveRenewalBox.UseVisualStyleBackColor = true;
            SaveRenewalBox.Checked = SaveRenewal;
            SaveRenewalBox.CheckedChanged += SaveRenewal_CheckedChanged;
            // 
            // SaveIncarnateBox
            // 
            SaveIncarnateBox.AutoSize = true;
            SaveIncarnateBox.Location = new Point(7, 465);
            SaveIncarnateBox.Name = "SaveIncarnateBox";
            SaveIncarnateBox.Size = new Size(139, 17);
            SaveIncarnateBox.TabIndex = 45;
            SaveIncarnateBox.Text = "Incarnation: Tree of Life";
            SaveIncarnateBox.UseVisualStyleBackColor = true;
            SaveIncarnateBox.Checked = SaveIncarnate;
            SaveIncarnateBox.CheckedChanged += SaveIncarnate_CheckedChanged;
            // 
            // AccountLabel
            // 
            AccountLabel.AutoSize = true;
            AccountLabel.Location = new Point(27, 220);
            AccountLabel.Name = "AccountLabel";
            AccountLabel.Size = new Size(47, 13);
            AccountLabel.TabIndex = 46;
            AccountLabel.Text = "Account";
            // 
            // RealmLabel
            // 
            RealmLabel.AutoSize = true;
            RealmLabel.Location = new Point(113, 220);
            RealmLabel.Name = "RealmLabel";
            RealmLabel.Size = new Size(37, 13);
            RealmLabel.TabIndex = 47;
            RealmLabel.Text = "Realm";
            // 
            // CharacterLabel
            // 
            CharacterLabel.AutoSize = true;
            CharacterLabel.Location = new Point(178, 218);
            CharacterLabel.Name = "CharacterLabel";
            CharacterLabel.Size = new Size(84, 13);
            CharacterLabel.TabIndex = 48;
            CharacterLabel.Text = "Character";
            // 
            // AccountNameTextBox
            // 
            AccountNameTextBox.Location = new Point(10, 237);
            AccountNameTextBox.Name = "AccountNameTextBox";
            AccountNameTextBox.Size = new Size(79, 20);
            AccountNameTextBox.TabIndex = 49;
            AccountNameTextBox.Text = AccountName;
            AccountNameTextBox.TextAlign = HorizontalAlignment.Right;
            AccountNameTextBox.TextChanged += AccountNameTextBox_TextChanged;
            // 
            // RealmNameTextBox
            // 
            RealmNameTextBox.Location = new Point(95, 237);
            RealmNameTextBox.Name = "RealmNameTextBox";
            RealmNameTextBox.Size = new Size(79, 20);
            RealmNameTextBox.TabIndex = 50;
            RealmNameTextBox.Text = RealmName;
            RealmNameTextBox.TextAlign = HorizontalAlignment.Right;
            RealmNameTextBox.TextChanged += RealmNameTextBox_TextChanged;
            // 
            // CharacterNameTextBox
            // 
            CharacterNameTextBox.Location = new Point(180, 237);
            CharacterNameTextBox.Name = "CharacterNameTextBox";
            CharacterNameTextBox.Size = new Size(79, 20);
            CharacterNameTextBox.TabIndex = 51;
            CharacterNameTextBox.Text = CharacterName;
            CharacterNameTextBox.TextAlign = HorizontalAlignment.Right;
            CharacterNameTextBox.TextChanged += CharacterNameTextBox_TextChanged;
            // 
            // CreateMacrosButton
            // 
            CreateMacrosButton.Location = new Point(170, 263);
            CreateMacrosButton.Name = "CreateMacrosButton";
            CreateMacrosButton.Size = new Size(89, 23);
            CreateMacrosButton.TabIndex = 5;
            CreateMacrosButton.Text = "Create Macros";
            CreateMacrosButton.UseVisualStyleBackColor = true;
            CreateMacrosButton.Click += CreateMacrosButton_Click;

            // 
            // SettingsForm
            // 
            SettingsForm.AutoScaleDimensions = new SizeF(6F, 13F);
            SettingsForm.AutoScaleMode = AutoScaleMode.Font;
            SettingsForm.BackgroundImage = BackroundLogo;
            SettingsForm.BackgroundImageLayout = ImageLayout.Zoom;
            SettingsForm.ClientSize = new Size(837, 612);
            SettingsForm.Controls.Add(DefaultsButton);
            SettingsForm.Controls.Add(SaveButton);
            SettingsForm.Controls.Add(SettingsGroup);
            SettingsForm.Controls.Add(MiscGroup);
            SettingsForm.Controls.Add(TalentsGroup);
            SettingsForm.Name = "SettingsForm";
            SettingsForm.Text = "Restoration Druid Settings";
            TalentsGroup.ResumeLayout(false);
            TalentsGroup.PerformLayout();
            ((ISupportInitialize) HPROCTrackBar).EndInit();
            ((ISupportInitialize) IncarnateNum).EndInit();
            ((ISupportInitialize) RenewalNum).EndInit();
            ((ISupportInitialize) TranquilityNum).EndInit();
            ((ISupportInitialize) InnervateNum).EndInit();
            ((ISupportInitialize) FlourishNum).EndInit();
            ((ISupportInitialize) EssenceNum).EndInit();
            ((ISupportInitialize) RejuvenationNum).EndInit();
            ((ISupportInitialize) GerminationNum).EndInit();
            MiscGroup.ResumeLayout(false);
            MiscGroup.PerformLayout();
            ((ISupportInitialize) ManaModifierTrackBar).EndInit();
            SettingsGroup.ResumeLayout(false);
            SettingsGroup.PerformLayout();
            SettingsForm.ResumeLayout(false);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Settings saved", "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SettingsForm.Close();
            foreach (var value in DisplayInfoForm.Processes)
            {
                Log.Write(Convert.ToString("Connected to WoW with id: " + value.Id));
            }
        }

        private void DefaultsButton_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Restore default values?\r\nRestart rotation for changes to take effect", "CloudMagic", MessageBoxButtons.YesNo);
            switch (dialogResult)
            {
                case DialogResult.Yes:
                    SettingsForm.SuspendLayout();
                    Germination = true;
                    SoulOfTheForest = false;
                    SpringBlossoms = false;
                    CenarionWard = true;
                    UseTranquility = false;
                    SaveTranquility = false;
                    UseEssence = true;
                    SaveEssence = true;
                    UseFlourish = false;
                    SaveFlourish = false;
                    UseInnervate = false;
                    SaveInnervate = true;
                    UseEfflorescence = true;
                    UseRenewal = false;
                    SaveRenewal = false;
                    UseIncarnate = false;
                    SaveIncarnate = false;
                    UseTier4 = true;
                    InitializeWithSends = false;
                    ForceAddonReload = true;
                    LoadExternalAddon = false;
                    UseDPSMode = false;
                    UseCoolDowns = false;
                    UseTankHealing = false;
                    AreKeybindsEnabled = false;
                    DisplayDPS = true;
                    DisplayCD = true;
                    DisplayTank = true;
                    ProwlOOC = false;
                    HPROC = 10;
                    MountName = "Corrupted Fire Hawk";
                    CoolDownKey = "None";
                    CoolDownMod = "None";
                    DPSModeKey = "None";
                    TankHealingMod = "None";
                    TankHealingKey = "None";
                    DPSModeMod = "None";
                    CoolKey = 0;
                    CoolMod = 0;
                    DPSKey = 0;
                    DPSMod = 0;
                    TankKey = 0;
                    TankMod = 0;
                    EfflorescenceHP = 80;
                    InnervateHP = 65;
                    FlourishHP = 70;
                    WildGrowthHP = 80;
                    EssenceHP = 80;
                    TranquilityHP = 65;
                    RegrowthHP = 80;
                    HealingTouchHP = 40;
                    RejuvenationHP = 80;
                    GerminationHP = 60;
                    SwiftmendHP = 40;
                    IronbarkHP = 80;
                    RenewalHP = 40;
                    IncarnateHP = 50;
                    ManaModifier = 0;
                    Affinity = "Guardian";
                    AffinityIndex = 2;
                    AccountName = "Account";
                    RealmName = "Realm";
                    CharacterName = "Character";
                    SettingsForm.Close();
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void RegisterMountButton_Click(object sender, EventArgs e)
        {
            try
            {
                var personalRotation = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\" + "Rotations" + "\\" + "Druid" + "\\" + "Druid-Restoration-Inhade.cs");
                Log.Write("Registering Mount");
                personalRotation = personalRotation.Replace("Aura,40192,Mount", "Aura,40192," + MountName);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\" + "Rotations" + "\\" + "Druid" + "\\" + "Druid-Restoration-Inhade-" + MountName + ".cs", personalRotation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void DisplayOverlayButton_Click(object sender, EventArgs e)
        {
            DisplayText = "Overlay Activated";
            var frm = new DisplayInfoFormRDI();
            frm.Show();
            Log.Write("Overlay Activated", Color.Green);
        }

        private void HighHealButton_Click(object sender, EventArgs e)
        {
            DisplayText = "Boosting heal for 30 secs";
            Log.Write("Boosting heal for 30 secs", Color.Red);
            HealingIntensity = "High";
            HealingIntensityTimer.Reset();
            HealingIntensityTimer.Start();
        }

        private void CreateMacrosButton_Click(object sender, EventArgs e)
        {
            Log.Write("Attempting to create macros. Relog and check your character (not global) macros menu.", Color.Red);
            try
            {
                var macroCache = File.ReadAllText("" + WoW.InstallPath + "\\" + "WTF\\Account" + "\\" + AccountName + "\\" + "macros-cache.txt");
                File.WriteAllText("" + WoW.InstallPath + "\\" + "WTF\\Account" + "\\" + AccountName + "\\" + "macros-cache.txt", macroCache + Macros);
                MessageBox.Show("Saving account wide macros succeded. Please relog and assign keybinds", "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Saving account wide macros failed, trying character specific. Make sure you have space for the extra macros", "CloudMagic", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                try
                {
                    var macroCache = File.ReadAllText("" + WoW.InstallPath + "\\" + "WTF\\Account" + "\\" + AccountName + "\\" + "macros-cache.txt");
                    File.WriteAllText("" + WoW.InstallPath + "\\" + "WTF\\Account" + "\\" + AccountName + "\\" + RealmName + "\\" + CharacterName + "\\" + "macros-cache.txt",
                        macroCache + Macros);
                }
                catch (Exception)
                {
                    MessageBox.Show("Saving character specific macros failed. Please manually create macros", "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                SettingsForm.Close();
            }
        }

        private void DPSKeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            DPSModeKey = Convert.ToString(DPSKeyCombo.SelectedItem);
            DPSKey = (int) Enum.Parse(typeof(WoW.Keys), DPSModeKey);
        }

        private void DPSModCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            DPSModeMod = Convert.ToString(DPSModCombo.SelectedItem);
            DPSMod = (int) Enum.Parse(typeof(ModifierKeys), DPSModeMod);
        }

        private void CoolDownKeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CoolDownKey = Convert.ToString(CoolDownKeyCombo.SelectedItem);
            CoolKey = (int) Enum.Parse(typeof(WoW.Keys), CoolDownKey);
            Log.Write(Convert.ToString(CoolDownKeyCombo.FindString(CoolDownKey)));
            Log.Write(Convert.ToString(CoolDownKeyCombo.DataSource));
        }

        private void TankHealingKeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            TankHealingKey = Convert.ToString(TankHealingKeyCombo.SelectedItem);
            TankKey = (int) Enum.Parse(typeof(WoW.Keys), TankHealingKey);
        }

        private void TankHealingModCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            TankHealingMod = Convert.ToString(TankHealingModCombo.SelectedItem);
            TankMod = (int) Enum.Parse(typeof(ModifierKeys), TankHealingMod);
        }

        private void CoolDownModCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CoolDownMod = Convert.ToString(CoolDownModCombo.SelectedItem);
            CoolMod = (int) Enum.Parse(typeof(ModifierKeys), CoolDownMod);
        }

        private void HPROCTrackBar_Scroll(object sender, EventArgs e)
        {
            HPROC = HPROCTrackBar.Value;
            HPROCLabel.Text = Convert.ToString(HPROCTrackBar.Value);
        }

        private void ManaModifierTrackBar_Scroll(object sender, EventArgs e)
        {
            ManaModifier = ManaModifierTrackBar.Value;
        }

        private void OffSpecListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (OffSpecListBox.SelectedIndices[0])
            {
                case 0:
                    Affinity = "Balance";
                    AffinityIndex = 0;
                    break;
                case 1:
                    Affinity = "Feral";
                    AffinityIndex = 1;
                    break;
                case 2:
                    Affinity = "Guardian";
                    AffinityIndex = 2;
                    break;
            }
        }

        private void OffSpecListBox_CheckSelected(object sender, EventArgs e)
        {
            var itemIndex = OffSpecListBox.SelectedIndices[0];
            OffSpecListBox.SetItemChecked(itemIndex, true);
        }


        private void OffSpecListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue != CheckState.Checked)
            {
                return;
            }
            var selectedItems = OffSpecListBox.CheckedIndices;
            if (selectedItems.Count > 0)
            {
                OffSpecListBox.SetItemChecked(selectedItems[0], false);
            }
        }

        private void CharacterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            CharacterName = CharacterNameTextBox.Text;
        }

        private void RealmNameTextBox_TextChanged(object sender, EventArgs e)
        {
            RealmName = RealmNameTextBox.Text;
        }

        private void AccountNameTextBox_TextChanged(object sender, EventArgs e)
        {
            AccountName = AccountNameTextBox.Text;
        }

        private void NameOfMountTextBox_TextChanged(object sender, EventArgs e)
        {
            MountName = NameOfMountTextBox.Text;
        }

        private void IncarnateNum_ValueChanged(object sender, EventArgs e)
        {
            IncarnateHP = (int) IncarnateNum.Value;
        }

        private void RenewalNum_ValueChanged(object sender, EventArgs e)
        {
            RenewalHP = (int) RenewalNum.Value;
        }

        private void TranquilityNum_ValueChanged(object sender, EventArgs e)
        {
            TranquilityHP = (int) TranquilityNum.Value;
        }

        private void InnervateNum_ValueChanged(object sender, EventArgs e)
        {
            InnervateHP = (int) InnervateNum.Value;
        }

        private void FlourishNum_ValueChanged(object sender, EventArgs e)
        {
            FlourishHP = (int) FlourishNum.Value;
        }

        private void EssenceNum_ValueChanged(object sender, EventArgs e)
        {
            EssenceHP = (int) EssenceNum.Value;
        }

        private void RejuvenationNum_ValueChanged(object sender, EventArgs e)
        {
            RejuvenationHP = (int) RejuvenationNum.Value;
        }

        private void GerminationNum_ValueChanged(object sender, EventArgs e)
        {
            GerminationHP = (int) GerminationNum.Value;
        }

        private void SaveTranquility_CheckedChanged(object sender, EventArgs e)
        {
            SaveTranquility = SaveTranquilityBox.Checked;
        }

        private void SaveIncarnate_CheckedChanged(object sender, EventArgs e)
        {
            SaveIncarnate = SaveIncarnateBox.Checked;
        }

        private void SaveRenewal_CheckedChanged(object sender, EventArgs e)
        {
            SaveRenewal = SaveRenewalBox.Checked;
        }

        private void SaveInnervate_CheckedChanged(object sender, EventArgs e)
        {
            SaveInnervate = SaveInnervateBox.Checked;
        }

        private void SaveEssence_CheckedChanged(object sender, EventArgs e)
        {
            SaveEssence = SaveEssenceBox.Checked;
        }

        private void SaveFlourish_CheckedChanged(object sender, EventArgs e)
        {
            SaveFlourish = SaveFlourishBox.Checked;
        }

        private void SpringBlossoms_CheckedChanged(object sender, EventArgs e)
        {
            SpringBlossoms = SpringBlossomsBox.Checked;
        }

        private void Incarnate_CheckedChanged(object sender, EventArgs e)
        {
            UseIncarnate = IncarnateBox.Checked;
        }

        private void SoulOfTheForest_CheckedChanged(object sender, EventArgs e)
        {
            SoulOfTheForest = SoulOfTheForestBox.Checked;
        }

        private void Renewal_CheckedChanged(object sender, EventArgs e)
        {
            UseRenewal = RenewalBox.Checked;
        }

        private void Tranquility_CheckedChanged(object sender, EventArgs e)
        {
            UseTranquility = TranquilityBox.Checked;
        }

        private void Innervate_CheckedChanged(object sender, EventArgs e)
        {
            UseInnervate = InnervateBox.Checked;
        }

        private void Flourish_CheckedChanged(object sender, EventArgs e)
        {
            UseFlourish = FlourishBox.Checked;
        }

        private void Essence_CheckedChanged(object sender, EventArgs e)
        {
            UseEssence = EssenceBox.Checked;
        }

        private void Efflorescence_CheckedChanged(object sender, EventArgs e)
        {
            UseEfflorescence = EfflorescenceBox.Checked;
        }

        private void Germination_CheckedChanged(object sender, EventArgs e)
        {
            Germination = GerminationBox.Checked;
        }

        private void CenarionWard_CheckedChanged(object sender, EventArgs e)
        {
            CenarionWard = CenarionWardBox.Checked;
        }

        private void DisplayOffSpec_CheckedChanged(object sender, EventArgs e)
        {
            DisplayDPS = DisplayOffSpecBox.Checked;
        }

        private void DisplayCD_CheckedChanged(object sender, EventArgs e)
        {
            DisplayCD = DisplayCDBox.Checked;
        }

        private void DisplayTank_CheckedChanged(object sender, EventArgs e)
        {
            DisplayTank = DisplayTankHealingBox.Checked;
        }

        private void Tier4_CheckedChanged(object sender, EventArgs e)
        {
            UseTier4 = Tier4Box.Checked;
        }

        private void ProwlOOC_CheckedChanged(object sender, EventArgs e)
        {
            ProwlOOC = ProwlOOCBox.Checked;
        }

        private void CoolDown_CheckedChanged(object sender, EventArgs e)
        {
            UseCoolDowns = CoolDownBox.Checked;
        }

        private void DPSMode_CheckedChanged(object sender, EventArgs e)
        {
            UseDPSMode = DPSModeBox.Checked;
        }

        private void TankHealing_CheckedChanged(object sender, EventArgs e)
        {
            UseTankHealing = TankHealingBox.Checked;
        }

        private void AreKeybindsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            AreKeybindsEnabled = AreKeybindsEnabledBox.Checked;
        }

        private void CheckIfAddonEdited_CheckedChanged(object sender, EventArgs e)
        {
            ForceAddonReload = CheckIfAddonEditedBox.Checked;
        }

        private void LoadExternalAddon_CheckedChanged(object sender, EventArgs e)
        {
            LoadExternalAddon = LoadExternalAddonBox.Checked;
        }

        private void InitializeWithSends_CheckedChanged(object sender, EventArgs e)
        {
            InitializeWithSends = InitializeWithSendsBox.Checked;
        }

        // Pulse method
        public override void Pulse()
        {
            AsyncPulse();
        }

        private void AsyncPulse()
        {
            Log.Write(DPSModeKey);
            // Editing the addon
            if (AddonEdited == false) {
                Log.Write("Editing Addon");
                AddonEdit();
            }

            // Recognising each party member's role
            if (!Initialized) {
                Log.Write("Identifying party members");
                InitializeResto();
            }

            // Does actions required in first run

            if (IsFirstRun) {
                DoFirstRun();
            }

            if (AreKeybindsEnabled) {
                Keypress(); // Task.Factory.StartNew(() => { });
            }

            if (!WoW.IsInCombat) {
                DoOOC();
            }

            // Does housekeeping and varous stuff

            DoVarious();

            // Main Rotation

            if (CoolDowns || DPSMode || TankHealing) {
                if (CoolDowns) {
                    DoCoolDownRotation();
                }
                if (DPSMode) {
                    switch (Affinity) {
                        case "Balance":
                            Moonkin();
                            break;
                        case "Feral":
                            Cat();
                            break;
                        case "Guardian":
                            Bear();
                            break;
                    }
                }
                if (TankHealing) {
                    HealTank();
                }
            }
            else {
                Log.Write(Convert.ToString(WoW.CurrentComboPoints));
                switch (HealingIntensity) {
                    case "Normal":
                        NormalHeal();
                        break;
                    case "High":
                        HighHeal();
                        break;
                }
            }


            // Delaying Pulse if no incoming damage
            Thread.Sleep(PulseSleep);
        }

        private void NormalHeal()
        {
            if (WoW.PlayerHasBuff("Innervate"))
            {
                CoolDowns = true;
                CoolDownsTriggeredByInnervate = true;
                return;
            }

            if (WoW.CanCast("Tier4") && WoW.TargetIsCasting && UseTier4 && (Tier4Timer.ElapsedMilliseconds > 50000 || !Tier4Timer.IsRunning))
            {
                WoW.CastSpell("Tier4");
                Tier4Timer.Reset();
                Tier4Timer.Start();
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (PartyLowestHealth > 20 || PartyLowestHealthMember == Tank)
            {
                DoTankChecks();
            }

            if (!WoW.PlayerHasBuff("Rejuvenation") && WoW.CanCast("Rejuvenation") && WoW.HealthPercent < 100 - ManaModifier)
            {
                CastSpellOnParty("Rejuvenation", Self);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Tranquility") && ((PartyHealth < TranquilityHP && !IsDeadParty) || (PartyHealth < TranquilityHP + 5 && IsDeadParty)) && UseTranquility && !SaveTranquility)
            {
                WoW.CastSpell("Tranquility");
                return;
            }

            if (WoW.CanCast("Incarnation: Tree of Life") && PartyHealth < IncarnateHP && UseIncarnate && !SaveIncarnate)
            {
                WoW.CastSpell("Incarnation: Tree of Life");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Regrowth") && PartyLowestHealth < 95 && WoW.PlayerHasBuff("Power of the Archdruid") && PartyLowestHealthMemberInRange && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Regrowth") && PartyLowestHealth < 95 && WoW.PlayerHasBuff("Clearcasting") && PartyLowestHealthMemberInRange && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Renewal") && WoW.HealthPercent < RenewalHP && UseRenewal && !SaveRenewal)
            {
                WoW.CastSpell("Renewal");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Innervate") &&
                ((PartyHealth < InnervateHP && !IsDeadParty) || (PartyHealth < InnervateHP + 5 && IsDeadParty) ||
                 (PartyHealth < InnervateHP + 15 && WoW.PlayerHasBuff("Incarnation: Tree of Life"))) && UseInnervate && !SaveInnervate)
            {
                WoW.CastSpell("Innervate");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Wild Growth") &&
                ((PartyHealth < WildGrowthHP && !IsDeadParty) || (PartyHealth < WildGrowthHP + 5 && IsDeadParty) || (WoW.PlayerHasBuff("Soul of the Forest") && PartyHealth < WildGrowthHP + 15) ||
                 (WoW.PlayerHasBuff("Incarnation: Tree of Life") && PartyHealth < WildGrowthHP + 15)) && !WoW.IsMoving)
            {
                WoW.CastSpell("Wild Growth");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Flourish") && ((PartyTotalHOTs > 7 && !Germination) || (PartyTotalHOTs > 8 && Germination)) &&
                ((PartyHealth < FlourishHP && !IsDeadParty) || (PartyHealth < FlourishHP + 10 && IsDeadParty)) && UseFlourish && !SaveFlourish)
            {
                WoW.CastSpell("Flourish");
                Thread.Sleep((int) (GCD*1000));
            }

            if (WoW.CanCast("Essence of G'Hanir") && ((PartyHealth < EssenceHP && !IsDeadParty) || (PartyHealth < EssenceHP + 5 && IsDeadParty)) &&
                ((PartyTotalHOTs > 7 && !Germination) || (PartyTotalHOTs > 8 && Germination)))
            {
                WoW.CastSpell("Essence of G'Hanir");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Efflorescence") &&
                ((PartyHealth < EfflorescenceHP - ManaModifier && !IsDeadParty) || (PartyHealth < EfflorescenceHP + 5 - ManaModifier && IsDeadParty) ||
                 (PartyHealth < EfflorescenceHP + 5 - ManaModifier && SpringBlossoms)) && UseEfflorescence && EfflorescenceTimer.ElapsedMilliseconds > 25000)
            {
                WoW.CastSpell("Efflorescence");
                EfflorescenceTimer.Reset();
                EfflorescenceTimer.Start();
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Regrowth") &&
                (PartyLowestHealth < RegrowthHP - ManaModifier || (WoW.PlayerHasBuff("Incarnation: Tree of Life") && PartyLowestHealth < RegrowthHP + 10 - ManaModifier)) &&
                PartyLowestHealthMemberInRange && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Swiftmend") && (PartyLowestHealth < SwiftmendHP || (PartyLowestHealth < SwiftmendHP + 10 && SoulOfTheForest)) && PartyLowestHealthMemberInRange)
            {
                CastSpellOnParty("Swiftmend", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Ironbark") && HealthPercentParty(Tank) < IronbarkHP && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Ironbark", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Swiftmend") && (HealthPercentParty(Tank) < SwiftmendHP + 20 || (TankHPROC1sAny && HealthPercentParty(Tank) < SwiftmendHP + 30)) && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Swiftmend", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Healing Touch") && (HealthPercentParty(Dps1) < HealingTouchHP + ManaModifier || (Dps1HPROC1sAny && HealthPercentParty(Dps1) < HealingTouchHP + ManaModifier + 10)) &&
                IsInHealRangeParty(Dps1) && !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Dps1);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (WoW.CanCast("Healing Touch") && (HealthPercentParty(Dps2) < HealingTouchHP + ManaModifier || (Dps2HPROC1sAny && HealthPercentParty(Dps2) < HealingTouchHP + ManaModifier + 10)) &&
                IsInHealRangeParty(Dps2) && !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Dps2);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (WoW.CanCast("Healing Touch") && (HealthPercentParty(Dps3) < HealingTouchHP + ManaModifier || (Dps3HPROC1sAny && HealthPercentParty(Dps3) < HealingTouchHP + ManaModifier + 10)) &&
                IsInHealRangeParty(Dps3) && !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Dps3);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (WoW.CanCast("Healing Touch") &&
                (HealthPercentParty(Tank) < HealingTouchHP + ManaModifier + 10 || (TankHPROC1sAny && HealthPercentParty(Tank) < HealingTouchHP + ManaModifier + 20)) && IsInHealRangeParty(Tank) &&
                !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Tank);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (!PartyHasBuff("Regrowth", Tank) && WoW.CanCast("Regrowth") &&
                (HealthPercentParty(Tank) < RegrowthHP - ManaModifier + 10 || (TankHPROC1sAny && HealthPercentParty(Tank) < RegrowthHP - ManaModifier + 20)) && IsInHealRangeParty(Tank) &&
                !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Tank) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Tank) < RejuvenationHP + ManaModifier &&
                IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Rejuvenation", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps1) && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps1) < RejuvenationHP + ManaModifier && IsInHealRangeParty(Dps1))
            {
                CastSpellOnParty("Rejuvenation", Dps1);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps2) && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps2) < RejuvenationHP + ManaModifier && IsInHealRangeParty(Dps2))
            {
                CastSpellOnParty("Rejuvenation", Dps2);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps3) && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps3) < RejuvenationHP + ManaModifier && IsInHealRangeParty(Dps3))
            {
                CastSpellOnParty("Rejuvenation", Dps3);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps1) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps1) < GerminationHP + ManaModifier &&
                IsInHealRangeParty(Dps1))
            {
                CastSpellOnParty("Rejuvenation", Dps1);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps2) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps2) < GerminationHP + ManaModifier &&
                IsInHealRangeParty(Dps2))
            {
                CastSpellOnParty("Rejuvenation", Dps2);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps3) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps3) < GerminationHP + ManaModifier &&
                IsInHealRangeParty(Dps3))
            {
                CastSpellOnParty("Rejuvenation", Dps3);
                Thread.Sleep((int) (GCD*1000));
            }
        }

        private void HighHeal()
        {
            if (HealingIntensityTimer.ElapsedMilliseconds > 30000)
            {
                HealingIntensity = "Normal";
                return;
            }

            if (PartyLowestHealth > 20 || PartyLowestHealthMember == Tank)
            {
                DoTankChecks();
            }

            if (!WoW.PlayerHasBuff("Rejuvenation") && WoW.CanCast("Rejuvenation"))
            {
                CastSpellOnParty("Rejuvenation", Self);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Renewal") && WoW.HealthPercent < 50 && UseRenewal && !SaveRenewal)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Tranquility") && ((PartyHealth < 80 && !IsDeadParty) || (PartyHealth < 80 && IsDeadParty)) && UseTranquility && !SaveTranquility)
            {
                WoW.CastSpell("Tranquility");
                return;
            }

            if (WoW.CanCast("Incarnation: Tree of Life") && PartyHealth < 70 && UseIncarnate && !SaveIncarnate)
            {
                WoW.CastSpell("Incarnation: Tree of Life");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Flourish") && ((PartyTotalHOTs > 4 && !Germination) || (PartyTotalHOTs > 6 && Germination)) &&
                ((PartyHealth < 70 && !IsDeadParty) || (PartyHealth < 65 && IsDeadParty)) && UseFlourish && !SaveFlourish)
            {
                WoW.CastSpell("Flourish");
                Thread.Sleep((int) (GCD*1000));
            }

            if (WoW.CanCast("Regrowth") && PartyLowestHealth < 95 && WoW.PlayerHasBuff("Power of the Archdruid") && PartyLowestHealthMemberInRange && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Regrowth") && PartyLowestHealth < 95 && WoW.PlayerHasBuff("Clearcasting") && PartyLowestHealthMemberInRange && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Wild Growth") && ((PartyHealth < 70 && !IsDeadParty) || (PartyHealth < 60 && IsDeadParty)) && !WoW.IsMoving)
            {
                WoW.CastSpell("Wild Growth");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Essence of G'Hanir") && ((PartyHealth < 70 && !IsDeadParty) || (PartyHealth < 60 && IsDeadParty)) &&
                ((PartyTotalHOTs > 4 && !Germination) || (PartyTotalHOTs > 6 && Germination)))
            {
                WoW.CastSpell("Essence of G'Hanir");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Efflorescence") && ((PartyHealth < 75 && !IsDeadParty) || (PartyHealth < 80 && IsDeadParty) || (PartyHealth < 80 && SpringBlossoms)) && UseEfflorescence &&
                EfflorescenceTimer.ElapsedMilliseconds > 25000)
            {
                WoW.CastSpell("Efflorescence");
                EfflorescenceTimer.Reset();
                EfflorescenceTimer.Start();
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Regrowth") && PartyLowestHealth < 50 && PartyLowestHealthMemberInRange && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Swiftmend") && HealthPercentParty(Tank) < 60 && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Swiftmend", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Swiftmend") && PartyLowestHealth < 40 && PartyLowestHealthMemberInRange)
            {
                CastSpellOnParty("Swiftmend", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Ironbark") && HealthPercentParty(Tank) < 40 && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Ironbark", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Healing Touch") && HealthPercentParty(Dps1) < 40 && IsInHealRangeParty(Dps1) && !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Dps1);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (WoW.CanCast("Healing Touch") && HealthPercentParty(Dps2) < 40 && IsInHealRangeParty(Dps2) && !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Dps2);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (WoW.CanCast("Healing Touch") && HealthPercentParty(Dps3) < 40 && IsInHealRangeParty(Dps3) && !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Dps3);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (WoW.CanCast("Healing Touch") && HealthPercentParty(Tank) < 50 && IsInHealRangeParty(Tank) && !WoW.IsMoving)
            {
                CastSpellOnParty("Healing Touch", Tank);
                Thread.Sleep(HealingTouchSleep);
                return;
            }

            if (!PartyHasBuff("Regrowth", Tank) && WoW.CanCast("Regrowth") && HealthPercentParty(Tank) < 60 && IsInHealRangeParty(Tank) && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Tank) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Tank) < 80 && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Rejuvenation", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps1) && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps1) < 80 && IsInHealRangeParty(Dps1))
            {
                CastSpellOnParty("Rejuvenation", Dps1);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps2) && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps2) < 80 && IsInHealRangeParty(Dps2))
            {
                CastSpellOnParty("Rejuvenation", Dps2);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps3) && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps3) < 80 && IsInHealRangeParty(Dps3))
            {
                CastSpellOnParty("Rejuvenation", Dps3);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps1) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps1) < 60 && IsInHealRangeParty(Dps1))
            {
                CastSpellOnParty("Rejuvenation", Dps1);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps2) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps2) < 60 && IsInHealRangeParty(Dps2))
            {
                CastSpellOnParty("Rejuvenation", Dps2);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps3) && Germination && WoW.CanCast("Rejuvenation") && HealthPercentParty(Dps3) < 60 && IsInHealRangeParty(Dps3))
            {
                CastSpellOnParty("Rejuvenation", Dps3);
                Thread.Sleep((int) (GCD*1000));
            }
        }

        private void DoCoolDownRotation()
        {
            if (!WoW.PlayerHasBuff("Innervate") && CoolDownsTriggeredByInnervate)
            {
                CoolDowns = false;
                CoolDownsTriggeredByInnervate = false;
                return;
            }

            if (HealthPercentParty(Tank) == 0 && WoW.CanCast("Rebirth") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Rebirth", Tank);
                return;
            }

            if (WoW.CanCast("Tranquility") && UseTranquility && !CoolDownsTriggeredByInnervate)
            {
                WoW.CastSpell("Tranquility");
                return;
            }

            if (WoW.CanCast("Innervate") && UseInnervate)
            {
                WoW.CastSpell("Innervate");
                return;
            }

            if (WoW.CanCast("Wild Growth") && !WoW.IsMoving)
            {
                WoW.CastSpell("Wild Growth");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Flourish") && UseFlourish && !CoolDownsTriggeredByInnervate)
            {
                WoW.CastSpell("Flourish");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Essence of G'Hanir") && UseEssence && !CoolDownsTriggeredByInnervate)
            {
                WoW.CastSpell("Essence of G'Hanir");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Cenarion Ward", Tank) && WoW.CanCast("Cenarion Ward") && IsInHealRangeParty(Tank) && !CoolDownsTriggeredByInnervate)
            {
                CastSpellOnParty("Cenarion Ward", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Lifebloom", Tank) && WoW.CanCast("Lifebloom") && IsInHealRangeParty(Tank) && !CoolDownsTriggeredByInnervate)
            {
                CastSpellOnParty("Lifebloom", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Renewal") && UseRenewal && !CoolDownsTriggeredByInnervate)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (PartyLowestHealthMemberInRange && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", PartyLowestHealthMember);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Regrowth", Tank) && IsInHealRangeParty(Tank) && !WoW.IsMoving)
            {
                CastSpellOnParty("Regrowth", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Efflorescence") && UseEfflorescence)
            {
                WoW.CastSpell("Wild Growth");
                EfflorescenceTimer.Reset();
                EfflorescenceTimer.Start();
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!WoW.PlayerHasBuff("Regrowth") && !WoW.IsMoving && WoW.HealthPercent < 90)
            {
                WoW.CastSpell("Regrowth");
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Regrowth", Dps1) && IsInHealRangeParty(Dps1) && !WoW.IsMoving && HealthPercentParty(Dps1) < 90)
            {
                CastSpellOnParty("Regrowth", Dps1);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Regrowth", Dps2) && IsInHealRangeParty(Dps2) && !WoW.IsMoving && HealthPercentParty(Dps2) < 90)
            {
                CastSpellOnParty("Regrowth", Dps2);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Regrowth", Dps3) && IsInHealRangeParty(Dps3) && !WoW.IsMoving && HealthPercentParty(Dps3) < 90)
            {
                CastSpellOnParty("Regrowth", Dps3);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Ironbark"))
            {
                CastSpellOnParty("Ironbark", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Tank) && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Rejuvenation", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps1) && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Dps1))
            {
                CastSpellOnParty("Rejuvenation", Dps1);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps2) && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Dps2))
            {
                CastSpellOnParty("Rejuvenation", Dps2);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Dps3) && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Dps3))
            {
                CastSpellOnParty("Rejuvenation", Dps3);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps1) && Germination && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Dps1))
            {
                CastSpellOnParty("Rejuvenation", Dps1);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps2) && Germination && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Dps2))
            {
                CastSpellOnParty("Rejuvenation", Dps2);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation (Germination)", Dps3) && Germination && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Dps3))
            {
                CastSpellOnParty("Rejuvenation", Dps3);
                Thread.Sleep((int) (GCD*1000));
            }
        }

        private void DoTankChecks()
        {
            if (HealthPercentParty(Tank) == 0 && WoW.CanCast("Rebirth") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Rebirth", Tank);
                return;
            }

            if (!PartyHasBuff("Cenarion Ward", Tank) && WoW.CanCast("Cenarion Ward") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Cenarion Ward", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Lifebloom", Tank) && WoW.CanCast("Lifebloom") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Lifebloom", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Rejuvenation", Tank) && WoW.CanCast("Rejuvenation") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Rejuvenation", Tank);
                Thread.Sleep((int) (GCD*1000));
            }
        }

        private void HealTank()
        {
            if (HealthPercentParty(Tank) == 0 && WoW.CanCast("Rebirth") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Rebirth", Tank);
                return;
            }

            if (!PartyHasBuff("Cenarion Ward", Tank) && WoW.CanCast("Cenarion Ward") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Cenarion Ward", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (!PartyHasBuff("Lifebloom", Tank) && WoW.CanCast("Lifebloom") && IsInHealRangeParty(Tank))
            {
                CastSpellOnParty("Lifebloom", Tank);
                Thread.Sleep((int) (GCD*1000));
                return;
            }

            if (WoW.CanCast("Regrowth") && HealthPercentParty(Tank) < 95 && WoW.PlayerHasBuff("Clearcasting") && PartyLowestHealthMemberInRange && !WoW.IsMoving)
                CastSpellOnParty("Regrowth", Tank);
            {
                Thread.Sleep((int) (GCD*1000));
            }
        }


        // Balance DPS rotation
        private void Moonkin()
        {
            if (WoW.IsInCombat && WoW.TargetIsEnemy)
            {
                // Get into Moonkin form
                if (WoW.CanCast("Moonkin Form") && !WoW.PlayerHasBuff("Moonkin Form"))
                {
                    WoW.CastSpell("Moonkin Form");
                    return;
                }
                // Starsurge
                if (WoW.IsSpellInRange("Starsurge") && WoW.CanCast("Starsurge"))
                {
                    WoW.CastSpell("Starsurge");
                    return;
                }
                // Moonfire always up
                if (WoW.IsSpellInRange("Moonfire") && WoW.CanCast("Moonfire") &&
                    (!WoW.TargetHasDebuff("Moonfire") ||
                     (WoW.IsSpellOnCooldown("Starsurge") && combatRoutine.Type == RotationType.SingleTarget && WoW.TargetHasDebuff("Sunfire") && WoW.PlayerBuffStacks("Solar Empowerement") < 3 &&
                      WoW.PlayerBuffStacks("Lunar Empowerement") < 3)))
                {
                    WoW.CastSpell("Moonfire");
                    return;
                }
                // Sunfire always up
                if (WoW.IsSpellInRange("Sunfire") && WoW.CanCast("Sunfire") &&
                    (!WoW.TargetHasDebuff("Sunfire") ||
                     (WoW.IsSpellOnCooldown("Starsurge") && combatRoutine.Type == RotationType.AOE && WoW.TargetHasDebuff("Sunfire") && WoW.PlayerBuffStacks("Solar Empowerement") < 3 &&
                      WoW.PlayerBuffStacks("Lunar Empowerement") < 3)))
                {
                    WoW.CastSpell("Sunfire");
                    return;
                }
                // Solar Wrath when we have 3 stacks of Solar Empowerement
                if (WoW.IsSpellInRange("Solar Wrath") && WoW.CanCast("Solar Wrath") && WoW.PlayerBuffStacks("Solar Empowerement") == 3)
                {
                    WoW.CastSpell("Solar Wrath");
                    return;
                }
                // Lunar Strike when we have 3 stacks of Lunar Empowerement
                if (WoW.IsSpellInRange("Lunar Strike") && WoW.CanCast("Lunar Strike") && WoW.PlayerBuffStacks("Lunar Empowerement") == 3)
                {
                    WoW.CastSpell("Lunar Strike");
                }
            }
            else if (WoW.TargetIsFriend)
            {
                HighHeal();
            }
        }

        // Feral DPS rotation
        private void Cat()
        {
            if (WoW.IsInCombat && WoW.TargetIsEnemy)
            {
                // Switch to cat form

                if (WoW.CanCast("Cat Form") && !WoW.PlayerHasBuff("Cat Form"))
                {
                    WoW.CastSpell("Cat Form");
                    Log.Write("Switching to cat", Color.Red);
                    Thread.Sleep(50);
                }
                // Use Rake if debuff is not there or is about to fall off

                if (WoW.CurrentComboPoints < 5 && (!WoW.TargetHasDebuff("Rake") || (WoW.TargetHasDebuff("Rake") && WoW.TargetDebuffTimeRemaining("Rake") < 3)))
                {
                    WoW.CastSpell("Rake");
                    Thread.Sleep(50);
                    return;
                }

                // Otherwise, use shred to build up combo points

                if (WoW.CurrentComboPoints < 5)
                {
                    WoW.CastSpell("Shred");
                    Thread.Sleep(50);
                    return;
                }

                // If we have 5 combo points and target is below 25% so that we can refresh our Rip
                // cast Ferocious Bite

                if (WoW.CurrentComboPoints == 5 && ((WoW.TargetHasDebuff("Rip") && WoW.TargetDebuffTimeRemaining("Rip") > 8) || (WoW.TargetHealthPercent < 25)))
                {
                    WoW.CastSpell("Ferocious Bite");
                    Thread.Sleep(50);
                    return;
                }

                // Otherwise, cast Rip as a finisher

                if (WoW.CurrentComboPoints == 5)
                {
                    WoW.CastSpell("Rip");
                    Thread.Sleep(50);
                }
            }
            else if (WoW.TargetIsFriend)
            {
                HighHeal();
            }
        }


        // Guardian DPS rotation
        private void Bear()
        {
            if (WoW.IsInCombat && WoW.TargetIsEnemy)
            {
                // Keep Bear Form up

                if (!WoW.PlayerHasBuff("Bear Form"))
                {
                    WoW.CastSpell("Bear Form");
                    Thread.Sleep(50);
                }

                if (WoW.CanCast("Growl") && !WoW.TargetHasDebuff("Intimidated"))
                {
                    WoW.CastSpell("Ironfur");
                    Thread.Sleep(50);
                    return;
                }

                if (WoW.CanCast("Ironfur") && WoW.Rage >= 45)
                {
                    WoW.CastSpell("Ironfur");
                    Thread.Sleep(50);
                    return;
                }

                if (WoW.CanCast("Frenzied Regeneration") && SelfHPROC1sAny && WoW.HealthPercent <= 80 && WoW.PlayerHasBuff("Bear Form") &&
                    (!FrenziedTimer.IsRunning || FrenziedTimer.ElapsedMilliseconds > 5000))
                {
                    WoW.CastSpell("Frenzied Regeneration");
                    FrenziedTimer.Reset();
                    FrenziedTimer.Start();
                    return;
                }

                if (WoW.CanCast("Barkskin") && WoW.HealthPercent <= 65 && !WoW.PlayerHasBuff("Barkskin"))
                {
                    WoW.CastSpell("Barkskin");
                    Thread.Sleep(50);
                    return;
                }

                // Moonfire always up
                if (WoW.IsSpellInRange("Moonfire") && WoW.CanCast("Moonfire") && !WoW.TargetHasDebuff("Moonfire"))
                {
                    WoW.CastSpell("Moonfire");
                    Thread.Sleep(50);
                    return;
                }

                // Cast Mangle and Thrash as long as we can cast them

                if (WoW.CanCast("Thrash"))
                {
                    WoW.CastSpell("Thrash");
                    Thread.Sleep(50);
                    return;
                }

                if (WoW.CanCast("Mangle"))
                {
                    WoW.CastSpell("Mangle");
                    Thread.Sleep(50);
                }
            }
            else if (WoW.TargetIsFriend)
            {
                HighHeal();
            }
        }


        private void DoOOC()
        {
            // Prowl when out of combat if selected

            if (ProwlOOC && WoW.CanCast("Prowl") && !WoW.PlayerHasBuff("Prowl"))
            {
                WoW.CastSpell("Prowl");
            }
        }

        // Checks for user selections

        private void Keypress()
        {
            while (((DPSMod != 0 && DetectKeyPress.GetKeyState(DPSMod) < 0) || DPSMod == 0) && DetectKeyPress.GetKeyState(DPSKey) < 0) {
                if (UseDPSMode && DPSTimer.ElapsedMilliseconds > 1000) //
                {
                    DPSMode = !DPSMode;
                    Log.Write("DPS Mode " + (DPSMode ? "Activated" : "Deactivated"), Color.Red);
                    Log.Write("D P S Mode " + (DPSMode ? "Activated" : "Deactivated"));
                    Thread.Sleep(50);
                    DPSTimer.Restart();
                    if (DisplayDPS) {
                        DisplayText = "DPS Mode " + (DPSMode ? "Activated" : "Deactivated");
                        OverlayTimer.Restart();
                    }
                }
            }

            while (((CoolMod != 0 && DetectKeyPress.GetKeyState(CoolMod) < 0) || CoolMod == 0) && DetectKeyPress.GetKeyState(CoolKey) < 0) {
                if (UseCoolDowns && CoolDownTimer.ElapsedMilliseconds > 1000) {
                    CoolDowns = !CoolDowns;
                    Log.Write("Cooldown Mode " + (CoolDowns ? "Activated" : "Deactivated"), Color.Red);
                    Log.Write("Cooldown Mode " + (CoolDowns ? "Activated" : "Deactivated"));
                    Thread.Sleep(50);
                    CoolDownTimer.Restart();
                    if (DisplayCD) {
                        DisplayText = "Cooldowns Activated";
                        OverlayTimer.Restart();
                    }
                }
            }

            while (((TankMod != 0 && DetectKeyPress.GetKeyState(TankMod) < 0) || TankMod == 0) && DetectKeyPress.GetKeyState(TankKey) < 0) {
                if (UseTankHealing && TankHealingTimer.ElapsedMilliseconds > 1000) {
                    TankHealing = !TankHealing;
                    Log.Write("Tank Healing " + (CoolDowns ? "Activated" : "Deactivated"), Color.Red);
                    Log.Write("Tank Healing " + (CoolDowns ? "Activated" : "Deactivated"));
                    Thread.Sleep(50);
                    CoolDownTimer.Restart();
                    if (DisplayTank) {
                        DisplayText = "Tank Healing " + (TankHealing ? "Activated" : "Deactivated");
                        OverlayTimer.Restart();
                    }
                }
            }
        }


        private void DoFirstRun()
        {
            FrenziedTimer.Start();
            CoolDownTimer.Start();
            DPSTimer.Start();
            OverlayTimer.Reset();
            OverlayTimer.Start();
            EfflorescenceTimer.Start();
            InitSelfTimer();
            InitTankTimer();
            InitDps1Timer();
            InitDps2Timer();
            InitDps3Timer();
            IsFirstRun = false;
        }

        // Do various tasks

        private void DoVarious()
        {
            // Halting at specific points

            HaltRotation();

            // Calculate party parameters
            CalculateParty();

            // Setting the sleep timer

            SleepTimer();

            // Calculate HPROC spikes and averages for party

            CalculateSelfHPROCLists();
            CalculateTankHPROCLists();
            CalculateDps1HPROCLists();
            CalculateDps2HPROCLists();
            CalculateDps3HPROCLists();
        }

        private void HaltRotation()
        {
            while ((WoW.PlayerHasBuff("Cat Form") && Affinity != "Feral") || (WoW.PlayerHasBuff("Bear Form") && Affinity != "Feral") || WoW.PlayerHasBuff("Travel Form") ||
                   WoW.PlayerHasBuff("Drink") || WoW.PlayerHasBuff(MountName) || WoW.PlayerIsChanneling)
            {
                Thread.Sleep(25);
            }
        }

        // Delaying Pulse() if no incoming damage

        private void SleepTimer()
        {
            if (PartyHealth == 100 && PartyTotalHOTs < 5)
            {
                PulseSleep = 200;
            }
            else
            {
                PulseSleep = 5;
            }
        }

        // Calculate which party member is lowest and party totals (health, HOTs etc)

        private void CalculateParty()
        {
            PartyHealthList.Clear();
            PartyHealthList.Add(WoW.HealthPercent);
            PartyHealthList.Add(HealthPercentParty(Tank));
            PartyHealthList.Add(HealthPercentParty(Dps1));
            PartyHealthList.Add(HealthPercentParty(Dps2));
            PartyHealthList.Add(HealthPercentParty(Dps3));
            PartyLowestHealth = 100;
            for (var i = 0; i < 5; i++) {
                if (PartyHealthList[i] < PartyLowestHealth && PartyHealthList[i] != 0) {
                    PartyLowestHealth = PartyHealthList[i];
                    switch (i) {
                        case 0:
                            PartyLowestHealthMember = Self;
                            PartyLowestHealthMemberInRange = true;
                            break;
                        case 1:
                            PartyLowestHealthMember = Tank;
                            PartyLowestHealthMemberInRange = IsInHealRangeParty(Tank);
                            break;
                        case 2:
                            PartyLowestHealthMember = Dps1;
                            PartyLowestHealthMemberInRange = IsInHealRangeParty(Dps1);
                            break;
                        case 3:
                            PartyLowestHealthMember = Dps2;
                            PartyLowestHealthMemberInRange = IsInHealRangeParty(Dps2);
                            break;
                        case 4:
                            PartyLowestHealthMember = Dps3;
                            PartyLowestHealthMemberInRange = IsInHealRangeParty(Dps3);
                            break;
                    }
                }
            }

            PartyRejuv = Convert.ToInt32(WoW.PlayerHasBuff("Rejuvenation")) + Convert.ToInt32(PartyHasBuff("Rejuvenation", Tank)) + Convert.ToInt32(PartyHasBuff("Rejuvenation", Dps1)) +
                         Convert.ToInt32(PartyHasBuff("Rejuvenation", Dps2)) + Convert.ToInt32(PartyHasBuff("Rejuvenation", Dps3));
            PartyGermination = Convert.ToInt32(WoW.PlayerHasBuff("Rejuvenation (Germination)")) + Convert.ToInt32(PartyHasBuff("Rejuvenation (Germination)", Tank)) +
                               Convert.ToInt32(PartyHasBuff("Rejuvenation (Germination)", Dps1)) + Convert.ToInt32(PartyHasBuff("Rejuvenation (Germination)", Dps2)) +
                               Convert.ToInt32(PartyHasBuff("Rejuvenation (Germination)", Dps3));
            PartyTotalHOTs = PartyRejuv + PartyGermination + Convert.ToInt32(WoW.PlayerHasBuff("Regrowth")) + Convert.ToInt32(PartyHasBuff("Regrowth", Tank)) +
                             Convert.ToInt32(PartyHasBuff("Regrowth", Dps1)) + Convert.ToInt32(PartyHasBuff("Regrowth", Dps2)) + Convert.ToInt32(PartyHasBuff("Regrowth", Dps3)) +
                             Convert.ToInt32(WoW.PlayerHasBuff("Wild Growth")) + Convert.ToInt32(PartyHasBuff("Wild Growth", Tank)) + Convert.ToInt32(PartyHasBuff("Wild Growth", Dps1)) +
                             Convert.ToInt32(PartyHasBuff("Wild Growth", Dps2)) + Convert.ToInt32(PartyHasBuff("Wild Growth", Dps3)) + Convert.ToInt32(PartyHasBuff("Lifebloom", Tank));
            PartyHealth = (WoW.HealthPercent + HealthPercentParty(Tank) + HealthPercentParty(Dps1) + HealthPercentParty(Dps2) + HealthPercentParty(Dps3)) / 5;
            IsDeadParty = WoW.HealthPercent == 0 || HealthPercentParty(Tank) == 0 || HealthPercentParty(Dps1) == 0 || HealthPercentParty(Dps2) == 0 || HealthPercentParty(Dps3) == 0;
        }

        private static void AddonEdit()
        {
            try
            {
                var addonlua = File.ReadAllText("" + WoW.AddonPath + "\\" + AddonName + "\\" + AddonName + ".lua");
                if (HealthPercentParty(1) == 0 && ForceAddonReload)
                {
                    Log.Write("Addon Editing in progress");
                    addonlua = addonlua.Replace("local lastCombat = nil" + Environment.NewLine + 
                        "local alphaColor = 1", "local lastCombat = nil" + Environment.NewLine + 
                        "local alphaColor = 1" + Environment.NewLine + 
                        CustomLua);
                    addonlua = addonlua.Replace("InitializeOne()" + Environment.NewLine + 
                                    "            InitializeTwo()",
                                                "InitializeOne()" + Environment.NewLine + 
                                    "            InitializeTwo()" + Environment.NewLine + 
                                    "            InitializeThree()");
                    File.WriteAllText("" + WoW.AddonPath + "\\" + AddonName + "\\" + AddonName + ".lua", addonlua);
                    WoW.SendMacro("/reload");
                    while (WoW.HealthPercent == 0 || WoW.HastePercent == 0)
                    {
                        Thread.Sleep(25);
                    }
                }
                AddonEdited = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void InitializeResto()
        {
            if (!InitializeWithSends)
            {
                Log.Write("Identifying Tank");
                DoInitializeWithSpec();
            }
            if (InitializeWithSends)
            {
                DoInitializeWithSends();
            }
        }

        private void DoInitializeWithSends()
        {
            while (WoW.HealthPercent == 0)
            {
                Thread.Sleep(25);
            }

            Thread.Sleep(500);
            WoW.SendMacro("/cleartarget");
            Log.Write("Please target the Tank!", Color.Red);
            DisplayText = "Please target the tank";
            Log.Write("Do not press any keys for 2 seconds!", Color.Red);

            while (WoW.HasTarget == false)
            {
                Thread.Sleep(25);
            }

            if (InitializeStep == 0)
            {
                WoW.SendMacro("/cast Lifebloom");
                InitializeStep++;
                Thread.Sleep((int) GCD*1000);
            }

            if (InitializeStep == 1)
            {
                WoW.SendMacro("/tar party1");
                if (WoW.TargetHasBuff("Lifebloom"))
                {
                    Tank = 1;
                }
                else if (!WoW.TargetHasBuff("Lifebloom"))
                {
                    Dps1 = 1;
                }
                InitializeStep++;
            }

            if (InitializeStep == 2)
            {
                WoW.SendMacro("/tar party2");
                if (WoW.TargetHasBuff("Lifebloom"))
                {
                    Tank = 2;
                }
                else if (!WoW.TargetHasBuff("Lifebloom") && Dps1 == -1)
                {
                    Dps1 = 2;
                }
                else if (!WoW.TargetHasBuff("Lifebloom"))
                {
                    Dps2 = 2;
                }
                InitializeStep++;
            }

            if (InitializeStep == 3)
            {
                WoW.SendMacro("/tar party3");
                if (WoW.TargetHasBuff("Lifebloom"))
                {
                    Tank = 3;
                    Dps3 = 4;
                }
                else if (!WoW.TargetHasBuff("Lifebloom") && Dps2 == -1 && Dps3 == -1)
                {
                    Dps2 = 3;
                    Dps3 = 4;
                }
                else if (!WoW.TargetHasBuff("Lifebloom") && Dps3 == -1 && Tank == -1)
                {
                    Dps3 = 3;
                    Tank = 4;
                }
                InitializeStep++;
            }

            if (InitializeStep == 4 && Tank != -1 && Dps1 != -1 && Dps2 != -1 && Dps3 != -1)
            {
                Initialized = true;
                Log.Write("Tank is party " + Convert.ToString(Tank), Color.Green);
                Thread.Sleep(1000);
            }
            else
            {
                Initialized = false;
                InitializeStep = 0;
                Tank = -1;
                Dps1 = -1;
                Dps2 = -1;
                Dps3 = -1;
                Pulse();
            }
        }

        private void DoInitializeWithSpec()
        {
            while (WoW.HealthPercent == 0)
            {
                Thread.Sleep(25);
            }
            WoW.CastSpell("Inspect");
            Thread.Sleep(1000);
            for (var i = 1; i < 5; i++)
            {
                Log.Write(Convert.ToString(i));
                Log.Write(Convert.ToString(IsTankParty(i)));
                if (IsTankParty(i))
                {
                    Tank = i;
                    Log.Write("Tank is party no. " + Convert.ToString(Tank), Color.Red);
                    Initialized = true;
                    break;
                }
            }
            if (Tank == -1)
            {
                Initialized = false;
                InitializeWithSends = true;
                return;
            }
            if (Tank == 1)
            {
                Dps1 = 2;
                Dps2 = 3;
                Dps3 = 4;
                DisplayText = "Tank Identified";
            }
            if (Tank == 2)
            {
                Dps1 = 1;
                Dps2 = 3;
                Dps3 = 4;
                DisplayText = "Tank Identified";
            }
            if (Tank == 3)
            {
                Dps1 = 1;
                Dps2 = 2;
                Dps3 = 3;
                DisplayText = "Tank Identified";
            }
        }

        //
        // HPROC Timers
        //

        // Starts timer to calculate Self HP loss 	

        private void InitSelfTimer()
        {
            SelfTimer = new Timer();
            SelfTimer.Enabled = true;
            SelfTimer.Elapsed += SelfTimer_Tick;
            SelfTimer.Interval = 200;
        }

        // Tick event to calculate rate of change for Self HP

        private void SelfTimer_Tick(object sender, ElapsedEventArgs e)
        {
            Interlocked.Increment(ref CurrentSelfHP);
            if (CurrentSelfHP == 0)
            {
                CurrentSelfHP = WoW.HealthPercent;
            }
            else
            {
                PreviousSelfHP = CurrentSelfHP;
                CurrentSelfHP = WoW.HealthPercent;
                SelfHPRateOfChange = CurrentSelfHP - PreviousSelfHP;
            }
        }

        // Starts timer to calculate Tank HP loss 	

        private void InitTankTimer()
        {
            TankTimer = new Timer();
            TankTimer.Enabled = true;
            TankTimer.Elapsed += TankTimer_Tick;
            TankTimer.Interval = 200;
        }

        // Tick event to calculate rate of change for Tank HP

        private void TankTimer_Tick(object sender, ElapsedEventArgs e)
        {
            Interlocked.Increment(ref CurrentTankHP);
            if (CurrentTankHP == 0)
            {
                CurrentTankHP = WoW.HealthPercent;
            }
            else
            {
                PreviousTankHP = CurrentTankHP;
                CurrentTankHP = WoW.HealthPercent;
                TankHPRateOfChange = CurrentTankHP - PreviousTankHP;
            }
        }

        // Starts timer to calculate Dps1 HP loss 	

        private void InitDps1Timer()
        {
            Dps1Timer = new Timer();
            Dps1Timer.Enabled = true;
            Dps1Timer.Elapsed += Dps1Timer_Tick;
            Dps1Timer.Interval = 200;
        }

        // Tick event to calculate rate of change for Dps1 HP

        private void Dps1Timer_Tick(object sender, ElapsedEventArgs e)
        {
            Interlocked.Increment(ref CurrentDps1HP);
            if (CurrentDps1HP == 0)
            {
                CurrentDps1HP = WoW.HealthPercent;
            }
            else
            {
                PreviousDps1HP = CurrentDps1HP;
                CurrentDps1HP = WoW.HealthPercent;
                Dps1HPRateOfChange = CurrentDps1HP - PreviousDps1HP;
            }
        }

        // Starts timer to calculate Dps2 HP loss 	

        private void InitDps2Timer()
        {
            Dps2Timer = new Timer();
            Dps2Timer.Enabled = true;
            Dps2Timer.Elapsed += Dps2Timer_Tick;
            Dps2Timer.Interval = 200;
        }

        // Tick event to calculate rate of change for Dps2 HP

        private void Dps2Timer_Tick(object sender, ElapsedEventArgs e)
        {
            Interlocked.Increment(ref CurrentDps2HP);
            if (CurrentDps2HP == 0)
            {
                CurrentDps2HP = WoW.HealthPercent;
            }
            else
            {
                PreviousDps2HP = CurrentDps2HP;
                CurrentDps2HP = WoW.HealthPercent;
                Dps2HPRateOfChange = CurrentDps2HP - PreviousDps2HP;
            }
        }

        // Starts timer to calculate Dps3 HP loss 	

        private void InitDps3Timer()
        {
            Dps3Timer = new Timer();
            Dps3Timer.Enabled = true;
            Dps3Timer.Elapsed += Dps3Timer_Tick;
            Dps3Timer.Interval = 200;
        }

        // Tick event to calculate rate of change for Dps3 HP

        private void Dps3Timer_Tick(object sender, ElapsedEventArgs e)
        {
            Interlocked.Increment(ref CurrentDps3HP);
            if (CurrentDps3HP == 0)
            {
                CurrentDps3HP = WoW.HealthPercent;
            }
            else
            {
                PreviousDps3HP = CurrentDps3HP;
                CurrentDps3HP = WoW.HealthPercent;
                Dps3HPRateOfChange = CurrentDps3HP - PreviousDps3HP;
            }
        }

        //
        // HPROC Averages
        //

        // Calculates Self HPROC averages and spikes

        private void CalculateSelfHPROCLists()
        {
            // Populates the variables of the last second
            // Max is the maximum change in HP for the given time period
            // Any is a boolean that indicates whether the unit has taken more damage than the threshold
            // at any point in the last second

            if (SelfQueue1s.Count > 4) {
                SelfQueue1s.Dequeue();
            }
            SelfQueue1s.Enqueue(SelfHPRateOfChange);

            foreach (var a in SelfQueue1s) {
                if (a < -HPROC) {
                    SelfHPROC1sAny = true;
                    break;
                }
                SelfHPROC1sAny = false;
            }

            // Populates the variables of the last 5 seconds
            // simirarly to the previous set

            if (SelfQueue5s.Count > 24) {
                SelfQueue5s.Dequeue();
            }
            SelfQueue5s.Enqueue(SelfHPRateOfChange);

            foreach (var b in SelfQueue5s) {
                if (b < -HPROC) {
                    SelfHPROC5sAny = true;
                    break;
                }
                SelfHPROC5sAny = false;
            }


            // Populates the variables of the last 10 seconds
            // simirarly to the previous set

            if (SelfQueue10s.Count > 49) {
                SelfQueue10s.Dequeue();
            }
            SelfQueue10s.Enqueue(SelfHPRateOfChange);


            foreach (var c in SelfQueue10s) {
                if (c < -HPROC) {
                    SelfHPROC10sAny = true;
                    break;
                }
                SelfHPROC10sAny = false;
            }
        }

        // Calculates Tank HPROC averages and spikes

        private void CalculateTankHPROCLists()
        {
            // Populates the variables of the last second
            // Max is the maximum change in HP for the given time period
            // Any is a boolean that indicates whether the unit has taken more damage than the threshold
            // at any point in the last second

            if (TankQueue1s.Count > 4)
            {
                TankQueue1s.Dequeue();
            }
            TankQueue1s.Enqueue(TankHPRateOfChange);

            foreach (var a in TankQueue1s)
            {
                if (a < -HPROC)
                {
                    TankHPROC1sAny = true;
                    break;
                }
                TankHPROC1sAny = false;
            }

            // Populates the variables of the last 5 seconds
            // simirarly to the previous set

            if (TankQueue5s.Count > 24)
            {
                TankQueue5s.Dequeue();
            }
            TankQueue5s.Enqueue(TankHPRateOfChange);

            foreach (var b in TankQueue5s)
            {
                if (b < -HPROC)
                {
                    TankHPROC5sAny = true;
                    break;
                }
                TankHPROC5sAny = false;
            }


            // Populates the variables of the last 10 seconds
            // simirarly to the previous set

            if (TankQueue10s.Count > 49)
            {
                TankQueue10s.Dequeue();
            }
            TankQueue10s.Enqueue(TankHPRateOfChange);


            foreach (var c in TankQueue10s)
            {
                if (c < -HPROC)
                {
                    TankHPROC10sAny = true;
                    break;
                }
                TankHPROC10sAny = false;
            }
        }

        // Calculates Dps1 HPROC averages and spikes

        private void CalculateDps1HPROCLists()
        {
            // Populates the variables of the last second
            // Max is the maximum change in HP for the given time period
            // Any is a boolean that indicates whether the unit has taken more damage than the threshold
            // at any point in the last second

            if (Dps1Queue1s.Count > 4)
            {
                Dps1Queue1s.Dequeue();
            }
            Dps1Queue1s.Enqueue(Dps1HPRateOfChange);

            foreach (var a in Dps1Queue1s)
            {
                if (a < -HPROC)
                {
                    Dps1HPROC1sAny = true;
                    break;
                }
                Dps1HPROC1sAny = false;
            }

            // Populates the variables of the last 5 seconds
            // simirarly to the previous set

            if (Dps1Queue5s.Count > 24)
            {
                Dps1Queue5s.Dequeue();
            }
            Dps1Queue5s.Enqueue(Dps1HPRateOfChange);

            foreach (var b in Dps1Queue5s)
            {
                if (b < -HPROC)
                {
                    Dps1HPROC5sAny = true;
                    break;
                }
                Dps1HPROC5sAny = false;
            }


            // Populates the variables of the last 10 seconds
            // simirarly to the previous set

            if (Dps1Queue10s.Count > 49)
            {
                Dps1Queue10s.Dequeue();
            }
            Dps1Queue10s.Enqueue(Dps1HPRateOfChange);


            foreach (var c in Dps1Queue10s)
            {
                if (c < -HPROC)
                {
                    Dps1HPROC10sAny = true;
                    break;
                }
                Dps1HPROC10sAny = false;
            }
        }

        // Calculates Dps2 HPROC averages and spikes

        private void CalculateDps2HPROCLists()
        {
            // Populates the variables of the last second
            // Max is the maximum change in HP for the given time period
            // Any is a boolean that indicates whether the unit has taken more damage than the threshold
            // at any point in the last second

            if (Dps2Queue1s.Count > 4) {
                Dps2Queue1s.Dequeue();
            }
            Dps2Queue1s.Enqueue(Dps2HPRateOfChange);

            foreach (var a in Dps2Queue1s) {
                if (a < -HPROC) {
                    Dps2HPROC1sAny = true;
                    break;
                }
                Dps2HPROC1sAny = false;
            }

            // Populates the variables of the last 5 seconds
            // simirarly to the previous set

            if (Dps2Queue5s.Count > 24) {
                Dps2Queue5s.Dequeue();
            }
            Dps2Queue5s.Enqueue(Dps2HPRateOfChange);

            foreach (var b in Dps2Queue5s) {
                if (b < -HPROC) {
                    Dps2HPROC5sAny = true;
                    break;
                }
                Dps2HPROC5sAny = false;
            }


            // Populates the variables of the last 10 seconds
            // simirarly to the previous set

            if (Dps2Queue10s.Count > 49) {
                Dps2Queue10s.Dequeue();
            }
            Dps2Queue10s.Enqueue(Dps2HPRateOfChange);


            foreach (var c in Dps2Queue10s) {
                if (c < -HPROC) {
                    Dps2HPROC10sAny = true;
                    break;
                }
                Dps2HPROC10sAny = false;
            }
        }

        // Calculates Dps3 HPROC averages and spikes

        private void CalculateDps3HPROCLists()
        {
            // Populates the variables of the last second
            // Max is the maximum change in HP for the given time period
            // Any is a boolean that indicates whether the unit has taken more damage than the threshold
            // at any point in the last second

            if (Dps3Queue1s.Count > 4) {
                Dps3Queue1s.Dequeue();
            }
            Dps3Queue1s.Enqueue(Dps3HPRateOfChange);

            foreach (var a in Dps3Queue1s) {
                if (a < -HPROC) {
                    Dps3HPROC1sAny = true;
                    break;
                }
                Dps3HPROC1sAny = false;
            }

            // Populates the variables of the last 5 seconds
            // simirarly to the previous set

            if (Dps3Queue5s.Count > 24) {
                Dps3Queue5s.Dequeue();
            }
            Dps3Queue5s.Enqueue(Dps3HPRateOfChange);

            foreach (var b in Dps3Queue5s) {
                if (b < -HPROC) {
                    Dps3HPROC5sAny = true;
                    break;
                }
                Dps3HPROC5sAny = false;
            }


            // Populates the variables of the last 10 seconds
            // simirarly to the previous set

            if (Dps3Queue10s.Count > 49) {
                Dps3Queue10s.Dequeue();
            }
            Dps3Queue10s.Enqueue(Dps3HPRateOfChange);


            foreach (var c in Dps3Queue10s) {
                if (c < -HPROC) {
                    Dps3HPROC10sAny = true;
                    break;
                }
                Dps3HPROC10sAny = false;
            }
        }

        //
        //	Methods for party related pixel recognition
        //

        private static void CastSpellOnParty(string spell, int id)
        {
            switch (id)
            {
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
                    WoW.KeyDown(WoW.Keys.LMenu);
                    WoW.KeyDown(WoW.Keys.ControlKey);
                    break;
            }
            WoW.CastSpell(spell);

            switch (id)
            {
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
                    WoW.KeyUp(WoW.Keys.LMenu);
                    WoW.KeyUp(WoW.Keys.ControlKey);
                    break;
            }
        }

        private static int HealthPercentParty(int partyID)
        {
            if (partyID == 0)
                return WoW.HealthPercent;
            var c = WoW.GetBlockColor(partyID, 12);
            try
            {
                if (c.R == 0)
                    return 0;
                var health = Convert.ToInt32((double) c.R*100/255);
                return health;
            }
            catch (Exception ex)
            {
                Log.Write($"[Health] Red = {c.R}");
                Log.Write(ex.Message, Color.Red);
                return 100;
            }
        }

        private static bool IsInHealRangeParty(int partyID)
        {
            if (partyID == 0)
                return true;
            var c = WoW.GetBlockColor(partyID, 12);
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

        private static bool IsTankParty(int partyID)
        {
            if (partyID == 0)
                return false;
            var c = WoW.GetBlockColor(partyID, 12);
            try
            {
                return c.B != 255;
            }
            catch (Exception ex)
            {
                Log.Write($"[Health] Blue = {c.B}");
                Log.Write(ex.Message, Color.Red);
                return false;
            }
        }

        private static bool PartyHasBuff(string buffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerHasBuff(buffName);
            Aura aura = null;
            foreach (var t in SpellBook.Auras)
            {
                if (t.AuraName == buffName)
                    aura = t;
            }
            if (aura == null)
            {
                Log.Write($"[Hasbuff] Can't find debuff '{buffName}' in Spell Book");
                return false;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 12 + partyID);
            return (c.R != 255) && (c.G != 255) && (c.B != 255);
        }

        public static bool PartyHasDeBuff(string debuffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerHasDebuff(debuffName);
            Aura aura = null;
            foreach (var t in SpellBook.Auras)
            {
                if (t.AuraName == debuffName)
                    aura = t;
            }
            if (aura == null)
            {
                Log.Write($"[HasDebuff] Can't find debuff '{debuffName}' in Spell Book");
                return false;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 16 + partyID);
            return (c.R != 255) && (c.G != 255) && (c.B != 255);
        }

        public static int GetPartyBuffTimeRemaining(string buffName, int partyID)
        {
            if (partyID == 0)
                return WoW.PlayerBuffTimeRemaining(buffName);
            Aura aura = null;

            foreach (var t in SpellBook.Auras)
            {
                if (t.AuraName == buffName)
                    aura = t;
            }
            if (aura == null)
            {
                Log.Write($"[HasDebuff] Can't find buff '{buffName}' in Spell Book");
                return 0;
            }
            var c = WoW.GetBlockColor(aura.InternalAuraNo, 12 + partyID);

            try
            {
                if (c.R == 0)
                    return 0;
                return Convert.ToInt32((double) c.R*100/255);
            }
            catch (Exception ex)
            {
                Log.Write("Failed to find debuff stacks for color G = " + c.B, Color.Red);
                Log.Write("Error: " + ex.Message, Color.Red);
            }
            return 0;
        }
    }

    public class DisplayInfoFormRDI : Form
    {
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        
        private static Process[] processes32 = Process.GetProcessesByName("Wow");
        private static readonly Process[] processes64 = Process.GetProcessesByName("Wow-64");

        private static readonly IntPtr WindowHandle = p.MainWindowHandle;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private readonly byte alpha = 255;

        // Variables for the form

        private readonly Label DisplayLabel;
        private Timer OverlayDisplayTimer;
        private readonly RECT rect;

        public DisplayInfoFormRDI()
        {
            DisplayLabel = new Label();
            GetWindowRect(WindowHandle, ref rect);
            InitOverlayTimer();
            SuspendLayout();
            // 
            // DisplayInfoForm
            // 
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            Controls.Add(DisplayLabel);
            Location = new Point(rect.Left, rect.Top);
            Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.White;
            TransparencyKey = Color.White;
            Name = "DisplayInfoForm";
            Text = "RDruid Info";
            ResumeLayout(false);
            // 
            // DisplayLabel
            // 
            DisplayLabel.AutoSize = false;
            DisplayLabel.Font = new Font("Microsoft Sans Serif", 48F, FontStyle.Regular, GraphicsUnit.Point, 161);
            DisplayLabel.Location = new Point(570, 100);
            DisplayLabel.Name = "DisplayLabel";
            DisplayLabel.Size = new Size(820, 73);
            DisplayLabel.TabIndex = 1;
            DisplayLabel.Text = Restoration.DisplayText;
            DisplayLabel.TextAlign = ContentAlignment.TopCenter;
            WindowState = FormWindowState.Maximized;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public sealed override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        private static Process[] Processes
        {
            get
            {
                var processes = new Process[0];
                Array.Resize(ref processes, processes32.Length + processes64.Length);
                Array.Copy(processes64, processes, processes64.Length);
                Array.Copy(processes32, 0, processes, processes64.Length, processes32.Length);
                return processes;
            }
            set { processes32 = value; }
        }

        private static Process p
        {
            get
            {
                try
                {
                    return Processes[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("" + ex, "CloudMagic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int User32_GetWindowLong(IntPtr hWnd, GetWindowLong nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int User32_SetWindowLong(IntPtr hWnd, GetWindowLong nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern bool User32_SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte bAlpha, LayeredWindowAttributes dwFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Sets transparency

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            var wl = User32_GetWindowLong(Handle, GetWindowLong.GWL_EXSTYLE);
            User32_SetWindowLong(Handle, GetWindowLong.GWL_EXSTYLE, wl | (int) ExtendedWindowStyles.WS_EX_LAYERED | (int) ExtendedWindowStyles.WS_EX_TRANSPARENT);
            User32_SetLayeredWindowAttributes(Handle, (TransparencyKey.B << 16) + (TransparencyKey.G << 8) + TransparencyKey.R, alpha,
                LayeredWindowAttributes.LWA_COLORKEY | LayeredWindowAttributes.LWA_ALPHA);
        }

        // Attempt to draw nicer fonts		
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            base.OnPaint(e);
        }

        // Update overlay text event

        private void UpdateDisplayLabel(object sender, ElapsedEventArgs e)
        {
            DisplayLabel.SuspendLayout();
            DisplayLabel.Text = Restoration.DisplayText;
            DisplayLabel.ResumeLayout(false);
        }

        // Starts timer to refresh label 	

        private void InitOverlayTimer()
        {
            OverlayDisplayTimer = new Timer();
            OverlayDisplayTimer.Enabled = true;
            OverlayDisplayTimer.Elapsed += UpdateDisplayLabel;
            OverlayDisplayTimer.Interval = 100;
        }

        // Variables for the overlay

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }

        private enum GetWindowLong
        {
            GWL_EXSTYLE = -20
        }

        private enum ExtendedWindowStyles
        {
            WS_EX_TRANSPARENT = 0x20,
            WS_EX_LAYERED = 0x80000
        }

        [Flags]
        private enum LayeredWindowAttributes
        {
            LWA_COLORKEY = 0x1,
            LWA_ALPHA = 0x2
        }
    }
}

/*
[AddonDetails.db]
AddonAuthor=Inhade
AddonName=CloudMagic
WoWVersion=Legion - 70100
[SpellBook.db]
Spell,774,Rejuvenation,NumPad0
Spell,33763,Lifebloom,NumPad1
Spell,5185,Healing Touch,NumPad2
Spell,8936,Regrowth,NumPad3
Spell,48438,Wild Growth,NumPad4
Spell,102351,Cenarion Ward,NumPad5
Spell,208253,Essence of G'Hanir,NumPad6
Spell,145205,Efflorescence,NumPad7
Spell,197721,Flourish,NumPad8
Spell,18562,Swiftmend,NumPad9
Spell,22812,Barkskin,Multiply
Spell,740,Tranquility,D0
Spell,20484,Rebirth,Divide
Spell,29166,Innervate,Decimal
Spell,102342,Ironbark,Minus
Spell,197626,Starsurge,F4
Spell,197628,Lunar Strike,F5
Spell,8921,Moonfire,F6
Spell,5176,Solar Wrath,F7
Spell,93402,Sunfire,F8
Spell,197625,Moonkin Form,F9
Spell,108238,Renewal,F10
Spell,5215,Prowl,F11
Spell,22842,Frenzied Regeneration,PageUp
Spell,210037,Growl,OemQuotes
Spell,33917,Mangle,End
Spell,77758,Thrash,NumPad0
Spell,192081,Ironfur,Insert
Spell,5487,Bear Form,Decimal
Spell,1822,Rake,Oemtilde
Spell,5221,Shred,OemCloseBrackets
Spell,1079,Rip,OemOpenBrackets
Spell,22568,Ferocious Bite,Oemplus
Spell,768,Cat Form,Subtract
Spell,0,Inspect,F12
Spell,33891,Incarnation: Tree of Life,Add
Spell,1,Tier4,PageDown
Aura,33763,Lifebloom
Aura,102351,Cenarion Ward
Aura,774,Rejuvenation
Aura,155777,Rejuvenation (Germination)
Aura,8936,Regrowth
Aura,48438,Wild Growth
Aura,16870,Clearcasting
Aura,29166,Innervate
Aura,114108,Soul of the Forest
Aura,117679,Incarnation: Tree of Life
Aura,5487,Bear Form
Aura,768,Cat Form
Aura,197625,Moonkin Form
Aura,783,Travel Form
Aura,40192,Mount
Aura,189877,Power of the Archdruid
Aura,164545,Solar Empowerement
Aura,164547,Lunar Empowerement
Aura,164812,Moonfire
Aura,164815,Sunfire
Aura,77758,Thrash
Aura,206891,Intimidated
Aura,1079,Rip
Aura,22812,Barkskin
Aura,5215,Prowl
Aura,1822,Rake
Aura,6,Drink
*/
