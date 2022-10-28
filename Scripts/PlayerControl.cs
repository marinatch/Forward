using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerControl : MonoBehaviour
{
    public enum PlayerState { NORMAL, THROW}
    public PlayerState state;

    private CharacterController control;
    public float speed, jumpSpeed, gravity, rotationSpeed;
    private Vector3 moveDir;

    public Transform leftLimit, rightLimit, forwardLimit, backLimit;
    public Transform punchPos, kickPos;
    public float punchDistance, kickDistance;//la distancia de attaque

    //crear un layer mask para decir al Player que items puede coger
    public LayerMask recollectable, attackMask;
    public int life;

    private Quaternion currentRotation; //para rotar de forma mas suave
    private bool hasAttack;
    private RaycastHit hitAttack;

    public float paddingLeft, rightDistance;

    public Animator anim;
    public Image lifeBar;
    public GameObject gameOverMenu;
    public bool youLose;

    public GameObject throwObject;
    public Transform throwPoint;

    public AudioSource hitSound;
    public AudioSource hammerSound;

    public GameObject prefabPlayerCopy;
    private GameObject currentPlayerCopy;


    void Start()
    {
        //coge el componente del Player
        control = GetComponent<CharacterController>();

        gameOverMenu.SetActive(false);
        youLose = false;
    }

   
    void Update()
    {
        switch (state)
        {
            case PlayerState.NORMAL:
                Movement();
                Recollectable();
                AttackSystem();
                break;
            case PlayerState.THROW:
                moveDir.y -= gravity * Time.deltaTime;
                control.Move(moveDir * Time.deltaTime);

                if(control.isGrounded)
                {
                    state = PlayerState.NORMAL;
                }
                break;
        }
        
        CheckLimits();
    }

    private void CheckLimits()
    {
        if (Vector3.Distance(leftLimit.position, rightLimit.position) > rightDistance)
        {
            if ((transform.position.x - paddingLeft) > leftLimit.position.x)
            {
                leftLimit.position = new Vector3((transform.position.x - paddingLeft), leftLimit.position.y, leftLimit.position.z);
            }
        }
    }
    private void Movement()
    {
        anim.SetBool("Grounded", control.isGrounded);
        anim.SetBool("Move", (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) != 0);
        //si el controlador esta tocando el suelo
        if (control.isGrounded)
        {
            moveDir = new Vector3(Input.GetAxis("Horizontal"), moveDir.y, Input.GetAxis("Vertical"));
            moveDir.x *= speed;
            moveDir.z *= speed;

            //girar solo cuando estas pulsando las teclas de movimiento
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
              //calcular el angulo para saber a donde hay que girar
              float angle = Mathf.Atan2(moveDir.z, moveDir.x) * Mathf.Rad2Deg; //Rad2Deg es para cambiar de radian a Degrease
                currentRotation = Quaternion.AngleAxis(angle, Vector3.down);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, currentRotation, rotationSpeed);

            //Saltar
            if (Input.GetButtonDown("Jump"))
            {
                moveDir.y = jumpSpeed;
            }
        }
        else //si no estoy tocando el suelo = estoy en el aire = tengo que caerme.
        {
            moveDir.y -= gravity * Time.deltaTime;
        }

        control.Move(moveDir * Time.deltaTime);

        //Control de limites
        //Limit Left
        if (transform.position.x < leftLimit.position.x)
        {
            transform.position = new Vector3(leftLimit.position.x, transform.position.y, transform.position.z);
        }
        //Limit Right
        if (transform.position.x > rightLimit.position.x)
        {
            transform.position = new Vector3(rightLimit.position.x, transform.position.y, transform.position.z);
        }
        //Limit forward
        if (transform.position.z > forwardLimit.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, forwardLimit.position.z);
        }
        //Limit Back
        if (transform.position.z < backLimit.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, backLimit.position.z);
        }
    }

    private void Recollectable()
    {
        if(Input.GetKeyDown(KeyCode.Q) && throwObject != null)
        {
            throwObject.transform.position = throwPoint.position;
            throwObject.transform.rotation = throwPoint.rotation;

            throwObject.SetActive(true);
            throwObject.GetComponent<Rigidbody>().AddForce(throwPoint.forward * 300);
            Destroy(throwObject, 5);
            throwObject = null;
        }
        if(Input.GetButtonDown("Recollectable"))
        {
            Vector3 finalPos = transform.position;
            finalPos.y -= 1.5f; //es mi posicion menos 0.5 en Y, para bajar 
            Collider[] allObjects = Physics.OverlapSphere(finalPos, 1, recollectable); //el 1, es el radius de la sfera

            // si encuentras algun objeto
            if (allObjects.Length > 0)
            {
                
                if(allObjects[0].tag == "Food")
                {
                    //sume la cantidad de vida que esta specificada en el script FoodControl
                    life += allObjects[0].GetComponent<FoodControl>().life;
                    lifeBar.fillAmount = (float)life / 100;
                    if (life > 100) life = 100;
                    //destruye el primer objeto que recoge, de esta manera no recoge dos cosas o mas a la vez
                    Destroy(allObjects[0].gameObject);
                }
                else if(allObjects[0].tag == "Throw")
                {
                    if(throwObject == null)
                    {
                        throwObject = allObjects[0].gameObject;
                        throwObject.SetActive(false);
                    }
                }
                
            }
        }
    }

    private void AttackSystem()
    {
        if(hasAttack == false)
        {

            if(Input.GetKeyDown(KeyCode.F) && currentPlayerCopy == null)
            {
                currentPlayerCopy = Instantiate(prefabPlayerCopy, transform.position, transform.rotation);
            }

            //hacer visible el rayo del attaque, simplemente para confirmar
            //Debug.DrawRay(punchPos.position, punchPos.right, Color.red, punchDistance);
            //Golpe por arriba
            if(Input.GetButtonDown("Punch"))
            {
                //print("Puño");
                anim.SetTrigger("Punch");
                hasAttack = true;
                //activar animacion de attaque
                Invoke("ResetAttack", 0.5f); //tiene que esperar 1.5 segundos para hacer otro ataque
                //emitir un ray desde el jugador, y si encuentra algo en su camino lo destroye o hacerle daño
                if(Physics.Raycast(punchPos.position, punchPos.right, out hitAttack, punchDistance, attackMask))
                {
                    //dame el nombre con lo que has golpeado
                    print(hitAttack.collider.name);
                    //lleve l ainformacion del objeto con lo que hemos chocado al AttackDetect para getionar lo que hay que hacer
                    AttackDetect(hitAttack.collider.gameObject, Random.Range(10, 22));
                }
            }
            //Golpe por abajo
            if (Input.GetButtonDown("Kick"))
            {
                // print("Patada");
                anim.SetTrigger("Kick");
                hasAttack = true;
                //activar animacion de attaque
                Invoke("ResetAttack", 1.5f);
                if (Physics.Raycast(kickPos.position, kickPos.right, out hitAttack, kickDistance, attackMask))
                {
                    //dame el nombre con lo que has golpeado
                    print(hitAttack.collider.name);
                    //lleve l ainformacion del objeto con lo que hemos chocado al AttackDetect para getionar lo que hay que hacer
                    AttackDetect(hitAttack.collider.gameObject, Random.Range(15, 35));
                }
            }
        }
    }
    //cuando acaba el attaque resetea la booleana de hasAttack
    public void ResetAttack()
    {
        hasAttack = false;
    }

    //gestionar que tiene que hacer la golpe ( atacar o detruir)
    private void AttackDetect(GameObject _obj, int _damage = 0)
    {
        if(_obj.tag == "Loot")
        {
            //conectar el player con el script del LootControl
            _obj.GetComponent<LootControl>().OpenLoot();
            hammerSound.Play();
        }
        if(_obj.tag == "Enemy")
        {
            _obj.GetComponent<EnemyControl>().GetDamage(_damage);
            hitSound.Play();
        }
    }

    public void GetDamage(int _damage) //pasar un int, asi cada golpe puede ser diferente
    {
        //Animacion de recibir daño
        life -= _damage;
        lifeBar.fillAmount = (float)life / 100;
        if (life <= 0)
        {
            //Game Over, NO DESTRUIR AL PLAYER
            print("Has muerto");
            anim.SetBool("Lose", true);
            gameOverMenu.SetActive(true);
            youLose = true;
        }
    }

    public void ThrowPlayer(Vector3 _shootDir)
    {
        state = PlayerState.THROW;
        this.enabled = true;
        Vector3 dir = _shootDir;
        dir.Normalize();
        moveDir = _shootDir * 8;

        GetDamage(15);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyBullet")
        {
            Destroy(other.gameObject);
            GetDamage(Random.Range(1, 5));
        }
    }

}
