using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OBase
{

    protected GameObject body;
    protected OMovement mover;

    protected ODefinition definition = new ODefinition();
    protected OColor baseColor = new OColor();

    protected SphereCollider collider;
    protected NavMeshAgent navAgent;
    protected NavMeshObstacle navObstacle;

    private HSPPredicate predicate;
    private PrimitiveType? primitive;
    private Renderer renderer;

    private string name;
    private Vector3 position = new Vector3(0, 0, 0);
    private Vector3 scale = new Vector3(1f, 1f, 1f);

    public OBase() { 
    }

    public void build()
    {
        if (primitive.HasValue)
        {
            body = GameObject.CreatePrimitive(primitive.Value);
            renderer = body.GetComponent<Renderer>();
            renderer.material.color = baseColor.colorSet[0];
            //redraw();
        }
    }

    public void makeAgent()
    {
        collider = body.AddComponent(typeof(SphereCollider)) as SphereCollider;
        navAgent = body.AddComponent<NavMeshAgent>();
        mover = body.AddComponent(typeof(OMovement)) as OMovement;
    }

    public void makeObstacle()
    {
        collider = body.AddComponent(typeof(SphereCollider)) as SphereCollider;
        navObstacle = body.AddComponent<NavMeshObstacle>();
        navObstacle.carving = true;
    }

    public void setDestination(Vector3 destination)
    {
        mover.SetDestination(destination);
    }

    public GameObject getBody()
    {
        return body;
    }

    public HSPPredicate getPredicate()
    {
        return predicate;
    }

    public void setPredicate(HSPPredicate _predicate)
    {
        predicate = _predicate;
    }

    public void setPrimitive(PrimitiveType? _primitive)
    {
        primitive = _primitive;
    }

    public PrimitiveType? getPrimitive()
    {
        return primitive;
    }

    public void setPosition(Vector3 _value)
    {
        position = _value;
    }

    public void setScale(Vector3 _value)
    {
        scale = _value;
    }

    public void setName(string _value)
    {
        name = _value;
    }

    public void setType(OType type)
    {
        definition._type = type;
    }

    public void redraw()
    {
        renderer.material.color = baseColor.colorSet[(int)definition._type];
        body.transform.localScale = scale;
        body.transform.position = position;
    }

    public string getName()
    {
        return name;
    }

}

