﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCubeMaster : Enemy
{
    private float cubeSize = 1f;
    public BoxCollider mainCollider;

    private List<Vector3> cubesPosition = new List<Vector3> {
        new Vector3( 0.5f, 0f, -0.5f),  // LEFT
        new Vector3( -0.5f, 0f, -0.5f), // RIGHT
        new Vector3( 0f, 0f, -1f),      // BACK
        new Vector3( 0f, 0.5f, -0.5f),  // UP
    };

    private List<Vector3> cubesRotation = new List<Vector3> {
        new Vector3(0f,90f,0f),   // LEFT
        new Vector3(0f,90f,0f),   // RIGHT
        new Vector3(0f,0f,0f),    // BACK
        new Vector3(90f,0f,0f),   // UP
    };

    public List<Enemy> enemySquad = new List<Enemy>();

    private void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        soundManager = SoundManager.instance;

    }

    public void DestroyCube()
    {
        //Kwadraty przestają należeć do cube
        foreach(Enemy enemy in enemySquad)
            resetEnemy(enemy);

        enemySquad.Clear();

        mainCollider.center = new Vector3(0f, 0f, 0f);
        mainCollider.size = new Vector3(1f, 1f, 0.05f);
    }

    void setEnemy(Enemy enemy, int index)
    {
        enemy.GetComponent<MoveAround>().enabled = false;
        enemy.GetComponent<BoxCollider>().enabled = false;
        enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        enemy.Unmobilized = true;

        //gameObject is now EnemyCubeMaster child and move with him
        enemy.transform.parent = this.transform;

        //Set rotation to parent rotation
        enemy.transform.rotation = this.transform.rotation;
        //rotate by cubesRotation[index]
        enemy.transform.rotation *= Quaternion.Euler(cubesRotation[index]);
        enemy.transform.localPosition = cubesPosition[index];
    }

    void resetEnemy(Enemy enemy)
    {
        enemy.transform.parent = this.transform.parent;

        enemy.GetComponent<MoveAround>().enabled = true;
        enemy.GetComponent<BoxCollider>().enabled = true;
        enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        enemy.Unmobilized = false;

    } 

    void OnTriggerEnter(Collider other)
    {
        if(health < maxHealth)
        {
            //Disable collider so we stop checking it when EnemyCubeMaster is hurt
            //(EnemyCubeMaster still have second collider)
            this.enabled = false;
            return;
        }

        //We don't take AttackingEnemy or EnemyCubeMaster to cube
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Dotknął mnie!");
            if (enemySquad.Count < 4)
            {
                Enemy otherEnemy = other.GetComponent<Enemy>();
                setEnemy(otherEnemy, enemySquad.Count);
                enemySquad.Add(otherEnemy);
                mainCollider.center = new Vector3(0f, 0f, -.5f);
                mainCollider.size = new Vector3(1.2f, 1f, 1.2f);
            }
        }
    }
}