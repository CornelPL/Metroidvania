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

    void Start()
    {
        mesh = meshFilter.mesh;

        constantPositions = mesh.vertices;
        vertices = mesh.vertices;
        UVs = mesh.uv;
        normals = mesh.normals;
        triangles = mesh.triangles;
    }

    void Update()
    {
        UpdateSprite();
    }

    void UpdateSprite()
    {
        vertices = mesh.vertices;

        currentPosition = transform.position;

        Vector2 positionChange = currentPosition - previousPosition;

        for (int i = 0; i < vertices.Length; i++)
        {
            float xChange = vertices[i].x - constantPositions[i].x;
            float yChange = vertices[i].y - constantPositions[i].y;
            vertices[i].x = constantPositions[i].x + spread * xChange;
            vertices[i].y = constantPositions[i].y + spread * yChange;
        }

        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.normals = normals;
        mesh.triangles = triangles;

        previousPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        int a, b, c;

        for (int i = 0; i < vertices.Length; i++)
        {
            a = triangles[i];
            b = triangles[i + 1];
            c = triangles[i + 2];
            Debug.DrawLine(vertices[a], vertices[b], Color.white, 100.0f);
            Debug.DrawLine(vertices[b], vertices[c], Color.white, 100.0f);
            Debug.DrawLine(vertices[c], vertices[a], Color.white, 100.0f);
        }
    }
}
