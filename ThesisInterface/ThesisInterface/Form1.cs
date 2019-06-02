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
using System.Diagnostics;

namespace ThesisInterface
{
    public partial class Form1 : Form
    {
        public class Vehicle
        {
            public double M1RefVelocity, M2RefVelocity, M1Velocity, M2Velocity, RefAngle, Angle, Test;

            public Vehicle(string[] ArrayInfo)
            {
                try
                {
                    M1RefVelocity = double.Parse(ArrayInfo[2], System.Globalization.CultureInfo.InvariantCulture);
                    M2RefVelocity = double.Parse(ArrayInfo[3], System.Globalization.CultureInfo.InvariantCulture);
                    M1Velocity = double.Parse(ArrayInfo[4], System.Globalization.CultureInfo.InvariantCulture);
                    M2Velocity = double.Parse(ArrayInfo[5], System.Globalization.CultureInfo.InvariantCulture);
                    RefAngle = double.Parse(ArrayInfo[6], System.Globalization.CultureInfo.InvariantCulture);
                    Angle = double.Parse(ArrayInfo[7], System.Globalization.CultureInfo.InvariantCulture);
                    Test = double.Parse(ArrayInfo[8], System.Globalization.CultureInfo.InvariantCulture);
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
                    + "Set Angle: " + RefAngle.ToString() + "\r\n"
                    + "Current Angle: " + Angle.ToString() + "\r\n"
                    +"Obstacle angle: " + Test.ToString() + "\r\n";

                return mess;
            }
        }

