using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HelperToolsCore_v21
{
    public class LogHelper
    {
        public enum LogType { ErrorLog, ProcessLog }

        static LogHelper()
        {
            //string Env = AppConfigResource.ConfigEnvironment;
            //IsLocalTest = (String.IsNullOrEmpty(Env) || Env == "DEV") ? true : false;
            IsLocalTest = false;
            //IsDockerEnv = (AppConfigResource.BaseAppDirectory == "/root") ? true : false;


        }

        public static readonly string BaseAppDirectory = AppContext.BaseDirectory;
        public static bool IsLocalTest = true;//ConsoleProject DeBug
        public static bool IsDockerEnv = true;//ConsoleProject DeBug
        private static bool IsWriteDeBugLogFile = true;//***Add on 20170306 by Arvin for MKTPO1225  Batch Create Item Docker Version Phase I

        /// <summary>
        /// //***Add on 20160617 by Arvin for MKTPO531  File Level Duplicate Check for Item
        /// 取得行數及method name
        /// </summary>
        /// <returns></returns>
        public static string MethodDebug(string MethodName, int LineNumber)
        {
            return MethodName + "(" + LineNumber.ToString() + ")...";
        }

        /// <summary>
        /// //***Add on 20170306 by Arvin for MKTPO1225  Batch Create Item Docker Version Phase I
        /// </summary>
        public static void WriteErrorLog(System.Exception ex)
        {
            WriteLogFile(LogType.ErrorLog, String.Format("[ErrorMsg]{0}[ErrorStackTrace]{1}", ex.Message, ex.StackTrace));
        }

        public static void WriteErrorLog(string messages, System.Exception ex)
        {
            WriteLogFile(LogType.ErrorLog, messages + Environment.NewLine +
                String.Format("[ErrorMsg]{0}[ErrorStackTrace]{1}", ex.Message, ex.StackTrace));
        }

        public static void WriteErrorLog(string messages)
        {
            WriteLogFile(LogType.ErrorLog, messages);
        }

        public static void WriteProcessLog(string messages, string fileName = null)
        {
            WriteLogFile(LogType.ProcessLog, messages, fileName);
        }

        /// <summary>
        /// //***Add on 20170306 by Arvin for MKTPO1225  Batch Create Item Docker Version Phase I
        /// </summary>
        private static void WriteLogFile(LogType logType, string messages, string logMark = null)
        {
            try
            {
                string FileName = String.Format("{0}{1}.log", logMark, DateTime.Now.ToString("yyyyMMdd"));
                string logFilePath = String.Empty;


                //logFilePath = System.IO.Path.Combine(Configer.DockerFileLogPath, FileName);
                logFilePath = System.IO.Path.Combine(BaseAppDirectory, "Log", logType.ToString(), FileName);

                messages = String.Format("[{0}]:  ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + messages + Environment.NewLine;
                FileInfo fi = new FileInfo(logFilePath);
                if (!fi.Directory.Exists)
                {
                    try
                    {
                        fi.Directory.Create();
                    }
                    catch { }
                }
                if (!fi.Directory.Exists) return;
                bool IsFiExists = fi.Exists;
                if (IsFiExists)
                {
                    messages = Environment.NewLine + messages;
                    System.IO.File.AppendAllText(logFilePath, messages);
                }
                else
                {
                    var logFile = System.IO.File.Create(logFilePath);
                    var logWriter = new StreamWriter(logFile);
                    logWriter.Write(messages);
                    logWriter.Dispose();
                }
                System.Console.WriteLine(messages);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// 記錄線程點與點的執行時間差
        /// </summary>
        /// <param name="ThreadTime">傳入thread DateTime物件</param>
        /// <returns>"Now:現在時間,TimeInterval:與前段呼叫到此method的時間差(ms)</returns>
        public static string PointLogTime(ref DateTime ThreadTime)
        {
            string result = "";
            DateTime now = DateTime.Now;
            double timeval = now.Subtract(ThreadTime).TotalMilliseconds;
            result = String.Format("[TimeInterval]{0} ms", timeval.ToString());
            ThreadTime = DateTime.Now;
            return result;
        }
    }
}
