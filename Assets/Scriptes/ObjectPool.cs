using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    // 新增：跟踪所有正在使用的对象（已从池子取出但尚未回收的）
    private HashSet<GameObject> activeObjects = new HashSet<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPool.Get: prefab 为空");
            return null;
        }

        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new Queue<GameObject>();

        Queue<GameObject> pool = poolDictionary[prefab];

        GameObject obj = null;
        while (pool.Count > 0)
        {
            obj = pool.Dequeue();
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.transform.SetParent(transform);
                obj.SetActive(true);
                break;
            }
            obj = null;
        }

        if (obj == null)
        {
            obj = Instantiate(prefab, position, rotation);
            obj.transform.SetParent(transform);
        }

        activeObjects.Add(obj); // 标记为使用中
        return obj;
    }

    public void Release(GameObject obj, GameObject prefab)
    {
        if (obj == null || prefab == null) return;

        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new Queue<GameObject>();

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[prefab].Enqueue(obj);
        activeObjects.Remove(obj); // 从使用中移除
    }

    /// <summary>
    /// 强制回收所有正在使用的对象（场景切换前调用）
    /// </summary>
    public void ReleaseAllActive()
    {
        // 复制一份，避免遍历时修改集合
        List<GameObject> toRelease = new List<GameObject>(activeObjects);

        foreach (var obj in toRelease)
        {
            if (obj != null)
            {
                // 找到这个对象对应的预制体（从池子字典里反向查找）
                GameObject prefab = FindPrefabForObject(obj);
                if (prefab != null)
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    poolDictionary[prefab].Enqueue(obj);
                    activeObjects.Remove(obj);
                }
                else
                {
                    // 找不到预制体映射，直接销毁
                    Destroy(obj);
                    activeObjects.Remove(obj);
                }
            }
        }
        activeObjects.Clear();
    }

    // 反向查找：根据实例找到对应的预制体
    private GameObject FindPrefabForObject(GameObject obj)
    {
        // 简单方案：遍历字典，通过名字匹配（你可以优化为更精确的方式）
        foreach (var kvp in poolDictionary)
        {
            // 检查队列里是否有同类型对象，或者直接用名字前缀匹配
            if (obj.name.Replace("(Clone)", "") == kvp.Key.name)
                return kvp.Key;
        }
        return null;
    }
}