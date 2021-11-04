using UnityEngine;

public class DoorScript : MonoBehaviour
{
 

    private bool opened = false;
    private Animator anim;



    void Update()
    {
        //This will tell if the player press F on the Keyboard. P.S. You can change the key if you want.
        if (Input.GetKeyDown(KeyCode.F))
        {
            Pressed();
            //Delete if you dont want Text in the Console saying that You Press F.
            Debug.Log("You Press F");
        }
    }

    void Pressed()
    {
     
        //This line will get the Animator from  Parent of the door that was hit by the raycast.
        anim = transform.GetComponentInParent<Animator>();

        //This will set the bool the opposite of what it is.
        opened = !opened;

        //This line will set the bool true so it will play the animation.
        anim.SetBool("Opened", !opened);
            

    }
}
