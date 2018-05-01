﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : PhaNetworkingMessager {

    [Tooltip("Amount of health for the object")]
    public float health;
    
    public float currentHealth;

    public float deathDelay;

    public bool disableOnDeath = false;
    public UnityEvent OnDisable;
    public bool destroyOnDeath = false;
    public UnityEvent OnDeath;

 // Use this for initialization
 void Start () {
        currentHealth = health;
 }
 
    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Min(currentHealth, health);
        if (PhaNetworkManager.characterSelection == 0 && gameObject.CompareTag("Player"))
        {
            SendHealthUpdate((int)damage, PhaNetworkingAPI.targetIP);
        }
        if (currentHealth <= 0.0f && disableOnDeath)
        {
            gameObject.SetActive(false);
            OnDisable.Invoke();
        }
        if (currentHealth <= 0.0f && destroyOnDeath)
        {
            //Kill(deathDelay);
            OnDeath.Invoke();
        }
    }

    public void Kill(float delay = 0.0f)
    {
        OnDeath.Invoke();
        Destroy(gameObject, delay);
    }

    
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha0) && Input.GetKey(KeyCode.KeypadPlus))
        {
            takeDamage(1000.0f);
        }
    }
}