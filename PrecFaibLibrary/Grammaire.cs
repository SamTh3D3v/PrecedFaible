using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrecFaibLibrary
{
    public class Grammaire
    {
        #region Fields

        private List<String> _terminaux;
        private List<String> _nomTerminaux;
        private String _axiome;
        private Dictionary<String, List<string>> _reglesDeProduction;

        #endregion
        #region Prperties
        public List<String> Terminaux
        {
            get { return _terminaux; }
            set { _terminaux = value; }
        } 
        public List<String> NomTerminaux
        {
            get { return _nomTerminaux; }
            set { _nomTerminaux = value; }
        } 
        public Dictionary<String, List<string>> ReglesDeProduction
        {
            get { return _reglesDeProduction; }            
        }
        public String Axiome
        {
            get { return _axiome; }
            set { _axiome = value; }
        }

        #endregion
        #region Constructors
        public Grammaire()
        {
            _reglesDeProduction = new Dictionary<string, List<string>>();
        }
        public Grammaire(List<String> terminaux,List<String> nomTerminaux,String axiome)
        {
            _reglesDeProduction = new Dictionary<string, List<string>>();
            _axiome = axiome;
            _terminaux = terminaux;
            _nomTerminaux = nomTerminaux;
        }
        
        #endregion
        #region Methods
        public void AjouterRegles(String mgd, List<String> mdp)
        {
            if (!_reglesDeProduction.ContainsKey(mgd))
            {
                _reglesDeProduction.Add(mgd, mdp);
            }
            else
            {
                _reglesDeProduction[mgd].AddRange(mdp);
            }
        } 
        //Il Faut Assurer Que La Grammaire Est Sans Cycle Sinon La recursiviter Va generer Une Boucle Infit 
        public List<String> Premier(String nomTer)
        {
            List<String> premier=new List<string>();
            if (_reglesDeProduction.ContainsKey(nomTer))
            {
                foreach (var item in _reglesDeProduction[nomTer])
                {
                    String nT = item.ToString().Split(new char[]{' '}).First();
                    if (!premier.Contains(nT))
                    {
                        premier.Add(nT);
                        if (_nomTerminaux.Contains(nT) && nomTer != nT)
                        {
                            //premier.AddRange(Premier(nT));
                            foreach (var va in Premier(nT))
                            {
                                if ( !premier.Contains(va) )
                                {
                                    premier.Add(va);
                                }                                
                            }
                        }
                    }                    
                }                
            }            
            return premier;
        } 
        public List<String> Dernier(String nomTer)
        {
            List<String> dernier = new List<string>();              
            if (_reglesDeProduction.ContainsKey(nomTer))
            {
                foreach (var item in _reglesDeProduction[nomTer])
                {                    
                    String nT = item.ToString().Split(new char[] { ' ' }).Last();
                    if (item.Count() != 0 && !dernier.Contains(nT))
                    {
                        dernier.Add(nT);
                        if (_nomTerminaux.Contains(nT) && nomTer != nT)
                        {
                            foreach (var va in Dernier(nT))
                            {
                                if (!dernier.Contains(va))
                                {
                                    dernier.AddRange(Dernier(nT));
                                }                                
                            }
                        }
                    }
                }
            }
            return dernier;
        }

        public List<String> Debut(String nomTer)
        {
            //Get Onlly TerMinal From The The "Premier" Groupe           
            List<String> debut = new List<string>();
            foreach (var item in Premier(nomTer))
            {
                if (_terminaux.Contains(item))
                {
                    debut.Add(item);
                }    
            }
            return debut;
        }       
        #endregion

    }
}
