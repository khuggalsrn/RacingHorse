using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] List<ComStatus> ComHorses;
    [SerializeField] PlayerInput PlayerHorse;
    [SerializeField] PlayerStatus PlayerStatus;
    [SerializeField] List<Button> Str_Btn;
    [SerializeField] Button Start_Btn;
    [SerializeField] Text Cur_str;
    // Start is called before the first frame update
    void Start()
    {
        Str_Btn[0].onClick.AddListener(()=>Btn_StrategySelect(0));
        Str_Btn[1].onClick.AddListener(()=>Btn_StrategySelect(1));
        Str_Btn[2].onClick.AddListener(()=>Btn_StrategySelect(2));
        Str_Btn[3].onClick.AddListener(()=>Btn_StrategySelect(3));
        Start_Btn.onClick.AddListener(()=>Btn_RaceStart());
    }

    public void Btn_RaceStart(){
        foreach (var race in ComHorses){
            race.enabled = true;
        }
        PlayerHorse.enabled = true;
        PlayerStatus.enabled = true;
        canvas.gameObject.SetActive(false);
    }
    public void Btn_StrategySelect(int strategy){
        switch (strategy){
            case 0:
                PlayerHorse.mystr = Strategy.Pacemaker;
                Cur_str.text = "Pacemaker";
                break;
            case 1:
                PlayerHorse.mystr = Strategy.Runner;
                Cur_str.text = "Runner";
                break;
            case 2:
                PlayerHorse.mystr = Strategy.Stalker;
                Cur_str.text = "Stalker";
                break;
            case 3:
                PlayerHorse.mystr = Strategy.Closer;
                Cur_str.text = "Closer";
                break;
        }
    }
}
