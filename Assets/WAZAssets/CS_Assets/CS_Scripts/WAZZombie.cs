using UnityEngine;
using System.Collections;

namespace WhackAZombie
{
    public class WAZZombie : MonoBehaviour
    {
        // A referemce to the Game Controller, which is taken by the first time this script runs, and is remembered across all other scripts of this type
        static WAZGameController gameController;

        // The animated part of the zombie. By default this is taken from this object
        internal Animator zombieAnimator;

        [Tooltip("The helmet object of this zombie, assigned from inside the zombie itself")]
        public GameObject helmet;

        [Tooltip("The broken helmet that appears when the helmet breaks. This is assigned from the project pane")]
        public Transform brokenHelmet;

        [Tooltip("The health of the zombie when it's wearing a helmet")]
        public int helmetHealth = 2;

        [Tooltip("The bonus we get for killing a regular zombie")]
        public int zombieBonus = 10;

        [Tooltip("The bonus we get for killing a helmet zombie")]
        public int helmetBonus = 30;

        [Tooltip("The bonus we get for killing a quick zombie")]
        public int quickBonus = 50;

        // The extra bonus that we give to this zombie if it's a quick or helmet type
        internal int currentBonus = 0;

        // The health of the zombie when it's not wearing a helmet
        internal int health = 1;

        [Tooltip("The tag of the object that can hit this zombie")]
        public string targetTag = "Player";

        // Is the zombie dead?
        internal bool isDead = false;

        // How long to wait before showing the zombie
        internal float showTime = 0;

        // How long to wait before hiding the zombie, after it has been revealed
        internal float hideDelay = 0;

        [Tooltip("The animation name when showing a zombie")]
        public string animationShow = "Show";

        [Tooltip("The animation name when hiding a zombie")]
        public string animationHide = "Hide";

        [Tooltip("A list of animations when the zombie dies. The animation is chosen randomly from the list")]
        public string[] animationDie = { "Smack", "Whack", "Thud" };

        // Use this for initialization
        void Start()
        {
            // Hold the gamcontroller object in a variable for quicker access
            if (gameController == null) gameController = GameObject.FindObjectOfType<WAZGameController>();

            // The animator of the zombie. This holds all the animations and the transitions between them
            zombieAnimator = GetComponent<Animator>();
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update()
        {
            // Count down the time until the zombie is hidden
            if (isDead == false && hideDelay > 0)
            {
                hideDelay -= Time.deltaTime;

                // If enough time passes, hide the zombie
                if (hideDelay <= 0) Hide();
            }
        }

        /// <summary>
        /// Raises the trigger enter2d event. Works only with 2D physics.
        /// </summary>
        /// <param name="other"> The other collider that this object touches</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            // Check if we hit the correct target
            if ( isDead == false && other.tag == targetTag )
            {
                // Change the health of the target, and if it dies give a bonus to the player
                if ( ChangeHealth(-1) <= 0 )    gameController.HitBonus(other.transform, currentBonus);
            }
        }

        /// <summary>
        /// Changes the health of the target, and checks if it should die
        /// </summary>
        /// <param name="changeValue"></param>
        public int ChangeHealth(int changeValue)
        {
            // Chnage health value
            health += changeValue;

            if (health > 0)
            {
                // Animated the hit effect
                zombieAnimator.Play("Hit", -1, 0f);
            }
            else
            {
                // Health reached 0, so the target is dead
                Die();
            }

            // Return the current health value to see if this object is dead
            return health;
        }

        /// <summary>
        /// Kills the object and gives it a random animation from a list of death animations
        /// </summary>
        public void Die()
        {
            // The zombie is now dead. It can't move.
            isDead = true;

            // If there is a helment object, deactivate it and create a helmet break effect
            if (helmet && helmet.activeSelf == true)
            {
                // Create the helmet break effect
                if (brokenHelmet) Instantiate(brokenHelmet, helmet.transform.position, helmet.transform.rotation);

                // Deactivate the helmet object
                helmet.SetActive(false);
            }
            
            // Choose one of the death animations randomly
            if (animationDie.Length > 0) zombieAnimator.Play(animationDie[Mathf.FloorToInt(Random.Range(0, animationDie.Length))]);
        }

        /// <summary>
        /// Hides the target, animating it and then sets it to hidden
        /// </summary>
        void Hide()
        {
            // Play the hiding animation
            GetComponent<Animator>().Play(animationHide);
        }

        /// <summary>
        /// Shows the regular zombie
        /// </summary>
        /// <returns>The target.</returns>
        public void Show(float showDuration)
        {
            // The zombie is not dead anymore, so it can appear from the hole
            isDead = false;

            // If the zombie has a helmet, deactivate it
            if (helmet) helmet.SetActive(false);

            // Set the health of the zombie to 1 hit
            health = 1;

            // Play the show animation
            GetComponent<Animator>().Play(animationShow);

            // Set how long to wait before hiding the zombie again
            hideDelay = showDuration;

            // Give no extra bonus for this regular zombie
            currentBonus = zombieBonus;
        }

        /// <summary>
        /// Shows the zombie with a helmet
        /// </summary>
        /// <returns>The target.</returns>
        public void ShowHelmet(float showDuration)
        {
            // The zombie is not dead anymore, so it can appear from the hole
            isDead = false;

            // If the zombie has a helmet, deactivate it
            if (helmet) helmet.SetActive(true);

            // Set the health of the zombie to the helmet health
            health = helmetHealth;

            // Play the show animation
            GetComponent<Animator>().Play(animationShow);

            // Set how long to wait before hiding the zombie again
            hideDelay = showDuration;

            // Give extra bonus for this helmet zombie
            currentBonus = helmetBonus;
        }

        /// <summary>
        /// Shows the quick zombie
        /// </summary>
        /// <returns>The target.</returns>
        public void ShowQuick(float showDuration)
        {
            // The zombie is not dead anymore, so it can appear from the hole
            isDead = false;

            // If the zombie has a helmet, deactivate it
            if (helmet) helmet.SetActive(false);

            // Set the health of the zombie to 1 hit
            health = 1;

            // Play the show animation
            GetComponent<Animator>().Play("Quick");

            // Set how long to wait before hiding the zombie again
            hideDelay = 0;

            // Give extra bonus for this quick zombie
            currentBonus = quickBonus;
        }

    }
}
