using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlatform : MonoBehaviour
{
    [Header("設定參數")]
    public float destroyDelay = 1f; // 板子消失前的延遲時間
    public float triggerChance = 0.5f; // 板子觸發消失的概率（0~1）

    private bool isTriggered = false; // 確保每塊板子只觸發一次

    private void OnCollisionEnter(Collision collision)
    {
        // 檢查是否是玩家踩上板子
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;

            // 判斷是否隨機觸發
            if (Random.value < triggerChance)
            {
                // 開始計時並摧毀板子
                Invoke(nameof(DestroyPlatform), destroyDelay);

                // 玩家死亡處理
                // PlayerDeath(collision.gameObject);
            }
        }
    }

    private void DestroyPlatform()
    {
        // 摧毀板子
        Destroy(gameObject);
    }

    private void PlayerDeath(GameObject player)
    {
        // 玩家死亡邏輯
        // 這裡可以觸發玩家死亡的動畫或重置場景等
        Debug.Log("玩家死亡！");

        // 示例：禁用玩家控制
        player.GetComponent<PlayerMovement>().enabled = false;
    }
}
