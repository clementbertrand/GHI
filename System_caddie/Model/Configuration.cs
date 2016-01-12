using System;
using Microsoft.SPOT;
using System.Net;

namespace System_caddie
{
    class Configuration
    {

        private String nomWifi;

        public String NomWifi
        {
            get { return nomWifi; }
            set { nomWifi = value; }
        }
        private String passwordWifi;

        public String PasswordWifi
        {
            get { return passwordWifi; }
            set { passwordWifi = value; }
        }
        //
        private IPAddress ipDuServeur;

        public IPAddress IpDuServeur
        {
            get { return ipDuServeur; }
            set { ipDuServeur = value; }
        }
        private int port;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

    }
}
