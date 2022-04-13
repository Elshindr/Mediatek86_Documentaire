using System;


namespace Mediatek86.metier
{
    /// <summary>
    /// Classe  mére commande
    /// </summary>
    public class Commande
    {

        /// <summary>
        /// Constructeur de la Classe Commande
        /// </summary>
        /// <param name="idCommande"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="idSuivi"></param>
        /// <param name="label"></param>
        public Commande(string idCommande, DateTime dateCommande, double montant, string idSuivi, string label)
        {
            this.IdCommande = idCommande;
            this.DateCommande = dateCommande;
            this.Montant = montant;
            this.IdSuivi = idSuivi;
            this.Label = label;
        }

        public string IdCommande { get; set; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public string IdSuivi { get; set; }
        public string Label { get; set; }

    }
}
