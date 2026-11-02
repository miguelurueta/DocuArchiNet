<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12 -->
# DOC-112 — Correcciones documentales aplicadas

El plan anterior de 52 comprobaciones se sustituye por 12 correcciones con una diferencia concreta y un entregable cada una. El contraste está en [gap-analysis.md](gap-analysis.md). No había tareas completadas que preservar. Las capacidades de DOC-110/111 se reutilizan; no se marcan como trabajo nuevo ni como tareas DOC-112 ejecutadas.

Estado: doce correcciones documentales aplicadas y comprobadas; resultado aprobado editorialmente por el usuario el 2026-11-02. Cada corrección conserva lo que ya coincide; una diferencia funcional no demostrada se registra antes de proponer desarrollo. Solo documentación: sin código, SQL aplicado, configuración, despliegue, E2E o cambios al almacenamiento legacy. S/M indican complejidad relativa. Evidencia: [acta de revisión](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc112-contratos-revision.md).

## 1. Diferencias comprobadas

- [x] 1.1 [S] Corregir el inventario que todavía clasifica rutas implementadas como nuevas propuestas. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/design.md`. Origen: D-01, RQ-01. Verificación: Las filas existentes en el repositorio dejan de figurar como creaciones pendientes; los estados de prueba remiten a la matriz final DOC-111 sin afirmar despliegue universal.

- [x] 1.2 [M] Corregir exclusivamente las diferencias del diccionario frente a SQL 02 y migración 03. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/01-modelo-datos.md`. Origen: D-02, RQ-02. Verificación: Una tabla de diferencias enumera columna o índice afectado, definición previa, definición versionada y fuente; las definiciones coincidentes se conservan y el corte de tablas vacías del 27 de octubre queda histórico.

- [x] 1.3 [S] Alinear el ejemplo de confirmación y repetición con los nombres exactos del DTO vigente. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md — sección 5`. Origen: D-03, RQ-03. Verificación: El ejemplo usa planId, revisionEsperada, planHashEsperado y claveConfirmacion; referencia la canonicalización existente y diferencia repetición idéntica de conflicto sin reconstruir el algoritmo.

- [x] 1.4 [M] Sustituir el sobre y la descripción de transporte propuestos por el contrato público existente. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md — sección 1`. Origen: D-04, RQ-04. Verificación: El sobre sintético y su matriz de errores se cotejan con RespuestaPerfilImportacion y los guards; se retiran campos o promesas HTTP no respaldados y se referencia la autoridad ya implementada sin rehacerla.

- [x] 1.5 [S] Unificar las referencias vigentes de snapshot y valores diferidos sin repetir su diseño. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md — sección 4`. Origen: D-05, RQ-05. Verificación: Una referencia vigente enlaza el resolvedor, PerfilImportacionJson y el checkpoint de valores; distingue configuración publicada, PREFLIGHT/TRAS_EXPEDIENTE y valores vacíos ya fijados, conservando las reglas de 05 y 06.

- [x] 1.6 [M] Reemplazar el catálogo conceptual de operaciones por sus equivalencias ASMX y ASHX existentes. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md — sección 2`. Origen: D-06, RQ-06. Verificación: Las doce operaciones públicas y el handler tienen correspondencia con su DTO y responsabilidad; se corrigen Listar/ObtenerPerfil, ListarLotes, ConsultarArchivos/Raices y ExportarResultados, y se incluyen consulta de confirmación y cierre sin inicio sin inventar endpoints.

- [x] 1.7 [M] Resolver las afirmaciones históricas contradictorias sobre coordinación y completitud. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/03-estados-recuperacion.md — sección 2`. Origen: D-07, RQ-07. Verificación: La tabla cerrada existente se conserva; la frase que declara coordinación/agregación/reconciliación no implementadas se identifica como corte histórico y remite al coordinador y a la evidencia posterior.

- [x] 1.8 [S] Vincular las barreras existentes con sus tres aceptaciones y límites de evidencia. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/03-estados-recuperacion.md — sección 3`. Origen: D-08, RQ-08. Verificación: Cada rama EXPEDIENTE, LOTE y PARCIAL referencia su reporte aprobado; se conservan CAS/reclamaciones existentes y se distingue la barrera probada de atomicidad externa o selección manual no acreditadas.

- [x] 1.9 [S] Actualizar la condición de aceptación de adaptadores ya caracterizados. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/03-estados-recuperacion.md — sección 8`. Origen: D-09, RQ-09. Verificación: La condición que presenta el almacenamiento como candidato no aprobado queda histórica; el estado vigente referencia copia consumible, original retenido, nombre/índices y Archivo Gestión comprobados, preservando incertidumbre y exclusión XML acordada.

- [x] 1.10 [M] Corregir el alcance de reconciliación pública y enlazar la recuperación realmente ejercida. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md — reconciliación`. Origen: D-10, RQ-10. Verificación: La ficha usa loteId/intentoId/revisión según DTO y declara verificación sin nueva creación/almacenamiento; remite a cierre de reserva 5 y reconciliación de intento 6 del lote 2, manteniendo INCIERTO cuando no hay evidencia suficiente.

- [x] 1.11 [S] Reclasificar los pendientes históricos frente a los límites operativos que siguen abiertos. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/04-decisiones-y-pendientes.md`. Origen: D-11, RQ-11. Verificación: P02/P03/P06/P07/P08/P09 distinguen solución/evidencia de pruebas y restricciones residuales; P04/P05 conservan responsable por rol y efecto real sobre capacidad anunciada o borrado. No se reabre la decisión de ámbitos ni se habilita mantenimiento.

- [x] 1.12 [M] Registrar la revisión de las diferencias documentales y su trazabilidad final. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc112-contratos-revision.md`. Origen: D-12, RQ-12. Verificación: El acta coteja las doce correcciones con sus fuentes y enlaces, conserva el alcance real/local/no ejecutado de DOC-111 y registra OpenSpec estricto; no repite REC ni convierte tareas históricas en trabajo DOC-112 completado.

## Orden y reutilización

Corregir el inventario (1.1) y el estado físico documentado (1.2). Las correcciones de contrato 1.3–1.6 y 1.10 usan DTO/guards vigentes; 1.7–1.9 conservan estados y barreras implementados. Reclasificar pendientes (1.11) a partir de esas fuentes y revisar únicamente el delta documental (1.12).

La matriz de brechas conserva la correspondencia de las 52 tareas anteriores. Ya no se exige reescribir nueve diccionarios, redactar cada operación desde cero ni repetir los recorridos REC de DOC-111. Las diferencias adicionales que aparezcan en el cotejo de columnas/payloads se registran por elemento con su fuente; no se presume que todo el documento esté incompleto.
