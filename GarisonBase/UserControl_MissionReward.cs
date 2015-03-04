using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Herbfunk.GarrisonBase
{
    public partial class UserControl_MissionReward : UserControl
    {
        private int Priority, SuccessRate;
        public Action<int> UpdatePriority { get; set; }
        public Action<int> UpdateSuccessRate { get; set; }

        public UserControl_MissionReward(int priority, int sucessrate, string labelName) : this(priority, sucessrate, labelName, Color.Black)
        {
        }

        public UserControl_MissionReward(int priority, int sucessrate, string labelName, Color labelColor)
        {
            Priority = priority;
            SuccessRate = sucessrate;
            InitializeComponent();
            label1.Text = labelName;
            label1.ForeColor = labelColor;

            numericUpDown_Priority.Value = Priority;
            numericUpDown_Priority.ValueChanged += Priority_ValueChanged;

            numericUpDown_SuccessRate.Value = SuccessRate;
            numericUpDown_SuccessRate.ValueChanged += SuccessRate_ValueChanged;
        }

        private void Priority_ValueChanged(object sender, EventArgs e)
        {
            UpdatePriority.Invoke((int)numericUpDown_Priority.Value);
        }
        private void SuccessRate_ValueChanged(object sender, EventArgs e)
        {
            UpdateSuccessRate.Invoke((int)numericUpDown_SuccessRate.Value);
        }
    }
}
