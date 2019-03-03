/**
 * Description: A set of methods that can be used for generating random position on a circle or a sphere.
 * Authors: Paweł, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 * */

using System;
using UnityEngine;

public static class RandomPoint
{
    private static System.Random Random = new System.Random();

    /// <summary>
    /// Generates a random point on a circle with given center and radius.
    /// </summary>
    /// <param name="center">Center point</param>
    /// <param name="radius">Radius</param>
    /// <returns>Random point on circle</returns>
    public static Vector3 OnCircle(Vector3 center, float radius)
    {
        double angle = RandomNumberInRange(0, 360) * (Math.PI / 180);

        return GenerateOnCircle(center, angle, radius);
    }

    /// <summary>
    /// Generates a random point on circle.
    /// </summary>
    /// <param name="center">Center point.</param>
    /// <param name="minRadius">Minimal radius.</param>
    /// <param name="maxRadius">Maximal radius.</param>
    /// <returns>Random point on circle.</returns>
    public static Vector3 OnCircle(Vector3 center, float minRadius, float maxRadius)
    {
        double angle = RandomNumberInRange(0, 360) * (Math.PI / 180);
        double randomizedRadius = RandomNumberInRange(minRadius, maxRadius);

        return GenerateOnCircle(center, angle, randomizedRadius);
    }

    /// <summary>
    /// Generates a random point on circle.
    /// </summary>
    /// <param name="center">Center point.</param>
    /// <param name="radius">Radius.</param>
    /// <param name="minAngle">Minimal angle.</param>
    /// <param name="maxAngle">Maximal angle.</param>
    /// <returns>Random point on circle.</returns>
    public static Vector3 OnCircle(Vector3 center, float radius, float minAngle, float maxAngle)
    {
        double randomizedAngle = RandomNumberInRange(minAngle, maxAngle) * (Math.PI / 180);

        return GenerateOnCircle(center, randomizedAngle, radius);
    }

    /// <summary>
    /// Generates a random point on circle.
    /// </summary>
    /// <param name="center">Center point.</param>
    /// <param name="minRadius">Minimal radius.</param>
    /// <param name="maxRadius">Maximal radius.</param>
    /// <param name="minAngle">Minimal angle.</param>
    /// <param name="maxAngle">Maximal angle.</param>
    /// <returns>Random point on circle.</returns>
    public static Vector3 OnCircle(Vector3 center, float minRadius, float maxRadius, float minAngle, float maxAngle)
    {
        double randomizedAngle = RandomNumberInRange(minAngle, maxAngle) * (Math.PI / 180);
        double randomizedRadius = RandomNumberInRange(minAngle, maxAngle);

        return GenerateOnCircle(center, randomizedAngle, randomizedRadius);
    }

    /// <summary>
    /// Generates a random point on sphere.
    /// </summary>
    /// <param name="center">Center point.</param>
    /// <param name="radius">Radius.</param>
    /// <returns>Random point on sphere.</returns>
    public static Vector3 OnSphere(Vector3 center, float radius)
    {
        Vector3 spherePoint = UnityEngine.Random.onUnitSphere * radius;

        return center + spherePoint;
    }

	/// <summary>
	/// Generates a random point on sphere.
	/// </summary>
	/// <param name="center">Center point.</param>
	/// <param name="minRadius">Minimal radius.</param>
	/// <param name="maxRadius">Maximal radius.</param>
	/// <returns>Random point on sphere.</returns>
	public static Vector3 OnSphere(Vector3 center, float minRadius, float maxRadius)
    {
        double randomizedRadius = RandomNumberInRange(minRadius, maxRadius);
        Vector3 spherePoint = UnityEngine.Random.onUnitSphere * Convert.ToSingle(randomizedRadius);

        return center + spherePoint;
    }

    private static double RandomNumberInRange(double min, double max)
    {
        return Random.NextDouble() * (max - min) + min;
    }

    private static Vector3 GenerateOnCircle(Vector3 center, double angle, double radius)
    {
        float x = center.x + Convert.ToSingle(Math.Cos(angle) * radius);
        float y = center.y + Convert.ToSingle(Math.Sin(angle) * radius);
        float z = center.z;

        return new Vector3(x, y, z);
    }
}
