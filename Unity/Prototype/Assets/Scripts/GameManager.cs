using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int boardLength = 10;
    public int boardWidth = 10;
    public int playerSpace = 3;
    public int numBoxes = 30;
    public int numPlayers = 5;
    public Color tileLight;
    public Color tileDark;
    public GameObject tilePF;
    public GameObject boxPF;
    public GameObject player1PF;
    public GameObject player2PF;

    private bool[,] player1s;
    private bool[,] player2s;
    private bool[,] boxes;

    // Triggers
    public bool triggerChange; // Notifies all pieces of a change and triggers calculations

    void Awake()
    {
        // initialize vars
        triggerChange = false;
        boxes = new bool[boardWidth, boardLength];
        player1s = new bool[boardWidth, boardLength];
        player2s = new bool[boardWidth, boardLength];
        System.Random rnd = new System.Random();

        // Create parent empties
        GameObject board = new GameObject("Board");
        GameObject prntPlayer1s = new GameObject("Player 1s");
        GameObject prntPlayer2s = new GameObject("Player 2s");
        GameObject prntBoxes = new GameObject("Boxes");

        // Generate Board
        bool isMatDark;
        for (int x = 0; x < boardWidth; x++)
        {
            for (int z = 0; z < boardLength; z++)
            {
                Vector3 position = new Vector3();
                if ((x + z) % 2 == 0)
                {
                    isMatDark = false;
                }
                else
                {
                    isMatDark = true;
                }
                position.y = -0.5f;
                position.x = x;
                position.z = z;
                CreateTile(position, x, z, board, isMatDark);
            }
        }
        
        // Randomly place pieces
        BoardAddPieces(boardArray: player1s, num: numPlayers, minX: 0,                        maxX: playerSpace, minZ: 0, maxZ: boardLength, obj: player1PF, parent: prntPlayer1s, name: "player1", rand: rnd);
        BoardAddPieces(boardArray: player2s, num: numPlayers, minX: boardWidth - playerSpace, maxX: boardWidth,  minZ: 0, maxZ: boardLength, obj: player2PF, parent: prntPlayer2s, name: "player2", rand: rnd);
        BoardAddPieces(boardArray: boxes,    num: numBoxes,   minX: 0,                        maxX: boardWidth,  minZ: 0, maxZ: boardLength, obj: boxPF,     parent: prntBoxes,    name: "box"    , rand: rnd);
    }

    void Update()
    {
        triggerChange = false; // set to false at beginning of every update
    }

    public void CreateTile(Vector3 pos, int x, int z, GameObject parent, bool isDark = false)
    {
        GameObject tile = Instantiate<GameObject>(tilePF, position: pos, rotation: Quaternion.identity, parent: parent.transform);
        tile.name = "tile" + x + z;
        MeshRenderer ren = tile.GetComponent<MeshRenderer>();
        if (isDark)
        {
            ren.material.color = tileDark;
        }
        else
        {
            ren.material.color = tileLight;
        }


    }

    // testX and testZ are to test the placement of an object that hasn't been recorded in arrays yet
    public bool BoardCheckEmpty(int x, int z, int testX = -1, int testZ = -1)
    {
        if (!(player1s[x, z] || player2s[x, z] || boxes[x, z]) && !(x == testX && z == testZ))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void BoardAddPieces(bool[,] boardArray, int num, int minX, int maxX, int minZ, int maxZ, GameObject obj, GameObject parent, string name, System.Random rand, float objHeight = 1f, int maxLoops = 500)
    {
        int cnt = 0;
        int cntLoops = 0;
        int posX;
        int posZ;
        while (cnt < num)
        {
            cntLoops++;
            if (cntLoops > maxLoops) { goto Exit; }
            posX = rand.Next(minX, maxX);
            posZ = rand.Next(minZ, maxZ);
            if (BoardCheckEmpty(posX, posZ) && !BoardCheckAnyGrpSurr(player1s, posX, posZ) && !BoardCheckAnyGrpSurr(player2s, posX, posZ))
            {
                GameObject box = Instantiate<GameObject>(obj);
                box.name = name + "_" + cnt;
                box.transform.parent = parent.transform;
                box.transform.position = new Vector3(posX, objHeight, posZ);
                boardArray[posX, posZ] = true;
                cnt++;
            }
        }
        return;

    Exit:
        Debug.Log("ERROR: Ran out of space on gameboard");
        return;
    }

    // for now passing in location on gameboard only; need to create Player class
    public bool BoardCheckLocSurr(int posX, int posZ, int testX = -1, int testZ = -1)
    {
        bool checkUp;
        bool checkDown;
        bool checkLeft;
        bool checkRight;

        // Check all directions
        checkUp = CheckDirection(x: posX,        z: posZ + 1, testX, testZ);
        checkDown = CheckDirection(x: posX,      z: posZ - 1, testX, testZ);
        checkLeft = CheckDirection(x: posX + 1,  z: posZ,     testX, testZ);
        checkRight = CheckDirection(x: posX - 1, z: posZ,     testX, testZ);

        return (checkUp && checkDown && checkLeft && checkRight);

        bool CheckDirection(int x, int z, int tstX, int tstZ)
        {
            if (x < 0 || x >= boardWidth || z < 0 || z >= boardLength)
            {
                return (true);
            }
            else
            {
                return (!BoardCheckEmpty(x, z, tstX, tstZ));
            }
        }
    }

    public bool BoardCheckAnyGrpSurr(bool[,] group, int testX = -1, int testZ = -1)
    {
        bool runningCheck = false;

        for (int x = 0; x < group.GetLength(0); x++)
        {
            for (int z = 0; z < group.GetLength(1); z++)
            {
                if (group[x, z])
                {
                    runningCheck = runningCheck || BoardCheckLocSurr(x, z, testX, testZ);
                }
            }
        }

        return (runningCheck);
    }
}
