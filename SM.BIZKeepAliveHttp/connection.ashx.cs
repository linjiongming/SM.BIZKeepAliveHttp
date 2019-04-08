using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SM.BIZKeepAliveHttp
{
    /// <summary>
    /// 命令接收回复
    /// </summary>
    public class Connection : IHttpHandler
    {
        public static readonly string DATAFIELD = "data";
        private static readonly ILog logger = LogManager.GetLogger(typeof(Connection));

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = "text/plain";
            string value = context.Request.Params.Get(DATAFIELD);

            if (String.IsNullOrEmpty(value))
            {
                context.Response.Write("500 create connection error.");
                return;
            }

            try
            {
                AliveClient ac = new AliveClient(null);
                //加入缓存
                AsyncManager.AddClient(ac);
                //直接回复
                context.Response.Write("收到回复");
            }
            catch (Exception ex)
            {
                logger.Error("创建连接发生错误:" + ex.Message);
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}