using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    public GameObject moveGround;
    public GameObject moveGroundWallCheck;
    public int targetOffset = 5;  // Z �b���ʪ��Z��
    public float moveSpeed = 2f;  // ���ʳt��

    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player"))
        {
            // �p��ؼЦ�m
            Vector3 targetPosition = moveGround.transform.position;
            targetPosition.x += targetOffset;

            // �}�l���ʦa�O
            StartCoroutine(MoveGroundToPosition(targetPosition));
            Debug.Log("test");
        }        
    }

    private IEnumerator MoveGroundToPosition(Vector3 targetPosition)
    {

        // ��e�a�O����m
        Vector3 startPosition = moveGround.transform.position;

        float elapsedTime = 0f;

        // ���򲾰ʪ����F�ؼЦ�m
        while (Vector3.Distance(moveGround.transform.position, targetPosition) > 0.01f)
        {
            elapsedTime += Time.deltaTime;
            moveGround.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * moveSpeed);
            yield return null; // ���ݤU�@�V
        }

        // �T�O�a�O��F��T�ؼЦ�m
        moveGround.transform.position = targetPosition;
    }
}
