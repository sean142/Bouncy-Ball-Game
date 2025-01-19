using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("玩家數據")]
    public float speed = 5f;
    public float maxChargeTime = 5f;  // 最大蓄力時間（秒）
    public float minJumpForce = 5f;  // 最小跳躍力
    public float maxJumpForce = 20f; // 最大跳躍力

    private Rigidbody rb;
    public float chargeTime = 0f;   // 當前蓄力時間
    private bool isCharging = false; // 是否正在蓄力
    private bool isJumping = false;  // 是否正在跳躍
    private Vector3 startPoint;

    [Header("彈力相關")]
    public PhysicMaterial bounceMaterial; // 彈力材質
    public float baseBounciness = 0.2f;   // 基礎彈性
    public float maxBounciness = 0.5f;    // 最大彈性
    private Collider ballCollider;        // 球的碰撞器

    [Header("地面設定")]
    public GameObject[] grounds;    // 地面陣列
    public GameObject wallCheck;    // 墙體檢測物件
    private int nextGroundIndex = 0;
    private int groundLength = 50;  // 單片地面的長度

    [Header("UI")]
    public Text distanceText;       // 距離顯示 UI
    public Text chargeTimeText;     // 蓄力顯示UI
    public Slider chargeSlider;     // 蓄力條

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<Collider>();

        startPoint = transform.position; // 初始化起點位置
        InitializeGrounds();             // 初始化地面位置
    }

    private void Update()
    {
        // 持續向前移動
        MoveForward();

        // 處理跳躍邏輯
        JumpControl();

        // 更新距離 UI
        UpdateDistanceUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            UpdateGroundPosition(); // 更新地面位置
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void InitializeGrounds()
    {
        for (int i = 0; i < grounds.Length; i++)
        {
            grounds[i].transform.position = new Vector3(0, 0, i * groundLength);
        }
    }

    private void UpdateGroundPosition()
    {
        GameObject groundToMove = grounds[nextGroundIndex];
        float newZPosition = grounds.Length * groundLength + groundToMove.transform.position.z;
        groundToMove.transform.position = new Vector3(0, 0, newZPosition);

        // 更新下一個地面的索引
        nextGroundIndex = (nextGroundIndex + 1) % grounds.Length;

        // 更新牆體檢測位置
        Vector3 wallPoint = wallCheck.transform.position;
        wallPoint.z += groundLength;
        wallCheck.transform.position = wallPoint;
    }

    private void JumpControl()
    {
        if (IsGrounded())
        {
            isJumping = false;            

            // 按下空白鍵開始蓄力
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isCharging = true;
                chargeTime = 0f; // 重置蓄力時間
                                
                bounceMaterial.bounciness = 0; //重置bounciness的值
            }

            // 持續蓄力
            if (isCharging && Input.GetKey(KeyCode.Space))
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime); // 限制蓄力時間

                chargeTimeText.text = Mathf.RoundToInt(chargeTime).ToString();
            }

            // 釋放空白鍵執行跳躍
            if (Input.GetKeyUp(KeyCode.Space) && isCharging)
            {
                Jump();
                isCharging = false;
            }
        }

        // 當在空中時啟用重力
        if (isJumping && rb.velocity.y <= 0)
        {
            rb.useGravity = true;
        }
    }

    private void Jump()
    {
        // 根據蓄力時間計算跳躍力
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);

        // 重置速度並禁用重力
        rb.velocity = Vector3.zero;
        rb.useGravity = false;

        // 向上施加跳躍力
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isJumping = true;

        // 設置彈性（bounciness）
        float newBounciness = Mathf.Lerp(baseBounciness, maxBounciness, chargeTime / maxChargeTime - 1.5f);
        bounceMaterial.bounciness = newBounciness;
        ballCollider.material = bounceMaterial;
    }

    private bool IsGrounded()
    {
        // 使用 Raycast 檢測是否在地面
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void UpdateDistanceUI()
    {
        if (distanceText != null)
        {
            float distance = transform.position.z - startPoint.z;
            distanceText.text = "距離: " + Mathf.RoundToInt(distance) + "m";
        }
    }
}
