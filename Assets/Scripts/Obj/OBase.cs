using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OBase
{

    protected GameObject body;
    protected OMovement mover;
    
    //public Color baseColor = Color.white;

    protected ODefinition definition = new ODefinition();
    protected OColor baseColor = new OColor();

    protected SphereCollider collider;
    protected NavMeshAgent navAgent;
    protected NavMeshObstacle navObstacle;


    public Renderer renderer;

    public OBase (PrimitiveType type) {
        
        body = GameObject.CreatePrimitive(type);

        renderer = body.GetComponent<Renderer>();
        renderer.material.color = baseColor.colorSet[0];

        mover = body.AddComponent(typeof(OMovement)) as OMovement;        
    }

    public void makeAgent() {
        collider = body.AddComponent(typeof(SphereCollider)) as SphereCollider;
        navAgent = body.AddComponent<NavMeshAgent>();
    }

    public void makeObstacle() {
        collider = body.AddComponent(typeof(SphereCollider)) as SphereCollider;
        navObstacle = body.AddComponent<NavMeshObstacle>();
        navObstacle.carving = true;
    }

    public void SetDestination() {
        mover.SetDestination();
    }

    public GameObject getBody()
    {
        return body;
    }
 

    public void setPosition(Vector3 pos)
    {
        body.transform.position = pos;
    }

    public void setScale(float _x, float _y, float _z)
    {
        body.transform.localScale = new Vector3(_x, _y, _z);
    }    

    public void setType(OType type)
    {
        definition._type = type;
    }

    public void setColor(OType type)
    {
        //baseColor.g += 0.1f;

        renderer.material.color = baseColor.colorSet[(int)type];
    }

}

