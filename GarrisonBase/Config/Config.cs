using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.Helpers;

namespace Herbfunk.GarrisonBase.Config
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();
            try
            {
                checkBox_HBRelogSkipTask.Checked = BaseSettings.CurrentSettings.HBRelog_SkipToNextTask;
                checkBox_HBRelogSkipTask.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.HBRelog_SkipToNextTask=!BaseSettings.CurrentSettings.HBRelog_SkipToNextTask;

                trackBar_ReservedGarrisonResources.Value = BaseSettings.CurrentSettings.ReservedGarrisonResources;
                //label_ReservedGarrisonResources.Text = BaseSettings.CurrentSettings.ReservedGarrisonResources.ToString(CultureInfo.InvariantCulture);
                textBox_ReservedGarrisonResources.Text = BaseSettings.CurrentSettings.ReservedGarrisonResources.ToString(CultureInfo.InvariantCulture);
                textBox_ReservedGarrisonResources.TextChanged += textBox_GarrisonReservered_TextedChanged;
                trackBar_ReservedGarrisonResources.ValueChanged += trackBar_ReservedGarrisonResources_SliderChanged;

                #region Mission Reward Controls
                //
                flowLayoutPanel_MissionRewards.Controls.Clear();

                

                #region Character Tokens
                UserControl_MissionReward missionReward_CharacterTokens =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.CharacterTokens.Priority,
                            BaseSettings.CurrentSettings.CharacterTokens.SuccessRate,
                            BaseSettings.CurrentSettings.CharacterTokens.MinimumLevel,
                            "Character Tokens",
                            Color.Purple)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.CharacterTokens.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.CharacterTokens.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.CharacterTokens.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_CharacterTokens); 
                #endregion

                #region Gold
                UserControl_MissionReward missionReward_Gold =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.Gold.Priority,
                            BaseSettings.CurrentSettings.Gold.SuccessRate,
                            BaseSettings.CurrentSettings.Gold.MinimumLevel,
                            "Gold Currency",
                            Color.Gold)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.Gold.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.Gold.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.Gold.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Gold); 
                #endregion

                #region Garrison Currency
                UserControl_MissionReward missionReward_Garrison =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.GarrisonResources.Priority,
                            BaseSettings.CurrentSettings.GarrisonResources.SuccessRate,
                            BaseSettings.CurrentSettings.GarrisonResources.MinimumLevel,
                            "Garrison Currency",
                            Color.SandyBrown)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.GarrisonResources.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.GarrisonResources.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.GarrisonResources.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Garrison); 
                #endregion

                #region Honor Points
                UserControl_MissionReward missionReward_HonorPoints =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.HonorPoints.Priority,
                    BaseSettings.CurrentSettings.HonorPoints.SuccessRate,
                    BaseSettings.CurrentSettings.HonorPoints.MinimumLevel,
                    "Honor Points",
                    Color.PaleVioletRed)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.HonorPoints.Priority = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.HonorPoints.SuccessRate = i; },
                    UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.HonorPoints.MinimumLevel = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_HonorPoints);

                #endregion


                #region Follower XP
                UserControl_MissionReward missionReward_XP =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.FollowerExperience.Priority,
                            BaseSettings.CurrentSettings.FollowerExperience.SuccessRate,
                            BaseSettings.CurrentSettings.FollowerExperience.MinimumLevel,
                            "Follower XP",
                            Color.Blue)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.FollowerExperience.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.FollowerExperience.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.FollowerExperience.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_XP);
                #endregion

                #region FollowerToken
                UserControl_MissionReward missionReward_FollowerToken =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.FollowerTokens.Priority,
                            BaseSettings.CurrentSettings.FollowerTokens.SuccessRate,
                            BaseSettings.CurrentSettings.FollowerTokens.MinimumLevel,
                            "Follower Tokens",
                            Color.Blue)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.FollowerTokens.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.FollowerTokens.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.FollowerTokens.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerToken); 
                #endregion

                #region FollowerTrait
                UserControl_MissionReward missionReward_FollowerTrait =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.FollowerTraits.Priority,
                            BaseSettings.CurrentSettings.FollowerTraits.SuccessRate,
                            BaseSettings.CurrentSettings.FollowerTraits.MinimumLevel,
                            "Follower Traits",
                            Color.Blue)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.FollowerTraits.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.FollowerTraits.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.FollowerTraits.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerTrait); 
                #endregion

                #region FollowerRetraining
                UserControl_MissionReward missionReward_FollowerRetraining =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.FollowerRetraining.Priority,
                            BaseSettings.CurrentSettings.FollowerRetraining.SuccessRate,
                            BaseSettings.CurrentSettings.FollowerRetraining.MinimumLevel,
                            "Follower Retraining",
                            Color.Blue)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.FollowerRetraining.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.FollowerRetraining.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.FollowerRetraining.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerRetraining); 
                #endregion

                #region FollowerContract
                UserControl_MissionReward missionReward_FollowerContract =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.FollowerContracts.Priority,
                            BaseSettings.CurrentSettings.FollowerContracts.SuccessRate,
                            BaseSettings.CurrentSettings.FollowerContracts.MinimumLevel,
                            "Follower Contracts",
                            Color.Blue)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.FollowerContracts.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.FollowerContracts.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.FollowerContracts.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_FollowerContract); 
                #endregion


                #region RushOrders
                UserControl_MissionReward missionReward_RushOrders =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.RushOrders.Priority,
                            BaseSettings.CurrentSettings.RushOrders.SuccessRate,
                            BaseSettings.CurrentSettings.RushOrders.MinimumLevel,
                            "Rush Orders",
                            Color.LawnGreen)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.RushOrders.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.RushOrders.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.RushOrders.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_RushOrders); 
                #endregion

                #region Apexis Crystal
                UserControl_MissionReward missionReward_Apexis =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.ApexisCrystal.Priority,
                            BaseSettings.CurrentSettings.ApexisCrystal.SuccessRate,
                            BaseSettings.CurrentSettings.ApexisCrystal.MinimumLevel,
                            "Apexis Crystals",
                            Color.LawnGreen)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.ApexisCrystal.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.ApexisCrystal.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.ApexisCrystal.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Apexis);
                #endregion

                #region PrimalSpirit

                UserControl_MissionReward missionReward_PrimalSpirit =
                new UserControl_MissionReward(
                    BaseSettings.CurrentSettings.PrimalSpirit.Priority,
                    BaseSettings.CurrentSettings.PrimalSpirit.SuccessRate,
                    BaseSettings.CurrentSettings.PrimalSpirit.MinimumLevel,
                    "Primal Spirit",
                    Color.LawnGreen)
                {
                    UpdatePriority = i => { BaseSettings.CurrentSettings.PrimalSpirit.Priority = i; },
                    UpdateSuccessRate = i => { BaseSettings.CurrentSettings.PrimalSpirit.SuccessRate = i; },
                    UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.PrimalSpirit.MinimumLevel = i; }
                };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_PrimalSpirit);

                #endregion

                #region SavageBlood
                UserControl_MissionReward missionReward_SavageBlood =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.SavageBlood.Priority,
                            BaseSettings.CurrentSettings.SavageBlood.SuccessRate,
                            BaseSettings.CurrentSettings.SavageBlood.MinimumLevel,
                            "Savage Blood",
                            Color.OrangeRed)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.SavageBlood.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.SavageBlood.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.SavageBlood.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_SavageBlood);

                #endregion

                #region SealOfTemperedFate
                UserControl_MissionReward missionReward_SealOfTemperedFate =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.SealOfTemperedFate.Priority,
                            BaseSettings.CurrentSettings.SealOfTemperedFate.SuccessRate,
                            BaseSettings.CurrentSettings.SealOfTemperedFate.MinimumLevel,
                            "Seal Of Tempered Fate",
                            Color.OrangeRed)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.SealOfTemperedFate.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.SealOfTemperedFate.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.SealOfTemperedFate.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_SealOfTemperedFate); 
                #endregion

                #region AbrogatorStone
                UserControl_MissionReward missionReward_AbrogatorStone =
                       new UserControl_MissionReward(
                           BaseSettings.CurrentSettings.AbrogatorStone.Priority,
                           BaseSettings.CurrentSettings.AbrogatorStone.SuccessRate,
                           BaseSettings.CurrentSettings.AbrogatorStone.MinimumLevel,
                           "Abrogator Stone",
                           Color.OrangeRed)
                       {
                           UpdatePriority = i => { BaseSettings.CurrentSettings.AbrogatorStone.Priority = i; },
                           UpdateSuccessRate = i => { BaseSettings.CurrentSettings.AbrogatorStone.SuccessRate = i; },
                           UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.AbrogatorStone.MinimumLevel = i; }
                       };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_AbrogatorStone); 
                #endregion

                #region ElementalRune
                UserControl_MissionReward missionReward_ElementalRune =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.ElementalRune.Priority,
                            BaseSettings.CurrentSettings.ElementalRune.SuccessRate,
                            BaseSettings.CurrentSettings.ElementalRune.MinimumLevel,
                            "Elemental Rune",
                            Color.OrangeRed)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.ElementalRune.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.ElementalRune.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.ElementalRune.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_ElementalRune); 
                #endregion

                #region Items
                UserControl_MissionReward missionReward_Items =
                        new UserControl_MissionReward(
                            BaseSettings.CurrentSettings.Items.Priority,
                            BaseSettings.CurrentSettings.Items.SuccessRate,
                            BaseSettings.CurrentSettings.Items.MinimumLevel,
                            "Misc Items",
                            Color.OrangeRed)
                        {
                            UpdatePriority = i => { BaseSettings.CurrentSettings.Items.Priority = i; },
                            UpdateSuccessRate = i => { BaseSettings.CurrentSettings.Items.SuccessRate = i; },
                            UpdateMinimumLevel = i => { BaseSettings.CurrentSettings.Items.MinimumLevel = i; }
                        };
                flowLayoutPanel_MissionRewards.Controls.Add(missionReward_Items);
                #endregion

                

                #endregion

                checkBox_MissionRewards_FollowerToken_ArmorSet615.Checked =BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615;
                checkBox_MissionRewards_FollowerToken_ArmorSet615.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615;

                checkBox_MissionRewards_FollowerToken_ArmorSet630.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630;
                checkBox_MissionRewards_FollowerToken_ArmorSet630.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630;

                checkBox_MissionRewards_FollowerToken_ArmorSet645.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645;
                checkBox_MissionRewards_FollowerToken_ArmorSet645.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645;

                checkBox_MissionRewards_FollowerToken_WeaponSet615.Checked=BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615;
                checkBox_MissionRewards_FollowerToken_WeaponSet615.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615=!BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615;

                checkBox_MissionRewards_FollowerToken_WeaponSet630.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630;
                checkBox_MissionRewards_FollowerToken_WeaponSet630.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630=!BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630;

                checkBox_MissionRewards_FollowerToken_WeaponSet645.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet645;
                checkBox_MissionRewards_FollowerToken_WeaponSet645.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet645=!BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet645;

                trackBar_ItemReward_CharacterTokenLevel.Value=BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel;
                label_ItemReward_CharacterTokenLevel.Text=BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel.ToString(CultureInfo.InvariantCulture);
                trackBar_ItemReward_CharacterTokenLevel.ValueChanged += trackBar_ItemReward_CharacterTokenLevel_SliderChanged;

                //
                checkBox_Behavior_Disenchant.Checked = BaseSettings.CurrentSettings.BehaviorDisenchanting;
                checkBox_Behavior_Disenchant.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorDisenchanting=!BaseSettings.CurrentSettings.BehaviorDisenchanting;

                checkBox_Behavior_HerbGather.Checked = BaseSettings.CurrentSettings.BehaviorHerbGather;
                checkBox_Behavior_HerbGather.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorHerbGather=!BaseSettings.CurrentSettings.BehaviorHerbGather;

                checkBox_Behavior_MineGather.Checked = BaseSettings.CurrentSettings.BehaviorMineGather;
                checkBox_Behavior_MineGather.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorMineGather=!BaseSettings.CurrentSettings.BehaviorMineGather;

                checkBox_Behavior_Quests.Checked = BaseSettings.CurrentSettings.BehaviorQuests;
                checkBox_Behavior_Quests.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorQuests=!BaseSettings.CurrentSettings.BehaviorQuests;

                checkBox_Behavior_Professions.Checked = BaseSettings.CurrentSettings.BehaviorProfessions;
                checkBox_Behavior_Professions.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorProfessions=!BaseSettings.CurrentSettings.BehaviorProfessions;

                checkBox_Behavior_RepairSell.Checked = BaseSettings.CurrentSettings.BehaviorRepairSell;
                checkBox_Behavior_RepairSell.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorRepairSell=!BaseSettings.CurrentSettings.BehaviorRepairSell;

                checkBox_Behavior_StartMissions.Checked = BaseSettings.CurrentSettings.BehaviorMissionStart;
                checkBox_Behavior_StartMissions.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorMissionStart=!BaseSettings.CurrentSettings.BehaviorMissionStart;

                checkBox_Behavior_WorkOrderPickup.Checked = BaseSettings.CurrentSettings.BehaviorWorkOrderPickup;
                checkBox_Behavior_WorkOrderPickup.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorWorkOrderPickup=!BaseSettings.CurrentSettings.BehaviorWorkOrderPickup;

                checkBox_Behavior_WorkOrderStart.Checked = BaseSettings.CurrentSettings.BehaviorWorkOrderStartup;
                checkBox_Behavior_WorkOrderStart.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorWorkOrderStartup=!BaseSettings.CurrentSettings.BehaviorWorkOrderStartup;

                checkBox_Behavior_Salvaging.Checked = BaseSettings.CurrentSettings.BehaviorSalvaging;
                checkBox_Behavior_Salvaging.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorSalvaging=!BaseSettings.CurrentSettings.BehaviorSalvaging;

                checkBox_Behavior_LootCache.Checked = BaseSettings.CurrentSettings.BehaviorLootCache;
                checkBox_Behavior_LootCache.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.BehaviorLootCache=!BaseSettings.CurrentSettings.BehaviorLootCache;

                checkBox_MailAutoGet.Checked = BaseSettings.CurrentSettings.MailAutoGet;
                checkBox_MailAutoGet.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MailAutoGet=!BaseSettings.CurrentSettings.MailAutoGet;

                checkBox_MailAutoSend.Checked = BaseSettings.CurrentSettings.MailAutoSend;
                checkBox_MailAutoSend.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MailAutoSend=!BaseSettings.CurrentSettings.MailAutoSend;

                //
                checkBox_MailEnchanting.Checked = BaseSettings.CurrentSettings.MailSendEnchanting;
                checkBox_MailEnchanting.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.MailSendEnchanting = !BaseSettings.CurrentSettings.MailSendEnchanting;;
                textBox_MailEnchanting.Text = BaseSettings.CurrentSettings.MailSendEnchantingRecipient;
                textBox_MailEnchanting.TextChanged += textBox_MailEnchanting_TextChanged;

                checkBox_MailEpic.Checked = BaseSettings.CurrentSettings.MailSendEpic;
                checkBox_MailEpic.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MailSendEpic=!BaseSettings.CurrentSettings.MailSendEpic;
                textBox_MailEpic.Text = BaseSettings.CurrentSettings.MailSendEpicRecipient;
                textBox_MailEpic.TextChanged += textBox_MailEpic_TextChanged;

                checkBox_MailHerbs.Checked = BaseSettings.CurrentSettings.MailSendHerbs;
                checkBox_MailHerbs.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MailSendHerbs=!BaseSettings.CurrentSettings.MailSendHerbs;
                textBox_MailHerbs.Text = BaseSettings.CurrentSettings.MailSendHerbsRecipient;
                textBox_MailHerbs.TextChanged += textBox_MailHerbs_TextChanged;

                checkBox_MailOre.Checked = BaseSettings.CurrentSettings.MailSendOre;
                checkBox_MailOre.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MailSendOre=!BaseSettings.CurrentSettings.MailSendOre;
                textBox_MailOre.Text = BaseSettings.CurrentSettings.MailSendOreRecipient;
                textBox_MailOre.TextChanged += textBox_MailOre_TextChanged;

                checkBox_MailRare.Checked = BaseSettings.CurrentSettings.MailSendRare;
                checkBox_MailRare.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MailSendRare=!BaseSettings.CurrentSettings.MailSendRare;
                textBox_MailRare.Text = BaseSettings.CurrentSettings.MailSendRareRecipient;
                textBox_MailRare.TextChanged += textBox_MailRare_TextChanged;

                checkBox_MailUncommon.Checked = BaseSettings.CurrentSettings.MailSendUncommon;
                checkBox_MailUncommon.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.MailSendUncommon=!BaseSettings.CurrentSettings.MailSendUncommon;
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
                    comboBox_PrimalSpiritItems.Items.Add(item.Type.ToString());
                    if (BaseSettings.CurrentSettings.PrimalSpiritItemId == item.ItemId)
                        primalTraderIndex = index;
                    index++;
                }
                comboBox_PrimalSpiritItems.SelectedIndex = primalTraderIndex;
                comboBox_PrimalSpiritItems.SelectedIndexChanged += comboBox_PrimalSpiritItems_SelectedIndexChanged;

                //
                flowLayoutPanel_WorkOrderTypes.Controls.Clear();

                Func<object, string> fRetrieveWorkOrderTypesName = s => Enum.GetName(typeof(WorkOrderType), s);
                bool noWorkOrderTypeFlag = BaseSettings.CurrentSettings.WorkOrderTypes.Equals(WorkOrderType.None);
                foreach (var value in Enum.GetValues(typeof(WorkOrderType)))
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
                checkBox_Vendor_Junk.CheckedChanged += (sender, args) =>  BaseSettings.CurrentSettings.VendorJunkItems=! BaseSettings.CurrentSettings.VendorJunkItems;
                checkBox_Vendor_Common.Checked = BaseSettings.CurrentSettings.VendorCommonItems;
                checkBox_Vendor_Common.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.VendorCommonItems=!BaseSettings.CurrentSettings.VendorCommonItems;
                checkBox_Vendor_Uncommon.Checked = BaseSettings.CurrentSettings.VendorUncommonItems;
                checkBox_Vendor_Uncommon.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.VendorUncommonItems=!BaseSettings.CurrentSettings.VendorUncommonItems;
                checkBox_Vendor_Rare.Checked = BaseSettings.CurrentSettings.VendorRareItems;
                checkBox_Vendor_Rare.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.VendorRareItems=!BaseSettings.CurrentSettings.VendorRareItems;

                checkBox_Disenchanting_UncommonItems.Checked = BaseSettings.CurrentSettings.DisenchantingUncommon;
                checkBox_Disenchanting_UncommonItems.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DisenchantingUncommon=!BaseSettings.CurrentSettings.DisenchantingUncommon;
                checkBox_Disenchanting_Epic.Checked = BaseSettings.CurrentSettings.DisenchantingEpic;
                checkBox_Disenchanting_Epic.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DisenchantingEpic=!BaseSettings.CurrentSettings.DisenchantingEpic;
                checkBox_Disenchanting_RareItems.Checked = BaseSettings.CurrentSettings.DisenchantingRare;
                checkBox_Disenchanting_RareItems.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DisenchantingRare=!BaseSettings.CurrentSettings.DisenchantingRare;

                checkBox_Disenchanting_UncommonSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded;
                checkBox_Disenchanting_UncommonSoulbound.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded=!BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded;
                checkBox_Disenchanting_RareSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingRareSoulbounded;
                checkBox_Disenchanting_RareSoulbound.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DisenchantingRareSoulbounded=!BaseSettings.CurrentSettings.DisenchantingRareSoulbounded;
                checkBox_Disenchanting_EpicSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded;
                checkBox_Disenchanting_EpicSoulbound.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded=!BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded;

                textBox_Disenchanting_UncommonLevel.Text = BaseSettings.CurrentSettings.DisenchantingUncommonItemLevel.ToString();
                textBox_Disenchanting_UncommonLevel.TextChanged += textBox_Disenchanting_UncommonLevel_TextedChanged;
                textBox_Disenchanting_RareLevel.Text = BaseSettings.CurrentSettings.DisenchantingRareItemLevel.ToString();
                textBox_Disenchanting_RareLevel.TextChanged += textBox_Disenchanting_RareLevel_TextedChanged;
                textBox_Disenchanting_EpicLevel.Text = BaseSettings.CurrentSettings.DisenchantingEpicItemLevel.ToString();
                textBox_Disenchanting_EpicLevel.TextChanged += textBox_Disenchanting_EpicLevel_TextedChanged;



                checkBox_ExchangePrimalSpirits.Checked = BaseSettings.CurrentSettings.ExchangePrimalSpirits;
                checkBox_ExchangePrimalSpirits.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.ExchangePrimalSpirits=!BaseSettings.CurrentSettings.ExchangePrimalSpirits;

                #region Profession Crafting Settings

                foreach (var value in PlayerProfessions.ProfessionDailyCooldownSpellIds)
                {
                    FlowLayoutPanel layoutPanelWrapper = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.TopDown,
                        BorderStyle = BorderStyle.Fixed3D,
                        Size = new Size(185, 125),
                        Dock = DockStyle.Top,
                    };

                    Label lblType = new Label
                    {
                        Text = "Crafting " + value.Key.ToString(),
                        Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                        AutoSize = true,
                    };
                    layoutPanelWrapper.Controls.Add(lblType);

                    foreach (var i in value.Value)
                    {
                        CheckBox newCheckBox = new CheckBox
                        {
                            Name = i.ToString(),
                            Text = PlayerProfessions.GetProfessionCraftingName(i),
                            Checked = BaseSettings.CurrentSettings.ProfessionSpellIds.Contains(i),
                            Font = new Font(FontFamily.GenericSansSerif, 9.5f, FontStyle.Regular),
                            AutoSize = true,

                        };
                        newCheckBox.CheckedChanged += checkBox_ProfessionSpellId_Checked;
                        layoutPanelWrapper.Controls.Add(newCheckBox);
                    }

                    switch (value.Key)
                    {
                        case SkillLine.Alchemy:
                            tabPage_Alchemy.Controls.Add(layoutPanelWrapper);
                            break;
                        case SkillLine.Blacksmithing:
                            tabPage_Blacksmithing.Controls.Add(layoutPanelWrapper);
                            break;
                        case SkillLine.Enchanting:
                            tabPage_Enchanting.Controls.Add(layoutPanelWrapper);
                            break;
                        case SkillLine.Engineering:
                            tabPage_Engineering.Controls.Add(layoutPanelWrapper);
                            break;
                        case SkillLine.Inscription:
                            tabPage_Inscription.Controls.Add(layoutPanelWrapper);
                            break;
                        case SkillLine.Jewelcrafting:
                            tabPage_Jewelcrafting.Controls.Add(layoutPanelWrapper);
                            break;
                        case SkillLine.Leatherworking:
                            tabPage_Leatherworking.Controls.Add(layoutPanelWrapper);
                            break;
                        case SkillLine.Tailoring:
                            tabPage_Tailoring.Controls.Add(layoutPanelWrapper);
                            break;
                    }
                }

                #region Milling Settings

                //Milling (Inscription)
                checkBox_Milling_Enabled.Checked = BaseSettings.CurrentSettings.MillingEnabled;
                checkBox_Milling_Enabled.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.MillingEnabled=!BaseSettings.CurrentSettings.MillingEnabled;

                textBox_Milling_RequiredAmount.Text = BaseSettings.CurrentSettings.MillingMinimum.ToString();
                textBox_Milling_RequiredAmount.TextChanged += textBox_Milling_Minimum_TextedChanged;

                foreach (var id in PlayerInventory.HerbIds)
                {
                    var name = Enum.GetName(typeof(CraftingReagents), id);
                    CraftingReagents type = (CraftingReagents)Enum.Parse(typeof(CraftingReagents), name);
                    InscriptionMillingSetting millingSetting = null;
                    switch (type)
                    {
                        case CraftingReagents.Frostweed:
                            millingSetting = BaseSettings.CurrentSettings.MillingFrostWeed;
                            break;
                        case CraftingReagents.Fireweed:
                            millingSetting = BaseSettings.CurrentSettings.MillingFireWeed;
                            break;
                        case CraftingReagents.NagrandArrowbloom:
                            millingSetting = BaseSettings.CurrentSettings.MillingNagrandArrowbloom;
                            break;
                        case CraftingReagents.GorgrondFlytrap:
                            millingSetting = BaseSettings.CurrentSettings.MillingGorgrondFlytrap;
                            break;
                        case CraftingReagents.Starflower:
                            millingSetting = BaseSettings.CurrentSettings.MillingStarflower;
                            break;
                        case CraftingReagents.TaladorOrchid:
                            millingSetting = BaseSettings.CurrentSettings.MillingTaladorOrchid;
                            break;
                    }

                    CheckBox newCheckBox = new CheckBox
                    {
                        Name = "millingenabled_" + id.ToString(),
                        Text = name,
                        Checked = !millingSetting.Ignored,
                        Font = new Font(FontFamily.GenericSansSerif, 9.5f, FontStyle.Regular),
                        Size = new Size(190, 25),
                        Padding = new Padding(0, 0, 25, 0),
                    };
                    newCheckBox.CheckedChanged += checkBox_MillingHerb_Checked;
                    flowLayoutPanel_Milling.Controls.Add(newCheckBox);

                    TextBox newTextBox = new TextBox
                    {
                        Name = "millingenabled_" + id.ToString(),
                        Text = millingSetting.Reserved.ToString(),
                        Font = new Font(FontFamily.GenericSansSerif, 9.5f, FontStyle.Regular),
                        Size = new Size(50, 25),
                    };
                    newTextBox.TextChanged += textBox_MillingHerb_TextChanged;
                    newTextBox.KeyPress += textBox_Numbers_KeyPress;
                    flowLayoutPanel_Milling.SetFlowBreak(newTextBox, true);
                    flowLayoutPanel_Milling.Controls.Add(newTextBox);
                }
                
                #endregion

                #endregion

                trackBar_MinimumBagSlotsFree.Value = BaseSettings.CurrentSettings.MinimumBagSlotsFree;
                textBox_MinimumBagSlotsFree.Text = BaseSettings.CurrentSettings.MinimumBagSlotsFree.ToString();
                trackBar_MinimumBagSlotsFree.ValueChanged += trackBar_MinimumBagSlotsFree_SliderChanged;

                //checkBox_Follower_170
                //checkBox_Follower_189
                //checkBox_Follower_190
                //checkBox_Follower_193
                //checkBox_Follower_207
                //checkBox_Follower_467


                //Optional follower aquire behavior
                checkBox_Follower_170.Name = "170";
                checkBox_Follower_189.Name = "189";
                checkBox_Follower_190.Name = "190";
                checkBox_Follower_193.Name = "193";
                checkBox_Follower_207.Name = "207";
                checkBox_Follower_467.Name = "467";
                checkBox_Follower_209.Name = "209";
                checkBox_Follower_170.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(170);
                checkBox_Follower_189.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(189);
                checkBox_Follower_190.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(190);
                checkBox_Follower_193.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(193);
                checkBox_Follower_207.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(207);
                checkBox_Follower_467.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(467);
                checkBox_Follower_209.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(209);
                checkBox_Follower_170.CheckedChanged += checkBox_Follower_CheckedChanged;
                checkBox_Follower_189.CheckedChanged += checkBox_Follower_CheckedChanged;
                checkBox_Follower_190.CheckedChanged += checkBox_Follower_CheckedChanged;
                checkBox_Follower_193.CheckedChanged += checkBox_Follower_CheckedChanged;
                checkBox_Follower_207.CheckedChanged += checkBox_Follower_CheckedChanged;
                checkBox_Follower_467.CheckedChanged += checkBox_Follower_CheckedChanged;
                checkBox_Follower_209.CheckedChanged += checkBox_Follower_CheckedChanged;

                //Daily Quests
                checkBox_DailyQuest_Warmill.Checked = BaseSettings.CurrentSettings.DailyWarMillQuestSettings.Enabled;
                checkBox_DailyQuest_Warmill.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DailyWarMillQuestSettings.Enabled = !BaseSettings.CurrentSettings.DailyWarMillQuestSettings.Enabled;
                if (BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex > -1)
                {
                    comboBox_DailyQuest_WarMill_Rewards.SelectedIndex =
                        BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex;
                }
                comboBox_DailyQuest_WarMill_Rewards.SelectedIndexChanged += (sender, args) =>
                {
                    ComboBox senderCB = (ComboBox) sender;
                    if (senderCB.SelectedIndex > -1)
                    {
                        BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex = senderCB.SelectedIndex;
                    }
                };

                checkBox_DailyQuest_AlchemyLab.Checked = BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.Enabled;
                checkBox_DailyQuest_AlchemyLab.CheckedChanged += (sender, args) =>BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.Enabled=!BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.Enabled;
                if (BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex > -1)
                {
                    comboBox_DailyQuest_Alchemy_Rewards.SelectedIndex =
                        BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex;
                }
                comboBox_DailyQuest_Alchemy_Rewards.SelectedIndexChanged += (sender, args) =>
                {
                    ComboBox senderCB = (ComboBox)sender;
                    if (senderCB.SelectedIndex > -1)
                    {
                        BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex = senderCB.SelectedIndex;
                    }
                };

                //debug
                checkBox_Debug_FakeStartWorkOrder.Checked = BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER;
                checkBox_Debug_FakeStartWorkOrder.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER=!BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER;

                checkBox_Debug_FakeFinishQuest.Checked = BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST;
                checkBox_Debug_FakeFinishQuest.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST=!BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST;


                checkBox_Debug_IgnoreHearthStone.Checked = BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE;
                checkBox_Debug_IgnoreHearthStone.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE=!BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE;

                checkBox_Debug_FakePickupWorkOrder.Checked = BaseSettings.CurrentSettings.DEBUG_FAKEPICKUPWORKORDER;
                checkBox_Debug_FakePickupWorkOrder.CheckedChanged += (sender, args) => BaseSettings.CurrentSettings.DEBUG_FAKEPICKUPWORKORDER=!BaseSettings.CurrentSettings.DEBUG_FAKEPICKUPWORKORDER;
            }
            catch (Exception ex)
            {
                GarrisonBase.Err("Failed to initalize config window!\r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private void trackBar_ReservedGarrisonResources_SliderChanged(object sender, EventArgs e)
        {
            TrackBar slider_sender = (TrackBar)sender;
            int Value = (int)slider_sender.Value;
            textBox_ReservedGarrisonResources.Text = Value.ToString();
        }
        private void textBox_GarrisonReservered_TextedChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
            var value = txtbox.Text.Trim();
            if (string.IsNullOrEmpty(value)) value = "0";
            var intValue = Convert.ToInt32(value);
            trackBar_ReservedGarrisonResources.Value = intValue;
            BaseSettings.CurrentSettings.ReservedGarrisonResources = intValue;
        }
        private void textBox_GarrisonReservered_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Non Numerical and Control keys ignored!
            if (!char.IsControl(e.KeyChar) && !char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
            }

            if (char.IsNumber(e.KeyChar))
            {
                TextBox senderTextBox = (sender as TextBox);

                var curText = senderTextBox.Text;
                var selectionLength = senderTextBox.SelectionLength;
                var selectionStart = senderTextBox.SelectionStart;


                if (selectionLength > 0)
                {//Selection (highlighted text) needs removed!
                    curText = curText.Remove(selectionStart, selectionLength);
                }

                //Add the new number to the text at current selected position.
                curText = curText.Insert(selectionStart, e.KeyChar.ToString());

                int currentValue = Convert.ToInt32(curText);
                if (currentValue > 10000)
                {//Exceedes are maximum value.. set to max!
                    senderTextBox.Text = "10000";
                    senderTextBox.Select(senderTextBox.Text.Length, 0);
                    e.Handled = true;
                }
                else if (curText.StartsWith("0"))
                {//Starts with a zero.. replace with correct text!
                    senderTextBox.Text = currentValue.ToString();
                    senderTextBox.Select(senderTextBox.Text.Length, 0);
                    e.Handled = true;
                }
            }
            else if (e.KeyChar == '\b')
            {
                TextBox senderTextBox = (sender as TextBox);
                if (senderTextBox.SelectionStart > 0 || senderTextBox.SelectionLength == senderTextBox.TextLength)
                {
                    if (senderTextBox.Text.Length == 1 || senderTextBox.SelectionLength == senderTextBox.TextLength)
                    {
                        senderTextBox.Text = "0";
                        senderTextBox.Select(senderTextBox.Text.Length, 0);
                        e.Handled = true;
                    }
                }

            }
        }
        private void checkBox_ProfessionSpellId_Checked(object sender, EventArgs e)
        {
            var cbSender = (CheckBox)sender;
            var value = cbSender.Name.ToInt32();

            if (BaseSettings.CurrentSettings.ProfessionSpellIds.Contains(value))
                BaseSettings.CurrentSettings.ProfessionSpellIds.Remove(value);
            else
                BaseSettings.CurrentSettings.ProfessionSpellIds.Add(value);
        }

        private void textBox_Milling_Minimum_TextedChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
            var value = txtbox.Text.Trim();
            if (string.IsNullOrEmpty(value)) value = "0";
            BaseSettings.CurrentSettings.MillingMinimum = value.ToInt32();
        }
        private void checkBox_MillingHerb_Checked(object sender, EventArgs e)
        {
            var cbSender = (CheckBox)sender;
            var id = StringHelper.ExtractNumbers(cbSender.Name).ToInt32();
            var name = Enum.GetName(typeof(CraftingReagents), id);
            CraftingReagents type = (CraftingReagents)Enum.Parse(typeof(CraftingReagents), name);
           
            switch (type)
            {
                case CraftingReagents.Frostweed:
                    BaseSettings.CurrentSettings.MillingFrostWeed.Ignored=!cbSender.Checked;
                    break;
                case CraftingReagents.Fireweed:
                    BaseSettings.CurrentSettings.MillingFireWeed.Ignored = !cbSender.Checked;
                    break;
                case CraftingReagents.NagrandArrowbloom:
                    BaseSettings.CurrentSettings.MillingNagrandArrowbloom.Ignored = !cbSender.Checked;
                    break;
                case CraftingReagents.GorgrondFlytrap:
                    BaseSettings.CurrentSettings.MillingGorgrondFlytrap.Ignored = !cbSender.Checked;
                    break;
                case CraftingReagents.Starflower:
                    BaseSettings.CurrentSettings.MillingStarflower.Ignored = !cbSender.Checked;
                    break;
                case CraftingReagents.TaladorOrchid:
                    BaseSettings.CurrentSettings.MillingTaladorOrchid.Ignored = !cbSender.Checked;
                    break;
            }
         
        }
        private void textBox_MillingHerb_TextChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
            var value = txtbox.Text.Trim();
            if (string.IsNullOrEmpty(value)) value = "0";

            var id = StringHelper.ExtractNumbers(txtbox.Name).ToInt32();
            var name = Enum.GetName(typeof(CraftingReagents), id);
            CraftingReagents type = (CraftingReagents)Enum.Parse(typeof(CraftingReagents), name);

            switch (type)
            {
                case CraftingReagents.Frostweed:
                    BaseSettings.CurrentSettings.MillingFrostWeed.Reserved = value.ToInt32();
                    break;
                case CraftingReagents.Fireweed:
                    BaseSettings.CurrentSettings.MillingFireWeed.Reserved = value.ToInt32();
                    break;
                case CraftingReagents.NagrandArrowbloom:
                    BaseSettings.CurrentSettings.MillingNagrandArrowbloom.Reserved = value.ToInt32();
                    break;
                case CraftingReagents.GorgrondFlytrap:
                    BaseSettings.CurrentSettings.MillingGorgrondFlytrap.Reserved = value.ToInt32();
                    break;
                case CraftingReagents.Starflower:
                    BaseSettings.CurrentSettings.MillingStarflower.Reserved = value.ToInt32();
                    break;
                case CraftingReagents.TaladorOrchid:
                    BaseSettings.CurrentSettings.MillingTaladorOrchid.Reserved = value.ToInt32();
                    break;
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

        private void textBox_Disenchanting_UncommonLevel_TextedChanged(object sender, EventArgs e)
        {
            var txtbox = (TextBox)sender;
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

        private void comboBox_PrimalSpiritItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_PrimalSpiritItems.SelectedIndex > -1)
            {
                string name = comboBox_PrimalSpiritItems.SelectedItem.ToString();
                var enumvalue = (GarrisonManager.PrimalTraderItemTypes)Enum.Parse(typeof(GarrisonManager.PrimalTraderItemTypes), name);
                //BaseSettings.CurrentSettings.PrimalSpiritItem = comboBox_PrimalSpiritItems.SelectedItem.ToString();
                BaseSettings.CurrentSettings.PrimalSpiritItemId = Convert.ToUInt32((int) enumvalue);
            }
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
      
        private void trackBar_ItemReward_CharacterTokenLevel_SliderChanged(object sender, EventArgs e)
        {
            TrackBar slider_sender = (TrackBar)sender;
            int Value = (int)slider_sender.Value;
            BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel = Value;
            label_ItemReward_CharacterTokenLevel.Text = Value.ToString();
        }
        
        private void trackBar_MinimumBagSlotsFree_SliderChanged(object sender, EventArgs e)
        {
            TrackBar slider_sender = (TrackBar)sender;
            int Value = (int)slider_sender.Value;
            BaseSettings.CurrentSettings.MinimumBagSlotsFree = Value;
            textBox_MinimumBagSlotsFree.Text = Value.ToString();
        }
        private void checkBox_Follower_CheckedChanged(object sender, EventArgs e)
        {
            var checkboxSender = (CheckBox) sender;
            int followerid = checkboxSender.Name.ToInt32();
            if (BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid))
                BaseSettings.CurrentSettings.FollowerOptionalList.Remove(followerid);
            else
                BaseSettings.CurrentSettings.FollowerOptionalList.Add(followerid);
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

            

            if (BehaviorManager.CurrentBehavior != null)
            {
                LBDebug.Controls.Add(
                    new UserControlDebugEntry(String.Format("Current behavior {0}",
                        BehaviorManager.CurrentBehavior)));
            }

            LBDebug.Controls.Add(
                 new UserControlDebugEntry(String.Format("Total behaviors {0}",
                     BehaviorManager.Behaviors.Count)));
            foreach (var b in BehaviorManager.Behaviors)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }

            if (BehaviorManager.SwitchBehavior != null)
            {
                LBDebug.Controls.Add(
                    new UserControlDebugEntry(String.Format("Switch behavior {0}",
                        BehaviorManager.SwitchBehavior)));
            }

            LBDebug.Controls.Add(
                new UserControlDebugEntry(String.Format("Total switch behaviors {0}",
                    BehaviorManager.SwitchBehaviors.Count)));

            foreach (var b in BehaviorManager.SwitchBehaviors)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }


            LBDebug.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();


            LBDebug.Controls.Add(
                new UserControlDebugEntry(String.Format("Total completed missions {0}",
                    GarrisonManager.CompletedMissionIds.Count)));

            LBDebug.Controls.Add(
                new UserControlDebugEntry(String.Format("Total mission IDs {0}",
                    GarrisonManager.AvailableMissionIds.Count)));

            foreach (var b in GarrisonManager.AvailableMissions)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("Available: " + b.ToString()));
            }

            foreach (var b in GarrisonManager.CompletedMissions)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("Completed: " + b.ToString()));
            }
            LBDebug.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            LBDebug.Controls.Add(new UserControlDebugEntry("Followers Collected", Color.WhiteSmoke, Color.DarkGray));
            foreach (var b in GarrisonManager.Followers.Values)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }

            LBDebug.Controls.Add(new UserControlDebugEntry("Followers Not Collected", Color.WhiteSmoke, Color.DarkGray));
            foreach (var b in GarrisonManager.FollowerIdsNotCollected)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }

            LBDebug.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!TextContainsAllNumbers(textBox.Text))
            {
                Char[] textBoxChars = textBox.Text.ToCharArray();
                List<int> removalList = new List<int>();
                for (int i = textBoxChars.Length - 1; i > 0; i--)
                {
                    if (!Char.IsNumber(textBoxChars[i]))
                        removalList.Add(i);
                }
                foreach (var i in removalList)
                {
                    textBox.Text = textBox.Text.Remove(i, 1);
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
                var Objects = Cache.ObjectCacheManager.ObjectCollection.Values.OrderByDescending(o => o.SubType).ThenBy(o => o.Distance).ToList();
                foreach (var cWoWObject in Objects)
                {
                    Color foreColor = (cWoWObject is C_WoWUnit) ? Color.Black : Color.GhostWhite;
                    Color backColor = (cWoWObject is C_WoWGameObject) ? Color.DarkSlateGray
                                : (cWoWObject is C_WoWUnit) ? Color.MediumSeaGreen
                                : Color.Gray;
                    string entryString = cWoWObject.ToString();
                    UserControlDebugEntry entry = new UserControlDebugEntry(entryString, foreColor, backColor);
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
                foreach (var item in Character.Player.Inventory.BagItems)
                {
                    string entryString = item.ToString();
                    UserControlDebugEntry entry = new UserControlDebugEntry(entryString);
                    LBDebug.Controls.Add(entry);
                }

                foreach (var item in Character.Player.Inventory.BankItems)
                {
                    string entryString = item.ToString();
                    UserControlDebugEntry entry = new UserControlDebugEntry(entryString);
                    LBDebug.Controls.Add(entry);
                }

                foreach (var item in Character.Player.Inventory.BankReagentItems)
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
                BaseSettings.CurrentSettings.Dictmailsenditems.Clear();
            }
        }

        private void button_MailItem_Add_Click(object sender, EventArgs e)
        {
            string entryId = textBox_MailItem_EntryId.Text;
            if (String.IsNullOrEmpty(entryId)) return;
            if (!StringHelper.TextIsAllNumerical(entryId)) return;

            string recipient = textBox_MailItem_Recipient.Text;
            if (String.IsNullOrEmpty(recipient)) return;

            string count = textBox_MailItem_Count.Text;
            if (String.IsNullOrEmpty(count)) return;
            if (!StringHelper.TextIsAllNumerical(count)) return;

            string name = "New Entry";
            string txtbox_name = textBox_MailItem_Name.Text;
            if (!String.IsNullOrEmpty(txtbox_name)) name = txtbox_name;

            string[] entry =
                    {
                        entryId, name, recipient, count
                    };
            ListViewItem lvi = new ListViewItem(entry);
            listView_MailItems.Items.Add(lvi);
            BaseSettings.CurrentSettings.MailSendItems.Add(new MailItem(
                                                                entryId.ToInt32(), 
                                                                "New Entry", 
                                                                recipient,
                                                                count.ToInt32()));
            BaseSettings.CurrentSettings.Dictmailsenditems.Clear();
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
                BaseSettings.CurrentSettings.Dictmailsenditems.Clear();
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


        private void button7_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                foreach (var item in QuestHelper.QuestLog)
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

        private void button8_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();
            try
            {
                foreach (var n in TaxiFlightHelper.FlightPaths)
                {
                    LBDebug.Controls.Add(new UserControlDebugEntry(n.ToString()));
                }
            }
            catch (Exception ex)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
            }

            LBDebug.Focus();
        }


        private void Control_MouseEnter(object sender, EventArgs e)
        {
            ((Control) sender).Focus();
        }

    }
}
