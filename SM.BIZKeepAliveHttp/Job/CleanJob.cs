
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Quartz;
using log4net;

namespace SM.BIZKeepAliveHttp
{
    /// <summary>
    /// 清理任务调度用于清理已经完成的Http请求和已经超时的Http请求
    /// </summary>
    public class CleanJob : IJob
    {
        private static bool jobFired;
        private ILog logger = LogManager.GetLogger(typeof(CleanJob));


        public void Execute(IJobExecutionContext context)
        {
            if (CleanJob.JobHasFired) {
                logger.Warn("前面的清理任务正在运行.本次任务忽略.");
                return;
            }
            //清理掉已经完成的Http任务
            AsyncManager.Clean();
            logger.Info("完成一次清理任务:" + DateTime.Now.ToLongTimeString());
        }

        public static bool JobHasFired
        {
            get { return jobFired; }
            set { jobFired = value; }
        }
    }
}