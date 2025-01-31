using System.Collections;
using UnityEngine;

public class MoveGroundZ : MonoBehaviour
{
    public float moveSpeed = 2f;  // 移動速度
    public GameObject Z_moveGround;
    public int Z_targetOffset = 5;  // Z 軸移動的距離
    public bool isZtrue = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isZtrue)
        {
            Vector3 targetPosition = Z_moveGround.transform.position;
            targetPosition.z += Z_targetOffset;
            StartCoroutine(MoveGroundToPosition(targetPosition));
            isZtrue = true;
            Debug.Log("Z");
        }
    }

    private IEnumerator MoveGroundToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = Z_moveGround.transform.position;
        float elapsedTime = 0f;

        while (Vector3.Distance(Z_moveGround.transform.position, targetPosition) > 0.01f)
        {
            elapsedTime += Time.deltaTime;
            Z_moveGround.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * moveSpeed);
            yield return null;
        }
        Z_moveGround.transform.position = targetPosition;
    }
}
