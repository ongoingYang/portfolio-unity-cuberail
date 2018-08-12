using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using GameCookInterface;

public class DatabaseManager : MonoBehaviour {

    #region 싱글턴 정의
    public static DatabaseManager Instance { get; private set; }
    private void MakeSingleton(){
        if (Instance != null) DestroyImmediate(gameObject);
        else{
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Awake(){
        MakeSingleton();
    }
    #endregion
    public long UserEasyScore{ // 로컬 스코어 저장 
        get{return _userEasyScore;}
        set{_userEasyScore = value;}
    }
    public long UserHardScore
    {
        get{return _userHardScore;}
        set{_userHardScore = value;}
    }
    private readonly string _editorDatabaseUrl = "https://cuberailproject.firebaseio.com/";
    private DatabaseReference _rootReference;
    private DatabaseReference _leaderboardRef;
    const int MaxScoreDisplayCount = 10;
    public List<string> leaderBoardEasy;
    public List<string> leaderBoardHard;
    private long _userEasyScore;
    private long _userHardScore;
    private void Start(){
        leaderBoardEasy = new List<string>();
        leaderBoardHard = new List<string>();
        InitDatabase();

    }
    private void InitDatabase(){

        // 서비스 계정 사용 초기화 (Url 지정과 RootReference 가져오기)
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(_editorDatabaseUrl);
        _rootReference = FirebaseDatabase.DefaultInstance.RootReference;
        _leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("leaderBoard");

        //leaderBoard 스코어 데이터 변경 이벤트 핸들
        _leaderboardRef.Child("easy").OrderByChild("score").ValueChanged += HandleEasyScoreChanged;
        _leaderboardRef.Child("hard").OrderByChild("score").ValueChanged += HandleHardScoreChanged;
    }

    #region 계정 생성시 데이터 기록 초기화
    public void NewRegistration()
    {
        Debug.Log("신규등록 , 기록을 디폴드값으로 초기화 합니다.");
        UserData userData = new UserData(LoginCanvas.user.Email, 0.5f, 0.5f, 0);
        string jsonData = JsonUtility.ToJson(userData);
        UserScore userScore = new UserScore(0, 0);
        string jsonScore = JsonUtility.ToJson(userScore);
        _rootReference
            .Child("users")
            .Child(LoginCanvas.user.UserId)
            .SetRawJsonValueAsync(jsonData);
        _rootReference
            .Child("users")
            .Child(LoginCanvas.user.UserId).Child("_score")
            .SetRawJsonValueAsync(jsonScore);
    }
    #endregion

    #region  // 데이터가 변경되었을 때의 이벤트 수신  -> 기록 갱신 
    #endregion
    void HandleEasyScoreChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null){
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        
        leaderBoardEasy.Clear(); 
        if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
        {
            foreach (var childSnapshot in args.Snapshot.Children){
                if (childSnapshot.Child("score") == null ||
                    childSnapshot.Child("score").Value == null){
                    Debug.LogError("EasyScore저장소 키참조 실패");
                    break;
                }
                else{
                    string name = childSnapshot.Child("email").Value.ToString().Split('@')[0];
                    string value = childSnapshot.Child("score").Value.ToString();
                    leaderBoardEasy.Insert(0, name.PadCutString(8, 2, true) + value.PadCutString(7, 0, false));
                }
            }
        }
    }
    void HandleHardScoreChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null){
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        leaderBoardHard.Clear(); 
        if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0){
            foreach (var childSnapshot in args.Snapshot.Children){
                if (childSnapshot.Child("score") == null ||
                    childSnapshot.Child("score").Value == null){
                    Debug.LogError("HardScore저장소 키참조 실패");
                    break;
                }
                else{
                    string name = childSnapshot.Child("email").Value.ToString().Split('@')[0];
                    string value = childSnapshot.Child("score").Value.ToString();
                    leaderBoardHard.Insert(0, name.PadCutString(8, 2, true) + value.PadCutString(7, 0, false));
                }
            }
        }
    }
    #region data Write
    public void UpdateGameScoreData(GameMode type, long score){ // stage clear 시 사용 (Failure / Success canvas에서 사용)
        UpdateUserScore(type, score); //개인 최고 점수 기록
        UpdateRankingRecord(type, score, LoginCanvas.user.Email); //난이도별 Top 10 랭킹 업데이트
    }
    #endregion
    public void UpdateUserSetting(float musicVol, float soundVol, int gameType)
    {  // 유저 옵션세팅 변경 업데이트(option canvas에서 사용)
        UserData userData = new UserData(LoginCanvas.user.Email, musicVol, soundVol, gameType);
        FirebaseDatabase.DefaultInstance.GetReference("users")
            .Child(LoginCanvas.user.UserId)
            .UpdateChildrenAsync(userData.ToDictionary());
    }
    private void UpdateUserScore(GameMode type, long score)
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        DatabaseReference userScoreReference =
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(LoginCanvas.user.UserId).Child("_score");

        userScoreReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) Debug.Log("유저 score 데이타 참조 실패");
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                switch (type)
                {
                    case GameMode.Easy:
                        if (score > (long)snapshot.Child("easy").Value){
                            childUpdates["easy"] = score;
                            UserEasyScore = score;
                            userScoreReference.UpdateChildrenAsync(childUpdates);
                        }
                        break;
                    case GameMode.Hard:
                        if (score > (long)snapshot.Child("hard").Value){
                            childUpdates["hard"] = score;
                            UserHardScore = score;
                            userScoreReference.UpdateChildrenAsync(childUpdates);
                        }
                        break;
                }
            }
        });
    }
    
    private void UpdateRankingRecord(GameMode type, long inputScore, string inputEmail)
    {
        string gameType;
        switch (type){
            default:
                gameType = "easy";
                break;
            case GameMode.Hard:
                gameType = "hard";
                break;
        }
        _leaderboardRef.Child(gameType).RunTransaction(nodeData =>
            {
                List<object> recordList = nodeData.Value as List<object>;
                if (recordList == null) recordList = new List<object>();
                else if (nodeData.ChildrenCount >= MaxScoreDisplayCount) //기록이 꽉차면
                {
                    long minScore = long.MaxValue;
                    object minValue = null;
                    foreach (var record in recordList.Where(x => x is Dictionary<string, object>))
                    {
                        long score = (long)(record as Dictionary<string, object>)["score"];
                        if (score < minScore){
                            minScore = score;
                            minValue = record;
                        }
                    }
                    if (inputScore < minScore) return TransactionResult.Abort(); //순위 안에 최소값 보다 못하면 중단
                    recordList.Remove(minValue); //기존 최소값 정보 제거
                }
                Dictionary<string, object> newScoreMap = new Dictionary<string, object>();
                newScoreMap["email"] = inputEmail; //식별자
                newScoreMap["score"] = inputScore; //점수
                recordList.Add(newScoreMap); // 새로운 점수 기록
                nodeData.Value = recordList;
                return TransactionResult.Success(nodeData);
            });
    }

    #region Data Read

    // options Canvas와 lobby canvas 에서 보여주는 내용 
    public void GetUserSettingData() // intro Scene 에서 젤 처음 로딩
    {
        UserData userData = new UserData();
        FirebaseDatabase.DefaultInstance.GetReference("users")
        .Child(LoginCanvas.user.UserId)
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("유저 setting 데이타 참조 실패");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                userData = JsonUtility.FromJson(snapshot.GetRawJsonValue(), 
                    typeof(UserData)) as UserData;

                AudioManager.Instance.musicVolume = userData.musicVolume;
                AudioManager.Instance.soundVolume = userData.soundVolume;
                GameDesignManager.Instance.gameType = userData.gameType;
                GameDesignManager.userName = userData.userEmail;
            }
        });
    }
    // score Canvas 에서 보여주는 내용
    // failure/success canvas에서 score 결과값 데이터베이스 업데이트 후  
    // UserEasyScore,UserHardScore 로컬 저장 변수에 다시 각각 입력
    public void GetUserScoreData() // intro Scene 에서 젤 처음 로딩
    {
        UserScore userScore = new UserScore();
        FirebaseDatabase.DefaultInstance.GetReference("users")
        .Child(LoginCanvas.user.UserId).Child("_score")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                UserEasyScore = -1;
                UserHardScore = -1;
                Debug.Log("유저 score 데이타 참조 실패");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                userScore = JsonUtility.FromJson(snapshot.GetRawJsonValue(), 
                    typeof(UserScore)) as UserScore;

                UserEasyScore = userScore.easy;
                UserHardScore = userScore.hard;

            }
        });
    }
    #endregion
}
