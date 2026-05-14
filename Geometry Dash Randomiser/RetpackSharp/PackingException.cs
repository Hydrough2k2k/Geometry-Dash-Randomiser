using System;

namespace RectpackSharp {

      /// <summary>
      /// Modified version of the original PackingException class.<br/>
      /// Originally created by ThomasMiz: https://github.com/ThomasMiz/RectpackSharp<br/>
      /// Modifications were necessary, to make it compatible with .NET Framework 4.8.
      /// </summary>
      public class PackingException : Exception {

            public PackingException() : base() { }

            public PackingException(string message) : base(message) { }

            public PackingException(string message, Exception innerException) : base(message, innerException) { }
      }
}
