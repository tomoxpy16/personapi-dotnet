# personapi-dotnet

Laboratorio 1 de Arquitectura de Software: **monolito ASP.NET Core MVC con patrón DAO** que expone el CRUD del modelo `persona / profesion / estudios / telefono` de dos formas:

- **Vistas Razor (MVC)**: `/Personas`, `/Profesiones`, `/Estudios`, `/Telefonos`
- **API REST** documentada con **Swagger (OpenAPI 3)**: `/api/...` y la interfaz en `/swagger`

## Stack

| Componente | Versión usada | Notas |
|---|---|---|
| .NET SDK | 7.0.410 (`net7.0`) | El procedimiento pide .NET 6; se usó 7 porque es la versión mínima del stack del laboratorio |
| ASP.NET Core MVC | 7.0 | |
| Entity Framework Core (SqlServer + Tools) | 7.0.20 | Última versión compatible con `net7.0`; la 8.x o superior no instala |
| SQL Server | 2025 Express | Probado en 2025; los scripts también corren en 2019 y 2022 |
| SQL Server Management Studio | 21.6 | |
| Swashbuckle.AspNetCore (Swagger / OpenAPI 3) | 6.5.0 | |
| IDE | Visual Studio Community 2022 (17.14) | |

## Estructura

```
personapi-dotnet/
├── personapi-dotnet.sln
├── scripts/
│   ├── 01_ddl.sql                # crea persona_db (dueño sa) y las 4 tablas
│   └── 02_dml.sql                # datos de prueba
├── docker-compose.yml            # opcional: SQL Server 2022 en Docker
└── personapi-dotnet/
    ├── Program.cs                # DI, EF Core, Swagger, rutas
    ├── appsettings.json          # cadena de conexión "PersonaDb"
    ├── Properties/
    │   └── launchSettings.json   # perfil de ejecución (http://localhost:5000)
    ├── Models/
    │   ├── Entities/             # entidades + PersonaDbContext (Scaffold-DbContext)
    │   └── DTOs/                 # objetos que viajan por la API REST
    ├── Interfaces/               # contratos DAO (IPersonaRepository, ...)
    ├── Repository/               # implementaciones DAO con EF Core
    ├── Controllers/              # controladores MVC (devuelven vistas)
    │   └── Api/                  # controladores REST (devuelven JSON)
    ├── Views/                    # vistas Razor por entidad
    └── wwwroot/css/              # estilos propios
```

Flujo de una petición: **Vista o cliente HTTP → Controlador → Interfaz DAO → Repositorio → DbContext (EF Core) → SQL Server**.

## 1. Configurar el ambiente

1. Instalar **SQL Server Express** (2019, 2022 o 2025) con la instalación *Básica*. Queda la instancia `localhost\SQLEXPRESS` con autenticación de Windows.
2. Instalar **SQL Server Management Studio (SSMS)**.
3. Instalar **Visual Studio Community 2022** con las cargas de trabajo:
   - Desarrollo de ASP.NET y web
   - Almacenamiento y procesamiento de datos
   - En *Componentes individuales*: **Entorno de ejecución de .NET 7.0** y **SDK de .NET 7**
4. Verificar que esté el SDK de .NET 7:

   ```powershell
   dotnet --list-sdks
   ```

   Debe aparecer una versión `7.0.x`. Si no aparece, instalarla desde https://dotnet.microsoft.com/download/dotnet/7.0.

5. Clonar el repositorio:

   ```powershell
   git clone https://github.com/tomoxpy16/personapi-dotnet.git
   cd personapi-dotnet
   ```

> **Si la instancia no se llama `SQLEXPRESS`** (por ejemplo, un SQL Server Developer que usa la instancia por defecto), hay que cambiar `Server=` en la cadena de conexión (sección 3). En SSMS el nombre aparece en la ventana *Conectar al servidor*.

## 2. Crear la base de datos

En SSMS, conectarse a `localhost\SQLEXPRESS` (autenticación de Windows, marcando *Confiar en el certificado del servidor*) y ejecutar en orden:

1. `scripts/01_ddl.sql`: crea `persona_db`, le asigna como propietario a `sa` y crea las tablas con sus llaves e índices.
2. `scripts/02_dml.sql`: inserta los datos de prueba.

Desde consola es equivalente:

```powershell
sqlcmd -S localhost\SQLEXPRESS -E -C -i scripts\01_ddl.sql
sqlcmd -S localhost\SQLEXPRESS -E -C -i scripts\02_dml.sql
```

Para comprobar que quedó bien:

```sql
USE persona_db;
SELECT (SELECT COUNT(*) FROM persona)   AS personas,
       (SELECT COUNT(*) FROM profesion) AS profesiones,
       (SELECT COUNT(*) FROM estudios)  AS estudios,
       (SELECT COUNT(*) FROM telefono)  AS telefonos;
-- Esperado: 4, 4, 5, 5
```

### Opción B: SQL Server 2022 en Docker

Si no se quiere instalar SQL Server en Windows:

