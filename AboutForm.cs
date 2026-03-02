using System;
using System.Diagnostics;
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
        private Image _logo;

        // -----------------------------
        // Timing (smooth game-loop)
        // -----------------------------
        private readonly Stopwatch _sw = new Stopwatch();
        private long _lastTicks;

        // -----------------------------
        // Ticker
        // -----------------------------
        private float _textX;

        private string _scrollText =
            "Hello and Welcome to Cathal's Kickstart Pal Patcher. This peace of software was created because some members of the A1K.org forum, led by QMadx64, were looking for a way to start their Amigas with ECS NTSC Agnus in Pal mode. It is important to note that all these patches only work if the Amiga has an Agnus chip that can be switched. This usually applies to all 8372A/AB and 8375 Agnus chips, as well as Alice in AGA Amigas, of course. Special Thanks to the A1K.org Users A10001986 and DingensCGN for the initial Patch Data with whose help I was able to create the Autopatcher";

        private readonly Font _font =
            new Font("Segoe UI", 48f, FontStyle.Bold, GraphicsUnit.Pixel);

        private readonly float _tickerSpeed = 2f; // px per frame at ~60fps (scaled by dt)

        // Endless "ballistic" bounce (no energy loss)
        private float _tickerY;
        private float _bouncePhase01;                 // 0..1 repeating
        private const float BounceSpeed = 0.012f;     // cycles per tick at ~60fps (scaled by dt)

        private float _tickerTextHeight = 48f;
        private const float TickerFloorMargin = 10f;

        // -----------------------------
        // Amiga ball
        // -----------------------------
        private float _ballX;
        private float _ballY;
        private float _ballVx = 3.2f; // px per frame at ~60fps (scaled by dt)
        private float _ballVy = 2.4f;
        private int _ballRadius = 70;

        private float _rollAngle;

        private Bitmap _ballBitmap;
        private int _ballBitmapRadius;

        // Reusable pixel buffer to avoid GC hitches
        private byte[] _ballPixelBuffer;

        public AboutForm()
        {
            Text = "Cathal's Kickstart Pal Patcher 2 v" + Application.ProductVersion;
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

            // Start ticker at the middle (top point of the bounce)
            _tickerY = (ClientSize.Height - _tickerTextHeight) / 2f;

            // Ball start
            _ballX = ClientSize.Width * 0.5f;
            _ballY = ClientSize.Height * 0.55f;

            EnsureBallBitmap();
            RenderAmigaBallIntoBitmap(_ballBitmap, _ballRadius, _rollAngle, _ballPixelBuffer);

            // Smooth loop using Application.Idle + Stopwatch (better than WinForms Timer)
            _sw.Start();
            _lastTicks = _sw.ElapsedTicks;
            Application.Idle += Application_Idle;

            FormClosed += (s, e) =>
            {
                Application.Idle -= Application_Idle;

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

        // -----------------------------
        // Smooth loop
        // -----------------------------
        private void Application_Idle(object sender, EventArgs e)
        {
            while (IsAppIdle())
            {
                long now = _sw.ElapsedTicks;
                float dt = (now - _lastTicks) / (float)Stopwatch.Frequency; // seconds
                _lastTicks = now;

                // Avoid giant jumps if the app was paused by the OS / window moves, etc.
                if (dt > 0.05f) dt = 0.05f;
                if (dt < 0f) dt = 0f;

                StepBall(dt);
                StepTicker(dt);

                Invalidate();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        private static bool IsAppIdle()
        {
            NativeMessage msg;
            return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
        }

        // -----------------------------
        // Animation steps (dt-based)
        // -----------------------------
        private void StepTicker(float dt)
        {
            // Convert "old per-frame @60fps" values to dt scaling
            float factor = dt * 60f;

            // Horizontal scroll
            _textX -= _tickerSpeed * factor;

            // Phase 0..1 (endless)
            _bouncePhase01 += BounceSpeed * factor;
            if (_bouncePhase01 >= 1f) _bouncePhase01 -= 1f;

            // Bounds: top = middle, bottom = floor
            float topY = (ClientSize.Height - _tickerTextHeight) / 2f;
            float bottomY = ClientSize.Height - _tickerTextHeight - TickerFloorMargin;
            if (bottomY < topY) bottomY = topY;

            // Piecewise ballistic bounce (no damping):
            // First half: falling (slow at top, faster down)  y = u^2
            // Second half: rising (fast up, slow near top)    y = (1-u)^2
            float t = _bouncePhase01;

            float s; // normalized 0..1 (0=top, 1=bottom)
            if (t < 0.5f)
            {
                float u = t / 0.5f;   // 0..1
                s = u * u;
            }
            else
            {
                float u = (t - 0.5f) / 0.5f; // 0..1
                float inv = 1f - u;
                s = inv * inv;
            }

            _tickerY = topY + (bottomY - topY) * s;

            // Wrap horizontally when off-screen
            // NOTE: width depends on font/text, updated in DrawTicker() measure.
        }

        private void StepBall(float dt)
        {
            float factor = dt * 60f;

            _ballX += _ballVx * factor;
            _ballY += _ballVy * factor;

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
            _rollAngle -= ((_ballVx * factor) / (float)_ballRadius);

            EnsureBallBitmap();
            RenderAmigaBallIntoBitmap(_ballBitmap, _ballRadius, _rollAngle, _ballPixelBuffer);
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

                // Recreate reusable buffer for this size
                _ballPixelBuffer = new byte[size * size * 4];
            }
        }

        // -----------------------------
        // Drawing
        // -----------------------------
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
            // Ground shadow (subtle)
            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
            {
                float w = _ballRadius * 1.9f;
                float h = _ballRadius * 0.55f;
                g.FillEllipse(shadow, _ballX - w / 2f, _ballY + _ballRadius * 0.75f, w, h);
            }

            int size = _ballRadius * 2;
            g.DrawImage(_ballBitmap, _ballX - _ballRadius, _ballY - _ballRadius, size, size);

            using (Pen outline = new Pen(Color.FromArgb(160, 0, 0, 0), 2f))
            {
                g.DrawEllipse(outline, _ballX - _ballRadius, _ballY - _ballRadius, size, size);
            }
        }

        private void DrawTicker(Graphics g)
        {
            SizeF size = g.MeasureString(_scrollText, _font);
            _tickerTextHeight = size.Height;

            // Wrap horizontally based on measured width
            if (_textX + size.Width < 0)
                _textX = ClientSize.Width;

            float y = _tickerY;

            using (Brush shadow = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                g.DrawString(_scrollText, _font, shadow, _textX + 3, y + 3);

            using (Brush textBrush = new SolidBrush(Color.White))
                g.DrawString(_scrollText, _font, textBrush, _textX, y);
        }

        // -----------------------------
        // Ball renderer (spherical checker + shading)
        // -----------------------------
        private static void RenderAmigaBallIntoBitmap(Bitmap bmp, int r, float rollAngle, byte[] buffer)
        {
            int size = r * 2;

            // Checker resolution (tweak to taste)
            int cols = 10;
            int rows = 10;

            // Light direction (normalized-ish)
            float lx = -0.35f, ly = -0.35f, lz = 0.87f;

            Rectangle rect = new Rectangle(0, 0, size, size);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int stride = data.Stride;

            float invR = 1f / r;
            float pi = (float)Math.PI;
            float twoPi = (float)(Math.PI * 2.0);

            // We assume buffer length is size*size*4 (tight). Stride may be larger,
            // so we write into the buffer in stride layout, then copy row-by-row.
            // To keep it simple and correct with stride, we fill an intermediate row-buffer layout.
            // Allocate once? We avoid allocations by requiring caller buffer large enough for stride*size.
            // If stride differs, ensure buffer is big enough.
            int required = stride * size;
            if (buffer.Length < required)
                throw new InvalidOperationException("Pixel buffer is too small for the bitmap stride.");

            // Clear alpha for safety (not strictly necessary as we set every pixel)
            // Array.Clear(buffer, 0, required);

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
                        buffer[idx + 3] = 0;
                        continue;
                    }

                    float dz = (float)Math.Sqrt(1f - rr);

                    float lon = (float)Math.Atan2(dx, dz) + rollAngle;
                    float lat = (float)Math.Asin(dy);

                    float u = (lon / twoPi);
                    u = u - (float)Math.Floor(u);
                    float v = 0.5f - (lat / pi);

                    int cu = (int)(u * cols);
                    int cv = (int)(v * rows);

                    bool red = ((cu + cv) & 1) == 0;

                    float br = red ? 0.85f : 1.00f;
                    float bg = red ? 0.12f : 1.00f;
                    float bb = red ? 0.12f : 1.00f;

                    float ndotl = dx * lx + dy * ly + dz * lz;
                    if (ndotl < 0f) ndotl = 0f;

                    float shade = 0.22f + 0.78f * ndotl;

                    float rim = 1f - dz;
                    shade *= (1f - rim * 0.35f);

                    float spec = (float)Math.Pow(Math.Max(0f, ndotl), 18f) * 0.35f;

                    float fr = Clamp01(br * shade + spec);
                    float fg = Clamp01(bg * shade + spec);
                    float fb = Clamp01(bb * shade + spec);

                    buffer[idx + 0] = (byte)(fb * 255f);
                    buffer[idx + 1] = (byte)(fg * 255f);
                    buffer[idx + 2] = (byte)(fr * 255f);
                    buffer[idx + 3] = 255;
                }
            }

            Marshal.Copy(buffer, 0, data.Scan0, required);
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