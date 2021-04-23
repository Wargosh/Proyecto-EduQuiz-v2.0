using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelResumeGameController : MonoBehaviour
{
    [Header("Textos")]
    public TextMeshProUGUI txtQuestCorrects; // numero de preguntas acertadas
    public TextMeshProUGUI txtMaxHits; // maximo numero de aciertos en sequencia (racha)
    public TextMeshProUGUI txtLevel; // nivel actual del jugador
    public TextMeshProUGUI[] txtButtons; // solo controla el alphacolor cuando de activan/desactivan

    [Header("Sliders")]
    public Slider sldExperience;

    [Header("Paneles")]
    public CanvasGroup canvasGroupFooterButtons;

    private int _level, _hits, _maxHitStreak, _coins, _XP;

    public static PanelResumeGameController Instance { set; get; }
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        sldExperience.minValue = 0;
        sldExperience.maxValue = 45; // base=15; aciertos se multiplican x3
        sldExperience.value = 0;
    }

    public void ShowAllPoints (int level, int hits, int maxHitStreak) {
        _level = level;
        _hits = hits;
        _maxHitStreak = maxHitStreak;
        // experiencia = (aciertos * 3) + (racha * 2)
        _XP = (hits * 3) + (maxHitStreak * 2);

        txtLevel.text = "Nivel " + level;
        txtQuestCorrects.text = txtMaxHits.text = "0";
        StartCoroutine("TimeShowCounterHits");
    }

    IEnumerator TimeShowCounterHits () { // numero de aciertos
        for (int i = 0; i <= _hits; i++) {
            yield return new WaitForSeconds(.2f);
            txtQuestCorrects.text = i + " / 7";
        }
        for (int i = 0; i <= _maxHitStreak; i++) { // racha maxima
            yield return new WaitForSeconds(.2f);
            sldExperience.value++;
            txtMaxHits.text = i.ToString();
        }

        StartCoroutine("TimeShowCounterPointsXP");
    }

    IEnumerator TimeShowCounterPointsXP () {
        for (int i = 0; i < _XP; i++){
            yield return new WaitForSeconds(.05f);
            if (sldExperience.value < sldExperience.maxValue) {
                sldExperience.value++;
            } else { // nuevo nivel
                sldExperience.value = 0;
                _level++;
                txtLevel.text = "Nivel " + _level;
            }
        }

        EnableResumePanelButtons();
    }

    public void EnableResumePanelButtons () {
        canvasGroupFooterButtons.blocksRaycasts = canvasGroupFooterButtons.interactable = true;

        for (int i = 0; i < txtButtons.Length; i++){
            txtButtons[i].alpha = 1f;
        }
    }

    public void DisableResumePanelButtons () {
        canvasGroupFooterButtons.blocksRaycasts = canvasGroupFooterButtons.interactable = false;

        for (int i = 0; i < txtButtons.Length; i++){
            txtButtons[i].alpha = .5f;
        }
    }
}
