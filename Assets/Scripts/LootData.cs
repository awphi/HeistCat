using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "LootData", menuName = "ScriptableObjects/LootData", order = 1)]
public class LootData : ScriptableObject
{
    public int minRespawnTime = 60;
    public int maxRespawnTime = 90;
    
    public LootWeightEntry[] weights;

    [Serializable]
    public class LootWeightEntry
    {
        public int value;
        public int weight;
    }

    public int RollRandomLoot()
    {
        var sum = weights.Sum(c => c.weight);
        var x = Random.Range(0, sum);

        
        return (from c in weights where (x -= c.weight) < 0 select c.value).FirstOrDefault();
    }
}
