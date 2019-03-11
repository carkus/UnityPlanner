using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class ObjectParser : IParser<HSPPredicate>
{

    private static ObjectParser instance;
    private ObjectParser() { }
    public static ObjectParser Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectParser();
            }
            return instance;
        }
    }

    public List<HSPPredicate> ParseNode(JsonNode node)
    {

        List<HSPPredicate> list = new List<HSPPredicate>();

        foreach (var obj in node.Members) {
            HSPPredicate predicate = new HSPPredicate(obj.Key, obj.Value.Members);
            list.Add(predicate);
        }

        return list;

    }   

}