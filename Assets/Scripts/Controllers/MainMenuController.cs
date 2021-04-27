using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenuController : MonoBehaviour
{
    public Animator animContentOptions;

    [Header("Text Total Coins")]
    public TextMeshProUGUI[] txtTotalCoins;

    private bool isHide = true;

    private void Start() {
        int value = CoinsManager.Instance.GetCurrentCoins();
        UpdateTextsCoins(value);
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
}
