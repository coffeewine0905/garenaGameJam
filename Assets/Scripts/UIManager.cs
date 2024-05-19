using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject endPanel;
    public GameObject mainUI;
    public GameObject leftTurnUI;
    public GameObject rightTurnUI;
    public List<GameObject> leftBuffUI;
    public List<GameObject> rightBuffUI;
    public TextMeshProUGUI endText;
    public GameObject logPrefab;
    public Transform logParent;
    public Image AddChiliImage;
    public Image NowChiliImage;
    public Image TotalChiliImage;
    public Image DrawCardImage;
    private List<GameObject> logPool = new List<GameObject>();
    private GameObject currentLog;
    private int currentChiliValue = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        mainUI.SetActive(true);
    }

    public void EndGame(Player player)
    {
        mainUI.SetActive(false);
        endPanel.SetActive(true);
        endText.text = "Player " + player.ID + " Lose!";
    }

    public void ShowLog(string log)
    {
        if (currentLog != null)
        {
            Destroy(currentLog);
        }
        GameObject go = Instantiate(logPrefab, logParent);
        currentLog = go;
        go.GetComponent<TextMeshProUGUI>().text = log;
        go.transform.localScale = Vector3.zero;
        //用DoScale方法讓物件在1秒內放大到原本的兩倍delay 1秒後執行縮小回原本大小
        go.transform.DOScale(1, 0.3f).OnComplete(() =>
        {
            go.transform.DOScale(0, 0.3f).SetDelay(1).onComplete += () =>
            {
                Destroy(go);
            };
        });
    }

    public void SetAddChiliImage(bool active)
    {
        //如果active為true，則顏色為白色，否則為灰色
        AddChiliImage.color = active ? Color.white : Color.gray;
        //比例為1.2倍
        AddChiliImage.transform.localScale = active ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(1, 1, 1);
        NowChiliImage.gameObject.SetActive(active);
        if (!active)
        {
            SetNowChiliValue(0);
        }
    }

    public void SetTotalChiliValue(int value)
    {
        TotalChiliImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
    }

    public void SetNowChiliValue(int value)
    {
        currentChiliValue = value;
        NowChiliImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
    }

    public void SetDrawCardImage(bool active)
    {
        //如果active為true，則顏色為白色，否則為灰色
        DrawCardImage.color = active ? Color.white : Color.gray;
        //比例為1.2倍
        DrawCardImage.transform.localScale = active ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(1, 1, 1);
    }

    public void ShowTurnUI(int id)
    {
        if (id == 1)
        {
            leftTurnUI.SetActive(true);
            rightTurnUI.SetActive(false);
        }
        else
        {
            leftTurnUI.SetActive(false);
            rightTurnUI.SetActive(true);
        }
    }

    public void ShowBuffUI(int id, int buffID, int value = 1)
    {
        if (id == 1)
        {
            leftBuffUI[buffID].SetActive(true);
            if (buffID == 1)
            {
                leftBuffUI[1].GetComponentInChildren<TextMeshProUGUI>().text = "+" + value;
            }
        }
        else
        {
            rightBuffUI[buffID].SetActive(true);
            if (buffID == 1)
            {
                rightBuffUI[1].GetComponentInChildren<TextMeshProUGUI>().text = "+" + value;
            }
        }
    }

    public void UpdateBuffUI(int id, int buffID, int value)
    {
        //特例處理
        if (id == 1)
        {
            if (value == 0)
            {
                leftBuffUI[buffID].SetActive(false);
                return;
            }
            if (buffID == 1)
                leftBuffUI[buffID].GetComponentInChildren<TextMeshProUGUI>().text = "+" + value;
        }
        else
        {
            if (value == 0)
            {
                rightBuffUI[buffID].SetActive(false);
                return;
            }
            if (buffID == 1)
                rightBuffUI[buffID].GetComponentInChildren<TextMeshProUGUI>().text = "+" + value;
        }
    }

    public void CloseBuffUI(int id, int buffID)
    {
        if (id == 1)
        {
            leftBuffUI[buffID].SetActive(false);
        }
        else
        {
            rightBuffUI[buffID].SetActive(false);
        }
    }

    public void ClearBuffUI(int id)
    {
        if (id == 1)
        {
            foreach (var buffUI in leftBuffUI)
            {
                buffUI.SetActive(false);
            }
        }
        else
        {
            foreach (var buffUI in rightBuffUI)
            {
                buffUI.SetActive(false);
            }
        }
    }
}
