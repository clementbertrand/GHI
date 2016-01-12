using System;
using System.Net;
using System.Threading;
using Gadgeteer.Modules.GHIElectronics;

using GTM = Gadgeteer.Modules;
using System.IO;

namespace System_caddie
{
    class GestionConnectionWifi
    {
        private Boolean etatConnexionWifi = false;
        //
        private String nomDeLaWifi;
        private String passwordWifi;
        //
        private WiFi_RS21 wifi_RS21;
        private SDCard carteSD;

        public GestionConnectionWifi(String nomWifi, String password)
        {
            this.wifi_RS21 = new GTM.GHIElectronics.WiFi_RS21(6);
            this.nomDeLaWifi = nomWifi;
            this.passwordWifi = password;           
        }

        public Boolean connexionWifi()
        {
            GHI.Premium.Net.WiFiNetworkInfo[] info = null;
            info = wifi_RS21.Interface.Scan(nomDeLaWifi);
            try
            {
                if (info != null)
                {
                    wifi_RS21.Interface.Join(info[0], passwordWifi);
                    //
                    Thread.Sleep(5000);
                    etatConnexionWifi = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void deconnexionWifi()
        {
            try
            {
                wifi_RS21.Interface.Disconnect();
            }
            catch (Exception ex)
            {
                wifi_RS21.Interface.Close();
            }
            etatConnexionWifi = false;
        }

        public Boolean EtatConnexionWifi
        {
            get { return etatConnexionWifi; }
            set { etatConnexionWifi = value; }
        }
    }
}
