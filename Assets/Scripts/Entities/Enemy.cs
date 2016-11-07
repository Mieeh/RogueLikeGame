﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    private float healthPoints, attack;
    private float critChance; // 1 - this, is the actual percentage chance
    private float critMultiplier;
    private PlayerManager playerManager;

    void Start()
    {
        critChance = Random.Range(75, 99);
        critMultiplier = 1f + Random.Range(0f, 10f) / 10;
        critMultiplier = Mathf.Round(critMultiplier * 100f) / 100f;
        playerManager = FindObjectOfType<PlayerManager>(); // Getting the player manager
    }

    public void SetUpEnemy(int _floorNumber)
    {
        healthPoints = Mathf.CeilToInt(BaseValues.EnemyBaseHP * Mathf.Pow(1.106f, _floorNumber)) + Mathf.FloorToInt(Random.Range(-_floorNumber, 5 * _floorNumber));
        attack = Mathf.CeilToInt(BaseValues.EnemyBaseAttack * Mathf.Pow(1.0838f, _floorNumber));
    }

    public float getHP() { return healthPoints; }
                    
    public float getAttack() { return attack; }

    public void looseHealth(float _hp)
    {
        healthPoints -= _hp;
        if (healthPoints <= 0)
        {
            playerManager.enemyDied();
        }
    }
}