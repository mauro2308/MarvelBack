Este proyecto fue construido con el framework .NET 8 utilizando una arquitectura de tipo Domain-Centric (Hexagonal, Clean, Onion).

Responsabilidades del Proyecto
El proyecto se enfoca en las operaciones básicas de un CRUD para productos, expuestos a través de un API RESTful. La documentación de los endpoints puede consultarse en el propio código del proyecto, donde se incluyen ejemplos de uso y especificaciones de cada endpoint.

Pasos previos para iniciar el proyecto
El único requisito para iniciar este microservicio es crear una base de datos en MySQL y configurar las credenciales en el archivo appsettings.json. Luego, se debe ejecutar el comando: Update-Database desde la ventana de Package Manager Console, seleccionando el proyecto de Infrastructure. Esto aplicará las migraciones y creará las tablas necesarias en la base de datos.

Patrones y Estilos de Arquitectura Implementados

Clean Architecture (Arquitectura Limpia)
El proyecto sigue los principios de Clean Architecture, que buscan crear aplicaciones altamente desacopladas, modulares y fácilmente testeables. Esto se logra separando las responsabilidades en capas claramente definidas:

Domain (Núcleo del Negocio): Contiene entidades, objetos de valor, agregados y servicios de dominio, completamente aislados de las tecnologías externas.

Application (Capa de Aplicación): Maneja la lógica de orquestación, incluyendo comandos, queries y servicios de aplicación.

Infrastructure (Infraestructura): Contiene adaptadores para bases de datos, APIs externas y otros servicios externos.

Api (Interfaz de Usuario / Exposición): Punto de entrada para las solicitudes de usuarios y otros sistemas, expone la lógica de negocio a través de endpoints RESTful.

Arquitectura de Puertos y Adaptadores (Hexagonal Architecture)
El enfoque de Puertos y Adaptadores (Hexagonal Architecture) se centra en hacer que el dominio de la aplicación sea el núcleo del sistema, desacoplado de tecnologías externas. Todo el flujo de datos entra y sale del dominio a través de puertos, lo que permite una mayor flexibilidad y testabilidad. Esto asegura que el dominio no tenga conocimiento directo de las tecnologías externas, como bases de datos o APIs, facilitando futuras modificaciones.

CQRS (Command Query Responsibility Segregation)
Este proyecto implementa CQRS para separar las responsabilidades de lectura y escritura:

Queries: Las consultas están optimizadas para la presentación de datos y utilizan Dapper para consultas rápidas y ligeras. Esto es ideal para operaciones como "ver productos" o "listar categorías", donde se requiere rapidez y eficiencia.

Commands: Los comandos se utilizan para operaciones que modifican el estado del sistema, como crear, actualizar o eliminar productos. Estos se manejan con Entity Framework Core, que permite trabajar con objetos de dominio más ricos en lógica de negocio.

Especificaciones Técnicas
Framework: .NET 8 (Top level statements, minimal APIs, mapGroup para endpoints, global usings, records).

Inyección de Dependencias: Uso de anotaciones DomainService para servicios de dominio y Repository para repositorios.

Entity Framework Core 7 (MySQL): Patrón Code First para la generación automática de esquemas de base de datos.

FluentValidation: Validación de comandos y objetos de entrada.

Repositorio Genérico y Extendido: Patrón para manejar agregados y ocultar detalles de implementación.

Shadow Properties: Propiedades ocultas en entidades para mejorar el diseño del dominio sin "contaminar" las entidades.

UnitOfWork: Patrón para gestionar transacciones que afectan múltiples repositorios.

Factory Logic: Uso de patrones de fábrica para la creación controlada de objetos complejos.

Pruebas Unitarias:

- Domain: Pruebas de lógica de negocio con xUnit.

- Mocks: Uso de NSubstitute para simular dependencias en pruebas unitarias.

- Pruebas de Integración: Validación de la API REST usando xUnit.

Swagger: Documentación automática de los endpoints.

Estructura del Proyecto
El proyecto sigue una estructura modular para facilitar el mantenimiento y la escalabilidad:

Api: Punto de entrada del sistema, API REST.

Api.Tests: Pruebas de integración para la API.

Application: Capa de orquestación de servicios de dominio (ports, commands, queries, handlers).

Infrastructure: Adaptadores para persistencia y acceso a datos.

Domain: Entidades, objetos de valor, servicios de dominio y agregados.

CI/CD
El proyecto está configurado para despliegues automatizados usando GitHub Actions, sin depender de servicios como Azure:

Versionamiento:

Uso de GitVersion para manejo de versiones semánticas (SemVer).

Conventional Changelog para generar changelogs automáticos basados en los mensajes de commit.

Contenedores Docker:

Las aplicaciones están listas para ser dockerizadas, lo que facilita su despliegue y portabilidad.

Despliegue Automático:

Los despliegues se manejan automáticamente a través de GitHub Actions tras cada push o merge a las ramas main o master.