using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
using TMPro;
public class SuccessCanvas : ClearedCanvas
{
    [SerializeField] private Image _note_Positive;

    [SerializeField] private TextMeshProUGUI _stageMesh;
    [SerializeField] private TextMeshProUGUI _lifeMesh;

    private void Start()
    {
        _backButton.onClick.AddListener(() => OnClickBack());
        _nextButton.onClick.AddListener(() => OnClickNext());

    }
    protected override void DisplayResult(StageResult result)
    {
        if (_userMesh != null && _scoreMesh != null && _coinMesh != null && _timeMesh != null && _lifeMesh != null)
        {
            UpdateScoreBoard(_userMesh, _scoreMesh, _coinMesh, _timeMesh, _lifeMesh);
        }
        switch (result)
        {
            case StageResult.Normal:
                OpenResultPanel(0);
                break;
            case StageResult.Good:
                OpenResultPanel(1);
                break;
            case StageResult.Great:
                OpenResultPanel(2);
                if (_note_Positive != null){
                    StartCoroutine(DisplayPlusLife(_note_Positive.gameObject, _lifeMesh, SFXSoundType.ClickPositive, Identifier.Positive));
                } 
                break;
        }
    }
    protected override void OnClickBack() // 로비로 돌아감
    {
        base.OnClickBack();
    }

    protected override void OnClickNext() // 씬 재시작
    {
        GameDesignManager.Instance.StageLevel += 1;
        base.OnClickNext();
    }
    protected override void OnEnable()
    {
        if (_note_Positive != null) _note_Positive.gameObject.SetActive(false);
        base.OnEnable();
        _stageMesh.text = string.Format("Stage {0}", GameDesignManager.Instance.StageLevel);
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }
}
