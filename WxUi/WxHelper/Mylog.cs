using System;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SrpAPI.Common
{
    public class Mylog
    {
        private static readonly string RootDir = string.Format("{0}", HttpContext.Current.Server.MapPath("~/Log"));

        public static void Log(string str)
        {
            string FilePath = string.Format("{0}\\{1:yyyyMMdd}.txt", RootDir, DateTime.Now);
            try
            {
                if (!Directory.Exists(RootDir))
                {
                    Directory.CreateDirectory(RootDir);
                }
                if (!File.Exists(FilePath))
                {
                    FileStream fs = File.Create(FilePath);
                    fs.Close();
                }
                using (StreamWriter sw = File.AppendText(FilePath))
                {
                    sw.WriteLineAsync("时间：" + DateTime.Now.ToString("yyyy.MM.dd--HH:mm:ss") + "\r\n" + str + "\r\n————————————————————————————————————————————");
                }
            }
            catch (Exception ex)
            { throw new Exception(ex.ToString()); }
        }

        public static void Error(string str)
        {
            string dir = RootDir + "/Error";
            string filePath = string.Format("{0}\\{1:yyyyMMdd}.txt", dir, DateTime.Now);
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!File.Exists(filePath))
                {
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                }
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLineAsync("时间：" + DateTime.Now.ToString("yyyy.MM.dd--HH:mm:ss") + "\r\n" + str + "\r\n————————————————————————————————————————————");
                }
            }
            catch (Exception ex)
            { throw new Exception(ex.ToString()); }
        }
        public static void Error(Exception exs)
        {
            string dir = RootDir + "/Error";
            string filePath = string.Format("{0}\\{1:yyyyMMdd}.txt", dir, DateTime.Now);
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!File.Exists(filePath))
                {
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                }
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLineAsync("时间：" + DateTime.Now.ToString("yyyy.MM.dd--HH:mm:ss") + "\r\n 错误信息：" + exs.Message + "\r\n 位置："+exs.StackTrace+"————————————————————————————————————————————");
                }
            }
            catch (Exception ex)
            { throw new Exception(ex.ToString()); }
        }
    }
}
