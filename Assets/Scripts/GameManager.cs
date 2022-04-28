using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager current = null;
    [SerializeField] private GameObject screenPause = null;
    public CharacterAttack character = null;
    public Settings settings = null;
    public List<Enemy> enemies = new List<Enemy>();
    [SerializeField] private CinemachineDollyCart cart;
    [SerializeField] private GameObject cinematiqueCam = null;
    [SerializeField] private GameObject targetCamPlayer = null;
    [SerializeField] private GameObject camQTEBoss = null;
    [SerializeField] private GameObject uiQTE = null;

    private void Awake()
    {
        current = this;
        GameManager.MouseFocus(false, CursorLockMode.Locked);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemies = FindObjectsOfType<Enemy>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            OnPause(!screenPause.activeSelf);
        }

        if (cinematiqueCam.activeSelf && cart.m_Position == cart.m_Path.PathLength)
        {
            cinematiqueCam.SetActive(false);
            character.enabled = true;
            character.GetComponent<CharacterMovement>().enabled = true;
            targetCamPlayer.SetActive(true);
        }
    }

    static public void MouseFocus(bool visible, CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
        Cursor.visible = visible;
    }

    public void OnPause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        screenPause.SetActive(pause);
        MouseFocus(pause, pause ? CursorLockMode.None : CursorLockMode.Locked);
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ActiveQTE(bool _active)
    {
        uiQTE.SetActive(_active);
        camQTEBoss.SetActive(_active);

        character.ResetAnimator();
        character.enabled = !_active;
        character.GetComponent<CharacterMovement>().enabled = !_active;
        targetCamPlayer.SetActive(!_active);

        enemies.ForEach(e => { e.ResetAnimator(); e.enabled = !_active; });
    }

    public void LooseQTE()
    {
        ActiveQTE(false);
    }

    public void WinQTE()
    {
        ActiveQTE(false);
        StartCoroutine(EndWin());
    }

    private IEnumerator EndWin()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }

    public static bool AnimIsFinish(Animator animator, string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
            && !animator.IsInTransition(0);
    }

    public static bool AnimIsNotFinish(Animator animator, string name)
    {
        return (animator.GetCurrentAnimatorStateInfo(0).IsName(name) || animator.GetNextAnimatorStateInfo(0).IsName(name))
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && !animator.IsInTransition(0);
    }
}
