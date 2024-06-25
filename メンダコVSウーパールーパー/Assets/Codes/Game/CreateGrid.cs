using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    public GameObject gridObject;
    
    private const float FIRST_X = -6.48f; //ウパルパ陣営側
    private const float FIRST_Z = -6.48f;
    private const float Y_POS = 0.04f;
    private const int GRID_NUM = 6;
    private float gridSize = Mathf.Abs(FIRST_X*2/5);
    // Start is called before the first frame update
    void Start()
    {
        CreateGrids();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGrids()
    {
        for(int i=0; i<GRID_NUM; ++i){
            for(int j=0; j<GRID_NUM; ++j){
                Vector3 position = new Vector3(FIRST_X+j*gridSize, Y_POS, FIRST_Z+i*gridSize);
                GameObject newGridObj = Instantiate(gridObject, position, Quaternion.identity);
                BoardGrid gridComponent = newGridObj.GetComponent<BoardGrid>();
                gridComponent.SetPosID(new Vector2(j,i));
                if(i==0 && (j==0 || j==5)) {
                    gridComponent.SetType((int)BoardGrid.types.corner_mendako);
                }else if(i==5 && (j==0 || j==5)) {
                    gridComponent.SetType((int)BoardGrid.types.corner_uparupa);
                }else if(i<3) {
                    gridComponent.SetType((int)BoardGrid.types.mendako);
                }else{
                    gridComponent.SetType((int)BoardGrid.types.uparupa);
                }
            }
        }
    }
}
