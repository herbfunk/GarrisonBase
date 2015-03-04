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
        public MailAddNewItem()
        {
            InitializeComponent();

            bagitems = new List<C_WoWItem>(Player.Inventory.GetBagItemsBOE());
            bankitems = new List<C_WoWItem>(Player.Inventory.GetBankItemsBOE());
            reagentbankitems = new List<C_WoWItem>(Player.Inventory.GetReagentBankItemsBOE());

            MailItem = null;

            comboBox_BagItems.Items.Clear();
            foreach (var item in bagitems)
            {
                comboBox_BagItems.Items.Add(item.Name);
            }
            comboBox_BagItems.SelectedIndexChanged += comboBox_BagItems_SelectedIndexChanged;

            comboBox_BankItems.Items.Clear();
            foreach (var item in bankitems)
            {
                comboBox_BankItems.Items.Add(item.Name);
            }
            comboBox_BankItems.SelectedIndexChanged += comboBox_BankItems_SelectedIndexChanged;

            comboBox_ReagentBankItems.Items.Clear();
            foreach (var item in reagentbankitems)
            {
                comboBox_ReagentBankItems.Items.Add(item.Name);
            }
            comboBox_ReagentBankItems.SelectedIndexChanged += comboBox_ReagentBankItems_SelectedIndexChanged;

        }

        private void comboBox_ReagentBankItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BagItems.SelectedIndex >= 0)
            {
                var item = reagentbankitems[comboBox_BagItems.SelectedIndex];
                textBox_EntryId.Text = item.Entry.ToString();
                textBox_Count.Text = "0";
            }
        }
        private void comboBox_BankItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BagItems.SelectedIndex >= 0)
            {
                var item = bankitems[comboBox_BagItems.SelectedIndex];
                textBox_EntryId.Text = item.Entry.ToString();
                textBox_Count.Text = "0";
            }
        }
        private void comboBox_BagItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BagItems.SelectedIndex >= 0)
            {
                var item = bagitems[comboBox_BagItems.SelectedIndex];
                textBox_EntryId.Text = item.Entry.ToString();
                textBox_Count.Text = "0";
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (comboBox_BagItems.SelectedIndex >= 0)
            {
                var item = bagitems[comboBox_BagItems.SelectedIndex];
                textBox_EntryId.Text = item.Entry.ToString();
                MailItem = new MailItem((int)item.Entry, item.Name, textBox_Recipient.Text, textBox_Count.Text.ToInt32());
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
