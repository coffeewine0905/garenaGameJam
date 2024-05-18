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
    public TextMeshProUGUI endText;
    public GameObject logPrefab;
    public Transform logParent;
    private List<GameObject> logPool = new List<GameObject>();
    private GameObject currentLog;
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

    }

    public void EndGame(Player player)
    {
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
}
