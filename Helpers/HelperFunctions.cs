using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Android.App;
using Android.Widget;
using Android.Net;
using Android.Content;

namespace SocialBeacon
{
	public static class HelperFunctions
	{
		public static byte[] FileToBytes(FileStream fs)
		{
			byte[] data = new byte[(int)fs.Length];
			fs.Read(data,0,(int)fs.Length);
			fs.Close();
			return data;
		}

		public static void BytesToFile(byte[] data, string savePath)
		{
			FileStream fs = new FileStream(savePath,FileMode.CreateNew);
			fs.Write(data,0,data.Length);
			fs.Close();
		}

		public static bool GetMyIP(Context context,ref string myIP)
		{
			Android.Content.Res.Resources res = context.Resources;

			bool is_connected = false;
			myIP = "";

			var connectivityManager = (ConnectivityManager)context.GetSystemService (Context.ConnectivityService);
			var mobileState = connectivityManager.GetNetworkInfo (ConnectivityType.Wifi).GetState ();
			if (mobileState != NetworkInfo.State.Connected)
				is_connected = false;
			else
			{
				IPHostEntry host = Dns.GetHostEntry (Dns.GetHostName ());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						myIP = ip.ToString ();
						is_connected = true;
					}
				}
			}
			return is_connected;
		}


		public static bool IsServiceRunning(Context context, Type t)
		{
			ActivityManager manager = (ActivityManager)context.GetSystemService (Context.ActivityService);
			foreach (ActivityManager.RunningServiceInfo service in manager.GetRunningServices(int.MaxValue))
				if (service.Service.ClassName.ToLower () == t.FullName.ToLower ())
					return true;
			return false;
		}
    }
}

