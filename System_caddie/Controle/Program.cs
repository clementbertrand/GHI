using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Input;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using System.Text;
using System.Net;
using Gadgeteer.Modules.GHIElectronics;
using System.IO;

namespace System_caddie
{
    public partial class Program
    {
        //Declaration des objets
        private Configuration laConfiguration;
        private GestionConnectionWifi laGestionConnectionWifi;
        private GestionAffichage laGestionAffichage;
        private GestionClient laGestionClient;
        private GestionTagRfid laGestionTagRfid;
        private WorkerChargement leThreadChargement;
        //       
        private GT.Timer timer;
        //
        private int secondeAttenteAvecNouvelEssaiConnection = 60000;
        //
        private Boolean autoriserPassageTagRFID = false;
        private Boolean autoriserTouchUPEcran = false;
        private Boolean informationsDeConnexionRecupere = false;

        //********************************************************************************************************************************************************************
        //******************************************************************************************************************************************************************** 
        /// <summary>
        /// This method is run when the mainboard is powered up or reset. 
        /// </summary>
        void ProgramStarted()
        {
            laConfiguration = new Configuration();
            laGestionAffichage = new GestionAffichage();
            //recuperation des informations de connexion sur la carte SD.

            recupererInformationsConnexion();
            if (!informationsDeConnexionRecupere)
            {
                this.laGestionAffichage.ecrireSurEcranLCD(Message.ERREURRECUPERATIONINFORMATIONSCARTESD, true);
                Thread.Sleep(30000);
                this.Reboot();
            }
            //Création des objets
            laGestionClient = new GestionClient(laConfiguration.IpDuServeur, laConfiguration.Port);
            laGestionTagRfid = new GestionTagRfid(laGestionClient);
            laGestionConnectionWifi = new GestionConnectionWifi(laConfiguration.NomWifi, laConfiguration.PasswordWifi);
            //
            this.creerLesEvenements();
            //
            leThreadChargement = new WorkerChargement(laGestionAffichage);
            // Permet de se connecter au reseau wifi, ainsi qu'au serveur, s'il echou, il recomence au bout du temp qui est defini, grace à la variable de classe
            // secondeAttenteAvecNouvelEssaiConnection. Si au bout de cinq tentives, il echou toujour, alors il s'arrete et demande a l'utilisteur de renouveler 
            // l'opperation ulterieurement.
            do
            {
                this.laGestionAffichage.ecrireSurEcranLCD(Message.CONNEXIONWIFI, false);
                try
                {
                    if (!this.laGestionConnectionWifi.connexionWifi())
                    {
                        this.laGestionAffichage.ecrireSurEcranLCD(Message.ECHECCONNEXIONWIFI + secondeAttenteAvecNouvelEssaiConnection / 60 + Message.MINUTES, true);
                        Thread.Sleep(secondeAttenteAvecNouvelEssaiConnection);
                    }
                }
                catch (Exception ex)
                {
                    this.laGestionAffichage.ecrireSurEcranLCD(ex.ToString(), true);
                }
                //Si la connexion est établie, on sort du "while".
            } while (this.laGestionConnectionWifi.EtatConnexionWifi == false);

            //Ceci, permet de lancer l'ecriture d'une information sur l'ecran, ce qsui va eviter que l'ecran du system devienne tout blanc.
            this.timer = new GT.Timer(50);
            this.timer.Tick += timer_Tick;
            this.timer.Start();
            this.leThreadChargement.leThread.Start();
            this.leThreadChargement.leThread.Suspend();
        }

        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************

