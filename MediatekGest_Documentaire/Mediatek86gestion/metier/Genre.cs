

namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Genre hérite de la Classe Categorie
    /// </summary>
    public class Genre : Categorie
    {

        /// <summary>
        /// Constructeur de la Classe Genre
        /// Herite de la Classe Categorie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="libelle"></param>
        public Genre(string id, string libelle) : base(id, libelle)
        {
        }

    }
}
