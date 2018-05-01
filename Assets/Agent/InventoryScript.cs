﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public GunHandle gunHandle;
    public Health healthHandle;

    public void OnCollisionEnter(Collision collision)
    {
        PickupScript tempPickup;
        if (PhaNetworkManager.characterSelection == 0 && (tempPickup = collision.gameObject.GetComponent<PickupScript>()) != null)
        {
            
            if (tempPickup.itemType == PickupType.Ammo && gunHandle != null)
            {
                gunHandle.weaponSettings.currentNumberOfClips = Mathf.Min(30, (int)tempPickup.amount + gunHandle.weaponSettings.currentNumberOfClips);
            }
            else if (tempPickup.itemType == PickupType.Health && healthHandle != null)
            {
                healthHandle.takeDamage(-tempPickup.amount);
            }

            DestroyObject(collision.gameObject);
        }
    }
    

    // Use this for initialization
    void Start()
    {
        healthHandle = GetComponent<Health>();
        gunHandle = GetComponent<GunHandle>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
