using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform UIGroup;
    public Animator animator;

    public GameObject[] itemObject;
    public int[] itemPrice;
    public Transform[] itemTransform;
    public Text talkText;
    public string[] talkData;
    public float talkDuration;

    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        UIGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        animator.SetTrigger("doHello");
        UIGroup.anchoredPosition = Vector3.down * 1000f;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];

        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 randomVec = Vector3.right * Random.Range(-3f, 3f)
                            + Vector3.forward * Random.Range(-3f, 3f);
        Instantiate(itemObject[index], itemTransform[index].position + randomVec, itemTransform[index].rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(talkDuration);
        talkText.text = talkData[0];
    }
}
