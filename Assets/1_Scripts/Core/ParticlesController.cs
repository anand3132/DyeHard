using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedGaint
{
    public class ParticlesController : MonoBehaviour
    {
        // The color to apply to the paintable surface when particles collide
        public Color paintColor;

        // The minimum and maximum radius for the paint effect applied on collision
        public float minRadius = 0.05f;
        public float maxRadius = 0.2f;

        // The strength of the paint effect (how strong the paint is)
        public float strength = 1;

        // The hardness of the paint effect (how sharp or soft the paint's edges are)
        public float hardness = 1;

        // Reference to the ParticleSystem component attached to the GameObject
        private ParticleSystem part;

        // List to store the collision events between particles and objects
        private List<ParticleCollisionEvent> collisionEvents;

        void Start()
        {
            // Get the ParticleSystem component attached to this GameObject
            part = GetComponent<ParticleSystem>();

            // Initialize the list to store collision events
            collisionEvents = new List<ParticleCollisionEvent>();

            // Optional: Dynamically set paintColor based on the particle system's material color
            // Uncomment if you want to pull the color from the particle system’s material
            // var pr = part.GetComponent<ParticleSystemRenderer>();
            // Color c = new Color(pr.material.color.r, pr.material.color.g, pr.material.color.b, .8f);
            // paintColor = c;
        }

        // This method is called automatically by Unity when particles collide with another GameObject
        void OnParticleCollision(GameObject other)
        {
            // Retrieve the collision events between the particles and the collided object
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            // Get the Paintable component from the collided object
            Paintable paintableSurface = other.GetComponent<Paintable>();

            // Proceed only if the collided object has a Paintable component
            if (paintableSurface != null)
            {
                // Loop through each collision event and apply paint
                for (int i = 0; i < numCollisionEvents; i++)
                {
                    // Get the intersection point (collision position) from each event
                    Vector3 collisionPosition = collisionEvents[i].intersection;

                    // Randomly choose a radius for the paint effect within the defined range
                    float radius = Random.Range(minRadius, maxRadius);

                    // Call PaintManager to apply the paint on the Paintable surface at the collision point
                    // The parameters are: the surface to paint, position, radius of the effect,
                    // hardness of the paint edge, strength of the paint, and the color
                    PaintManager.instance.paint(paintableSurface, collisionPosition, radius, hardness, strength, paintColor);
                }
            }
        }

        // New function to change the paint color
        // This function allows other scripts or components to change the paint color dynamically
        public void SetPaintColor(Color newColor)
        {
            // Set the paint color to the new color passed as an argument
            paintColor = newColor;
        }
    } // ParticlesController class
} // RedGaint namespace
