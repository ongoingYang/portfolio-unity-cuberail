using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameCookInterface;
public class GameDesignManager : MonoBehaviour {

    #region 싱글턴 정의
    private static GameDesignManager _instance;
    public static GameDesignManager Instance
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
    private void Awake(){
        MakeSingleton();
    }
    #endregion

    public delegate void TimeEventHandler(float time);
    public static event TimeEventHandler TimeUpdate;

    public delegate void lifeEventHandler(int life);
    public static event lifeEventHandler LifeUpdate;

    public delegate void GoldEventHandler(int gold);
    public static event GoldEventHandler GoldUpdate;

    public delegate void StageTurnEventHandler();
    public static event StageTurnEventHandler LevelReformEvent;


    private void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Start()
    {
        allStagesCount = gameLevel.Count;
    }
    private void Update(){
        if (OnTimerActivate) GameTimer -= Time.deltaTime;
    }

    [SerializeField] private List<GameLevelProperty> gameLevel = new List<GameLevelProperty>();
    public static string userName = null;
    public float blockSetDelay = 1.0f;
    public static int blockLevelSize;
    public static int allStagesCount;
    public bool OnTimerActivate = false;
    public int gameType;


    private int _lastStage;
    private static int _stageLevel;
    private static float _gameTimer;
    private static int _gameLife = 3;
    private static int _gameGold;
    private static long _gameScore;


    public float GameTimer{
        get{return _gameTimer;}
        set{_gameTimer = value;
            TimeUpdate(_gameTimer);
            if (_gameTimer <= 0.0f){
                _gameTimer = 0.0f;
                OnTimerActivate = false;
                CheckWhenTimeOut();
            }
        }
    }
    public int StageLevel{
        get{return _stageLevel;}
        set{_stageLevel = value;
            blockLevelSize = gameLevel[_stageLevel - 1].size;
        }
    }
    public int GameLife{
        get{return _gameLife;}
        set{_gameLife = value;
            LifeUpdate(_gameLife);
            if (_gameLife <= 0) StageFailure(StageResult.Worst);
        }
    }
    public void AddLife(Identifier type)
    {
        switch (type)
        {
            case Identifier.Positive:
                _gameLife++;
                break;
            case Identifier.Negative:
                _gameLife--;
                break;
        }
    
    }
    public int GameGold{
        get{return _gameGold;}
        set{_gameGold = value;
            GoldUpdate(_gameGold);
        }
    }

    public static long GameScore
    {
        get
        {
            return _gameScore;
        }

        set
        {
            _gameScore = value;
        }
    }

    public void CheckConsumed()
    {
        for (int i = 0; i < 3; i++){
            if (LevelDesigner.Instance.vertexArray[i].allConsumed != true) return; 
        }
        if (StageLevel < allStagesCount){
            StageSuccess(StageResult.Great);
        }
        else StageAllClear(StageResult.Amazing);
    }

