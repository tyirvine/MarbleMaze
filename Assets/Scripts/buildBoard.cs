using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class buildBoard : MonoBehaviour {
	//size of the gameBoard
	public int boardWidth = 5;
	public int boardHeight = 5;

	public GameObject cornerPiece;
	public GameObject sidePiece;
	public GameObject centerPiece;
	public GameObject cornerWall;
	public GameObject sideWall;

	public float tileScale = 1f;                    //alter this if the board tile model scale changes. it will affect the spacing between tiles
	public float tileHeight = 0f;                   //the Y height of the tiles.
	public float wallHeight = 1f;                   //the additional height of the wall, this will be added to the tileHeight

	public float cornerWallOffsetX = 2f;            //the corner wall pieces need to move to the outside of the floor squares, this will enable that movement
	public float cornerWallOffsetZ = 2f;

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
		//assuming we are going to use this gameobject as the "board"
		transform.position = new Vector3(((boardWidth / 2) * tileScale)-tileScale/2, 0, (boardHeight / 2) * tileScale);
		boardData = new int[boardWidth, boardHeight];
		BuildBoardData();
		InstantiateBoard();
		GroupTilesToParent();
	}


	void BuildBoardData() {
		for (int i = 0; i < boardWidth; i++) {
			for (int ii = 0; ii < boardHeight; ii++) {
				boardData[i, ii] = centerPieceIndex;
			}
		}

		//fill in the corner pieces first
		boardData[0, 0] = lowerLeftCornerPieceIndex; //lower left
		boardData[boardWidth - 1, 0] = lowerRightCornerPieceIndex; //lower right
		boardData[0, boardHeight - 1] = upperLeftCornerPieceIndex; //top left
		boardData[boardWidth - 1, boardHeight - 1] = upperRightCornerPieceIndex; //top right

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
		void TileInstantiation(GameObject piece, Vector3 position, float x, float y,float z) {
			tempTile = Instantiate(piece, position, Quaternion.identity);
			tempTile.transform.Rotate(new Vector3(x, y, z));
		}


		for (int i = 0; i < boardWidth; i++) {
			for (int ii = 0; ii < boardHeight; ii++) {

				// Some commonly used position types
				Vector3 positionStandard = new Vector3(i * tileScale, tileHeight, ii * tileScale);
				Vector3 positionOffsetZ(float xOffset, float zOffset) => new Vector3(i * tileScale + xOffset, tileHeight + wallHeight, ii * tileScale + zOffset);

				switch (boardData[i, ii]) {
				// Corners
				case lowerLeftCornerPieceIndex:
					TileInstantiation(cornerPiece, positionStandard, -90, 0, -90);
					TileInstantiation(cornerWall, positionOffsetZ(-cornerWallOffsetX, -cornerWallOffsetZ), -90, 0, -180);
					break;
				case lowerRightCornerPieceIndex:
					TileInstantiation(cornerPiece, positionStandard, -90, 0, 180);
					TileInstantiation(cornerWall, positionOffsetZ(+cornerWallOffsetX, -cornerWallOffsetZ), -90, 0, -270);
					break;
				case upperRightCornerPieceIndex:
					TileInstantiation(cornerPiece, positionStandard, -90, 0, 90);
					TileInstantiation(cornerWall, positionOffsetZ(+cornerWallOffsetX, +cornerWallOffsetZ), -90, 0, 0);
					break;
				case upperLeftCornerPieceIndex:
					TileInstantiation(cornerPiece, positionStandard, -90, 0, 0);
					TileInstantiation(cornerWall, positionOffsetZ(-cornerWallOffsetX, +cornerWallOffsetZ), -90, 0, -90);
					break;


				// Sides
				case leftSidePieceIndex:
					TileInstantiation(sidePiece, positionStandard, -90, 0, 90);
				    TileInstantiation(sideWall, positionStandard + new Vector3(-1.5f,0.5f,0), 0,-90, 0);
					break;
				case rightSidePieceIndex:
					TileInstantiation(sidePiece, positionStandard, -90, 0, -90);
						TileInstantiation(sideWall, positionStandard + new Vector3(1.5f, 0.5f, 0), 0, 90, 0);
						break;
				case topSidePieceIndex:
					TileInstantiation(sidePiece, positionStandard, -90, 0, -180);
						TileInstantiation(sideWall, positionStandard + new Vector3(0, 0.5f, 1.5f), 0, 0, 0); 
						break;
				case bottomSidePieceIndex:
					TileInstantiation(sidePiece, positionStandard, -90, 0, 0);
						TileInstantiation(sideWall, positionStandard + new Vector3(0, 0.5f, -1.5f), 0, -180, 0);
						break;


				// Centers
				case centerPieceIndex:
					TileInstantiation(centerPiece, positionStandard, -90, 0, 0);
					break;


				default:
					break;
				}
			}
		}
	}

	void GroupTilesToParent() {
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("gameTile");
		foreach (GameObject tile in tiles) {
			tile.AddComponent<Rigidbody>();
			Rigidbody rigidbody = tile.GetComponent<Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
			tile.transform.SetParent(transform);
		}
	}
}

