using UnityEngine;
using GameCookInterface;
[CreateAssetMenu(fileName = "bgmSound", menuName = "SoundSource/BGMSound", order = 1)]
public class BGMSound : SoundSource {

    public BGMSoundType bgmType;
    public bool isLoop;
}
