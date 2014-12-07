using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.NavBar;
using PrecFaibLibrary;


namespace PrecFaibApp
{
    public partial class MainWindow : DXWindow
    {
       
        #region Fields
        private List<String> tokenType = new List<String>
                                             {
                                                 "IsNumber",
                                                 "IsLetter"
                                             };
        private List<String> _termineau;
        private List<String> _nomTerminaux;
        private List<String> _tokensList;    //--> Les Token Son Considérer Comme Des 
        private Grammaire _grammaire;
        private String _axiome;
        private String _header = "";
        private String _main="";
        Dictionary<String,String>tokenDico; 
        #endregion
        
        public MainWindow()
        {
            InitializeComponent();
            DocumentPanel dc = new DocumentPanel();
            dc.Caption = "Grammaire " + index;
            //dc.ItemHeight = docPn.ItemHeight;
            dc.Tag = index;

            TextBox tx = new TextBox();
            tx.VerticalAlignment = VerticalAlignment.Stretch;
            tx.HorizontalAlignment = HorizontalAlignment.Stretch;
            tx.AcceptsReturn = true;
            dc.Content = tx;

            DocGrp.Add(dc);
            DocGrp.SelectedTabIndex = index;

            TreeViewItem newChild = new TreeViewItem();
            newChild.Header = dc.Caption;
            newChild.IsExpanded = true;
            // newChild.IsSelected = true;
            newChild.Tag = index;
            tvGrammaire.Items.Add(newChild);
        }

