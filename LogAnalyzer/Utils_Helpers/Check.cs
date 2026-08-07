
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace LogAnalyzer.Utils_Helpers

{
    internal class Check
    {



        public static string ReadLine(string message)
        {
            string input_name = "";
            bool work = true;
            int attemptCount = 0;

            while (work)
            {
                try
                {
                    attemptCount++;
                    Console.WriteLine(message);
                    input_name = Console.ReadLine();

                    if (string.IsNullOrEmpty(input_name))
                    {
                        Console.WriteLine("error, please input true data:");
                        Console.WriteLine("data input:");
                    }
                    

                    else
                    {
                        work = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка при вводе, попробуйте еще раз.");
                }
            }

            return input_name;
        }


        
    }


}





