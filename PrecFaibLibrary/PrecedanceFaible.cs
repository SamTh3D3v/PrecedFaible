using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PrecFaibLibrary
{
    //Most Funtiionnality Are Implemented Whithin This Class
    public class PrecedanceFaible
    {
        #region Fields

        String[,] tableDanalyse;
        private Grammaire _grammaire;
        private int size;



        #endregion

        public String Header = "";
        public String Main = "";
        public Dictionary<String, String> TokenDico;
        public PrecedanceFaible(Grammaire grammaire)
        {
            size = grammaire.Terminaux.Count + grammaire.NomTerminaux.Count + 1;
            tableDanalyse = new string[size, size];
            int i = 1;
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    tableDanalyse[j, k] = "";
                }

            }
            foreach (var item in grammaire.NomTerminaux)
            {
                tableDanalyse[i, 0] = item;
                tableDanalyse[0, i] = item;
                i++;
            }
            foreach (var item in grammaire.Terminaux)
            {
                tableDanalyse[i, 0] = item;
                tableDanalyse[0, i] = item;
                i++;
            }

            _grammaire = grammaire;
            //Verification Des Premiere Condition De PrecedanceFaibe 
            //-->1 Pas Des Production Vide 
            foreach (var reg in _grammaire.ReglesDeProduction)
            {
                foreach (string mdp in reg.Value)
                {
                    if (mdp.Trim() == "")
                    {
                        throw new Exception("Erreur : Grammaire Contient La Production Vide ");
                        return;
                    }
                }
            }
            //-->2 pas de "deux regle de production avec le meme Membre Droite "
            foreach (var reg in _grammaire.ReglesDeProduction)
            {
                foreach (var reg2 in _grammaire.ReglesDeProduction)
                {
                    if (reg.Key != reg2.Key)
                    {
                        foreach (string s in reg.Value)
                        {
                            if (reg2.Value.Contains(s))
                            {
                                throw new Exception("Erreur : Deux Regle de Production avec le meme mdp ");
                            }
                        }
                    }
                }
            }
            //-->3 et 4 Les Deux Autre Condition Sont Verifier A partir de la table D'analyse



        }
        public void Analyser()
        {
            foreach (var listRegle in _grammaire.ReglesDeProduction)
            {
                foreach (String regle in listRegle.Value)
                {
                    _AnalyserRegle(regle.Split(new char[] { ' ' }));

                }
            }
        }
        private void _AnalyserRegle(String[] nonTers)
        {
            if (nonTers.Count() > 1)
            {
                String nonTerG = nonTers[0];
                String nonTerD;
                for (int i = 1; i < nonTers.Count(); i++)
                {
                    nonTerD = nonTers[i];
                    if (!tableDanalyse[_getIndex(nonTerG), _getIndex(nonTerD)].Contains("="))
                        tableDanalyse[_getIndex(nonTerG), _getIndex(nonTerD)] += "=";

                    if (_grammaire.Terminaux.Contains(nonTerG) && _grammaire.NomTerminaux.Contains(nonTerD))  //aA
                    {
                        //a<Premier(A)   

                        foreach (var a in _grammaire.Premier(nonTerD))
                        {
                            if (!tableDanalyse[_getIndex(a), _getIndex(nonTerD)].Contains("<"))
                                tableDanalyse[_getIndex(nonTerG), _getIndex(a)] += "<";
                        }
                    }

                    if (_grammaire.NomTerminaux.Contains(nonTerG) && _grammaire.Terminaux.Contains(nonTerD))  //Aa
                    {
                        //Dernier(A) >a  

                        foreach (var a in _grammaire.Dernier(nonTerG))
                        {
                            if (!tableDanalyse[_getIndex(a), _getIndex(nonTerD)].Contains(">"))
                            {
                                tableDanalyse[_getIndex(a), _getIndex(nonTerD)] += ">";
                            }
                        }
                    }


                    if (_grammaire.NomTerminaux.Contains(nonTerG) && _grammaire.NomTerminaux.Contains(nonTerD))  //AS
                    {
                        //A<Premier(A)   

                        foreach (var a in _grammaire.Premier(nonTerD))
                        {
                            if (!tableDanalyse[_getIndex(nonTerG), _getIndex(a)].Contains("<"))
                                tableDanalyse[_getIndex(nonTerG), _getIndex(a)] += "<";
                        }
                    }
                    if (_grammaire.NomTerminaux.Contains(nonTerG) && _grammaire.NomTerminaux.Contains(nonTerD))  //AS
                    {
                        //Dernier(A)<Debut(S)   

                        foreach (var a in _grammaire.Dernier(nonTerG))
                        {
                            foreach (var b in _grammaire.Debut(nonTerD))
                            {
                                if (!tableDanalyse[_getIndex(a), _getIndex(b)].Contains(">"))
                                {
                                    tableDanalyse[_getIndex(a), _getIndex(b)] += ">";
                                }

                            }
                        }
                    }


                    nonTerG = nonTerD;
                }
            }
        }
        private int _getIndex(String itm)
        {
            int i = 1;
            while (itm != tableDanalyse[i, 0])
            {
                i++;
            }
            return i;
        }
        public void Aff_Tab_Analyse()
        {
            int sizeTab = _grammaire.Terminaux.Count + _grammaire.NomTerminaux.Count + 1;
            for (int i = 0; i < sizeTab; i++)
            {
                for (int j = 0; j < sizeTab; j++)
                {
                    if (tableDanalyse[i, j] != null)
                    {

                        Console.Write(" | " + tableDanalyse[i, j]);
                    }
                    else
                    {
                        Console.Write(" |  ");
                    }
                }
                Console.WriteLine();
            }
        }
        public void TableDanalyseMonoDefinit()
        {
            for (int i = 1; i < size; i++)
            {
                for (int j = 1; j < size; j++)
                {
                    if (tableDanalyse[i, j] != null)
                    {
                        if (tableDanalyse[i, j].Length > 1 && !tableDanalyse[i, j].Contains("="))
                        {
                            throw new Exception("Erreur : La table d'analyse n'est pas mono definit");
                            //return false;
                        }
                    }
                }
            }
            // return true;
        }
        //permet de verifier si la condition de precedance faible est verifier ou pas
        public void ConditionDePrecdanceFaible()
        {
            //A -->xay et B-->y alors pas de relation entre a et B 
            foreach (var regles in _grammaire.ReglesDeProduction)
            {
                foreach (String reg in regles.Value)
                {

                    foreach (var regles2 in _grammaire.ReglesDeProduction)
                    {
                        if (regles2.Key != regles.Key)
                        {

                            foreach (String reg2 in regles2.Value)
                            {
                                if (reg2.EndsWith(reg))
                                {
                                    //pas derelation entre regles2.Key et 'a'
                                    if (Relation(regles.Key, reg2, reg))
                                    {
                                        throw new Exception("Erreur : La Condition de precedance Faible n'est pas verifier");
                                    }

                                }

                            }
                        }
                    }
                }
            }
        }
        public bool Relation(String B, String xay, String y)
        {
            String xa = xay.Substring(0, xay.Length - y.Length);
            String[] xaTab = xa.Split(new char[] { ' ' });
            if (tableDanalyse[_getIndex(xaTab[xaTab.Length - 1]), _getIndex(B)] == null)
                return false;
            else
            {
                return true;
            }
        }


        public void GenererCodeSource()
        {
            StreamReader reader = new StreamReader("AlgorithmDanalyse");
            StreamWriter writer = new StreamWriter("Programme.cs");


            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                #region table
                if (line == "//Table")
                {
                    //Insetion De la table Danalyse                    
                    writer.Write("static String[,] tableDanalyse=new string[" + size + "," + size + "]");
                    for (int i = 0; i < size; i++)
                    {
                        if (i == 0)
                        {
                            writer.WriteLine("{");
                        }

                        for (int j = 0; j < size; j++)
                        {
                            if (j == 0)
                            {
                                writer.Write("{");
                            }
                            writer.Write("\"" + tableDanalyse[i, j] + "\"");
                            if (j == size - 1)
                            {
                                writer.Write("}");
                            }
                            else
                            {
                                writer.Write(",");
                            }

                        }
                        if (i == size - 1)
                        {
                            writer.WriteLine("};");
                        }
                        else
                        {
                            writer.WriteLine(",");
                        }
                    }
                }

                #endregion
                #region regle De Production

                else if (line == "//Regles")
                {
                    int y = 0;
                    writer.WriteLine("static String _axiome = \"" + _grammaire.Axiome + "\";");
                    writer.WriteLine("static Dictionary<String, List<string>> _reglesDeProduction= new Dictionary<string, List<string>>(){");
                    foreach (var regle in _grammaire.ReglesDeProduction)
                    {
                        y++;
                        //{{"",new List<string>(){"",""}}}
                        writer.Write("{\"" + regle.Key + "\",new List<String>()");
                        for (int i = 0; i < regle.Value.Count; i++)
                        {
                            if (i == 0)
                            {
                                writer.Write("{");
                            }

                            writer.Write("\"" + regle.Value[i] + "\"");

                            if (i == regle.Value.Count - 1)
                            {
                                writer.Write("}");
                            }
                            else
                            {
                                writer.Write(",");
                            }
                        }
                        if (y != _grammaire.ReglesDeProduction.Count)
                        {
                            writer.WriteLine("},");
                        }
                        else
                        {
                            writer.WriteLine("}};");
                        }


                    }

                }
                #endregion
                else if (line == "//Terminal")
                {
                    writer.Write("static List<String> _terminal=new List<string>()");
                    for (int i = 0; i < _grammaire.Terminaux.Count; i++)
                    {
                        if (i == 0)
                        {
                            writer.Write("{");
                        }

                        writer.Write("\"" + _grammaire.Terminaux[i] + "\"");

                        if (i == _grammaire.Terminaux.Count - 1)
                        {
                            writer.WriteLine("};");
                        }
                        else
                        {
                            writer.Write(",");
                        }
                    }


                }
                else if (line == "//Header")
                {
                    writer.Write(Header);
                }
                else if (line == "//Main")
                {
                    writer.Write(Main);
                }
                else if (line == "//TkLex")
                {
                    if (TokenDico.Count != 0)
                    {
                        int y = 0;
                        foreach (var vv in TokenDico)
                        {
                            if (y==0)
                            {
                                writer.WriteLine("if (Char." + vv.Value + "(c)) \n" +
                                          " {" +
                                               " return new Token() { Rval = int.Parse(c.ToString()), type = TokenType." + vv.Value + ", Value = \"" + vv.Key + "\" };\n" +
                                         "  }\n");
                            }
                            else
                            {
                                writer.WriteLine("else if (Char." + vv.Value + "(c)) \n" +
                                         " {\n" +
                                              " return new Token() { Rval = int.Parse(c.ToString()), type = TokenType." + vv.Value + ", Value = \"" + vv.Key + "\" };\n" +
                                        "  }\n");
                            }
                        }

                        writer.WriteLine("else \n" +
                                           " {\n" +
                                                " return new Token() { Rval = 0, type = TokenType.Symbole, Value = c.ToString() };\n" +
                                          "  }\n");
                    }
                    else
                    {
                        writer.WriteLine("return new Token() { Rval = 0, type = TokenType.Symbole, Value = c.ToString() };\n ");
                    }                    
                }
                else
                {

                    writer.WriteLine(line);
                }

            }
            reader.Close();
            writer.Close();



        }



    }
}
