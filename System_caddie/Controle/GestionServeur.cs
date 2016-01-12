using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.SPOT;
using GTM = Gadgeteer.Modules;
using System.Threading;

namespace System_caddie
{
    class GestionServeur
    {
        string result;
        //Declaration des objets
        private Boolean etatConnexionServeur = false;
        IPAddress adresseDuServeur;
        int portDuServeur;
        Socket socketDeConnexion;
        //
        Byte[] bytesReceived;
        StringBuilder messageRessussb = new StringBuilder();
        String[] chaineRecusSplit;

        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        //
        public GestionServeur(IPAddress adresseServeur, int port)
        {
            this.adresseDuServeur = adresseServeur;
            this.portDuServeur = port;
        }

        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        //
        public Boolean connexionServer()
        {
            try
            {
                //Preparation pour la connexion
                IPEndPoint ipe = new IPEndPoint(adresseDuServeur, portDuServeur);
                socketDeConnexion = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Connexion
                socketDeConnexion.Connect(ipe);
                Thread.Sleep(5000);   

                etatConnexionServeur = true;
                return etatConnexionServeur;
            }
            catch (Exception ex)
            {
                etatConnexionServeur = false;               
                return etatConnexionServeur;
            }
        }

        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        public Boolean deconnexionServer()
        {
            try
            {
                this.envoyerMessageAuServer("exit");
                //this.tempSocket.Close();
                etatConnexionServeur = false;
                return etatConnexionServeur;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        public Boolean envoyerMessageAuServer(String requette)
        {
            try
            {
                Byte[] bytesSent = Encoding.UTF8.GetBytes(requette);
                //Envoi du paquet
                socketDeConnexion.Send(bytesSent, bytesSent.Length, 0);
                return true;
            }
            catch (Exception ex)
            {
                return false;                
            }
        }

        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        public String[] receptionMessageServeur()
        {
            try
            {
                bytesReceived = new Byte[1500];
                int bytes = socketDeConnexion.Receive(bytesReceived, bytesReceived.Length, 0);
                //

                if (messageRessussb.Length > 0)
                {
                    messageRessussb.Clear();
                }

                for (int i = 0; i < bytesReceived.Length; i++)
                {
                    messageRessussb.Append((char)bytesReceived[i]);
                }

                chaineRecusSplit = messageRessussb.ToString().Split('/');

                return chaineRecusSplit;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        //********************************************************************************************************************************************************************
        public Boolean EtatConnexionServeur
        {
            get { return etatConnexionServeur; }
        }     
    }
}
