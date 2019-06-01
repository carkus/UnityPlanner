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

    private Dictionary <string, Plan> planStack = new Dictionary <string, Plan>();
    private List<Plan> planAgenda = new List<Plan>();

    private PreProcessor _processor;
    private WorldManager _world;

    JSONParser _domain;
    JSONParser _problem; 

    private Boolean awake = false;

    public PlanManager (WorldManager world) {
        _world = world;
        awake = true;
    }

    public void frameTick() {

        if (awake) {
        
            if (planAgenda.Count > 0) {
                for (int i=0; i<planAgenda.Count; i++) {   
                    if (!planStack.ContainsKey(planAgenda[i].getLabel()) && planAgenda[i].isAccessible()) {
                        planAgenda[i].setAccessible(false);
                        planStack.Add(planAgenda[i].getLabel(), planAgenda[i]);
                        initPlanner(planAgenda[i].getLabel(), planAgenda[i].getDomainLabel(), planAgenda[i].getProblemLabel());
                        planAgenda.RemoveAt(i);
                    }
                }                
            }
            
            if (planStack.Count > 0) {

                foreach (var item in planStack) {
                    if (planStack[item.Key].isAccessible()) {
                        if (planStack[item.Key].getPlan().Count > 0) {
                            printPlanActions(planStack[item.Key]);
                            
                            //Send plan into the world
                            planStack[item.Key].setProblem(_problem);
                            _world.addPlanToWorld(planStack[item.Key]);

                            //Set inaccessible in this scope 
                            planStack[item.Key].setAccessible(false);
                        }
                    }
                }
            }
        }

    }

    public void addNewPlan(string _label, string _dname, string _pname) {
        Plan plan = new Plan(_label, _dname, _pname, null);
        plan.setAccessible(true);
        planAgenda.Add(plan);
    }
    
    private void initPlanner(string _label, string _dname, string _pname) {
        _domain = parseJSON("./Assets/PDDL/" + _dname + ".json");
        _problem = parseJSON("./Assets/PDDL/" + _pname + ".json");
        List<HSPAction> _actions = groundActions(_domain, _problem);
        callPlanner(_label, _problem, _actions);
    }

    private async void callPlanner(string _planId, JSONParser _problem, List<HSPAction> _actions) {

        Stopwatch watch = new Stopwatch();
        watch.Start();
            
        ProgressionPlanner _planner = new ProgressionPlanner();
        Task<List<HSPNode>> planTask = _planner.PerformPlan(_problem.state, _problem.goal, _actions);
        List<HSPNode> plan = await planTask;

        watch.Stop();

        UnityEngine.Debug.Log(_planId + " : Time taken: " + watch.Elapsed.TotalSeconds.ToString());

        planStack[_planId].setPlan(plan);
        planStack[_planId].setAccessible(true);

    }

    private JSONParser parseJSON(string ctxt) {
        JSONParser parse = new JSONParser();
        parse.parseJSONDomain(ctxt);
        return parse;
    }

    private List<HSPAction> groundActions(JSONParser _domain, JSONParser _problem) {
        List<HSPAction> actions = new List<HSPAction>();
        _processor = PreProcessor.Instance;
        actions = _processor.groundActions(_domain.operations, _problem.objects);
        return actions;
    }

    private void printPlanActions(Plan plan) {
        List<HSPNode> planNodes = plan.getPlan();
        foreach(HSPNode node in planNodes) {
            UnityEngine.Debug.Log(plan.getLabel() + " : " + node.GetG() + " : " + node.GetAction().GetString());
        }
    }

    public Plan getStackedPlan(string _label) {
        return planStack[_label];
    }

    
}

