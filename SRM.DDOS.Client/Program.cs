using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Configuration.Json;
    
    // See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

var config = configuration.Build();
var host = config["Configurations:HostAddress"];
var counter = Convert.ToInt32(config["Configurations:Counter"]);
HttpClient httpClient = new HttpClient();
var url = $"{host}api/WeatherForecast";
//https://localhost:7286/api/WeatherForecast
for (int i = 0; i < counter; i++)
{
    var response = await httpClient.GetAsync(url);
    if (response.IsSuccessStatusCode)
    {
        var data = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Counter {i} : Status {response.StatusCode}");
        //Console.WriteLine(data);
    }
    else
    {
        Console.WriteLine(response.StatusCode.ToString());
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}
