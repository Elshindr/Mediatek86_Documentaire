using System;


namespace Mediatek86.metier
{
    public class Commande : CommandeDocument
    {

        /// <summary>
        /// Constructeur de la Classe Commande
        /// </summary>
        /// <param name="idCommande"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="nbExemplaire"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="idSuivi"></param>
        /// <param name="label"></param>
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
