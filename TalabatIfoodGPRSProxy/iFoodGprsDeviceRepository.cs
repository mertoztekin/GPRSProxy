using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Handlers;

namespace TalabatIfoodGPRSProxy
{
    internal class iFoodGprsDeviceRepository
    {
        static int _cacheMinute = -1;
        static string _terminalListSourceEndPoint = null;
        static int CacheMinute
        {
            get
            {
                if (_cacheMinute > -1)
                    return _cacheMinute;
                if (!int.TryParse(WebConfigurationManager.AppSettings["iFoodGPRSCacheMinute"],out _cacheMinute))
                {
                    _cacheMinute = 10;
                }
                return _cacheMinute;
            }
        }

        static string TerminalListSourceEndPoint
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_terminalListSourceEndPoint))
                {
                    if (!string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings["iFoodGPRSTerminalListSourceEndPoint"]))
                    {
                        _terminalListSourceEndPoint =
                            WebConfigurationManager.AppSettings["iFoodGPRSTerminalListSourceEndPoint"];
                    }
                    else
                    {
                        _terminalListSourceEndPoint = "http://ifood.jo/terminalids.txt";
                    }
                }
                return _terminalListSourceEndPoint;
            }
        }

        static readonly int[] _predefinedIds = new int[]
        {
            1281, 1282, 1287, 1288, 1289, 1299, 1290, 1303, 1304, 1210, 1025, 1213, 1235, 1273, 1284, 1342, 1072, 1074,
            1075, 1083, 1084, 1087, 1088, 1127, 1135, 1103, 1105, 1164, 1165, 1176, 1178, 1076, 1079, 1080, 1081, 1104,
            1112, 1150, 1157, 1159, 1162, 1163, 1186, 1189, 1207, 1209, 1314, 1177, 1216, 1274, 1276, 1277, 1278, 1302,
            1316, 873, 874, 860, 854, 858, 857, 861, 875, 1199, 851, 878, 879, 882, 884, 885, 886, 887, 888, 889, 890,
            891, 894, 895, 896, 897, 898, 899, 900, 901, 902, 903, 904, 905, 906, 907, 908, 909, 910, 876, 877, 881, 911,
            893, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 933, 935, 936, 942, 943, 945, 946, 948, 949, 951, 952,
            953, 954, 955, 956, 961, 963, 967, 968, 970, 978, 980, 982, 991, 992, 993, 994, 995, 998, 999, 1001, 1004,
            1003, 1006, 1007, 1008, 1009, 1015, 1016, 1017, 1018, 1019, 1021, 989, 862, 872, 990, 1023, 1024, 1034, 1036,
            1040, 1041, 1052, 1065, 1066, 1208, 1211, 1212, 859, 1293, 1309, 1280, 1296, 1295, 1308, 1160, 1161, 1172,
            1338, 1340, 1341, 1214, 1192, 1300, 1337, 1339, 1134, 1233, 1166, 1275, 1285, 940, 944, 947, 950, 981, 1020,
            1022, 1240, 1262, 1263, 1312, 883, 926, 927, 929, 930, 880, 1026, 1027, 1029, 1031, 1035, 1132, 1221, 1149,
            1158, 1167, 1168, 1169, 1170, 1179, 1298, 1215, 969, 1028, 1030, 1032, 1033, 1063, 1174, 1297, 1301, 931,
            932, 934, 937, 938, 939, 957, 958, 959, 960, 962, 964, 965, 966, 1010, 1011, 1012, 1042, 1043, 1239, 1133,
            1171, 1173, 1175, 1336, 1283, 1307
        };

        private static DateTime _lastUpdateDate = DateTime.MinValue;

        private static int[] _terminalIds = new int[0];

        private static object lockObject = new object();
        protected static int[] IfoodTerminalIds
        {
            get
            {
                lock (lockObject)
                {
                    if ((DateTime.Now - _lastUpdateDate).TotalMinutes > CacheMinute)
                    {
                        ReloadDeviceIds();
                    }
                }
                return _terminalIds;
            }
        }

        public static ILogger Logger { get; set; }

        /// <summary>
        /// Reloads device ids from ifood web site.
        /// If fails, loads predefined ones
        /// </summary>
        private static void ReloadDeviceIds()
        {
            try
            {
                _lastUpdateDate=DateTime.Now;
                WebClient client = new WebClient();
                string downloadedString = client.DownloadString(TerminalListSourceEndPoint + "?nocache="+DateTime.Now.Millisecond);
                if(string.IsNullOrWhiteSpace(downloadedString))
                    throw new Exception("response is empty!");

                List<int> newIds = new List<int>();
                foreach (string idInString in downloadedString.Split(','))
                {
                    int i = 0;
                    if(int.TryParse(idInString,out i))
                        newIds.Add(i);
                }
                _terminalIds = newIds.ToArray();
                LoggingHelper.Debug(Logger, String.Format("{0} New terminal ids gathered",_terminalIds.Length));
            }
            catch (Exception ex)
            {
                LoggingHelper.Error(Logger,"Unable to gather new terminal ids:" + ex.Message);
                _terminalIds = _predefinedIds;
            }
        }


        /// <summary>
        /// Checks if request is done by ifood device
        /// </summary>
        /// <param name="terminalId></param>
        /// <returns></returns>
        public static bool CheckIsIfood(int terminalId)
        {
            var ids = IfoodTerminalIds;
            return ids != null && ids.Length > 0 && ids.Contains(terminalId);
        }

    }
}
