using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCookInterface;


public abstract class SoundSource : ScriptableObject
{
    private int _sourceID;
    public AudioClip audioClip;

    public int SourceID{
        get{
            return _sourceID;
        }
        set{
            _sourceID = value;
        }
    }
}
