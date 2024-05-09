using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Price_Checker.Services;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Data;

namespace Price_Checker
{
    public partial class mainForm : Form
    {

        private readonly ImagesManagerService imageManager;
        private readonly ScanBarcodeService scanBarcodeService;
        private readonly BarcodeTimer barcodeTimer;
        private readonly FontManagerService fontManager;
        private readonly VideoManagerService videoManager;
        private readonly DataRetrievalService dataRetrievalService;
        private string connstring;
        private DatabaseConfig _config;

       

        public mainForm()
        {
            InitializeComponent();
            lbl_barcode.KeyDown += Lbl_barcode_KeyDown;
            KeyPreview = true;
            this.Shown += MainForm_Shown;
            //lbl_barcode.Focus();


            timer1.Start();
            timer1.Interval = 1000; // 1000 milliseconds = 1 second
            timer1.Tick += timer1_Tick;

            // Update the label with the current date and time
            lbl_date.Text = DateTime.Now.ToString();
            UpdateStatusLabel(lbl_status);


            scanBarcodeService = new ScanBarcodeService();
            scanBarcodeService.BarcodeScanned += ScanBarcodeService_BarcodeScanned;
            barcodeTimer = new BarcodeTimer(lbl_barcode);
            imageManager = new ImagesManagerService(pictureBox1);
            imageManager.ImageSlideshow();

            fontManager = new FontManagerService();
            lbl_barcode.Font = fontManager.GetCustomFont();

            videoManager = new VideoManagerService(axWindowsMediaPlayer1);
           


            DatabaseConfig databaseConfig = new DatabaseConfig();

            //// Prepare the message to be displayed
            //string message = $"Server: {databaseConfig.Server}\n" +
            //                  $"Uid: {databaseConfig.Uid}\n" +
            //                  $"Pwd: {databaseConfig.Pwd}\n" +
            //                  $"Database: {databaseConfig.Database}";

            //// Show the MessageBox
            //MessageBox.Show(message, "Database Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dataRetrievalService = new DataRetrievalService(databaseConfig);
            dataRetrievalService.SetInterval(5, DataRetrievalService.TimeUnit.Seconds);

        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Set focus to lbl_barcode when the form is shown
            lbl_barcode.Focus();
        }


        private void UpdateStatusLabel(Label lbl_status)
        {
            string status = "Server Offline"; // Default status

            try
            {
                using (MySqlConnection con = new MySqlConnection(connstring))
                {
                    con.Open();

                    // Check if the connection is open
                    if (con.State == ConnectionState.Open)
                    {
                        string sql = "SELECT set_status FROM settings";
                        MySqlCommand cmd = new MySqlCommand(sql, con);

                        // Execute the query and get the result
                        object result = cmd.ExecuteScalar();

                        // Check if the result is not null and is of integer type
                        if (result != null && result is int statusValue)
                        {
                            status = statusValue == 1 ? "Server Online" : "Server Offline";
                        }
                    }
                    else
                    {
                        // Connection failed to open
                        status = "Failed to connect to the database.";
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle MySQL-specific exceptions
                status = "MySQL Error: " + ex.Message;
                MessageBox.Show(status);
            }

            lbl_status.Text = status;
        }


        private void Lbl_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            scanBarcodeService.HandleBarcodeInput(e, lbl_barcode, scanPanel, this);
        }
        private void ScanBarcodeService_BarcodeScanned(object sender, string barcode)
        {
            barcodeTimer.StartTimer();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the label with the current date and time
            lbl_date.Text =  DateTime.Now.ToString();
        }

    }
}