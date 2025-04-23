using System;
public class PerlinNoise
{
    private int[] permutation;
    private int[] p;

    public PerlinNoise(int seed)
    {
        permutation = new int[256];
        p = new int[512];

        Random rand = new Random(seed);
        for (int i = 0; i < 256; i++)
        {
            permutation[i] = i;
        }

        // Shuffle the array
        for (int i = 0; i < 256; i++)
        {
            int r = rand.Next(256);
            int temp = permutation[i];
            permutation[i] = permutation[r];
            permutation[r] = temp;
        }

        // Duplicate the permutation array
        for (int i = 0; i < 512; i++)
        {
            p[i] = permutation[i % 256];
        }
    }

    public double Noise(double x, double y)
    {
        int xi = (int)Math.Floor(x) & 255;
        int yi = (int)Math.Floor(y) & 255;

        x -= Math.Floor(x);
        y -= Math.Floor(y);

        double u = Fade(x);
        double v = Fade(y);

        int aa = p[p[xi] + yi];
        int ab = p[p[xi] + yi + 1];
        int ba = p[p[xi + 1] + yi];
        int bb = p[p[xi + 1] + yi + 1];

        double gradAA = Grad(aa, x, y);
        double gradAB = Grad(ab, x, y - 1);
        double gradBA = Grad(ba, x - 1, y);
        double gradBB = Grad(bb, x - 1, y - 1);

        double lerp1 = Lerp(gradAA, gradBA, u);
        double lerp2 = Lerp(gradAB, gradBB, u);

        return Lerp(lerp1, lerp2, v);
    }

    private double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);
    private double Lerp(double a, double b, double t) => a + t * (b - a);
    private double Grad(int hash, double x, double y)
    {
        int h = hash & 3; // Convert low 2 bits of hash code
        double u = h < 2 ? x : y; // Gradient value 1-2
        double v = h < 2 ? y : x; // Gradient value 1-2
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v); // Randomly invert half of the gradients
    }
}
