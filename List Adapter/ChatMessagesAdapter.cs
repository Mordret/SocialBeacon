using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;

using AbstractClasses;

namespace SocialBeacon
{
	class ChatMessagesAdapter : BaseAdapter<ChatMessage>
	{
		List<ChatMessage> items;
		Activity context;
		Person friend;

		double density,ratio;
		string filesDir;

		Bitmap myBitmap = null, friendBitmap = null;
		string myUID, friendUID, imgUri;

		public ChatMessagesAdapter(Activity context, Person friend, string filesDir, List<ChatMessage> items,string myUID, string friendUID, double ratio, double density)
		{
			this.friend = friend;
			this.context = context;
			this.filesDir = filesDir;
			this.items = items;
			this.ratio = ratio;
			this.density = density;
			this.myUID = myUID;
			this.friendUID = friendUID;

			BitmapFactory.Options options = new BitmapFactory.Options();
			options.InSampleSize = 2;
			imgUri = filesDir + SBConst.C_ROOT_IMG_FOLDER + myUID + SBConst.PP_EXTENSION_NAME;
			myBitmap = BitmapFactory.DecodeFile(imgUri, options);
			imgUri = filesDir + SBConst.C_ROOT_IMG_FOLDER + friendUID + SBConst.PP_EXTENSION_NAME;
			friendBitmap = BitmapFactory.DecodeFile(imgUri,options);
		}

		public override long GetItemId(int position)
		{
			return position;
		}
		public override ChatMessage this[int index]
		{
			get { return items[index]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = null;
			View isCVMine = null;
			if(convertView != null)
				isCVMine = convertView.FindViewById<LinearLayout>(Resource.Id.myDif);
			if(items[position].IsMine)
			{
				if(convertView != null)
				{
					if(isCVMine != null) view = convertView;
					else view = convertView = context.LayoutInflater.Inflate (Resource.Layout.MyChatBubble, null);
				}
				else view = context.LayoutInflater.Inflate (Resource.Layout.MyChatBubble, null);
			}
			else
			{
				if(convertView != null)
				{
					if(isCVMine != null) view = convertView = context.LayoutInflater.Inflate (Resource.Layout.FriendChatBubble, null);
					else view = convertView;
				}
				else view = context.LayoutInflater.Inflate (Resource.Layout.FriendChatBubble, null);
			}
			TextView personName = view.FindViewById<TextView>(Resource.Id.personName);
			personName.Text = items[position].IsMine ? "Me" : friend.FirstName + " " + friend.LastName;
			TextView chatMsg = view.FindViewById<TextView> (Resource.Id.chatMsg);
			chatMsg.Text = items [position].Message;
			ImageView image = view.FindViewById<ImageView> (Resource.Id.profilePic);

//			BitmapDrawable bd = (BitmapDrawable)image.Drawable;
//			if(bd.Bitmap != null)
//			{
//				bd.Bitmap.Recycle();
//				image.SetImageBitmap(null);
//			}
			image.SetImageBitmap(items[position].IsMine ? myBitmap : friendBitmap);
			return view;
		}
	}
}