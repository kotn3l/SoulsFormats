using System;

namespace SoulsFormats
{
    public partial class FLVER
    {
        /// <summary>
        /// Four weights for binding a vertex to bones, accessed like an array. Unused bones should be set to 0.
        /// </summary>
        public struct VertexBoneWeights
        {
            private float A, B, C, D;

            /// <summary>
            /// Length of bone weights is always 4.
            /// </summary>
            public int Length => 4;
            public double SumD => A + B + C + D;
            public float Sum => A + B + C + D;

            /// <summary>
            /// Accesses bone weights as a float[4].
            /// </summary>
            public float this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                        case 3: return D;
                        default:
                            throw new IndexOutOfRangeException($"Index ({i}) was out of range. Must be non-negative and less than 4.");
                    }
                }

                set
                {
                    switch (i)
                    {
                        case 0: A = value; break;
                        case 1: B = value; break;
                        case 2: C = value; break;
                        case 3: D = value; break;
                        default:
                            throw new IndexOutOfRangeException($"Index ({i}) was out of range. Must be non-negative and less than 4.");
                    }
                }
            }

            /// <summary>
            /// If the vertex weights don't add up to 1, normalize it. 
            /// </summary>
            public void Normalize()
            {
                if (Sum != 1 && Sum != 0)
                {
                    //while (Sum < 1)
                    {
                        float s = Sum;
                        for (int i = 0; i < this.Length; i++)
                        {
                            this[i] = ((this[i] / s)); 
                        }
                    }
                    float difference = 1f - Sum;
                    if (difference != 0f && Sum != 1f)
                    {
                        /*for (int i = 0; i < this.Length; i++)
                        {
                            this[i] += ((this[i] / Sum) * (difference / (float)this.Length));
                        }*/
                        if (Math.Abs(Sum - 1) > 0.00001f) // adjust tolerance as needed
                        {
                            float diff = 1 - Sum;
                            for (int i = 0; i < this.Length; i++)
                            {
                                this[i] = (float)Math.Round(this[i] + (difference / this.Length), 6);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < this.Length; i++)
                            {
                                this[i] = (float)Math.Round(this[i], 6);
                            }
                        }
                        if (Sum != 1f)
                        {
                            /*float diff = 1 - Sum;
                            int max = 0;
                            for (int i = 0; i < this.Length; i++)
                            {
                                if (this[i] > this[max])
                                {
                                    max = i;
                                }
                            }
                            this[max] += diff;*/
                            float s = Sum;
                            for (int i = 0; i < this.Length; i++)
                            {
                                this[i] = ((this[i] / s));
                            }
                        }
                    }
                    /*float difference = 1f - Sum;
                    for (int i = 0; i < this.Length; i++)
                    {
                        this[i] += ((this[i] / Sum) * difference);
                    }*/
                    /*for (int i = 0; i < this.Length; i++)
                    {
                        this[i] = (float)(((double)this[i] / SumD) * 1d);
                    }*/
                    /*for (int i = 0; i < this.Length; i++)
                    {
                        float see = (float)Math.Round(this[i] / Sum, 2);
                        this[i] = (float)Math.Round(this[i] / Sum, 2) * 1f;
                    }*/
                    
                    /*int count = 0;
                    for (int i = 0; i < this.Length; i++)
                    {
                        if (this[i] != 0)
                        {
                            count++;
                        }
                    }
                    float add = difference / (float)count;
                    for (int i = 0; i < this.Length; i++)
                    {
                        if (this[i] != 0)
                        {
                            this[i] += add;
                        }
                    }*/
                    NormalizeAdd();
                }
            }
            private void NormalizeAdd()
            {
                if(Sum < 1)
                {
                    /*double difference = 1d - SumD;
                    bool[] add = new bool[this.Length];
                    int count = 0;
                    for (int i = 0; i < this.Length; i++)
                    {
                        if (this[i] == 0)
                        {
                            add[i] = false;
                        }
                        else
                        {
                            add[i] = true;
                            count++;
                        }
                    }
                    double toAdd = difference / (double)count;
                    for (int i = 0; i < this.Length; i++)
                    {
                        if (add[i])
                        {
                            this[i] += (float)toAdd;
                        }
                    }*/
                    //this[max] += difference;

                    if (Sum != 1)
                    {
                        ;                   
                        //this[max] += d1;
                    }
                }
            }
        }
    }
}
