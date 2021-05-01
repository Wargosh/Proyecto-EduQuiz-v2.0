using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Animator animContentOptions;

    [Header("Info Player")]
    public TextMeshProUGUI txtUsername;
    public TextMeshProUGUI txtLevel;
    public Image[] imgPlayer;

    [Header("Text Total Coins")]
    public TextMeshProUGUI[] txtTotalCoins;

    private bool isHide = true;

    private void Start() {
        int value = CoinsManager.Instance.GetCurrentCoins();
        UpdateTextsCoins(value);

        LoadInfoPlayer();
    }

    private void LoadInfoPlayer () {
        txtUsername.text = ServerListener.Instance._player.username;
        txtLevel.text = ServerListener.Instance._player._level.ToString();
        if (ServerListener.Instance._player.image_google != "")
            StartCoroutine(LoadImage(ServerListener.Instance._player.image_google));
    }

    private void UpdateTextsCoins (int value) {
        for (int i = 0; i < txtTotalCoins.Length; i++) {
            txtTotalCoins[i].text = value.ToString();
        }
    }

    public void BTN_PlayR () {
        SceneManager.LoadScene("game_trivia");
    }

    public void BTN_ShowHideContentOptions () {
        if (isHide) {
            isHide = false;
            BTN_ShowContentOptions();
        } else {
            isHide = true;
            BTN_HideContentOptions();
        }
    }

    public void BTN_ShowContentOptions () {
        animContentOptions.SetBool("showPanel", true);
    }
    
    public void BTN_HideContentOptions () {
        animContentOptions.SetBool("showPanel", false);
    }

    IEnumerator LoadImage(string url) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite mySprite = Sprite.Create(myTexture, new Rect(0, 0,
                        myTexture.width, myTexture.height), new Vector2(0, 0));
            for (int i = 0; i < imgPlayer.Length; i++) {
                imgPlayer[i].sprite = mySprite;
            }
        }
    }
}
