namespace Geometry_Dash_Randomiser {

      public enum ResourceType {
            // The default value. If anything has this, a resource's type has not been recognised, and will be ignored during randomization
            Unknown,

            // Main Resource types
            Block,
            Portal,
            Orb,
            Pad,
            Editor,
            Menu,
            Particle,
            Effect,
            Miscellaneous,
            Shop,

            // Icon Types
            Cube,
            Ship,
            Ball,
            UFO,
            Wave,
            Robot,
            Spider,
            Swing,
            Jetpack
      }
}
