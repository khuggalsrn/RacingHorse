using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
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
    [SerializeField] Slider HpBarSlider;
    float MaxHP;
    

    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        MaxHP = GetComponent<PlayerInput>().MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        Max_vel.text = $"Max Velocity : {GetComponent<PlayerInput>().Max_Velocity}";
        Cur_vel.text = $"Your Velocity : {rigid.velocity.magnitude}";
        Cur_Acc.text = $"Cur Acceleration : {GetComponent<PlayerInput>().Cur_Acceleration}";
        float curHP = GetComponent<PlayerInput>().HP;
        HP.text = $"HP : {curHP} / {MaxHP} ";
        CheckHp(curHP);
    }
    public void CheckHp(float curHP) //*HP 갱신
    {
        if (HpBarSlider != null)
            HpBarSlider.value = curHP / MaxHP;
    }
}
