using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPool", menuName = "Backpack/ItemPool")]
public class ItemPool : ScriptableObject
{
    [SerializeField] private List<ItemData> candidates = new ();

    public List<ItemData> GetRandomChoices(int count)
    {
        List<ItemData> candidatesCopy = new List<ItemData>(candidates);
        List<ItemData> resultList = new ();  

        for(int i = 0; i < count && candidatesCopy.Count > 0; i++) 
        {
            int index = Random.Range(0, candidatesCopy.Count);
            resultList.Add(candidatesCopy[index]);
            candidatesCopy.RemoveAt(index);
        }

        return resultList;
    }
}
