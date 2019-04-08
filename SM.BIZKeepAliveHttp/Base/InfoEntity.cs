using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using log4net;

namespace SM.BIZKeepAliveHttp
{
    #region 客户端类型
    /// <summary>
    /// 客户端类型
    /// </summary>
    public enum CustomType : int
    {
        /// <summary>
        /// 网关
        /// </summary>
        Gateway = 1,

        /// <summary>
        /// 手机
        /// </summary>
        Mobile = 2
    }
    #endregion

    #region 定义在线用户类
    /// <summary>
    /// 定义在线用户类 
    /// </summary>
    public static class OnLineUser
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(OnLineUser));

        #region 定义变量

        public static Dictionary<string, User> DicObject = new Dictionary<string, User>(1000);

        private static readonly object _object = new object();

        #endregion

        //功能说明：将当前用户加入在线列表 
        //如果该用户的数据当前仍然在在线列表中，则暂时先不让该用户登陆,提示用户存在 
        public static bool AddUserToOnLine(User user)
        {
            if (OnLineUser.DicObject.ContainsKey(user.MAC))
            {
                return false;
            }
            else
            {
                lock (OnLineUser._object)
                {
                    OnLineUser.DicObject.Add(user.MAC, user);
                }
                return true;
            }
        }

        //功能说明:判断某用户是否在线,本部分暂时不用 
        //返回值：TRUE代表在线，FALSE不在 
        public static Boolean IsUserOnLine(string mac)
        {
            if (OnLineUser.DicObject.ContainsKey(mac))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //功能说明：更新用户在线时间 
        //返回值：最新的在线用户列表 
        public static Boolean CheckUserOnLine(string mac)
        {
            if (OnLineUser.DicObject.ContainsKey(mac))
            {
                User tempuser = OnLineUser.DicObject[mac];
                tempuser.LastOPDate = DateTime.Now;//更新用户在线时间 
                lock (OnLineUser._object)
                {
                    OnLineUser.DicObject.Remove(mac);
                    OnLineUser.DicObject.Add(mac, tempuser);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //功能说明：获取用户 
        //返回值：用户实体
        public static User FindUserOnLine(string mac)
        {
            if (OnLineUser.DicObject.ContainsKey(mac))
            {
                return OnLineUser.DicObject[mac];
            }
            else
            {
                return new User();
            }
        }

        //功能说明：删除缓存 
        //返回值：用户实体
        public static bool DeleteUserOnLine(string mac)
        {
            if (OnLineUser.DicObject.ContainsKey(mac))
            {
                lock (OnLineUser._object)
                {
                    OnLineUser.DicObject.Remove(mac);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion

    #region 在线用户结构体
    /// <summary>
    /// 在线用户结构体
    /// </summary>
    public struct User
    {
        public string sessionId;

        public string MAC;

        //public string IP;

        //public int Port;

        public DateTime AddDate;

        public DateTime LastOPDate;

        /// <summary>
        /// 缓存类型 1 网关，2 手机
        /// </summary>
        public byte Type;
    }
    #endregion

    #region 非心跳检测是否加入缓存结构体
    /// <summary>
    /// 非心跳检测是否加入缓存结构体
    /// </summary>
    public struct UploadCheck
    {
        public string sessionId;

        public string Mac;

        public string IP;

        public int Port;

        /// <summary>
        /// 缓存类型 1 网关 ， 2 手机
        /// </summary>
        public byte Type;
    }
    #endregion

    #region 在线功能类
    /// <summary>
    /// 在线功能类
    /// </summary>
    public static class OnLineNews
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(OnLineNews));
        #region 定义变量
        
        /// <summary>
        /// 是否是初始状态
        /// </summary>
        public static bool IsInitial = true;

        /// <summary>
        /// 时间刻度常量
        /// </summary>
        internal const long TimeTicks = 10000000;

        /// <summary>
        /// 检测数字
        /// </summary>
        internal readonly static long CheckLong;

        /// <summary>
        /// 检测秒数
        /// </summary>
        internal const long Seconds = 180;

        #endregion

        #region 静态构造函数
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static OnLineNews()
        {
            CheckLong = TimeTicks * Seconds;
        }
        #endregion

        #region 检测客户端或网关是否加入缓存
        /// <summary>
        /// 检测客户端或网关是否加入缓存
        /// </summary>
        /// <param name="SessionID">SessionID</param>
        /// <param name="Mac">Mac</param>
        /// <param name="Address">Address</param>
        /// <param name="Port">Port</param>
        /// <param name="Type">类型</param>
        public static void CheckIsJoinChache(string SessionID, string Mac, byte Type)
        {
            try
            {
                if (DateTime.Now.Ticks - AsyncManager.StartTicks > CheckLong)
                {
                    OnLineNews.IsInitial = false;
                }


                UploadCheck UC = new UploadCheck();
                UC.sessionId = SessionID;
                UC.Mac = Mac;
                //UC.IP = Address;
                //UC.Port = Port;
                UC.Type = Type;
                ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(OnLineNews.CheckIsChange), UC);
            }
            catch (Exception ex)
            {
                logger.Error("error:" + ex.Message);
            }
        }
        #endregion

        #region 检测在协议发起前是否发送过心跳协议
        /// <summary>
        /// 检测在协议发起前是否发送过心跳协议
        /// </summary>
        /// <param name="state">传递对象</param>
        public static void CheckIsChange(Object state)
        {
            try
            {
                UploadCheck UC = (UploadCheck)state;
                if (!OnLineUser.IsUserOnLine(UC.Mac))
                {
                    User u = new User();
                    u.sessionId = UC.sessionId;
                    u.MAC = UC.Mac;
                    //u.IP = UC.IP;
                    //u.Port = UC.Port;
                    u.AddDate = u.LastOPDate = DateTime.Now;
                    u.Type = UC.Type;
                    OnLineUser.AddUserToOnLine(u);
                }
            }
            catch (Exception ex)
            {
                logger.Error("error:" + ex.Message);
            }
        }
        #endregion
    }
    #endregion
}