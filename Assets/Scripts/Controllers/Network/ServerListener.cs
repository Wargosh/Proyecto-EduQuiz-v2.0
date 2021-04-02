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
        socket.On("click", On_Click);
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
        for (int i = listQuestions.questions.Count - 1; i >= 0; i--)
        {
            print("Pregunta: " + listQuestions.questions[i].question);
            for (int j = 0; j < listQuestions.questions[i].options.Count; j++)
            {
                print("Opcion " + (j+1) + ": " + listQuestions.questions[i].options[j].option + " | es: " +
                    listQuestions.questions[i].options[j].status);
            }
        }
    }

    public void On_Click(SocketIOEvent e)
    {
        print("btn clicked: " + e.data);
    }

    public void BTN_click()
    {
        JSONObject dataMsg = new JSONObject(JSONObject.Type.OBJECT);
        dataMsg.AddField("id", "---");
        dataMsg.AddField("user", "Wargosh");
        dataMsg.AddField("msg", "Hi, There!");
        socket.Emit("chat", dataMsg);

        

        socket.Emit("click");
    }


    /*
    private QSocket socket;
    private void Start()
    {
        socket = IO.Socket(URL_Server);


        socket.On(QSocket.EVENT_CONNECT, () => {
            Debug.Log("Connected");

            JObject dataMsg = new JObject();
            dataMsg["id"] = "--";
            dataMsg["user"] = "Wargosh";
            dataMsg["msg"] = "Hi there!";

            socket.Emit("chat", dataMsg);

            socket.Emit("GetQuestions");
        });

        socket.On(QSocket.EVENT_RECONNECT, () => {
            Debug.Log("Reconnecting...");
        });

        socket.On("GetQuestions", data => {
            Debug.Log("Get questions" + data);

            JObject questions = JObject.Parse(data.ToString());
            Debug.Log("total preguntas obtenidas: " + questions[0]);
            for (int i = 0; i < questions.Count; i++)
            {
                Debug.Log("Pregunta: " + questions["question"][i]);
            }
        });

        socket.On("chat", data => {
            JObject obj = JObject.Parse(data.ToString());
            print("nombre usuario: " + obj["user"] + " mensaje: " + obj["msg"]);
        });

        socket.On("click", data => {
            print("response click");
        });
    }

    public void BTN_click()
    {
        socket.Emit("click");
    }

    private void OnDestroy()
    {
        socket.Disconnect();
    }*/
}
