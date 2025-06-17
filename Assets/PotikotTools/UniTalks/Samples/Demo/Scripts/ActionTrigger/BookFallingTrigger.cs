using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    [RequireComponent(typeof(Animator))]
    public class BookFallingTrigger : MonoBehaviour, IActionTrigger
    {
        private static readonly int ActionAnimationHash = Animator.StringToHash("action");
        
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            G.BookActionTrigger = this;
        }

        public void Trigger()
        {
            animator.SetTrigger(ActionAnimationHash);
        }   
    }
}