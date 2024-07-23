using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public GAction action;

    public Node(Node parent, float cost, Dictionary<string, int> allStates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);
        this.action = action;
    }
}

public class GPlanner
{
    public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates states)
    {
        List<GAction> usableActions = new List<GAction>();
        foreach (GAction a in actions)
        {
            if (a.IsAchievable())
            {
                usableActions.Add(a);
                //Debug.Log(a.actionName);
            }
        }
        
        //leaves in node graph
        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, GWorld.Instance.GetWorld().GetStates(), null);
        
        //node graph
        bool success = BuildGraph(start, leaves, usableActions, goal);

        if(!success)
        {
            Debug.Log("NO PLAN!!");
            return null;
        }
        
        //if we are here we have a plan, YAY!
        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;
            }
            else
            {
                if (leaf.cost < cheapest.cost)
                {
                    // we've found our cheapest node
                    cheapest = leaf;
                }
            }
        }

        List<GAction> result = new List<GAction>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<GAction> queue = new Queue<GAction>();
        foreach (GAction a in result)
        {
            queue.Enqueue(a);
        }
        
        // We have a plan at this point, so add some debug features here to show it
        Debug.Log("The Plan is: ");
        foreach (GAction a in queue)
        {
            Debug.Log("Q: " + a.actionName);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
    {
        // using recursion for this, not 100% confident so should get checked later
        
        bool foundPath = false;
        foreach (GAction action in usableActions)
        {
            if (action.IsAchievableGiven(parent.state))
            {
                // get the current state of the effect in each action
                // we use effect, bc we work backwards in the tree
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                foreach(KeyValuePair<string, int> eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                    {
                        currentState.Add(eff.Key, eff.Value);
                    }
                    //Debug.Log(eff.Key);
                }
                
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                if (GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    // removes action from usable actions that we have already created a node for in the graph
                    // prevents a circular path
                    List<GAction> subset = ActionSubset(usableActions, action);
                    // recursion
                    bool found = BuildGraph(node, leaves, subset, goal);
                    
                    if (found)
                    {
                        foundPath = true;
                    }
                }
            }
        }
        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach (KeyValuePair<string, int> g in goal)
        {
            if (!state.ContainsKey(g.Key))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Removes GAction from List of GActions
    /// </summary>
    /// <param name="actions"></param>
    /// <param name="removeMe"></param>
    /// <returns></returns>
    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe)
    {
        List<GAction> subset = new List<GAction>();
        foreach (GAction a in actions)
        {
            if (!a.Equals(removeMe))
            {
                subset.Add(a);
            }
        }

        return subset;
    }
}
