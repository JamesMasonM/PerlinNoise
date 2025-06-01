using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private const int Width = 512;
        private const int Height = 512;

        private int octaves = 12;
        private double persistence = 1;
        private double scale = 0.01;
        private bool isDarkTheme = false;

        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
            InitializeSliders();
            this.Text = "Perlin Noise Height Map Generator";
            this.ClientSize = new Size(Width, Height + 150); // Increased height for sliders

            TextBox seedTextBox = new TextBox { Location = new Point(10, Height + 10), Width = 100 };
            Button generateButton = new Button { Text = "Generate", Location = new Point(120, Height + 10) };
            PictureBox pictureBox = new PictureBox { Location = new Point(0, 0), Size = new Size(Width, Height) };
            Button themeButton = new Button { Text = "Switch to Dark Theme", Location = new Point(300, Height + 10), Width = 150 };

            generateButton.Click += (sender, e) =>
            {
                int seed;
                if (int.TryParse(seedTextBox.Text, out seed))
                {
                    GenerateHeightMap(seed, pictureBox);
                }
                else
                {
                    MessageBox.Show("Please enter a valid integer seed.");
                }
            };

            themeButton.Click += (sender, e) =>
            {
                isDarkTheme = !isDarkTheme;
                ApplyTheme();
                themeButton.Text = isDarkTheme ? "Switch to Light Theme" : "Switch to Dark Theme";
            };

            this.Controls.Add(seedTextBox);
            this.Controls.Add(generateButton);
            this.Controls.Add(pictureBox);
            this.Controls.Add(themeButton);

            // Apply initial theme
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (isDarkTheme)
            {
                // Dark theme colors
                this.BackColor = Color.FromArgb(45, 45, 48);
                this.ForeColor = Color.White;

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        control.BackColor = Color.FromArgb(37, 37, 38);
                        control.ForeColor = Color.White;
                    }
                    else if (control is Button)
                    {
                        control.BackColor = Color.FromArgb(63, 63, 70);
                        control.ForeColor = Color.White;
                    }
                    else if (control is Label)
                    {
                        control.ForeColor = Color.White;
                    }
                    else if (control is TrackBar)
                    {
                        control.BackColor = Color.FromArgb(63, 63, 70);
                    }
                }
            }
            else
            {
                // Light theme colors (default)
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        control.BackColor = SystemColors.Window;
                        control.ForeColor = SystemColors.WindowText;
                    }
                    else if (control is Button)
                    {
                        control.BackColor = SystemColors.Control;
                        control.ForeColor = SystemColors.ControlText;
                    }
                    else if (control is Label)
                    {
                        control.ForeColor = SystemColors.ControlText;
                    }
                    else if (control is TrackBar)
                    {
                        control.BackColor = SystemColors.Control;
                    }
                }
            }
        }

        private void InitializeSliders()
        {
            // Octaves TrackBar
            TrackBar trackBarOctaves = new TrackBar
            {
                Minimum = 1,
                Maximum = 20,
                Value = octaves,
                Location = new Point(10, Height + 50),
                Width = 200
            };
            trackBarOctaves.Scroll += (sender, e) =>
            {
                octaves = trackBarOctaves.Value;
                // Optionally regenerate the height map here if needed
            };
            this.Controls.Add(trackBarOctaves);
            Label labelOctaves = new Label { Text = "Octaves: " + octaves, Location = new Point(220, Height + 50) };
            trackBarOctaves.Scroll += (sender, e) => labelOctaves.Text = "Octaves: " + trackBarOctaves.Value;
            this.Controls.Add(labelOctaves);

            // Persistence TrackBar
            TrackBar trackBarPersistence = new TrackBar
            {
                Minimum = 1,
                Maximum = 100, // Scale for easier adjustment
                Value = (int)(persistence * 100),
                Location = new Point(10, Height + 80),
                Width = 200
            };
            trackBarPersistence.Scroll += (sender, e) =>
            {
                persistence = trackBarPersistence.Value / 100.0; // Convert to 0.01 - 1.0
                // Optionally regenerate the height map here if needed
            };
            this.Controls.Add(trackBarPersistence);
            Label labelPersistence = new Label { Text = "Persistence: " + persistence.ToString("F2"), Location = new Point(220, Height + 80) };
            trackBarPersistence.Scroll += (sender, e) => labelPersistence.Text = "Persistence: " + (trackBarPersistence.Value / 100.0).ToString("F2");
            this.Controls.Add(labelPersistence);

            // Scale TrackBar
            TrackBar trackBarScale = new TrackBar
            {
                Minimum = 1,
                Maximum = 100, // Scale for easier adjustment
                Value = (int)(scale * 100),
                Location = new Point(10, Height + 110),
                Width = 200
            };
            trackBarScale.Scroll += (sender, e) =>
            {
                scale = trackBarScale.Value / 100.0; // Convert to 0.01 - 1.0
                // Optionally regenerate the height map here if needed
            };
            this.Controls.Add(trackBarScale);
            Label labelScale = new Label { Text = "Scale: " + scale.ToString("F2"), Location = new Point(220, Height + 110) };
            trackBarScale.Scroll += (sender, e) => labelScale.Text = "Scale: " + (trackBarScale.Value / 100.0).ToString("F2");
            this.Controls.Add(labelScale);
        }

        private void GenerateHeightMap(int seed, PictureBox pictureBox)
        {
            PerlinNoise perlin = new PerlinNoise(seed);
            Bitmap bitmap = new Bitmap(Width, Height);

            double[,] heightMap = new double[Width, Height];

            // Generate height map
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double total = 0;
                    double frequency = 1;
                    double amplitude = 1;
                    double maxValue = 0;

                    for (int o = 0; o < octaves; o++)
                    {
                        double noiseValue = perlin.Noise(x * scale * frequency, y * scale * frequency);
                        total += noiseValue * amplitude;

                        maxValue += amplitude;
                        amplitude *= persistence;
                        frequency *= 2;
                    }

                    heightMap[x, y] = total / maxValue; // Store normalized height value
                }
            }

            // Apply smoothing using linear interpolation
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    double smoothedValue = (heightMap[x - 1, y] + heightMap[x + 1, y] +
                                             heightMap[x, y - 1] + heightMap[x, y + 1]) / 4.0;
                    int colorValue = (int)((smoothedValue + 1) * 127.5);
                    Color color = Color.FromArgb(colorValue, colorValue, colorValue);
                    bitmap.SetPixel(x, y, color);
                }
            }

            pictureBox.Image = bitmap;
        }
    }
}