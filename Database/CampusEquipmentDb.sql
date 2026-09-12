/* ============================================================
   Campus Equipment Management System
   Database: CampusEquipmentDb
   Database-First SQL Script
   ============================================================ */

USE master;
GO

/* ============================================================
   CREATE DATABASE IF IT DOES NOT EXIST
   ============================================================ */

IF DB_ID('CampusEquipmentDb') IS NULL
BEGIN
    CREATE DATABASE CampusEquipmentDb;
END
GO

USE CampusEquipmentDb;
GO

/* ============================================================
   DROP EXISTING TABLES
   ============================================================ */

IF OBJECT_ID('dbo.Equipment', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Equipment;
END
GO

IF OBJECT_ID('dbo.Department', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Department;
END
GO

/* ============================================================
   DEPARTMENT TABLE
   ============================================================ */

CREATE TABLE dbo.Department
(
    DepartmentId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,

    CONSTRAINT PK_Department
        PRIMARY KEY (DepartmentId),

    CONSTRAINT UQ_Department_Name
        UNIQUE (Name)
);
GO

/* ============================================================
   EQUIPMENT TABLE
   ============================================================ */

CREATE TABLE dbo.Equipment
(
    EquipmentId INT IDENTITY(1,1) NOT NULL,
    AssetCode NVARCHAR(50) NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Brand NVARCHAR(100) NULL,
    Model NVARCHAR(100) NULL,
    PurchaseDate DATE NULL,
    Status NVARCHAR(30) NOT NULL,
    DepartmentId INT NOT NULL,

    CONSTRAINT PK_Equipment
        PRIMARY KEY (EquipmentId),

    CONSTRAINT UQ_Equipment_AssetCode
        UNIQUE (AssetCode),

    CONSTRAINT FK_Equipment_Department
        FOREIGN KEY (DepartmentId)
        REFERENCES dbo.Department(DepartmentId),

    CONSTRAINT CK_Equipment_Status
        CHECK
        (
            Status IN
            (
                'Available',
                'Assigned',
                'UnderMaintenance',
                'Retired'
            )
        )
);
GO

/* ============================================================
   SAMPLE DEPARTMENTS
   ============================================================ */

INSERT INTO dbo.Department (Name)
VALUES
    ('Information Technology'),
    ('Computer Science'),
    ('Engineering'),
    ('Business Administration');
GO

/* ============================================================
   SAMPLE EQUIPMENT
   ============================================================ */

INSERT INTO dbo.Equipment
(
    AssetCode,
    Name,
    Category,
    Brand,
    Model,
    PurchaseDate,
    Status,
    DepartmentId
)
VALUES
(
    'PC-001',
    'Desktop Computer',
    'Computer',
    'Dell',
    'OptiPlex 7010',
    '2025-01-15',
    'Available',
    1
),
(
    'PC-002',
    'Desktop Computer',
    'Computer',
    'HP',
    'ProDesk 400',
    '2024-06-10',
    'Assigned',
    2
),
(
    'PRJ-001',
    'Projector',
    'Projector',
    'Epson',
    'EB-X06',
    '2024-03-20',
    'Available',
    1
),
(
    'LAP-001',
    'Laptop',
    'Computer',
    'Lenovo',
    'ThinkPad E14',
    '2023-08-05',
    'UnderMaintenance',
    3
),
(
    'PC-003',
    'Desktop Computer',
    'Computer',
    'Acer',
    'Veriton',
    '2022-02-15',
    'Retired',
    2
);
GO

/* ============================================================
   VERIFICATION
   ============================================================ */

SELECT
    DepartmentId,
    Name
FROM dbo.Department
ORDER BY DepartmentId;
GO

SELECT
    EquipmentId,
    AssetCode,
    Name,
    Category,
    Brand,
    Model,
    PurchaseDate,
    Status,
    DepartmentId
FROM dbo.Equipment
ORDER BY EquipmentId;
GO