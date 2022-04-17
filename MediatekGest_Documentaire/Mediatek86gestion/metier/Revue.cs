
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Revue qui hérite de la Classe Document. 
    /// </summary>
    public class Revue : Document
    {
        /// <summary>
        /// Constructeur de la classe Revue
        /// Hérite des propriétés de la Classe Document
        /// Création de ses propriétés internes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="titre"></param>
        /// <param name="image"></param>
        /// <param name="idGenre"></param>
        /// <param name="genre"></param>
        /// <param name="idPublic"></param>
        /// <param name="lePublic"></param>
        /// <param name="idRayon"></param>
        /// <param name="rayon"></param>
        /// <param name="empruntable"></param>
        /// <param name="periodicite"></param>
        /// <param name="delaiMiseADispo"></param>
        public Revue(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon,
            bool empruntable, string periodicite, int delaiMiseADispo)
             : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            Periodicite = periodicite;
            Empruntable = empruntable;
            DelaiMiseADispo = delaiMiseADispo;
        }

        /// <summary>
        /// Getter et Setter de la propriété periodicite
        /// </summary>
        public string Periodicite { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété empruntable
        /// </summary>
        public bool Empruntable { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété delaiMiseADispo
        /// </summary>
        public int DelaiMiseADispo { get; set; }
    }
}
