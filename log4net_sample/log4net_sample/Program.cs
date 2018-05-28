using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net_sample
{
    class Program
    {
        //log4net
        private static readonly ILog logboth = LogManager.GetLogger("logboth");

        private static readonly ILog logappender = LogManager.GetLogger("logappender");

        private static readonly ILog loggererror = LogManager.GetLogger("logerror");
        //private static readonly ILog logger2 = LogManager.GetLogger("logerror");
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));

            logboth.Debug("logboth!");
            logappender.Debug("logappender!");
            loggererror.Debug("loggererror!");

            Console.Read();
        }
    }
}
