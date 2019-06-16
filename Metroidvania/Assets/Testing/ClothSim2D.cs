using UnityEngine;

public class ClothSim2D : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter = null;
    [SerializeField] private float spread = 0.5f;

    private Mesh mesh;
    private Vector2 currentPosition;
    private Vector2 previousPosition;
    private Vector3[] constantPositions;
    private Vector3[] vertices;
    private Vector2[] UVs;
    private Vector3[] normals;
    private int[] triangles;

    private void Start()
    {
        mesh = meshFilter.sharedMesh;

        constantPositions = mesh.vertices;
        vertices = mesh.vertices;
        UVs = mesh.uv;
        normals = mesh.normals;
        triangles = mesh.triangles;
    }

    private void Update()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        vertices = mesh.vertices;

        currentPosition = transform.position;

        Vector2 positionChange = currentPosition - previousPosition;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = constantPositions[i].x - spread * positionChange.x;
            vertices[i].y = constantPositions[i].y - spread * positionChange.y;
        }
        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.normals = normals;
        mesh.triangles = triangles;

        previousPosition = transform.position;
    }
}
