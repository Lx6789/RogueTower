using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("动画参数")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float destroyDelay = 2.0f;

    private TMP_Text damageText;
    private Color originalColor;
    private float timer;
    private GameObject selfPrefab;
    private bool isActive;

    private void Awake()
    {
        damageText = GetComponentInChildren<TMP_Text>();
        if (damageText != null)
        {
            // 第一次创建时，强制使用预制体上保存的独立材质实例
            Material savedMaterial = damageText.fontMaterial;
            damageText.fontMaterial = savedMaterial;
            originalColor = damageText.color;
        }
    }

    public void Init(int damage, Vector3 worldPosition, Color? color = null, GameObject prefabRef = null)
    {
        if (damageText == null)
        {
            Debug.LogError("DamagePopup: 未找到 TMP_Text 组件");
            return;
        }

        // === 双重保障：每次从对象池取出时，再次强制使用独立材质实例 ===
        // 这能防止对象池复用导致材质实例被意外覆盖
        Material savedMaterial = damageText.fontMaterial;
        damageText.fontMaterial = savedMaterial;

        damageText.text = damage.ToString();
        damageText.color = color ?? originalColor;
        timer = 0f;
        isActive = true;
        selfPrefab = prefabRef;

        transform.position = worldPosition;
        gameObject.SetActive(true);

        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), destroyDelay);
    }

    private void Update()
    {
        if (!isActive) return;

        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer <= fadeDuration && damageText != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            Color col = damageText.color;
            col.a = alpha;
            damageText.color = col;
        }

        if (timer >= fadeDuration)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (!isActive) return;
        isActive = false;
        CancelInvoke(nameof(ReturnToPool));

        if (selfPrefab != null && ObjectPool.Instance != null)
            ObjectPool.Instance.Release(gameObject, selfPrefab);
        else
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isActive = false;
        timer = 0f;
        if (damageText != null)
        {
            Color col = originalColor;
            col.a = 1f;
            damageText.color = col;
        }
    }
}