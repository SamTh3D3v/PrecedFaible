using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrecFaibLibrary
{
    public class class1
    {
        //This Is A Test Main Class
        void Main()
        {
            
            Grammaire grammaire = new Grammaire(new List<string>() { "a", ")","(" }, new List<string>() { "A", "S" },"S");
            grammaire.AjouterRegles("S",new List<string>(){"A )"});
            grammaire.AjouterRegles("A", new List<string>() { "(","A a","A S" });
             //List<String> Tmp= grammaire.Premier("A");           
        }
    }
}
