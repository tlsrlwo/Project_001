using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // 이동
    [Header("Movement")]
    CharacterController controller;
    [HideInInspector] public Animator anim;

    public float moveSpeed = 3.0f;
    public float sprintSpeed = 5.0f;
    public float speedChangeRate = 10.0f;

    public float hInput, vInput;
    private bool isSprint = false;
    private Vector2 move;
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    


    // 카메라
    Camera mainCamera;
    private Vector2 look;

    [Range(0.0f, 0.3f)] public float rotationSmoothTime = 0.12f;


    // Strafe
    private bool isStrafe;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 플레이어의 이동(PlayerController)
        Movement();

        // 카메라 input값
        float hMouse = Input.GetAxis("Mouse X");
        float vMouse = Input.GetAxis("Mouse Y") * -1; //-1을 곱하는 이유는 상하반전을 일으킴(마우스 위아래 움직임)
        look = new Vector2(hMouse, vMouse);

        isSprint = Input.GetKey(KeyCode.LeftShift);
        if (isSprint)
        {
            anim.SetFloat("MotionSpeed", 1.2f);
        }
        else
        {
            anim.SetFloat("MotionSpeed", 1f);
        }

        isStrafe = Input.GetKey(KeyCode.Mouse1);

        if (isStrafe)
        {
            Vector3 cameraForward = Camera.main.transform.forward.normalized;
            cameraForward.y = 0;
            transform.forward = cameraForward;      // 캐릭터의 방향을 카메라의 앞 방향으로 고정
        }

        anim.SetFloat("Speed", animationBlend);
        anim.SetFloat("hInput", move.x);
        anim.SetFloat("vInput", move.y);
        anim.SetFloat("Strafe", isStrafe ? 1 : 0);

    }

    private void Movement()
    {
        // 플레이어 이동 input값
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        move = new Vector2(horizontal, vertical);

        // isSprint에 따라 스피드 변화
        float targetSpeed = isSprint ? sprintSpeed : moveSpeed;

        // 인풋값이 없으면 targetSpeed를 0으로 설정
        if (move == Vector2.zero) targetSpeed = 0.0f;

        // 플레이어의 수평 이동속도값 (점프가 수직)
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = move.magnitude;

        // targetSpeed값에 맞춰 감속과 가속 ( 자연스러운 속도 변화를 위한 )
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // 선형적인 결과보다는 곡선적인 결과를 생성하여 더 유기적인 속도 변화를 제공합니다.               
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);           

            // 속도를 소숫점 3자리까지 반올림
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }


        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;
            
        if (move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                rotationSmoothTime);

            if (!isStrafe)
            {
                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        // Controller 이동
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

    }

    private void CameraRotation()
    {

    }

}
