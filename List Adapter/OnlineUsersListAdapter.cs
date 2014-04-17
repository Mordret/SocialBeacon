using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Linq;
using System.Reflection;
using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;

using AbstractClasses;

namespace SocialBeacon
{
	class OnlineUsersListAdapter : BaseAdapter<Person>
	{
		List<Person> users;
		Activity context;

		double density,ratio;
		string filesDir;

		List<object> drawableIDs;
		string[] countrynames;

		string imgUri;
		List<Bitmap> profilePics = new List<Bitmap>();

		public OnlineUsersListAdapter(Activity context, string filesDir, List<Person> users ,double ratio, double density)
		{
			this.context = context;
			this.filesDir = filesDir;
			this.users = users;
			this.ratio = ratio;
			this.density = density;

			foreach(Person user in users)
			{
				imgUri = this.filesDir + "/" + SBConst.C_ROOT_IMG_FOLDER + user.UID + SBConst.PP_EXTENSION_NAME;
				if(File.Exists(imgUri))
				{
					Java.IO.File picture = new Java.IO.File(imgUri);
					BitmapFactory.Options options = new BitmapFactory.Options();
					options.InSampleSize = 2;
					Bitmap myBitmap = BitmapFactory.DecodeFile(imgUri, options);
					profilePics.Add(myBitmap);
				}
				else profilePics.Add(null);
			}

			drawableIDs = typeof(Resource.Drawable).GetFields(BindingFlags.Public | BindingFlags.Static |
				BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name.StartsWith(SBConst.FLAG_START_NAME))
				.Select(val => val.GetRawConstantValue())
				.ToList();

			countrynames = context.Resources.GetStringArray(Resource.Array.country_names);
		}

		public override long GetItemId(int position)
		{
			return position;
		}
		public override Person this[int index]
		{
			get { return users[index]; }
		}
		public override int Count
		{
			get { return users.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.ChatListItem, null);
			TextView firstNameTV = view.FindViewById<TextView>(Resource.Id.firstNameTV);
			TextView lastNameTV = view.FindViewById<TextView>(Resource.Id.lastNameTV);
			ImageView profilePic = view.FindViewById<ImageView>(Resource.Id.profilePic);
			ImageView flag = view.FindViewById<ImageView>(Resource.Id.flag);

			firstNameTV.Text = users[position].FirstName;
			lastNameTV.Text = users[position].LastName;
			if(profilePics[position] != null)
				profilePic.SetImageBitmap(profilePics[position]);
			flag.SetImageResource((int)drawableIDs[Array.IndexOf(countrynames,users[position].Country)]);
			return view;
		}
	}
}