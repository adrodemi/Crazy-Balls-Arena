using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    public Transform target;
    private float speed = 20f, rocketStrength = 25f, aliveTimer = 5f;
    private bool homing;

    // Update is called once per frame
    void Update()
    {
        if (homing && target != null)
        {
            //float range = Random.Range(-1f, 1f);
            //Vector3 spread = new Vector3(range, range, range);
            Vector3 moveDirection = (target.transform.position - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.LookAt(target);
        }
        else
            Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (target != null)
        {
            if (collision.gameObject.CompareTag(target.tag))
            {
                Rigidbody targetRb = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 away = -collision.contacts[0].normal;
                targetRb.AddForce(away * rocketStrength, ForceMode.Impulse);
                Destroy(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
    public void Fire(Transform newTarget)
    {
        target = newTarget;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }
}