using UnityEngine;

// Handgun: has a hitbox. Right grip button picks the gun up when the controller is near it
// and drops it again on the next press. Right index trigger fires black bullets while it is held.

public class HandgunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private Material bulletMaterial;

    [Header("Model fitting (glTF model: barrel = -X, 2 m long)")]
    [SerializeField] private float modelScale = 0.1f;
    [SerializeField] private Vector3 modelEuler = new Vector3(0f, 90f, 0f);

    [Header("Hitbox (follows Model Scale automatically)")]
    [SerializeField] private float hitboxPadding = 0.03f;

    [Header("Holding")]
    [SerializeField] private bool toggleGrab = true;
    [SerializeField] private float grabDistance = 0.15f;
    [SerializeField] private Vector3 holdOffset = Vector3.zero;
    [SerializeField] private bool keepSceneRotation = true;
    [SerializeField] private Vector3 holdEuler = Vector3.zero;

    [Header("Shooting")]
    [SerializeField] private float bulletSize = 0.025f;
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private float fireCooldown = 0.15f;
    [SerializeField] private float bulletLifetime = 6f;

    private Quaternion sceneRotation;
    private Vector3 holdPosition;
    private Vector3 muzzleLocalPos;
    private BoxCollider box;
    private OVRCameraRig rig;
    private bool held;
    private float nextFireTime;

    private void Awake()
    {
        sceneRotation = transform.localRotation;

        if (transform.childCount > 0)
        {
            modelScale = transform.GetChild(0).localScale.x;
        }
        else if (modelPrefab != null)
        {
            GameObject model = Instantiate(modelPrefab, transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.Euler(modelEuler);
            model.transform.localScale = Vector3.one * modelScale;
        }

        box = GetComponent<BoxCollider>();
        if (box == null)
        {
            box = gameObject.AddComponent<BoxCollider>();
            box.center = Vector3.zero;
            box.size = new Vector3(0.36f, 1.4f, 2.0f) * modelScale + Vector3.one * hitboxPadding;
        }

        holdPosition = new Vector3(0f, 0.35f, 0.5f) * modelScale;
        muzzleLocalPos = new Vector3(0f, 0.49f, 1.05f) * modelScale;

        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        if (rig == null)
        {
            rig = FindFirstObjectByType<OVRCameraRig>();
            if (rig == null) return;
        }

        Transform hand = rig.rightControllerAnchor;
        bool grip = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        bool gripDown = OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);

        if (!held)
        {
            bool pressed = toggleGrab ? gripDown : grip;
            if (pressed && Vector3.Distance(box.ClosestPoint(hand.position), hand.position) <= grabDistance)
                Grab(hand);
            return;
        }

        if (toggleGrab ? gripDown : !grip) { Release(); return; }

        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            Fire();
        }
    }

    private void Grab(Transform hand)
    {
        held = true;
        transform.SetParent(hand, false);
        Quaternion rot = keepSceneRotation ? sceneRotation : Quaternion.Euler(holdEuler);
        transform.localRotation = rot;
        transform.localPosition = rot * holdPosition + holdOffset;
    }

    private void Release()
    {
        held = false;
        transform.SetParent(null, true);
    }

    private void Fire()
    {
        Vector3 pos = transform.TransformPoint(muzzleLocalPos);
        Vector3 dir = transform.forward;

        GameObject b = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        b.name = "GunBullet";
        b.transform.position = pos;
        b.transform.localScale = Vector3.one * bulletSize;

        var rend = b.GetComponent<Renderer>();
        if (bulletMaterial != null)
        {
            var mat = new Material(bulletMaterial);
            mat.SetColor("_BaseColor", Color.black);
            rend.material = mat;
        }
        else rend.material.color = Color.black;

        var col = b.GetComponent<SphereCollider>();
        Physics.IgnoreCollision(col, box);

        var rb = b.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = dir * bulletSpeed;

        b.AddComponent<GunBullet>();
        Destroy(b, bulletLifetime);

        OVRInput.SetControllerVibration(0.3f, 0.5f, OVRInput.Controller.RTouch);
        Invoke(nameof(StopVibration), 0.06f);
    }

    private void StopVibration()
    {
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
    }
}
