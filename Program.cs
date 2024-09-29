using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace lab6_1
{
    public struct Weather
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public double Temp { get; set; }
        public string Description { get; set; }
    }

    public class MakeWeather
    {
        public static Weather Make(double latitude, double longitude, string API_key)
        {
            HttpClient httpClient = new HttpClient();
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={API_key}";
            var response = httpClient.GetAsync(url).Result; // асинхронно отправляем запрос на url, тем самым создав задачу. С помощью Result дожидаемся ответа и идем дальше по программе.
            response.EnsureSuccessStatusCode(); // проверяем что ответ пришел, в противном случае выдаст исключение и дальше по программе мы не пойдем
            string responseBody = response.Content.ReadAsStringAsync().Result; // из ответа достаем тело ответа и асинхронно переводим его в JSON-строку, дожидаясь конца выполнения перевода
            WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(responseBody);

            return new Weather()
            {
                Country = weatherData.sys.country,
                Name = weatherData.name,
                Temp = weatherData.main.temp,
                Description = weatherData.weather[0].description
            };
        }
    }

    public class WeatherData
    {
        public Sys sys;
        public string name;
        public Main main;
        public WeatherOBJ[] weather;
    }

    public class Sys
    {
        public string country;
    }

    public class Main
    {
        public double temp;
    }

    public class WeatherOBJ
    {
        public string description;
    }

    public class CollectionWeather
    {
        public static Weather[] MakeCollectionWeather(){
            Random random = new Random();
            Console.WriteLine("Введите количество мест, в которых надо узнать погоду");
            int count = Convert.ToInt32(Console.ReadLine());
            Weather[] mass = new Weather[count];
            int i = 0;
            while(i != count)
            {
                Weather w = MakeWeather.Make(-90 + 180 * random.NextDouble(), -180 + 360 * random.NextDouble(), "db6642f622789c2dbe748c0ab3e0b176");
                if (w.Country == "" || w.Name == "") continue;
                mass[i] = w;
                i++;
            }
            return mass;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Weather[] arrW = CollectionWeather.MakeCollectionWeather(); // тут потребуется ввести число >=50; Выполнения занимает порядка 30 секунд.

            var selectedWeather = from w in arrW
                                  orderby w.Temp
                                  select w;

            Console.WriteLine($"Максимальная температура в городе: {selectedWeather.ToArray()[arrW.Length-1].Name}");
            Console.WriteLine($"Минимальная температура в городе: {selectedWeather.ToArray()[0].Name}") ;

            var averageTemp = arrW.Select(w => w.Temp).Average();
            Console.WriteLine($"Средняя температура в мире: {averageTemp} К");

            var countryCount = arrW.Select(w => w.Country).Distinct().Count();
            Console.WriteLine($"Количество стран в коллекции: {countryCount}");

            var clearSkyWeather = (from w in arrW
                                   where w.Description == "clear sky"
                                   select w).FirstOrDefault();

            var rainWeather = (from w in arrW
                               where w.Description == "rain"
                               select w).FirstOrDefault();

            var fewCloudsWeather = (from w in arrW
                                    where w.Description == "few clouds"
                                    select w).FirstOrDefault();

            if (!clearSkyWeather.Equals(null))
            {
                Console.WriteLine($"Первая найденная страна и название местности с ясным небом: {clearSkyWeather.Country}, {clearSkyWeather.Name}");
            }

            if (!rainWeather.Equals(null))
            {
                Console.WriteLine($"Первая найденная страна и название местности с дождем: {rainWeather.Country}, {rainWeather.Name}");
            }

            if (!fewCloudsWeather.Equals(null))
            {
                Console.WriteLine($"Первая найденная страна и название местности с несколькими облаками: {fewCloudsWeather.Country}, {fewCloudsWeather.Name}");
            }
        }
    }
}