        /// <summary>
        /// evenement reception tag rfid.
        /// </summary>
        /// <param name="nomRFID"></param>
        /// <param name="tag"></param>
        public void receptionTag(RFID nomRFID, String tag)
        {
            if (this.autoriserPassageTagRFID)
            {
                //on supprime le listener du tag, ce qui va eviter que le client ne passe plusieurs fois le meme produit par idnavertance. Le listener est recréé une fois que nous avons
                //fini le traitement du tag precedament detecté.
                this.interdirEvenements();
                Boolean typeDeMessage = false;
                //
                this.leThreadChargement.leThread.Resume();
                laGestionTagRfid.traitementDuTagRFID(tag);
                ArrayList reponse = laGestionTagRfid.reponseAAfficher;
                this.leThreadChargement.leThread.Suspend();
                //
                if (this.laGestionClient.EtatConnexionClient == true)
                {
                    this.laGestionAffichage.dessinerGraphiqueCaddie();
                    this.laGestionAffichage.passerEnModeAjout();
                    //Regarde si le message est un message d'erreur ou non.
                    typeDeMessage = this.chercherTypeDeMessage(reponse[reponse.Count - 1].ToString());
                    if (typeDeMessage == true)
                    {
                        if (reponse[0].ToString().Substring(0, 4).Equals("err1"))
                        {
                            this.laGestionAffichage.ecrireSurEcranLCD(reponse[0].ToString().Substring(5, reponse[0].ToString().Length), typeDeMessage);
                            Thread.Sleep(10000);
                            //
                            this.Reboot();
                        }                        
                        else
                        {
                            for (int i = 0; i < reponse.Count - 1; i++)
                            {
                                this.laGestionAffichage.ecrireSurEcranLCD(reponse[i].ToString(), typeDeMessage);
                            }
                        }
                    }
                    else
                    {
                        if (reponse[0].ToString().Substring(0, 4).Equals("err1"))
                        {
                            this.laGestionAffichage.ecrireSurEcranLCD(reponse[0].ToString().Substring(5, reponse[0].ToString().Length), typeDeMessage);
                            Thread.Sleep(10000);
                            //
                            this.laGestionConnectionWifi.deconnexionWifi();
                            this.laGestionConnectionWifi.connexionWifi();
                            //
                            this.lancerDeconnexionClient();
                        }
                        else
                        {
                            for (int i = 0; i < reponse.Count - 1; i++)
                            {
                                this.laGestionAffichage.ecrireSurEcranLCD(reponse[i].ToString(), typeDeMessage);
                            }
                        }
                    }
                }
                else
                {
                    //Regarde si le message est un message d'erreur ou non.
                    if (reponse[0].ToString().Equals("err1"))
                    {
                        typeDeMessage = this.chercherTypeDeMessage(reponse[reponse.Count - 1].ToString());
                        this.laGestionAffichage.ecrireSurEcranLCD(reponse[1].ToString(), typeDeMessage);
                        Thread.Sleep(10000);
                        this.Reboot();
                    }
                    else
                    {
                        typeDeMessage = this.chercherTypeDeMessage(reponse[reponse.Count - 1].ToString());
                        this.laGestionAffichage.effacerEcranLCD();
                        this.laGestionAffichage.ecrireSurEcranLCD(reponse[0].ToString(), typeDeMessage);
                        Thread.Sleep(10000);
                        this.laGestionAffichage.dessinerGraphiqueConnexionClient();
                    }
                }
                //
                if (this.laGestionClient.EtatConnexionClient)
                {
                    this.autoriserEvenements();
                }
                else
                {
                    autoriserPassageTagRFID = true;
                }
            }
        }

