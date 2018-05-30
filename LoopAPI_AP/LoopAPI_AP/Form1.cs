using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoopAPI_AP
{
    public partial class Form1 : Form
    {
        int intcount;
        string strurl;


        //更新畫面
        public delegate void updateview(String myString);
        updateview updateviewfun;
        //1

        //2
        //3
        //4
        //5
        public Form1()
        {
            InitializeComponent();
            updateviewfun = new updateview(updateviewdelegate);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            strurl = textBox1.Text;
            intcount = int.Parse(textBox2.Text);

            timer1.Interval = Convert.ToInt32(textBox3.Text) * 1000;
            timer1.Enabled = true;


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateviewfun.Invoke("---------------------------------------------------------------------");
            for (int i = 0; i < intcount; i++)
            {

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(strurl).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        updateviewfun.Invoke((i+1)+"-"+response.IsSuccessStatusCode.ToString());
                    }

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        //更新畫面
        public void updateviewdelegate(string msg)
        {
            try
            {
                textBox4.AppendText(msg);
                textBox4.AppendText(Environment.NewLine);
            }
            catch (Exception ex)
            { 
            }

        }
    }
}
