# DOC-110: condiciones de las tres verificaciones pendientes

Estado actual: **178/178 tareas completas**. 10.16 fue completada mediante [instalación aislada autorizada](doc110-instalacion-nueva-aislada-20261027.md), 7.5 mediante [renombrado/restauración y carga oficial autorizados en el ambiente de pruebas](doc110-sin-esquema-ambiente-pruebas.md), y 15.19 mediante [proveedor legado sintético inactivo y rechazo ASMX](doc110-proveedor-legado-sintetico-20261027.md). Las secciones inferiores conservan condiciones y límites históricos previos a la autorización expresa del usuario para usar las tablas del ambiente actual de pruebas; no representan pendientes actuales. No se afirma que el fixture sintético sea un proveedor histórico real.

## Inspección posterior a instalación

SELECT repetido el 2026-10-27T18:55:22.3809393Z: **0 versiones de proveedor fuera del prefijo**, base doc110_instalacion_prueba existente y tres tablas de perfil conservadas en RA actual. Esta consulta no modificó datos.

Se consultó el inventario de aplicaciones IIS en solo lectura. No se identificó una aplicación destinada a la prueba aislada de DOC-110; la existencia de otras aplicaciones no autoriza elegirlas, alterar su configuración o usar sus datos. No se creó, detuvo ni reconfiguró ningún sitio/app pool. Tampoco se supone que la base mínima de instalación sea un sistema legacy operativo.

Para 7.5 se requiere identificar un entorno autorizado que permita login, preparación y almacenamiento individual de un documento descartable, usando la versión DOC-110 pero sin las tablas del perfil en su propio origen. Deben conocerse URL, cuentas autorizadas y destino descartable; las credenciales se introducen solo por consola. No basta con una base vacía ni con simular un 503 en el cliente.

Para 15.19 se necesita un proveedor previamente persistido fuera del prefijo en un ambiente de prueba autorizado, para validar el rechazo antes de consultar sus datos y conservar su documento. La inspección actual no encontró ninguno. Un fixture legado sintético solo podría prepararse como tal en un entorno aislado expresamente autorizado; no se insertará una configuración insegura en RA actual ni se confundirá una creación nueva con evidencia histórica.

Estos son requisitos de entorno/fixture, no fallos nuevos detectados en la implementación. No se amplía la autorización anterior (limitada a crear doc110_instalacion_prueba) para clonar bases, cambiar DSN, módulos o sitios, copiar datos personales, retirar tablas o añadir proveedores. Hasta disponer del entorno o una decisión explícita de alcance del usuario, ambas tareas continúan abiertas, sin PASS atribuido.

## Inspección actual de solo lectura

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/e2e/scripts/inspect-doc110-final-prerequisites.ps1
```

Reporte saneado doc110-final-prerequisites.json: INSPECTED, 2026-10-27T16:48:41.9810679Z. Versiones de proveedor guardadas con alias RA/DA, tabla de nombre válido y fuera del prefijo literal imp_02: **0**. La base propuesta doc110_instalacion_prueba **no existe**. RA actual conserva las tres tablas del perfil. No se abrieron conexiones de negocio de proveedores ni se modificaron registros/configuración.

## 10.16: instalación nueva

La [migración local ya aplicada](doc110-correccion-incrementales.md) tiene POST_CUTOVER_READ_ONLY PASS sobre 5 perfiles, 6 versiones y 13 reglas, con publicaciones/JSON/huellas preservados. Las inserciones nuevas y las carreras incrementales se acreditaron después. Falta aplicar y verificar SQL 01 en un destino nuevo; no se vuelve a ejecutar sobre RA actual.

Paquete concreto preparado: tools/e2e/artifacts/doc110-instalacion-nueva-preparada.sql. Crea únicamente doc110_instalacion_prueba, una marca de fixture y una tabla mínima de prerrequisito para probar el ALTER; después incluye el SQL 01 original sin cambios. No contiene usuarios/datos reales ni una copia completa de las tablas legacy. No conecta el sitio actual a esa base, no cambia gestor_modulos ni contiene DROP, limpieza o repetición automática. Si CREATE DATABASE encuentra una base existente, debe detenerse e inspeccionar, sin continuar ni borrarla.

SHA-256 del SQL 01 incluido: 002e34b873b90d0b92d822aba0385e645fbdeaba645c94633eabf4394d7c0461. SHA-256 del paquete: 1b46bef87631ab775a5b1be846e71fd41ba9ca2d12a695fa415319d0e42a17a8. El generador offline .tmp/doc110-prepare-isolated-install.cjs solo escribió archivos locales; no abrió BD ni ejecutó DDL.

El paquete está disponible para aplicación manual o autorización explícita de ejecución en esa base nueva. Después requiere verificación del esquema mediante el repositorio productivo, permisos por defecto, inserciones incrementales y controles SELECT, identificando qué parte es repositorio/BD y qué parte corresponde a corridas UI previas. Estado: PREPARED_NOT_EXECUTED.

## 7.5: aplicación sin esquema de perfiles

Necesita un sitio/base aislados con las tablas legacy que sostienen login, Producción y carga individual, pero sin las tres tablas nuevas del perfil. El fixture mínimo anterior no contiene esas tablas legacy, por lo que no acredita este caso. No se retirarán tablas de RA actual para simularlo ni se copiarán datos personales sin necesidad y alcance definidos.

La corrida debe comprobar indisponibilidad de la capacidad nueva y una carga individual descartable operativa en ese mismo contexto. Las defensas locales y la carga exitosa con esquema presente no sustituyen esa combinación. Falta identificar/preparar ese entorno; sin cambios en el sitio actual.

## 15.19: proveedor previamente guardado fuera del prefijo

Los listados/rechazos de metadatos ya tienen evidencia real. El escenario adicional requiere un proveedor previamente guardado fuera del prefijo, que la consulta actual no encuentra. No se habilitarán excepciones ni se registrará un proveedor inseguro en el ambiente actual para fabricar PASS. Se requiere un fixture histórico de prueba controlado o una decisión explícita del usuario sobre la aplicabilidad del caso, sin atribuir ejecución inexistente.
