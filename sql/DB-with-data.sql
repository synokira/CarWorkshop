-- MySQL dump 10.13  Distrib 9.3.0, for Linux (x86_64)
--
-- Host: localhost    Database: Car_Workshop
-- ------------------------------------------------------
-- Server version	9.3.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `Car_Workshop`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `Car_Workshop` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `Car_Workshop`;

--
-- Table structure for table `Car`
--

DROP TABLE IF EXISTS `Car`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Car` (
  `CarId` int NOT NULL AUTO_INCREMENT,
  `Brand` varchar(50) NOT NULL,
  `Model` varchar(50) NOT NULL,
  `OwnerId` int NOT NULL,
  PRIMARY KEY (`CarId`),
  KEY `Owner_id` (`OwnerId`),
  CONSTRAINT `Car_ibfk_1` FOREIGN KEY (`OwnerId`) REFERENCES `CarOwner` (`OwnerId`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Car`
--

LOCK TABLES `Car` WRITE;
/*!40000 ALTER TABLE `Car` DISABLE KEYS */;
INSERT INTO `Car` VALUES (1,'Toyota','Camry',1),(2,'Honda','Civic',2),(3,'Ford','Mustang',3),(4,'Chevrolet','Malibu',4),(16,'Chevrolet','Kyoto',16),(17,'Chevrolet','Kyoto',3),(18,'Chevrolet','yer',1),(19,'Toyota','Camry',3),(20,'Ford','Mustang',17),(21,'Lexus','lbx',18),(22,'Lexus','lbx',3),(23,'Opel','Meriva',19);
/*!40000 ALTER TABLE `Car` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CarOwner`
--

DROP TABLE IF EXISTS `CarOwner`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CarOwner` (
  `OwnerId` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Phone` varchar(15) NOT NULL,
  PRIMARY KEY (`OwnerId`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CarOwner`
--

LOCK TABLES `CarOwner` WRITE;
/*!40000 ALTER TABLE `CarOwner` DISABLE KEYS */;
INSERT INTO `CarOwner` VALUES (1,'John Doe','1234567890'),(2,'Jane Smithy','0987654321'),(3,'Alice Johnson','5551234567'),(4,'Bob Browny','4449876543'),(16,'Alex Mercer','1231233212'),(17,'Edy Smith','9348232344'),(18,'Karen Bonds','2344323434'),(19,'Lina Strange','8798743242');
/*!40000 ALTER TABLE `CarOwner` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CarWorkshopInvoice`
--

DROP TABLE IF EXISTS `CarWorkshopInvoice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CarWorkshopInvoice` (
  `InvoiceId` int NOT NULL AUTO_INCREMENT,
  `CarId` int NOT NULL,
  `ServiceId` int NOT NULL,
  `PartId` int NOT NULL,
  `InvoiceDate` date NOT NULL,
  `TotalAmount` decimal(10,2) NOT NULL,
  PRIMARY KEY (`InvoiceId`),
  KEY `Car_id` (`CarId`),
  KEY `Part_id` (`PartId`),
  KEY `Service_id` (`ServiceId`),
  CONSTRAINT `CarWorkshopInvoice_ibfk_1` FOREIGN KEY (`CarId`) REFERENCES `Car` (`CarId`),
  CONSTRAINT `CarWorkshopInvoice_ibfk_2` FOREIGN KEY (`PartId`) REFERENCES `Parts` (`PartId`),
  CONSTRAINT `CarWorkshopInvoice_ibfk_3` FOREIGN KEY (`ServiceId`) REFERENCES `Service` (`ServiceId`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CarWorkshopInvoice`
--

LOCK TABLES `CarWorkshopInvoice` WRITE;
/*!40000 ALTER TABLE `CarWorkshopInvoice` DISABLE KEYS */;
INSERT INTO `CarWorkshopInvoice` VALUES (1,1,1,1,'2023-10-01',45.00),(2,2,2,2,'2023-10-02',90.00),(3,3,3,3,'2023-10-03',35.00),(4,4,4,4,'2023-10-04',110.00),(16,16,2,3,'2025-06-18',60.00),(17,17,1,3,'2025-06-18',50.00),(18,18,4,2,'2025-06-18',150.00),(19,3,4,1,'2025-06-18',115.00),(20,16,4,2,'2025-06-18',150.00),(21,19,4,1,'2025-06-18',115.00),(22,20,16,17,'2025-06-18',243.00),(23,21,4,4,'2025-06-18',110.00),(24,22,3,16,'2025-06-18',365.00),(25,23,1,4,'2025-06-19',40.00);
/*!40000 ALTER TABLE `CarWorkshopInvoice` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Parts`
--

DROP TABLE IF EXISTS `Parts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Parts` (
  `PartId` int NOT NULL AUTO_INCREMENT,
  `PartName` varchar(50) NOT NULL,
  `PartPrice` decimal(10,2) NOT NULL,
  PRIMARY KEY (`PartId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Parts`
--

LOCK TABLES `Parts` WRITE;
/*!40000 ALTER TABLE `Parts` DISABLE KEYS */;
INSERT INTO `Parts` VALUES (1,'Oil Filter',15.00),(2,'Brake Pads',50.00),(3,'Air Filter',20.00),(4,'Spark Plugs',10.00),(16,'Tires',340.00),(17,'Windshield',198.00);
/*!40000 ALTER TABLE `Parts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Service`
--

DROP TABLE IF EXISTS `Service`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Service` (
  `ServiceId` int NOT NULL AUTO_INCREMENT,
  `ServiceType` varchar(50) NOT NULL,
  `ServicePrice` decimal(10,2) NOT NULL,
  PRIMARY KEY (`ServiceId`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Service`
--

LOCK TABLES `Service` WRITE;
/*!40000 ALTER TABLE `Service` DISABLE KEYS */;
INSERT INTO `Service` VALUES (1,'Oil Change',30.00),(2,'Brake Inspection',40.00),(3,'Tire Rotation',25.00),(4,'Engine Tune-up',100.00),(16,'Windshield change',45.00);
/*!40000 ALTER TABLE `Service` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-06-19 11:23:52
