using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using JsonToDataContract;
using Planner;

public class PlanManager
{

    List<string> importList = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    enum Colors { Red, Green, Blue, Yellow };

    //List<JsonNode> _domain { get; set;}
    JSONParser _domain { get; set;}
    JSONParser _problem { get; set;}
    List<HSPAction> _actions  { get; set;}

    PreProcessor _processor;
    ProgressionPlanner _planner;

    public PlanManager () {
    }

    public void groundProblem() {
        
        string d = "./Assets/PDDL/robot.json";
        parseDomain(d);

        string p = "./Assets/PDDL/robot-3.json";
        parseProblem(p);

        //Construct actions from operators with respect to problem objects:
        _actions = groundActions();

    }

    public void parseDomain(string ctxt) {
        _domain = new JSONParser();
        _domain.parseDomain(ctxt);
    }


    public void parseProblem(string ctxt) {
        _problem = new JSONParser();
        _problem.parseProblem(ctxt);        
    }    

    public void callPlanner() {
        _planner = ProgressionPlanner.Instance;
        _planner.buildPlan(_problem.state, _problem.goal, _actions);
    }

    public List<HSPAction> groundActions() {
        List<HSPAction> actions = new List<HSPAction>();
        _processor = PreProcessor.Instance;
        actions = _processor.groundActions(_domain.operations, _problem.objects);
        return actions;
    }
    
}

