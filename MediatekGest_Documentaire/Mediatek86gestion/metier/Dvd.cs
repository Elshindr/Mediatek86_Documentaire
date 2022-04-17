namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Dvd Hérite de LivreDvd
    /// </summary>
    public class Dvd : LivreDvd
    {

        private readonly int duree;
        private readonly string realisateur;
        private readonly string synopsis;

        /// <summary>
        /// Constructeur de la Classe Dvd
        /// Hérite des propriétés de LivreDvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="titre"></param>
        /// <param name="image"></param>
        /// <param name="duree"></param>
        /// <param name="realisateur"></param>
        /// <param name="synopsis"></param>
        /// <param name="idGenre"></param>
        /// <param name="genre"></param>
        /// <param name="idPublic"></param>
        /// <param name="lePublic"></param>
        /// <param name="idRayon"></param>
        /// <param name="rayon"></param>
        public Dvd(string id, string titre, string image, int duree, string realisateur, string synopsis,
            string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.duree = duree;
            this.realisateur = realisateur;
            this.synopsis = synopsis;
        }

        /// <summary>
        /// Getter de la propriété durée duree
        /// </summary>
        public int Duree { get => duree; }

        /// <summary>
        /// Getter de la propriété durée realisateur
        /// </summary>
        public string Realisateur { get => realisateur; }

        /// <summary>
        /// Getter de la propriété durée synopsis
        /// </summary>
        public string Synopsis { get => synopsis; }

    }
}
