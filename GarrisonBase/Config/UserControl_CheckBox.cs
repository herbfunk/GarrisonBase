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
    public partial class UserControl_CheckBox : CheckBox
    {
        public UserControl_CheckBox()
        {
            InitializeComponent();
            CheckedChanged += CcheckedChanged;
        }
        public UserControl_CheckBox(bool ischecked, string text)
        {
            InitializeComponent();
            Checked = ischecked;
            Text = text;
            CheckedChanged += CcheckedChanged;
        }

        public delegate void OnCheckedChanged(object sender, EventArgs args);

        public event OnCheckedChanged CCheckedChanged;

        private void CcheckedChanged(object s, EventArgs e)
        {
            if (CCheckedChanged != null)
            {
                CCheckedChanged.Invoke(s, e);
            }
        }

        public void ClearHandlers()
        {
            if (CCheckedChanged != null)
            {
                foreach (Delegate d in CCheckedChanged.GetInvocationList())
                {
                    CCheckedChanged -= (OnCheckedChanged)d;
                }
            }

        }
    }
}
