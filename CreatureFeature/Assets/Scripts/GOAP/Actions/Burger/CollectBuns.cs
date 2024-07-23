using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectBuns : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        //ModelDictionary.Instance.SpawnModel("food_ingredient_bun", 0);

        return true;
    }
}
