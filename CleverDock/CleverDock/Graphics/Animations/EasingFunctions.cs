using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Graphics.Animations
{
    public class EasingFunctions
    {
        // Code adapted from: https://github.com/warrenm/AHEasing/blob/master/AHEasing/easing.c
        // Published under the Do What The Fuck You Want To Public License, Version 2.
        // Cheat sheet this way: http://easings.net/

        public static double LinearInterpolation(double p)
        {
            return p;
        }

        // Modeled after the parabola y = x^2
        public static double QuadraticEaseIn(double p)
        {
            return p * p;
        }

        // Modeled after the parabola y = -x^2 + 2x
        public static double QuadraticEaseOut(double p)
        {
            return -(p * (p - 2));
        }

        // Modeled after the piecewise quadratic
        // y = (1/2)((2x)^2)             ; [0, 0.5)
        // y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
        public static double QuadraticEaseInOut(double p)
        {
            if (p < 0.5)
                return 2 * p * p;
            else
                return (-2 * p * p) + (4 * p) - 1;
        }

        // Modeled after the cubic y = x^3
        public static double CubicEaseIn(double p)
        {
            return p * p * p;
        }

        // Modeled after the cubic y = (x - 1)^3 + 1
        public static double CubicEaseOut(double p)
        {
            double f = (p - 1);
            return f * f * f + 1;
        }

        // Modeled after the piecewise cubic
        // y = (1/2)((2x)^3)       ; [0, 0.5)
        // y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
        public static double CubicEaseInOut(double p)
        {
            if (p < 0.5)
                return 4 * p * p * p;
            else
            {
                double f = ((2 * p) - 2);
                return 0.5 * f * f * f + 1;
            }
        }

        // Modeled after the quartic x^4
        public static double QuarticEaseIn(double p)
        {
            return p * p * p * p;
        }

        // Modeled after the quartic y = 1 - (x - 1)^4
        public static double QuarticEaseOut(double p)
        {
            double f = (p - 1);
            return f * f * f * (1 - p) + 1;
        }

        // Modeled after the piecewise quartic
        // y = (1/2)((2x)^4)        ; [0, 0.5)
        // y = -(1/2)((2x-2)^4 - 2) ; [0.5, 1]
        public static double QuarticEaseInOut(double p)
        {
            if (p < 0.5)
                return 8 * p * p * p * p;
            else
            {
                double f = (p - 1);
                return -8 * f * f * f * f + 1;
            }
        }

        // Modeled after the quintic y = x^5
        public static double QuinticEaseIn(double p)
        {
            return p * p * p * p * p;
        }

        // Modeled after the quintic y = (x - 1)^5 + 1
        public static double QuinticEaseOut(double p)
        {
            double f = (p - 1);
            return f * f * f * f * f + 1;
        }

        // Modeled after the piecewise quintic
        // y = (1/2)((2x)^5)       ; [0, 0.5)
        // y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
        public static double QuinticEaseInOut(double p)
        {
            if (p < 0.5)
                return 16 * p * p * p * p * p;
            else
            {
                double f = ((2 * p) - 2);
                return 0.5 * f * f * f * f * f + 1;
            }
        }

        // Modeled after quarter-cycle of sine wave
        public static double SineEaseIn(double p)
        {
            return Math.Sin((p - 1) * Math.PI / 2) + 1;
        }

        // Modeled after quarter-cycle of sine wave (different phase)
        public static double SineEaseOut(double p)
        {
            return Math.Sin(p * Math.PI / 2);
        }

        // Modeled after half sine wave
        public static double SineEaseInOut(double p)
        {
            return 0.5 * (1 - Math.Cos(p * Math.PI));
        }

        // Modeled after shifted quadrant IV of unit circle
        public static double CircularEaseIn(double p)
        {
            return 1 - Math.Sqrt(1 - (p * p));
        }

        // Modeled after shifted quadrant II of unit circle
        public static double CircularEaseOut(double p)
        {
            return Math.Sqrt((2 - p) * p);
        }

        // Modeled after the piecewise circular function
        // y = (1/2)(1 - sqrt(1 - 4x^2))           ; [0, 0.5)
        // y = (1/2)(sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
        public static double CircularEaseInOut(double p)
        {
            if (p < 0.5)
                return 0.5 * (1 - Math.Sqrt(1 - 4 * (p * p)));
            else
                return 0.5 * (Math.Sqrt(-((2 * p) - 3) * ((2 * p) - 1)) + 1);
        }

        // Modeled after the exponential function y = 2^(10(x - 1))
        public static double ExponentialEaseIn(double p)
        {
            return (p == 0.0) ? p : Math.Pow(2, 10 * (p - 1));
        }

        // Modeled after the exponential function y = -2^(-10x) + 1
        public static double ExponentialEaseOut(double p)
        {
            return (p == 1.0) ? p : 1 - Math.Pow(2, -10 * p);
        }

        // Modeled after the piecewise exponential
        // y = (1/2)2^(10(2x - 1))         ; [0,0.5)
        // y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
        public static double ExponentialEaseInOut(double p)
        {
            if (p == 0.0 || p == 1.0) return p;

            if (p < 0.5)
                return 0.5 * Math.Pow(2, (20 * p) - 10);
            else
                return -0.5 * Math.Pow(2, (-20 * p) + 10) + 1;
        }

        // Modeled after the damped sine wave y = sin(13pi/2*x)*pow(2, 10 * (x - 1))
        public static double ElasticEaseIn(double p)
        {
            return Math.Sin(13 * Math.PI / 2 * p) * Math.Pow(2, 10 * (p - 1));
        }

        // Modeled after the damped sine wave y = sin(-13pi/2*(x + 1))*pow(2, -10x) + 1
        public static double ElasticEaseOut(double p)
        {
            return Math.Sin(-13 * Math.PI / 2 * (p + 1)) * Math.Pow(2, -10 * p) + 1;
        }

        // Modeled after the piecewise exponentially-damped sine wave:
        // y = (1/2)*sin(13pi/2*(2*x))*pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
        // y = (1/2)*(sin(-13pi/2*((2x-1)+1))*pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
        public static double ElasticEaseInOut(double p)
        {
            if (p < 0.5)
                return 0.5 * Math.Sin(13 * Math.PI / 2 * (2 * p)) * Math.Pow(2, 10 * ((2 * p) - 1));
            else
                return 0.5 * (Math.Sin(-13 * Math.PI / 2 * ((2 * p - 1) + 1)) * Math.Pow(2, -10 * (2 * p - 1)) + 2);
        }

        // Modeled after the overshooting cubic y = x^3-x*sin(x*pi)
        public static double BackEaseIn(double p)
        {
            return p * p * p - p * Math.Sin(p * Math.PI / 2);
        }

        // Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*sin((1-x)*pi))
        public static double BackEaseOut(double p)
        {
            double f = (1 - p);
            return 1 - (f * f * f - f * Math.Sin(f * Math.PI / 2));
        }

        // Modeled after the piecewise overshooting cubic function:
        // y = (1/2)*((2x)^3-(2x)*sin(2*x*pi))           ; [0, 0.5)
        // y = (1/2)*(1-((1-x)^3-(1-x)*sin((1-x)*pi))+1) ; [0.5, 1]
        public static double BackEaseInOut(double p)
        {
            if (p < 0.5)
            {
                double f = 2 * p;
                return 0.5 * (f * f * f - f * Math.Sin(f * Math.PI));
            }
            else
            {
                double f = (1 - (2 * p - 1));
                return 0.5 * (1 - (f * f * f - f * Math.Sin(f * Math.PI))) + 0.5;
            }
        }

        public static double BounceEaseIn(double p)
        {
            return 1 - BounceEaseOut(1 - p);
        }

        public static double BounceEaseOut(double p)
        {
            if (p < 4 / 11.0)
                return (121 * p * p) / 16.0;
            else if (p < 8 / 11.0)
                return (363 / 40.0 * p * p) - (99 / 10.0 * p) + 17 / 5.0;
            else if (p < 9 / 10.0)
                return (4356 / 361.0 * p * p) - (35442 / 1805.0 * p) + 16061 / 1805.0;
            else
                return (54 / 5.0 * p * p) - (513 / 25.0 * p) + 268 / 25.0;
        }

        public static double BounceEaseInOut(double p)
        {
            if (p < 0.5)
                return 0.5 * BounceEaseIn(p * 2);
            else
                return 0.5 * BounceEaseOut(p * 2 - 1) + 0.5;
        }
    }
}
