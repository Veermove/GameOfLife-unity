using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

    public int targetFramesPerSecond;
    public int animationSpeed = 2;
    public int animationProgress = 5;

    // Map objects
    GameObject mapObject;
    SpriteRenderer mapRenderer;

    Dictionary<int, bool[,]> glidersReference;
    public int distanceFromGliders = 15;


    // Dimensions
    public int sizeX = 100; // X size of plane
    public int sizeY = 100; // Y size of plane

    // Colors of map
    public Color DeadColor;

    public Color AliveColor;

    // Matrix of dead/alive cells
    public bool[,] mainMatrix;
    public bool[,] background;

    bool animationStarted = false;
    int numberOfGliders = 5;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFramesPerSecond;
        mainMatrix = new bool[sizeX, sizeY];
        background = new bool[sizeX, sizeY];
        feelGlidersDictionary();
        for (int xi = 0; xi < sizeX; xi++)
        {
            for (int yi = 0; yi < sizeY; yi++)
            {
                background[xi, yi] = false;
            }
        }
        randomGlidersFill(numberOfGliders);
        string text = System.IO.File.ReadAllText(@"./PlayExitSetting.txt");
        char[] textChar = text.ToCharArray();
        int x = 0;
        int y = 0;
        int i = 0;
        while(true)
        {
            if (textChar[i] == 'E')
            {
                break;
            }
            if (textChar[i] == 'z')
            {
                mainMatrix[x % sizeX, y % sizeY] = false;
            }
            else
            {
                mainMatrix[x % sizeX, y % sizeY] = true;
            }
            i++;
            y++;
            if(y % sizeX == 0)
            {
                x++;
            }
        }
        createPlane();
    }

    //update onscreen plane
    void updatePlane(bool[,] matrix)
    {
        Texture2D txt = new Texture2D(sizeX, sizeY);
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if(!matrix[x, y])
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
        GM.Rules temp = new GM.Rules();
        background = temp.updateMatrix(background, sizeX, sizeY);
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.x += sizeX/2;
            mouse.y += sizeY/2;
            if(mouse.x > 13 && mouse.x < 54 && mouse.y < 80 && mouse.y > 66)
            {
                animationStarted = true;
                mainMatrix = mergeMatrices(mainMatrix, background);
                StartCoroutine(RulesOfLife());
            }
            else if (mouse.x > 13 && mouse.x < 54 && mouse.y < 64 && mouse.y > 51)
            {
                Debug.Log("exit");
                Application.Quit();
            }
        }
        if (!animationStarted)
        {
            updatePlane(mergeMatrices(mainMatrix, background));
        }
        else
        {
            updatePlane(mainMatrix);
        }

    }

    IEnumerator RulesOfLife()
    {
        int frames = 0;
        int animationY = sizeY - 1 - animationProgress;
        GM.Rules temp = new GM.Rules();
        while (true)
        {
            mainMatrix = temp.updateMatrix(mainMatrix, sizeX, sizeY);
            frames++;
            if (frames % animationSpeed == 0)
            {

                if (animationY <= 0)
                {
                    break;
                }
                for (int y = sizeY - 1; y > animationY; y--)
                {
                    for(int x = 0; x < sizeX; x++)
                    {
                        mainMatrix[x, y] = false;
                    }
                }
                animationY -= animationProgress;

            }
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void randomGlidersFill(int gliders)
    {
        List<Vector2> posCheck = new List<Vector2>();
        System.Random rand = new System.Random();
        Vector2 prev = Vector2.zero;
        for (int i = 0; i < gliders; i++)
        {
            while(true)
            {
                int x = rand.Next(0, 100);
                int y = rand.Next(0, 100);
                Vector2 temp = new Vector2(x, y);
                if (!posCheck.Contains(temp) && Vector2.Distance(temp, prev) > distanceFromGliders)
                {
                    posCheck.Add(temp);
                    drawGlider(x, y, rand.Next(0, 4));
                    prev = temp;
                    break;
                }
            }
        }
    }

    void drawGlider(int x, int y, int gliderNum)
    {
        bool[,] tempGlider;
        Debug.Log(glidersReference.TryGetValue(gliderNum, out tempGlider));
        // glidersReference.TryGetValue(gliderNum, out tempGlider);
        int gliderX = 0;
        int gliderY = 0;
        for (int xDev = -1; xDev < 2; xDev++)
        {
            for (int yDev = -1; yDev < 2; yDev++)
            {
                background[(x + xDev + sizeX) % sizeX, (y + yDev + sizeY) % sizeY] = tempGlider[gliderX, gliderY];
                gliderY++;
            }
            gliderX++;
            gliderY = 0;
        }
    }

    void feelGlidersDictionary()
    {
        glidersReference = new Dictionary<int, bool[,]>();
        bool[,] temp1 = {{false, true, false}, {false, false, true}, {true, true, true}};
        glidersReference.Add(0, temp1);
        bool[,] temp2 = {{false, true, false}, {true, false, false}, {true, true, true}};
        glidersReference.Add(1, temp2);
        bool[,] temp3 = {{true, true, true}, {false, false, true}, {false, true, false}};
        glidersReference.Add(2, temp3);
        bool[,] temp4 = {{true, true, true}, {true, false, false}, {false, true, false}};
        glidersReference.Add(3, temp4);
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

    bool[,] mergeMatrices(bool[,] temp1, bool[,] temp2)
    {
        bool[,] retVal = new bool[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                retVal[x, y] = temp1[x,y] || temp2[x,y];
            }
        }
        return retVal;
    }
}
