using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveControl : MonoBehaviour
{
    public List<WaveProperties> waves;
    public Transform player;
    public Transform leftLimit, backLimit, forwardLimit;
    private bool activeWave;

    public List<GameObject> middleEnemies, possibleEnemies;
    public int totalMiddleEnemies;
    private float middlePrint;

    //public GameObject winText;
    public Animator anim;
    public bool youWin;
    public GameObject winMenu;

    public GameObject goPanel;
    private void Start()
    {
        youWin = false;
        //winText.SetActive(false);
        winMenu.SetActive(false);
        middlePrint = ((waves[0].playerX - transform.position.x) / 2) + transform.position.x;
        middleEnemies = new List<GameObject>();
        transform.position = new Vector3(waves[0].posX, transform.position.y, transform.position.z);
        goPanel.SetActive(false);
    }

    private void SwitchGoPanel()
    {
        goPanel.SetActive(!goPanel.activeSelf);
    }

    private void DesactiveGoPanel()
    {
        goPanel.SetActive(false);
        CancelInvoke("SwitchGoPanel");
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            DestroyEnemy(waves[0].enemies[0]);
        }
        //forzar a eliminar los enemigos, despues hay que quitarlo
        if (Input.GetKeyDown(KeyCode.H)) waves[0].enemies.RemoveAt(0);

        if(waves.Count > 0)
        {
            if(activeWave == true)
            {
                if (waves[0].enemies.Count == 0)
                {
                    
                    waves.RemoveAt(0);
                    if (waves.Count > 0)
                    {
                        //He terminado una oleada
                        InvokeRepeating("SwitchGoPanel", 0, 0.2f);
                        Invoke("DesactiveGoPanel", 3);
                        //posicion inicial antes de la oleada
                        middlePrint = ((waves[0].playerX - transform.position.x) / 2) + transform.position.x;
                        middleEnemies = new List<GameObject>();
                        transform.position = new Vector3(waves[0].posX, transform.position.y, transform.position.z);
                        activeWave = false;
                    }
                }

            }
            else
            {
                if (player.position.x > waves[0].playerX)
                {
                    print("El player ya ha llegado");
                    activeWave = true;
                    PrintEnemies();
                    //Imprimir enemigos

                }
                else
                {
                    //print(middlePrint +"  "+ player.position.x + "  "+ middleEnemies.Count);
                    if(middleEnemies.Count == 0 && player.position.x > middlePrint)
                    {
                        for (int i = 0; i < totalMiddleEnemies; i++)
                        {
                            int randomEnemy = Random.Range(0, possibleEnemies.Count);
                            //donde imprimir, aloteriamente entre left = 0 y Right = 1
                            int randomLimit = Random.Range(0, 2); //hay que escribir entre 0 y 2 porque unity resta el ultimo numero del Range
                                                                  //transform de int a bool. //el random limit es true = Right  // false = Left
                            bool limit = randomLimit == 1;
                            //para saber exactamente en que valor. Back y Forward. // el Back es menor al forward
                            float randomZ = Random.Range(backLimit.position.z, forwardLimit.position.z);
                            // para tenerlo todo controlado es mejor añadir el Y tambien. por se caso que el esenario no sería en (0,0,0)
                            //una posicion es un Vector3 porque necesita ejes x,y,z
                            Vector3 finalPos = new Vector3(limit ? transform.position.x : leftLimit.position.x, 1, randomZ); //si es true que quieres que te devuelva : y si es falso que

                            //identity mantiene la rotacion del original
                            GameObject newEnemy = Instantiate(possibleEnemies[randomEnemy], finalPos, Quaternion.identity);

                            //llamar a la funcion virtual SetManager en el script EnemyControl  //this refiere al este script mismo
                            newEnemy.GetComponent<EnemyControl>().SetManager(this);

                            middleEnemies.Add(newEnemy);

                        }
                    }
                    //Todavia no ha llegado
                }
            }

        }
        else
        {
            print("Fin de la partida");
            YouWin();
        }
        
    }

    private void PrintEnemies()
    {
        for (int i = 0; i < waves[0].enemies.Count; i++)
        {
            //donde imprimir, aloteriamente entre left = 0 y Right = 1
            int randomLimit = Random.Range(0, 2); //hay que escribir entre 0 y 2 porque unity resta el ultimo numero del Range
            //transform de int a bool. //el random limit es true = Right  // false = Left
            bool limit = randomLimit == 1;
            //para saber exactamente en que valor. Back y Forward. // el Back es menor al forward
            float randomZ = Random.Range(backLimit.position.z, forwardLimit.position.z);
            // para tenerlo todo controlado es mejor añadir el Y tambien. por se caso que el esenario no sería en (0,0,0)
            //una posicion es un Vector3 porque necesita ejes x,y,z
            Vector3 finalPos = new Vector3(limit ? transform.position.x : leftLimit.position.x, 1, randomZ); //si es true que quieres que te devuelva : y si es falso que

            //identity mantiene la rotacion del original
            waves[0].enemies[i] = Instantiate(waves[0].enemies[i], finalPos, Quaternion.identity);

            //llamar a la funcion virtual SetManager en el script EnemyControl  //this refiere al este script mismo
            waves[0].enemies[i].GetComponent<EnemyControl>().SetManager(this);
        }
    }

    public void DestroyEnemy(GameObject _enemy)
    {
        for (int i = 0; i < waves[0].enemies.Count; i++)
        {
            if(waves[0].enemies[i] == _enemy)
            {
                waves[0].enemies.RemoveAt(i);
                Destroy(_enemy);
                return;
            }
        }
        Destroy(_enemy);
    }

    public void YouWin()
    {
        youWin = true;
        //winText.SetActive(true);
        winMenu.SetActive(true);
        anim.SetBool("Dance", true);
    }
}
[System.Serializable]
public class WaveProperties
{
    public List<GameObject> enemies;
    public float posX, playerX;
}
