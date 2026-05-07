using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNormal : Enemy
{
    [Header("移动相关")]
    [SerializeField]
    [Tooltip("速度")]
    private int speed = 2;
    [SerializeField]
    [Tooltip("射线检测距离")]
    private float rayLength = 0.6f;
    [SerializeField]
    [Tooltip("侧方射线偏移量")]
    private float sideRayOffset = 0.4f;
    [SerializeField]
    [Tooltip("检测的层级")]
    private LayerMask wallLayer;

    [Header("组件")]
    private Rigidbody2D rb;

    private Vector2 moveDirection = Vector2.left;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        DetectWalls();
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rb.position + speed * moveDirection * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    /// <summary>
    /// 检测方向
    /// </summary>
    private void DetectWalls()
    {
        // 四个方向：前、后、左、右
        Vector2 forward = moveDirection;
        Vector2 backward = -moveDirection;
        Vector2 left = Vector2.Perpendicular(moveDirection);
        Vector2 right = -left;

        // 探测四个方向，记录哪些是墙
        bool forwardBlocked = Physics2D.Raycast(transform.position, forward, rayLength, wallLayer);
        bool backwardBlocked = Physics2D.Raycast(transform.position, backward, rayLength, wallLayer);
        bool leftBlocked = Physics2D.Raycast(transform.position, left, rayLength, wallLayer);
        bool rightBlocked = Physics2D.Raycast(transform.position, right, rayLength, wallLayer);

        // 收集可行方向（不是墙 且 不是回头路）
        List<Vector2> availableDirections = new List<Vector2>();

        if (!forwardBlocked)
            availableDirections.Add(forward);

        if (!leftBlocked)
            availableDirections.Add(left);

        if (!rightBlocked)
            availableDirections.Add(right);

        // 前面三面都是墙才考虑掉头
        if (availableDirections.Count == 0 && !backwardBlocked)
            availableDirections.Add(backward);

        // 从可行方向里选一个
        if (availableDirections.Count > 0)
        {
            // 优先保持原方向，原方向可行就不换
            if (availableDirections.Contains(forward))
            {
                moveDirection = forward;
            }
            else
            {
                // 随机选一个可行方向
                int index = Random.Range(0, availableDirections.Count);
                moveDirection = availableDirections[index];
            }
        }
    }
}
