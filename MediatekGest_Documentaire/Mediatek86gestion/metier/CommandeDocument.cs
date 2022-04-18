using System;

namespace Mediatek86.metier
{
    /// <summary>
    /// Classe CommandeDocument de gestion des Commandes de documents
    /// Hérite de la Classe Commande
    /// </summary>
    public class CommandeDocument : Commande
    {

        /// <summary>
        /// Constructeur de la classe CommandeDocument
        /// Herite des propriétés de la classe Commande
        /// </summary>
        /// <param name="idCommande"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="idSuivi"></param>
        /// <param name="label"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="nbExemplaire"></param>
        public CommandeDocument(string idCommande, DateTime dateCommande, double montant, string idSuivi, string label, string idLivreDvd, int nbExemplaire)
            : base(idCommande, dateCommande, montant, idSuivi, label)
        {
            this.IdLivreDvd = idLivreDvd;
            this.NbExemplaire = nbExemplaire;
        }
        /// <summary>
        /// Getter et Setter de la propriété IdLivreDvd autogénérés
        /// </summary>
        public string IdLivreDvd { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété NbExemplaire autogénérés
        /// </summary>
        public int NbExemplaire { get; set; }
    }
}
