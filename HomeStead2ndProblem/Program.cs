using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeStead2ndProblem
{
    class Program
    {
        static void Main(string[] args)
        {

            HeadSteadResult(6, 5, 7, 3, 7, 2, new[] {"3, 4", "3, 3", "6, 1", "1, 1", "5, 5", " 5, 5", "3, 1"});
        }


        public static int HeadSteadResult(int safetyAreaL, int safetyAreaW, int totalHubs, int hubsInSection, int rows,
            int cols, string[] hubsCorodinates)
        {

            //Plot a safety area

            int[,] safetyArea = new int[safetyAreaL, safetyAreaW];
            
            for (int row = 0; row < rows; row++)
            {
                var hubPos = hubsCorodinates[row].Split(',');
                {
                    safetyArea[Convert.ToInt16(hubPos[0]) - 1, Convert.ToInt16(hubPos[1]) - 1] = safetyArea[Convert.ToInt16(hubPos[0]) -1, Convert.ToInt16(hubPos[1]) - 1] + 1;
                }
                
            }
            

            return 0;
        }
    }
}
