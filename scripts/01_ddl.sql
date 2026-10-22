/* =====================================================================
   personapi-dotnet - Script DDL (SQL Server 2019/2022)
   Traduccion del modelo MySQL (arq_per_db) a T-SQL, base de datos persona_db.

   Diferencias respecto al script MySQL original:
     - ENUM('M','F')  -> CHAR(1) + CHECK (SQL Server no tiene ENUM).
     - INT(15), INT(6), INT(3) -> INT (en SQL Server INT no lleva ancho).
     - TEXT -> VARCHAR(MAX) (TEXT esta obsoleto en SQL Server).
     - Los indices de las FK se crean explicitamente con CREATE INDEX.
   ===================================================================== */

IF DB_ID('persona_db') IS NULL
    CREATE DATABASE persona_db;
GO

-- Paso 4 del procedimiento: la base de datos queda con propietario "sa".
ALTER AUTHORIZATION ON DATABASE::persona_db TO sa;
GO

USE persona_db;
GO

-- ---------------------------------------------------------------------
-- Tabla persona
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.persona', 'U') IS NULL
CREATE TABLE dbo.persona (
    cc        INT          NOT NULL,
    nombre    VARCHAR(45)  NOT NULL,
    apellido  VARCHAR(45)  NOT NULL,
    genero    CHAR(1)      NOT NULL,
    edad      INT          NULL,
    CONSTRAINT PK_persona PRIMARY KEY (cc),
    CONSTRAINT CK_persona_genero CHECK (genero IN ('M', 'F'))
);
GO

-- ---------------------------------------------------------------------
-- Tabla profesion
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.profesion', 'U') IS NULL
CREATE TABLE dbo.profesion (
    id   INT           NOT NULL,
    nom  VARCHAR(90)   NOT NULL,
    des  VARCHAR(MAX)  NULL,
    CONSTRAINT PK_profesion PRIMARY KEY (id)
);
GO

-- ---------------------------------------------------------------------
-- Tabla estudios (relacion N:M entre persona y profesion)
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.estudios', 'U') IS NULL
CREATE TABLE dbo.estudios (
    id_prof  INT          NOT NULL,
    cc_per   INT          NOT NULL,
    fecha    DATE         NULL,
    univer   VARCHAR(50)  NULL,
    CONSTRAINT PK_estudios PRIMARY KEY (id_prof, cc_per),
    CONSTRAINT estudio_persona_fk   FOREIGN KEY (cc_per)  REFERENCES dbo.persona (cc),
    CONSTRAINT estudio_profesion_fk FOREIGN KEY (id_prof) REFERENCES dbo.profesion (id)
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'estudio_persona_fk' AND object_id = OBJECT_ID('dbo.estudios'))
    CREATE INDEX estudio_persona_fk ON dbo.estudios (cc_per);
GO

-- ---------------------------------------------------------------------
-- Tabla telefono (relacion 1:N persona -> telefono)
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.telefono', 'U') IS NULL
CREATE TABLE dbo.telefono (
    num     VARCHAR(15)  NOT NULL,
    oper    VARCHAR(45)  NOT NULL,
    duenio  INT          NOT NULL,
    CONSTRAINT PK_telefono PRIMARY KEY (num),
    CONSTRAINT telefono_persona_fk FOREIGN KEY (duenio) REFERENCES dbo.persona (cc)
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'telefono_persona_fk' AND object_id = OBJECT_ID('dbo.telefono'))
    CREATE INDEX telefono_persona_fk ON dbo.telefono (duenio);
GO
