using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
public class CreditsCanvas : CanvasUI
{

    [SerializeField] private Button _backButton;
    private void Start()
    {
        _backButton.onClick.AddListener(() => OnClickBack());
    }
    protected override void OnClickBack()
    {
        AudioManager.Instance.PlayBGMSound(BGMSoundType.Lobby);
        base.OnClickBack();
    }
}
