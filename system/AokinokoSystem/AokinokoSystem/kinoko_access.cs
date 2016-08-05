using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KinokoLib
{
	public class AccessClass
	{
        public static string push(string status_name, string status)
        {
            string url = "http://localhost:2828";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest req = context.Request;
            HttpListenerResponse res = context.Response;




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
