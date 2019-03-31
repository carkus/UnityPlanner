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
            HSPPredicate predicate = new HSPPredicate(obj.Key, obj.Value.Members);
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
                HSPPredicate predicate = new HSPPredicate(obj.Key, obj.Value.Members);
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

}