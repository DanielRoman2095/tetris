using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.Networking;
using System.Collections;


public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject fade;
    public Menu menu;
    public TextMeshProUGUI linesCountText;
    public TextMeshProUGUI ScoreCountText;
    public TextMeshProUGUI sigBloqueText;
    private int linesCount;
    private int scoreCount;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public NextPiece nextPiece;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector3Int offBoardPosition = new Vector3Int(12, 4, 0);

    private bool once = true;
    private int random;
    private int randomNext;
    [SerializeField]
    private bool isDemo;

    public RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (once) {
            random = Random.Range(0, tetrominoes.Length);
            randomNext = Random.Range(0, tetrominoes.Length);
            once = false;
        }
        else {
            random = randomNext;
            randomNext = Random.Range(0, tetrominoes.Length);
        }
         
        TetrominoData data = tetrominoes[random];
        TetrominoData data2 = tetrominoes[randomNext];
       

        activePiece.Initialize(this, spawnPosition, data);
        nextPiece.Initialize(this, offBoardPosition, data2);

        

        SetNextPiece(nextPiece);

        if (IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            GameOver();
        }

        Color temp = data2.color;
        sigBloqueText.faceColor = temp;
        //sigBloqueText.faceColor = new Color32(255, 128, 0, 255);
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        fade.SetActive(true);
        // Do anything else you want on game over here..
        if (!isDemo)
        {
            StartCoroutine(PostResults());
        }
        else
        {
            menu.loadGameOver();
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void SetNextPiece(NextPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public void ClearNextPiece(NextPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        scoreCount += 150;

        linesCount++;

        linesCountText.text = linesCount.ToString();

        ScoreCountText.text = scoreCount.ToString();

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }


    public IEnumerator PostResults()
    {
        Debug.Log("posting results");

        string api = Linker.GetEnv() + ".monou.gg/api/tournament/match/decision/round-robin/";


        WWWForm form = new WWWForm();
        form.AddField("place", 99);
        form.AddField("team_id", Linker.m_myTeamId);
        form.AddField("round", Linker.round);
        form.AddField("tournament_id", Linker.m_tournamentIdNumber);
        form.AddField("kills", scoreCount);
        form.AddField("deaths", 0);
        form.AddField("assistence", 0);

        Debug.Log("rondaaaa " + Linker.round);

        UnityWebRequest www = UnityWebRequest.Post(api, form);

        Debug.Log("POSTING RESULTS API " + api);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("get Success");
            Debug.Log(www.downloadHandler.text);
            //Debug.Log(www.downloadHandler.text);
            //ReadJson(www.downloadHandler.text);
            /*username = _username.text;
            SceneManager.LoadScene("CustomizationScene");*/
        }

        menu.loadGameOver();
    }
}
