using System;

namespace Mediatek86.metier
{
    /// <summary>
    /// Classe de gestion des Abonnements d'une revue
    /// Herite de la Classe Commande
    /// </summary>
    public class Abonnement : Commande
    {

        /// <summary>
        /// Variable privée de type string de l'identifiant de la revue
        /// </summary>
        private string idRevue;

        /// <summary>
        /// Variable privée de type DateTime de la date de fin d'abonnement
        /// </summary>
        private DateTime dateFinAbonnement;

        /// <summary>
        /// Constructeur de la classe Abonnement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idRevue"></param>
        /// <param name="dateFinAbonnement"></param>
        public Abonnement(string idCommande, DateTime dateCommande, double montant, string idSuivi, string label, string idRevue, DateTime dateFinAbonnement) :
            base(idCommande, dateCommande, montant, idSuivi, label)
        {
            this.IdRevue = idRevue;
            this.DateFinAbonnement = dateFinAbonnement;
        }


        /// <summary>
        /// Getter et Setter de la propriété idRevue autogénérés
        /// </summary>
        public string IdRevue { get => idRevue; set => idRevue = value; }

        /// <summary>
        /// Getter et Setter de la propriété dateFinAbonnement autogénérés
        /// </summary>
        public DateTime DateFinAbonnement { get => dateFinAbonnement; set => dateFinAbonnement = value; }
    }
}
