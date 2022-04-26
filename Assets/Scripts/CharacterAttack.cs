using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : Entity
{
    [SerializeField] private Weapon weapon = null;
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
        if (Input.GetButtonDown("Attack"))
        {
            attack = StartCoroutine(Attack(0f));
        }
    }

    private IEnumerator Attack(float delay)
    {
        bool okay = animator.GetCurrentAnimatorStateInfo(0).IsName("SlashOut") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
        while (okay)
        {
            yield return null;
        }

        attack = null;
    }

    public override void ChangeHealth(int _life)
    {
        base.ChangeHealth(_life);

        if (life == 0)
        {

        }
    }
}
