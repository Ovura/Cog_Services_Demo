using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using Microsoft.Rest;
using System.Reflection.Metadata;

class ApiKeyServiceClientCreds: ServiceClientCredentials
{
    private readonly string? subscriptionKey;

    public ApiKeyServiceClientCreds(string subscriptionKey)
    {
        this.subscriptionKey = subscriptionKey;
    }

    public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if(request == null)
        {
            throw new ArgumentException("request");
        }

        request.Headers.Add("Ocp-Apim-Subscription-key", this.subscriptionKey);
        
        return base.ProcessHttpRequestAsync(request, cancellationToken);
    }
}

class Program
{
    private const string CogServicesSecret = "";
    private const string Endpoint = "https://ai-komponent-demos.cognitiveservices.azure.com/";
    static void Main()
    {
        Console.WriteLine("Three samples of Cognitive services");

        DetectLanguage().Wait();
        DetectSentiment().Wait();
        DetectObject().Wait();

    }

    public static async Task DetectLanguage()
    {
        var credentials = new ApiKeyServiceClientCreds(CogServicesSecret);

        var client = new TextAnalyticsClient(credentials)
        {
            Endpoint = Endpoint
        };

        var inputData =  new LanguageBatchInput(
        new List<LanguageInput>
        {
            new LanguageInput("1","j'aime le code et les sortes."),
            new LanguageInput("2","Oye, mi nombre es Arturo"),
            new LanguageInput("3","Hej! Vad heter du?."),
            new LanguageInput("4","Jina lako ni nani? ")
        });

        var results = await client.DetectLanguageBatchAsync(inputData);

        Console.WriteLine("===== Language Recognition=====");

        foreach (var document in results.Documents)
        {
            Console.WriteLine($"Document ID: {document.Id}, Language:{document.DetectedLanguages[0].Name}");

        }
        Console.WriteLine("\n");
    }

    public static async Task DetectSentiment()
    {

        var credentials = new ApiKeyServiceClientCreds(CogServicesSecret);
        var sentimentMeaning = "";

        var client = new TextAnalyticsClient(credentials)
        {
            Endpoint = Endpoint
        };

        var inputData = new MultiLanguageBatchInput(
            new List<MultiLanguageInput>
        {
            new MultiLanguageInput("1","I love it here","en"),
            new MultiLanguageInput("2","I'm Confused","en"),
            new MultiLanguageInput("3","I hate it Salami","en"),
            new MultiLanguageInput("4","She's mad at me","en")
        });

        var results =await client.SentimentBatchAsync(inputData);

        Console.WriteLine("===== Sentiment Analysis====");

        foreach (var document in results.Documents)
        {
            if(document.Score > 0.5)
            {
                sentimentMeaning = "Positive";
            }
            else
            {
                sentimentMeaning = "Negative";
            }
            
            Console.WriteLine($"Document ID: {document.Id} is {sentimentMeaning}, Sentiment Score:{document.Score}");

        }
        Console.WriteLine("\n");
    }

    public static async Task DetectObject()
    {
        var credentials = new ApiKeyServiceClientCreds(CogServicesSecret);

        var client = new ComputerVisionClient(credentials)
        {
            Endpoint = Endpoint
        };

        Console.WriteLine("====Object Detection: 1 ====");
        string imageUrl = "https://cdn.britannica.com/54/75854-050-E27E66C0/Eiffel-Tower-Paris.jpg";

        DetectResult analysis = await client.DetectObjectsAsync(imageUrl);
        ObjectInfo(analysis);

        Console.WriteLine("====Object Detection: 2 ====");
        string imageUrl2 = "https://static.independent.co.uk/s3fs-public/thumbnails/image/2016/12/15/10/crowded-street.jpg?quality=75&width=990&auto=webp&crop=982:726,smart";

        DetectResult analysis2 = await client.DetectObjectsAsync(imageUrl2);
        ObjectInfo(analysis2);
    }

    public static void ObjectInfo(DetectResult analysis)
    {
        foreach (var obj in analysis.Objects)
        {
            Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} as location {obj.Rectangle.X}, {obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
        }
        Console.WriteLine("\n");

    }

}
