using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Herbfunk.GarrisonBase.Cache;
using Styx.Helpers;

namespace Herbfunk.GarrisonBase
{
    public partial class MailAddNewItem : Form
    {
        public MailItem MailItem { get; set; }
        private List<C_WoWItem> bagitems, bankitems, reagentbankitems;
        private List<int> bagitem_Ids = new List<int>();
        private List<int> bankitem_Ids = new List<int>();
        private List<int> reagentbankitem_Ids = new List<int>();
        private List<int> ignoredIds = new List<int>(); 
        public MailAddNewItem()
        {
            InitializeComponent();

            foreach (var mailSendItem in BaseSettings.CurrentSettings.MailSendItems)
            {
                ignoredIds.Add(mailSendItem.EntryId);
            }

            bagitems = new List<C_WoWItem>(Player.Inventory.GetBagItemsBOE()).Where(i => !ignoredIds.Contains((int)i.Entry)).OrderBy(i => i.Name).ToList();
            bankitems = new List<C_WoWItem>(Player.Inventory.GetBankItemsBOE()).Where(i => !ignoredIds.Contains((int)i.Entry)).OrderBy(i => i.Name).ToList();
            reagentbankitems = new List<C_WoWItem>(Player.Inventory.GetReagentBankItemsBOE()).Where(i => !ignoredIds.Contains((int)i.Entry)).OrderBy(i => i.Name).ToList();

            MailItem = null;

            comboBox_BagItems.Items.Clear();
            foreach (var item in bagitems)
            {
                int entryId = (int)item.Entry;
                if (!bagitem_Ids.Contains(entryId))
                {
                    comboBox_BagItems.Items.Add(item.Name);
                    bagitem_Ids.Add(entryId);
                }
            }
            comboBox_BagItems.SelectedIndexChanged += comboBox_BagItems_SelectedIndexChanged;

            comboBox_BankItems.Items.Clear();
            foreach (var item in bankitems)
            {
                int entryId = (int)item.Entry;
                if (!bankitem_Ids.Contains(entryId))
                {
                    comboBox_BankItems.Items.Add(item.Name);
                    bankitem_Ids.Add(entryId);
                }
            }
            comboBox_BankItems.SelectedIndexChanged += comboBox_BankItems_SelectedIndexChanged;

            comboBox_ReagentBankItems.Items.Clear();
            foreach (var item in reagentbankitems)
            {
                int entryId = (int)item.Entry;
                if (!reagentbankitem_Ids.Contains(entryId))
                {
                    comboBox_ReagentBankItems.Items.Add(item.Name);
                    reagentbankitem_Ids.Add(entryId);
                }
            }
            comboBox_ReagentBankItems.SelectedIndexChanged += comboBox_ReagentBankItems_SelectedIndexChanged;

            comboBox_CraftingItems.Items.Clear();
            foreach (var name in Enum.GetNames(typeof(CraftingReagents)))
            {
                comboBox_CraftingItems.Items.Add(name);
            }
            comboBox_CraftingItems.SelectedIndexChanged += comboBox_CraftingItems_SelectedIndexChanged;
        }
        private void comboBox_CraftingItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_CraftingItems.SelectedIndex >= 0)
            {
                comboBox_BagItems.SelectedItem = String.Empty;
                comboBox_BagItems.SelectedIndex = -1;
                comboBox_BankItems.SelectedItem = string.Empty;
                comboBox_BankItems.SelectedIndex = -1;
                comboBox_ReagentBankItems.SelectedItem = string.Empty;
                comboBox_ReagentBankItems.SelectedIndex = -1;

                var value = (int)Enum.Parse(typeof (CraftingReagents), comboBox_CraftingItems.SelectedItem.ToString());
                textBox_EntryId.Text = value.ToString();
                textBox_Count.Text = "0";
            }
        }

        private void comboBox_ReagentBankItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_ReagentBankItems.SelectedIndex >= 0)
            {
                comboBox_BagItems.SelectedItem = String.Empty;
                comboBox_BagItems.SelectedIndex = -1;
                comboBox_BankItems.SelectedItem = string.Empty;
                comboBox_BankItems.SelectedIndex = -1;
                comboBox_CraftingItems.SelectedItem = string.Empty;
                comboBox_CraftingItems.SelectedIndex = -1;

                var item = reagentbankitem_Ids[comboBox_ReagentBankItems.SelectedIndex];
                textBox_EntryId.Text = item.ToString();
                textBox_Count.Text = "0";
            }
        }
        private void comboBox_BankItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BankItems.SelectedIndex >= 0)
            {
                comboBox_BagItems.SelectedItem = String.Empty;
                comboBox_BagItems.SelectedIndex = -1;
                comboBox_ReagentBankItems.SelectedItem = string.Empty;
                comboBox_ReagentBankItems.SelectedIndex = -1;
                comboBox_CraftingItems.SelectedItem = string.Empty;
                comboBox_CraftingItems.SelectedIndex = -1;

                var item = bankitem_Ids[comboBox_BankItems.SelectedIndex];
                textBox_EntryId.Text = item.ToString();
                textBox_Count.Text = "0";
            }
        }
        private void comboBox_BagItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BagItems.SelectedIndex >= 0)
            {
                comboBox_BankItems.SelectedItem = String.Empty;
                comboBox_BankItems.SelectedIndex = -1;
                comboBox_ReagentBankItems.SelectedItem = string.Empty;
                comboBox_ReagentBankItems.SelectedIndex = -1;
                comboBox_CraftingItems.SelectedItem = string.Empty;
                comboBox_CraftingItems.SelectedIndex = -1;

                var item = bagitem_Ids[comboBox_BagItems.SelectedIndex];
                textBox_EntryId.Text = item.ToString();
                textBox_Count.Text = "0";
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (comboBox_BagItems.SelectedIndex >= 0)
            {
                MailItem = new MailItem((int)bagitem_Ids[comboBox_BagItems.SelectedIndex],
                    comboBox_BagItems.SelectedItem.ToString(), textBox_Recipient.Text, textBox_Count.Text.ToInt32());
                Close();
            }
            else if (comboBox_BankItems.SelectedIndex >= 0)
            {
                MailItem = new MailItem((int)bankitem_Ids[comboBox_BankItems.SelectedIndex],
                        comboBox_BankItems.SelectedItem.ToString(), textBox_Recipient.Text, textBox_Count.Text.ToInt32());
                Close();
            }
            else if (comboBox_ReagentBankItems.SelectedIndex >= 0)
            {
                MailItem = new MailItem((int)reagentbankitem_Ids[comboBox_ReagentBankItems.SelectedIndex],
                        comboBox_ReagentBankItems.SelectedItem.ToString(), textBox_Recipient.Text, textBox_Count.Text.ToInt32());
                Close();
            }
            else if (comboBox_CraftingItems.SelectedIndex >= 0)
            {
                var value = Enum.Parse(typeof(CraftingReagents), comboBox_CraftingItems.SelectedItem.ToString());
                MailItem = new MailItem(Convert.ToInt32(value),
                        comboBox_CraftingItems.SelectedItem.ToString(), textBox_Recipient.Text, textBox_Count.Text.ToInt32());
                Close();
            }
        }

        private void textBox_Count_TextChanged(object sender, EventArgs e)
        {
            if (!GarrisonBase.TextIsAllNumerical(textBox_Count.Text))
                textBox_Count.Text = "0";
        }
    }
}
