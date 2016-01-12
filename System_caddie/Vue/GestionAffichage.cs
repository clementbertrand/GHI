using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using System.Threading;
using System_caddie.Model;
using Gadgeteer.Modules.GHIElectronics;

using GTM = Gadgeteer.Modules;


namespace System_caddie
{
    public enum nomDesRectangles { supprimerProduit = 1, annulerModeSuppression, menu, deconnexion, terminerAchats, sortirMenu, validerChoix, annulerChoix };

    class GestionAffichage
    {
        public Display_T35 display_T35;
        private int longueurMAxChaineCaractere = 41;
        //
        private ComposantGraphiqueRectangle leRectangle;
        private GestionRectangle laGestionRectangle;
        //
        private ArrayList listLigne;
        private Boolean dernierMessageAfficheTypeErreur;
        //
        private int numeroLigneActuel;
        private const int NOMBRE_LIGNES_MAXIMUM = 12;
        private const int HAUTEUR_LIGNE = 17;
        //
        private Boolean modeMenu;
        private Boolean modeSupprimerProduit;
        private Boolean modeValiderChoix;

        //
        private static String MESSAGE_ATTENTE_CONNEXION_CLIENT = Message.DEMANDECONNEXIONCLIENT;

        private enum typeDeRectangle
        {
            ModeCaddie = 1,
            ModeMenu,
            ModeSupprimmeUnProduit,
            ModeValiderDeconnexion,
            ModeValiderChoix,
        };

        //
        //Definition des alignement de l'ecran
        private uint alignementVertical;
        private uint alignementHorizontal;
        //
        Bitmap bmp = new Bitmap(320, 240);

