using Mediatek86.controleur;
using System.Windows.Forms;

namespace Mediatek86
{
    /// <summary>
    /// Classe FrmAuthentification Windows Form
    /// Permet l'affichage de la fenêtre d'authentification à l'application
    /// Herite de la Classe Form
    /// </summary>
    public partial class FrmAuthentification : Form
    {
        private readonly Controle controle;

        /// <summary>
        /// Constructeur du Windows Form FrmAuthentification
        /// Herite de la Classe Form
        /// </summary>
        /// <param name="controle">Instance du controleur</param>
        internal FrmAuthentification(Controle controle)
        {
            InitializeComponent();
            this.controle = controle;
        }


        private void btnConnexion_Click(object sender, System.EventArgs e)
        {
            if (txbLogin.Text != "" && txbPwd.Text != "")
            {
                bool acces = controle.Authentification(txbLogin.Text, txbPwd.Text);

                if (!acces)
                {
                    MessageBox.Show("Mauvais mot de passe ou login", "Erreur Connexion");
                }
            }
            else
            {
                MessageBox.Show("Un ou plusieurs champs sont vides", "Erreur Connexion");
            }

        }
    }
}
