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
            
    public int Ground(Dictionary<string, List<string>> subst) {

        /*
        args = []
        for arg in self._args:
            if arg in subst:
                value = subst[arg]
                arg = Term.constant(value)
            args.append(arg)
        return Predicate(self._name, args)
         */

        List<HSPTerm> args = new List<HSPTerm>();
        foreach(var arg in _args) {
            //if (subst[arg] != null) {

            //}
        }

        return _args.Count;
    }    

    public int Arity() {
        return _args.Count;
    }


}