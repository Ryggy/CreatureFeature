using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastBuns : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        //ModelDictionary.Instance.RemoveModel(1);
        return true;
    }
}
