using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Animator animContentOptions;
    private bool isHide = true;
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
