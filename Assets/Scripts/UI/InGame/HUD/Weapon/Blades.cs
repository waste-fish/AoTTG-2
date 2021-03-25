using Assets.Scripts.Characters.Humans;
using UnityEngine;

namespace Assets.Scripts.UI.InGame.Weapon
{
    public class Blades : UiElement
    {
        [SerializeField] private GameObject BladeLeft;
        [SerializeField] private GameObject BladeRight;

        [SerializeField] private GameObject BladeLeftSpritePrefab;
        [SerializeField] private GameObject BladeRightSpritePrefab;

        private int distance = 18;
        private int previousBlades;

        private Hero hero;

        public void SetBlades(int blades)
        {
            if (blades == previousBlades) return;
            previousBlades = blades;

            foreach (Transform child in BladeLeft.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in BladeRight.transform)
            {
                Destroy(child.gameObject);
            }

            var previousLeftBlade = BladeLeft;
            var previousRightBlade = BladeRight;
            for (var i = 0; i < blades; i++)
            {
                var cordsLeft = previousLeftBlade.transform.position;
                cordsLeft.x -= distance;
                previousLeftBlade = Instantiate(BladeLeftSpritePrefab, cordsLeft, previousLeftBlade.transform.rotation, BladeLeft.transform);
                previousLeftBlade.transform.position = cordsLeft;

                var cordsRight = previousRightBlade.transform.position;
                cordsRight.x += distance;
                previousRightBlade = Instantiate(BladeRightSpritePrefab, cordsRight, previousRightBlade.transform.rotation, BladeRight.transform);
                previousRightBlade.transform.position = cordsRight;
            }
        }
    }
}
