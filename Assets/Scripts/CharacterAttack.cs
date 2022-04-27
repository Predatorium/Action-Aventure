using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : Entity
{
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private CharacterMovement movement = null;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (animator.GetBool("Roll") || animator.GetBool("Land"))
            return;

        base.Update();

        if (Input.GetButtonDown("Attack") && attack == null && controller.isGrounded)
        {
            attack = StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        animator.SetInteger("Attack", 1);
        yield return null;
        while (animator.GetInteger("Attack") == 1)
        {
            if (Input.GetButtonDown("Attack"))
                animator.SetInteger("Attack", 2);
            yield return null;
        }
    }

    public override void ChangeHealth(int _life)
    {
        base.ChangeHealth(_life);
    }

    protected override IEnumerator Diying()
    {
        animator.SetBool("Die", true);
        while (GameManager.AnimIsNotFinish(animator, "Die"))
            yield return null;

        Destroy(movement);
        Destroy(this);
    }
}
