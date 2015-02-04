using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
namespace GumTree_Scraper
{
    public partial class Form1 : Form
    {

        private const string pattern = "<a class=\"listing-link\" href=\"(.*?)\" itemprop=\"url\">";
        private bool shouldIrun = false;
        private int counter;
        private HashSet<string> ids = new HashSet<string>();
  // private IContainer components = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.shouldIrun = true;
            int num = 1; //from
            int num2 = 3158; //to
            ThreadPool.SetMaxThreads(25, 25); // 25 threads
            this.counter = num + 1;
            for (int i = num; i < num2; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.Worker), i);
            }
        }

        private void button1_Click(object sender, EventArgs e)//export button
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt) | *.txt"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(dialog.FileName))
                {
                    foreach (string str in this.ids)
                    {
                        writer.WriteLine(str);
                    }
                }
            }
        }

       /* protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }*/

        public string SimpleGET(string index)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.textBox1.Text + "page" + index);//textbox1 = guntry link
            request.Method = "GET";
            Stream responseStream = ((HttpWebResponse)request.GetResponse()).GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string str = reader.ReadToEnd();
            reader.Close();
            responseStream.Flush();
            responseStream.Close();
            return str;
        }

        private void Worker(object o)
        {
            Invoky method = null;
            if (this.shouldIrun)
            {
                string index = o.ToString();
                string input = this.SimpleGET(index);
                foreach (Match match in Regex.Matches(input, "<a class=\"listing-link\" href=\"(.*?)\" itemprop=\"url\">"))
                {
                    string str3 = match.Groups[1].Value;
                    this.ids.Add(str3.Substring(str3.LastIndexOf('/') + 1));
                    Console.WriteLine(str3.Substring(str3.LastIndexOf('/') + 1));
                }
                if (method == null)
                {
                    method = () => this.Text = "GumTree Scraper - " + this.counter.ToString() + "/" + "3158";
                }
                base.Invoke(method);
                this.counter++;
            }
        }
        private delegate void Invoky();//in the worker thread

    }
}
