using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bullet : Item
{
    public float lifespan = 3f;

    public override void Interact(GameObject owner) {}
    private void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter " + collision.gameObject.name);
        Destroy(gameObject);
    }
}
