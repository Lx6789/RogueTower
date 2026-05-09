using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{

    private void Start()
    {
        // Init 中已经设置了 baseTransform，这里只需计算初始方向
        if (baseTransform != null)
        {
            UpdateDirectionToBase();
        }
    }

    private void Update()
    {
        // 持续更新方向指向基地
        if (baseTransform != null)
        {
            UpdateDirectionToBase();
        }
    }

    /// <summary>
    /// 重写移动方法，直接飞向基地
    /// </summary>
    protected override void Move()
    {
        if (baseTransform == null || rb == null) return;

        // 直接向基地移动（使用父类的 moveDirection）
        Vector2 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    /// <summary>
    /// 更新方向指向基地
    /// </summary>
    private void UpdateDirectionToBase()
    {
        // 直接修改父类的 moveDirection
        Vector2 directionToBase = ((Vector2)(baseTransform.position - transform.position)).normalized;
        moveDirection = directionToBase;
    }

    /// <summary>
    /// 飞行敌人忽略墙壁检测
    /// </summary>
    protected override void DetectWalls(){}
}