using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] WeaponSpecs weaponSpecs;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform gunEnd;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    private float nextFire;

    private WaitForSeconds reloadDuration;
    private bool isReloading = false;

    private GameManager gameManager;
    private ControlsSerializable controls;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        controls = gameManager.controls;

        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
    }

    void Start() {
        gameManager.ammoCnt = weaponSpecs.magazine;
        reloadDuration = new WaitForSeconds(weaponSpecs.reloadSpeed);
    }


    void Update() {
        bool isFiring = weaponSpecs.isAutomatic ? Input.GetKey(controls.shoot) : Input.GetKeyDown(controls.shoot);
        if (isFiring && Time.time > nextFire && gameManager.ammoCnt > 0 && !isReloading) {
            nextFire = Time.time + weaponSpecs.fireRate;
            gameManager.ammoCnt--;

            StartCoroutine(ShotEffect());

            Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position);

            if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, weaponSpecs.weaponRange)) {
                laserLine.SetPosition(1, hit.point);

                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null) {
                    enemy.TakeDamage(weaponSpecs.gunDamage);
                }

                if (hit.rigidbody != null) {
                    hit.rigidbody.AddForce(-hit.normal * weaponSpecs.hitForce);
                }
            }
            else {
                laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * weaponSpecs.weaponRange));
            }
        }

        if(Input.GetKeyDown(controls.reload)) {
            if (gameManager.ammoCnt < weaponSpecs.magazine && !isReloading)
                StartCoroutine(Reload());

        }
    }


    private IEnumerator ShotEffect() {
        gunAudio.Play();
        laserLine.enabled = true;

        yield return shotDuration;

        laserLine.enabled = false;
    }

    private IEnumerator Reload() {
        isReloading = true;

        yield return reloadDuration;

        isReloading = false;
        gameManager.ammoCnt = weaponSpecs.magazine;
    }
}
