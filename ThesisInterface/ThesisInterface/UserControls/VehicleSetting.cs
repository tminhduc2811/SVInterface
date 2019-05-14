using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThesisInterface.UserControls
{
    public partial class VehicleSetting : UserControl
    {
        public VehicleSetting()
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

        public void UpdateSPBtClickHandler(EventHandler handler)
        {
            this.UpdateSP.Click += handler;
        }
    }
}
