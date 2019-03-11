using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class PredicateParser : IParser<HSPPredicate>
{

    private static PredicateParser instance;
    private PredicateParser() { }
    public static PredicateParser Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PredicateParser();
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