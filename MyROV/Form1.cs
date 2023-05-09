using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyROV
{
    public partial class Form1 : Form
    {
        string dataRead = "";

        delegate void SetTextCallback(string text); // Khai bao delegate SetTextCallBack voi tham so string
        public Form1()
        {   
            InitializeComponent();
        

        }

    
        private void cbxCom_DropDown(object sender, EventArgs e)
        {
            if (SerialPort.GetPortNames().Length > 0)
            {
                cbxCom.DataSource = SerialPort.GetPortNames();
                btnConnect.Enabled = true;
            }
            else
            {
                btnConnect.Enabled = false;
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = cbxCom.Text;
                    serialPort1.BaudRate = 9600;
                    serialPort1.Open();
                    MessageBox.Show("Connection is successful!");
                }
                else 
                {
                    serialPort1.Close();
                    MessageBox.Show("Disconnect is successful!");
                    // lbStatus.Text = "Connected !";
                    // lbStatus.ForeColor = Color.Green;


                }
            }
            catch (Exception ex)
            {
                    MessageBox.Show("Connection is failed!\n"+ex.Message);

            }

        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
      
            //lbStatus.Text = "Disconnected !";
            //lbStatus.ForeColor = Color.Red;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                btnConnect.Text = "Disconnect";
                btnConnect.ForeColor = Color.Red;

                lbStatus.Text = "Connected !";
                lbStatus.ForeColor = Color.Green;
        
               // btnConnect.Enabled = true;
               // btnDisconnect.Enabled = false;
            }
            else
            {
                btnConnect.Text = "Connection";
                btnConnect.ForeColor = Color.Green;

                lbStatus.Text = "Disconnected !";
                lbStatus.ForeColor = Color.Red;
                //btnConnect.Enabled = false;
                //btnDisconnect.Enabled = true;
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                dataRead = serialPort1.ReadLine().ToString().Trim();
                TachChuoi(dataRead);
            }catch
            (Exception ex)
            {
                MessageBox.Show("Error");
                throw;
            }
        }

        private void TachChuoi(string dataRead)
        {
            if (this.txbX1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(TachChuoi);
                this.BeginInvoke(d, new object[] { dataRead});

            }
            else
            {
                Console.WriteLine("dataRead: " + dataRead);
                string keyX1 = "X1:";
                string keyY1 = "~Y1:";
                string keyX2 = "X2:";
                string keyY2 = "~Y2:";
                TachJoystick(dataRead, keyX1, keyY1, txbX1, txbY1);
                TachJoystick(dataRead, keyX2, keyY2, txbX2, txbY2);

            }
        }

        private void TachJoystick(string data, string keyX, string keyY, System.Windows.Forms.TextBox txbX, System.Windows.Forms.TextBox txbY)
        {
            string value1, value2;
            double value1XX, value2XX;
            if (data.Contains(keyX) && data.Contains(keyY))
            {
                value1 = returnText(data, data.IndexOf(keyX) + keyX.Length, data.IndexOf(keyY) - keyY.Length + 1);
                //value1 = dataRead.Substring(dataRead.IndexOf(key1)+key1.Length, dataRead.IndexOf(key2)-key2.Length+1);
                double.TryParse(value1, out value1XX);
                txbX.Text = value1XX.ToString();
                if (value1XX >= 0)
                {
                    txbX.BackColor = Color.FromArgb(255, 255 - Convert.ToInt32(value1XX), 255 - Convert.ToInt32(value1XX));

                }
                else
                {
                    txbX.BackColor = Color.FromArgb(255 + Convert.ToInt32(value1XX), 255, 255 + Convert.ToInt32(value1XX));

                }
                value2 = returnText(data, data.IndexOf(keyY) + keyY.Length);
                //value2 = dataRead.Substring(dataRead.IndexOf(key2)+key2.Length);
                double.TryParse(value2, out value2XX);
                txbY.Text = value2XX.ToString();
                Console.WriteLine(value2.ToString());

                if (value2XX >= 0)
                {
                    txbY.BackColor = Color.FromArgb(255, 255 - Convert.ToInt32(value2XX), 255 - Convert.ToInt32(value2XX));

                }
                else
                {
                    txbY.BackColor = Color.FromArgb(255 + Convert.ToInt32(value2XX), 255, 255 + Convert.ToInt32(value2XX));

                }
            }
            else
            {
                Console.WriteLine("None");
            }
        }
        String returnText(string data, int pos1, int pos2)
        {
            return data.Substring(pos1, pos2);
        }
        String returnText(string data, int pos1)
        {
            return data.Substring(pos1);
        }
    }
}
