using System;

namespace Geometry_Dash_Randomiser {

      /// <summary>
      /// Interface for a class to raise an event if any property of it changed<br/>
      /// A class using this must have all it's properties raise this event in the set; field
      /// </summary>
      internal interface IUpdateNotify {

            event EventHandler Updated;
      }
}
