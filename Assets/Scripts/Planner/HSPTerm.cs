using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPTerm {

    public string _name { get; set; }
    public string _type { get; set; }

    private string _value;

    public HSPTerm (string name, string type, string value) {
        _name = name;
        _type = type;
        _value = value;
    }    

    public HSPTerm constant() {
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

}