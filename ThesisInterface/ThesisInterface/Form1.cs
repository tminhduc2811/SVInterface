using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;

namespace ThesisInterface
{
    public partial class Form1 : Form
    {
        Vehicle MyVehicle;
        
        // SET DEFAULT IMAGE FOR VELOCITY OF VEHICLE, CONSIST OF: RED, ORANGE, YELLOW, GREEN
        Image ZeroVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\ZERO.png");

        Image LowVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\LOW.png");

        Image MediumVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\MEDIUM.png");

        Image HighVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\HIGH.png");

        public string ConfigMessToWait = "";
        
        public int timeout = 40;

        public int timeoutIMU = 40;

        public int timeoutAuto = 40;

        public string AutoWaitKey = "|";

        public string IMUWaitKey = "|";  // This is used for creating waiting messages...

        public string ConfigWaitKey = "|";

        public string OldMess = "";

        public bool PlanMapEnable = false;

        public List<PointLatLng> PlanCooridnatesList = new List<PointLatLng>();

        public List<PointLatLng> ActualCooridnatesList = new List<PointLatLng>();

        public GMapOverlay PlanLines = new GMapOverlay("PlanLines");

        public GMapOverlay ActualLines = new GMapOverlay("ActualLines");

        public class Vehicle
        {
            public double M1RefVelocity, M2RefVelocity, M1Velocity, M2Velocity, M1Duty, M2Duty, RefAngle, Angle, PosX, PosY;
            public bool GPSStatus = false;
            public Vehicle(string [] ArrayInfo)
            {
                try
                {
                    M1RefVelocity = double.Parse(ArrayInfo[1], System.Globalization.CultureInfo.InvariantCulture);
                    M2RefVelocity = double.Parse(ArrayInfo[2], System.Globalization.CultureInfo.InvariantCulture);
                    M1Velocity = double.Parse(ArrayInfo[3], System.Globalization.CultureInfo.InvariantCulture);
                    M2Velocity = double.Parse(ArrayInfo[4], System.Globalization.CultureInfo.InvariantCulture);
                    M1Duty = double.Parse(ArrayInfo[5], System.Globalization.CultureInfo.InvariantCulture);
                    M2Duty = double.Parse(ArrayInfo[6], System.Globalization.CultureInfo.InvariantCulture);
                    RefAngle = double.Parse(ArrayInfo[7], System.Globalization.CultureInfo.InvariantCulture);
                    Angle = double.Parse(ArrayInfo[8], System.Globalization.CultureInfo.InvariantCulture);
                    PosX = double.Parse(ArrayInfo[10], System.Globalization.CultureInfo.InvariantCulture);
                    PosY = double.Parse(ArrayInfo[11], System.Globalization.CultureInfo.InvariantCulture);
                    if (ArrayInfo[9] == "Y")
                        GPSStatus = true;
                    else
                        GPSStatus = false;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            public string GetVehicleStatus()
            {
                string mess;
                mess = "M1 Velocity Ref: " + M1RefVelocity.ToString() + "\r\n"
                    + "M1 Velocity: " + M1Velocity.ToString() + "\r\n"
                    + "M2 Velocity Ref: " + M2RefVelocity.ToString() + "\r\n"
                    + "M2 Velocity: " + M2Velocity.ToString() + "\r\n"
                    + "M1 Duty Cycle: " + M1Duty.ToString() + "\r\n"
                    + "M2 Duty Cycle: " + M2Duty.ToString() + "\r\n"
                    + "Set Angle: " + RefAngle.ToString() + "\r\n"
                    + "Current Angle: " + Angle.ToString() + "\r\n";

                if (GPSStatus)
                {
                    mess += "GPS Status: OK\r\n" +
                            "Position(x, y): " + PosX.ToString() + ", " + PosY.ToString() + "\r\n";
                }
                else
                    mess += "GPS Status: Lost";
                    
                return mess;
            }

        }
        
        public Form1()
        {
            InitializeComponent();
            InitForm();
        }
        
        private bool ManualEnabled = false;

        private bool AutoEnabled = false;

        // Main form buttons click events ----------------------------------------//

        private void menubt_Click(object sender, EventArgs e)
        {
            if (SidePanel.Width == 0)
                SidePanel.Width = 156;
            else
                SidePanel.Width = 0;
        }

        private void minbt_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void maxbt_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }

