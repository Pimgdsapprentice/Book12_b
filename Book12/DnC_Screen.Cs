﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using Engine.Locations;
using Engine.Language;
using Engine.Language.Examples;
using Engine;
using System.Reflection;
using Book12.GameData;
using DelaunayVoronoi;
using Book12.MapStuff;

namespace Book12
{
    public partial class DnC_Screen : Form
    {
        //References the first instance of Main Screen
        public MainScreen main = Application.OpenForms.OfType<MainScreen>().FirstOrDefault();
        public static int Scale2 = 100;
        public DnC_Screen(MainScreen main)
        {
            InitializeComponent();
            this.main = main;

            //Creates a World
            World.World_Name = "New World";
            World.locationIndex = 0;
            World.w_settlements = new Dictionary<int, NL_Settlement> { };
            World.seed = 123;
            World.World_Scale = 200;

            Randomer.SetSeed(World.seed);


            //Prepares Map
            MapRenderer mapper = new MapRenderer();
            mapper.RenderMapInitial();
            mapper.RenderCities();
            
            //DL gen
            DVPrinter dvprint = new DVPrinter();
            dvprint.Intialize();
            //dvprint.GenerateAndDraw();
            var centerPoints = World.w_settlements.Values.Select(settlement => new DVPoint(settlement.X_Cord, settlement.Y_Cord));
            dvprint.GenerateAndDrawWithCenters(centerPoints);


            
            
            //Gets the Locations of the Cities
            List<Point> points = new List<Point>();
            foreach (var settlement in World.w_settlements.Values)
            {
                points.Add(new Point(settlement.X_Cord, settlement.Y_Cord));
            }
            Bitmap bitmap4 = MapRenderer.CopyBitmap(World.map_Dict["DVMapWithCenters"]);
            int borderWidth = 2;
            Color borderColor = Color.DarkViolet;
            Bitmap borderedBitmap = DrawBorder(bitmap4, borderWidth, borderColor);

            World.map_Dict["t2"] = borderedBitmap;

            foreach (var seedPoint in points)
            {
                FloodFill(World.map_Dict["t2"], seedPoint, Color.Black, GenerateColor2());
            }
            /*
            Bitmap bitmap4 = MapRenderer.CopyBitmap(World.map_Dict["DVMapWithCenters"]);
            int borderWidth = 2;
            Color borderColor = Color.DarkViolet;
            Bitmap borderedBitmap = DrawBorder(bitmap4, borderWidth, borderColor);
            World.map_Dict["t2"] = bitmap4;
            foreach (var seedPoint in points)
            {
                FloodFill(World.map_Dict["t2"], seedPoint, Color.Black, GenerateColor2());
            }
            */
            MenuList[1] = new MenuControls(1, "Map Menu", ":Main Menu/Map Selection", World.map_Dict.Keys.ToArray());

            //Ensures Autoscroll
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
            richTextBox1.HideSelection = false;

            //Menu();
            MenuOut();
        }


        public static Color GenerateColor2()
        {
            int red = Randomer.Instance.Next(0, 256);
            int green = Randomer.Instance.Next(0, 256);
            int blue = Randomer.Instance.Next(0, 256);

            // Clamp values to valid RGB range (0-255)
            red = Math.Min(255, Math.Max(0, red));
            green = Math.Min(255, Math.Max(0, green));
            blue = Math.Min(255, Math.Max(0, blue));

            return Color.FromArgb(red, green, blue);
        }

        public static Bitmap DrawBorder(Bitmap originalBitmap, int borderWidth, Color borderColor)
        {
            Bitmap borderedBitmap = new Bitmap(originalBitmap);

            using (Graphics g = Graphics.FromImage(borderedBitmap))
            {
                using (Pen borderPen = new Pen(borderColor, borderWidth))
                {
                    g.DrawRectangle(borderPen, new Rectangle(0, 0, borderedBitmap.Width, borderedBitmap.Height));
                }
            }

            return borderedBitmap;
        }

