using UnityEngine;
using SocketIO;
public class MessagesToServer : MonoBehaviour
{
    SocketIOComponent socket;
    public static MessagesToServer Instance { get; set; }
    void Awake () {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        socket = GetComponent<SocketIOComponent>();
    }

    public void GetRandomQuestions () {
        socket.Emit("questions:get");
    }
}
