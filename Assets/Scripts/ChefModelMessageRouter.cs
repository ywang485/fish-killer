using UnityEngine;

public class ChefModelMessageRouter : MonoBehaviour {
    public ChefController chefController;
    // called by animation event
    public void OnKnifeDown () {
        chefController.OnKnifeDown();
    }
}
