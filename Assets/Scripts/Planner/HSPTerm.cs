using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPTerm {

    private string _name;
    private string _type;
    private string _value;

    public HSPTerm (string name = null, string type = null, string value = null) {
        _name = name;
        _type = type;
        _value = value;
    }

    public bool isEqualTo(HSPTerm other) {
        return true;
    }

    public HSPTerm constant(string _type = null, string _value = null) {
        return new HSPTerm(_name, _type, _value);
    }

    public bool IsVariable() {
        return _name != null;
    }

    public bool IsTyped() {
        return _type != null;
    }

    public bool IsConstant() {
        return _value != null;
    }

    public string GetTermType() {
        return _type;
    } 

    public string GetName() {
        return _name;
    }   

    public string GetValue() {
        return _value;
    }

}