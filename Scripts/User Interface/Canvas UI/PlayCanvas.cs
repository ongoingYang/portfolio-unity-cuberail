using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayCanvas : CanvasUI
{
    #region TextMeshProUGUI
    [Header("textMesh")] 
    [SerializeField] private TextMeshProUGUI _timerMesh;
    [SerializeField] private TextMeshProUGUI _lifeMesh;
    [SerializeField] private TextMeshProUGUI _coinMesh;
    [SerializeField] private TextMeshProUGUI _stageMesh;
    [SerializeField] private TextMeshProUGUI _countDownMesh;
    #endregion
    [Header("button & panel")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private RectTransform _upperPanel;
    [SerializeField] private RectTransform _underPanel;
    [SerializeField] private RectTransform _countDownPanel;
    [SerializeField] private RectTransform _borderReadyPanel;

    private Color countColor;
    private const int countTime = 3;
    private const float fadeHeight = 200f;
    private float fadeTime = 0.5f;
    private float delayTime;


    private void Awake(){
        countColor = _countDownMesh.color;
        _pauseButton.onClick.AddListener(() => OnClickPause());
    }
    private void OnClickPause(){
        SoundOnByNext();
        Time.timeScale = 0;
        AudioManager.Instance.PauseSound();
        CanvasManager.Instance.OpenCanvasUI(CanvasType.Pause);
        GameDesignManager.Instance.OnTimerActivate = false;
    }
    private void UpdateGameTimer(float time){
        int min = (int)time / 60;
        int sec = (int)time % 60;
        if (sec < 10)
            _timerMesh.text = string.Format("{0}:0{1}", min, sec);
        else _timerMesh.text = string.Format("{0}:{1}", min, sec);
    }
    private void UpdateGameLife(int life){
        _lifeMesh.text = string.Format("{0}", life);
    }
    private void UpdateGameGold(int gold){
        _coinMesh.text = string.Format("{0}", gold);
    }
    protected override void OnEnable(){
        base.OnEnable();
        GameDesignManager.TimeUpdate += UpdateGameTimer;
        GameDesignManager.GoldUpdate += UpdateGameGold;
        GameDesignManager.LifeUpdate += UpdateGameLife;
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (GameDesignManager.Instance != null) {
            _stageMesh.text = string.Format("{0}", GameDesignManager.Instance.StageLevel);
            if (GameDesignManager.allStagesCount == GameDesignManager.Instance.StageLevel)
                _stageMesh.text = "Last";
        }
    }
    protected override void OnDisable(){
        base.OnDisable();
        GameDesignManager.TimeUpdate -= UpdateGameTimer;
        GameDesignManager.GoldUpdate -= UpdateGameGold;
        GameDesignManager.LifeUpdate -= UpdateGameLife;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #region play Scene 인트로 효과
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        if (arg0.buildIndex == SceneLevel.Play.SceneToIndex()){
            delayTime = GameDesignManager.Instance.blockSetDelay;
            StartCoroutine(RoutineGameReady(countTime, delayTime));
            StartCoroutine(FadeInPlayPanel(fadeHeight, countTime + delayTime, fadeTime, _upperPanel));
            StartCoroutine(FadeInPlayPanel(-fadeHeight, countTime + delayTime, fadeTime, _underPanel));
            StartCoroutine(FadeInBorderPanel(countTime + delayTime, fadeTime));
        }
    }
    IEnumerator RoutineGameReady(int count, float delayTime)
    {
        Vector2 prev = new Vector2(1.0f, 1.0f);
        Vector2 next = new Vector2(0.5f, 0.5f);

        float elapsedTime = 0f;
        while (elapsedTime < delayTime){
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i <= count; i++){
            float lerpTime = 1f;
            if (i != count) _countDownMesh.text = string.Format("{0}", (count - i));
            else{
                _countDownMesh.text = "Go";
                lerpTime = 0.5f;
            }
            elapsedTime = 0f;
            while (elapsedTime <= 1){
                elapsedTime += Time.deltaTime / lerpTime;
                countColor.a = Mathf.Lerp(1, 0, elapsedTime.Interpolation(SmoothType.Exponential));
                _countDownMesh.color = countColor;
                _countDownPanel.localScale = Vector2.Lerp(prev, next, elapsedTime.Interpolation(SmoothType.Exponential));
                yield return null;
            }
        }
    }
    IEnumerator FadeInBorderPanel(float waitTime, float fadeTime){
        Vector2 prev = new Vector2(0f, 0f);
        Vector2 next = new Vector2(1f, 1f);

        _borderReadyPanel.gameObject.SetActive(false);
        float elapsedTime = 0f;
        while (elapsedTime < waitTime){
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _borderReadyPanel.gameObject.SetActive(true);
        elapsedTime = 0f;
        while (elapsedTime <= 1){
            elapsedTime += Time.deltaTime / fadeTime;
            _borderReadyPanel.localScale = Vector2.Lerp(prev, next, elapsedTime.Interpolation(SmoothType.Exponential));
            yield return null;
        }
    }
    IEnumerator FadeInPlayPanel(float height, float waitTime, float fadeTime, RectTransform panelRect){
        float from = panelRect.position.y + height;
        float to = panelRect.position.y;
        panelRect.position = new Vector3(panelRect.position.x, from, panelRect.position.z);

        float elapsedTime = 0f;
        while (elapsedTime <= waitTime){
            elapsedTime += Time.deltaTime;
            yield return null;
        } // 카운트 대기 시간 
        elapsedTime = 0f;
        while (elapsedTime <= 1f){
            elapsedTime += Time.deltaTime / fadeTime;
            panelRect.position = new Vector3(
                panelRect.position.x,
                Mathf.Lerp(from, to, elapsedTime.Interpolation(SmoothType.EaseInWithCos)),
                panelRect.position.z);
            yield return null;
        }
    }
    #endregion
}
