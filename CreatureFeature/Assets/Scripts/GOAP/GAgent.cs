using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal
{
    public Dictionary<string, int> sgoals;
    public bool remove;

    public SubGoal(string s, int i, bool r)
    {
        sgoals = new Dictionary<string, int>();
        sgoals.Add(s, i);
        remove = r;
    }
}
public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();

    private GPlanner planner;
    private Queue<GAction> actionQueue;
    public GAction currentAction;
    private SubGoal currentGoal;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts)
        {
            actions.Add(a);
        }
    }

    private bool invoked = false;

    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }
    
    void LateUpdate()
    {

        if (currentAction != null && currentAction.running)
        {
           // checking if we are in proximity to our destination
            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
            if (currentAction.agent.hasPath && distanceToTarget < 2.5f)
            {
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }
        
        //Check if we have a plan or action queue to perform
        if (planner == null || actionQueue == null)
        {
            planner = new GPlanner();
            
            // sort goals from most to least important
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                actionQueue = planner.plan(actions, sg.Key.sgoals, null);
                if(actionQueue != null)
                {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        // run out of things to do in the queue
        if (actionQueue != null && actionQueue.Count == 0)
        {
            // remove the goal if it is removable
            if (currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }

            planner = null;
        }

        // start performing our actions
        if (actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }
                
                if (currentAction.target != null)
                {
                    currentAction.running = true;
                    //NAVMESH CODE - remove once custom pathfinding is made
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
            }
            else
            {
                actionQueue = null;
            }
        }
        
    }
}
