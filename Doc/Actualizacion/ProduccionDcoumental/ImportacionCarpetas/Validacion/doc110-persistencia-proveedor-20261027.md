# DOC-110: persistencia y lectura de versiones de proveedor

Fecha: 2026-10-27. Tarea 14.6, D-06/RQ-06 y AMP-01. Conciliación de implementación y aceptación existente, con nuevas pruebas locales de las entradas de repositorio. Sin nueva sesión autenticada ni cambios productivos.

## Lectura autorizada

`MySqlProveedorIndicesRepository` valida contexto servidor y referencias decimales canónicas antes de construir una conexión. Listar exige que proveedor/versión estén ambos presentes o ambos ausentes. El SELECT parametriza ámbito, proveedor, versión, límite y desplazamiento; filtra proveedor activo y versión PUBLICADA. Obtener une cabecera/versión por proveedor y exige simultáneamente el proveedor, versión y ámbito solicitados, activo y PUBLICADA. No selecciona la última versión en sustitución de la referenciada. Ausencia de fila produce ACCESO_DENEGADO y no expone si el ID existe en otro ámbito. Configuración incompatible o múltiples filas producen rechazo.

La lectura administrativa tiene un propósito distinto: muestra borradores y proveedores inactivos, bajo permiso adicional ADMINISTRAR_PROVEEDORES_IMPORTACION. Sus SELECT también filtran el ámbito del contexto y la pareja proveedor/versión; el cliente no proporciona ese ámbito.

Estas condiciones SQL se contrastaron por inspección del repositorio productivo. No se presenta esa inspección como una consulta real entre empresas.

## Persistencia e inmutabilidad

`MySqlAdministracionProveedorRepository.Mutar` abre una transacción; bloquea primero cabecera del ámbito y después versión. Compara ambas revisiones. GUARDAR y PUBLICAR requieren BORRADOR. VERSION inserta una fila nueva copiando el contrato; no actualiza la publicación anterior. DISPONIBILIDAD modifica la cabecera, conservando las filas versionadas. Devuelve el documento leído dentro de la transacción y confirma con Commit; un error sale del Using sin confirmar.

La [aceptación del ciclo completo](doc110-fuentes-aceptacion-preparada-20261026.md), aportada por el usuario, registra **1 passed (23.8 s)**, caso 22.8 s. Sus comprobaciones incluyen:

- Alta/configuración/publicación desde UI y rechazo HTTP de edición publicada.
- Segunda versión con ID nuevo y número siguiente; lectura puntual de la primera exige estado, revisión y contrato originales.
- Selección de la versión publicada desde el perfil y resolución de índices reales; una tercera versión BORRADOR activa queda excluida del catálogo consumidor.
- Dos publicaciones completas del perfil y su historial conservados al desactivar el proveedor; catálogo consumidor filtrado vacío.
- Revocación de la fuente sin reescribir publicaciones, referencias ni historial.

La comprobación de documentos completos e historial se realiza mediante lecturas ASMX, no mediante controles SQL nuevos. El [diagnóstico posterior](doc110-fuente-referencias-guardadas-20261027.md) también conserva las referencias originales de una publicación con fuente revocada.

## Nuevas pruebas locales

`ProveedorIndicesBehaviorTests` instancia ambos repositorios MySQL productivos con una cadena centinela deliberadamente inválida, sin servidor ni credenciales. Si se intentara construir la conexión antes del rechazo, la prueba fallaría. Ejecuta 51 rechazos de contexto ausente/sin facultad/no canónico, proveedor/versión inválidos, filtro incompleto, paginación inválida y permiso administrativo adicional ausente. No sustituye el aislamiento real entre empresas.

```powershell
node --test tests/doc110-perfil-importacion-behavior.test.cjs tools/e2e/tests/doc110-provider-history.test.cjs
```

Resultado: **3 pruebas PASS**. Una compila y ejecuta el arnés .NET Framework; dos ejercitan el verificador de conservación de consumidores con dobles, incluida detección de pérdida de referencia, alteración de historial y proveedor inactivo seleccionable. No son nuevas corridas HTTP.

Se completa 14.6 con consultas revisadas, defensas locales y persistencia/versionado positivos acreditados por la corrida autenticada existente. Siguen pendientes sus tareas específicas de aislamiento empresarial real, migración con controles SELECT, fallos transaccionales y demás criterios abiertos. No se ejecutó DDL ni SQL de control nuevo, no se alteró SII y no se accedió nuevamente a DA.
