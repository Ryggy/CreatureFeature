using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutCheese : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        //ModelDictionary.Instance.RemoveModel(2);
        return true;
    }
}
