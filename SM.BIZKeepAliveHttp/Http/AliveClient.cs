using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SM.BIZKeepAliveHttp
{
    /// <summary>
    /// 一个Http客户端连接,里面封装了一个长连接和一个Parse后的协议数据
    /// </summary>
    public class AliveClient
    {
        private HKAsyncRequest result = null;
        private DateTime createTime = DateTime.Now;
        private string sessionId;
        private static readonly ILog logger = LogManager.GetLogger(typeof(AliveClient));

        public AliveClient(HKAsyncRequest result)
        {
            this.result = result;
            //建立一个ID
            //this.sessionId = Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Millisecond.ToString();
            //先写死
            this.sessionId = "1234567890";
        }

        public HKAsyncRequest Result
        {
            get { return result; }
            set { result = value; }
        }


        public DateTime CreateTime
        {
            get { return this.createTime; }
            set { this.createTime = value; }
        }

        public string MAC
        {
            get;
            set;
        }

        public CustomType Type
        {
            get;
            set;
        }

        public string SessionId
        {
            get { return this.sessionId; }
        }

        public bool IsCompleted
        {
            get 
            {
                if (this.result == null)
                    return false;
                return this.result.IsCompleted;
            }
        }


        public void Send(string msg) 
        {
            if (this.result == null) {
                logger.Error("向无效客户端发送请求.自动忽略.");
                return;
            }
            this.result.Send(msg);
        }

    }
}