using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager current = null;
    [SerializeField] private GameObject screenPause = null;
    public Entity character = null;
    public Settings settings = null;

    private void Awake()
    {
        current = this;
        GameManager.MouseFocus(false, CursorLockMode.Locked);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            OnPause(!screenPause.activeSelf);
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

    public static bool AnimIsFinish(Animator animator, string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public static bool AnimIsNotFinish(Animator animator, string name)
    {
        return (animator.GetCurrentAnimatorStateInfo(0).IsName(name) || animator.GetNextAnimatorStateInfo(0).IsName(name))
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f;
    }
}
