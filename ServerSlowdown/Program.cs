using Microsoft.AspNetCore.SignalR;
using ServerSlowdown.Hubs;

namespace ServerSlowdown
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			//// Lisää tämä ennen app.Build() -kutsua
			//builder.WebHost.ConfigureKestrel(serverOptions =>
			//{
			//	// Kuuntelee ulkoisia IP-osoitteita ja porttia 7107
			//	serverOptions.ListenAnyIP(7107, listenOptions =>
			//	{
			//		listenOptions.UseHttps("certs\\SlowdownGit-cert.pfx", "salasana123");
			//	});
			//});

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy.AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader();
				});
			});

			builder.Services.AddSignalR();
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseCors("AllowAll");
			app.UseAuthorization();

			app.MapHub<RoomHub>("/roomhub");
			app.MapControllers();

			app.Run();
		}
	}
}
