# PostgreSQL para desarrollo

ServiceDesk utiliza PostgreSQL 17 mediante Docker Compose.
La API utiliza Entity Framework Core y el proveedor Npgsql.

## Entorno verificado

Verificación realizada el 19 de octubre de 2026.

| Componente | Versión |
| --- | --- |
| .NET SDK | 10.0.401 |
| Docker CLI de Windows | 25.0.3 |
| Docker Compose | 2.24.6-desktop.1 |
| dotnet-ef local | 10.0.4 |

La API utiliza HTTP 5022 y HTTPS 7238.
PostgreSQL utiliza el puerto 5432 del host por defecto.

Ejecutar los comandos desde la raíz del repositorio.

## Preparar las variables de Docker

En una instalación nueva, copiar el ejemplo.

```powershell
Copy-Item .env.example .env
```

Editar `.env` y completar `POSTGRES_PASSWORD`.
Conservar un `.env` existente si ya contiene la configuración local.

Las variables requeridas son POSTGRES_DB, POSTGRES_USER y
POSTGRES_PASSWORD. POSTGRES_PORT permite cambiar el puerto del host.

El archivo `.env` queda excluido de Git.
No compartir su contenido ni guardar contraseñas en la documentación.

## Iniciar PostgreSQL

Abrir Docker Desktop y esperar a que el motor esté disponible.

```powershell
docker compose config --quiet
docker compose up -d --wait postgres
docker compose ps
```

El servicio debe aparecer como healthy.

Comprobar la conexión interna.

```powershell
docker compose exec postgres psql -U servicedesk_dev -d servicedesk_dev -c 'SELECT current_database(), current_user;'
```

Los comandos suponen los nombres incluidos en `.env.example`.

## Configurar la conexión de la API

En Visual Studio, utilizar Manage User Secrets sobre
ServiceDesk.Tickets.Api y guardar esta estructura con la contraseña local.

```json
{
  "ConnectionStrings": {
    "Tickets": "Host=localhost;Port=5432;Database=servicedesk_dev;Username=servicedesk_dev;Password=<contraseña local>"
  }
}
```

Sustituir el marcador antes de utilizar la conexión.
El archivo de secretos queda fuera del repositorio.

La API carga User Secrets en el entorno Development.
Docker Compose lee `.env`; la API no importa ese archivo.

Los secretos pertenecen al usuario y a la máquina.
Un clon del repositorio no copia los secretos.

Cambiar POSTGRES_PASSWORD en `.env` no actualiza la contraseña
de un usuario existente dentro de un volumen inicializado.

## Restaurar herramientas y aplicar migraciones

El manifiesto `dotnet-tools.json` fija dotnet-ef 10.0.4.

```powershell
dotnet tool restore
dotnet ef --version
dotnet build
```

Aplicar las migraciones existentes.

```powershell
dotnet ef database update `
    --project .\src\services\TicketService\ServiceDesk.Tickets.Infrastructure `
    --startup-project .\src\services\TicketService\ServiceDesk.Tickets.Api `
    --context TicketsDbContext `
    -- --environment Development
```

Una instalación nueva aplica InitialUserReferences y AddTicketsAndHistory.
No generar migraciones nuevas para instalar el esquema existente.

Verificar las tablas.

```powershell
docker compose exec postgres psql -U servicedesk_dev -d servicedesk_dev -c '\dt'
```

Deben aparecer user_references, tickets, ticket_history y
__EFMigrationsHistory.

Una segunda ejecución de database update debe indicar que
la base está actualizada.

## Ejecutar las pruebas

PostgreSQL debe estar activo y tener las migraciones aplicadas.

```powershell
dotnet test
```

La prueba de persistencia lee User Secrets y después variables de entorno.
ConnectionStrings__Tickets permite seleccionar otra base para las pruebas.

La prueba guarda y consulta un ticket con su historial.
Utiliza dos contextos y revierte los datos mediante una transacción.
No crea ni migra la base automáticamente.

## Verificar desde una base vacía

Crear una base exclusiva para la comprobación.

```powershell
docker compose exec postgres createdb -U servicedesk_dev servicedesk_setup_check_71
```

Si ya existe, utilizar otro nombre y ajustarlo en los comandos.
No borrar una base existente para repetir esta verificación.

Preparar una conexión temporal desde los secretos locales.

