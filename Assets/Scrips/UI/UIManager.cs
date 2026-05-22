using System;
using UnityEngine;
using UnityEngine.UI;

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

        public float sizeMultiplier = 4;

        void Start()
        {

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
            value_actual = Mathf.Clamp(value_actual, 0, 1000);

            r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
            r_v.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
        }
        public void Tick(CharacterStats stats, float delta)
        {
            health.value = stats._health;
            focus.value = stats._focus;
            stamina.value = stats._stamina;

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

        public static UIManager singleton;

        void Awake()
        {
            singleton = this;
        }
    }
}

