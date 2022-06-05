using System;
using UnityEngine;

namespace Ziggurat
{
	[RequireComponent(typeof(Animator))]
	public class UnitEnvironment : MonoBehaviour
	{
		[SerializeField]
		private Animator _animator;
		[SerializeField]
		private Collider _collider;


		private string _currentState;

		public void ChangeAnimationState(string newState)
        {
			if (_currentState == newState) return;
			_animator.Play(newState);
			_currentState = newState;
        }


		/// <summary>
		/// Событие, вызываемое по окончанию анимации
		/// </summary>
		public event EventHandler OnEndAnimation;

		/// <summary>
		/// Этот метод нужно вызвать/подписать на передвижение в Unit, чтобы подключить анимацию стояния или хотьбы
		/// </summary>
		/// <remarks>Если передается 0f - персонаж в Idle анимации, если >0f - персонаж ходит</remarks>
		public void Moving(float direction)
		{
			ChangeAnimationState("Run");
			return;
			_animator.SetFloat("Movement", direction);
		}

		/// <summary>
		/// Вызывать для всех прочих, кроме хотьбы анимаций
		/// </summary>
		/// <param name="key"></param>
		public void StartAnimation(string key)
        {
			ChangeAnimationState(key);
			return;
			StopMovement();
			_animator.SetTrigger(key);
        }

		public void StopMovement()
        {
			ChangeAnimationState("Idle");
			return;
			_animator.SetFloat("Movement", 0f);
        }

        //Вызывается внутри анимаций для переключения атакующего коллайдера
        private void AnimationEventCollider_UnityEditor(int isActivity)
		{
			_collider.enabled = isActivity != 0;
		}

		//Вызывается внутри анимаций для оповещения об окончании анимации
		private void AnimationEventEnd_UnityEditor(string result)
		{
			//В конце анимации смерти особый аргумент и своя логика обработки
			if (result == "die") Destroy(gameObject);
			OnEndAnimation?.Invoke(this, null);
		}
	}
}
