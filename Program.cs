﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static gosaicoCallerID.Form1;

namespace gosaicoCallerID
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    /// <summary>
    /// DataGridView設定相關
    /// </summary>
    public static class DGVSetting
    {
        /// <summary>
        /// 設定DataGridView公版樣式
        /// </summary>
        /// <param name="dgv"></param>
        public static void SetStyle(DataGridView dgv)
        {
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.Font = new Font(dgv.Font.FontFamily, 12);
        }

    }

    public static class SQLLite
    {
        private static string _connectString = "Data Source = " + ConfigurationManager.AppSettings["DataSource"];
        /// <summary>
        /// SQL Lite 搜尋
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable SelectTable(string sql)
        {
            using (var connection = new SqliteConnection(_connectString))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = sql;
                    DataTable dt = new DataTable();
                    using (var reader = command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    ErrorMsg(ex.Message);
                    return null;
                }
            }
        }

        public static bool InserTable(string table, Dictionary<string, string> dicData)
        {
            using (var conn = new SqliteConnection(_connectString))
            {
                try
                {
                    conn.Open();
                    string sql = $"INSERT INTO {table} ({string.Join(",", dicData.Keys)}) VALUES ({string.Join(",", dicData.Keys.Select(x => "@" + x))})";
                    using (var command = new SqliteCommand(sql, conn))
                    {
                        foreach (var entry in dicData)
                        {
                            command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                        }
                        if (command.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg(ex.Message);
                }
            }
            return false;
        }

        private static void ErrorMsg(string err)
        {
            string title = "資料庫異常";
            MessageBox.Show(err, title);
            new LogMsg(LogType.ERROR, err);
        }

    }

    public class LogMsg
    {
        private LogType _logType;
        private string infLogFilePath;
        private string errorLogFilePath;

        public LogMsg(LogType logType, string msg)
        {
            BuildLogFile();
            _logType = logType;
            WriteToLog(msg);
        }

        /// <summary>
        /// 檢查建立Log資料夾、檔
        /// </summary>
        private void BuildLogFile()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            //檢查有沒有Log資料夾
            string logPath = Path.Combine(basePath, "Log");
            if (!Directory.Exists(logPath)) { Directory.CreateDirectory(logPath); }
            //檢查有沒有INF資料夾
            string infPath = Path.Combine(logPath, "INF");
            if (!Directory.Exists(infPath)) { Directory.CreateDirectory(infPath); }
            //檢查有沒有ERROR資料夾
            string errorPath = Path.Combine(logPath, "ERROR");
            if (!Directory.Exists(errorPath)) { Directory.CreateDirectory(errorPath); }

            string today = DateTime.Now.ToString("yyyMMdd");
            //檢查有沒有今天的INF Log檔
            infLogFilePath = Path.Combine(infPath, $"INF_{today}.log");
            if (!File.Exists(infLogFilePath))
            {
                FileStream file = File.Create(infLogFilePath);
                file.Close();
            }
            //檢查有沒有今天的ERROR Log檔
            errorLogFilePath = Path.Combine(errorPath, $"ERROR_{today}.log");
            if (!File.Exists(errorLogFilePath))
            {
                FileStream file = File.Create(errorLogFilePath);
                file.Close();
            }
        }

        public void WriteToLog(string text)
        {
            string msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string filePath;
            if (_logType == LogType.INF)
            {
                filePath = infLogFilePath;
                msg += " [INF] ";
            }
            else
            {
                filePath = errorLogFilePath;
                msg += " [ERROR] ";
            }

            try
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    msg += text;
                    sw.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                // 處理寫入檔案時的異常狀況
                Console.WriteLine("Error writing to log file: " + ex.Message);
            }
            Form1 form = Application.OpenForms.OfType<Form1>().First();
            form.LogTextBox(msg);
        }

    }

    public static class CheckConnect
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(ref int Description, int ReservedValue);
        public static bool InternetConnect()
        {
            int dwFlag = new int();
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                return false;
            }
            return true;
        }

        public static bool CheckServer(string ipString, int port)
        {
            System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient() { SendTimeout = 1000 };
            IPAddress ip = IPAddress.Parse(ipString);
            try
            {
                tcpClient.Connect(ip, port);
            }
            catch (Exception)
            {
                //连接失败
                return false;
            }
            bool right = tcpClient.Connected;
            tcpClient.Close();
            tcpClient.Dispose();
            return right;
        }
    }
}
