using System;
using System.Collections.Generic;

[Serializable]
public class UserScore{

    public long easy;
    public long hard;

    public UserScore(long easy, long hard)
    {
        this.easy = easy;
        this.hard = hard;
    }
    public UserScore()
    {

    }
    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["easy"] = easy;
        result["hard"] = hard;
        return result;
    }
}
