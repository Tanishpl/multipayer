using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using Unity.Burst.CompilerServices;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _cellprefab ;
    [SerializeField] public GameObject origin;
    [SerializeField] private int _mazewidth =20;
    [SerializeField] private int _mazedepth =20;
    [SerializeField] private GameObject[] _LootSpawns;
    private MazeCell[,] _mazeGrid;


    private List<MazeCell> _leftOuterCells ;
    private List<MazeCell> _topOuterCells ;
    private List<MazeCell> _bottomOuterCells ;
    private List<MazeCell> _rightOuterCells;

    private int _wallBuffer = 5; //Must be less than half of the width/depth of the maze
    private int _innerOffset;
    private int _outerOffset;


    void Start()
    {
        _leftOuterCells = new List<MazeCell>();
        _topOuterCells = new List<MazeCell>();
        _bottomOuterCells = new List<MazeCell>();
        _rightOuterCells = new List<MazeCell>();

        origin = gameObject;
        _innerOffset = _wallBuffer + (int)origin.gameObject.transform.position.x;
        _outerOffset = _mazewidth - _wallBuffer + (int)origin.gameObject.transform.position.x;
         
        //IEnumerator MazeCoroutine = StartMaze(origins[1]);
         StartCoroutine(StartMaze(origin));
    }
    //(int)origin.gameObject.transform.position.z
    private IEnumerator StartMaze(GameObject origin)
    {
        _mazeGrid = new MazeCell[_mazewidth, _mazedepth];
        for (int x = 0; x < _mazewidth; x++)
        {
            for (int z = 0; z < _mazedepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_cellprefab, new Vector3((int)origin.gameObject.transform.position.x+x, origin.gameObject.transform.position.y, (int)origin.gameObject.transform.position.z+z), Quaternion.identity);
               
                if (((int)_mazeGrid[x, z].transform.position.z) == ((int)origin.transform.position.z+_mazedepth-1)) // && (_innerOffset < _mazeGrid[x,z].transform.position.x) && (_outerOffset > _mazeGrid[x, z].transform.position.x))
                {
                    _topOuterCells.Add(_mazeGrid[x, z]);
                    //print($"added,top " );
                }
                if ((int)_mazeGrid[x, z].transform.position.x == ((int)origin.transform.position.x+_mazewidth-1))// && (_innerOffset < _mazeGrid[x, z].transform.position.z) && (_outerOffset > _mazeGrid[x, z].transform.position.z))
                {
                   _rightOuterCells.Add(_mazeGrid[x, z]);
                    //print("added right");
                }
                if ((int)_mazeGrid[x, z].transform.position.z == (int)origin.transform.position.z) //&& (_innerOffset < _mazeGrid[x, z].transform.position.x) && (_outerOffset > _mazeGrid[x, z].transform.position.x))
                {

                    _bottomOuterCells.Add(_mazeGrid[x, z]);
                    //print("added bottom");
                }
                if ((int)_mazeGrid[x, z].transform.position.x == (int)origin.transform.position.x ) //&& (_innerOffset < _mazeGrid[x, z].transform.position.z) && (_outerOffset > _mazeGrid[x, z].transform.position.z))
                {
                    _leftOuterCells.Add(_mazeGrid[x, z]);
                    //print("added left");
                }
               
            }
        }
        //yields-- so this should be the last to be called 
        yield return GenerateMaze(null, _mazeGrid[0, 0]); //--actually read this one--there is not previous and the current is the starting point-should turn this into a variable later so i can make it movebale
        print($"cortiune finskh {gameObject.name}");
        for (int x = 0; x < _mazewidth; x++)
        {
            for (int z = 0; z < _mazedepth; z++)
            {
                _mazeGrid[x,z].transform.position -= _mazeGrid[x, z].GroundRaycast();
            }
        }
        print($"top: {_topOuterCells.Count}  bottom: {_bottomOuterCells.Count}   right: {_rightOuterCells.Count}  left;{_leftOuterCells.Count}");
        //print($"top: {_topOuterCells}  bottom: {_bottomOuterCells}   right: {_rightOuterCells}  left;{_leftOuterCells}");
        CreateEnterence();
        ItemSpawn();
    }

    private IEnumerator GenerateMaze(MazeCell previous, MazeCell current)
    {
        current.Visit();//set starting pointed as visited so you dont go back
        Clear(previous, current);//to carve out inital walls and get going-- the null is always just 0 for the current to always be larger than and the "previous" just returns nothing

        yield return new WaitForSeconds(0.0f);//this is not nessary but means  i can actually see the maze generate

        MazeCell chosenCell;
        do
        {
            chosenCell = FindNextCell(current);
            if (chosenCell != null) //if there is an unvisited cell
            {
                yield return GenerateMaze(current, chosenCell); //recursion,calls itself until no unvisited cells left(hopefully)
            }
        } while (chosenCell != null);//stops after no cells left in its current radius
       
    }

    private MazeCell FindNextCell(MazeCell currentCell)//returns only 1 cell randomly
    {

        //use nextcells to find the neighboroughing cells to the current cell and put them in a collection
        IEnumerable<MazeCell> unvisitedCells = NextCells(currentCell);//idk why ienumerable works here and not list

        //returns a random cell in that collection of unvisited cells 
        return unvisitedCells.OrderBy(placeholder => Random.Range(1, 10)).FirstOrDefault();//returns null if not any, need firstorderdefault or this thing breaks
    }

    private IEnumerable<MazeCell> NextCells(MazeCell currentCell)//finds what the next cell is and only if the cell isnt visited---returns a collection
    {
        int Xoffset = (int)origin.gameObject.transform.position.x;
        int Zoffset = (int)origin.gameObject.transform.position.z;

        int x = (int)currentCell.transform.position.x - Xoffset;                      //for some reason they need to be explictily casted 
        int z = (int)currentCell.transform.position.z - Zoffset;

        if (x + 1 < _mazewidth) //if in bounds
        {
            MazeCell cellToTheRight = _mazeGrid[x + 1, z];
            if (cellToTheRight.isVisited == false)
            {
                yield return cellToTheRight;
            }
        }
        if (x - 1 >= 0)//from 0 because that is the min start point that the width depends on 
        {
            MazeCell cellToTheLeft = _mazeGrid[x-1,z];
            if (cellToTheLeft.isVisited == false)
            {
                yield return cellToTheLeft;
            }
        }
        if (z + 1 < _mazedepth) //if in bounds
        {
            MazeCell cellToTheFront = _mazeGrid[x, z+1];
            if (cellToTheFront.isVisited == false)
            {
                yield return cellToTheFront;
            }
        }
        if (z - 1>=0) //if in bounds from back
        {
            MazeCell cellToTheBack = _mazeGrid[x, z - 1];
            if (cellToTheBack.isVisited == false)
            {
                yield return cellToTheBack;
            }
        }
    }
    private void Clear(MazeCell previous, MazeCell current)//clearing walls to carve out maze based on direction
    {
        if (previous == null)
        {
            return;
        }
        if (previous.transform.position.x < current.transform.position.x)
        {
            previous.Clearright();
            current.Clearleft();
            return;
        }
        if (previous.transform.position.x > current.transform.position.x)
        {
            previous.Clearleft();
            current.Clearright();
            return;
        }
        if (previous.transform.position.z < current.transform.position.z)
        {
            previous.Clearfront();
            current.Clearback();
            return;
        }
        if (previous.transform.position.z > current.transform.position.z) // this line is reduendant but it makes it more readable for my dementia
        {
            previous.Clearback();
            current.Clearfront();
            return;
        }
    }

    private void CreateEnterence()
    {
        int chosenWall = Random.Range(_wallBuffer, _mazewidth -_wallBuffer);


        /*identifier leftCell = _leftOuterCells[chosenWall].GetComponent<identifier>();
        leftCell.gameObject.SetActive(false);
        identifier rightCell = _rightOuterCells[chosenWall].GetComponent<identifier>();
        rightCell.gameObject.SetActive(false);
        identifier frontCell = _topOuterCells[chosenWall].GetComponent<identifier>();
        frontCell.gameObject.SetActive(false);
        identifier backCell = _bottomOuterCells[chosenWall].GetComponent<identifier>();
        backCell.gameObject.SetActive(false);
        */

        _leftOuterCells[chosenWall].gameObject.SetActive(false);
        _rightOuterCells[chosenWall].gameObject.SetActive(false);
        _topOuterCells[chosenWall].gameObject.SetActive(false);
        _bottomOuterCells[chosenWall].gameObject.SetActive(false);

    }

    private void ItemSpawn()
    {
        RaycastHit hit;
        int ChosenCellX = Random.Range(_wallBuffer, _mazewidth - _wallBuffer); //using wallbuffer as the buffer for the margin inside the maze where the cell is chosen
        int ChosenCellZ = Random.Range(_wallBuffer, _mazewidth - _wallBuffer);
        int RandItem = Random.Range(0,_LootSpawns.Length);

        GameObject Item = Instantiate(_LootSpawns[RandItem], new Vector3(_mazeGrid[ChosenCellX, ChosenCellZ].transform.position.x, _mazeGrid[ChosenCellX, ChosenCellZ].transform.position.y + 20f , _mazeGrid[ChosenCellX, ChosenCellZ].transform.position.z) , Quaternion.identity);
        if (Physics.Raycast(Item.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 HeightFromGround = new Vector3(0, hit.distance, 0);
            Item.transform.position -= HeightFromGround;
        }
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    void Update()
    {
        
    }
}
