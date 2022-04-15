

namespace Mediatek86.metier
{/// <summary>
/// Classe Abctraicte Categorie 
/// Permet de récupérer les différents types de catégories
/// </summary>
    public abstract class Categorie
    {

        private readonly string id;
        private readonly string libelle;

        /// <summary>
        /// Constructeur de la classe mere Categorie
        /// </summary>
        /// <param name="id">un identifiant</param>
        /// <param name="libelle">un libelle</param>
        protected Categorie(string id, string libelle)
        {
            this.id = id;
            this.libelle = libelle;
        }

        /// <summary>
        /// Getter de la propriété Id autogénéré
        /// </summary>
        public string Id { get => id; }

        /// <summary>
        /// Getter ede la propriété libelle autogénéré
        /// </summary>
        public string Libelle { get => libelle; }

        /// <summary>
        /// Récupération du libellé pour l'affichage dans les combos
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.libelle;
        }

    }
}
