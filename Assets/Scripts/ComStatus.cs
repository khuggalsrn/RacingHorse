using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEngine;

public class ComStatus : MonoBehaviour
{
    public Rigidbody rigid;
    [SerializeField]
    public int Speed, Stamina, Power, Intelligence;
    public Strategy mystr;
    float MaxHP = 0;
    [SerializeField]
    float HP = 0;
    [SerializeField]
    float Basic_Velocity, Max_Velocity = 0;
    float Basic_Acceleration = 0;
    [SerializeField]
    float Cur_Target_Velocity = 0;
    [SerializeField]
    float Cur_Acceleration = 0;
    float GetOverpace_Probability = 0;
    float Additional_Acceleration = 0;
    float Additional_Velocity = 0;
    float correction = 0;
    int is_slipstream = 0;
    [SerializeField]
    float time1 = 0; //기본시간
    [SerializeField]
    float time2 = 0; //시작시간
    [SerializeField]
    float time3 = 0; //기본 레인이동 시간
    [SerializeField]
    float time4 = 0; // 추월시도 시간
    public Situation mysit;
    [SerializeField]
    GameObject CurLane;
    [SerializeField]
    int CurLaneNum = 0;
    bool isspurt = false;
    [SerializeField]
    bool Overtake = false;
    int isOnSkill = 0;
    [SerializeField]
    bool ismovingLane = false;
    void Awake()
    {
        SetVelocity();
        SetAcceleration();
        SetGetOverpace_Probability();
        SetHP();
        StartCoroutine(InStart());//임시코드 
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
        Basic_Acceleration = 5 + Power * 0.005f; // 파워1000이면 가속 초당 5+5, 최대 가속은 보통 100정도일듯
        if (mysit == Situation.spurt) Basic_Acceleration += (Speed * 0.0025f); // 위의 절반치
    }
    void SetGetOverpace_Probability()
    {
        GetOverpace_Probability = 100 - 0.1f * Intelligence; //퍼센트단위, 지능1000이면 0%임
    }
    void SetHP()
    {
        MaxHP = (Stamina + 2000) * 100 * correction;
        HP = MaxHP;
    }

    //추가효과, 스킬 등
    void SetAdditional_Acc(float addiA)
    {
        Additional_Acceleration += addiA;
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
    public IEnumerator InStart()
    { //게이트 오픈 출발시 가속 15추가해줌
        mysit = Situation.early;
        SetAdditional_Acc(15f);
        yield return new WaitForSeconds(3f);
        SetAdditional_Acc(-15f);
    }
    public void InMid()
    {
        mysit = Situation.mid;
        Basic_Velocity = 25 + 0.5f * Intelligence * 0.1f;
        Max_Velocity = Basic_Velocity * 1.1f;
    }
    public void InLast()
    {
        mysit = Situation.last;
        Basic_Velocity *= 1.25f;
        Max_Velocity = Basic_Velocity * 1.5f;
    }
    public void OnSpurtLine()
    {
        mysit = Situation.spurt;
        SetAcceleration();
        Basic_Velocity = 25 + (Intelligence * 0.5f + Speed * 2.5f) * 0.5f * 0.1f * correction;
        Max_Velocity = Basic_Velocity;
        // 지능스탯 1000, 스피드 스탯 1000이면 무보정 최대 150일듯
    }
    Vector3 Torquedir;
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Torquedir += -Vector3.up * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Torquedir += Vector3.up * Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            Torquedir = Vector3.zero;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            Torquedir = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        time1 += 0.02f;
        time2 += 0.02f;
        time4 += 0.02f;
        float rand1 = Random.Range(0.0f, 0.1f);

        int InnerLane = 100;
        int SameLane = 0;
        int OuterLane = 100;

        if (CurLaneNum > 0) {
            SameLane = CurLane.transform.parent.GetChild(CurLaneNum - 1).GetComponent<DirectionLine>().Horses.Count;
            if (CurLaneNum > 1) 
                InnerLane = CurLane.transform.parent.GetChild(CurLaneNum - 2).GetComponent<DirectionLine>().Horses.Count;
            if (CurLaneNum < CurLane.transform.parent.childCount)
                OuterLane = CurLane.transform.parent.GetChild(CurLaneNum).GetComponent<DirectionLine>().Horses.Count;
            
        }
        
        if (InnerLane == 0)
        {
            time3 += 0.02f;
            if (time3 > 0.5f)
            {
                rigid.velocity += transform.right * 25;
                ismovingLane = true;
                Invoke("isnotmoveLane",1f);
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
            if(SameLane > 1){
                if (InnerLane == 0)
                {
                    rigid.velocity += transform.right * 25;
                    ismovingLane = true;
                    Invoke("isnotmoveLane",1f);
                    Debug.Log("추월 안쪽으로");
                }
                else if (OuterLane == 0)
                {
                    rigid.velocity -= transform.right * 25;
                    ismovingLane = true;
                    Invoke("isnotmoveLane",1f);
                    Debug.Log("추월 바깥쪽으로");
                }
                else{
                    // Debug.Log("가로막힘 추월실패");
                }
            }
            else
            {
                
            }
        }
        if (time2 > 0.2f && time2 < 0.24f)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        if (HP > 0)
        {
            Cur_Target_Velocity = Basic_Velocity + Additional_Velocity;
            Cur_Acceleration = Basic_Acceleration + Additional_Acceleration;

            if (time2 > 3f)
            {

                Vector3 projvec = Vector3.Project(rigid.velocity, transform.forward);

                if (projvec.magnitude > Max_Velocity && !isspurt && isOnSkill == 0)
                {
                    HP -= Mathf.Pow(rigid.velocity.magnitude, 2);
                    Debug.Log($" {this.name} , {projvec.magnitude}, 오버페이스");
                }
                else
                {
                    if (is_slipstream == 0)
                    {
                        HP -= Mathf.Pow(rigid.velocity.magnitude, 2) / 50;
                    }
                    else
                    {
                        HP -= Mathf.Pow(rigid.velocity.magnitude, 2) / 50 / 2;
                    }
                }
            }
            if (time2 < 0.2f) return;
            if (rigid.velocity.magnitude < Cur_Target_Velocity)
            {
                rigid.velocity +=
                    new Vector3(
                        transform.forward.x * Cur_Acceleration,
                    transform.forward.y * Cur_Acceleration,
                    transform.forward.z * Cur_Acceleration) / (5 - rand1);
            }
        }
        else
        {
            rigid.velocity +=
                    new Vector3(
                        transform.forward.x * Cur_Acceleration,
                    transform.forward.y * Cur_Acceleration,
                    transform.forward.z * Cur_Acceleration) / (15 - rand1);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name == "InMid") InMid();
        if (other.tag != "Animal") { return; }
        Debug.Log("animal enter");
        if(time4 < 1f) {return;}
        time4 = 0f;
        if (other.gameObject.GetComponent<Rigidbody>().velocity.magnitude < Cur_Target_Velocity && !Overtake)
        {
            StartCoroutine(OverTaking(10f, 3f));
            Debug.Log("추월시도");
        }
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
    void isnotmoveLane(){
        ismovingLane = false;
    }
}
