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

namespace gosaicoCallerID
{
    public partial class Form1 : Form
    {
        private string _connectionString = "Data Source=gosaicoCallerID.db";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"select * from call";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string date = reader.GetString(1);
                        string time = reader.GetString(2);
                        int line = reader.GetInt32(3);
                        string num = reader.GetString(4);
                        string status = reader.GetString(5);
                        int store = reader.GetInt32(6);
                        dataGridView1.Rows.Add(date, time, line, num, status, store);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //標題
            Text = ConfigurationManager.AppSettings["Title"] + " v0.0.1";
            //門市店號
            txtStroreId.Text = ConfigurationManager.AppSettings["StoreID"];
            //API url
            txtAPI_url.Text = ConfigurationManager.AppSettings["API_url"];
            //DGV設定
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.Font = new Font(dataGridView1.Font.FontFamily, 12);
            dataGridView1.Columns.Add("DATE", "DATE");
            dataGridView1.Columns.Add("TIME", "TIME");
            dataGridView1.Columns.Add("LINE", "LINE");
            dataGridView1.Columns.Add("NUM", "NUM");
            dataGridView1.Columns.Add("STATUS", "STATUS");
            dataGridView1.Columns.Add("STORE", "STORE");
        }
    }
}