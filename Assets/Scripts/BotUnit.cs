using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public enum Statuses
    {
        Idle,
        Moving,
        FastAttack,
        StrongAttack,
        Impact,
        Die
    }

    public enum DyingStage
    {
        NotDying,
        Dying,
        Dead
    }

    [RequireComponent((typeof(Animator)))]
    public class BotUnit : BaseUnit
    {
        private Dictionary<Statuses, string> StatStrings = new Dictionary<Statuses, string>()
        {
            {Statuses.Idle,"Idle"},
            {Statuses.Moving,"Run"},
            {Statuses.FastAttack,"FastAttack"},
            {Statuses.StrongAttack,"StrongAttack1"},
            {Statuses.Impact,"Impact"},
            {Statuses.Die,"Die"}
        };

        private float _maxhealth = 100f;
        private float _health = 100f;
        private float _speed = 1f;
        private float _fastAttackDamage = 1f;
        private float _strongAttackDamage = 1f;
        private float _missChance = 0.05f;
        private float _critChance = 0.05f;
        private float _fastToStrongRatio = 0.5f;

        [SerializeField]
        bool CantAttack = false;
        [SerializeField]
        private Collider _collider;
        private Animator _animator;
        [SerializeField]
        private Slider _healthSlider;
        private BaseUnit _target;
        public BaseUnit Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }
        private Rigidbody _rigidBody;
        public Rigidbody ThisRigidBody
        {
            get
            {
                return _rigidBody;
            }
        }
        private Vector3 _wanderPosition;
        public Vector3 WanderPosition
        {
            get
            {
                return _wanderPosition;
            }
            set
            {
                _wanderPosition = value;
            }
        }

        internal void GoUltra()
        {
            _fastToStrongRatio = 0;
            _maxhealth = 10000;
            _health = 10000;
            _speed = 15;
            _fastAttackDamage = 100;
            gameObject.transform.localScale *= 1.5f;
        }

        private float radiusonPoint = 1.8f; //атака регистрируется с такого расстояниеб следовательно нужно подойти на это расстояние
        [SerializeField]
        private Material _materialForShield;
        public Material MaterialForShield
        {
            get
            {
                return _materialForShield;
            }
            set
            {
                _materialForShield = value;
            }
        }

        private Statuses _stat = Statuses.Idle;
        bool changingstate = false;
        private Statuses _currentStatus
        {
            get
            {
                return _stat;
            }
            set
            {
                //if (changingstate) return;
                changingstate = true;
                if (_stat != value)
                {
                    switch ((value))
                    {
                        case Statuses.FastAttack:
                        case Statuses.StrongAttack:
                            {
                                _inAttack = true;
                                break;
                            }
                        default:
                            {
                                _inAttack = false;
                                break;
                            }
                    }
                    ChangeAnimationState(value);
                    _stat = value;
                }
                //changingstate = false;
            }
        }
        private Canvas _healthcanvas;
        public DyingStage CurrentDyingStage;
        public void ChangeAnimationState(Statuses newStatus)
        {
            _animator.Play(StatStrings[newStatus]);
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidBody = GetComponent<Rigidbody>();
            var shield = GetComponentInChildren(typeof(ShieldStub));
            var shieldMesh = shield.gameObject.GetComponent<MeshRenderer>();
            if (_materialForShield != null)
            {
                shieldMesh.material = _materialForShield;
            }
            if (_healthSlider != null)
            {
                _healthcanvas = _healthSlider.GetComponentInParent<Canvas>();
            }
        }

        private bool ShowingHealth;
        internal void SetHealthBar(bool showingHealth)
        {
            ShowingHealth = showingHealth;
        }

        internal void SetParams(SpawnManagerScriptableObject toclone)
        {
            _maxhealth = toclone.Health;
            _health = toclone.Health;
            _speed = toclone.Speed;
            _fastAttackDamage = toclone.FastAttackDamage;
            _strongAttackDamage = toclone.StrongAttackDamage;
            _missChance = toclone.MissChance;
            _critChance = toclone.CritChance;
            _fastToStrongRatio = toclone.FastToStrongRatio;
        }

        public void SetHealth(int newhealth)
        {
            _health = newhealth;
        }
        public float GetHealth()
        {
            return _health;
        }

        private void Update()
        {
            if (_health <= 0 && CurrentDyingStage == DyingStage.NotDying)
            {
                CurrentDyingStage = DyingStage.Dying;
                _currentStatus = Statuses.Die;
            }
            if (_healthSlider != null)
            {
                _healthSlider.minValue = 0;
                _healthSlider.maxValue = _maxhealth;
                _healthSlider.value = _health;
                _healthSlider.transform.LookAt(Camera.main.transform.position);
                _healthcanvas.enabled = ShowingHealth;
                if (CurrentDyingStage != DyingStage.NotDying)
                {
                    _healthcanvas.enabled = false;
                }
            }
        }



        public void SetShieldMaterial(Material mat)
        {
            var shield = GetComponentInChildren(typeof(ShieldStub));
            var shieldMesh = shield.gameObject.GetComponent<MeshRenderer>();
            MaterialForShield = mat;
            shieldMesh.material = mat;
        }

        public event EventHandler OnEndAnimation;
        private void AnimationEventEnd_UnityEditor(string result)
        {
            OnEndAnimation?.Invoke(this, null);
            hits.Clear();
            if (CurrentDyingStage == DyingStage.NotDying)
            {
                _currentStatus = Statuses.Idle;
            }
            else
            {
                StartCoroutine(DeathDelay());
            }
        }

        IEnumerator DeathDelay()
        {
            var thisrigid = GetComponent<Rigidbody>();
            thisrigid.isKinematic = true;
            var thiscollider = GetComponent<CapsuleCollider>();
            thiscollider.enabled = false;
            yield return new WaitForSeconds(3);
            CurrentDyingStage = DyingStage.Dead;
        }

        private void AnimationEventCollider_UnityEditor(int isActivity)
        {
            _collider.enabled = isActivity != 0;
        }
        internal void MoveForward(float _force)
        {
            if (CurrentDyingStage != DyingStage.NotDying) return;
            _currentStatus = Statuses.Moving;
            var tomove = BotManager.IgnoreAxisUpdate(IgnoreAxisType.Y, transform.forward * _speed);
            tomove.y = ThisRigidBody.velocity.y;
            ThisRigidBody.velocity = tomove;
        }
        internal void LookAt(Vector3 tolook)
        {
            if (CurrentDyingStage != DyingStage.NotDying) return;
            tolook.y = gameObject.transform.position.y;
            transform.LookAt(tolook);
        }

        private bool _inAttack = false;
        internal void Attack()
        {
            //Костыль,не понимаю,почему застревает в idle
            //int idleHash = Animator.StringToHash("Idle");
            //AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            //if (animStateInfo.shortNameHash == idleHash)
            //{
            //    _currentStatus = Statuses.Idle;
            //}
            //Сделал другой костыль, пробрасываю из idle неуместное сообщение о завершении анимации

            if (CurrentDyingStage != DyingStage.NotDying) return;
            if (CantAttack) return;
            if (_inAttack) return;
            if (_currentStatus == Statuses.Impact) return;
            if (UnityEngine.Random.Range(0f, 1f) >= _fastToStrongRatio)
            {
                _currentStatus = Statuses.FastAttack;
            }
            else
            {
                _currentStatus = Statuses.StrongAttack;
            }
        }
        internal void Stop()
        {
            if (CurrentDyingStage != DyingStage.NotDying) return;
            if (_currentStatus != Statuses.Moving) return;
            _currentStatus = Statuses.Idle;
            ThisRigidBody.velocity = Vector3.zero;
        }

        private List<BotUnit> hits = new List<BotUnit>();
        public bool AddHit(BotUnit whatHit)
        {
            bool result = true;
            if (hits.FirstOrDefault(h => h == whatHit) != null)
            {
                result = false;
            }
            else
            {
                Debug.Log("Hit");
                float damage = 0f;
                switch (_currentStatus)
                {
                    case Statuses.FastAttack:
                        {
                            damage = _fastAttackDamage;
                            break;
                        }
                    case Statuses.StrongAttack:
                        {

                            damage = _strongAttackDamage;
                            break;
                        }
                    default:
                        break;
                }
                if (UnityEngine.Random.Range(0f, 1f) <= _critChance)
                {
                    damage *= 2;
                    Debug.Log("Crit!");
                }
                else if (UnityEngine.Random.Range(0f, 1f) <= _missChance)
                {
                    damage = 0;
                    Debug.Log("Miss!");
                }
                whatHit.ApplyDamage(damage);
                hits.Add(whatHit);
            }
            return result;
        }

        private void ApplyDamage(float damage)
        {
            _health -= damage;
        }


        //private HitManager _hitManager = new HitManager();

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<SwordStub>() != null) return;
            var whatHit = other.GetComponentInParent<BotUnit>();
            if (whatHit != null)
            {
                AddHit(whatHit);
            }
        }


        internal bool OnPoint(Vector3 pos)
        {
            bool result = false;
            pos.y = transform.position.y;
            Vector3 offset = pos - transform.position;
            float sqrLen = offset.sqrMagnitude;
            if (sqrLen < radiusonPoint * radiusonPoint)
            {
                result = true;
            }
            return result;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var pos = WanderPosition;
            pos.y = transform.position.y;
            Gizmos.DrawSphere(pos, radiusonPoint);
        }
    }
}