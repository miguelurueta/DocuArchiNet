# DOC-111: persistencia de plan y confirmación de lote

Estado inicial del corte: implementación compilada y con comprobaciones locales, antes de autorizar escrituras. Actualización posterior: el usuario autorizó la prueba y la activación temporal del perfil 24; la integración real ejecutada pasó y el perfil quedó inactivo. [Evidencia real y alcance preciso](doc111-persistencia-real-20261027.md). El fallo inducido a mitad de transacción continúa pendiente.

## Comportamiento implementado

El único repositorio `MySqlImportacionCarpetasRepository.vb` incorpora GuardarPlan, ObtenerPlan, ConfirmarPlan y ObtenerLote. Conserva la verificación de esquema antes de operar, sin migraciones. Aún no implementa las capacidades restantes del puerto completo ni está conectado a endpoints operativos.

GuardarPlan crea una identidad nueva con LastInsertedId de su propio comando/conexión; no actualiza una preparación anterior. Guarda contexto, snapshot de publicación, manifiesto, evaluación, huella y fechas UTC con precisión de segundos. La publicación se contrasta contra la versión PUBLICADA, perfil activo y ámbito, número/configuración/huella de DOC-110. No simula facultad administrativa.

El plan se clona y canoniza antes de guardar. Los documentos usan el codec cerrado existente, Unicode escapado y sus límites, sin relajar el contrato administrativo. La huella incluye destino/sello, snapshot, inventario, evaluación, esquema y fechas; excluye identidad incremental, estado y revisión, que cambian al persistir/confirmar. LeerPlan comprueba nuevamente la huella y las referencias de las columnas; un documento alterado o de forma inválida no se devuelve. Las consultas filtran por solicitante, empresa y nivel y contrastan el sello/ruta fijados.

ConfirmarPlan bloquea la fila del plan, comprueba revisión/huella y publicación y crea lote, todas las raíces, todos los archivos y evento dentro de la misma transacción. Una excepción antes del commit deja la transacción sin confirmar y se desecha con su conexión. Los rechazos se conservan en el inventario y los totales originales incluyen todos los archivos. Un fallo no se reintenta automáticamente. No se crean expedientes, documentos ni intentos.

La repetición compatible retorna el lote existente antes de comprobar el TTL de una preparación nueva. El mismo plan con otra clave también converge y conserva la clave original; la clave alternativa no se registra como una segunda confirmación. La misma clave para otro plan o contenido/revisión distintos produce conflicto. La confirmación nueva exige elegibilidad coherente con completo/EXPEDIENTE, completo/LOTE o parcial/NO_APLICA. No inicia efectos de recepción ni documentales.

## Evidencia local

`node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS. El arnés Framework existente ejecuta DOC-110 y los casos DOC-111. Los casos nuevos comprueban:

- Round-trip de los documentos mediante IDataRecord en memoria, valores diferidos y snapshot independiente del objeto original.
- Huella estable al reordenar raíces/archivos y después de cambiar estado/revisión de confirmación; rechazo de alteración de manifiesto, fecha, nivel y propiedades JSON.
- Conservación de UINT64 máximo en documentos y parámetros reales del conector MySql.Data; huellas enviadas como bytes binarios.
- Rechazo de cantidades no canónicas, suma desbordada, traversal, identidad duplicada, elemento ajeno, snapshot discordante, fecha anterior al rango DATETIME y evaluación global contradictoria.
- Coherencia de elegibilidad por LOTE/EXPEDIENTE, denegación antes de abrir conexión e incompatibilidad de identidad/huella en la resolución de repetición.

Compilación Debug del proyecto con MSBuild, DeployOnBuild=false y salida en `.tmp/doc111-build`: PASS. No se desplegó ni reemplazó el binario activo. Estas pruebas locales no ejecutan las sentencias DML ni acreditan concurrencia o rollback en MySQL.

## Comprobación previa y autorización de la prueba real

Actualización posterior: el usuario respondió «autorizado» para la prueba descrita de hasta dos planes, un lote, dos raíces, dos archivos de metadatos y un evento. Esa autorización permanece vigente; no se solicita de nuevo para ese alcance. La comprobación SELECT previa encontró **cero perfiles activos con versión PUBLICADA**, por lo que no se invocó RunPersistenceIntegration ni se hicieron escrituras. Las seis tablas operativas continúan con cero filas. Resultado: NO_EJECUTADO por falta de fixture activo, no un fallo de confirmación.

Como candidato se identificó el perfil E2E 24, versión publicada 38 (número 2), entonces inactivo/revisión 5, con cinco reglas y ninguna de tipo PROVEEDOR. Tras solicitar el alcance adicional, el usuario autorizó activarlo temporalmente y restaurarlo. La prueba pasó y el perfil quedó inactivo/revisión 7; las reglas, versiones, demás perfiles y permisos conservaron sus huellas. La comprobación inicial de ausencia de fixture activo es histórica: `tools/e2e/artifacts/doc111-persistence-preflight.json`. El resultado actual está en `doc111-persistencia-real-20261027.md`.

`ImportacionCarpetasBehaviorTests.RunPersistenceIntegration` está compilada en el arnés existente y no se llama desde Run() ni desde la prueba Node local. Requiere autorización explícita y contexto/snapshot de fixture existentes aportados por el ejecutor; no concede permisos ni crea configuración.

Al ejecutarse, crea hasta **dos planes, un lote, dos raíces, dos archivos de metadatos y un evento** exclusivamente en las tablas operativas nuevas. Comprueba dos confirmaciones simultáneas mediante conexiones independientes, repetición con la misma y otra clave, repetición de un lote confirmado tras vencer el TTL, rechazo de reutilización de clave para otro plan y rechazo de un plan no confirmado expirado. Los controles de conteos son SELECT sobre las identidades de la prueba. Se conserva la evidencia; no borra datos.

No sube archivos, no inicia intentos, no toca perfiles/proveedores/fuentes/permisos ni SII. La prueba a nivel de repositorio no acredita autenticación, permiso operativo o validación de recursos desde HTTP; esas capacidades continúan en bloques 5–6. Si falta un contexto/snapshot de fixture aprobado, se detiene sin sustituirlo ni crear uno silenciosamente.

El fallo inducido a mitad de confirmación/evento y la recuperación ante transporte siguen requiriendo evidencia específica; no se dan por probados por revisar el uso de la transacción. No se afirma que RunPersistenceIntegration cubra esos fallos.

## Tareas

4.2 se completó con evidencia local del codec y las huellas. Posteriormente se cerraron 4.3, 4.4, 4.7 y 4.8 mediante la integración real y controles de lectura. 4.5 y 4.6 siguen abiertas por faltar evidencia de fallo intermedio/rollback. Progreso actual: **28/130**.
