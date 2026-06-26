using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SA.UI;
using System.Collections.Generic;

namespace SA
{
    public class UIManager : MonoBehaviour
    {
        public float lerpSpeed = 2;
        public Slider health;
        public Slider h_visual;
        public Slider focus;
        public Slider f_visual;
        public Slider stamina;
        public Slider s_visual;
        public TextMeshProUGUI souls;
        public float sizeMultiplier = 4;
        int curSouls;
        public GesturesManager gestures;

        public GameObject interactCard;
        public TextMeshProUGUI ac_action_type;

        int ac_index;
        public List<AnnounceCard> an_cards;


        void Start()
        {
            gestures = GesturesManager.singleton;
            interactCard.SetActive(false);
            CloseCards();
            CloseAnnounceType();
        }


        public void InitSouls(int v)
        {
            curSouls = v;
        }
        public void InitSlider(StatSlider t, int value)
        {
            Slider s = null;
            Slider v = null;
            switch (t)
            {
                case StatSlider.health:
                    s = health;
                    v = h_visual;
                    break;
                case StatSlider.focus:
                    s = focus;
                    v = f_visual;
                    break;
                case StatSlider.stamina:
                    s = stamina;
                    v = s_visual;
                    break;
            }
            s.maxValue = value;
            v.maxValue = value;

            RectTransform r = s.GetComponent<RectTransform>();
            RectTransform r_v = v.GetComponent<RectTransform>();
            float value_actual = value * sizeMultiplier;
            value_actual = Mathf.Clamp(value_actual, 0, 500);

            r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
            r_v.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
        }
        public void Tick(CharacterStats stats, float delta)
        {
            GameUI(stats, delta);

        }

        void GameUI(CharacterStats stats, float delta)
        {
            health.value = Mathf.Lerp(health.value, stats._health, delta * lerpSpeed * 2);
            focus.value = Mathf.Lerp(focus.value, stats._focus, delta * lerpSpeed * 2);
            stamina.value = stats._stamina;

            curSouls = Mathf.RoundToInt(Mathf.Lerp(curSouls, stats._souls, delta * lerpSpeed));
            souls.text = curSouls.ToString();

            h_visual.value = Mathf.Lerp(h_visual.value, stats._health, delta * lerpSpeed);
            f_visual.value = Mathf.Lerp(f_visual.value, stats._focus, delta * lerpSpeed);
            s_visual.value = Mathf.Lerp(s_visual.value, stats._stamina, delta * lerpSpeed);
        }
        public void AffectAll(int h, int f, int s)
        {
            InitSlider(StatSlider.health, h);
            InitSlider(StatSlider.focus, f);
            InitSlider(StatSlider.stamina, s);
        }
        public enum StatSlider
        {
            health, focus, stamina
        }
        public void OpenAnnounceType(UIActionType t)
        {
            switch (t)
            {
                case UIActionType.pick:
                    ac_action_type.text = StaticStrings.ui_ac_pick;
                    break;
                case UIActionType.interract:
                    ac_action_type.text = StaticStrings.ui_ac_interract;
                    break;
                case UIActionType.talk:
                    ac_action_type.text = StaticStrings.ui_ac_talk;
                    break;
                case UIActionType.open:
                    ac_action_type.text = StaticStrings.ui_ac_open;
                    break;
            }
            interactCard.SetActive(true);
        }
        public void AddAnnounceCard(Item i)
        {
            an_cards[ac_index].itemName.text = i.name_item;
            an_cards[ac_index].icon.sprite = i.icon;
            an_cards[ac_index].gameObject.SetActive(true);
            ac_index++;

            if (ac_index > 5)
            {
                ac_index = 0;
            }
        }
        public void CloseCards()
        {
            for(int i = 0; i < an_cards.Count; i++)
            {
                an_cards[i].gameObject.SetActive(false);
            }
        }
        public void CloseAnnounceType()
        {
            interactCard.SetActive(false);
        }
        public static UIManager singleton;

        void Awake()
        {
            singleton = this;
        }
    }
    public enum UIActionType
    {
        pick, interract, open, talk
    }
}

