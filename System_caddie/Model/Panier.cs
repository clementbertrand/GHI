using System;
using Microsoft.SPOT;

namespace System_caddie
{
    class Panier
    {
        //Definition des variables de class:
        private int idPanier;
        private String prixPanier;
        private Produit dernierProduitAjoute;        
        private Boolean panierClos;

        public Panier ()
        {

        }



        public int IdPanier
        {
            get { return idPanier; }
            set { idPanier = value; }
        }

        public String PrixPanier
        {
            get { return prixPanier; }
            set { prixPanier = value; }
        }

        public Produit DernierProduitAjoute
        {
            get { return dernierProduitAjoute; }
            set { dernierProduitAjoute = value; }
        }

        public Boolean PanierClos
        {
            get { return panierClos; }
            set { panierClos = value; }
        }

    }
}
