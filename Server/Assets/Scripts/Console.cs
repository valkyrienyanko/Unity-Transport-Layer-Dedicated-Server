﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public const int MESSAGE_HEIGHT = 20;
    public const float PADDING = 50.5f;

    public GameObject GoServer;
    public GameObject GoContent;
    public GameObject GoTextPrefab;
    public GameObject GoInput;

    private Server Server;
    private ScrollRect UIScrollRect;
    private InputField UIInputField;
    private RectTransform ContentRectTransform;
    private List<GameObject> ConsoleMessages = new List<GameObject>();

    void Awake() 
    {
        Server = GoServer.GetComponent<Server>();
        UIInputField = GoInput.GetComponent<InputField>();
        ContentRectTransform = GoContent.GetComponent<RectTransform>();
    }

    void Start() 
    {
        FocusInput();
    }

    public void Log(string message) 
    {
        Log(message, new Color(0.6f, 0.6f, 0.6f, 1.0f));
    }

    public void Log(string message, Color color) 
    {
        GameObject goText = Instantiate(GoTextPrefab, GoContent.transform);
        RectTransform goTextRect = goText.GetComponent<RectTransform>();
        Text text = goText.GetComponent<Text>();

        text.text = message;
        text.color = color;
        
        int lines = CalcLines(text.preferredWidth, goTextRect.rect.width);

        // Offset text
        Debug.Log(ConsoleMessages.Count);
        Debug.Log(lines);
        goText.transform.Translate(new Vector3(0, -20 * (ConsoleMessages.Count + lines), 0));
        // Resize text
        goTextRect.sizeDelta = new Vector2(goTextRect.sizeDelta.x, 20 * lines);

        // Offset content box
        ContentRectTransform.Translate(new Vector3(0, -20 / 2 * (ConsoleMessages.Count + lines), 0));
        // Resize content box
        ContentRectTransform.sizeDelta = new Vector2(0, (20 * (ConsoleMessages.Count + lines)));
        
        ConsoleMessages.Add(goText); // Add message to list

        ResetInput();
        FocusInput();
    }

    int CalcLines(float contentWidth, float lineWidth) 
    {
        int lines = 1;
        while (contentWidth > lineWidth) 
        {
            contentWidth -= lineWidth;
            lines++;
        }
        return lines;
    }

    public void HandleInput() 
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;
        
        string input = UIInputField.text;
        string[] args = input.ToLower().Split();

        switch (args[0])
        {
            case "help":
            Log("Commands: broadcast, list, kick, status, start, stop, restart, exit");
            break;
            case "broadcast":
            // Broadcast message to game server. Alias: ['say']
            break;
            case "list":
            // List all players in the server. Alias: ['players']
            break;
            case "kick":
            if (args.Length <= 1) 
            {
                Log("Error: Command kick requires <user> to kick", new Color(1f, 0.75f, 0.75f, 1f));
                return;
            }

            Log("Kicked " + args[0]);
            break;
            case "status":
            if (Server.IsRunning()) 
            {
                Log("Server is online.");
            } else 
            {
                Log("Server is offline.");
            }
            break;
            case "start":
            if (!Server.IsRunning()) 
            {
                Server.StartServer();
            } 
            else 
            {
                Log("Server is already running.", new Color(1f, 0.75f, 0.75f, 1f));
            }
            break;
            case "stop":
            if (Server.IsRunning()) 
            {
                Server.StopServer();
            } 
            else 
            {
                Log("Server is not running.");
            }
            break;
            case "restart":
            if (!Server.IsRunning()) 
            {
                Log("Server needs to be running to restart.");
            } else 
            {
                Server.StopServer();
                Server.StartServer();
            }
            break;
            case "exit":
            Application.Quit();
            break;
            default:
            if (!args[0].Equals("")) 
            {
                Log("Error: Unknown command \"" + args[0] + "\"", new Color(1f, 0.75f, 0.75f, 1f));
            }
            break;
        }
    }

    void ResetInput() 
    {
        UIInputField.text = "";
        ContentRectTransform.anchoredPosition = new Vector2(10, -Screen.height + PADDING);
    }

    void FocusInput() 
    {
        UIInputField.Select();
        UIInputField.ActivateInputField();
    }

    void Update() 
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) 
        {
            FocusInput();
        }
    }
}