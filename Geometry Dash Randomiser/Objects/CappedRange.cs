using System;

namespace Geometry_Dash_Randomiser {

      public class CappedRange {

            public event EventHandler ConfigUpdated;

            public CappedRange() { }

            public CappedRange(int min, int max, int minCap, int maxCap) {
                  Min = min;
                  Max = max;

                  _minCap = minCap;
                  _maxCap = maxCap;
            }

            public CappedRange(int min, int max) {
                  Min = min;
                  Max = max;
            }

            public static CappedRange Default => new CappedRange();

            public int Min {
                  get { return _min; }
                  set {
                        _min = value.Clamp(_minCap, _maxCap);

                        if (_min > _max)
                              _min = _max;

                        ConfigUpdated?.Invoke(this, null);
                  }
            }

            public int Max {
                  get { return _max; }
                  set {
                        _max = value.Clamp(_minCap, _maxCap);

                        if (_max < _min)
                              _max = _min;

                        ConfigUpdated?.Invoke(this, null);
                  }
            }

            public void CopyDataFrom(CappedRange source) {
                  Min = source._min;
                  Max = source._max;
            }

            private int _min;
            private int _max;

            private readonly int _minCap = -128;
            private readonly int _maxCap = 127;
      }
}
