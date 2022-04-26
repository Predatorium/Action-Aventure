using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : Entity
{
    [SerializeField] private CharacterController controller = null;
    private Coroutine attack = null;

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
        if (animator.GetInteger("Attack") > 0 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.SetInteger("Attack", 0);
            attack = null;
        }

        if (animator.GetBool("React"))
            animator.SetBool("React", false);

        if (Input.GetButtonDown("Attack") && attack == null && controller.isGrounded)
        {
            attack = StartCoroutine(Attack(0f));
        }
    }

    private IEnumerator Attack(float delay)
    {
        animator.SetInteger("Attack", 1);
        yield return null;
        while (animator.GetInteger("Attack") == 1 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            if (Input.GetButtonDown("Attack"))
                animator.SetInteger("Attack", 2);
            yield return null;
        }
    }

    public override void ChangeHealth(int _life)
    {
        base.ChangeHealth(_life);

        if (life == 0)
            StartCoroutine(Diying());
        else
            animator.SetBool("React", true);
    }

    private IEnumerator Diying()
    {
        animator.SetBool("Die", true);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {


            yield return null;
        }
    }
}
