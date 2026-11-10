namespace Geometry_Dash_Randomiser {

      public enum ResourceType {
            // The default value. If anything has this, a resource's type has not been recognised, and will be ignored during randomization
            Unknown,

            // Icon Types
            Cube,
            Ship,
            Ball,
            UFO,
            Wave,
            Robot,
            Spider,
            Swing,
            Jetpack,

            // Text Types
            Letter,
            Number,
            Symbol,
            Special,

            // Main Resource types
            Menu,
            Shop,
            Editor,
            Block,
            Portal,
            Orb,
            Pad,
            Particle,
            Effect,
            Miscellaneous
      }
}
