using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace KickstartPalPatcherGui
{
    public class AboutForm : Form
    {
        private readonly Timer _timer;
        private Image _logo;

        // --- Ticker ---
        private float _textX;
        private string _scrollText =
            "Hello and Welcome to Cathal's Kickstart Pal Patcher. This peace of software was created because some members of the A1K.org forum, led by QMadx64, were looking for a way to start their Amigas with ECS NTSC Agnus in Pal mode. It is important to note that all these patches only work if the Amiga has an Agnus chip that can be switched. This usually applies to all 8372A/AB and 8375 Agnus chips, as well as Alice in AGA Amigas, of course. Special Thanks to the A1K.org Users A10001986 and DingensCGN for the initial Patch Data with whose help I was able to create the Autopatcher";

        private readonly Font _font =
            new Font("Segoe UI", 48f, FontStyle.Bold, GraphicsUnit.Pixel);

        private readonly int _tickerSpeed = 2;

        // --- Bouncy ticker vertical motion ---
        private float _tickerY;
        private float _tickerVy;

        // --- Endless bouncy ticker vertical motion (no energy loss) ---
        private float _bouncePhase;                 // 0..2π
        private float _bouncePhase01;                 // 0..1 repeating
        private const float BounceSpeed = 0.012f;     // cycles per tick (tune this)
        // last measured height (to compute floor)
        private float _tickerTextHeight = 48f;

        // margins
        private const float TickerFloorMargin = 10f;

        // --- Ball physics ---
        private float _ballX;
        private float _ballY;
        private float _ballVx = 3.2f;
        private float _ballVy = 2.4f;
        private int _ballRadius = 70;

        // Rolling angle (radians), tied to motion
        private float _rollAngle;

        // Ball render buffer (reused, no per-frame allocations)
        private Bitmap _ballBitmap;
        private int _ballBitmapRadius;

        public AboutForm()
        {
            Text = "Cathal's Kickstart Pal Patcher 2 v"+ Application.ProductVersion.ToString();
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;

            DoubleBuffered = true;
            ClientSize = new Size(900, 520);

            // Load background image (logo.png)
            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
            if (File.Exists(logoPath))
            {
                using (var tmp = Image.FromFile(logoPath))
                    _logo = new Bitmap(tmp);
            }

            _textX = ClientSize.Width;
            // Start at "center" (top point of the bounce)
            _tickerY = (ClientSize.Height - _tickerTextHeight) / 2f;
            _tickerVy = 0f;

            // Ball start
            _ballX = ClientSize.Width * 0.5f;
            _ballY = ClientSize.Height * 0.55f;

            EnsureBallBitmap();

            _timer = new Timer();
            _timer.Interval = 10; // ~60 FPS
            _timer.Tick += (s, e) =>
            {
                StepBall();
                StepTicker();
                Invalidate();
            };
            _timer.Start();

            FormClosed += (s, e) =>
            {
                _timer.Stop();
                _timer.Dispose();
                if (_logo != null) _logo.Dispose();
                if (_ballBitmap != null) _ballBitmap.Dispose();
            };

            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    Close();
            };

            Resize += (s, e) =>
            {
                if (_textX > ClientSize.Width) _textX = ClientSize.Width;
                KeepBallInsideBounds();
            };
        }

        public string ScrollText
        {
            get { return _scrollText; }
            set
            {
                _scrollText = value ?? "";
                _textX = ClientSize.Width;
                Invalidate();
            }
        }
        private void StepTicker()
        {
            // Horizontal scroll (wie vorher)
            _textX -= _tickerSpeed;

            // Phase 0..1 (endless)
            _bouncePhase01 += BounceSpeed;
            if (_bouncePhase01 >= 1f) _bouncePhase01 -= 1f;

            // Bounds: top = middle, bottom = floor
            float topY = (ClientSize.Height - _tickerTextHeight) / 2f;
            float bottomY = ClientSize.Height - _tickerTextHeight - TickerFloorMargin;
            if (bottomY < topY) bottomY = topY;

            // Piecewise ballistic bounce:
            // First half: falling (slow at top, faster down)  y = u^2
            // Second half: rising (fast up, slow near top)    y = (1-u)^2
            float t = _bouncePhase01;

            float s; // normalized 0..1 (0=top, 1=bottom)
            if (t < 0.5f)
            {
                float u = t / 0.5f;   // 0..1
                s = u * u;            // accelerate down
            }
            else
            {
                float u = (t - 0.5f) / 0.5f; // 0..1
                float inv = 1f - u;
                s = inv * inv;              // decelerate to top
            }

            _tickerY = topY + (bottomY - topY) * s;
        }

        private void StepBall()
        {
            _ballX += _ballVx;
            _ballY += _ballVy;

            // Bounce on edges
            float left = _ballRadius;
            float right = ClientSize.Width - _ballRadius;
            float top = _ballRadius;
            float bottom = ClientSize.Height - _ballRadius;

            if (_ballX < left)
            {
                _ballX = left;
                _ballVx = -_ballVx;
            }
            else if (_ballX > right)
            {
                _ballX = right;
                _ballVx = -_ballVx;
            }

            if (_ballY < top)
            {
                _ballY = top;
                _ballVy = -_ballVy;
            }
            else if (_ballY > bottom)
            {
                _ballY = bottom;
                _ballVy = -_ballVy;
            }

            // Rolling rotation tied to horizontal travel:
            // dAngle = distance / radius  (radians)
            _rollAngle -= (_ballVx / (float)_ballRadius);

            // Update ball bitmap (pattern rotates smoothly and consistently)
            EnsureBallBitmap();
            RenderAmigaBallIntoBitmap(_ballBitmap, _ballRadius, _rollAngle);
        }

        private void KeepBallInsideBounds()
        {
            float left = _ballRadius;
            float right = ClientSize.Width - _ballRadius;
            float top = _ballRadius;
            float bottom = ClientSize.Height - _ballRadius;

            if (_ballX < left) _ballX = left;
            if (_ballX > right) _ballX = right;
            if (_ballY < top) _ballY = top;
            if (_ballY > bottom) _ballY = bottom;
        }

        private void EnsureBallBitmap()
        {
            if (_ballBitmap == null || _ballBitmapRadius != _ballRadius)
            {
                if (_ballBitmap != null) _ballBitmap.Dispose();

                int size = _ballRadius * 2;
                _ballBitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                _ballBitmapRadius = _ballRadius;

                RenderAmigaBallIntoBitmap(_ballBitmap, _ballRadius, _rollAngle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Background
            if (_logo != null)
                g.DrawImage(_logo, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
            else
                g.Clear(Color.Black);

            // Ball between background and ticker
            DrawBall(g);

            // Ticker on top
            DrawTicker(g);
        }

        private void DrawBall(Graphics g)
        {
            // Subtle ground shadow
            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
            {
                float w = _ballRadius * 1.9f;
                float h = _ballRadius * 0.55f;
                g.FillEllipse(shadow, _ballX - w / 2f, _ballY + _ballRadius * 0.75f, w, h);
            }

            int size = _ballRadius * 2;
            g.DrawImage(_ballBitmap, _ballX - _ballRadius, _ballY - _ballRadius, size, size);

            // Outline
            using (Pen outline = new Pen(Color.FromArgb(160, 0, 0, 0), 2f))
            {
                g.DrawEllipse(outline, _ballX - _ballRadius, _ballY - _ballRadius, size, size);
            }
        }

        private void DrawTicker(Graphics g)
        {
            SizeF size = g.MeasureString(_scrollText, _font);
            _tickerTextHeight = size.Height;

            if (_textX + size.Width < 0)
                _textX = ClientSize.Width;

            float y = _tickerY;

            using (Brush shadow = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                g.DrawString(_scrollText, _font, shadow, _textX + 3, y + 3);

            using (Brush textBrush = new SolidBrush(Color.White))
                g.DrawString(_scrollText, _font, textBrush, _textX, y);
        }

        /// <summary>
        /// Renders an "Amiga ball" with spherical checker mapping + shading into an existing bitmap.
        /// This creates the effect: larger squares in the center, compressed toward the edges.
        /// </summary>
        private static void RenderAmigaBallIntoBitmap(Bitmap bmp, int r, float rollAngle)
        {
            int size = r * 2;

            // Checker resolution: tweak for taste
            // More columns/rows = smaller squares.
            int cols = 10;
            int rows = 10;

            // Light direction (normalized-ish)
            float lx = -0.35f, ly = -0.35f, lz = 0.87f;

            Rectangle rect = new Rectangle(0, 0, size, size);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int stride = data.Stride;
            int bytes = stride * size;
            byte[] buffer = new byte[bytes];

            float invR = 1f / r;
            float pi = (float)Math.PI;
            float twoPi = (float)(Math.PI * 2.0);

            for (int y = 0; y < size; y++)
            {
                float dy = (y - r) * invR;

                for (int x = 0; x < size; x++)
                {
                    float dx = (x - r) * invR;

                    float rr = dx * dx + dy * dy;
                    int idx = y * stride + x * 4;

                    if (rr > 1f)
                    {
                        // Transparent outside sphere
                        buffer[idx + 3] = 0;
                        continue;
                    }

                    // Sphere normal
                    float dz = (float)Math.Sqrt(1f - rr);

                    // Spherical UV mapping
                    // Longitude around Y axis (rollAngle rotates this)
                    float lon = (float)Math.Atan2(dx, dz) + rollAngle;
                    // Latitude
                    float lat = (float)Math.Asin(dy);

                    float u = (lon / twoPi);
                    u = u - (float)Math.Floor(u); // wrap to [0..1)
                    float v = 0.5f - (lat / pi);   // [0..1]

                    int cu = (int)(u * cols);
                    int cv = (int)(v * rows);

                    bool red = ((cu + cv) & 1) == 0;

                    // Base colors (Amiga-ish red/white)
                    float br = red ? 0.85f : 1.00f;
                    float bg = red ? 0.12f : 1.00f;
                    float bb = red ? 0.12f : 1.00f;

                    // Lambert shading
                    float ndotl = dx * lx + dy * ly + dz * lz;
                    if (ndotl < 0f) ndotl = 0f;

                    // Ambient + diffuse
                    float shade = 0.22f + 0.78f * ndotl;

                    // Rim darkening for depth
                    float rim = 1f - dz;              // 0 center, 1 edge
                    shade *= (1f - rim * 0.35f);

                    // Specular highlight (simple)
                    // View direction is roughly (0,0,1)
                    float spec = (float)Math.Pow(Math.Max(0f, ndotl), 18f) * 0.35f;

                    float fr = Clamp01(br * shade + spec);
                    float fg = Clamp01(bg * shade + spec);
                    float fb = Clamp01(bb * shade + spec);

                    buffer[idx + 0] = (byte)(fb * 255f); // B
                    buffer[idx + 1] = (byte)(fg * 255f); // G
                    buffer[idx + 2] = (byte)(fr * 255f); // R
                    buffer[idx + 3] = 255;               // A
                }
            }

            Marshal.Copy(buffer, 0, data.Scan0, bytes);
            bmp.UnlockBits(data);
        }

        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }



}
