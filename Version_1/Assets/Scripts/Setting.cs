using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] public GameObject Setting1;//储存设置界面
    private bool IsOpen_Setting = false;
    [SerializeField] public Slider Volume_Slider;//储存进度条
    [SerializeField] public AudioSource audioSource;//储存音频源
    // Start is called before the first frame update
    void Start()
    {
        Volume_Slider.value = 0.35f;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = Volume_Slider.value; //实时同步音频
    }
    public void Setting_Open_Exit()
    {
        if (!IsOpen_Setting)
        {
            Setting1.SetActive(true);
            IsOpen_Setting = !IsOpen_Setting;//开启状态反转
        }
        else
        {
            Setting1.SetActive(false);
            IsOpen_Setting = !IsOpen_Setting;//开启状态反转
        }
    }
    public void Exit_Game()
    {
        //退出游戏
        Application.Quit();
    }

}
