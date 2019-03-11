using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPType
{

    private string _name;
    
    public HSPType (KeyValuePair<string, JsonNode> obj) {
        _name = obj.Key;        
    }

}