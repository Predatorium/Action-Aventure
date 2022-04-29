using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;

public class Enemy : Entity
{
    [SerializeField] private NavMeshAgent agent = null;
    public CharacterAttack target = null;
    private Coroutine randPath = null;
    [SerializeField] private Vector2 distancePath;
    [SerializeField] private Vector2 inactiveTime;
    [SerializeField] private Image ui = null;
    [SerializeField] private Bullet prefabsBullet = null;
    [SerializeField] private float forceProj = 20f;
    public bool boss = false;
    private Coroutine delayAttack = null;
    private int iterator = 0;
    private bool range = false;
    private List<Bullet> bullets = new List<Bullet>();

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

        bullets.RemoveAll(b => b == null);

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
        if (animator.GetBool("Roll") || range)
            return;

        if (Vector3.Distance(target.transform.position, transform.position) >= agent.stoppingDistance && animator.GetInteger("Attack") == 0)
            agent.SetDestination(target.transform.position);
        else if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running")) && delayAttack == null)
            delayAttack = StartCoroutine(DelayAttackBoss(Random.Range(5f, 8f)));
    }

    private void PatrolBoss()
    {
        if (!GameManager.current.character)
            return;

        Vector3 dirPlayer = (GameManager.current.character.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up, dirPlayer, out RaycastHit hit, 50f))
            if (GameManager.current.character.gameObject == hit.collider.gameObject)
            {
                target = GameManager.current.character;
                ui.gameObject.SetActive(true);
            }
    }

    private IEnumerator DelayAttackBoss(float delay)
    {
        int rand = iterator < 3 ? Random.Range(0, 3) : 0;

        if (rand == 0)
        {
            iterator = 0;
            animator.SetInteger("Attack", 1);
            transform.LookAt(GameManager.current.character.transform.position);

            yield return null;
            while (animator.GetInteger("Attack") == 1)
            {
                if (TimeAttack(0.6f, 1f, "SlashOut") && Vector3.Distance(GameManager.current.character.transform.position, transform.position) <= agent.stoppingDistance
                    && Vector3.Dot(transform.forward, (GameManager.current.character.transform.position - transform.position).normalized) >= 0.7f)
                    animator.SetInteger("Attack", 2);
                yield return null;
            }
        }
        else if (rand == 1)
        {
            iterator++;

            range = true;
            agent.SetDestination(transform.position);
            int randtir = Random.Range(3, 6);

            for (int i = 0; i < randtir; i++)
            {
                Bullet tmp = Instantiate(prefabsBullet, transform.position + Vector3.up * 2.5f, Quaternion.identity);
                bullets.Add(tmp);
                yield return new WaitForSeconds(1.5f);
                tmp.rb.AddForce((GameManager.current.character.transform.position - tmp.transform.position).normalized * forceProj, ForceMode.Impulse);
            }

            range = false;
        }
        else
        {
            transform.forward = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            animator.SetBool("Roll", true);
            iterator++;
        }

        yield return new WaitForSeconds(delay);
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

    private void OnEnable()
    {
        gameObject.GetComponent<Collider>().enabled = true;
    }

    private void OnDisable()
    {
        gameObject.GetComponent<Collider>().enabled = false;
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
        else if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running")) && delayAttack == null)
        {
            transform.LookAt(GameManager.current.character.transform.position);
            animator.SetInteger("Attack", 2);
            delayAttack = StartCoroutine(DelayAttack(3f));
        }
    }

    private void PatrolEnemyBase()
    {
        if (randPath == null)
            randPath = StartCoroutine(RandomPath());

        if (!GameManager.current.character || GameManager.current.cinematiqueCam.activeSelf)
            return;

        Vector3 dirPlayer = (GameManager.current.character.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up, dirPlayer, out RaycastHit hit, 20f) && Vector3.Dot(transform.forward, dirPlayer) >= 0.2f)
        {
            CharacterAttack character = hit.collider.GetComponent<CharacterAttack>();
            if (character)
            {
                target = character;
                GameManager.current.enemies.ForEach(e => e.target = Vector3.Distance(e.transform.position, transform.position) < 20f ? target : null);
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

    public override void ChangeHealth(int _life, bool react)
    {
        if (boss)
        {
            if (life + _life <= 0)
            {
                ResetAnimator();
                StopAllCoroutines();
                animator.SetTrigger("React");
                GameManager.current.ActiveQTE(true);
            }
            else
            {
                if (_life < 0 && TimeAttack(0.35f, 0.65f, "Roll"))
                    return;

                life = Mathf.Clamp(life + _life, 0, maxLife);
            }
        }
        else
            base.ChangeHealth(_life, react);

        if (_life < 0f && life > 0f && !target)
        {
            target = GameManager.current.character;
            GameManager.current.enemies.ForEach(e => e.target = Vector3.Distance(e.transform.position, transform.position) < 10f ? target : null);
        }
    }

    public override IEnumerator Diying(Animator _animator)
    {
        _animator.SetTrigger("Die");
        life = 0;

        while (bullets.Count > 0)
        {
            Bullet tmp = bullets[0];
            bullets.Remove(tmp);
            Destroy(tmp.gameObject);
        }

        gameObject.GetComponent<Collider>().enabled = false;
        GameManager.current.enemies.Remove(this);
        Destroy(agent);
        Destroy(this);

        yield return new WaitForSeconds(5f);
    }
}
