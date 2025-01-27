using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("玩家數據")]
    public float speed = 5f;
    public float maxChargeTime = 5f;  // 最大蓄力時間（秒）
    public float minJumpForce = 5f;  // 最小跳躍力
    public float maxJumpForce = 20f; // 最大跳躍力

    private Rigidbody rb;
    private Collider ballCollider;       
    private Vector3 startPoint;

    public float chargeTime = 0f;   // 當前蓄力時間
    public bool isCharging = false; // 是否正在蓄力
    public bool isGrounded = true;

    [Header("場景設定")]
    public bool isWithinBounds = true; //判斷是否超出場外
    public float outOfBoundsY = 0.5f; // 場外檢測最低高度

    [Header("彈力設定")]
    public PhysicMaterial bounceMaterial; // 彈力材質
    public float baseBounciness = 0.2f;   // 基礎彈性
    public float maxBounciness = 0.5f;    // 最大彈性

    [Header("地面設定")]
    public GameObject[] normalGrounds;   
    public GameObject normalGroundsWallCheck;    
    private int nextGroundIndex = 0;
    private int groundLength = 50;  // 單片地面的長度

    public GameObject moveGround;
    public GameObject moveGroundWallCheck;
    public int targetOffset = 5;  // Z 軸移動的距離
    public float moveSpeed = 2f;  // 移動速度

    [Header("UI")]
    public Text distanceText;       // 距離顯示 UI
    //public Text chargeTimeText;     // 蓄力顯示UI
    public Slider chargeSlider;     // 蓄力條

    [Header("撞擊相關")]
    public bool isStunned = false; // 是否處於暈眩狀態
    private float stunEndTime = 0f; // 暈眩結束時間

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<Collider>();

        startPoint = transform.position; // 初始化起點位置
        InitializeGrounds();             // 初始化地面位置

        rb.useGravity = true;
    }

    private void Update()
    {
        if (isStunned && Time.time > stunEndTime)
        {
            isStunned = false;
        }

        if (transform.position.y < outOfBoundsY)
        {
            isWithinBounds = false;
        }

        if (!isStunned && isWithinBounds)
        {       
            // 處理跳躍邏輯
            JumpControl();
        }

        // 更新距離 UI
        UpdateDistanceUI();
    }

    private void FixedUpdate()
    {   
        if (!isStunned && isWithinBounds)
        {
            Movement();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            UpdateGroundPosition(); // 更新地面位置          
        }

        if (other.CompareTag("MoveGroundWallCheck"))
        {
            // 計算目標位置
            Vector3 targetPosition = moveGround.transform.position;
            targetPosition.z += targetOffset;

            // 開始移動地板
            StartCoroutine(MoveGroundToPosition(targetPosition));
        }

        if (other.CompareTag("DeadLine"))
        {
            // TODO: 場景重製
            // TODO: 
            ResetPlayer();
        }
    }     

    void Movement()
    {
        // TODO: wad控制Player

        float horizontalmove = Input.GetAxis("Horizontal");
        float verticalmove = Input.GetAxis("Vertical");

        if (verticalmove > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, verticalmove * speed * Time.fixedDeltaTime);
        }

        if (horizontalmove != 0f)
        {
            rb.velocity = new Vector3(horizontalmove * speed * Time.fixedDeltaTime, rb.velocity.y, rb.velocity.z);
        }
    }

    private void InitializeGrounds()
    {
        for (int i = 0; i < normalGrounds.Length; i++)
        {
            normalGrounds[i].transform.position = new Vector3(0, 0, i * groundLength);
        }
    }

    private void UpdateGroundPosition()
    {
        GameObject groundToMove = normalGrounds[nextGroundIndex];
        float newZPosition = normalGrounds.Length * groundLength + groundToMove.transform.position.z;
        groundToMove.transform.position = new Vector3(0, 0, newZPosition);

        // 更新下一個地面的索引
        nextGroundIndex = (nextGroundIndex + 1) % normalGrounds.Length;

        // 更新牆體檢測位置
        Vector3 wallPoint = normalGroundsWallCheck.transform.position;
        wallPoint.z += groundLength;
        normalGroundsWallCheck.transform.position = wallPoint;
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

    private void JumpControl()
    {     
        isGrounded = IsGrounded();

        // 按下空白鍵開始蓄力
        if (Input.GetKeyDown(KeyCode.Space)&&isGrounded)
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

            UpdateChargeUI();
        }

        // 釋放空白鍵執行跳躍
        if (Input.GetKeyUp(KeyCode.Space) && isCharging && isGrounded)
        {
            Jump();

            StartCoroutine(ResetChargeSlider()); 
        }
    }

    private void Jump()
    {
        isCharging = false;

        // 根據蓄力時間計算跳躍力
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeTime / maxChargeTime);
               
        // 向上施加跳躍力
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // 設置彈性（bounciness）
        float newBounciness = Mathf.Lerp(baseBounciness, maxBounciness, chargeTime / maxChargeTime);
        bounceMaterial.bounciness = newBounciness;
        ballCollider.material = bounceMaterial;
    }    

    public void HitPlayer(Vector3 force, float stunTime)
    {
        rb.velocity = force;
        isStunned = true;
        stunEndTime = Time.time + stunTime;
    }   

    bool IsGrounded()
    {
        // 使用 Raycast 檢測是否在地面
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    private void ResetPlayer()
    {
        transform.position = startPoint;
        isStunned = false;
        isCharging = false;
        isWithinBounds = true;
        isGrounded = true;
        chargeSlider.value = 0;

        // 重置剛體速度
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // 如果有旋轉，重置角速度
        }
    }
    private void UpdateDistanceUI()
    {
        if (distanceText != null)
        {
            float distance = transform.position.z - startPoint.z;
            distanceText.text = "距離: " + Mathf.RoundToInt(distance) + "m";
        }
    }

    private void UpdateChargeUI()
    {/*
        if (chargeTimeText)
        {
            chargeTimeText.text = Mathf.RoundToInt(chargeTime).ToString();
        }*/

        if (chargeSlider)
        {
            chargeSlider.value = chargeTime / maxChargeTime;
        }
    }

    private IEnumerator ResetChargeSlider()
    {
        float decrementSpeed = 1f; // 遞減速度，每秒減少的值

        while (chargeSlider.value > 0)
        {
            chargeSlider.value -= Time.deltaTime * decrementSpeed;
            yield return null; // 等待下一幀
        }

        chargeSlider.value = 0; // 確保最後值為 0
    }
}
