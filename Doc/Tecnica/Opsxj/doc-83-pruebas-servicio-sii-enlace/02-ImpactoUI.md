# DOC-83 — Impacto UI

- Ticket: DOC-83
- Cambio OpenSpec: doc-83-pruebas-servicio-sii-enlace
- Clasificacion: cross_cutting

## Superficies UI

El alcance comenzó como validación, pero las corridas reales revelaron defectos reproducibles y el refinamiento aprobado modificó JavaScript productivo y la proyección de respuesta necesaria para actualizar la lista ENLASE sin recarga. La regresión valida estas superficies:

- modal ENLASE con semántica, foco y anuncios accesibles;
- selección individual y total de anexos importables;
- preparación múltiple y tipología predeterminada inequívoca;
- preview mediado sin URL externa;
- scroll horizontal/vertical dentro del viewport;
- cierre solo ante éxito total confirmado;
- error, parcial o incertidumbre visibles;
- asignación como acción separada y explícita.
- preparación masiva con scroll exclusivo en documentos y acciones persistentes;
- preview de página completa con retorno visible a la lista.

## Validacion visual

Además de reutilizar la evidencia DOC-82, DOC-83 ejecutó el escenario gobernado `import-sii-enlase-manual-visual`. La corrida autorizada confirmó que la fila aparece inmediatamente sin recargar y que el documento recién proyectado abre sin error. El runner restauró el gate al finalizar.

La revisión final detectó texto superpuesto en valores extensos de la tabla SII. La corrección asigna anchos semánticos por columna, recorta visualmente con elipsis, conserva el valor completo en `title` y mantiene opaca la columna fija de acciones. El scroll permanece dentro de la tabla tanto en escritorio como en móvil.

Una revisión posterior detectó que una preparación extensa desplazaba `Cancelar` y `Crear intención` fuera del área visible, y que la composición de escritorio ocultaba el retorno del preview mientras forzaba nuevamente la lista. La corrección mantiene el preflight, retira únicamente su lista visual redundante, confina el scroll a los documentos preparados, fija las acciones dentro del panel y presenta el preview como una vista completa con `Volver a documentos` siempre visible. El iframe usa el alto restante sin desalinear verticalmente el modal.
