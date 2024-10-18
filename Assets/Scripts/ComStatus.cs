using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEngine;

public class ComStatus : MonoBehaviour
{
    Animator anim;
    /// <summary> physics body </summary>
    public Rigidbody rigid;
    /// <summary> Status </summary>
    [SerializeField]
    public int Speed, Stamina, Power, Intelligence;
    /// <summary> Racing strategy </summary>
    public Strategy mystr;
    /// <summary> First Hp = MaxHP </summary>
    float MaxHP = 0;
    /// <summary> CurHP </summary>
    [SerializeField]
    float HP = 0;
    /// <summary> Velocity will be related Speed & Intelligence </summary>
    [SerializeField]
    float Basic_Velocity, Max_Velocity = 0;
    [SerializeField]
    /// <summary> Acceleration will be related Power </summary>
    float Basic_Acceleration = 0;
    /// <summary> CurTarget = Basic + Additional </summary>
    [SerializeField]
    float Cur_Target_Velocity = 0;
    /// <summary> CurTarget = Basic + Additional </summary>
    [SerializeField]
    float Cur_Acceleration = 0;
    /// <summary> Overpace probability will be related Intelligence </summary>
    float GetOverpace_Probability = 0;
    /// <summary> OverpaceCorrection, Stamina is consumed in proportion to this. </summary>
    float OverpaceCorrection = 1;
    /// <summary> Additinal something </summary>
    [SerializeField]
    float Additional_Acceleration = 0;
    /// <summary> Additinal something </summary>
    [SerializeField]
    float Additional_Velocity = 0;
    /// <summary> Correction value by strategy </summary>
    float correction = 0;
    /// <summary> when this is on slipstream, HP will be less reduced</summary>
    bool is_slipstream = false;
    /// <summary> Basic timer </summary>
    [SerializeField]
    float time1 = 0;
    /// <summary> start after timer </summary> ///
    [SerializeField]
    float time2 = 0;
    /// <summary> moving lane timer </summary>
    [SerializeField]
    float time3 = 0; //기본 레인이동 시간
    /// <summary> Overtaking timer </summary>
    [SerializeField]
    float time4 = 0; // 추월시도 시간
    /// <summary> slipstream timer </summary>
    [SerializeField]
    float time5 = 0;
    /// <summary> Situation during the race </summary>
    public Situation mysit;
    /// <summary> Current Lane on race </summary>
    [SerializeField]
    GameObject CurLane;
    /// <summary> Current Lane number on race </summary>
    [SerializeField]
    int CurLaneNum = 0;
    /// <summary> is this overtaking now</summary>
    [SerializeField]
    bool Overtake = false;
    /// <summary> is this overpaced </summary>
    [SerializeField]
    bool Overpace = false;
    /// <summary> num of -> is this on skill now </summary>
    int isOnSkill = 0;
    /// <summary> is this moving on lanes now </summary>
    [SerializeField]
    bool ismovingLane = false;
    /// <summary> Other Horses number of inner lane</summary>
    int InnerLane = 100;
    /// <summary> Other Horses number of same lane</summary>
    int SameLane = 0;
    /// <summary> Other Horses number of outer lane</summary>
    int OuterLane = 100;
    void Awake()
    {
        SetVelocity();
        SetAcceleration();
        SetGetOverpace_Probability();
        SetHP();
        Invoke("InStart", 3f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid.velocity = Vector3.zero;
    }
    //기본세팅
    public void SetLane(object[] param)
    {
        CurLane = (GameObject)param[0];
        CurLaneNum = (int)param[1];
    }
    public void allocLane_null()
    {
        CurLane = null;
        CurLaneNum = 0;
    }
    void SetVelocity()
    {
        switch (mystr)
        {
            case Strategy.Pacemaker:
                correction = 0.9f;
                break;
            case Strategy.Runner:
                correction = 1;
                break;
            case Strategy.Stalker:
                correction = 1.1f;
                break;
            case Strategy.Closer:
                correction = 1.15f;
                break;
            default:
                correction = 1;
                break;
        }
        Basic_Velocity = 25 + 0.5f * Intelligence * 0.1f * (1 / correction);
        //지능 스탯 1000이면 무보정 최대 75일듯 75의 보정을받아서 도주는 
        Max_Velocity = Basic_Velocity * 1.1f;
    }
    void SetAcceleration()
    {
        Basic_Acceleration = 10 + Power * 0.005f; // 파워1000이면 가속 초당 10+5, 최대 가속은 보통 15정도일듯
        if (mysit == Situation.spurt) Basic_Acceleration += (Speed * 0.0025f); // 위의 절반치
    }
    void CheckOverpace()
    {
        if (Overpace) return;
        int rand_Overpace = Random.Range(1, 101); // 1, 2, 3, ..., 100
        if (GetOverpace_Probability >= rand_Overpace)
        {
            StartCoroutine(SetAdditional_Acc(Basic_Acceleration / 10, 9999f));
            OverpaceCorrection += 0.2f;
            Overpace = true;
        }
    }
    void UndoOverpace()
    {
        if (!Overpace) return;
        OverpaceCorrection -= 0.2f;
        Overpace = false;
    }
    void SetGetOverpace_Probability()
    {
        GetOverpace_Probability = 100 - 0.1f * Intelligence; //퍼센트단위, 지능1000이면 0%임
    }
    void SetHP()
    {
        MaxHP = (Stamina + 2000) * 200 * correction;
        HP = MaxHP;
    }

    //추가효과, 스킬 등
    IEnumerator SetAdditional_Acc(float addiA, float time)
    {
        Additional_Acceleration += addiA;
        yield return new WaitForSeconds(time);
        Additional_Acceleration -= addiA;
    }
    IEnumerator SetAdditional_Vel(float addiV, float time)
    {
        Additional_Velocity += addiV;
        isOnSkill++;
        yield return new WaitForSeconds(time);
        isOnSkill--;
        Additional_Velocity -= addiV;
    }
    void SetAdditional_HP_per(float addiHPper)
    {
        HP += MaxHP * addiHPper;
    }
    public void InStart()
    { //게이트 오픈 출발시 가속 15추가해줌
        mysit = Situation.early;
        StartCoroutine(SetAdditional_Acc(15f, 3f));
        CheckOverpace();
    }
    public void InMid()
    {
        mysit = Situation.mid;
        Basic_Velocity = 25 + 0.5f * Intelligence * 0.1f;
        Max_Velocity = Basic_Velocity * 1.1f;
        CheckOverpace();
    }
    public void InLast()
    {
        mysit = Situation.last;
        Basic_Velocity = 25 + 0.5f * Intelligence * 0.1f * 1.25f;
        Max_Velocity = Basic_Velocity * 1.5f;
    }
    public void OnSpurtLine()
    {
        mysit = Situation.spurt;
        SetAcceleration();
        UndoOverpace();
        Basic_Velocity = (Intelligence * 0.5f + Speed * 2.5f) * 0.5f * 0.1f * Mathf.Pow(correction, 2);
        Max_Velocity = Basic_Velocity * 1.25f;
        StartCoroutine(SetAdditional_Acc(10, 9999f));
        // 지능스탯 1000, 스피드 스탯 1000이면 무보정 최대 150일듯
    }
    void Al_Lane_Check()
    {
        if (CurLaneNum > 0)
        {
            SameLane = CurLane.transform.parent.GetChild(CurLaneNum - 1).GetComponent<DirectionLine>().Horses.Count;
            if (CurLaneNum > 1)
                InnerLane = CurLane.transform.parent.GetChild(CurLaneNum - 2).GetComponent<DirectionLine>().Horses.Count;
            else
                InnerLane = 100;
            if (CurLaneNum < CurLane.transform.parent.childCount)
                OuterLane = CurLane.transform.parent.GetChild(CurLaneNum).GetComponent<DirectionLine>().Horses.Count;
            else
                OuterLane = 100;
        }
    }
    void Al_Lane_Move()
    {
        if (InnerLane == 0)
        {
            time3 += 0.02f;
            if (time3 > 0.5f)
            {
                rigid.velocity += transform.right * 25;
                ismovingLane = true;
                Invoke("isnotmoveLane", 1f);
                Debug.Log("turn right");
                time3 -= 0.1f;
            }
        }
        else
        {
            time3 = 0;
        }

        if (Overtake && !ismovingLane)
        {
            if (SameLane > 1)
            {
                if (InnerLane == 0)
                {
                    rigid.velocity += transform.right * 25;
                    ismovingLane = true;
                    Invoke("isnotmoveLane", 0.25f);
                    Debug.Log($" {this.name} 추월 안쪽으로");
                }
                else if (OuterLane == 0)
                {
                    rigid.velocity -= transform.right * 25;
                    ismovingLane = true;
                    Invoke("isnotmoveLane", 0.25f);
                    Debug.Log($" {this.name} 추월 바깥쪽으로");
                }
                else
                {
                    // Debug.Log("가로막힘 추월실패");
                }
            }
            else
            {

            }
        }
    }
    void HP_Consumption()
    {
        Vector3 projvec = Vector3.Project(rigid.velocity, transform.forward);

        if (projvec.magnitude > Max_Velocity && mysit != Situation.spurt && isOnSkill == 0)
        {
            HP -= Mathf.Pow(rigid.velocity.magnitude, 2);
            Debug.Log($" {this.name} , {projvec.magnitude}, 오버페이스");
        }
        else
        {
            if (time5 < 2f) // 슬립스트림 2초지속
            {
                HP -= Mathf.Pow(rigid.velocity.magnitude, 2) / 50 * OverpaceCorrection;
            }
            else
            {
                HP -= Mathf.Pow(rigid.velocity.magnitude, 2) / 50 / 2 * OverpaceCorrection;
            }
        }
    }
    private void FixedUpdate()
    {
        time1 += 0.02f;
        if (time1 > 3f)
            time2 += 0.02f;
        if (time5 > 2f) // 앞에 2초간 무언가 뒤 에 있었으면
            time4 += 0.02f; // 추월시도 타이머
        else
            time4 = 0;
        if (is_slipstream)
            time5 += 0.02f;
        float velocityRand1 = Random.Range(0.0f, 0.1f);

        Al_Lane_Check();


        Vector3 Speed_R_value = Vector3.zero;

        if (time2 > 0)
        {
            Al_Lane_Move();
            if (HP > 0)
            {
                Cur_Target_Velocity = (Basic_Velocity + Additional_Velocity + Max_Velocity) / 2;
                Cur_Acceleration = Basic_Acceleration + Additional_Acceleration;
                Speed_R_value = new Vector3(
                        transform.forward.x * Cur_Acceleration,
                        transform.forward.y * Cur_Acceleration,
                        transform.forward.z * Cur_Acceleration);
                if (time2 > 3f) // 출발 후 3초 후부터 체력소모
                {
                    HP_Consumption();
                }
                if (rigid.velocity.magnitude < Cur_Target_Velocity)
                {
                    rigid.velocity += Speed_R_value / (5 - velocityRand1);
                }
            }
            else
            {
                rigid.velocity += Speed_R_value / (15 - velocityRand1);
                gameObject.layer = LayerMask.NameToLayer("HP0Horse");
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "InMid") InMid();
        if (other.name == "Inlast") InLast();
        if (other.name == "Onspurt") OnSpurtLine();
        if (other.name == "gall")
        {
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Animal" && other.tag != "Player") { return; }

        is_slipstream = true;
        if (time4 > 0.5f && other.gameObject.GetComponent<Rigidbody>().velocity.magnitude < Cur_Target_Velocity && !Overtake)
        {
            StartCoroutine(OverTaking(10f, 3f));
            Debug.Log("추월시도");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Animal" || other.tag != "Player") { return; }
        is_slipstream = false;
        time3 = 0;
    }
    IEnumerator OverTaking(float addiV, float time)
    {
        Overtake = true;
        Additional_Velocity += addiV;
        isOnSkill++;
        yield return new WaitForSeconds(time);
        isOnSkill--;
        Additional_Velocity -= addiV;
        Overtake = false;
    }
    void isnotmoveLane()
    {
        ismovingLane = false;
    }
}
