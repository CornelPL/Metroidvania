using UnityEngine;

public class ClothSim2D : MonoBehaviour
{
    [SerializeField] private int verticalNodesCount = 9;
    [SerializeField] private Transform anchor = null;
    [SerializeField] private Transform[] rope = null;
    [SerializeField] private Transform[] referencePoints = null;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Vector2 gravity = new Vector2(0f, -100f);
    [SerializeField] private float restDistance = 0.25f;
    [SerializeField] private float damping = 0.5f;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] UVs;
    private Vector3[] normals;
    private int[] triangles;
    private Vector2[] previousRopePositions;

    private Vector2 previousPosition;


    private void Start()
    {
        mesh = new Mesh();

        previousRopePositions = new Vector2[rope.Length];
        for (int i = 0; i < previousRopePositions.Length; i++)
        {
            previousRopePositions[i] = rope[i].position;
        }

        vertices = new Vector3[verticalNodesCount * 2];
        UVs = new Vector2[verticalNodesCount * 2];
        normals = new Vector3[verticalNodesCount * 2];
        triangles = new int[(verticalNodesCount - 1) * 6];

        for (int i = 0; i < vertices.Length; i += 2)
        {
            float y = (float)i / (float)(verticalNodesCount - 1) / 2f;
            vertices[i].x = -0.5f;
            vertices[i].y = y - 0.5f;
            vertices[i + 1].x = 0.5f;
            vertices[i + 1].y = y - 0.5f;

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
        // first we need to rotate whole sprite with movement direction
        Vector2 movementDirection = (Vector2)anchor.position - previousPosition;
        if (movementDirection.magnitude > 0.05f)
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg - 90f;
            float curAngle = anchor.rotation.eulerAngles.z;
            curAngle = curAngle > 180f ? curAngle - 360f : curAngle;
            float angleDiff = angle - curAngle;
            if (Mathf.Abs(angleDiff) > 5f)
            {
                anchor.Rotate(Vector3.forward * angleDiff * Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            float angleDiff = anchor.eulerAngles.z - 180f;
            if (angleDiff + 180f > 5f)
            {
                anchor.Rotate(Vector3.forward * angleDiff * Time.deltaTime * rotationSpeed);
            }
        }

        // then we need to move rope points towards anchor
        rope[verticalNodesCount - 1].position = anchor.position;

        // we start from -2 because first point is on the anchor
        for (int i = verticalNodesCount - 2; i >= 0; i--)
        {
            Transform r = rope[i];
            Vector2 velocity = (Vector2)r.position - previousRopePositions[i];
            float dt = Time.deltaTime;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Debug.Log(velocity);
            Debug.Log(angle);
            r.position = (Vector2)r.position + velocity * damping + gravity * dt * dt;

            Vector2 direction = r.position - rope[i + 1].position;
            r.position = (Vector2)rope[i + 1].position + direction.normalized * restDistance;

            previousRopePositions[i] = r.position;
        }

        // then we need to rotate rope points towards edge between points

        // then setup sprite as previous

        previousPosition = anchor.position;

        /*for (int i = 0; i < vertices.Length - 2; i += 2)
        {
            Transform refPoint = referencePoints[i / 2];
            Transform ropePoint = rope[i / 2];

            if (ropePoint.position.x > rope[verticalNodesCount - 1].position.x)
            {
                ropePoint.position = new Vector3(rope[verticalNodesCount - 1].position.x, ropePoint.position.y, ropePoint.position.z);
            }
        }

        for (int i = verticalNodesCount - 1; i >= 0; i--)
        {
            if (i == verticalNodesCount - 1)
            {
                rope[i].position = anchor.position;
            }
            else
            {
                Vector2 direction = rope[i].position - rope[i + 1].position;
                rope[i].position = (Vector2)rope[i + 1].position + direction.normalized * 0.25f;
            }
        }

        for (int i = 0; i < vertices.Length; i += 2)
        {
            Transform refPoint = referencePoints[i / 2];
            Transform ropePoint = rope[i / 2];

            Vector2 pos = ropePoint.position;
            Vector2 pos1 = pos + (Vector2)ropePoint.TransformDirection(-0.5f, 0f, 0f);
            Vector2 pos2 = pos + (Vector2)ropePoint.TransformDirection(0.5f, 0f, 0f);
            pos1 = refPoint.InverseTransformPoint(pos1);
            pos2 = refPoint.InverseTransformPoint(pos2);
            pos1.y += (refPoint.localPosition.y) / 2f + 0.5f;
            pos2.y += (refPoint.localPosition.y) / 2f + 0.5f;

            vertices[i] = pos1;
            vertices[i + 1] = pos2;
            UVs[i] = new Vector2(pos1.x + 0.5f, pos1.y + 0.5f);
            UVs[i + 1] = new Vector2(pos2.x + 0.5f, pos2.y + 0.5f);
        }

        mesh.vertices = vertices;*/
    }
}
