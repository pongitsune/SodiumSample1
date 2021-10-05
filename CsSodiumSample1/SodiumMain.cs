using Sodium.Frp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsSodiumSample1
{
    class SodiumMain
    {
        static void Main()
        { 
            Console.WriteLine("C# Sodium Sample1");
            StreamSink<ConsoleKeyInfo> sKeyInput = 
                Stream.CreateSink<ConsoleKeyInfo>();
            Stream<string> sStrOutput =
                    sKeyInput
                    .Map((ConsoleKeyInfo ki) =>
                        {
                            if (char.IsNumber(ki.KeyChar))
                            {
                                return $" '{ki.KeyChar}' は数字です";
                            }
                            else if (char.IsLetter(ki.KeyChar))
                            {
                                return $" '{ki.KeyChar}' はアルファベットです";
                            }
                            else
                            {
                                return null;
                            }
                        })
                    .Filter((string s) => !string.IsNullOrEmpty(s));

            IStrongListener outputListener = 
                sStrOutput.Listen((string s) => Console.WriteLine($"{s}"));

            while (true)
            {
                ConsoleKeyInfo ki = Console.ReadKey(true);
                if (ki.Key == ConsoleKey.Escape)
                {
                    break;
                }
                sKeyInput.Send(ki);
            }

            outputListener.Unlisten();
        }
    }
}
