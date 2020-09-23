using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dice
{
    private ArrayList _possibilities;

    public Dice()
    {
        _possibilities = new ArrayList();
    }



    public void add<T>(T a, int weight)
    {
        for (int i = 0; i < weight; i++)
        {
            _possibilities.Add(a);
        }
    }

    public T roll<T>()
    {
        int index = Random.Range(0, _possibilities.Count);
//        Debug.Log(_possibilities.Count);
        return (T) _possibilities[index];
    }
}


