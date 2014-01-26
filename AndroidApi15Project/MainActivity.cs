using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace AndroidApi15Project
{
	[Activity (Label = "Пример приложения Xamarin", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			Button GPSButton = FindViewById<Button> (Resource.Id.OpenGPSActivityButton);
			GPSButton.Click += delegate {
				var GPSIntent=new Intent(this,typeof(GPSActivity));
				StartActivity(GPSIntent);
			};
			Button WebButton = FindViewById<Button> (Resource.Id.OpenWebRequestActivityButton);
			WebButton.Click += delegate {
				var WebIntent=new Intent(this,typeof(WebRequestActivity));
				StartActivity(WebIntent);
			};
		}
	}
}


