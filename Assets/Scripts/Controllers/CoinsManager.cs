using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{

    private int _coins;
    public static CoinsManager Instance { set; get; }

    private void Awake() {
        Instance = this;

        Initialize();
    }

    private void Initialize() {
        _coins = 0;


        Debug.Log("holi");
    }

    public int GetCurrentCoins () {
        Debug.Log("update the coins text");
        return _coins;
    }
}
