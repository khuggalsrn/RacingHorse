using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Strategy {
        Pacemaker, Runner, Stalker, Closer
};
public enum Situation {
        early, mid, last, spurt
};
public class SystemClass : MonoBehaviour
{
    private static SystemClass instance = null;
    void Awake(){
        if (instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    [SerializeField] float timeScale = 1f;
    void Start(){
        Time.timeScale = timeScale;
    }
}
