CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `ConsolePlays` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `ConsoleId` int NOT NULL,
    `Gamer` int NOT NULL,
    `Price` float NOT NULL,
    `isFixedTime` bit NOT NULL,
    `FixedTime` longtext NULL,
    `PauseStart` longtext NULL,
    `PauseFinish` longtext NULL,
    `PauseDuration` int NOT NULL,
    `PlayStart` longtext NULL,
    `PlayFinish` longtext NULL,
    `Paid` double NOT NULL,
    CONSTRAINT `PK_ConsolePlays` PRIMARY KEY (`Id`)
);

CREATE TABLE `Consoles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ConsoleName` longtext NULL,
    `ConsoleNumber` int NOT NULL,
    `Type` longtext NULL,
    `StandartPrice` float NOT NULL,
    `CurrentPrice` float NOT NULL,
    CONSTRAINT `PK_Consoles` PRIMARY KEY (`Id`)
);

CREATE TABLE `Magazines` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext NULL,
    `Price` float NOT NULL,
    `Quantity` int NOT NULL,
    `Type` longtext NULL,
    CONSTRAINT `PK_Magazines` PRIMARY KEY (`Id`)
);

CREATE TABLE `PromotionConsoles` (
    `id` int NOT NULL AUTO_INCREMENT,
    `promotionId` int NOT NULL,
    `consoleId` int NOT NULL,
    CONSTRAINT `PK_PromotionConsoles` PRIMARY KEY (`id`)
);

CREATE TABLE `Promotions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `PromotionName` longtext NULL,
    `PromotionDesc` longtext NULL,
    `Price` int NOT NULL,
    `AllTime` bit NOT NULL,
    `DateStart` longtext NULL,
    `DateEnd` longtext NULL,
    `HourStart` longtext NULL,
    `HourEnd` longtext NULL,
    CONSTRAINT `PK_Promotions` PRIMARY KEY (`Id`)
);

CREATE TABLE `Role` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `StatusRole` longtext NULL,
    CONSTRAINT `PK_Role` PRIMARY KEY (`Id`)
);

CREATE TABLE `StaffHistories` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `EnterTime` longtext NULL,
    `ExitTime` longtext NULL,
    `CountEnter` int NOT NULL,
    CONSTRAINT `PK_StaffHistories` PRIMARY KEY (`Id`)
);

CREATE TABLE `Trades` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `MagazineId` int NOT NULL,
    `QuantityProduct` int NOT NULL,
    `BuyDate` longtext NULL,
    CONSTRAINT `PK_Trades` PRIMARY KEY (`Id`)
);

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `AvatarId` int NOT NULL,
    `FirstName` longtext NULL,
    `LastName` longtext NULL,
    `Email` longtext NULL,
    `Number` int NOT NULL,
    `PasswordHash` longblob NULL,
    `PasswordSalt` longblob NULL,
    `CreateDate` longtext NULL,
    `FK_Role` int NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190214174648_mysqlInitial', '2.2.1-servicing-10028');

