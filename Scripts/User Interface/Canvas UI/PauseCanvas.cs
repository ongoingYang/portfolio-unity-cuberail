using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
using TMPro;
public class PauseCanvas : CanvasUI
{
    [SerializeField] private Button _lobbyButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _backButton;
    
    [SerializeField] private TextMeshProUGUI _lifeMesh;
    [SerializeField] private TextMeshProUGUI _scoreDisplay;
    [SerializeField] private TextMeshProUGUI _InformMesh_HighStage;
    [SerializeField] private TextMeshProUGUI _InformMesh_OutOfLife;

    private void Start()
    {
        _lobbyButton.onClick.AddListener(() => OnClickLobby());
        _restartButton.onClick.AddListener(() => OnClickRestart());
        _backButton.onClick.AddListener(() => OnClickBack());
    
    }

    private void OnClickLobby() // sound pause 처리 
    {
        Time.timeScale = 1;
        SoundOnByNext();
        if (LevelSceneManager.Instance != null)
            LevelSceneManager.Instance.BackToTheScene(SceneLevel.Lobby, BGMSoundType.Lobby);
        if (GameDesignManager.userName != null)
        {
            DatabaseManager.Instance.UpdateGameScoreData(GameDesignManager.Instance.gameType.TypeToID(), GameDesignManager.GameScore);
        }
    }
    private void OnClickRestart() // sound pause 처리 
    {
        Time.timeScale = 1;
       
        SoundOnByNext();

        if (GameDesignManager.Instance.GameLife > 1){
            GameDesignManager.Instance.AddLife(Identifier.Negative);
        }
        else
        {
            GameDesignManager.Instance.StageLevel = 1;
        }

        if (CanvasManager.Instance != null && LevelSceneManager.Instance != null) 
        {
            CanvasManager.Instance.CloseCanvasUI();
            LevelSceneManager.Instance.ReloadPlayScene(AudioManager.Instance.playOnMusic);
        }else
        {
            Debug.LogWarning("CanvasManager 또는 LevelSceneManager 참조 실패");
        }

    }
    protected override void OnClickBack()
    {
        Time.timeScale = 1;
        base.OnClickBack();
        AudioManager.Instance.UnPauseSound();
        GameDesignManager.Instance.OnTimerActivate = true;

    }
    protected override void OnEnable()
    {
        base.OnEnable();

        if (GameDesignManager.Instance == null) return;

        _scoreDisplay.text = string.Format("Score : {0}", GameDesignManager.GameScore);
        _lifeMesh.text = string.Format("{0}", GameDesignManager.Instance.GameLife);

        if (GameDesignManager.Instance.StageLevel != 1)
        {
            if (GameDesignManager.Instance.GameLife == 1)
            {
                AudioManager.Instance.PlayUISound(UISoundType.ButtonError);
                _InformMesh_HighStage.gameObject.SetActive(false);
                _InformMesh_OutOfLife.gameObject.SetActive(true);
            }
            else
            {
                _InformMesh_HighStage.gameObject.SetActive(true);
                _InformMesh_OutOfLife.gameObject.SetActive(false);
            }
        }
        else {
            _InformMesh_OutOfLife.gameObject.SetActive(false);
            _InformMesh_HighStage.gameObject.SetActive(false);
        } 
        

    }
}
