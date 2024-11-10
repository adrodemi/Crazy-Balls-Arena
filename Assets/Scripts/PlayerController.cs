using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public bool hasPowerUp = false;
    public GameObject powerupIndicator;

    private float powerupStrength = 25f;
    private GameObject focalPoint;

    public PowerUpType currentPowerUp = PowerUpType.None;

    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;
    private float fireRate = 0.2f, nextFire = 0.0f;

    public float hangTime, smashSpeed, explosionForce, explosionRadius;
    private bool smashing = false;
    private float floorY;

    private Rigidbody playerRb;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");

        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed);

        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (currentPowerUp == PowerUpType.Rockets && Input.GetKey(KeyCode.F) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            LaunchRockets();
        }
        else if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            powerupIndicator.SetActive(true);
            hasPowerUp = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            Destroy(other.gameObject);

            if (powerupCountdown != null)
                StopCoroutine(powerupCountdown);

            powerupCountdown = StartCoroutine(PowerupCountdownCoroutine());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (currentPowerUp == PowerUpType.Pushback)
            {
                Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

                enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
        }
    }
    private void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }

    IEnumerator PowerupCountdownCoroutine()
    {
        yield return new WaitForSeconds(7f);
        hasPowerUp = false;
        currentPowerUp = PowerUpType.None;
        powerupIndicator.SetActive(false);
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        floorY = transform.position.y;

        float jumpTime = Time.time + hangTime;

        while (Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(0, smashSpeed);
            yield return null;
        }
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(0, -smashSpeed * 2);
            yield return null;
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position,
                explosionRadius, 0.0f, ForceMode.Impulse);
        }
        smashing = false;

    }
}