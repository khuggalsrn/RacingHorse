using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    public int Speed, Stamina, Power, Intelligence;
    Strategy mystr;
    [SerializeField] Text Max_vel;
    [SerializeField] Text Cur_vel;
    [SerializeField] Text Cur_Acc;
    [SerializeField] Text HP;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Max_vel.text = $"Max Velocity : {GetComponent<PlayerInput>().Max_Velocity}";
        Cur_vel.text = $"Your Velocity : {rigid.velocity.magnitude}";
        Cur_Acc.text = $"Cur Acceleration : {GetComponent<PlayerInput>().Cur_Acceleration}";
        HP.text = $"HP : {GetComponent<PlayerInput>().HP} / {GetComponent<PlayerInput>().MaxHP} ";
    }
}
