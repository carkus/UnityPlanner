using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class WorldManager
{
    public static int score;        // The player's score.

    private Dictionary<string, Plan> planStack = new Dictionary<string, Plan>();

    public OBase[] worldObjects
    {
        get;
        set;
    }

    public bool awake;

    public WorldManager() 
    {
    }

    public void startWorld()
    {
        awake = true;
    }

    public void addPlanToWorld(Plan _plan)
    {
        planStack.Add(_plan.getLabel(), _plan);
    }

    public void addObjectsToWorld(List<OBase> _objects)
    {
        return;
    }    

    public void frameTick()
    {

        if (awake)
        {
            if (planStack.Count > 0)
            {
                foreach (var item in planStack)
                {
                    if (!planStack[item.Key].isAddedToWorld())
                    {
                        planStack[item.Key].setAddedToWorld(true);
                        //populateFromPlan(planStack[item.Key]);
                    }
                }

                //TODO - Clean up planStack when no longer needed...
                
            }
        }


    }

    private void populateFromPlan(Plan _plan) 
    {
        
        JSONParser _problem = _plan.getProblem();

        //Compile Plan Objects

        foreach(HSPPredicate type in _problem.objects) {

            //Debug.Log("TYPE: " + type.GetName());

        }
        

    }


    T[] InitializeArray<T>(int length) where T : new()
    {
        T[] array = new T[length];
        for (int i = 0; i < length; ++i)
        {
            array[i] = new T();
        }
        return array;
    }


}