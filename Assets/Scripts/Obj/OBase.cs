using System.Collections;
using UnityEngine;

public class OBase
{

    protected GameObject form;
    
    //public Color baseColor = Color.white;

    protected ODefinition definition = new ODefinition();
    protected OColor baseColor = new OColor();


    public Renderer renderer;

    public OBase () {
        PrimitiveType primitive = PrimitiveType.Cube;
        form = GameObject.CreatePrimitive(primitive);
        renderer = form.GetComponent<Renderer>();
        renderer.material.color = baseColor.colorSet[0];
    }

    public void setPosition(Vector3 pos)
    {
        form.transform.position = pos;
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

