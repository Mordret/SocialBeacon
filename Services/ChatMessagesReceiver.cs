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
	class ChatMessageReceiver : Service
	{
		DashReceiver chatMsgReceiver;
		ClientQuery onlineUsersQuery;

		public override IBinder OnBind(Intent intent)
		{
			throw new NotImplementedException ();
		}
			
		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			chatMsgReceiver = new DashReceiver(SBConst.ConnectionSettings.CHATMSG_SEND_PORT);
			chatMsgReceiver.IncomingDash += (e) => { chatMsgReceiver.HandleAnswer
				(
					DashConst.Handler.HANDLED_YES,
					this.FilesDir + "/" + SBConst.C_ROOT_IMG_FOLDER,
					true
				);
			};
			chatMsgReceiver.Start += (e) => {};
			chatMsgReceiver.Update += (e) => {};
			chatMsgReceiver.End += OnReceiveChatMessage;
			chatMsgReceiver.WaitForDashes();

			onlineUsersQuery = new ClientQuery(System.Net.IPAddress.Any.ToString(), SBConst.ConnectionSettings.USERSLIST_BRAODCAST_PORT);
			onlineUsersQuery.NewResponse += OnReceiveNewUser;
			onlineUsersQuery.StartQuerySession();
			return base.OnStartCommand (intent, flags, startId);
		}

		void OnReceiveNewUser(ClientQuery.NewResponseEventArgs e)
		{
			Intent gotNewUser = new Intent(SBConst.Intents.GOT_NEW_ONLINE_USER);
			gotNewUser.PutExtra(DashConst.Networking.CLIENT_STATE,e.ClientState);
			gotNewUser.PutExtra(SBConst.Prefs.USERS_LIST,e.SerialData.Data);
			SendBroadcast(gotNewUser);
		}

		void OnReceiveChatMessage(DashNode.EndEventArgs e)
		{
			if(e.Successful)
			{
				Intent gotNewMessage = new Intent(SBConst.Intents.GOT_NEW_CHAT_MESSAGE);
				gotNewMessage.PutExtra(SBConst.Prefs.MESSAGE,chatMsgReceiver.Data);
				SendBroadcast(gotNewMessage);
			}
		}

		public override void OnDestroy()
		{
			chatMsgReceiver.StopWaiting();
			onlineUsersQuery.StopQuerySession();
			base.OnDestroy ();
		}
	}
}

