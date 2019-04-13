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
            List<HSPTerm> args = DeriveArgs(obj.Value.Members);
            HSPPredicate predicate = new HSPPredicate(obj.Key, args);
            list.Add(predicate);
        }

        return list;

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


}