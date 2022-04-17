
namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Mére Document qui définie l'ensemble des propriétés de ses Classe Filles LivresDvd et Revue
    /// </summary>
    public class Document
    {

        private readonly string id;
        private readonly string titre;
        private readonly string image;
        private readonly string idGenre;
        private readonly string genre;
        private readonly string idPublic;
        private readonly string lePublic;
        private readonly string idRayon;
        private readonly string rayon;

        /// <summary>
        /// Constructeur de la classe Mére Document
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
        public Document(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon)
        {
            this.id = id;
            this.titre = titre;
            this.image = image;
            this.idGenre = idGenre;
            this.genre = genre;
            this.idPublic = idPublic;
            this.lePublic = lePublic;
            this.idRayon = idRayon;
            this.rayon = rayon;
        }

        /// <summary>
        /// Getter de la propriété id autogénéré
        /// </summary>
        public string Id { get => id; }

        /// <summary>
        /// Getter de la propriété titre autogénéré
        /// </summary>
        public string Titre { get => titre; }

        /// <summary>
        /// Getter de la propriété image autogénéré
        /// </summary>
        public string Image { get => image; }

        /// <summary>
        /// Getter de la propriété idGenre autogénéré
        /// </summary>
        public string IdGenre { get => idGenre; }

        /// <summary>
        /// Getter de la propriété genre autogénéré
        /// </summary>
        public string Genre { get => genre; }

        /// <summary>
        /// Getter de la propriété idPublic autogénéré
        /// </summary>
        public string IdPublic { get => idPublic; }

        /// <summary>
        /// Getter de la propriété lePublic autogénéré
        /// </summary>
        public string Public { get => lePublic; }

        /// <summary>
        /// Getter de la propriété idRayon autogénéré
        /// </summary>
        public string IdRayon { get => idRayon; }

        /// <summary>
        /// Getter de la propriété rayon autogénéré
        /// </summary>
        public string Rayon { get => rayon; }

    }


}
