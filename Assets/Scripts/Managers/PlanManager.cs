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

    private PreProcessor processor;

    JSONParser domain;
    JSONParser problem; 

    private Boolean awake = false;

    public PlanManager () {
        awake = true;
    }

    public void frameTick() {

        if (awake) {
        
            if (planAgenda.Count > 0) {
                for (int i=0; i<planAgenda.Count; i++) {   
                    if (!planStack.ContainsKey(planAgenda[i].getLabel()) && planAgenda[i].isAccessible()) {
                        planAgenda[i].setAccessible(false);
                        planStack.Add(planAgenda[i].getLabel(), planAgenda[i]);
                        initPlanner(planAgenda[i]);
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
                            planStack[item.Key].setProblem(problem);
                            //_world.addPlanToWorld(planStack[item.Key]);

                            //Set inaccessible in this scope 
                            planStack[item.Key].setAccessible(false);
                        }
                    }
                }
            }
        }
    }

    public void addProblemToAgenda(string _label, string _dname, string _pname, List<OBase> _objects) {
        Plan plan = new Plan(_label, _dname, _pname, _objects);
        plan.setAccessible(true);
        planAgenda.Add(plan);
    }
    
    private void initPlanner(Plan _agenda) {
        domain = parseJSON("./Assets/PDDL/" + _agenda.getDomainLabel() + ".json");
        problem = parseJSON("./Assets/PDDL/" + _agenda.getProblemLabel() + ".json");
        callPlanner(_agenda, problem);
    }

    private async void callPlanner(Plan _plan, JSONParser _problem) {
        
        //ground action uses problem objects which need top be derived from world:
        List<HSPAction> actions = groundActions(domain.operations, _plan.getPlanObjects());

        Stopwatch watch = new Stopwatch();
        watch.Start();
            
        ProgressionPlanner _planner = new ProgressionPlanner();
        Task<List<HSPNode>> planTask = _planner.PerformPlan(_problem.state, _problem.goal, actions);
        List<HSPNode> plan = await planTask;
        _plan.setPlan(plan);

        watch.Stop();

        string planKey = _plan.getLabel();
        UnityEngine.Debug.Log(planKey + " : Time taken: " + watch.Elapsed.TotalSeconds.ToString());

        //planStack[planKey].setPlan(_plan);
        planStack[planKey].setAccessible(true);

    }

    private JSONParser parseJSON(string ctxt) {
        JSONParser parse = new JSONParser();
        parse.parseJSONDomain(ctxt);
        return parse;
    }

    private List<HSPAction> groundActions(List<HSPOperator> _operations, List<OBase> _objects) {
        List<HSPAction> actions = new List<HSPAction>();
        processor = PreProcessor.Instance;
        actions = processor.groundActions(_operations, getObjectPredicates(_objects));
        return actions;
    }

    private List<HSPPredicate> getObjectPredicates(List<OBase> _objects) {
        List<HSPPredicate> predicates = new List<HSPPredicate>();
        foreach(OBase obj in _objects) {
            predicates.Add(obj.getPredicate());
        }
        return predicates;
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

