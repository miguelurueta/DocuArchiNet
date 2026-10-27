# PRODUCCION-IMPORTACION-CARPETA-PERFIL

- Ticket: DOC-110
- Cambio OpenSpec: doc-110-produccion-importacion-carpeta-perfil
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Producción Documental permite preparar perfiles de importación sin ejecutar todavía el procesamiento de carpetas. DOC-110 incorpora administración de borradores, publicación de versiones inmutables, historial, selección administrativa y reglas configurables. Añade proveedores versionados y autorizaciones persistentes de fuentes RA/DA, con facultades separadas y límite de tablas controlado por servidor.

La interfaz se integra en `Gestion/WebFormProducionDocumental.aspx`; sus clientes están en `js/gestion/ImportacionCarpetas/`. La fachada propia `webservice/WebServiceImportacionCarpetas.asmx` delega en servicios y repositorios bajo `ProduccionDocumental/ImportacionCarpetas`. La identidad, empresa, permisos y conexiones proceden del servidor. SQL y contratos están en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`.

## Alcance y compatibilidad

El alcance comprende configuración y ensayo administrativo, no lectura de carpetas, creación de expedientes, planes, lotes o importación SII. La selección conserva el nivel documental vigente. Los catálogos existentes se consultan en sus orígenes autorizados, sin modificar equivalencias o destinos compartidos.

La carga individual conserva su flujo oficial DOC-88. Su regresión real con PDF sintético y la disponibilidad sin tablas de perfil cuentan con evidencia separada. No se afirma ejecución de una importación SII real. Los ensayos del perfil no producen efectos documentales; la consulta autorizada de un proveedor es de solo lectura.

La reversa retira la capacidad nueva desde una versión compatible y conserva tablas, publicaciones e historial. El esquema se instala/migra mediante SQL manual revisado, sin migración automática al iniciar y sin borrar tablas como mecanismo de rollback. Permisos ausentes o esquema incompatible deshabilitan la capacidad nueva; no conceden acceso implícito.

Estado: 178/178 tareas y cambio archivado el 2026-10-27. [Revisión y límites de aceptación](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-verificacion-final-20261027.md). Empresa única, cierre expreso de 7.6, aplicación externa fuera del proyecto y fixture legado sintético se conservan como decisiones explícitas, sin convertir escenarios no ejecutados en PASS.
