using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
using TMPro;

public abstract class ClearedCanvas : CanvasUI {

    [SerializeField] protected Button _backButton;
    [SerializeField] protected Button _nextButton;
    [SerializeField] protected Transform _resultPanel;

    #region TextMeshProUGUI
    [Header("textMesh")]
    [SerializeField] protected TextMeshProUGUI _scoreMesh;
    [SerializeField] protected TextMeshProUGUI _coinMesh;
    [SerializeField] protected TextMeshProUGUI _timeMesh;
    [SerializeField] protected TextMeshProUGUI _userMesh;
    #endregion

    private float lerpTime = 2.0f;

    protected override void OnClickBack(){

        UpdateUserScore();
        SoundOnByBack();
        LevelSceneManager.Instance.BackToTheScene(SceneLevel.Lobby, BGMSoundType.Lobby);
    }
    protected override void OnClickNext(){
        SoundOnByNext();
        LevelSceneManager.Instance.BackToTheScene(SceneLevel.Play, BGMSoundType.Play);
    }
    protected override void OnEnable(){
        base.OnEnable();
        if (LevelSceneManager.Instance.currentSceneIndex == SceneLevel.Score.SceneToIndex()){
         
            DisplayResult(GameDesignManager.stageResult);
        } 
    }
    protected override void OnDisable(){
        base.OnDisable();
    }

    protected virtual void DisplayResult(StageResult result){
    }
    protected void OpenResultPanel(int index)
    {
        for (int i = 0; i < _resultPanel.childCount; i++)
        {
            _resultPanel.GetChild(i).gameObject.SetActive(false);
        }
        _resultPanel.GetChild(index).gameObject.SetActive(true);
    }
    protected virtual  void UpdateScoreBoard(TextMeshProUGUI user, TextMeshProUGUI score, TextMeshProUGUI coin, TextMeshProUGUI time, TextMeshProUGUI life)
    {
        
        life.text = string.Format("{0}", GameDesignManager.Instance.GameLife);
        StartCoroutine(RoutineDisplayScore(GameDesignManager.Instance.GameGold, 0, coin));
        StartCoroutine(RoutineDisplayScore((long)GameDesignManager.Instance.GameTimer, 0, time));
        long ScoreResult =
        GameDesignManager.GameScore.CalculateScore(GameDesignManager.Instance.GameTimer,
            GameDesignManager.Instance.GameGold,
            GameDesignManager.Instance.StageLevel,
            GameDesignManager.stageResult);
        StartCoroutine(RoutineDisplayScore(GameDesignManager.GameScore, ScoreResult, score));
        GameDesignManager.GameScore = ScoreResult;
        if (GameDesignManager.userName != null){
            user.text = string.Format("{0}", GameDesignManager.userName.Split('@')[0]);
        }
    }

    protected void UpdateUserScore()
    {
      
        if (GameDesignManager.userName != null)
        {
            DatabaseManager.Instance.UpdateGameScoreData(GameDesignManager.Instance.gameType.TypeToID(), GameDesignManager.GameScore);
        }
    }

    protected void UpdateScoreBoard(TextMeshProUGUI user, TextMeshProUGUI score, TextMeshProUGUI coin, TextMeshProUGUI time)
    {
        StartCoroutine(RoutineDisplayScore(GameDesignManager.Instance.GameGold, 0, coin));
        StartCoroutine(RoutineDisplayScore((long)GameDesignManager.Instance.GameTimer, 0, time));
        long ScoreResult = GameDesignManager.Instance.GameLife * 700 +
        GameDesignManager.GameScore.CalculateScore(GameDesignManager.Instance.GameTimer,
            GameDesignManager.Instance.GameGold,
            GameDesignManager.Instance.StageLevel,
            GameDesignManager.stageResult);
        StartCoroutine(RoutineDisplayScore(GameDesignManager.GameScore, ScoreResult, score));

        GameDesignManager.GameScore = ScoreResult; 
        if (GameDesignManager.userName != null){
            user.text = string.Format("{0}", GameDesignManager.userName.Split('@')[0]);
        }
    }
    protected IEnumerator RoutineDisplayScore(long prev, long next, TextMeshProUGUI value)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= 1)
        {
            elapsedTime += Time.deltaTime / lerpTime;
            long progress = (long)Mathf.Lerp(prev, next, elapsedTime.Interpolation(SmoothType.SoSmootherStep));
            value.text = string.Format("{0}", progress);
            yield return null;
        }
    }
    protected IEnumerator DisplayPlusLife(GameObject note, TextMeshProUGUI life, SFXSoundType sfxSound, Identifier identifier)
    {
        GameDesignManager.Instance.AddLife(identifier);
        note.SetActive(true);
        float elapsedTime = 0f;
        bool isDisplayed = false;
        while (elapsedTime <= 2.5f)
        {
            if (elapsedTime >= 0.6 && isDisplayed == false)
            {
                isDisplayed = true;
                life.text = string.Format("{0}", GameDesignManager.Instance.GameLife);
                AudioManager.Instance.PlaySFXSound(sfxSound);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        note.SetActive(false);
    }
}


