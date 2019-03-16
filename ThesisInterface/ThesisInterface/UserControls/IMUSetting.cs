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
    public partial class IMUSetting : UserControl
    {
        public IMUSetting()
        {
            InitializeComponent();
        }

        private void IMUSetting_Load(object sender, EventArgs e)
        {
            bunifuMetroTextbox1.IsAccessible = false;
        }

        public void SendMessConfigBtClickHandler(EventHandler handler)
        {
            this.SendMessConfigBt.Click += handler;
        }

        public void SendBaudrateConfigBtClickHandler(EventHandler handler)
        {
            this.SendBaudrateBt.Click += handler;
        }

        public void SendFreqBtClickHandler(EventHandler handler)
        {
            this.SendFreqBt.Click += handler;
        }

        public void CalibBtClickHandler(EventHandler handler)
        {
            this.CalibBt.Click += handler;
        }

        public void ReadConfigBtClickHandler(EventHandler handler)
        {
            this.ReadBt.Click += handler;
        }

        public void StartBtClickHandler(EventHandler handler)
        {
            this.StartBt.Click += handler;
        }

        private void BaudrateCheckBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                for (int ix = 0; ix < BaudrateCheckBox.Items.Count; ++ix)
                    if (e.Index != ix) BaudrateCheckBox.SetItemChecked(ix, false);
        }

        private void SentTextBox_TextChanged(object sender, EventArgs e)
        {
            SentTextBox.SelectionStart = SentTextBox.Text.Length;
            SentTextBox.ScrollToCaret();
        }

        private void ReceivedTextBox_TextChanged(object sender, EventArgs e)
        {
            ReceivedTextBox.SelectionStart = ReceivedTextBox.Text.Length;
            ReceivedTextBox.ScrollToCaret();
        }
    }
}
