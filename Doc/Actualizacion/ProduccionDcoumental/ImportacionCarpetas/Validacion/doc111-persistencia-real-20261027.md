# DOC-111: integración de persistencia en MySQL 5.1

**Resultado: PASS de los casos ejecutados**, 2026-10-27. El usuario autorizó la prueba acotada y posteriormente la activación temporal del perfil E2E 24, versión 38, con restauración a inactivo. No se amplió el alcance a carga de archivos, creación de expedientes/documentos, permisos o edición de reglas.

## Ejecución y controles reales

Se reutilizó el arnés Framework existente a través de `compileBehaviorHarness`, exportado por `tests/doc110-perfil-importacion-behavior.test.cjs`. El script `tools/e2e/scripts/run-doc111-persistence.ps1` resuelve la conexión configurada en memoria, exige ambas autorizaciones, verifica ambiente y fixture exactos y controla la restauración en finally. No guarda ni muestra credenciales o cadenas de conexión.

Antes de escribir comprobó perfil 24 inactivo/revisión 5, versión publicada 38 con huella coherente y cinco reglas sin proveedor; identidad activa 141 de la empresa 2 y facultad administrativa existente para cambiar disponibilidad. El destino existente seleccionado fue nivel 7; se comprobó pertenencia/permisos de agregar expediente y se construyó la ruta y sello a partir de su cadena real de ancestros. IMPORTAR_CARPETAS no se concedió: el contexto operativo es una fixture explícita para probar el repositorio directamente, no una sesión autenticada ni aceptación de la vía HTTP.

Los nueve controles principales pasaron:

- Guardado y lectura del plan con documentos y huella conservados.
- Dos conexiones independientes confirmando simultáneamente el mismo plan: un único lote.
- Repetición con la misma clave: mismo lote, inventario y evento.
- Otra clave para el mismo plan: mismo lote y clave original conservada.
- Repetición del lote ya confirmado usando reloj posterior al TTL: mismo lote sin nueva confirmación.
- Misma clave para otro plan: conflicto sin lote nuevo.
- Plan no confirmado expirado: conflicto y plan sin modificar.
- Conteos originales, destino y revisiones persistidos.
- Cero intentos de recepción o efectos documentales.

Una verificación posterior, exclusivamente de lectura, comprobó que un contexto negativo con otro usuario o nivel no recibe plan/lote; un sello de destino distinto produce conflicto. Se usaron dobles de contexto para estos casos negativos, siempre en la misma empresa. No acredita login, permisos del transporte ni controles de recursos del bloque 5.

## Registros conservados y restauración

| Entidad | Resultado |
|---|---|
| Plan 1 | CONFIRMADO, revisión 2 |
| Plan 2 | REVISABLE, revisión 1; rechazos de confirmación no lo modificaron |
| Lote 1 | Un único lote, revisión 1, nivel 7, dos raíces, dos archivos y 14 bytes previstos |
| Raíces 1 y 2 | Correlaciones r1/r2, PENDIENTE, revisión 1 |
| Archivos 1 y 2 | Uno por raíz, 7 bytes previstos cada uno, bytes recibidos NULL, recepción PENDIENTE y efecto NO_INICIADO |
| Evento | Un único LOTE_CONFIRMADO |
| Intentos | Cero |

Se conservaron exactamente dos planes, un lote, dos raíces, dos archivos de metadatos y un evento. No se borraron datos ni se reejecutó la prueba para crear más fixtures. Los archivos son únicamente inventario; no se subió contenido.

El perfil 24 se activó en revisión 6 y quedó **inactivo en revisión 7**. La revisión y los campos de actualización reflejan esas dos operaciones autorizadas; no se revierten artificialmente. Las huellas antes/después coinciden para el contenido restante de la cabecera, todas sus versiones y reglas, los demás perfiles y los permisos de la cuenta. La versión publicada continúa siendo 38.

## Evidencia y límites

Evidencia saneada en el repositorio local:

- `tools/e2e/artifacts/doc111-persistence-execution-result.json`: corrida original, IDs, conteos, huellas antes/después y SHA de las fuentes ejecutadas.
- `tools/e2e/artifacts/doc111-persistence-result.json`: resultado consolidado y enlaces a verificaciones.
- `tools/e2e/artifacts/doc111-persistence-read-result.json`: SELECT posteriores, filtros de lectura, estados/revisiones y perfil inactivo.

El modo Create del script bloquea una ejecución nueva si las tablas operativas ya contienen registros; no volver a ejecutarlo sobre estos datos como si fuera una instalación vacía. El modo Rollback posterior reutiliza exclusivamente la fixture y revisiones esperadas y conserva las filas; véase [verificación posterior](doc111-rollback-autoridad-20261027.md). Su bloqueo de ejecución evita dos consolas simultáneas del mismo script. El fallo inicial de compilación del comando Node ocurrió antes de resolver la conexión y no tuvo efectos; se corrigió usando un archivo de compilación que reutiliza el arnés existente.

Tareas cerradas con esta corrida inicial: **4.3, 4.4, 4.7 y 4.8**, progreso de ese corte **28/130**. Posteriormente 4.5/4.6 se verificaron con fallos controlados y 5.1/5.2 con casos de autoridad, alcanzando 32/130, según la evidencia posterior enlazada. Esta corrida inicial, por sí sola, no prueba fallos intermedios. Transporte incierto, recepción, ejecución documental, UI y REC-01/02 siguen sin acreditarse.
