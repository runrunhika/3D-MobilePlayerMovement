using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementController : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] GameObject fpsCamera;

    private Vector3 velocity = Vector3.zero;

    private Vector3 rotation = Vector3.zero;

    private Joystick moveJoystick;
    private float CurrentCamera = 0f;//視点上下用の変数

    private Rigidbody rb;

    //マウス角度の制限
    float minX = -80f, maxX = 80f;

    /// <summary> カメラ操作を受け付けるタッチエリア </summary>
    [SerializeField]
    private DragHandler _lookController;

    /// <summary> カメラ速度（°/px） </summary>
    [SerializeField]
    private float _angularPerPixel = 1f;

    /// <summary> カメラ操作として前フレームにタッチしたキャンバス上の座標 </summary>
    private Vector2 _lookPointerPosPre;
    [SerializeField]

    /// <summary> 起動時 </summary>
    private void Awake()
    {
        _lookController.OnBeginDragEvent += OnBeginDragLook;
        _lookController.OnDragEvent += OnDragLook;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveJoystick = GameObject.Find("MoveJoystick").GetComponent<FixedJoystick>(); //ゲーム開始時にCanvasのMoveJoystickを取得
    }


    private void Update()
    {
        float _xMovement = moveJoystick.Horizontal; //Joystickの水平の入力値を代入
        float _zMovement = moveJoystick.Vertical; //Joystickの垂直の入力値を代入

        Vector3 _movementHorizontal = transform.right * _xMovement;
        Vector3 _movementVertical = transform.forward * _zMovement;

        Vector3 _movementVelocity = (_movementHorizontal + _movementVertical).normalized * speed;
        velocity = _movementVelocity;
    }

    private void FixedUpdate()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation)); //MoveRotationメソッドを呼び、回転させる値を渡す
    }

    /// カメラ操作
#region Look
    /// <summary> ドラッグ操作開始（カメラ用） </summary>
    private void OnBeginDragLook(PointerEventData eventData)
    {
        _lookPointerPosPre = _lookController.GetPositionOnCanvas(eventData.position);
    }

    /// <summary> ドラッグ操作中（カメラ用） </summary>
    private void OnDragLook(PointerEventData eventData)
    {
        var pointerPosOnCanvas = _lookController.GetPositionOnCanvas(eventData.position);
        // キャンバス上で前フレームから何px操作したかを計算
        var vector = pointerPosOnCanvas - _lookPointerPosPre;
        // 操作量に応じてカメラを回転
        LookRotate(new Vector2(-vector.y, vector.x));
        _lookPointerPosPre = pointerPosOnCanvas;
    }

    private void LookRotate(Vector2 angles)
    {
        Vector2 deltaAngles = angles * _angularPerPixel;
        transform.eulerAngles += new Vector3(0f, deltaAngles.y);
        CurrentCamera = Mathf.Clamp(deltaAngles.x, minX, maxX);
        fpsCamera.transform.localEulerAngles += new Vector3(CurrentCamera, 0f, 0f);
    }
    #endregion
}
