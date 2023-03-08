using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer
{
    public class Matrix
    {
        public HSV[,] mtrx;
        int index1 ;
        int index2 ;        

        public Matrix(int _i1,int _i2)
        {
            index1 = _i1;
            index2 = _i2;
            mtrx = new HSV[index1, index2];            
                                    
            Define();
        }
        public void Define()
        {
            for (int d1 = 0; d1 < index1; d1++)
            {
                for (int d2 = 0; d2 < index2; d2++)
                {
                    mtrx[d1, d2] = new HSV();
                }
            }
        }

    }
}
