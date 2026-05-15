
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "用户数据存储")]
public class UserData : ScriptableObject
{
    public LevelUserData[] levelUserDatas;
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
