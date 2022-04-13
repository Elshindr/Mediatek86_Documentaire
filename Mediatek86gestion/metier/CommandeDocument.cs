using System;

namespace Mediatek86.metier
{
    /// <summary>
    /// Classe CommandeDocument de gestion des Commandes de documents
    /// Hérite de la Classe Commande
    /// </summary>
    public class CommandeDocument : Commande
    {
        private string idLivreDvd;
        private int nbExemplaire;

        /// <summary>
        /// Constructeur de la classe CommandeDocument
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="nbExemplaire"></param>
        public CommandeDocument(string idCommande, DateTime dateCommande, double montant, string idSuivi, string label, string idLivreDvd, int nbExemplaire)
            : base(idCommande, dateCommande, montant, idSuivi, label)
        {
            this.IdLivreDvd = idLivreDvd;
            this.NbExemplaire = nbExemplaire;
        }

        public string IdLivreDvd { get => idLivreDvd; set => idLivreDvd = value; }
        public int NbExemplaire { get => nbExemplaire; set => nbExemplaire = value; }
    }
}