        /// <summary>
        /// Constructeur Principal
        /// </summary>
        /// <param name="display_T35"></param>
        public GestionAffichage()
        {
            laGestionRectangle = new GestionRectangle();
            ajouterLesRectangles();
            //
            this.display_T35 = new GTM.GHIElectronics.Display_T35(14, 13, 12, 10);
            //
            display_T35.SimpleGraphics.AutoRedraw = true;
            display_T35.WPFWindow.Invalidate();
            //Innitialisation des tableau pour les lignes.
            listLigne = new ArrayList();
            //
            effacerEcranLCD();
            modeMenu = false;
            modeSupprimerProduit = false;
            modeValiderChoix = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ajouterLesRectangles()
        {
            //ajout du rectangle supprimer un produit
            laGestionRectangle.ajouterRectangleALaListe(50, 210, 180, 30, (int)typeDeRectangle.ModeCaddie, Message.BOUTONSUPPRIMERUNPRODUIT);

            //ajout du rectangle annuler supprimer un produit
            laGestionRectangle.ajouterRectangleALaListe(100, 90, 75, 35, (int)typeDeRectangle.ModeSupprimmeUnProduit, Message.BOUTONANNULER);

            //ajout du rectangle menu
            laGestionRectangle.ajouterRectangleALaListe(235, 210, 85, 30, (int)typeDeRectangle.ModeCaddie, Message.BOUTONMENU);

            //ajout du rectangle deconnexion client
            laGestionRectangle.ajouterRectangleALaListe(60, 10, 145, 30, (int)typeDeRectangle.ModeMenu, Message.BOUTONDECONNEXION);

            //ajout du rectangle terminer le panier
            laGestionRectangle.ajouterRectangleALaListe(60, 50, 145, 30, (int)typeDeRectangle.ModeMenu, Message.BOUTONTERMINERPANIER);

            //ajout du rectangle Sortir du menu
            laGestionRectangle.ajouterRectangleALaListe(60, 160, 145, 30, (int)typeDeRectangle.ModeMenu, Message.BOUTONANNULER);

            //ajout du rectangle valider choix
            laGestionRectangle.ajouterRectangleALaListe(60, 160, 100, 30, (int)typeDeRectangle.ModeValiderChoix, Message.BOUTONVALIDER);

            //ajout rectangle annuler choix.
            laGestionRectangle.ajouterRectangleALaListe(170, 160, 100, 30, (int)typeDeRectangle.ModeValiderChoix, Message.BOUTONANNULER);
        }


        /// <summary>
        /// Methode qui permet d'ecrire sur l'ecran lcd, en lui passant en parametre une chaine de caracter, qui sera decoupé si besoin.
        /// </summary>
        /// <param name="textAEcrir"></param>
        public void ecrireSurEcranLCD(String textAEcrir, Boolean messageDeTypeErreur)
        {
            //max41 carac
            Boolean dejaCoupe = false;
            int indexDeDepart = 0;
            listLigne.Clear();
            if (textAEcrir.Length > longueurMAxChaineCaractere)
            {
                do
                {
                    //verifie si le nombre de caractere restant a afficher est supperieur au nombre de caracteres maximal.
                    if ((textAEcrir.Length - indexDeDepart) > longueurMAxChaineCaractere)
                    {
                        if ((textAEcrir.Length - indexDeDepart >= indexDeDepart + longueurMAxChaineCaractere + 1) && !dejaCoupe)
                        {
                            if (textAEcrir.Substring(indexDeDepart, indexDeDepart + longueurMAxChaineCaractere + 1).Equals(' '))
                            {
                                listLigne.Add(textAEcrir.Substring(indexDeDepart, indexDeDepart + longueurMAxChaineCaractere));
                                indexDeDepart = indexDeDepart + longueurMAxChaineCaractere + 2;
                                dejaCoupe = true;
                            }
                        }
                        if ((textAEcrir.Length - indexDeDepart >= indexDeDepart + longueurMAxChaineCaractere + 1) && !dejaCoupe)
                        {
                            if (textAEcrir.Substring(indexDeDepart, indexDeDepart + longueurMAxChaineCaractere).Equals(' '))
                            {
                                listLigne.Add(textAEcrir.Substring(indexDeDepart, indexDeDepart + longueurMAxChaineCaractere));
                                indexDeDepart = indexDeDepart + longueurMAxChaineCaractere + 1;
                                dejaCoupe = true;
                            }
                        }
                        if(!dejaCoupe)
                        {
                            int indexEspace = textAEcrir.LastIndexOf(' ', indexDeDepart, longueurMAxChaineCaractere);
                            if (indexEspace != -1)
                            {
                                listLigne.Add(textAEcrir.Substring(indexDeDepart, indexEspace - indexDeDepart));
                                indexDeDepart = indexEspace + 1;
                            }
                            else
                            {
                                listLigne.Add(textAEcrir.Substring(indexDeDepart, longueurMAxChaineCaractere));
                                indexDeDepart = longueurMAxChaineCaractere;
                            }
                        }
                    }
                    else
                    {
                        listLigne.Add(textAEcrir.Substring(indexDeDepart, textAEcrir.Length - indexDeDepart));
                        indexDeDepart = textAEcrir.Length;
                    }
                    dejaCoupe = false;
                } while (textAEcrir.Length != indexDeDepart);
            }
            else
            {
                listLigne.Add(textAEcrir);
            }
            //
            ecrireSurEcranLCD(listLigne, messageDeTypeErreur);
        }

        /// <summary>
        /// Permet d'ecrire sur l'ecran lcd. Atention, chaque ligne du tableau doivent faire au plus 38 caractères.
        /// </summary>
        /// <param name="listAEcrir"></param>
        public void ecrireSurEcranLCD(ArrayList listAEcrir, Boolean messageDeTypeErreur)
        {
            dernierMessageAfficheTypeErreur = messageDeTypeErreur;
            if ((numeroLigneActuel + listAEcrir.Count) >= NOMBRE_LIGNES_MAXIMUM)
            {
                effacerEcranLCD();
            }

            for (int i = 1; i < listAEcrir.Count + 1; i++)
            {
                alignementVertical += HAUTEUR_LIGNE;
                if (messageDeTypeErreur)
                {
                    display_T35.SimpleGraphics.DisplayText(listAEcrir[i - 1].ToString(), Resources.GetFont(Resources.FontResources.NinaB),
                        Colors.Red, alignementHorizontal, alignementVertical);
                }
                else
                {
                    display_T35.SimpleGraphics.DisplayText(listAEcrir[i - 1].ToString(), Resources.GetFont(Resources.FontResources.NinaB),
                        Colors.White, alignementHorizontal, alignementVertical);
                }
                numeroLigneActuel++;
            }
        }


        /// <summary>
        /// Metode qui perlmet de reecrire les dernieres inforations correspondants au panier.
        /// </summary>
        public void ecrireSurEcranLCDLesDernieresInformationsPanier()
        {
            ModeSupprimerProduit = false;
            ModeMenu = false;
            ModeValiderChoix = false;
            this.dessinerGraphiqueCaddie();
            this.ecrireSurEcranLCD(listLigne, dernierMessageAfficheTypeErreur);
        }

        /// <summary>
        /// Permet d'effacer l'ecran lcd.
        /// </summary>
        public void effacerEcranLCD()
        {
            display_T35.SimpleGraphics.Clear();
            alignementVertical = 20;
            alignementHorizontal = 0;
            numeroLigneActuel = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public void dessinerGraphiqueCaddie()
        {
            effacerEcranLCD();
            dessinerBoutonSupprimerProduit();
            dessinerBoutonMenu();
        }

        /// <summary>
        /// 
        /// </summary>
        public void dessinerGraphiqueConnexionClient()
        {
            this.effacerEcranLCD();
            ecrireSurEcranLCD(MESSAGE_ATTENTE_CONNEXION_CLIENT, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="TextAAfficher"></param>
        public void dessinerValiderAnnulerChoix(String TextAAfficher)
        {
            this.ModeSupprimerProduit = false;
            this.ModeMenu = false;
            this.ModeValiderChoix = true;
            this.effacerEcranLCD();
            //Ecrire la variable dur l'ecran
            display_T35.SimpleGraphics.DisplayTextInRectangle(TextAAfficher, 25, 6, 200, 50, Colors.White, Resources.GetFont(Resources.FontResources.NinaB));
            //
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.validerChoix];
            //
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 25, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.White, Resources.GetFont(Resources.FontResources.NinaB));
            //
            //
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.annulerChoix];
            //
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 25, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.White, Resources.GetFont(Resources.FontResources.NinaB));
        }

