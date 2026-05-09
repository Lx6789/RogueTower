using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{

    [Header("关卡卡片预制体")]
    public GameObject cardPrefab;

    private void OnEnable()
    {
        generatCard();
    }

    private void generatCard()
    {
        int levelCount = GameManager.Instance.LevelList.levels.Length;

        Transform parent = transform.GetChild(0);

        // 删除所有现有子物体
        if (parent != null && parent.childCount > 0)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < levelCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, parent);
            newCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
        }
    }
}
