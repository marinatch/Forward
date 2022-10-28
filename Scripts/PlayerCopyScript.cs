using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCopyScript : MonoBehaviour
{
    private Rigidbody control;
    private Vector3 moveDir;
    public float speed;
    private Transform findEnemy;

    void Start()
    {
        control = GetComponent<Rigidbody>();
        Destroy(gameObject, 1);

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (allEnemies.Length > 0)
        {
            int randomEnemy = Random.Range(0, allEnemies.Length);
            findEnemy = allEnemies[randomEnemy].transform;
        }
    }

    void Update()
    {
        if (findEnemy != null)
        {
            Vector3 finalPos = findEnemy.position;
            finalPos.y = transform.position.y;
            transform.LookAt(finalPos);
            Vector3 finalRot = transform.eulerAngles;
            finalRot.y -= 90;
            transform.eulerAngles = finalRot;
        }

        moveDir = Vector3.right;
        moveDir = transform.TransformDirection(moveDir);
        moveDir.x *= speed;
        moveDir.y *= 0;
        moveDir.z *= speed;
        control.velocity = moveDir;
    }
}
