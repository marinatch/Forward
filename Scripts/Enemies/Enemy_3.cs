using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : EnemyControl
{
    private CharacterController control;
    public float gravity, speed;
    public int life;
    public WaveControl manager;

    public Transform target;
    private Vector3 moveDir;

    public EnemyEstate state;
    private float stateTime, stateRate; //contador, cuanto tiempo quiero estar en el estado
    private float fireTime, fireRate;
    private Vector3 guardPos;

    public GameObject bullet;
    public Transform shootPoint;
    public float shootForce;

    public Animator anim;

    //Parpadello
    //public Renderer damageRender;

    //Recoger objetos y lanzarlos

    void Start()
    {
        control = GetComponent<CharacterController>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        stateRate = Random.Range(5, 15);
        fireRate = Random.Range(2, 4);

        float posX = Random.Range(-2f, 2f);
        float posZ = Random.Range(-2f, 2f);
        guardPos = target.position; //lo actualizo como el de player, y despues solamente cambio el x y z
        guardPos.x += posX;
        guardPos.z += posZ;
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
                print(Vector3.Distance(target.position, transform.position));
                if (Vector3.Distance(target.position, transform.position) < 2f)
                {
                    //Estoy cerca ya no me muevo
                    anim.SetTrigger("Attack 0");
                    moveDir = new Vector3(0, moveDir.y, 0);
                    fireTime += Time.deltaTime;
                    if(fireTime >= 1f)
                    {
                        //estoy cogiendo al player
                        anim.SetTrigger("Troll 0");
                        //parar el funcionamiento del player
                        target.GetComponent<PlayerControl>().enabled = false;
                        target.transform.position = shootPoint.position;
                        Vector3 finalPos = transform.position;
                        finalPos.y = shootPoint.position.y;
                        target.LookAt(finalPos);
                        Vector3 finalRot = target.eulerAngles;
                        finalRot.y -= 90;
                        target.eulerAngles = finalRot;

                        state = EnemyEstate.DRAG;
                        fireTime = 0;
                    }
                }
                else
                {
                    //Estoy lejos del player me muevo
                    anim.SetTrigger("Run 0");
                    //Ir hacia o alejarse del player 
                    moveDir = new Vector3(0, moveDir.y, speed);
                    moveDir = transform.TransformDirection(moveDir);
                }
                //El contador de estado
                stateTime += Time.deltaTime;
                if (stateTime >= stateRate)
                {
                    //Conseguir posiciones alejadas del player (la posición donde tiene que ir el enemigo cuando entra el estado GUARD)
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
                anim.SetTrigger("Run 0");
                if (Vector3.Distance(guardPos, transform.position) < 0.5f)
                {
                    //Estoy cerca del punto, me paro
                    anim.SetBool("Idle", true);
                    moveDir = new Vector3(0, moveDir.y, 0);
                }
                else
                {
                    //Estoy lejos del punto, me muevo
                    anim.SetTrigger("Run 0");
                    Vector3 newDir = guardPos - transform.position;
                    //normalizar el Vector3 para tener la misma velocidad siempre, sin que importe la distancia
                    newDir.Normalize();
                    moveDir = new Vector3(newDir.x, moveDir.y, newDir.z);
                    moveDir.x *= speed;
                    moveDir.z *= speed;
                    //si has llegado coges otro punto diferente
                    if(Vector3.Distance(transform.position, guardPos) < 0.5f)
                    {
                        float posX = Random.Range(-2f, 2f);
                        float posZ = Random.Range(-2f, 2f);
                        guardPos = target.position; //lo actualizo como el de player, y despues solamente cambio el x y z
                        guardPos.x += posX;
                        guardPos.z += posZ;
                    }
                }

                //Lanzar objetos en parabola
                //Contamos el tiempo
                //si el tiempo es mayor que x
                //lanzamos un objeto
                //Reiniciamos el contador
                fireTime += Time.deltaTime;
                if(fireTime>= fireRate)
                {
                    //Disparo
                    GameObject newBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
                    newBullet.GetComponent<Rigidbody>().AddForce(shootPoint.forward*shootForce);
                    Destroy(newBullet, 3);
                    fireTime = 0;
                }
                


                //El contador de estado
                stateTime += Time.deltaTime;
                if (stateTime >= stateRate)
                {
                    state = EnemyEstate.FOCUS;
                    stateTime = 0;
                }
                break;

            case EnemyEstate.DRAG:
                fireTime += Time.deltaTime;
                if(fireTime >= 1.5f)
                {
                    //lanzo al player
                    anim.SetTrigger("Attack 0");
                    target.GetComponent<PlayerControl>().ThrowPlayer(shootPoint.forward);
                    fireTime = 0;
                    float posX = Random.Range(-2f, 2f);
                    float posZ = Random.Range(-2f, 2f);
                    guardPos = target.position; //lo actualizo como el de player, y despues solamente cambio el x y z
                    guardPos.x += posX;
                    guardPos.z += posZ;
                    state = EnemyEstate.GUARD;
                }
                break;

            case EnemyEstate.TROLL:
                //animacion  troll
                anim.SetTrigger("Troll 0");
                fireTime += Time.deltaTime;
                if (fireTime >= 2f)
                {
                    fireTime = 0;
                    state = EnemyEstate.GUARD;
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
        /*CancelAnimDamage();
        CancelInvoke("CancelAnimDamage");
        InvokeRepeating("AnimDamage", 0, 0.1f);
        Invoke("CancelAnimDamage", 3);*/

        //animacion de daño
        life -= _damage;
        if (life <= 0)
        { 
            //animacion de muerte
            //gameObject es para decir al manager que soy yo quien quiere morir
            manager.DestroyEnemy(gameObject);
        }
    }

   /* private void AnimDamage()
    {
        //me activo y desactivo al contrareo de lo que estoy
        damageRender.enabled = !damageRender.enabled;
    }

    private void CancelAnimDamage()
    {
        CancelInvoke("AnimDamage");
    }*/
    public override void SetManager(WaveControl _manager)
    {
        manager = _manager;
    }
}
