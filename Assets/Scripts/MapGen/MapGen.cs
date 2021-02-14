using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    //Editor Variables
    public enum DrawMode{NoiseMap,ColorMap,Mesh}
    [SerializeField]private DrawMode drawMode;
    public bool autoUpdate;
    public AnimationCurve meshHeightCurve;
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

    //Generates map
    public void GenerateMap(){
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
        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        if(drawMode==DrawMode.NoiseMap){
            display.DrawTexture(TextureGen.TextureFromHeightMap(noiseMap));
        }else if (drawMode == DrawMode.ColorMap){
            display.DrawTexture(TextureGen.TextureFromColorMap(colorMap,mapChunkSize,mapChunkSize));
        }else if(drawMode == DrawMode.Mesh){
            display.DrawMesh(MeshGen.GenerateTerrainMesh(noiseMap,meshHeightMultiplier,meshHeightCurve,levelOfDetail), TextureGen.TextureFromColorMap(colorMap,mapChunkSize,mapChunkSize));
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
}

[System.Serializable] 
public struct TerrainType{
    public string name;
    public float height;
    public Color color;
}