        private void closebt_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            this.vehicleSetting1.BringToFront();
            SidePanel.Width = 0;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            this.manualUC1.BringToFront();
            SidePanel.Width = 0;
        }

        private void bunifuFlatButton4_Click(object sender, EventArgs e)
        {
            imuSetting1.BringToFront();
        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            autoUC1.BringToFront();
            SidePanel.Width = 0;
        }

        private void TopPanel_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }

        //-------------------------------------------------------------------------//

        // Init Functions & Linking to events -------------------------------------//

        private void InitForm()
        {
            SidePanel.Width = 0;
            InitSettingUC();
            LinkToUCEvents();
            this.vehicleSetting1.BringToFront();
            InitManualUC();
            ConfigWaitForRespond.Enabled = false;
            IMUConfigWaitForRespond.Enabled = false;
            InitAutoUC();
        }

        private void InitSettingUC()
        {
            vehicleSetting1.ConnectedImage.Visible = false;
            vehicleSetting1.ConnectedLabel.Visible = false;
            vehicleSetting1.CloseSPBt.Enabled = false;
            // Add items to combo box
            string[] Ports = SerialPort.GetPortNames();
            vehicleSetting1.PortNameBox.Items.AddRange(Ports);
            vehicleSetting1.BaudrateBox.Text = "9600";
            // Read txt file
            StreamReader sr = new StreamReader(Application.StartupPath + @"\Config.txt");

            string PIDConfig = sr.ReadLine();
            string[] a;
            a = PIDConfig.Split(',');
            vehicleSetting1.Velocity.Text = a[1];
            vehicleSetting1.Angle.Text = a[2];
            vehicleSetting1.Kp1.Text = a[3];
            vehicleSetting1.Ki1.Text = a[4];
            vehicleSetting1.Kd1.Text = a[5];
            vehicleSetting1.Kp2.Text = a[6];
            vehicleSetting1.Ki2.Text = a[7];
            vehicleSetting1.Kd2.Text = a[8];
            vehicleSetting1.Mode.Text = "0";
            // Exit stream reader
            sr.Close();
            sr.Dispose();
            ConfigWaitForRespond.Interval = 20;
            IMUConfigWaitForRespond.Interval = 20;
        }

        private void InitManualUC()
        {
            timer1.Enabled = false;
        }

        private void LinkToUCEvents()
        {
            this.vehicleSetting1.OpenSPBtClickHandler(new EventHandler(OpenBtSettingUCClickHandler));
            this.vehicleSetting1.CloseSPBtClickHandler(new EventHandler(CloseBtSettingUCClickHandler));
            this.vehicleSetting1.SendBtClickHandler(new EventHandler(SendBtSettingUCClickHandler));
            this.vehicleSetting1.SaveBtClickHandler(new EventHandler(SaveBtSettingUCClickHandler));
            this.vehicleSetting1.UpdateSPBtClickHandler(new EventHandler(UpdateSPBtSettingUCClickHandler));
            this.manualUC1.StartBtClickHandler(new EventHandler(StartBtManualUCClickHandler));
            this.manualUC1.StopBtClickHandler(new EventHandler(StopBtManualUCClickHandler));
            this.manualUC1.ImportBtClickHandler(new EventHandler(ImportBtManualUCClickHandler));
            this.manualUC1.ExportBtClickHandler(new EventHandler(ExportBtManualUCClickHandler));
            this.imuSetting1.SendMessConfigBtClickHandler(new EventHandler(SendMessConfigIMUClickHandler));
            this.imuSetting1.SendBaudrateConfigBtClickHandler(new EventHandler(SendBaudrateConfigClickHandler));
            this.imuSetting1.SendFreqBtClickHandler(new EventHandler(SendFreqIMUClickHandler));
            this.imuSetting1.CalibBtClickHandler(new EventHandler(CalibIMUBtClickHandler));
            this.imuSetting1.ReadConfigBtClickHandler(new EventHandler(ReadIMUConfigClickHandler));
            this.imuSetting1.StartBtClickHandler(new EventHandler(StartIMUBtClickHandler));
            this.autoUC1.StartBtClickHandler(new EventHandler(StartBtAutoUCClickHandler));
            this.autoUC1.StopBtClickHandler(new EventHandler(StopBtAutoUCClickHandler));
            this.autoUC1.OpenBtClickHandler(new EventHandler(OpenBtAutoUCClickHandler));
            this.autoUC1.SaveBtClickHandler(new EventHandler(SaveBtAutoUCClickHandler));
            this.autoUC1.PlanRoutesBtClickHandler(new EventHandler(PlanRoutesBtAutoUCClickHandler));
            this.autoUC1.SendRoutesBtClickHandler(new EventHandler(SendRoutesBtAutoUCClickHandler));
            this.autoUC1.ClearPlannedMapBtClickHandler(new EventHandler(ClearPlannedMapBtAutoUCClickHandler));
            this.autoUC1.ClearActualMapBtClickHandler(new EventHandler(ClearActualMapBtAutoUCClickHandler));
        }

