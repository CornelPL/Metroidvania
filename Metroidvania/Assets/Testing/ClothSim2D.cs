using UnityEngine;

public class ClothSim2D : MonoBehaviour
{
    [SerializeField] private int verticalNodesCount = 9;
    [SerializeField] private Transform[] rope = new Transform[9];
    [SerializeField] private Transform[] referencePoints = new Transform[9];

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
        mesh = new Mesh();

        constantPositions = new Vector3[verticalNodesCount * 2];
        vertices = new Vector3[verticalNodesCount * 2];
        UVs = new Vector2[verticalNodesCount * 2];
        normals = new Vector3[verticalNodesCount * 2];
        triangles = new int[(verticalNodesCount - 1) * 6];

        for (int i = 0; i < vertices.Length; i += 2)
        {
            float y = (float)i / (float)(verticalNodesCount - 1) / 2f;
            constantPositions[i].x = -0.5f;
            constantPositions[i].y = y - 0.5f;
            constantPositions[i + 1].x = 0.5f;
            constantPositions[i + 1].y = y - 0.5f;

            vertices = constantPositions;

            UVs[i].x = 0f;
            UVs[i].y = y;
            UVs[i + 1].x = 1f;
            UVs[i + 1].y = y;

            normals[i] = normals[i + 1] = new Vector3(0f, 0f, -1f);
        }

        for (int i = 0, v = 0; v < vertices.Length - 2; i += 6, v += 2)
        {
            triangles[i] = v;
            triangles[i + 1] = v + 3;
            triangles[i + 2] = v + 1;
            triangles[i + 3] = v + 3;
            triangles[i + 4] = v;
            triangles[i + 5] = v + 2;
        }

        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.normals = normals;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
    }

    private void Update()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        for (int i = 0; i < vertices.Length; i += 2)
        {
            Transform refPoint = referencePoints[i / 2];
            Transform ropePoint = rope[i / 2];
            Vector2 pos = ropePoint.position;
            Vector2 pos1 = pos + (Vector2)ropePoint.TransformDirection(-0.5f, 0f, 0f);
            Vector2 pos2 = pos + (Vector2)ropePoint.TransformDirection(0.5f, 0f, 0f);
            pos1 = refPoint.InverseTransformPoint(pos1);
            pos2 = refPoint.InverseTransformPoint(pos2);
            pos1.y += (refPoint.localPosition.y - 0.25f) / 2f + 0.5f;
            pos2.y += (refPoint.localPosition.y - 0.25f) / 2f + 0.5f;

            vertices[i] = pos1;
            vertices[i + 1] = pos2;
            UVs[i] = new Vector2(pos1.x + 0.5f, pos1.y + 0.5f);
            UVs[i + 1] = new Vector2(pos2.x + 0.5f, pos2.y + 0.5f);
        }

        mesh.vertices = vertices;
    }
}
