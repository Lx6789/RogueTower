using UnityEngine;
using System.IO;

public static class SaveManager
{
    // 存档文件名
    private const string SaveFileName = "player_save.json";

    /// <summary>
    /// 获取完整的存档文件路径
    /// </summary>
    private static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    /// <summary>
    /// 将 UserData 保存到本地文件
    /// </summary>
    /// <param name="userData">要保存的用户数据</param>
    public static void Save(UserData userData)
    {
        if (userData == null)
        {
            Debug.LogError("SaveManager.Save: userData 为空，无法保存");
            return;
        }

        // 将 UserData 里的数据转换为可序列化的格式
        //JsonUtility 有一个硬性限制：它不能直接序列化继承自 UnityEngine.Object 的类。
        SaveData saveData = new SaveData();
        saveData.levelUserDatas = userData.levelUserDatas;
        saveData.bgmVolume = userData.bgmVolume;
        saveData.sfxVolume = userData.sfxVolume;

        // 序列化为 JSON 字符串
        string json = JsonUtility.ToJson(saveData, true);

        // 写入文件
        string path = GetSavePath();
        File.WriteAllText(path, json);

        Debug.Log($"存档已保存到: {path}");
    }

    /// <summary>
    /// 从本地文件加载 UserData
    /// </summary>
    /// <param name="userData">要填充数据的 UserData 对象</param>
    /// <returns>是否成功加载存档</returns>
    public static bool Load(UserData userData)
    {
        if (userData == null)
        {
            Debug.LogError("SaveManager.Load: userData 为空，无法加载");
            return false;
        }

        string path = GetSavePath();

        // 如果存档文件不存在，返回 false
        if (!File.Exists(path))
        {
            Debug.Log($"存档文件不存在: {path}");
            return false;
        }

        // 读取 JSON 字符串
        string json = File.ReadAllText(path);

        // 反序列化
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (saveData == null || saveData.levelUserDatas == null)
        {
            Debug.LogError("SaveManager.Load: 存档数据损坏");
            return false;
        }

        // 将加载的数据填充回 UserData
        userData.levelUserDatas = saveData.levelUserDatas;

        Debug.Log($"存档已从 {path} 加载");
        return true;
    }

    /// <summary>
    /// 删除存档文件
    /// </summary>
    public static void DeleteSave()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"存档已删除: {path}");
        }
    }

    // 用于序列化的中间数据结构
    [System.Serializable]
    private class SaveData
    {
        public LevelUserData[] levelUserDatas;

        public float bgmVolume = 1f;
        public float sfxVolume = 1f;
    }
}