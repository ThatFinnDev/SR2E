using SR2E;
using System.Collections;
using UnityEngine;
using MelonLoader;

namespace VirtualSlime
{
    [RegisterTypeInIl2Cpp]
    public class RandomExecuteBehaviour : MonoBehaviour
    {
        public bool IsInsideRange(int number, int rangeMin, int rangeMax) => (number >= rangeMin && number <= rangeMax);


        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == player)
                Random();
        }

        public void Random()
        {
            var r = UnityEngine.Random.Range(0, 1001);

            if (IsInsideRange(r, 0, 5))
            {
                SendToMainMenu();
            }
            else if (IsInsideRange(r, 6, 60))
            {
                SR2Console.ExecuteByString("killall PinkSlime", true);
            }
            else if (IsInsideRange(r, 61, 100))
            {
                GetComponent<Rigidbody>().velocity = Vector3.up * 30f;
            }

        }
        public void SendToMainMenu()
        {
            sceneContext.PauseMenuDirector.Quit();
        }
    }
}