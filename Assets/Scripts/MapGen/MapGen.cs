using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

    public enum DrawMode{NoiseMap,ColorMap,Mesh}
    [SerializeField]private DrawMode drawMode;
    public bool autoUpdate;

    //Map Parameters
    [SerializeField]private int mapWidth;
    [SerializeField]private int mapHeight;
    [SerializeField]private int seed;
    [SerializeField]private float noiseScale;
    [SerializeField]private int octaves;
    [SerializeField][Range(0,1)]private float persistance;
    [SerializeField]private float lacunarity;    
    [SerializeField]private Vector2 offset;
    //
    
    public TerrainType[] regions;

    //Generates our map
    public void GenerateMap(){
        // Get our noise map
        float[,] noiseMap = NoiseGen.GenerateNoiseMap(mapWidth,mapHeight,seed,noiseScale,octaves,persistance,lacunarity,offset);
        
        //Assign Terrain based on height
        Color[] colorMap = new Color[mapWidth*mapHeight];
        for(int x = 0;x < mapWidth; x++){
            for(int y = 0;y < mapHeight; y++){
                float currentHeight = noiseMap[x,y];

                for(int i = 0;i < regions.Length; i++){
                    if(currentHeight<= regions[i].height){
                        colorMap[y*mapWidth+x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        if(drawMode==DrawMode.NoiseMap){
            display.DrawTexture(TextureGen.TextureFromHeightMap(noiseMap));
        }else if (drawMode == DrawMode.ColorMap){
            display.DrawTexture(TextureGen.TextureFromColorMap(colorMap,mapWidth,mapHeight));
        }else if(drawMode == DrawMode.Mesh){
            display.DrawMesh(MeshGen.GenerateTerrainMesh(noiseMap), TextureGen.TextureFromColorMap(colorMap,mapWidth,mapHeight));
        }
        
    }
    

    //Ensure correct Values
    private void OnValidate() {
        if(mapWidth < 1){
            mapWidth = 1;
        }    
        if(mapHeight < 1){
            mapHeight = 1;
        }
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