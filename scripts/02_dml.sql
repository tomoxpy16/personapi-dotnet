/* =====================================================================
   personapi-dotnet - Script DML (datos de prueba)
   Ejecutar despues de 01_ddl.sql. Es idempotente: limpia y vuelve a insertar.
   ===================================================================== */

USE persona_db;
GO

-- Se borra en orden inverso a las dependencias (hijas primero).
DELETE FROM dbo.estudios;
DELETE FROM dbo.telefono;
DELETE FROM dbo.profesion;
DELETE FROM dbo.persona;
GO

INSERT INTO dbo.persona (cc, nombre, apellido, genero, edad) VALUES
    (1001234567, 'Ana',    'Gomez',    'F', 22),
    (1002345678, 'Carlos', 'Ramirez',  'M', 25),
    (1003456789, 'Laura',  'Martinez', 'F', 30),
    (1004567890, 'Andres', 'Lopez',    'M', NULL);

INSERT INTO dbo.profesion (id, nom, des) VALUES
    (1, 'Ingenieria de Sistemas',   'Diseno, desarrollo y gestion de sistemas de informacion.'),
    (2, 'Ingenieria Industrial',    'Optimizacion de procesos productivos y de servicios.'),
    (3, 'Administracion de Empresas', NULL),
    (4, 'Medicina',                 'Prevencion, diagnostico y tratamiento de enfermedades.');

INSERT INTO dbo.estudios (id_prof, cc_per, fecha, univer) VALUES
    (1, 1001234567, '2024-12-10', 'Pontificia Universidad Javeriana'),
    (2, 1002345678, '2023-06-15', 'Universidad Nacional de Colombia'),
    (1, 1003456789, '2018-11-30', 'Universidad de los Andes'),
    (3, 1003456789, '2021-05-20', 'Universidad del Rosario'),
    (4, 1004567890, NULL,         'Universidad de Antioquia');

INSERT INTO dbo.telefono (num, oper, duenio) VALUES
    ('3001112233', 'Claro',    1001234567),
    ('3102223344', 'Movistar', 1001234567),
    ('3203334455', 'Tigo',     1002345678),
    ('3014445566', 'Claro',    1003456789),
    ('3155556677', 'WOM',      1004567890);
GO

SELECT 'persona'   AS tabla, COUNT(*) AS filas FROM dbo.persona
UNION ALL SELECT 'profesion', COUNT(*) FROM dbo.profesion
UNION ALL SELECT 'estudios',  COUNT(*) FROM dbo.estudios
UNION ALL SELECT 'telefono',  COUNT(*) FROM dbo.telefono;
GO