        public class GPS
        {
            public string GPS_Available, GPS_Mode;
            public double GPS_Lat, GPS_Lng;
            public GPS(string[] ArrayInfo)
            {
                try
                {
                    GPS_Available = ArrayInfo[2];
                    GPS_Lat = double.Parse(ArrayInfo[4], System.Globalization.CultureInfo.InvariantCulture);
                    GPS_Lng = double.Parse(ArrayInfo[5], System.Globalization.CultureInfo.InvariantCulture);
                    switch (ArrayInfo[3])
                    {
                        case "0":
                            GPS_Mode = "GPS Not Available";
                            break;
                        case "1":
                            GPS_Mode = "GPS Available";
                            break;
                        case "2":
                            GPS_Mode = "DGNSS";
                            break;
                        case "4":
                            GPS_Mode = "Fixed";
                            break;
                        case "5":
                            GPS_Mode = "Float";
                            break;
                        default:
                            GPS_Mode = "Parsed Error";
                            break;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            public string GetGPSStatus()
            {
                string mess;
                if (GPS_Available.Contains("Y"))
                {
                    mess = "GPS Mode: " + GPS_Mode + "\r\nPosition: " + GPS_Lat.ToString() + ", " + GPS_Lng.ToString() + "\r\n";
                }
                else
                {
                    mess = "GPS is not available\r\n";
                }

                return mess;
            }
        }

        public class StanleyControl
        {
            public double thetaE, thetaD, Delta, ErrorDistance;
            
            public StanleyControl(string []ArrayInfo)
            {
                try
                {
                    thetaE = double.Parse(ArrayInfo[2], System.Globalization.CultureInfo.InvariantCulture);
                    thetaD = double.Parse(ArrayInfo[3], System.Globalization.CultureInfo.InvariantCulture);
                    Delta = double.Parse(ArrayInfo[4], System.Globalization.CultureInfo.InvariantCulture);
                    ErrorDistance = double.Parse(ArrayInfo[5], System.Globalization.CultureInfo.InvariantCulture);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            public string GetStanleyControlStatus()
            {
                string mess = "Theta E: " + thetaE.ToString() + "\r\nTheta D: " 
                        + thetaD.ToString() + "\r\nDelta Angle: " 
                        + Delta.ToString() + "\r\nDistance Error: "
                        + ErrorDistance.ToString() + "\r\n";
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
        public bool NoError = true;

        public class CoordinatesInfo
        {
            public List<PlannedCoordinate> plannedCoordinates { get; set; }
            public List<ActualCoordinate> actualCoordinates { get; set; }
            public List<double> ErrorDistances { get; set; }
        }

        public class RootObject
        {
            public CoordinatesInfo coordinatesInfo { get; set; }
        }

        public class ProcessedMap
        {
            public List<double> lat { get; set; }
            public List<double> lng { get; set; }
            public List<double> x { get; set; }
            public List<double> y { get; set; }
        }
        ProcessedMap processedMap = new ProcessedMap();
        Vehicle MyVehicle;
        GPS MyGPS;
        StanleyControl MyStanleyControl;

        // SET DEFAULT IMAGE FOR VELOCITY OF VEHICLE, CONSIST OF: RED, ORANGE, YELLOW, GREEN
        Image ZeroVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\ZERO.png");

        Image LowVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\LOW.png");

        Image MediumVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\MEDIUM.png");

        Image HighVelocity = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\HIGH.png");

        public string Vehicle_Information = "", GPS_Information = "", StanleyControl_Information = "";

        //TODO: Set temporarily, fix later
        char KeyW = '!', KeyS = '!', KeyA = '!', KeyD = '!';
        int level = 4;
        int defaultLevel = 4;
        private void SendCommandAsync()
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write(MessagesDocker("KCTRL," + KeyW.ToString() + "," + KeyS.ToString() + "," + KeyA.ToString() + "," + KeyD.ToString() + "," + level.ToString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------
        private bool AutoSetting = false;

        public string ConfigMessToWait = "";
        
        public int timeout = 40;

        public int timeoutIMU = 40;

        public int timeoutAuto = 9;

        public string AutoWaitKey = "|";

        public string IMUWaitKey = "|";  // This is used for creating waiting messages...

        public string ConfigWaitKey = "|";

        public string OldMess = "";

        private string PrevMode;

        public int KcontrolWaitTimes = 3, defaultwaitTimes = 3;

        public bool PlanMapEnable = false, online = false;

        public List<PointLatLng> ProcessLatLng = new List<PointLatLng>();

        public List<PointLatLng> PlanCoordinatesList = new List<PointLatLng>();

        public List<PointLatLng> ActualCoordinatesList = new List<PointLatLng>();

        public List<double> DistanceErrors = new List<double>();

        public GMapOverlay PlanLines = new GMapOverlay("PlanLines");

        public GMapOverlay ActualLines = new GMapOverlay("ActualLines");
        
        public bool KctrlEnabled = false;

        public int KctrlTimerTimes = 20;

        private bool OnHelperPanel = false;

        private int DefaultWaitTimes = 20;

        private int ResendTimes = 8, ResendDefaultTimes = 8, ReWaitTimes = 45, ReWaitDefault = 45;

        private string ResendMess = "";

        BackgroundWorker TransferMapBackGroundWorker;
        public Form1()
        {
            InitializeComponent();
            InitForm();
            KeyPreview = true;
            KeyDown += new KeyEventHandler(Form1_KeyDown);
            KeyUp += new KeyEventHandler(Form1_KeyUp);
            TransferMapBackGroundWorker = new BackgroundWorker();
            TransferMapBackGroundWorker.WorkerReportsProgress = true;
            TransferMapBackGroundWorker.WorkerSupportsCancellation = true;

            TransferMapBackGroundWorker.DoWork += TransferMapBackGroundWorker_DoWork;
            TransferMapBackGroundWorker.ProgressChanged += TransferMapBackGroundWorker_ProgressChanged;
            TransferMapBackGroundWorker.RunWorkerCompleted += TransferMapBackGroundWorker_RunWorkerCompleted;
        }
        private void TransferMapBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            //string[] dirs = Directory.GetDirectories(txtFolderPath.Text);
            //float length = dirs.Length;
            //progressBar1.Invoke((Action)(() => progressBar1.Maximum = dirs.Length));
            //for (int i = 0; i < dirs.Length; i++)
            //{
            //    backgroundWorker1.ReportProgress((int)(i / length * 100));
            //    ScanDirectory(dirs[i], txtSearch.Text);
            //}

            //backgroundWorker1.ReportProgress(100);
            try
            {
                string text = File.ReadAllText(@"D:\Project\CubicSpline\map_out.txt");
                //MessageBox.Show(text.ToString());
                ProcessedMap processedMap = ParseProcessedMapInfo(text);
                //MessageBox.Show(processedMap.lng[0].ToString());
                List<PointLatLng> ProcessLatLng = new List<PointLatLng>();
                //PointLatLng point = new PointLatLng();
                ClearPLannedData();
                //ClearActualData();
                for (int i = 0; i < processedMap.lat.Count(); i++)
                {

                    double a = processedMap.lat[i];
                    double b = processedMap.lng[i];
                    PlanCoordinatesList.Add(new PointLatLng(a, b));
                }
                DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(PlanCoordinatesList, "single_line") { Stroke = new Pen(Color.LightGoldenrodYellow, 3) }, "Planned");
                //MessageBox.Show("Starting to send to vehicle...");

                if (AutoEnabled)
                {
                    try
                    {
                        serialPort1.Write(MessagesDocker("VPLAN,SPLINE," + processedMap.x.Count().ToString()));
                        System.Threading.Thread.Sleep(200);
                        for (int i = 0; (i < processedMap.x.Count) && !TransferMapBackGroundWorker.CancellationPending; i++)
                        {
                            TransferMapBackGroundWorker.ReportProgress((int)(100*i/processedMap.x.Count));
                            serialPort1.Write(MessagesDocker("VPLAN," + i.ToString() + "," + processedMap.x[i].ToString() + "," + processedMap.y[i].ToString()));
                            System.Threading.Thread.Sleep(200);

                        }
                        TransferMapBackGroundWorker.ReportProgress(100);
                        //serialPort1.Write(MessagesDocker("VPLAN,STOP"));
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TransferMapBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!TransferMapBackGroundWorker.CancellationPending)
            {
                //MessageBox.Show(e.ProgressPercentage.ToString());
                progressUC1.ProgressBar.Value = e.ProgressPercentage;
                progressUC1.ProgressBar.Update();
                progressUC1.ProgressBar.PerformLayout();
            }
        }

        private void TransferMapBackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (progressUC1.ProgressBar.Value < 100)
            {
                MessageBox.Show("Transfering map has been cancelled");
            }
            
            else
            {
                progressUC1.BringToFront();
            }
        }
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
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
                    KctrlChangeMode(0);
                    break;
                case 117: // Key = F6, Get all vehicle information
                    KctrlChangeMode(1);
                    break;

                case 118: // Key = F7, ON Send data from vehicle
                    ControlSendDataFromVehicle(0);
                    break;
                case 119: // Key = F8, OFF Send data from vehicle
                    ControlSendDataFromVehicle(1);
                    break;
                case 121:
                    serialPort1.Write(MessagesDocker("SFRST"));
                    DefaultWaitForResponseTimer.Enabled = true;
                    SetPreviousMode();
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
                    if(AutoSetting)
                    {
                        autoSetting1.SendToBack();
                        AutoSetting = false;
                    }
                    break;
            }
            if(KctrlEnabled)
            {
                if (e.KeyCode == Keys.W)
                {
                    if (KeyW != 'W')
                    {
                        KeyW = 'W';
                        level = defaultLevel;
                        SetCommandForKcontrol();

                    }
                }
                else if (e.KeyCode == Keys.S)
                {
                    if (KeyS != 'S')
                    {
                        KeyS = 'S';
                        level = defaultLevel;
                        SetCommandForKcontrol();
                    }
                }
                else if (e.KeyCode == Keys.A)
                {
                    if (KeyA != 'A')
                    {
                        KeyA = 'A';
                        level = Convert.ToInt16(0.5 * defaultLevel);
                        SetCommandForKcontrol();
                    }
                }
                else if (e.KeyCode == Keys.D)
                {
                    if (KeyD != 'D')
                    {
                        KeyD = 'D';
                        level = Convert.ToInt16(0.5 * defaultLevel);
                        SetCommandForKcontrol();
                    }
                }
                else if (e.KeyCode == Keys.E)
                {
                    if (defaultLevel >= 10)
                    {
                        defaultLevel = 10;
                        level = defaultLevel;
                    }
                    else
                    {
                        defaultLevel++;
                        level = defaultLevel;
                    }
                    SetCommandForKcontrol();
                }
                else if (e.KeyCode == Keys.Q)
                {
                    if (defaultLevel <= 0)
                    {
                        defaultLevel = 0;
                        level = defaultLevel;
                    }
                    else
                    {
                        defaultLevel--;
                        level = defaultLevel;
                    }
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
                    level = defaultLevel;
                    SetCommandForKcontrol();
                }
                else if (e.KeyCode == Keys.D)
                {
                    KeyD = '!';
                    level = defaultLevel;
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
        
        private void InitProgressBar()
        {
            progressUC1.ProgressBar.MaxValue = 100;
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
            this.autoUC1.SettingBtClickHandler(new EventHandler(SettingBtAutoUCClickHandler));
            this.autoSetting1.CancelButtonClickHandler(new EventHandler(CancelBtAutoUCClickHandler));
            this.autoSetting1.OkButtonClickHandler(new EventHandler(OkBtAutoUCClickHandler));
            this.autoSetting1.OffSelfUpdateBtClickHandler(new EventHandler(OffSelfUpdateAutoClickHandler));
            this.autoSetting1.OnSelfUpdateBtClickHandler(new EventHandler(OnSelfUpdateAutoClickHandler));
            this.autoUC1.CreatePreProcessingBtClickHandler(new EventHandler(CreatePreProcessingMapHandler));
            this.autoUC1.ImportProcessedMapBtClickHandler(new EventHandler(ImportProcessedMapHandler));


            this.helperControls1.CloseBtClickHandler(new EventHandler(CloseBtHelperUCClickHandler));
            this.helperControls1.SettingVehicleBtClickHandler(new EventHandler(SettingVehicleHelperUCClickHandler));
            this.helperControls1.IMUSettingBtClickHandler(new EventHandler(IMUSettingHelperUCClickHandler));
            this.helperControls1.ManualBtClickHandler(new EventHandler(ManualBtHelperUCClickHandler));
            this.helperControls1.AutoBtClickHandler(new EventHandler(AutoBtHelperUCClickHandler));
            this.helperControls1.KctrlBtClickHandler(new EventHandler(KctrlBtHelperUCClickHandler));
            this.helperControls1.OnSendDataBtClickHandler(new EventHandler(OnSendDataBtHelperUCClickHandler));
            this.helperControls1.OffSendDataBtClickHandler(new EventHandler(OffSendDataBtHelperUCClickHandler));

            this.progressUC1.HideBtClickHandler(new EventHandler(HideProgressBarBtClickHandler));
            this.progressUC1.CancelBtClickHandler(new EventHandler(CancelProgressBarBtClickHandler));
        }

        //--------------------------------------------------------------------------//
        //Handler for progress UC
        private void HideProgressBarBtClickHandler(object sender, EventArgs e)
        {
            this.progressUC1.SendToBack();
        }

        private void CancelProgressBarBtClickHandler(object sender, EventArgs e)
        {
            if(TransferMapBackGroundWorker.IsBusy)
            {
                TransferMapBackGroundWorker.CancelAsync();
                progressUC1.SendToBack();
                progressUC1.ProgressBar.Value = 0;
                progressUC1.ProgressBar.Update();
            }
        }

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
                serialPort1.Write(MessagesDocker("MACON,START"));
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
                serialPort1.Write(MessagesDocker("MACON,STOP"));
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
            DisableAllTimers();
            OldMess = autoUC1.ReceivedTb.Text;
            AutoTimer.Enabled = true;
            
        }

        private void StopVehicleBtAutoUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                DisableAllTimers();
                string mess = MessagesDocker("AUCON,PAUSE");
                serialPort1.Write(mess);
                autoUC1.SentTb.Text += DateTime.Now.ToString("hh:mm:ss") + " Stopping vehicle...\r\n";
                ResendMess = mess;
                SendWithResTimer.Enabled = true;
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
                string mess = MessagesDocker("AUCON,START");
                serialPort1.Write(mess);
                autoUC1.SentTb.Text += DateTime.Now.ToString("hh:mm:ss") + " Turning on auto mode\r\n";
                AutoTimer.Enabled = true;
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
                string mess = MessagesDocker("AUCON,STOP");
                serialPort1.Write(mess);
                autoUC1.SentTb.Text += DateTime.Now.ToString("hh:mm:ss") + " Turning off auto mode\r\n";
                ResendMess = mess;
                SendWithResTimer.Enabled = true;
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
                DisableAllTimers();
                string mess = MessagesDocker("AUCON,RUN");
                serialPort1.Write(mess);
                autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Starting to run...\r\n";
                ResendMess = mess;
                SendWithResTimer.Enabled = true;
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
                for(int i = 0; i < coordinatesInformation.plannedCoordinates.Count(); i++)
                {
                    PlanCoordinatesList.Add(new PointLatLng(coordinatesInformation.plannedCoordinates[i].Lat, coordinatesInformation.plannedCoordinates[i].Lng));
                    DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(PlanCoordinatesList, "single_line") { Stroke = new Pen(Color.LightGoldenrodYellow, 3) }, "Planned");
                }
                
                for (int i = 0; i < coordinatesInformation.actualCoordinates.Count; i++)
                {
                    
                    if (i>0)
                    {
                        double [] utm1 = LatLonToUTM(coordinatesInformation.actualCoordinates[i - 1].Lat, coordinatesInformation.actualCoordinates[i - 1].Lng);
                        double [] utm2 = LatLonToUTM(coordinatesInformation.actualCoordinates[i].Lat, coordinatesInformation.actualCoordinates[i].Lng);
                        double deltaError = Math.Abs(utm1[0] - utm2[0]);
                        if (deltaError > 10)
                        {
                            coordinatesInformation.actualCoordinates[i].Lat = coordinatesInformation.actualCoordinates[i - 1].Lat;
                            coordinatesInformation.actualCoordinates[i].Lng = coordinatesInformation.actualCoordinates[i - 1].Lng;
                        }
                        ActualCoordinatesList.Add(new PointLatLng(coordinatesInformation.actualCoordinates[i].Lat, coordinatesInformation.actualCoordinates[i].Lng));
                    }
                    else
                    {
                        ActualCoordinatesList.Add(new PointLatLng(coordinatesInformation.actualCoordinates[i].Lat, coordinatesInformation.actualCoordinates[i].Lng));
                    }
                    
                    DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(ActualCoordinatesList, "single_line") { Stroke = new Pen(Color.Red, 3) }, "Actual");
                }
                DistanceErrors.Clear();
                DistanceErrors = coordinatesInformation.ErrorDistances;
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
                PlanCoordinatesList.Add(autoUC1.gmap.FromLocalToLatLng(e.X, e.Y));
                GMap.NET.WindowsForms.GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                    PlanCoordinatesList[PlanCoordinatesList.Count - 1],
                    GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_big_stop);
                DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(PlanCoordinatesList, "single_line") { Stroke = new Pen(Color.LightGoldenrodYellow, 3) }, "Planned", marker);
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
                    serialPort1.Write(MessagesDocker("VPLAN,START"));
                    System.Threading.Thread.Sleep(300);
                    for (int i = 0; i < PlanCoordinatesList.Count; i++)
                    {
                        serialPort1.Write(MessagesDocker("VPLAN," + PlanCoordinatesList[i].Lat.ToString() + "," + PlanCoordinatesList[i].Lng.ToString()));
                        System.Threading.Thread.Sleep(300);
                    }

                    serialPort1.Write(MessagesDocker("VPLAN,STOP"));
                    autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Sending map coordinates\r\n";
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

        private void SettingBtAutoUCClickHandler(object sender, EventArgs e)
        {
            if(AutoSetting)
            {
                this.autoSetting1.SendToBack();
                AutoSetting = false;
            }
            else
            {
                this.autoSetting1.BringToFront();
                AutoSetting = true;
            }
        }

        private void OkBtAutoUCClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(MessagesDocker("AUCON,DATA," + autoSetting1.VelocityTb.Text + "," + autoSetting1.KGainTb.Text + ",0.5"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelBtAutoUCClickHandler(object sender, EventArgs e)
        {
            this.autoSetting1.SendToBack();
        }

        private void OffSelfUpdateAutoClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(MessagesDocker("AUCON,SUPDT,0"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSelfUpdateAutoClickHandler(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(MessagesDocker("AUCON,SUPDT,1"));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreatePreProcessingMapHandler(object sender, EventArgs e)
        {
            
            try
            {
                if (PlanCoordinatesList.Count > 0)
                {
                    ProcessedMap processedMap = new ProcessedMap();
                    List<double> listLat = new List<double>();
                    List<double> listLng = new List<double>();

                    for (int i = 0; i < PlanCoordinatesList.Count; i++)
                    {
                        double lat, lng;
                        lat = PlanCoordinatesList[i].Lat;
                        lng = PlanCoordinatesList[i].Lng;
                        listLat.Add(lat);
                        listLng.Add(lng);
                    }
                    processedMap.lat = listLat;
                    processedMap.lng = listLng;

                    File.WriteAllText(@"D:\Project\CubicSpline\pre_map.txt", JsonConvert.SerializeObject(processedMap, Formatting.Indented));
                    MessageBox.Show("Processing Map...");
                }
                else
                {
                    MessageBox.Show("No map to process");
                    return;
                }
                var processInfo = new ProcessStartInfo("cmd.exe", "D:/Project/CubicSpline/cmd.bat");
                processInfo.WorkingDirectory = "D:/Project/CubicSpline/";
                processInfo.FileName = "execute_creating_map.bat";
                //Do not create command propmpt window 
                processInfo.CreateNoWindow = true;

                //Do not use shell execution
                processInfo.UseShellExecute = false;

                //Redirects error and output of the process (command prompt).
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;

                //start a new process
                var process = Process.Start(processInfo);

                //wait until process is running
                process.WaitForExit();

                //reads output and error of command prompt to string.
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportProcessedMapHandler(object sender, EventArgs e)
        {
            //try
            //{
            //    string text = File.ReadAllText(@"D:\Project\CubicSpline\map_out.txt");
            //    //MessageBox.Show(text.ToString());
            //    ProcessedMap processedMap = ParseProcessedMapInfo(text);
            //    //MessageBox.Show(processedMap.lng[0].ToString());
            //    List<PointLatLng> ProcessLatLng = new List<PointLatLng>();
            //    //PointLatLng point = new PointLatLng();
            //    ClearPLannedData();
            //    //ClearActualData();
            //    for (int i = 0; i < processedMap.lat.Count(); i++)
            //    {

            //        double a = processedMap.lat[i];
            //        double b = processedMap.lng[i];
            //        PlanCoordinatesList.Add(new PointLatLng(a, b));
            //    }
            //    DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(PlanCoordinatesList, "single_line") { Stroke = new Pen(Color.LightGoldenrodYellow, 3) }, "Planned");
            //    MessageBox.Show("Starting to send to vehicle...");

            //    if (AutoEnabled)
            //    {
            //        try
            //        {
            //            serialPort1.Write(MessagesDocker("VPLAN,SPLINE," + processedMap.x.Count().ToString()));
            //            System.Threading.Thread.Sleep(200);
            //            for (int i = 0; i < processedMap.x.Count; i++)
            //            {
            //                serialPort1.Write(MessagesDocker("VPLAN," + i.ToString() + "," + processedMap.x[i].ToString() + "," + processedMap.y[i].ToString()));
            //                System.Threading.Thread.Sleep(200);
            //            }

            //            //serialPort1.Write(MessagesDocker("VPLAN,STOP"));
            //            autoUC1.SentTb.Text += DateTime.Now.ToString("h:mm:ss tt") + " Sent map coordinates\r\n";
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please start this mode first.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            progressUC1.ProgressBar.Value = 0;
            progressUC1.ProgressBar.Update();
            TransferMapBackGroundWorker.RunWorkerAsync();
            progressUC1.BringToFront();
        }

        private void InitAutoUC()
        {
            AutoTimer.Interval = 100;
            AutoTimer.Enabled = false;
            autoUC1.gmap.DragButton = MouseButtons.Left;
            autoUC1.gmap.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            autoUC1.gmap.SetPositionByKeywords("Vietnam, Ho Chi Minh");
            autoUC1.gmap.Position = new PointLatLng(10.772801, 106.659273);
            autoUC1.gmap.Zoom = 19;
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
            KctrlChangeMode(0);
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
                            temp = mess;
                            if(mess.Length >= 5)
                            {
                                temp = temp.Remove(temp.Length - 3, 3);
                                temp = temp.Remove(0, 1);
                            }
                            string CC = checksum(temp);

                            // Update Vehicle Information
                            if (mess.Contains("$VINFO,0") && mess.Contains(CC))
                            {
                                MyVehicle = new Vehicle(value);
                                Vehicle_Information = MyVehicle.GetVehicleStatus();
                            }

                            // Update GPS Information
                            if (mess.Contains("$VINFO,1") && mess.Contains(CC))
                            {
                                MyGPS = new GPS(value);
                                GPS_Information = MyGPS.GetGPSStatus();
                            }

                            // Update Stanley Information
                            if (mess.Contains("$VINFO,2") && mess.Contains(CC))
                            {
                                MyStanleyControl = new StanleyControl(value);
                                StanleyControl_Information = MyStanleyControl.GetStanleyControlStatus();
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
                            if(mess.Contains("$"))
                            {
                                value = mess.Split(',');
                                temp = mess;
                                if (mess.Length >= 5)
                                {
                                    temp = temp.Remove(temp.Length - 3, 3);
                                    temp = temp.Remove(0, 1);
                                }
                                string CC = checksum(temp);

                                // Update Vehicle Information
                                if (mess.Contains("$VINFO,0") && mess.Contains(CC))
                                {
                                    MyVehicle = new Vehicle(value);
                                    Vehicle_Information = MyVehicle.GetVehicleStatus();

                                    /*  Draw turning State of vehicle by subtracting the RefAngle and the ActualAngle
                                    (It help users to understand whether the vehicle is turning left or right)      */
                                    DrawVehicleTurningStatusOnImage(autoUC1.VehicleStatusImage, -MyVehicle.RefAngle + MyVehicle.Angle, LowVelocity);
                                    autoUC1.TurningState.Text = "Turning " + Math.Round(-MyVehicle.RefAngle + MyVehicle.Angle, 4).ToString() + "°";
                                    //autoUC1.ReceivedTb.Text += mess;
                                }

                                // Update GPS Information
                                if (mess.Contains("$VINFO,1") && mess.Contains(CC))
                                {
                                    MyGPS = new GPS(value);
                                    GPS_Information = MyGPS.GetGPSStatus();
                                    autoUC1.ReceivedTb.Text += mess;
                                    if (MyGPS.GPS_Available.Contains("Y"))
                                    {

                                        // Save Position Data & Draw On Map

                                        ActualCoordinatesList.Add(new GMap.NET.PointLatLng(MyGPS.GPS_Lat, MyGPS.GPS_Lng));
                                        GMap.NET.WindowsForms.GMapMarker marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                                            new GMap.NET.PointLatLng(MyGPS.GPS_Lat, MyGPS.GPS_Lng),
                                            GMap.NET.WindowsForms.Markers.GMarkerGoogleType.orange_dot);
                                        DisplayRouteOnMap(autoUC1.gmap, new GMapRoute(ActualCoordinatesList, "single_line") { Stroke = new Pen(Color.Red, 3) }, "Actual", marker);
                                    }
                                }

                                // Update Stanley Information
                                if (mess.Contains("$VINFO,2") && mess.Contains(CC))
                                {
                                    MyStanleyControl = new StanleyControl(value);
                                    StanleyControl_Information = MyStanleyControl.GetStanleyControlStatus();
                                    DistanceErrors.Add(MyStanleyControl.ErrorDistance);
                                }
                                
                                /* If GPS Status is OK, then draw the positions of the vehicle on MAP. Otherwise, skip drawing positions. */

                                if (mess.Contains("SINFO,VPLAN,1"))
                                {
                                    SendCommandSuccessfully = true;
                                    autoUC1.ReceivedTb.Text += "Map planned successfully, ready to go\r\n";
                                }

                                if (mess.Contains("SINFO,1"))
                                {
                                    //MessageBox.Show("checked");
                                    SendCommandSuccessfully = true;
                                    autoUC1.SentTb.Text += "Command sent successfully\r\n";
                                }

                                if (mess.Contains("SINFO,VPLAN,0"))
                                {
                                    TransferMapBackGroundWorker.CancelAsync();
                                    autoUC1.ReceivedTb.Text += "Some errors happened when sending map\r\n";
                                    //NoError = false;
                                }

                                if(mess.Contains("SINFO,0"))
                                {
                                    autoUC1.SentTb.Text += "Command Error\r\n";

                                }

                                autoUC1.DetailInfoTb.Text = Vehicle_Information;
                                autoUC1.PosTb.Text = GPS_Information;
                                autoUC1.StanleyControlTb.Text = StanleyControl_Information;
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
                if (serialPort1.BytesToRead != 0)
                {
                    string mess = serialPort1.ReadLine();
                    if (mess.Contains("$SINFO,1"))
                    {
                        AutoTimer.Enabled = false;
                        autoUC1.ReceivedTb.Text = OldMess + DateTime.Now.ToString("hh:mm:ss") + " Done.\r\n";
                        PrevMode = "auto";
                        timeoutAuto = 9;
                        AutoEnabled = true;
                        timer1.Enabled = true;
                    }
                }
            }
            else
            {
                AutoTimer.Enabled = false;
                timeoutAuto = 9;
                autoUC1.ReceivedTb.Text = OldMess + DateTime.Now.ToString("hh:mm:ss") + " Request failed\r\n";
                
            }
        }

        private void KcontrolTimer_Tick(object sender, EventArgs e)
        {
            KcontrolWaitTimes--;
            if (KcontrolWaitTimes >= 0)
            {
                if(SendCommandSuccessfully)
                {
                    KcontrolTimer.Enabled = false;
                    KcontrolWaitTimes = defaultwaitTimes;
                    SendCommandSuccessfully = false;
                }
                else
                {
                    SendCommandAsync();
                }
            }
            else
            {
                KcontrolTimer.Enabled = false;
                KcontrolWaitTimes = defaultwaitTimes;
                SendCommandSuccessfully = false;
            }
        }
        
        private void StartKctrlTimer_Tick(object sender, EventArgs e)
        {
            if(KctrlTimerTimes > 0)
            {
                KctrlTimerTimes--;
                if(serialPort1.IsOpen && serialPort1.BytesToRead != 0)
                {
                        try
                        {
                            string mess = serialPort1.ReadLine();
                            string[] value;
                            value = mess.Split(',');
                            if (mess.Contains("$SINFO,1"))
                            {
                                StartKctrlTimer.Enabled = false;
                                KctrlEnabled = true;
                                KctrlTimerTimes = 20;
                                //helperControls1.KctrlBt.LabelText = "KCTRL-OFF (F5)";
                                MessageBox.Show("Started KCTRL successfully");
                                SetPreviousMode();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            StartKctrlTimer.Enabled = false;
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            SetPreviousMode();
                        }
                    }
            }
            else
            {
                StartKctrlTimer.Enabled = false;
                KctrlTimerTimes = 20;
                MessageBox.Show("No respone from vehicle");
                SetPreviousMode();
            }
        }

        private void StopKctrlTimer_Tick(object sender, EventArgs e)
        {
            if (KctrlTimerTimes > 0)
            {
                KctrlTimerTimes--;
                if (serialPort1.IsOpen && serialPort1.BytesToRead != 0)
                {
                    try
                    {
                        string mess = serialPort1.ReadLine();
                        string[] value;
                        value = mess.Split(',');
                        if (mess.Contains("$SINFO,1"))
                        {
                            StopKctrlTimer.Enabled = false;
                            KctrlEnabled = false;
                            KctrlTimerTimes = 20;
                            //helperControls1.KctrlBt.LabelText = "KCTRL-OFF (F5)";
                            MessageBox.Show("Stoped KCTRL successfully");
                            SetPreviousMode();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        StopKctrlTimer.Enabled = false;
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        SetPreviousMode();
                    }
                }
            }
            else
            {
                StopKctrlTimer.Enabled = false;
                KctrlTimerTimes = 20;
                MessageBox.Show("No respone from vehicle");
                SetPreviousMode();
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
                    if (mess.Contains("$SINFO,1"))
                    {
                        DefaultWaitForResponseTimer.Enabled = false;
                        SendCommandSuccessfully = false;
                        DefaultWaitTimes = 20;
                        MessageBox.Show("Command sent to vehicle successfully");
                        return;
                    }
                }
                else if (SendCommandSuccessfully)
                {
                    DefaultWaitForResponseTimer.Enabled = false;
                    SendCommandSuccessfully = false;
                    DefaultWaitTimes = 20;
                    MessageBox.Show("Command sent to vehicle successfully");
                    return;
                }
            }
            else
            {
                DefaultWaitForResponseTimer.Enabled = false;
                DefaultWaitTimes = 20;
                MessageBox.Show("Command sent to vehicle failed");
            }
        }


        private void SendWithResTimer_Tick(object sender, EventArgs e)
        {
            if (ReWaitTimes > 0)
            {
                ReWaitTimes--;
                if (ResendTimes > 0)
                {
                    ResendTimes--;
                }
                else
                {
                    if (ResendMess != "")
                    {
                        serialPort1.Write(ResendMess);
                    }
                    ResendTimes = ResendDefaultTimes;
                }

                if (serialPort1.BytesToRead != 0)
                {
                    string mess = serialPort1.ReadLine();
                    if (mess.Contains("$SINFO,1"))
                    {
                        SendWithResTimer.Enabled = false;
                        ReWaitTimes = ReWaitDefault;
                        ResendTimes = ResendDefaultTimes;
                        MessageBox.Show("Command sent to vehicle successfully");
                        SetPreviousMode();
                        return;
                    }
                }
            }
            else
            {
                SendWithResTimer.Enabled = false;
                ReWaitTimes = ReWaitDefault;
                ResendTimes = ResendDefaultTimes;
                MessageBox.Show("Command sent to vehicle failed");
                SetPreviousMode();
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

            for (int i = 0; i < PlanCoordinatesList.Count; i++)
            {
                PlannedCoordinate plannedCoordinates = new PlannedCoordinate();
                plannedCoordinates.Lat = PlanCoordinatesList[i].Lat;
                plannedCoordinates.Lng = PlanCoordinatesList[i].Lng;
                listPlanned.Add(plannedCoordinates);        
            }

            for (int i = 0; i < ActualCoordinatesList.Count; i++)
            {
                ActualCoordinate actualCoordinates = new ActualCoordinate();
                actualCoordinates.Lat = Convert.ToDouble(ActualCoordinatesList[i].Lat);
                actualCoordinates.Lng = Convert.ToDouble(ActualCoordinatesList[i].Lng);
                listActual.Add(actualCoordinates);
            }

            CoordinatesInformation.plannedCoordinates = listPlanned;

            CoordinatesInformation.actualCoordinates = listActual;

            CoordinatesInformation.ErrorDistances = DistanceErrors;

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

        private ProcessedMap ParseProcessedMapInfo(string text)
        {
            ProcessedMap processedMap = JsonConvert.DeserializeObject<ProcessedMap>(text);
            return processedMap;
        }

        private void ClearPLannedData()
        {
            //autoUC1.gmap.Overlays.Clear();
            PlanCoordinatesList.Clear();
            PlanLines.Clear();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            progressUC1.Location = new Point(this.Width / 2 - progressUC1.Width / 2, this.Height/2 - progressUC1.Height/2);
            autoSetting1.Location = new Point(this.Width / 2 - autoSetting1.Width / 2, this.Height / 2 - autoSetting1.Height / 2);
            helperControls1.Location = new Point(this.Width / 2 - helperControls1.Width / 2, this.Height / 2 - helperControls1.Height / 2);
            //MessageBox.Show(this.Height.ToString());
        }

        private void ClearActualData()
        {
            //autoUC1.gmap.Overlays.Clear();
            ActualCoordinatesList.Clear();
            ActualLines.Clear();
            DistanceErrors.Clear();
        }

        private void DisplayRouteOnMap(GMapControl map, GMapRoute route, string mode, GMapMarker marker=null)
        {
            try
            {
                if (mode.Contains("Plan"))
                {
                    
                    //PlanLines.Routes.Clear();
                    if(marker != null)
                    {
                        GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
                        map.Overlays.Add(markers);
                        markers.Markers.Add(marker);
                    }
                    map.Overlays.Add(PlanLines);
                    map.Overlays.Add(ActualLines);
                    PlanLines.Routes.Add(route);
                }
                else
                {
                    map.Overlays.Clear();
                    ActualLines.Routes.Clear();
                    if(marker != null)
                    {
                        GMap.NET.WindowsForms.GMapOverlay markers = new GMap.NET.WindowsForms.GMapOverlay("markers");
                        map.Overlays.Add(markers);
                        markers.Markers.Add(marker);
                    }
                    map.Overlays.Add(ActualLines);
                    map.Overlays.Add(PlanLines);
                    ActualLines.Routes.Add(route);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            AutoEnabled = false;
            KcontrolTimer.Enabled = false;
            StartKctrlTimer.Enabled = false;
            DefaultWaitForResponseTimer.Enabled = false;
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

        private void KctrlChangeMode(int mode)
        {
            if (mode == 0)
            {
                try
                {
                    DisableAllTimers();
                    serialPort1.Write(MessagesDocker("KCTRL,START,1"));
                    StartKctrlTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (mode == 1)
            {
                try
                {
                    DisableAllTimers();
                    serialPort1.Write(MessagesDocker("KCTRL,STOP"));
                    StopKctrlTimer.Enabled = true;
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
                    DisableAllTimers();
                    string mess = MessagesDocker("VEHCF,DATA,1");
                    serialPort1.Write(mess);
                    ResendMess = mess;
                    SendWithResTimer.Enabled = true;
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
                    DisableAllTimers();
                    string mess = MessagesDocker("VEHCF,DATA,0");
                    serialPort1.Write(mess);
                    ResendMess = mess;
                    SendWithResTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetPreviousMode()
        {
            switch(PrevMode)
            {
                case "auto":
                    AutoEnabled = true;
                    timer1.Enabled = true;
                    break;
                case "":
                    break;

            }
                
            AutoEnabled = true;
        }

        private double[] LatLonToUTM(double Lat, double Lon)
        {
            double[] result = new double[2];
            double la, lo, lat, lon, sa, sb, e2, e2cuadrada, c, Huso, S, deltaS, a, epsilon, nu, v, ta, a1, a2, j2, j4, j6, alfa, beta, gama, Bm, xx, yy;
            la = Lat;
            lo = Lon;
            sa = 6378137.000000;
            sb = 6356752.314245;
            e2 = Math.Pow((Math.Pow(sa, 2) - Math.Pow(sb, 2)), 0.5) / sb;
            e2cuadrada = Math.Pow(e2, 2);
            c = Math.Pow(sa, 2) / sb;
            lat = la * (Math.PI / 180);
            lon = lo * (Math.PI / 180);
            Huso = Math.Round(lo / 6, 0) + 31;
            S = ((Huso * 6) - 183);
            deltaS = lon - (S * (Math.PI / 180));
            a = Math.Cos(lat) * Math.Sin(deltaS);
            epsilon = 0.5 * Math.Log((1 + a) / (1 - a));
            nu = Math.Atan(Math.Tan(lat) / Math.Cos(deltaS)) - lat;
            v = (c / Math.Pow((1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2))), 0.5d)) * 0.9996;
            ta = (e2cuadrada / 2) * Math.Pow(epsilon, 2) * Math.Pow(Math.Cos(lat), 2);
            a1 = Math.Sin(2 * lat);
            a2 = a1 * Math.Pow(Math.Cos(lat), 2);
            j2 = lat + (a1 / 2);
            j4 = ((3 * j2) + a2) / 4;
            j6 = ((5 * j4) + (a2 * Math.Pow(Math.Cos(lat), 2))) / 3;
            alfa = (3d / 4) * e2cuadrada;
            beta = (5d / 3) * Math.Pow(alfa, 2);
            gama = (35d / 27) * Math.Pow(alfa, 3);
            Bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            xx = epsilon * v * (1 + (ta / 3)) + 500000;
            yy = nu * v * (1 + ta) + Bm;
            if (yy < 0)
            {
                yy = 9999999 + yy;
            }
            result[0] = xx;
            result[1] = yy;
            return result;
        }

    }
}
