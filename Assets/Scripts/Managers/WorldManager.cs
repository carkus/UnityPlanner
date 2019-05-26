using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldManager
{
    public static int score;        // The player's score.

    public OBase[] worldObjects {
        get;
        set;        
    }

    public bool awake;

    //Text text;                      // Reference to the Text component.

    public void startWorld()
    {
        awake = true;
    }


    public void frameTick()
    {

        if (awake) {

            //Debug.Log("I AM AWAKE!!!");

        }
        // Set the displayed text to be the word "Score" followed by the score value.
        //text.text = "Score: " + score;
    }


    T[] InitializeArray<T>(int length) where T : new()
    {
        T[] array = new T[length];
        for (int i = 0; i < length; ++i)
        {
            array[i] = new T();
        }
        return array;
    }
    
        
}