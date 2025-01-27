using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    public GameObject moveGround;
    public GameObject moveGroundWallCheck;
    public int targetOffset = 5;  // Z 軸移動的距離
    public float moveSpeed = 2f;  // 移動速度

    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player"))
        {
            // 計算目標位置
            Vector3 targetPosition = moveGround.transform.position;
            targetPosition.x += targetOffset;

            // 開始移動地板
            StartCoroutine(MoveGroundToPosition(targetPosition));
            Debug.Log("test");
        }        
    }

    private IEnumerator MoveGroundToPosition(Vector3 targetPosition)
    {

        // 當前地板的位置
        Vector3 startPosition = moveGround.transform.position;

        float elapsedTime = 0f;

        // 持續移動直到到達目標位置
        while (Vector3.Distance(moveGround.transform.position, targetPosition) > 0.01f)
        {
            elapsedTime += Time.deltaTime;
            moveGround.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * moveSpeed);
            yield return null; // 等待下一幀
        }

        // 確保地板到達精確目標位置
        moveGround.transform.position = targetPosition;
    }
}
