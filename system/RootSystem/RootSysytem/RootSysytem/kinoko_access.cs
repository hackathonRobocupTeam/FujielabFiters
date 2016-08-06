using System;
using System.Net;
using System.IO;

namespace KinokoLib
{
	public class AccessClass
	{
		private static string url = "http://localhost:2828?";
        public static string push(string status_name, string status)
        {
            string reqstr = url + "role=push%26status_name=" + status_name + "%26" + "status=" + status;
            string response = get_request(reqstr);
            return response;
        }
        public static string pull(string status_name)
		{

            string reqstr = url + "role=pull%26status_name=" + status_name;
            string response = get_request(reqstr);
            return response;
        }
		public static bool update(string module_name, string status_name, long time)
		{

            string reqstr = url + "role=update%26status_name=" + status_name + "%26" + "module_name=" + module_name + "%26" + "time=" + time.ToString();
            string response = get_request(reqstr);
            if (response == "True"){
                return true;
            }
            return false;
		}
        private static string get_request(string requrl){
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requrl);
            req.Method = "GET";

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            string str = "";
            for (int i = 0; i < res.Headers.Count; i++)
            {
                str += res.Headers[i] + "\r\n";
            }

            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string content = sr.ReadToEnd();
            return content;

        }
	}
}
