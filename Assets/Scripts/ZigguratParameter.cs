using System.Collections;
using UnityEngine;

namespace Ziggurat
{
    public class ZigguratParameter : MonoBehaviour
    {
        [SerializeField]
        public SpawnManagerScriptableObject _spawnManagerScriptableObject;
        [SerializeField]
        private BotUnit _unitPrefab;
        [SerializeField]
        private Material _zigguratMaterial;
        [SerializeField]
        private GameObject _spawnPoint;

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        IEnumerator Spawn()
        {
            while (true)
            {
                if (_unitPrefab != null && _zigguratMaterial != null && _spawnPoint != null)
                {
                    BotUnit newunit = GameObject.Instantiate(_unitPrefab);
                    newunit.SetParams(_spawnManagerScriptableObject);
                    newunit.SetShieldMaterial(_zigguratMaterial);
                    newunit.transform.position = _spawnPoint.transform.position;
                    BotManager.CurrentBotManager.AddUnit(newunit);
                }
                yield return new WaitForSeconds(_spawnManagerScriptableObject.SpawnRate);
            }
        }
    }
}
