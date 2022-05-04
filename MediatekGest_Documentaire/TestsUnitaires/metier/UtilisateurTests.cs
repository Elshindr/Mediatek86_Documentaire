using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mediatek86.metier.Tests
{
    [TestClass()]
    public class UtilisateurTests
    {
        private const int idUser = 2;
        private const int idService = 34;
        private const string label = "dev";

        private readonly Utilisateur user = new Utilisateur(idUser, idService, label);

        [TestMethod()]
        public void UtilisateurTest()
        {
            Assert.AreEqual(idUser, user.IdUser, "devrait réussir : idUser valorisé");
            Assert.AreEqual(idService, user.IdService, "devrait réussir : idRevue valorisé");
            Assert.AreEqual(label, user.Label, "devrait réussir : idSuivi valorisé");

            Assert.AreNotEqual(23, user.IdUser, "devrait échouer");
            Assert.AreNotEqual(12, user.IdUser, "devrait échouer");
            Assert.AreNotEqual("gfsdgs", user.Label, "devrait échouer");
        }
    }
}