using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;

namespace SM.BIZKeepAliveHttp
{
    public class Data : IHttpAsyncHandler
    {

        public static readonly string DATAFIELD = "data";
        private static readonly ILog logger = LogManager.GetLogger(typeof(Data));

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string value = context.Request.Params.Get(DATAFIELD);

            //这里传过来的是SessionId，不是数据,数据不做重复Parse
            //用sessionId去缓存中找对应的会话，并填充异步AsyncResult
            HKAsyncRequest result = new HKAsyncRequest(context, cb, extraData);
            string error = null;
            if (String.IsNullOrEmpty(value))
            {
                error = "500 SessionId is null";
                context.Response.StatusCode = 500;
                logger.Error(error);
                result.Send(error);
                return result;
            }

            List<AliveClient> acs = AsyncManager.Sessions.FindAll(x => x.SessionId.Equals(value));
            if (acs == null || acs.Count == 0)
            {
                error = "404 SessionId:" + value + " has no connection.";
                context.Response.StatusCode = 404;
                logger.Debug(error);
                result.Send(error);
                return result;
            }

            AliveClient ac = acs.First();
            ac.Result = result;
            //执行命令

            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }


        public void ProcessRequest(HttpContext context)
        {
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