using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] ItemPool pool ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(ItemData item in pool.GetRandomChoices(3))
        {
            Debug.Log(item.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
