using UnityEngine;
using SocketIO;
using System.Collections.Generic;

// Este script esta encargado de escuchar cada uno de los eventos que desencadene el servidor
// y de esta manera poder actualizar la informacion recibida en el cliente
public class ServerListener : MonoBehaviour
{
    public string URL_Server;
    public string idSocket; // id asignado por el servidor al cliente

    public class MessageList { public List<Question> questions; }
    public MessageList listQuestions = new MessageList();

    SocketIOComponent socket;
    public static ServerListener Instance { set; get; }
    void Awake () {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        socket = GetComponent<SocketIOComponent>();
    }

    void Start () {
        // Server notifications
        socket.On("connectionEstabilished", OnConnectionEstabilished);
        socket.On("disconnected", OnDisconnected);

        // chat
        socket.On("chat", OnMessageChat);

        // questions
        socket.On("questions:get", OnGetQuestions);
    }

    void OnConnectionEstabilished(SocketIOEvent e) {
        idSocket = e.data["id"].str;
        MenuSingInController.Instance.onConnectionEstabilished = true;
        Debug.Log("Player is connected: " + idSocket);
    }

    void OnDisconnected(SocketIOEvent e) {
        var id = e.data["id"].str;
        Debug.Log("Player is disconnected: " + id);
    }

    void OnMessageChat(SocketIOEvent e) {
        print("message: " + e.data);
    }

    
    void OnGetQuestions(SocketIOEvent e) {
        print("questions: " + e.data);

        GetQuestions(e.data.ToString());
    }


    public void GetQuestions(string json) {
        // almacenar preguntas en una lista
        JsonUtility.FromJsonOverwrite(json, listQuestions);

        // empezar nuevo juego 
        GameManager.Instance.NewGame();
    }
}
