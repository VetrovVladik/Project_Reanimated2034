using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utilities;

public class MapGeneration : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;

    public Vector2 mapSize;
    public int screenHeight;
    public int screenWidth;

    // Can be only be powers of two!
    [Range(0,40)]
    public float outlinePix;

    [Range(0,1)]
    public float obstalcePercent;
    public int seed = 10;

    Coord mapCentre;

    public int regionsNumber;

    public int areasNumber;

    public Vector2 areaSize;

    int buffer = 5;

    InvNormalProbability2D norm;

    void Start()
    {

        GenerateMap();
    }

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords; 

    public void GenerateMap(int scrHgt =360, int scrWdt = 640)
    {
        allTileCoords = new List<Coord>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }

        shuffledTileCoords = new Queue<Coord>(StaticUtility.ShuffleArray(allTileCoords.ToArray(), seed));

        mapCentre = new Coord((int)(mapSize.x/2), (int)(mapSize.y/2));

        string holderName = "Generated Map";

        if (transform.FindChild(holderName))
        { 
             DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapholder = new GameObject(holderName).transform;
        mapholder.parent = transform;

        if(outlinePix%2 != 0)
        {
            float tempF = outlinePix / 2;
            tempF = (int)tempF;
            outlinePix = tempF * 2;
        }

        int xpad = 0;
        int ypad = 0;

        if (((((mapSize.x*40)- scrWdt)/2/40)%1) == 0)
        {
            xpad = 1;
        }
        if (((((mapSize.y * 40) - scrHgt) / 2 / 40) % 1) == 0)
        {
            ypad = 1;
        }

        //Can structs be compared?
        for (int x = 0; x<mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                //Vector3 tilePosition = new Vector3(-(( mapSize.x)*(40 /2 + outlinePix/2)) + xpad * 20f + 40f*x + outlinePix*(x + 1) - outlinePix / 2, -((mapSize.y) * (40/2 + outlinePix/2)) + ypad * 20f + 40f * y + outlinePix* (y +1) - outlinePix / 2, 0);
                Vector3 tilePosition = CoordToPosition(x, y, xpad, ypad);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as Transform; // Quaternion.Euler(Vector3.right*90)
                newTile.parent = mapholder;
            }
        }

        GenerateBase(regionsNumber, areaSize, areasNumber, xpad, ypad, mapholder);

        Region ZoneBuffers = new Region(buffer);


        //Debug.Log(ZoneBuffers.RegionArea[3].lowerRight.x + " " + ZoneBuffers.RegionArea[3].lowerRight.y);
        //Debug.Log(ZoneBuffers.RegionArea[3].upperLeft.x + " " + ZoneBuffers.RegionArea[3].upperLeft.y);

        ZoneBuffers.RegionArea.Add(new Area(new Coord((int)(mapSize.x) - 1 - buffer, (int)(mapSize.y) - 2), new Coord((int)(mapSize.x) - 2, 1)));
        ZoneBuffers.RegionArea.Add(new Area(new Coord(1, buffer), new Coord((int)(mapSize.x) - 2, 1)));
        ZoneBuffers.RegionArea.Add(new Area(new Coord(1, (int)(mapSize.y) - 2), new Coord(buffer, 1)));
        ZoneBuffers.RegionArea.Add(new Area(new Coord(1, (int)(mapSize.y) - 2), new Coord((int)(mapSize.x) - 2, (int)(mapSize.y) - 1 - buffer)));


        Region[] ArtifRegion = new Region[1];
        ArtifRegion[0] = ZoneBuffers;
        //GenerateWalls(ArtifRegion, xpad, ypad, mapholder);

        Area ar3 = new Area(new Coord(0, 8), new Coord(8,3));
        List<Coord> lt = ar3.GetOverlaps(new Area(new Coord(0, 3), new Coord(5, 0)));
        //Debug.Log(lt[0].x + " " + lt[0].y);
        //Debug.Log(lt[lt.Count-1].x + " " + lt[lt.Count - 1].y);
        //Debug.Log(ar3.AreasOverlap(new Area(new Coord(0, 4), new Coord(0, 7))));


    }

    void GenerateBase(int intendedRegions, Vector2 AreaMaxSize, int maxAreas, int xpad, int ypad, Transform mapholder, float closedRegionsPerc = 1f)
    {
        List<float> xVec = new List<float>();

        List<float> yVec = new List<float>();

        yVec.Add(0);
        for (int j = 0 + 6; j < mapSize.y - 6; j++) { yVec.Add(j); }

        xVec.Add(0);
        for (int i = 0 + 6; i < mapSize.x - 6; i++) { xVec.Add(i); }
        xVec.Add(mapSize.x - 1);

        norm = new InvNormalProbability2D(xVec, yVec);

        //For closed regions - might want to destroy walls
        Region[] regions = new Region[intendedRegions];

        int closedRegions = (int)(closedRegionsPerc * intendedRegions);
        int openRegions = intendedRegions - closedRegions;

        //System.Random prng = new System.Random(seed);
        System.Random prng = new System.Random();

        Region[] closedRegionsArr = new Region[closedRegions];
        Region[] openRegionsArr = new Region[openRegions];

        for (int i = 0; i < closedRegions; i++) { closedRegionsArr[i] = new Region(buffer); }

        for (int i = 0; i < closedRegions; i++)
        {
            int Areas = prng.Next(0, maxAreas);
            
            for (int j = 0; j <= Areas; j++)
            {
                //Generate coord and new region 
                Coord coord = norm.GetCell();
                //GenerateArea(AreaMaxSize, coord, i, ref closedRegionsArr);
            }
            //Generate walls
            //Generate floor
            //GenerateWalls(closedRegionsArr, xpad, ypad, mapholder);

        }
        Region[] testReg = new Region[4];
        testReg[0] = new Region(buffer);
        testReg[1] = new Region(buffer);
        testReg[2] = new Region(buffer);
        testReg[3] = new Region(buffer);
        GenerateArea(AreaMaxSize, new Coord(46, 29), 0, ref testReg, false);

        //Region[] buffersReg = new Region[1];
        //buffersReg[0] = new Region();
        //buffersReg[0].AddArea(testReg[0].BufferArea[0]);

        //GenerateArea(AreaMaxSize, new Coord(40, 12), 1, ref testReg);
        GenerateArea(AreaMaxSize, new Coord(20, 29), 1, ref testReg, false);
        GenerateArea(AreaMaxSize, new Coord(20, 12), 2, ref testReg, false);
        GenerateArea(AreaMaxSize, new Coord(46, 15), 3, ref testReg, false);
        GenerateArea(AreaMaxSize, new Coord(38, 15), 3, ref testReg, false);
        GenerateWalls(testReg, xpad, ypad, mapholder);

        for (int i = 0; i < openRegions; i++)
        {
            //Generate coord and new region 
            Coord coord = new Coord(0, 0);
            openRegionsArr[i] = new Region(buffer);

            //Create object NullArea for parents

            int Areas = prng.Next(0, maxAreas + 1);
            for (int j = 0; j < Areas; i++)
            {
                //GenerateArea(AreaMaxSize, coord, i, openRegionsArr);
            }
            //Generate floor
        }

    }

    void GenerateCave()
    {

    }

    /*
    void GenerateObstacles(Transform mapholder, int xpad, int ypad)
    {
        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstalcePercent);

        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true; //Add new value, and then check if that value fits into connected map
            currentObstacleCount++;
            //if (!(randomCoord.Equals(mapCentre)) && MapIsFullyAccessable(obstacleMap, currentObstacleCount))
            if (!(randomCoord == mapCentre) && MapIsFullyAccessable(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y, xpad, ypad);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity) as Transform;
                newObstacle.parent = mapholder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false; //Added value doesn't fit into connected map
                currentObstacleCount--;
            }
        }
    }
    */

    Vector3 CoordToPosition(int x, int y, int xpad, int ypad)
    {
        return new Vector3(-((mapSize.x) * (40 / 2 + outlinePix / 2)) + xpad * 20f + 40f * x + outlinePix * (x + 1) - outlinePix / 2, -((mapSize.y) * (40 / 2 + outlinePix / 2)) + ypad * 20f + 40f * y + outlinePix * (y + 1) - outlinePix / 2, 0);
    }

    /*
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue(); //Get first item
        shuffledTileCoords.Enqueue(randomCoord); //Add it back to the end of the queue 

        return randomCoord;
    }
    */

    public struct Area
    {
        public Coord upperLeft;
        public Coord lowerRight;

        public Area(Coord _upperLeft, Coord _lowerRight)
        {
            upperLeft = _upperLeft;
            lowerRight = _lowerRight;
        }

        public override bool Equals(object obj)
        {
            Area item;
            try
            {
                item = (Area)obj;
            }
            catch (InvalidCastException e)
            {
                throw;
            }

            return this == item;
        }

        public override int GetHashCode()
        {
            return GetHashCode();
        }

        public static bool operator ==(Area b1, Area b2)
        {
            return b1.lowerRight == b2.lowerRight && b1.upperLeft == b2.upperLeft;
        }

        public static bool operator !=(Area b1, Area b2)
        {
            return !(b1 == b2);
        }

        public bool AreasOverlap(Area a2)
        {
            Coord l1 = this.upperLeft;
            Coord r1 = this.lowerRight;

            Coord l2 = a2.upperLeft;
            Coord r2 = a2.lowerRight;

            // If one rectangle is on left side of other
            if (l1.x > r2.x || l2.x > r1.x)
            {
                return false;
            }


            // If one rectangle is above other
            if (l1.y < r2.y || l2.y < r1.y)
            {
                return false;
            }


            return true;
        }

        public List<Coord> GetOverlaps(Area a2)
        {
            if (!this.AreasOverlap(a2))
            {
                return new List<Coord>();
            }

            Coord l1 = this.upperLeft;
            Coord r1 = this.lowerRight;

            Coord l2 = a2.upperLeft;
            Coord r2 = a2.lowerRight;

            List<Coord> coords = new List<Coord>();
            float left = Mathf.Max(l1.x, l2.x);
            float right = Math.Min(r1.x, r2.x);
            float bottom = Mathf.Max(r1.y, r2.y);
            float top = Math.Min(l1.y, l2.y);

            for (int x = 0; x <= right - left; x++)
            {
                for (int y = 0; y <= top - bottom; y++)
                {
                    coords.Add(new Coord((int)(left + x), (int)(top - y)));
                }
            }

            return coords;
        }
    }

    public class Region
    {
        int buffer;
        //List<Area> Areas = new List<Area>();
        public List<Area> BufferArea { get; protected set; }
        public List<Area> RegionArea { get; protected set; }
        public List<Area> ParentArea { get; protected set; }
        public List<Area> InnerIntersections { get; protected set; }


        public static void CheckZoneBuffers(Area a, Vector2 mapSize, int _buffer)
        {
            Region ZoneBuffers = new Region(_buffer);
            ZoneBuffers.RegionArea.Add(new Area(new Coord((int)(mapSize.x) - 1 - _buffer, (int)(mapSize.y) - 2), new Coord((int)(mapSize.x) - 2, 1)));
            ZoneBuffers.RegionArea.Add(new Area(new Coord(1, _buffer), new Coord((int)(mapSize.x) - 2, 1)));
            ZoneBuffers.RegionArea.Add(new Area(new Coord(1, (int)(mapSize.y) - 2), new Coord(_buffer, 1)));
            ZoneBuffers.RegionArea.Add(new Area(new Coord(1, (int)(mapSize.y) - 2), new Coord((int)(mapSize.x) - 2, (int)(mapSize.y) - 1 - _buffer)));
        }

        public Region(int _buffer)
        {
            buffer = _buffer;
            RegionArea = new List<Area>();
            BufferArea = new List<Area>();
            ParentArea = new List<Area>();
        }

        public void AddArea(Area a)
        {
            RegionArea.Add(a);
            RecalcBuffers(a);
            RecalcParents(a);
        }

        void RecalcBuffers(Area a)
        {
            Area a2 = new Area(a.upperLeft.Add(-buffer, buffer), a.lowerRight.Add(buffer, -buffer));
            BufferArea.Add(a2);
        }

        void RecalcParents(Area a)
        {
            Area a1 = new Area(a.upperLeft.Add(1, -1), a.lowerRight.Add(-1, 1));
            ParentArea.Add(a1);
        }

        void RecalcIntersections(Area a)
        {

        }

        public bool CheckBufferRegionOverlap(Area a)
        {
            foreach (Area ba in BufferArea)
            {
                if (BufferArea.Count >= 1 && a.AreasOverlap(ba))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckParentRegionOverlap(Area a)
        {
            foreach (Area pa in ParentArea)
            {
                if (ParentArea.Count >=1 && a.AreasOverlap(pa))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckInnerRegionOverlap(Area a)
        {
            int overlaps = 0;
            int corners = 0;
            foreach (Area ra in RegionArea)
            {
                if (RegionArea.Count >=1 && a.AreasOverlap(ra))
                {
                    int[] xs = { ra.lowerRight.x, ra.upperLeft.x };
                    int[] ys = { ra.lowerRight.y, ra.upperLeft.y };
                    foreach (float pointX in xs)
                    {
                        foreach (float pointY in ys)
                        {
                            if (pointX == a.upperLeft.x && pointY == a.upperLeft.x)
                            {
                                corners++;
                            }
                        }
                    }
                    overlaps++;
                }
            }
            if (overlaps <= 1) return false;
            else if (overlaps == 2 && corners >= 1) return false;
            else if (overlaps == 3 && corners >= 3) return false;
            else return true;
        }

        public bool CheckInnerBorders(Area a, int parent)
        {
            int ind = -1;

            foreach (Area ba in BufferArea)
            {
                ind++;
                if (ind == parent) continue;

                if (ba.AreasOverlap(a))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckParentRegionSustainability(Area a, int parent)
        {
            int reg = -1;
            int ind = -1;

            int err = 0;
            foreach (Area ba in BufferArea)
            {
                ind++;
                if (ind == parent) continue;
                
                if (ba.AreasOverlap(a))
                {
                    if (RegionArea[ind].AreasOverlap(a))
                    {
                        List<Coord> coords = RegionArea[ind].GetOverlaps(a);
                        if ((coords[0].x - coords[coords.Count - 1].x == 0 || coords[0].y - coords[coords.Count - 1].y == 0))
                        {
                            if (a.lowerRight.x - a.upperLeft.x == 0 || a.upperLeft.y - a.lowerRight.y == 0)
                            {
                                continue;
                            }

                            if (coords[0].x - coords[coords.Count - 1].x == 0 && Mathf.Abs(coords[0].y - coords[coords.Count - 1].y) >= buffer + 2 - 1)
                            {
                                continue;
                            }
                            else if (coords[0].y - coords[coords.Count - 1].y == 0 && Mathf.Abs(coords[0].x - coords[coords.Count - 1].x) >= buffer + 2 - 1)
                            {
                                continue;
                            }
                            else
                            {
                                err++;
                            }
                        }
                        else
                        {
                            err++;
                        }
                    }
                    else
                    {
                        err++;
                    }
                }
            }
            if (err > 0)
            {
                return false;
            }
            return true;
        }
    }

    bool MapIsFullyAccessable(bool[,] obstacleMap, int currentObastcleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)]; //Map cells, haven't been checked
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1; //Centre tile 

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            //for (int x = -3; x <= 3; x++)
            {
                for (int y = -1; y <= 1; y++)
                //for (int y = -3; y <= 3; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0) //Check only for horizontal and vertical neighbours 
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) //Check if neighbour is not of boundaries 
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY]) //Check if map hasn't been checked and not occupied by obstacle
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY)); //Add this cell to process, and increment accessible count
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObastcleCount);
        return targetAccessibleTileCount == accessibleTileCount; //Check if there is no cell, that has been blocked by added obstacle 
    }

    bool ClustersMapIsFullyAccessable(bool[,] obstacleMap, int currentObastcleCount, int clusterSize)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)]; //Map cells, haven't been checked
        Queue<Coord> queue = new Queue<Coord>();
        for (int x0 = 0; x0 < clusterSize; x0++)
        {
            for (int y0 = 0; y0 < clusterSize; y0++)
            {
                queue.Enqueue(new Coord(mapCentre.x + x0, mapCentre.x + x0));
                mapFlags[mapCentre.x + x0, mapCentre.x + x0] = true;
            }
        }

        int accessibleTileCount = 1; //Centre tile 

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x1 = -1; x1 <= 1; x1++)
            //for (int x = -3; x <= 3; x++)
            {
                for (int y1 = -1; y1 <= 1; y1++)
                //for (int y = -3; y <= 3; y++)
                {
                    int neighbourX = tile.x + x1;
                    int neighbourY = tile.y + y1;
                    if (x1 == 0 || y1 == 0) //Check only for horizontal and vertical neighbours 
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) //Check if neighbour is not of boundaries 
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY]) //Check if map hasn't been checked and not occupied by obstacle
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY)); //Add this cell to process, and increment accessible count
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObastcleCount);
        return targetAccessibleTileCount == accessibleTileCount; //Check if there is no cell, that has been blocked by added obstacle 
    }

    void GenerateArea(Vector2 maxArea, Coord startCoord, int region, ref Region[] regions, bool isChild)
    {
        int parent = -1;
        int side = -1;

        List<int> parents = new List<int>();
        List<int> sides = new List<int>();
        List<int[]> xvars = new List<int[]>();
        List<int[]> yvars = new List<int[]>();

        //Debug.Log(new Vector2(startCoord.x, startCoord.y) + " " + region);
        Dictionary<int, Area> dictAreas = new Dictionary<int, Area>();

        if (maxArea.x < buffer + 2)
        {
            maxArea = new Vector2(buffer + 2, maxArea.y);
        }
        if (maxArea.y < buffer + 2)
        {
            maxArea = new Vector2(maxArea.x, buffer + 2);
        }

        float maxX = maxArea.x;
        float maxY = maxArea.y;

        //Debug.Log(maxArea);

        List<Coord> startCoords = new List<Coord>();

        int reserve = 4;

        System.Random randArea = new System.Random();

        //Looking for right coord to start from 
        if (regions[region].RegionArea.Count >= 1 && isChild)
        {
            //Debug.Log("regions exists");

            //Reserve = extra tries to generate area 
            List<Area> parRegion = regions[region].RegionArea;
            for (int i = 0; i < 1; i++)
            {
                parent = randArea.Next(0, parRegion.Count);
                Area parArea = parRegion[parent];

                int sideToGrow = -1;

                //Need to check if random generator is working properly 
                //Debug.Log(new Vector2(parArea.lowerRight.x, parArea.lowerRight.y));

                //Determine max distance from the corner
                int width = parArea.lowerRight.x - parArea.upperLeft.x ;
                int height = parArea.upperLeft.y - parArea.lowerRight.y ;
                int availablePadw = (int)((width + 1- 2 - buffer) / 2);
                int availablePadh = (int)((height + 1 - 2 - buffer) / 2);
                Debug.Log(width + " " + height);

                //Now we choose suitable random corner
                int randCoord = randArea.Next(0, 2);
                int randSide = randArea.Next(0, 2);
                int randOrient = randArea.Next(0, 2);
                int cornerY = 0;
                int cornerX = 0;

                Coord cornerCoord = randCoord == 0 ? parArea.lowerRight : parArea.upperLeft;
                cornerY = randSide == 0 ? randArea.Next(0, availablePadh+1) : 0;
                cornerX = randSide == 1 ? randArea.Next(0, availablePadw+1) : 0;

                if (randOrient == 1 && randSide == 1)
                {
                    cornerX = width - cornerX;
                }
                if (randOrient == 1 && randSide == 0)
                {
                    cornerY = height - cornerY;
                }

                //Debug.Log(cornerX + " " + cornerY + " " + randCoord + " " + randOrient);
                //Debug.Log(availablePadh + " " + availablePadw);

                cornerCoord = randCoord == 0 ? cornerCoord.Add(-cornerX, cornerY) : cornerCoord.Add(cornerX, -cornerY);
                if (!regions[region].CheckInnerRegionOverlap(new Area(cornerCoord, cornerCoord)))
                {
                    startCoords.Add(cornerCoord);
                    if (randCoord == 0 && randSide == 0 && randOrient == 0)
                    {
                        xvars.Add(new int[] { 1 });
                        yvars.Add(new int[] { 1 });
                        //right 
                        sideToGrow = 2;
                    }
                    if (randCoord == 0 && randSide == 0 && randOrient == 1)
                    {
                        xvars.Add(new int[] { 1 });
                        yvars.Add(new int[] { -1 });
                        //right
                        sideToGrow = 2;
                    }
                    if (randCoord == 0 && randSide == 1 && randOrient == 0)
                    {
                        xvars.Add(new int[] {-1 });
                        yvars.Add(new int[] { -1 });
                        //bottom
                        sideToGrow = 3;
                    }
                    if (randCoord == 0 && randSide == 1 && randOrient == 1)
                    {
                        xvars.Add(new int[] { 1 });
                        yvars.Add(new int[] { -1 });
                        //bottom
                        sideToGrow = 3;
                    }
                    if (randCoord == 1 && randSide == 0 && randOrient == 0)
                    {
                        xvars.Add(new int[] { -1 });
                        yvars.Add(new int[] { -1 });
                        //left
                        sideToGrow = 0;
                    }
                    if (randCoord == 1 && randSide == 0 && randOrient == 1)
                    {
                        xvars.Add(new int[] { -1 });
                        yvars.Add(new int[] { 1 });
                        //left
                        sideToGrow = 0;
                    }
                    if (randCoord == 1 && randSide == 1 && randOrient == 0)
                    {
                        xvars.Add(new int[] { 1 });
                        yvars.Add(new int[] { 1 });
                        //top
                        sideToGrow = 1;
                    }
                    if (randCoord == 1 && randSide == 1 && randOrient == 1)
                    {
                        xvars.Add(new int[] { -1 });
                        yvars.Add(new int[] { 1 });
                        //top
                        sideToGrow = 1;
                    }
                    sides.Add(sideToGrow);
                    parents.Add(parent);
                    //xvars.Add(new int[] { -1, 1 });
                    //yvars.Add(new int[] { -1, 1 });
                }
                else
                {
                    if (reserve >= 1)
                    {
                        i--;
                        reserve--;
                    }
                }
            }
        }
        else
        {
            startCoords.Add(startCoord);
            xvars.Add(new int[] { -1, 1 });
            yvars.Add(new int[] { -1, 1 });
            parents.Add(parent);
            sides.Add(side);
        }

        //Build areas from chosen coords, compute area for each
        //foreach (Coord start in startCoords)
        for (int coordNum = 0; coordNum < startCoords.Count; coordNum++)
        {
            Coord start = startCoords[coordNum];

            int[] xcont = new int[] { -1, 1 };
            int[] ycont = new int[] { -1, 1 };

            if (coordNum < xvars.Count) { xcont = xvars[coordNum]; }
            if (coordNum < yvars.Count) { ycont = yvars[coordNum]; }
            //Try to grow Area in four directions - count total area
            Area area = new Area(start, start);

            int ul_up = 0;
            int ul_right = 0;
            int lr_up = 0;
            int lr_right = 0;

            int ySize = 0;
            int xSize = 0;

            foreach (int x in xcont)
            {
                foreach (int y in ycont)
                {
                    if (x!=0 && y != 0)
                    {
                        //Reinitialize area so that previous changes to area are cancelled
                        area = new Area(start, start);
                        maxArea = new Vector2(maxX, maxY);

                        ul_up = 0;
                        ul_right = 0;
                        lr_up = 0;
                        lr_right = 0;

                        int deltaX = 0;
                        int deltaY = 0;

                        if (x==1 & y == 1)
                        {
                            ul_up = 1;
                            lr_right = 1;
                            deltaX = (int)(mapSize.x - 1 - (start.x ) - Convert.ToInt32(Convert.ToBoolean((int)maxArea.x)) * x * (maxArea.x - 1));
                            deltaY = (int)(mapSize.y - 1 - (start.y ) - Convert.ToInt32(Convert.ToBoolean((int)maxArea.y)) * y * (maxArea.y - 1));

                        }
                        else if (x == 1 & y == -1)
                        {
                            lr_right = 1;
                            lr_up = -1;
                            deltaX = (int)(mapSize.x - 1 - (start.x) - Convert.ToInt32(Convert.ToBoolean((int)maxArea.x)) * x * (maxArea.x-1));
                            deltaY = (int)((start.y) + y * Convert.ToInt32(Convert.ToBoolean((int)maxArea.y)) * (maxArea.y - 1));
                        }
                        else if (x == -1 & y == 1)
                        {
                            ul_right = -1;
                            ul_up = 1;         
                            deltaX = (int)((start.x) + x * Convert.ToInt32(Convert.ToBoolean((int)maxArea.x)) * (maxArea.x - 1));
                            deltaY = (int)(mapSize.y - 1 - (start.y) - Convert.ToInt32(Convert.ToBoolean((int)maxArea.y)) * y * (maxArea.y - 1));
                        }
                        else if (x == -1 & y == -1)
                        {
                            ul_right = -1;
                            lr_up = -1;
                            deltaX = (int)((start.x - 1) + Convert.ToInt32(Convert.ToBoolean((int)maxArea.x)) * x * (maxArea.x - 1));
                            deltaY = (int)((start.y) + Convert.ToInt32(Convert.ToBoolean((int)maxArea.y)) * y * (maxArea.y - 1));
                        }

                        bool jumpX = false;
                        bool jumpY = false;

                        //Debug.Log(new Vector2(x, y) + " " +deltaX + " " + " " + deltaY + " " + new Vector2(start.x, start.y));
                        //Debug.Log("maxX " +maxX);

                        if (deltaX == 0 || (deltaX > 0 && deltaX <= buffer+1 && (maxX + deltaX - buffer - 1 < buffer + 2))) //Check if building can align to borders or: 
                        {                                                                                                   //building is on border, and is its zone before buffer is lower than we need
                            maxArea = new Vector2(maxArea.x + deltaX - buffer - 1, maxArea.y); //Check standrad iterations before the buffer
                            jumpX = true; //After the buffer if there were no overlaps with regions, cycle will jump to border and try it at last 
                        }
                        else if (deltaX > 0 && deltaX <= buffer && !(maxX + deltaX - buffer - 1 < buffer + 2)) 
                        {
                            maxArea = new Vector2(maxArea.x + deltaX - buffer - 1, maxArea.y);
                            jumpX = false;
                        }
                        else if(deltaX < 0)
                        {
                            if (maxX + deltaX >= buffer +2)
                            {
                                maxArea = new Vector2(maxArea.x + deltaX - buffer - 1, maxArea.y);
                                jumpX = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        //Determines if x has connected to something. Strategy is simple: if connection is present try to perform sufficient jump, and then grow if you can. 
                        bool connectedX = false;
                        //If y is connected we can try to grow it further
                        bool connectedY = false;

                        //Debug.Log("JumpX " + jumpX);
                        //Debug.Log("JumpY " + jumpY);
                        //Debug.Log("deltaX " + deltaX);
                        //Debug.Log("deltaY " + deltaY);


                        if (deltaY == 0 || (deltaY > 0 && deltaY <= buffer+1 && (maxY + deltaY - buffer - 1 < buffer + 2))) //Check if building can align to borders or: 
                        {                                                                      //building is on border, and is its zone before buffer is lower than we need
                            maxArea = new Vector2(maxArea.x, maxArea.y + deltaY - buffer - 1); //Check standrad iterations before the buffer
                            jumpY = true; //After the buffer if there were no overlaps with regions, cycle will jump to border and try it at last 
                        }
                        else if (deltaY > 0 && deltaY <= buffer && !(maxY + deltaY - buffer - 1 < buffer + 2))
                        {
                            maxArea = new Vector2(maxArea.x, maxArea.y + deltaY - buffer - 1); 
                            jumpY = false; 
                        }
                        else if (deltaY < 0)
                        {
                            if (maxX + deltaY >= buffer + 2)
                            {
                                maxArea = new Vector2(maxArea.x, maxArea.y + deltaY - buffer - 1);
                                jumpY = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        //Debug.Log(new Vector2(maxArea.x, maxArea.y) + " " + new Vector2(start.x, start.y));

                        for (int i = 1; i <= maxArea.x; i++)
                        {
                            bool outerBreak = false;

                            if (maxArea.x < buffer + 2)
                            {
                                outerBreak = true;
                                break;
                            }

                            int first = 1;
                            if (i == 1) { first = 0; }

                            area.upperLeft.Add(ul_right * first, 0);
                            area.lowerRight.Add(lr_right * first, 0);

                            for (int j = 0; j < regions.Length; j++)
                            {

                                if (j == region)
                                {
                                    //Check for possible overlaps with parent
                                    if (regions[j].CheckParentRegionOverlap(area))
                                    {
                                        area.upperLeft.Add(-ul_right * first, 0);
                                        area.lowerRight.Add(-lr_right * first, 0);

                                        outerBreak = true;
                                        break;
                                    }
                                    
                                    //If there are overlap with inner buffers -lauch procedure of jump
                                    if (regions[j].CheckInnerBorders(area, parents[coordNum]))
                                    {
                                        Debug.Log("overlap with parent " + x + " " + y + " " + region);
                                        //If there are enough steps to connect to collided area, we can try to connect
                                        if (maxArea.x - i < buffer)
                                        {
                                            area.upperLeft.Add(-ul_right * first, 0);
                                            area.lowerRight.Add(-lr_right * first, 0);
                                            Debug.Log("Insufficient reserve x");
                                            outerBreak = true;
                                            break;
                                            
                                        }
                                        //Connecting...
                                        else
                                        {
                                            area.upperLeft.Add(ul_right * first * buffer, 0);
                                            area.lowerRight.Add(lr_right * first * buffer, 0);
                                            jumpX = !jumpX;

                                            Debug.Log("uppeleft " + new Vector2(area.upperLeft.x, area.upperLeft.y) + " lower right " + new Vector2(area.lowerRight.x, area.lowerRight.y));

                                            if (regions[j].CheckParentRegionSustainability(area, parents[coordNum]))
                                            {
                                                Debug.Log("Region is sustainable x");
                                                for (int l = 0; l < regions.Length; l++)
                                                {
                                                    if (l == parents[coordNum]) continue;
                                                    if (regions[l].CheckParentRegionOverlap(area) || regions[l].CheckBufferRegionOverlap(area))
                                                    {
                                                        area.upperLeft.Add(-ul_right * first * buffer, 0);
                                                        area.lowerRight.Add(-lr_right * first * buffer, 0);
                                                        outerBreak = true;
                                                        jumpX = !jumpX;
                                                        break;
                                                    }
                                                }
                                                connectedX = true;
                                                outerBreak = true;
                                                break;
                                            }
                                            else
                                            {
                                                area.upperLeft.Add(-ul_right * first * buffer, 0);
                                                area.lowerRight.Add(-lr_right * first * buffer, 0);
                                                outerBreak = true;
                                                jumpX = !jumpX;
                                                break;
                                            }
                                        }
                                    }
                                    continue;
                                }

                                //Secondly we check possible overlaps with outer buffers
                                if (regions[j].CheckBufferRegionOverlap(area))
                                {
                                    area.upperLeft.Add(-ul_right * first, 0);
                                    area.lowerRight.Add(-lr_right * first, 0);

                                    outerBreak = true;
                                    break;
                                }
                            }
                            if (outerBreak == true)
                            {
                                break;
                            }
                        }

                        for (int i = 1; i <= maxArea.y; i++)
                        {
                            int first = 1;
                            if (i == 1) { first = 0; }
                            bool outerBreak = false;

                            if (maxArea.y < buffer + 2)
                            {
                                outerBreak = true;
                                break;
                            }

                            area.upperLeft.Add(0, ul_up * first);
                            area.lowerRight.Add(0, lr_up * first);

                            for (int j = 0; j < regions.Length; j++)
                            {

                                if (j == region)
                                {
                                    //Check for possible overlaps with parent
                                    if (regions[j].CheckParentRegionOverlap(area))
                                    {
                                        area.upperLeft.Add(0, -ul_up * first);
                                        area.lowerRight.Add(0, -lr_up * first);
                                  
                                        outerBreak = true;
                                        break;
                                    }

                                    //If there are overlap with inner buffers -lauch precuder of jump
                                    if (regions[j].CheckInnerBorders(area, parents[coordNum]))
                                    {
                                        Debug.Log("overlap with parent " + x + " " + y + " " + region);
                                        //If there are enough steps to connect to collided area, we can try to connect
                                        if (maxArea.y - i < buffer)
                                        {
                                            area.upperLeft.Add(0, -ul_up * first);
                                            area.lowerRight.Add(0, -lr_up * first);
                                            Debug.Log("Insufficient reserve y");
                                            outerBreak = true;
                                            break;
                                        }
                                        //Connecting...
                                        else
                                        {
                                            int areaToAdd = 0;

                                            if (connectedX && !connectedY)
                                            {
                                                areaToAdd = buffer+2;
                                                //jumpY = !jumpY;
                                            }
                                            else if (connectedX && connectedY)
                                            {
                                                areaToAdd = 1;
                                                //jumpY = !jumpY;
                                            }
                                            else
                                            {
                                                areaToAdd = buffer;
                                                jumpY = !jumpY;
                                            }

                                            area.upperLeft.Add(0, ul_up * first * areaToAdd);
                                            area.lowerRight.Add(0, lr_up * first * areaToAdd);

                                            Debug.Log("uppeleft " + new Vector2(area.upperLeft.x, area.upperLeft.y) + " lower right " + new Vector2(area.lowerRight.x, area.lowerRight.y));

                                            if (regions[j].CheckParentRegionSustainability(area, parents[coordNum]))
                                            {
                                                
                                                Debug.Log("Region is sustainable y");
                                                for (int l = 0; l < regions.Length; l++)
                                                {
                                                    if (l == parents[coordNum]) continue;
                                                    if (regions[l].CheckParentRegionOverlap(area) || regions[l].CheckBufferRegionOverlap(area))
                                                    {
                                                        area.upperLeft.Add(0, -ul_up * first * areaToAdd);
                                                        area.lowerRight.Add(0, -lr_up * first * areaToAdd);
                                                        outerBreak = true;
                                                        if (!connectedX)
                                                        {
                                                            jumpY = !jumpY;
                                                        }
                                                        break;
                                                    }
                                                }

                                                if (connectedX && !connectedY)
                                                {
                                                    connectedY = true;
                                                }
                                                else
                                                {
                                                    outerBreak = true;
                                                    break;
                                                }

                                            }
                                            else
                                            {
                                                area.upperLeft.Add(0, -ul_up * first * areaToAdd);
                                                area.lowerRight.Add(0, -lr_up * first * areaToAdd);
                                                outerBreak = true;
                                                jumpY = !jumpY;
                                                break;
                                            }
                                        }
                                    }
                                    continue;
                                }
                                if (regions[j].CheckBufferRegionOverlap(area))
                                {
                                    area.upperLeft.Add(0, -ul_up * first);
                                    area.lowerRight.Add(0, -lr_up * first);

                                    outerBreak = true;
                                    break;
                                }
                            }
                            if (outerBreak == true)
                            {
                                break;
                            }
                        }

                        if (jumpX)
                        {
                            //Debug.Log("jumpX " + new Vector2(start.x, start.y));
                            int extraWidth = (int)(maxX + deltaX - 1 - (area.lowerRight.x - area.upperLeft.x));
                            area.upperLeft.Add(ul_right * extraWidth, 0);
                            area.lowerRight.Add(lr_right * extraWidth, 0);
                            for (int j = 0; j < regions.Length; j++)
                            {
                                if (j == region)
                                {
                                    //Check for possible overlaps with parent
                                    if (regions[j].CheckParentRegionOverlap(area))
                                    {
                                        area.upperLeft.Add(-ul_right * extraWidth, 0);
                                        area.lowerRight.Add(-lr_right * extraWidth, 0);
                                        break;
                                    }

                                    //If there are overlap with inner buffers -launch procedure of jump
                                    if (regions[j].CheckInnerBorders(area, parents[coordNum]))
                                    {
                                        if (!regions[j].CheckParentRegionSustainability(area, parents[coordNum]))
                                        {
                                            area.upperLeft.Add(-ul_right * extraWidth, 0);
                                            area.lowerRight.Add(-lr_right * extraWidth, 0);
                                            break;
                                        }
                                    }
                                    continue;
                                }
                                if (regions[j].CheckBufferRegionOverlap(area))
                                {
                                    area.upperLeft.Add(-ul_right * extraWidth, 0);
                                    area.lowerRight.Add(-lr_right * extraWidth, 0);
                                    break;
                                }
                            }
                            //Debug.Log(extraWidth);
                        }

                        if (jumpY)
                        {
                            //Debug.Log("jumpY " + new Vector2(start.x, start.y));
                            int extraHeight = (int)(maxY + deltaY - 1- (area.upperLeft.y - area.lowerRight.y));
                            area.upperLeft.Add(0, ul_up * extraHeight);
                            area.lowerRight.Add(0, lr_up * extraHeight);
                            for (int j = 0; j < regions.Length; j++)
                            {
                                if (j == region)
                                {
                                    //Check for possible overlaps with parent
                                    if (regions[j].CheckParentRegionOverlap(area))
                                    {
                                        area.upperLeft.Add(0, -ul_up * extraHeight);
                                        area.lowerRight.Add(0, -lr_up * extraHeight);
                                        break;
                                    }

                                    //If there are overlap with inner buffers -launch procedure of jump
                                    if (regions[j].CheckInnerBorders(area, parents[coordNum]))
                                    {
                                        if (!regions[j].CheckParentRegionSustainability(area, parents[coordNum]))
                                        {
                                            area.upperLeft.Add(0, - ul_up * extraHeight);
                                            area.lowerRight.Add(0, -lr_up * extraHeight);
                                            break;
                                        }
                                    }
                                    continue;
                                }

                                if (regions[j].CheckBufferRegionOverlap(area))
                                {
                                    area.upperLeft.Add(0, -ul_up * extraHeight);
                                    area.lowerRight.Add(0, -lr_up * extraHeight);
                                    break;
                                }
                            }
                            //Debug.Log(extraHeight);
                        }
                        //After all adjustments have been made we calculate area for specific, well... area 
                        int squareArea = Mathf.Abs((area.upperLeft.y - area.lowerRight.y + 1) * (area.lowerRight.x - area.upperLeft.x + 1));

                        if ((Mathf.Abs(area.upperLeft.y +1- area.lowerRight.y) >= buffer +2) && (Mathf.Abs(area.lowerRight.x +1- area.upperLeft.x) >= buffer + 2))
                        {
                            //Debug.Log(squareArea);
                            if (dictAreas.ContainsKey(squareArea))
                            {
                                dictAreas[squareArea] = area;
                            }
                            else
                            {
                                dictAreas.Add(squareArea, area);
                            }
                        }
                    }
                }
            }
        }
        if ((regions[region] != null) && (dictAreas.Count > 0))
        {
            int maxAr = 0;
            foreach (int sqArea in dictAreas.Keys)
            {
                if (sqArea >= maxAr) maxAr = sqArea;
            }
            regions[region].AddArea(dictAreas[maxAr]);
        }  else if ((regions[region] == null) && (dictAreas.Count > 0))
        {
            int maxAr = 0;
            foreach (int sqArea in dictAreas.Keys)
            {
                if (sqArea >= maxAr) maxAr = sqArea;
            }
            regions[region] = new Region(buffer);
            regions[region].AddArea(dictAreas[maxAr]);
        }
        foreach (Region reg in regions)
        {
            if( reg != null)
            {
                foreach (Area ar in reg.RegionArea)
                {
                    //Debug.Log("lowerRight" + new Vector2(ar.lowerRight.x, ar.lowerRight.y));
                    //Debug.Log("upperLeft " + new Vector2(ar.upperLeft.x, ar.upperLeft.y));
                }
            }
        }
    }

    void GenerateWalls(Region[] regions, int xpad, int ypad, Transform mapholder)
    {
        foreach (Region region in regions)
        {
            foreach(Area area in region.RegionArea)
            {
                for (int x = 0; x < area.lowerRight.x- area.upperLeft.x+1; x++)
                {
                    for (int y = 0; y <  area.upperLeft.y - area.lowerRight.y+1; y++)
                    {
                        //Debug.Log("Skwanch!");
                        Coord startCoord = new Coord(area.upperLeft.x+x, area.upperLeft.y-y);
                        Vector3 obstaclePosition = CoordToPosition(startCoord.x, startCoord.y, xpad, ypad);
                        Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity) as Transform;
                        newObstacle.parent = mapholder;
                    }
                }
            }
        }
    }
}

public struct Coord
{
    public int x;
    public int y;

    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public static bool operator ==(Coord c1, Coord c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(Coord c1, Coord c2)
    {
        return !(c1 == c2);
    }

    public override bool Equals(object obj)
    {
        Coord item; 
        try
        {
            item = (Coord)obj;
        }
        catch (InvalidCastException e)
        {
            throw;
        }

        return this == item;
    }

    public override int GetHashCode()
    {
        return GetHashCode();
    }

    public Coord Add(int x_, int y_)
    {
        x = x + x_;
        y = y + y_;

        return new Coord(x, y);
    }
}