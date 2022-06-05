using UnityEngine;

namespace Ziggurat
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class SpawnManagerScriptableObject : ScriptableObject
    {
        [Range(0.1f, 10f)]
        public float SpawnRate=1f;
        [Range(0.1f, 100f)]
        public float Health = 10f;
        [Range(1f, 15f)]
        public float Speed = 2f;
        [Range(0.1f, 100f)]
        public float FastAttackDamage = 1f;
        [Range(0.1f, 100f)]
        public float StrongAttackDamage = 1f;
        [Range(0f, 1f)]
        public float MissChance = 0.05f;
        [Range(0f, 1f)]
        public float CritChance = 0.05f;
        [Range(0f, 1f)]
        public float FastToStrongRatio = 0.5f;

        public SpawnManagerScriptableObject(float spawnRate, float health, float speed, float fastAttackDamage, float strongAttackDamage, float missChance, float critChance, float fastToStrongRatio)
        {
            SpawnRate = spawnRate;
            Health = health;
            Speed = speed;
            FastAttackDamage = fastAttackDamage;
            StrongAttackDamage = strongAttackDamage;
            MissChance = missChance;
            CritChance = critChance;
            FastToStrongRatio = fastToStrongRatio;
        }
        public SpawnManagerScriptableObject(SpawnManagerScriptableObject toclone)
        {
            SpawnRate = toclone.SpawnRate;
            Health = toclone.Health;
            Speed = toclone.Speed;
            FastAttackDamage = toclone.FastAttackDamage;
            StrongAttackDamage = toclone.StrongAttackDamage;
            MissChance = toclone.MissChance;
            CritChance = toclone.CritChance;
            FastToStrongRatio = toclone.FastToStrongRatio;
        }
    } 
}