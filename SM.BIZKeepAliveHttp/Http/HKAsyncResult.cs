using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;

namespace SM.BIZKeepAliveHttp
{
    /// <summary>
    /// 一个异步会话，会话会被临时缓存
    /// </summary>
    public class HKAsyncRequest : IAsyncResult
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HKAsyncRequest));

        public HKAsyncRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            this.Context = context;
            this.CallBack = cb;
            this.ExtraData = extraData;
        }

        public HttpContext Context
        {
            get;
            set;
        }

        public object ExtraData
        {
            get;
            set;
        }

        public AsyncCallback CallBack
        {
            get;
            set;
        }

        public bool IsCompleted
        {
            get;
            set;
        }


        public object AsyncState
        {
            get;
            set;
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get;
            set;
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public void Send(string response) {
            if (String.IsNullOrEmpty(response))
                return;
            try
            {
                this.Context.Response.ContentType = "text/plain";
                this.Context.Response.Write(response);
                if (this.CallBack != null)
                {
                    this.CallBack(this); 
                }
            }
            catch (Exception ex)
            {
                logger.Error("输出到客户端发生错误:" + ex.Message);
            }
            finally 
            {
                IsCompleted = true; 
            }
        }


    }
}