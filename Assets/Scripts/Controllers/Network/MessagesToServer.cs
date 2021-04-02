using UnityEngine;

public class MessagesToServer : MonoBehaviour
{
    public static MessagesToServer Instance { get; set; }
    //private QSocket socket;
    void Awake()
    {
        Instance = this;
        //socket = ServerListener.Instance.socket;
    }

    void Start()
    {
        
    }
}
