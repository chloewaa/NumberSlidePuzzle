using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform emptySpace;    // The invisible “hole” tile
    [SerializeField] private TileManager[] tiles;     // 15 actual tiles + one null for the empty spot
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI endTimerText; 

    private Camera camera;            // Cached reference to main camera
    private int emptySpaceIndex;   // Which index in ‘tiles’ is currently empty
    private bool isFinished; 

    void Start()
    {
        camera = Camera.main;         // Grab the main camera only once

        // Find the null slot in our array—that’s the empty space
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                emptySpaceIndex = i;
                break;
            }
        }

        Shuffle();                 // Shake things up at launch
    }

    void Update()
    {
        // On click, see if we hit a tile that’s next to the empty spot
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit)
            {
                if (Vector2.Distance(emptySpace.position, hit.transform.position) < 2.5f)
                {
                    // Swap world positions
                    Vector2 oldEmptyPos = emptySpace.position;
                    TileManager clicked = hit.transform.GetComponent<TileManager>();

                    emptySpace.position = clicked.transform.position;
                    clicked.targetPosition = oldEmptyPos;

                    // Swap our array entries to match the visuals
                    int clickedIndex = findIndex(clicked);
                    tiles[emptySpaceIndex] = tiles[clickedIndex];
                    tiles[clickedIndex] = null;
                    emptySpaceIndex = clickedIndex;
                }
            }
        }
        if(!isFinished) {
        int correctTiles = 0;
        foreach(var a in tiles) {
            if(a != null) {
                if(a.inRightPlace)
                    correctTiles++;
            }
        }   
        if(correctTiles == tiles.Length - 1) {
            isFinished = true;
            winPanel.SetActive(true);
            var a = GetComponent<TimerManager>();
            a.StopTimer(); // Stop the timer when the game is finished
            endTimerText.text = string.Format("{0:D2}:{1:D2}", a.minutes, a.second); // Format the time as MM:SS
        }
        }
    }

    public void PlayAgain() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Shuffle()
    {
        // First, move the null (empty) slot into the very last index
        if (emptySpaceIndex != tiles.Length - 1)
        {
            Vector2 lastTilePos = tiles[tiles.Length - 1].targetPosition;
            tiles[tiles.Length - 1].targetPosition = emptySpace.position;
            emptySpace.position = lastTilePos;

            tiles[emptySpaceIndex] = tiles[tiles.Length - 1];
            tiles[tiles.Length - 1] = null;
            emptySpaceIndex = tiles.Length - 1;
        }

        int inversionCount;
        do
        {
            // Fisher–Yates shuffle—but skip over the null slot entirely
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == null) continue;    // don’t touch the empty space

                int rand;
                // pick another non-null tile to swap with
                do { rand = Random.Range(0, tiles.Length); }
                while (tiles[rand] == null);

                // swap their target positions so tiles know where to glide
                Vector2 tmpPos = tiles[i].targetPosition;
                tiles[i].targetPosition = tiles[rand].targetPosition;
                tiles[rand].targetPosition = tmpPos;

                // swap the actual array entries
                TileManager tmpTile = tiles[i];
                tiles[i] = tiles[rand];
                tiles[rand] = tmpTile;
            }

            inversionCount = GetInversions();
            Debug.Log($"Puzzle shuffled — inversions: {inversionCount}");
        }
        // only accept even inversion counts (ensures solvability)
        while (inversionCount % 2 != 0);

        // Finally, re‑locate our null slot index just in case
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                emptySpaceIndex = i;
                break;
            }
        }
    }

    public int findIndex(TileManager tm)
    {
        // Linear search to map a TileManager back to its slot index
        for (int i = 0; i < tiles.Length; i++)
            if (tiles[i] == tm)
                return i;
        return -1;  // shouldn’t happen unless something’s off
    }

    private int GetInversions()
    {
        int count = 0;
        // Count each pair (i, j) with i < j where tile numbers are “out of order”
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null) continue;    // empty spot has no number
            for (int j = i + 1; j < tiles.Length; j++)
            {
                if (tiles[j] == null) continue;
                if (tiles[i].number > tiles[j].number)
                    count++;
            }
        }
        return count;
    }
}
