using UnityEngine;
using UnityEngine.UI;

// Game switch on the wall board
// Red board = level not running. Shoot the board: level starts, board turns green
// Shoot again: level stops, board turns red again

public class BoardTrigger : MonoBehaviour
{
    [SerializeField] private Color idleColor = new Color(0.8f, 0f, 0f, 1f);
    [SerializeField] private Color runningColor = new Color(0f, 0.7f, 0f, 1f);
    [SerializeField] private float toggleCooldown = 0.7f;

    private Image background;
    private SphereSwarmSpawner swarm;
    private float nextToggleTime;

    private void Start()
    {
        background = GetComponentInChildren<Image>();
        swarm = FindFirstObjectByType<SphereSwarmSpawner>();
        SetColor(false);
    }

    public void OnShot()
    {
        if (swarm == null || Time.time < nextToggleTime) return;
        nextToggleTime = Time.time + toggleCooldown;

        if (swarm.Running)
        {
            swarm.StopLevel();
            SetColor(false);
        }
        else
        {
            swarm.StartLevel();
            SetColor(swarm.Running);
        }
    }

    private void SetColor(bool running)
    {
        if (background != null) background.color = running ? runningColor : idleColor;
    }
}
