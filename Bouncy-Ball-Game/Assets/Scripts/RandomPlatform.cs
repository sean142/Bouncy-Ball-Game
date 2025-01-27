using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlatform : MonoBehaviour
{
    [Header("�]�w�Ѽ�")]
    public float destroyDelay = 1f; // �O�l�����e������ɶ�
    public float triggerChance = 0.5f; // �O�lĲ�o���������v�]0~1�^

    private bool isTriggered = false; // �T�O�C���O�l�uĲ�o�@��

    private void OnCollisionEnter(Collision collision)
    {
        // �ˬd�O�_�O���a��W�O�l
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;

            // �P�_�O�_�H��Ĳ�o
            if (Random.value < triggerChance)
            {
                // �}�l�p�ɨúR���O�l
                Invoke(nameof(DestroyPlatform), destroyDelay);

                // ���a���`�B�z
                // PlayerDeath(collision.gameObject);
            }
        }
    }

    private void DestroyPlatform()
    {
        // �R���O�l
        Destroy(gameObject);
    }

    private void PlayerDeath(GameObject player)
    {
        // ���a���`�޿�
        // �o�̥i�HĲ�o���a���`���ʵe�έ��m������
        Debug.Log("���a���`�I");

        // �ܨҡG�T�Ϊ��a����
        player.GetComponent<PlayerMovement>().enabled = false;
    }
}
