# Entrega del código incremental — DOC-110

El usuario confirmó que el servidor principal no tenía las tablas y que ejecutó correctamente SQL 01. Es confirmación del operador; no se consultó directamente ese servidor. No volver a ejecutar SQL 01 ni aplicar la migración 03/04 sobre esa instalación nueva.

Se preparó el candidato `.tmp/doc110-entrega-incremental.zip`, con manifiesto SHA-256 y seis archivos de sitio. La compilación Release del proyecto WebForms real terminó correctamente, en `.tmp/doc110-release-build/`, sin sustituir el DLL local usado por E2E. No se publicó en el servidor principal.

| Archivo de destino | Función |
|---|---|
| bin/GestionDocumental-Docuarchi.net.dll | Modelos, repositorio, servicio, guard, ASMX y cambios compilados de Producción/Global. |
| Gestion/WebFormProducionDocumental.aspx | Contenedor y referencias del editor de perfiles. |
| webservice/WebServiceImportacionCarpetas.asmx | Punto de entrada del servicio. |
| js/gestion/ImportacionCarpetas/PerfilImportacionApi.js | Transporte HTTP del perfil. |
| js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js | Editor y operaciones del perfil. |
| js/gestion/ImportacionCarpetas/PerfilImportacionEditor.css | Presentación del editor. |

El DLL pertenece a toda la aplicación, no es un complemento aislado. Antes de usar este candidato, verificar que el servidor principal corresponde a la misma base de código y dependencias. El manifiesto registra el commit base y aclara que contiene cambios aún sin commit; no representa una versión publicada en Git. Si el principal tiene otra versión, integrar y compilar DOC-110 sobre esa versión, no reemplazar a ciegas el ensamblado compartido.

## Aplicación por el operador

1. Confirmar destino IIS, versión base y dependencias existentes MySql.Data/Newtonsoft.Json compatibles con el proyecto. El perfil gestor.pubxml encontrado solo publica a C:\salida_sitio y elimina archivos existentes; no identifica el principal y no se utilizó.
2. Respaldar los seis archivos que existan y registrar cuáles son nuevos. Programar una ventana breve del sitio, sin sobrescribir un archivo de mantenimiento ajeno.
3. Copiar el contenido de sitio/ respetando las rutas y comprobar SHA-256 contra manifest.json. El paquete no incluye Web.config, conexiones, credenciales, repositorios documentales ni datos de pruebas.
4. Reiniciar la aplicación y comprobar login, Producción y acceso al editor con una cuenta autorizada. El permiso ADMINISTRAR_PERFILES_IMPORTACION debe concederse por el mecanismo administrativo existente; crear la columna no concede permiso automáticamente.
5. Validar creación/guardado/publicación en el ambiente de pruebas autorizado. No ejecutar E2E que escriba datos en el principal sin autorización explícita para ese ambiente y cuenta. Mantener el alcance de empresa y el nivel documental del árbol existente.
6. Ante fallo de despliegue, detener escrituras y restaurar archivos respaldados; retirar únicamente archivos nuevos identificados. Conservar las tablas y los datos creados. No convertir IDs ni borrar tablas como parte de la reversión de archivos.

## Verificación pendiente

Se abrió la consola visible del runner existente para el recorrido local `npm.cmd --prefix tools/e2e run test:doc110:session -- --publish --headed`; requiere credenciales efímeras ingresadas allí por el usuario. El PASS histórico sin INCREMENTAL_IDENTITIES no acredita esta corrida. La prueba local usa el DLL Debug ya desplegado; no acredita por sí sola el candidato Release ni el servidor principal.

Faltan confirmación del destino/procedimiento de publicación y resultados de aceptación. El despliegue principal y las tareas de E2E/concurrencia permanecen abiertos.

Actualización COR-03: el paquete temporal mencionado arriba precede al autocompletado y no representa el código vigente. No usarlo para entregar COR-03. La publicación principal corresponde al usuario; recompilar desde la versión que incluya esta corrección.
