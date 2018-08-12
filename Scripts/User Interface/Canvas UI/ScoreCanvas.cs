using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
using TMPro;
public class ScoreCanvas : CanvasUI
{

    [SerializeField] GameObject displayEasyRanking;
    [SerializeField] GameObject displayHardRanking;

    [Header("textMesh")]
    [SerializeField] TextMeshProUGUI _userEasyScore;
    [SerializeField] TextMeshProUGUI _userHardScore;
    [SerializeField] TextMeshProUGUI _topEasyRanking;
    [SerializeField] TextMeshProUGUI _topHardRanking;

    [SerializeField] private Button _easyButton;
    [SerializeField] private Button _hardButton;
    [SerializeField] private Button _backButton;

    private Color activeColor = new Color(0.796f, 0.509f, 0.886f);
    private Color deActiveColor = new Color(0.796f, 0.758f, 0.886f);

    private void Start(){
        _backButton.onClick.AddListener(() => OnClickBack());
        _easyButton.onClick.AddListener(() => SoundOnBySwitch());
        _easyButton.onClick.AddListener(() => OnClickDisplayEasy());
        _hardButton.onClick.AddListener(() => SoundOnBySwitch());
        _hardButton.onClick.AddListener(() => OnClockDisplayHard());
    }

    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (GameDesignManager.Instance != null)
        {
            switch (GameDesignManager.Instance.gameType.TypeToID()){
                case GameMode.Easy:
                    OnClickDisplayEasy();
                    break;
                case GameMode.Hard:
                    OnClockDisplayHard();
                    break;
            }
        }
    }

    public void SwitchTab()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUISound(UISoundType.Switch);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void OnClickDisplayEasy()
    {
        _easyButton.image.color = activeColor;
        _hardButton.image.color = deActiveColor;
        if (DatabaseManager.Instance != null)
        {
            displayEasyRanking.SetActive(true);
            displayHardRanking.SetActive(false);
            DisplayEasyScore();
        }
    }
    private void OnClockDisplayHard()
    {
        _hardButton.image.color = activeColor;
        _easyButton.image.color = deActiveColor;
        if(DatabaseManager.Instance != null)
        {
            displayEasyRanking.SetActive(false);
            displayHardRanking.SetActive(true);
            DisplayHardScore();
        }
    }
    private void DisplayEasyScore()
    {
      
        if (GameDesignManager.userName != null)
            _userEasyScore.text = string.Format("{0}", DatabaseManager.Instance.UserEasyScore);
        else _userEasyScore.text = "anonymous";

        int rank =0;
        _topEasyRanking.text = "";

        foreach (string item in DatabaseManager.Instance.leaderBoardEasy){

            if (rank < 9) _topEasyRanking.text += string.Format("0{0}. {1}\n", ++rank, item);
            else _topEasyRanking.text += string.Format("{0}. {1}\n", ++rank, item);

        }
        
    }
    private void DisplayHardScore()
    {
        if (GameDesignManager.userName != null)
            _userHardScore.text = string.Format("{0}", DatabaseManager.Instance.UserHardScore);
        else _userHardScore.text = "anonymous";

        _userHardScore.text = string.Format("{0}", DatabaseManager.Instance.UserHardScore);
        int rank = 0;
        _topHardRanking.text = "";

        foreach (string item in DatabaseManager.Instance.leaderBoardHard){
            if (rank < 9) _topEasyRanking.text += string.Format("0{0}. {1}\n", ++rank, item);
            else _topEasyRanking.text += string.Format("{0}. {1}\n", ++rank, item);
        }
    }
}
