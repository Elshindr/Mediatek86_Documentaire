using System;

namespace Mediatek86.metier
{
    public class Commande : CommandeDocument
    {

        private readonly DateTime dateCommande;
        private readonly double montant;
        private readonly string idSuivi;
        private readonly string label;

        public Commande(string idCommande, string idLivreDvd, int nbExemplaire, DateTime dateCommande, double montant, string idSuivi, string label)
        : base(idCommande, idLivreDvd, nbExemplaire)
        {
            this.DateCommande = dateCommande;
            this.Montant = montant;
            this.IdSuivi = idSuivi;
            this.Label = label;
        }

        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public string IdSuivi { get; set; }
        public string Label { get; set; }
    }
}
