using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GoalLine : MonoBehaviour
{
    List<string> Horses = new List<string>();
    [SerializeField] Text One;
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.tag != "Player" 
        && other.gameObject.tag!= "Animal"){return;}
        if(Horses.IndexOf(other.gameObject.name) == -1){
            Horses.Add(other.gameObject.name);
            Debug.Log($"{other.gameObject.name} + {Horses.Count}착");
        }
        if(other.gameObject.tag == "Player" )
        {
            One.text = $"당신은 {Horses.Count}착";
        }
        
    }
}
