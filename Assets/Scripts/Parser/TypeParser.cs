
using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class TypeParser : IParser<HSPType>
{

    private static TypeParser instance;
    private TypeParser() { }
    public static TypeParser Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TypeParser();
            }
            return instance;
        }
    }

    public List<HSPType> ParseNode(JsonNode node) {

        List<HSPType> list = new List<HSPType>();

        foreach (var obj in node.Members)
        {
            HSPType newType = new HSPType(obj);
            list.Add(newType);
        }

        return list;

    }


}