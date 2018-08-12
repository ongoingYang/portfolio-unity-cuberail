using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
using TMPro;
public class FailureCanvas : ClearedCanvas
{

    [SerializeField] private Image _note_LifeEmpty;
    [SerializeField] private Image _note_Negative;
    [SerializeField] private TextMeshProUGUI _stageMesh;
    [SerializeField] private TextMeshProUGUI _lifeMesh;

    private void Start(){
        _backButton.onClick.AddListener(() => OnClickBack());
        _nextButton.onClick.AddListener(() => OnClickNext());
    }
    protected override void DisplayResult(StageResult result){
        if (_userMesh != null && _scoreMesh != null && _coinMesh != null && _timeMesh != null && _lifeMesh != null)
        {
            UpdateScoreBoard(_userMesh, _scoreMesh, _coinMesh, _timeMesh, _lifeMesh);
        }
        switch (result){
            case StageResult.Worst:
                OpenResultPanel(0);
                break;
            case StageResult.Worse:
                OpenResultPanel(1);
                if (_note_Negative != null){
                    StartCoroutine(DisplayPlusLife(_note_Negative.gameObject, _lifeMesh, SFXSoundType.ClickNegative, Identifier.Negative));
                }
                break;
            case StageResult.Bad:
                OpenResultPanel(2);
                break;
        }
    }
    protected override void OnClickBack() // 로비로 돌아감
    {
        base.OnClickBack();
    }
    protected override void OnClickNext()// 씬 재시작
    {
        // life가 부족할 경우 알람 메시지 출력
        if (GameDesignManager.Instance.GameLife < 1){
            AudioManager.Instance.PlayUISound(UISoundType.ButtonError);
            if (_note_LifeEmpty != null) _note_LifeEmpty.gameObject.SetActive(true);
            return;
        }
        SoundOnByNext();
        LevelSceneManager.Instance.BackToTheScene(SceneLevel.Play, BGMSoundType.Play);
    }
    protected override void OnEnable()
    {
        if(_note_LifeEmpty!=null) _note_LifeEmpty.gameObject.SetActive(false);
        if (_note_Negative != null) _note_Negative.gameObject.SetActive(false);
        base.OnEnable();
        _stageMesh.text = string.Format("Stage {0}", GameDesignManager.Instance.StageLevel);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }
}
