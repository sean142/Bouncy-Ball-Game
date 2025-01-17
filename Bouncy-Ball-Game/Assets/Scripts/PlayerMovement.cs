using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("玩家數據")]
    public float speed;
    public float maxChargeTime = 5f;  // 最大蓄力時間（秒）
    public float minJumpForce = 5f;  // 最小跳躍力
    public float maxJumpForce = 20f; // 最大跳躍力
    private Rigidbody rb;             // 球的剛體組件

    private float chargeTime = 0f;   // 當前蓄力時間
    public bool isCharging = false; // 是否正在蓄力
    public bool isGrounded = true;
    public bool isJumping = false;  // 是否正在跳躍

    [Header("地面")]
    public GameObject[] grounds; // 地面數組
    private int scaleZ = 50;     // 地面的長度

    [Header("地面觸發器")]
    public GameObject wallCheck;
    private Vector3 wallPoint;
    private int wallCheckScaleZ = 50;
    private int nextGroundIndex = 0; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < grounds.Length; i++)
        {
            grounds[i].transform.position = new Vector3(0, 0, i * scaleZ);
            
        }
    }
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);

        JumpControl();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {         
            // 移動下一片地面至最前端
            GameObject groundToMove = grounds[nextGroundIndex];
            float newZPosition = grounds.Length * scaleZ + groundToMove.transform.position.z;
            groundToMove.transform.position = new Vector3(0, 0, newZPosition);

            // 更新下一個地面的索引
            nextGroundIndex = (nextGroundIndex + 1) % grounds.Length;

            wallPoint = wallCheck.transform.position;
            wallPoint.z += wallCheckScaleZ;
            wallCheck.transform.position = wallPoint;
        }
    }

    private void JumpControl()
    {
        // 判斷是否著地
        isGrounded = IsGrounded();

        // 按下空白鍵開始蓄力
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isCharging = true;
            chargeTime = 0f; // 重置蓄力時間
        }

        // 持續蓄力
        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime); // 限制蓄力時間
        }

        // 釋放空白鍵執行跳躍
        if (Input.GetKeyUp(KeyCode.Space) && isCharging && isGrounded)
        {
            Jump();
            isCharging = false;
        }

        // 判斷是否到達最高點
        if (isJumping && rb.velocity.y <= 0)
        {
            rb.useGravity = true; // 開啟重力
            isJumping = false;    // 結束跳躍狀態
        }
    }
    void Jump()
    {
        // 根據蓄力時間計算跳躍力
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);

        // 跳躍時禁用重力
        rb.useGravity = false;
        rb.velocity = Vector3.zero; // 重置速度以避免累積
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 向上施加跳躍力

        isJumping = true; // 設定跳躍狀態
    }

    bool IsGrounded()
    {
        // 使用 Raycast 檢測是否在地面
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
