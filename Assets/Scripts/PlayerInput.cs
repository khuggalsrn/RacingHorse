using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    Animator anim;
    public Rigidbody rigid;
    bool isfrontmove = false;
    [SerializeField]
    float speedLimit = 75;
    Vector3 Dir;
    Vector3 Torquedir;
    float Force;
    float time = 0;
    float Basic_Velocity;
    bool isspurt = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid.velocity = Vector3.zero;
        Basic_Velocity = speedLimit;
        // rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(anim.GetBool("Forward")) {
            Dir = transform.forward;
        }
        else if (anim.GetBool("Backward")){
            Dir = -transform.forward;
        }
        else{
            Dir = Vector3.zero;
        }
        if(Input.GetKeyDown(KeyCode.W)){
            speedLimit = 100;
            anim.SetBool("Forward",true);
            isfrontmove = true;
        }
        if(Input.GetKeyUp(KeyCode.W)){
            anim.SetTrigger("Stop");
            anim.SetBool("Forward",false);
            isfrontmove = false;
            anim.SetBool("Sprint",false);
        }
        if(Input.GetKeyDown(KeyCode.S)){
            speedLimit = 50;
            anim.SetBool("Backward",true);
            isfrontmove = false;
        }
        if(Input.GetKeyUp(KeyCode.S)){
            anim.SetTrigger("Stop");
            anim.SetBool("Backward",false);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) && isfrontmove){
            isspurt = true;
            Dir = transform.forward;
            anim.SetBool("Sprint",true);
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)){
            isspurt = false;
            anim.SetBool("Sprint",false);
        }
        if(Input.GetKey(KeyCode.A)){
            Torquedir = -new Vector3(0,1,0)* Time.deltaTime;
            anim.SetBool("Left",true);
        }
        if(Input.GetKey(KeyCode.D)){
            Torquedir = new Vector3(0,1,0)* Time.deltaTime;
            anim.SetBool("Right",true);
        }
        if(Input.GetKeyUp(KeyCode.A)){
            Torquedir = Vector3.zero;
            anim.SetBool("Left",false);
        }
            
        if(Input.GetKeyUp(KeyCode.D)){
            Torquedir = Vector3.zero;
            anim.SetBool("Right",false);
        }
        if(Input.GetKeyUp(KeyCode.Space)){
            Debug.Log(time);
        }
    }
    float avg = 0;
    private void FixedUpdate() {
        Basic_Velocity = speedLimit;
        // if(time < 1000) avg += rigid.velocity.magnitude/1000;
        // else{
        //     Debug.Log(avg);
        // }
        time += 0.02f;

        if (rigid.velocity.magnitude < Basic_Velocity){
            rigid.velocity += 
                new Vector3(
                    transform.forward.x * speedLimit/10, 
                transform.forward.y * speedLimit/10, 
                transform.forward.z * speedLimit/10)/10;
            if(isspurt){
                rigid.velocity += 
                new Vector3(
                    transform.forward.x * speedLimit/10, 
                transform.forward.y * speedLimit/10, 
                transform.forward.z * speedLimit/10)/4;
            }
        }
        // time++;
        // if(time == 3)
        // {
        //     rigid.AddForce( 50* speedLimit * transform.forward, ForceMode.Acceleration);
        //     time = 0;
        // }
        
        // rigid.AddForce(5*speedLimit * Dir);
        rigid.AddTorque(Torquedir * 1000,ForceMode.Impulse);
        // if(rigid.velocity.magnitude > speedLimit){
        //     rigid.velocity = speedLimit * rigid.velocity.normalized;
        //     Debug.Log(rigid.velocity);
        // }
        
    }

    
    private void OnTriggerEnter(Collider other) {
        if (other.tag != "Animal"){return;}
        if(other.gameObject.GetComponent<Rigidbody>().velocity.magnitude < rigid.velocity.magnitude){

        }
    }
}
