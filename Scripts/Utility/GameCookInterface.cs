using UnityEngine;
namespace GameCookInterface
{

    public static class EnumTypeExtension
    {
        public static int SceneToIndex(this SceneLevel scene)
        {
            switch (scene)
            {
                default:
                    return 0;
                case SceneLevel.Intro:
                    return 1;
                case SceneLevel.Lobby:
                    return 2;
                case SceneLevel.Play:
                    return 3;
                case SceneLevel.Score:
                    return 4;
            }
        }
        public static int AxisToIndex(this DirectionAxis axis)
        {
            switch (axis)
            {
                case DirectionAxis.xDirection:
                    return 0;
                case DirectionAxis.yDirection:
                    return 1;
                case DirectionAxis.zDirection:
                    return 2;
                default:
                    Debug.LogWarning("invaild axis values");
                    return -1;
            }
        }
        public static float Interpolation(this float t, SmoothType type)
        {
            switch (type)
            {
                case SmoothType.SmootherStep:
                    return t * t * (3 - 2 * t);
                case SmoothType.SoSmootherStep:
                    return t * t * t * (t * (6f * t - 15f) + 10f);
                case SmoothType.EaseOutWithSin:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);
                case SmoothType.EaseInWithCos:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                case SmoothType.Exponential:
                    return t * t;
                default:
                    return t;
            }
        }

        public static float PalabolaMethod(this float t, float height)
        {
            return -4 * height * t * t + 4 * height * t;
        }

        public static string PadCutString(this string name, int cut, int blank, bool right)
        {
            switch (right){ //padRight
                default:
                    if (name.Length > cut){
                        string subName = name.Substring(0, cut);
                        name = subName.PadRight(cut + blank, ' ');
                        return name;
                    }
                    else{
                        name = name.PadRight(cut + blank, ' ');
                        return name;
                    }
                case false: //padLeft
                    if (name.Length > cut){
                        string subName = name.Substring(0, cut);
                        name = subName.PadLeft(cut + blank, ' ');
                        return name;
                    }
                    else{
                        name = name.PadLeft(cut + blank, ' ');
                        return name;
                    }
            }
        }
        public static int BlockItemToIndex(this PlaneChildItem planeItem)
        {
            switch (planeItem)
            {
                default:
                    return 0;
                case PlaneChildItem.Coin:
                    return 1;
                case PlaneChildItem.Clockwise:
                    return 2;
                case PlaneChildItem.CounterClockwise:
                    return 3;
                case PlaneChildItem.Slash:
                    return 4;
                case PlaneChildItem.BackSlash:
                    return 5;
                case PlaneChildItem.Interation_Positive:
                    return 6;
                case PlaneChildItem.Interation_Negative:
                    return 7;
            }
        }
        public static int CanvasTypeToIndex(this CanvasType canvasType)
        {
            switch (canvasType)
            {
                default:
                    return 0;
                case CanvasType.Login:
                    return 1;
                case CanvasType.Intro:
                    return 2;
                case CanvasType.Lobby:
                    return 3;
                case CanvasType.Options:
                    return 4;
                case CanvasType.Score:
                    return 5;
                case CanvasType.Credits:
                    return 6;
                case CanvasType.Pause:
                    return 7;
                case CanvasType.Play:
                    return 8;
                case CanvasType.Success:
                    return 9;
                case CanvasType.Failure:
                    return 10;
                case CanvasType.Ending:
                    return 11;
            }
        }

