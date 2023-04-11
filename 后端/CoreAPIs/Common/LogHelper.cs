using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPIs.Common
{
    public class LogHelper
    /*
     * 日志中间类
     */
    {
        private static ILoggerRepository repository { get; set; }
        private static ILog _Loginfo;
        private static ILog _Logerror;
        private static ILog LogInfo
        {
            get
            {
                if (_Loginfo == null)
                {
                    Configure();
                }
                return _Loginfo;
            }
        }
        private static ILog LogError
        {
            get
            {
                if (_Logerror == null)
                {
                    Configure();
                }
                return _Logerror;
            }
        }
        public static void Configure(string repositoryName = "NETCoreRepository", string configFile = "log4net.config")
        {
            repository = LogManager.CreateRepository(repositoryName);
            XmlConfigurator.Configure(repository, new FileInfo(configFile));
            _Logerror = LogManager.GetLogger(repositoryName, "logerror");
            _Loginfo = LogManager.GetLogger(repositoryName, "loginfo");
        }
        public static void Info(string msg)
        {
            LogInfo.Info(msg);
        }
        public static void Warn(string msg)
        {
            LogInfo.Warn(msg);
        }
        public static void Error(string msg)
        {
            LogError.Error(msg);
        }
        public static void Info(object ex)
        {
            LogInfo.Info(ex);
        }
        public static void Debug(object message, Exception ex)
        {
            LogInfo.Debug(message, ex);
        }
        public static void Warn(object message, Exception ex)
        {
            LogInfo.Warn(message, ex);
        }
        public static void Error(object message, Exception ex)
        {
            LogError.Error(message, ex);
        }
        public static void LogErrorInfo(Exception ex, object message)
        {
            LogError.Error(message, ex);
        }
        public static void Info(object message, Exception ex)
        {
            LogInfo.Info(message, ex);
        }
    }
}