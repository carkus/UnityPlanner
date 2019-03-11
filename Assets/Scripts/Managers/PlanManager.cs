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

    PreProcessor _processor;

    public PlanManager () {
        
        string d = "./Assets/PDDL/robot.json";
        ParseDomain(d);

        string p = "./Assets/PDDL/robot-3.json";
        ParseProblem(p);

        //Construct actions from operators with respect to problem objects:
        GroundActions();
    }

    public void ParseDomain(string ctxt)
    {
        
        JSONParser parser = JSONParser.Instance;
        _domain = new List<JsonNode>(); 
        _domain = parser.ParseDomain(ctxt);

    }

    public void ParseProblem(string ctxt)
    {
        
        JSONParser parser = JSONParser.Instance;
        _problem = new List<JsonNode>(); 
        _problem = parser.ParseProblem(ctxt);

    }    

    public void GroundActions() {

        _processor = PreProcessor.Instance;

        foreach(var dnode in _domain) {
            if (dnode.UserDefinedTypeName == Config.Constants.JSON_NODELABEL_OPERATORS) {
        
                foreach(var pnode in _problem) {
                    if (pnode.UserDefinedTypeName == Config.Constants.JSON_NODELABEL_OBJECTS) {
                        _processor.GroundActions(dnode, pnode);
                    }
                }

            }
        }
    
    }
    
}

