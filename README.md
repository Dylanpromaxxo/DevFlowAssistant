# DevFlow Assistant

DevFlow Assistant es una aplicación de escritorio desarrollada con **WPF**, **.NET**, **Entity Framework Core** y **SQLite**. Su objetivo es permitir la gestión local de flujos de trabajo mediante una interfaz de escritorio simple y una base de datos liviana.

El proyecto está pensado para funcionar de manera local, sin necesidad de un servidor externo de base de datos. Para ello utiliza SQLite como motor de persistencia.

## Tecnologías utilizadas

- C#
- .NET
- WPF
- SQLite
- Entity Framework Core
- Dependency Injection
- Arquitectura por capas

## Características principales

- Aplicación de escritorio con WPF.
- Persistencia local usando SQLite.
- Integración con Entity Framework Core.
- Uso de repositorios y servicios.
- Inyección de dependencias con `Microsoft.Extensions.DependencyInjection`.
- Base de datos incluida como plantilla dentro del proyecto.
- Copia automática de la base de datos al directorio local del usuario.
