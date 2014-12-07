using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PrecFaibLibrary;

namespace TmpTestConsole
{
    class Program
    {


        static void Main(string[] args)
        {
            Grammaire grammaire = new Grammaire(new List<string>() { "a", ")", "(", "#" }, new List<string>() { "A", "S", "Z" }, "S");
            grammaire.AjouterRegles("Z", new List<string>() { "# S #" });
            grammaire.AjouterRegles("S", new List<string>() { "A )" });
            grammaire.AjouterRegles("A", new List<string>() { "(", "A a", "A S" });
            grammaire.AjouterRegles("A", new List<string>() { "A a" });
            List<String> tmp = grammaire.Premier("S");
            List<String> tmp2 = grammaire.Dernier("S");


            PrecedanceFaible preFaib = new PrecedanceFaible(grammaire);
            preFaib.Analyser();
            preFaib.Aff_Tab_Analyse();
            preFaib.GenererCodeSource();
             Console.WriteLine(preFaib.ConditionDePrecdanceFaible());
            Console.ReadKey();

           
            
    
        }        
    }
}
