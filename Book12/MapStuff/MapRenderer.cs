﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Book12.GameData;
using Engine;
using Engine.Language;
using Engine.Language.Examples;
using Engine.LanguageB;
using Engine.Locations;



namespace Book12.MapStuff
{
    public class MapRenderer
    {
        DnC_Screen dnC_scrn;
        //Map Perlin Settings
        public static float[] mapESet = new float[] { 10, 0.0036F, 1.5F, 2400, 10 };
        public static float[] mapMSet = new float[] { 10, 0.005F, 1.7F, 0, 100 };
        public static float[] mapEVal = new float[] { 0.4F, 0.42F, 0.8F, 0.7F, 0.6F };
        public static float[] mapMVal = new float[] { 0.1F, 0.2F, 0.5F, 0.33F, 0.66F, 0.16F, 0.50F, 0.83F, 0.16F, 0.33F, 0.66F };

        //Map Settings
        public static int mapX_Max = 1300;
        public static int mapY_Max = 700;
        public static Size size = new Size(mapX_Max, mapY_Max);

        public static int cityDotBorder = 9;
        public static int cityDotInner = 7;

        public static int cityExclusion = 50;
        public static int cityExclusionflux = 30;

        Random rngI = Randomer.Instance;
        public static Tuple<int, int> randomCords(int xbuffer, int ybuffer)
        {
            Random rnd = new Random();
            int randomX = Randomer.Instance.Next(xbuffer, mapX_Max - xbuffer);
            int randomY = Randomer.Instance.Next(ybuffer, mapY_Max - ybuffer);
            return Tuple.Create(randomX, randomY);

        }
        public void RenderMapInitial()
        {
            Bitmap bmp_Map = new Bitmap(size.Width, size.Height);
            Bitmap bmp_is_Land = new Bitmap(size.Width, size.Height);
            for (int x = 0; x < mapX_Max; x++)
            {
                for (int y = 0; y < mapY_Max; y++)
                {
                    float CalcE = (float)SimplexNoise.GenerateO(x, y, (int)mapESet[0], mapESet[1], mapESet[2], mapESet[3], mapESet[4]);
                    float CalcM = (float)SimplexNoise.GenerateO(x, y, (int)mapMSet[0], mapMSet[1], mapMSet[2], mapMSet[3], mapMSet[4]);

                    // shading
                    if (CalcE < mapEVal[0])
                    {
                        //Ocean
                        bmp_Map.SetPixel(x, y, Color.FromArgb(67, 67, 122));
                    }
                    else
                    {
                        bmp_is_Land.SetPixel(x, y, Color.FromArgb(34, 139, 34));
                        if (CalcE < mapEVal[1])
                        {
                            //Beach
                            bmp_Map.SetPixel(x, y, Color.FromArgb(160, 145, 119));
                        }
                        else if (CalcE > mapEVal[2])
                        {
                            if (CalcM < mapMVal[0])
                            {
                                //return SCORCHED;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(85, 85, 85));
                            }
                            else if (CalcM < mapMVal[1])
                            {
                                //return BARE;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(136, 136, 136));
                            }
                            else if (CalcM < mapMVal[2])
                            {
                                //return TUNDRA;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(188, 188, 171));
                            }
                            else
                            {
                                //return SNOW;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(221, 221, 228));
                            }
                        }
                        else if (CalcE > mapEVal[3])
                        {
                            if (CalcM < mapMVal[3])
                            {
                                //return TEMPERATE_DESERT;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(201, 210, 155));
                            }
                            else if (CalcM < mapMVal[4])
                            {
                                // RETURN SHRUBLAND;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(136, 153, 119));
                            }
                            else
                            {
                                // RETURN TAIGA;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(153, 171, 119));
                            }
                        }
                        else if (CalcE > mapEVal[4])
                        {
                            if (CalcM < mapMVal[5])
                            {
                                // RETURN TEMPERATE_DESERT;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(201, 210, 155));
                            }
                            else if (CalcM < mapMVal[6])
                            {
                                // RETURN GRASSLAND;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(136, 171, 86));
                            }
                            else if (CalcM < mapMVal[7])
                            {
                                // RETURN TEMPERATE_DECIDUOUS_FOREST;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(103, 147, 89));
                            }
                            else
                            {
                                // RETURN TEMPERATE_RAIN_FOREST;
                                //bmp_Map.SetPixel(x, y, Color.FromArgb(201, 210, 155);
                                bmp_Map.SetPixel(x, y, Color.FromArgb(67, 136, 85));
                            }
                        }
                        else
                        {
                            if (CalcM < mapMVal[8])
                            {
                                // RETURN SUBTROPICAL_DESERT;
                                //bmp_Map.SetPixel(x, y, Color.FromArgb(136, 171, 86);
                                bmp_Map.SetPixel(x, y, Color.FromArgb(210, 186, 139));
                            }
                            else if (CalcM < mapMVal[9])
                            {
                                // RETURN GRASSLAND;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(136, 171, 85));
                            }

                            else if (CalcM < mapMVal[10])
                            {
                                // RETURN TROPICAL_SEASONAL_FOREST;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(86, 153, 68));
                            }
                            else
                            {
                                // RETURN TROPICAL_RAIN_FOREST;
                                bmp_Map.SetPixel(x, y, Color.FromArgb(51, 119, 65));
                            }
                        }
                    }

                }
            }
            World.map_Dict["Map"] = bmp_Map;
            World.map_Dict["is_Land_Map"] = bmp_is_Land;
        }
        public Bitmap AddDot(int x, int y, Color dotColor, int dotRad, Bitmap bmp_map)
        {
            using (Graphics graphics = Graphics.FromImage(bmp_map))
            {
                // Set the dot coordinates
                int dotX = x - dotRad; // X-coordinate
                int dotY = y - dotRad; // Y-coordinate

                // Draw the dot on the bitmap
                using (SolidBrush brush = new SolidBrush(dotColor))
                {
                    graphics.FillEllipse(brush, dotX, dotY, dotRad * 2, dotRad * 2);
                }
            }
            return bmp_map;
        }

