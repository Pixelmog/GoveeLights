using System;
using System.Net.Http; 
using System.Net.Http.Headers; 
using System.Text; 
using System.Threading.Tasks; 
using System.Drawing; 
using System.Collections.Generic;

public class CPHInline
{
	
	public string userInput; //the color, scene, or hex code the user inputs with a channel point redemption
    public string apiKey = Environment.GetEnvironmentVariable("goveeAPIKey"); //Request API key on Govee app
    public string device = Environment.GetEnvironmentVariable("goveeDevice"); //Find by using the getDevices function
    public string model = Environment.GetEnvironmentVariable("goveeModel"); //Find by using the getDevices function
	
	
	Dictionary<string, (int paramId, int id)> sceneMap = new Dictionary<string, (int, int)>(StringComparer.OrdinalIgnoreCase)
	{
		{ "Sunrise", (2686, 2578) },
		{ "Sunset", (2687, 2579) },
		{ "Forest", (2688, 2580) },
		{ "Aurora", (2689, 2581) },
		{ "Starry Sky", (2690, 2582) },
		{ "Spring", (2691, 2583) },
		{ "Summer", (2692, 2584) },
		{ "Fall", (2693, 2585) },
		{ "Winter", (2694, 2586) },
		{ "Rainbow", (2695, 2587) },
		{ "Fire", (2696, 2588) },
		{ "Wave", (2697, 2589) },
		{ "Deep sea", (2698, 2590) },
		{ "Glacier", (2699, 2591) },
		{ "Desert", (2700, 2592) },
		{ "Moonlight", (2701, 2593) },
		{ "Flower Field", (2702, 2594) },
		{ "Downpour", (2703, 2595) },
		{ "Sunny", (2704, 2596) },
		{ "Volcano", (2705, 2597) },
		{ "Meteor shower", (2706, 2598) },
		{ "Cherry blossoms", (2707, 2599) },
		{ "Stream", (2708, 2600) },
		{ "Christmas", (2709, 2601) },
		{ "Halloween", (2710, 2602) },
		{ "Easter", (2711, 2603) },
		{ "Carnival", (2712, 2604) },
		{ "Mother's Day", (2713, 2605) },
		{ "Father's Day", (2714, 2606) },
		{ "Valentine's Day", (2715, 2607) },
		{ "Candlelight", (2716, 2608) },
		{ "Dance Party", (2717, 2609) },
		{ "Birthday", (2718, 2610) },
		{ "Sweet", (2719, 2611) },
		{ "Romantic", (2720, 2612) },
		{ "Movie", (2721, 2613) },
		{ "Game", (2722, 2614) },
		{ "Technology", (2723, 2615) },
		{ "Candy", (2724, 2616) },
		{ "Business", (2725, 2617) },
		{ "Reading", (2726, 2618) },
		{ "Work", (2727, 2619) },
		{ "Dreamland", (2728, 2620) },
		{ "Night", (2729, 2621) },
		{ "Sleep", (2730, 2622) },
		{ "Energetic", (2731, 2623) },
		{ "Profound", (2732, 2624) },
		{ "Quiet", (2733, 2625) },
		{ "Warm", (2734, 2626) },
		{ "Longing", (2735, 2627) },
		{ "Happy", (2736, 2628) },
		{ "Excited", (2737, 2629) },
		{ "Optimistic", (2738, 2630) },
		{ "Heartbeat", (2739, 2631) },
		{ "Cheerful", (2740, 2632) },
		{ "Dancing", (2741, 2633) },
		{ "Flowing Light", (2742, 2634) },
		{ "Alternate", (2743, 2635) },
		{ "Breathe", (2744, 2636) },
		{ "Fight", (2745, 2637) },
		{ "Stacking", (2746, 2638) },
		{ "Firefly", (2747, 2639) },
		{ "Greedy Snake", (2748, 2640) },
		{ "Blossom", (2749, 2641) },
		{ "Split", (2750, 2642) }
	};
	
	public bool Execute()
	{
		
		CPH.TryGetArg("rawInput", out userInput); //get the string that the viewer put into the channel point redemption 
		
		if(sceneMap.TryGetValue(userInput, out var scene))
		{
			setDynamicScene(scene.paramId, scene.id); 
		}
		
		else if(System.Drawing.Color.FromName(userInput).IsKnownColor)
		{
			updateColor(userInput); 
		}
		else if(userInput.StartsWith("#") && tryParseHexColor(userInput, out int r, out int g, out int b))
		{
			updateColor(r, g, b); 
		}
		else
		{
			CPH.SendMessage("That is not a valid input! Going back to default"); 
			setDynamicScene(2692, 2584); 
		}
		
		
		//getAvailableScenes(); use this function to see what scenes are available on your device. Will go to streamer.bot logs 
		//getDevices(); //use this function if you need your device name. Will go to streamer.bot logs
		return true;
	}
	
	public async Task updateColor(string colorName)
    {
      
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
    
	public async Task updateColor(int r, int g, int b)
    {
 
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
    
    public async Task setDynamicScene(int paramId, int id)
    {

        string requestId = Guid.NewGuid().ToString();

        string json = $@"
        {{
            ""requestId"": ""{requestId}"",
            ""payload"": {{
                ""sku"": ""{model}"",
                ""device"": ""{device}"",
                ""capability"": {{
                    ""type"": ""devices.capabilities.dynamic_scene"",
                    ""instance"": ""lightScene"",
                    ""value"": {{
                        ""paramId"": {paramId},
                        ""id"": {id}
                    }}
                }}
            }}
        }}";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Govee-API-Key", apiKey);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://openapi.api.govee.com/router/api/v1/device/control", content);

            var result = await response.Content.ReadAsStringAsync();
            CPH.LogInfo($"Scene activation response: {result}");
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
    
	public async Task getAvailableScenes()
    {
    	string apiKey = Environment.GetEnvironmentVariable("goveeAPIKey"); //Request API key on Govee app
    	string device = Environment.GetEnvironmentVariable("goveeDevice"); //Find by using the getDevices function
    	string model = Environment.GetEnvironmentVariable("goveeModel"); //Find by using the getDevices function

        string requestId = Guid.NewGuid().ToString(); // Unique per request

        string json = $@"
        {{
            ""requestId"": ""{requestId}"",
            ""payload"": {{
                ""sku"": ""{model}"",
                ""device"": ""{device}""
            }}
        }}";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Govee-API-Key", apiKey);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://openapi.api.govee.com/router/api/v1/device/scenes", content);

            string result = await response.Content.ReadAsStringAsync();
            CPH.LogInfo("Scene query response: " + result);
        }
    }
    
    public bool tryParseHexColor(string hex, out int r, out int g, out int b)
	{
		r = g = b = 0;

		if (string.IsNullOrEmpty(hex))
			return false;

		hex = hex.TrimStart('#');

		if (hex.Length != 6)
			return false;

		try
		{
			r = Convert.ToInt32(hex.Substring(0, 2), 16);
			g = Convert.ToInt32(hex.Substring(2, 2), 16);
			b = Convert.ToInt32(hex.Substring(4, 2), 16);
			return true;
		}
		catch
		{
			return false;
		}
	}

}


