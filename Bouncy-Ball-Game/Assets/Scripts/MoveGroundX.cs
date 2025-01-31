using System.Collections;
using UnityEngine;

public class MoveGroundX : MonoBehaviour
{
    public float moveSpeed = 2f;  // 移動速度
    public GameObject X_moveGround;
    public int X_targetOffset = 5;  // X 軸移動的距離
    public bool isXtrue = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isXtrue)
        {
            Vector3 targetPosition = X_moveGround.transform.position;
            targetPosition.x += X_targetOffset;
            StartCoroutine(MoveGroundToPosition(targetPosition));
            isXtrue = true;
            Debug.Log("X");
        }
    }

    private IEnumerator MoveGroundToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = X_moveGround.transform.position;
        float elapsedTime = 0f;

        while (Vector3.Distance(X_moveGround.transform.position, targetPosition) > 0.01f)
        {
            elapsedTime += Time.deltaTime;
            X_moveGround.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * moveSpeed);
            yield return null;
        }
        X_moveGround.transform.position = targetPosition;
    }
}