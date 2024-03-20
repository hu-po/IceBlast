using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public int teamIndex; // Assign 1 for Red team, 2 for Blue team in the Inspector
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // Increment score and update UI
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.GoalScored(teamIndex);
            }
        }
    }
}