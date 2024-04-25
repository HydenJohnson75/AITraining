using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ShootOnActivate : MonoBehaviour
{
    public GameObject bulletPref;
    public Transform bulletSpawn;
    public float bulletSpeed = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(Shoot);
    }

    private void Shoot(ActivateEventArgs arg0)
    {
        GameObject bullet = Instantiate(bulletPref, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * bulletSpeed;
        Destroy(bullet, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
