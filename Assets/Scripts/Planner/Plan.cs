using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class Plan
{

    private Boolean _accessible = false;
    private Boolean _added = false;
    private string _label;
    private string _domainLabel;
    private string _problemLabel;
    private JSONParser _problem;
    private List<HSPNode> _plan = new List<HSPNode>();

    public Plan(string label, string dname, string pname, List<HSPNode> plan) {
        _label = label;
        _domainLabel = dname;
        _problemLabel = pname;
        _plan = plan;
    }

    public string getLabel() {
        return _label;
    }

    public string getDomainLabel() {
        return _domainLabel;
    }

    public string getProblemLabel() {
        return _problemLabel;
    }   

    public void setProblem(JSONParser problem) {
        _problem = problem;
    }        

    public JSONParser getProblem() {
        return _problem;
    }           

    public List<HSPNode> getPlan() {
        return _plan;
    }

    public void setPlan(List<HSPNode> plan) {
        _plan = plan;
    }

    public Boolean isAddedToWorld() {
        return _added;
    } 

    public Boolean isAccessible() {
        return _accessible;
    } 

    public void setAddedToWorld(Boolean value) {
        _added = value;
    }   


    public void setAccessible(Boolean value) {
        _accessible = value;
    }   


}