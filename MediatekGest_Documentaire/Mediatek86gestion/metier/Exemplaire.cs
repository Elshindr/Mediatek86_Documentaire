using System;

namespace Mediatek86.metier
{
    /// <summary>
    /// Classe Exemplaire: un exemplaire d 'un document
    /// </summary>
    public class Exemplaire
    {
        /// <summary>
        /// Constructeur de la Classe Exemplaire
        /// </summary>
        /// <param name="numero"></param>
        /// <param name="dateAchat"></param>
        /// <param name="photo"></param>
        /// <param name="idEtat"></param>
        /// <param name="idDocument"></param>
        public Exemplaire(int numero, DateTime dateAchat, string photo, string idEtat, string idDocument)
        {
            this.Numero = numero;
            this.DateAchat = dateAchat;
            this.Photo = photo;
            this.IdEtat = idEtat;
            this.IdDocument = idDocument;
        }

        /// <summary>
        /// Getter et Setter de la propriété numero
        /// </summary>
        public int Numero { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété photo
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété dateAchat
        /// </summary>
        public DateTime DateAchat { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété IdEtat
        /// </summary>
        public string IdEtat { get; set; }

        /// <summary>
        /// Getter et Setter de la propriété IdDocument
        /// </summary>
        public string IdDocument { get; set; }
    }
}
