using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;

// Spawns the info board on the key wall of the room and makes it shootable
public class WallBoardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private string authorName = "authorName";
    [TextArea]
    [SerializeField]
    private string machineDescription =
        "machineDescription";
    [SerializeField] private float heightFromFloor = 1.5f;

    private void Start()
    {
        MRUK.Instance.RegisterSceneLoadedCallback(OnSceneLoaded);
    }

    private void OnSceneLoaded()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room == null) return;
        MRUKAnchor keyWall = room.GetKeyWall(out _);
        if (keyWall == null) return;

        Vector3 normal = keyWall.transform.forward;
        float floorY = room.FloorAnchors.Count > 0 ? room.FloorAnchors[0].transform.position.y : 0f;

        Vector3 pos = keyWall.transform.position;
        pos.y = floorY + heightFromFloor;

        Quaternion rot = Quaternion.LookRotation(-normal, Vector3.up);

        GameObject board = Instantiate(boardPrefab, pos, rot);

        var zone = board.AddComponent<BoxCollider>();
        zone.isTrigger = true;
        zone.center = new Vector3(0f, 0f, -200f);
        zone.size = new Vector3(600f, 400f, 400f);
        board.AddComponent<BoardTrigger>();

        var texts = board.GetComponentsInChildren<TMP_Text>();
        texts[0].text = authorName;
        texts[1].text = machineDescription;
    }
}
