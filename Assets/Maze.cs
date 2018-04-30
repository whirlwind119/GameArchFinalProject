using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    public int _width = 21;
    public int _height = 21;
    public int _num_exits = 1;
    public Material _floor_mat;
    public Material _maze_mat;

    List<List<bool>> grid = new List<List<bool>>();

	// Use this for initialization
	void Start () 
    {
		for (int i = 0; i < _width; ++i) 
        {
            List<bool> temp = new List<bool>();
            for (int j = 0; j < _height; ++j) 
            {
                temp.Add(false);
            }
            grid.Add(temp);
        }
        createMaze(_width-1, _height-1, 0, 0);
        addBorders();
        makeExits();
        constructMesh();
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    void createMaze(int upperx, int uppery, int lowerx, int lowery) 
    {
        if (upperx - lowerx < 3 || uppery - lowery < 3)
        {
            return;
        }

        int randx = Random.Range(lowerx, upperx);
        int randy = Random.Range(lowery, uppery);

        randx = (randx / 2) * 2;
        randy = (randy / 2) * 2;

        for (int i = lowerx; i < upperx; ++i) 
        {
            grid[i][randy] = true;
        }

        for (int i = lowery; i < uppery; ++i) 
        {
            grid[randx][i] = true;
        }

        int dont = Random.Range(0, 4);

        if (dont != 0) 
        {
            int rand = Random.Range(lowery + 1, randy);
            rand = ((rand / 2) * 2) + 1;
            grid[randx][rand] = false;
        }
        if (dont != 1)
        {
            int rand = Random.Range(lowerx + 1, randx);
            rand = ((rand / 2) * 2) + 1;
            grid[rand][randy] = false;
        }
        if (dont != 2) 
        {
            int rand = Random.Range(randy+1, uppery);
            rand = ((rand / 2) * 2) + 1;
            grid[randx][rand] = false;
        }
        if (dont != 3) 
        {
            int rand = Random.Range(randx+1, upperx);
            rand = ((rand / 2) * 2) + 1;
            grid[rand][randy] = false;
        }


        createMaze(randx, randy, lowerx, lowery);
        createMaze(upperx, uppery, randx, randy);
        createMaze(upperx, randy, randx, lowery);
        createMaze(randx, uppery, lowerx, randy);
    }

    void addBorders() {
        //add top and bottom borders 
        for (int i = 0; i < _width; ++i) 
        {
            grid[i].Insert(0, true);
            grid[i].Add(true);
        }

        //add left and right borders
        List<bool> temp1 = new List<bool>();
        List<bool> temp2 = new List<bool>();
        for (int i = 0; i < _height+2; ++i) 
        {
            temp1.Add(true);
        }
        grid.Insert(0, temp1);
        for (int i = 0; i < _height + 2; ++i) 
        {
            temp2.Add(true);
        }
        grid.Add(temp2);
    
        _width = grid.Count;
        _height = grid[0].Count;
    }
    
    void makeExits() {
        int cur_exits = 0;
        //increment _num_exits to account for the entrance
        while (cur_exits < _num_exits+1) 
        {
            //vertical
            if (Random.Range(0, 2) == 0) 
            {
                if (Random.Range(0, 2) == 0) 
                {
                    
                    int randy = Random.Range(1, _height - 1);
                    if (grid[1][randy] == true || 
                        grid[0][randy] == false || 
                        grid[0][randy + 1] == false || 
                        grid[0][randy - 1] == false) 
                    {
                        continue;
                    }
                    else 
                    {
                        grid[0][randy] = false;
                        cur_exits++;
                    }
                    
                }
                else 
                {
                    int randy = Random.Range(1, _height - 1);
                    if (grid[_width-2][randy] == true || 
                        grid[_width-1][randy] == false || 
                        grid[_width-1][randy + 1] == false || 
                        grid[_width-1][randy - 1] == false) 
                    {
                        continue;
                    }
                    else 
                    {
                        grid[_width-1][randy] = false;
                        cur_exits++;
                    }
                }
                
            }
            //horizontal
            else 
            {
                if (Random.Range(0,2) == 0) 
                {
                    int randx = Random.Range(1, _width - 1);
                    if(grid[randx][1] == true || 
                        grid[randx][0] == false || 
                        grid[randx + 1][0] == false || 
                        grid[randx-1][0] == false) 
                    {
                        continue;
                    }
                    else 
                    {
                        grid[randx][0] = false;
                        cur_exits++;
                    }
                    
                }
                else 
                {
                    int randx = Random.Range(1, _width - 1);
                    if(grid[randx][_height-2] == true || 
                        grid[randx][_height-1] == false || 
                        grid[randx + 1][_height-1] == false || 
                        grid[randx - 1][_height-1] == false) 
                    {
                        continue;
                    }
                    else 
                    {
                        grid[randx][_height-1] = false;
                        cur_exits++;
                    }
                }
            }
        }
    }

    void constructMesh() 
        {
        //count number of walls
        int numWalls = 0;
        for (int i = 0; i < _width; ++i) 
        {
            for (int j = 0; j < _height; ++j) 
            {
                if (grid[i][j]) 
                {
                    ++numWalls;
                }
            }
        }

        //prepare mesh components
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        Material[] mats = new Material[2];
        mats[0] = _maze_mat;
        mats[1] = _floor_mat;
        mr.materials = mats;
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        //init mesh data arrays, with a bit of extra data for the two floor triangles (separate tri list, other data in same arrays) 
        Vector3[] vertices = new Vector3[8 * numWalls + 4];
        int[] tri = new int[30 * numWalls];
        int[] floorTris = new int[6];
        Vector3[] normals = new Vector3[8 * numWalls + 4];
        Vector2[] uv = new Vector2[8 * numWalls + 4];

        //generate independent wall pieces
        int curPiece = 0;
        for (int i = 0; i < _width; ++i) 
        {
            for (int r = 0; r < _height; ++r) 
            {
                if (grid[i][r]) 
                {
                    //construct verts (4 top, then 4 bottom)
                    for (int j = 0; j < 2; ++j) {
                        IDictionary<string,bool>  surWalls = getSurroundingWalls(i, r);
                        bool canShrinkHoriz = false;
                        bool canShrinkVert = false;
                        if (Random.Range(0,100) < 20) 
                        {
                            canShrinkHoriz = !(surWalls["left"] || surWalls["right"]);
                            canShrinkVert = !(surWalls["top"] || surWalls["bottom"]);
                        }
                        float amount = Random.Range(.1f, .3f);
                        vertices[curPiece * 8 + 4 * j] = new Vector3(i+(canShrinkHoriz ? amount: 0), 1 - j, r + (canShrinkVert ? amount : 0));
                        vertices[curPiece * 8 + 1 + 4 * j] = new Vector3(i + 1 - (canShrinkHoriz ? amount : 0), 1 - j, r + (canShrinkVert ? amount : 0));
                        vertices[curPiece * 8 + 2 + 4 * j] = new Vector3(i + (canShrinkHoriz ? amount : 0), 1 - j, r + 1 - (canShrinkVert ? amount : 0));
                        vertices[curPiece * 8 + 3 + 4 * j] = new Vector3(i + 1 - (canShrinkHoriz ? amount : 0), 1 - j, r + 1 - (canShrinkVert ? amount : 0));
                    }

                    //construct tris
                    int[,] pieceNumbers;
                    //define vertex orders for each of the 5 wall polys
                    pieceNumbers = new int[,] 
                    {
                        {
                            0,1,2,3
                        },
                        {
                            4,5,0,1
                        },
                        {
                            2,3,6,7
                        },
                        {
                            0,2,4,6
                        },
                        {
                            5,7,1,3
                        }
                    };

                    for (int j = 0; j < 5; ++j) 
                    {
                        tri[curPiece * 30 + 6 * j] = curPiece * 8 + pieceNumbers[j, 0];
                        tri[curPiece * 30 + 1 + 6 * j] = curPiece * 8 + +pieceNumbers[j, 2];
                        tri[curPiece * 30 + 2 + 6 * j] = curPiece * 8 + +pieceNumbers[j, 1];
                        tri[curPiece * 30 + 3 + 6 * j] = curPiece * 8 + +pieceNumbers[j, 2];
                        tri[curPiece * 30 + 4 + 6 * j] = curPiece * 8 + +pieceNumbers[j, 3];
                        tri[curPiece * 30 + 5 + 6 * j] = curPiece * 8 + +pieceNumbers[j, 1];
                    }

                    //construct uvs
                    for (int j = 0; j < 2; ++j)
                    {
                        uv[curPiece * 8 + 4 * j] = new Vector2(j, j);
                        uv[curPiece * 8 + 1 + 4 * j] = new Vector2(1 - j, j);
                        uv[curPiece * 8 + 2 + 4 * j] = new Vector2(j, 1 - j);
                        uv[curPiece * 8 + 3 + 4 * j] = new Vector2(1 - j, 1 - j);
                    }

                    ++curPiece;
                }
            }
        }

        //generate floor verts
        vertices[vertices.Length - 4] = new Vector3(0, 0, 0);
        vertices[vertices.Length - 3] = new Vector3(_width, 0, 0);
        vertices[vertices.Length - 2] = new Vector3(0, 0, _height);
        vertices[vertices.Length - 1] = new Vector3(_width, 0, _height);

        //generate floor tris
        floorTris[0] = vertices.Length - 4;
        floorTris[1] = vertices.Length - 2;
        floorTris[2] = vertices.Length - 3;
        floorTris[3] = vertices.Length - 2;
        floorTris[4] = vertices.Length - 1;
        floorTris[5] = vertices.Length - 3;


        //generate floor uvs
        uv[uv.Length - 4] = new Vector2(0, 0);
        uv[uv.Length - 3] = new Vector2(0, _height);
        uv[uv.Length - 2] = new Vector2(_width, 0);
        uv[uv.Length - 1] = new Vector2(_width, _height);

        //assign mesh vals to mesh component
        mesh.subMeshCount = 2;
        mesh.vertices = vertices;
        mesh.SetTriangles(tri, 0);
        mesh.SetTriangles(floorTris, 1);
        mesh.normals = normals;
        mesh.uv = uv;

        //recalculate normals
        mf.mesh.RecalculateNormals();
    }

    IDictionary<string, bool> getSurroundingWalls(int x, int y) 
    {
        IDictionary<string, bool> surDict = new Dictionary<string, bool>();
        surDict["left"] = false;
        surDict["right"] = false;
        surDict["top"] = false;
        surDict["bottom"] = false;
        if (x != 0 && grid[x - 1][y]) 
        {
            surDict["left"] = true;
        }
        if (x != _width - 1 && grid[x + 1][y]) 
        {
            surDict["right"] = true;
        }
        if (y != 0 && grid[x][y-1]) 
        {
            surDict["top"] = true;
        }
        if (y != _height - 1 && grid[x][y+1]) 
        {
            surDict["bottom"] = true;
        }
        return surDict;
    }

}
