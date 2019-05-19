using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class Plan
{

    private Boolean _accessible = false;
    private string _label;
    private string _domain;
    private string _problem;
    private List<HSPNode> _plan = new List<HSPNode>();

    public Plan(string label, string domain, string problem, List<HSPNode> plan) {
        _label = label;
        _domain = domain;
        _problem = problem;
        _plan = plan;
    }

    public string getLabel() {
        return _label;
    }

    public string getDomain() {
        return _domain;
    }

    public string getProblem() {
        return _problem;
    }        

    public List<HSPNode> getPlan() {
        return _plan;
    }

    public void setPlan(List<HSPNode> plan) {
        _plan = plan;
    }

    public Boolean getAccessible() {
        return _accessible;
    } 

    public void setAccessible(Boolean value) {
        _accessible = value;
    }   


}