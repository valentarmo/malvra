using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace malvra
{
    public class Flower
    {
        public string Name { get; set; }
        public float PlantingRadius { get; set; }
    }

    public static class Flowers
    {
        public static async Task<List<Flower>> GetFlowers()
        {
            string flowersJSON = await GetData();
            List<Flower> flowers = JsonConvert.DeserializeObject<List<Flower>>(flowersJSON);
            return flowers;
        }

        private static async Task<string> GetData()
        {
            string baseUrl = "URL";
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(baseUrl);
            using HttpContent content = response.Content;
            return await content.ReadAsStringAsync();
        }
    }
}
