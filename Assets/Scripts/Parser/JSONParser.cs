using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using JsonToDataContract;

public class JSONParser
{
    
    enum readState {
        None,
        Type,
        Predicate,
        Action
    }
    readState iState;

    int iDepth;

    private TypeParser typeParser;
    private PredicateParser predicateParser;
    private OperatorParser operatorParser;

    private ObjectParser objectParser;
    private StateParser initStateParser;
    private StateParser goalStateParser;


    //DOMAIN
    public List<HSPType> types { get; set; }
    public List<HSPPredicate> predicates { get; set; }
    public List<HSPOperator> operations { get; set; }

    //PROBLEM INIT STATE
    public List<HSPPredicate> objects { get; set; }
    public List<HSPPredicate> state { get; set; }
    public List<HSPPredicate> goal { get; set; }

    public Action buildType { get; set; }

    List<JsonNode> domainNodes;
    List<JsonNode> problemNodes;

    public JSONParser() { 
        typeParser = TypeParser.Instance;
        predicateParser = PredicateParser.Instance;
        operatorParser = OperatorParser.Instance;

        objectParser = ObjectParser.Instance;
        initStateParser = StateParser.Instance;
        goalStateParser = StateParser.Instance;
    }

    public void ParseDomain(string ctxt) 
    {        
        domainNodes = new List<JsonNode>();
        domainNodes.Clear();
        domainNodes = ParseJSONFilename(ctxt);
    } 

    public void ParseProblem(string ctxt) 
    {        
        problemNodes = new List<JsonNode>();
        problemNodes.Clear();
        problemNodes = ParseJSONFilename(ctxt);
    }

    public List<JsonNode> ParseJSONFilename(string ctxt)
    {

        var list = new List<JsonNode>();
        
        try
        {
            StreamReader streamReader = new StreamReader(@ctxt);
            JsonTextReader reader = new JsonTextReader(streamReader);
            JObject obj = (JObject) JToken.ReadFrom(reader);
            foreach (var branch in obj)
            {
                JsonNode node = JsonNode.ParseJTokenToNode(branch.Value, branch.Key);
                GetNodeTreeFromKey(node, branch.Key);   
                list.Add(node);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(" >>> Exception: " + ex);
        }
        finally
        {
        }
        
        return list;

    }


    private void GetNodeTreeFromKey(JsonNode node, string key) {

        switch (key) {
            case Config.Constants.JSON_NODELABEL_TYPES:
                types = new List<HSPType>( typeParser.ParseNode(node) );
            break;
            case Config.Constants.JSON_NODELABEL_PREDICATES:
                predicates = new List<HSPPredicate>( predicateParser.ParseNode(node) );
            break;
            case Config.Constants.JSON_NODELABEL_OPERATORS:
                operations = new List<HSPOperator>( operatorParser.ParseNode(node) );
            break;
            case Config.Constants.JSON_NODELABEL_OBJECTS:
                objects = new List<HSPPredicate>( objectParser.ParseNode(node) );
            break;
            case Config.Constants.JSON_NODELABEL_STATEINIT:
                state = new List<HSPPredicate>( initStateParser.ParseNode(node) );
            break;        
            case Config.Constants.JSON_NODELABEL_STATEGOAL:
                goal = new List<HSPPredicate>( goalStateParser.ParseNode(node) );
            break;
        }

    }

}

public class ParseType
{
    public Action buildType { get; set; }
}