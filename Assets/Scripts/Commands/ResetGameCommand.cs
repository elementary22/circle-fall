using UnityEngine.SceneManagement;

public class ResetGameCommand : ICommand
{
    public void Execute()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(Config.GameScene);
    }

}

public class ResetGameCommandSignal : ISignal
{
    
}