using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TestFloors))]
public class TestFloorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = target as TestFloors;

        if (GUILayout.Button("FindPipes"))
        {
            script.FindPipes();
        }

        if (GUILayout.Button("FindAllPipes"))
        {
            script.FindAllPipes();
        }
    }
}
#endif


[Serializable]
public struct TestPipeFloor
{
    public Pipe[] pipes;

    public TestPipeFloor(IEnumerable<Pipe> pipes)
    {
        this.pipes = pipes.ToArray();
    }
}

public class TestFloors : MonoBehaviour
{
    public int floor;
    public Pipe[] pipesFound;

    [Space]
    public TestPipeFloor[] allPipes;
    
    public void FindPipes()
    {
        pipesFound = FloorManager.Instance.FindObjectsOnFloor<Pipe>(floor).ToArray();
    }

    public void FindAllPipes()
    {
        allPipes = FloorManager.Instance.GetObjectsByFloors<Pipe>().Select(pipes => new TestPipeFloor(pipes)).ToArray();
    }
}