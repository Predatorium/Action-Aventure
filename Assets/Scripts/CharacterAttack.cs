using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (Input.GetButtonDown("Attack") && controller.isGrounded)
        {
            if (animator.GetInteger("Attack") == 0)
                animator.SetInteger("Attack", 1);
            else if (TimeAttack(0.4f, 1f, "SlashOut"))
                animator.SetInteger("Attack", 2);
        }
    }

    public override void ChangeHealth(int _life)
    {
        base.ChangeHealth(_life);
    }

    public override IEnumerator Diying(Animator _animator)
    {
        _animator.SetTrigger("Die");
        Destroy(movement);
        Destroy(this);

        yield return null;
        while (GameManager.AnimIsNotFinish(_animator, "Diying"))
            yield return null;

        SceneManager.LoadScene("Game");
    }
}
