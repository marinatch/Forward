using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//los estados de los enemigos, los ponemos en el padre asi lo tendran aceso todos los hijos
public enum EnemyEstate { FOCUS, GUARD, DRAG, TROLL }
public class EnemyControl : MonoBehaviour
{
   

    //una funcion virtual يتم من خلالها استدعاء وظيفة التغيير Override
    //من نص تحكم آخر -- الابن 
    public virtual void GetDamage(int _damage)
    {

    }

    /*una función que sirve para llamar todos los enemies a la vez, de este manera no se repite la llamada varias veces en el juego.
       llamamos la función desde el script Enemy-1*/
    public virtual void SetManager(WaveControl _manager)
    {

    }
}
