using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

public class AttackBody : MonoBehaviour
{
    public int Damage;
    public GenericBody source;

    public Collider col;
    public Rigidbody rb;

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null)
        {
            var gb = collision.gameObject.GetComponent<GenericBody>();
            var pb = collision.gameObject.GetComponent<PlayerGenericBody>();
            if (pb != null)
            {
                pb.PlayerTakeDamage(Damage);
                Despawn();
            }
            else if (gb != null)
            {
                gb.TakeDamage(Damage);
                Despawn();
            }

        }
    }
    void Despawn()
    {
        //add a check for particle systems having 0 particles
        Destroy(gameObject);
    }
}
