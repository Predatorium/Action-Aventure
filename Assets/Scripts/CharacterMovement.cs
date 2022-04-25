using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
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
        Gravity();
        Jump();
        Move();
    }

    private void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (move.sqrMagnitude > 0f)
        {
            transform.rotation = Quaternion.Euler(0f, followTarget.eulerAngles.y, 0f);
            followTarget.localEulerAngles = new Vector3(followTarget.localEulerAngles.x, 0f, 0f);

            Vector3 dir = (transform.right * move.x + transform.forward * move.z);
            velocity = (dir * speed) + (Vector3.up * velocity.y);

            modele.LookAt(transform.position + new Vector3(dir.x, 0f, dir.z));
        }

        character.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && character.isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    private void Gravity()
    {
        if (character.isGrounded && velocity.y < 0f)
            velocity.y = 0f;

        velocity += Physics.gravity * Time.deltaTime;
    }
}
