// HitRangeVisualizer.cs
using UnityEngine;

public class HitRangeVisualizer : MonoBehaviour
{
    [Header("Materials")]
    public Material materialMeh;
    public Material materialGood;
    public Material materialGreat;

    [Header("Cube Settings")]
    public float width = 2f;    // Width along X-axis
    public float height = 0.1f; // Height along Y-axis (thin to lie flat)

    [Header("Player Reference")]
    public Transform playerTransform;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned in HitRangeVisualizer.");
            return;
        }

        if (ScoreManager.instance == null)
        {
            Debug.LogError("ScoreManager instance not found.");
            return;
        }

        float greatRange = ScoreManager.instance.greatRange;
        float goodRange = ScoreManager.instance.goodRange;
        float mehRange = ScoreManager.instance.mehRange;

        // Calculate total span: meh + good + great + good + meh
        float totalSpan = mehRange + goodRange + greatRange + goodRange + mehRange;

        // Starting Z position: center the total span around the player
        float startZ = playerTransform.position.z - (totalSpan / 2f);

        // Create hit range cubes in the following order:
        // MehBack, GoodBack, Great, GoodFront, MehFront
        CreateCube("MehRangeBack", ref startZ, mehRange, materialMeh);
        CreateCube("GoodRangeBack", ref startZ, goodRange, materialGood);
        CreateCube("GreatRange", ref startZ, greatRange, materialGreat);
        CreateCube("GoodRangeFront", ref startZ, goodRange, materialGood);
        CreateCube("MehRangeFront", ref startZ, mehRange, materialMeh);
    }

    /// <summary>
    /// Creates a cube representing a hit range zone.
    /// </summary>
    /// <param name="name">Name of the cube GameObject.</param>
    /// <param name="currentZ">Reference to the current Z position. It gets updated after placing the cube.</param>
    /// <param name="range">Length of the cube along the Z-axis.</param>
    /// <param name="material">Material to assign to the cube.</param>
    void CreateCube(string name, ref float currentZ, float range, Material material)
    {
        // Create a new cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.parent = this.transform;

        // Calculate the center position of the cube
        Vector3 position = playerTransform.position + new Vector3(0, 0, currentZ + (range / 2f));
        cube.transform.position = position;

        // Scale the cube based on the range (Z-axis)
        cube.transform.localScale = new Vector3(width, height, range);

        // Assign the specified material
        MeshRenderer mr = cube.GetComponent<MeshRenderer>();
        mr.material = material;

        // Slightly elevate the cube to prevent z-fighting
        cube.transform.position += new Vector3(0, height / 2f, 0); // Lift half the height above ground

        // Remove the collider to avoid unintended interactions
        Destroy(cube.GetComponent<Collider>());

        // Log the position and scale for debugging
        Debug.Log($"{name} - Position: {cube.transform.position}, Scale: {cube.transform.localScale}");

        // Update the currentZ for the next cube
        currentZ += range;
    }
}
