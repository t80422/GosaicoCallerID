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

        public const int WM_AD800MSG = 1024 + 1800;
        //啟用中的話機集合 key:話機編號
        Dictionary<int, Phone> phones = new Dictionary<int, Phone>();
        bool isNetConnect;

        public enum LogType
        {
            INF, ERROR
        }

        /// <summary>
        /// Channel Status
        /// </summary>
        public enum AD800_STATUS : int
        {
            AD800_DEVICE_CONNECTION,	//Device connection status 設備連接狀態
            AD800_LINE_STATUS, //Line Status e.g pickup,hangup,ringing,power off 話機狀態
            AD800_LINE_VOLTAGE, //Line voltage 話機電壓
            AD800_LINE_POLARITY, //Line Polarity Changed 話機 極性
            AD800_LINE_CALLERID, //Caller Id number 來電號碼
            AD800_LINE_DTMF, //Dialed number 話機撥號
            AD800_REC_DATA,	//Recording data 錄音數據
            AD800_PLAY_FINISHED, //Playback finished 放音完成
            AD800_VOICETRIGGER,//Voice trigger status 語音觸發狀態
            AD800_BUSYTONE,	//Busy tone status 忙音狀態
            AD800_DTMF_FINISHED //Send DTMF finished DTMF發送完成
        };

        /// <summary>
        /// 設備連線狀態
        /// </summary>
        public enum AD800_CONNECTION : int
        {
            AD800DEVICE_DISCONNECTED = 0,	//Device connected 連上
            AD800DEVICE_CONNECTED			//Device disconnected 斷開
        };

        /// <summary>
        /// 話機狀態
        /// </summary>
        public enum AD800_LINESTATUS : int
        {
            AD800LINE_POWEROFF = 0, //Power off/no line 斷線
            AD800LINE_HOOKOFF,		//Pick up 接起
            AD800LINE_HOOKON,		//Hang up 掛斷
            AD800LINE_RING,		//Ringing 響鈴
            AD800LINE_POWERON //連線
        };

        public Form1()
        {
            InitializeComponent();
            //初始化AD800 
            AD800_Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //標題
            Text = ConfigurationManager.AppSettings["Title"] + " v0.2";
            //門市店號
            txtStroreId.Text = ConfigurationManager.AppSettings["StoreID"];
            //API url
            txtAPI_url.Text = ConfigurationManager.AppSettings["API_url"];
            //DGV處理
            DGVSetting.SetStyle(dataGridView1);
            dataGridView1.Columns.Add("call_date", "Date");
            dataGridView1.Columns.Add("call_time", "Time");
            dataGridView1.Columns.Add("call_line", "Line");
            dataGridView1.Columns.Add("call_num", "Num");
            dataGridView1.Columns.Add("call_status", "Status");
            dataGridView1.Columns.Add("call_api_id", "API ID");

            AD800_SetMsgHwnd(Handle.ToInt32());

            SQLLite.CheckSqlExist();

            //設定檢查網路、伺服器連線狀態時間
            tmrNet.Interval = int.Parse(ConfigurationManager.AppSettings["CheckNet"]) * 1000;
            isNetConnect = true;
            tmrNet_Tick(sender, e);
            tmrNet.Enabled = true;
            btnServerConnect.PerformClick();
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_AD800MSG:
                    OnDeviceMsg(m.WParam, m.LParam);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void OnDeviceMsg(IntPtr wParam, IntPtr lParam)
        {
            int iEvent = wParam.ToInt32() % 65536;
            int iChannel = wParam.ToInt32() / 65536;
            string msg;
            switch (iEvent)
            {
                //設備連線狀態
                case (int)AD800_STATUS.AD800_DEVICE_CONNECTION:
                    {
                        if ((int)AD800_CONNECTION.AD800DEVICE_CONNECTED == lParam.ToInt32())
                        {
                            msg = "設備" + "已啟用";
                            lblDevice.Text = msg;
                            lblDevice.BackColor = Color.LightGoldenrodYellow;
                        }
                        else
                        {
                            msg = "設備" + "未啟用";
                            lblDevice.Text = msg;
                            lblDevice.BackColor = Color.LightPink;
                        }
                        new LogMsg(LogType.INF, msg);
                    }
                    break;

                //話機狀態
                case (int)AD800_STATUS.AD800_LINE_STATUS:
                    {
                        if (phones.ContainsKey(iChannel + 1))
                        {
                            Phone phone = phones[iChannel + 1];
                            phone.Now = DateTime.Now;
                            switch (lParam.ToInt32())
                            {
                                case (int)AD800_LINESTATUS.AD800LINE_POWEROFF:
                                    phones.Remove(iChannel);
                                    msg = $"Line:{iChannel + 1} 已斷線";
                                    new LogMsg(LogType.INF, msg);
                                    break;

                                case (int)AD800_LINESTATUS.AD800LINE_HOOKOFF://接起
                                    if (phone.Status == "Ringing")
                                    {
                                        phone.Status = "HookOff";
                                        PhoneStatus(phone, iEvent.ToString(), iChannel.ToString(), lParam.ToString());
                                    }

                                    break;

                                case (int)AD800_LINESTATUS.AD800LINE_HOOKON://掛斷
                                    //未接
                                    if (phone.Status == "Ringing")
                                    {
                                        phone.Status = "Missed";
                                        PhoneStatus(phone, iEvent.ToString(), iChannel.ToString(), lParam.ToString());
                                    }
                                    else if (phone.Status == "HookOff")
                                    {
                                        phone.Status = "HookOn";
                                        PhoneStatus(phone, iEvent.ToString(), iChannel.ToString(), lParam.ToString());
                                    }
                                    break;

                                case (int)AD800_LINESTATUS.AD800LINE_RING:
                                    if (phone.Status != "Ringing")
                                    {
                                        StringBuilder szBuff = new StringBuilder(128);
                                        AD800_GetCallerId(phone.Line - 1, szBuff, 64);
                                        string str = szBuff.ToString();
                                        //市話打進來,若是同縣市就不會顯示區碼,所以要另外加進去
                                        //if (str[0] != '0') { str = ConfigurationManager.AppSettings["AreaCode"] + str; }
                                        phone.Num = str;
                                        phone.Status = "Ringing";
                                        PhoneStatus(phone, iEvent.ToString(), iChannel.ToString(), lParam.ToString());
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    break;

                case (int)AD800_STATUS.AD800_LINE_VOLTAGE:
                    {
                        int volt = (int)lParam;
                        if (volt > 20)
                        {
                            if (!phones.ContainsKey(iChannel + 1))
                            {
                                Phone phone = new Phone() { Line = iChannel };
                                phones.Add(phone.Line, phone);
                                msg = $"Line:{iChannel + 1} 已連線";
                                new LogMsg(LogType.INF, msg);
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void PhoneStatus(Phone phone, string Event, string channel, string lParam)
        {
            dataGridView1.Rows.Insert(0, phone.Date, phone.Time, phone.Line, phone.Num, phone.Status);
            new LogMsg(LogType.INF, $"Event:{Event},Channel:{channel} lParam:{lParam}");
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"call_date",phone.Date },
                {"call_time",phone.Time},
                {"call_line",phone.Line.ToString() },
                {"call_num",phone.Num},
                {"call_status",phone.Status },
                {"call_store",ConfigurationManager.AppSettings["StoreID"]}
            };
            SQLLite.InserTable("call", dic);
        }

        public void LogTextBox(string msg)
        {
            txtLog.Text = msg + "\r\n" + txtLog.Text;
        }

        private void tmrNet_Tick(object sender, EventArgs e)
        {
            if (CheckConnect.InternetConnect())
            {
                if (!isNetConnect)
                {
                    new LogMsg(LogType.INF, "網路已連線");
                    isNetConnect = true;
                }
            }
            else
            {
                if (isNetConnect)
                {
                    new LogMsg(LogType.ERROR, "網路斷線");
                    isNetConnect = false;
                }
            }
        }

        private void btnServerConnect_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string errMsg = "";

            if (CheckConnect.CheckServer(ref errMsg))
            {
                new LogMsg(LogType.INF, "伺服器已連線");
            }
            else
            {
                new LogMsg(LogType.ERROR, "伺服器斷線");
                MessageBox.Show(errMsg, "伺服器斷線");
            }
            Cursor.Current = Cursors.Default;
        }
    }
}