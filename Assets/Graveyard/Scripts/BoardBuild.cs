using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class buildBoard : MonoBehaviour {
    //size of the gameBoard
    int boardWidth = 5;
    int boardHeight = 5;

    public PhysicMaterial slippyMaterial;
    public GameObject cornerPiece;
    public GameObject sidePiece;
    public GameObject centerPiece;
    public GameObject cornerWall;
    public GameObject sideWall;

    public GameObject player;

    //public float tileScale = 1f;                    //alter this if the board tile model scale changes. it will affect the spacing between tiles
    public float tileHeight = 0f;                   //the Y height of the tiles.
    public float wallHeight = 1f;                   //the additional height of the wall, this will be added to the tileHeight

    //	public float cornerWallOffsetX = 2f;            //the corner wall pieces need to move to the outside of the floor squares, this will enable that movement
    //public float cornerWallOffsetZ = 2f;

    //	public float boardOffsetX = -2.5f;
    //public float boardOffsetZ = -0.5f;				//to allow for discrepancy between path generation and board instantiation.

    //the following lines are references to be used in the datagrid to represent the appropriate tiles
    const int lowerRightCornerPieceIndex = 1;
    const int lowerLeftCornerPieceIndex = 2;
    const int upperRightCornerPieceIndex = 3;
    const int upperLeftCornerPieceIndex = 4;
    const int leftSidePieceIndex = 5;
    const int topSidePieceIndex = 6;
    const int bottomSidePieceIndex = 7;
    const int rightSidePieceIndex = 8;
    const int centerPieceIndex = 9;

    /// <summary><see cref="BuildBoardData()"/> assigns this variable with positioning indices inorder to place
    /// floor tiles in <see cref="InstantiateBoard()"/> at the correct positions.</summary>
    int[,] boardData;

    void Start() {
        GlobalStaticVariables.Instance.debugLog.Add("Started buildBoard.cs      Time Executed : " + Time.deltaTime.ToString());
        //assuming we are going to use this gameobject as the "board"

        //transform.position = new Vector3(((boardWidth / 2) * tileScale)-tileScale/2, 0, (boardHeight / 2) * tileScale);
        //transform.position = new Vector3(0, 0, 0);

        //do some weird magic because the board is bigger than the generated path "because walls"
        boardWidth = GlobalStaticVariables.Instance.gridXSizeHalfLength * 2 + 1;
        boardHeight = GlobalStaticVariables.Instance.gridZSizeHalfLength * 2 + 1;
        boardData = new int[boardWidth, boardHeight];

        BuildBoardData();
        InstantiateBoard();

        GroupTilesToParent(); //called from pathmanager instead
        placePlayer();

    }

    void placePlayer() {
        GameObject startPosition = GameObject.FindGameObjectWithTag("startPosition");
        startPosition.transform.position += new Vector3(0, 1, 0);
        Instantiate(player, startPosition.transform.position, Quaternion.identity);

    }

    void BuildBoardData() {
        for (int i = 0; i < boardWidth; i++) {
            for (int ii = 0; ii < boardHeight; ii++) {
                boardData[i, ii] = centerPieceIndex;
            }
        }

        //fill in the corner pieces first
        boardData[0, 0] = lowerLeftCornerPieceIndex;                                //lower left
        boardData[boardWidth - 1, 0] = lowerRightCornerPieceIndex;                  //lower right
        boardData[0, boardHeight - 1] = upperLeftCornerPieceIndex;                  //top left
        boardData[boardWidth - 1, boardHeight - 1] = upperRightCornerPieceIndex;    //top right

        for (int i = 1; i < boardHeight - 1; i++) //assign the left and right hand floor pieces
        {
            boardData[0, i] = leftSidePieceIndex;
            boardData[boardWidth - 1, i] = rightSidePieceIndex;
        }

        for (int i = 1; i < boardWidth - 1; i++) { //assign the top and bottom floor pieces
            boardData[i, 0] = bottomSidePieceIndex;
            boardData[i, boardHeight - 1] = topSidePieceIndex;
        }
    }

    void InstantiateBoard() {
        // Just a temporary GameObject placeholder used to set the rotation of each tile instantiated.
        GameObject tempTile;
        /// <summary>Simplifies instantiation of tiles. Position is where the tile will be instantiated.
        /// X & Z are the x & z components of the rotation of each tile.</summary>
        void TileInstantiation(GameObject piece, Vector3 position, Quaternion rotation) {
            tempTile = Instantiate(piece, position, rotation);
            //tempTile.transform.Rotate(new Vector3(x, y, z));
        }

        for (int i = 0; i < boardWidth; i++) {
            for (int ii = 0; ii < boardHeight; ii++) {

                // Some commonly used position types
                Vector3 positionStandard = Vector3.Scale(new Vector3(i - boardWidth / 2, tileHeight, ii - boardHeight / 2), GlobalStaticVariables.Instance.GlobalScale);

                //Vector3 positionOffsetZ(float xOffset, float zOffset) => new Vector3(i * tileScale + xOffset, tileHeight + wallHeight, ii * tileScale + zOffset);

                switch (boardData[i, ii]) {
                    // Corners
                    case lowerLeftCornerPieceIndex:
                        TileInstantiation(cornerPiece, positionStandard, cornerPiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, cornerPiece.transform.rotation);
                        break;
                    case lowerRightCornerPieceIndex:
                        TileInstantiation(cornerPiece, positionStandard, cornerPiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, cornerPiece.transform.rotation);
                        break;
                    case upperRightCornerPieceIndex:
                        TileInstantiation(cornerPiece, positionStandard, cornerPiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, cornerPiece.transform.rotation);
                        break;
                    case upperLeftCornerPieceIndex:
                        TileInstantiation(cornerPiece, positionStandard, cornerPiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, cornerPiece.transform.rotation);
                        break;

                    // Sides
                    case leftSidePieceIndex:
                        TileInstantiation(sidePiece, positionStandard, sidePiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, sidePiece.transform.rotation);
                        break;
                    case rightSidePieceIndex:
                        TileInstantiation(sidePiece, positionStandard, sidePiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, sidePiece.transform.rotation);
                        break;
                    case topSidePieceIndex:
                        TileInstantiation(sidePiece, positionStandard, sidePiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, sidePiece.transform.rotation);
                        break;
                    case bottomSidePieceIndex:
                        TileInstantiation(sidePiece, positionStandard, sidePiece.transform.rotation);
                        TileInstantiation(sideWall, new Vector3(0, wallHeight, 0) + positionStandard, sidePiece.transform.rotation);
                        break;

                    // Centers
                    case centerPieceIndex:
                        TileInstantiation(centerPiece, positionStandard, centerPiece.transform.rotation);
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void GroupTilesToParent() {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("gameTile");
        foreach (GameObject tile in tiles) {
            if (!GlobalStaticVariables.Instance.renderBoardAsSingleMesh) {
                tile.AddComponent<Rigidbody>();
                Rigidbody rigidbody = tile.GetComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
                tile.GetComponent<Collider>().material = slippyMaterial;
            }
            tile.transform.SetParent(transform);
        }
        if (GlobalStaticVariables.Instance.renderBoardAsSingleMesh) {
            MeshFilter[] meshfilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshfilters.Length];

            int i = 0;
            while (i < meshfilters.Length) {
                combine[i].mesh = meshfilters[i].sharedMesh;
                combine[i].transform = meshfilters[i].transform.localToWorldMatrix;
                meshfilters[i].gameObject.SetActive(false);
                i++;
            }
            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            transform.gameObject.SetActive(true);
            transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;
            transform.GetComponent<MeshCollider>().material = slippyMaterial;
        }

        //		transform.position = new Vector3(boardOffsetX, -0.5f, boardOffsetZ);
        GameObject[] wallTiles = GameObject.FindGameObjectsWithTag("wallTile");
        foreach (GameObject tile in wallTiles) {
            tile.AddComponent<Rigidbody>();
            Rigidbody rigidbody = tile.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            tile.GetComponent<Collider>().material = slippyMaterial;
            tile.transform.SetParent(transform);
        }
    }
}
