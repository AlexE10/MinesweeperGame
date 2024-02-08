using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace MinesweeperWPF.Utilities
{
    public static class PrologToTxtFile
    {
        private static readonly string prologName = "swipl-win.exe";
        public static int CreateMatrixInfile(int difficulty)
        {
            string inputFilePath = "../../../PrologMap/input.txt";
            int nrows;
            int ncols;
            int bombsNumber;
            switch (difficulty)
            {
                case 0:
                    nrows = 10;
                    ncols = 10;
                    bombsNumber = 10;
                    break;
                case 1:
                    nrows = 18;
                    ncols = 18;
                    bombsNumber = 40;
                    break;
                case 2:
                    nrows = 24;
                    ncols = 24;
                    bombsNumber = 99;
                    break;
                default:
                    nrows = 10;
                    ncols = 10;
                    bombsNumber = 10;
                    break;
            }

            File.WriteAllLines(inputFilePath, new string[] { nrows.ToString(), ncols.ToString(), bombsNumber.ToString() });


            Process prologProcess = new Process();
            prologProcess.StartInfo.FileName = "D:/Informatica/swipl/bin//swipl-win.exe"; // D:/Informatica/swipl/bin/     C:/Program Files/swipl/bin/
            //prologProcess.StartInfo.FileName = GetPrologExecutablePath(); // D:/Informatica/swipl/bin/     C:/Program Files/swipl/bin/

            prologProcess.StartInfo.Arguments = @"-s ../../../PrologMap/Test.pl"; 
            prologProcess.StartInfo.UseShellExecute = false;

            prologProcess.Start();
            prologProcess.WaitForExit();
            return bombsNumber;
        }
    }
}
