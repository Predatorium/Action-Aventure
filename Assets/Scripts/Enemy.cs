using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Enemy : Entity
{
    [SerializeField] private NavMeshAgent agent = null;
    public CharacterAttack target = null;
    private Coroutine randPath = null;
    [SerializeField] private Vector2 distancePath;
    [SerializeField] private Vector2 inactiveTime;
    [SerializeField] private bool boss = false;

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

        if (boss)
        {
            if (target)
                PaternBoss();
            else
                PatrolBoss();
        }
        else
        {
            if (target)
                PaternEnemyBase();
            else
                PatrolEnemyBase();
        }

    }

    private void PaternBoss()
    {
        if (randPath != null)
        {
            StopCoroutine(randPath);
            randPath = null;
        }

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

    private void PatrolBoss()
    {
        if (randPath == null)
            randPath = StartCoroutine(RandomPath());

        if (!GameManager.current.character)
            return;

        Vector3 dirPlayer = (GameManager.current.character.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up * agent.height / 2f, dirPlayer, out RaycastHit hit, 10f) && Vector3.Dot(transform.forward, dirPlayer) >= 0.7f)
        {
            CharacterAttack character = hit.collider.GetComponent<CharacterAttack>();
            if (character)
            {
                target = character;
                GameManager.current.enemies.ForEach(e => e.target = Vector3.Distance(e.transform.position, transform.position) < 10f ? target : null);
            }
        }
    }

    private void PaternEnemyBase()
    {
        if (randPath != null)
        {
            StopCoroutine(randPath);
            randPath = null;
        }

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

    private void PatrolEnemyBase()
    {
        if (randPath == null)
            randPath = StartCoroutine(RandomPath());

        if (!GameManager.current.character)
            return;

        Vector3 dirPlayer = (GameManager.current.character.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up * agent.height / 2f, dirPlayer, out RaycastHit hit, 10f) && Vector3.Dot(transform.forward, dirPlayer) >= 0.7f)
        {
            CharacterAttack character = hit.collider.GetComponent<CharacterAttack>();
            if (character)
            {
                target = character;
                GameManager.current.enemies.ForEach(e => e.target = Vector3.Distance(e.transform.position, transform.position) < 10f ? target : null);
            }
        }
    }

    private IEnumerator RandomPath()
    {
        Vector3 randomPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0f,
            Random.Range(-1f, 1f)).normalized * Random.Range(distancePath.x, distancePath.y) + Vector3.up * Random.Range(-1f, 1f);
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, 1))
        {
            agent.SetDestination(hit.position);
            animator.SetBool("Run", true);
        }

        while (agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        animator.SetBool("Run", false);
        float randTime = Random.Range(inactiveTime.x, inactiveTime.y);
        yield return new WaitForSeconds(randTime);
        randPath = null;
    }

    public override void ChangeHealth(int _life)
    {
        base.ChangeHealth(_life);

        if (_life < 0f && life > 0f && !target)
        {
            target = GameManager.current.character;
            GameManager.current.enemies.ForEach(e => e.target = Vector3.Distance(e.transform.position, transform.position) < 10f ? target : null);
        }
    }

    protected override IEnumerator Diying(Animator _animator)
    {
        _animator.SetTrigger("Die");

        GameManager.current.enemies.Remove(this);
        Destroy(agent);
        Destroy(this);

        yield return null;
        while (GameManager.AnimIsNotFinish(_animator, "Diying"))
            yield return null;
    }
}
