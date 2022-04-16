namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Utilisateur : Contient les informations utilisateur de son service d'affectation
    /// </summary>
    public class Utilisateur
    {
        private readonly int idUser;
        private readonly int idService;
        private readonly string label;

        /// <summary>
        /// Constructeur de la Classe Utilisateur
        /// </summary>
        /// <param name="idUser">Identifiant de l'utilisateur</param>
        /// <param name="idService">Identifiant du service d'affection</param>
        /// <param name="label">Label du service d'affection</param>
        public Utilisateur(int idUser, int idService, string label)
        {
            this.idUser = idUser;
            this.idService = idService;
            this.label = label;
        }


        /// <summary>
        /// Getter de la propriété idUser
        /// </summary>
        public int IdUser { get => idUser; }

        /// <summary>
        /// Getter de la propriété IdService
        /// </summary>
        public int IdService { get => idService; }

        /// <summary>
        /// Getter de la propriété Label
        /// </summary>
        public string Label { get => label; }
    }
}
