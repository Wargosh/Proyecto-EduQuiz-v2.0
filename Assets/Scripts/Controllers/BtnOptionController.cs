using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BtnOptionController : MonoBehaviour
{
    public TextMeshProUGUI txtOption;

    [Header("Animaciones")]
    public Animator animQuestion;
    private Animator anim;

    [SerializeField]
    private string idBtn;
    [SerializeField]
    private string text = "";

    public bool isCorrect = false;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    public void SetAnswerBtnOption(string option, bool isCorrect)
    {
        txtOption.text = text = option;
        this.isCorrect = isCorrect;
    }

    public void BTN_OptionClicked ()
    {
        if (GameManager.Instance.OptionIsClicked()) {
            if (!isCorrect){
                GameManager.Instance.IncorrectQuestion();
                StartCoroutine("TimeAnimError");
            } else {
                GameManager.Instance.CorrectQuestion();
                StartCoroutine("TimeAnimCorrect");
            }
        }
    }

    IEnumerator TimeAnimError(){
        animQuestion.SetBool("isError", true);
        anim.SetBool("isIncorrect", true);

        yield return new WaitForSeconds(0.2f);
        animQuestion.SetBool("isError", false);
        yield return new WaitForSeconds(3.8f);
        
        GameManager.Instance.BTN_Continue();
    }

    IEnumerator TimeAnimCorrect(){
        anim.SetBool("isCorrect", true);
        yield return new WaitForSeconds(4f);

        GameManager.Instance.BTN_Continue();
    }
}
