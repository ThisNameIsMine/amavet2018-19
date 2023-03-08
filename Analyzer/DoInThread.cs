using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;


namespace Analyzer
{
    public class DoInThread
    {

        #region  

        public Bitmap bitmap;
        public Bitmap dotResult;
        public Bitmap[,] list;        
        public Matrix[,] lMatrix;//uses avg HSV (box)
        private List<Point> pointLess;
        int box = 2;//5 
        float koef = 0.051f;//0.015f
        
        private bool MapRewriteEnabled = false;

        public bool isOn1 = true;// --- 
        public bool isOn2 = true;
        public bool isOn3 = true;
        public bool isOn4 = true;
        #endregion
        public DoInThread(Bitmap _bmp)//Bmp -> rozdelit az tu
        {
            lMatrix = new Matrix[2, 2];
            list = new Bitmap[2, 2];
            bitmap = _bmp;
            dotResult = _bmp;
            PrepareDotResult();
            PrepareBitmaps();
            //isOn1 = true;
            
            DefineMatrix();
            
            // 

        }

        public DoInThread(Bitmap[,] _list,Bitmap _bmp,Bitmap _bmp2)
        {
            lMatrix = new Matrix[2, 2];            
            list = _list;
            pointLess = new List<Point>();            

            bitmap = _bmp;

            dotResult = _bmp2;
            //PrepareDotResult();
            
            //isOn1 = true;

            DefineMatrix();
        }
        
