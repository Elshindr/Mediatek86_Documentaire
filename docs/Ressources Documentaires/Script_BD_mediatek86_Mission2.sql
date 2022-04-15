-- phpMyAdmin SQL Dump
-- version 5.0.2
-- https://www.phpmyadmin.net/
--
-- Hôte : 127.0.0.1:3306
-- Généré le : ven. 15 avr. 2022 à 13:17
-- Version du serveur :  5.7.31
-- Version de PHP : 7.4.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `mediatek86`
--

DELIMITER $$
--
-- Fonctions
--
DROP FUNCTION IF EXISTS `fctEndingAbo`$$
CREATE DEFINER=`root`@`localhost` FUNCTION `fctEndingAbo` () RETURNS VARCHAR(255) CHARSET utf8 COLLATE utf8_unicode_ci READS SQL DATA
BEGIN
   DECLARE idRevues VARCHAR(5);
   DECLARE grpStr VARCHAR(255);
   
    SELECT DISTINCT idRevue,
     GROUP_CONCAT( DISTINCT idRevue)
     INTO idRevues,  grpStr		
     FROM abonnement 
   	 WHERE dateFinAbonnement <= 							DATE_ADD(NOW(), INTERVAL 1 					MONTH);
    
  IF(grpStr = null) THEN
    SET grpStr = "x";
   END IF;
    
RETURN grpStr;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Structure de la table `abonnement`
--

