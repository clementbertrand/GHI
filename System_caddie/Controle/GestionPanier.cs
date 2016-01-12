using System;
using Microsoft.SPOT;

namespace System_caddie
{
    class GestionPanier
    {
        //Definition des classes:
        private GestionServeur laGestionConnexionServer;
        private Panier lePanier;
        private String requetteServeur;
        private Produit dernierProduitDuPanier;
        private Boolean enModeSuppression;
        private String erreurRetourneParLeServer;

        

        

        public GestionPanier(GestionServeur connexionServeur)
        {
            lePanier = new Panier();
            enModeSuppression = false;
            laGestionConnexionServer = connexionServeur;            
        }


        public Boolean ajoutSupprimerProduitDansPanier(String numeroTagProduit)
        {
            //envoie au serveur les informations
            if (enModeSuppression == false)
            {
                requetteServeur = "add/" + numeroTagProduit.ToLower();
            }
            else
            {
                requetteServeur = "del/" + numeroTagProduit.ToLower();
            }
            //envoie de la requette au serveur
            if (laGestionConnexionServer.envoyerMessageAuServer(requetteServeur))
            {
                //reception de la requette serveur
                String[] requetteRecus = laGestionConnexionServer.receptionMessageServeur();
                return traitementReponseServeur(requetteRecus);
            }
            else
            {
                erreurRetourneParLeServer = "err1" + Message.CONNEXIONSERVEURINTEROMPU;
                return false;
            }
        }


        public Boolean cloturerLePanier()
        {
            //envoie au serveur les informations
            requetteServeur = "clospanier";
            //envoie de la requette au serveur
            if (laGestionConnexionServer.envoyerMessageAuServer(requetteServeur))
            {
                //reception de la requette serveur
                String[] requetteRecus = laGestionConnexionServer.receptionMessageServeur();
                return traitementReponseServeur(requetteRecus);
            }
            else
            {
                erreurRetourneParLeServer = "err1" + Message.CONNEXIONSERVEURINTEROMPU;                
                return false;
            }
        }

        public Boolean traitementReponseServeur(String[] requetteRecus)
        {
            try
            {
                switch (requetteRecus[0])
                {
                    case "error":
                        erreurRetourneParLeServer = requetteRecus[1];
                        return false;
                        break;

                    case"produit":
                        dernierProduitDuPanier = new Produit(requetteRecus[1], requetteRecus[2], requetteRecus[3]);
                        lePanier.PrixPanier = requetteRecus[3];
                        lePanier.PanierClos = false;
                        lePanier.DernierProduitAjoute = dernierProduitDuPanier;
                        return true;
                        break;

                    case "panierclos":
                        if (requetteRecus[1].Equals("true"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        break;

                    default:
                        return false;
                        break;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public String ErreurRetourneParLeServer
        {
            get { return erreurRetourneParLeServer; }
        }

        public Panier LePanier
        {
            get { return lePanier; }
            set { lePanier = value; }
        }


        public Boolean EnModeSuppression
        {
            get { return enModeSuppression; }
            set { enModeSuppression = value; }
        }
         
    }
}
