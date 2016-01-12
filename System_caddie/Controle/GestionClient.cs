using System;
using Microsoft.SPOT;

//rajout
using System.Threading;
using System.Net;

namespace System_caddie
{
    class GestionClient
    {
        //Definition des classes:
        private GestionServeur laGestionServeur;
        private Client leClient;
        private Boolean etatConnexionClient;
        private String requetteServeur;
        private String[] requetteRecusTab;

        public GestionClient(IPAddress adresseDuServeur, int port)
        {
            laGestionServeur = new GestionServeur(adresseDuServeur, port);
            etatConnexionClient = false;            
        }


        public Boolean connexionDuClient(String numeroTagClient)
        {
            requetteRecusTab = null;
            if (laGestionServeur.connexionServer())
            {
                //envoie au serveur les informations
                if (!laGestionServeur.envoyerMessageAuServer("client/" + numeroTagClient.ToLower()))
                {
                    requetteRecusTab = new string[2];
                    requetteRecusTab[0] = "err1";
                    requetteRecusTab[1] = Message.CONNEXIONSERVEURINTEROMPU;
                    return false;
                }
                requetteRecusTab = laGestionServeur.receptionMessageServeur();
                etatConnexionClient = traitementReponseServeur(requetteRecusTab);
                //
                if (etatConnexionClient == false)
                {
                    LaGestionServer.deconnexionServer();
                }
                Thread.Sleep(500);
                return etatConnexionClient;
            }
            else
            {
                requetteRecusTab = new string[2];
                requetteRecusTab[0] = "err1";
                requetteRecusTab[1] = Message.CONNEXIONSERVEURINTEROMPU;               
                return false;
            }
        }

        public void deconnexionClient()
        {
            etatConnexionClient = laGestionServeur.deconnexionServer();
        }

        public Boolean traitementReponseServeur(String[] requetteRecus)
        {
            try
            {
                switch (requetteRecus[0])
                {
                    case "error":
                        return false;                                           

                    default:
                        leClient = new Client(int.Parse(requetteRecus[2]), requetteRecus[0], requetteRecus[1], requetteRecus[3]);
                        return true;                       
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public Boolean EtatConnexionClient
        {
            get { return etatConnexionClient; }
        }

        public Client LeClient
        {
            get { return leClient; }
        }


        public String MessageErreurServeur
        {
            get { return requetteRecusTab[1]; }
        }

        public String TypeMessageErreurServeur
        {
            get { return requetteRecusTab[0]; }
        }

        internal GestionServeur LaGestionServer
        {
            get { return laGestionServeur; }
        }


    }
}
