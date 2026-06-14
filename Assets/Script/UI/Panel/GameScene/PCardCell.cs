using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PCardCell : MonoBehaviour
{
    private Animator anim;
    private Transform UIIcon;
    public bool IsSelect=false;
    private bool isMine;

    public Card card;
    private void Awake()
    {
        UIIcon = transform.Find("Icon");

        anim=UIIcon.GetComponent<Animator>();

        UIIcon.GetComponent<Button>().onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        if(!isMine) return;
        IsSelect = !IsSelect;
        anim.SetBool("IsPress", IsSelect);
    }
    public void Refresh(Card card,bool isMine)
    {
        this.isMine= isMine;
        this.card = card;
        string IconPath = "";
        if (isMine)
        {
            if (card.rank == CardRank.BigJoker || card.rank == CardRank.LittleJoker)
                IconPath = "Poker/" + card.rank;
            else
                IconPath = "Poker/" + card.suit + card.rank;
        }
        else
        {
            IconPath = "Poker/CardBack";
        }
        Sprite sp = Resources.Load<Sprite>(IconPath);
        UIIcon.GetComponent<Image>().sprite = sp;
    }
}
