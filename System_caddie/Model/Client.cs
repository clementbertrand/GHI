using System;
using Microsoft.SPOT;

namespace System_caddie
{
    class Client
    {
        private int idClient;        
        private String nomClient;       
        private String prenomClient;        
        private String numeroTagRFIDClient;
        

        public Client(int idDuClient, String nomDuClient, String prenomDuClient, String numeroTaDugClient)
        {
            this.IdClient = idDuClient;
            this.NomClient = nomDuClient;
            this.PrenomClient = prenomDuClient;
            this.NumeroTagRFIDClient = numeroTaDugClient;
        }

        public int IdClient
        {
            get { return idClient; }
            set { idClient = value; }
        }

        public String NomClient
        {
            get { return nomClient; }
            set { nomClient = value; }
        }

        public String PrenomClient
        {
            get { return prenomClient; }
            set { prenomClient = value; }
        }

        public String NumeroTagRFIDClient
        {
            get { return numeroTagRFIDClient; }
            set { numeroTagRFIDClient = value; }
        }
    }
}
