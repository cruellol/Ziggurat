using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class ShowZigguratProps : MonoBehaviour
    {
        [SerializeField]
        private Slider _spawnRate;
        [SerializeField]
        private Slider _health;
        [SerializeField]
        private Slider _speed;
        [SerializeField]
        private Slider _fastToStrongRatio;
        [SerializeField]
        private Slider _fastDamage;
        [SerializeField]
        private Slider _strongDamage;
        [SerializeField]
        private Slider _missChance;
        [SerializeField]
        private Slider _critChance;

        private ZigguratParameter _currentZiggurat;
        private bool _onUpdate=false;
        public void UpdateUI(ZigguratParameter zigparam)
        {
            _onUpdate = true;
            _currentZiggurat = zigparam;
            _spawnRate.value = _currentZiggurat._spawnManagerScriptableObject.SpawnRate;
            _health.value = _currentZiggurat._spawnManagerScriptableObject.Health;
            _speed.value = _currentZiggurat._spawnManagerScriptableObject.Speed;
            _fastToStrongRatio.value = _currentZiggurat._spawnManagerScriptableObject.FastToStrongRatio;
            _fastDamage.value = _currentZiggurat._spawnManagerScriptableObject.FastAttackDamage;
            _strongDamage.value = _currentZiggurat._spawnManagerScriptableObject.StrongAttackDamage;
            _missChance.value = _currentZiggurat._spawnManagerScriptableObject.MissChance;
            _critChance.value = _currentZiggurat._spawnManagerScriptableObject.CritChance;
            _onUpdate = false;
        }

        public void OnEdit()
        {
            if (_onUpdate) return;
            _currentZiggurat._spawnManagerScriptableObject.SpawnRate = _spawnRate.value;
            _currentZiggurat._spawnManagerScriptableObject.Health = _health.value;
            _currentZiggurat._spawnManagerScriptableObject.Speed = _speed.value;
            _currentZiggurat._spawnManagerScriptableObject.FastToStrongRatio = _fastToStrongRatio.value;
            _currentZiggurat._spawnManagerScriptableObject.FastAttackDamage = _fastDamage.value;
            _currentZiggurat._spawnManagerScriptableObject.StrongAttackDamage = _strongDamage.value;
            _currentZiggurat._spawnManagerScriptableObject.MissChance = _missChance.value;
            _currentZiggurat._spawnManagerScriptableObject.CritChance = _critChance.value;
        }
    } 
}
