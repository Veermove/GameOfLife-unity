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


        // Colors of map
        public Color DeadColor;

        public Color AliveColor;

        public Color tempCol;

        // Matrix of dead/alive cells
        public bool[,] mainMatrix;

        bool wasPaused = false;


        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = targetFramesPerSecond;
            createMatrix();
            createPlane();
        }

        // Initializes mainMatirx, fills with false
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

        void UpdatePlane()
        {
            Rules temp = new Rules();
            mainMatrix = temp.updateMatrix(mainMatrix, sizeX, sizeY);
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
            if (Input.GetKeyDown(KeyCode.R))
            {
                createMatrix();
            } else if (Input.GetKeyDown(KeyCode.Space))
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
            if(!wasPaused)
            {
                UpdatePlane();
            }
        }
    }

}
