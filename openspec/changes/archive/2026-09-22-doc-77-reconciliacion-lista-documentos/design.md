<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
# Diseño técnico — DOC-77

## Contexto

DOC-76 termina con una respuesta estructurada de ejecución. DOC-77 cierra el recorrido visual tomando el backend como fuente de verdad, reconciliando resultados inciertos y reflejando documentos confirmados en la lista ya existente de la tarea. Los módulos nuevos son aditivos y no modifican almacenamiento, ASMX ni funciones legacy.

## Decisiones

### D-01 — Reconciliación únicamente por la API moderna

`importar-servicio-web-reconciliation.js` recibe API, intención y contexto autorizado. El flujo normal consume el resultado final de ejecución; timeout, pérdida de respuesta o reapertura pueden consultar `GetImportIntent`, y un resultado incierto puede usar `ReconcileImportIntent`. Ambas operaciones atraviesan `importar-servicio-web-api.js`. El navegador no consulta SII, no persiste y no sondea periódicamente.

### D-02 — Proyección conservadora de estados

El reconciliador normaliza los estados contractuales sin interpretar códigos legacy. `Disponible` exige confirmación backend; timeout o ausencia queda `Verificando`; `ResultadoIncierto`, `Inconsistente`, `Completado`, `Parcial`, `Detenido` y `Fallido` se conservan como resultados explícitos. Un valor desconocido nunca se convierte en importado ni disponible.

### D-03 — Aislamiento estricto por tarea y autorización

Antes de tocar la lista, el adaptador compara numéricamente el `TaskId` confirmado con el identificador de la tarea actualmente visible. También exige `DocumentId` positivo y estado `Disponible`. Si cambia la tarea durante la operación, descarta la proyección visual. La acción “Ver documento importado” solo se ofrece cuando el identificador ha pasado estas validaciones.

### D-04 — Recorrido único y deduplicación por documento

Al finalizar, el coordinador recorre `response.Items` una sola vez, mantiene un conjunto local de `DocumentId` y produce un lote estable de documentos confirmados. No inserta durante la espera, no usa la posición visual como identidad y no repite un documento aunque varias identidades externas lo referencien.

### D-05 — Adaptador visual seguro con fallback autoritativo

`importar-servicio-web-document-list-adapter.js` encapsula toda interacción con la lista existente. Cuando el DTO permite construir de forma segura la entrada visual documentada, delega item por item al punto de extensión existente. Si faltan campos, solicita refrescar la lista completa mediante una función inyectada. Nunca analiza ni fabrica `dato_lista` y no modifica `insert_row_documento_relacionado(...)`.

### D-06 — Contexto visual estable y reapertura persistida

Antes de abrir un documento se captura filtro y scroll de la vista; al volver se restauran, se limpia la selección transitoria y el foco regresa a una acción conocida. El cierre elimina estado efímero del modal. La reapertura consulta el snapshot persistido autorizado, por lo que no depende de memoria local para afirmar importación.

## Flujo paso a paso

1. `ExecuteImportIntent` termina y entrega `Items`, o el usuario inicia una recuperación autorizada.
2. El reconciliador obtiene el snapshot mediante la API moderna cuando corresponde.
3. Se normalizan estados sin inferir éxito por timeout, ausencia o código desconocido.
4. Se valida que la tarea del snapshot y de cada item coincida con la tarea visible.
5. Se recorre una vez la colección y se deduplican documentos confirmados por `DocumentId`.
6. El adaptador actualiza la lista existente; si el contrato visual no es seguro, solicita refresco autoritativo completo.
7. La UI habilita “Ver documento importado” solo para identificadores autorizados y conserva el contexto al regresar.
8. Al cerrar y reabrir, el estado se reconstruye desde el backend.

## Riesgos y mitigaciones

- Cambio de tarea durante la respuesta: comparación estricta inmediatamente antes de actualizar.
- DTO visual incompleto: refresco autoritativo, nunca campos inventados.
- Documento repetido: conjunto por `DocumentId` y pruebas con fixture duplicado.
- Resultado incierto: reconciliación explícita y estado no optimista.
- Regresión legacy: archivos aditivos y pruebas que prohíben modificar/interpretar funciones y datos legacy.

## Validación y reversión

Se crearán las tres pruebas canónicas DOC-77, se ejecutará la suite focal y el build MSBuild. El flujo completo requiere E2E real autorizado para confirmar reconciliación, aislamiento por tarea, lista, filtros, scroll, foco y reapertura. La reversión elimina módulos y registros aditivos; no requiere migración de datos.
