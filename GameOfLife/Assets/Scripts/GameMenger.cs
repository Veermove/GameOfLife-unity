using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GM
{
    public class GameMenger : MonoBehaviour
    {
        public int targetFramesPerSecond;

        // Map objects
        GameObject mapObject;
        SpriteRenderer mapRenderer;


        // Dimensions
        public int sizeX = 100; // X size of plane
        public int sizeY = 100; // Y size of plane
        Rect rectangle;


        // Colors of map
        public Color DeadColor;

        public Color AliveColor;

        // Matrix of dead/alive cells
        public bool[,] mainMatrix;

        // state of simulation: Paused or Unpaused
        bool wasPaused = false;


        // Start is called before the first frame update
        void Start()
        {
            rectangle = new Rect(0, 0, sizeX, sizeY);
            Application.targetFrameRate = targetFramesPerSecond;
            createMatrix();
            createPlane();
        }

        // Initializes randomized mainMatirx
        public void createMatrix()
        {
            var Rand = new System.Random();
            mainMatrix = new bool[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(Rand.Next(101) >= 50)
                    {
                        mainMatrix[x, y] = false;
                    }
                    else
                    {
                         mainMatrix[x, y] = true;
                    }


                }
            }
        }

        // Initializes all-false mainMatirx
        public void clearMatrix()
        {
            mainMatrix = new bool[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    mainMatrix[x, y] = false;
                }
            }
        }

        // Initializes gameObjects, shows onscreen first frame
        public void createPlane()
        {
            mapObject = new GameObject("Map");
            mapRenderer = mapObject.AddComponent<SpriteRenderer>();
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        // Updates onscreen plane, calculates next generation of Dead/Alive cells
        // if changePixel = true -> paintPixel(Vector3 mousePos) will be called
        void UpdatePlane(bool changePixel, Vector3 mousePos)
        {
            //calculte next generatrion
            Rules temp = new Rules();
            mainMatrix = temp.updateMatrix(mainMatrix, sizeX, sizeY);

            //paint given pixel
            if (changePixel)
            {
                paintPixel(mousePos);
            }

            //update onscreen plane
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(!mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }

                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }


        // Updates onscreen plane WHILE PathPainting cells, does NOT calculate next generation of Dead/Alie cells
        void UpdatePlane()
        {
            //update onscreen plane
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(!mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }

                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        // Updates onscreen plane WHILE IN-GAME PAUSED, does NOT calculate next generation of Dead/Alie cells
        // if changePixel = true -> paintPixel(Vector3 mousePos) will be called
        void UpdatePausedPlane(bool changePixel, Vector3 mousePos)
        {
            if (changePixel)
            {
                paintPixel(mousePos);
            }
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(!mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }

                }
            }
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        // Update is called once per frame
        void Update()
        {
            bool pixelPiant = false;
            Vector3 mouse = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.R))
            {
                createMatrix();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if(wasPaused)
                {
                    wasPaused = false;
                }
                else
                {
                    wasPaused = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                clearMatrix();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (mouse.x < sizeX/2 && mouse.x > -sizeX/2
                    && mouse.y < sizeY/2 && mouse.y > -sizeY/2)
                {
                    pixelPiant = true;
                }
            } else if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(BrushPixel());
            }
            if(!wasPaused)
            {
                UpdatePlane(pixelPiant, mouse);
            }
            else
            {
                UpdatePausedPlane(pixelPiant, mouse);
            }
        }

        // Coroutine that allows to brushPaint cells across multiple frames
        IEnumerator BrushPixel()
        {
            Vector2 previous = Vector2.zero;
            bool firstIteratrion = true;
            // loop that does one round every frame
            while (true)
            {
                // translating from Vector3 mousePositon to Vector2 MainMatrix addres
                Vector3 transitional = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 currentPos = new Vector2(transitional.x, transitional.y);
                currentPos.x += sizeX/2;
                currentPos.y += sizeY/2;

                // if current mouse is inside "plane" set pixel poited to by mouse to alive
                if (currentPos.x >= 0 && currentPos.x < sizeX && currentPos.y >= 0 && currentPos.y < sizeY)
                {
                    if (firstIteratrion)
                    {
                        firstIteratrion = false;
                        previous = currentPos;
                    }

                    mainMatrix[(int)Math.Floor(currentPos.x), (int)Math.Floor(currentPos.y)] = true;

                    // for all pixels between current mousePosition and previous mousePosition interpolate points, and set them to Alive
                    List<Vector2> pointsAlong = AproximatePoints(previous, currentPos);
                    foreach(Vector2 temp in pointsAlong)
                    {
                        if(temp.x >= 0 && temp.x < sizeX &&  temp.y >= 0 &&  temp.y < sizeY)
                        {
                            mainMatrix[(int)Math.Floor(temp.x), (int)Math.Floor(temp.y)] = true;
                        }
                    }

                    // Update onscreen plane
                    UpdatePlane();
                    previous = currentPos;
                }
                // if MouseKey is released break out of the loop and thus end Coroutine
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    break;
                }
                yield return null;
            }
        }

        // Aproximates points given two points for from subsequent frames using interpolation
        public List<Vector2> AproximatePoints(Vector2 current, Vector2 previous)
        {
            int resolution = 100;
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < resolution; i++)
            {
                points.Add(Vector2.Lerp(previous, current, (float)i / (float)resolution));
            }
            return points;
        }

        // sets cell pointed by Vector3 to Alive in mainMatrix
        // Vector3 mouse should be given with offset +size/2
        public void paintPixel(Vector3 mouse)
        {
            int xPos = (int)(mouse.x + sizeX/2);
            int yPos = (int)(mouse.y + sizeY/2);
            if (xPos >= 0 && xPos < sizeX && yPos >= 0 && yPos < sizeY)
            {
                if (mainMatrix[xPos, yPos])
                {
                    mainMatrix[xPos, yPos] = false;
                }
                else
                {
                    mainMatrix[xPos, yPos] = true;
                }

            }
        }


        // Draws "car" onscreen
        public void DrawCar(Vector2 car, Color CarColor)
        {
            //update onscreen plane
            Texture2D txt = new Texture2D(sizeX, sizeY);
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if(!mainMatrix[x, y])
                    {
                        txt.SetPixel(x, y, DeadColor);
                    }
                    else
                    {
                        txt.SetPixel(x, y, AliveColor);
                    }
                }
            }
            txt.SetPixel((int)car.x, (int)car.y, CarColor);
            txt.filterMode = FilterMode.Point;
            txt.Apply();
            Rect rect = new Rect(0, 0, sizeX, sizeY);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }

        // Returns mainMatrix
        public bool[,] getMatrix()
        {
            return mainMatrix;
        }
    }

}
