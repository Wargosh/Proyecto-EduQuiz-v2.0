using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void BTN_PlayR()
    {
        SceneManager.LoadScene("game_trivia");
    }
}
