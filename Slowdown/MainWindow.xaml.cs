using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.AspNetCore.SignalR.Client;

namespace Slowdown
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private HubConnection _connection;
		private const string CorrectPin = "5555";
		private RoomView _roomView;

		public MainWindow()
		{
			InitializeComponent();
		}

		// Kun tekstikenttään klikataan, tyhjennetään tekstikenttä
		private void RoomPinTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var textBox = sender as TextBox;

			if (textBox != null && textBox.Text == "Room pin")
			{
				textBox.Text = "";
				textBox.Foreground = Brushes.Black;
			}
		}

		private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			// Luetaan syötetty huonekoodi
			string roomCode = RoomPinTextBox.Text.Trim();

			bool creatingNewRoom = false;

			// Jos kenttä on tyhjä tai "Room pin", luodaan uusi koodi
			if (string.IsNullOrWhiteSpace(roomCode) || roomCode.Equals("Room pin", StringComparison.OrdinalIgnoreCase))
			{

				MessageBox.Show("Syötä pinkoodi"); 
			}

			try
			{
				// Luodaan yhteys
				_connection = new HubConnectionBuilder()
					.WithUrl("https://slowdown-dev.azurewebsites.net/roomhub", options =>
					{
						options.HttpMessageHandlerFactory = handler =>
						{
							if (handler is HttpClientHandler clientHandler)
							{
								clientHandler.ServerCertificateCustomValidationCallback +=
									(sender, cert, chain, sslPolicyErrors) => true;
							}
							return handler;
						};
					})
					.WithAutomaticReconnect()
					.Build();

				RoomView roomView = new RoomView(_connection, roomCode);


				_connection.On<int>("UserJoined", (userCount) =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						roomView.JoinedCountLabel.Content = userCount.ToString();
					});
				});


				await _connection.StartAsync();
			// Liitytään huoneeseen
				await _connection.InvokeAsync("JoinRoom", roomCode);

				// Avataan huoneikkuna

				roomView.Show();
				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Virhe yhdistettäessä huoneeseen: {ex.Message}", "Virhe", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void CreateRoomButton_Click(object sender, RoutedEventArgs e)
		{
			string roomCode = RoomPinTextBox.Text.Trim();

			bool creatingNewRoom = false;


			//Tässä kohtaa tarkastetaan, että onko syötetty koodi tyhjä, jos on niin generoidaan randomilla pin koodi ja vataan huone.
			//Jos käyttäjä on syöttänyt jotain tekstikenttään tarkastetaan, että onko kyseessä numeroita vai kirjaimia, jos on numeroita,
			//niin sitten tarkastetaan onko niitä enemmän tai vähemmän kuin neljä, tarpeen mukaan syötetään neljä numeroa tai poistetaan numeroita, jotta niitä jää neljä. 


			if (string.IsNullOrWhiteSpace(roomCode) || roomCode.Equals("Room pin", StringComparison.OrdinalIgnoreCase))
			{
				Random random = new Random();
				roomCode = random.Next(1000, 10000).ToString(); // 4-numeroa
				creatingNewRoom = true;
			}
			else
			{
				if (!roomCode.Any(char.IsLetter))
				{
					// Syöte ei ole pelkästään kirjaimia, jatka prosessointia

					roomCode = RoomPinTextBox.Text.Trim();

					if (roomCode.Length > 4)
					{
						roomCode = roomCode.Substring(0, 4);  // Ottaa vain ensimmäiset 4 merkkiä
					}
					else if(roomCode.Length < 4)
					{
						Random random = new Random();
						roomCode += random.Next(1000, 10000).ToString(); // 4-numeroa
						roomCode = roomCode.Substring(0, 4);
					}

					creatingNewRoom = true;

				}
			}


			try
			{
				// Luodaan yhteys
				_connection = new HubConnectionBuilder()
					.WithUrl("https://slowdown-dev.azurewebsites.net/roomhub", options =>
					{
						options.HttpMessageHandlerFactory = handler =>
						{
							if (handler is HttpClientHandler clientHandler)
							{
								clientHandler.ServerCertificateCustomValidationCallback +=
									(sender, cert, chain, sslPolicyErrors) => true;
							}
							return handler;
						};
					})
					.WithAutomaticReconnect()
					.Build();

				// Avataan huoneikkuna
				RoomView roomView = new RoomView(_connection, roomCode);


				_connection.On<int>("UserJoined", (userCount) =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						roomView.JoinedCountLabel.Content = userCount.ToString();
					});
				});

				await _connection.StartAsync();

				// Jos luodaan uusi huone, kutsutaan ensin CreateRoom
				if (creatingNewRoom)
				{
					await _connection.InvokeAsync("CreateRoom", roomCode);
					MessageBox.Show($"Uusi huone luotu!\nHuonekoodi: {roomCode}", "Huone luotu", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				// Liitytään huoneeseen
				await _connection.InvokeAsync("JoinRoom", roomCode);


				roomView.Show();
				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Virhe yhdistettäessä huoneeseen: {ex.Message}", "Virhe", MessageBoxButton.OK, MessageBoxImage.Error);
			}


		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}

}
