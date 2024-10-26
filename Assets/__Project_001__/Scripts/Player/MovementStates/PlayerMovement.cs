using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // Move
    CharacterController controller;
    [HideInInspector] public Animator anim;

    public float moveSpeed;

    public float hInput, vInput;
    public Vector3 dir;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();  
    }

    private void Update()
    {
        anim.SetFloat("hInput", hInput);
        anim.SetFloat("vInput", vInput);

        Movement();
    }

    private void Movement()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        dir = transform.forward * vInput + transform.right * hInput;

        controller.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

}
