using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;     // DInjection: read appsettings.Development.json (because current Hosting env is Development)
        }
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync(_configuration["CommandService"], httpContent);      //"CommandService" defined in appsettings.json

            //although async-await: this just apply internally inside Platformservice, meaning it will release thread to thread pool
            // externally (bwt service-service): it still Sync request: waiting for a response...
            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync Post to CommandService was OK");    
            }
            else
            {
                Console.WriteLine("--> Sync Post to CommandService was NOT OK");
            }
        }
    }
}