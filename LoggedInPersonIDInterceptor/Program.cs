

using LoggedInPersonIDInterceptor.TestService;
using System;
using WcfService.Schema;

namespace ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{

			try
			{
				var client = new TestServiceClient();

				var response = client.SaveOrUpdateVoyage(new VoyageRequest
				{
					Header = new RequestHeaderType
					{
						UserName = "foo@client.com"
					}
				});

				Console.WriteLine($"{response.StatusCode} {response.StatusMessage}");

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
	}
}
