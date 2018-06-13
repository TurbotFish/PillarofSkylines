using Game.Model;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.UI.PauseMenu
{
    public class AbilityListView : MonoBehaviour
    {

        //###########################################################

        // -- ATTRIBUTES

        [SerializeField] float entranceDelay = 0.1f;
        [SerializeField] float stickIntensity = 10f;
        [SerializeField] GameObject blinkFeedback;

        private AbilityEntryView[] abilityEntries;

        private VerticalLayoutGroup layoutGroup;

        RectTransform movable;
        Vector3 movableAnchorPosition;

        bool isInitialized;

        //###########################################################

        // -- INITIALIZATION

        public void Initialize(PlayerModel playerModel)
        {
            layoutGroup = GetComponent<VerticalLayoutGroup>();

            layoutGroup.enabled = false;

            abilityEntries = GetComponentsInChildren<AbilityEntryView>();

            foreach (AbilityEntryView ability in abilityEntries)
                ability.containerList = this;

            isInitialized = true;
        }

        //###########################################################

        // -- OPERATIONS

        private void Update() {
            if (movable) {

                Vector3 offset = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * stickIntensity;

                movable.anchoredPosition = movableAnchorPosition + offset;

                foreach (RectTransform child in movable)
                    child.anchoredPosition = -offset;

            }   
        }

        public void SetMovable(RectTransform newMovable)
        {
            // reset previous
            if (movable)
            {
                movable.anchoredPosition = movableAnchorPosition;
                foreach (RectTransform child in movable)
                    child.anchoredPosition = Vector3.zero;
            }
            movable = newMovable;

            if (movable) {
                movableAnchorPosition = movable.anchoredPosition;
                movableAnchorPosition.x = 0;


                RectTransform feedback = Instantiate(blinkFeedback, transform).transform as RectTransform;
                feedback.anchoredPosition = movableAnchorPosition;
            }
        }


        private void OnEnable() {
            if (isInitialized)
                StartCoroutine(_TriggerEntrance());
        }

        private void OnDisable()
        {
            SetMovable(null);
        }

        IEnumerator _TriggerEntrance()
        {
            foreach (AbilityEntryView ability in abilityEntries)
                ability.Hide();

            foreach (AbilityEntryView ability in abilityEntries) {
                ability.Entrance();
                yield return new WaitForSeconds(entranceDelay);
            }
        }

    }
}