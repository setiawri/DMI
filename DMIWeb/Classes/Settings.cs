using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMIWeb
{
    public class Settings
    {
        public static string version = "251001";
        public static bool ConnectToLiveRemoteServer = false;

		public const string IMAGEFOLDERURL = "/assets/img/";
		public const string IMAGEFOLDERPATH = "~" + IMAGEFOLDERURL;
		public const string NOIMAGEFILE = "no-image.jpg";

		public static string ConnectionString
        {
            get
            {
                if (ConnectToLiveRemoteServer)
					return Tools.getConnectionString("connDBLiveRemoteServer");

				if (isDevEnvironment())
                    return Tools.getConnectionString("connDBDev");
                else if (isLiveLocalServer())
                    return Tools.getConnectionString("connDBLiveLocalServer");
                else
                    return Tools.getConnectionString("connDBLiveRemoteServer");
            }
        }

		public static bool isDevEnvironment()
		{
			return System.Environment.MachineName == "RQ";
		}

		public static bool isLiveLocalServer()
		{
			return System.Environment.MachineName == "SERVER";
		}
	}
}