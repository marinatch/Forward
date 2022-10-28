using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : EnemyControl  //actua simpre como quiere el script padre EnemyControl
{
    private CharacterController control;
    public float gravity, speed;
    public int life;
    public WaveControl manager;

    public Transform target;
    private Vector3 moveDir;

    private float fireTime, fireRate;

    public Animator anim;

    void Start()
    {
        fireRate = 1.5f;
        control = GetComponent<CharacterController>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    void Update()
    {
        // generar un objeto vector 3 con todo y luego modificamos los ejes que queremos
        Vector3 lookPoint = target.position;
        //ahora modificamos la Y
        lookPoint.y = transform.position.y;
        transform.LookAt(lookPoint);

        //if(Distance < 0.5f) //Estoy cerca no me muevo
        //else //Estoy lejos me muevo

        if (Vector3.Distance(target.position, transform.position) < 1.9f)
        {
            moveDir = new Vector3(0, moveDir.y, 0);
            //ANIMACION  //Estoy muy cerca del player, ya no me muevo, pero golpeo al player
            
            anim.SetBool("Walk", false);
            fireTime += Time.deltaTime;
            if(fireTime > fireRate)
            {
                target.GetComponent<PlayerControl>().GetDamage(Random.Range(2, 5));
                anim.SetTrigger("Attack");
                fireTime = 0;
            }
            
           
        }
        else
        {

            //Ir hacia o alejarse del player 
            moveDir = new Vector3(0, moveDir.y, speed);
            moveDir = transform.TransformDirection(moveDir);
            //Estoy lejos del player me muevo hacia el
            anim.SetBool("Walk", true);
        }

        if (control.isGrounded == false)
        {
            moveDir.y -= gravity * Time.deltaTime;
        }
        control.Move(moveDir * Time.deltaTime);
    }

    public override void GetDamage(int _damage) //pasar un int, asi cada golpe puede ser diferente
    {
        //Animacion de recibir daño
        life -= _damage;
        if (life <= 0)
        {
            //Animacion de recibir daño
            //gameObject es para decir al manager que soy yo quien quiere morir
            manager.DestroyEnemy(gameObject);
        }
    }

    public override void SetManager(WaveControl _manager)
    {
        manager = _manager;
    }
}
