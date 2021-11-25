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
    private float CurrentCamera = 0f;//���_�㉺�p�̕ϐ�

    private Rigidbody rb;

    //�}�E�X�p�x�̐���
    float minX = -80f, maxX = 80f;

    /// <summary> �J����������󂯕t����^�b�`�G���A </summary>
    [SerializeField]
    private DragHandler _lookController;

    /// <summary> �J�������x�i��/px�j </summary>
    [SerializeField]
    private float _angularPerPixel = 1f;

    /// <summary> �J��������Ƃ��đO�t���[���Ƀ^�b�`�����L�����o�X��̍��W </summary>
    private Vector2 _lookPointerPosPre;
    [SerializeField]

    /// <summary> �N���� </summary>
    private void Awake()
    {
        _lookController.OnBeginDragEvent += OnBeginDragLook;
        _lookController.OnDragEvent += OnDragLook;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveJoystick = GameObject.Find("MoveJoystick").GetComponent<FixedJoystick>(); //�Q�[���J�n����Canvas��MoveJoystick���擾
    }


    private void Update()
    {
        float _xMovement = moveJoystick.Horizontal; //Joystick�̐����̓��͒l����
        float _zMovement = moveJoystick.Vertical; //Joystick�̐����̓��͒l����

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
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation)); //MoveRotation���\�b�h���ĂсA��]������l��n��
    }

    /// �J��������
#region Look
    /// <summary> �h���b�O����J�n�i�J�����p�j </summary>
    private void OnBeginDragLook(PointerEventData eventData)
    {
        _lookPointerPosPre = _lookController.GetPositionOnCanvas(eventData.position);
    }

    /// <summary> �h���b�O���쒆�i�J�����p�j </summary>
    private void OnDragLook(PointerEventData eventData)
    {
        var pointerPosOnCanvas = _lookController.GetPositionOnCanvas(eventData.position);
        // �L�����o�X��őO�t���[�����牽px���삵�������v�Z
        var vector = pointerPosOnCanvas - _lookPointerPosPre;
        // ����ʂɉ����ăJ��������]
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
