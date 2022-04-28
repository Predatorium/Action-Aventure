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
    public bool boss = false;
    private Coroutine delayAttack = null;
    private float range = 0f;
    private int iterator = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        range = agent.stoppingDistance;

        if (target)
            agent.SetDestination(target.transform.position);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (animator.GetBool("Roll"))
            agent.Move(transform.forward * agent.speed * 1.5f * Time.deltaTime);

        if (GameManager.AnimIsFinish(animator, "Roll"))
            animator.SetBool("Roll", false);

            if (agent.velocity != Vector3.zero)
            animator.SetBool("Run", true);
        else
            animator.SetBool("Run", false);

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
        if (animator.GetBool("Roll"))
            return;

        if (Vector3.Distance(target.transform.position, transform.position) >= agent.stoppingDistance && animator.GetInteger("Attack") == 0)
            agent.SetDestination(target.transform.position);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running") && delayAttack == null)
            delayAttack = StartCoroutine(DelayAttack(Random.Range(2f, 5f), GameManager.current.character));
    }

    private void PatrolBoss()
    {
        if (!GameManager.current.character)
            return;

        Vector3 dirPlayer = (GameManager.current.character.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up, dirPlayer, out RaycastHit hit, 50f))
            if (GameManager.current.character.gameObject == hit.collider.gameObject)
                target = GameManager.current.character;
    }

    private IEnumerator DelayAttack(float delay, CharacterAttack character)
    {
        int rand = iterator < 3 ? Random.Range(0, 2) : 0;

        if (rand == 0)
        {
            iterator = 0;
            animator.SetInteger("Attack", 1);
            transform.LookAt(character.transform.position);

            yield return null;
            while (animator.GetInteger("Attack") == 1)
            {
                if (TimeAttack(0.6f, 1f, "SlashOut") && Vector3.Distance(character.transform.position, transform.position) <= agent.stoppingDistance
                    && Vector3.Dot(transform.forward, (character.transform.position - transform.position).normalized) >= 0.7f)
                    animator.SetInteger("Attack", 2);
                yield return null;
            }

            yield return new WaitForSeconds(delay);
        }
        else
        {
            animator.SetBool("Roll", true);
            iterator++;
        }

        delayAttack = null;
    }

    public void LooseQTE(float delay)
    {
        delayAttack = StartCoroutine(DelayAttack(delay));
    }

    private IEnumerator DelayAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        delayAttack = null;
    }

    private void PaternEnemyBase()
    {
        if (randPath != null)
        {
            StopCoroutine(randPath);
            randPath = null;
        }

        if (Vector3.Distance(target.transform.position, transform.position) >= agent.stoppingDistance && animator.GetInteger("Attack") == 0)
            agent.SetDestination(target.transform.position);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            transform.LookAt(GameManager.current.character.transform.position);
            animator.SetInteger("Attack", 2);
        }
    }

    private void PatrolEnemyBase()
    {
        if (randPath == null)
            randPath = StartCoroutine(RandomPath());

        if (!GameManager.current.character)
            return;

        Vector3 dirPlayer = (GameManager.current.character.transform.position - transform.position).normalized;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + dirPlayer * 10f, Color.red);
        if (Physics.Raycast(transform.position + Vector3.up, dirPlayer, out RaycastHit hit, 10f) && Vector3.Dot(transform.forward, dirPlayer) >= 0.5f)
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
            agent.SetDestination(hit.position);

        while (agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        float randTime = Random.Range(inactiveTime.x, inactiveTime.y);
        yield return new WaitForSeconds(randTime);
        randPath = null;
    }

    public override void ChangeHealth(int _life)
    {
        if (boss && life + _life <= 0)
        {
            ResetAnimator();
            GameManager.current.ActiveQTE(true);
        }
        else
            base.ChangeHealth(_life);

        if (_life < 0f && life > 0f && !target)
        {
            target = GameManager.current.character;
            GameManager.current.enemies.ForEach(e => e.target = Vector3.Distance(e.transform.position, transform.position) < 10f ? target : null);
        }
    }

    public override IEnumerator Diying(Animator _animator)
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
