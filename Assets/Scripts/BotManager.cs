using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class BotManager : MonoBehaviour
    {
        [SerializeField]
        public List<BotUnit> _units = new List<BotUnit>();
        [SerializeField]
        private BaseUnit _wanderPoint;
        [SerializeField]
        private float _searchTargetRadius;
        [SerializeField]
        private float _force;
        [SerializeField]
        private float _wanderRadius;

        [SerializeField]
        private Text _blueAliveText;
        [SerializeField]
        private Text _blueDeadText;
        [SerializeField]
        private Text _redAliveText;
        [SerializeField]
        private Text _redDeadText;
        [SerializeField]
        private Text _greenAliveText;
        [SerializeField]
        private Text _greenDeadText;

        [SerializeField]
        private Material _blueShieldMaterial;
        [SerializeField]
        private Material _redShieldMaterial;
        [SerializeField]
        private Material _greenShieldMaterial;

        private int _blueDeadCount = 0;
        private int _redDeadCount = 0;
        private int _greenDeadCount = 0;
        private int _blueCount = 0;
        private int _redCount = 0;
        private int _greenCount = 0;

        private void Start()
        {
            CurrentBotManager = this;
            foreach (var unit in _units)
            {
                unit.WanderPosition = GetnextWanderPoint();
            }
        }

        internal void AddUnit(BotUnit newunit)
        {
            newunit.WanderPosition = GetnextWanderPoint();
            newunit.SetHealthBar(ShowingHealth);
            _units.Add(newunit);
        }

        public static BotManager CurrentBotManager;


        public void Update()
        {            
            CheckKills();

            _blueCount = 0;
            _redCount = 0;
            _greenCount = 0;
            foreach (var unit in _units)
            {
                SelectNextTarget(unit);

                if (unit.Target == null)
                {
                    if (unit.OnPoint(unit.WanderPosition))
                    {
                        unit.WanderPosition = GetnextWanderPoint();
                    }
                    else
                    {
                        unit.LookAt(unit.WanderPosition);
                        unit.MoveForward(_force);
                    }
                }
                else
                {
                    unit.LookAt(unit.Target.transform.position);
                    if (unit.OnPoint(unit.Target.transform.position))
                    {
                        unit.Stop();
                        unit.Attack();
                    }
                    else
                    {
                        unit.MoveForward(_force);
                    }
                }


                if (unit.MaterialForShield == _blueShieldMaterial)
                {
                    _blueCount++;
                }
                if (unit.MaterialForShield == _redShieldMaterial)
                {
                    _redCount++;
                }
                if (unit.MaterialForShield == _greenShieldMaterial)
                {
                    _greenCount++;
                }
            }

            UpdateStatisticks();
        }

        public void KillAll()
        {
            foreach (var unit in _units)
            {
                unit.SetHealth(0);
            }
        }

        private bool ShowingHealth = false;
        public void ToggleHealthBar()
        {
            ShowingHealth = !ShowingHealth;
            foreach (var unit in _units)
            {
                unit.SetHealthBar(ShowingHealth);
            }
        }

        public void ClearDeadStatistick()
        {
            _blueDeadCount = 0;
            _redDeadCount = 0;
            _greenDeadCount = 0;
            UpdateStatisticks();
        }

        private void UpdateStatisticks()
        {
            _blueAliveText.text = _blueCount.ToString();
            _blueDeadText.text = _blueDeadCount.ToString();
            _redAliveText.text = _redCount.ToString();
            _redDeadText.text = _redDeadCount.ToString();
            _greenAliveText.text = _greenCount.ToString();
            _greenDeadText.text = _greenDeadCount.ToString();
        }

        private void CheckKills()
        {
            List<BotUnit> tokill = new List<BotUnit>();
            List<BotUnit> toignore = new List<BotUnit>();
            foreach (var unit in _units)
            {
                //if (unit.GetHealth() <= 0)
                if (unit.CurrentDyingStage == DyingStage.Dead)
                {
                    tokill.Add(unit);
                }
                if (unit.CurrentDyingStage != DyingStage.NotDying)
                {
                    toignore.Add(unit);
                }
            }
            foreach (var unit in _units)
            {
                foreach (var ignore in toignore)
                {
                    if (unit.Target == ignore)
                    {
                        unit.Target = null;
                    }
                }
            }
            foreach (var killed in tokill)
            {
                if (killed.MaterialForShield == _blueShieldMaterial)
                {
                    _blueDeadCount++;
                }
                if (killed.MaterialForShield == _redShieldMaterial)
                {
                    _redDeadCount++;
                }
                if (killed.MaterialForShield == _greenShieldMaterial)
                {
                    _greenDeadCount++;
                }
                _units.Remove(killed);
                Destroy(killed.gameObject);
            }
        }

        private void SelectNextTarget(BotUnit unit)
        {
            BotUnit chosenenemy = null;
            float minDist = float.MaxValue;
            foreach (var enemy in _units)
            {
                if (enemy.CurrentDyingStage == DyingStage.NotDying && enemy != unit && enemy.MaterialForShield != unit.MaterialForShield)
                {
                    var sqrdist = (enemy.gameObject.transform.position - unit.gameObject.transform.position).sqrMagnitude;
                    if (minDist > sqrdist)
                    {
                        minDist = sqrdist;
                        chosenenemy = enemy;
                    }
                }
            }
            if (chosenenemy != null)
            {
                if (minDist < _searchTargetRadius * _searchTargetRadius)
                {
                    unit.Target = chosenenemy;
                }
            }
            else
            {
                unit.Target = null;
            }
        }

        private Vector3 GetnextWanderPoint()
        {
            return IgnoreAxisUpdate(IgnoreAxisType.Y, _wanderPoint.transform.position + Random.insideUnitSphere * _wanderRadius);
        }

        public static Vector3 IgnoreAxisUpdate(IgnoreAxisType ignore, Vector3 toignore)
        {
            if (ignore == IgnoreAxisType.None) return toignore;
            if ((ignore & IgnoreAxisType.X) == IgnoreAxisType.X) toignore.x = 0f;
            if ((ignore & IgnoreAxisType.Y) == IgnoreAxisType.Y) toignore.y = 0f;
            if ((ignore & IgnoreAxisType.Z) == IgnoreAxisType.Z) toignore.z = 0f;
            return toignore;
        }
        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawSphere(_wanderPoint.transform.position, _wanderRadius);
        //}
    }
}
