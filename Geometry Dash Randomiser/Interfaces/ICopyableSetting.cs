using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_Randomiser {

      public interface ICopyableSetting<T> {

            void CopyDataFrom(T source);
      }
}