    public void SetPlayerProperties( int life, int gold, long score) {
        GameLife = life;
        GameGold = gold;
        GameScore = score;
    }
    #region // play Scene 로드시 실행 _ planeBlock item과 moverObject 스폰
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if (scene.buildIndex == 3) SetGameStart();
    }
    public void SetGameStart()
    {
        if(StageLevel == 1){

            SetPlayerProperties(3, 0, 0);
        }
        else SetPlayerProperties(_gameLife, 0, _gameScore);
        StartCoroutine(RoutineGameReady(_stageLevel - 1, blockSetDelay));
    }
    IEnumerator RoutineGameReady(int level, float delayTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (level + 1 == 5)
        {
            PoolController.Instance.OnSpawnMover(DirectionAxis.xDirection, ObjecTPoolType.SpawnWolf, Identifier.Positive);
            PoolController.Instance.OnSpawnMover(DirectionAxis.yDirection, ObjecTPoolType.SpawnLizard, Identifier.Positive);
            PoolController.Instance.OnSpawnMover(DirectionAxis.zDirection, ObjecTPoolType.SpawnLizard, Identifier.Positive);
        }
        else if (level + 1 == 6) 
        {
            PoolController.Instance.OnSpawnMover(DirectionAxis.xDirection,ObjecTPoolType.SpawnLizard, Identifier.Positive);
            PoolController.Instance.OnSpawnMover(DirectionAxis.zDirection,ObjecTPoolType.SpawnLizard, Identifier.Positive);
        }   
        else PoolController.Instance.OnSpawnMover(ObjecTPoolType.SpawnLizard, Identifier.Positive);


        AudioManager.Instance.PlaySFXSound(SFXSoundType.GameReady);
        for (int i = 0; i < 3; i++)
        {
            SetGameLevelProperty(i, level);
            elapsedTime = 0;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        _gameTimer = gameLevel[level].timer;
        OnTimerActivate = true;
    }
    private void SetGameLevelProperty(int axis, int lv)
    {
        switch (axis)
        {
            case 0:
                if (gameLevel[lv].clockWise_X != 0)
                    GeneratePlaneItem(0, gameLevel[lv].clockWise_X, PlaneChildItem.Clockwise);
                if (gameLevel[lv].counterClockWize_X != 0)
                    GeneratePlaneItem(0, gameLevel[lv].counterClockWize_X, PlaneChildItem.CounterClockwise);
                if (gameLevel[lv].Slash_X != 0)
                    GeneratePlaneItem(0, gameLevel[lv].Slash_X, PlaneChildItem.Slash);
                if (gameLevel[lv].BackSlash_X != 0)
                    GeneratePlaneItem(0, gameLevel[lv].BackSlash_X, PlaneChildItem.BackSlash);
                if (gameLevel[lv].interation_Positive_X != 0)
                    GeneratePlaneItem(0, gameLevel[lv].interation_Positive_X, PlaneChildItem.Interation_Positive);
                if (gameLevel[lv].interation_Negative_X != 0)
                    GeneratePlaneItem(0, gameLevel[lv].interation_Negative_X, PlaneChildItem.Interation_Negative);
                LevelDesigner.Instance.vertexArray[0].AddReferenceList();
                break;
            case 1:
                if (gameLevel[lv].clockWise_Y != 0)
                    GeneratePlaneItem(1, gameLevel[lv].clockWise_Y, PlaneChildItem.Clockwise);
                if (gameLevel[lv].counterClockWize_Y != 0)
                    GeneratePlaneItem(1, gameLevel[lv].counterClockWize_Y, PlaneChildItem.CounterClockwise);
                if (gameLevel[lv].Slash_Y != 0)
                    GeneratePlaneItem(1, gameLevel[lv].Slash_Y, PlaneChildItem.Slash);
                if (gameLevel[lv].BackSlash_Y != 0)
                    GeneratePlaneItem(1, gameLevel[lv].BackSlash_Y, PlaneChildItem.BackSlash);
                if (gameLevel[lv].interation_Positive_Y != 0)
                    GeneratePlaneItem(1, gameLevel[lv].interation_Positive_Y, PlaneChildItem.Interation_Positive);
                if (gameLevel[lv].interation_Negative_Y != 0)
                    GeneratePlaneItem(1, gameLevel[lv].interation_Negative_Y, PlaneChildItem.Interation_Negative);
                LevelDesigner.Instance.vertexArray[1].AddReferenceList();
                break;
            case 2:
                if (gameLevel[lv].clockWise_Z != 0)
                    GeneratePlaneItem(2, gameLevel[lv].clockWise_Z, PlaneChildItem.Clockwise);
                if (gameLevel[lv].counterClockWize_Z != 0)
                    GeneratePlaneItem(2, gameLevel[lv].counterClockWize_Z, PlaneChildItem.CounterClockwise);
                if (gameLevel[lv].Slash_Z != 0)
                    GeneratePlaneItem(2, gameLevel[lv].Slash_Z, PlaneChildItem.Slash);
                if (gameLevel[lv].BackSlash_Z != 0)
                    GeneratePlaneItem(2, gameLevel[lv].BackSlash_Z, PlaneChildItem.BackSlash);
                if (gameLevel[lv].interation_Positive_Z != 0)
                    GeneratePlaneItem(2, gameLevel[lv].interation_Positive_Z, PlaneChildItem.Interation_Positive);
                if (gameLevel[lv].interation_Negative_Z != 0)
                    GeneratePlaneItem(2, gameLevel[lv].interation_Negative_Z, PlaneChildItem.Interation_Negative);
                LevelDesigner.Instance.vertexArray[2].AddReferenceList();
                break;
        }
    }

    List<int> usableGroup = new List<int>();
    public void GeneratePlaneItem(int axis, int amount, PlaneChildItem itemName)
    {
        usableGroup.Clear();
        int usableRange = 0;
        for (int index = 0; index < blockLevelSize * blockLevelSize; index++)
        {
            // 새로운 아이템을 표시할 수 있는 빈 공간 조사
            if (LevelDesigner.Instance.vertexArray[axis].planeBlockGroup[index].CurrentItemName == PlaneChildItem.None)
            {
                usableGroup.Add(index);
                usableRange++;
            }
        }
        for (int index = 0; index < usableRange; index++) // 셔플
        {
            int rand = Random.Range(0, usableRange);
            int temp = usableGroup[index];
            usableGroup[index] = usableGroup[rand];
            usableGroup[rand] = temp;
        }
        for (int index = 0; index < amount && index < usableRange; index++)
        {
            LevelDesigner.Instance.vertexArray[axis].
                planeBlockGroup[usableGroup[index]].SetActiveBlockItem(itemName);
        }
    }
    #endregion

    public int CheckClearedCondition()
    {
        int count = 0;
        for (int i = 0; i < 3; i++){
            if (LevelDesigner.Instance.vertexArray[i].IsOncleared) count++;
        }
        return count;
    }

    public void CheckWhenTimeOut()
    {
        int count = CheckClearedCondition();
        if (count > 1){
            if (StageLevel < allStagesCount) StageSuccess(count.IdToResult());
            else{ 
                if (count > 2) StageAllClear(StageResult.Awesome);
                else StageFailure(StageResult.Worse); // life -1
            }
        }
        else{
            StageFailure(count.IdToResult()); // 0개 또는 1개
        }
    }
    public static StageResult stageResult;
    public void StageSuccess(StageResult result)
    {
        stageResult = result;
        OnTimerActivate = false;
        AudioManager.Instance.PlaySFXSound(SFXSoundType.Success);
        LevelReformEvent();
        LevelSceneManager.Instance.LoadSceneByFade(SceneLevel.Score, CanvasType.Success, BGMSoundType.Success);
    }
    public void StageAllClear(StageResult result)
    {
        stageResult = result;
        OnTimerActivate = false;
        AudioManager.Instance.PlaySFXSound(SFXSoundType.AllClear);
        LevelReformEvent();
        LevelSceneManager.Instance.LoadSceneByFade(SceneLevel.Score, CanvasType.Ending, BGMSoundType.Ending);
    }
    public void StageFailure(StageResult result)
    {
        stageResult = result;
        OnTimerActivate = false;
        AudioManager.Instance.PlaySFXSound(SFXSoundType.Failure);
        LevelReformEvent();
        LevelSceneManager.Instance.LoadSceneByFade(SceneLevel.Score, CanvasType.Failure, BGMSoundType.Failure);
    }
}
