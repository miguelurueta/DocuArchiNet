# DOC-115 — Lotes y recepción recuperable

## Why

El prompt 07 requiere persistencia operativa (07-A) y confirmación/recepción (07-B). DOC-111 ya incorporó el esquema, repositorio, servicio, handler y staging. DOC-114 verificó preparación. DOC-115 debe contrastar garantías y completar aceptación específica sin reconstruir esas capacidades.

## What Changes

- Auditar y reutilizar el esquema y repositorio únicos; corregir solo incumplimientos demostrados por casos reproducibles.
- Caracterizar confirmación concurrente, autoridad multipart, cuota, integridad, exclusión y recuperación disco/BD.
- Entregar especificaciones RECIB-01/02/03 y pruebas de repositorio sobre infraestructura existente; distinguir evidencia local de integración real.
- Mantener mapa común, contratos y evidencia por fase sincronizados.

## Capabilities

### New Capabilities
- `produccion-importacion-carga-carpeta-lotes`: aceptación verificable de persistencia y recepción recuperable ya integradas.

### Modified Capabilities
Ninguna especificación principal se modifica durante este refinamiento. Las correcciones posteriores conservarán contratos de DOC-112 y preparación de DOC-114.

## Impact

WebForms/VB.NET .NET Framework 4.6.1, MySQL 5.1, ASMX/ASHX y herramientas Node/Playwright existentes. La entrega añade pruebas y verificación sobre componentes existentes, con 07-A y RECIB-01/02/03 ejecutados bajo autorización. No cambia VB, proyecto, DDL ni configuración de producto. Fuera de alcance: creación, almacenamiento definitivo, XML, SII, nuevas pantallas, fragmentación, trabajador desatendido y cambios globales de Web.config. El refinamiento fue aprobado; las tareas están completadas y la evidencia lista para revisión de entrega.
