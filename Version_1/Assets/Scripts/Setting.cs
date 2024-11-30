using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] public GameObject Setting1;//�������ý���
    private bool IsOpen_Setting = false;
    [SerializeField] public Slider Volume_Slider;//���������
    [SerializeField] public AudioSource audioSource;//������ƵԴ
    // Start is called before the first frame update
    void Start()
    {
        Volume_Slider.value = 0.35f;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = Volume_Slider.value; //ʵʱͬ����Ƶ
    }
    public void Setting_Open_Exit()
    {
        if (!IsOpen_Setting)
        {
            Setting1.SetActive(true);
            IsOpen_Setting = !IsOpen_Setting;//����״̬��ת
        }
        else
        {
            Setting1.SetActive(false);
            IsOpen_Setting = !IsOpen_Setting;//����״̬��ת
        }
    }
    public void Exit_Game()
    {
        //�˳���Ϸ
        Application.Quit();
    }

}
