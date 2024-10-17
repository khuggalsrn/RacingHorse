using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastLine : MonoBehaviour
{
    [SerializeField] GameObject GallLine;
    [SerializeField] GameObject Gate;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player" 
        && other.gameObject.tag!= "Animal"){return;}
        GallLine.gameObject.SetActive(true);
        Gate.gameObject.SetActive(false);

        
    }
}
