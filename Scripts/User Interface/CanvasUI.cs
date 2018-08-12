using UnityEngine;
using GameCookInterface;

[RequireComponent(typeof(Canvas))]
public abstract class CanvasUI : MonoBehaviour {

    public CanvasType canvasType;
    private int _canvasIndex;

    public int CanvasIndex    {
        get{
            return _canvasIndex;
        }
        set{
            _canvasIndex = value;
        }
    }
    protected virtual void OnEnable()
    {
        CanvasManager.ActiveEventCall += DeactiveCanvas;
    }
    protected virtual void OnDisable(){
        CanvasManager.ActiveEventCall -= DeactiveCanvas;
    }
    protected void DeactiveCanvas(){
        gameObject.SetActive(false);
    }
    protected void SoundOnByNext()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUISound(UISoundType.ButtonNext);
    }
    protected void SoundOnByBack()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUISound(UISoundType.ButtonBack);
    }
    protected void SoundOnByCheck()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUISound(UISoundType.SoundCheck);
    }
    protected void SoundOnBySwitch()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUISound(UISoundType.Switch);
    }
    protected virtual void OnClickNext(){
        SoundOnByNext();
    }
    protected virtual void OnClickBack(){
        SoundOnByBack();
        CanvasManager.Instance.CloseCanvasUI();
    }
}
