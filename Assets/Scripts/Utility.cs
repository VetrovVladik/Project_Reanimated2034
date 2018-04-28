using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Utilities {
    public static class StaticUtility {

        // ( ͡° ͜ʖ ͡°)━☆ﾟ.*･｡ﾟ
        public static T[] ShuffleArray<T>(T[] array, int seed = 69)
        {
            if (seed == 69)
            {
                System.Random rand = new System.Random();
                seed = rand.Next(-1000000, 1000000);
            }

            System.Random prng = new System.Random(seed);
            for (int i = 0; i < array.Length; i++)
            {
                int randomIndex = prng.Next(i, array.Length);
                T tempItem = array[randomIndex];

                array[randomIndex] = array[i];
                array[i] = tempItem;
            }
            return array;
        }
    }

    public class InvNormalProbability2D
    {
        float varScaler= 1; //Make distribution tails more fat

        List<float> xVec;
        List<float> yVec;
        float xMean;
        float yMean;
        float xSigma;
        float ySigma;
        Dictionary<float, List<Coord>> probs;

        public Vector2 loc;
        public Vector2 scale;
        float cov = 0.000001f;

        float rng1;
        int rng2;

        System.Random rand = new System.Random();

        public InvNormalProbability2D(List<float> _xVec, List<float> _yVec)
        {
            xVec = _xVec;
            yVec = _yVec;
            xMean = xVec.Count > 0 ? xVec.Mean() : 0f;
            yMean = yVec.Count > 0 ? yVec.Mean() : 0f;
            xSigma = xVec.Count > 0 ? xVec.StandardDeviation() * varScaler : 0f;
            ySigma = yVec.Count > 0 ? yVec.StandardDeviation() * varScaler : 0f;
            probs = new Dictionary<float, List<Coord>>();

            loc = new Vector2(xMean, yMean);
            scale = new Vector2(xSigma, ySigma);

            for (int x = 0; x < xVec.Count; x++)
            {
                for (int y = 0; y < yVec.Count; y++)
                {
                    Coord coord = new Coord((int)xVec[x], (int)yVec[y]);
                    float prob = Get2DNormProb(loc, scale, coord, cov);
                    if (probs.ContainsKey(prob))
                    {
                        List<Coord> list = probs[prob];
                        list.Add(coord);
                        probs[prob] = list;
                    }
                    else
                    {
                        List<Coord> list = new List<Coord>();
                        list.Add(coord);
                        probs[prob] = list;
                    }
                }
            }
        }

        public Coord GetCell()
        {
            List<float> distribution = new List<float>();
            List<float> initDistribution = new List<float>();

            foreach (KeyValuePair<float, List<Coord>> pair in probs)
            {
                initDistribution.Add(pair.Key);
                distribution.Add(pair.Key * pair.Value.Count);
            }

            distribution.Sort();

            List<float> adjDistribution = new List<float>();
            float distSum = distribution.Sum();

            float cumProb = 0;
            foreach (float probability in distribution)
            {
                cumProb = cumProb + (probability / distSum);
                adjDistribution.Add(cumProb);
            }

            //Create adjustment, so that sum of probabilities will be 1, not like 0.9991...
            if (adjDistribution.Sum() != 1) adjDistribution[adjDistribution.Count - 1] = adjDistribution[adjDistribution.Count - 1] + (1 - adjDistribution.Sum());

             rng1 = rand.Next(0,1000)/1000f; //Generate random 



            float lBoundary = 0;
            for (int i = 0; i < adjDistribution.Count; i++)
            {
                if (rng1 > lBoundary && rng1 <= adjDistribution[i])
                {
                    List<Coord> coords = probs[initDistribution[i]];
                    int cap = coords.Count;
                    rng2 = rand.Next(0, cap);
                    //Debug.Log(rng2);
                    return coords[rng2];
                }
                lBoundary = adjDistribution[i];
            }
            return new Coord(0,0);
        }

        float Get2DNormProb(Vector2 loc, Vector2 scale, Coord point, float cov = 0.00001f)
        {
            float p = cov / (scale.x * scale.y);
            float z = (Mathf.Pow((point.x - loc.x), 2) / Mathf.Pow(scale.x, 2)) + (Mathf.Pow((point.y - loc.y), 2) / Mathf.Pow(scale.y, 2)) -
                 (2 * p * (point.x - loc.x) * (point.y - loc.y) / (scale.x * scale.y));

            float prob = (1 / (2 * Mathf.PI * scale.x * scale.y * Mathf.Pow((1 - Mathf.Pow(p, 2)), 1 / 2))) * Mathf.Exp(-z / (2*(1 - Mathf.Pow(p, 2))));

            //Debug.Log(prob);

            return 1 - prob;
        }
}


    public static class ListExtensions
    {
        public static float Mean(this List<float> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static float Mean(this List<float> values, int start, int end)
        {
            float s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static float Variance(this List<float> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static float Variance(this List<float> values, float mean)
        {
            return values.Variance(mean, 0, values.Count);
        }

        public static float Variance(this List<float> values, float mean, int start, int end)
        {
            float variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += (float)Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        public static float StandardDeviation(this List<float> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static float StandardDeviation(this List<float> values, int start, int end)
        {
            float mean = values.Mean(start, end);
            float variance = values.Variance(mean, start, end);

            return (float)Math.Sqrt(variance);
        }
    }
}