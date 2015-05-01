using System;
using System.Windows.Forms;

namespace Herbfunk.GarrisonBase.Config
{
    public partial class DialogBox : Form
    {
        public bool Result = false;
        public DialogBox(string dialogText, string caption="")
        {
            InitializeComponent();
            this.Text = caption;
            label1.Text = dialogText;
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }

        private void button_Decline_Click(object sender, EventArgs e)
        {
            Result = false;
            Close();
        }
    }
}
