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
    public partial class UserControl_Textbox : TextBox
    {
        public UserControl_Textbox()
        {
            InitializeComponent();
            TextChanged += ConTextChanged;
            KeyPress += ConKeyPressed;
        }

        public delegate void OnTextChange(object sender, EventArgs args);

        public event OnTextChange COnTextChanged;

        private void ConTextChanged(object s, EventArgs e)
        {
            if (COnTextChanged != null)
            {
                COnTextChanged.Invoke(s, e);
            }
        }


        public delegate void OnKeyPress(object sender, KeyPressEventArgs args);

        public event OnKeyPress COnKeyPressed;

        private void ConKeyPressed(object s, KeyPressEventArgs e)
        {
            if (COnKeyPressed != null)
            {
                COnKeyPressed.Invoke(s, e);
            }
        }


        public void ClearHandlers()
        {
            if (COnTextChanged != null)
            {
                foreach (Delegate d in COnTextChanged.GetInvocationList())
                {
                    COnTextChanged -= (OnTextChange)d;
                }
            }

            if (COnKeyPressed != null)
            {
                foreach (Delegate d in COnKeyPressed.GetInvocationList())
                {
                    COnKeyPressed -= (OnKeyPress)d;
                }
            }

        }
    }
}
