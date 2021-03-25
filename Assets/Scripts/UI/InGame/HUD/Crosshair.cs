using Assets.Scripts.Characters.Humans;
using Assets.Scripts.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.UI.InGame.HUD
{
    public class Crosshair : UiElement
    {
        [SerializeField] private TMP_Text distanceLabel;
        [SerializeField] private Transform cross;
        [SerializeField] private Transform crossL;
        [SerializeField] private Transform crossR;

        [SerializeField] private Image crossImage;
        [SerializeField] private Image crossImageL;
        [SerializeField] private Image crossImageR;

        private Hero hero;

        private void Awake()
        {
            MenuManager.MenuOpened += Hide;
            MenuManager.MenuClosed += Show;
            Hero.OnSpawnClient += (hero) => this.hero = hero;
        }

        private void OnDestroy()
        {
            MenuManager.MenuOpened -= Hide;
            MenuManager.MenuClosed -= Show;
        }

        private void LateUpdate()
        {
            if (MenuManager.IsAnyMenuOpen || !hero)
                return;

            var mousePos = Mouse.current.position.ReadValue();

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(mousePos);
            LayerMask mask = Layers.Ground.ToLayer() | Layers.EnemyBox.ToLayer();

            var distanceText = "???";
            var magnitude = Hero.HOOK_RAYCAST_MAX_DISTANCE;
            var hitDistance = Hero.HOOK_RAYCAST_MAX_DISTANCE;
            var hitPoint = ray.GetPoint(hitDistance);

            cross.position = mousePos;

            if (Physics.Raycast(ray, out var hit, hitDistance, mask.value))
            {
                magnitude = (hit.point - hero.transform.position).magnitude;
                distanceText = ((int) magnitude).ToString();
                hitDistance = hit.distance;
                hitPoint = hit.point;
            }

            crossImage.color = magnitude > 120f ? Color.red : Color.white;
            distanceLabel.transform.localPosition = cross.localPosition + new Vector3(0, -30f);

            if (((int) FengGameManagerMKII.settings[0xbd]) == 1)
                distanceText += hero.CurrentSpeed.ToString("F1") + " u/s";
            else if (((int) FengGameManagerMKII.settings[0xbd]) == 2)
                distanceText += (hero.CurrentSpeed / 100f).ToString("F1") + "K";

            distanceLabel.text = distanceText;

            var leftBase = new Vector3(0f, 0.4f, 0f);
            leftBase -= hero.transform.right * 0.3f;

            var rightBase = new Vector3(0f, 0.4f, 0f);
            rightBase += hero.transform.right * 0.3f;

            var scaledHitDistance = hitDistance * (hitDistance <= 50f ? 0.05f : 0.3f);
            var left = (hitPoint - (hero.transform.right * scaledHitDistance)) - (hero.transform.position + leftBase);
            var right = (hitPoint + (hero.transform.right * scaledHitDistance)) - (hero.transform.position + rightBase);

            left.Normalize();
            right.Normalize();

            left *= Hero.HOOK_RAYCAST_MAX_DISTANCE;
            right *= Hero.HOOK_RAYCAST_MAX_DISTANCE;

            hitPoint = hero.transform.position + leftBase + left;
            hitDistance = Hero.HOOK_RAYCAST_MAX_DISTANCE;
            if (Physics.Linecast(hero.transform.position + leftBase, hitPoint, out var hit2, mask.value))
            {
                hitPoint = hit2.point;
                hitDistance = hit2.distance;
            }

            crossL.transform.position = hero.CurrentCamera.WorldToScreenPoint(hitPoint);
            crossL.transform.localRotation = Quaternion.Euler(0f, 0f,
                (Mathf.Atan2(crossL.transform.position.y - mousePos.y, crossL.transform.position.x - mousePos.x) * Mathf.Rad2Deg) + 180f);
            crossImageL.color = hitDistance > 120f ? Color.red : Color.white;

            hitPoint = hero.transform.position + rightBase + right;
            hitDistance = Hero.HOOK_RAYCAST_MAX_DISTANCE;
            if (Physics.Linecast(hero.transform.position + rightBase, hitPoint, out var hit3, mask.value))
            {
                hitPoint = hit3.point;
                hitDistance = hit3.distance;
            }

            crossR.transform.position = hero.CurrentCamera.WorldToScreenPoint(hitPoint);
            crossR.transform.localRotation = Quaternion.Euler(0f, 0f,
                Mathf.Atan2(crossR.transform.position.y - mousePos.y, crossR.transform.position.x - mousePos.x) * Mathf.Rad2Deg);
            crossImageR.color = hitDistance > 120f ? Color.red : Color.white;
        }
    }
}
