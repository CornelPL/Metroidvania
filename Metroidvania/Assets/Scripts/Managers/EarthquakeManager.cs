using UnityEngine;
using System.Collections.Generic;

public class EarthquakeManager : MonoBehaviour
{
    [SerializeField] private int rocksNum = 2;
    [SerializeField] private List<GameObject> rocks = null;
    [SerializeField] private float maxDistance = 50f;

    public void DoEarthquake(Vector2 playerPos)
    {
        if (Vector2.Distance(playerPos, transform.position) < maxDistance && rocksNum > 0)
        {
            int rock = Random.Range(0, rocks.Count);
            Instantiate(rocks[rock], transform.position, transform.rotation);
            rocksNum--;
        }
    }
}
