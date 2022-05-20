using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regelstreckenparametrierung
{
    public static class ZeitprozentkennwertMethode
    {
        private static float u_n_1 = (float) 0.045757F;
        private static float u_n_2 = (float) 0.136722F;
        private static float u_n_3 = (float) 0.207065F;

        private static float tau_n1_10 = (float)0.105361F;
        private static float tau_n1_50 = (float)0.693147F;
        private static float tau_n1_90 = (float)2.302585F;

        private static float tau_n2_10 = (float)0.531812F;
        private static float tau_n2_50 = (float)1.678347F;
        private static float tau_n2_90 = (float)3.889720F;

        private static float tau_n3_10 = (float)1.102065F;
        private static float tau_n3_50 = (float)2.674060F;
        private static float tau_n3_90 = (float)5.322320F;

        private static float zeitkonstante = 0;

        public static int berechneOrdnung(float t10, float t90)
        {
            float u = (float)(t10 / t90);

            float diff1 = (float) (u_n_1 - u);
            float diff2 = (float)(u_n_2 - u);
            float diff3 = (float)(u_n_3 - u);

            if (diff1 < 0)
                diff1 *= (-1);

            if (diff2 < 0)
                diff2 *= (-1);

            if (diff3 < 0)
                diff3 *= (-1);

            if (diff1 < diff2 && diff1 < diff3)
            {
                return 1;
            }
            else
            if(diff2 < diff1 && diff2 < diff3)
            {
                return 2;
            }
            else
            if(diff3 < diff1 && diff3 < diff2)
            {
                return 3;
            }
            else
            { return 4; }

        }

        public static float berechneZeitkonstante(int ordnung, float t10, float t50, float t90)
        {
            if(ordnung == 1)
            {
                zeitkonstante = (float) (1F / 3F) * (((float) t10 / (float) tau_n1_10) + ((float) t50 / (float) tau_n1_50) + ((float) t90 / (float) tau_n1_90));
                Utility._Debug("++" + zeitkonstante);

            }
            else
            if(ordnung == 2)
            {
                zeitkonstante = (float) (1F / 3F) * (((float) t10 / (float) tau_n2_10) + ((float) t50 / (float) tau_n2_50) + ((float) t90 / (float) tau_n2_90));

            }
            else
            if(ordnung == 3)
            {
                zeitkonstante = (float) (1F / 3F) * (((float)t10 / (float) tau_n3_10) + ((float)t50 / (float) tau_n3_50) + ((float) t90 / (float) tau_n3_90));

            }

            return zeitkonstante;
        }
    }
}
