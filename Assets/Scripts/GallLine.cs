using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GallLine : MonoBehaviour
{
    List<string> Horses = new List<string>();
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player" 
        && other.gameObject.tag!= "Animal"){return;}
        if(Horses.IndexOf(other.gameObject.name) == -1){
            Horses.Add(other.gameObject.name);
            Debug.Log($"{other.gameObject.name} + {Horses.Count}착");
        }

        
    }
}
