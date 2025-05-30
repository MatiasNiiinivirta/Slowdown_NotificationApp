using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Media;
using NAudio.Wave;
using System.Windows.Media.Imaging;

namespace Slowdown
{
	/// <summary>
	/// Interaction logic for RoomView.xaml
	/// </summary>
	public partial class RoomView : Window
    {
		private HubConnection _connection;
		private string _roomCode;
		private readonly string HubUrl;
		private int joinedUsersCount;
		private WaveOutEvent _outputDevice;
		private AudioFileReader _audioFile;

		// Ylikuormitettu konstruktori – liittyy olemassa olevaan huoneeseen
		public RoomView(HubConnection connection, string roomCode)
		{
			InitializeComponent();

			HubUrl = "https://slowdown-dev.azurewebsites.net/roomhub";
			_connection = connection;
			_roomCode = roomCode;

			WelcomeToRoom.Content = $"Welcome to room No: {_roomCode}";

			// Pyyntöjen määrän päivitys käyttöliittymässä
			SetupUserJoinedHandler();
			ClickCounterHandler();
		}

		private async void BTN_Slowdown_Click(object sender, RoutedEventArgs e)
		{
			// Lähetetään klikkaukset serverille
			try
			{
				if (_connection != null && _connection.State == HubConnectionState.Connected)
				{
					// Soitetaan ääni
					if (_outputDevice != null)
					{
						_outputDevice.Dispose();
						_outputDevice = null;
					}

					_audioFile = new AudioFileReader("Sounds/NotificationSound.wav")
					{
						Volume = (float)VolumeSlider.Value  // 0.0–1.0
					};

					_outputDevice = new WaveOutEvent();
					_outputDevice.Init(_audioFile);
					_outputDevice.Play();

					await _connection.InvokeAsync("Request", _roomCode);

					// Estetään napin klikkaus 15 sekunniksi
					BTN_Slowdown.IsEnabled = false;
					BTN_Slowdown.Background = Brushes.Gray;
					BTN_Slowdown.Content = "Request Sent";

					await Task.Delay(15000); // 15 sekunnin odotus, muuta tarvittaessa - huolehdi, että tämä on sama kuin palvelimen puolella

					// Palautetaan nappi käyttöön
					BTN_Slowdown.IsEnabled = true;
					BTN_Slowdown.Background = Brushes.IndianRed;
					BTN_Slowdown.Content = "Slow Down!";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Virhe: {ex.Message}");
			}
		}

		// Pyyntöjen määrän päivitys käyttöliittymässä
		private void ClickCounterHandler()
		{
			if (_connection != null)
			{
				_connection.On<int>("ReceiveClickCount", (clickCount) =>
				{
					// Päivitetään määrä käyttöliittymässä
					Dispatcher.Invoke(() =>
					{
						SlowdownCounter.Content = clickCount.ToString();
					});
				});
			}
		}

		public void UpdateUserCount(int count)
		{
			Dispatcher.Invoke(() =>
			{
				JoinedCountLabel.Content = count.ToString();
			});
		}

		private void SetupUserJoinedHandler()
		{
			if (_connection != null)
			{
				_connection.On<int>("UserJoined", (userCount) =>
				{
					UpdateUserCount(userCount); // tämä päivittää käyttöliittymän
				});
			}
		}

		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (_audioFile != null)
			{
				_audioFile.Volume = (float)e.NewValue;

			}

			if (VolumeSlider.Value == 1 && VolumeSlider.Value >= 0.6)
			{
				VolumeImage.Source = new BitmapImage(new Uri("Images/SoundWaves3.png", UriKind.Relative));


			}
			else if (VolumeSlider.Value <= 0.6 && VolumeSlider.Value >= 0.3)
			{
				VolumeImage.Source = new BitmapImage(new Uri("Images/SoundWaves2.png", UriKind.Relative));
			}
			else if (VolumeSlider.Value <= 0.3 && VolumeSlider.Value > 0)
			{
				VolumeImage.Source = new BitmapImage(new Uri("Images/SoundWaves1.png", UriKind.Relative));
			}
			else if (VolumeSlider.Value == 0)
			{
				VolumeImage.Source = new BitmapImage(new Uri("Images/MuteIcon.png", UriKind.Relative));
			}

		}

		private async void ExitButton_Click(object sender, RoutedEventArgs e)
		{

		    MainWindow mainWindow = new MainWindow();
			if (_connection != null && _connection.State == HubConnectionState.Connected)
			{
				await _connection.StopAsync();
				await _connection.DisposeAsync();
			}
			mainWindow.Show();
			this.Close();

		}
	}
}
