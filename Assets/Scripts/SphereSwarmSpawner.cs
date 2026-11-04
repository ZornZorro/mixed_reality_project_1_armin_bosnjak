using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

// Spawns spheres that fly randomly through the real room and bounce off walls
// The level is started and stopped by shooting the board

public class SphereSwarmSpawner : MonoBehaviour
{
    [SerializeField] private Material sphereMaterial;
    [SerializeField] private int sphereCount = 8;
    [SerializeField] private float sphereDiameter = 0.15f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float spawnClearance = 0.3f;
    [SerializeField] private float wallColliderThickness = 0.3f;

    [SerializeField] private float spawnInterval = 0.4f;
    [SerializeField] private int maxSpheres = 20;

    private float spawnTimer;
    private readonly List<Rigidbody> spheres = new List<Rigidbody>();
    private MRUKRoom room;
    private PhysicsMaterial bouncy;

    private void Start()
    {
        if (MRUK.Instance == null) return;
        MRUK.Instance.RegisterSceneLoadedCallback(OnSceneLoaded);
    }

    private void OnSceneLoaded()
    {
        room = MRUK.Instance.GetCurrentRoom();
        if (room == null) return;

        bouncy = new PhysicsMaterial("SphereBouncy")
        {
            bounciness = 1f,
            dynamicFriction = 0f,
            staticFriction = 0f,
            bounceCombine = PhysicsMaterialCombine.Maximum,
            frictionCombine = PhysicsMaterialCombine.Minimum
        };

        AddColliders(room.WallAnchors);
        AddColliders(room.FloorAnchors);
        AddColliders(room.CeilingAnchors);
    }

    public bool Running { get; private set; }

    public void StartLevel()
    {
        if (room == null) return;
        Running = true;
        spawnTimer = 0f;
        for (int i = 0; i < sphereCount; i++) SpawnSphere();
    }

    public void StopLevel()
    {
        Running = false;
        foreach (Rigidbody rb in spheres)
        {
            if (rb != null) Destroy(rb.gameObject);
        }
        spheres.Clear();
    }

    private void AddColliders(List<MRUKAnchor> anchors)
    {
        foreach (MRUKAnchor anchor in anchors)
        {
            if (!anchor.PlaneRect.HasValue) continue;
            Rect r = anchor.PlaneRect.Value;

            var box = new GameObject($"{anchor.name}_Hitbox");
            box.transform.SetParent(anchor.transform, false);
            var col = box.AddComponent<BoxCollider>();
            col.center = new Vector3(r.center.x, r.center.y, 0f);
            col.size = new Vector3(r.width, r.height, wallColliderThickness);
            col.sharedMaterial = bouncy;
        }
    }

    private void SpawnSphere()
    {
        Vector3? pos = room.GenerateRandomPositionInRoom(spawnClearance, true);
        if (!pos.HasValue) return;

        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.name = "FlyingSphere";
        s.AddComponent<FlyingSphere>();
        s.transform.position = pos.Value;
        s.transform.localScale = Vector3.one * sphereDiameter;
        var rend = s.GetComponent<Renderer>();
        Color c = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
        if (sphereMaterial != null)
        {
            var mat = new Material(sphereMaterial);
            mat.SetColor("_BaseColor", c);
            rend.material = mat;
        }
        else rend.material.color = c;

        var col = s.GetComponent<SphereCollider>();
        col.sharedMaterial = bouncy;

        var rb = s.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = Random.onUnitSphere * speed;

        spheres.Add(rb);
    }

    private void Update()
    {
        if (room == null || !Running) return;

        spheres.RemoveAll(rb => rb == null);
        spawnTimer += Time.deltaTime;
        while (spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;
            if (spheres.Count >= maxSpheres)
            {
                spawnTimer = 0f;
                break;
            }
            SpawnSphere();
        }
    }

    private void FixedUpdate()
    {
        if (room == null) return;

        foreach (Rigidbody rb in spheres)
        {
            if (rb == null) continue;
            if (!room.IsPositionInRoom(rb.position))
            {
                Vector3? p = room.GenerateRandomPositionInRoom(spawnClearance, true);
                if (p.HasValue) rb.position = p.Value;
                rb.linearVelocity = Random.onUnitSphere * speed;
                continue;
            }
            Vector3 v = rb.linearVelocity;
            if (v.sqrMagnitude < 0.0001f) v = Random.onUnitSphere;
            rb.linearVelocity = v.normalized * speed;
        }
    }
}
