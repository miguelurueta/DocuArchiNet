# COR-01/COR-02 — Identidad incremental

Decisión aprobada: perfil_id, version_id y regla_id son INT UNSIGNED AUTO_INCREMENT, generados únicamente por MySQL. perfil_id de versión, version_id de regla y version_publicada_id de perfil usan INT UNSIGNED sin AUTO_INCREMENT; solo la última permite NULL. No usar MAX(id)+1, GUID ni secuencias por empresa. La numeración puede tener huecos y no define el orden de presentación.

El dominio VB, DTO y HTTP conservan String para estas referencias: decimal canónico entre "1" y "4294967295", sin signo, espacios ni ceros iniciales. Una referencia opcional ausente es Nothing/null. La API no cambia a JSON number; el editor conserva el valor recibido sin parseInt. Esta representación no altera el tipo numérico de las columnas. Las referencias de catálogos legacy mantienen su contrato. Conocer un ID no concede autorización.

Cada INSERT omite su PK y obtiene LastInsertedId en la misma conexión y transacción. Las reglas se insertan, obtienen su ID y serializan ese ID en regla_doc antes del commit. Nueva versión obtiene IDs nuevos; numero, revision y orden de regla siguen siendo conceptos independientes.

## Migración verificable

Las tres correspondencias GUID anterior → entero nuevo son independientes y persistidas en ra_integracion_mapa_perfil, ra_integracion_mapa_version y ra_integracion_mapa_regla. Las tablas GUID se conservan completas como respaldo. No se consideran descartables los perfiles de pruebas existentes.

regla_doc contiene ReglaId y participa en config_hash. La utilidad reemplaza exclusivamente esa propiedad, tras comprobar que coincide con la columna original; no hace sustituciones globales ni cambia valores extraídos del nombre de archivo. La representación se canonicaliza con el mismo contrato del codec de producción. Antes de transformar verifica la huella original de cada publicación; ante discrepancia se detiene. Después calcula y verifica la huella nueva y guarda ambas en ra_integracion_migracion_huella. Los borradores conservan hash NULL.

Es una migración de identidad explícita y auditada: la huella técnica cambia, las reglas documentales se conservan. Las publicaciones anteriores y sus hashes originales siguen disponibles en las tablas de respaldo. No se introduce edición de publicaciones en la API. SQL no interpreta JSON; el mecanismo funciona sin funciones JSON de MySQL moderno.

Los consumidores actuales son Repository, Service, resolvedor/codec, DTO/ASMX, editor y pruebas DOC-110. Los contratos futuros de plan/lote/reclamación deben usar las identidades incrementales, pero esta corrección no implementa esas tablas. SII no se modifica.

Inventario SELECT del 2026-10-20: 5 perfiles, 6 versiones, 13 reglas, 2 publicaciones; sin huérfanos ni FK externas detectadas. Las dos huellas originales se verificaron con el conversor. Evidencia en Validacion/doc110-inventario-migracion-identidades.json y doc110-huellas-antes-migracion.json. Esto no acredita ejecución de la migración ni descarta consumidores fuera del repositorio; durante la ventana deben detenerse todos los escritores.

Procedimiento obligatorio: [aplicación manual](../Sql/03-aplicacion-migracion-identidades.md).
