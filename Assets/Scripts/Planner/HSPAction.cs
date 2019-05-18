
using System;
using System.Collections;
using System.Collections.Generic;

public class HSPAction {

    public string _name
    {
        get ;
        set ;
    }

    public List<string> _args { 
        get; 
        set;
    }

    private HashSet<string> _preConditions = new HashSet<string>();
    private HashSet<string> _posEffects = new HashSet<string>();
    private HashSet<string> _negEffects = new HashSet<string>();

    /*public List<HSPPredicate> _preconditions { 
        get; 
        set;
    }

    public List<HSPPredicate> _posEffects {
        get;
        set;
    }

    public List<HSPPredicate> _negEffects {
        get;
        set;
    }*/


    public HSPAction(
        string name, 
        List<string> args, 
        List<HSPPredicate> preconditions, 
        List<HSPPredicate> posEffects,
        List<HSPPredicate> negEffects) 
        {

        _name = name;
        _args = args;
        _preConditions = groundPredicates(preconditions);
        _posEffects = groundPredicates(posEffects);
        _negEffects = groundPredicates(negEffects);

    }

    private HashSet<string> groundPredicates(List<HSPPredicate> _predicates) {
        HashSet<string> grounds = new HashSet<string>();
        foreach(var predicate in _predicates) {
            grounds.UnionWith(predicate.GetGroundString());
        }
        return grounds;
    }

    public HashSet<string> getPreconditions() {
        return _preConditions;
    }

    public HashSet<string> getPosEffects() {
        return _posEffects;
    }

    public HashSet<string> getNegEffects() {
        return _negEffects;
    }

    public Boolean isApplicableIn(HashSet<string> state) {
        return _preConditions.IsSubsetOf(state);
    }

    public string GetString() {
        string sep = ",";
        return _name + " (" + String.Join( sep, _args ) + ")";
    }

}