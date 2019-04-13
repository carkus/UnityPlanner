using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPPredicate : IComparable<HSPPredicate> {

    public string _name
    {
        get ;
        set ;
    }

    public List<HSPTerm> _args { 
        get; 
        set;
    }

    public HSPPredicate (string name, List<HSPTerm> args) {
        _args = args;
        _name = name;
    }

    public int CompareTo(HSPPredicate other) {
        if (this._name == other._name) {
            return 1;
        }
        return 0;
    }

    public bool isApplicableTo(HSPPredicate other) {
        
        if (this._name != other._name) {
            return false;
        }

        List<string> thisArgs = new List<string>();
        HashSet<string> otherArgs = new HashSet<string>();

        for(int i=0; i<this._args.Count; i++) {
            thisArgs.Add(this._args[i]._value);
        }

        for(int i=0; i<other._args.Count; i++) {
            otherArgs.Add(other._args[i]._value);
        }

        bool found = false;
        for(int i=0; i<thisArgs.Count; i++) {
            if (otherArgs.Contains(thisArgs[i])) {
                found = true;
            }
        }
        if (!found) return false;
        
        return true;
    }

    public bool IsGrounded() {
        for (int i = 0; i < _args.Count; i++) {
            if (!_args[i].IsConstant())
                return false;
        }
        return true;
    }
            
    public HSPPredicate Ground(Dictionary<string, string> subst) {
        List<HSPTerm> args = new List<HSPTerm>();
        foreach(HSPTerm arg in _args) {
            if (subst.ContainsKey(arg._name)) {
                string value = subst[arg._name];//.why not just OO _value??
                HSPTerm _const = arg.constant(value);
                args.Add(_const);
            }
        }
        return new HSPPredicate(_name, args);
    }

    public string GetString() {
        
        if (_name.Equals('=')) {
            return _args[0]._name + " = " + _args[1]._name;
        }
        else if (Arity() == 0) {
            return _name;
        }
        else {
            string sep = ", ";
            string[] val = new string[_args.Count];
            for (var i=0; i< _args.Count; i++) {
                val[i] = _args[i]._value;
            }
            return _name + "(" + String.Join( sep, val, 0, Arity() ) + ")";
        }

    }

    public int Arity() {
        return _args.Count;
    }


}