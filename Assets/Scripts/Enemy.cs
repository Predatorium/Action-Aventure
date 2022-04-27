using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [SerializeField] private NavMeshAgent agent = null;
    public CharacterAttack target = null;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        if (target)
            agent.SetDestination(target.transform.position);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (target)
        {
            if (Vector3.Distance(target.transform.position, transform.position) >= agent.stoppingDistance && animator.GetInteger("Attack") == 0)
            {
                agent.SetDestination(target.transform.position);
                animator.SetBool("Run", true);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
            {
                animator.SetInteger("Attack", 2);
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                animator.SetBool("Run", false);
            }
        }
        else
        {
            animator.SetBool("Run", false);

            Vector3 dirPlayer = (GameManager.current.character.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position + Vector3.up * agent.height / 2f, dirPlayer, out RaycastHit hit) && Vector3.Dot(transform.forward, dirPlayer) >= 0.7f)
            {
                CharacterAttack character = hit.collider.GetComponent<CharacterAttack>();
                if (character)
                    target = character;
            }
        }
    }

    public override void ChangeHealth(int _life)
    {
        base.ChangeHealth(_life);

        target = GameManager.current.character as CharacterAttack;
    }

    protected override IEnumerator Diying()
    {
        animator.SetTrigger("Die");
        yield return null;
        while (GameManager.AnimIsNotFinish(animator, "Diying"))
            yield return null;

        Destroy(agent);
        Destroy(this);
    }
}