        private void BComp_OnItemClick(object sender, ItemClickEventArgs e)
        {
            _termineau = new List<string>();
            _nomTerminaux = new List<string>();
            _tokensList = new List<string>();
            _grammaire = new Grammaire();
            tokenDico = new Dictionary<string, string>();
            //Lancer La Compilation 
            String txtMain = (DocGrp.SelectedItem.GetUIElement() as TextBox).Text;
            //Stepe One : Verifier La syntax Yacc
            List<String> textIn = txtMain.Split(new char[] { '\n' }).ToList();
            int i = 0;
            while (textIn[i].Trim() == "")
            {
                if (i < textIn.Count - 1)
                {
                    
                    i++;
                }
                else
                {
                    return;
                }
            }
            if (!textIn[i].StartsWith("%{"))
            {
                //Exception 1 :
                txtError.Text += "\n Erreurr ligne " + i + " :Le Programme YACC Ne Commance Pas avec {%";
                return;
            }
            while (i < textIn.Count - 1)
            {
                if (!textIn[i].Trim().EndsWith("%}"))
                {
                    i++;
                    if (!textIn[i].Trim().EndsWith("%}"))                
                    _header += textIn[i];
                    

                }
                else
                {
                    break;
                }
            }
            if (i == textIn.Count - 1)
            {
                //Exception 2 :
                txtError.Text += "\n Erreur :Le Bloc %{ n'as pas ete fermer ";
                return;
            }
            if (i < textIn.Count - 1)
            {
                i++;
            }
            else
            {
                return;
            }

            while (textIn[i].Trim() == "")
            {
                if (i < textIn.Count - 1)
                {
                    i++;
                }
                else
                {
                    return;
                }

            }
            while (textIn[i].StartsWith("%token "))
            {

                String[] tmpTer3 = textIn[i].Trim().Split(new char[] { ' ' });
                String rgx = "";
                List<String> tknsTmp=new List<string>();
                foreach (string s in tmpTer3)
                {
                    if (s.StartsWith("{"))
                    {
                        try
                        {
                            rgx = Regex.Matches(s.Trim(), @"\{(.*?)\}")[0].Groups[1].Value.Trim();
                        }
                        catch (Exception)
                        {
                            txtError.Text += "\n Erreur Syntaxique ligne "+i;
                            return;
                            
                        }
                         
                        if(!tokenType.Contains(rgx))
                        {
                            txtError.Text += "\n Incorrect Token Type ,ligne " + i ;
                            return;
                        }
                        //Regex Associer Au Token 

                        //String tS = Regex.Matches(s.Trim(), @"'(.*?)'")[0].Groups[1].Value;
                        //if (!_termineau.Contains(tS))
                        //{
                        //    _termineau.Add(tS);
                        //}
                    }
                    else if (s.Trim() != "%token")
                    {
                        if (!_tokensList.Contains(s.Trim()))
                        {
                            _tokensList.Add(s.Trim());
                            tknsTmp.Add(s.Trim());
                        }
                    }
                }
                foreach (var tk in tknsTmp)
                {
                    if (!tokenDico.ContainsKey(tk))
                    {
                        tokenDico.Add(tk,rgx);
                    }
                    
                }
                
                //Those Tokens Must Be Saved
                if (i < textIn.Count - 1)
                {
                    i++;
                }
                else
                {
                    return;
                }

            }
            


            if (textIn[i].StartsWith("%%"))
            {
                //Le Block Des Regles De la Grammaire
                if (i < textIn.Count - 1)
                {
                    i++;
                }
                else
                {
                    return;
                }

                while (textIn[i].Trim() == "")
                {
                    if (i < textIn.Count - 1)
                    {
                        i++;
                    }
                    else
                    {
                        return;
                    }

                }
                bool First = true;
                
                while (!textIn[i].Trim().EndsWith("%%"))
                {
                    //Read Rule                    
                    Regex g = new Regex(@"( )*([a-zA-Z0-9]+)( )+: ((( )+'(.*?)'( )*)|(( )+[a-zA-Z0-9]+( )*))*");
                    Regex g2 = new Regex(@"( )*\|((( )+'(.*?)'( )*)|(( )+[a-zA-Z0-9]+( )*))*");
                    if (!g.Match(textIn[i].Trim()).Success)
                    {
                        //Exception 3:
                        txtError.Text += "\n Error dans l'expression reguliere Lingne " + i + " : Syntax Incorrect  ";
                        return;
                    }
                    //A Perfect Match                    
                    String[] tmpTer = textIn[i].Trim().Split(new char[] { ' ', ':' });
                    foreach (string s in tmpTer)
                    {
                        if (s.StartsWith("'"))
                        {
                            String tS = Regex.Matches(s.Trim(), @"'(.*?)'")[0].Groups[1].Value;
                            if (!_termineau.Contains(tS))
                            {
                                _termineau.Add(tS);
                            }
                        }
                        else if(s.StartsWith("{"))
                        {
                            ////////////// 
                            String rgx2 = "";                            
                        try
                        {
                             rgx2 = Regex.Matches(s.Trim(), @"\{(.*?)\}")[0].Groups[1].Value.Trim();
                        }
                        catch (Exception)
                        {
                            txtError.Text += "\n Erreur Syntaxique ligne "+i;
                            return;                            
                        }
                         
                       
                        //Regex Associer Au Token 

                        //String tS = Regex.Matches(s.Trim(), @"'(.*?)'")[0].Groups[1].Value;
                        //if (!_termineau.Contains(tS))
                        //{
                        //    _termineau.Add(tS);
                        //}
                    
                            //////////////
                            
                        }
                        else
                        {
                            if (!_nomTerminaux.Contains(s.Trim()) && s.Trim()!="")
                            {
                                _nomTerminaux.Add(s.Trim());
                            }
                        }
                    }
                    string mdp = "";
                    for (int j = 1; j < tmpTer.Count(); j++)
                    {
                        if (tmpTer[j].StartsWith("'"))
                        {
                            String tt = Regex.Matches(tmpTer[j].Trim(), @"'(.*?)'")[0].Groups[1].Value;
                            mdp += tt.Trim() + " ";
                        }
                        else if (!tmpTer[j].StartsWith("{"))
                        {
                            mdp += tmpTer[j].Trim() + " ";
                        }

                    }
                    if (First)
                    {
                        _axiome = tmpTer[0].Trim();
                        First = false;
                    }
                    
                        _grammaire.AjouterRegles(tmpTer[0].Trim(), new List<string>() { mdp.Trim() });
                    
                    


                    if (i < textIn.Count - 1)
                    {
                        i++;
                    }
                    else
                    {
                        //Exception 3:
                        txtError.Text += "\n Error dans l' expression reguliere Lingne " + i + " :Point Vergul manquante  ";
                    }

                    while (g2.Match(textIn[i].Trim()).Success)
                    {

                        //A Perfect Match                    
                        String[] tmpTer2 = textIn[i].Trim().Split(new char[] { ' ', '|' });
                        foreach (string s in tmpTer2)
                        {
                            if (s.StartsWith("'"))
                            {
                                String tS = Regex.Matches(s.Trim(), @"'(.*?)'")[0].Groups[1].Value;
                                if (!_termineau.Contains(tS))
                                {
                                    _termineau.Add(tS);
                                }
                            }
                            else if (s.StartsWith("{"))
                            {
                                ////////////// 
                                String rgx2 = "";
                                try
                                {
                                    rgx2 = Regex.Matches(s.Trim(), @"\{(.*?)\}")[0].Groups[1].Value.Trim();
                                }
                                catch (Exception)
                                {
                                    txtError.Text += "\n Erreur Syntaxique ligne " + i;
                                    return;
                                }


                                //Regex Associer Au Token 

                                //String tS = Regex.Matches(s.Trim(), @"'(.*?)'")[0].Groups[1].Value;
                                //if (!_termineau.Contains(tS))
                                //{
                                //    _termineau.Add(tS);
                                //}

                                //////////////

                            }
                            else
                            {
                                if (!_nomTerminaux.Contains(s.Trim()) && !_tokensList.Contains(s.Trim()) && s.Trim() != "")
                                {
                                    _nomTerminaux.Add(s.Trim());
                                }
                            }
                        }
                        string mdp2 = "";
                        for (int j = 0; j < tmpTer2.Count(); j++)
                        {
                            if (tmpTer2[j].StartsWith("'"))
                            {
                                String tt = Regex.Matches(tmpTer2[j].Trim(), @"'(.*?)'")[0].Groups[1].Value;
                                mdp2 += tt.Trim() + " ";
                            }

                            else if (!tmpTer2[j].StartsWith("{"))
                            {
                                mdp2 += tmpTer2[j].Trim() + " ";
                            }

                        }
                        
                       
                            _grammaire.AjouterRegles(tmpTer[0].Trim(), new List<string>() {mdp2.Trim()});
                        




                        if (i < textIn.Count - 1)
                        {
                            i++;
                        }
                        else
                        {
                            //Exception 3:
                            txtError.Text += "\n Error dans l'expression reguliere Lingne " + i  + " :Point Vergul manquante  ";
                            return;
                        }

                    }
                    if (textIn[i].Trim() != ";")
                    {
                        //Exception 3:
                        txtError.Text += "\n Error dans l'expression reguliere Lingne " + i + " :Point Vergul manquante  ";
                        return;
                    }
                    if (i < textIn.Count - 1)
                    {
                        i++;
                    }
                    else
                    {
                        //Exception 3:
                        txtError.Text += "\n Error %% Manquante at ligne " + i ;
                        return;
                    }

                }
                _termineau.Add("#");
                _nomTerminaux.Add("Z");
                _grammaire.AjouterRegles("Z", new List<String>() { "# "+_axiome+" #" });

                while (i < textIn.Count - 1)
                {
                    i++;  
                        _main += textIn[i];
                                         
                }

                txtError.Text += " \n Pas D'erreur Dans La Syntaxe de YACC ";
                _termineau.AddRange(_tokensList);
                _grammaire.Axiome = _axiome;
                _grammaire.Terminaux = _termineau;
                _grammaire.NomTerminaux = _nomTerminaux;
                Compiler();

                //List<String> rr = _termineau;
                //List<String> rT = _nomTerminaux;
                //List<String> ry = _tokensList;
            }
            else
            {
                //Exception 4:
                txtError.Text += "\n Error %% Manquante at ligne " + i;
            }
            //The Rest Aint Interesting Cuz it Will Be Copied As It is In OutPut File

        }
        public void Compiler()
        {
            PrecedanceFaible preFaib;
            try
            {
                preFaib = new PrecedanceFaible(_grammaire);
                preFaib.Header = _header;
                preFaib.Main = _main;
                preFaib.TokenDico = tokenDico;
                preFaib.Analyser();
                preFaib.TableDanalyseMonoDefinit();
                //preFaib.Aff_Tab_Analyse();
                preFaib.ConditionDePrecdanceFaible();
                preFaib.GenererCodeSource();
                txtError.Text += "\n Compilation Terminer Avec Succes ";
            }
            catch (Exception e)
            {
                txtError.Text += "\n " + e.Message;
            }
        }

