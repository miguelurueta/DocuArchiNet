# DOC-111: esquema aplicado por el usuario y verificado

El usuario informó la ejecución manual de `Sql/02-carga-carpetas-mysql51.sql`. Se verificó el ambiente configurado el 2026-10-27 mediante SELECT. El agente no aplicó DDL, concedió permisos ni realizó escrituras de prueba.

## Resultado real de solo lectura

**PASS de estructura**, usando `MySqlImportacionCarpetasRepository.VerificarEsquema()` compilado con el proyecto existente y el conector configurado. Comprueba las seis tablas operativas y las dependencias de versión/permiso: motor, collations, todas las columnas consumidas, tipos, tamaños, nulabilidad, valores predeterminados, identidades incrementales, PK, índices requeridos y diez FK con pertenencia al esquema y reglas RESTRICT. No aplica migraciones ni conserva una aceptación global entre verificaciones.

La captura tiene ocho tablas incluyendo dependencias, 89 columnas y 48 filas de metadatos de índices; estas últimas representan columnas de índices, no 48 índices distintos. Las seis tablas operativas están vacías y el conteo de registros con IMPORTAR_CARPETAS distinto de cero o nulo es cero.

Evidencia saneada: `tools/e2e/artifacts/doc111-installed-schema.json` y `tests/Fixtures/ProduccionDocumental/doc111-esquema-instalado-mysql51.json`. La fixture contiene únicamente estructura obtenida de information_schema: no incluye cadenas de conexión, credenciales, datos documentales ni identidades de usuarios.

## Implementación y pruebas locales

Se incorporó la base del único repositorio operativo en `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb` y se registró en el proyecto. Su capacidad actual es verificar el esquema; todavía no implementa el puerto operativo completo ni se conecta a endpoints de importación. Los errores de conexión se devuelven como CAPACIDAD_NO_DISPONIBLE sin conservar la excepción original con datos sensibles.

El arnés Framework existente usa una copia de los metadatos reales y prueba once defectos: tabla ausente, motor distinto, reducción de capacidad de bytes, signo incorrecto de ID, permiso concedido por defecto, collation incompatible, pérdida de unicidad, índice con prefijo, FK a otro esquema, cascada y FK ausente. Los defectos se introducen en memoria, nunca en la base. También comprueba que un error de conexión no exponga detalles internos.

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS con familias DOC-110, contratos/límites DOC-111 y guardas de esquema DOC-111.
- Build Debug con MSBuild del proyecto, DeployOnBuild=false y salida `.tmp/doc111-build`: PASS; sin reemplazar binarios de la aplicación activa.

Tarea 4.1 completada. Progreso: **23/130**. Continúan documentos/huellas, inserción/lectura del plan, evento transaccional y confirmación idempotente del lote (4.2–4.7). No se acredita todavía persistencia operativa ni carreras transaccionales; estas requieren su propia evidencia y autorización para las escrituras reales.
