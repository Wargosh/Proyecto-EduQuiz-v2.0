using UnityEngine;
using UnityEngine.SceneManagement;
public class PanelMenuQuitGame : MonoBehaviour
{
    public GameObject contentQuitGame;

    public void BTN_QuitGame_ToMenu () {
        SceneManager.LoadScene("main_menu");
    }

    public void BTN_EnablePanelQuitGame () {
        contentQuitGame.SetActive(true);
    }

    public void BTN_DisablePanelQuitGame () {
        contentQuitGame.SetActive(false);
    }
}
