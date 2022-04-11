namespace Mediatek86.metier
{
    public class Suivi : Categorie
    {

        private readonly string id;
        private readonly string libelle;

        public Suivi(string id, string libelle) : base(id, libelle)
        {
            this.id = id;
            this.libelle = libelle;
        }

        public string Id { get => id; }
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
