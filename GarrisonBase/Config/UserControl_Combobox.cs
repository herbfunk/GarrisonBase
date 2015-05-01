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
    public partial class UserControl_Combobox : ComboBox
    {
        public UserControl_Combobox()
        {
            InitializeComponent();
            SelectedIndexChanged += ConSelectedIndexChanged;
        }

        public delegate void OnSelectedIndexChanged(object sender, EventArgs args);

        public event OnSelectedIndexChanged COnSelectedIndexChanged;

        private void ConSelectedIndexChanged(object s, EventArgs e)
        {
            if (COnSelectedIndexChanged != null)
            {
                COnSelectedIndexChanged.Invoke(s, e);
            }
        }


        public void ClearHandlers()
        {
            if (COnSelectedIndexChanged != null)
            {
                foreach (Delegate d in COnSelectedIndexChanged.GetInvocationList())
                {
                    COnSelectedIndexChanged -= (OnSelectedIndexChanged)d;
                }
            }
        }

    }
}
