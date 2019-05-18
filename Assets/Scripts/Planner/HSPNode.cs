using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HSPNode : IComparable<HSPNode> {

    private HashSet<string> _state;
    private HSPAction _action ;
    private HSPNode _parent;
    private string _stateString;
    
    private int _f;
    private int _g;
    private int _h;

    public int CompareTo(HSPNode other) {
        if (this.GetF() > other.GetF()) {
            return 1;
        }
        return 0;
    }

    public HSPNode (HashSet<string> state, HSPAction action, HSPNode parent = null, int g = 0, int h = 0) {
        _state = state;
        _action = action;
        _parent = parent;
        _g = g;
        _h = h;
        _stateString = setStateString();
    }

    public HashSet<string> GetState() {
        HashSet<string> state = new HashSet<string>();
        foreach(string predicate in _state) {
            state.Add(predicate);
        }
        return state;
    }

    /*public HSPState GetState() {
        return (HSPState)_state.Clone();
    }*/

    public HSPAction GetAction() {
        return _action;
    }

    public string getStateString() {
        return _stateString;
    }

    private string setStateString() {
        string sep = ", ";
        string[] output = new string[_state.Count];
        output = _state.ToArray();
        Array.Sort(output);
        return "( " + String.Join( sep, output ) + " )";
    }    

    public List<HSPNode> getPath() {

        List<HSPNode> path = new List<HSPNode>();
        HSPNode node = this;
        
        while (node._parent != null) {
            path.Add(node);
            node = node._parent;
        }
        
        return path;
    }

    public int GetF() {
        return _g + _h;
    }

    public int GetG() {
        return _g;
    }

    public int GetH() {
        return _h;
    }

}