        /*
        public static Bitmap DrawBorder(Bitmap originalBitmap, int borderWidth, Color borderColor)
        {
            // Create a new bitmap with the desired size
            Bitmap borderedBitmap = new Bitmap(originalBitmap.Width + 2 * borderWidth, originalBitmap.Height + 2 * borderWidth);

            // Draw the original bitmap onto the new bitmap
            using (Graphics g = Graphics.FromImage(borderedBitmap))
            {
                g.Clear(borderColor);
                g.DrawImage(originalBitmap, new Point(borderWidth, borderWidth));
            }

            // Draw the border lines
            using (Graphics g = Graphics.FromImage(borderedBitmap))
            {
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawRectangle(pen, 0, 0, borderedBitmap.Width - 1, borderedBitmap.Height - 1);
                }
            }

            return borderedBitmap;
        }*/
        public static Color GenerateColor(int x, int y, int scale)
        {
            // Map coordinates to RGB color
            double angle = Math.Atan2(y, x);
            double distance = Math.Sqrt(x * x + y * y);

            int red = (int)(Math.Abs(Math.Cos(angle) * scale) + Randomer.Instance.Next(50, 100));
            int green = (int)(Math.Abs(Math.Sin(angle) * scale) + Randomer.Instance.Next(50, 100));
            int blue = (int)((distance / scale + Math.Abs(Math.Cos(angle))) * 255);

            // Clamp values to valid RGB range (0-255)
            red = Math.Min(255, Math.Max(0, red));
            green = Math.Min(255, Math.Max(0, green));
            blue = Math.Min(255, Math.Max(0, blue));

            return Color.FromArgb(red, green, blue);
        }



        //Menu Section
        string crntMenuDir = ":Main Menu";
        List<MenuControls> MenuList = new List<MenuControls>
        {
            new MenuControls(0, "Main Menu", ":Main Menu", "Map Selection"),
            new MenuControls(1, "Map Menu", ":Main Menu/Map Selection", World.map_Dict.Keys.ToArray())
        };
        public MenuControls menuAtAddrs(string targetdirectory)
        {
            return MenuList.SingleOrDefault(menu => menu.address == targetdirectory);
        }
        private void MenuOut()
        {
            int index = 0;
            nOut(menuAtAddrs(crntMenuDir).title + ":");
            nOut("---");
            foreach (string i in menuAtAddrs(crntMenuDir).submenus)
            {
                nOut($"{index++}: {i}");
            }
            nOut("e: Exit to Main Menu");
            nOut("x-----");
        }

        private static Bitmap FloodFillRegions(Bitmap voronoiBitmap)
        {
            Dictionary<Point, Color> coloredRegions = new Dictionary<Point, Color>();

            // Implement or use a flood fill algorithm to fill the Voronoi regions with different colors
            // For simplicity, let's assume you have a function FloodFill(bitmap, startPoint, targetColor, replacementColor) that fills a region.

            // Placeholder function (replace with actual implementation)
            for (int x = 0; x < voronoiBitmap.Width; x++)
            {
                for (int y = 0; y < voronoiBitmap.Height; y++)
                {
                    Point startPoint = new Point(x, y);
                    Color targetColor = voronoiBitmap.GetPixel(x, y);
                    Color replacementColor = GetUniqueColor(coloredRegions);

                    FloodFill(voronoiBitmap, startPoint, targetColor, replacementColor);
                    coloredRegions[startPoint] = replacementColor;
                }
            }

            return voronoiBitmap;
        }
        public static Color GenerateRandomColor(int x, int y, int scale)
        {
            // Modulate RGB values based on x, y coordinates
            double angleX = Math.Sin(x / (double)scale);
            double angleY = Math.Cos(y / (double)scale);

            int red = (int)((Math.Sin(angleX) + 1) * 127.5);
            int green = (int)((Math.Cos(angleY) + 1) * 127.5);
            int blue = (int)((Math.Sin(angleX + angleY) + 1) * 127.5);

            // Clamp values to valid RGB range (0-255)
            red = Math.Min(255, Math.Max(0, red));
            green = Math.Min(255, Math.Max(0, green));
            blue = Math.Min(255, Math.Max(0, blue));

            return Color.FromArgb(red, green, blue);
        }


