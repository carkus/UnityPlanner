using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class Plan
{

    private Boolean accessible = false;
    private Boolean added = false;
    private string label;
    private string domainLabel;
    private string problemLabel;

    private JSONParser problem;
    private List<HSPNode> planNodes = new List<HSPNode>();
    private List<OBase> objectList = new List<OBase>();

    public Plan(string _label, string _dname, string _pname, List<OBase> _objects) {
        label = _label;
        domainLabel = _dname;
        problemLabel = _pname;
        objectList = _objects;
    }

    public string getLabel() {
        return label;
    }

    public string getDomainLabel() {
        return domainLabel;
    }

    public string getProblemLabel() {
        return problemLabel;
    } 

    public List<OBase> getPlanObjects() {
        return objectList;
    }     

    public void setProblem(JSONParser _problem) {
        problem = _problem;
    }        

    public JSONParser getProblem() {
        return problem;
    }           

    public List<HSPNode> getPlan() {
        return planNodes;
    }

    public void setPlan(List<HSPNode> plan) {
        planNodes = plan;
    }

    public Boolean isAddedToWorld() {
        return added;
    } 

    public Boolean isAccessible() {
        return accessible;
    } 

    public void setAddedToWorld(Boolean value) {
        added = value;
    }   


    public void setAccessible(Boolean value) {
        accessible = value;
    }   


}