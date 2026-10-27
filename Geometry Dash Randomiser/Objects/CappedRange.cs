namespace Geometry_Dash_Randomiser {

      public class CappedRange : Range {

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

            public static new CappedRange Default => new CappedRange();

            public override int Min {
                  get { return _min; }
                  set {
                        _min = Math.Clamp(value, _minCap, _maxCap);

                        if (_min > _max)
                              _min = _max;

                        base.InvokeUpdatedEvent(this, null);
                  }
            }

            public override int Max {
                  get { return _max; }
                  set {
                        _max = Math.Clamp(value, _minCap, _maxCap);

                        if (_max < _min)
                              _max = _min;

                        base.InvokeUpdatedEvent(this, null);
                  }
            }

            public void CopyDataFrom(CappedRange source) {
                  Min = source._min;
                  Max = source._max;
            }

            private readonly int _minCap = -128;
            private readonly int _maxCap = 127;
      }
}
