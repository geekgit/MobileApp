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
using Android.Locations;

namespace AndroidApi15Project
{
	[Activity (Label = "Тестирование геолокационных функций")]			
	public class GPSActivity : Activity, ILocationListener
	{
		Location _currentLocation;
		LocationManager _locationManager;
		String _locationProvider;
		TextView GPSTV;
		TextView GPSRawTV;
		TextView AddressTV;
		int LinesCount=0;
		const int LINES_COUNT_LIMIT=5;
		public void AddText(string Data)
		{
			if (LinesCount >= LINES_COUNT_LIMIT) ClearText();
			GPSTV.Text += Data + "\r\n";
			++LinesCount;
		}
		public void ClearText()
		{	
			GPSTV.Text = "";
			LinesCount = 0;
		}
		async void LocationWorks()
		{
			Geocoder geocoder = new Geocoder(this);
			IList<Address> addressList = await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);
			Address address = addressList.FirstOrDefault();
			if (address != null)
			{
				StringBuilder deviceAddress = new StringBuilder();
				for (int i = 0; i < address.MaxAddressLineIndex; i++)
				{
					deviceAddress.Append (address.GetAddressLine (i));
					if (i != address.MaxAddressLineIndex - 1)
						deviceAddress.AppendLine (",");
					else
						deviceAddress.Append (".");
				}
				string strAdr = deviceAddress.ToString ();
				AddText(String.Format("[LocationWorks] Текущий примерный адрес устройства: {0}",strAdr));
				AddressTV.Text = strAdr;
			}
			else
			{
				AddText("[LocationWorks] Невозможно определить текущее местоположение!");
			}
		}
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.GPSInfoLayout);
			GPSRawTV = FindViewById<TextView> (Resource.Id.GPSRawTextView);
			AddressTV = FindViewById<TextView> (Resource.Id.AddressTextView);
			Button FetchButton = FindViewById<Button> (Resource.Id.FetchGPSButton);
			GPSTV = FindViewById<TextView> (Resource.Id.GPSDataTextView);
			FetchButton.Click += delegate {
				AddText("[Click] Press");
				if (_currentLocation == null) AddText("[Click] Пока нельзя определить текущее местоположение");
				else LocationWorks();
			};
			ClearText ();
			InitializeLocationManager();
		}
		protected override void OnResume()
		{
			base.OnResume();
			_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
		}
		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}
		void InitializeLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria {
				//Accuracy = Accuracy.Fine
				Accuracy = Accuracy.NoRequirement,
				PowerRequirement=Power.NoRequirement
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);
		
			foreach (string provider in acceptableLocationProviders) {
				AddText(String.Format("[InitializeLocationManager] Доступные гео-провайдеры: {0}",provider));
			}
			if (acceptableLocationProviders.Any ()) {
				if (acceptableLocationProviders.Contains ("network"))
					_locationProvider = "network"; //использование быстрого A-GPS. Требуется мобильный интернет.
				else if (acceptableLocationProviders.Contains ("gps"))
					_locationProvider = "gps"; //использование очень медленного GPS в оффлайн-режиме
				else
					_locationProvider = acceptableLocationProviders.First ();//использование иного геопровайдера
			}
			else _locationProvider = ""; //GPS нет
			AddText (String.Format("[InitializeLocationManager] Используем '{0}' геопровайдер", _locationProvider));
		}

		public void OnLocationChanged(Location location)
		{
			_currentLocation = location;
			if (_currentLocation == null)
			{
				AddText("[OnLocationChanged] Невозможно определить текущее положение.");
			}
			else
			{
				string geo = String.Format ("{0},{1}", _currentLocation.Latitude.ToString (), _currentLocation.Longitude.ToString ());
				AddText(String.Format("[OnLocationChanged] {0}",geo));
				GPSRawTV.Text = geo;
				LocationWorks ();
			}
		}
		public void OnProviderDisabled(string provider) {}

		public void OnProviderEnabled(string provider) {}

		public void OnStatusChanged(string provider, Availability status, Bundle extras) {}
	}
}

