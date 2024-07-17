using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCard : MonoBehaviour
{
    [SerializeField] private GameObject card1phb;
    [SerializeField] private GameObject card2phb;
    [SerializeField] private GameObject noCard;
    private Dictionary<GameObject, int> myCards = new Dictionary<GameObject, int>();
    private bool OnUI;
    private const int posY = 70;
    private GameObject[] cardPrehabs;
    public void Start()
    {
        cardPrehabs = new GameObject[] { card1phb, card2phb };
    }
    /// <summary>
    /// アイコンクリックしたら
    /// </summary>
    public void Click()
    {
        // カード一覧を表示
        if (!OnUI)
        {
            ViewUI(true);
            OnUI = true;
        }
        else
        {
            ViewUI(false);
            OnUI = false;
        }

    }
    /// <summary>
    /// UI位置調整して表示、非表示
    /// </summary>
    /// <param name="tf"></param>
    private void ViewUI(bool tf)
    {
        if (myCards.Count == 0)
        {
            // 所持カードがない
            noCard.SetActive(tf);
            return;
        }
        else
        {
            int id = 0;
            // 表示
            foreach (var card in myCards)
            {
                if (myCards.Count == 1)
                {
                    // posX=0
                    card.Key.transform.position = new Vector3(0,posY,0);
                }
                else if (myCards.Count == 2)
                {
                    // posX=-350,350
                    if (id == 0) card.Key.transform.position = new Vector3(-350,posY,0);
                    else if (id == 1) card.Key.transform.position = new Vector3(350,posY,0);
                }
                else if (myCards.Count == 3)
                {
                    // posX=-500,0,500
                    if (id == 0) card.Key.transform.position = new Vector3(-500,posY,0);
                    else if (id == 1) card.Key.transform.position = new Vector3(0,posY,0);
                    else if (id == 2) card.Key.transform.position = new Vector3(500,posY,0);
                }
                card.Key.SetActive(tf);
                id++;
            }
        }
    }
    /// <summary>
    /// カード追加
    /// </summary>
    private void AddCardUI(int cardnum)
    {
        GameObject newCard = Instantiate(cardPrehabs[cardnum], new Vector3(0, 0, 0), cardPrehabs[cardnum].transform.rotation);
        myCards.Add(newCard, cardnum);
        // TODO:アニメーション？くるくる
        WaitLoading(3.0f);
        // 非表示に
        newCard.SetActive(false);
    }
    /// <summary>
    /// カードを引く
    /// </summary>
    public void DrawCard()
    {
        int cardnum = Random.Range(0, cardPrehabs.Length);
        Debug.Log("カード"+cardnum+"を引く");
        AddCardUI(cardnum);
    }
    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }

}
