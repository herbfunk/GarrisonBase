using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.Helpers;

namespace Herbfunk.GarrisonBase
{
    public class MailItem
    {
        public int EntryId { get; set; }
        public string Name { get; set; }
        public string Recipient { get; set; }
        public int OnCount { get; set; }

        public MailItem()
        {
            EntryId = 0;
            Name = String.Empty;
            Recipient = String.Empty;
            OnCount = 0;
        }
        public MailItem(int id, string name, string recipient, int oncount)
        {
            EntryId = id;
            Name = name;
            Recipient = recipient;
            OnCount = oncount;
        }

        public override int GetHashCode()
        {
            return EntryId;
        }
    }

    public class MissionSettings
    {
       // public RewardTypes RewardType { get; set; }
        public int SuccessRate { get; set; }
        public int Priority { get; set; }
        public int MinimumLevel { get; set; }

        public MissionSettings()
        {
           // RewardType= RewardTypes.None;
            SuccessRate = 100;
            Priority = 0;
            MinimumLevel = 90;
        }

        public MissionSettings(RewardTypes type, int success, int priority, int minlevel)
        {
           // RewardType = type;
            SuccessRate = success;
            Priority = priority;
            MinimumLevel = minlevel;
        }
    }

    public class InscriptionMillingSetting
    {
        public bool Ignored { get; set; }
        public int Reserved { get; set; }

        public InscriptionMillingSetting()
        {
            Ignored = true;
            Reserved = 0;
        }
        public InscriptionMillingSetting(bool ignored, int reserved)
        {
            Ignored = ignored;
            Reserved = reserved;
        }
    }

    public class DailyQuestSettings
    {
        public bool Enabled { get; set; }
        public int RewardIndex { get; set; }

        public DailyQuestSettings()
        {
            Enabled = false;
            RewardIndex = -1;
        }

        public DailyQuestSettings(bool enabled, int rewardindex)
        {
            Enabled = enabled;
            RewardIndex = rewardindex;
        }
    }

    public class BaseSettings
    {
        internal static BaseSettings CurrentSettings = new BaseSettings();

        public MissionSettings GarrisonResources { get; set; }
        public MissionSettings Gold { get; set; }
        public MissionSettings FollowerExperience { get; set; }
        public MissionSettings ApexisCrystal { get; set; }
        public MissionSettings Items { get; set; }
        public MissionSettings FollowerTokens { get; set; }
        public MissionSettings FollowerTraits { get; set; }
        public MissionSettings FollowerRetraining { get; set; }
        public MissionSettings FollowerContracts { get; set; }
        public MissionSettings CharacterTokens { get; set; }
        public MissionSettings RushOrders { get; set; }
        public MissionSettings SealOfTemperedFate { get; set; }
        public MissionSettings HonorPoints { get; set; }
        
        public MissionSettings AbrogatorStone { get; set; }
        public MissionSettings ElementalRune { get; set; }
        public MissionSettings SavageBlood { get; set; }
        public MissionSettings PrimalSpirit { get; set; }


        public int ReservedGarrisonResources { get; set; }

        public bool MissionReward_FollowerToken_ArmorSet615 { get; set; }
        public bool MissionReward_FollowerToken_ArmorSet630 { get; set; }
        public bool MissionReward_FollowerToken_ArmorSet645 { get; set; }
        public bool MissionReward_FollowerToken_WeaponSet615 { get; set; }
        public bool MissionReward_FollowerToken_WeaponSet630 { get; set; }
        public bool MissionReward_FollowerToken_WeaponSet645 { get; set; }

        public int MissionReward_CharacterToken_ItemLevel { get; set; }
        

        public bool MailAutoSend { get; set; }
        public bool MailAutoGet { get; set; }

        public bool MailSendUncommon { get; set; }
        public string MailSendUncommonRecipient { get; set; }

        public bool MailSendRare { get; set; }
        public string MailSendRareRecipient { get; set; }

        public bool MailSendEpic { get; set; }
        public string MailSendEpicRecipient { get; set; }

        public bool MailSendHerbs { get; set; }
        public string MailSendHerbsRecipient { get; set; }

        public bool MailSendOre { get; set; }
        public string MailSendOreRecipient { get; set; }

        public bool MailSendEnchanting { get; set; }
        public string MailSendEnchantingRecipient { get; set; }

        public List<MailItem> MailSendItems { get; set; }

        [XmlIgnore]
        internal Dictionary<int, MailItem> DictMailSendItems
        {
            get
            {
                if (Dictmailsenditems.Count != MailSendItems.Count)
                {
                    Dictmailsenditems.Clear();
                    foreach (var item in MailSendItems)
                    {
                        if (!Dictmailsenditems.ContainsKey(item.EntryId))
                            Dictmailsenditems.Add(item.EntryId, item);
                    }
                }

                return Dictmailsenditems;
            }
        }
        [XmlIgnore]
        internal readonly Dictionary<int, MailItem> Dictmailsenditems = new Dictionary<int, MailItem>();

