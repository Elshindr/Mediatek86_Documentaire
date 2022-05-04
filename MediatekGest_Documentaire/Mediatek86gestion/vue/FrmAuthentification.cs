using Mediatek86.controleur;
using Serilog;
using System;
using System.Windows.Forms;
namespace Mediatek86.vue
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
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Information()
                   .WriteTo.Console()
                   .WriteTo.File("logs/Auth_log.txt", rollingInterval: RollingInterval.Day)
                   .CreateLogger();

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
                    Log.Error("Erreur de Connexion, mauvaises informations : login='{0}' pwd='{1}'", txbLogin.Text, txbPwd.Text);
                }
                Log.Information("Connexion: login='{0}' pwd='{1}' date :{2}", txbLogin.Text, txbPwd.Text, DateTime.Now.ToString());

            }
            else
            {
                Log.Error("Erreur de Connexion, mauvaises informations : login='{0}', pwd='{1}'", txbLogin.Text, txbPwd.Text);
                MessageBox.Show("Un ou plusieurs champs sont vides", "Erreur Connexion");
            }

        }
    }
}
