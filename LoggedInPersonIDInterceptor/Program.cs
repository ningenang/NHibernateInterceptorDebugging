using DAL.Service.Classes;
using log4net;
using LoggedInPersonIDInterceptor.TestService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
	class Program
	{

		private static readonly ILog log = LogManager.GetLogger("ConsoleApp");
		private static bool isRunning = true;

		static void Main(string[] args)
		{
			log4net.Config.XmlConfigurator.Configure();

			try
			{
				//Make sure WcfService has started
				Thread.Sleep(4000);

				Task.Run(async () =>
				{
					var w1 = SpawnWriter("foo@client.com", "BoatyMcBoatFace");
					Thread.Sleep(2000); //Avoid race-condition
					var w2 = SpawnWriter("bar@client.com", "Titanic");

					await Task.WhenAll(w1, w2);
				});


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				Console.ReadLine();
			}			
			

		}

		private static async Task SpawnWriter(string username, string shipname)
		{
			await Task.Run(() => {
				try
				{
					log.Info($"Retrieving user account for {username}");
					var person = new PersonService().GetByUserIdentification(username);
					if (person == null)
						throw new ArgumentException($"No user account exists for {username}");

					log.Info($"Creating sample voyage for {username}");
					var eta = DateTime.Now.AddDays(2);

					//Create an initial voyage
					var voyage = new VoyageService().SaveOrUpdate(new DAL.DTO.Classes.Voyage
					{
						ShipName = shipname,
						FromLocation = "NOTRD",
						ToLocation = "NOBGO",
						ETA = eta,
						ETD = DateTime.Now.AddHours(1),
						LoggedInPersonID = person.PersonID
					});

					log.Info("Spawning client ...");
					var client = new TestServiceClient();

					while (isRunning)
					{
						var response = client.SaveOrUpdateVoyage(new WcfService.Schema.VoyageRequest
						{
							Header = new WcfService.Schema.RequestHeaderType
							{
								UserName = username
							},
							Body = new WcfService.Schema.VoyageType
							{
								VoyageID = voyage.VoyageID,
								VoyageIDSpecified = true,
								ETA = eta.AddMinutes(1)
							}
						});

						if (response.StatusCode != WcfService.Schema.StatusCodeEnumType.OK)
						{
							isRunning = false; //Abort further execution
							log.Error($"{response.StatusCode} - {response.StatusMessage}");
						}
						else
						{
							log.Debug($"{response.StatusCode}");
						}

					}
				}
				catch (Exception ex)
				{
					log.Error(ex);
				}
			});
		}


	}
}
