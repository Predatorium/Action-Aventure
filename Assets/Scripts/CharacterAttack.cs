using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterAttack : Entity
{
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private CharacterMovement movement = null;
    [SerializeField] private UiLifePlayer ui = null;

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
        if (animator.GetBool("Roll"))
            return;

        base.Update();

        if (Input.GetButtonDown("Attack") && controller.isGrounded)
        {
            if (animator.GetInteger("Attack") == 0)
                animator.SetInteger("Attack", 1);
            else if (TimeAttack(0.4f, 1f, "SlashOut"))
                animator.SetInteger("Attack", 2);
        }

        if (Input.GetButtonDown("Jump"))
            life = maxLife;
    }

    public override void ChangeHealth(int _life, bool react)
    {
        base.ChangeHealth(_life, react);
    }

    private void OnEnable()
    {
        if (ui)
            ui.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (ui)
            ui.gameObject.SetActive(false);
    }

    public override IEnumerator Diying(Animator _animator)
    {
        _animator.SetTrigger("Die");
        Destroy(movement);
        Destroy(this);

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("Game");
    }
}