        private void BSave_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "PrecFaible"; // Default file name
            dlg.DefaultExt = ".y"; // Default file extension
            dlg.Filter = "Text documents (.y)|*.y"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                StreamWriter writer = new StreamWriter(filename);
                writer.Write((DocGrp.SelectedItem.GetUIElement() as TextBox).Text);
                writer.Close();
            }
        }

        private void BOpen_OnItemClick(object sender, ItemClickEventArgs e)
        {



            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".y";
            dlg.Filter = "JPEG Files (*.y)|*.y";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                // Open document 
                string filename = dlg.FileName;
                StreamReader reader = new StreamReader(filename);

                (DocGrp.SelectedItem.GetUIElement() as TextBox).Text = reader.ReadToEnd();
            }
        }

        private void BNew_OnItemClick(object sender, ItemClickEventArgs e)
        {
            index += 1;
            DocumentPanel dc = new DocumentPanel();
            dc.Caption = "Grammaire " + index ;
            //dc.ItemHeight = docPn.ItemHeight;
            dc.Tag = index ;
            
            TextBox tx = new TextBox();
            tx.VerticalAlignment = VerticalAlignment.Stretch;
            tx.HorizontalAlignment = HorizontalAlignment.Stretch;
            tx.AcceptsReturn = true;
            tx.VerticalScrollBarVisibility=ScrollBarVisibility.Visible;
            dc.Content = tx;
            
            DocGrp.Add(dc);
            DocGrp.SelectedTabIndex = index;

            TreeViewItem newChild = new TreeViewItem();
            newChild.Header = dc.Caption;
            newChild.IsExpanded = true;
           // newChild.IsSelected = true;
            newChild.Tag = index;
            tvGrammaire.Items.Add(newChild);
            
        }

        private int index = 0;

        private void TvGrammaire_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DocGrp.SelectedTabIndex = (int) ((sender as TreeView).SelectedItem as TreeViewItem).Tag;



        }

        private void BExp1_OnItemClick(object sender, ItemClickEventArgs e)
        {
            StreamReader red=new StreamReader("Exp1.y");
            (DocGrp.SelectedItem.GetUIElement() as TextBox).Text = red.ReadToEnd();
        }

        private void BExp2_OnItemClick(object sender, ItemClickEventArgs e)
        {
            StreamReader red = new StreamReader("Exp2.y");
            (DocGrp.SelectedItem.GetUIElement() as TextBox).Text = red.ReadToEnd();
        }
    }


}
//Structure D'un Programme Yacc
/*
 *{%
 * %}
 * %token tttt {Regular Expression}
 * %token rrrr {Regular Expression}
 * 
 * %%
 *Regular Expression Zone 
 *
 * tttt : gggg 'd' 'fff'
 *     |dddddddddddd
 *     ;
 * %%
 * Functions Zone
 * 
 */