namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Suivi de commande Hérite de la Classe Catégorie
    /// </summary>
    public class Suivi : Categorie
    {

        /// <summary>
        /// Constructeur de la Classe Suivi
        /// Herite de la Classe Catégorie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="libelle"></param>
        public Suivi(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
