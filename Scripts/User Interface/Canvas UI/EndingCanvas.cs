using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
public class EndingCanvas : ClearedCanvas {

    private void Start()
    {
        _backButton.onClick.AddListener(() => OnClickBack());
    }
    protected override void DisplayResult(StageResult result)
    {
        if(_userMesh!=null&& _scoreMesh != null && _coinMesh != null && _timeMesh != null)
        {
            UpdateScoreBoard(_userMesh, _scoreMesh, _coinMesh, _timeMesh);
        }
        switch (result)
        {
            case StageResult.Awesome:
                OpenResultPanel(0);
                break;
            case StageResult.Amazing:
                OpenResultPanel(1);
                break;
        }
    }
    protected override void OnClickBack() // 로비로 돌아감
    {
        base.OnClickBack();
    }

}
