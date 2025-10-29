using UnityEngine;
using UnityEngine.Playables;

namespace Work.PSB.Code.Test
{
    public class SkipTimeline : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private KeyCode skipKey = KeyCode.Space;

        [SerializeField] private double jumpTime = 10.0;

        private void Update()
        {
            if (director == null) return;

            if (director.state == PlayState.Playing && Input.GetKeyDown(skipKey))
            {
                JumpToTime(jumpTime);
            }
        }

        private void JumpToTime(double time)
        {
            time = Mathf.Clamp((float)time, 0f, (float)director.duration);
            
            director.time = time;
            director.Evaluate();
            director.Play();
            gameObject.SetActive(false);
        }
        
    }
}