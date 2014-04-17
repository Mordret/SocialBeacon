using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace SocialBeacon
{
	[Activity (Icon = "@drawable/logo", Label = "SocialBeacon")]
	public class InfoEditForm : Activity
	{
		ISharedPreferences prefsReader;
		ISharedPreferencesEditor prefsEditor;

		List<object> drawableIDs;
		string[] countryNames;

		string imgUri, temp_imgUri;
		bool isReturningFromImage = false;

		ImageView profilePic, flag;
		EditText firstName, lastName, age, city, phone, email, about;
		Spinner country;
		Button saveBtn, cancelBtn;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.InfoEditForm);

			prefsReader = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this);
			prefsEditor = prefsReader.Edit();

			drawableIDs = typeof(Resource.Drawable).GetFields(BindingFlags.Public | BindingFlags.Static |
				BindingFlags.FlattenHierarchy)
				.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name.StartsWith("CFLAG"))
				.Select(val => val.GetRawConstantValue())
				.ToList();

			profilePic = FindViewById<ImageView>(Resource.Id.profilePic);
			profilePic.Click += new EventHandler(profileImg_Click);
			flag = FindViewById<ImageView>(Resource.Id.flag);

			imgUri = this.FilesDir + "/" + SBConst.PP_NAME;
			temp_imgUri = this.FilesDir + "/" + SBConst.TEMP_PP_NAME;

			firstName = FindViewById<EditText>(Resource.Id.firstNameET);
			lastName = FindViewById<EditText>(Resource.Id.lastNameET);
			age = FindViewById<EditText>(Resource.Id.ageET);

			country = FindViewById<Spinner>(Resource.Id.countrySPN);
			country.ItemSelected += (sender, e) =>
			{
				flag.SetImageResource((int)drawableIDs[e.Position]);
			};
			countryNames = Resources.GetStringArray(Resource.Array.country_names);

			city = FindViewById<EditText>(Resource.Id.cityET);
			phone = FindViewById<EditText>(Resource.Id.phoneET);
			email = FindViewById<EditText>(Resource.Id.emailET);
			about = FindViewById<EditText>(Resource.Id.aboutET);

			cancelBtn = FindViewById<Button>(Resource.Id.cancelBtn);
			saveBtn = FindViewById<Button>(Resource.Id.saveBtn);

			cancelBtn.Click += (sender, e) => { File.Copy(imgUri,temp_imgUri,true); Finish(); };
			saveBtn.Click += (sender, e) => 
			{
				List<string> result = ParseData();
				if(result.Count == 0)
				{
					prefsEditor.PutString(SBConst.Prefs.PROFILE_PIC, imgUri);
					prefsEditor.PutString(SBConst.Prefs.FIRST_NAME, firstName.Text);
					prefsEditor.PutString(SBConst.Prefs.LAST_NAME, lastName.Text);
					prefsEditor.PutInt(SBConst.Prefs.AGE, int.Parse(age.Text));
					prefsEditor.PutString(SBConst.Prefs.COUNTRY, country.SelectedItem.ToString());
					prefsEditor.PutString(SBConst.Prefs.CITY, city.Text);
					prefsEditor.PutString(SBConst.Prefs.PHONE, phone.Text);
					prefsEditor.PutString(SBConst.Prefs.EMAIL, email.Text);
					prefsEditor.PutString(SBConst.Prefs.ABOUT, about.Text);

					prefsEditor.PutBoolean(SBConst.Prefs.IS_VALID_USER,true);
					prefsEditor.Commit();

					if(File.Exists(temp_imgUri))
						File.Copy(temp_imgUri,imgUri,true);

					Finish();
				}
				else
				{
					profilePic.SetImageURI(Android.Net.Uri.Parse(temp_imgUri));
					AlertDialog.Builder wrongFields = new AlertDialog.Builder(this);
					string fields = "";
					foreach(string s in result)
						fields += s + "\n";
					wrongFields.SetMessage("The following fields are not filled in correctly :\n\n" + fields);
					wrongFields.SetNeutralButton("OK",delegate {} );
					wrongFields.Show();
				}
			};
		}

		void profileImg_Click(object sender, EventArgs e)
		{
			Intent getPic = new Intent(Intent.ActionPick);
			getPic.SetType("image/*");
			getPic.PutExtra("crop","true");
			getPic.PutExtra("return-data", true);
			isReturningFromImage = true;
			StartActivityForResult(Intent.CreateChooser(getPic,"Select an app :"),SBConst.RequestCodes.REQ_LOAD_IMAGE);
		}

		protected override void OnResume()
		{
			base.OnResume();

			if(isReturningFromImage)
			{
				//newPicUrl = prefsReader.GetString(SBConst.Prefs.TEMP_PROFILE_PIC,"");
				firstName.Text = prefsReader.GetString(SBConst.Prefs.TEMP_FIRST_NAME,"");
				lastName.Text = prefsReader.GetString(SBConst.Prefs.TEMP_LAST_NAME,"");
				age.Text = prefsReader.GetInt(SBConst.Prefs.TEMP_AGE,0).ToString();
				country.SetSelection(Array.IndexOf(countryNames,prefsReader.GetString(SBConst.Prefs.TEMP_COUNTRY,"")));
				city.Text = prefsReader.GetString(SBConst.Prefs.TEMP_CITY,"");
				phone.Text = prefsReader.GetString(SBConst.Prefs.TEMP_PHONE,"");
				email.Text = prefsReader.GetString(SBConst.Prefs.TEMP_EMAIL,"");
				about.Text = prefsReader.GetString(SBConst.Prefs.TEMP_ABOUT,"");
			}
			else
			{
				profilePic.SetImageURI(Android.Net.Uri.Parse(imgUri));
				firstName.Text = prefsReader.GetString(SBConst.Prefs.FIRST_NAME,"");
				lastName.Text = prefsReader.GetString(SBConst.Prefs.LAST_NAME,"");
				int ageValue = prefsReader.GetInt(SBConst.Prefs.AGE,0);
				age.Text = ageValue == 0 ? "" : ageValue.ToString();
				string[] countrynames = this.Resources.GetStringArray(Resource.Array.country_names);
				string selectedcountry = prefsReader.GetString(SBConst.Prefs.COUNTRY,"Select");
				country.SetSelection(Array.IndexOf(countrynames,selectedcountry));
				city.Text = prefsReader.GetString(SBConst.Prefs.CITY,"");
				phone.Text = prefsReader.GetString(SBConst.Prefs.PHONE,"");
				email.Text = prefsReader.GetString(SBConst.Prefs.EMAIL,"");
				about.Text = prefsReader.GetString(SBConst.Prefs.ABOUT,"");
			}
		}

		protected override void OnPause()
		{
			base.OnPause();
			int myAge = 0;
			int.TryParse(age.Text, out myAge);
			//prefsEditor.PutString(SBConst.Prefs.TEMP_PROFILE_PIC, newPicUrl);
			prefsEditor.PutString(SBConst.Prefs.TEMP_FIRST_NAME, firstName.Text);
			prefsEditor.PutString(SBConst.Prefs.TEMP_LAST_NAME, lastName.Text);
			prefsEditor.PutInt(SBConst.Prefs.TEMP_AGE, myAge);
			prefsEditor.PutString(SBConst.Prefs.TEMP_COUNTRY, country.SelectedItem.ToString());
			prefsEditor.PutString(SBConst.Prefs.TEMP_CITY, city.Text);
			prefsEditor.PutString(SBConst.Prefs.TEMP_PHONE, phone.Text);
			prefsEditor.PutString(SBConst.Prefs.TEMP_EMAIL, email.Text);
			prefsEditor.PutString(SBConst.Prefs.TEMP_ABOUT, about.Text);
			prefsEditor.Commit();
		}

		List<string> ParseData()
		{
			int testInt = 0;
			List<string> wrongFields = new List<string>();

			if(firstName.Text == "" || firstName.Text.Contains(" ") || firstName.Text.Length < 2)
				wrongFields.Add("First Name");
			if(lastName.Text == "" || lastName.Text.Contains(" ") || lastName.Text.Length < 2)
				wrongFields.Add("Last Name");
			int.TryParse(age.Text, out testInt);
			if(age.Text == "" || (testInt <= 0 || testInt > 150))
					wrongFields.Add("Age");
			if(country.SelectedItem.ToString() == "Select")
				wrongFields.Add("Country");
			return wrongFields;
		}

		// RequestCodes reffed in Helpers/SBConst.cs
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (resultCode == Result.Ok)
			{
				switch(requestCode)
				{
				case SBConst.RequestCodes.REQ_LOAD_IMAGE:
//					Intent cropImage = new Intent("com.android.camera.action.CROP");
//					cropImage.SetDataAndType(data.Data,"image/*");
					Bitmap bitmap = data.Extras.GetParcelable("data") as Bitmap;
					Bitmap newBitmap = ResizeBitmap(bitmap);
					bitmap.Dispose();
					if(File.Exists(temp_imgUri))
						File.Delete(temp_imgUri);
					using(FileStream fs = new FileStream(temp_imgUri, FileMode.Create))
						newBitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, fs);
					profilePic.SetImageDrawable(null);
					profilePic.SetImageURI(Android.Net.Uri.Parse(temp_imgUri));
					//	StartActivityForResult(cropImage,SBConst.RequestCodes.REQ_CROP_IMAGE);
					break;
//				case SBConst.RequestCodes.REQ_CROP_IMAGE:
//					// Bitmap bitmap = BitmapFactory.DecodeFile(data.Action);
//					Bitmap bitmap = data.Extras.GetParcelable("data") as Bitmap;
//					Bitmap newBitmap = ResizeBitmap(bitmap);
//					bitmap.Dispose();
//					if(File.Exists(temp_imgUri))
//						File.Delete(temp_imgUri);
//					using(FileStream fs = new FileStream(temp_imgUri, FileMode.Create))
//						newBitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, fs);
//					profilePic.SetImageDrawable(null);
//					profilePic.SetImageURI(Android.Net.Uri.Parse(temp_imgUri));
//					break;
				}
			}
		}

		Bitmap ResizeBitmap(Bitmap originalBitmap)
		{
			const int maxSize = 240;
			int outWidth;
			int outHeight;
			int inWidth = originalBitmap.Width;
			int inHeight = originalBitmap.Height;
			if(inWidth > inHeight){
				outWidth = maxSize;
				outHeight = (inHeight * maxSize) / inWidth; 
			} else {
				outHeight = maxSize;
				outWidth = (inWidth * maxSize) / inHeight; 
			}
			return Bitmap.CreateScaledBitmap(originalBitmap, outWidth, outHeight, false);
		}
	}
}


