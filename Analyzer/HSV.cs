using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    public  class HSV
    {
        public float hue = 0;
        public float saturation = 0;
        public float value = 0;
        public float result = 0;

        public HSV()
        {

        }
        public float GetResult()
        {
            result = hue + saturation + value;
            return result;
        }
    }
}