        /// <summary>
        /// 
        /// </summary>
        private void dessinerBoutonSupprimerProduit()
        {
            this.effacerEcranLCD();
            //
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.supprimerProduit];
            //
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 15, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.White, Resources.GetFont(Resources.FontResources.NinaB));
        }

        /// <summary>
        /// 
        /// </summary>
        private void dessinerBoutonMenu()
        {
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.menu];
            //
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 25, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.White, Resources.GetFont(Resources.FontResources.NinaB));

        }

        /// <summary>
        /// 
        /// </summary>
        private void dessinerBoutonDeconnexion()
        {
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.deconnexion];
            //
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 20, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.Black, Resources.GetFont(Resources.FontResources.NinaB));

        }

        /// <summary>
        /// 
        /// </summary>
        private void dessinerBoutonValiderPanier()
        {
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.terminerAchats];
            //
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 3, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.Black, Resources.GetFont(Resources.FontResources.NinaB));
        }

        /// <summary>
        /// 
        /// </summary>
        private void dessinerBoutonSortirDuMenu()
        {
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.sortirMenu];
            //
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 20, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.Black, Resources.GetFont(Resources.FontResources.NinaB));
        }

        /// <summary>
        /// 
        /// </summary>
        public void passerEnModeSuppression()
        {
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.annulerModeSuppression];
            //
            //Dessiner le fond de l'ecran
            griserAffichage();
            //
            display_T35.SimpleGraphics.DisplayTextInRectangle(Message.DEMANDETAGCLIENT, 10, 10
                , 250, 100, Colors.White, Resources.GetFont(Resources.FontResources.NinaB));
            //Dessiner le fond du bouton
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Red, 0, Colors.Orange, leRectangle.CoordonneXRectangle, leRectangle.CoordonneYRectangle
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, 256);
            //Ecrir dans le rectangle.
            display_T35.SimpleGraphics.DisplayTextInRectangle(leRectangle.NomDuRectangle, leRectangle.CoordonneXRectangle + 5, leRectangle.CoordonneYRectangle + 6
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.Black, Resources.GetFont(Resources.FontResources.NinaB));
            //
            modeSupprimerProduit = true;
            modeMenu = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void passerEnModeMenu()
        {
            griserAffichage();
            //
            dessinerBoutonSortirDuMenu();
            dessinerBoutonDeconnexion();
            dessinerBoutonValiderPanier();
            modeMenu = true;
            modeSupprimerProduit = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void passerEnModeAjout()
        {
            ModeSupprimerProduit = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void dessinerPanierTermine()
        {
            leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[(int)nomDesRectangles.sortirMenu];
            //
            display_T35.SimpleGraphics.Clear();
            //            
            display_T35.SimpleGraphics.DisplayTextInRectangle(Message.FERMETUREPANIER
                , 75, 60, leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.Orange, Resources.GetFont(Resources.FontResources.NinaB));
            Thread.Sleep(15000);
            display_T35.SimpleGraphics.Clear();
            display_T35.SimpleGraphics.DisplayTextInRectangle(Message.REMERCIMENTFINPANIER, 75, 60
                , leRectangle.LargeurRectangle, leRectangle.HauteurRectangle, Colors.Orange, Resources.GetFont(Resources.FontResources.NinaB));
            modeMenu = false;
            modeSupprimerProduit = false;
        }

        /// <summary>
        /// Permet de dessiner le chargement en cours, ce qui permet de ne pas laisser figé le systeme.
        /// </summary>
        public void dessinerChargement()
        {
            this.griserAffichage();
            display_T35.SimpleGraphics.DisplayTextInRectangle(Message.CHARGEMENT + " .", 75, 60
                , 80, 30, Colors.Orange, Resources.GetFont(Resources.FontResources.NinaB));
            Thread.Sleep(1000);
            display_T35.SimpleGraphics.DisplayTextInRectangle(Message.CHARGEMENT + " ..", 75, 60
                , 80, 30, Colors.Orange, Resources.GetFont(Resources.FontResources.NinaB));
            Thread.Sleep(1000);
            display_T35.SimpleGraphics.DisplayTextInRectangle(Message.CHARGEMENT + " ...", 75, 60
                , 80, 30, Colors.Orange, Resources.GetFont(Resources.FontResources.NinaB));
            Thread.Sleep(1000);
        }

        /// <summary>
        /// 
        /// </summary>
        public void griserAffichage()
        {
            //Dessiner le fond de l'ecran
            display_T35.SimpleGraphics.DisplayRectangle(Colors.Yellow, 0, Colors.Gray, 0, 0, 320, 240, 200);
        }

        /// <summary>
        /// Cette metode, permet de rechercher le rectangle qui a était selectionné.
        /// </summary>
        /// <param name="coordonneX"></param>
        /// <param name="coordonneY"></param>
        /// <returns></returns>
        public int rechercherLeRectangle(uint coordonneX, uint coordonneY)
        {
            if (modeMenu)
            {
                for (int i = 1; i < laGestionRectangle.listeDesRectangle.Count; i++)
                {
                    ComposantGraphiqueRectangle leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[i];
                    if (leRectangle.TypeDeRectangle == (int)typeDeRectangle.ModeMenu)
                    {
                        if ((coordonneX >= leRectangle.CoordonneXRectangle) && (coordonneX <= leRectangle.CoordonneX2Rectangle))
                        {
                            if ((coordonneY >= leRectangle.CoordonneYRectangle) && (coordonneY <= leRectangle.CoordonneY2Rectangle))
                                return i;
                        }
                    }
                }
                return 0;
            }
            else if (modeSupprimerProduit)
            {
                for (int i = 1; i < laGestionRectangle.listeDesRectangle.Count; i++)
                {
                    ComposantGraphiqueRectangle leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[i];
                    if (leRectangle.TypeDeRectangle == (int)typeDeRectangle.ModeSupprimmeUnProduit)
                    {
                        if ((coordonneX >= leRectangle.CoordonneXRectangle) && (coordonneX <= leRectangle.CoordonneX2Rectangle))
                        {
                            if ((coordonneY >= leRectangle.CoordonneYRectangle) && (coordonneY <= leRectangle.CoordonneY2Rectangle))
                                return i;
                        }
                    }
                }
                return 0;
            }
            else if (modeValiderChoix)
            {
                for (int i = 1; i < laGestionRectangle.listeDesRectangle.Count; i++)
                {
                    ComposantGraphiqueRectangle leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[i];
                    if (leRectangle.TypeDeRectangle == (int)typeDeRectangle.ModeValiderChoix)
                    {
                        if ((coordonneX >= leRectangle.CoordonneXRectangle) && (coordonneX <= leRectangle.CoordonneX2Rectangle))
                        {
                            if ((coordonneY >= leRectangle.CoordonneYRectangle) && (coordonneY <= leRectangle.CoordonneY2Rectangle))
                                return i;
                        }
                    }
                }
                return 0;
            }
            else
            {
                for (int i = 1; i < laGestionRectangle.listeDesRectangle.Count; i++)
                {
                    ComposantGraphiqueRectangle leRectangle = (ComposantGraphiqueRectangle)laGestionRectangle.listeDesRectangle[i];
                    if (leRectangle.TypeDeRectangle == (int)typeDeRectangle.ModeCaddie)
                    {
                        if ((coordonneX >= leRectangle.CoordonneXRectangle) && (coordonneX <= leRectangle.CoordonneX2Rectangle))
                        {
                            if ((coordonneY >= leRectangle.CoordonneYRectangle) && (coordonneY <= leRectangle.CoordonneY2Rectangle))
                            {
                                return i;
                            }
                        }
                    }
                }
                return 0;
            }
        }



        //-----------------------------------------------------//
        //get et set pour le mode de fonctionnement
        public Boolean ModeMenu
        {
            get { return modeMenu; }
            set { modeMenu = value; }
        }

        public Boolean ModeSupprimerProduit
        {
            get { return modeSupprimerProduit; }
            set { modeSupprimerProduit = value; }
        }
        public Boolean ModeValiderChoix
        {
            get { return modeValiderChoix; }
            set { modeValiderChoix = value; }
        }
    }
}