        //--------------------------------------------------------------------------//
       
        // Handler for SETTING User Control-----------------------------------------//

        private void UpdateSPBtSettingUCClickHandler(object sender, EventArgs e)
        {
            string[] Ports = SerialPort.GetPortNames();
            vehicleSetting1.PortNameBox.Items.Clear();
            vehicleSetting1.PortNameBox.Items.AddRange(Ports);
        }

        private void OpenBtSettingUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = vehicleSetting1.PortNameBox.Text;
                serialPort1.BaudRate = Convert.ToInt32(vehicleSetting1.BaudrateBox.Text);
                serialPort1.ReadBufferSize = 100000;
                serialPort1.Open();
                vehicleSetting1.CloseSPBt.Enabled = true;
                vehicleSetting1.OpenSPBt.Enabled = false;
                vehicleSetting1.ConnectedImage.Visible = true;
                vehicleSetting1.ConnectedLabel.Visible = true;
                vehicleSetting1.DisonnectedImage.Visible = false;
                vehicleSetting1.DisconnectedLabel.Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseBtSettingUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                vehicleSetting1.CloseSPBt.Enabled = false;
                vehicleSetting1.OpenSPBt.Enabled = true;
                vehicleSetting1.ConnectedImage.Visible = false;
                vehicleSetting1.ConnectedLabel.Visible = false;
                vehicleSetting1.DisonnectedImage.Visible = true;
                vehicleSetting1.DisconnectedLabel.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendBtSettingUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                string mess = "";
                mess = "VEHCF," + vehicleSetting1.Velocity.Text + ","
                    + vehicleSetting1.Kp1.Text + "," + vehicleSetting1.Ki1.Text + ","
                    + vehicleSetting1.Kd1.Text + "," + vehicleSetting1.Kp2.Text
                    + "," + vehicleSetting1.Ki2.Text + "," + vehicleSetting1.Kd2.Text;
                mess = MessagesDocker(mess);
                serialPort1.Write(mess);
                ConfigMessToWait = mess;
                OldMess = vehicleSetting1.ReceiveMessTextBox.Text;
                vehicleSetting1.SentMessTextBox.Text += DateTime.Now.ToString("h:mm:ss tt") + ": Sending Configuration...\r\n(Mess sent:" + mess;
                ConfigWaitForRespond.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveBtSettingUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                string mess = "";
                mess = "$VEHCF," + vehicleSetting1.Velocity.Text + "," + vehicleSetting1.Angle.Text + ","
                    + vehicleSetting1.Kp1.Text + "," + vehicleSetting1.Ki1.Text + ","
                    + vehicleSetting1.Kd1.Text + "," + vehicleSetting1.Kp2.Text
                    + "," + vehicleSetting1.Ki2.Text + "," + vehicleSetting1.Kd2.Text;

                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\Config.txt");
                sw.WriteLine(mess);

                sw.Close();
                sw.Dispose();
                MessageBox.Show("Saved Setting");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //--------------------------------------------------------------------------//

