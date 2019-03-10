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
    public partial class AutoUC : UserControl
    {
        public AutoUC()
        {
            InitializeComponent();
        }
        
        public void StartBtClickHandler(EventHandler handler)
        {
            this.StartBt.Click += handler;
        }

        public void StopBtClickHandler(EventHandler handler)
        {
            this.StopBt.Click += handler;
        }

        public void OpenBtClickHandler(EventHandler handler)
        {
            this.OpenBt.Click += handler;
        }

        public void SaveBtClickHandler(EventHandler handler)
        {
            this.SaveBt.Click += handler;
        }
    }
}
