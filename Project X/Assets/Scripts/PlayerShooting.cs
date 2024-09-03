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

    private ControlsSerializable controls;
    void Start() {
        controls = GameObject.Find("GameManager").GetComponent<GameManager>().controls;

        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
    }


    void Update() {
        bool isFiring = weaponSpecs.isAutomatic ? Input.GetKey(controls.shoot) : Input.GetKeyDown(controls.shoot);
        if (isFiring && Time.time > nextFire) {
            nextFire = Time.time + weaponSpecs.fireRate;
            StartCoroutine(ShotEffect());

            Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position);

            if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, weaponSpecs.weaponRange)) {
                laserLine.SetPosition(1, hit.point);

                if (hit.rigidbody != null) {
                    hit.rigidbody.AddForce(-hit.normal * weaponSpecs.hitForce);
                }
            }
            else {
                laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * weaponSpecs.weaponRange));
            }
        }
    }


    private IEnumerator ShotEffect() {
        gunAudio.Play();
        laserLine.enabled = true;

        yield return shotDuration;

        laserLine.enabled = false;
    }
}
