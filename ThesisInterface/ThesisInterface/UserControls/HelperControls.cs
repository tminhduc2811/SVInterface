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
    public partial class HelperControls : UserControl
    {
        public HelperControls()
        {
            InitializeComponent();
        }

        public void CloseBtClickHandler(EventHandler e)
        {
            this.ClosePanelBt.Click += e;
        }

        public void SettingVehicleBtClickHandler(EventHandler e)
        {
            this.SettingBt.Click += e;
        }

        public void IMUSettingBtClickHandler(EventHandler e)
        {
            this.ImuSettingBt.Click += e;
        }

        public void ManualBtClickHandler(EventHandler e)
        {
            this.ManualControlBt.Click += e;
        }

        public void AutoBtClickHandler(EventHandler e)
        {
            this.AutoControlBt.Click += e;
        }

        public void KctrlBtClickHandler(EventHandler e)
        {
            this.KctrlBt.Click += e;
        }
        
        public void GetVehicleInfoBtClickHandler(EventHandler e)
        {
            this.GetVehicleInfoBt.Click += e;
        }

        public void OnSendDataBtClickHandler(EventHandler e)
        {
            this.OnSendDataBt.Click += e;
        }

        public void OffSendDataBtClickHandler(EventHandler e)
        {
            this.OffSendDataBt.Click += e;
        }
    }
}
