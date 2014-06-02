using System;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace thinkgear_form
{
    public partial class ProgramForm : Form
    {
        #region Global Vars
        /// <summary>
        /// Specify COM port that MindWave is connected to.
        /// 
        /// On Windows, COM10 and higher must be preceded by \\.\, as in
        /// "\\\\.\\COM12" (must escape backslashes in strings).  COM9
        /// and lower do not require the \\.\, but are allowed to include
        /// them.  On Mac OS X, COM ports are named like 
        /// "/dev/tty.MindSet-DevB-1".
        /// </summary>
        static string portName = @"\\.\COM3";

        /// <summary>
        /// connection ID for ThinkGear device
        /// </summary>
        static int connectionId;

        /// <summary>
        /// BackgroundWorker to monitor ThinkGear data
        /// </summary>
        BackgroundWorker _Worker;
        #endregion

        /// <summary>
        /// Initialize program
        /// </summary>
        public ProgramForm()
        {
            InitializeComponent();

            // update start button text
            btnStart.Text = "Start connection on port " + portName;

            // List out Serial ports
            string[] _ports = SerialPort.GetPortNames();
            cmboCOM.Items.AddRange(_ports);
            
            // check if users has selected something in the past
            int selectedPort = 0;
            int defaultPort = Properties.Settings.Default.userCOMPort;

            if (defaultPort >= cmboCOM.Items.Count)
                selectedPort = 0;
            else
                selectedPort = defaultPort;
            cmboCOM.SelectedIndex = selectedPort;

            // ini BackgroundWorker
            _Worker = new BackgroundWorker();
            _Worker.DoWork += ThinkGearWorker;
            _Worker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Start/Stop button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start collecting data
            if (btnStart.Text.StartsWith("Start"))
            {
                bool StartSuccess = true;

                // update COM port
                Properties.Settings.Default.userCOMPort = cmboCOM.SelectedIndex; // remember user's selection
                Properties.Settings.Default.Save();
                portName = @"\\.\" + (string)cmboCOM.SelectedItem;

                // Update text
                btnStart.Enabled = false;
                lstBoxLog.Items.Add("Starting data collection on ThinkGear DLL version: " +
                    ThinkGear.TG_GetDriverVersion().ToString());

                // Request connection ID
                connectionId = ThinkGear.TG_GetNewConnectionId();
                if (connectionId < 0)
                {
                    lstBoxLog.Items.Add("ERROR: TG_GetNewConnectionId() returned "
                                      + connectionId.ToString());
                    StartSuccess = false;
                }

                // start Stream/Data Loggers
                if (!_Log(connectionId, "data_log.txt", ThinkGear.TG_SetDataLog) ||
                    !_Log(connectionId, "stream_log.txt", ThinkGear.TG_SetStreamLog))
                    StartSuccess = false;

                // Do connection
                int errCode = ThinkGear.TG_Connect(connectionId,
                                           portName,
                                           ThinkGear.BAUD_57600,
                                           ThinkGear.STREAM_PACKETS);
                if (errCode != 0)
                {
                    lstBoxLog.Items.Add("ERROR: TG_Connect() returned " +
                        errCode.ToString());
                    StartSuccess = false;
                }

                // done setting up
                if (StartSuccess)
                {
                    _Worker.RunWorkerAsync(); // start thread
                    btnStart.Text = "Stop Collecting Data";
                }
                btnStart.Enabled = true;
            }

            // stop collecting data
            else if (btnStart.Text.StartsWith("Stop"))
            {
                btnStart.Enabled = false;

                // Stop worker
                _Worker.CancelAsync();

                // Disconnect
                ThinkGear.TG_FreeConnection(connectionId);

                // print out
                btnStart.Text = "Start connection on port " + portName;
                lstBoxLog.Items.Add("done stopping worker");
                btnStart.Enabled = true;
            }
            else
            {
                MessageBox.Show("Error in btnStart_Click");
            }
        }

        #region Read Packets as BackgroundWorker
        /// <summary>
        /// Worker to monitor think gear data
        /// </summary>
        private void ThinkGearWorker(object sender, DoWorkEventArgs e)
        {
            // stopwatch for timing
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // main loop, until worker is cancelled
            while (true)
            {
                // check if we should cancel
                if (_Worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                
                // Read all currently available Packets, one at a time
                int packetsRead = ThinkGear.TG_ReadPackets(connectionId, 1);
                if (packetsRead == 1)
                {
                    double _etime =  (double)sw.ElapsedMilliseconds / 1000; // elapsed time since start in seconds

                    ReadPacket(connectionId, ThinkGear.DATA_ATTENTION, _etime, "atn");
                    ReadPacket(connectionId, ThinkGear.DATA_MEDITATION, _etime, "med");
                    ReadPacket(connectionId, ThinkGear.DATA_POOR_SIGNAL, _etime, "sig");
                }
            }
        }

        /// <summary>
        /// Check if packet that was read contains a certain data type, and prints its value
        /// </summary>
        /// <param name="connectionId">ThinkGear connectionId</param>
        /// <param name="DATA_TYPE">ThinkGear.DATA_* code</param>
        /// <param name="etime">ellapsed time in seconds</param>
        /// <param name="label">label for list box output</param>
        private void ReadPacket(int connectionId, int DATA_TYPE, double etime, string label)
        {
            // format string for print out
            string fmt1 = "{0,10:f3}: {1,5:s}: {2,6:g} 0x{2,4:X4}";

            if (ThinkGear.TG_GetValueStatus(connectionId, DATA_TYPE) != 0)
            {
                float res = ThinkGear.TG_GetValue(connectionId, DATA_TYPE);
                string sout = String.Format(fmt1, etime, label, (short)res);
                lstBoxLog.Invoke((Action<string>)AddItem, sout);
            }
        }

        /// <summary>
        /// Add item to listbox for BackgrounWorker
        /// </summary>
        /// <param name="_item"></param>
        private void AddItem(string _item)
        {
            // add item
            lstBoxLog.Items.Add(_item);

            // autoscroll
            lstBoxLog.SelectedIndex = lstBoxLog.Items.Count - 1;
            lstBoxLog.SelectedIndex = -1;
        }
        #endregion

        #region Logging
        /// <summary>
        /// Delegate to pass TG logging functions (TG_SetStreamLog and TG_SetDataLog)
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private delegate int TGLogger(int connectionID, string filename);

        /// <summary>
        /// Set up a log file.
        /// 
        /// Removes log file if it exists
        /// </summary>
        /// <param name="_path">path to file</param>
        private bool _Log(int connectionID, string _path, TGLogger _t)
        {
            // remove log file if it exists
            if (File.Exists(_path))
                File.Delete(_path);

            // start the log
            int errCode = _t(connectionID, _path);

            // check for errors
            bool success = false;
            switch (errCode)
            {
                case -1:
                    lstBoxLog.Items.Add("ERROR: connectionId does not refer to a valid ThinkGear Connection ID handle in " + _t.Method.Name);
                    break;
                case -2:
                    lstBoxLog.Items.Add("ERROR: could not open file " + _path + " in logger " + _t.Method.Name);
                    break;
                default:
                    success = true;
                    break;
            }

            return (success);
        }
        #endregion
    }
}
