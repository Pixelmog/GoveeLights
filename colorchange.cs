using System;
using System.Net.Http; 
using System.Net.Http.Headers; 
using System.Text; 
using System.Threading.Tasks; 
using System.Drawing; 

public class CPHInline
{
	
	public string colorName; //the color the user will input 
	public bool Execute()
	{
		
		CPH.TryGetArg("rawInput", out colorName);
		updateColor(colorName); 
		
		//getDevices(); //use this function if you need your device name 
		return true;
	}
	
	
	public async Task updateColor(string colorName)
    {
    	string apiKey = Environment.GetEnvironmentVariable("goveeAPIKey"); //Request API key on Govee app
    	string device = Environment.GetEnvironmentVariable("goveeDevice"); //Find by using the getDevices function
    	string model = Environment.GetEnvironmentVariable("goveeModel"); //Find by using the getDevices function
        
        Color color = Color.FromName(colorName); //from System.Drawing
        int r = color.R; 
        int g = color.G; 
        int b = color.B; 


        string json = $@"
        {{
            ""device"": ""{device}"",
            ""model"": ""{model}"",
            ""cmd"": {{
                ""name"": ""color"",
                ""value"": {{
                    ""r"": {r},
                    ""g"": {g},
                    ""b"": {b}
                }}
            }}
        }}";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Govee-API-Key", apiKey);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://developer-api.govee.com/v1/devices/control", content);

            var result = await response.Content.ReadAsStringAsync();
            CPH.LogInfo($"Status: {response.StatusCode} - {response.ReasonPhrase}");
            CPH.LogInfo($"Response: {result}");
        }
    }
    
    
    public async Task getDevices()
    {
    	//use this function to get information about your Govee devices 
        string apiKey = Environment.GetEnvironmentVariable("goveeAPIKey");//Request API key on Govee app

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Govee-API-Key", apiKey);

            var response = await client.GetAsync("https://developer-api.govee.com/v1/devices");
            var result = await response.Content.ReadAsStringAsync();

            CPH.LogInfo($"Govee Devices: {result}");
        }
    }
}