        // Handler for IMU User Control --------------------------------------------//

        private void SendMessConfigIMUClickHandler(object sender, EventArgs e)
        {
            try
            {
                int[] indexes = imuSetting1.MessConfigCheckBox.CheckedIndices.Cast<int>().ToArray();
                string Mess="";
                for (int i = 0; i < imuSetting1.MessConfigCheckBox.Items.Count; i++ )
                {
                    if (indexes.Contains(i))
                        Mess += "1";
                    else
                        Mess += "0";
                }
                Mess = "IMUCF,DATOP," + Mess;
                Mess = MessagesDocker(Mess);
                serialPort1.Write(Mess);
                imuSetting1.SentTextBox.Text += "Sending Messages Configuration \r\nMessages: "+ Mess;
                OldMess = imuSetting1.ReceivedTextBox.Text;
                IMUConfigWaitForRespond.Enabled = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendBaudrateConfigClickHandler(object sender, EventArgs e)
        {
            try
            {
                int[] indexes = imuSetting1.BaudrateCheckBox.CheckedIndices.Cast<int>().ToArray();
                if(indexes.Count() != 0)
                {
                    string Mess = "IMUCF,DATOP," + (indexes[0] + 1).ToString();
                    Mess = MessagesDocker(Mess);
                    serialPort1.Write(Mess);
                    imuSetting1.SentTextBox.Text += "Sending Baudrate Configuration... \r\nMessages: " + Mess;
                    OldMess = imuSetting1.ReceivedTextBox.Text;
                    IMUConfigWaitForRespond.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Please choose the suitable baudrate", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendFreqIMUClickHandler(object sender, EventArgs e)
        {
            try
            {
                string Mess = "IMUCF,TSAMP," + imuSetting1.FreqTextBox.Text;
                Mess = MessagesDocker(Mess);
                serialPort1.Write(Mess);
                imuSetting1.SentTextBox.Text += "Sending Update Frequency Configuration... \r\nMessages: " + Mess;
                OldMess = imuSetting1.ReceivedTextBox.Text;
                IMUConfigWaitForRespond.Enabled = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalibIMUBtClickHandler(object sender, EventArgs e)
        {
            if(imuSetting1.CalibBt.Text == "Start")
            {
                try
                {
                    string Mess = "IMUCF,MAG2D";
                    Mess = MessagesDocker(Mess);
                    serialPort1.Write(Mess);
                    imuSetting1.SentTextBox.Text += "Sending Calibration Command... \r\nMessages: " + Mess;
                    OldMess = imuSetting1.ReceivedTextBox.Text;
                    imuSetting1.CalibBt.Text = "Stop";
                    IMUConfigWaitForRespond.Enabled = true;
                }

                catch (Exception ex)
                {                    
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    serialPort1.Write("$VSTOP");
                    imuSetting1.CalibBt.Text = "Start";
                    IMUConfigWaitForRespond.Enabled = true;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ReadIMUConfigClickHandler(object sender, EventArgs e)
        {
            try
            {
                string Mess = "IMUCF,GPARA";
                Mess = MessagesDocker(Mess);
                serialPort1.Write(Mess);
                imuSetting1.SentTextBox.Text += "Sending Read Config Command... \r\nMessages: " + Mess;
                OldMess = imuSetting1.ReceivedTextBox.Text;
                IMUConfigWaitForRespond.Enabled = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartIMUBtClickHandler(object sender, EventArgs e)
        {
            try
            {
                string Mess = "IMUCF,START";
                Mess = MessagesDocker(Mess);
                serialPort1.Write(Mess);
                imuSetting1.SentTextBox.Text += "Sending Start Command... \r\nMessages: " + Mess;
                OldMess = imuSetting1.ReceivedTextBox.Text;
                IMUConfigWaitForRespond.Enabled = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //--------------------------------------------------------------------------//

        // Handler for MANUAL User Control -----------------------------------------//

        private void ImportBtManualUCClickHandler(object sender, EventArgs e)
        {

        }

        private void ExportBtManualUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Text File|*.txt";
                sfd.FileName = "Map";
                sfd.Title = "Save Text File";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = sfd.FileName;
                    File.WriteAllText(path, manualUC1.PositionBox.Text);
                    MessageBox.Show("Map Saved");
                }
                
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void StartBtManualUCClickHandler(object sender, EventArgs e)
        {
            
            // Write start command to MCU
            try
            {
                serialPort1.Write(MessagesDocker("MACON,1"));
                manualUC1.SentBox.Text += DateTime.Now.ToString("h:mm:ss tt") + ": Started to control manually\r\n";
                manualUC1.FormStatus.Text = "STARTED";
                ManualEnabled = true;
                timer1.Enabled = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void StopBtManualUCClickHandler(object sender, EventArgs e)
        {
            
            try
            {
                serialPort1.Write(MessagesDocker("MACON,0"));
                ManualEnabled = false;
                timer1.Enabled = false;
                manualUC1.FormStatus.Text = "STOPED";
                manualUC1.SentBox.Text += DateTime.Now.ToString("h:mm:ss tt") + ": Stop controlling manually\r\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void manualUC1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ManualEnabled)
            {
                try
                {
                    string key = e.KeyData.ToString().ToUpper();
                    if (key == "W")
                    {
                        serialPort1.Write(MessagesDocker("MACON,W"));
                        manualUC1.SentBox.Text += "Increasing velocity...\r\n(Messages Sent: " + "$MACON," + key + "," + checksum("MACON,W,")+"\r\n";
                    }
                    if (key == "A")
                    {
                        serialPort1.Write(MessagesDocker("MACON,A"));
                        manualUC1.SentBox.Text += "Turning Left...\r\n(Messages Sent: $MACON,A)\r\n";
                    }
                    if (key == "D")
                    {
                        serialPort1.Write(MessagesDocker("MACON,D"));
                        manualUC1.SentBox.Text += "Turning Right...\r\n(Messages Sent: $MACON,D)\r\n";
                    }
                    if (key == "S")
                    {
                        serialPort1.Write(MessagesDocker("MACON,S"));
                        manualUC1.SentBox.Text += "Decreasing Velocity...\r\n(Messages Sent: $MACON,S)\r\n";
                    }
                    if (key == "F")
                    {
                        serialPort1.Write(MessagesDocker("MACON,F"));
                        manualUC1.SentBox.Text += "RESET VEHICLE...\r\n(Messages Sent: $MACON,F)\r\n";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please start this mode first!");
            }
        }

        //--------------------------------------------------------------------------//

        // Handler for AUTO User Control -------------------------------------------//

        private void StartBtAutoUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(MessagesDocker("AUCON,1"));
                OldMess = autoUC1.ReceivedTb.Text;
                autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Start auto mode\r\n";
                AutoTimer.Enabled = true;
                //test from here
                AutoEnabled = true;
                timer1.Enabled = true;
            
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopBtAutoUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(MessagesDocker("AUCON,0"));
                autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Stop auto mode\r\n";
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenBtAutoUCClickHandler(object sender, EventArgs e)
        {

        }

        private void SaveBtAutoUCClickHandler(object sender, EventArgs e)
        {

        }

        private void autoUC1_MouseClick(object sender, MouseEventArgs e)
        {
            if(PlanMapEnable && (e.Button == MouseButtons.Right))
            {
                //MessageBox.Show(autoUC1.gmap.FromLocalToLatLng(e.X, e.Y).ToString());
                PlanCooridnatesList.Add(autoUC1.gmap.FromLocalToLatLng(e.X, e.Y));
                var line = new GMapRoute(PlanCooridnatesList, "single_line")
                {
                    Stroke = new Pen(Color.DarkRed, 2)
                };
                PlanLines.Routes.Clear();
                PlanLines.Routes.Add(line);
                autoUC1.gmap.Overlays.Add(PlanLines);
            }
        }

        private void PlanRoutesBtAutoUCClickHandler(object sender, EventArgs e)
        {
            if(PlanMapEnable == true)
            {
                PlanMapEnable = false;
                autoUC1.PlanMapBt.Text = "Plan Routes";
            }
            else
            {
                PlanMapEnable = true;
                autoUC1.PlanMapBt.Text = "OK";
            }
        }

        private void SendRoutesBtAutoUCClickHandler(object sender, EventArgs e)
        {

        }

        private void ClearPlannedMapBtAutoUCClickHandler(object sender, EventArgs e)
        {
            autoUC1.gmap.Overlays.Clear();
            PlanCooridnatesList.Clear();
            PlanLines.Clear();
        }

        private void ClearActualMapBtAutoUCClickHandler(object sender, EventArgs e)
        {
            autoUC1.gmap.Overlays.Clear();
            ActualCooridnatesList.Clear();
            ActualLines.Clear();
        }

        private void InitAutoUC()
        {
            AutoTimer.Interval = 20;
            AutoTimer.Enabled = false;
            autoUC1.gmap.DragButton = MouseButtons.Left;
            autoUC1.gmap.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            autoUC1.gmap.SetPositionByKeywords("Vietnam, Ho Chi Minh");
            autoUC1.gmap.Position = new PointLatLng(10.772801, 106.659273);
            autoUC1.gmap.Zoom = 19;
            
            //GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
            //GMap.NET.WindowsForms.GMapMarker marker =
            //    new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
            //        new GMap.NET.PointLatLng(10.772801, 106.659273),
            //        GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);
            //markers.Markers.Add(marker);
            //autoUC1.gmap.Overlays.Add(markers);
        }

        //--------------------------------------------------------------------------//
        
        // TIMERS ------------------------------------------------------------------//

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ManualEnabled)
            {
                if (serialPort1.IsOpen)
                {
                    if (serialPort1.BytesToRead != 0)
                    {
                        try
                        {
                            string temp;
                            string mess = serialPort1.ReadLine();
                            string[] value = new string[255];
                            value = mess.Split(',');
                            temp = mess;
                            temp = temp.Remove(temp.Length - 3, 3);
                            temp = temp.Remove(0, 1);
                            if (value[12].Contains(checksum(temp)))
                            {
                                MyVehicle = new Vehicle(value);
                                manualUC1.VehicleStatusBox.Text = MyVehicle.GetVehicleStatus();
                                // Save Position Data & Draw On Map
                                manualUC1.PositionBox.Text += MyVehicle.PosX.ToString() + "," + MyVehicle.PosY.ToString() + "\r\n";
                                manualUC1.chart1.Series["Position"].Points.AddXY(MyVehicle.PosX, MyVehicle.PosY);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }                
                    }
                }
            }
            else if(AutoEnabled)
            {
                if (serialPort1.IsOpen)
                {
                    if (serialPort1.BytesToRead != 0)
                    {
                        try
                        {
                            string mess = serialPort1.ReadLine();
                            string[] value = mess.Split(',');
                            if (value[0] == "$GNGLL")
                            {
                                autoUC1.ReceivedTb.Text += mess;
                                ActualCooridnatesList.Add(new GMap.NET.PointLatLng(Convert.ToDouble(value[1]), Convert.ToDouble(value[3])));
                                var line = new GMapRoute(ActualCooridnatesList, "single_line")
                                {
                                    Stroke = new Pen(Color.DarkGreen, 2)
                                };
                                ActualLines.Routes.Clear();                                
                                ActualLines.Routes.Add(line);
                                autoUC1.gmap.Overlays.Add(ActualLines);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void ConfigWaitForRespond_Tick(object sender, EventArgs e)
        {
            if(timeout > 0)
            {
                timeout--;
                if (ConfigWaitKey == "|")
                {
                    vehicleSetting1.ReceiveMessTextBox.Text = OldMess + "Waiting for respond" + ConfigWaitKey;
                    ConfigWaitKey = "-";

                }
                else
                {
                    vehicleSetting1.ReceiveMessTextBox.Text = OldMess + "Waiting for respond" + ConfigWaitKey;
                    ConfigWaitKey = "|";
                }
                if (serialPort1.IsOpen && (ManualEnabled == false))
                {
                    if (serialPort1.BytesToRead != 0)
                    {

                        string temp = serialPort1.ReadLine();
                        if (temp.Contains("$SINFO,1"))
                        {
                            ConfigWaitForRespond.Enabled = false;
                            vehicleSetting1.ReceiveMessTextBox.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + ": " + "Setting successfully\r\n";
                            timeout = 40;
                            ConfigMessToWait = "";
                            
                        }
                        else if(temp.Contains("$SINFO,0"))
                        {
                            ConfigWaitForRespond.Enabled = false;
                            vehicleSetting1.ReceiveMessTextBox.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + ": " + "Fail to config\r\n";
                            timeout = 40;
                            ConfigMessToWait = "";
                            
                        }
                    }
                }
            }
            else
            {
                ConfigWaitForRespond.Enabled = false;
                timeout = 40;
                MessageBox.Show("Error: Timeout, no respone from MCU", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                vehicleSetting1.ReceiveMessTextBox.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + ": " + "Fail to config\r\n";
            }
        }

        private void IMUConfigWaitForRespond_Tick(object sender, EventArgs e)
        {          
            if(timeoutIMU > 0)
            {
                timeoutIMU--;
                if(IMUWaitKey == "|")
                {
                    imuSetting1.ReceivedTextBox.Text = OldMess + "Waiting for respond" + IMUWaitKey;
                    IMUWaitKey = "-";
                    
                }
                else
                {
                    imuSetting1.ReceivedTextBox.Text = OldMess + "Waiting for respond" + IMUWaitKey;
                    IMUWaitKey = "|";
                }
                if(serialPort1.BytesToRead != 0)
                {
                    string mess = serialPort1.ReadLine();
                    string [] messParsed = mess.Split(',');
                    if(messParsed[0].Contains("$SINFO") && messParsed[1].Contains("1"))
                    {
                        imuSetting1.ReceivedTextBox.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + " Done\r\n";
                        timeoutIMU = 40;
                        IMUConfigWaitForRespond.Enabled = false;
                    }
                }
            }
            else
            {
                timeoutIMU = 40;
                imuSetting1.ReceivedTextBox.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + " Failed to config IMU\r\n";
                IMUConfigWaitForRespond.Enabled = false;
            }
        }

        private void AutoTimer_Tick(object sender, EventArgs e)
        {
            if (timeoutAuto > 0)
            {
                timeoutAuto--;
                if (AutoWaitKey == "|")
                {
                    autoUC1.ReceivedTb.Text = OldMess + "Waiting for respond" + AutoWaitKey;
                    AutoWaitKey = "-";

                }
                else
                {
                    autoUC1.ReceivedTb.Text = OldMess + "Waiting for respond" + AutoWaitKey;
                    AutoWaitKey = "|";
                }
                if (serialPort1.BytesToRead != 0)
                {
                    string mess = serialPort1.ReadLine();
                    string[] messParsed = mess.Split(',');
                    if (messParsed[0].Contains("$SINFO") && messParsed[1].Contains("1"))
                    {
                        autoUC1.ReceivedTb.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + " Started auto mode successfully.\r\n";
                        timeoutAuto = 40;
                        AutoTimer.Enabled = false;
                    }
                }
            }
            else
            {
                timeoutAuto = 40;
                autoUC1.ReceivedTb.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + " Failed to start auto mode\r\n";
                AutoTimer.Enabled = false;
            }
        }

        //---------------------------------------------------------------------------//

        // Other functions ----------------------------------------------------------//

        public void DrawVehicleStatusOnImage(PictureBox ImgBox, float angle, Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PointF offset = new PointF((float)image.Width / 2, (float)image.Height / 2);

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            ImgBox.Image = rotatedBmp;
        }

        public static string checksum(string message)
        {
            byte[] buffer;
            int sum = 0;
            buffer = Encoding.Unicode.GetBytes(message);
            foreach (var b in buffer)
                sum ^= b;
            if (sum.ToString("X").Count() < 2)
                return "0" + sum.ToString("X");
            return sum.ToString("X");
        }

        public string MessagesDocker(string RawMess)
        {
            string MessWithoutKey = RawMess + ",";

            return "$" + MessWithoutKey + checksum(MessWithoutKey) + "\r\n";
        }


    }
}