```powershell
& {
    $secretsPath = Join-Path $env:APPDATA 'Microsoft\UserSecrets\8bbb66ac-376b-4347-9082-3565f6f8c067\secrets.json'
    $localSecrets = Get-Content $secretsPath -Raw | ConvertFrom-Json

    $originalConnection = $localSecrets.'ConnectionStrings:Tickets'

    if ([string]::IsNullOrWhiteSpace($originalConnection)) {
        $originalConnection = $localSecrets.ConnectionStrings.Tickets
    }

    if ([string]::IsNullOrWhiteSpace($originalConnection)) {
        throw 'Falta ConnectionStrings:Tickets en los secretos locales.'
    }

    $checkConnection = [System.Data.Common.DbConnectionStringBuilder]::new()
    $checkConnection.set_ConnectionString($originalConnection)

    if (-not $checkConnection.ContainsKey('Host')) {
        throw 'La conexión no contiene Host.'
    }

    $checkConnection['Database'] = 'servicedesk_setup_check_71'
    $env:ConnectionStrings__Tickets = $checkConnection.get_ConnectionString()
}
```

No imprimir las variables que contienen la conexión.

Desde esa misma terminal, ejecutar database update y dotnet test.
Confirmar la base y sus tablas.

```powershell
docker compose exec postgres psql -U servicedesk_dev -d servicedesk_setup_check_71 -c 'SELECT current_database();'
docker compose exec postgres psql -U servicedesk_dev -d servicedesk_setup_check_71 -c '\dt'
```

Repetir database update y confirmar que no hay migraciones pendientes.

## Verificar la persistencia del volumen

Crear un registro auxiliar en la base de comprobación.

```powershell
docker compose exec postgres psql -U servicedesk_dev -d servicedesk_setup_check_71 -c 'CREATE TABLE setup_persistence_check (id integer PRIMARY KEY); INSERT INTO setup_persistence_check VALUES (71);'
```

Recrear el contenedor conservando su volumen.

```powershell
docker compose up -d --force-recreate --wait postgres
```

Consultar el registro después de la recreación.

```powershell
docker compose exec postgres psql -U servicedesk_dev -d servicedesk_setup_check_71 -c 'SELECT * FROM setup_persistence_check;'
```

Debe devolver 71.

Eliminar la tabla auxiliar y retirar la conexión temporal.

```powershell
docker compose exec postgres psql -U servicedesk_dev -d servicedesk_setup_check_71 -c 'DROP TABLE IF EXISTS setup_persistence_check;'
Remove-Item Env:ConnectionStrings__Tickets -ErrorAction SilentlyContinue
```

La base de comprobación permanece disponible.
Retirar la variable devuelve la configuración al secreto local habitual.

## Detener y volver a iniciar

```powershell
docker compose down
docker compose up -d --wait postgres
```

El volumen nombrado conserva los datos.
`docker compose down -v` elimina el volumen y sus bases.
No utilizar esa opción para detener el entorno.

## Acceso desde Linux

Si PostgreSQL corre en Windows, localhost desde Linux apunta a Linux.
Configurar Host con la dirección IPv4 actual del equipo Windows.

El acceso requiere conectividad de red y permiso del firewall de Windows.
El Compose actual publica el puerto en las interfaces del host.

Configurar User Secrets también en Linux.
Usar la misma clave ConnectionStrings:Tickets del proyecto API.
El identificador de secretos está versionado; sus valores son locales.

Los comandos dotnet aceptan rutas con `/`.
En Bash, usar `\` para continuar líneas en lugar del acento de PowerShell.

El bloque de lectura de secretos anterior es específico de Windows.
La configuración de acceso desde Linux debe verificarse en esa máquina.

## Resultado de la verificación

El 19 de octubre de 2026 se aplicaron ambas migraciones
sobre servicedesk_setup_check_71, inicialmente vacía.

Se comprobaron las cuatro tablas y pasaron cuatro pruebas.
Una segunda actualización confirmó que no había migraciones pendientes.

La consulta del registro auxiliar devolvió 71.
Para acreditar persistencia, esta consulta debe ejecutarse después
de recrear el contenedor conservando el volumen.

La compilación conservó las advertencias NU1903 preexistentes
de Microsoft.OpenApi. Su corrección queda pendiente.

Esta comprobación valida una base vacía dentro del contenedor existente.
No constituye una instalación verificada en un equipo nuevo.