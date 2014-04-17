using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using AbstractClasses;

using Dash;

namespace SocialBeacon
{
	[Service]
	class ServerQuery : Service
	{
		ClientQuery serverQuery;
		ClientBeacon chatBeacon;

		public override IBinder OnBind(Intent intent)
		{
			throw new NotImplementedException ();
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			var ongoing = new Notification (Resource.Drawable.logo, "SocialBeacon is on");
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(MainActivity)), 0);
			ongoing.SetLatestEventInfo (this, "SocialBeacon", "SocialBeacon is on", pendingIntent);

			StartForeground ((int)NotificationFlags.ForegroundService, ongoing);

			serverQuery = new ClientQuery
				(
					System.Net.IPAddress.Any.ToString(),
					SBConst.ConnectionSettings.SERVER_BROADCAST_PORT
				);
			chatBeacon = new ClientBeacon
				(
					"SociealBeacon Client",
					System.Net.IPAddress.Broadcast.ToString(),
					SBConst.ConnectionSettings.USERSLIST_BRAODCAST_PORT
				);
			SerialData myData = new SerialData(intent.GetByteArrayExtra(SBConst.Prefs.ME));
			chatBeacon.SerialData = myData;
			chatBeacon.StartBeaconSession();
			serverQuery.NewResponse += serverQuery_NewResponse;
			serverQuery.StartQuerySession();
			return base.OnStartCommand (intent, flags, startId);
		}

		public void serverQuery_NewResponse(ClientQuery.NewResponseEventArgs e)
		{
			Intent gotNewServer = new Intent(SBConst.Intents.GOT_NEW_SERVER);
			gotNewServer.PutExtra (DashConst.Networking.REMOTE_IP, e.RemoteIP);
			gotNewServer.PutExtra (DashConst.Networking.CLIENT_STATE, e.ClientState);
			gotNewServer.PutExtra (DashConst.Networking.DEVICE_NAME, e.DeviceName);
			SendBroadcast (gotNewServer);
		}

		public override void OnDestroy()
		{
			serverQuery.StopQuerySession ();
			chatBeacon.StopBeaconSession();
			base.OnDestroy ();
		}
	}
}

