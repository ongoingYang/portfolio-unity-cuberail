using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameLevel", menuName = "GameLevelProperty", order = 3)]
public class GameLevelProperty : ScriptableObject {

    public int gameLevel;

    public int size;
    public int timer;
    [Header("X axis")]

    public int clockWise_X;
    public int counterClockWize_X;
    public int Slash_X;
    public int BackSlash_X;

    public int interation_Positive_X;
    public int interation_Negative_X;
    
   

    [Header ("Y axis")]
    public int clockWise_Y;
    public int counterClockWize_Y;
    public int Slash_Y;
    public int BackSlash_Y;

    public int interation_Positive_Y;
    public int interation_Negative_Y;
    
    

    [Header ("Z axis")]
    public int clockWise_Z;
    public int counterClockWize_Z;
    public int Slash_Z;
    public int BackSlash_Z;

    public int interation_Negative_Z;
    public int interation_Positive_Z;
    // blocklevel size
    // timer
    // 아이템 오브젝트

}
