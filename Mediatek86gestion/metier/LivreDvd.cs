namespace Mediatek86.metier
{
    /// <summary>
    /// Classe LivreDvd Hérite de la Classe Document
    /// Classe Mere des Classes Livre et Dvd
    /// </summary>
    public abstract class LivreDvd : Document
    {
        /// <summary>
        /// Constructeur de la Classe LivreDvd
        /// Herite de la Classe Document
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
        protected LivreDvd(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
        }

    }
}

