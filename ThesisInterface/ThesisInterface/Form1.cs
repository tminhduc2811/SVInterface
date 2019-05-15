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
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace ThesisInterface
{
    public partial class Form1 : Form
    {
        public class Vehicle
        {
            public double M1RefVelocity, M2RefVelocity, M1Velocity, M2Velocity, M1Duty, M2Duty, RefAngle, Angle, PosX, PosY, Lat, Lng;
            public bool GPSStatus = false;
            public Vehicle(string[] ArrayInfo)
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
                    PosX = double.Parse(ArrayInfo[11], System.Globalization.CultureInfo.InvariantCulture);
                    PosY = double.Parse(ArrayInfo[12], System.Globalization.CultureInfo.InvariantCulture);
                    Lat = double.Parse(ArrayInfo[13], System.Globalization.CultureInfo.InvariantCulture);
                    Lng = double.Parse(ArrayInfo[14], System.Globalization.CultureInfo.InvariantCulture);
                    if (ArrayInfo[9].Contains("Y"))
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
                            "Position(x, y): " + PosX.ToString() + ", " + PosY.ToString() + "\r\n" + "Lat,Lng: "
                            + Lat.ToString() + ", " + Lng.ToString() + "\r\n";
                }
                else
                    mess += "GPS Status: Lost";

                return mess;
            }
        }

        public class PlannedCoordinate
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

        public class ActualCoordinate
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

        public class CoordinatesInfo
        {
            public List<PlannedCoordinate> plannedCoordinates { get; set; }
            public List<ActualCoordinate> actualCoordinates { get; set; }
        }

        public class RootObject
        {
            public CoordinatesInfo coordinatesInfo { get; set; }
        }



        Vehicle MyVehicle;
        
        // SET DEFAULT IMAGE FOR VELOCITY OF VEHICLE, CONSIST OF: RED, ORANGE, YELLOW, GREEN
        Image ZeroVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\ZERO.png");

        Image LowVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\LOW.png");

        Image MediumVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\MEDIUM.png");

        Image HighVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\HIGH.png");

        //TODO: Set temporarily, fix later
        char KeyW = '!', KeyS = '!', KeyA = '!', KeyD = '!';
        int level = 4;
        private void SendCommandAsync()
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("$KCTRL," + KeyW.ToString() + "," + KeyS.ToString() + "," + KeyA.ToString() + "," + KeyD.ToString() + "," + level.ToString() + "\r\n");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------
        public string ConfigMessToWait = "";
        
        public int timeout = 40;

        public int timeoutIMU = 40;

        public int timeoutAuto = 40;

        public string AutoWaitKey = "|";

        public string IMUWaitKey = "|";  // This is used for creating waiting messages...

        public string ConfigWaitKey = "|";

        public string OldMess = "";

        public int KcontrolWaitTimes = 2, defaultwaitTimes = 2;

        public bool PlanMapEnable = false, online = false;

        public List<PointLatLng> PlanCooridnatesList = new List<PointLatLng>();

        public List<PointLatLng> ActualCooridnatesList = new List<PointLatLng>();

        public GMapOverlay PlanLines = new GMapOverlay("PlanLines");

        public GMapOverlay ActualLines = new GMapOverlay("ActualLines");
        
        public bool KctrlEnabled = false;

        public int KctrlTimerTimes = 20;

        private bool OnHelperPanel = false;

        private int DefaultWaitTimes = 20;

        public Form1()
        {
            InitializeComponent();
            InitForm();
            KeyPreview = true;
            KeyDown += new KeyEventHandler(Form1_KeyDown);
            KeyUp += new KeyEventHandler(Form1_KeyUp);
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyValue)
            {
                case 112: //Key = F1
                    vehicleSetting1.BringToFront();
                    break;
                case 113: // Key = F2
                    imuSetting1.BringToFront();
                    break;
                case 114: // Key = F3
                    manualUC1.BringToFront();
                    break;
                case 115: // Key = F4
                    autoUC1.BringToFront();
                    break;
                case 116: // Key = F5
                    KctrlChangeMode();
                    break;
                case 117: // Key = F6, Get all vehicle information

                    break;

                case 118: // Key = F7, ON Send data from vehicle
                    ControlSendDataFromVehicle(0);
                    break;
                case 119: // Key = F8, OFF Send data from vehicle
                    ControlSendDataFromVehicle(1);
                    break;
                case 123: // Key = F12, Get all help controls
                    if(OnHelperPanel)
                    {
                        OnHelperPanel = false;
                        helperControls1.SendToBack();
                    }
                    else
                    {
                        OnHelperPanel = true;
                        helperControls1.BringToFront();
                    }
                    break;
                case 27: // Key = Escape
                    helperControls1.SendToBack();
                    break;
            }
            if(KctrlEnabled)
            {
                if (e.KeyCode == Keys.W)
                {
                    if (KeyW != 'W')
                    {
                        KeyW = 'W';
                        SetCommandForKcontrol();

                    }
                }
                else if (e.KeyCode == Keys.S)
                {
                    if (KeyS != 'S')
                    {
                        KeyS = 'S';
                        SetCommandForKcontrol();
                    }
                }
                else if (e.KeyCode == Keys.A)
                {
                    if (KeyA != 'A')
                    {
                        KeyA = 'A';
                        int oldLevel = level;
                        level = Convert.ToInt16(0.5 * level);
                        SetCommandForKcontrol();
                        level = oldLevel;
                    }
                }
                else if (e.KeyCode == Keys.D)
                {
                    if (KeyD != 'D')
                    {
                        KeyD = 'D';
                        int oldLevel = level;
                        level = Convert.ToInt16(0.5 * level);
                        SetCommandForKcontrol();
                        level = oldLevel;
                    }
                }
                else if (e.KeyCode == Keys.E)
                {
                    if (level >= 10)
                        level = 10;
                    else
                        level++;
                    SetCommandForKcontrol();
                }
                else if (e.KeyCode == Keys.Q)
                {
                    if (level <= 0)
                        level = 0;
                    else
                        level--;
                    SetCommandForKcontrol();
                }
            }
        }

        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if(KctrlEnabled)
            {
                if (e.KeyCode == Keys.W)
                {
                    KeyW = '!';
                    SetCommandForKcontrol();
                }
                else if (e.KeyCode == Keys.S)
                {
                    KeyS = '!';
                    SetCommandForKcontrol();
                }
                else if (e.KeyCode == Keys.A)
                {
                    KeyA = '!';
                    SetCommandForKcontrol();
                }
                else if (e.KeyCode == Keys.D)
                {
                    KeyD = '!';
                    SetCommandForKcontrol();
                }
            }
        }

        private bool ManualEnabled = false;

        private bool AutoEnabled = false;

        private bool SendCommandSuccessfully = false;
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

        private void ResetVehicleBt_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(MessagesDocker("RESET"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void ManualBtClickHandler(object sender, EventArgs e)
        {
            // TODO: Add disable for this mode latter
            try
            {
                serialPort1.Write(MessagesDocker("KCTRL,1"));
                manualUC1.SentBox.Text += DateTime.Now.ToString("h:mm:ss tt") + ": Started to control manually\r\n";
                manualUC1.FormStatus.Text = "STARTED";
                AutoEnabled = true;
                timer1.Enabled = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //-------------------------------------------------------------------------//

        // Init Functions & Linking to events -------------------------------------//

        private void InitForm()
        {
            StartKctrlTimer.Enabled = false;
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
            vehicleSetting1.Kp1.Enabled = false;
            vehicleSetting1.Ki1.Enabled = false;
            vehicleSetting1.Kd1.Enabled = false;
            vehicleSetting1.Kp2.Enabled = false;
            vehicleSetting1.Ki2.Enabled = false;
            vehicleSetting1.Kd2.Enabled = false;
            vehicleSetting1.Velocity.Enabled = false;
            vehicleSetting1.Angle.Enabled = false;
            vehicleSetting1.Mode.Enabled = false;

            vehicleSetting1.ConnectedImage.Visible = false;
            vehicleSetting1.ConnectedLabel.Visible = false;
            vehicleSetting1.CloseSPBt.Enabled = false;
            // Add items to combo box
            string[] Ports = SerialPort.GetPortNames();
            vehicleSetting1.PortNameBox.Items.AddRange(Ports);
            vehicleSetting1.PortNameBox.Text = "COM4";
            vehicleSetting1.BaudrateBox.Text = "115200";
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
            InitManual();
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
            this.manualUC1.ChangeModeBtClickHandler(new EventHandler(ChangeModeBtManualUCClickHandler));
            this.imuSetting1.SendMessConfigBtClickHandler(new EventHandler(SendMessConfigIMUClickHandler));
            this.imuSetting1.SendBaudrateConfigBtClickHandler(new EventHandler(SendBaudrateConfigClickHandler));
            this.imuSetting1.SendFreqBtClickHandler(new EventHandler(SendFreqIMUClickHandler));
            this.imuSetting1.CalibBtClickHandler(new EventHandler(CalibIMUBtClickHandler));
            this.imuSetting1.ReadConfigBtClickHandler(new EventHandler(ReadIMUConfigClickHandler));
            this.imuSetting1.StartBtClickHandler(new EventHandler(StartIMUBtClickHandler));
            this.autoUC1.OnBtClickHandler(new EventHandler(OnBtAutoUCClickHandler));
            this.autoUC1.StartBtClickHandler(new EventHandler(StartBtAutoUCClickHandler));
            this.autoUC1.StopBtClickHandler(new EventHandler(StopBtAutoUCClickHandler));
            this.autoUC1.OpenBtClickHandler(new EventHandler(OpenBtAutoUCClickHandler));
            this.autoUC1.SaveBtClickHandler(new EventHandler(SaveBtAutoUCClickHandler));
            this.autoUC1.PlanRoutesBtClickHandler(new EventHandler(PlanRoutesBtAutoUCClickHandler));
            this.autoUC1.SendRoutesBtClickHandler(new EventHandler(SendRoutesBtAutoUCClickHandler));
            this.autoUC1.ClearPlannedMapBtClickHandler(new EventHandler(ClearPlannedMapBtAutoUCClickHandler));
            this.autoUC1.ClearActualMapBtClickHandler(new EventHandler(ClearActualMapBtAutoUCClickHandler));
            this.autoUC1.StopVehicleBtClickHandler(new EventHandler(StopVehicleBtAutoUCClickHandler));
            this.helperControls1.CloseBtClickHandler(new EventHandler(CloseBtHelperUCClickHandler));
            this.helperControls1.SettingVehicleBtClickHandler(new EventHandler(SettingVehicleHelperUCClickHandler));
            this.helperControls1.IMUSettingBtClickHandler(new EventHandler(IMUSettingHelperUCClickHandler));
            this.helperControls1.ManualBtClickHandler(new EventHandler(ManualBtHelperUCClickHandler));
            this.helperControls1.AutoBtClickHandler(new EventHandler(AutoBtHelperUCClickHandler));
            this.helperControls1.KctrlBtClickHandler(new EventHandler(KctrlBtHelperUCClickHandler));
            this.helperControls1.OnSendDataBtClickHandler(new EventHandler(OnSendDataBtHelperUCClickHandler));
            this.helperControls1.OffSendDataBtClickHandler(new EventHandler(OffSendDataBtHelperUCClickHandler));
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
                vehicleSetting1.PortNameBox.Enabled = false;
                vehicleSetting1.BaudrateBox.Enabled = false;
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
                vehicleSetting1.PortNameBox.Enabled = true;
                vehicleSetting1.BaudrateBox.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendBtSettingUCClickHandler(object sender, EventArgs e)
        {
            if(vehicleSetting1.Kp1.Enabled)
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
                    // Disable text box
                    DisableAllConfigTextBoxes();
                }
                catch (Exception ex)
                {
                    DisableAllConfigTextBoxes();
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                EnableAllConfigTextBoxes();
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
                DisableAllTimers();
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
                serialPort1.Write(MessagesDocker("KCTRL,0"));
                DisableAllTimers();
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
           
            try
            {
                if (e.KeyCode == Keys.W)
                {
                    serialPort1.Write(MessagesDocker("MACON,W"));
                }
                else if (e.KeyCode == Keys.S)
                {
                    serialPort1.Write(MessagesDocker("MACON,S"));
                }
                else if (e.KeyCode == Keys.A)
                {
                    serialPort1.Write(MessagesDocker("MACON,A"));
                }
                else if (e.KeyCode == Keys.D)
                {
                    serialPort1.Write(MessagesDocker("MACON,D"));
                }
                else if (e.KeyCode == Keys.F)
                {
                    serialPort1.Write(MessagesDocker("MACON,F"));
                }
            }
            catch
            {

            }
        }

        private void manualUC1_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.W)
            //{
            //    KeyW = '!';
            //    SetCommandForKcontrol();
            //}
            //else if (e.KeyCode == Keys.S)
            //{
            //    KeyS = '!';
            //    SetCommandForKcontrol();
            //}
            //else if (e.KeyCode == Keys.A)
            //{
            //    KeyA = '!';
            //    SetCommandForKcontrol();
            //}
            //else if (e.KeyCode == Keys.D)
            //{
            //    KeyD = '!';
            //    SetCommandForKcontrol();
            //}
        }

        private void ChangeModeBtManualUCClickHandler(object sender, EventArgs e)
        {
            if(online)
            {
                manualUC1.modeBt.ButtonText = "Offline";
                manualUC1.gmap.SendToBack();
                online = false;
            }
            else
            {
                manualUC1.modeBt.ButtonText = "Online";
                manualUC1.chart1.SendToBack();
                online = true;
            }
        }

        private void InitManual()
        {
            manualUC1.gmap.DragButton = MouseButtons.Left;
            manualUC1.gmap.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            manualUC1.gmap.SetPositionByKeywords("Vietnam, Ho Chi Minh");
            manualUC1.gmap.Position = new PointLatLng(10.772801, 106.659273);
            manualUC1.gmap.Zoom = 19;
        }
        //--------------------------------------------------------------------------//

        // Handler for AUTO User Control -------------------------------------------//

        private void StartWaitingForResponse()
        {
            OldMess = autoUC1.ReceivedTb.Text;
            AutoTimer.Enabled = true;
            AutoEnabled = true;
            timer1.Enabled = true;
        }

        private void StopVehicleBtAutoUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(MessagesDocker("AUCON,STOP"));
                autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Stopped vehicle...\r\n";
                autoUC1.StartBt.Text = "Start";
                StartWaitingForResponse();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // *TODO: Consider to remove this

        private void OnBtAutoUCClickHandler(object sender, EventArgs e)  
        {
            try
            {
                DisableAllTimers();
                serialPort1.Write(MessagesDocker("AUCON,1"));
                autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Started auto mode\r\n";
                AutoEnabled = true;
                timer1.Enabled = true;
                StartWaitingForResponse();

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
                DisableAllTimers();
                serialPort1.Write(MessagesDocker("AUCON,0"));
                autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Stopped auto mode\r\n";
                StartWaitingForResponse();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartBtAutoUCClickHandler(object sender, EventArgs e)  // *TODO: Consider to remove this
        {
            try
            {
                serialPort1.Write(MessagesDocker("AUCON,START"));
                autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Started vehicle...\r\n";
                autoUC1.StartBt.Text = "Stop";
                StartWaitingForResponse();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void OpenBtAutoUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                string jsonfile = ReadJsonFile();

                CoordinatesInfo coordinatesInformation = ParseCoordinatesInformation(jsonfile);

                ClearPLannedData();

                ClearActualData();

                for(int i = 0; i < coordinatesInformation.plannedCoordinates.Count; i++)
                {                    
                    PlanCooridnatesList.Add(new PointLatLng(coordinatesInformation.plannedCoordinates[i].Lat, coordinatesInformation.plannedCoordinates[i].Lng));
                    DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(PlanCooridnatesList, "single_line") { Stroke = new Pen(Color.DarkRed, 2) }, "Planned");
                }
                
                for(int i = 0; i < coordinatesInformation.actualCoordinates.Count; i++)
                {
                    ActualCooridnatesList.Add(new PointLatLng(coordinatesInformation.actualCoordinates[i].Lat, coordinatesInformation.actualCoordinates[i].Lng));
                    DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(ActualCooridnatesList, "single_line") { Stroke = new Pen(Color.DarkGreen, 2) }, "Actual");
                }
        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveBtAutoUCClickHandler(object sender, EventArgs e)
        {
            
            try
            {
                CoordinatesInfo coordinatesInformation = CreateCoordinatesInformation();

                WriteJsonFile(coordinatesInformation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void autoUC1_MouseClick(object sender, MouseEventArgs e)
        {
            if(PlanMapEnable && (e.Button == MouseButtons.Right))
            {
                autoUC1.gmap.Overlays.Clear();
                PlanCooridnatesList.Add(autoUC1.gmap.FromLocalToLatLng(e.X, e.Y));
                GMap.NET.WindowsForms.GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                    PlanCooridnatesList[PlanCooridnatesList.Count - 1],
                    GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_big_stop);
                DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(PlanCooridnatesList, "single_line") { Stroke = new Pen(Color.LightGoldenrodYellow, 3) }, "Planned", marker);
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
            if(AutoEnabled)
            {
                try
                {
                    string mess = "";
                    for(int i = 0; i < PlanCooridnatesList.Count; i++)
                    {
                        mess += PlanCooridnatesList[i].Lat.ToString() + "," + PlanCooridnatesList[i].Lng.ToString() + ",";
                    }
                    mess = mess.Remove(mess.Count() - 1, 1);
                    mess = "VPLAN," + PlanCooridnatesList.Count.ToString() + "," + mess;
                    serialPort1.Write(MessagesDocker(mess));
                    autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Sending map coordinates\r\n";
                    autoUC1.SentTb.Text += "Message sent: " + MessagesDocker(mess);
                    StartWaitingForResponse();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please start this mode first.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearPlannedMapBtAutoUCClickHandler(object sender, EventArgs e)
        {
            ClearPLannedData();
        }

        private void ClearActualMapBtAutoUCClickHandler(object sender, EventArgs e)
        {
            ClearActualData();
        }

        private void AutoUCControlByKeyDown(object sender, KeyEventArgs e)  // *TODO: Consider to remove this later
        {
            
        }

        private void AutoUCControlByKeyUp(object sender, KeyEventArgs e)    // *TODO: Consider to remove this later
        {
            
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
        
        //---------------------------------------------------------------------------//
        // Helper Control

        private void CloseBtHelperUCClickHandler(object sender, EventArgs e)
        {
            helperControls1.SendToBack();
        }

        private void SettingVehicleHelperUCClickHandler(object sender, EventArgs e)
        {
            vehicleSetting1.BringToFront();
        }

        private void IMUSettingHelperUCClickHandler(object sender, EventArgs e)
        {
            imuSetting1.BringToFront();
        }

        private void ManualBtHelperUCClickHandler(object sender, EventArgs e)
        {
            manualUC1.BringToFront();
        }

        private void AutoBtHelperUCClickHandler(object sender, EventArgs e)
        {
            autoUC1.BringToFront();
        }

        private void KctrlBtHelperUCClickHandler(object sender, EventArgs e)
        {
            KctrlChangeMode();
        }

        private void OnSendDataBtHelperUCClickHandler(object sender, EventArgs e)
        {
            ControlSendDataFromVehicle(0);
        }

        private void OffSendDataBtHelperUCClickHandler(object sender, EventArgs e)
        {
            ControlSendDataFromVehicle(1);
        }
        
        //--------------------------------------------------------------------------//

        // TIMERS ------------------------------------------------------------------//

        private void timer1_Tick(object sender, EventArgs e) // Main timer for displaying data of vehicle
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
                            string[] value;
                            value = mess.Split(',');
                            if(value[0] == "$VINFO" && (value.Count() == 16))
                            {
                                temp = mess;
                                temp = temp.Remove(temp.Length - 3, 3);
                                temp = temp.Remove(0, 1);
                                if (true)
                                {
                                    MyVehicle = new Vehicle(value);
                                    manualUC1.VehicleStatusBox.Text = MyVehicle.GetVehicleStatus();
                                    // Save Position Data & Draw On Map
                                    manualUC1.PositionBox.Text += mess + "\r\n";
                                    manualUC1.PositionBox.Text += MyVehicle.PosX.ToString() + "," + MyVehicle.PosY.ToString() + "\r\n";
                                    if (MyVehicle.GPSStatus)
                                    {
                                        // Save Position Data & Draw On Map
                                        ActualCooridnatesList.Add(new GMap.NET.PointLatLng(MyVehicle.Lat, MyVehicle.Lng));
                                        if (online)
                                        {
                                            DisplayRouteOnMap(manualUC1.gmap, new GMapRoute(ActualCooridnatesList, "single_line") { Stroke = new Pen(Color.DarkGreen, 2) }, "Actual");
                                        }
                                        else
                                        {
                                            manualUC1.chart1.Series["Position"].Points.AddXY(MyVehicle.PosX/100000, MyVehicle.PosY/100000);
                                        }
                                    }

                                    
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }                
                    }
                }
            }
            else if(AutoEnabled)              //Change mode a little bit here, *TODO: Change it back later
            {
                if (serialPort1.IsOpen)
                {
                    if (serialPort1.BytesToRead != 0)
                    {                
                        try
                        {
                            string temp;
                            string mess = serialPort1.ReadLine();
                            string[] value;
                            value = mess.Split(',');
                            if (value[0] == "$VINFO" && (value.Count() == 16))
                            {
                                autoUC1.ReceivedTb.Text += mess + "\r\n";
                                temp = mess;
                                temp = temp.Remove(temp.Length - 3, 3);
                                temp = temp.Remove(0, 1);
                                //if (value[14].Contains(checksum(temp)))
                                if(true)
                                {
                                    MyVehicle = new Vehicle(value);
                                    autoUC1.DetailInfoTb.Text = MyVehicle.GetVehicleStatus();
                                    /* If GPS Status is OK, then draw the positions of the vehicle on MAP. Otherwise, skip drawing positions. */
                                    if (MyVehicle.GPSStatus)
                                    {
                                        
                                        // Save Position Data & Draw On Map
                                        autoUC1.ReceivedTb.Text += mess;
                                        ActualCooridnatesList.Add(new GMap.NET.PointLatLng(MyVehicle.Lat, MyVehicle.Lng));
                                        GMap.NET.WindowsForms.GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                                            new GMap.NET.PointLatLng(MyVehicle.Lat, MyVehicle.Lng),
                                            GMap.NET.WindowsForms.Markers.GMarkerGoogleType.orange_dot);
                                        DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(ActualCooridnatesList, "single_line") { Stroke = new Pen(Color.Red, 3) }, "Actual", marker);
                                    }
                                    /*  Draw turning State of vehicle by subtracting the RefAngle and the ActualAngle
                                        (It help users to understand whether the vehicle is turning left or right)      */
                                    DrawVehicleTurningStatusOnImage(autoUC1.VehicleStatusImage, - MyVehicle.RefAngle + MyVehicle.Angle, LowVelocity);
                                    autoUC1.TurningState.Text = "Turning " + Math.Round(- MyVehicle.RefAngle + MyVehicle.Angle, 4).ToString() + "°";
                                }
                            }
                            if(value[0] == "$KCTRL")
                            {
                                SendCommandSuccessfully = true;
                            }
                            if(value[0] == "$SINFO")
                            {
                                autoUC1.ReceivedTb.Text += mess + "\r\n";
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
                DisableAllConfigTextBoxes();
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
                        autoUC1.ReceivedTb.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + " Done.\r\n";
                        timeoutAuto = 40;
                        AutoTimer.Enabled = false;
                    }
                }
            }
            else
            {
                timeoutAuto = 40;
                autoUC1.ReceivedTb.Text = OldMess + DateTime.Now.ToString("h:mm:ss tt") + " Request failed\r\n";
                AutoTimer.Enabled = false;
            }
        }

        private void KcontrolTimer_Tick(object sender, EventArgs e)
        {
            KcontrolWaitTimes--;
            if (KcontrolWaitTimes >= 0)
            {
                if(SendCommandSuccessfully)
                {
                    KcontrolWaitTimes = defaultwaitTimes;
                    SendCommandSuccessfully = false;
                    KcontrolTimer.Enabled = false;
                }
                else
                {
                    SendCommandAsync();
                }
            }
            else
            {
                KcontrolWaitTimes = defaultwaitTimes;
                SendCommandSuccessfully = false;
                KcontrolTimer.Enabled = false;                
            }
        }
        
        private void StartKctrlTimer_Tick(object sender, EventArgs e)
        {
            if(KctrlTimerTimes > 0)
            {
                KctrlTimerTimes--;
                if(serialPort1.IsOpen && serialPort1.BytesToRead != 0)
                {
                    if (KctrlEnabled)
                    {
                        try
                        {
                            string mess = serialPort1.ReadLine();
                            string[] value;
                            value = mess.Split(',');
                            if (value[0] == "$SINFO")
                            {
                                StartKctrlTimer.Enabled = false;
                                KctrlEnabled = false;
                                KctrlTimerTimes = 20;
                                helperControls1.KctrlBt.LabelText = "KCTRL-OFF (F5)";
                                MessageBox.Show("Stoped KCTRL successfully");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            StartKctrlTimer.Enabled = false;
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            string mess = serialPort1.ReadLine();
                            string[] value;
                            value = mess.Split(',');
                            if (value[0] == "$SINFO")
                            {
                                StartKctrlTimer.Enabled = false;
                                KctrlEnabled = true;
                                KctrlTimerTimes = 20;
                                helperControls1.KctrlBt.LabelText = "KCTRL-ON (F5)";
                                MessageBox.Show("Started KCTRL successfully");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            StartKctrlTimer.Enabled = false;
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                StartKctrlTimer.Enabled = false;
                KctrlTimerTimes = 20;
                MessageBox.Show("No respone from vehicle");
            }
        }

        private void DefaultWaitForResponseTimer_Tick(object sender, EventArgs e)
        {
            if(DefaultWaitTimes > 0)
            {
                DefaultWaitTimes--;
                if(serialPort1.IsOpen && serialPort1.BytesToRead != 0)
                {
                    string mess = serialPort1.ReadLine();
                    string[] value;
                    value = mess.Split(',');
                    if (value[0] == "$SINFO")
                    {
                        DefaultWaitForResponseTimer.Enabled = false;
                        DefaultWaitTimes = 20;
                        MessageBox.Show("Command sent to vehicle successfully");
                        return;
                    }
                }
            }
            else
            {
                DefaultWaitForResponseTimer.Enabled = false;
                DefaultWaitTimes = 20;
                MessageBox.Show("Command sent to vehicle failed");
            }
        }

        //---------------------------------------------------------------------------//

        // Other functions ----------------------------------------------------------//

        public void DrawVehicleTurningStatusOnImage(PictureBox ImgBox, double angle, Image image)
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
            g.RotateTransform((float)angle);

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

        private string MessagesDocker(string RawMess)
        {
            string MessWithoutKey = RawMess + ",";

            return "$" + MessWithoutKey + checksum(MessWithoutKey) + "\r\n";
        }

        private void WriteJsonFile(CoordinatesInfo listCoordinates)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    sfd.FilterIndex = 2;

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(listCoordinates, Formatting.Indented));
                    }
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private CoordinatesInfo CreateCoordinatesInformation()
        {
            List<PlannedCoordinate> listPlanned = new List<PlannedCoordinate>();

            List<ActualCoordinate> listActual = new List<ActualCoordinate>();

            CoordinatesInfo CoordinatesInformation = new CoordinatesInfo();

            for (int i = 0; i < PlanCooridnatesList.Count; i++)
            {
                PlannedCoordinate plannedCoordinates = new PlannedCoordinate();
                plannedCoordinates.Lat = PlanCooridnatesList[i].Lat;
                plannedCoordinates.Lng = PlanCooridnatesList[i].Lng;
                listPlanned.Add(plannedCoordinates);        
            }

            for (int i = 0; i < ActualCooridnatesList.Count; i++)
            {
                ActualCoordinate actualCoordinates = new ActualCoordinate();
                actualCoordinates.Lat = Convert.ToDouble(ActualCooridnatesList[i].Lat);
                actualCoordinates.Lng = Convert.ToDouble(ActualCooridnatesList[i].Lng);
                listActual.Add(actualCoordinates);
            }

            CoordinatesInformation.plannedCoordinates=listPlanned;

            CoordinatesInformation.actualCoordinates=listActual;

            return CoordinatesInformation;
        }

        private string ReadJsonFile()
        {
            string text = "";
            try
            {
                using (var sfd = new OpenFileDialog())
                {
                    sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    sfd.FilterIndex = 2;

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        text = File.ReadAllText(sfd.FileName);                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return text;
        }

        private CoordinatesInfo ParseCoordinatesInformation(string text)
        {
            CoordinatesInfo CoordinatesInformation = JsonConvert.DeserializeObject<CoordinatesInfo>(text);

            return CoordinatesInformation;
        }

        private void ClearPLannedData()
        {
            autoUC1.gmap.Overlays.Clear();
            PlanCooridnatesList.Clear();
            PlanLines.Clear();
        }

        private void ClearActualData()
        {
            autoUC1.gmap.Overlays.Clear();
            ActualCooridnatesList.Clear();
            ActualLines.Clear();
        }

        private void DisplayRouteOnMap(GMapControl map, GMapRoute route, string mode, GMapMarker marker=null)
        {
            if(mode.Contains("Plan"))
            {
                PlanLines.Routes.Clear();
                PlanLines.Routes.Add(route);
                GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
                markers.Markers.Add(marker);
                map.Overlays.Add(markers);
                map.Overlays.Add(PlanLines);
                double zoom = autoUC1.gmap.Zoom;

                autoUC1.gmap.Zoom = 18;
                autoUC1.gmap.Zoom = zoom;
                //map.Show();
            }
            else
            {
                ActualLines.Routes.Clear();
                ActualLines.Routes.Add(route);
                GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
                markers.Markers.Add(marker);
                map.Overlays.Add(markers);
                map.Overlays.Add(ActualLines);
                double zoom = autoUC1.gmap.Zoom;

                autoUC1.gmap.Zoom = 18;
                autoUC1.gmap.Zoom = zoom;
                //map.Show();
            }
        }

        private void SetCommandForKcontrol()
        {
            KcontrolWaitTimes = defaultwaitTimes;
            KcontrolTimer.Enabled = true;
            SendCommandAsync();
        }

        private void DisableAllTimers()
        {
            timer1.Enabled = false;
            KcontrolTimer.Enabled = false;
            StartKctrlTimer.Enabled = false;
            DefaultWaitForResponseTimer.Enabled = false;
            ManualEnabled = false;
            AutoEnabled = false;
            KctrlEnabled = false;
        }

        private void DisableAllConfigTextBoxes()
        {
            vehicleSetting1.Kp1.Enabled = false;
            vehicleSetting1.Ki1.Enabled = false;
            vehicleSetting1.Kd1.Enabled = false;
            vehicleSetting1.Kp2.Enabled = false;
            vehicleSetting1.Ki2.Enabled = false;
            vehicleSetting1.Kd2.Enabled = false;
            vehicleSetting1.Velocity.Enabled = false;
            vehicleSetting1.Angle.Enabled = false;
            vehicleSetting1.Mode.Enabled = false;
        }

        private void EnableAllConfigTextBoxes()
        {
            vehicleSetting1.Kp1.Enabled = true;
            vehicleSetting1.Ki1.Enabled = true;
            vehicleSetting1.Kd1.Enabled = true;
            vehicleSetting1.Kp2.Enabled = true;
            vehicleSetting1.Ki2.Enabled = true;
            vehicleSetting1.Kd2.Enabled = true;
            vehicleSetting1.Velocity.Enabled = true;
            vehicleSetting1.Angle.Enabled = true;
            vehicleSetting1.Mode.Enabled = true;
        }

        private void KctrlChangeMode()
        {
            if (!KctrlEnabled && StartKctrlTimer.Enabled == false)
            {
                try
                {
                    DisableAllTimers();
                    serialPort1.Write(MessagesDocker("KCTRL,1"));
                    StartKctrlTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (KctrlEnabled && StartKctrlTimer.Enabled == false)
            {
                try
                {
                    serialPort1.Write(MessagesDocker("KCTRL,0"));
                    StartKctrlTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ControlSendDataFromVehicle(int mode)
        {
            if(mode == 0)
            {
                // Allow vehicle to send data
                try
                {
                    serialPort1.Write(MessagesDocker("VEHCF,DATA,1"));
                    DisableAllTimers();
                    DefaultWaitForResponseTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    DefaultWaitForResponseTimer.Enabled = false;
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    serialPort1.Write(MessagesDocker("VEHCF,DATA,0"));
                    DisableAllTimers();
                    DefaultWaitForResponseTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    DefaultWaitForResponseTimer.Enabled = false;
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
