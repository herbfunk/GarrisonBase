using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.Helpers;

namespace Herbfunk.GarrisonBase
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();
            try
            {
                checkBox_HBRelogSkipTask.Checked = BaseSettings.CurrentSettings.HBRelog_SkipToNextTask;
                checkBox_HBRelogSkipTask.CheckedChanged += checkBox_HBRelogSkipToNextTask_CheckedChanged;

                trackBar_ReservedGarrisonResources.Value = BaseSettings.CurrentSettings.ReservedGarrisonResources;
                label_ReservedGarrisonResources.Text = BaseSettings.CurrentSettings.ReservedGarrisonResources.ToString(CultureInfo.InvariantCulture);
                trackBar_ReservedGarrisonResources.ValueChanged += trackBar_ReservedGarrisonResources_SliderChanged;

                #region Mission Reward Controls
                //
                flowLayoutPanel_MissionRewards.Controls.Clear();

                UserControl_MissionReward missionReward_Items =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityItems,
                    BaseSettings.CurrentSettings.MissionRewardSuccessItems,
                    "Items",
                    Color.OrangeRed)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityItems = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessItems = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Items);

                UserControl_MissionReward missionReward_CharacterTokens =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityCharacterItems,
                    BaseSettings.CurrentSettings.MissionRewardSuccessCharacterItems,
                    "Character Tokens",
                    Color.OrangeRed)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityCharacterItems = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessCharacterItems = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_CharacterTokens);

                UserControl_MissionReward missionReward_Apexis =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityApexis,
                    BaseSettings.CurrentSettings.MissionRewardSuccessApexis,
                    "Apexis Crystals",
                    Color.Brown)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityApexis = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessApexis = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Apexis);

                UserControl_MissionReward missionReward_XP =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityXp,
                    BaseSettings.CurrentSettings.MissionRewardSuccessXp,
                    "Follower XP",
                    Color.Brown)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityXp = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessXp = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_XP);

                UserControl_MissionReward missionReward_Gold =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityGold,
                    BaseSettings.CurrentSettings.MissionRewardSuccessGold,
                    "Gold Currency",
                    Color.Brown)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityGold = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessGold = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Gold);

                UserControl_MissionReward missionReward_Garrison =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityGarrison,
                    BaseSettings.CurrentSettings.MissionRewardSuccessGarrison,
                    "Garrison Currency",
                    Color.Brown)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityGarrison = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessGarrison = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Garrison);

                UserControl_MissionReward missionReward_FollowerToken =
                new UserControl_MissionReward(BaseSettings.CurrentSettings.MissionRewardPriorityFollowerTokens, BaseSettings.CurrentSettings.MissionRewardSuccessFollowerTokens, "Follower Tokens", Color.Blue)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityFollowerTokens = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessFollowerTokens = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerToken);

                UserControl_MissionReward missionReward_FollowerTrait =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityFollowerTraits,
                    BaseSettings.CurrentSettings.MissionRewardSuccessFollowerTraits,
                    "Follower Traits",
                    Color.Blue)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityFollowerTraits = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessFollowerTraits = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerTrait);

                UserControl_MissionReward missionReward_FollowerRetraining =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityFollowerRetraining,
                    BaseSettings.CurrentSettings.MissionRewardSuccessFollowerRetraining,
                    "Follower Retraining",
                    Color.Blue)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityFollowerRetraining = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessFollowerRetraining = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerRetraining);

                UserControl_MissionReward missionReward_FollowerContract =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityContracts,
                    BaseSettings.CurrentSettings.MissionRewardSuccessContracts,
                    "Follower Contracts",
                    Color.Blue)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityContracts = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessContracts = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerContract);

                UserControl_MissionReward missionReward_RushOrders =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.MissionRewardPriorityRushOrders,
                    BaseSettings.CurrentSettings.MissionRewardSuccessRushOrders,
                    "Rush Orders",
                    Color.Green)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.MissionRewardPriorityRushOrders = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.MissionRewardSuccessRushOrders = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_RushOrders);

                #endregion

                //
                checkBox_Behavior_Disenchant.Checked = BaseSettings.CurrentSettings.BehaviorDisenchanting;
                checkBox_Behavior_Disenchant.CheckedChanged += checkBox_Behavior_Disenchant_CheckedChanged;

                checkBox_Behavior_HerbGather.Checked = BaseSettings.CurrentSettings.BehaviorHerbGather;
                checkBox_Behavior_HerbGather.CheckedChanged += checkBox_Behavior_HerbGather_CheckedChanged;

                checkBox_Behavior_MineGather.Checked = BaseSettings.CurrentSettings.BehaviorMineGather;
                checkBox_Behavior_MineGather.CheckedChanged += checkBox_Behavior_MineGather_CheckedChanged;

                checkBox_Behavior_Quests.Checked = BaseSettings.CurrentSettings.BehaviorQuests;
                checkBox_Behavior_Quests.CheckedChanged += checkBox_Behavior_Quests_CheckedChanged;

                checkBox_Behavior_Professions.Checked = BaseSettings.CurrentSettings.BehaviorProfessions;
                checkBox_Behavior_Professions.CheckedChanged += checkBox_Behavior_Professions_CheckedChanged;

                checkBox_Behavior_RepairSell.Checked = BaseSettings.CurrentSettings.BehaviorRepairSell;
                checkBox_Behavior_RepairSell.CheckedChanged += checkBox_Behavior_RepairSell_CheckedChanged;

                checkBox_Behavior_StartMissions.Checked = BaseSettings.CurrentSettings.BehaviorMissionStart;
                checkBox_Behavior_StartMissions.CheckedChanged += checkBox_Behavior_StartMissions_CheckedChanged;

                checkBox_Behavior_WorkOrderPickup.Checked = BaseSettings.CurrentSettings.BehaviorWorkOrderPickup;
                checkBox_Behavior_WorkOrderPickup.CheckedChanged += checkBox_Behavior_WorkOrderPickup_CheckedChanged;

                checkBox_Behavior_WorkOrderStart.Checked = BaseSettings.CurrentSettings.BehaviorWorkOrderStartup;
                checkBox_Behavior_WorkOrderStart.CheckedChanged += checkBox_Behavior_WorkOrderStart_CheckedChanged;

                checkBox_Behavior_Salvaging.Checked = BaseSettings.CurrentSettings.BehaviorSalvaging;
                checkBox_Behavior_Salvaging.CheckedChanged += checkBox_Behavior_Salvaging_CheckedChanged;

                checkBox_Behavior_LootCache.Checked = BaseSettings.CurrentSettings.BehaviorLootCache;
                checkBox_Behavior_LootCache.CheckedChanged += checkBox_Behavior_LootCache_CheckedChanged;

                checkBox_MailAutoGet.Checked = BaseSettings.CurrentSettings.MailAutoGet;
                checkBox_MailAutoGet.CheckedChanged += checkBox_MailAutoGet_CheckedChanged;

                checkBox_MailAutoSend.Checked = BaseSettings.CurrentSettings.MailAutoSend;
                checkBox_MailAutoSend.CheckedChanged += checkBox_MailAutoSend_CheckedChanged;

                //
                checkBox_MailEnchanting.Checked = BaseSettings.CurrentSettings.MailSendEnchanting;
                checkBox_MailEnchanting.CheckedChanged += checkBox_MailEnchanting_CheckedChanged;
                textBox_MailEnchanting.Text = BaseSettings.CurrentSettings.MailSendEnchantingRecipient;
                textBox_MailEnchanting.TextChanged += textBox_MailEnchanting_TextChanged;
                
                checkBox_MailEpic.Checked = BaseSettings.CurrentSettings.MailSendEpic;
                checkBox_MailEpic.CheckedChanged += checkBox_MailEpic_CheckedChanged;
                textBox_MailEpic.Text = BaseSettings.CurrentSettings.MailSendEpicRecipient;
                textBox_MailEpic.TextChanged += textBox_MailEpic_TextChanged;

                checkBox_MailHerbs.Checked = BaseSettings.CurrentSettings.MailSendHerbs;
                checkBox_MailHerbs.CheckedChanged += checkBox_MailHerbs_CheckedChanged;
                textBox_MailHerbs.Text = BaseSettings.CurrentSettings.MailSendHerbsRecipient;
                textBox_MailHerbs.TextChanged += textBox_MailHerbs_TextChanged;

                checkBox_MailOre.Checked = BaseSettings.CurrentSettings.MailSendOre;
                checkBox_MailOre.CheckedChanged += checkBox_MailOre_CheckedChanged;
                textBox_MailOre.Text = BaseSettings.CurrentSettings.MailSendOreRecipient;
                textBox_MailOre.TextChanged += textBox_MailOre_TextChanged;

                checkBox_MailRare.Checked = BaseSettings.CurrentSettings.MailSendRare;
                checkBox_MailRare.CheckedChanged += checkBox_MailRare_CheckedChanged;
                textBox_MailRare.Text = BaseSettings.CurrentSettings.MailSendRareRecipient;
                textBox_MailRare.TextChanged += textBox_MailRare_TextChanged;

                checkBox_MailUncommon.Checked = BaseSettings.CurrentSettings.MailSendUncommon;
                checkBox_MailUncommon.CheckedChanged += checkBox_MailUncommon_CheckedChanged;
                textBox_MailUncommon.Text = BaseSettings.CurrentSettings.MailSendUncommonRecipient;
                textBox_MailUncommon.TextChanged += textBox_MailUncommon_TextChanged;

                listView_MailItems.Items.Clear();
                foreach (var mailSendItem in BaseSettings.CurrentSettings.MailSendItems)
                {
                    string[] entry =
                    {
                        mailSendItem.EntryId.ToString(), mailSendItem.Name, mailSendItem.Recipient,
                        mailSendItem.OnCount.ToString()
                    };
                    ListViewItem lvi = new ListViewItem(entry);
                    listView_MailItems.Items.Add(lvi);
                }

              
                comboBox_PrimalSpiritItems.Items.Clear();
                var primalTraderIndex = -1;
                var index = 0;
                foreach (var item in GarrisonManager.PrimalTraderItems)
                {
                    comboBox_PrimalSpiritItems.Items.Add(item.Name);
                    if (BaseSettings.CurrentSettings.PrimalSpiritItem == item.Name)
                        primalTraderIndex = index;
                    index++;
                }
                comboBox_PrimalSpiritItems.SelectedIndex = primalTraderIndex;
                comboBox_PrimalSpiritItems.SelectedIndexChanged += comboBox_PrimalSpiritItems_SelectedIndexChanged;

                //
                flowLayoutPanel_WorkOrderTypes.Controls.Clear();

                Func<object, string> fRetrieveWorkOrderTypesName = s => Enum.GetName(typeof(WorkOrderType), s);
                bool noWorkOrderTypeFlag = BaseSettings.CurrentSettings.WorkOrderTypes.Equals(WorkOrderType.None);
                foreach (var value in Enum.GetValues(typeof (WorkOrderType)))
                {
                    WorkOrderType enumValue = (WorkOrderType)value;
                    if (enumValue.Equals(WorkOrderType.None) || enumValue.Equals(WorkOrderType.All) || enumValue.Equals(WorkOrderType.DwarvenBunker)) continue;

                    CheckBox newCheckBox = new CheckBox
                    {
                        Name = fRetrieveWorkOrderTypesName(value),
                        Text = fRetrieveWorkOrderTypesName(value),
                        Size = checkBox_Behavior_WorkOrderStart.Size,
                        Font = checkBox_Behavior_WorkOrderStart.Font,
                        Checked = !noWorkOrderTypeFlag && BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(enumValue),
                    };
                    newCheckBox.CheckedChanged += checkBox_WorkOrderType_Checked;

                    flowLayoutPanel_WorkOrderTypes.Controls.Add(newCheckBox);
                }


                //
                flowLayoutPanel_TradePostReagents.Controls.Clear();
                Func<object, string> fRetrieveTradePostReagentTypesName = s => Enum.GetName(typeof(WorkOrder.TradePostReagentTypes), s);
                bool noTradePostReagentFlags = BaseSettings.CurrentSettings.TradePostReagents.Equals(WorkOrder.TradePostReagentTypes.None);
                foreach (var value in Enum.GetValues(typeof(WorkOrder.TradePostReagentTypes)))
                {
                    WorkOrder.TradePostReagentTypes reagentTypes = (WorkOrder.TradePostReagentTypes)value;
                    if (reagentTypes.Equals(WorkOrder.TradePostReagentTypes.None) || reagentTypes.Equals(WorkOrder.TradePostReagentTypes.All)) continue;

                    CheckBox newCheckBox = new CheckBox
                    {
                        Name = fRetrieveTradePostReagentTypesName(value),
                        Text = fRetrieveTradePostReagentTypesName(value),
                        Checked = !noTradePostReagentFlags && BaseSettings.CurrentSettings.TradePostReagents.HasFlag(reagentTypes),
                    };
                    newCheckBox.CheckedChanged += checkBox_TradePostReagent_Checked;

                    flowLayoutPanel_TradePostReagents.Controls.Add(newCheckBox);
                }
                checkBox_Vendor_Junk.Checked = BaseSettings.CurrentSettings.VendorJunkItems;
                checkBox_Vendor_Junk.CheckedChanged +=checkBox_Vendor_Junk_CheckedChanged;
                checkBox_Vendor_Common.Checked = BaseSettings.CurrentSettings.VendorCommonItems;
                checkBox_Vendor_Common.CheckedChanged +=checkBox_Vendor_Common_CheckedChanged;
                checkBox_Vendor_Uncommon.Checked = BaseSettings.CurrentSettings.VendorUncommonItems;
                checkBox_Vendor_Uncommon.CheckedChanged +=checkBox_Vendor_Uncommon_CheckedChanged;
                checkBox_Vendor_Rare.Checked = BaseSettings.CurrentSettings.VendorRareItems;
                checkBox_Vendor_Rare.CheckedChanged +=checkBox_Vendor_Rare_CheckedChanged;

                checkBox_Disenchanting_UncommonItems.Checked = BaseSettings.CurrentSettings.DisenchantingUncommon;
                checkBox_Disenchanting_UncommonItems.CheckedChanged += checkBox_Disenchanting_UncommonItems_CheckedChanged;
                checkBox_Disenchanting_Epic.Checked =BaseSettings.CurrentSettings.DisenchantingEpic;
                checkBox_Disenchanting_Epic.CheckedChanged += checkBox_Disenchanting_Epic_CheckedChanged;
                checkBox_Disenchanting_RareItems.Checked = BaseSettings.CurrentSettings.DisenchantingRare;
                checkBox_Disenchanting_RareItems.CheckedChanged += checkBox_Disenchanting_RareItems_CheckedChanged;

                checkBox_Disenchanting_UncommonSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded;
                checkBox_Disenchanting_UncommonSoulbound.CheckedChanged += checkBox_Disenchanting_UncommonSoulbound_CheckedChanged;
                checkBox_Disenchanting_RareSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingRareSoulbounded;
                checkBox_Disenchanting_RareSoulbound.CheckedChanged += checkBox_Disenchanting_RareSoulbound_CheckedChanged;
                checkBox_Disenchanting_EpicSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded;
                checkBox_Disenchanting_EpicSoulbound.CheckedChanged += checkBox_Disenchanting_EpicSoulbound_CheckedChanged;

                textBox_Disenchanting_UncommonLevel.Text = BaseSettings.CurrentSettings.DisenchantingUncommonItemLevel.ToString();
                textBox_Disenchanting_UncommonLevel.TextChanged+=textBox_Disenchanting_UncommonLevel_TextedChanged;
                textBox_Disenchanting_RareLevel.Text = BaseSettings.CurrentSettings.DisenchantingRareItemLevel.ToString();
                textBox_Disenchanting_RareLevel.TextChanged+=textBox_Disenchanting_RareLevel_TextedChanged;
                textBox_Disenchanting_EpicLevel.Text = BaseSettings.CurrentSettings.DisenchantingEpicItemLevel.ToString();
                textBox_Disenchanting_EpicLevel.TextChanged+=textBox_Disenchanting_EpicLevel_TextedChanged;

                

                checkBox_ExchangePrimalSpirits.Checked = BaseSettings.CurrentSettings.ExchangePrimalSpirits;
                checkBox_ExchangePrimalSpirits.CheckedChanged += checkBox_ExchangePrimalSpirits_CheckedChanged;

                //debug
                checkBox_Debug_FakeStartWorkOrder.Checked = BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER;
                checkBox_Debug_FakeStartWorkOrder.CheckedChanged += checkBox_Debug_FakeStartWorkOrder_CheckedChanged;

                checkBox_Debug_FakeFinishQuest.Checked = BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST;
                checkBox_Debug_FakeFinishQuest.CheckedChanged += checkBox_Debug_FakeFinishQuest_CheckedChanged;
            }
            catch (Exception)
            {

            }
        }

        private void checkBox_WorkOrderType_Checked(object sender, EventArgs e)
        {
            var cbSender = (CheckBox)sender;
            var value = (WorkOrderType)Enum.Parse(typeof(WorkOrderType), cbSender.Name);

            if (BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(value))
                BaseSettings.CurrentSettings.WorkOrderTypes &= ~value;
            else
                BaseSettings.CurrentSettings.WorkOrderTypes |= value;
        }
        private void checkBox_TradePostReagent_Checked(object sender, EventArgs e)
        {
            var cbSender = (CheckBox)sender;
            var value = (WorkOrder.TradePostReagentTypes)Enum.Parse(typeof(WorkOrder.TradePostReagentTypes), cbSender.Name);

            if (BaseSettings.CurrentSettings.TradePostReagents.HasFlag(value))
                BaseSettings.CurrentSettings.TradePostReagents &= ~value;
            else
                BaseSettings.CurrentSettings.TradePostReagents |= value;
        }
        private void checkBox_Disenchanting_UncommonItems_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DisenchantingUncommon = !BaseSettings.CurrentSettings.DisenchantingUncommon;
        }
        private void checkBox_Disenchanting_Epic_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DisenchantingEpic = !BaseSettings.CurrentSettings.DisenchantingEpic;
        }
        private void checkBox_Disenchanting_RareItems_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DisenchantingRare = !BaseSettings.CurrentSettings.DisenchantingRare;
        }
        private void checkBox_Disenchanting_UncommonSoulbound_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded = !BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded;
        }
        private void checkBox_Disenchanting_RareSoulbound_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DisenchantingRareSoulbounded = !BaseSettings.CurrentSettings.DisenchantingRareSoulbounded;
        }
        private void checkBox_Disenchanting_EpicSoulbound_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded = !BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded;
        }
        private void textBox_Disenchanting_UncommonLevel_TextedChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox) sender;
            var value = txtbox.Text.Trim();
            if (string.IsNullOrEmpty(value)) value = "0";
            BaseSettings.CurrentSettings.DisenchantingUncommonItemLevel = value.ToInt32();
        }
        private void textBox_Disenchanting_RareLevel_TextedChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
            var value = txtbox.Text.Trim();
            if (string.IsNullOrEmpty(value)) value = "0";
            BaseSettings.CurrentSettings.DisenchantingRareItemLevel = value.ToInt32();
        }
        private void textBox_Disenchanting_EpicLevel_TextedChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
            var value = txtbox.Text.Trim();
            if (string.IsNullOrEmpty(value)) value = "0";
            BaseSettings.CurrentSettings.DisenchantingEpicItemLevel = value.ToInt32();
        }
        private void checkBox_Vendor_Junk_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.VendorJunkItems = !BaseSettings.CurrentSettings.VendorJunkItems;
        }
        private void checkBox_Vendor_Common_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.VendorCommonItems = !BaseSettings.CurrentSettings.VendorCommonItems;
        }
        private void checkBox_Vendor_Uncommon_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.VendorUncommonItems = !BaseSettings.CurrentSettings.VendorUncommonItems;
        }
        private void checkBox_Vendor_Rare_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.VendorRareItems = !BaseSettings.CurrentSettings.VendorRareItems;
        }
        private void comboBox_PrimalSpiritItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_PrimalSpiritItems.SelectedIndex > -1)
            {
                BaseSettings.CurrentSettings.PrimalSpiritItem = comboBox_PrimalSpiritItems.SelectedItem.ToString();
            }
        }
        private void checkBox_ExchangePrimalSpirits_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.ExchangePrimalSpirits = !BaseSettings.CurrentSettings.ExchangePrimalSpirits;
        }
        private void checkBox_HBRelogSkipToNextTask_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.HBRelog_SkipToNextTask = !BaseSettings.CurrentSettings.HBRelog_SkipToNextTask;
        }
        private void textBox_MailRare_TextChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendRareRecipient = textBox_MailRare.Text;
        }
        private void textBox_MailOre_TextChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendOreRecipient = textBox_MailOre.Text;
        }
        private void textBox_MailHerbs_TextChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendHerbsRecipient = textBox_MailHerbs.Text;
        }
        private void textBox_MailEpic_TextChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendEpicRecipient = textBox_MailEpic.Text;
        }
        private void textBox_MailEnchanting_TextChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendEnchantingRecipient = textBox_MailEnchanting.Text;
        }
        private void textBox_MailUncommon_TextChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendUncommonRecipient = textBox_MailUncommon.Text;
        }
        private void checkBox_MailUncommon_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendUncommon = !BaseSettings.CurrentSettings.MailSendUncommon;
        }
        private void checkBox_MailRare_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendRare = !BaseSettings.CurrentSettings.MailSendRare;
        }
        private void checkBox_MailOre_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendOre = !BaseSettings.CurrentSettings.MailSendOre;
        }
        private void checkBox_MailHerbs_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendHerbs = !BaseSettings.CurrentSettings.MailSendHerbs;
        }
        private void checkBox_MailEpic_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendEpic = !BaseSettings.CurrentSettings.MailSendEpic;
        }
        private void checkBox_MailEnchanting_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailSendEnchanting = !BaseSettings.CurrentSettings.MailSendEnchanting;
        }
        private void checkBox_MailAutoGet_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailAutoGet = !BaseSettings.CurrentSettings.MailAutoGet;
        }
        private void checkBox_MailAutoSend_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.MailAutoSend = !BaseSettings.CurrentSettings.MailAutoSend;
        }
        private void checkBox_Behavior_LootCache_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorLootCache = !BaseSettings.CurrentSettings.BehaviorLootCache;
        }
        private void checkBox_Behavior_Salvaging_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorSalvaging = !BaseSettings.CurrentSettings.BehaviorSalvaging;
        }
        private void checkBox_Behavior_Disenchant_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorDisenchanting = !BaseSettings.CurrentSettings.BehaviorDisenchanting;
        }
        private void checkBox_Behavior_HerbGather_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorHerbGather = !BaseSettings.CurrentSettings.BehaviorHerbGather;
        }
        private void checkBox_Behavior_MineGather_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorMineGather = !BaseSettings.CurrentSettings.BehaviorMineGather;
        }
        private void checkBox_Behavior_Quests_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorQuests = !BaseSettings.CurrentSettings.BehaviorQuests;
        }
        private void checkBox_Behavior_Professions_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorProfessions = !BaseSettings.CurrentSettings.BehaviorProfessions;
        }
        private void checkBox_Behavior_RepairSell_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorRepairSell = !BaseSettings.CurrentSettings.BehaviorRepairSell;
        }
        private void checkBox_Behavior_StartMissions_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorMissionStart = !BaseSettings.CurrentSettings.BehaviorMissionStart;
        }
        private void checkBox_Behavior_WorkOrderPickup_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorWorkOrderPickup = !BaseSettings.CurrentSettings.BehaviorWorkOrderPickup;
        }
        private void checkBox_Behavior_WorkOrderStart_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.BehaviorWorkOrderStartup = !BaseSettings.CurrentSettings.BehaviorWorkOrderStartup;
        }

        private void trackBar_ReservedGarrisonResources_SliderChanged(object sender, EventArgs e)
        {
            TrackBar slider_sender = (TrackBar)sender;
            int Value = (int)slider_sender.Value;
            BaseSettings.CurrentSettings.ReservedGarrisonResources = Value;
            label_ReservedGarrisonResources.Text = Value.ToString();
        }
        private void checkBox_Debug_FakeStartWorkOrder_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER = !BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER;
        }
        private void checkBox_Debug_FakeFinishQuest_CheckedChanged(object sender, EventArgs e)
        {
            BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST = !BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST;
        }
        private void Config_Load(object sender, EventArgs e)
        {

        }

        private void Config_FormClosing(object sender, FormClosingEventArgs e)
        {
            BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            foreach (var b in GarrisonManager.Buildings.Values)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }

            LBDebug.Focus();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            foreach (var b in Coroutines.Behaviors)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(String.Format("{0} {1}", b.Type, b.CheckCriteria())));
            }

            LBDebug.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();


            LBDebug.Controls.Add(
                new UserControlDebugEntry(String.Format("Total completed missions {0} [{1}]",
                    LuaCommands._getNumberCompletedMissions, GarrisonManager.CompletedMissionIds.Count)));
            
            LBDebug.Controls.Add(
                new UserControlDebugEntry(String.Format("Total mission IDs {0}",
                    GarrisonManager.AvailableMissionIds.Count)));

            foreach (var b in GarrisonManager.AvailableMissions)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }

            LBDebug.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var b in GarrisonManager.Followers)
            {
                GarrisonBase.Log("{0}", b.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            if (!TextContainsAllNumbers(textBox.Text))
            {
                Char[] textBoxChars = textBox.Text.ToCharArray();
                List<int> removalList = new List<int>();
                for (int i = textBoxChars.Length - 1; i>0; i--)
                {
                    if (!Char.IsNumber(textBoxChars[i]))
                        removalList.Add(i);
                }
                foreach (var i in removalList)
                {
                   textBox.Text=textBox.Text.Remove(i, 1);
                }
            }
        }

        private bool TextContainsAllNumbers(string text)
        {
            return text.ToCharArray().All(c => Char.IsNumber(c));
        }


        private void button5_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                var Objects = Cache.ObjectCacheManager.ObjectCollection.Values.OrderBy(o => o.SubType).ThenBy(o => o.Distance).ToList();
                foreach (var cWoWObject in Objects)
                {
                    Color foreColor = (cWoWObject is C_WoWUnit) ? Color.Black : Color.GhostWhite;
                    Color backColor = (cWoWObject is C_WoWGameObject) ? Color.DarkSlateGray
                                : (cWoWObject is C_WoWUnit) ? Color.MediumSeaGreen
                                : Color.Gray;
                    string entryString = cWoWObject.ToString();
                    UserControlDebugEntry entry = new UserControlDebugEntry(entryString, foreColor,backColor);
                    LBDebug.Controls.Add(entry);
                }   
            }
            catch (Exception ex)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
            }

            LBDebug.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                foreach (var item in Player.Inventory.BagItems)
                {
                    string entryString = item.ToString();
                    UserControlDebugEntry entry = new UserControlDebugEntry(entryString);
                    LBDebug.Controls.Add(entry);
                }

                foreach (var item in Player.Inventory.BankItems)
                {
                    string entryString = item.ToString();
                    UserControlDebugEntry entry = new UserControlDebugEntry(entryString);
                    LBDebug.Controls.Add(entry);
                }

                foreach (var item in Player.Inventory.BankReagentItems)
                {
                    string entryString = item.ToString();
                    UserControlDebugEntry entry = new UserControlDebugEntry(entryString);
                    LBDebug.Controls.Add(entry);
                }
            }
            catch (Exception ex)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
            }

            LBDebug.Focus();
        }
      



        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView_MailItems.SelectedItems.Count > 0)
            {
                int count = listView_MailItems.SelectedItems.Count;
                var items = new ListViewItem[count];
                listView_MailItems.SelectedItems.CopyTo(items, 0);
                foreach (var listViewItem in items)
                {
                    int entryid = Convert.ToInt32(listViewItem.SubItems[0].Text);
                    MailItem removeItem =
                        BaseSettings.CurrentSettings.MailSendItems.FirstOrDefault(m => m.EntryId == entryid);
                    if (removeItem != null)
                        BaseSettings.CurrentSettings.MailSendItems.Remove(removeItem);
                    
                    listView_MailItems.Items.Remove(listViewItem);
                    
                }
            }
        }

        private void button_MailItem_Add_Click(object sender, EventArgs e)
        {
            string entryId = textBox_MailItem_EntryId.Text;
            if (String.IsNullOrEmpty(entryId)) return;
            if (!GarrisonBase.TextIsAllNumerical(entryId)) return;

            string recipient = textBox_MailItem_Recipient.Text;
            if (String.IsNullOrEmpty(recipient)) return;

            string count = textBox_MailItem_Count.Text;
            if (String.IsNullOrEmpty(count)) return;
            if (!GarrisonBase.TextIsAllNumerical(count)) return;

            string name = "New Entry";
            string txtbox_name = textBox_MailItem_Name.Text;
            if (!String.IsNullOrEmpty(txtbox_name)) name = txtbox_name;

            string[] entry =
                    {
                        entryId, name, recipient, count
                    };
            ListViewItem lvi = new ListViewItem(entry);
            listView_MailItems.Items.Add(lvi);
            BaseSettings.CurrentSettings.MailSendItems.Add(new MailItem(entryId.ToInt32(), "New Entry", recipient,
                count.ToInt32()));
        }

        private void AddNewMailItemFromBags()
        {
            MailAddNewItem newitemform = new MailAddNewItem();
            newitemform.ShowDialog();
            if (newitemform.MailItem != null)
            {
                string[] entry =
                {
                    newitemform.MailItem.EntryId.ToString(), newitemform.MailItem.Name, newitemform.MailItem.Recipient,
                    newitemform.MailItem.OnCount.ToString()
                };
                ListViewItem lvi = new ListViewItem(entry);
                listView_MailItems.Items.Add(lvi);
                BaseSettings.CurrentSettings.MailSendItems.Add(newitemform.MailItem);
            }
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddNewMailItemFromBags();
        }

        private void button_MailItem_AddFromBags_Click(object sender, EventArgs e)
        {
            AddNewMailItemFromBags();
        }

        private void textBox_Numbers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

    }
}
