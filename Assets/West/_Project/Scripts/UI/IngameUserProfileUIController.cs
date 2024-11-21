using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIEffects;
using TMPro;
using System;


public enum PLAYER_NUMB
{
    PLAYER1,PLAYER2
}

public class IngameUserProfileUIController : MonoBehaviour
{
    public PLAYER_NUMB playerNumber;
    public UIEffect uiEffect;
    public TextMeshProUGUI tmp;
    [Space(6)]
    [Header("GreyScale")]
    public Material greyMaterial;
    public Color greyColor;

    [Space(6)]
    [Header("1P")]
    public Material player1Material; // 1P 머테리얼 프리셋
    public Color activeColor1P;

    [Space(6)]
    [Header("2P")]
    public Material player2Material; // 2P 머테리얼 프리셋
    public Color activeColor2P;


    public void SetProfileGrey()
    {
        if (tmp is null) return;

        uiEffect.toneFilter = ToneFilter.Grayscale;
        tmp.fontSharedMaterial = greyMaterial;
        tmp.color = greyColor;
    }

    private void OnEnable()
    {
        SetProfileNormal();
    }

    public void SetProfileNormal()
    {
        if (tmp is null) return;

        
        uiEffect.toneFilter = ToneFilter.None;

        switch (playerNumber)
        {
            case PLAYER_NUMB.PLAYER1:
                tmp.fontSharedMaterial = player1Material;
                tmp.color = activeColor1P;
                break;
            case PLAYER_NUMB.PLAYER2:
                tmp.fontSharedMaterial = player2Material;
                tmp.color = activeColor2P;
                break;
        }
    }

}