        //Cette methde, permet de convertir une valeur string en boolean, on lui passe le paramettre "true" ou "false", et le convertie ainsi en boolean.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valeurAComparer"></param>
        /// <returns></returns>
        private Boolean chercherTypeDeMessage(String valeurAComparer)
        {
            if (valeurAComparer.Equals("true"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Evenement qui se declanche quand l'utilisateur clique sur l'ecran lcd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evenementTouche"></param>
        void WPFWindow_TouchUp(object sender, TouchEventArgs evenementTouche)
        {
            if (this.autoriserTouchUPEcran)
            {
                switch (this.laGestionAffichage.rechercherLeRectangle((uint)evenementTouche.Touches[0].X, (uint)evenementTouche.Touches[0].Y))
                {
                    case (int)nomDesRectangles.menu:
                        this.laGestionAffichage.passerEnModeMenu();
                        break;

                    case (int)nomDesRectangles.supprimerProduit:
                        this.laGestionAffichage.passerEnModeSuppression();
                        this.laGestionTagRfid.laGestionPanier.EnModeSuppression = true;
                        break;

                    case (int)nomDesRectangles.terminerAchats:

                        if (laGestionTagRfid.laGestionPanier.cloturerLePanier())
                        {
                            this.laGestionClient.deconnexionClient();
                            this.interdirEvenements();
                            laGestionTagRfid.laGestionPanier.LePanier.PanierClos = true;
                            this.laGestionAffichage.dessinerPanierTermine();
                            this.laGestionAffichage.dessinerGraphiqueConnexionClient();
                            this.autoriserPassageTagRFID = true;
                        }
                        break;

                    case (int)nomDesRectangles.deconnexion:
                        this.laGestionAffichage.dessinerValiderAnnulerChoix(Message.DEMANDEVALIDATIONDECONNEXION);
                        break;

                    case (int)nomDesRectangles.validerChoix:
                        this.lancerDeconnexionClient();
                        break;

                    case (int)nomDesRectangles.sortirMenu:
                    case (int)nomDesRectangles.annulerModeSuppression:
                    case (int)nomDesRectangles.annulerChoix:
                        this.laGestionAffichage.ecrireSurEcranLCDLesDernieresInformationsPanier();
                        this.laGestionTagRfid.laGestionPanier.EnModeSuppression = false;
                        break;
                }
            }
        }


        /// <summary>
        /// Methode qui permet de deconncter le client du systeme, sans clors son panier.
        /// </summary>
        private void lancerDeconnexionClient()
        {
            this.laGestionClient.deconnexionClient();
            this.interdirEvenements();
            //
            this.laGestionAffichage.effacerEcranLCD();
            this.laGestionAffichage.ecrireSurEcranLCD(Message.REMERCIMENTCLOSPANIER, false);
            Thread.Sleep(9000);
            //
            this.laGestionAffichage.dessinerGraphiqueConnexionClient();
            this.autoriserPassageTagRFID = true;
        }


        /// <summary>
        /// Metode qui permet de creer les evenements.
        /// </summary>
        private void creerLesEvenements()
        {
            this.laGestionTagRfid.lecteurDeTag.CardIDReceived += new RFID.CardIDReceivedEventHandler(this.receptionTag);
            this.laGestionAffichage.display_T35.WPFWindow.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(this.WPFWindow_TouchUp);
            this.autoriserEvenements();
        }


        /// <summary>
        /// Metode qui permet de supprimer les evenements.
        /// </summary>
        private void supprimerEvenements()
        {
            this.laGestionTagRfid.lecteurDeTag.CardIDReceived -= new RFID.CardIDReceivedEventHandler(this.receptionTag);
            this.laGestionAffichage.display_T35.WPFWindow.TouchUp -= new Microsoft.SPOT.Input.TouchEventHandler(this.WPFWindow_TouchUp);
        }

        private void interdirEvenements()
        {
            this.autoriserPassageTagRFID = false;
            this.autoriserTouchUPEcran = false;
        }

        private void autoriserEvenements()
        {
            this.autoriserPassageTagRFID = true;
            this.autoriserTouchUPEcran = true;
        }

        private void timer_Tick(GT.Timer timer)
        {
            this.timer.Stop();
            this.timer.Tick -= timer_Tick;
            this.laGestionAffichage.dessinerGraphiqueConnexionClient();
        }

        /// <summary>
        /// Methode, qui permet de recuperer les informations du fichier config, situé sur la carte sd. Ce fichier doit avoir un format bien precis, voir documentation technique, 
        /// et etre à la racine du fichier. Atention il doit etre en premiere position sur la carte sd.
        /// </summary>
        private void recupererInformationsConnexion()
        {
            int indexTableauFichier = -1;
            String[] fichier = sdCard.GetStorageDevice().ListRootDirectoryFiles();
            for (int i = 0; i < fichier.Length; i++)
            {
                if(fichier[i].Equals("fichier_configuration.txt"))
                {
                    indexTableauFichier = i;
                }
            }

                if (indexTableauFichier !=-1)
                {
                try
                {
                    String contenuDuFichierST = null;
                    //Donne le chemin du fichier de configuration.

                    byte[] contenuDuFichier = sdCard.GetStorageDevice().ReadFile(fichier[indexTableauFichier]);
                    //
                    for (int i = 0; i < contenuDuFichier.Length; i++)
                    {
                        contenuDuFichierST += (char)contenuDuFichier[i];
                    }
                    //
                    string[] ligneTab;
                    ligneTab = contenuDuFichierST.Split(';');
                    //
                    if (ligneTab[0] != null)
                    {
                        laConfiguration.NomWifi = ligneTab[0];
                    }
                    else
                    {
                        informationsDeConnexionRecupere = false;
                    }
                    if (ligneTab[1] != null)
                    {
                        laConfiguration.PasswordWifi = ligneTab[1];
                    }
                    else
                    {
                        laConfiguration.PasswordWifi = "";
                    }
                    if (ligneTab[2] != null)
                    {
                        laConfiguration.IpDuServeur = IPAddress.Parse(ligneTab[2]);
                    }
                    else
                    {
                        informationsDeConnexionRecupere = false;
                    }
                    if (ligneTab[3] != null)
                    {
                        laConfiguration.Port = int.Parse(ligneTab[3]);
                    }
                    else
                    {
                        informationsDeConnexionRecupere = false;
                    }
                    informationsDeConnexionRecupere = true;
                }
                catch (Exception ex)
                {
                    informationsDeConnexionRecupere = false;
                }
                }
                else
                {
                    informationsDeConnexionRecupere = false;
                }
        }
    }
}