using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPPredicate : IComparable<HSPPredicate> {

    private string _name;
    private List<HSPTerm> _args;
    private HashSet<string> _grounds;

    public HSPPredicate (string name, List<HSPTerm> args) {
        _args = args;
        _name = name;
        _grounds = GroundToString();
    }

    public int CompareTo(HSPPredicate other) {
        if (this._name == other._name) {
            return 1;
        }
        return 0;
    }

    private HashSet<string> GroundToString() {

        HashSet<string> temp = new HashSet<string>();

        string deriveGround(HSPPredicate predicate, HSPTerm arg) {
            if (arg.IsTyped()) {
                return $"{predicate.GetName()}({arg.GetValue()}, {arg.GetTermType()})"; 
            }
            return $"{predicate.GetName()}({arg.GetValue()})";
        }    
        
        foreach(HSPTerm arg in _args) {
           temp.Add(deriveGround(this, arg));
        }
        return temp;

    }

    public bool IsGrounded() {
        for (int i = 0; i < _args.Count; i++) {
            if (!_args[i].IsConstant())
                return false;
        }
        return true;
    }
            
    public HSPPredicate GroundToConstant(Dictionary<string, string> subst) {

        List<HSPTerm> args = new List<HSPTerm>();
        foreach(HSPTerm arg in _args) {
            
            if (subst.ContainsKey(arg.GetValue())) {
            
                string value = subst[arg.GetValue()];

                if (arg.IsTyped()) {
                    string type = subst[arg.GetTermType()];
                    HSPTerm _const = arg.constant(type, value);
                    args.Add(_const);
                }
                else {
                    HSPTerm _const = arg.constant(null, value);
                    args.Add(_const);
                }

            }
        }
        return new HSPPredicate(_name, args);
    }

    public string GetString() {
        
        if (_name.Equals('=')) {
            return _args[0].GetName() + " = " + _args[1].GetName();
        }
        else if (Arity() == 0) {
            return _name;
        }
        else {
            string sep = ", ";
            string[] val = new string[_args.Count];
            for (var i=0; i< _args.Count; i++) {
                val[i] = _args[i].GetValue();
            }
            return _name + "(" + String.Join( sep, val, 0, Arity() ) + ")";
        }

    }

    public int Arity() {
        return _args.Count;
    }

    public string GetName() {
        return _name;
    }      

    public List<HSPTerm> GetArgs() {
        return _args;
    }  

    public HashSet<string> GetGroundString() {
        return _grounds;
    } 



}