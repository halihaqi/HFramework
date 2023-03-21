using System;
using HFramework;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Base")]
    public float moveSpeed = 3;
    public float sprintSpeed = 5;
    public float rotateSmoothTime = 0.12f;
    public float jumpForce = 1;

    [Header("Gravity")]
    public float gravity = -15;

    [Header("FollowCamera")]
    public Camera followCamera;
    public Transform followTarget;
    public float topClamp = 70;//相机最大仰角
    public float bottomClamp = -30;//相机最大俯角
    public bool isFilpPitch = true;

    [Header("GroundCheck")]
    public float groundOffset = -0.29f;
    public float groundRadius = 0.28f;
    public LayerMask groundLayers = 1;
    
    [Header("Switch")] 
    public bool useSprint = true;
    public bool useJump = true;
    public bool useAnim = true;
    public bool useMove = true;
    public bool useRotate = true;
    public bool useGravity = true;

    //输入参数
    private Vector2 _inputLook;
    private Vector2 _inputMove;
    private float _threshold = 0.01f;//输入最低门槛
    private bool _isInputShift = false;

    //移动参数
    private bool _isGrounded = true;
    private float _targetRot = 0;
    private float _rotVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53;

    //相机参数
    private float _camTargetYaw;
    private float _camTargetPitch;

    //Component
    protected Animator anim;
    protected CharacterController cc;
    protected ThirdPersonCam thirdPersonCam;
    
    //Anim
    private bool _hasAnim = false;
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Ground = Animator.StringToHash("ground");
    private static readonly int Jump = Animator.StringToHash("jump");

    protected virtual void Awake()
    {
        //获取组件
        anim = GetComponent<Animator>();
        _hasAnim = anim != null && anim.runtimeAnimatorController != null;
        cc = GetComponent<CharacterController>();
        //打开输入监听
        HEntry.InputMgr.Enabled = true;
        HEntry.EventMgr.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnShift);
        HEntry.EventMgr.AddListener<KeyCode>(ClientEvent.GET_KEY_UP, OnShiftEnd);
        HEntry.EventMgr.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnSpace);
        HEntry.EventMgr.AddListener<Vector2>(ClientEvent.GET_MOVE, OnMoveInput);
        HEntry.EventMgr.AddListener<Vector2>(ClientEvent.GET_LOOK, OnLookInput);
    }

    private void Start()
    {
        followTarget ??= this.transform.GetChild(0);
        if (followCamera != null)
        {
            if (!followCamera.TryGetComponent(out thirdPersonCam))
                thirdPersonCam = followCamera.gameObject.AddComponent<ThirdPersonCam>();
            thirdPersonCam.followTarget = followTarget;
        }
    }

    protected virtual void OnDestroy()
    {
        //关闭输入监听
        HEntry.InputMgr.Enabled = false;
        HEntry.EventMgr.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnShift);
        HEntry.EventMgr.RemoveListener<KeyCode>(ClientEvent.GET_KEY_UP, OnShiftEnd);
        HEntry.EventMgr.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnSpace);
        HEntry.EventMgr.RemoveListener<Vector2>(ClientEvent.GET_MOVE, OnMoveInput);
        HEntry.EventMgr.RemoveListener<Vector2>(ClientEvent.GET_LOOK, OnLookInput);
    }

    protected virtual void Update()
    {
        Gravity();
        GroundCheck();
        Move();
        CameraTargetRotation();       
    }
    
    protected virtual void OnShift(KeyCode key)
    {
        if(!useSprint) return;
        if (key == KeyCode.LeftShift)
            _isInputShift = true;
    }

    protected virtual void OnShiftEnd(KeyCode key)
    {
        if(key == KeyCode.LeftShift)
            _isInputShift = false;
    }

    protected virtual void OnSpace(KeyCode key)
    {
        if(!useJump) return;
        if(key == KeyCode.Space)
            OnJump();
    }

    protected virtual void OnMoveInput(Vector2 input)
    {
        _inputMove = input;
    }

    protected virtual void OnLookInput(Vector2 input)
    {
        _inputLook = input;
    }

    /// <summary>
    /// 移动和转身
    /// </summary>
    private void Move()
    {
        if(!useMove || followCamera == null)
        {
            if(_hasAnim && useAnim)
                anim.SetFloat(Speed, 0);
            return;
        }
        //判断是否为冲刺速度
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        //判断移动输入是否为0
        if (_inputMove == Vector2.zero)
            targetSpeed = 0;

        //输入的方向
        Vector3 inputDir = new Vector3(_inputMove.x, 0, _inputMove.y);

        //人物移动时的八向旋转
        if(_inputMove != Vector2.zero)
        {
            _targetRot = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + followCamera.transform.eulerAngles.y;
            float rot = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRot, ref _rotVelocity, rotateSmoothTime);

            transform.rotation = Quaternion.Euler(0, rot, 0);
        }

        //旋转后的移动方向
        Vector3 targetDir = Quaternion.Euler(0, _targetRot, 0) * Vector3.forward;

        //移动人物,加上垂直速度
        cc.Move(targetDir.normalized * (targetSpeed * Time.deltaTime) + Vector3.up * (_verticalVelocity * moveSpeed * Time.deltaTime));

        //移动动画
        if(_hasAnim)
            anim.SetFloat(Speed, Input.GetKey(KeyCode.LeftShift) ? inputDir.magnitude : inputDir.magnitude / 2);
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    private void GroundCheck()
    {
        var position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - groundOffset, position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// 重力
    /// </summary>
    private void Gravity()
    {
        if (!useGravity)
        {
            _isGrounded = true;
            return;
        }
        if (_hasAnim && useAnim)
            anim.SetBool(Ground, _isGrounded);

        //Gravity
        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += gravity * Time.deltaTime;
        
        if (_isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;
    }
    
    private void OnJump()
    {
        //Jump
        if (_isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            
            if(_hasAnim && useAnim)
                anim.SetTrigger(Jump);
        }
    }

    /// <summary>
    /// 第三人称相机旋转
    /// </summary>
    private void CameraTargetRotation()
    {
        if(!useRotate || followCamera == null) return;
        //如果输入大于阈值
        if (_inputLook.sqrMagnitude >= _threshold)
        {
            _camTargetYaw += _inputLook.x;
            _camTargetPitch += _inputLook.y * (isFilpPitch ? -1 : 1);
        }
        //限制相机角度
        _camTargetYaw = TransformUtils.ClampAngle(_camTargetYaw, float.MinValue, float.MaxValue);
        _camTargetPitch = TransformUtils.ClampAngle(_camTargetPitch, bottomClamp, topClamp);
        //移动相机目标点
        followTarget.rotation = Quaternion.Euler(_camTargetPitch, _camTargetYaw, 0);
    }

    private void OnDrawGizmosSelected()
    {
        //如果在地面为绿色，如果不在为红色
        Color groundedColorGreen = new Color(0, 1, 0, 0.35f);
        Color airColorRed = new Color(1, 0, 0, 0.35f);
        Gizmos.color = _isGrounded ? groundedColorGreen : airColorRed;

        //画球
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundRadius);
    }

    public void BindCamera(Camera cam, ThirdPersonCam thirdCam)
    {
        followCamera = cam;
        thirdPersonCam = thirdCam;
        thirdCam.followTarget = followTarget;
    }
}
