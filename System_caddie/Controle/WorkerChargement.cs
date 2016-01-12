using System;
using Microsoft.SPOT;
using System.Threading;

namespace System_caddie
{
    class WorkerChargement
    {

        private static GestionAffichage laGestionAffichageChargement;
        public Thread leThread = new Thread(run);
        public static Boolean flag = true;

        public WorkerChargement(GestionAffichage laGestionAffichage)
        {
            laGestionAffichageChargement = laGestionAffichage;
          
        }


        public static void run()
        {
            laGestionAffichageChargement.griserAffichage();
            do
            {
                laGestionAffichageChargement.dessinerChargement();
            } while (flag);

            
        }

        
    }
}
