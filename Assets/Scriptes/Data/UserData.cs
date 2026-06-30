
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "用户数据存储")]
public class UserData : ScriptableObject
{
    public LevelUserData[] levelUserDatas;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;
}

[System.Serializable]
public class LevelUserData 
{
    public bool isUnlock =false;
    public int score;

    public void UnlockLevel()
    {
        isUnlock = true;
    }

    public void setScore(int score)
    {
        this.score = score;
    }
}
