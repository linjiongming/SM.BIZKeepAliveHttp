using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Quartz;
using log4net;

namespace SM.BIZKeepAliveHttp
{
    /// <summary>
    /// 客户端回复，模拟数据推送
    /// </summary>
    public class HeartJob : IJob
    {
        private static bool jobFired;
        private ILog logger = LogManager.GetLogger(typeof(HeartJob));

        public void Execute(IJobExecutionContext context)
        {
            if (HeartJob.JobHasFired)
            {
                logger.Warn("前面的心跳任务正在运行.本次任务忽略.");
                return;
            }

            AsyncManager.Heart();
            logger.Info("完成一次客户端回复:" + DateTime.Now.ToLongTimeString());
        }

        public static bool JobHasFired
        {
            get { return jobFired; }
            set { jobFired = value; }
        }
    }
}