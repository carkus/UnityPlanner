using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class ObjectManager : MonoBehaviour
{
    
    private Dictionary<string, List<Tuple<string, string, PrimitiveType?>>> objTuples;
    private IDictionary<string, OBase> objectList;

    //public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    //public float spawnTime = 3f;            // How long between each spawn.

    public NavMeshSurface surface;
    public NavMeshBaker navMeshBaker;


    void Awake()
    {
        navMeshBaker = GameObject.Find("main").AddComponent(typeof(NavMeshBaker)) as NavMeshBaker;
        
        objTuples = new Dictionary<string, List<Tuple<string, string, PrimitiveType?>>>();
        objectList = new Dictionary<string, OBase>();

        buildStaticWorld();

        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        //InvokeRepeating ("Spawn", spawnTime, spawnTime);

    }

    public void frameTick()
    {
        //if (awake) {

        //Debug.Log("I AM AWAKE!!!");

        //}
        // Set the displayed text to be the word "Score" followed by the score value.
        //text.text = "Score: " + score;
    }
    
    public void findObjects() {

        addPlanObject("room", "room1", PrimitiveType.Cube);
        addPlanObject("room", "room2", PrimitiveType.Cube);
        addPlanObject("room", "room3", PrimitiveType.Cube);
        addPlanObject("room", "room4", PrimitiveType.Cube);

        addPlanObject("object", "object1", PrimitiveType.Sphere);
        addPlanObject("object", "object2", PrimitiveType.Sphere);
        addPlanObject("object", "object3", PrimitiveType.Sphere);
        addPlanObject("object", "object4", PrimitiveType.Sphere);
    }


    public void setWorldObjects()
    {
        buildPlanObjects();
    }

    private void buildPlanObjects()
    {
        foreach (KeyValuePair<string, OBase> obj in objectList) {
            OType randomType = (OType)(UnityEngine.Random.Range(1, System.Enum.GetNames(typeof(OType)).Length));
            
            int randomX = (int)(UnityEngine.Random.Range(-15, 15));
            int randomZ = (int)(UnityEngine.Random.Range(-15, 15));

            (obj.Value).build();

            if ((obj.Value).getPrimitive().HasValue) {
                (obj.Value).setPosition(new Vector3(randomX, 0, randomZ));
                (obj.Value).setType(randomType);

                (obj.Value).makeObstacle();
                (obj.Value).redraw();
            }
        }
    }

    private void buildStaticWorld()
    {
        OBase plane = new OBase();
        plane.setPrimitive(PrimitiveType.Plane);
        plane.build();
        
        plane.setPosition(new Vector3(5f, 0, 5f));
        plane.setScale(new Vector3(10f, 1f, 10f));
        plane.redraw();

        NavMeshSurface surface = plane.getBody().AddComponent(typeof(NavMeshSurface)) as NavMeshSurface;

        navMeshBaker.addMeshSurface(surface);
        navMeshBaker.bakeNavMeshSurfaces();
    }

    public void addAgent() 
    {
        OBase agent = new OBase();
        agent.setPrimitive(PrimitiveType.Capsule);
        agent.build();

        agent.setName("agent");
        agent.setType(OType.Agent);
        agent.setPosition(new Vector3(0, 0, 0));
        agent.makeAgent();
        agent.redraw();
        
        addPlanObject("arm", "left", null);
        addPlanObject("arm", "right", null);
    }

    private void addPlanObject(string _type, string _name, PrimitiveType? _primitive)
    {
        Tuple<string, string, PrimitiveType?> obj = new Tuple<string, string, PrimitiveType?>(_type, _name, _primitive);
        if (!objTuples.ContainsKey(_type))
        {
            objTuples[_type] = new List<Tuple<string, string, PrimitiveType?>>();
        }
        objTuples[_type].Add(obj);
    }

    public void addPlanObjects()
    {
        foreach (KeyValuePair<string, List<Tuple<string, string, PrimitiveType?>>> types in objTuples)
        {
            foreach (Tuple<string, string, PrimitiveType?> obj in types.Value)
            {
                string name = obj.Item2;
                PrimitiveType? primitive = obj.Item3;
                string type = types.Key;

                List<HSPTerm> args = new List<HSPTerm>();
                args.Add(new HSPTerm(name, type, null));

                OBase newObject = new OBase();
                newObject.setPredicate(new HSPPredicate(type, args));
                newObject.setPrimitive(primitive);
                newObject.setName(name);
                
                objectList.Add(name, newObject);
            }
        }
    }

    public List<OBase> getObjectList()
    {
        List<OBase> objects = new List<OBase>();
        foreach (KeyValuePair<string, OBase> obj in objectList)
        {
            objects.Add(objectList[obj.Key]);
        }
        return objects;
    }

    void Spawn()
    {
        // If the player has no health left...
        /* /if(playerHealth.currentHealth <= 0f)
        {
            // ... exit the function.
            return;
        }

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range (0, spawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);*/
    }


}