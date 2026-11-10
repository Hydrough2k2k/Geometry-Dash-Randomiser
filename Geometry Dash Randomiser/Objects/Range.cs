using System;

namespace Geometry_Dash_Randomiser {

      public class Range : IUpdateNotify {

            public event EventHandler Updated;

            public Range() { }

            public Range(int min, int max) {
                  Min = min;
                  Max = max;
            }

            public static Range Default => new Range();

            public virtual int Min {
                  get { return _min; }
                  set {
                        _min = value;

                        if (_min > _max)
                              _min = _max;

                        InvokeUpdatedEvent(this, null);
                  }
            }

            public virtual int Max {
                  get { return _max; }
                  set {
                        _max = value;

                        if (_max < _min)
                              _max = _min;

                        InvokeUpdatedEvent(this, null);
                  }
            }

            public void CopyDataFrom(Range source) {
                  Min = source._min;
                  Max = source._max;
            }

            virtual protected void InvokeUpdatedEvent(object sender, EventArgs e) {
                  Updated?.Invoke(this, null);
            }

            private protected int _min;
            private protected int _max;
      }
}
