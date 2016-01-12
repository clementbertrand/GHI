using System;
using Microsoft.SPOT;
using GTM = Gadgeteer.Modules;

using Gadgeteer.Modules.GHIElectronics;
using System.Collections;

namespace System_caddie
{
    class GestionTagRfid
    {
        public RFID lecteurDeTag;
        public ArrayList reponseAAfficher;
        public GestionPanier laGestionPanier;
        private GestionClient laGestionClient;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="leLecteurDeTag"></param>
        /// <param name="laGestionClient"></param>
        public GestionTagRfid(GestionClient laGestionClient)
        {
            this.lecteurDeTag = new GTM.GHIElectronics.RFID(8);
            reponseAAfficher = new ArrayList();
            this.laGestionClient = laGestionClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomRFID"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public void traitementDuTagRFID(String tag)
        {
            reponseAAfficher = new ArrayList();
            //
            if (laGestionClient.EtatConnexionClient == true)
            {
                if (laGestionPanier.EnModeSuppression == false)
                {
                    ajouterUnProduit(tag);
                }
                else
                {
                    supprimerUnProduit(tag);
                }
            }
            else
            {
                connexionDuClient(tag);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        private void connexionDuClient(String tag)
        {
            if (laGestionClient.connexionDuClient(tag) == true)
            {
                laGestionPanier = new GestionPanier(laGestionClient.LaGestionServer);
                reponseAAfficher.Add(Message.BONJOUR + laGestionClient.LeClient.NomClient + " " + laGestionClient.LeClient.PrenomClient + Message.COMMENCERCOURSES);
                reponseAAfficher.Add("false");
            }
            else
            {
                if (laGestionClient.TypeMessageErreurServeur.Equals("err1"))
                {
                    reponseAAfficher.Add(laGestionClient.TypeMessageErreurServeur);
                    reponseAAfficher.Add(laGestionClient.MessageErreurServeur);
                    reponseAAfficher.Add("true");
                }
                else
                {
                    reponseAAfficher.Add(Message.IMPOSSIBLECONNEXIONSERVEUR + laGestionClient.MessageErreurServeur);
                    reponseAAfficher.Add("true");
                }
            }
        }



        /// <summary>
        /// si la methode retourne false, ceci veut dire que la connexion avec le serveur est indisponible.
        /// </summary>
        /// <param name="tag"></param>
        private void ajouterUnProduit(String tag)
        {
            if (laGestionPanier.ajoutSupprimerProduitDansPanier(tag))
            {
                reponseAAfficher.Add(Message.NOMPRODUIT + laGestionPanier.LePanier.DernierProduitAjoute.NomProduit + Message.ETANTAUPRIX + laGestionPanier.LePanier.DernierProduitAjoute.PrixProduit + Message.ABIENETAITAJOUTE);
                reponseAAfficher.Add(Message.PRIXTOTALPANIER + laGestionPanier.LePanier.PrixPanier);
                reponseAAfficher.Add("false");
            }
            else
            {
                if (laGestionPanier.ErreurRetourneParLeServer.Substring(0, 4).Equals("err1"))
                {
                    reponseAAfficher.Add(laGestionPanier.ErreurRetourneParLeServer);
                    reponseAAfficher.Add("true");
                }
                else
                {
                    reponseAAfficher.Add(Message.ERREURAJOUTPRODUIT + laGestionPanier.ErreurRetourneParLeServer);
                    reponseAAfficher.Add("true");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        private void supprimerUnProduit(String tag)
        {
            if (laGestionPanier.ajoutSupprimerProduitDansPanier(tag))
            {
                reponseAAfficher.Add(Message.NOMPRODUIT + laGestionPanier.LePanier.DernierProduitAjoute.NomProduit + Message.ETANTAUPRIX + laGestionPanier.LePanier.DernierProduitAjoute.PrixProduit + Message.ABIENETAITSUPPRIME);
                reponseAAfficher.Add(Message.PRIXTOTALPANIER + laGestionPanier.LePanier.PrixPanier);
                reponseAAfficher.Add("false");
            }
            else
            {
                reponseAAfficher.Add(Message.ERREURSUPPRESSIONPRODUIT + laGestionPanier.ErreurRetourneParLeServer);
                reponseAAfficher.Add("true");
            }

            laGestionPanier.EnModeSuppression = false;
        }

    }
}
