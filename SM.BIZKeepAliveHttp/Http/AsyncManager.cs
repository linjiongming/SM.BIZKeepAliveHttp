using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SM.BIZKeepAliveHttp
{
    /// <summary>
    /// 会话临时缓存
    /// </summary>
    public static class AsyncManager
    {
        private static List<AliveClient> clients = new List<AliveClient>();

        private static readonly ILog logger = LogManager.GetLogger(typeof(AsyncManager));
        public static long StartTicks;
        private static object syncObj = new object();


        public static void AddClient(AliveClient client)
        {
            if (client == null)
                return;
            lock (syncObj) {
                clients.Add(client);
            }
        }


        public static List<AliveClient> Sessions
        {
            get { return clients; }
        }

        /// <summary>
        /// 心跳回复
        /// </summary>
        public static void Heart() {
            AliveClient c = null;
            lock (syncObj) {
                for (int i = 0; i < clients.Count; i++)
                {
                    c = clients[i];
                    logger.Info("回复:111111");
                    c.Send("服务器端推，有数据啦....");
                }
            }
        }



        public static void Clean() {
            int maxTimeSpan = Int32.Parse(ConfigurationManager.AppSettings["maxTimeSpan"]);

            int comepleteCount = 0;
            int overTimeCount = 0;
            int invalidCount = 0;

            AliveClient ac = null;

            lock (syncObj)
            {
                for(int i=0;i < clients.Count;i++)
                {
                    ac = clients[i];
                    if (ac == null)
                        continue;
                    else if (ac.IsCompleted)
                    {
                        //完成请求
                        comepleteCount += 1;
                        clients.Remove(ac);
                    }
                    else if ((DateTime.Now - ac.CreateTime).Minutes > maxTimeSpan)
                    {
                        //超时请求
                        overTimeCount += 1;
                        clients.Remove(ac);
                    }
                    String msg = String.Format("完成一次清理任务：无效请求:%s ; 超时请求:%s ; 已完成请求:s%", invalidCount.ToString(), overTimeCount.ToString(), comepleteCount.ToString());
                    logger.Info(msg);
                }   
            }
        }

    }
}