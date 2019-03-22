using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace ThesisInterface.UserControls
{
    public partial class Setting : UserControl
    {
        
        public Setting()
        {
            InitializeComponent();
        }

        public void OpenSPBtClickHandler(EventHandler handler)
        {
            this.OpenSPBt.Click += handler;
        }

        public void CloseSPBtClickHandler(EventHandler handler)
        {
            this.CloseSPBt.Click += handler;
        }

        public void SendBtClickHandler(EventHandler handler)
        {
            this.SendBt.Click += handler;
        }

        public void SaveBtClickHandler(EventHandler handler)
        {
            this.SaveBt.Click += handler;
        }
    }
}
