-- MySQL dump 10.13  Distrib 8.0.28, for Win64 (x86_64)
--
-- Host: 139.224.50.124    Database: school
-- ------------------------------------------------------
-- Server version	8.0.28

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `admin`
--
CREATE DATABASE school CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE school;
DROP TABLE IF EXISTS `admin`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `admin` (
  `admin_id` int NOT NULL,
  `password` varchar(45) NOT NULL,
  `name` varchar(45) NOT NULL,
  `salt` varchar(45) DEFAULT NULL COMMENT 'MD5码的盐',
  PRIMARY KEY (`admin_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admin`
--

LOCK TABLES `admin` WRITE;
/*!40000 ALTER TABLE `admin` DISABLE KEYS */;
INSERT INTO `admin` VALUES (1853636,'CB2AF6CFFCEE0350EDCFC86C717AC849','侯彤','7ERMQ');
/*!40000 ALTER TABLE `admin` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enrollment`
--

DROP TABLE IF EXISTS `enrollment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `enrollment` (
  `student_id` int NOT NULL,
  `schedule_id` int NOT NULL,
  `score` int DEFAULT NULL COMMENT '分数',
  `grade_point` int DEFAULT NULL COMMENT '绩点',
  `grade_status` varchar(45) DEFAULT NULL COMMENT '成绩状况',
  `select_status` int NOT NULL COMMENT '1为正常选课，2为重修选课',
  `input_time` datetime DEFAULT NULL COMMENT '成绩录入时间',
  PRIMARY KEY (`student_id`,`schedule_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enrollment`
--

LOCK TABLES `enrollment` WRITE;
/*!40000 ALTER TABLE `enrollment` DISABLE KEYS */;
INSERT INTO `enrollment` VALUES (1752138,42,91,5,'正常',1,'2022-05-09 15:23:36'),(1762211,22,98,5,'正常',1,'2022-03-10 10:53:24'),(1762211,42,NULL,NULL,'未出成绩',1,NULL),(1853636,2,80,4,'正常',1,'2021-07-04 16:23:46'),(1853636,12,55,0,'正常',1,'2021-07-02 16:25:46'),(1853636,19,70,3,'正常',1,'2022-01-21 16:25:46'),(1853636,22,NULL,NULL,'未出成绩',1,NULL),(1853636,30,NULL,NULL,'未出成绩',1,NULL),(1853636,31,NULL,NULL,'未出成绩',1,NULL),(1853636,37,NULL,NULL,'未出成绩',1,NULL);
/*!40000 ALTER TABLE `enrollment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `information`
--

DROP TABLE IF EXISTS `information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `information` (
  `year` int NOT NULL COMMENT '年份',
  `half` int NOT NULL COMMENT '上下半年,0为上,1为下',
  `select_status` int NOT NULL COMMENT '选课状态，0为不能选课，1为正常选课，2为重修选课',
  `select_begin_time` datetime DEFAULT NULL COMMENT '选课开始时间',
  `select_end_time` datetime DEFAULT NULL COMMENT '选课截止时间',
  `semester_begin_time` datetime NOT NULL COMMENT '学期开始时间',
  `can_import_grade` int NOT NULL COMMENT '能否导入成绩',
  `grade_begin_time` datetime DEFAULT NULL COMMENT '导入成绩开始时间',
  `grade_end_time` datetime DEFAULT NULL COMMENT '导入成绩截止时间',
  `id` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `information`
--

LOCK TABLES `information` WRITE;
/*!40000 ALTER TABLE `information` DISABLE KEYS */;
INSERT INTO `information` VALUES (2022,0,1,'2022-05-08 08:00:00','2022-06-30 20:00:00','2022-02-21 00:00:00',1,'2022-05-08 08:00:00','2022-06-22 20:00:00','1');
/*!40000 ALTER TABLE `information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lesson`
--

DROP TABLE IF EXISTS `lesson`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lesson` (
  `lesson_id` int NOT NULL AUTO_INCREMENT,
  `lesson_name` varchar(45) NOT NULL,
  `type` varchar(45) NOT NULL COMMENT '属于哪一大类的课程',
  `credit` decimal(3,1) NOT NULL COMMENT '课程占几个学分',
  `preq_id` int DEFAULT NULL COMMENT '替代的以往课程的id',
  `note` varchar(45) DEFAULT NULL COMMENT '备注',
  `need_depart` varchar(45) NOT NULL COMMENT '什么专业能选,若为 ''all''则都可以选',
  `identity` varchar(45) NOT NULL COMMENT '本科生研究生还是博士生',
  PRIMARY KEY (`lesson_id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lesson`
--

LOCK TABLES `lesson` WRITE;
/*!40000 ALTER TABLE `lesson` DISABLE KEYS */;
INSERT INTO `lesson` VALUES (1,'软件工程','专业课',3.0,0,'新生课','软件工程','本科生'),(2,'体育(1)','体育课',1.5,12,NULL,'all','本科生'),(3,'西方哲学史','通识选修',1.5,NULL,NULL,'all','本科生'),(4,'体育(2)','体育课',1.5,NULL,NULL,'all','本科生'),(5,'数论','专业课',3.0,NULL,NULL,'数学系','本科生'),(12,'体育大类1','体育课',1.5,NULL,NULL,'all','本科生'),(13,'数据结构','专业课',3.0,NULL,NULL,'软件工程','本科生'),(14,'数据库','专业课',3.0,NULL,NULL,'软件工程','本科生'),(15,'算法','专业课',3.0,NULL,NULL,'软件工程','本科生'),(16,'线性代数','通识必修',3.0,NULL,NULL,'all','本科生'),(17,'创业修炼','通识选修',1.5,NULL,'周末','all','本科生'),(18,'大学物理一','通识必修',2.0,NULL,NULL,'all','本科生'),(19,'系统设计与实现','专业课',2.0,NULL,'','软件工程','本科生'),(20,'离散数学','通识必修',3.0,NULL,NULL,'all','本科生'),(21,'复变函数','专业课',3.0,NULL,NULL,'数学系','本科生'),(22,'艺术鉴赏','通识选修',1.5,NULL,NULL,'all','本科生'),(23,'电影鉴赏','通识选修',1.5,NULL,NULL,'all','本科生'),(24,'马克思主义原理','通识必修',2.0,NULL,NULL,'all','本科生');
/*!40000 ALTER TABLE `lesson` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lesson_requirement`
--

DROP TABLE IF EXISTS `lesson_requirement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lesson_requirement` (
  `lesson_id` int NOT NULL,
  `in_year` int NOT NULL COMMENT '哪一级能选(比如2018级)',
  `min_grade` int NOT NULL COMMENT '最低几年级能选',
  `max_grade` int NOT NULL COMMENT '最高几年级能选',
  PRIMARY KEY (`lesson_id`,`in_year`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lesson_requirement`
--

LOCK TABLES `lesson_requirement` WRITE;
/*!40000 ALTER TABLE `lesson_requirement` DISABLE KEYS */;
INSERT INTO `lesson_requirement` VALUES (1,2018,2,4),(1,2019,2,3),(1,2020,1,2),(2,2018,1,9),(2,2019,2,3),(3,2018,1,10),(4,2018,2,10),(5,2018,1,4),(12,2021,1,4),(13,2018,1,4),(13,2019,1,4),(14,2018,1,4),(15,2018,1,4),(16,2018,1,4),(17,2018,1,4),(17,2019,1,4),(18,2018,1,4),(19,2018,2,4),(19,2019,2,4),(20,2018,2,4),(21,2019,1,4),(21,2020,2,4),(22,2018,1,4),(22,2019,1,4),(23,2019,1,4),(24,2020,2,4);
/*!40000 ALTER TABLE `lesson_requirement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notice`
--

DROP TABLE IF EXISTS `notice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notice` (
  `notice_id` int NOT NULL AUTO_INCREMENT,
  `time` datetime NOT NULL,
  `title` varchar(45) NOT NULL,
  `content` varchar(200) NOT NULL COMMENT '内容',
  `admin_id` int NOT NULL,
  PRIMARY KEY (`notice_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notice`
--

LOCK TABLES `notice` WRITE;
/*!40000 ALTER TABLE `notice` DISABLE KEYS */;
INSERT INTO `notice` VALUES (1,'2022-03-11 12:51:21','选课通知','高年级学生（三、四、五年级）有体育课程重修者，请在学校教务处规定时间内网上报名并缴费。根据《苏州科技大学体育课程教学管理办法》，学生每学期只能重修当学期开设的课程（18级及以前的毕业生（除18级建筑学与城乡规划专业）可重修非当期开设的课程），重修及格，即取得相应的学分。体育部将会根据教务处提供的缴费学生名单安排随一、二年级体育课跟班上课。',1853636),(2,'2022-03-21 22:55:58','选择导师（1）','自愿原则',1853636),(3,'2022-03-21 23:21:28','大学生创业','请前往官网查看',1853636);
/*!40000 ALTER TABLE `notice` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `schedule`
--

DROP TABLE IF EXISTS `schedule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `schedule` (
  `schedule_id` int NOT NULL AUTO_INCREMENT,
  `lesson_id` int NOT NULL,
  `teacher_id` int NOT NULL,
  `year` int NOT NULL COMMENT '年份',
  `half` int NOT NULL COMMENT '上下半年,上为0,下为1',
  `is_over` int NOT NULL DEFAULT '0' COMMENT '是否结束',
  `begin_week` int NOT NULL COMMENT '开始周',
  `end_week` int NOT NULL,
  `place` varchar(45) NOT NULL COMMENT '上课地点',
  `current_num` int NOT NULL DEFAULT '0' COMMENT '当前人数',
  `max_num` int NOT NULL COMMENT '人数上限',
  `note` varchar(45) DEFAULT NULL COMMENT '备注',
  `teaching_material` varchar(45) DEFAULT NULL COMMENT '教材',
  `can_retake` int NOT NULL COMMENT '学生是否能重修选择该课，0为否，1为是',
  `campus` varchar(45) NOT NULL COMMENT '哪个校区',
  PRIMARY KEY (`schedule_id`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schedule`
--

LOCK TABLES `schedule` WRITE;
/*!40000 ALTER TABLE `schedule` DISABLE KEYS */;
INSERT INTO `schedule` VALUES (2,1,6,2021,0,1,1,17,'北楼310',1,40,'',NULL,1,'四平路'),(3,12,1,2020,1,1,1,17,'129篮球场',0,40,'足球课',NULL,1,'四平路'),(12,2,1,2021,0,1,1,17,'129篮球场',1,40,'足球课',NULL,1,'四平路'),(19,2,1,2021,1,1,1,17,'129篮球场',1,40,'足球课',NULL,1,'四平路'),(21,2,1,2022,0,0,1,17,'129篮球场',0,40,'篮球课',NULL,1,'四平路'),(22,3,5,2022,0,0,1,17,'南楼101',1,50,NULL,NULL,1,'四平路'),(25,2,3,2022,0,0,1,17,'129篮球场',0,40,'篮球课',NULL,1,'四平路'),(26,4,3,2022,0,0,1,17,'操场',0,40,'排球课',NULL,1,'四平路'),(27,5,7,2022,0,0,1,17,'北楼108',0,40,NULL,NULL,0,'四平路'),(28,4,1,2022,0,0,1,17,'操场',0,40,'排球课',NULL,1,'四平路'),(30,13,6,2022,0,0,1,17,'南楼129',1,40,NULL,NULL,1,'四平路'),(31,14,8,2022,0,0,1,17,'北楼129',1,40,NULL,NULL,1,'四平路'),(32,13,8,2022,0,0,1,17,'北213',0,50,'','',1,'四平路'),(35,14,6,2022,0,0,1,17,'北312',0,50,'','',1,'四平路'),(36,15,9,2022,0,0,1,17,'南203',0,50,'','',1,'四平路'),(37,16,12,2022,0,0,1,17,'北楼301',1,100,'','',1,'四平路'),(38,19,8,2022,0,0,1,17,'北312',-1,50,'','',1,'四平路'),(39,15,13,2022,0,0,1,17,'南203',0,50,'','',1,'四平路'),(42,1,6,2022,0,0,1,17,'北201',2,100,NULL,NULL,1,'四平路');
/*!40000 ALTER TABLE `schedule` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `schedule_time`
--

DROP TABLE IF EXISTS `schedule_time`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `schedule_time` (
  `schedule_id` int NOT NULL COMMENT '排课的上课时间信息',
  `begin_section` int NOT NULL COMMENT '第几节课开始',
  `end_section` int NOT NULL COMMENT '到第几节课为止(包括在其中)',
  `day_week` int NOT NULL COMMENT '星期几',
  `Single_OR_Double` int NOT NULL COMMENT '单双周，1为单，2为双，3为全',
  PRIMARY KEY (`schedule_id`,`begin_section`,`day_week`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schedule_time`
--

LOCK TABLES `schedule_time` WRITE;
/*!40000 ALTER TABLE `schedule_time` DISABLE KEYS */;
INSERT INTO `schedule_time` VALUES (2,1,2,3,3),(2,6,8,4,3),(3,1,2,3,1),(3,3,4,2,3),(12,5,6,4,3),(19,3,4,3,3),(21,5,6,2,3),(21,7,8,4,1),(22,7,8,1,3),(25,5,6,3,3),(26,4,5,2,2),(26,7,8,4,3),(27,3,4,1,3),(28,5,6,3,3),(30,1,2,5,3),(31,5,7,3,3),(32,1,2,4,3),(32,5,6,2,1),(35,1,2,1,3),(35,5,6,3,2),(36,3,4,5,3),(37,3,4,2,3),(37,7,8,4,1),(38,1,2,1,3),(38,5,6,3,2),(39,3,4,5,3),(42,1,2,3,1),(42,3,4,2,3);
/*!40000 ALTER TABLE `schedule_time` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `student`
--

DROP TABLE IF EXISTS `student`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `student` (
  `student_id` int NOT NULL,
  `student_name` varchar(45) NOT NULL,
  `department` varchar(45) NOT NULL,
  `origin_inyear` int NOT NULL COMMENT '最初的年级(即刚入学时候的年级)',
  `in_year` int NOT NULL COMMENT '哪一级(如2018级)，可能是留级的',
  `password` varchar(45) NOT NULL,
  `identity` varchar(45) NOT NULL COMMENT '本科生研究生还是博士生',
  `is_graduate` int NOT NULL DEFAULT '0' COMMENT '是否毕业了,0为否，1为是',
  `salt` varchar(45) DEFAULT NULL COMMENT 'MD5码的盐',
  PRIMARY KEY (`student_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `student`
--

LOCK TABLES `student` WRITE;
/*!40000 ALTER TABLE `student` DISABLE KEYS */;
INSERT INTO `student` VALUES (1752138,'王来','软件工程',2017,2017,'45557993A6F10CC0C5A211E9D891ECDF','本科生',0,'QYUIA'),(1762211,'张三','软件工程',2017,2018,'1BBB0D05F3FC7FCE07A6190BB341C304','本科生',1,'QYUIA'),(1852163,'章中宏','物理系',2018,2018,'5129C4CD137D7AA301D34A65B379C33F','本科生',0,'QYUIA'),(1852263,'李一','软件工程',2018,2018,'58B8B030CEBB25FD4D55EE67129CCA65','本科生',0,'khkyy'),(1852264,'王六','汽车',2018,2018,'E578DC0B83CC46BD08E722268C6D310F','本科生',0,'mwwch'),(1852265,'金维','计算机',2018,2018,'FE624D2AE1E5A4E5915BDB876A138262','本科生',0,'xnrys'),(1852266,'李二','软件工程',2018,2018,'EFBF5D960A3B6CE184A6A0362BEC68D8','本科生',0,'fhamh'),(1852267,'王七','汽车',2018,2018,'8D2CB64E634C6624E347BF328A93D10E','本科生',0,'erwca'),(1852268,'金宣','计算机',2018,2018,'1D119014FEB87AA1ECDF5D5ABBE40825','本科生',0,'mhnew'),(1852301,'李三','软件工程',2018,2018,'9B76B14B529214ECE13AC411729870E4','本科生',0,'dwaee'),(1852302,'王五','汽车',2018,2018,'AF3E7F3302C328EAA1C0754176C818EC','本科生',0,'nnhat'),(1852305,'金祎','计算机',2018,2018,'D4FC50A750ED06DAA74D66FFC70B9284','本科生',0,'xahmy'),(1853145,'兴易','物理系',2018,2018,'2EA042CB8F2141011EF59D7A553C2B72','本科生',0,'ratcf'),(1853471,'金角','计算机',2018,2018,'A255220D5619BF482D7845372DB81B0C','本科生',0,'QYUIA'),(1853636,'侯彤','软件工程',2018,2018,'BDC2C086D3957EE024ED4C1BDEE54B79','本科生',0,'QYUIA'),(1856611,'荣光','汽车',2018,2018,'63B3656A0406015830A063C10BCE4210','本科生',0,'QYUIA');
/*!40000 ALTER TABLE `student` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `teacher`
--

DROP TABLE IF EXISTS `teacher`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `teacher` (
  `teacher_id` int NOT NULL,
  `password` varchar(45) NOT NULL,
  `department` varchar(45) NOT NULL,
  `teacher_name` varchar(45) NOT NULL,
  `is_quit` int NOT NULL DEFAULT '0' COMMENT '是否离职，0为否，1为是',
  `salt` varchar(45) DEFAULT NULL COMMENT 'MD5码的盐',
  PRIMARY KEY (`teacher_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `teacher`
--

LOCK TABLES `teacher` WRITE;
/*!40000 ALTER TABLE `teacher` DISABLE KEYS */;
INSERT INTO `teacher` VALUES (1,'A9B3F95B00F03042DE89B12E45E1E4C9','体育部','林涛',0,'82YJK'),(3,'A9B3F95B00F03042DE89B12E45E1E4C9','体育部','邹润',0,'82YJK'),(5,'A9B3F95B00F03042DE89B12E45E1E4C9','人文学院','马哲',0,'82YJK'),(6,'A9B3F95B00F03042DE89B12E45E1E4C9','软件工程','张杰',0,'82YJK'),(7,'A9B3F95B00F03042DE89B12E45E1E4C9','数学系','杨林',0,'82YJK'),(8,'A9B3F95B00F03042DE89B12E45E1E4C9','软件工程','袁胤',0,'82YJK'),(9,'A9B3F95B00F03042DE89B12E45E1E4C9','软件工程','李四',0,'82YJK'),(12,'A9B3F95B00F03042DE89B12E45E1E4C9','数学系','张鑫',0,'82YJK'),(13,'A9B3F95B00F03042DE89B12E45E1E4C9','软件工程','李承',0,'82YJK'),(278181,'1F7BE722B966587CBD9D7B62C8BA4672','软件工程','李江',0,'82YJK'),(1853636,'A9B3F95B00F03042DE89B12E45E1E4C9','软件工程','侯彤',0,'82YJK'),(1856611,'C44AC8DFE3F6F694F2581A40861DF9AE','汽车','荣光',0,'82YJK');
/*!40000 ALTER TABLE `teacher` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-06-22 18:10:08
