// InputManager.cs
// Attach to an empty GameObject (e.g. "InputManager").
// Now with extra logs to debug preview & movement.

using UnityEngine;
// Make sure we’re calling the UnityEngine.Object APIs, not System.Object.
using Object = UnityEngine.Object;

public class InputManager : MonoBehaviour
{
    private CrewMember selectedCrew = null;
    private Camera mainCamera;
    private BoardManager boardMgr;
    private TurnManager turnMgr;

    [Header("Preview Settings")]
    public int previewRange = 3;

    void Start()
    {
        mainCamera = Camera.main;
        boardMgr = Object.FindFirstObjectByType<BoardManager>();
        turnMgr = Object.FindFirstObjectByType<TurnManager>();
        Debug.Log($"[InputManager] Init: boardMgr={(boardMgr != null)}, turnMgr={(turnMgr != null)}");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleTap(Input.mousePosition);
        else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            HandleTap(Input.touches[0].position);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            turnMgr.UndoLast();
            Debug.Log("[InputManager] Z: UndoLast");
            boardMgr.ClearHighlights();
            selectedCrew = null;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            turnMgr.CancelTurn();
            Debug.Log("[InputManager] R: CancelTurn");
            boardMgr.ClearHighlights();
            selectedCrew = null;
        }
    }

    private void HandleTap(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("[InputManager] Raycast missed");
            return;
        }

        Debug.Log($"[InputManager] Hit: {hit.collider.name}");

        // 1) Crew selection
        var crew = hit.collider.GetComponent<CrewMember>();
        if (crew != null)
        {
            selectedCrew = crew;
            Debug.Log($"[InputManager] Selected crew '{crew.name}'");

            // Preview
            var crewTile = boardMgr.GetTileAtPosition(crew.transform.position);
            if (crewTile != null)
            {
                Debug.Log($"[InputManager] Preview from Tile({crewTile.Col},{crewTile.Row})");
                boardMgr.ClearHighlights();
                boardMgr.HighlightRange(crewTile.Col, crewTile.Row, previewRange);
            }
            return;
        }

        // 2) Move commit
        if (selectedCrew != null)
        {
            var tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                var rend = hit.collider.GetComponent<Renderer>();
                Debug.Log($"[InputManager] Tapped Tile({tile.Col},{tile.Row}) renderer.enabled={rend.enabled}");

                // Only move if it’s currently highlighted
                if (rend.enabled)
                {
                    Vector3 targetPos = tile.transform.position;
                    turnMgr.DoAction(new MoveCommand(selectedCrew, targetPos));
                    Debug.Log($"[InputManager] Moved '{selectedCrew.name}' → {targetPos}");

                    boardMgr.ClearHighlights();
                    selectedCrew = null;
                }
                else
                {
                    Debug.LogWarning("[InputManager] Tile not highlighted—no move.");
                }
            }
        }
    }
}
