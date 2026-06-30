using UnityEngine;

namespace SA
{
    public class MainMenuButtons : MonoBehaviour
    {
        public void Press()
        {
            MySceneManager.singleton.PressStartGame();
        }
    }
}