        public void PrepareBitmaps()
        {
            Console.WriteLine("Zacina 1");
            list[0, 0] = bitmap.Clone(new Rectangle(0, 0, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);
            Console.WriteLine("Zacina 2");
            list[0, 1] = bitmap.Clone(new Rectangle(bitmap.Width / 2, 0, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);
            Console.WriteLine("Zacina 3");
            list[1, 0] = bitmap.Clone(new Rectangle(0, bitmap.Height / 2, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);
            Console.WriteLine("Zacina 4");
            list[1, 1] = bitmap.Clone(new Rectangle(bitmap.Width / 2, bitmap.Height / 2, bitmap.Width / 2, bitmap.Height / 2), bitmap.PixelFormat);
            Console.WriteLine("Done");
            //this.Update();

        }
        private void PrepareDotResult()
        {
            using (Graphics graphics = Graphics.FromImage(dotResult))
            {
                using (SolidBrush myBrush = new SolidBrush(Color.White))
                {
                    graphics.FillRectangle(myBrush, new Rectangle(0, 0, dotResult.Width, dotResult.Height)); // whatever
                                                                                                    // and so on...
                } // myBrush will be disposed at this line

            }
        }

        
        
        private void DefineMatrix()
        {
            int f = lMatrix.GetLength(0);
            for (int i1 = 0; i1 < f; i1++)
            {
                for(int i2 = 0; i2 < lMatrix.GetLength(1); i2++)
                {
                    lMatrix[i1, i2] = new Matrix(list[i1,i2].Width/box, list[i1,i2].Height/box);
                }
            }
        }

        private HSV GetAvg(Bitmap bmp,int i1,int i2,int _x,int _y)
        {
            HSV result = new HSV();
            HSV[] hsvList = new HSV[bmp.Width * bmp.Height];
            int cycle = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for(int y = 0; y < bmp.Height; y++)
                {
                    hsvList[x + y + cycle] = GetHSV(bmp.GetPixel(x, y));
                    hsvList[x + y + cycle].GetResult();

                    result.hue += hsvList[x + y + cycle].hue;
                    result.saturation += hsvList[x + y + cycle].saturation;
                    result.value += hsvList[x + y + cycle].value;
                }
                cycle+= x;//bmp.Width
            }
            result.hue /= (bmp.Width * bmp.Height);
            result.saturation /= (bmp.Width * bmp.Height);
            result.value /= (bmp.Width * bmp.Height);
            result.GetResult();

            lMatrix[i1, i2].mtrx[(_x - (_x % box)) / box, (_y - (_y % box)) / box] = result;

            return result;
        }
        private HSV GetBoxAvg(Matrix m,int _sx,int _sy)
        {
            HSV result = new HSV();

            for (int x = _sx; x < (_sx + box); x++)
            {
                for(int y = _sy;y < (_sy + box); y++)
                {
                    result.hue += m.mtrx[x, y].hue;
                    result.saturation += m.mtrx[x, y].saturation;
                    result.value += m.mtrx[x, y].value;
                }
            }

            result.hue /= box;
            result.saturation /= box;
            result.value /= box;
            result.GetResult();

            return result;
        }
        private HSV Delta(HSV clr1,HSV clr2)
        {
            HSV result = new HSV();

            result.hue = Math.Abs(clr1.hue - clr2.hue);
            result.saturation = Math.Abs(clr1.saturation - clr2.saturation);
            result.value = Math.Abs(clr1.value - clr2.value);
            result.GetResult();

            return result;
        }
        
        private void BoxObjectIdentify(Matrix matrix, int i1, int i2, int _sx, int _sy, List<Point> why)//GetBoxDifference
        {

            //HSV[] difference = DefineHelp(box * box);
            HSV avgColor = GetBoxAvg(lMatrix[i1, i2], _sx, _sy);
            int cycle = 0;
            float avgHSV = avgColor.GetResult();
            float result = 0;


            for (int x = _sx; x < (_sx + box); x++)
            {
                for (int y = _sy; y < (_sy + box); y++)
                {
                    result += Delta(matrix.mtrx[x, y], avgColor).GetResult();//matrix.mtrx[_sy + (box - (x - _sx)), _sx + (box - (y - _sy))
                    cycle++;
                }

            }

            result /= cycle;

            int adjx = 0;

            for (int x = _sx; x < (_sx + box); x++)
            {
                for (int y = _sy; y < (_sy + box); y++)
                {
                    if (Delta(matrix.mtrx[x, y], avgColor).GetResult() > result + (result * koef))
                    {
                        //if (x  > lMatrix[i1, i2].mtrx.GetLength(0)) x = x - (x - lMatrix[i1, i2].mtrx.GetLength(0));
                        if ((y + 1) > lMatrix[i1, i2].mtrx.GetLength(1)) y = y - ((y + 1) - lMatrix[i1, i2].mtrx.GetLength(1));
                        adjx = ((x + 1) - lMatrix[i1, i2].mtrx.GetLength(0));
                        if ((x + 1) > lMatrix[i1, i2].mtrx.GetLength(0)) x = x - adjx;
                        DrawBox(x, y, i1, i2, Color.Red, box, why);
                    }
                }
                if ((x + 1) > lMatrix[i1, i2].mtrx.GetLength(0)) x = x - ((x + 1) - lMatrix[i1, i2].mtrx.GetLength(0));

            }

                foreach (var item in why)
                {
                if (why.Any(x => x.X >= item.X - 1 && x.X <= item.X + 1 && x.Y >= item.Y - 1 && x.Y <= item.Y + 1 && !(x.X == item.X && x.Y == item.Y)))
                {
                    draws(item.X, item.Y, i1, i2);
                    //Console.WriteLine("fkl " + item + "fsfsdf" + why.First(x => x.X >= item.X - 1 && x.X <= item.X + 1 && x.Y >= item.Y - 1 && x.Y <= item.Y + 1));
                }
                }

            /*
             * Claculate again every pixel delta -> if delta > result - mark - > DONE
            */

            #region result
            /*
            resutl.hue /= cycle;
            resutl.saturation /= cycle;
            resutl.value /= cycle;
            resutl.GetResult();
            */
            #endregion

            //return result;
        }

        private void draws(int _sx, int _sy, int i1, int i2)
        {
            using (Graphics graphics = Graphics.FromImage(list[i1, i2]))
            {
                using (SolidBrush myBrush = new SolidBrush(Color.Red))
                {
                    graphics.FillRectangle(myBrush, new Rectangle(_sx * box, _sy * box, box, box)); // whatever
                                                                                                    // and so on...
                } // myBrush will be disposed at this line

            }
        }
        private int DrawBox(int _sx,int _sy ,int i1,int i2,Color clr,int size, List<Point> why)
        {

                why.Add(new Point(_sx, _sy));

            /*
            for (int x = _sx; x < _sx + size; x++)
            {
                for(int y = _sy;y < _sy + size; y++)  
                {
                    if ((_sx * box) + x >= list[i1, i2].Width) return 0; //(_sx * box)+x
                    if ((_sy * box) + y >= list[i1, i2].Height) return 0;//(_sy * box)
                    list[i1, i2].SetPixel((_sx * box)+x, (_sy * box)+y,clr);                     
                }
                
            }*/
            return 0;
        }
        /*
        private HSV[] DefineHelp(int size)
        {
            HSV[] result = new HSV[size];

            for (int i = 0; i < size; i++)
            {
                result[i] = new HSV();
            }

            return result;
        }
        */
        private Color GetRGB(HSV clr)
        {            
            int hi = Convert.ToInt32(Math.Floor(clr.hue / 60)) % 6;
            double f = clr.hue / 60 - Math.Floor(clr.hue / 60);

            clr.value = clr.value * 255;
            int v = Convert.ToInt32(clr.value);
            int p = Convert.ToInt32(clr.value * (1 - clr.saturation));
            int q = Convert.ToInt32(clr.value * (1 - f * clr.saturation));
            int t = Convert.ToInt32(clr.value * (1 - (1 - f) * clr.saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);            
        }

        private void SetBmp(int _sx,int _sy,int _ex,int _ey,HSV _clr,int _i1,int _i2)
        {
            Color clr = GetRGB(_clr);
            for(int x = _sx; x < _ex; x++)
            {
                for(int y = _sy; y < _ey; y++)
                {
                    list[_i1, _i2].SetPixel(x, y, clr);
                }
            }
        }

        private void RewriteBmp(int i1,int i2)
        {
            if (MapRewriteEnabled)
            {
                Bitmap NewBmp = new Bitmap(list[i1, i2].Width / box, list[i1, i2].Height / box);

                for (int x = 1; x < list[i1, i2].Width - box; x += box)
                {
                    for (int y = 1; y < list[i1, i2].Height - box; y += box)// list[i1, i2].Height/box
                    {
                        //NewBmp.SetPixel(x / box, y / box, list[i1, i2].GetPixel(x, y));
                        NewBmp.SetPixel(x/box, y/box, list[i1, i2].GetPixel(x, y));
                    }
                }
                list[i1, i2] = NewBmp;
                NewBmp.Save("new.jpg");
            }
        }

        private void ObjectDetection(int index1,int index2)
        {
            // Matrix matrix,int i1,int i2,int _sx,int _sy
            #region Total - of All items in List
            /*
            for (int index1 = 0; index1 < 2; index1++)
            {
                for(int index2 = 0;index2 < 2; index2++)
                {

                    for (int x = 0; x < lMatrix[index1, index2].mtrx.GetLength(0) - box; x += box)
                    {
                        for (int y = 0; y < lMatrix[index1, index2].mtrx.GetLength(1) - box; y += box)
                        {
                            BoxObjectIdentify(lMatrix[index1, index2], index1, index2, x, y);
                        }
                    }
                }
            }
            */
            #endregion
            int f = lMatrix[index1, index2].mtrx.GetLength(0);

            for (int x = 0; x < f  - (box); x += box)
            {
                for (int y = 0; y < lMatrix[index1, index2].mtrx.GetLength(1) - (box); y += box)
                {
                    List<Point> why = new List<Point>();
                    BoxObjectIdentify(lMatrix[index1, index2], index1, index2, x, y, why);
                    if (y + box > lMatrix[index1, index2].mtrx.GetLength(1)) y = y - ((y + box) - lMatrix[index1, index2].mtrx.GetLength(1));
                }
                

                if (x + box > lMatrix[index1, index2].mtrx.GetLength(0)) x = x - ((x + box) - lMatrix[index1, index2].mtrx.GetLength(0));
            }
            


        }

        private void FindObject()
        {

        }
        public void CreateFullImage()
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {

                graphics.DrawImage(list[0, 0], 0, 0);
                graphics.DrawImage(list[0, 1], list[0, 0].Width, 0);
                graphics.DrawImage(list[1, 0], 0, list[0, 0].Height);
                graphics.DrawImage(list[1, 1], list[1, 0].Width, list[0, 1].Height);
                bitmap.Save("Hope.jpg");

            }
        }
        public void CreateResultImage()
        {
            for(int i = 0; i < pointLess.Count; i++)
            {
                using (Graphics graphics = Graphics.FromImage(dotResult))
                {
                    using (SolidBrush myBrush = new SolidBrush(Color.Black))
                    {
                        graphics.FillRectangle(myBrush, new Rectangle(pointLess[i].X, pointLess[i].Y, box, box)); // whatever
                                                                                                                  // and so on...
                    } // myBrush will be disposed at this line

                }
            }
            dotResult.Save("RESULT.jpg");
        }


        public void DOStuff1()//Thread1
        {
            int i1 = 0;//list index 1
            int i2 = 0;//list index 2
            int fromX = 0;//0
            int toX = list[i1,i2].Width;
            int fromY = 0;//0
            int toY = list[i1,i2].Height;
            Rectangle rect;
            Bitmap clone;            

            for (int x = fromX; x < toX - box; x += box)
            {
                for(int y = fromY; y < toY - box; y += box)
                {
                    rect = new Rectangle(x, y, box, box);
                    clone = list[i1, i2].Clone(rect, list[i1, i2].PixelFormat);
                    SetBmp(x, y, x + box, y + box, GetAvg(clone,0,0,x,y), i1, i2);
                    clone.Dispose();
                    
                }
            }
            //list[0, 0].Save("pre-result1.jpg");
            //RewriteBmp(0,0);
            ObjectDetection(i1,i2);
            
            list[0, 0].Save("result1.jpg");
            //list[0, 1].Save("result2.jpg");
            

            isOn1 = false;

        }
        
        public void DOStuff2()
        {
            int i1 = 0;//list index 1
            int i2 = 1;//list index 2
            int fromX = 0;//0
            int toX = list[i1, i2].Width;
            int fromY = 0;//0
            int toY = list[i1, i2].Height;
            Rectangle rect;
            Bitmap clone;
            

            for (int x = fromX; x < toX - box; x += box)
            {
                for (int y = fromY; y < toY - box; y += box)
                {
                    rect = new Rectangle(x, y, box, box);
                    clone = list[i1, i2].Clone(rect, list[i1, i2].PixelFormat);
                    SetBmp(x, y, x + box, y + box, GetAvg(clone,0,1,x,y), i1, i2);
                    clone.Dispose();
                    
                    /*
                    Console.WriteLine("DOSstuff1");
                    Console.WriteLine("x: " + x);
                    Console.WriteLine("y: " + y);
                    Console.WriteLine();
                    */
                }
            }
            //RewriteBmp(0, 1);
            ObjectDetection(0, 1);
            list[0, 1].Save("result2.jpg");
            /*
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {                                
                graphics.DrawImage(list[0, 1], list[0, 0].Width, 0);                
            }
            */
            isOn2 = false;

        }
        public void DOStuff3()
        {
            int i1 = 1;//list index 1
            int i2 = 0;//list index 2
            int fromX = 0;//0
            int toX = list[i1, i2].Width;
            int fromY = 0;//0
            int toY = list[i1, i2].Height;
            Rectangle rect;
            Bitmap clone;
            

            for (int x = fromX; x < toX - box; x += box)
            {
                for (int y = fromY; y < toY - box; y += box)
                {
                    rect = new Rectangle(x, y, box, box);
                    clone = list[i1, i2].Clone(rect, list[i1, i2].PixelFormat);
                    SetBmp(x, y, x + box, y + box, GetAvg(clone,1,0,x,y), i1, i2);
                    clone.Dispose();                    
                }
            }            
            ObjectDetection(1, 0);
            list[1, 0].Save("result3.jpg");
            /*
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {                
                graphics.DrawImage(list[1, 0], 0, list[0, 0].Height);                                
            }
            */
            isOn3 = false;

        }
        public void DOStuff4()
        {
            int i1 = 1;//list index 1
            int i2 = 1;//list index 2
            int fromX = 0;//0
            int toX = list[i1, i2].Width;
            int fromY = 0;//0
            int toY = list[i1, i2].Height;
            Rectangle rect;
            Bitmap clone;
            

            for (int x = fromX; x < toX - box; x += box)
            {
                for (int y = fromY; y < toY - box; y += box)
                {
                    rect = new Rectangle(x, y, box, box);
                    clone = list[i1, i2].Clone(rect, list[i1, i2].PixelFormat);
                    SetBmp(x, y, x + box, y + box, GetAvg(clone,1,1,x,y), i1, i2);
                    clone.Dispose();
                    /*
                    Console.WriteLine("DOSstuff1");
                    Console.WriteLine("x: " + x);
                    Console.WriteLine("y: " + y);

                    Console.WriteLine();
                    */
                }
            }
            //RewriteBmp(1, 1);
            ObjectDetection(1, 1);
            list[1, 1].Save("result4.jpg");
            /*
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {                                                                
                graphics.DrawImage(list[1, 1], list[1, 0].Width, list[0, 1].Height);                
            }
            */
            isOn4 = false;

        }
        

        public static HSV GetHSV(Color color)
        {
            
            HSV result = new HSV();

            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            result.hue = color.GetHue();
            result.saturation = (max == 0) ? 0 : 1f - (1f * min / max);
            result.value = max / 255f;                        
            return result;
        }
    }
}
