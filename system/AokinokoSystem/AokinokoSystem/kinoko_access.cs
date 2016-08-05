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
			WebRequest wrGETURL;
			string reqstr = url + "role=push%26status_name=" + status_name + "%26status=" + status;
 			wrGETURL = WebRequest.Create(reqstr);
			WebProxy myProxy = new WebProxy("myproxy",80);
			myProxy.BypassProxyOnLocal = true;
			wrGETURL.Proxy = WebProxy.GetDefaultProxy();
			Stream objStream;
			objStream = wrGETURL.GetResponse().GetResponseStream();
			

            //return "SUCCESS";
            return;
        }
		public static string pull(string status_name)
		{
            
			return "string";
		}
		public static bool update(string module_name, string status_name, long time)
		{

			return true;
		}
	}
}
