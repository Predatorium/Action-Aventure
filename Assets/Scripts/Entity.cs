using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected Weapon weapon = null;
    [SerializeField] protected int maxLife = 0;
    protected int life = 0;

    protected virtual void Awake()
    {
        life = maxLife;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SlashIn") || TimeAttack(0.46f, 0.6f, "SlashOut"))
            weapon.gameObject.SetActive(true);

        if (AnimIsFinish("SlashIn") || AnimIsFinish("SlashOut") || animator.GetCurrentAnimatorStateInfo(0).IsName("React")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Diying"))
        {
            animator.SetInteger("Attack", 0);
            weapon.gameObject.SetActive(false);
        }

        if (AnimIsFinish("React"))
            animator.SetBool("React", false);
    }

    protected bool AnimIsFinish(string name)
    {
        return GameManager.AnimIsFinish(animator, name);
    }

    protected bool TimeAttack(float min, float max, string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > min &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < max && !animator.IsInTransition(0);
    }

    public float GetPourcentageLife()
    {
        return (float)life / (float)maxLife;
    }

    public virtual void ChangeHealth(int _life)
    {
        if (_life < 0 && TimeAttack(0.35f, 0.65f, "Roll"))
            return;

        life = Mathf.Clamp(life + _life, 0, maxLife);

        if (_life > 0)
            return;
        
        if (life == 0)
            GameManager.current.StartCoroutine(Diying(animator));
        else
            animator.SetTrigger("React");
    }

    public void ResetAnimator()
    {
        animator.SetInteger("Attack", 0);
        animator.SetBool("Roll", false);
        animator.SetBool("Jump", false);
        animator.SetBool("Fall", false);
        animator.SetBool("Land", false);
        animator.SetBool("Run", false);
    }

    protected virtual IEnumerator Diying(Animator _animator)
    {
        yield return null;
    }
}
