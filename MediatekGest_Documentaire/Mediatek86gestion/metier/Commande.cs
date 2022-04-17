using System;


namespace Mediatek86.metier
{
    /// <summary>
    /// Classe mére commande
    /// </summary>
    public class Commande
    {

        /// <summary>
        /// Constructeur de la Classe mere Commande
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

        /// <summary>
        /// Getter et Setter de la propriété IdCommande autogénérés
        /// </summary>
        public string IdCommande { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété DateCommande autogénérés
        /// </summary>
        public DateTime DateCommande { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété Montant autogénérés
        /// </summary>
        public double Montant { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété IdSuivi autogénérés
        /// </summary>
        public string IdSuivi { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété Label autogénérés
        /// </summary>
        public string Label { get; set; }

    }
}