        [XmlIgnore]
        public DateTime LastCheckedMine
        {
            get
            {
                return DateTime.Parse(LastCheckedMineString);
            }
        }
        [XmlIgnore]
        public DateTime LastCheckedHerb
        {
            get
            {
                return DateTime.Parse(LastCheckedHerbString);
            }
        }

        public string LastCheckedHerbString { get; set; }
        public string LastCheckedMineString { get; set; }

        public bool BehaviorMissionStart { get; set; }
        public bool BehaviorMissionComplete { get; set; }
        public bool BehaviorHerbGather { get; set; }
        public bool BehaviorMineGather { get; set; }
        public bool BehaviorWorkOrderPickup { get; set; }
        public bool BehaviorWorkOrderStartup { get; set; }
        public bool BehaviorDisenchanting { get; set; }
        public bool BehaviorProfessions { get; set; }
        public bool BehaviorQuests { get; set; }
        public bool BehaviorRepairSell { get; set; }
        public bool BehaviorSalvaging { get; set; }
        public bool BehaviorLootCache { get; set; }
        public bool LootAnyMobs { get; set; }

        public bool BarnWorkOrderFur { get; set; }
        public bool BarnWorkOrderLeather { get; set; }
        public bool BarnWorkOrderMeat { get; set; }

        public bool HBRelog_SkipToNextTask { get; set; }

        public bool ExchangePrimalSpirits { get; set; }

        public uint PrimalSpiritItemId { get; set; }

        public WorkOrder.TradePostReagentTypes TradePostReagents { get; set; }
        public WorkOrderType WorkOrderTypes { get; set; }

        public bool DisenchantingUncommon { get; set; }
        public bool DisenchantingUncommonSoulbounded { get; set; }
        public int DisenchantingUncommonItemLevel { get; set; }
        public bool DisenchantingRare { get; set; }
        public bool DisenchantingRareSoulbounded { get; set; }
        public int DisenchantingRareItemLevel { get; set; }
        public bool DisenchantingEpic{ get; set; }
        public bool DisenchantingEpicSoulbounded { get; set; }
        public int DisenchantingEpicItemLevel { get; set; }


        public bool VendorJunkItems { get; set; }
        public bool VendorCommonItems { get; set; }
        public bool VendorUncommonItems { get; set; }
        public bool VendorRareItems { get; set; }

        public List<int> ProfessionSpellIds { get; set; }

        public bool MillingEnabled { get; set; }
        public int MillingMinimum { get; set; }
        public InscriptionMillingSetting MillingFrostWeed { get; set; }
        public InscriptionMillingSetting MillingFireWeed { get; set; }
        public InscriptionMillingSetting MillingNagrandArrowbloom { get; set; }
        public InscriptionMillingSetting MillingTaladorOrchid { get; set; }
        public InscriptionMillingSetting MillingGorgrondFlytrap { get; set; }
        public InscriptionMillingSetting MillingStarflower { get; set; }

        public int MinimumBagSlotsFree { get; set; }

        public List<int> FollowerOptionalList = new List<int>();

        public DailyQuestSettings DailyWarMillQuestSettings { get; set; }
        public DailyQuestSettings DailyAlchemyLabQuestSettings { get; set; }

        public bool DisableMasterPlanAddon { get; set; }


        public bool DEBUG_FAKESTARTWORKORDER { get; set; }
        public bool DEBUG_FAKEFINISHQUEST { get; set; }
        public bool DEBUG_IGNOREHEARTHSTONE { get; set; }
        public bool DEBUG_FAKEPICKUPWORKORDER { get; set; }

