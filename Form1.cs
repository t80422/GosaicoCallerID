using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;

namespace gosaicoCallerID
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //標題
            Text = ConfigurationManager.AppSettings["Title"] + " v0.0.1";
            //門市店號
            txtStroreId.Text = ConfigurationManager.AppSettings["StoreID"];
            //API url
            txtAPI_url.Text = ConfigurationManager.AppSettings["API_url"];
            //DGV處理
            DGVSetting.SetStyle(dataGridView1);
            dataGridView1.DataSource= SQLLite.SelectTable("SELECT * FROM call");
            dataGridView1.Columns["call_sn"].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Date";
            dataGridView1.Columns[2].HeaderText = "Time";
            dataGridView1.Columns[3].HeaderText = "Line";
            dataGridView1.Columns[4].HeaderText = "Num";
            dataGridView1.Columns[5].HeaderText = "Status";
            dataGridView1.Columns[6].HeaderText = "Store";
            dataGridView1.Columns[7].HeaderText = "API ID";
            //Log顯示
            //List<string> logs= LogMsg.ReadLogFile(AppDomain.CurrentDomain.BaseDirectory+ $"\\Log\\INF\\INF_{DateTime.Now:yyyyMMdd}.log");
            //logs.
        }
    }
}