DROP TABLE IF EXISTS `abonnement`;
CREATE TABLE IF NOT EXISTS `abonnement` (
  `id` varchar(5) NOT NULL,
  `dateFinAbonnement` date DEFAULT NULL,
  `idRevue` varchar(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idRevue` (`idRevue`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `abonnement`
--

INSERT INTO `abonnement` (`id`, `dateFinAbonnement`, `idRevue`) VALUES
('00021', '2022-04-30', '10002'),
('00022', '2022-04-03', '10002'),
('00024', '2022-04-15', '10002'),
('00025', '2022-04-27', '10003');

-- --------------------------------------------------------

--
-- Structure de la table `commande`
--

DROP TABLE IF EXISTS `commande`;
CREATE TABLE IF NOT EXISTS `commande` (
  `id` varchar(5) NOT NULL,
  `dateCommande` date DEFAULT NULL,
  `montant` double DEFAULT NULL,
  `idSuivi` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `commande_ibfk_1` (`idSuivi`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `commande`
--

INSERT INTO `commande` (`id`, `dateCommande`, `montant`, `idSuivi`) VALUES
('00001', '2022-04-11', 1, 2),
('00002', '2022-04-12', 2, 2),
('00003', '2022-04-13', 2.5, 3),
('00004', '2022-04-12', 6.5, 2),
('00005', '2022-04-13', 2.5, 3),
('00006', '2022-04-12', 1, 1),
('00007', '2022-04-28', 1.5, 2),
('00008', '2022-04-29', 3, 3),
('00009', '2022-04-12', 2.5, 3),
('00010', '2022-04-12', 1.5, 3),
('00013', '2022-04-12', 1, 3),
('00014', '2022-04-12', 0.5, 4),
('00015', '2022-04-13', 1, 3),
('00016', '2022-04-13', 2, 2),
('00017', '2022-04-13', 0.5, 4),
('00018', '2022-04-13', 0.5, 1),
('00020', '2022-04-13', 0.5, 3),
('00021', '2022-04-13', 12, 1),
('00022', '2022-04-01', 255, 1),
('00023', '2022-04-14', 3, 1),
('00024', '2022-04-14', 2, 1),
('00025', '2022-04-15', 3, 1);

--
-- Déclencheurs `commande`
--
DROP TRIGGER IF EXISTS `trigInsertCommandes`;
DELIMITER $$
CREATE TRIGGER `trigInsertCommandes` BEFORE INSERT ON `commande` FOR EACH ROW BEGIN
	DECLARE idCmdDoc INT;
	DECLARE idCmdRevue INT;
   
   	SELECT Count(id) into idCmdDoc FROM commandedocument WHERE id = NEW.id;
    SELECT Count(id) into idCmdRevue FROM abonnement WHERE id = NEW.id;
    
    IF(idCmdDoc > 0  || idCmdDoc > 0) THEN
       SIGNAL SQLSTATE "45000"
            SET MESSAGE_TEXT = "opération impossible doublon idCmd" ;  
   
    END IF;
    
END
$$
DELIMITER ;
DROP TRIGGER IF EXISTS `trigInsertExemplrs`;
DELIMITER $$
CREATE TRIGGER `trigInsertExemplrs` AFTER UPDATE ON `commande` FOR EACH ROW BEGIN
    DECLARE increm INT;
    DECLARE v_nbExemplaire INT ; 
    DECLARE v_newMaxExemplaire INT ;
    DECLARE v_testExemplaire int;
    DECLARE v_NumExemplaire INT ; 
    DECLARE v_dSuivi INT ; 
    DECLARE v_dateCommande DATETIME ; 
    DECLARE v_id VARCHAR(5) ; 
    DECLARE v_idLivreDvd VARCHAR(5) ;

    IF(NEW.idSuivi = 2) THEN
        SELECT 1 into increm;
               
        SELECT cd.idLivreDvd, cd.nbExemplaire, c.id, c.idSuivi, c.dateCommande
        into v_idLivreDvd, v_nbExemplaire, v_id, v_dSuivi, v_dateCommande
        FROM commandedocument cd join commande c on (c.id=cd.id)
        WHERE cd.id = NEW.id;

        SELECT COUNT(numero) into v_newMaxExemplaire
        FROM exemplaire
        WHERE id = v_idLivreDvd;
            
      REPEAT 
      
            INSERT INTO exemplaire(id, numero, dateAchat, photo, idEtat)
            VALUES ( v_idLivreDvd, v_newMaxExemplaire+increm, v_dateCommande, "",'00001');
            
            SELECT (increm+1) into increm ;

         UNTIL (increm > v_nbExemplaire) END REPEAT ;

    END IF ;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Structure de la table `commandedocument`
--

DROP TABLE IF EXISTS `commandedocument`;
CREATE TABLE IF NOT EXISTS `commandedocument` (
  `id` varchar(5) NOT NULL,
  `nbExemplaire` int(11) DEFAULT NULL,
  `idLivreDvd` varchar(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idLivreDvd` (`idLivreDvd`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `commandedocument`
--

INSERT INTO `commandedocument` (`id`, `nbExemplaire`, `idLivreDvd`) VALUES
('00001', 4, '00017'),
('00002', 2, '00017'),
('00003', 4, '00017'),
('00004', 7, '00017'),
('00005', 7, '00017'),
('00006', 4, '00004'),
('00007', 3, '00017'),
('00008', 3, '00017'),
('00009', 4, '00025'),
('00010', 4, '20003'),
('00013', 1, '20003'),
('00014', 1, '20003'),
('00015', 2, '00017'),
('00016', 4, '00017'),
('00017', 1, '00017'),
('00018', 2, '00017'),
('00020', 1, '20003'),
('00023', 4, '20003');

-- --------------------------------------------------------

--
-- Structure de la table `document`
--

DROP TABLE IF EXISTS `document`;
CREATE TABLE IF NOT EXISTS `document` (
  `id` varchar(10) NOT NULL,
  `titre` varchar(60) DEFAULT NULL,
  `image` varchar(100) DEFAULT NULL,
  `idRayon` varchar(5) NOT NULL,
  `idPublic` varchar(5) NOT NULL,
  `idGenre` varchar(5) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idRayon` (`idRayon`),
  KEY `idPublic` (`idPublic`),
  KEY `idGenre` (`idGenre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `document`
--

INSERT INTO `document` (`id`, `titre`, `image`, `idRayon`, `idPublic`, `idGenre`) VALUES
('00001', 'Quand sort la recluse', '', 'LV003', '00002', '10014'),
('00002', 'Un pays à l\'aube', '', 'LV001', '00002', '10004'),
('00003', 'Et je danse aussi', '', 'LV002', '00003', '10013'),
('00004', 'L\'armée furieuse', '', 'LV003', '00002', '10014'),
('00005', 'Les anonymes', '', 'LV001', '00002', '10014'),
('00006', 'La marque jaune', '', 'BD001', '00003', '10001'),
('00007', 'Dans les coulisses du musée', '', 'LV001', '00003', '10006'),
('00008', 'Histoire du juif errant', '', 'LV002', '00002', '10006'),
('00009', 'Pars vite et reviens tard', '', 'LV003', '00002', '10014'),
('00010', 'Le vestibule des causes perdues', '', 'LV001', '00002', '10006'),
('00011', 'L\'île des oubliés', '', 'LV002', '00003', '10006'),
('00012', 'La souris bleue', '', 'LV002', '00003', '10006'),
('00013', 'Sacré Pêre Noël', '', 'JN001', '00001', '10001'),
('00014', 'Mauvaise étoile', '', 'LV003', '00003', '10014'),
('00015', 'La confrérie des téméraires', '', 'JN002', '00004', '10014'),
('00016', 'Le butin du requin', '', 'JN002', '00004', '10014'),
('00017', 'Catastrophes au Brésil', '', 'JN002', '00004', '10014'),
('00018', 'Le Routard - Maroc', '', 'DV005', '00003', '10011'),
('00019', 'Guide Vert - Iles Canaries', '', 'DV005', '00003', '10011'),
('00020', 'Guide Vert - Irlande', '', 'DV005', '00003', '10011'),
('00021', 'Les déferlantes', '', 'LV002', '00002', '10006'),
('00022', 'Une part de Ciel', '', 'LV002', '00002', '10006'),
('00023', 'Le secret du janissaire', '', 'BD001', '00002', '10001'),
('00024', 'Pavillon noir', '', 'BD001', '00002', '10001'),
('00025', 'L\'archipel du danger', '', 'BD001', '00002', '10001'),
('00026', 'La planète des singes', '', 'LV002', '00003', '10002'),
('10001', 'Arts Magazine', '', 'PR002', '00002', '10016'),
('10002', 'Alternatives Economiques', '', 'PR002', '00002', '10015'),
('10003', 'Challenges', '', 'PR002', '00002', '10015'),
('10004', 'Rock and Folk', '', 'PR002', '00002', '10016'),
('10005', 'Les Echos', '', 'PR001', '00002', '10015'),
('10006', 'Le Monde', '', 'PR001', '00002', '10018'),
('10007', 'Telerama', '', 'PR002', '00002', '10016'),
('10008', 'L\'Obs', '', 'PR002', '00002', '10018'),
('10009', 'L\'Equipe', '', 'PR001', '00002', '10017'),
('10010', 'L\'Equipe Magazine', '', 'PR002', '00002', '10017'),
('10011', 'Geo', '', 'PR002', '00003', '10016'),
('20001', 'Star Wars 5 L\'empire contre attaque', '', 'DF001', '00003', '10002'),
('20002', 'Le seigneur des anneaux : la communauté de l\'anneau', '', 'DF001', '00003', '10019'),
('20003', 'Jurassic Park', '', 'DF001', '00003', '10002'),
('20004', 'Matrix', '', 'DF001', '00003', '10002');

-- --------------------------------------------------------

--
-- Structure de la table `dvd`
--

DROP TABLE IF EXISTS `dvd`;
CREATE TABLE IF NOT EXISTS `dvd` (
  `id` varchar(10) NOT NULL,
  `synopsis` text,
  `realisateur` varchar(20) DEFAULT NULL,
  `duree` int(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `dvd`
--

INSERT INTO `dvd` (`id`, `synopsis`, `realisateur`, `duree`) VALUES
('20001', 'Luc est entraîné par Yoda pendant que Han et Leia tentent de se cacher dans la cité des nuages.', 'George Lucas', 124),
('20002', 'L\'anneau unique, forgé par Sauron, est porté par Fraudon qui l\'amène à Foncombe. De là, des représentants de peuples différents vont s\'unir pour aider Fraudon à amener l\'anneau à la montagne du Destin.', 'Peter Jackson', 228),
('20003', 'Un milliardaire et des généticiens créent des dinosaures à partir de clonage.', 'Steven Spielberg', 128),
('20004', 'Un informaticien réalise que le monde dans lequel il vit est une simulation gérée par des machines.', 'Les Wachowski', 136);

-- --------------------------------------------------------

--
-- Structure de la table `etat`
--

DROP TABLE IF EXISTS `etat`;
CREATE TABLE IF NOT EXISTS `etat` (
  `id` char(5) NOT NULL,
  `libelle` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `etat`
--

INSERT INTO `etat` (`id`, `libelle`) VALUES
('00001', 'neuf'),
('00002', 'usagé'),
('00003', 'détérioré'),
('00004', 'inutilisable');

-- --------------------------------------------------------

--
-- Structure de la table `exemplaire`
--

DROP TABLE IF EXISTS `exemplaire`;
CREATE TABLE IF NOT EXISTS `exemplaire` (
  `id` varchar(10) NOT NULL,
  `numero` int(11) NOT NULL,
  `dateAchat` date DEFAULT NULL,
  `photo` varchar(100) NOT NULL,
  `idEtat` char(5) NOT NULL,
  PRIMARY KEY (`id`,`numero`),
  KEY `idEtat` (`idEtat`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `exemplaire`
--

INSERT INTO `exemplaire` (`id`, `numero`, `dateAchat`, `photo`, `idEtat`) VALUES
('00017', 1, '2022-04-13', '', '00001'),
('00017', 2, '2022-04-13', '', '00001'),
('00017', 3, '2022-04-13', '', '00001'),
('00017', 4, '2022-04-13', '', '00001'),
('00017', 5, '2022-04-13', '', '00001'),
('00017', 6, '2022-04-13', '', '00001'),
('00017', 7, '2022-04-13', '', '00001'),
('00017', 8, '2022-04-28', '', '00001'),
('00017', 9, '2022-04-28', '', '00001'),
('00017', 10, '2022-04-28', '', '00001'),
('00017', 11, '2022-04-28', '', '00001'),
('00017', 12, '2022-04-29', '', '00001'),
('00017', 13, '2022-04-29', '', '00001'),
('00017', 14, '2022-04-29', '', '00001'),
('00017', 15, '2022-04-13', '', '00001'),
('00017', 16, '2022-04-13', '', '00001'),
('00017', 17, '2022-04-13', '', '00001'),
('00017', 18, '2022-04-13', '', '00001'),
('00017', 19, '2022-04-13', '', '00001'),
('00017', 20, '2022-04-13', '', '00001'),
('00025', 1, '2022-04-12', '', '00001'),
('00025', 2, '2022-04-12', '', '00001'),
('00025', 3, '2022-04-12', '', '00001'),
('00025', 4, '2022-04-12', '', '00001'),
('00025', 5, '2022-04-12', '', '00001'),
('00025', 6, '2022-04-12', '', '00001'),
('00025', 7, '2022-04-12', '', '00001'),
('00025', 8, '2022-04-12', '', '00001'),
('10002', 55, '2022-04-11', '', '00001'),
('10005', 12, '2022-04-10', '', '00001'),
('10007', 3237, '2021-11-23', '', '00001'),
('10007', 3238, '2021-11-30', '', '00001'),
('10007', 3239, '2021-12-07', '', '00001'),
('10007', 3240, '2021-12-21', '', '00001'),
('10011', 506, '2021-04-01', '', '00001'),
('10011', 507, '2021-05-03', '', '00001'),
('10011', 508, '2021-06-05', '', '00001'),
('10011', 509, '2021-07-01', '', '00001'),
('10011', 510, '2021-08-04', '', '00001'),
('10011', 511, '2021-09-01', '', '00001'),
('10011', 512, '2021-10-06', '', '00001'),
('10011', 513, '2021-11-01', '', '00001'),
('10011', 514, '2021-12-01', '', '00001'),
('20003', 1, '2022-04-12', '', '00001'),
('20003', 2, '2022-04-12', '', '00001'),
('20003', 3, '2022-04-12', '', '00001'),
('20003', 4, '2022-04-12', '', '00001'),
('20003', 5, '2022-04-12', '', '00001'),
('20003', 6, '2022-04-13', '', '00001');

-- --------------------------------------------------------

--
-- Structure de la table `genre`
--

DROP TABLE IF EXISTS `genre`;
CREATE TABLE IF NOT EXISTS `genre` (
  `id` varchar(5) NOT NULL,
  `libelle` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `genre`
--

INSERT INTO `genre` (`id`, `libelle`) VALUES
('10000', 'Humour'),
('10001', 'Bande dessinée'),
('10002', 'Science Fiction'),
('10003', 'Biographie'),
('10004', 'Historique'),
('10006', 'Roman'),
('10007', 'Aventures'),
('10008', 'Essai'),
('10009', 'Documentaire'),
('10010', 'Technique'),
('10011', 'Voyages'),
('10012', 'Drame'),
('10013', 'Comédie'),
('10014', 'Policier'),
('10015', 'Presse Economique'),
('10016', 'Presse Culturelle'),
('10017', 'Presse sportive'),
('10018', 'Actualités'),
('10019', 'Fantazy');

-- --------------------------------------------------------

--
-- Structure de la table `livre`
--

DROP TABLE IF EXISTS `livre`;
CREATE TABLE IF NOT EXISTS `livre` (
  `id` varchar(10) NOT NULL,
  `ISBN` varchar(13) DEFAULT NULL,
  `auteur` varchar(20) DEFAULT NULL,
  `collection` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `livre`
--

INSERT INTO `livre` (`id`, `ISBN`, `auteur`, `collection`) VALUES
('00001', '1234569877896', 'Fred Vargas', 'Commissaire Adamsberg'),
('00002', '1236547896541', 'Dennis Lehanne', ''),
('00003', '6541236987410', 'Anne-Laure Bondoux', ''),
('00004', '3214569874123', 'Fred Vargas', 'Commissaire Adamsberg'),
('00005', '3214563214563', 'RJ Ellory', ''),
('00006', '3213213211232', 'Edgar P. Jacobs', 'Blake et Mortimer'),
('00007', '6541236987541', 'Kate Atkinson', ''),
('00008', '1236987456321', 'Jean d\'Ormesson', ''),
('00009', '3,21457E+12', 'Fred Vargas', 'Commissaire Adamsberg'),
('00010', '3,21457E+12', 'Manon Moreau', ''),
('00011', '3,21457E+12', 'Victoria Hislop', ''),
('00012', '3,21457E+12', 'Kate Atkinson', ''),
('00013', '3,21457E+12', 'Raymond Briggs', ''),
('00014', '3,21457E+12', 'RJ Ellory', ''),
('00015', '3,21457E+12', 'Floriane Turmeau', ''),
('00016', '3,21457E+12', 'Julian Press', ''),
('00017', '3,21457E+12', 'Philippe Masson', ''),
('00018', '3,21457E+12', '', 'Guide du Routard'),
('00019', '3,21457E+12', '', 'Guide Vert'),
('00020', '3,21457E+12', '', 'Guide Vert'),
('00021', '3,21457E+12', 'Claudie Gallay', ''),
('00022', '3,21457E+12', 'Claudie Gallay', ''),
('00023', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00024', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00025', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00026', '', 'Pierre Boulle', 'Julliard');

-- --------------------------------------------------------

--
-- Structure de la table `livres_dvd`
--

DROP TABLE IF EXISTS `livres_dvd`;
CREATE TABLE IF NOT EXISTS `livres_dvd` (
  `id` varchar(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `livres_dvd`
--

INSERT INTO `livres_dvd` (`id`) VALUES
('00001'),
('00002'),
('00003'),
('00004'),
('00005'),
('00006'),
('00007'),
('00008'),
('00009'),
('00010'),
('00011'),
('00012'),
('00013'),
('00014'),
('00015'),
('00016'),
('00017'),
('00018'),
('00019'),
('00020'),
('00021'),
('00022'),
('00023'),
('00024'),
('00025'),
('00026'),
('20001'),
('20002'),
('20003'),
('20004');

-- --------------------------------------------------------

--
-- Structure de la table `public`
--

DROP TABLE IF EXISTS `public`;
CREATE TABLE IF NOT EXISTS `public` (
  `id` varchar(5) NOT NULL,
  `libelle` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `public`
--

INSERT INTO `public` (`id`, `libelle`) VALUES
('00001', 'Jeunesse'),
('00002', 'Adultes'),
('00003', 'Tous publics'),
('00004', 'Ados');

-- --------------------------------------------------------

--
-- Structure de la table `rayon`
--

DROP TABLE IF EXISTS `rayon`;
CREATE TABLE IF NOT EXISTS `rayon` (
  `id` char(5) NOT NULL,
  `libelle` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `rayon`
--

INSERT INTO `rayon` (`id`, `libelle`) VALUES
('BD001', 'BD Adultes'),
('BL001', 'Beaux Livres'),
('DF001', 'DVD films'),
('DV001', 'Sciences'),
('DV002', 'Maison'),
('DV003', 'Santé'),
('DV004', 'Littérature classique'),
('DV005', 'Voyages'),
('JN001', 'Jeunesse BD'),
('JN002', 'Jeunesse romans'),
('LV001', 'Littérature étrangère'),
('LV002', 'Littérature française'),
('LV003', 'Policiers français étrangers'),
('PR001', 'Presse quotidienne'),
('PR002', 'Magazines');

-- --------------------------------------------------------

--
-- Structure de la table `revue`
--

DROP TABLE IF EXISTS `revue`;
CREATE TABLE IF NOT EXISTS `revue` (
  `id` varchar(10) NOT NULL,
  `empruntable` tinyint(1) DEFAULT NULL,
  `periodicite` varchar(2) DEFAULT NULL,
  `delaiMiseADispo` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `revue`
--

INSERT INTO `revue` (`id`, `empruntable`, `periodicite`, `delaiMiseADispo`) VALUES
('10001', 1, 'MS', 52),
('10002', 1, 'MS', 52),
('10003', 1, 'HB', 15),
('10004', 1, 'HB', 15),
('10005', 0, 'QT', 5),
('10006', 0, 'QT', 5),
('10007', 1, 'HB', 26),
('10008', 1, 'HB', 26),
('10009', 0, 'QT', 5),
('10010', 1, 'HB', 12),
('10011', 1, 'MS', 52);

-- --------------------------------------------------------

--
-- Structure de la table `suivi`
--

DROP TABLE IF EXISTS `suivi`;
CREATE TABLE IF NOT EXISTS `suivi` (
  `idSuivi` int(11) NOT NULL AUTO_INCREMENT,
  `label` varchar(15) NOT NULL,
  PRIMARY KEY (`idSuivi`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `suivi`
--

INSERT INTO `suivi` (`idSuivi`, `label`) VALUES
(1, 'en cours'),
(2, 'livrée'),
(3, 'réglée'),
(4, 'relancée');

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table `abonnement`
--
ALTER TABLE `abonnement`
  ADD CONSTRAINT `abonnement_ibfk_1` FOREIGN KEY (`id`) REFERENCES `commande` (`id`),
  ADD CONSTRAINT `abonnement_ibfk_2` FOREIGN KEY (`idRevue`) REFERENCES `revue` (`id`);

--
-- Contraintes pour la table `commande`
--
ALTER TABLE `commande`
  ADD CONSTRAINT `commande_ibfk_1` FOREIGN KEY (`idSuivi`) REFERENCES `suivi` (`idSuivi`);

--
-- Contraintes pour la table `commandedocument`
--
ALTER TABLE `commandedocument`
  ADD CONSTRAINT `commandedocument_ibfk_1` FOREIGN KEY (`id`) REFERENCES `commande` (`id`),
  ADD CONSTRAINT `commandedocument_ibfk_2` FOREIGN KEY (`idLivreDvd`) REFERENCES `livres_dvd` (`id`);

--
-- Contraintes pour la table `document`
--
ALTER TABLE `document`
  ADD CONSTRAINT `document_ibfk_1` FOREIGN KEY (`idRayon`) REFERENCES `rayon` (`id`),
  ADD CONSTRAINT `document_ibfk_2` FOREIGN KEY (`idPublic`) REFERENCES `public` (`id`),
  ADD CONSTRAINT `document_ibfk_3` FOREIGN KEY (`idGenre`) REFERENCES `genre` (`id`);

--
-- Contraintes pour la table `dvd`
--
ALTER TABLE `dvd`
  ADD CONSTRAINT `dvd_ibfk_1` FOREIGN KEY (`id`) REFERENCES `livres_dvd` (`id`);

--
-- Contraintes pour la table `exemplaire`
--
ALTER TABLE `exemplaire`
  ADD CONSTRAINT `exemplaire_ibfk_1` FOREIGN KEY (`id`) REFERENCES `document` (`id`),
  ADD CONSTRAINT `exemplaire_ibfk_2` FOREIGN KEY (`idEtat`) REFERENCES `etat` (`id`);

--
-- Contraintes pour la table `livre`
--
ALTER TABLE `livre`
  ADD CONSTRAINT `livre_ibfk_1` FOREIGN KEY (`id`) REFERENCES `livres_dvd` (`id`);

--
-- Contraintes pour la table `livres_dvd`
--
ALTER TABLE `livres_dvd`
  ADD CONSTRAINT `livres_dvd_ibfk_1` FOREIGN KEY (`id`) REFERENCES `document` (`id`);

--
-- Contraintes pour la table `revue`
--
ALTER TABLE `revue`
  ADD CONSTRAINT `revue_ibfk_1` FOREIGN KEY (`id`) REFERENCES `document` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
