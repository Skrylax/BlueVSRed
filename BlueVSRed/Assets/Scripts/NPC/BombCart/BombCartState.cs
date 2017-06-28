using UnityEngine;
using System.Collections;

public abstract class BombCartState : MonoBehaviour {

    public BombCart bCart;

    public abstract void Execute();
	
}
