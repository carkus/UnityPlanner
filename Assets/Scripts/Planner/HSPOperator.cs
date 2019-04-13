using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPOperator
{

    public string _name;
    public List<HSPTerm> _params { get; set; }
    public List<HSPLiteral> _preconditions { get; set; }
    public List<HSPLiteral> _effects { get; set; }

    public HSPOperator(KeyValuePair<string, JsonNode> obj)
    {

        _name = obj.Key;
        _params = ParseParameters(obj.Value.Members["parameters"]);
        _preconditions = ParsePrecondition(obj.Value.Members["precondition"]);
        _effects = ParseEffect(obj.Value.Members["effect"]);

    }

    private List<HSPTerm> ParseParameters(JsonNode node)
    {

        List<HSPTerm> items = new List<HSPTerm>();

        foreach (var obj in node.Members)
        {
            foreach (var param in obj.Value.Members)
            {
                HSPTerm item = new HSPTerm(obj.Key, param.Key, null);
                items.Add(item);
            }
        }

        return items;

    }

    private List<HSPLiteral> ParsePrecondition(JsonNode node)
    {

        List<HSPLiteral> items = new List<HSPLiteral>();

        foreach (var obj in node.Members)
        {
            List<HSPTerm> args = DeriveArgs(obj.Value.Members);
            HSPPredicate predicate = new HSPPredicate(obj.Key, args);
            if (obj.Key == "*not") {
                HSPLiteral item = new HSPLiteral(predicate, false);
                items.Add(item);
            }
            else {
                HSPLiteral item = new HSPLiteral(predicate, true);
                items.Add(item);
            }
        }

        return items;

    }

    private List<HSPLiteral> ParseEffect(JsonNode node)
    {

        List<HSPLiteral> items = new List<HSPLiteral>();

        foreach (var type in node.Members)
        {
            foreach (var obj in type.Value.Members)
            {
                List<HSPTerm> args = DeriveArgs(obj.Value.Members);
                HSPPredicate predicate = new HSPPredicate(obj.Key, args);
                HSPLiteral item;

                if (type.Key == "positive")
                {
                    item = new HSPLiteral(predicate, true);
                    items.Add(item);
                }
                else if (type.Key == "negative")
                {
                    item = new HSPLiteral(predicate, false);
                    items.Add(item);
                }

            }
        }

        return items;

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