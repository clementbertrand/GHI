using System;
using Microsoft.SPOT;

namespace System_caddie.Model
{
    class ComposantGraphiqueRectangle
    {
        private uint coordonneXRectangle;      
        private uint coordonneYRectangle;        
        private uint largeurRectangle;        
        private uint hauteurRectangle;
        private int typeDeRectangle;        
        private String nomDuRectangle;
       
               

        public ComposantGraphiqueRectangle(uint coordonneXRectangle, uint coordonneYRectangle, uint largeurRectangle, uint hauteurRectangle, int typeDeRectangle,String nomDuRectangle)
        {
            this.coordonneXRectangle = coordonneXRectangle;
            this.coordonneYRectangle = coordonneYRectangle;
            this.largeurRectangle = largeurRectangle;
            this.hauteurRectangle = hauteurRectangle;
            this.typeDeRectangle = typeDeRectangle;
            this.nomDuRectangle = nomDuRectangle;
        }


        public uint CoordonneXRectangle
        {
            get { return coordonneXRectangle; }
            set { coordonneXRectangle = value; }
        }
        public uint CoordonneYRectangle
        {
            get { return coordonneYRectangle; }
            set { coordonneYRectangle = value; }
        }

        public uint LargeurRectangle
        {
            get { return largeurRectangle; }
            set { largeurRectangle = value; }
        }

        public uint HauteurRectangle
        {
            get { return hauteurRectangle; }
            set { hauteurRectangle = value; }
        }

        public int TypeDeRectangle
        {
            get { return typeDeRectangle; }
            set { typeDeRectangle = value; }
        }

        public String NomDuRectangle
        {
            get { return nomDuRectangle; }
            set { nomDuRectangle = value; }
        }
        
        public uint CoordonneX2Rectangle
        {
            get { return largeurRectangle + CoordonneXRectangle; }
        }
        public uint CoordonneY2Rectangle
        {
            get { return hauteurRectangle + CoordonneYRectangle; }
        }  

    }
}
