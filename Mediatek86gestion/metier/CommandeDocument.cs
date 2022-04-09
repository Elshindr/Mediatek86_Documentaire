namespace Mediatek86.metier
{
    public class CommandeDocument
    {
        private string id;
        private string idLivreDvd;
        private int nbExemplaire;
        public CommandeDocument(string id, string idLivreDvd, int nbExemplaire)
        {
            this.Id = id;
            this.IdLivreDvd = idLivreDvd;
            this.NbExemplaire = nbExemplaire;
        }

        public string Id { get => id; set => id = value; }
        public string IdLivreDvd { get => idLivreDvd; set => idLivreDvd = value; }
        public int NbExemplaire { get => nbExemplaire; set => nbExemplaire = value; }
    }
}
