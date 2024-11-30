using UnityEngine;
using UnityEngine.EventSystems;  // ���� EventSystems �����ռ�

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ui_Image")]
    [SerializeField] public GameObject Ui_image;//����Ui��������
    [SerializeField] public GameObject Levels;//������Ϸѡ�ؽ���

    private bool IsOpen_Setting = false;
    // �����밴ť����ʱ
    public void OnPointerEnter(PointerEventData eventData)
    {
        Ui_image.SetActive(true);
        Debug.Log("�����ڰ�ť��");
    }

    // ����˳���ť����ʱ
    public void OnPointerExit(PointerEventData eventData)
    {
        Ui_image.SetActive(false);
        Debug.Log("����뿪��ť");
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
