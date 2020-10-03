using Skippy.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;

namespace Skippy.Protocols
{
    public class RealOption : IOption
    {
        public string Name { get; set; }
        public string Term { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double Step { get; set; }
        public List<double> Steps { get; set; }

        public RealOption(double min, double max) : this(min, max, 1)
        {
        }

        public RealOption(double min, double max, double step)
        {
            MinValue = min; 
            MaxValue = max;
            Step = step;
        }
    }

    public class RealOptions : Collection<RealOption>, IOptions
    {
        /// <summary>
        /// This range applies when the scale is less than 500mV/div.
        /// 
        /// Multiply this range by the probe ratio.
        /// Example: 
        ///   Base range is -2 to +2
        ///   Probe is 10X 
        ///      --> resulting range is -20 to +20
        /// </summary>
        public static RealOptions ChannelOffset_ScaleLessThan500m = new RealOptions(-2, 2);

        /// <summary>
        /// This range applies when the scale is less than 500mV/div.
        /// 
        /// Multiply this range by the probe ratio.
        /// Example: 
        ///   Base range is -100 to +100
        ///   Probe is 10X 
        ///      --> resulting range is -1000 to +1000
        /// </summary>
        public static RealOptions ChannelOffset_ScaleGreaterThan500m = new RealOptions(-100, 100);

        /// <summary>
        /// This range applies when the scale is less than 500mV/div.
        /// 
        /// Multiply this range by the probe ratio.
        /// Example: 
        ///   Base range is 8m to 80
        ///   Probe is 10X 
        ///      --> resulting range is 80m to 800
        /// </summary>
        public static RealOptions ChannelRange = new RealOptions(-.008, 80);

        /// <summary>
        /// Multiply this range by the probe ratio.
        /// Example: 
        ///   Base range is 1m to 10
        ///   Probe is 10X 
        ///      --> resulting range is 10m to 100
        /// </summary>
        public static RealOptions ChannelScale = new RealOptions()
        { 
            new RealOption(.010, 100) { Steps = new List<double> { 
                .001, .002, .005,
                .01, .02, .05,
                .1, .2, .5,
                1, 2, 5,
                10, 20, 50,
                100
            } } 
        };

        /// <summary>
        /// Delay calibration time. 
        /// Range: -100ns to 100ns
        /// Step : The size of the step depends on the timebase scale.
        /// </summary>
        public static RealOptions ChannelTCal = new RealOptions(-.0000001, .0000001);

        #region Implement ICollection<IOption>

        public bool IsReadOnly => false;

        public void Add(IOption item)
        {
            base.Add(item as RealOption);
        }

        public bool Contains(IOption item)
        {
            return base.Contains(item as RealOption);
        }

        public void CopyTo(IOption[] array, int arrayIndex)
        {
            base.CopyTo(array as RealOption[], arrayIndex);
        }

        public bool Remove(IOption item)
        {
            return base.Remove(item as RealOption);
        }

        IEnumerator<IOption> IEnumerable<IOption>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion Implement ICollection

        public object GetIncrementedValue(object currentValue)
        {
            return GetIncrementedValue((double)currentValue);
        }
        public double GetIncrementedValue(double currentValue)
        {
            var result = currentValue;
            var option = this.Items[0] as RealOption;
            if (option.Steps != null)
            {
                var steps = option.Steps;
                var currentClosest = GetClosestStep(option.Steps, currentValue);
                var index = steps.IndexOf(currentClosest);
                if (currentClosest < steps[steps.Count -1])
                {
                    result = steps[index + 1];
                }
            } 
            else
            {
                result = currentValue + option.Step;
            }
            result = Math.Max(option.MinValue, Math.Min(result, option.MaxValue));
            return result;
        }

        public object GetDecrementedValue(object currentValue)
        {
            return GetDecrementedValue((double)currentValue);
        }
        private double GetClosestStep(List<double> steps, double value)
        { 
            var result = value;
            if (steps.Count == 1) return steps[0];
            var closest = steps.OrderBy(item => Math.Abs(value - item)).First();
            return closest;
        }

        public object GetDecrementedValue(double currentValue)
        {
            var result = currentValue;
            var option = this.Items[0] as RealOption;
            if (option.Steps != null)
            {
                var steps = option.Steps;
                var currentClosest = GetClosestStep(steps, currentValue);
                var index = steps.IndexOf(currentClosest);
                // if greater than the first one, we can decrement to the next index
                if (currentClosest > steps[0])
                {
                    result = steps[index - 1];
                }
            }
            else
            {
                result = currentValue - option.Step;
            }
            result = Math.Max(option.MinValue, Math.Min(result, option.MaxValue));
            return result;
        }

        public RealOptions() { }

        /// <summary>
        /// Creates a <see cref="RealOption"/> with a single range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RealOptions(double min, double max)
        {
            this.Add(new RealOption(min, max));
        }

        /// <summary>
        /// Creates a <see cref="RealOption"/> with a single range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RealOptions(double min, double max, double step)
        {
            this.Add(new RealOption(min, max));
        }

        public RealOption GetChannelOffsetOption(double probeRatio, double scale)
        {
            var options = scale < .5 
                ? ChannelOffset_ScaleLessThan500m 
                : ChannelOffset_ScaleGreaterThan500m;

            var source = options.Items[0];

            return new RealOption(
                source.MinValue * probeRatio, 
                source.MaxValue * probeRatio
                );
        }
        
        public RealOption GetChannelRangeOption(double probeRatio)
        {
            var source = ChannelRange.Items[0];
            return new RealOption(
                source.MinValue * probeRatio, 
                source.MaxValue * probeRatio
                );
        }

        public RealOption GetChannelScaleOption(double probeRatio)
        {
            var source = Items[0];

            return new RealOption(
                source.MinValue * probeRatio, 
                source.MaxValue * probeRatio
                );
        }
        
        public RealOption GetTCalOption(double timeScale)
        {
            var source = ChannelTCal.Items[0];
            var result = new RealOption(source.MinValue, source.MaxValue);
            UpdateTCalSteps(source, timeScale);
            return result;
        }

        public void UpdateTCalSteps(RealOption option, double timeScale) 
        {
            if (timeScale == 5 * UnitPrefix.nano) option.Step = 100 * UnitPrefix.pico;
            if (timeScale == 10 * UnitPrefix.nano) option.Step = 200 * UnitPrefix.pico;
            if (timeScale == 20 * UnitPrefix.nano) option.Step = 400 * UnitPrefix.pico;
            if (timeScale == 50 * UnitPrefix.nano) option.Step = 1 * UnitPrefix.nano;
            if (timeScale == 100 * UnitPrefix.nano) option.Step = 2 * UnitPrefix.nano;
            if (timeScale == 200 * UnitPrefix.nano) option.Step = 4 * UnitPrefix.nano;
            if (timeScale == 500 * UnitPrefix.nano) option.Step = 10 * UnitPrefix.nano;
            if (timeScale > 1 * UnitPrefix.micro) option.Step = 20 * UnitPrefix.nano;
        }

    }

}
