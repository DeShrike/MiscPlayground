namespace Fire
{
    using Microsoft.GotDotNet;
    using System;

    public class Fire
    {
        private byte[][] world;
        private byte[][] buffer;

        public Fire()
        {
            this.world = new byte[Console.WindowHeight][];
            this.buffer = new byte[Console.WindowHeight][];
            for (var i = 0; i < this.world.Length; i++)
            {
                this.world[i] = new byte[Console.WindowWidth];
                this.buffer[i] = new byte[Console.WindowWidth];
            }
        }

        public void AddFuel()
        {
            var r = new System.Random();
            var bottom = this.buffer[this.buffer.Length - 1];
            for (var i = 0; i < bottom.Length; i++)
            {
                if (r.NextDouble() < 0.4)
                {
                    bottom[i] = 250;
                }
                else
                {
                    bottom[i] = 0;
                }
            }
        }

        public void Burn()
        {
            for (int y = this.world.Length - 1; y > 1; y--)
            {
                var b = this.buffer[y - 1];
                var l0 = this.world[y];
                var l1 = this.world[y - 1];
                for (int x = 1; x < l0.Length - 1; x++)
                {
                    int c = (l0[x] + l0[x + 1] + l1[x] + l0[x - 1]) / 4;
                    b[x] = (byte)c;
                }
            }

            var t = this.buffer;
            this.buffer = this.world;
            this.world = t;
        }

        public void Draw()
        {
            for (int y = 0; y < this.world.Length-1; y++)
            {
                var l = this.world[y];
                ConsoleEx.CursorX = 0;
                ConsoleEx.CursorY = (short)y;
                for (int x = 0; x < l.Length; x++)
                {
                    this.DrawPixel(l[x]);
                }
            }
        }

        private void DrawPixel(byte pixel)
        {
            var col = ConsoleForeground.Black;
            if (pixel > 150)
            {
                col = ConsoleForeground.White;
            }
            else if (pixel > 130)
            {
                col = ConsoleForeground.Yellow;
            }
            else if (pixel > 100)
            {
                col = ConsoleForeground.Red;
            }
            else if (pixel > 80)
            {
                col = ConsoleForeground.LightGray;
            }

            ConsoleEx.TextColor(col, ConsoleBackground.Black);
            Console.Write('O');
        }
    }
}
