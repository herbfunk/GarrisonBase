using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals.Garrison;
using FlowDirection = System.Windows.Forms.FlowDirection;
using FontStyle = System.Drawing.FontStyle;
using Size = System.Drawing.Size;

namespace Herbfunk.GarrisonBase.Config
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();

            try
            {

                InitalizeControls();

            }
            catch (Exception ex)
            {
                GarrisonBase.Err("Failed to initalize config window!\r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void InitalizeControls()
        {
            initalizecontrols_missions();
            initalizecontrols_workorders();
            initalizecontrols_professions();
            initalizecontrols_mail();
            initalizecontrols_misc();
            initalizecontrols_debug();
        }

        private void initalizecontrols_missions()
        {
            checkBox_Behavior_CompleteMissions.ClearHandlers();
            checkBox_Behavior_CompleteMissions.Checked = BaseSettings.CurrentSettings.BehaviorMissionComplete;
            checkBox_Behavior_CompleteMissions.CCheckedChanged += (sender, args) =>
            {
                BaseSettings.CurrentSettings.BehaviorMissionComplete = !BaseSettings.CurrentSettings.BehaviorMissionComplete;
                checkBox_Behavior_StartMissions.Enabled = BaseSettings.CurrentSettings.BehaviorMissionComplete;
            };

            checkBox_Behavior_StartMissions.ClearHandlers();
            checkBox_Behavior_StartMissions.Checked = BaseSettings.CurrentSettings.BehaviorMissionStart;
            checkBox_Behavior_StartMissions.CCheckedChanged += (sender, args) => BaseSettings.CurrentSettings.BehaviorMissionStart = !BaseSettings.CurrentSettings.BehaviorMissionStart;
            checkBox_Behavior_StartMissions.Enabled = BaseSettings.CurrentSettings.BehaviorMissionComplete;


            trackBar_ReservedGarrisonResources.ClearHandlers();
            textBox_ReservedGarrisonResources.ClearHandlers();
            trackBar_ReservedGarrisonResources.Value = BaseSettings.CurrentSettings.ReservedGarrisonResources;
            textBox_ReservedGarrisonResources.COnKeyPressed += textBox_GarrisonReservered_KeyPress;
            textBox_ReservedGarrisonResources.Text = BaseSettings.CurrentSettings.ReservedGarrisonResources.ToString(CultureInfo.InvariantCulture);
            textBox_ReservedGarrisonResources.COnTextChanged += textBox_GarrisonReservered_TextedChanged;
            trackBar_ReservedGarrisonResources.COnValueChanged += (sender, args) =>
            {
                int Value = (int)((TrackBar)sender).Value;
                textBox_ReservedGarrisonResources.Text = Value.ToString();
            };
            trackBar_ReservedGarrisonResources.MouseEnter += Control_MouseEnter;

            #region Mission Reward Controls
            //
            flowLayoutPanel_MissionRewards.MouseEnter += Control_MouseEnter;
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

            checkBox_MissionRewards_FollowerToken_ArmorSet615.ClearHandlers();
            checkBox_MissionRewards_FollowerToken_ArmorSet615.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615;
            checkBox_MissionRewards_FollowerToken_ArmorSet615.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615;

            checkBox_MissionRewards_FollowerToken_ArmorSet630.ClearHandlers();
            checkBox_MissionRewards_FollowerToken_ArmorSet630.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630;
            checkBox_MissionRewards_FollowerToken_ArmorSet630.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630;

            checkBox_MissionRewards_FollowerToken_ArmorSet645.ClearHandlers();
            checkBox_MissionRewards_FollowerToken_ArmorSet645.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645;
            checkBox_MissionRewards_FollowerToken_ArmorSet645.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645;

            checkBox_MissionRewards_FollowerToken_WeaponSet615.ClearHandlers();
            checkBox_MissionRewards_FollowerToken_WeaponSet615.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615;
            checkBox_MissionRewards_FollowerToken_WeaponSet615.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615;

            checkBox_MissionRewards_FollowerToken_WeaponSet630.ClearHandlers();
            checkBox_MissionRewards_FollowerToken_WeaponSet630.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630;
            checkBox_MissionRewards_FollowerToken_WeaponSet630.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630;

            checkBox_MissionRewards_FollowerToken_WeaponSet645.ClearHandlers();
            checkBox_MissionRewards_FollowerToken_WeaponSet645.Checked = BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet645;
            checkBox_MissionRewards_FollowerToken_WeaponSet645.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet645 = !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet645;

            trackBar_ItemReward_CharacterTokenLevel.ClearHandlers();
            trackBar_ItemReward_CharacterTokenLevel.Value = BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel;
            label_ItemReward_CharacterTokenLevel.Text = BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel.ToString(CultureInfo.InvariantCulture);
            trackBar_ItemReward_CharacterTokenLevel.MouseEnter += Control_MouseEnter;
            trackBar_ItemReward_CharacterTokenLevel.COnValueChanged += (sender, args) =>
            {
                int Value = (int)((TrackBar)sender).Value;
                BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel = Value;
                label_ItemReward_CharacterTokenLevel.Text = Value.ToString();
            };

        }

        private void initalizecontrols_workorders()
        {
            checkBox_Behavior_WorkOrderPickup.ClearHandlers();
            checkBox_Behavior_WorkOrderPickup.Checked = BaseSettings.CurrentSettings.BehaviorWorkOrderPickup;
            checkBox_Behavior_WorkOrderPickup.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.BehaviorWorkOrderPickup = !BaseSettings.CurrentSettings.BehaviorWorkOrderPickup;

            checkBox_Behavior_WorkOrderStart.ClearHandlers();
            checkBox_Behavior_WorkOrderStart.Checked = BaseSettings.CurrentSettings.BehaviorWorkOrderStartup;
            checkBox_Behavior_WorkOrderStart.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.BehaviorWorkOrderStartup = !BaseSettings.CurrentSettings.BehaviorWorkOrderStartup;

            #region WorkOrders

            flowLayoutPanel_WorkOrderTypes.Controls.Clear();

            Func<object, string> fRetrieveWorkOrderTypesName = s => Enum.GetName(typeof(WorkOrderType), s);
            bool noWorkOrderTypeFlag = BaseSettings.CurrentSettings.WorkOrderTypes.Equals(WorkOrderType.None);
            foreach (var value in Enum.GetValues(typeof(WorkOrderType)))
            {
                WorkOrderType enumValue = (WorkOrderType)value;
                if (enumValue.Equals(WorkOrderType.None) || enumValue.Equals(WorkOrderType.All) || enumValue.Equals(WorkOrderType.DwarvenBunker)) continue;

                UserControl_CheckBox newCheckBox = new UserControl_CheckBox
                {
                    Name = fRetrieveWorkOrderTypesName(value),
                    Text = fRetrieveWorkOrderTypesName(value),
                    Size = checkBox_Behavior_WorkOrderStart.Size,
                    Font = checkBox_Behavior_WorkOrderStart.Font,
                    Checked = !noWorkOrderTypeFlag && BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(enumValue),
                };
                newCheckBox.CCheckedChanged +=checkBox_WorkOrderType_Checked;

                flowLayoutPanel_WorkOrderTypes.Controls.Add(newCheckBox);
            }

            #endregion

            #region Trade Post

            flowLayoutPanel_TradePostReagents.Controls.Clear();
            Func<object, string> fRetrieveTradePostReagentTypesName = s => Enum.GetName(typeof(WorkOrder.TradePostReagentTypes), s);
            bool noTradePostReagentFlags = BaseSettings.CurrentSettings.TradePostReagents.Equals(WorkOrder.TradePostReagentTypes.None);
            foreach (var value in Enum.GetValues(typeof(WorkOrder.TradePostReagentTypes)))
            {
                WorkOrder.TradePostReagentTypes reagentTypes = (WorkOrder.TradePostReagentTypes)value;
                if (reagentTypes.Equals(WorkOrder.TradePostReagentTypes.None) || reagentTypes.Equals(WorkOrder.TradePostReagentTypes.All)) continue;

                UserControl_CheckBox newCheckBox = new UserControl_CheckBox
                {
                    Name = fRetrieveTradePostReagentTypesName(value),
                    Text = fRetrieveTradePostReagentTypesName(value),
                    Checked = !noTradePostReagentFlags && BaseSettings.CurrentSettings.TradePostReagents.HasFlag(reagentTypes),
                };
                newCheckBox.CCheckedChanged +=checkBox_TradePostReagent_Checked;

                flowLayoutPanel_TradePostReagents.Controls.Add(newCheckBox);
            }

            #endregion

            #region Barn
            checkBox_Barn_Furs.ClearHandlers();
            checkBox_Barn_Furs.Checked = BaseSettings.CurrentSettings.BarnWorkOrderFur;
            checkBox_Barn_Furs.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.BarnWorkOrderFur = !BaseSettings.CurrentSettings.BarnWorkOrderFur;

            checkBox_Barn_Leather.ClearHandlers();
            checkBox_Barn_Leather.Checked = BaseSettings.CurrentSettings.BarnWorkOrderLeather;
            checkBox_Barn_Leather.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.BarnWorkOrderLeather = !BaseSettings.CurrentSettings.BarnWorkOrderLeather;

            checkBox_Barn_Meat.ClearHandlers();
            checkBox_Barn_Meat.Checked = BaseSettings.CurrentSettings.BarnWorkOrderMeat;
            checkBox_Barn_Meat.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.BarnWorkOrderMeat = !BaseSettings.CurrentSettings.BarnWorkOrderMeat;

            checkBox_Trapping_NonElite_Fur.ClearHandlers();
            checkBox_Trapping_NonElite_Fur.Checked = BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.Enabled;
            checkBox_Trapping_NonElite_Fur.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.Enabled =
                        !BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.Enabled;

            textBox_Trapping_NonElite_Fur.ClearHandlers();
            textBox_Trapping_NonElite_Fur.Text = BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.MaximumItemCount.ToString(CultureInfo.InvariantCulture);
            textBox_Trapping_NonElite_Fur.COnKeyPressed += textBox_Trapping_KeyPress;
            textBox_Trapping_NonElite_Fur.COnTextChanged += (sender, args) =>
            {
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.MaximumItemCount =
                    GetTextBoxIntValue((TextBox)sender);
            };
            //TrapSettings_NonElite_Fur

            checkBox_Trapping_Elite_Fur.ClearHandlers();
            checkBox_Trapping_Elite_Fur.Checked = BaseSettings.CurrentSettings.TrapSettings_Elite_Fur.Enabled;
            checkBox_Trapping_Elite_Fur.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.TrapSettings_Elite_Fur.Enabled =
                        !BaseSettings.CurrentSettings.TrapSettings_Elite_Fur.Enabled;

            textBox_Trapping_Elite_Fur.ClearHandlers();
            textBox_Trapping_Elite_Fur.Text = BaseSettings.CurrentSettings.TrapSettings_Elite_Fur.MaximumItemCount.ToString(CultureInfo.InvariantCulture);
            textBox_Trapping_Elite_Fur.COnKeyPressed += textBox_Trapping_KeyPress;
            textBox_Trapping_Elite_Fur.COnTextChanged += (sender, args) =>
            {
                BaseSettings.CurrentSettings.TrapSettings_Elite_Fur.MaximumItemCount =
                    GetTextBoxIntValue((TextBox)sender);
            };
            //TrapSettings_Elite_Fur

            checkBox_Trapping_NonElite_Leather.ClearHandlers();
            checkBox_Trapping_NonElite_Leather.Checked = BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.Enabled;
            checkBox_Trapping_NonElite_Leather.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.Enabled =
                        !BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.Enabled;

            textBox_Trapping_NonElite_Leather.ClearHandlers();
            textBox_Trapping_NonElite_Leather.Text = BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.MaximumItemCount.ToString(CultureInfo.InvariantCulture);
            textBox_Trapping_NonElite_Leather.COnKeyPressed += textBox_Trapping_KeyPress;
            textBox_Trapping_NonElite_Leather.COnTextChanged += (sender, args) =>
            {
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.MaximumItemCount =
                    GetTextBoxIntValue((TextBox)sender);
            };
            //TrapSettings_NonElite_Leather

            checkBox_Trapping_Elite_Leather.ClearHandlers();
            checkBox_Trapping_Elite_Leather.Checked = BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.Enabled;
            checkBox_Trapping_Elite_Leather.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.Enabled =
                        !BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.Enabled;

            textBox_Trapping_Elite_Leather.ClearHandlers();
            textBox_Trapping_Elite_Leather.Text = BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.MaximumItemCount.ToString(CultureInfo.InvariantCulture);
            textBox_Trapping_Elite_Leather.COnKeyPressed += textBox_Trapping_KeyPress;
            textBox_Trapping_Elite_Leather.COnTextChanged += (sender, args) =>
            {
                BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.MaximumItemCount =
                    GetTextBoxIntValue((TextBox)sender);
            };
            //TrapSettings_Elite_Leather

            checkBox_Trapping_NonElite_Meat.ClearHandlers();
            checkBox_Trapping_NonElite_Meat.Checked = BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.Enabled;
            checkBox_Trapping_NonElite_Meat.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.Enabled =
                        !BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.Enabled;

            textBox_Trapping_NonElite_Meat.ClearHandlers();
            textBox_Trapping_NonElite_Meat.Text = BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.MaximumItemCount.ToString(CultureInfo.InvariantCulture);
            textBox_Trapping_NonElite_Meat.COnKeyPressed += textBox_Trapping_KeyPress;
            textBox_Trapping_NonElite_Meat.COnTextChanged += (sender, args) =>
            {
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.MaximumItemCount =
                    GetTextBoxIntValue((TextBox)sender);
            };
            //TrapSettings_NonElite_Meat

            checkBox_Trapping_Elite_Meat.ClearHandlers();
            checkBox_Trapping_Elite_Meat.Checked = BaseSettings.CurrentSettings.TrapSettings_Elite_Meat.Enabled;
            checkBox_Trapping_Elite_Meat.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.TrapSettings_Elite_Meat.Enabled =
                        !BaseSettings.CurrentSettings.TrapSettings_Elite_Meat.Enabled;

            textBox_Trapping_Elite_Meat.ClearHandlers();
            textBox_Trapping_Elite_Meat.Text = BaseSettings.CurrentSettings.TrapSettings_Elite_Meat.MaximumItemCount.ToString(CultureInfo.InvariantCulture);
            textBox_Trapping_Elite_Meat.COnKeyPressed += textBox_Trapping_KeyPress;
            textBox_Trapping_Elite_Meat.COnTextChanged += (sender, args) =>
            {
                BaseSettings.CurrentSettings.TrapSettings_Elite_Meat.MaximumItemCount =
                    GetTextBoxIntValue((TextBox)sender);
            };
            //TrapSettings_Elite_Meat
            #endregion
        }

        private void initalizecontrols_professions()
        {
            checkBox_Behavior_Professions.ClearHandlers();
            checkBox_Behavior_Professions.Checked = BaseSettings.CurrentSettings.BehaviorProfessions;
            checkBox_Behavior_Professions.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.BehaviorProfessions = !BaseSettings.CurrentSettings.BehaviorProfessions;


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
                    UserControl_CheckBox newCheckBox = new UserControl_CheckBox
                    {
                        Name = i.ToString(),
                        Text = PlayerProfessions.GetProfessionCraftingName(i),
                        Checked = BaseSettings.CurrentSettings.ProfessionSpellIds.Contains(i),
                        Font = new Font(FontFamily.GenericSansSerif, 9.5f, FontStyle.Regular),
                        AutoSize = true,

                    };
                    newCheckBox.CCheckedChanged +=checkBox_ProfessionSpellId_Checked;
                    layoutPanelWrapper.Controls.Add(newCheckBox);
                }

                switch (value.Key)
                {
                    case SkillLine.Alchemy:
                        tabPage_Alchemy.Controls.Clear();
                        tabPage_Alchemy.Controls.Add(layoutPanelWrapper);
                        break;
                    case SkillLine.Blacksmithing:
                        tabPage_Blacksmithing.Controls.Clear();
                        tabPage_Blacksmithing.Controls.Add(layoutPanelWrapper);
                        break;
                    case SkillLine.Enchanting:
                        tabPage_Enchanting.Controls.Clear();
                        tabPage_Enchanting.Controls.Add(layoutPanelWrapper);
                        break;
                    case SkillLine.Engineering:
                        tabPage_Engineering.Controls.Clear();
                        tabPage_Engineering.Controls.Add(layoutPanelWrapper);
                        break;
                    case SkillLine.Inscription:
                        panel_Professions_Inscripition.Controls.Clear();
                        panel_Professions_Inscripition.Controls.Add(layoutPanelWrapper);
                        break;
                    case SkillLine.Jewelcrafting:
                        tabPage_Jewelcrafting.Controls.Clear();
                        tabPage_Jewelcrafting.Controls.Add(layoutPanelWrapper);
                        break;
                    case SkillLine.Leatherworking:
                        tabPage_Leatherworking.Controls.Clear();
                        tabPage_Leatherworking.Controls.Add(layoutPanelWrapper);
                        break;
                    case SkillLine.Tailoring:
                        tabPage_Tailoring.Controls.Clear();
                        tabPage_Tailoring.Controls.Add(layoutPanelWrapper);
                        break;
                }
            }

           

            #endregion

            #region Milling Settings

            //Milling (Inscription)
            checkBox_Milling_Enabled.ClearHandlers();
            checkBox_Milling_Enabled.Checked = BaseSettings.CurrentSettings.MillingEnabled;
            checkBox_Milling_Enabled.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MillingEnabled = !BaseSettings.CurrentSettings.MillingEnabled;

            textBox_Milling_RequiredAmount.ClearHandlers();
            textBox_Milling_RequiredAmount.Text = BaseSettings.CurrentSettings.MillingMinimum.ToString();
            textBox_Milling_RequiredAmount.COnKeyPressed += textBox_Numbers_KeyPress;
            textBox_Milling_RequiredAmount.COnTextChanged += textBox_Milling_Minimum_TextedChanged;

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

                UserControl_CheckBox newCheckBox = new UserControl_CheckBox
                {
                    Name = "millingenabled_" + id.ToString(),
                    Text = name,
                    Checked = !millingSetting.Ignored,
                    Font = new Font(FontFamily.GenericSansSerif, 9.5f, FontStyle.Regular),
                    Size = new Size(190, 25),
                    Padding = new Padding(0, 0, 25, 0),
                };
                newCheckBox.CCheckedChanged +=checkBox_MillingHerb_Checked;
                flowLayoutPanel_Milling.Controls.Add(newCheckBox);

                UserControl_Textbox newTextBox = new UserControl_Textbox
                {
                    Name = "millingenabled_" + id.ToString(),
                    Text = millingSetting.Reserved.ToString(),
                    Font = new Font(FontFamily.GenericSansSerif, 9.5f, FontStyle.Regular),
                    Size = new Size(50, 25),
                };
                newTextBox.COnTextChanged += textBox_MillingHerb_TextChanged;
                newTextBox.COnKeyPressed += textBox_Numbers_KeyPress;
                flowLayoutPanel_Milling.SetFlowBreak(newTextBox, true);
                flowLayoutPanel_Milling.Controls.Add(newTextBox);
            }

            #endregion

        }

        private void initalizecontrols_mail()
        {
            checkBox_MailAutoGet.ClearHandlers();
            checkBox_MailAutoGet.Checked = BaseSettings.CurrentSettings.MailAutoGet;
            checkBox_MailAutoGet.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailAutoGet = !BaseSettings.CurrentSettings.MailAutoGet;

            checkBox_MailAutoSend.ClearHandlers();
            checkBox_MailAutoSend.Checked = BaseSettings.CurrentSettings.MailAutoSend;
            checkBox_MailAutoSend.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailAutoSend = !BaseSettings.CurrentSettings.MailAutoSend;

            checkBox_MailEnchanting.ClearHandlers();
            checkBox_MailEnchanting.Checked = BaseSettings.CurrentSettings.MailSendEnchanting;
            checkBox_MailEnchanting.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailSendEnchanting = !BaseSettings.CurrentSettings.MailSendEnchanting; ;

            textBox_MailEnchanting.ClearHandlers();
            textBox_MailEnchanting.Text = BaseSettings.CurrentSettings.MailSendEnchantingRecipient;
            textBox_MailEnchanting.COnTextChanged += (sender, args) => BaseSettings.CurrentSettings.MailSendEnchantingRecipient = textBox_MailEnchanting.Text;  

            checkBox_MailEpic.ClearHandlers();
            checkBox_MailEpic.Checked = BaseSettings.CurrentSettings.MailSendEpic;
            checkBox_MailEpic.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailSendEpic = !BaseSettings.CurrentSettings.MailSendEpic;

            textBox_MailEpic.ClearHandlers();
            textBox_MailEpic.Text = BaseSettings.CurrentSettings.MailSendEpicRecipient;
            textBox_MailEpic.COnTextChanged += (sender, args) => BaseSettings.CurrentSettings.MailSendEpicRecipient = textBox_MailEpic.Text;

            checkBox_MailHerbs.ClearHandlers();
            checkBox_MailHerbs.Checked = BaseSettings.CurrentSettings.MailSendHerbs;
            checkBox_MailHerbs.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailSendHerbs = !BaseSettings.CurrentSettings.MailSendHerbs;

            textBox_MailHerbs.ClearHandlers();
            textBox_MailHerbs.Text = BaseSettings.CurrentSettings.MailSendHerbsRecipient;
            textBox_MailHerbs.COnTextChanged += (sender, args) => BaseSettings.CurrentSettings.MailSendHerbsRecipient = textBox_MailHerbs.Text;

            checkBox_MailOre.ClearHandlers();
            checkBox_MailOre.Checked = BaseSettings.CurrentSettings.MailSendOre;
            checkBox_MailOre.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailSendOre = !BaseSettings.CurrentSettings.MailSendOre;

            textBox_MailOre.ClearHandlers();
            textBox_MailOre.Text = BaseSettings.CurrentSettings.MailSendOreRecipient;
            textBox_MailOre.COnTextChanged += (sender, args) => BaseSettings.CurrentSettings.MailSendOreRecipient = textBox_MailOre.Text;

            checkBox_MailRare.ClearHandlers();
            checkBox_MailRare.Checked = BaseSettings.CurrentSettings.MailSendRare;
            checkBox_MailRare.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailSendRare = !BaseSettings.CurrentSettings.MailSendRare;

            textBox_MailRare.ClearHandlers();
            textBox_MailRare.Text = BaseSettings.CurrentSettings.MailSendRareRecipient;
            textBox_MailRare.COnTextChanged += (sender, args) => BaseSettings.CurrentSettings.MailSendRareRecipient = textBox_MailRare.Text;

            checkBox_MailUncommon.ClearHandlers();
            checkBox_MailUncommon.Checked = BaseSettings.CurrentSettings.MailSendUncommon;
            checkBox_MailUncommon.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.MailSendUncommon = !BaseSettings.CurrentSettings.MailSendUncommon;

            textBox_MailUncommon.ClearHandlers();
            textBox_MailUncommon.Text = BaseSettings.CurrentSettings.MailSendUncommonRecipient;
            textBox_MailUncommon.COnTextChanged += (sender, args) => BaseSettings.CurrentSettings.MailSendUncommonRecipient = textBox_MailUncommon.Text;

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

        }

        private void initalizecontrols_misc()
        {
            #region Misc Behaviors

            checkBox_HBRelogSkipTask.ClearHandlers();
            checkBox_HBRelogSkipTask.Checked = BaseSettings.CurrentSettings.HBRelog_SkipToNextTask;
            checkBox_HBRelogSkipTask.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.HBRelog_SkipToNextTask =
                        !BaseSettings.CurrentSettings.HBRelog_SkipToNextTask;

            checkBox_Behavior_LootCache.ClearHandlers();
            checkBox_Behavior_LootCache.Checked = BaseSettings.CurrentSettings.BehaviorLootCache;
            checkBox_Behavior_LootCache.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.BehaviorLootCache = !BaseSettings.CurrentSettings.BehaviorLootCache;

            checkBox_Behavior_HerbGather.ClearHandlers();
            checkBox_Behavior_HerbGather.Checked = BaseSettings.CurrentSettings.BehaviorHerbGather;
            checkBox_Behavior_HerbGather.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.BehaviorHerbGather = !BaseSettings.CurrentSettings.BehaviorHerbGather;

            checkBox_Behavior_MineGather.ClearHandlers();
            checkBox_Behavior_MineGather.Checked = BaseSettings.CurrentSettings.BehaviorMineGather;
            checkBox_Behavior_MineGather.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.BehaviorMineGather = !BaseSettings.CurrentSettings.BehaviorMineGather;

            checkBox_Behavior_Quests.ClearHandlers();
            checkBox_Behavior_Quests.Checked = BaseSettings.CurrentSettings.BehaviorQuests;
            checkBox_Behavior_Quests.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.BehaviorQuests = !BaseSettings.CurrentSettings.BehaviorQuests;


            checkBox_LootAnything.ClearHandlers();
            checkBox_LootAnything.Checked = BaseSettings.CurrentSettings.LootAnyMobs;
            checkBox_LootAnything.CCheckedChanged +=
                (sender, args) => BaseSettings.CurrentSettings.LootAnyMobs = !BaseSettings.CurrentSettings.LootAnyMobs;


            checkBox_Behavior_Salvaging.ClearHandlers();
            checkBox_Behavior_Salvaging.Checked = BaseSettings.CurrentSettings.BehaviorSalvaging;
            checkBox_Behavior_Salvaging.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.BehaviorSalvaging = !BaseSettings.CurrentSettings.BehaviorSalvaging;

            checkBox_IgnoreBankItems.ClearHandlers();
            checkBox_IgnoreBankItems.Checked = BaseSettings.CurrentSettings.IgnoreBankItems;
            checkBox_IgnoreBankItems.CCheckedChanged +=
               (sender, args) =>
                   BaseSettings.CurrentSettings.IgnoreBankItems = !BaseSettings.CurrentSettings.IgnoreBankItems;

            checkBox_IgnoreReagentBankItems.ClearHandlers();
            checkBox_IgnoreReagentBankItems.Checked = BaseSettings.CurrentSettings.IgnoreReagentBankItems;
            checkBox_IgnoreReagentBankItems.CCheckedChanged +=
               (sender, args) =>
                   BaseSettings.CurrentSettings.IgnoreReagentBankItems = !BaseSettings.CurrentSettings.IgnoreReagentBankItems;

            #endregion

            #region Vendoring

            checkBox_Behavior_RepairSell.ClearHandlers();
            checkBox_Behavior_RepairSell.Checked = BaseSettings.CurrentSettings.BehaviorRepairSell;
            checkBox_Behavior_RepairSell.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.BehaviorRepairSell = !BaseSettings.CurrentSettings.BehaviorRepairSell;

            checkBox_Vendor_Junk.ClearHandlers();
            checkBox_Vendor_Junk.Checked = BaseSettings.CurrentSettings.VendorJunkItems;
            checkBox_Vendor_Junk.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.VendorJunkItems = !BaseSettings.CurrentSettings.VendorJunkItems;

            checkBox_Vendor_Common.ClearHandlers();
            checkBox_Vendor_Common.Checked = BaseSettings.CurrentSettings.VendorCommonItems;
            checkBox_Vendor_Common.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.VendorCommonItems = !BaseSettings.CurrentSettings.VendorCommonItems;

            checkBox_Vendor_Uncommon.ClearHandlers();
            checkBox_Vendor_Uncommon.Checked = BaseSettings.CurrentSettings.VendorUncommonItems;
            checkBox_Vendor_Uncommon.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.VendorUncommonItems = !BaseSettings.CurrentSettings.VendorUncommonItems;

            checkBox_Vendor_Rare.ClearHandlers();
            checkBox_Vendor_Rare.Checked = BaseSettings.CurrentSettings.VendorRareItems;
            checkBox_Vendor_Rare.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.VendorRareItems = !BaseSettings.CurrentSettings.VendorRareItems;

            trackBar_MinimumBagSlotsFree.ClearHandlers();
            trackBar_MinimumBagSlotsFree.Value = BaseSettings.CurrentSettings.MinimumBagSlotsFree;
            textBox_MinimumBagSlotsFree.Text = BaseSettings.CurrentSettings.MinimumBagSlotsFree.ToString();
            trackBar_MinimumBagSlotsFree.COnValueChanged += (sender, args) =>
            {
                int Value = (int) ((TrackBar) sender).Value;
                BaseSettings.CurrentSettings.MinimumBagSlotsFree = Value;
                textBox_MinimumBagSlotsFree.Text = Value.ToString();
            };

            #endregion

            #region Disenchanting

            checkBox_Behavior_Disenchant.ClearHandlers();
            checkBox_Behavior_Disenchant.Checked = BaseSettings.CurrentSettings.BehaviorDisenchanting;
            checkBox_Behavior_Disenchant.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.BehaviorDisenchanting =
                        !BaseSettings.CurrentSettings.BehaviorDisenchanting;

            checkBox_Disenchanting_ProfessionSkillDisenchanting.ClearHandlers();
            checkBox_Disenchanting_ProfessionSkillDisenchanting.Checked = BaseSettings.CurrentSettings.DisenchantingProfessionSkill;
            checkBox_Disenchanting_ProfessionSkillDisenchanting.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.DisenchantingProfessionSkill =
                        !BaseSettings.CurrentSettings.DisenchantingProfessionSkill;

            checkBox_Disenchanting_UncommonItems.ClearHandlers();
            checkBox_Disenchanting_UncommonItems.Checked = BaseSettings.CurrentSettings.DisenchantingUncommon;
            checkBox_Disenchanting_UncommonItems.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.DisenchantingUncommon =
                        !BaseSettings.CurrentSettings.DisenchantingUncommon;

            checkBox_Disenchanting_Epic.ClearHandlers();
            checkBox_Disenchanting_Epic.Checked = BaseSettings.CurrentSettings.DisenchantingEpic;
            checkBox_Disenchanting_Epic.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.DisenchantingEpic = !BaseSettings.CurrentSettings.DisenchantingEpic;

            checkBox_Disenchanting_RareItems.ClearHandlers();
            checkBox_Disenchanting_RareItems.Checked = BaseSettings.CurrentSettings.DisenchantingRare;
            checkBox_Disenchanting_RareItems.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.DisenchantingRare = !BaseSettings.CurrentSettings.DisenchantingRare;

            checkBox_Disenchanting_UncommonSoulbound.ClearHandlers();
            checkBox_Disenchanting_UncommonSoulbound.Checked =
                BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded;
            checkBox_Disenchanting_UncommonSoulbound.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded =
                        !BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded;

            checkBox_Disenchanting_RareSoulbound.ClearHandlers();
            checkBox_Disenchanting_RareSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingRareSoulbounded;
            checkBox_Disenchanting_RareSoulbound.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.DisenchantingRareSoulbounded =
                        !BaseSettings.CurrentSettings.DisenchantingRareSoulbounded;

            checkBox_Disenchanting_EpicSoulbound.ClearHandlers();
            checkBox_Disenchanting_EpicSoulbound.Checked = BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded;
            checkBox_Disenchanting_EpicSoulbound.CCheckedChanged +=
                (sender, args) =>
                    BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded =
                        !BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded;

            textBox_Disenchanting_UncommonLevel.ClearHandlers();
            textBox_Disenchanting_UncommonLevel.Text =
                BaseSettings.CurrentSettings.DisenchantingUncommonItemLevel.ToString();
            textBox_Disenchanting_UncommonLevel.COnKeyPressed += textBox_Numbers_KeyPress;
            textBox_Disenchanting_UncommonLevel.COnTextChanged += textBox_Disenchanting_UncommonLevel_TextedChanged;

            textBox_Disenchanting_RareLevel.ClearHandlers();
            textBox_Disenchanting_RareLevel.Text = BaseSettings.CurrentSettings.DisenchantingRareItemLevel.ToString();
            textBox_Disenchanting_RareLevel.COnKeyPressed += textBox_Numbers_KeyPress;
            textBox_Disenchanting_RareLevel.COnTextChanged += textBox_Disenchanting_RareLevel_TextedChanged;

            textBox_Disenchanting_EpicLevel.ClearHandlers();
            textBox_Disenchanting_EpicLevel.Text = BaseSettings.CurrentSettings.DisenchantingEpicItemLevel.ToString();
            textBox_Disenchanting_EpicLevel.COnKeyPressed += textBox_Numbers_KeyPress;
            textBox_Disenchanting_EpicLevel.COnTextChanged += textBox_Disenchanting_EpicLevel_TextedChanged;


            #endregion

            #region Primal Exchange

            comboBox_PrimalSpiritItems.ClearHandlers();
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
            comboBox_PrimalSpiritItems.COnSelectedIndexChanged += comboBox_PrimalSpiritItems_SelectedIndexChanged;

            checkBox_ExchangePrimalSpirits.ClearHandlers();
            checkBox_ExchangePrimalSpirits.Checked = BaseSettings.CurrentSettings.ExchangePrimalSpirits;
            checkBox_ExchangePrimalSpirits.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.ExchangePrimalSpirits = !BaseSettings.CurrentSettings.ExchangePrimalSpirits;


            #endregion

            #region Followers
            checkBox_Follower_170.ClearHandlers();
            checkBox_Follower_189.ClearHandlers();
            checkBox_Follower_190.ClearHandlers();
            checkBox_Follower_193.ClearHandlers();
            checkBox_Follower_207.ClearHandlers();
            checkBox_Follower_467.ClearHandlers();
            checkBox_Follower_209.ClearHandlers();
            checkBox_Follower_32.ClearHandlers();

            checkBox_Follower_170.Name = "170";
            checkBox_Follower_189.Name = "189";
            checkBox_Follower_190.Name = "190";
            checkBox_Follower_193.Name = "193";
            checkBox_Follower_207.Name = "207";
            checkBox_Follower_467.Name = "467";
            checkBox_Follower_209.Name = "209";
            checkBox_Follower_32.Name = "32";
            checkBox_Follower_170.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(170);
            checkBox_Follower_189.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(189);
            checkBox_Follower_190.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(190);
            checkBox_Follower_193.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(193);
            checkBox_Follower_207.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(207);
            checkBox_Follower_467.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(467);
            checkBox_Follower_209.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(209);
            checkBox_Follower_32.Checked = BaseSettings.CurrentSettings.FollowerOptionalList.Contains(32);
            checkBox_Follower_170.CCheckedChanged +=checkBox_Follower_CheckedChanged;
            checkBox_Follower_189.CCheckedChanged +=checkBox_Follower_CheckedChanged;
            checkBox_Follower_190.CCheckedChanged +=checkBox_Follower_CheckedChanged;
            checkBox_Follower_193.CCheckedChanged +=checkBox_Follower_CheckedChanged;
            checkBox_Follower_207.CCheckedChanged +=checkBox_Follower_CheckedChanged;
            checkBox_Follower_467.CCheckedChanged +=checkBox_Follower_CheckedChanged;
            checkBox_Follower_209.CCheckedChanged +=checkBox_Follower_CheckedChanged;
            checkBox_Follower_32.CCheckedChanged +=checkBox_Follower_CheckedChanged;

            #endregion

            #region Daily Quests
            checkBox_DailyQuest_Warmill.ClearHandlers();
            checkBox_DailyQuest_Warmill.Checked = BaseSettings.CurrentSettings.DailyWarMillQuestSettings.Enabled;
            checkBox_DailyQuest_Warmill.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.DailyWarMillQuestSettings.Enabled = !BaseSettings.CurrentSettings.DailyWarMillQuestSettings.Enabled;

            comboBox_DailyQuest_WarMill_Rewards.ClearHandlers();
            if (BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex > -1)
            {
                comboBox_DailyQuest_WarMill_Rewards.SelectedIndex =
                    BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex;
            }
            comboBox_DailyQuest_WarMill_Rewards.COnSelectedIndexChanged += (sender, args) =>
            {
                ComboBox senderCB = (ComboBox)sender;
                if (senderCB.SelectedIndex > -1)
                {
                    BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex = senderCB.SelectedIndex;
                }
            };

            checkBox_DailyQuest_AlchemyLab.ClearHandlers();
            checkBox_DailyQuest_AlchemyLab.Checked = BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.Enabled;
            checkBox_DailyQuest_AlchemyLab.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.Enabled = !BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.Enabled;

            comboBox_DailyQuest_Alchemy_Rewards.ClearHandlers();
            if (BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex > -1)
            {
                comboBox_DailyQuest_Alchemy_Rewards.SelectedIndex =
                    BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex;
            }
            comboBox_DailyQuest_Alchemy_Rewards.COnSelectedIndexChanged += (sender, args) =>
            {
                ComboBox senderCB = (ComboBox)sender;
                if (senderCB.SelectedIndex > -1)
                {
                    BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex = senderCB.SelectedIndex;
                }
            };

            #endregion



        }

        private void initalizecontrols_debug()
        {
            checkBox_Debug_FakeStartWorkOrder.ClearHandlers();
            checkBox_Debug_FakeStartWorkOrder.Checked = BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER;
            checkBox_Debug_FakeStartWorkOrder.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER = !BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER;

            checkBox_Debug_FakeFinishQuest.ClearHandlers();
            checkBox_Debug_FakeFinishQuest.Checked = BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST;
            checkBox_Debug_FakeFinishQuest.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST = !BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST;

            checkBox_Debug_IgnoreHearthStone.ClearHandlers();
            checkBox_Debug_IgnoreHearthStone.Checked = BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE;
            checkBox_Debug_IgnoreHearthStone.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE = !BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE;

            checkBox_Debug_FakePickupWorkOrder.ClearHandlers();
            checkBox_Debug_FakePickupWorkOrder.Checked = BaseSettings.CurrentSettings.DEBUG_FAKEPICKUPWORKORDER;
            checkBox_Debug_FakePickupWorkOrder.CCheckedChanged +=(sender, args) => BaseSettings.CurrentSettings.DEBUG_FAKEPICKUPWORKORDER = !BaseSettings.CurrentSettings.DEBUG_FAKEPICKUPWORKORDER;
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

        private int GetTextBoxIntValue(TextBox textbox)
        {
            var value = textbox.Text.Trim();
            if (string.IsNullOrEmpty(value)) value = "0";
            return Convert.ToInt32(value);
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
        private void textBox_Trapping_KeyPress(object sender, KeyPressEventArgs e)
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
                if (currentValue > 100)
                {//Exceedes are maximum value.. set to max!
                    senderTextBox.Text = "100";
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



        private void Control_MouseEnter(object sender, EventArgs e)
        {
            ((Control) sender).Focus();
        }

        private void defaultSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogBox newitemform = new DialogBox("Are you sure you want to reset settings to default?", "Default Settings");
            newitemform.ShowDialog();

            if (newitemform.Result)
            {
                BaseSettings.CurrentSettings = new BaseSettings();
                BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
                InitalizeControls();
            }
        }

        private void dumpObjectsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                var Objects = Cache.ObjectCacheManager.ObjectCollection.Values.OrderByDescending(o => o.SubType).ThenBy(o => o.Distance).ToList();
                foreach (var cWoWObject in Objects)
                {
                    Color foreColor =
                        TargetManager.CombatObject != null && TargetManager.CombatObject.Equals(cWoWObject) ? Color.GhostWhite :
                        TargetManager.LootableObject != null && TargetManager.LootableObject.Equals(cWoWObject) ? Color.Black :
                        (cWoWObject is C_WoWUnit) ? Color.Black
                        : Color.GhostWhite;

                    Color backColor =
                                TargetManager.CombatObject != null && TargetManager.CombatObject.Equals(cWoWObject) ? Color.DarkRed :
                                 TargetManager.LootableObject != null && TargetManager.LootableObject.Equals(cWoWObject) ? Color.Gold :
                                (cWoWObject is C_WoWGameObject) ? Color.DarkSlateGray
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

        private void dumpEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                var Objects = Cache.ObjectCacheManager.EntryCache.Values.ToList();
                foreach (var cWoWObject in Objects)
                {
                    string entryString = cWoWObject.ToString();
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

        private void dumpBagsToolStripMenuItem_Click(object sender, EventArgs e)
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
            }
            catch (Exception ex)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
            }

            LBDebug.Focus();
        }

        private void dumpBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
                foreach (var item in Character.Player.Inventory.BankItems)
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

        private void dumpReagentBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            try
            {
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

        private void dumpCurrentBehaviorsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void dumpFollowersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            LBDebug.Controls.Add(new UserControlDebugEntry("Followers Collected", Color.WhiteSmoke, Color.DarkGray));
            foreach (var b in GarrisonManager.Followers.Values)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }

            
            LBDebug.Focus();
        }

        private void dumpBuildingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();

            foreach (var b in GarrisonManager.Buildings.Values)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry(b.ToString()));
            }

            LBDebug.Focus();
        }

        private void dumpMissionsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void dumpQuestLogToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void dumpFlightPathsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void dumpMissionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();
            try
            {
                foreach (var n in GarrisonInfo.Missions)
                {
                    LBDebug.Controls.Add(new UserControlDebugEntry("State: " + n.State + " " + n.ToString()));
                }
            }
            catch (Exception ex)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
            }

            LBDebug.Focus();
        }

        private void dumpBuildingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();
            try
            {
                foreach (var n in GarrisonInfo.OwnedBuildings)
                {
                    var s = String.Format("GarrBuildingId:{0} ID:{1} {2}", n.GarrBuildingId, n.Id, n.ToString());
                    LBDebug.Controls.Add(new UserControlDebugEntry(s));
                }
            }
            catch (Exception ex)
            {
                LBDebug.Controls.Add(new UserControlDebugEntry("End of Output due to Modification Exception"));
            }

            LBDebug.Focus();
        }

        private void dumpFollowersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();
            try
            {
                foreach (var n in GarrisonInfo.Followers)
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

        private void dumpShipmentInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();
            try
            {
                foreach (var n in GarrisonInfo.LandingPageShipmentInfos)
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


        private void professionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LBDebug.Controls.Clear();
            try
            {
                foreach (var n in Player.Professions.ProfessionSkills.Values)
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




    }
}
