using UnityEngine;
using System;

public class Head : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static event Action OnFoodEaten; // 食べ物を食べたときのイベント
    public static event Action OnNeedleHit; // 針に当たったときのイベント
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Food"))
        {
            OnFoodEaten?.Invoke();
            Destroy(other.gameObject);
        }
        if(other.gameObject.CompareTag("Needle"))
        {
            OnNeedleHit?.Invoke();
            Debug.Log("Game Over");
            
        }
    }
}
