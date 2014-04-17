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
using System.Reflection;

using AbstractClasses;
using Dash;

namespace SocialBeacon
{
	[Activity (Label = "User Details")]			
	public class InfoViewForm : Activity
	{
		List<object> drawableIDs;
		string[] countryNames;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.InfoViewForm);

			drawableIDs = typeof(Resource.Drawable).GetFields(BindingFlags.Public | BindingFlags.Static |
				BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name.StartsWith(SBConst.FLAG_START_NAME))
				.Select(val => val.GetRawConstantValue())
				.ToList();

			countryNames = this.Resources.GetStringArray(Resource.Array.country_names);

			Person me = DashSerializer.Deserialize(new SerialData(Intent.GetByteArrayExtra(SBConst.Prefs.ME))) as Person;

			ImageView profilePic = FindViewById<ImageView>(Resource.Id.profilePic),
			flag = FindViewById<ImageView>(Resource.Id.flag);

			TextView firstName = FindViewById<TextView>(Resource.Id.firstNameTV),
			lastName = FindViewById<TextView>(Resource.Id.lastNameTV),
			age = FindViewById<TextView>(Resource.Id.ageTV),
			country = FindViewById<TextView>(Resource.Id.countryTV),
			city = FindViewById<TextView>(Resource.Id.cityTV),
			phone = FindViewById<TextView>(Resource.Id.phoneTV),
			email = FindViewById<TextView>(Resource.Id.emailTV),
			about = FindViewById<TextView>(Resource.Id.aboutTV);

			profilePic.SetImageURI(Android.Net.Uri.Parse(this.FilesDir + "/" + SBConst.C_ROOT_IMG_FOLDER + me.UID + SBConst.PP_EXTENSION_NAME));
			flag.SetImageResource((int)drawableIDs[Array.IndexOf(countryNames,me.Country)]);

			firstName.Text = me.FirstName;
			lastName.Text = me.LastName;
			age.Text = me.Age.ToString();
			country.Text = me.Country;
			city.Text = me.City;
			phone.Text = me.Phone;
			email.Text = me.Email;
			about.Text = me.About;

			Button backBtn = FindViewById<Button>(Resource.Id.backBtn);
			backBtn.Click += (sender, e) => Finish();;
		}
	}
}

