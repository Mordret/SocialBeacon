using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Net.Wifi;
using Android.Graphics;
using AbstractClasses;
using Dash;

namespace SocialBeacon
{
	[Activity (Label = "SocialBeacon", MainLauncher = true)]			
	public class MainActivity : Activity
	{
		string imgUrl = "";

		bool isValidUser = false, isServiceRunning;
		string uID = "", remoteIP, myIP;

		public string RemoteIP { get { return remoteIP; } set { remoteIP = value; } }
		public string ImgURI { get { return imgUrl; } }

		WifiManager wm;
		WifiManager.MulticastLock mclock;

		public List<Server> servers = new List<Server>();

		List<object> drawableIDs;

		Button editInfoBtn, chatBtn;
		TextView firstName, lastName;
		ImageView profilePic, flag;
		ISharedPreferences prefsReader;
		Switch clientSwitch;
		public TextView serverName;
		public ListView serverList;

		IntentFilter gotNewServer = new IntentFilter(SBConst.Intents.GOT_NEW_SERVER);
		ServerDetailsReceiver sdRec;

		public Person me;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.MainLayout);

			HelperFunctions.GetMyIP(this,ref myIP);

			if(!Directory.Exists(FilesDir + "/" + SBConst.C_ROOT_IMG_FOLDER))
				Directory.CreateDirectory(FilesDir + "/" + SBConst.C_ROOT_IMG_FOLDER);
			if(!Directory.Exists(FilesDir + "/" + SBConst.C_ROOT_CHAT_FOLDER))
				Directory.CreateDirectory(FilesDir + "/" + SBConst.C_ROOT_CHAT_FOLDER);

			drawableIDs = typeof(Resource.Drawable).GetFields(BindingFlags.Public | BindingFlags.Static |
				BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name.StartsWith(SBConst.FLAG_START_NAME))
				.Select(val => val.GetRawConstantValue())
				.ToList();

			wm = (WifiManager)GetSystemService(WifiService);
			mclock = wm.CreateMulticastLock("reqreclock");
			mclock.Acquire();

