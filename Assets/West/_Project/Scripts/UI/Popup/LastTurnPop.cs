using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastTurnPop : Popup
{
    public override void Init()
    {
        base.Init();

        Invoke(nameof(ClosePopup), 1.2f);
    }

    private void ClosePopup()
    {
        PopupManager.ClosePopup<LastTurnPop>();
    }
}