        public static int BGMSoundTypeToIndex(this BGMSoundType musicType)
        {
            switch (musicType)
            {
                default:
                    return 0;
                case BGMSoundType.Login:
                    return 1;
                case BGMSoundType.Intro:
                    return 2;
                case BGMSoundType.Lobby:
                    return 3;
                case BGMSoundType.Play:
                    return 4;
                case BGMSoundType.Success:
                    return 5;
                case BGMSoundType.Failure:
                    return 6;
                case BGMSoundType.Ending:
                    return 7;
                case BGMSoundType.Credits:
                    return 8;
            }
        }
        public static int UISoundTypeToIndex(this UISoundType UIsoundType)
        {
            switch (UIsoundType)
            {
                default:
                    return 0;
                case UISoundType.ButtonNext:
                    return 1;
                case UISoundType.ButtonBack:
                    return 2;
                case UISoundType.ButtonError:
                    return 3;
                case UISoundType.Access:
                    return 4;
                case UISoundType.Success:
                    return 5;
                case UISoundType.Switch:
                    return 6;
                case UISoundType.SoundCheck:
                    return 7;
                case UISoundType.Click:
                    return 8;
                case UISoundType.Interrupt:
                    return 9;
            }
        }
        public static int SFXSoundTypeToIndex(this SFXSoundType SFXsoundType)
        {
            switch (SFXsoundType)
            {
                default:
                    return 0;
                case SFXSoundType.GameReady:
                    return 1;
                case SFXSoundType.Failure:
                    return 2;
                case SFXSoundType.Success:
                    return 3;
                case SFXSoundType.ClickPositive:
                    return 4;
                case SFXSoundType.ClickNegative:
                    return 5;
                case SFXSoundType.MoverSpawn:
                    return 6;
                case SFXSoundType.MoverTurn:
                    return 7;
                case SFXSoundType.MoverJump:
                    return 8;
                case SFXSoundType.MoverEnter:
                    return 9;
                case SFXSoundType.MoverCrash:
                    return 10;
                case SFXSoundType.ItemTouch:
                    return 11;
                case SFXSoundType.InformSign:
                    return 12;
                case SFXSoundType.ItemDrop:
                    return 13;
                case SFXSoundType.CoinPickup:
                    return 14;
                case SFXSoundType.CoinDrop:
                    return 15;
                case SFXSoundType.BlockDrop:
                    return 16;
                case SFXSoundType.AllClear:
                    return 17;
            }
        }
        public static int PoolTypeToIndex(this ObjecTPoolType particleType)
        {
            switch (particleType)
            {
                default:
                    return 0;
                case ObjecTPoolType.SpawnLizard:
                    return 1;
                case ObjecTPoolType.SpawnWolf:
                    return 2;
                case ObjecTPoolType.SpawnCoin:
                    return 3;
            }
        }
        public static StageResult IdToResult(this int result)
        {
            switch (result)
            {
               default:
                    return StageResult.Worst;
                case 0:
                    return StageResult.Worse;
                case 1:
                    return StageResult.Bad;
                case 2:
                    return StageResult.Normal;
                case 3:
                    return StageResult.Good;
                case 4:
                    return StageResult.Great;
                case 5:
                    return StageResult.Awesome;
                case 6:
                    return StageResult.Amazing;
            }
        }
        public static GameMode TypeToID(this int type)
        {
            switch (type)
            {
                default:
                    return GameMode.Easy;
                case 1:
                    return GameMode.Hard;
            }
        }
        public static long CalculateScore(this long score, float time, int gold, int level, StageResult bonus)
        {
            long result = 0;
            switch (bonus)
            {
                case StageResult.Worst:
                    result -= 300;
                    break;
                case StageResult.Worse:
                    result -= 200;
                    break;
                case StageResult.Bad:
                    result -= 100;
                    break;
                case StageResult.Normal:
                    result += 100;
                    break;
                case StageResult.Good:
                    result += 300;
                    break;
                case StageResult.Great:
                    result += 500;
                    break;
                case StageResult.Awesome:
                    result += 700;
                    break;
                case StageResult.Amazing:
                    result += 1000;
                    break;
            }
            result += gold;
            result += level * 100;
            result += (long)time * 15;
            return score + result;
        }
    }
    

    public enum SmoothType
    {
        SmootherStep,
        SoSmootherStep,
        EaseOutWithSin,
        EaseInWithCos,
        Exponential
    }


    public interface IFirebaseAuth
    {
        void FirebaseSignIn(string email, string password);
        void FirebaseSignUp(string email, string password);
        void FirebaseAnonymous();
    }


    #region  UserInterface Element
    public enum SceneLevel
    {
        Auth,
        Intro,
        Lobby,
        Play,
        Score
    }


    public enum CanvasType
    {
        Extra,
        Login,
        Intro,
        Lobby,
        Options,
        Score,
        Credits,
        Pause,
        Play,
        Success,
        Failure,
        Ending
    }
    public enum BGMSoundType
    {
        None,
        Login,
        Intro,
        Lobby,
        Play,
        Success,
        Failure,
        Ending,
        Credits
    }
    public enum UISoundType
    {
        None,
        ButtonNext,
        ButtonBack,
        ButtonError,
        Access,
        Success,
        Switch,
        SoundCheck,
        Click,
        Interrupt
    }

    public enum SFXSoundType
    {
        None,
        GameReady,
        Failure,
        Success,
        ClickPositive,
        ClickNegative,
        MoverSpawn,
        MoverTurn,
        MoverJump,
        MoverEnter,
        MoverCrash,
        ItemTouch,
        InformSign,
        ItemDrop,
        CoinPickup,
        CoinDrop,
        BlockDrop,
        AllClear
    }
    #endregion

    #region  level Design Element

    public enum GameMode
    {
        Easy,
        Hard,
    }
    public enum DirectionAxis
    {
        xDirection,
        yDirection,
        zDirection
    };
    public enum Identifier
    {
        Positive,
        Negative
    }
    public enum PlaneChildItem
    {
        None,
        Coin,
        Clockwise,
        CounterClockwise,
        Slash,
        BackSlash,
        Interation_Positive,
        Interation_Negative,
    }

    public enum PlaneItemType
    {
        Interaction,
        Consumption,
        Direction,
    }
    #endregion

    #region movement
    public enum MoverType
    {
        Lizard,
        Wolf,
    }
    public enum SensorType
    {
        Hole = 9, 
        Plane = 10,
        Rotator = 11
    }
    public enum ActionState
    {
        Jumping,
        hitting,
        Waitng
    }
    #endregion
    #region  ObjectPool Item
    public enum ObjecTPoolType
    {
        Extra,
        SpawnLizard,
        SpawnWolf,
        SpawnCoin
    }
    #endregion
    public enum StageResult
    {
        Worst,
        Worse,
        Bad,
        Normal,
        Good,
        Great,
        Awesome,
        Amazing
    }
}
