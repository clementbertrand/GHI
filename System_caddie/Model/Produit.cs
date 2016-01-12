using System;
using Microsoft.SPOT;

namespace System_caddie
{
    class Produit
    {
        //Declaration des variables de class:
        private String nomProduit;
        private String prixProduit;
        private String numeroTagProduit;


        public Produit(String nomDuProduit, String prixDuProduit, String numeroTagDuProduit)
        {
            this.nomProduit = nomDuProduit;
            this.prixProduit = prixDuProduit;
            this.numeroTagProduit = numeroTagDuProduit;
        }  

        public String NomProduit
        {
            get { return nomProduit; }
            set { nomProduit = value; }
        }        

        public String PrixProduit
        {
            get { return prixProduit; }
            set { prixProduit = value; }
        }        

        public String NumeroTagProduit
        {
            get { return numeroTagProduit; }
            set { numeroTagProduit = value; }
        }

        

    }
}
