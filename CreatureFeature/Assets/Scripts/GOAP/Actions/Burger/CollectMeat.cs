using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMeat : GAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        //ModelDictionary.Instance.SpawnModel("food_ingredient_burger_uncooked", 0);
        return true;
    }
}