			prefsReader = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this);

			chatBtn = FindViewById<Button>(Resource.Id.chatBtn);
			chatBtn.Click += (sender, e) =>
			{
				Intent cvIntent = new Intent(this,typeof(ChatView));
				cvIntent.PutExtra(SBConst.Prefs.ME,DashSerializer.SerializeObject(me).Data);
				StartActivity(cvIntent);
			};;

			firstName = FindViewById<TextView>(Resource.Id.firstNameTV);
			lastName = FindViewById<TextView>(Resource.Id.lastNameTV);
			profilePic = FindViewById<ImageView>(Resource.Id.profilePic);

			flag = FindViewById<ImageView>(Resource.Id.flag);

			editInfoBtn = FindViewById<Button>(Resource.Id.editInfoBtn);
			editInfoBtn.Click += (sender, e) => 
			{
				Intent editInfo = new Intent(this,typeof(InfoEditForm));
				StartActivity(editInfo);
			};

			serverList = FindViewById<ListView>(Resource.Id.serversList);
			serverList.ItemClick += serverList_ItemClick;

			clientSwitch = FindViewById<Switch>(Resource.Id.clientSwitch);
			clientSwitch.CheckedChange += clientSwitch_CheckedChange;
			clientSwitch.Checked = HelperFunctions.IsServiceRunning(this,typeof(ServerQuery));
			serverName = FindViewById<TextView>(Resource.Id.serverNameTV);
		}

		void clientSwitch_CheckedChange(object sender, EventArgs e)
		{
			if(clientSwitch.Checked)
			{
				if(!HelperFunctions.IsServiceRunning(this,typeof(ServerQuery)))
				{
					sdRec = new ServerDetailsReceiver();
					RegisterReceiver(sdRec,gotNewServer);
					Intent startServerQuery = new Intent(this,typeof(ServerQuery));
					startServerQuery.PutExtra(SBConst.Prefs.ME,DashSerializer.SerializeObject(me).Data);
					StartService(startServerQuery);
				}
				if(!HelperFunctions.IsServiceRunning(this,typeof(ChatMessageReceiver)))
					StartService(new Intent(this,typeof(ChatMessageReceiver)));
			}
			else
			{
				StopService(new Intent(this,typeof(ServerQuery)));
				StopService(new Intent(this,typeof(ChatMessageReceiver)));
				UnregisterReceiver (sdRec);
				editInfoBtn.Enabled = true;
				chatBtn.Enabled = false;
				serverList.Adapter = null;
				servers.Clear();
			}
		}

		void serverList_ItemClick(object sender, AdapterView.ItemClickEventArgs eargs)
		{
			remoteIP = servers[eargs.Position].IP;
			serverName.Text = servers[eargs.Position].Name;
			chatBtn.Enabled = true;
			editInfoBtn.Enabled = false;

			Dash.Dash myDash = new Dash.Dash();
			myDash.LoadSerialData(DashSerializer.SerializeObject(me));
			string[] files = { ImgURI };
			myDash.LoadFiles(files);
			DashSender ds = new DashSender("SocialBeacon Client",SBConst.ConnectionSettings.CLIENT_BROADCAST_PORT);
			ds.Start += (DashNode.StartEventArgs e) => {};
			ds.Update += (DashNode.UpdateEventArgs e) => {};
			ds.End += (DashNode.EndEventArgs e) => {};
			ds.Dash = myDash;
			ds.Connect(RemoteIP);
			ds.StartSend();
		}

		protected override void OnResume()
		{
			base.OnResume();
			profilePic.SetImageDrawable(null);

			isServiceRunning = HelperFunctions.IsServiceRunning(this,typeof(ServerQuery));

			// if(serverName.Text != "Not Connected")
			//	{
				chatBtn.Enabled = isServiceRunning;
				editInfoBtn.Enabled = !isServiceRunning;
			// }

			if(isServiceRunning)
			{
				sdRec = new ServerDetailsReceiver();
				RegisterReceiver(sdRec,gotNewServer);
			}

			isValidUser = prefsReader.GetBoolean(SBConst.Prefs.IS_VALID_USER,false);

			Bitmap b = BitmapFactory.DecodeResource(Resources,Resource.Drawable.default_profile_pic);
			imgUrl = this.FilesDir + "/" + SBConst.PP_NAME;
			if(!File.Exists(imgUrl))
				using(FileStream fs = new FileStream(imgUrl, FileMode.Create))
					b.Compress(Bitmap.CompressFormat.Jpeg, 50, fs);

			profilePic.SetImageURI(Android.Net.Uri.Parse(imgUrl));

			if(!isValidUser)
			{
				AlertDialog.Builder notValidUser = new AlertDialog.Builder(this);
				notValidUser.SetMessage("Please take the time to fill in your details before using the app. \r\n\r\nWould you like to do it now?");
				notValidUser.SetPositiveButton("Yes", delegate
					{
						Intent i = new Intent(this,typeof(InfoEditForm));
						StartActivityForResult(i,1);
					}
				);
				notValidUser.SetNegativeButton("No",delegate { Finish(); });
				notValidUser.SetCancelable(false);
				notValidUser.Show();
			}
			else
			{
				firstName.Text = prefsReader.GetString(SBConst.Prefs.FIRST_NAME,"");
				lastName.Text = prefsReader.GetString(SBConst.Prefs.LAST_NAME,"");

				string[] countrynames = this.Resources.GetStringArray(Resource.Array.country_names);
				string selectedcountry = prefsReader.GetString(SBConst.Prefs.COUNTRY,"Select");

				flag.SetImageResource((int)drawableIDs[Array.IndexOf(countrynames,selectedcountry)]);

				me = new Person
					(
						myIP,
						firstName.Text,
						lastName.Text,
						prefsReader.GetInt(SBConst.Prefs.AGE,0),
						prefsReader.GetString(SBConst.Prefs.COUNTRY,""),
						prefsReader.GetString(SBConst.Prefs.CITY,""),
						prefsReader.GetString(SBConst.Prefs.PHONE,""),
						prefsReader.GetString(SBConst.Prefs.EMAIL,""),
						prefsReader.GetString(SBConst.Prefs.ABOUT,"")
					);
				uID = me.UID;
				File.Copy
				(
					FilesDir + "/" + SBConst.PP_NAME,
					FilesDir + "/" + SBConst.C_ROOT_IMG_FOLDER + uID + SBConst.PP_EXTENSION_NAME,
					true
				);
			}
		}

		protected override void OnPause()
		{
			base.OnPause();
			try
			{
				UnregisterReceiver(sdRec);
			}
			catch {}
		}
	}

	class ServerDetailsReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			MainActivity mainActivity = context as MainActivity;
			Server s = new Server
				(
					intent.GetStringExtra (DashConst.Networking.REMOTE_IP),
					intent.GetStringExtra (DashConst.Networking.DEVICE_NAME)
				);

			int clientState = intent.GetIntExtra (DashConst.Networking.CLIENT_STATE,0);

			if(clientState == 1)
			{
				if(!mainActivity.servers.Contains(s))
					mainActivity.servers.Add(s);
			}
			else
			{
				if(mainActivity.servers.Contains(s))
					mainActivity.servers.Remove(s);
			}

			List<string> serverNames = new List<string>();
			foreach(Server serv in mainActivity.servers)
				serverNames.Add(serv.Name);

			mainActivity.RunOnUiThread
			(new Action(() => mainActivity.serverList.Adapter = new ArrayAdapter(mainActivity,Android.Resource.Layout.SimpleListItem1,serverNames)));
		}
	}
}

