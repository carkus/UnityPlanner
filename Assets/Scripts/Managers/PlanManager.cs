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

    List<JsonNode> _domain { get; set;}
    List<JsonNode> _problem { get; set;}
    List<HSPAction> _actions  { get; set;}

    PreProcessor _processor;

    public PlanManager () {
        
        string d = "./Assets/PDDL/robot.json";
        string p = "./Assets/PDDL/robot-3.json";

        _domain = ParseDomain(d);
        _problem = ParseProblem(p);

        //Construct actions from operators with respect to problem objects:
        _actions = GroundActions();
    }

    public List<JsonNode> ParseDomain(string ctxt)
    {
        
        JSONParser parser = JSONParser.Instance;
        List<JsonNode> domain = new List<JsonNode>(); 
        domain = parser.ParseDomain(ctxt);
        return domain;

    }

    public List<JsonNode> ParseProblem(string ctxt)
    {
        
        JSONParser parser = JSONParser.Instance;
        var problem = new List<JsonNode>(); 
        problem = parser.ParseProblem(ctxt);
        return problem;

    }    

    public List<HSPAction> GroundActions() {

        List<HSPAction> actions = new List<HSPAction>();
        _processor = PreProcessor.Instance;
        actions = _processor.GroundActions(_domain);
        return actions;

    }
    
}

