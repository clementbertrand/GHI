using System;
using Microsoft.SPOT;
using System.Collections;
using System_caddie.Model;

namespace System_caddie
{
    class GestionRectangle
    {
        ComposantGraphiqueRectangle leRectangle;        
        public ArrayList listeDesRectangle;


        public GestionRectangle()
        {
            listeDesRectangle = new ArrayList();
            listeDesRectangle.Add(leRectangle);
        }


        public void ajouterRectangleALaListe(uint coordonneXRectangle, uint coordonneYRectangle, uint largeurRectangle, uint hauteurRectangle, int typeDeRectangle, String nomDuRectangle)
        {
            leRectangle = new ComposantGraphiqueRectangle(coordonneXRectangle, coordonneYRectangle, largeurRectangle, hauteurRectangle, typeDeRectangle,nomDuRectangle);
            listeDesRectangle.Add(leRectangle);
        }       
    }
}
