using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    public GameObject[] characters;
    public int  bodyCount, animCount;

    private int currentCharacter;
    private CharacterControl[] characterControls;
    private int currentBody, currentAnim;

    // Start is called before the first frame update
    void Start()
    {
        characterControls = new CharacterControl[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            characterControls[i] = characters[i].GetComponent<CharacterControl>();
        }

        SetCharacter(0);
    }
    
    public void NextCharacter()
    {
        if (currentCharacter >= (characters.Length - 1)) SetCharacter(0);
        else SetCharacter(currentCharacter + 1);
    }

    public void SetCharacter(int newCharacter)
    {
        currentCharacter = newCharacter;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == currentCharacter);

            if (i == currentCharacter)
            {
                //characterControls[i].SetBodyMaterial(currentBody);
               // characterControls[i].SetAnim(currentAnim);
            }
        }
    }

    /* public void NextBodyMaterial()
     {
         currentBody++;
         if (currentBody >= bodyCount) currentBody = 0;

         characterControls[currentCharacter].SetBodyMaterial(currentBody);
     }

     public void NextAnim()
     {
         currentAnim++;
         if (currentAnim >= animCount) currentAnim = 0;

         characterControls[currentCharacter].SetAnim(currentAnim);
     }*/
}
