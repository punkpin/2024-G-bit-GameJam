using UnityEngine;
using UnityEngine.EventSystems;  // 引入 EventSystems 命名空间

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ui_Image")]
    [SerializeField] public GameObject Ui_image;//储存Ui交互界面
    [SerializeField] public GameObject Levels;//储存游戏选关界面

    private bool IsOpen_Setting = false;
    // 鼠标进入按钮区域时
    public void OnPointerEnter(PointerEventData eventData)
    {
        Ui_image.SetActive(true);
        Debug.Log("鼠标放在按钮上");
    }

    // 鼠标退出按钮区域时
    public void OnPointerExit(PointerEventData eventData)
    {
        Ui_image.SetActive(false);
        Debug.Log("鼠标离开按钮");
    }

    public void Exit_Game()
    {
        Application.Quit();
    }

    public void Start_Game()
    {
        this.transform.parent.parent.gameObject.SetActive(false);
        Levels.SetActive(true);
    }
   
}
