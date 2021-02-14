using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGen : MonoBehaviour
{
    //Editor Variables
    public enum DrawMode{NoiseMap,ColorMap,Mesh}
    [SerializeField]private DrawMode drawMode;
    public bool autoUpdate;
    public AnimationCurve meshHeightCurve;
    //

    //Threading
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    //

    //Map Parameters
    //Mesh is 240x240
    public const int mapChunkSize = 241;
    [SerializeField][Range(0,6)]int levelOfDetail;
    [SerializeField]private float meshHeightMultiplier;
    [SerializeField]private int seed;
    [SerializeField]private float noiseScale;
    [SerializeField]private int octaves;
    [SerializeField][Range(0,1)]private float persistance;
    [SerializeField]private float lacunarity;    
    [SerializeField]private Vector2 offset;
    //
    
    public TerrainType[] regions;



    // MAP GENERATION
    MapData GenerateMapData(){
        // Get noise map
        float[,] noiseMap = NoiseGen.GenerateNoiseMap(mapChunkSize,mapChunkSize,seed,noiseScale,octaves,persistance,lacunarity,offset);
        
        //Assign Terrain based on height
        Color[] colorMap = new Color[mapChunkSize*mapChunkSize];
        for(int x = 0;x < mapChunkSize; x++){
            for(int y = 0;y < mapChunkSize; y++){
                float currentHeight = noiseMap[x,y];

                for(int i = 0;i < regions.Length; i++){
                    if(currentHeight<= regions[i].height){
                        colorMap[y*mapChunkSize+x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        
        return new MapData(noiseMap, colorMap);
    }
    public void DrawMapInEditor(){
        MapData mapData = GenerateMapData();
        
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode==DrawMode.NoiseMap){
            display.DrawTexture(TextureGen.TextureFromHeightMap(mapData.heightMap));
        }else if (drawMode == DrawMode.ColorMap){
            display.DrawTexture(TextureGen.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
        }else if(drawMode == DrawMode.Mesh){
            display.DrawMesh(MeshGen.GenerateTerrainMesh(mapData.heightMap,meshHeightMultiplier,meshHeightCurve,levelOfDetail), TextureGen.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
        }
    }

    //Ensure correct Values
    private void OnValidate() {
        if(lacunarity<1){
            lacunarity=1;
        }
        if(octaves<0){
            octaves = 0;
        }
        
    }
    //


    //THREADING
    public void RequestMapData(Action<MapData> callback){
        //Start our thread
        ThreadStart threadStart = delegate{
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
    }
    void MapDataThread(Action<MapData> callback){
        MapData mapData = GenerateMapData();
        //Prevent other threads from executing this code at once
        lock(mapDataThreadInfoQueue){
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback,mapData));
        }
    }
    public void RequestMeshData(MapData mapData, Action<MeshData> callback){
        //Start our thread
        ThreadStart threadStart = delegate{
            MeshDataThread(mapData,callback);
        };
        new Thread(threadStart).Start();
    }
    void MeshDataThread(MapData mapData, Action<MeshData> callback){
        MeshData meshData = MeshGen.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier,meshHeightCurve,levelOfDetail);
        //Prevent other threads from executing this code at once
        lock(meshDataThreadInfoQueue){
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback,meshData));
        }
    }

    // Generic to handle map and mesh data
    struct MapThreadInfo<T>{
        
        public readonly Action<T> callback;
        public readonly T parameter;
        
        public MapThreadInfo(Action<T> callback, T parameter){
            this.callback = callback;
            this.parameter = parameter;
        }

    }
    void Update(){
        if(mapDataThreadInfoQueue.Count > 0){
            for(int i = 0;i < mapDataThreadInfoQueue.Count; i++){
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if(meshDataThreadInfoQueue.Count > 0){
            for(int i = 0; i < meshDataThreadInfoQueue.Count; i++){
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    //
}

[System.Serializable] 
public struct TerrainType{
    public string name;
    public float height;
    public Color color;
}

public struct MapData{
    
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;
    
    public MapData(float[,] heightMap,Color[] colorMap){
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}
