using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef :  GAgent
{
    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("servedBurger", 1, false);
        goals.Add(s1, 3);

    }
}
