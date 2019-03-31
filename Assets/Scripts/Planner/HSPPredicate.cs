using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPPredicate {

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

    public HSPPredicate (string name, IDictionary<string, JsonNode> args) {
        _args = new List<HSPTerm>();
        _args = DeriveArgs(args);
        _name = name;
    }

    public List<HSPTerm> DeriveArgs(IDictionary<string, JsonNode> nodes) {
        List<HSPTerm> args = new List<HSPTerm>();
        foreach (var item in nodes) {
            if (item.Value.Members.Count > 0) {
                foreach (var member in item.Value.Members) {
                    HSPTerm newarg = new HSPTerm(item.Key, member.Key, null);
                    args.Add(newarg);
                }
            }
            else {
                HSPTerm newarg = new HSPTerm(item.Key, null, null);
                args.Add(newarg);                
            }
        }
        return args;
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