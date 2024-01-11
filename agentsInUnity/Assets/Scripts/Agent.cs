using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    OpenAI_Rest api;


    private void Start()
    {
        Init();
        Move();
    }

    public void Init()
    {
        api = new OpenAI_Rest();
        api.Init();
    }
    public void perceiveThing(string _information)
    {
        
    }

    private async void Move()
    {
        ChatRequest chatRequest = new ChatRequest();
        chatRequest.Messages = new List<ChatMessage>
        {
            new ChatMessage() { Role = role.system, Message = "Move the character in the up, down, right, or left direction according to the given situation." },
            new ChatMessage() { Role = role.system, Message = "Responses can only be UP, DOWN, RIGHT, LEFT." },
            new ChatMessage() { Role = role.user, Message = "I want to go in the upward direction."},
            new ChatMessage() { Role = role.assistant, Message = "UP"},
            new ChatMessage() { Role = role.user, Message = "The destination is 5 spaces ahead. There is an obstacle 3 spaces ahead. Find a way to reach the destination without touching the obstacle."}
        };

        var data = ((await (api.ClieantResponseChat(chatRequest))).Choice);
        foreach (var choice in data)
        {
            Debug.Log(choice.Message.Message);
        }
    }
}
