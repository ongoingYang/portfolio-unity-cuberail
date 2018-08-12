using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameCookInterface;

public class LevelSceneManager : MonoBehaviour {

    #region 싱글턴 정의 
    private static LevelSceneManager _instance;
    public static LevelSceneManager Instance
    { get { return _instance; } }
    private void MakeSingleton()
    {
        if (_instance != null) DestroyImmediate(gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Awake()
    {
        MakeSingleton();
        _totalSceneCount = SceneManager.sceneCountInBuildSettings;
    }
    #endregion
    [SerializeField]private int _totalSceneCount;
    private float _introDelay = 1.2f;
    public int currentSceneIndex;

    [SerializeField] private Image _fadeScreen;
    [SerializeField] private Animator _fadeAnim;
    readonly int _fadeInHash = Animator.StringToHash("FadeIn");
    readonly int _fadeOutHash = Animator.StringToHash("FadeOut");

    private void LoadSceneByIndex(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= _totalSceneCount)
        {
            Debug.LogWarning("씬 인덱스 범위 밖 접근 오류");
            return;
        }
        currentSceneIndex = levelIndex;
        SceneManager.LoadScene(levelIndex);
    }
    private void LoadSceneTheNext(){
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1);
        nextSceneIndex = Mathf.Clamp(nextSceneIndex, SceneLevel.Lobby.SceneToIndex(), _totalSceneCount - 1);// 접근 가능 씬 (Lobby 부터 마지막)
        LoadSceneByIndex(nextSceneIndex);
    }

    #region 사용 접근 메서드
    public void LoadSceneByFade(SceneLevel scene, CanvasType canvasType, BGMSoundType musicType)
    {
        StartCoroutine(RoutineLoadScene(scene.SceneToIndex(), canvasType, musicType));
    }
    public void ReloadPlayScene(BGMSoundType musicType)
    {
        StartCoroutine(RoutineReloadScene(SceneManager.GetActiveScene().buildIndex, CanvasType.Play, musicType));
    }
    public void BackToTheScene(SceneLevel scene, BGMSoundType music)
    {
        StartCoroutine(RoutineBackToTheScene(scene, music));
    }
    public void SwitchGamePower(bool power)
    {
        switch (power)
        {
            case true: // 애플리케이션 시작 시점
                StartCoroutine(RoutineIntroToLobby());
                break;
            case false:// 애플리케이션 종료 시점
                StartCoroutine(RoutineQuitApplication());
                break;
        }
    }
    #endregion

    #region 코루틴 씬 페이드 매서드
    IEnumerator RoutineLoadScene(int sceneLevel, CanvasType type, BGMSoundType musicType)
    {
        _fadeAnim.Play(_fadeOutHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 1);
        
        LoadSceneByIndex(sceneLevel);
        CanvasManager.Instance.OpenCanvasUI(type);

        _fadeAnim.Play(_fadeInHash);
        AudioManager.Instance.PlayBGMSound(musicType);
        yield return new WaitUntil(() => _fadeScreen.color.a == 0);
    }
    IEnumerator RoutineReloadScene(int sceneLevel, CanvasType type, BGMSoundType musicType)
    {
        _fadeAnim.Play(_fadeOutHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 1);
        LoadSceneByIndex(sceneLevel);
        yield return null; // 씬이 다시 로드 되는 것을 먼저 한 프레임 기다린다.
        _fadeAnim.Play(_fadeInHash);
        AudioManager.Instance.UnPauseSound();
        AudioManager.Instance.PlayBGMSound(musicType);
        yield return new WaitUntil(() => _fadeScreen.color.a == 0);
    }

    IEnumerator RoutineBackToTheScene(SceneLevel scene, BGMSoundType music)
    {
        _fadeAnim.Play(_fadeOutHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 1);

        LoadSceneByIndex(scene.SceneToIndex());
        while (CanvasManager.Instance.stackCount > scene.SceneToIndex() + 1){
            CanvasManager.Instance.CloseCanvasUI();
        }
        AudioManager.Instance.UnPauseSound();
        AudioManager.Instance.PlayBGMSound(music);

        _fadeAnim.Play(_fadeInHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 0);
    }
    IEnumerator RoutineIntroToLobby()
    {
        _fadeAnim.Play(_fadeOutHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 1);
        CanvasManager.Instance.OpenCanvasUI(CanvasType.Intro);
        AudioManager.Instance.PlayBGMSound(BGMSoundType.Intro);
        _fadeAnim.Play(_fadeInHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 0);
        LoadSceneByIndex(SceneLevel.Intro.SceneToIndex());

        yield return new WaitForSeconds(_introDelay);

        _fadeAnim.Play(_fadeOutHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 1);
        CanvasManager.Instance.OpenCanvasUI(CanvasType.Lobby);
        AudioManager.Instance.PlayBGMSound(BGMSoundType.Lobby);
        _fadeAnim.Play(_fadeInHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 0);
        LoadSceneByIndex(SceneLevel.Lobby.SceneToIndex());
    }
    IEnumerator RoutineQuitApplication()
    {
        _fadeAnim.Play(_fadeOutHash);
        yield return new WaitUntil(() => _fadeScreen.color.a == 1);
        LoginCanvas.AuthLogOut();
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
    #endregion

}
