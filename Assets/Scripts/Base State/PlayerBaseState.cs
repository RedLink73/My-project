using UnityEngine;

public abstract class PlayerBaseState {
   public abstract void Enterstate(Rigidbody2D rigidbody);

   public abstract void UpdateState();
   public abstract void OnExit();
}


