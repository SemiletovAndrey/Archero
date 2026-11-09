namespace Persistance.Features.EntityCore
{
	public partial class Entity
	{
		public Persistance.Features.EntityCore.RigidbodyComponent RigidbodyC => GetComponent<Persistance.Features.EntityCore.RigidbodyComponent>();

		public UnityEngine.Rigidbody Rigidbody => RigidbodyC.Value;

		public Persistance.Features.EntityCore.Entity AddRigidbody(UnityEngine.Rigidbody value)
		{
			return AddComponent(new Persistance.Features.EntityCore.RigidbodyComponent() {Value = value});
		}

		public Gameplay.Features.MovementFeature.MoveDirection MoveDirectionC => GetComponent<Gameplay.Features.MovementFeature.MoveDirection>();

		public Contexts.Scripts.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> MoveDirection => MoveDirectionC.Value;

		public Persistance.Features.EntityCore.Entity AddMoveDirection()
		{
			return AddComponent(new Gameplay.Features.MovementFeature.MoveDirection() { Value = new Contexts.Scripts.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Persistance.Features.EntityCore.Entity AddMoveDirection(Contexts.Scripts.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Gameplay.Features.MovementFeature.MoveDirection() {Value = value});
		}

		public Gameplay.Features.MovementFeature.MoveSpeed MoveSpeedC => GetComponent<Gameplay.Features.MovementFeature.MoveSpeed>();

		public Contexts.Scripts.Utilities.Reactive.ReactiveVariable<System.Single> MoveSpeed => MoveSpeedC.Value;

		public Persistance.Features.EntityCore.Entity AddMoveSpeed()
		{
			return AddComponent(new Gameplay.Features.MovementFeature.MoveSpeed() { Value = new Contexts.Scripts.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Persistance.Features.EntityCore.Entity AddMoveSpeed(Contexts.Scripts.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Gameplay.Features.MovementFeature.MoveSpeed() {Value = value});
		}

	}
}
