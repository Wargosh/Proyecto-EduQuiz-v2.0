using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Image imgClock;
    public Image imgQuestion;

    public TextMeshProUGUI txtClock;
    public TextMeshProUGUI txtTittleQuestion;
    public TextMeshProUGUI txtTittleMainQuestion;

    // Control de los botones de opcion
    public BtnOptionController[] btnOptions;

    // nuevo juego
    bool activateClock = false;
    [SerializeField]
    private List<Question> questions = new List<Question>();

    // reloj
    const float segMax = 20;
    float segs = 20;

    private void Awake()
    {
        Question qq = new Question();
        qq.question = "Cuanto es 2 + 5 - 3?";
        Option op1 = new Option();
        op1.text = "1";
        op1.isCorrect = false;
        Option op2 = new Option();
        op2.text = "2";
        op2.isCorrect = false;
        Option op3 = new Option();
        op3.text = "6";
        op3.isCorrect = false;
        Option op4 = new Option();
        op4.text = "4";
        op4.isCorrect = true;

        qq.options.Add(op1);
        qq.options.Add(op2);
        qq.options.Add(op3);
        qq.options.Add(op4);

        questions.Add(qq);
    }
    void Start()
    {
        NewGame();
    }

    void Update()
    {
        if (!activateClock)
            return;

        CountDownClock();
    }

    public void NewGame()
    {
        int indexQuestion = 0;
        segs = segMax;
        
        if (questions[indexQuestion].img == "")
        {
            // mostrar solo pregunta
            txtTittleQuestion.gameObject.SetActive(false);
            txtTittleMainQuestion.gameObject.SetActive(true);
            txtTittleMainQuestion.text = questions[indexQuestion].question;
            imgQuestion.gameObject.SetActive(false);
        } else
        {
            // mostrar pregunta con imagen
            txtTittleQuestion.gameObject.SetActive(true);
            txtTittleMainQuestion.gameObject.SetActive(false);
            txtTittleQuestion.text = questions[indexQuestion].question;
            imgQuestion.gameObject.SetActive(true);
            imgQuestion.sprite = Resources.Load<Sprite>("Images/EnDesarrollo"/* + questions[indexQuestion].img*/); // Cargar imagen
        }

        // llenar texto de las opciones
        for (int i = 0; i < btnOptions.Length; i++)
        {
            btnOptions[i].SetAnswerBtnOption(questions[indexQuestion].options[i].text,
                questions[indexQuestion].options[i].isCorrect);
        }

        // activar cuenta atras
        activateClock = true;
    }

    public void ResetGame()
    {
        NewGame();
    }

    /// <summary>
    /// Mantiene la cuenta atras del reloj por pregunta con el tiempo maximo establecido
    /// </summary>
    private void CountDownClock ()
    {
        if (segs > 0)
        {
            segs -= Time.deltaTime;
            imgClock.fillAmount -= Time.deltaTime / segMax;
            txtClock.text = "" + segs.ToString("00");
        }
    }
}
