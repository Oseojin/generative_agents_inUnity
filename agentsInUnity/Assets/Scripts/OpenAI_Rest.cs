using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class OpenAI_Rest
{
    public string OpenAI_Key = "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";

    private static HttpClient client;
    private static string JsonFilelocation = Application.streamingAssetsPath + "/Json/JsonData.json";

    public delegate void StringEvent(string _string);
    public StringEvent CompletedRepostEvent;

    private string API_Url = "";

    private const string AuthorizationHeader = "Bearer";
    private const string UserAgentHeader = "User-Agent";

    public string speechFile = Application.streamingAssetsPath + "/TestSound.mp3";
    public void Init()
    {
        CreateHttpClient();
    }
    private void CreateHttpClient()
    {
        client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(AuthorizationHeader, OpenAI_Key);
        client.DefaultRequestHeaders.Add(UserAgentHeader, "okgodoit/dotnet_openai_api");
    }

    private async Task<string> ClieantResponse<SendData>(SendData request)
    {
        if (client == null)
        {
            CreateHttpClient();
        }

        API_Url = ((URL)request).Get_API_Url();

        string jsonContent = JsonConvert.SerializeObject(request, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        var stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");

        Debug.Log(API_Url);
        Debug.Log(stringContent);
        using (var response = await client.PostAsync(API_Url, stringContent))
        {

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException("Error calling OpenAi API to get completion.  HTTP status code: " + response.StatusCode.ToString() + ". Request body: " + jsonContent);
            }
        }
    }

    public async Task<ChatResponse> ClieantResponseChat(ChatRequest r)
    {
        return JsonConvert.DeserializeObject<ChatResponse>(await ClieantResponse(r));
    }

    public async Task<ChatResponse> ClieantResponseChatAnalyze(ChatMessageAnalyze r)
    {
        return JsonConvert.DeserializeObject<ChatResponse>(await ClieantResponse(r));
    }


}

interface URL
{
    public string Get_API_Url();
}

public class ChatRequest : URL
{
    public const string API_Url = "https://api.openai.com/v1/chat/completions";

    [JsonProperty("model")]
    public string Model { get; set; } = "gpt-3.5-turbo";
    [JsonProperty("messages")]
    public List<ChatMessage> Messages { get; set; }
    public string Get_API_Url()
    {
        return API_Url;
    }
}


public enum role
{
    [EnumMember(Value = "system")]
    system,
    [EnumMember(Value = "user")]
    user,
    [EnumMember(Value = "assistant")]
    assistant
}

public class ChatMessage
{
    [JsonProperty("role"), JsonConverter(typeof(StringEnumConverter)), XmlAttribute("role")]
    public role Role;
    [JsonProperty("content"), XmlAttribute("content")]
    public string Message = "";
}

public class ChatChoice
{
    [JsonProperty("index")]
    public int Index { get; set; }
    [JsonProperty("message")]
    public ChatMessage Message { get; set; }
    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
}

public class ChatUsage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }
    [JsonProperty("completionTokens")]
    public int CompletionTokens { get; set; }
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}

public class ChatResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("object")]
    public string Object { get; set; }
    [JsonProperty("created")]
    public int Created { get; set; }
    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("system_fingerprint")]
    public string SystemFingerprint { get; set; }
    [JsonProperty("choices")]
    public List<ChatChoice> Choice { get; set; }
    [JsonProperty("usage")]
    public ChatUsage Usage { get; set; }
}

public class ChatMessageAnalyze
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public ContentAnalyze Content { get; set; }
}

public class ContentAnalyze
{

    [JsonProperty("text")]
    public string text { get; set; }

    [JsonProperty("image_url")]
    public string image_url { get; set; }
}