```powershell
docker compose up -d
sqlcmd -S localhost,1433 -U sa -P "Personapi#2025" -C -i scripts\01_ddl.sql
sqlcmd -S localhost,1433 -U sa -P "Personapi#2025" -C -i scripts\02_dml.sql
```

Y cambiar la cadena de conexión en `appsettings.json` por:

```
Server=localhost,1433;Database=persona_db;User Id=sa;Password=Personapi#2025;TrustServerCertificate=true
```

La contraseña de `sa` solo sirve para este contenedor local de laboratorio.

## 3. Configuración

### Cadena de conexión

`personapi-dotnet/appsettings.json`:

```json
"ConnectionStrings": {
  "PersonaDb": "Server=localhost\\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=true"
}
```

| Parte | Para qué sirve |
|---|---|
| `Server=localhost\SQLEXPRESS` | Servidor e instancia (en JSON la `\` va doble) |
| `Database=persona_db` | Base creada por `01_ddl.sql` |
| `Trusted_Connection=True` | Usa el usuario de Windows; no hace falta contraseña |
| `TrustServerCertificate=true` | Acepta el certificado autofirmado de SQL Server. Desde EF Core 7 (Microsoft.Data.SqlClient 5) la conexión va cifrada por defecto y sin esto falla |

`Program.cs` la lee con `builder.Configuration.GetConnectionString("PersonaDb")`, así que también se puede cambiar sin tocar el archivo, con la variable de entorno `ConnectionStrings__PersonaDb`.

### Perfil de ejecución

`personapi-dotnet/Properties/launchSettings.json` define un solo perfil, `personapi-dotnet`:

- URL: `http://localhost:5000`
- `ASPNETCORE_ENVIRONMENT=Development`: muestra errores detallados en lugar de la página `/Home/Error`
- Abre el navegador al arrancar

### Regenerar las entidades (Database First)

Las clases de `Models/Entities` se generaron desde la **Consola del Administrador de paquetes** de Visual Studio con:

```powershell
Scaffold-DbContext "Server=localhost\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/Entities
```

Después del scaffold:

- La cadena de conexión se movió de `OnConfiguring` a `appsettings.json`.
- Se añadieron anotaciones de validación (`[Required]`, `[StringLength]`, `[Display]`...).

Si se vuelve a ejecutar con `-Force`, esos cambios se pierden.

Con la CLI, fuera de Visual Studio, el equivalente es:

```powershell
dotnet tool install --global dotnet-ef --version 7.0.20
dotnet ef dbcontext scaffold "Server=localhost\SQLEXPRESS;Database=persona_db;Trusted_Connection=True;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -o Models/Entities
```

## 4. Compilar y ejecutar

### Visual Studio

1. Abrir `personapi-dotnet.sln`.
2. *Compilar → Compilar solución* (Ctrl+Mayús+B).
3. Pulsar F5 (con depuración) o Ctrl+F5 (sin depuración) con el perfil `personapi-dotnet`.

### Consola

Desde la raíz del repositorio:

```powershell
dotnet restore
dotnet build
dotnet run --project personapi-dotnet
```

La aplicación queda en http://localhost:5000:

| URL | Qué es |
|---|---|
| http://localhost:5000 | Inicio con enlaces al CRUD de cada entidad |
| http://localhost:5000/swagger | Swagger UI para probar la API |
| http://localhost:5000/swagger/v1/swagger.json | Especificación OpenAPI 3 |

> Las vistas cargan Bootstrap 5.3 desde un CDN (`cdn.jsdelivr.net`). Sin internet la aplicación funciona, pero las páginas se ven sin estilos.

> Si además del SDK 7 está instalado el SDK 8 o 9 (Visual Studio 2022 lo instala), al compilar aparece el aviso `NETSDK1138` porque .NET 7 ya no tiene soporte. Es solo una advertencia: compila y corre igual.

## 5. Endpoints REST

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/personas` | Lista personas |
| GET | `/api/personas/{cc}` | Persona por cédula |
| POST | `/api/personas` | Crea persona (409 si la cédula existe) |
| PUT | `/api/personas/{cc}` | Actualiza persona |
| DELETE | `/api/personas/{cc}` | Borra persona con sus teléfonos y estudios |
| GET/POST | `/api/profesiones` | Lista / crea profesiones |
| GET/PUT/DELETE | `/api/profesiones/{id}` | Consulta / actualiza / borra profesión (borra también sus estudios) |
| GET/POST | `/api/estudios` | Lista / crea estudios |
| GET/PUT/DELETE | `/api/estudios/{idProf}/{ccPer}` | Llave compuesta en la URL |
| GET/POST | `/api/telefonos` | Lista / crea teléfonos |
| GET/PUT/DELETE | `/api/telefonos/{num}` | Consulta / actualiza / borra teléfono |

Códigos de respuesta:

| Código | Cuándo |
|---|---|
| `200` | Consulta correcta |
| `201` | Creado; trae el encabezado `Location` |
| `204` | PUT o DELETE correcto, sin contenido |
| `400` | Datos inválidos, persona o profesión inexistente, o llave del cuerpo distinta a la de la URL |
| `404` | No existe |
| `409` | Llave duplicada |

## 6. Despliegue

### Publicar

Compilar en modo Release y dejar todo lo necesario en una carpeta:

```powershell
dotnet publish personapi-dotnet -c Release -o .\publish
```

La carpeta `publish/` queda con `personapi-dotnet.dll`, `appsettings.json`, `wwwroot/` y las dependencias. Está en el `.gitignore`, así que no se sube al repositorio.

El equipo donde se despliegue necesita el **runtime de ASP.NET Core 7.0** (no el SDK) y acceso a una base `persona_db` creada con los scripts de la sección 2.

### Opción A: Kestrel directo

```powershell
cd publish
dotnet personapi-dotnet.dll --urls http://0.0.0.0:5000
```

Fuera de Visual Studio no se lee `launchSettings.json`, así que el ambiente es `Production`: los errores muestran `/Home/Error` en lugar del detalle.

### Opción B: IIS

1. Instalar el **ASP.NET Core 7.0 Hosting Bundle**.
2. En el Administrador de IIS, crear un sitio cuyo directorio físico sea la carpeta `publish/`.
3. En su grupo de aplicaciones, poner *Versión de .NET CLR* en **Sin código administrado**.
4. Si se usa `Trusted_Connection=True`, la identidad del grupo de aplicaciones necesita permiso en `persona_db`. Si no, usar un usuario de SQL Server en la cadena de conexión.

### Cadena de conexión en producción

Se puede sobrescribir sin editar `appsettings.json`:

```powershell
$env:ConnectionStrings__PersonaDb = "Server=MI_SERVIDOR;Database=persona_db;User Id=...;Password=...;TrustServerCertificate=true"
dotnet personapi-dotnet.dll
```

## 7. Problemas comunes

| Síntoma | Causa | Solución |
|---|---|---|
| `A network-related or instance-specific error...` al abrir una página | El servicio de SQL Server está detenido o la instancia tiene otro nombre | Iniciar *SQL Server (SQLEXPRESS)* en `services.msc` o corregir `Server=` |
| `Cannot open database "persona_db"` | No se ejecutó `01_ddl.sql` | Ejecutar los scripts de la sección 2 |
| `The certificate chain was issued by an authority that is not trusted` | Falta `TrustServerCertificate=true` | Añadirlo a la cadena de conexión |
| `NU1202` al instalar paquetes de EF Core | Se instaló EF Core 8 o superior en un proyecto `net7.0` | Usar la versión `7.0.20` |
| `Address already in use` / puerto 5000 ocupado | Otra instancia de la app sigue corriendo | Detenerla (Mayús+F5 en VS) o usar `--urls http://localhost:5001` |
| Páginas sin estilos | No hay internet para el CDN de Bootstrap | Conectarse o no tener en cuenta el estilo |

## 8. Decisiones de diseño

- **DAO = Interfaces + Repository**: los controladores dependen de `IPersonaRepository` (abstracción), no de EF Core. Las implementaciones se registran en `Program.cs` con `AddScoped` y ASP.NET Core las inyecta en el constructor.
- **Dos tipos de controlador**: los de `Controllers/` heredan de `Controller` y devuelven vistas; los de `Controllers/Api/` heredan de `ControllerBase`, llevan `[ApiController]` y devuelven JSON. Ambos usan los mismos repositorios.
- **DTOs en la API**: la API no expone las entidades directamente, para no serializar las relaciones (ciclos persona → estudios → persona) y no aceptar campos que no se deben enviar.
- **Borrado**: las FK no tienen `ON DELETE CASCADE`, por eso el repositorio borra primero los teléfonos y estudios de la persona (y los estudios de una profesión) dentro del mismo `SaveChanges`, que es una sola transacción.
- **Cambios al modelo MySQL del enunciado**: `ENUM('M','F')` pasa a `CHAR(1)` con `CHECK`; `TEXT` pasa a `VARCHAR(MAX)`; `INT(n)` pasa a `INT`.

## 9. Flujo de trabajo en Git

- `main`: versiones entregadas. La entrega es el tag **`v1.0.0`**.
- `develop`: integración.
- `feature/*`: una rama por parte del trabajo, integrada a `develop` por Pull Request con *merge commit*.

| Rama | Contenido | Integración |
|---|---|---|
| `feature/scripts-bd` | Scripts DDL y DML | Base del proyecto |
| `feature/entidades-dbcontext` | Entidades y DbContext | Base del proyecto |
| `feature/layout-inicio` | Layout, inicio y Swagger | Base del proyecto |
| `feature/persona-telefono` | CRUD de persona y teléfono | PR #1 |
| `feature/profesion-estudios` | CRUD de profesión y estudios | PR #2 |

Este README se agregó directamente en `develop`. Los mensajes de commit siguen *Conventional Commits* (`feat:`, `docs:`, `merge:`...).

## Autores

- Tomás Ospina Ulloa: base del proyecto, base de datos, entidades y documentación
- David Santiago Rodríguez Prieto: CRUD de persona y teléfono
- Gustavo Salvador Aguilar: CRUD de profesión y estudios
