using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameCookInterface;

public class CanvasManager : MonoBehaviour {

    public delegate void CanvasActiveHandler();
    public static event CanvasActiveHandler ActiveEventCall;

    #region 싱글턴 정의
    private static CanvasManager _instance;
    public static CanvasManager Instance
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
    }
    #endregion

    [SerializeField]
    private List<CanvasUI> canvasUIPrefabs;

    [Header("UI Canvas Reference")]
    [SerializeField]
    private List<CanvasUI> canvasReference = new List<CanvasUI>();
    private static List<CanvasUI> canvasTemporary = new List<CanvasUI>();


    [Header("Canvas Transform")]
    [SerializeField] private Transform _canvasContainer;

    private Stack<int> _stackCanvasIndex = new Stack<int>();
    public int stackCount;
 
    IEnumerable<CanvasUI> orderByResult = from canvas in canvasTemporary
                                          orderby canvas.CanvasIndex
                                          select canvas;
    private void Start()
    {
        Initialization();
    }
    private void SortCanvasIndex()
    {
        foreach (CanvasUI canvas in orderByResult)
        {
            canvasReference.Add(canvas);
            canvas.transform.SetParent(_canvasContainer);
        }
    }
    private void AddTemporary(CanvasUI canvas, int index)
    {
        canvas.CanvasIndex = index;
        canvasTemporary.Add(canvas);
    }
    private void Initialization() // CanvasTypeToIndex 와 매치
    {
        foreach (CanvasUI canvas in canvasUIPrefabs){
            AddTemporary(Instantiate(canvas) as CanvasUI, canvas.canvasType.CanvasTypeToIndex());
        }
        SortCanvasIndex();
        OpenCanvasUI(CanvasType.Login);
        AudioManager.Instance.PlayBGMSound(BGMSoundType.Login);
    }

    public void OpenCanvasUI(CanvasType type)
    {
        if (canvasReference[type.CanvasTypeToIndex()] != null)
        {
            ActiveEventCall(); // 현재 열려있는 다른 canvas 닫기
            canvasReference[type.CanvasTypeToIndex()].gameObject.SetActive(true);
            _stackCanvasIndex.Push(type.CanvasTypeToIndex());
        }
        stackCount = _stackCanvasIndex.Count;
    }
    public void CloseCanvasUI()
    {
        int index = _stackCanvasIndex.Pop();
        if (_stackCanvasIndex.Count > 0)
        { // 하지만 절대 0일 경우는 만들지 않는다.
            index = _stackCanvasIndex.Peek();
            ActiveEventCall();
            canvasReference[index].gameObject.SetActive(true);
        }
        stackCount = _stackCanvasIndex.Count;
    }
}
