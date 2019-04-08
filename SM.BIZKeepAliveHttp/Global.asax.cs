using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using SM.BIZKeepAliveHttp;
using Quartz;
using System.Configuration;
using System.Collections.Specialized;
using Quartz.Impl;

namespace SM.BIZKeepAliveHttp
{
    public class Global : HttpApplication
    {
        private IScheduler sched;

        void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.DOMConfigurator.Configure();
            AsyncManager.StartTicks = DateTime.Now.Ticks;

            //启动清理任务调度
            NameValueCollection config = new NameValueCollection();
            config["quartz.scheduler.instanceName"] = "SchedulerTest_Scheduler";
            config["quartz.scheduler.instanceId"] = "AUTO";
            config["quartz.threadPool.threadCount"] = "2";
            config["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";

            sched = new StdSchedulerFactory(config).GetScheduler();

            IJobDetail job = JobBuilder.Create()
                                       .OfType<CleanJob>()
                                       .WithIdentity("c1")
                                       .StoreDurably()
                                       .Build();

            string cronExpr = ConfigurationManager.AppSettings["cronExpr"];

            var trigger = (ICronTrigger)TriggerBuilder.Create()
                                 .WithIdentity("c1")
                                 .WithCronSchedule(cronExpr)
                                 .Build();

            sched.ScheduleJob(job, trigger);

            IJobDetail job2 = JobBuilder.Create()
                                       .OfType<HeartJob>()
                                       .WithIdentity("c30")
                                       .StoreDurably()
                                       .Build();

            string hertExpr = ConfigurationManager.AppSettings["heartExpr"];
            var heartTrigger = (ICronTrigger)TriggerBuilder.Create()
                     .WithIdentity("c30")
                     .WithCronSchedule(hertExpr)
                     .Build();
            sched.ScheduleJob(job2, heartTrigger);

            sched.Start();
        }

        void Application_End(object sender, EventArgs e)
        {
            if (sched != null)
            {
                sched.Shutdown(true);
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
        }
    }
}
