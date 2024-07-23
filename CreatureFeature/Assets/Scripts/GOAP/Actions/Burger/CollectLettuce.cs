using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectLettuce : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        //ModelDictionary.Instance.SpawnModel("food_ingredient_lettuce", 0);
        return true;
    }
}
