//Chloe Walsh
//Tile Manager
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Vector3 targetPosition; 
    private Vector3 correctPosition; 
    private SpriteRenderer sprite;
    public int number;
    public bool inRightPlace; 

    void Awake() {
        targetPosition = transform.position;
        correctPosition = transform.position;  
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update() {
        transform.position = Vector3.Lerp(a: transform.position, b: targetPosition, t: 0.05f); 
    }
}
