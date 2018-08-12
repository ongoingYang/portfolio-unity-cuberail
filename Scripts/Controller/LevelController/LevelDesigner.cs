using UnityEngine;

public class LevelDesigner : MonoBehaviour {

    public static LevelDesigner Instance;
    private void MakeInstance()
    {
        if (Instance == null){
            Instance = this;
        }
    }
    private void Awake(){
        MakeInstance();
        if(GameDesignManager.Instance != null){
            Initialize(GameDesignManager.blockLevelSize);
        }
   
    }
    const int _axisIndex = 3; // default

    #region level Design
    [Header("Block Prefabs")]
    [SerializeField] private GameObject planeBlock;
    [SerializeField] private GameObject rotatorPrefab;
    [SerializeField] private GameObject holeBlockPrefab;
    [SerializeField] private GameObject vertexBlockPrefab;

    [Header("Axis Direction Transform")]
    public Transform xDirectionAxis;
    public Transform yDirectionAxis;
    public Transform zDirectionAxis;
    #endregion
    public HoleBlock[,,] holeArray;
    public PlaneBlock[,,] planeArray;
    public RotatorBlock[,] rotatorArray;
    public VertexBlock[] vertexArray;

    private void Initialize(int size)
    {
        int blockSize = size;

        holeArray = new HoleBlock[_axisIndex, 2, blockSize];
        planeArray = new PlaneBlock[_axisIndex, blockSize, blockSize];
        rotatorArray = new RotatorBlock[_axisIndex, blockSize];
        vertexArray = new VertexBlock[_axisIndex];

        SetHoleBlock(blockSize);
        SetPlaneBlock(blockSize);
        SetRotatorBlock(blockSize);
        SetVertexBlock(blockSize);

        xDirectionAxis.rotation = Quaternion.Euler(90.0f, 90.0f, 0.0f);
        yDirectionAxis.rotation = Quaternion.Euler(90.0f, 90.0f, 0.0f); // ? 이상한 버그 이렇게 해야 콜리더가 살아있음
        yDirectionAxis.rotation = Quaternion.identity;
        zDirectionAxis.rotation = Quaternion.Euler(0.0f, -90.0f, 90.0f);
    }

    private void SetHoleBlock(int blockSize) //axisIndex: 0 == x, 1 == y, 2 == z
    {
        for (int i = 0; i < _axisIndex; i++){ 
            int j = 0; int k = 0;

            while (j < blockSize) // negative
            {
                GameObject block = Instantiate(holeBlockPrefab) as GameObject;
                HoleBlock holeBlock = block.GetComponent<HoleBlock>();
                holeBlock.Initialize(i, j, blockSize);
                holeArray[i, 0, j] = holeBlock;
                j++;
            }
            while (k < blockSize) // positive
            {
                GameObject block = Instantiate(holeBlockPrefab) as GameObject;
                HoleBlock holeBlock = block.GetComponent<HoleBlock>();
                holeBlock.Initialize(i, blockSize, k);
                holeArray[i, 1, k] = holeBlock;
                k++;
            }
        }
    }
    private void SetPlaneBlock(int blockSize)
    { 
        for (int i = 0; i < _axisIndex; i++){ 
            for (int j = 0; j < blockSize; j++){
                for (int k = 0; k < blockSize; k++){
                    GameObject block = Instantiate(planeBlock) as GameObject;
                    PlaneBlock planeblock = block.GetComponent<PlaneBlock>();
                    planeblock.Initialize(i, j, k);
                    planeArray[i, j, k] = planeblock;
                }
            }
        }
    }
    private void SetRotatorBlock(int blockSize)
    {
        for (int i = 0; i < _axisIndex; i++){
            for (int j = 0; j < blockSize; j++){
                GameObject block = Instantiate(rotatorPrefab) as GameObject;
                RotatorBlock rotatorBlock = block.GetComponent<RotatorBlock>();
                rotatorBlock.Initialize(i, j, 0);
                rotatorArray[i, j] = rotatorBlock;
            }
        }
    }
    private void SetVertexBlock(int blockSize)
    {
        for (int i = 0; i < _axisIndex; i++){
            GameObject block = Instantiate(vertexBlockPrefab) as GameObject;
            VertexBlock vertexBlock = block.GetComponent<VertexBlock>();
            vertexBlock.Initialize(i, blockSize, blockSize); 
            vertexArray[i] = vertexBlock;
        }
    }
}
