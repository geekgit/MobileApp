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
using System.Net;
using Android.Text;

namespace AndroidApi15Project
{
	[Activity (Label = "Тестирование веб-запросов")]			
	public class WebRequestActivity : Activity
	{
		public string WebRequest(string Link,string Method,string Data)
		{
			WebClient wc = new WebClient ();
			return wc.UploadString (Link, Method, Data);
		}
		public async void PostRequestAsync(Uri Link,string Data,Action<object,UploadStringCompletedEventArgs> ActionAfterUpload=null)
		{
			WebClient wc = new WebClient ();
			wc.Headers ["Content-Type"] = "application/x-www-form-urlencoded";
			wc.Encoding = Encoding.UTF8;
			wc.UploadStringCompleted += new UploadStringCompletedEventHandler (ActionAfterUpload);
			wc.UploadStringAsync (Link, "POST", Data);
		}
		public async void GetRequestAsync(Uri Link,Action<object,DownloadStringCompletedEventArgs> ActionAfterDownload=null)
		{
			WebClient wc = new WebClient ();
			wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler (ActionAfterDownload);
			wc.DownloadStringAsync (Link);
		}
		public void AfterUploadAction(object sender, UploadStringCompletedEventArgs args)
		{
			TextView tv = FindViewById<TextView> (Resource.Id.RequestTextVIew);
			string data = "";
			try
			{
				data=args.Result;
			}
			catch(Exception E) {
				data = E.InnerException.InnerException.Message;
			}
			RunOnUiThread (()=>tv.TextFormatted=Html.FromHtml(data));
		}
		public void AfterDownloadAction(object sender, DownloadStringCompletedEventArgs args)
		{
			TextView tv = FindViewById<TextView> (Resource.Id.RequestTextVIew);
			string data = "";
			try
			{
				data=args.Result;
			}
			catch(Exception E) {
				data = E.InnerException.InnerException.Message;
			}
			RunOnUiThread (()=>tv.TextFormatted=Html.FromHtml(data));
		}
		protected override void OnCreate (Bundle bundle)
		{

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.WebRequestLayout);
			Button PostButton = FindViewById<Button> (Resource.Id.PostRequestButton);
			PostButton.Click += delegate {
				PostRequestAsync(new Uri("http://supacoder.jp/request.php"),"?test=hello_from_POST",AfterUploadAction);
			};
			Button GetButton = FindViewById<Button> (Resource.Id.GetRequestButton);
			GetButton.Click += delegate {
				GetRequestAsync(new Uri("http://supacoder.jp/request.php?test=hello_from_GET"),AfterDownloadAction);
			};
		}
	}
}

