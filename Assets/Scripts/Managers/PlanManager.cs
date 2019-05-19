using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;
using System.Threading;

using JsonToDataContract;
using Planner;

public class PlanManager
{

    List<string> planLabels = new List<string>() { "Plan-", "Plan-1", "Plan-1", "Thursday", "Friday", "Saturday", "Sunday" };

    Dictionary <string, Plan> planStack = new Dictionary <string, Plan>();
    List<Plan> planAgenda = new List<Plan>();

    PreProcessor _processor;

    Boolean awake = false;

    public PlanManager () {

            for (int i=5; i>0; i--) {
                Plan plan = new Plan("plan"+(i+1).ToString(), "robot", "robot-"+(i).ToString(), null);
                planAgenda.Add(plan);
            }

            awake = true;

    }

    public void frameTick() {

        if (awake) {
        
            if (planAgenda.Count > 0) {
                if (!planStack.ContainsKey(planAgenda[0].getLabel())) {
                    planStack.Add(planAgenda[0].getLabel(), planAgenda[0]);
                    initPlanner(planAgenda[0].getLabel(), planAgenda[0].getDomain(), planAgenda[0].getProblem());
                    planAgenda.RemoveAt(0);
                }
            }

            foreach(var item in planStack) {
                if (planStack[item.Key].getAccessible()) {
                    UnityEngine.Debug.Log(item.Key);
                    printPlanActions(planStack[item.Key].getPlan());
                    planStack[item.Key].setAccessible(false);
                }
            }

        }

    }
    
    public void initPlanner(string _label, string _dname, string _pname) {

        string d = "./Assets/PDDL/" + _dname + ".json";
        string p = "./Assets/PDDL/" + _pname + ".json";

        JSONParser _domain = parseJSON(d);
        JSONParser _problem = parseJSON(p);
        List<HSPAction> _actions = groundActions(_domain, _problem);

        callPlanner(_label, _problem, _actions);

    }

    public async void callPlanner(string _planId, JSONParser _problem, List<HSPAction> _actions) {

        Stopwatch watch = new Stopwatch();
        watch.Start();
            
        ProgressionPlanner _planner = new ProgressionPlanner();
        Task<List<HSPNode>> planTask = _planner.PerformPlan(_problem.state, _problem.goal, _actions);
        List<HSPNode> plan = await planTask;
        planStack[_planId].setPlan(plan);
        planStack[_planId].setAccessible(true);
        //printPlanActions(planStack[_planId]);

        watch.Stop();

        UnityEngine.Debug.Log(_planId + " : Time taken: " + watch.Elapsed.TotalSeconds.ToString());

    }

    public JSONParser parseJSON(string ctxt) {
        JSONParser parse = new JSONParser();
        parse.parseJSONDomain(ctxt);
        return parse;
    }

    public List<HSPAction> groundActions(JSONParser _domain, JSONParser _problem) {
        List<HSPAction> actions = new List<HSPAction>();
        _processor = PreProcessor.Instance;
        actions = _processor.groundActions(_domain.operations, _problem.objects);
        return actions;
    }

    private void printPlanActions(List<HSPNode> plan) {            
        foreach(HSPNode node in plan) {
            string act = node.GetAction().GetString();
            UnityEngine.Debug.Log(act + " : " + node.GetG());
        }
    }

    
}

