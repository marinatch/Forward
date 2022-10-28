using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : EnemyControl 
{
    private CharacterController control;
    public float gravity, speed;
    public int life;
    public WaveControl manager;
    public EnemyEstate state;

    public Transform target;
    private Vector3 moveDir;
    private float stateTime, stateRate; //contador, cuanto tiempo quiero estar en el estado
    private Vector3 guardPos;

    public Animator anim;
    void Start()
    {
        control = GetComponent<CharacterController>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        stateRate = Random.Range(2, 5);
    }


    void Update()
    {
        //mirar al player
        // generar un objeto vector 3 con todo y luego modificamos los ejes que queremos
        Vector3 lookPoint = target.position;
        //ahora modificamos la Y
        lookPoint.y = transform.position.y;
        transform.LookAt(lookPoint);

        switch (state)
        {
            case EnemyEstate.FOCUS:

                if (Vector3.Distance(target.position, transform.position) < 1.9f)
                {
                    moveDir = new Vector3(0, moveDir.y, 0);
                    //Estoy muy cerca del player, ya no me muevo, pero golpeo al player
                    anim.SetBool("Walk", false);
                    anim.SetTrigger("Attack");

                    target.GetComponent<PlayerControl>().GetDamage(Random.Range(2, 5));
                    float posX = Random.Range(-2f, 2f);
                    float posZ = Random.Range(-2f, 2f);
                    guardPos = target.position; //lo actualizo como el de player, y despues solamente cambio el x y z
                    guardPos.x += posX;
                    guardPos.z += posZ;

                    state = EnemyEstate.GUARD;
                    stateTime = 0;

                }
                else
                {

                    //Ir hacia o alejarse del player 
                    moveDir = new Vector3(0, moveDir.y, speed);
                    moveDir = transform.TransformDirection(moveDir);
                    //Estoy lejos del player me muevo hacia el
                    anim.SetBool("Walk", true);
                }
                //El contador de estado
                stateTime += Time.deltaTime;
                if(stateTime >= stateRate)
                {
                    //la posición donde tiene que ir el enemigo cuando entra el estado GUARD
                    float posX = Random.Range(-2f, 2f);
                    float posZ = Random.Range(-2f, 2f);
                    guardPos = target.position; //lo actualizo como el de player, y despues solamente cambio el x y z
                    guardPos.x += posX;
                    guardPos.z += posZ;
                    
                    state = EnemyEstate.GUARD;
                    stateTime = 0;  
                }
                break;
            case EnemyEstate.GUARD:
                // Alejarse del player 
                if(Vector3.Distance(guardPos, transform.position) < 0.1f)
                {
                    moveDir = new Vector3(0, moveDir.y, 0);
                    //Estoy muy cerca del punto de Guardia, me paro
                    anim.SetBool("Walk", false);
                }
                else 
                {
                    Vector3 newDir = guardPos - transform.position;
                    //normalizar el Vector3 para tener la misma velocidad siempre, sin que importe la distancia
                     newDir.Normalize();
                     moveDir = new Vector3(newDir.x, moveDir.y, newDir.z);
                     moveDir.x *= speed;
                     moveDir.z *= speed;
                    //Estoy muy lejos del punto de Guardia, me muevo
                    anim.SetBool("Walk", true);
                }
                //El contador de estado
                stateTime += Time.deltaTime;
                if (stateTime >= stateRate)
                {
                    state = EnemyEstate.FOCUS;
                    stateTime = 0;
                }
                break;
        }
        
        //gravedad
        if (control.isGrounded == false)
        {
            moveDir.y -= gravity * Time.deltaTime;
        }

        //movimiento general
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
