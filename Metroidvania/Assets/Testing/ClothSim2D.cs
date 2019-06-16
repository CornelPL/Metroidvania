using UnityEngine;

public class ClothSim2D : MonoBehaviour
{
    [SerializeField] private int verticalNodesCount = 5;
    [SerializeField] private float spread = 0.5f;
    [SerializeField] private Transform line = null;
    [SerializeField] private Transform[] rope = new Transform[5];
    [SerializeField] private Transform[] referencePoints = new Transform[5];

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
        //mesh = GetComponent<MeshFilter>().sharedMesh;
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

        /*constantPositions = mesh.vertices;
        vertices = mesh.vertices;
        UVs = mesh.uv;
        normals = mesh.normals;
        triangles = mesh.triangles;*/

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
        /*vertices = mesh.vertices;

        currentPosition = transform.position;

        Vector2 positionChange = currentPosition - previousPosition;

        for (int i = 0; i < vertices.Length; i += 2)
        {
            vertices[i].x = constantPositions[i].x - spread * positionChange.x / ((i + 1) / 5f);
            vertices[i].y = constantPositions[i].y - spread * positionChange.y / ((i + 1) / 5f);
            vertices[i + 1].x = constantPositions[i + 1].x - spread * positionChange.x / ((i + 1) / 5f);
            vertices[i + 1].y = constantPositions[i + 1].y - spread * positionChange.y / ((i + 1) / 5f);
        }
        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.normals = normals;
        mesh.triangles = triangles;

        previousPosition = transform.position;*/

        for (int i = 0; i < vertices.Length; i += 2)
        {
            Vector2 pos = rope[i / 2].position;
            Vector2 pos1 = pos + (Vector2)referencePoints[i / 2].TransformDirection(-0.5f, 0f, 0f);
            Vector2 pos2 = pos + (Vector2)referencePoints[i / 2].TransformDirection(0.5f, 0f, 0f);
            //pos1 = rope[i / 2].TransformPoint(pos1);
            //pos2 = rope[i / 2].TransformPoint(pos2);
            pos1 = referencePoints[i / 2].InverseTransformPoint(pos1);
            pos2 = referencePoints[i / 2].InverseTransformPoint(pos2);
            //pos1.y += (referencePoints[i / 2].localPosition.y - 0.5f) / 2f + 0.5f;
            //pos2.y += (referencePoints[i / 2].localPosition.y - 0.5f) / 2f + 0.5f;

            vertices[i].x = pos1.x;
            vertices[i + 1].x = pos2.x;
            /*vertices[i].x = childs[i].position.x -
            vertices[i].y = constantPositions[i].y - spread * positionChange.y / ((i + 1) / 5f);
            vertices[i + 1].x = constantPositions[i + 1].x - spread * positionChange.x / ((i + 1) / 5f);
            vertices[i + 1].y = constantPositions[i + 1].y - spread * positionChange.y / ((i + 1) / 5f);*/
        }

        mesh.vertices = vertices;
    }
}
