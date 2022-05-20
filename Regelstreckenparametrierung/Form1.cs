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

namespace Regelstreckenparametrierung
{
    public partial class Form1 : Form
    {

        public delegate void USARTRead();
        public USARTRead USARTReadPtr;
        private List<string> series;

        string currentStrecke;
        double time = 0;

        public Form1()
        {
            InitializeComponent();
            USARTReadPtr = new USARTRead(serialPort_Read);
            series = new List<string>();
            series.Add("Strecke 1");
            updateSerieslist();
            currentStrecke = "Strecke 1";
            updatePortList();
            if (combobx_ports.Items.Count > 0)
                combobx_ports.SelectedIndex = 0;
        }

        private void updatePortList()
        {
            combobx_ports.Items.Clear();
            foreach(string name in System.IO.Ports.SerialPort.GetPortNames())
            {
                combobx_ports.Items.Add(name);
            }
        }

        private void updateSerieslist()
        {
            comboBox1.Items.Clear();
            foreach(string name in series)
            {
                comboBox1.Items.Add(name);
            }
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if(!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Open();
                    btn_connect.Text = "Verbindung trennen";
                }catch(Exception ex) { Utility._Debug(ex.Message); }
            }
            else
            {
                try
                {
                    serialPort1.Close();
                    btn_connect.Text = "Verbinden";
                }catch(Exception ex) { Utility._Debug(ex.Message); }
            }
        }

        private void serialPort_Read()
        {
            string line = serialPort1.ReadLine();
            int value = Convert.ToInt32(line);
            chart1.Series[currentStrecke].Points.AddXY(time, value);
            time += 0.05;

            if(value == 1021)
            {
                Utility._Debug("fertig");
            }
            
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            this.Invoke(USARTReadPtr);
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("start");
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("stop");
        }

        private void btn_dc_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("dc");
        }

        private void btn_dcoff_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("dcoff");
        }

        private void btn_charge_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("charge");
        }

        private void btn_chargestop_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("chargestop");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            time = 0;
            serialPort1.WriteLine("rec");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("recstop");
        }

        private void combobx_ports_DropDown(object sender, EventArgs e)
        {
            updatePortList();
        }

        private void combobx_ports_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = combobx_ports.SelectedItem.ToString();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            string name = "Strecke " + (series.Count + 1).ToString();
            chart1.Series.Add(name);
            series.Add(name);
            chart1.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            updateSerieslist();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentStrecke = comboBox1.SelectedItem.ToString();
            time = 0;
        }

        private void export(System.Windows.Forms.DataVisualization.Charting.DataPointCollection points, string path)
        {
            int pointcounter = points.Count;
            string x_array = "[";
            string y_array = "[";

            for (int i = 0; i < pointcounter; i++)
            {
                x_array += points[i].XValue + ":";
                y_array += points[i].YValues[0] + ":";
            }

            x_array += "]";
            y_array += "]";

            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(x_array);
            writer.WriteLine(y_array);
            writer.Close();
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();

            if(result == DialogResult.OK)
            {
                
                string path = saveFileDialog1.FileName;
                export(chart1.Series[currentStrecke].Points, path);
                /*
                string line = "";
                for(int i = 0; i < chart1.Series[currentStrecke].Points.Count; i++)
                {
                    line += "[" + chart1.Series[currentStrecke].Points[i].XValue + ";" + chart1.Series[currentStrecke].Points[i].YValues[0] + "]";
                }
                writer.WriteLine(line);
                writer.Close();*/
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            float zehnProzent = 0;
            float fünfzigProzent = 0;
            float neunzigProzent = 0;

            double zeitZehn = 0;
            double zeitFünfzig = 0;
            double zeitNeunzig = 0;
            int diff = 0;

            for(int i = 0; i < chart1.Series[currentStrecke].Points.Count; i++)
            {
                /// 102,1 = 10% 
                /// 510,5 = 50%
                /// 918,9 = 90 %
                float diff_z = (float) (102.1 - chart1.Series[currentStrecke].Points[i].YValues[0]);
                float diff_f = (float)(510.5 - chart1.Series[currentStrecke].Points[i].YValues[0]);
                float diff_n = (float)(918.9 - chart1.Series[currentStrecke].Points[i].YValues[0]);
                if (diff_z < (102.1 - zehnProzent) && diff_z >= 0)
                {
                    Utility._Debug("Diff: " + (102.1 - chart1.Series[currentStrecke].Points[i].YValues[0]));
;                    zehnProzent = (float) chart1.Series[currentStrecke].Points[i].YValues[0];
                    zeitZehn = chart1.Series[currentStrecke].Points[i].XValue;
                }

                if (diff_f < (510.5 - fünfzigProzent) && diff_f >= 0)
                {
                    fünfzigProzent = (float)chart1.Series[currentStrecke].Points[i].YValues[0];
                    zeitFünfzig = chart1.Series[currentStrecke].Points[i].XValue;
                }

                if (diff_n < (918.9 - neunzigProzent) && diff_n >= 0)
                {
                    neunzigProzent = (float)chart1.Series[currentStrecke].Points[i].YValues[0];
                    zeitNeunzig = chart1.Series[currentStrecke].Points[i].XValue;
                }

                
            }

            Utility._Debug("10%: " + zehnProzent + " : " + zeitZehn);
            Utility._Debug("50%: " + fünfzigProzent + " : " + zeitFünfzig);
            Utility._Debug("90%: " + neunzigProzent + " : " + zeitNeunzig);

            int ordnung = ZeitprozentkennwertMethode.berechneOrdnung((float) zeitZehn, (float) zeitNeunzig);
            float zeitkonstante = ZeitprozentkennwertMethode.berechneZeitkonstante(ordnung, (float)zeitZehn, (float)zeitFünfzig, (float)zeitNeunzig);
            Utility._Debug("Ordnung: " + ordnung + " | Zetkonstante: " + zeitkonstante);
            simulateStrecke(currentStrecke + " - Annäherung", zeitkonstante, ordnung);
        }

        private void simulateStrecke(string name, float zeitkonstante, int ordnung)
        {
            chart1.Series.Add(name);
            chart1.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
           
            float y = 0;
            float x = 0;

            for(int i = 0; i < chart1.Series[currentStrecke].Points.Count; i++)
            {
                x = (float) chart1.Series[currentStrecke].Points[i].XValue;
                if(ordnung == 1)
                {
                    y = (float)(1 - Math.Pow(Math.E, (-1) * (x / zeitkonstante)));
                }
                else
                if (ordnung == 2)
                {
                    y = (float)(1 - Math.Pow(Math.E, (-1) * (x / zeitkonstante)) - ((x / zeitkonstante) * Math.Pow(Math.E, (-1) * (x / zeitkonstante))));
                }
                else
                if(ordnung == 3)
                {
                    y = (float)(1 - Math.Pow(Math.E, (-1) * (x / zeitkonstante)) - ((x / zeitkonstante) * Math.Pow(Math.E, (-1) * (x / zeitkonstante))) - ((Math.Pow(x, 2) / (2 * Math.Pow(zeitkonstante, 2))) * Math.Pow(Math.E, (-1) * (x / zeitkonstante))));
                }
                Utility._Debug("X: " + x + " | Y: " + y);
                chart1.Series[name].Points.AddXY(x, 1021 * y);
            }

        }

        

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }
    }
}
