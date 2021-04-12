using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Imagenes")]
    public Image imgClock;
    public Image imgQuestion;

    [Header("Textos")]
    public TextMeshProUGUI txtClock;
    public TextMeshProUGUI txtTittleQuestion;
    public TextMeshProUGUI txtTittleMainQuestion;

    // Control de los botones de opcion
    [Header("Control de las opciones")]
    public BtnOptionController[] btnOptions;
    private int[] orderOptions = new int[4] { 0, 1, 2, 3 };

    [Header("Animaciones")]
    public Animator animQuestion;

    // nuevo juego
    bool activateClock = false;

    // variables pregunta
    int indexQuestion = 0;

    // reloj
    const float segMax = 20;
    float segs = 20;

    private void Awake()
    {

    }
    void Start()
    {
        
    }

    void Update()
    {
        if (!activateClock)
            return;

        CountDownClock();
    }

    public void BTN_NewGame()
    {
        NewGame();
    }

    public void NewGame()
    {
        indexQuestion = Random.Range(0, ServerListener.Instance.listQuestions.questions.Count);
        segs = segMax;
        imgClock.fillAmount = 1;

        ShowQuestion();
    }

    public void ShowQuestion()
    {
        if (ServerListener.Instance.listQuestions.questions[indexQuestion].images.Length == 0)
        {
            // mostrar solo pregunta
            txtTittleQuestion.gameObject.SetActive(false);
            txtTittleMainQuestion.gameObject.SetActive(true);
            txtTittleMainQuestion.text = ServerListener.Instance.listQuestions.questions[indexQuestion].question;
            imgQuestion.gameObject.SetActive(false);
        }
        else
        {
            // mostrar pregunta con imagen
            txtTittleQuestion.gameObject.SetActive(true);
            txtTittleMainQuestion.gameObject.SetActive(false);
            txtTittleQuestion.text = ServerListener.Instance.listQuestions.questions[indexQuestion].question;
            imgQuestion.gameObject.SetActive(true);
            //imgQuestion.sprite = Resources.Load<Sprite>("Images/EnDesarrollo"/* + questions[indexQuestion].img*/); // Cargar imagen
            StartCoroutine(LoadImage("http://localhost:3000/upload/questions/" + ServerListener.Instance.listQuestions.questions[indexQuestion].images[0]));
        }

        // posiciones de las opciones en aleatorio
        RandomOptions();

        // llenar texto de las opciones
        for (int i = 0; i < btnOptions.Length; i++)
        {
            btnOptions[i].SetAnswerBtnOption(
                ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].option,
                ServerListener.Instance.listQuestions.questions[indexQuestion].options[orderOptions[i]].status);
        }

        // activar cuenta atras
        activateClock = true;
    }

    IEnumerator LoadImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        imgQuestion.sprite = Sprite.Create(www.texture, new Rect(0, 0,
            www.texture.width, www.texture.height), new Vector2(0, 0));
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

    private void RandomOptions ()
    {
        int steps = 5;
        int aux;
        for (int i = 0; i < steps; i++)
        {
            int index1 = Random.Range(0, orderOptions.Length);
            int index2 = Random.Range(0, orderOptions.Length);
            if (index1 == index2)
            {
                float prob = Random.Range(0f, 1f);
                if (prob <= 0.5f)
                {
                    if (index2 == (orderOptions.Length - 1))
                        index2 = 0;
                    else
                        index2++;
                }
                else
                {
                    if (index2 == 0)
                        index2 = orderOptions.Length - 1; // 3
                    else
                        index2--;
                }
            }
            aux = orderOptions[index1];
            orderOptions[index1] = orderOptions[index2];
            orderOptions[index2] = aux;
        }
    }
}
