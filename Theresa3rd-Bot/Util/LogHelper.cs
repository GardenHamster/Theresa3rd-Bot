using Business.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class LogHelper
    {

        public static void LogError(string errorMsg)
        {
            try
            {
                string Folder = FilePath.getErrorLogPath() + DateTime.Now.ToString("yyyyMMdd") + "\\";
                string fileName = Folder + "Error_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                if (!File.Exists(fileName))
                {
                    FileStream stream = System.IO.File.Create(fileName);
                    stream.Close();
                    stream.Dispose();
                }
                using (TextWriter fs = new StreamWriter(fileName, true))
                {
                    fs.WriteLine("");
                    fs.WriteLine(string.Format("[{0}]{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), errorMsg));
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex2)
            {
                CQHelper.CQLog.Error("", ex2.Message);
            }
        }


        public static void LogError(Exception ex, string errorMsg)
        {
            try
            {
                string Folder = FilePath.getErrorLogPath() + DateTime.Now.ToString("yyyyMMdd")+"\\";
                string fileName = Folder + "Error_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                if (!File.Exists(fileName))
                {
                    FileStream stream = System.IO.File.Create(fileName);
                    stream.Close();
                    stream.Dispose();
                }
                using (TextWriter fs = new StreamWriter(fileName, true))
                {
                    fs.WriteLine("");
                    fs.WriteLine(string.Format("[{0}]{1}：{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), errorMsg, ex.Message));
                    fs.WriteLine(string.Format("[Source]{0}", ex.Source));
                    fs.WriteLine(string.Format("[TargetSite]{0}", ex.TargetSite));
                    fs.WriteLine(string.Format("[StackTrace]{0}", ex.StackTrace));
                    fs.WriteLine(string.Format("[InnerException]{0}", ex.InnerException == null ? "" : ex.InnerException.Message));
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex2) {
                CQHelper.CQLog.Error("", ex2.Message);
            }
        }

        public static void LogError(Exception ex)
        {
            try
            {
                string Folder = FilePath.getErrorLogPath() + DateTime.Now.ToString("yyyyMMdd") + "\\";
                string fileName = Folder + "Error_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!System.IO.Directory.Exists(Folder)) System.IO.Directory.CreateDirectory(Folder);
                if (!File.Exists(fileName))
                {
                    FileStream stream = System.IO.File.Create(fileName);
                    stream.Close();
                    stream.Dispose();
                }
                using (TextWriter fs = new StreamWriter(fileName, true))
                {
                    fs.WriteLine("");
                    fs.WriteLine(string.Format("[{0}]：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message));
                    fs.WriteLine(string.Format("[Source]{0}", ex.Source));
                    fs.WriteLine(string.Format("[TargetSite]{0}", ex.TargetSite));
                    fs.WriteLine(string.Format("[StackTrace]{0}", ex.StackTrace));
                    fs.WriteLine(string.Format("[InnerException]{0}", ex.InnerException == null ? "" : ex.InnerException.Message));
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex2)
            {
                CQHelper.CQLog.Error("", ex2.Message);
            }
        }





    }
}
