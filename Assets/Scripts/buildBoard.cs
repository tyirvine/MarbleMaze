using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class buildBoard : MonoBehaviour
{
    // Start is called before the first frame update

    //size of the gameBoard
    public int boardWidth = 5;
    public int boardHeight = 5;


    public GameObject cornerPiece;
    public GameObject sidePiece;
    public GameObject centerPiece;
    public GameObject cornerWall;

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




    int[,] boardData;
    void Start()
    {
        boardData = new int[boardWidth, boardHeight];
        BuildBoardData();
        InstantiateBoard();

    }

    void BuildBoardData()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int ii = 0; ii < boardHeight; ii++)
            {
                boardData[i, ii] = centerPieceIndex;
            }
        }

        //fill in the corner pieces first
        boardData[0, 0] = lowerLeftCornerPieceIndex; //lower left
        boardData[boardWidth - 1, 0] = lowerRightCornerPieceIndex; //lower right
        boardData[0, boardHeight - 1] = upperLeftCornerPieceIndex; //top left
        boardData[boardWidth - 1, boardHeight - 1] = upperRightCornerPieceIndex; //top right

        for (int i = 1; i < boardHeight - 1; i++) //assign the left and right hand pieces
        {
            boardData[0,i] = leftSidePieceIndex;
            boardData[boardWidth - 1,i] = rightSidePieceIndex;
        }


        for (int i = 1; i < boardWidth - 1; i++)
        {
            boardData[i,0] = bottomSidePieceIndex;
            boardData[i,boardHeight - 1] = topSidePieceIndex;
        }


    }

    void InstantiateBoard()
    {
        GameObject tempTile;
        for (int i = 0; i < boardWidth; i++)
        {
            for (int ii = 0; ii < boardHeight; ii++)
            {
                switch (boardData[i, ii])
                {
                    case lowerLeftCornerPieceIndex:
                        tempTile = Instantiate(cornerPiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, -90));
                        tempTile = Instantiate(cornerWall, new Vector3(i * tileScale-cornerWallOffsetX, tileHeight+wallHeight, ii * tileScale-cornerWallOffsetZ), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, -180));

                        break;
                    case lowerRightCornerPieceIndex:
                        tempTile = Instantiate(cornerPiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, 180));
                        break;
                    case upperRightCornerPieceIndex:
                        tempTile = Instantiate(cornerPiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, 90));
                        break;
                    case upperLeftCornerPieceIndex:
                        tempTile = Instantiate(cornerPiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, 0));
                        break;

                    case leftSidePieceIndex:
                        tempTile = Instantiate(sidePiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, 90));
                        break;
                    case rightSidePieceIndex:
                        tempTile = Instantiate(sidePiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, -90));
                        break;
                    case topSidePieceIndex:
                        tempTile = Instantiate(sidePiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, -180));
                        break;
                    case bottomSidePieceIndex:
                        tempTile = Instantiate(sidePiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, 0));
                        break;


                    case centerPieceIndex:
                        tempTile = Instantiate(centerPiece, new Vector3(i * tileScale, tileHeight, ii * tileScale), Quaternion.identity);
                        tempTile.transform.Rotate(new Vector3(-90, 0, 0));
                        break;

                    default:
                        break;


                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

