using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class OperatorParser : IParser<HSPOperator>
{

    private static OperatorParser instance;
    private OperatorParser() { }
    public static OperatorParser Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new OperatorParser();
            }
            return instance;
        }
    }


    public List<HSPOperator> ParseNode(JsonNode node)
    {

        List<HSPOperator> list = new List<HSPOperator>();

        foreach (var obj in node.Members)
        {
            HSPOperator objList = new HSPOperator(obj);
            list.Add(objList);
        }

        return list;

    }


}