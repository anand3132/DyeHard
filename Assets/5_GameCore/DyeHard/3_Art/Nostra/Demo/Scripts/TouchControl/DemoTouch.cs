using nostra.input;
using UnityEngine;
using UnityEngine.UI;

namespace nostra.demo
{
    public class DemoTouch : MonoBehaviour
    {
        [SerializeField] Text debugTxt;

        string msg;
        private void Update ()
        {
            msg = string.Empty;
            if ( NostraInput.GetAction ( "button", EActionEvent.Click ) )
            {
                msg = "Button Clicked";
            }

            Vector2 move = NostraInput.GetAxis ( "joystick" );
            msg += "  Moved X: " + move.x + " : Y:" + move.y;
            if ( debugTxt != null ) debugTxt.text = msg;

            Vector2 look = NostraInput.GetAxis ( "touchpad" );
            msg += "  Looked X: " + look.x + " : Y:" + look.y;
            if ( debugTxt != null ) debugTxt.text = msg;
        }
    }
}