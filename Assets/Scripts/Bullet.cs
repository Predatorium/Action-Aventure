using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb = null;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        CharacterAttack character = collision.gameObject.GetComponent<CharacterAttack>();
        if (character)
            character.ChangeHealth(-10, false);

        Destroy(gameObject);
    }
}