        public BaseSettings()
        {
            GarrisonResources = new MissionSettings(RewardTypes.Garrison, 100, 6, 90);
            Gold = new MissionSettings(RewardTypes.Gold, 100, 4, 90);
            FollowerExperience = new MissionSettings(RewardTypes.XP, 100, 0, 90);
            ApexisCrystal = new MissionSettings(RewardTypes.ApexisCrystal, 100, 3, 90);
            Items = new MissionSettings(RewardTypes.Items, 100, 1, 90);
            FollowerTokens = new MissionSettings(RewardTypes.FollowerToken, 100, 3, 90);
            FollowerTraits = new MissionSettings(RewardTypes.FollowerTrait, 100, 3, 90);
            FollowerRetraining = new MissionSettings(RewardTypes.RetrainingCertificate, 100, 3, 90);
            FollowerContracts = new MissionSettings(RewardTypes.FollowerContract, 100, 5, 90);
            CharacterTokens = new MissionSettings(RewardTypes.CharacterToken, 100, 2, 90);
            RushOrders = new MissionSettings(RewardTypes.RushOrder, 100, 5, 90);
            SealOfTemperedFate = new MissionSettings(RewardTypes.SealOfTemperedFate, 100, 6, 90);
            HonorPoints = new MissionSettings(RewardTypes.HonorPoints, 100, 4, 90);
            AbrogatorStone = new MissionSettings(RewardTypes.AbrogatorStone, 100, 2, 90);
            ElementalRune = new MissionSettings(RewardTypes.ElementalRune, 100, 2, 90);
            SavageBlood = new MissionSettings(RewardTypes.SavageBlood, 100, 2, 90);
            PrimalSpirit= new MissionSettings(RewardTypes.PrimalSpirit, 100, 2, 90);

            MailAutoSend = false;
            MailAutoGet = false;

            MailSendUncommon = false;
            MailSendUncommonRecipient = String.Empty;
            MailSendRare = false;
            MailSendRareRecipient = String.Empty;
            MailSendEpic = false;
            MailSendEpicRecipient = String.Empty;

            MailSendEnchanting = false;
            MailSendEnchantingRecipient = String.Empty;
            MailSendHerbs = false;
            MailSendHerbsRecipient = String.Empty;
            MailSendOre = false;
            MailSendOreRecipient = String.Empty;

            MailSendItems = new List<MailItem>();

            BehaviorMissionStart = true;
            BehaviorMissionComplete = true;
            BehaviorHerbGather = true;
            BehaviorMineGather = true;
            BehaviorWorkOrderPickup = true;
            BehaviorWorkOrderStartup = true;
            BehaviorDisenchanting = true;
            BehaviorProfessions = true;
            BehaviorQuests = true;
            BehaviorRepairSell = true;
            BehaviorSalvaging = true;
            BehaviorLootCache = true;
            LootAnyMobs = false;

            BarnWorkOrderFur = true;
            BarnWorkOrderLeather = true;
            BarnWorkOrderMeat = true;

            ReservedGarrisonResources = 0;

            MissionReward_FollowerToken_ArmorSet615 = true;
            MissionReward_FollowerToken_ArmorSet630 = true;
            MissionReward_FollowerToken_ArmorSet645 = true;
            MissionReward_FollowerToken_WeaponSet615 = true;
            MissionReward_FollowerToken_WeaponSet630 = true;
            MissionReward_FollowerToken_WeaponSet645 = true;
            MissionReward_CharacterToken_ItemLevel = 519;

            HBRelog_SkipToNextTask = true;

            ExchangePrimalSpirits = false;
            PrimalSpiritItemId = 0;
            TradePostReagents= WorkOrder.TradePostReagentTypes.All;
            WorkOrderTypes = WorkOrderType.All;

            DisenchantingUncommon = true;
            DisenchantingUncommonSoulbounded = false;
            DisenchantingUncommonItemLevel = 615;
            DisenchantingRare = true;
            DisenchantingRareSoulbounded = true;
            DisenchantingRareItemLevel = 600;
            DisenchantingEpic = false;
            DisenchantingEpicSoulbounded = false;
            DisenchantingEpicItemLevel = 600;

            VendorJunkItems = true;
            VendorCommonItems = true;
            VendorUncommonItems = true;
            VendorRareItems = false;

            //var professionSpellIds =new List<int>();
            //PlayerProfessions.ProfessionDailyCooldownSpellIds.Values.ForEach(professionSpellIds.AddRange);
            ProfessionSpellIds = new List<int>();
            MinimumBagSlotsFree = 4;

            MillingEnabled = false;
            MillingMinimum = 50;
            MillingFireWeed = new InscriptionMillingSetting(true, 100);
            MillingFrostWeed = new InscriptionMillingSetting(true, 100);
            MillingGorgrondFlytrap = new InscriptionMillingSetting(true, 100);
            MillingNagrandArrowbloom = new InscriptionMillingSetting(true, 100);
            MillingStarflower = new InscriptionMillingSetting(true, 100);
            MillingTaladorOrchid = new InscriptionMillingSetting(true, 100);

            DailyWarMillQuestSettings=new DailyQuestSettings();
            DailyAlchemyLabQuestSettings=new DailyQuestSettings();
            DisableMasterPlanAddon = true;

            DEBUG_FAKESTARTWORKORDER = false;
            DEBUG_FAKEFINISHQUEST = false;
            DEBUG_IGNOREHEARTHSTONE = false;
            DEBUG_FAKEPICKUPWORKORDER = false;

            LastCheckedHerbString = "0001-01-01T00:00:00";
            LastCheckedMineString = "0001-01-01T00:00:00";
        }

        private static string settingsPath = Path.Combine(Settings.CharacterSettingsDirectory, "GarrisonBaseSettings.xml");
        public static void LoadSettings()
        {
            //Check for Config file
            if (!File.Exists(settingsPath))
            {
                GarrisonBase.Log("No settings file found, now creating a new config from defaults at: " + settingsPath);
                SerializeToXML(CurrentSettings);
            }

            CurrentSettings = DeserializeFromXML(settingsPath);
            GarrisonBase.Log("Settings Loaded!");
        }
        public static void SerializeToXML(BaseSettings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BaseSettings));
            TextWriter textWriter = new StreamWriter(settingsPath);
            serializer.Serialize(textWriter, settings);
            textWriter.Close();
        }
        public static BaseSettings DeserializeFromXML(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(BaseSettings));
            TextReader textReader = new StreamReader(path);
            BaseSettings settings;
            settings = (BaseSettings)deserializer.Deserialize(textReader);
            textReader.Close();
            return settings;
        }
        public static BaseSettings DeserializeFromXML()
        {
            return DeserializeFromXML(settingsPath);
        }

    }
}
