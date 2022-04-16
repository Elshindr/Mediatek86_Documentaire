using Mediatek86.controleur;
using System;
using System.Windows.Forms;


namespace Mediatek86
{
    /// <summary>
    /// Classe d'entrée dans le programme
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new Controle();

        }
    }
}
