﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMIWeb
{
    public class Settings
    {

        public static string ConnectionString
        {
            get
            {
                if (System.Environment.MachineName == "RQ-ASUS")
                    return Tools.getConnectionString("connDBDev");
                else if (System.Environment.MachineName == "SERVER")
                    return Tools.getConnectionString("connDBLiveLocalServer");
                else
                    return Tools.getConnectionString("connDBLiveRemoteServer");
            }
        }

    }
}