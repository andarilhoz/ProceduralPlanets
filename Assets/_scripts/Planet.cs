using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)] public int resolution = 10;
    public bool autoUpdate = true;

    public enum FaceRenderMask
    {
        All,
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    }

    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    private ShapeGenerator shapeGenerator = new ShapeGenerator();
    private ColourGenerator colourGenerator = new ColourGenerator();

    [SerializeField] public MeshFilter[] meshFilter;
    private TerrainFace[] terrainFaces;
    [HideInInspector] public bool shapeSettingsFoldout;
    [HideInInspector] public bool colourSettingsFoldout;

    private void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if ( meshFilter == null || meshFilter.Length == 0 )
        {
            meshFilter = new MeshFilter[6];
        }

        terrainFaces = new TerrainFace[6];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};


        for (var i = 0; i < 6; i++)
        {
            if ( meshFilter[i] == null )
            {
                var meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;


                meshObj.AddComponent<MeshRenderer>();
                meshFilter[i] = meshObj.AddComponent<MeshFilter>();
                meshFilter[i].sharedMesh = new Mesh();
            }

            meshFilter[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilter[i].sharedMesh, resolution, directions[i]);
            var renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
            meshFilter[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColour();
    }

    public void OnShapeSettingsUpdated()
    {
        if ( !autoUpdate )
        {
            return;
        }

        Initialize();
        GenerateMesh();
    }

    public void OnColourSettingsUpdated()
    {
        if ( !autoUpdate )
        {
            return;
        }

        Initialize();
        GenerateColour();
    }

    private void GenerateMesh()
    {
        for (var i = 0; i < 6; i++)
        {
            if ( meshFilter[i].gameObject.activeSelf )
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    private void GenerateColour()
    {
        colourGenerator.UpdateColours();
        for (var i = 0; i < 6; i++)
        {
            if ( meshFilter[i].gameObject.activeSelf )
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }
    }
}