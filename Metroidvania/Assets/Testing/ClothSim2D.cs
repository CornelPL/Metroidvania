using UnityEngine;

public class ClothSim2D : MonoBehaviour
{
    [SerializeField] private int verticalNodesCount = 9;
    [SerializeField] private Transform[] capePoints = null;
    [SerializeField] private Transform[] referencePoints = null;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Vector2 gravity = new Vector2(0f, -100f);
    [SerializeField] private float restDistance = 0.25f;
    [SerializeField] private float damping = 0.5f;
    [SerializeField] private float maxAngleDeviation = 10f;
    [SerializeField] private float noiseFrequency = 3f;
    [SerializeField] private float noiseMultiplier = 0.05f;
    [SerializeField] private float moveNoiseFrequency = 5f;
    [SerializeField] private float moveNoiseMultiplier = 0.3f;
    [SerializeField] private int relaxation = 5;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] UVs;
    private Vector3[] normals;
    private int[] triangles;
    private Vector2[] previousCapePointsPositions;
    private Transform anchor;
    private Vector2 previousAnchorPosition;


    private void Start()
    {
        InitMesh();
        InitCape();
    }


    private void Update()
    {
        RotateWithMovement();
        CalculatePositions();
        CalculateAngles();
        RelaxEdges();
        UpdateSprite();
    }


    private void InitMesh()
    {
        mesh = new Mesh();

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


    private void InitCape()
    {
        previousCapePointsPositions = new Vector2[capePoints.Length];
        for (int i = 0; i < previousCapePointsPositions.Length; i++)
        {
            previousCapePointsPositions[i] = capePoints[i].position;
        }

        anchor = capePoints[0];
    }


    // Rotates sprite and reference points towards movement direction
    private void RotateWithMovement()
    {
        Vector2 movementDirection = (Vector2)anchor.position - previousAnchorPosition;
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
    }


    private void CalculatePositions()
    {
        for (int i = 1; i < verticalNodesCount; i++)
        {
            Transform r = capePoints[i];
            Vector2 velocity = (Vector2)r.position - previousCapePointsPositions[i];
            float dt = Time.deltaTime;

            previousCapePointsPositions[i] = r.position;

            r.position = (Vector2)r.position + velocity * damping + gravity * dt * dt;
        }

        previousAnchorPosition = anchor.position;
    }


    private void CalculateAngles()
    {
        for (int i = 1; i < verticalNodesCount; i++)
        {
            Transform capePoint = capePoints[i];

            //Vector2 direction = r.position - capePoints[i - 1].position;
            //float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg - 90f;
            /*if (angle > -45f)
            {
                float rand = (Mathf.PerlinNoise(Time.time * noiseFrequency, 0f) - 0.5f) * noiseMultiplier;
                direction += new Vector2(rand / 10f, 0f);
            }
            if (angle < -45f && angle > -120f)
            {
                float rand = (Mathf.PerlinNoise(Time.time * moveNoiseFrequency, 0f) - 0.5f) * moveNoiseMultiplier;
                direction += new Vector2(0f, rand);
            }
            else if (angle < -120f)
            {
                float rand = (Mathf.PerlinNoise(Time.time * moveNoiseFrequency, 0f) - 0.5f) * moveNoiseMultiplier;
                direction += new Vector2(rand, 0f);
            }*/


            // Rotate points towards higher points and check if they don't cross max angle deviation

            Vector2 difference = capePoints[i - 1].position - capePoint.position;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90f;
            float secondAngle = capePoints[i - 1].rotation.eulerAngles.z;
            secondAngle = secondAngle > 180f ? secondAngle - 360f : secondAngle;
            float angleDiff = angle - secondAngle;

            if (angleDiff > maxAngleDeviation)
            {
                float a = (secondAngle + maxAngleDeviation) * Mathf.Deg2Rad;
                Vector2 v = new Vector2(Mathf.Sin(a), -Mathf.Cos(a));
                capePoint.position = (Vector2)capePoints[i - 1].position + v * difference.magnitude;

                capePoint.eulerAngles = new Vector3(0f, 0f, secondAngle + maxAngleDeviation);
            }
            else if (angleDiff < -maxAngleDeviation)
            {
                float a = (secondAngle - maxAngleDeviation) * Mathf.Deg2Rad;
                Vector2 v = new Vector2(Mathf.Sin(a), -Mathf.Cos(a));
                capePoint.position = (Vector2)capePoints[i - 1].position + v * difference.magnitude;

                capePoint.eulerAngles = new Vector3(0f, 0f, secondAngle - maxAngleDeviation);
            }
            else
            {
                capePoint.eulerAngles = new Vector3(0f, 0f, angle);
            }
        }
    }


    private void RelaxEdges()
    {
        for (int j = 0; j < relaxation; j++)
        {
            for (int i = 1; i < verticalNodesCount; i++)
            {
                Transform capePoint = capePoints[i];
                Vector2 direction = capePoints[i - 1].position - capePoint.position;

                if (i == 1)
                {
                    capePoint.position = (Vector2)capePoints[i - 1].position - direction.normalized * restDistance;
                }
                else
                {
                    float diff = direction.magnitude - restDistance;
                    Debug.Log(diff);
                    capePoint.position = (Vector2)capePoint.position + direction.normalized * diff * 0.5f;
                    capePoints[i - 1].position = (Vector2)capePoints[i - 1].position - direction.normalized * diff * 0.5f;
                }
            }
        }
    }


    private void UpdateSprite()
    {
        for (int i = 0, j = verticalNodesCount - 1; i < vertices.Length; i += 2, j--)
        {
            Transform refPoint = referencePoints[j];
            Transform capePoint = capePoints[j];

            Vector2 capePos = capePoint.position;
            Vector2 leftCapePos = capePos + (Vector2)capePoint.TransformDirection(-0.5f, 0f, 0f);
            Vector2 rightCapePos = capePos + (Vector2)capePoint.TransformDirection(0.5f, 0f, 0f);
            leftCapePos = refPoint.InverseTransformPoint(leftCapePos);
            rightCapePos = refPoint.InverseTransformPoint(rightCapePos);
            leftCapePos.y += (refPoint.localPosition.y) / 2f + 0.5f;
            rightCapePos.y += (refPoint.localPosition.y) / 2f + 0.5f;

            vertices[i] = leftCapePos;
            vertices[i + 1] = rightCapePos;
            UVs[i] = new Vector2(leftCapePos.x + 0.5f, leftCapePos.y + 0.5f);
            UVs[i + 1] = new Vector2(rightCapePos.x + 0.5f, rightCapePos.y + 0.5f);
        }

        mesh.vertices = vertices;
    }
}