        public static Bitmap CopyBitmap(Bitmap sourceBitmap)
        {
            if (sourceBitmap == null)
                return null;
            Bitmap copiedBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            using (Graphics g = Graphics.FromImage(copiedBitmap))
            {
                g.DrawImage(sourceBitmap, 0, 0, sourceBitmap.Width, sourceBitmap.Height);
            }
            return copiedBitmap;
        }
        public void RenderCities()
        {
            //Placeholder Language
            LanguageGenerator lg = new LanguageGenerator();
            //Found in PLGL.Examples. Set this to your own language (derived from Language).
            Qen qen = new Qen();
            lg.Language = qen;

            int citiescount = 50;
            int generatedCities = 0;
            int maxAttempts = 1000; // Set a maximum number of attempts to generate a city

            Bitmap bmp_Cities = CopyBitmap(World.map_Dict["Map"]);
            World.map_Dict["cities_Map"] = bmp_Cities;
            World.map_Dict["cities_Ex_Map"] = CopyBitmap(World.map_Dict["is_Land_Map"]);
            Bitmap cities_Ex_Map = World.map_Dict["cities_Ex_Map"];

            while (generatedCities < citiescount)
            {
                if (maxAttempts <= 0)
                {
                    // Break the loop if we can't generate more cities
                    break;
                }
                var coords = randomCords(cityDotBorder, cityDotBorder);
                int x = coords.Item1;
                int y = coords.Item2;
                Color pixelColor = cities_Ex_Map.GetPixel(x, y);
                if (pixelColor == Color.FromArgb(34, 139, 34))
                {
                    NL_Settlement nL_Settlement = new NL_Settlement(World.locationIndex, x, y, lg.GenerateClean(LanguageReferences.realCityNames[generatedCities]));

                    World.w_settlements.Add(World.locationIndex, nL_Settlement);
                    World.locationIndex++;
                    AddDot(x, y, Color.Gray, cityDotBorder, bmp_Cities);
                    AddDot(x, y, Color.Yellow, cityDotInner, bmp_Cities);
                    AddDot(x, y, Color.Black, cityExclusion + rngI.Next(cityExclusionflux), cities_Ex_Map);
                    generatedCities++;
                }
                maxAttempts--;
            }
            if (maxAttempts <= 0)
            {
                MessageBox.Show("Unable to place all cities. Some areas may be too crowded.");
            }
        }

        public static class FloodFill
        {
            public static void ColorRegions(Bitmap bitmap, List<Point> predeterminedPoints)
            {
                if (bitmap == null)
                {
                    throw new ArgumentNullException(nameof(bitmap));
                }

                // Create a dictionary to store the color assigned to each region
                Dictionary<Point, Color> regionColors = new Dictionary<Point, Color>();

                // Used to check if a pixel has been visited
                bool[,] visited = new bool[bitmap.Width, bitmap.Height];

                // Assign a unique color to each region
                foreach (var seedPoint in predeterminedPoints)
                {
                    if (!visited[seedPoint.X, seedPoint.Y])
                    {
                        Color fillColor = GetRandomColor();
                        FloodFillRecursive(bitmap, seedPoint, fillColor, visited, regionColors);
                    }
                }
            }

            private static Color GetRandomColor()
            {
                Random rand = new Random();
                return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            }

            private static void FloodFillRecursive(Bitmap bitmap, Point seed, Color fillColor, bool[,] visited, Dictionary<Point, Color> regionColors)
            {
                Stack<Point> stack = new Stack<Point>();
                stack.Push(seed);

                while (stack.Count > 0)
                {
                    Point current = stack.Pop();

                    if (visited[current.X, current.Y])
                    {
                        continue;
                    }

                    visited[current.X, current.Y] = true;
                    bitmap.SetPixel(current.X, current.Y, fillColor);
                    regionColors[current] = fillColor;

                    // Check neighboring pixels
                    CheckAndPush(stack, bitmap, current.X - 1, current.Y, visited);
                    CheckAndPush(stack, bitmap, current.X + 1, current.Y, visited);
                    CheckAndPush(stack, bitmap, current.X, current.Y - 1, visited);
                    CheckAndPush(stack, bitmap, current.X, current.Y + 1, visited);
                }
            }

            private static void CheckAndPush(Stack<Point> stack, Bitmap bitmap, int x, int y, bool[,] visited)
            {
                if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height && !visited[x, y])
                {
                    stack.Push(new Point(x, y));
                }
            }
        }
    }
}