        private static Color GetUniqueColor(Dictionary<Point, Color> coloredRegions)
        {
            // Generate a unique color for a region
            // You can implement this based on your requirements
            // For simplicity, let's use a random color here.

            Random random = new Random();
            Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

            // Ensure the generated color is unique
            while (coloredRegions.ContainsValue(randomColor))
            {
                randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            }

            return randomColor;
        }

        private static void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Stack<Point> pixels = new Stack<Point>();
            targetColor = bmp.GetPixel(pt.X, pt.Y);
            pixels.Push(pt);

            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();
                if (a.X < bmp.Width && a.X >= 0 &&
                        a.Y < bmp.Height && a.Y >= 0) // Make sure we stay within bounds
                {
                    if (bmp.GetPixel(a.X, a.Y) == targetColor)
                    {
                        bmp.SetPixel(a.X, a.Y, replacementColor);
                        pixels.Push(new Point(a.X - 1, a.Y));
                        pixels.Push(new Point(a.X + 1, a.Y));
                        pixels.Push(new Point(a.X, a.Y - 1));
                        pixels.Push(new Point(a.X, a.Y + 1));
                    }
                }
            }
        }

        private void HandleMenuInput()
        {
            //Try to get int from menu input
            int menuSel = -1;
            try
            {
                menuSel = int.Parse(textBox1.Text);
            }
            catch (FormatException)
            {
                menuSel = -1;
            }
            catch (OverflowException)
            {
                menuSel = -1;
            }
            //Checks if Exit
            if (textBox1.Text == "e")
            {
                crntMenuDir = ":Main Menu";
                MenuOut();
            }
            else if (textBox1.Text == "t")
            {
                MenuOut();
            }
            else if (crntMenuDir == ":Main Menu/Map Selection")
            {
                if (menuSel >= 0 && menuSel < menuAtAddrs(crntMenuDir).submenus.Count)
                {
                    main.ChangePictureBoxImage(World.map_Dict[World.map_Dict.Keys.ToArray()[menuSel]]);
                    MenuOut();
                }
                else
                {
                    nOut("Invalid Map choice");
                    nOut("x-----");
                }
            }
            else if (menuSel >= 0 && menuSel < menuAtAddrs(crntMenuDir).submenus.Count)
            {

                crntMenuDir = crntMenuDir + "/" + menuAtAddrs(crntMenuDir).submenus[menuSel];
                MenuOut();
            }
            else
            {
                nOut("Invalid Input");
            }
        }
        //Uses Enter For Menu Entry
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is NumPad Enter (ASCII code 13) and the focused control is textBox1
            if (e.KeyChar == (char)Keys.Enter && this.ActiveControl == textBox1)
            {
                // Perform the same function as the button click
                HandleMenuInput();
                // Prevent the Enter key from being processed by the TextBox
                e.Handled = true;
                textBox1.Text = string.Empty;
            }
        }
        //Prevents closing of form
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void objOut(object obj)
        {
            nOut($"Object Name: {nameof(obj)}");

            Type objectType = obj.GetType();
            FieldInfo[] fields = objectType.GetFields();
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(obj);
                nOut($"{field.Name}: {value}");
            }
            // Get the properties of the object using reflection
            PropertyInfo[] properties = obj.GetType().GetProperties();
            // Loop through properties and print their names and values
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);
                nOut($"{property.Name}: {value}");
            }
        }

        public void nOut(string outLine)
        {
            richTextBox1.AppendText("\n" + outLine);
        }
        public void inva()
        {
            nOut("Invalid choice.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HandleMenuInput();
        }

        //Relatively Unused
        private void button2_Click(object sender, EventArgs e)
        {
            Action2();
        }

        private void Action2()
        {
            MessageBox.Show("Bye!");
        }
    }
}
