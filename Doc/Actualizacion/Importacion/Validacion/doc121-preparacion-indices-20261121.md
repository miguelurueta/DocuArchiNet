# DOC-121 — preparación de índices específicos, 2026-11-21

Este documento conserva el corte de preparación previo al ALTER. Después el usuario autorizó «aplica por mi» y 08 quedó [aplicado y verificado en CONTRATOS](doc121-contratos-indice-aplicado-20261121.md). El JSON de preparación y sus huellas originales permanecen como antecedentes, no como estado actual de ejecución.

Continuación solicitada después de la [verificación base 01–03](doc121-verificacion-instalacion-20261121.md). El usuario mantiene la ejecución manual de cualquier SQL modificador. Se revisaron metadatos y conteos agregados mediante **11 SELECT**, cero DDL/DML/HTTP. [Resultado saneado y huellas](doc121-preparacion-indices-20261121.json).

## Resultado observado

| Tabla y clave | Filas | NULL | Vacías | Grupos duplicados | Filas excedentes en esos grupos | Dictamen |
|---|---:|---:|---:|---:|---:|---|
| imp_02_egresos / CONSECUTIVO | 25153 | 0 | 2 | 738 | 4479 | Bloqueada para la clave única prevista en esta base. |
| imp_02_contratos / NCONTRATO | 625 | 0 | 0 | 0 | 0 | Datos elegibles en el momento de lectura; ALTER preparado, sin aplicar. |

Los duplicados se compararon por GROUP BY con la collation real latin1_swedish_ci. Los conteos de duplicados excluyen NULL/vacíos, informados aparte. Se guardan agregados, no claves ni valores de negocio. Las lecturas son observaciones acotadas, no un snapshot transaccional ni garantía frente a escrituras posteriores.

Ambas tablas aparecen InnoDB; ambas claves son VARCHAR(250) latin1/latin1_swedish_ci, nullable, DEFAULT NULL, sin EXTRA o comentarios, y no tienen índices en esta base. Se conserva la huella del mismo ambiente físico local inspeccionado anteriormente; no acredita una resolución autenticada RA/DA por sesión de aplicación.

El catálogo local registra una fuente activa EGRESOS en conexión lógica DA, aislamiento CONEXION, sin columna empresarial; su proveedor publicado consulta CONSECUTIVO como TEXTO. Esto describe la configuración de lectura, no demuestra que la tabla de datos en esta base física sea la DA efectiva de la sesión del usuario. No se encontró fuente activa de CONTRATOS en ese catálogo: su ausencia no concede ni define aislamiento.

## SQL entregado

1. [07-preflight-claves-doc121.sql](../Sql/07-preflight-claves-doc121.sql): SELECT de ambiente, atributos, índices, conteos e aislamiento registrado; el último SELECT corresponde a RA si es distinta de DA.
2. [08-indice-contratos-ncontrato-doc121.sql](../Sql/08-indice-contratos-ncontrato-doc121.sql): un único ALTER de NCONTRATO a NOT NULL con UNIQUE completo, conservando VARCHAR(250) y su charset/collation. No toca EGRESOS, BENEFICIARIO, plantillas o fuentes. Requiere confirmar DA, identidad única global, atributos actuales y cero incidencias antes de su ejecución manual.
3. [09-verificacion-claves-doc121.sql](../Sql/09-verificacion-claves-doc121.sql): comprobación posterior exclusivamente SELECT. No repetir el ALTER si ya quedó aplicado.

Se revisaron los tres archivos con **12 controles estáticos aprobados**: solo SELECT en 07/09; una sola sentencia ALTER limitada a CONTRATOS en 08; tipo/collation preservados; UNIQUE de columna completa; ausencia de limpieza DML, ejecución dinámica o ALTER de EGRESOS. Es revisión estática, no ejecución de la sintaxis en MySQL. Los siete archivos originales 00–06 se conservan sin cambios.

## Bloqueos y decisiones que siguen abiertas

**EGRESOS:** no se entrega ALTER ejecutable para su clave mientras existan las incidencias observadas. No se eliminan duplicados, rellenan vacíos, normalizan valores ni redefine la clave automáticamente. Se solicitó al usuario confirmar si esa es la DA que desea adecuar o si corresponde revisar otra fuente. Si es el destino correcto, la resolución de datos o de identidad exige revisión funcional y otro archivo SQL explícito para ejecución manual; no elegir arbitrariamente una fila ganadora.

**Cuenta dedicada, paso 05:** se solicitó nombre de cuenta MySQL, host autorizado y base DA, sin contraseña. No se deduce el destinatario del GRANT desde la cuenta usada para inspeccionar el esquema. No se ejecutaron concesiones ni se acredita una cuenta dedicada por entregar su plantilla.

**CONTRATOS:** la elegibilidad de datos se observa en la base inspeccionada; el usuario confirma destino e identidad antes de aplicar 08. Si el destino es compartido por columna empresarial, este archivo simple no corresponde y se genera otro compuesto. La tabla no queda autorizada como fuente por agregar un índice.

Los controles estáticos de la entrega base y OpenSpec se revisan por separado de estos conteos. No hay importación Excel, prueba E2E, rollback o concurrencia de escritor acreditados. Esta continuación mantiene los límites elegidos de los prompts y no modifica producto, configuración efectiva, datos o permisos desde el agente.
