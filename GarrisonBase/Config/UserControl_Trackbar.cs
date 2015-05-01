using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Herbfunk.GarrisonBase.Config
{
    public partial class UserControl_Trackbar : TrackBar
    {
        public UserControl_Trackbar()
        {
            InitializeComponent();
            ValueChanged += ConValueChanged;
        }

        public delegate void OnValueChange(object sender, EventArgs args);

        public event OnValueChange COnValueChanged;

        private void ConValueChanged(object s, EventArgs e)
        {
            if (COnValueChanged != null)
            {
                COnValueChanged.Invoke(s, e);
            }
        }

        public void ClearHandlers()
        {
            if (COnValueChanged != null)
            {
                foreach (Delegate d in COnValueChanged.GetInvocationList())
                {
                    COnValueChanged -= (OnValueChange)d;
                } 
            }

        }
    }
}
