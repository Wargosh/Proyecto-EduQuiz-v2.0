using UnityEngine;
using System;
using SocketIO;
using System.Collections.Generic;

// Este script esta encargado de escuchar cada uno de los eventos que desencadene el servidor
// y de esta manera poder actualizar la informacion recibida en el cliente
public class ServerListener : MonoBehaviour
{
    public string URL_Server;
    public string idSocket; // id asignado por el servidor al cliente
    SocketIOComponent socket;

    public static ServerListener Instance { set; get; }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        socket = GetComponent<SocketIOComponent>();
    }

    void Start()
    {
        // Server notifications
        socket.On("connectionEstabilished", OnConnectionEstabilished);
        socket.On("disconnected", OnDisconnected);

        // chat
        socket.On("chat", OnMessageChat);

        // questions
        socket.On("questions:get", OnGetQuestions);
    }

    void OnConnectionEstabilished(SocketIOEvent e)
    {
        idSocket = e.data["id"].str;

        Debug.Log("Player is connected: " + idSocket);


        socket.Emit("questions:get");
    }

    void OnDisconnected(SocketIOEvent e)
    {
        var id = e.data["id"].str;
        Debug.Log("Player is disconnected: " + id);
    }

    void OnMessageChat(SocketIOEvent e)
    {
        print("message: " + e.data);
    }

    public class MessageList { public List<Question> questions; }
    public MessageList listQuestions = new MessageList();
    void OnGetQuestions(SocketIOEvent e)
    {
        print("questions: " + e.data);

        GetQuestions(e.data.ToString());
    }

    // Enviar datos al API Rest
    /*private void GetMessagesChatGlobal()
    {
        string url = MessengerToServer.Instance.server + "chat/global/get_messages";
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json; charset=utf-8");
        byte[] pData = System.Text.Encoding.UTF8.GetBytes("{}".ToCharArray());
        WWW www = new WWW(url, pData, headers);
        StartCoroutine(GetMessagesGlobal(www));
    }*/

    public void GetQuestions(string json)
    {
        JsonUtility.FromJsonOverwrite(json, listQuestions);
    }
}
