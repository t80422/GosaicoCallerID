using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        static string _connectString = "Data Source = " + ConfigurationManager.AppSettings["DataSource"];

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
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    public static class LogMsg
    {
        public static List<string> ReadLogFile(string filePath)
        {
            List<string> logLines = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        logLines.Add(line);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return logLines;
        }
    }
}
