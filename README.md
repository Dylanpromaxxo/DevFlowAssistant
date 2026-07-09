# DevFlow Assistant

DevFlow Assistant es una aplicacion de escritorio WPF para automatizar tareas rutinarias de desarrollo desde workflows locales. Un workflow puede abrir aplicaciones, abrir paginas web, ejecutar comandos, lanzar scripts o usar Docker/Docker Compose.

## Objetivo

El objetivo del proyecto es ofrecer una herramienta local y simple para preparar entornos de desarrollo con un boton, manteniendo una arquitectura limpia y facil de extender.

## Stack

- C# / .NET 8
- WPF
- SQLite
- Entity Framework Core
- Dependency Injection
- Arquitectura por capas

## Estructura del proyecto

- `Domain`: entidades principales y constantes de dominio.
- `DevFlowAssistant.Application`: contratos, modelos de entrada y servicios de negocio.
- `DevFlowAssistant.Infrastructure`: EF Core, repositorios, SQLite e inicializacion de base de datos.
- `DevFlow Assistant`: aplicacion WPF, vistas, ViewModels, navegacion, estilos y recursos.

## Entidades principales

- `Workflow`: representa una automatizacion completa. Tiene nombre, descripcion, estado activo y fechas.
- `WorkflowAction`: representa una accion dentro de un workflow. Tiene tipo, valor, argumentos, directorio, timeout, orden y estado.
- `ExecutionLog`: registra el historial de ejecucion, estado, duracion, salida y errores.

## Pantallas

- `Dashboard`: muestra resumen de workflows y ejecuciones recientes.
- `Workflows`: lista workflows, permite buscar, abrir detalle, editar o eliminar.
- `Crear workflow`: formulario dedicado para crear workflows con placeholders y validacion.
- `Editar workflow`: modifica nombre, descripcion y estado.
- `Detalle workflow`: muestra resumen, acciones asociadas y boton de ejecucion.
- `Acciones`: administra acciones del workflow, orden, tipo, argumentos y timeout.
- `Historial`: muestra logs recientes por workflow o generales.

## Conexion entre UI y logica

La UI usa MVVM:

1. La vista XAML expone botones, formularios y tablas.
2. Cada vista se conecta a un ViewModel especifico.
3. El ViewModel ejecuta comandos (`ICommand`).
4. Los comandos llaman servicios de Application.
5. Los servicios validan reglas y usan repositorios.
6. Los repositorios leen/escriben SQLite con EF Core.

`MainWindow` solo funciona como shell: menu lateral, layout general y `ContentControl` para cambiar pantallas. La logica de workflows, acciones y logs vive en ViewModels y servicios separados.

## Base de datos y fechas

SQLite no tiene un tipo fecha estricto. Para evitar errores al crear o leer workflows, DevFlow guarda fechas como texto ISO UTC usando conversiones de EF Core.

Esto evita problemas con fechas guardadas en formatos locales como `9/7/2026 8:10 PM`. Las fechas nuevas se guardan con `DateTime.UtcNow` y formato round-trip.

La base local se guarda en:

`%LocalAppData%/DevFlowAssistant/DevFlow.db`

## Ejecutar

```powershell
dotnet build "DevFlow Assistant.slnx"
```

Luego ejecuta el proyecto WPF desde Visual Studio o con:

```powershell
dotnet run --project "DevFlow Assistant/DevFlowAssistant.Desktop.csproj"
```

## Crear un workflow paso a paso

1. Abre DevFlow Assistant.
2. Entra a `Workflows`.
3. Presiona `Nuevo workflow`.
4. Escribe un nombre, por ejemplo `Preparar backend`.
5. Escribe una descripcion, por ejemplo `Abre VS Code, levanta Docker y ejecuta dotnet build`.
6. Guarda.
7. En el detalle, entra a `Acciones`.
8. Agrega las acciones necesarias.
9. Vuelve al detalle y presiona `Ejecutar`.
10. Revisa el resultado en `Historial`.

## Ejemplos de workflows

### Abrir una aplicacion

- Nombre: `Abrir editor`
- Descripcion: `Abre VS Code en el proyecto actual`
- Acciones:
  - Tipo: `OpenApp`
  - Valor: `code`
  - Argumentos: `.`
  - Directorio: ruta del proyecto
- Resultado esperado: VS Code abre el directorio indicado.
- En la app: se ve como un workflow con una accion activa y un log `Succeeded`.

### Ejecutar una accion automatica

- Nombre: `Verificar SDK`
- Descripcion: `Muestra la version instalada de .NET`
- Acciones:
  - Tipo: `RunCommand`
  - Valor: `dotnet`
  - Argumentos: `--version`
- Resultado esperado: el log registra la salida del comando.
- En la app: aparece una fila en historial con duracion y estado correcto.

### Registrar una tarea

- Nombre: `Crear nota de inicio`
- Descripcion: `Ejecuta un script local que prepara archivos de trabajo`
- Acciones:
  - Tipo: `RunCommand`
  - Valor: `powershell`
  - Argumentos: `-File scripts/start-day.ps1`
  - Directorio: ruta del repositorio
- Resultado esperado: el script corre y el log guarda la salida.
- En la app: el workflow muestra el script como accion asociada.

### Workflow con varias acciones

- Nombre: `Preparar ambiente completo`
- Descripcion: `Abre herramientas y levanta servicios`
- Acciones:
  - `OpenApp`: `code`, argumentos `.`
  - `OpenUrl`: `http://localhost:5000/swagger`
  - `DockerCompose`: valor `up`, argumentos `-d`
  - `RunCommand`: `dotnet`, argumentos `build`
- Resultado esperado: el entorno queda listo para trabajar.
- En la app: las acciones se muestran ordenadas y pueden subirse o bajarse.

### Workflow con historial

- Nombre: `Build diario`
- Descripcion: `Compila el proyecto y guarda el resultado`
- Acciones:
  - `RunCommand`: `dotnet`, argumentos `build`
- Resultado esperado: cada ejecucion crea un `ExecutionLog`.
- En la app: la pantalla `Historial` muestra estado, mensaje, duracion y errores.

## Mejoras futuras

- Plantillas de workflows comunes.
- Variables por workflow.
- Importar/exportar workflows.
- Ejecucion programada.
- Confirmaciones antes de eliminar.
- Tests automatizados de servicios.
- Vista detallada de salida estandar y errores.
