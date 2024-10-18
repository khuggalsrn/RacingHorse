using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    Animator anim;
    /// <summary> physics body </summary> ///
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
    float Additional_Acceleration = 0;
    /// <summary> Additinal something </summary>
    float Additional_Velocity = 0;
    /// <summary> Correction value by strategy </summary>
    float correction = 0;
    [SerializeField]
    /// <summary> when this is on slipstream, HP will be less reduced</summary>
    bool is_slipstream = false;
    /// <summary> Basic timer </summary>
    [SerializeField]
    float time1 = 0;
    /// <summary> start after timer </summary>
    [SerializeField]
    float time2 = 0;
    /// <summary> slipstream timer </summary>
    [SerializeField]
    float time3 = 0;
    /// <summary> Situation during the race </summary>
    public Situation mysit;
    /// <summary> Current Lane on race </summary>
    [SerializeField]
    GameObject CurLane;
    /// <summary> Current Lane number on race </summary>
    [SerializeField]
    int CurLaneNum = 0;
    /// <summary> is this on spurt now </summary>
    bool isspurt = false;
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
    /// <summary> rotate Direction Vector </summary>
    Vector3 Torquedir;
    void Awake()
    {
        SetVelocity();
        SetAcceleration();
        SetGetOverpace_Probability();
        SetHP();
        Invoke("InStart", 3f);//임시코드 
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // if (anim.GetBool("Forward"))
        // {
        //     Dir = transform.forward;
        // }
        // else if (anim.GetBool("Backward"))
        // {
        //     Dir = -transform.forward;
        // }
        // else
        // {
        //     Dir = Vector3.zero;
        // }
        // if(Input.GetKeyDown(KeyCode.W)){
        //     speedLimit = 100;
        //     anim.SetBool("Forward",true);
        //     isfrontmove = true;
        // }
        // if(Input.GetKeyUp(KeyCode.W)){
        //     anim.SetTrigger("Stop");
        //     anim.SetBool("Forward",false);
        //     isfrontmove = false;
        //     anim.SetBool("Sprint",false);
        // }
        // if(Input.GetKeyDown(KeyCode.S)){
        //     speedLimit = 50;
        //     anim.SetBool("Backward",true);
        //     isfrontmove = false;
        // }
        // if(Input.GetKeyUp(KeyCode.S)){
        //     anim.SetTrigger("Stop");
        //     anim.SetBool("Backward",false);
        // }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isspurt = true;
            // Dir = transform.forward;
            anim.SetBool("Forward", false);
            anim.SetBool("Sprint", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isspurt = false;
            anim.SetBool("Sprint", false);
            anim.SetBool("Forward", true);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Torquedir = -new Vector3(0, 1, 0) * Time.deltaTime;
            anim.SetBool("Left", true);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Torquedir = new Vector3(0, 1, 0) * Time.deltaTime;
            anim.SetBool("Right", true);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            Torquedir = Vector3.zero;
            anim.SetBool("Left", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            Torquedir = Vector3.zero;
            anim.SetBool("Right", false);
        }
        if (Input.GetKey(KeyCode.R))
        {
            gameObject.transform.LookAt(new Vector3(
                CurLane.transform.GetChild(0).position.x,
                gameObject.transform.position.y,
                CurLane.transform.GetChild(0).transform.position.z
                ));
        }
    }
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
        Max_Velocity = Basic_Velocity * 1.25f;
    }
    void SetAcceleration()
    {
        Basic_Acceleration = 5 + Power * 0.005f; // 파워1000이면 가속 초당 5+5, 최대 가속은 보통 100정도일듯
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
        MaxHP = (Stamina + 2000) * 100 * correction;
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
        Debug.Log("출발");
        mysit = Situation.early;
        StartCoroutine(SetAdditional_Acc(15f, 3f));
        CheckOverpace();
        anim.SetBool("Sprint", true);
    }
    public void InMid()
    {
        mysit = Situation.mid;
        Basic_Velocity = 25 + 0.5f * Intelligence * 0.1f;
        Max_Velocity = Basic_Velocity * 1.5f;
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
        Basic_Velocity = (Intelligence * 0.5f + Speed * 2.5f) * 0.5f * 0.1f * correction;
        Max_Velocity = Basic_Velocity * 1.25f;
        // 지능스탯 1000, 스피드 스탯 1000이면 무보정 최대 150일듯
    }
    void HP_Consumption()
    {
        Vector3 projvec = Vector3.Project(rigid.velocity, transform.forward);
        if (projvec.magnitude > Max_Velocity && mysit != Situation.spurt && isOnSkill == 0)
        {
            HP -= Mathf.Pow(rigid.velocity.magnitude, 2) * OverpaceCorrection;
            // Debug.Log($" {this.name} , {projvec.magnitude}, 오버페이스");
            Debug.Log($" {this.name} , {projvec.magnitude}, 오버페이스");
        }
        else
        {
            if (time3 < 2f) // 슬립스트림 2초 지속
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
        if (is_slipstream)
            time3 += 0.02f;


        Vector3 Speed_R_value = Vector3.zero;
        if (time2 > 0)
        {
            if (HP > 0)
            {
                Cur_Target_Velocity = (Basic_Velocity + Additional_Velocity + Max_Velocity) / 2;
                Cur_Acceleration = Basic_Acceleration + Additional_Acceleration;
                Speed_R_value = new Vector3(
                        transform.forward.x * Cur_Acceleration,
                        transform.forward.y * Cur_Acceleration,
                        transform.forward.z * Cur_Acceleration);

                if (isspurt)
                {
                    rigid.velocity += Speed_R_value / 2.5f;
                }
                else
                {
                    rigid.velocity += Speed_R_value / 10;
                }

                if (time2 > 3f && isspurt) // 체력소모는 3초 후부터, LShift(spurt버튼)를 눌렀을 경우에만
                {
                    HP_Consumption();
                }

            }
            else
            {
                rigid.velocity += Speed_R_value / 15;
                gameObject.layer = LayerMask.NameToLayer("HP0Horse");
            }
        }
        rigid.AddTorque(Torquedir * 1000, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "InMid") InMid();
        if (other.name == "Inlast") InLast();
        if (other.name == "Onspurt") OnSpurtLine();
        if (other.name == "gall") Time.timeScale = 0;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Animal") { return; }
        is_slipstream = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Animal") { return; }
        is_slipstream = false;
        time3 = 0;
    }
}
