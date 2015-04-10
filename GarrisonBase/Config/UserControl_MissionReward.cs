using System;
using System.Drawing;
using System.Windows.Forms;

namespace Herbfunk.GarrisonBase.Config
{
    public partial class UserControl_MissionReward : UserControl
    {
        private int Priority, SuccessRate;
        public Action<int> UpdatePriority { get; set; }
        public Action<int> UpdateSuccessRate { get; set; }
        public Action<int> UpdateMinimumLevel { get; set; }

        public UserControl_MissionReward(int priority, int sucessrate, int minlevel, string labelName)
            : this(priority, sucessrate, minlevel, labelName, Color.White)
        {
        }

        public UserControl_MissionReward(int priority, int sucessrate, int minlevel, string labelName, Color labelColor)
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

            numericUpDown_MinimumLevel.Value = minlevel;
            numericUpDown_MinimumLevel.ValueChanged += MinimumLevel_ValueChanged;
        }
        private void MinimumLevel_ValueChanged(object sender, EventArgs e)
        {
            UpdateMinimumLevel.Invoke((int)numericUpDown_MinimumLevel.Value);
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
