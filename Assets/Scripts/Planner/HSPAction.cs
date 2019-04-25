
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

    public List<HSPPredicate> _preconditions { 
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
    }


    public HSPAction(
        string name, 
        List<string> args, 
        List<HSPPredicate> preconditions, 
        List<HSPPredicate> posEffects,
        List<HSPPredicate> negEffects) 
        {

        _name = name;
        _args = args;
        _preconditions = preconditions;
        _posEffects = posEffects;
        _negEffects = negEffects;

    }

    public string GetString() {
        string sep = ",";
        return _name + " (" + String.Join( sep, _args ) + ")";
    }

}