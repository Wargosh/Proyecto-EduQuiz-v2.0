using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BtnOptionController : MonoBehaviour
{
    public TextMeshProUGUI txtOption;

    [SerializeField]
    private string idBtn;
    [SerializeField]
    private string text = "";

    private bool isCorrect = false;

    public void SetAnswerBtnOption(string option, bool isCorrect)
    {
        txtOption.text = text = option;
        this.isCorrect = isCorrect;
    }

    public void BTN_OptionClicked ()
    {
        print("opcion es: " + isCorrect);
        ServerListener.Instance.BTN_click();
    }
}
