using System;
using System.Collections.Generic;

[Serializable]
public class UserData{

    public int gameType; 
    public float musicVolume;
    public float soundVolume;
    public string userEmail;

    public UserData(string userEmail, float musicVolume, float soundVolume, int gameType)
    {
        this.gameType = gameType;
        this.musicVolume = musicVolume;
        this.soundVolume = soundVolume;
        this.userEmail = userEmail;

    }
    public UserData()
    {

    }
    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        result["gameType"] = gameType;
        result["musicVolume"] = musicVolume;
        result["soundVolume"] = soundVolume;
        result["userEmail"] = userEmail;
        return result;
    }
}
