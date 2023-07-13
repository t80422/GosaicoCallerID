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
using System.Runtime.InteropServices;

namespace gosaicoCallerID
{
    public partial class Form1 : Form
    {
        //[DllImport("DevLink.dll", EntryPoint = "DLOpen")]
        //public static extern int DLOpen();
        //[DllImport("DevLink.dll", EntryPoint = "DLClose")]
        //public static extern int DLClose();
        //[DllImport("DevLink.dll", EntryPoint = "DLSetMsg")]
        //public static extern int DLSetMsg(int hWnd);

        //初始化
        [DllImport("AD800Device.dll", EntryPoint = "AD800_Init")]
        public static extern bool AD800_Init();
        //設置狀態傳送方式為消息方式
        [DllImport("AD800Device.dll", EntryPoint = "AD800_SetMsgHwnd")]
        public static extern void AD800_SetMsgHwnd(int hWnd);
        //得到連接到電腦上的設備總數
        [DllImport("AD800Device.dll", EntryPoint = "AD800_GetTotalDev")]
        public static extern int AD800_GetTotalDev();
        //讀取設備的出廠序號,沒有設備會顯示0
        [DllImport("AD800Device.dll", EntryPoint = "AD800_GetDevSN")]
        public static extern int AD800_GetDevSN(int device);
        //讀取所有可用通道數量
        [DllImport("AD800Device.dll", EntryPoint = "AD800_GetTotalCh")]
        public static extern int AD800_GetTotalCh();
        //取得電話狀態 0:斷線 1:接起 2:掛斷 3:響鈴
        [DllImport("AD800Device.dll", EntryPoint = "AD800_GetChState")]
        public static extern int AD800_GetChState(int channel);
        //取得電話號碼
        [DllImport("AD800Device.dll", EntryPoint = "AD800_GetCallerId")]
        public static extern bool AD800_GetCallerId(int channel, StringBuilder number, int len);

        //public const int WM_USBLINEMSG = 1024 + 200;
        public const int WM_AD800MSG = 1024 + 1800;

        string ip = ConfigurationManager.AppSettings["ServerIP"];
        int port = int.Parse(ConfigurationManager.AppSettings["Port"]);

        public enum LogType
        {
            INF, ERROR
        }

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

            DataTable dt = SQLLite.SelectTable("SELECT * FROM call ORDER BY call_date,call_time");
            if (dt == null) { Environment.Exit(0); }
            dataGridView1.DataSource = dt;
            dataGridView1.Columns["call_sn"].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Date";
            dataGridView1.Columns[2].HeaderText = "Time";
            dataGridView1.Columns[3].HeaderText = "Line";
            dataGridView1.Columns[4].HeaderText = "Num";
            dataGridView1.Columns[5].HeaderText = "Status";
            dataGridView1.Columns[6].HeaderText = "Store";
            dataGridView1.Columns[7].HeaderText = "API ID";
            //設定檢查網路、伺服器連線狀態時間
            tmrNet.Interval = int.Parse(ConfigurationManager.AppSettings["CheckNet"]) * 1000;
            tmrServer.Interval = int.Parse(ConfigurationManager.AppSettings["CheckServer"]) * 1000;
            tmrNet.Enabled = true;
            tmrServer.Enabled = true;
            tmrCheck.Enabled = false;
            tmrCheck.Interval = 1000;
            //DLOpen();
            //DLSetMsg(Handle.ToInt32());
        }

        //protected override void DefWndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        case WM_USBLINEMSG: //处理消息　
        //            OnDeviceMsg(m.WParam, m.LParam);
        //            break;
        //        default:
        //            base.DefWndProc(ref m);//调用基类函数处理非自定义消息。　　
        //            break;
        //    }
        //}

        //private void OnDeviceMsg(IntPtr wParam, IntPtr Lparam)
        //{
        //    string strbuff = Marshal.PtrToStringAnsi(Lparam);
        //    Console.WriteLine(strbuff);

        //    string[] words = strbuff.Split(',');
        //    string[] line = words[0].Split('=');
        //    string[] number = words[2].Split('=');
        //    Dictionary<string, string> dic = new Dictionary<string, string>
        //    {
        //        { "call_date", DateTime.Now.ToString("d") },
        //        { "call_time", DateTime.Now.ToString("HH:mm:ss") },
        //        {"call_line",line[1] },
        //        {"call_num",number[1] },
        //        {"call_status","ring" },
        //        {"call_store" ,ConfigurationManager.AppSettings["StoreID"]},
        //        {"call_api_id","api" }
        //    };
        //    if (words[0] != "" && (number[0] == "callerid(dtmf)" || number[0] == "callerid(fsk)"))
        //    {
        //        SQLLite.InserTable("call", dic);
        //        DataTable dt = SQLLite.SelectTable("SELECT * FROM call ORDER BY call_date,call_time");
        //        dataGridView1.DataSource = dt;
        //        new LogMsg(LogType.INF, Lparam + " " + number[1]);
        //    }
        //}

        public void LogTextBox(string msg)
        {
            txtLog.AppendText(msg + Environment.NewLine);
        }

        //檢查網路連線
        private void tmrNet_Tick(object sender, EventArgs e)
        {
            if (!CheckConnect.InternetConnect())
            {
                tmrNet.Enabled = false;
                tmrCheck.Enabled = true;
                new LogMsg(LogType.ERROR, "網路斷線");
                MessageBox.Show("網路斷線", "警告");
            }
        }

        private void tmrServer_Tick(object sender, EventArgs e)
        {
            if (!CheckConnect.CheckServer(ip, port))
            {
                tmrServer.Enabled = false;
                tmrCheck.Enabled = true;
                new LogMsg(LogType.ERROR, "伺服器斷線");
                MessageBox.Show("伺服器斷線", "警告");
            }
        }

        private void tmrCheck_Tick(object sender, EventArgs e)
        {
            //網路連線恢復的話就開始檢查網路連線
            if (CheckConnect.InternetConnect())
            {
                tmrNet.Enabled = true;
            }
            if (CheckConnect.CheckServer(ip, port))
            {
                tmrServer.Enabled = true;
            }
        }
    }
}