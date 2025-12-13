-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Anamakine: 127.0.0.1:3306
-- Üretim Zamanı: 13 Ara 2025, 14:06:51
-- Sunucu sürümü: 9.1.0
-- PHP Sürümü: 8.3.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Veritabanı: `rehber`
--

-- --------------------------------------------------------

--
-- Tablo için tablo yapısı `rehber`
--

DROP TABLE IF EXISTS `rehber`;
CREATE TABLE IF NOT EXISTS `rehber` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Ad` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `Soyad` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `Telefon` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `KaydedenKullaniciId` int DEFAULT NULL,
  `KayitTarihi` datetime DEFAULT CURRENT_TIMESTAMP,
  `IsDeleted` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `KaydedenKullaniciId` (`KaydedenKullaniciId`)
) ENGINE=MyISAM AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Tablo döküm verisi `rehber`
--

INSERT INTO `rehber` (`Id`, `Ad`, `Soyad`, `Telefon`, `KaydedenKullaniciId`, `KayitTarihi`, `IsDeleted`) VALUES
(35, 'Sevgi', 'Yalçınkaya', '22358', 8, '2025-12-13 16:03:38', 0),
(34, 'Elif', 'Kalıçlı', '565464', 8, '2025-12-13 16:02:31', 0),
(33, 'Rabia Zeynep', 'Kenirci', '43242343232', 8, '2025-12-13 16:01:24', 0),
(32, 'Aslı', 'Ulusoy', '432432', 8, '2025-12-13 15:27:54', 0),
(31, 'Selen', 'Sırlan', '53454353', 8, '2025-12-13 15:27:33', 0),
(30, 'Zehra Ceren', 'Kırmızıgül', '54354', 8, '2025-12-13 15:27:00', 0),
(29, 'Zeynep', 'Kaçar', '45439543', 8, '2025-12-13 15:26:35', 0);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
