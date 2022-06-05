using UnityEngine;

namespace Ziggurat
{
    [RequireComponent(typeof(UnitEnvironment))]
    public class UnitMoveToTest : MonoBehaviour
    {
        private UnitEnvironment _envi;
        private void Start()
        {
            _envi=GetComponent<UnitEnvironment>();
        }
        private void Update()
        {
            //_envi.Moving(10);
            //_envi.StartAnimation("Die");
            //_envi.StartAnimation("Fast");
            //_envi.StartAnimation("Strong");
            //_envi.StartAnimation("Impact");
        }
    } 
}
