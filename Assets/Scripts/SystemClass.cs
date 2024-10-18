using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Strategy {
        Pacemaker, Runner, Stalker, Closer
};
public enum Situation {
        early, mid, last, spurt
};
public class SystemClass : MonoBehaviour
{
    [SerializeField] float timeScale = 1f;
    void Start(){
        Time.timeScale = timeScale;
    }
}
