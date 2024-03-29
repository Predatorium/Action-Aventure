using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private CharacterController character = null;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpHeight = 1f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform followTarget = null;
    [SerializeField] private Transform modele = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetInteger("Attack") != 0 || GameManager.AnimIsNotFinish(animator, "React"))
            return;

        Move();
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    private void Move()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if (move.sqrMagnitude > 0f)
        {
            animator.SetBool("Run", true);

            if (!animator.GetBool("Roll"))
            {
                transform.rotation = Quaternion.Euler(0f, followTarget.eulerAngles.y, 0f);
                followTarget.localEulerAngles = new Vector3(followTarget.localEulerAngles.x, 0f, 0f);

                Vector3 dir = (transform.right * move.x + transform.forward * move.z).normalized;
                character.Move(dir * speed * Time.deltaTime);

                modele.LookAt(transform.position + new Vector3(dir.x, 0f, dir.z));
            }
        }
        else
            animator.SetBool("Run", false);

        if (Input.GetButtonDown("Roll"))
            animator.SetBool("Roll", true);

        if (animator.GetBool("Roll"))
        {
            character.Move(modele.forward * speed * 1.5f * Time.deltaTime);
            character.center = Vector3.up * 0.45f;
            character.height = 0.9f;
        }

        if (GameManager.AnimIsFinish(animator, "Roll"))
        {
            animator.SetBool("Roll", false);
            character.center = Vector3.up * 0.9f;
            character.height = 1.8f;
        }
    }

    private void Gravity()
    {
        if (character.isGrounded && velocity.y < 0f)
            velocity.y = 0f;

        velocity += Physics.gravity * Time.fixedDeltaTime;

        character.Move(velocity * Time.fixedDeltaTime);
    }
}
