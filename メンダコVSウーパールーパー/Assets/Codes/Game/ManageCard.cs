using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// attach to GameManager
/// </summary>
public class ManageCard : MonoBehaviour
{
    [SerializeField] private Transform cardfolder;
    [SerializeField] private GameObject card1phb;
    [SerializeField] private GameObject card2phb;
    [SerializeField] private GameObject card3phb;
    [SerializeField] private GameObject noCard;

    // カード効果状態
    public enum Card
    {
        Default, // カード効果なし
        Naname, // 斜め移動可
        OneMore1, // ２回行動(一回目のとき)
        OneMore2, // ２回行動(二回目のとき)
        Forecast // ランダムの本物を光らせる

    }
    public Card card { get; set; }

    // カードオブジェクト、カード番号
    private List<GameObject> myCards;
    private bool OnUI;
    private const int posY = 70;
    private GameObject[] cardPrefabs;
    private CardState cardState;


    public void Start()
    {
        cardPrefabs = new GameObject[] { card1phb, card2phb, card3phb };
        myCards = new List<GameObject>();
        OnUI = false;
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
            Debug.Log("所持カードを表示します");
            int id = 0;
            // 表示
            foreach (var card in myCards)
            {
                RectTransform cadeTransform = card.GetComponent<RectTransform>();
                if (myCards.Count == 1)
                {
                    // posX=0
                    cadeTransform.anchoredPosition = new Vector2(0, posY);
                }
                else if (myCards.Count == 2)
                {
                    // posX=-350,350
                    if (id == 0) cadeTransform.anchoredPosition = new Vector2(-350, posY);
                    else if (id == 1) cadeTransform.anchoredPosition = new Vector2(350, posY);
                }
                else if (myCards.Count == 3)
                {
                    // posX=-500,0,500
                    if (id == 0) cadeTransform.anchoredPosition = new Vector2(-500, posY);
                    else if (id == 1) cadeTransform.anchoredPosition = new Vector2(0, posY);
                    else if (id == 2) cadeTransform.anchoredPosition = new Vector2(500, posY);
                }
                card.SetActive(tf);
                id++;
            }
        }
    }
    /// <summary>
    /// カード追加
    /// </summary>
    private void AddCardUI(int cardnum)
    {
        GameObject newCard = Instantiate(
            cardPrefabs[cardnum], new Vector3(0, 0, -1), cardPrefabs[cardnum].transform.rotation, cardfolder);
        myCards.Add(newCard);
        newCard.SetActive(true);
        newCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
        // TODO:アニメーション？くるくる
        // 2s待って非表示
        StartCoroutine(WaitLoadingSetActive(2.0f, newCard));
    }
    IEnumerator WaitLoadingSetActive(float time, GameObject newCard)
    {
        // 待つ
        yield return new WaitForSeconds(time);
        newCard.SetActive(false);
    }
    /// <summary>
    /// カードを引く
    /// </summary>
    public void DrawCard()
    {
        int cardnum = Random.Range(0, cardPrefabs.Length);
        Debug.Log("カード" + cardnum + "を引く");
        AddCardUI(cardnum);
        GetComponent<GameUI>().GetPieceUI();
    }

    public void ActiveCard(int num)
    {
        switch (num)
        {
            case 0:
                card = Card.Naname;
                Debug.Log("斜め移動できるようになりました");
                break;
            case 1:
                card = Card.OneMore1;
                Debug.Log("二回行動できるようになりました");
                break;
            case 2:
                card = Card.Forecast;
                Debug.Log("相手の駒の中から１つ、本物を見破りました");
                this.gameObject.GetComponent<PlayGame>().SearchRealFromPartner();
                break;
        }
        foreach (var card in myCards)
        {
            if (card.name.StartsWith(cardPrefabs[num].name)) //クリックした名前のカードを
            {
                ViewUI(false); //UI非表示
                OnUI = false;
                myCards.Remove(card); //配列から削除
                Destroy(card);
                return;
            }
        }
        ViewUI(false);
        OnUI = false;
    }

    // 全カードの使用可能状況を切り替える
    public void SwitchCanUse(bool canUse)
    {
        foreach (GameObject cardObj in cardPrefabs)
        {
            cardObj.GetComponent<CardState>().canUse = canUse;
            Debug.Log("canUse:"+cardObj.GetComponent<CardState>().canUse);
        }
        if (canUse)
        { // 使える状態(カードの効果が切れた状態)
            card = Card.Default;
        }
    }

}
