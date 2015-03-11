using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Documents;
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

    public class BaseSettings
    {
        internal static BaseSettings CurrentSettings = new BaseSettings();


        public int ReservedGarrisonResources { get; set; }
        public int MissionRewardPriorityGarrison { get; set; }
        public int MissionRewardPriorityGold { get; set; }
        public int MissionRewardPriorityXp { get; set; }
        public int MissionRewardPriorityApexis { get; set; }
        public int MissionRewardPriorityItems { get; set; }
        public int MissionRewardPriorityFollowerTokens { get; set; }
        public int MissionRewardPriorityFollowerTraits { get; set; }
        public int MissionRewardPriorityFollowerRetraining { get; set; }
        public int MissionRewardPriorityCharacterItems { get; set; }
        public int MissionRewardPriorityContracts { get; set; }
        public int MissionRewardPriorityRushOrders { get; set; }

        public int MissionRewardSuccessFollowerTokens { get; set; }
        public int MissionRewardSuccessGarrison { get; set; }
        public int MissionRewardSuccessGold { get; set; }
        public int MissionRewardSuccessXp { get; set; }
        public int MissionRewardSuccessApexis { get; set; }
        public int MissionRewardSuccessItems { get; set; }
        public int MissionRewardSuccessFollowerTraits { get; set; }
        public int MissionRewardSuccessFollowerRetraining { get; set; }
        public int MissionRewardSuccessCharacterItems { get; set; }
        public int MissionRewardSuccessContracts { get; set; }
        public int MissionRewardSuccessRushOrders { get; set; }

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


        public bool HBRelog_SkipToNextTask { get; set; }

        public bool ExchangePrimalSpirits { get; set; }
        public string PrimalSpiritItem { get; set; }

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
        

        public bool DEBUG_FAKESTARTWORKORDER { get; set; }

        public BaseSettings()
        {
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

            ReservedGarrisonResources = 0;

            MissionRewardPriorityContracts = 5;
            MissionRewardPriorityGarrison = 5;
            MissionRewardPriorityGold = 4;
            MissionRewardPriorityItems = 3;
            MissionRewardPriorityFollowerTokens = 3;
            MissionRewardPriorityFollowerTraits = 2;
            MissionRewardPriorityFollowerRetraining = 3;
            MissionRewardPriorityCharacterItems = 3;
            MissionRewardPriorityApexis = 2;
            MissionRewardPriorityRushOrders = 2;
            MissionRewardPriorityXp = 0;

            MissionRewardSuccessFollowerTokens = 100;
            MissionRewardSuccessGarrison = 100;
            MissionRewardSuccessGold = 100;
            MissionRewardSuccessXp = 100;
            MissionRewardSuccessApexis = 100;
            MissionRewardSuccessItems = 100;
            MissionRewardSuccessFollowerTraits = 100;
            MissionRewardSuccessFollowerRetraining = 100;
            MissionRewardSuccessCharacterItems = 100;
            MissionRewardSuccessContracts = 100;
            MissionRewardSuccessRushOrders = 100;

            HBRelog_SkipToNextTask = true;

            ExchangePrimalSpirits = false;
            PrimalSpiritItem = string.Empty;
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

            DEBUG_FAKESTARTWORKORDER = false;

            LastCheckedHerbString = "0001-01-01T00:00:00";
            LastCheckedMineString = "0001-01-01T00:00:00";
        }

        private static string settingsPath = Path.Combine(Settings.CharacterSettingsDirectory, "GarrisonBaseSettings.xml");
        public static void LoadSettings()
        {
            //Check for Config file
            if (!File.Exists(settingsPath))
            {
                Herbfunk.GarrisonBase.GarrisonBase.Log("No config file found, now creating a new config from defaults at: " + settingsPath);
                SerializeToXML(CurrentSettings);
            }

            CurrentSettings = DeserializeFromXML(settingsPath);
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
