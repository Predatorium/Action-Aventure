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
        if (animator.GetInteger("Attack") != 0)
            return;

        Jump();
        Move();
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    private void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (move.sqrMagnitude > 0f && !animator.GetBool("Roll") && !animator.GetBool("Land"))
        {
            transform.rotation = Quaternion.Euler(0f, followTarget.eulerAngles.y, 0f);
            followTarget.localEulerAngles = new Vector3(followTarget.localEulerAngles.x, 0f, 0f);

            Vector3 dir = (transform.right * move.x + transform.forward * move.z).normalized;
            character.Move(dir * speed * Time.deltaTime);
            animator.SetBool("Run", true);

            modele.LookAt(transform.position + new Vector3(dir.x, 0f, dir.z));
        }
        else
        {
            animator.SetBool("Run", false);
            velocity = Vector3.up * velocity.y;
        }

        if (Input.GetButtonDown("Roll"))
            animator.SetBool("Roll", true);

        if (animator.GetBool("Roll"))
            character.Move(modele.forward * speed * Time.deltaTime);

        if (GameManager.AnimIsFinish(animator, "Roll"))
            animator.SetBool("Roll", false);
    }

    private void Jump()
    {
        if (GameManager.AnimIsFinish(animator, "Land") || !animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
            animator.SetBool("Land", false);

        if (character.isGrounded && animator.GetBool("Fall"))
        {
            animator.SetBool("Fall", false);
            animator.SetBool("Land", true);
        }

        if (!character.isGrounded && character.velocity.y < 0)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", true);
        }

        if (Input.GetButtonDown("Jump") && character.isGrounded && !animator.GetBool("Land") && !animator.GetBool("Roll"))
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            animator.SetBool("Jump", true);
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
