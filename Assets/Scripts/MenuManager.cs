
using UnityEngine;

public class MenuManager : MonoBehaviour

    //Missing main menu UI to active baseMovement gamestate
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        // GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
         
    }

    private void OnDestroy()
    {
      //  GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }

   /* private void GameManagerOnGameStateChanged(GameState state) //must implement main menu UI to continue
    {
        throw new System.NotImplementedException();
    }*/
}
