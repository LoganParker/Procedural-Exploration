using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGen))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI(){
        MapGen mapGen = (MapGen)target;

        if(DrawDefaultInspector()){
            if(mapGen.autoUpdate){
                mapGen.GenerateMap();
            }
        }
        if(GUILayout.Button("Generate")){
            mapGen.GenerateMap();
        }
    }
}
