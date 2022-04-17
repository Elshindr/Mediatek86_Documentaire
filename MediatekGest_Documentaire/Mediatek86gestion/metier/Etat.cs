

namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Etat : etat physique d'un document
    /// </summary>
    public class Etat
    {
        /// <summary>
        /// Constructeur de la Classe Etat
        /// </summary>
        /// <param name="id"></param>
        /// <param name="libelle"></param>
        public Etat(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        /// <summary>
        /// Getter et Setter de la propriété id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Getter et Setter de la propriété libelle
        /// </summary>
        public string Libelle { get; set; }
    }
}
