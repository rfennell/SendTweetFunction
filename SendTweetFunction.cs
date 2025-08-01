using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OAuth;
using System.Text;

namespace SendTweetFunction;

public class SendTweetFunction
{
    private readonly ILogger<SendTweetFunction> _logger;

    public SendTweetFunction(ILogger<SendTweetFunction> logger)
    {
        _logger = logger;

    }

    [Function("Post")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function to send a Tweet.");

        string oauth_consumer_key = Environment.GetEnvironmentVariable("oauth_consumer_key");
        string oauth_consumer_secret = Environment.GetEnvironmentVariable("oauth_consumer_secret");
        string oauth_token = Environment.GetEnvironmentVariable("oauth_token");
        string oauth_token_secret = Environment.GetEnvironmentVariable("oauth_token_secret");

        string url = "https://api.twitter.com/2/tweets";

        string tweet = req.Query["tweet"];
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        tweet = tweet ?? data?.tweet;



        _logger.LogInformation($"Message:{tweet}");

        var oauth = new OAuthMessageHandler(oauth_consumer_key, oauth_consumer_secret, oauth_token, oauth_token_secret);

        var tweetData = new { text = tweet };
        var jsonData = JsonConvert.SerializeObject(tweetData);

        var createTweetRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
        };

        using var httpClient = new HttpClient(oauth);

        var response = await httpClient.SendAsync(createTweetRequest);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to send tweet. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            return new BadRequestObjectResult("Failed to send tweet.");
        }
        else
        {
            _logger.LogInformation("Tweet sent successfully!");
            return new OkObjectResult("Tweet sent successfully!");
        }

    }
}