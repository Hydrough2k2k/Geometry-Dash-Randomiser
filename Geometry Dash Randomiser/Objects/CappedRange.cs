using System;

namespace Geometry_Dash_Randomiser {

      public class CappedRange : Range, IEquatable<CappedRange> {

            public CappedRange() { }

            public CappedRange(int min, int max, int minCap, int maxCap) {
                  _minCap = minCap;
                  _maxCap = maxCap;

                  Min = min;
                  Max = max;
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
                  if (source == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"source\" was passed as null in CappedRange.CopyDataFrom");
                        return;
                  }

                  Min = source._min;
                  Max = source._max;
            }

            public bool Equals(CappedRange other) {
                  if (other == null) {
                        Logger.Write(Logger.Mode.Error, "The parameter \"other\" was passed as null in CappedRange.Equals");
                        return false;
                  }

                  if (this.Min != other.Min || this.Max != other.Max)
                        return false;

                  return true;
            }

            private readonly int _minCap = -255;
            private readonly int _maxCap = 256;
      }
}
