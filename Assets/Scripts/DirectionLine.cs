using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirectionLine : MonoBehaviour
{
    [SerializeField]
    public int LineNum;
    [SerializeField]
    GameObject Target;
    public List<GameObject> Horses = new List<GameObject>();
    void Start(){
        LineNum = int.Parse(gameObject.name);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag != "Animal") {return;}
        // Debug.Log("enter");
        Horses.Add(other.gameObject);
        float mag = other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        other.gameObject.transform.LookAt(new Vector3(
            Target.transform.position.x,
            other.gameObject.transform.position.y,
            Target.transform.position.z
            ));
        other.gameObject.GetComponent<Rigidbody>().velocity = other.transform.forward*mag;
        object[] param = new object[2];
        param[0] = this.gameObject;
        param[1] = LineNum;
        other.gameObject.SendMessage("SetLane", param);
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag != "Animal") {return;}
        // Debug.Log("exit");
        Horses.Remove(other.gameObject);
    }
}
