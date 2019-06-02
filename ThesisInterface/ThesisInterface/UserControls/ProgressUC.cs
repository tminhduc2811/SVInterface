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
    public partial class ProgressUC : UserControl
    {
        public ProgressUC()
        {
            InitializeComponent();
            
        }
        
        public void HideBtClickHandler(EventHandler handler)
        {
            this.HideBt.Click += handler;
        }

        public void CancelBtClickHandler(EventHandler handler)
        {
            this.CancelBt.Click += handler;
        }
    }
}
