using System;

namespace SocialBeacon
{
	public static class SBConst
	{
		public const string S_ROOT_FOLDER = @"data";
		public const string S_ROOT_LOGS_FOLDER = @"data\logs\";
		public const string S_ROOT_IMG_FOLDER = @"data\profilepics\";

		public const string C_ROOT_IMG_FOLDER = @"profilepics/";
		public const string C_ROOT_CHAT_FOLDER = @"chathistory/";

		public const string PP_NAME = "profileimg.jpg";
		public const string TEMP_PP_NAME = "temp_profilepic.jpg";
		public const string PP_EXTENSION_NAME = ".jpg";
		public const string FLAG_START_NAME = "CFLAG";
		public const string LOG_EXTENSION_NAME = ".sblog";
		public const string CHAT_EXTENSION_NAME = ".sbchat";

		public const string CHAT_DELIMITER = " [#] ";

		public static class Intents
		{
			public const string GOT_NEW_SERVER = "sb.got.new.server";
			public const string GOT_NEW_ONLINE_USER = "sb.got.new.online.user";
			public const string GOT_NEW_CHAT_MESSAGE = "sb.got.new.chat.message";
		}

		public static class Prefs
		{
			public const string PROFILE_PIC = "PROFILE_PIC";
			public const string FIRST_NAME = "FIRST_NAME";
			public const string LAST_NAME = "LAST_NAME";
			public const string AGE = "AGE";
			public const string COUNTRY = "COUNTRY";
			public const string CITY = "CITY";
			public const string PHONE ="PHONE";
			public const string EMAIL ="EMAIL";
			public const string ABOUT = "ABOUT";

			public const string TEMP_PROFILE_PIC = "TEMP_PROFILE_PIC";
			public const string TEMP_FIRST_NAME = "TEMP_FIRST_NAME";
			public const string TEMP_LAST_NAME = "TEMP_LAST_NAME";
			public const string TEMP_AGE = "TEMP_AGE";
			public const string TEMP_COUNTRY = "TEMP_COUNTRY";
			public const string TEMP_CITY = "TEMP_CITY";
			public const string TEMP_PHONE ="TEMP_PHONE";
			public const string TEMP_EMAIL ="TEMP_EMAIL";
			public const string TEMP_ABOUT = "TEMP_ABOUT";

			public const string IS_VALID_USER = "IS_VALID_USER";
			public const string UID = "UID";

			public const string MESSAGE = "MESSAGE";

			public const string REMOTE_IP = "REMOTE_IP";
			public const string ADAPTER_IP = "ADAPTER_IP";

			public const string USERS_LIST = "USERS_LIST";

			public const string ME = "ME";
		}

        public static class ConnectionSettings
        {
            public const int SERVER_BROADCAST_PORT = 8373;
            public const int CLIENT_BROADCAST_PORT = 8375;

			public const int USERSLIST_BRAODCAST_PORT = 8573;

			public const int CHATMSG_SEND_PORT = 8473;
			public const int CHATMSG_RECEIVE_PORT = 8475;
        }

		public static class RequestCodes
		{
			public const int REQ_LOAD_IMAGE = 0;
			public const int REQ_CROP_IMAGE = 1;
		}
	}
}

