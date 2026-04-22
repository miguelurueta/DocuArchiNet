## 1. Diagnostico y endurecimiento del modelo actual

- [x] 1.1 Auditar `application/autoPagination.ts` para separar claramente la
  deteccion de overflow, la clasificacion estructural de bloques y la emision
  de acciones de layout
- [x] 1.2 Auditar `application/useAppEditor.ts` para identificar el punto
  exacto donde el flujo actual sigue siendo correctivo y produce correccion
  tardia visible
- [x] 1.3 Auditar `application/usePaginationMetrics.ts` para validar su acople
  con el flujo continuo y definir que parte debe seguir siendo solo visual

## 2. Modelo estructural de bloques

- [x] 2.1 Introducir clasificacion de nodos top-level en categorias utiles para
  layout (`text-divisible`, `list-structured`, `atomic-indivisible`,
  `manual-break`)
- [x] 2.2 Definir contrato interno para representar bloques medidos, altura,
  pagina objetivo y estrategia de corte
- [x] 2.3 Confirmar que listas, task lists e imagenes no dependan unicamente de
  `node.isTextblock` para decidir su tratamiento

## 3. Paginacion preventiva

- [x] 3.1 Reemplazar la estrategia basada en desborde visible por una estrategia
  preventiva que corte o mueva bloques antes de invadir el borde inferior
- [x] 3.2 Ajustar el planificador de layout para que opere por pagina logica y
  no solo por flujo corregido con `pageBreak`
- [x] 3.3 Garantizar que no haya contenido visible fuera de la hoja durante la
  transicion entre paginas

## 4. Reglas de corte por tipo de contenido

- [x] 4.1 Mantener split fino para bloques textuales usando posiciones reales de
  lineas/caret
- [x] 4.2 Implementar estrategia estructural para `bulletList`, `orderedList` y
  `taskList`, priorizando continuidad por item antes que tratamiento como texto
  plano
- [x] 4.3 Implementar politica estricta para bloques indivisibles e imagenes:
  si no caben, se mueven completos a la siguiente hoja
- [x] 4.4 Definir reglas claras para decidir cuando dividir un bloque y cuando
  moverlo completo

## 5. Compatibilidad con TipTap y estado del editor

- [x] 5.1 Confirmar que la nueva estrategia preserve transactions, seleccion,
  foco y scroll
- [x] 5.2 Confirmar que la seleccion de imagenes y node views siga estable
  despues de la repaginacion
- [x] 5.3 Confirmar que la serializacion HTML y el parsing no incorporen
  metadata espuria del layout visual

## 6. Performance y scheduling

- [x] 6.1 Reducir dependencia de medicion global completa por cada keystroke
- [x] 6.2 Introducir invalidacion incremental por bloque/pagina afectada
- [x] 6.3 Revisar `debounce`, `requestAnimationFrame` y locks de interaccion
  para minimizar flicker y correccion tardia
- [x] 6.4 Documentar impacto esperado en typing y estrategia de cache

## 7. Ajustes visuales compatibles con el nuevo motor

- [x] 7.1 Revisar `AppEditor.module.css` para asegurar que los estilos no
  contradigan la paginacion estricta
- [x] 7.2 Confirmar que hojas, content flow, zoom stage y page counter sigan
  alineados con el nuevo plan de layout
- [x] 7.3 Confirmar que el modo continuo no herede restricciones o regresiones
  del modo visual

## 8. Pruebas y no regresion

- [x] 8.1 Agregar pruebas unitarias del planificador de layout para texto,
  listas, task lists e imagenes
- [x] 8.2 Agregar pruebas de regresion para escritura al final de pagina sin
  overflow visible
- [x] 8.3 Agregar pruebas para listas y numeracion en el borde inferior
- [x] 8.4 Agregar pruebas para insercion de imagenes cerca del final de pagina
- [x] 8.5 Agregar pruebas para contenido pegado largo y contenido mixto
- [x] 8.6 Validar compatibilidad con zoom, contador de pagina y modo continuo
- [x] 8.7 Ejecutar pruebas focalizadas del modulo y registrar evidencia
- [x] 8.8 Ejecutar validacion TypeScript o equivalente y registrar residuos
  ajenos si aparecen

## Backend

- [x] No aplica: este cambio no requiere integracion backend ni contratos HTTP

## Evidencia

- `src/app/Components/UI/AppEditor/application/autoPagination.ts`:
  - se incorporo clasificacion estructural de bloques top-level
  - se agrego accion `list-item` para partir listas por item cuando el bloque
    completo no cabe al final de pagina
  - se introdujo margen de seguridad efectivo basado en el `line-height` del
    bloque para anticipar el corte antes del borde visible
  - se habilito medicion real de nodos atomicos como imagenes top-level para
    que tambien entren al planificador preventivo
  - se endurecio la regla para bloques no textuales altos, moviendolos a la
    siguiente hoja cuando ya no caben en la actual
- `src/app/Components/UI/AppEditor/application/autoPageBreak.ts`:
  - se agrego `splitListBlockBeforeItemAndInsertPageBreak`
  - `removeAutoPageBreaks` ahora recompone tambien listas compatibles separadas
    por `pageBreak` automaticos
- `src/app/Components/UI/AppEditor/application/useAppEditor.ts`:
  - la autopaginacion ahora reacciona con prioridad inmediata a transacciones
    con cambio de documento
  - se integra la nueva accion `list-item` dentro del loop de repaginacion
  - el HTML propagado por `onChange` ahora omite `pageBreak` automaticos para
    no persistir metadata visual espuria, pero conserva `pageBreak` manuales
  - se agrego una regresion de append/typing al final de pagina que valida
    continuidad a la siguiente hoja con metricas y contexto de pagina
- `src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts`:
  - el evento `app-editor-pagination-updated` ahora fuerza medicion inmediata
    en vez de pasar por debounce, cerrando la ventana entre repaginacion logica
    y actualizacion visual del sheet/contador
- `src/app/Components/UI/AppEditor/application/usePageContext.ts`:
  - el contexto de pagina escucha `app-editor-pagination-updated` y sincroniza
    el contador de pagina inmediatamente cuando cambia el layout visual
- `src/app/Components/UI/AppEditor/application/useAppEditor.ts`:
  - se introdujo `dirtyStartChildIndexRef` para que la repaginacion preventiva
    comience desde el bloque top-level afectado por la transaccion, en vez de
    escanear siempre el documento completo
- `src/app/Components/UI/AppEditor/application/autoPagination.ts`:
  - `resolveAutoPageBreakActions` ahora acepta `startChildIndex`
  - se agrego `resolveTopLevelChildIndexFromPosition` para convertir posicion
    de documento en punto de invalidacion incremental por bloque
- `openspec/changes/scrumcore-149-ajuste-paginacion-appeditor/design.md`:
  - se documento el impacto esperado en typing
  - se explico la estrategia de cache/invalidez incremental y cuando el motor
    debe volver a recomputacion amplia
- `src/app/Components/UI/AppEditor/AppEditor.module.css`:
  - `sheet` y `contentFlow` ahora recortan overflow visual para no dejar
    contenido visible por fuera del documento durante la transicion
- `src/app/Components/UI/AppEditor/application/normalizeEditorHtml.ts`:
  - se agrego saneamiento de `pageBreak` automaticos para comparacion/persistencia
    sin contaminar el contrato HTML del editor
- Pruebas ejecutadas:
  - `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppEditor/autoPagination.test.ts src/app/Components/UI/AppEditor/autoPageBreak.test.ts src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditor.integration.test.tsx`
  - `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppEditor/autoPagination.test.ts src/app/Components/UI/AppEditor/autoPageBreak.test.ts src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditor.integration.test.tsx src/app/Components/UI/AppEditor/AppEditorSaveAction.test.tsx`
  - resultado actualizado: `7 files passed`, `60 tests passed`
- Cobertura nueva en `autoPagination.test.ts`:
  - `bulletList` completa que debe moverse de pagina
  - `bulletList` partida por item
  - `orderedList` partida por item
  - `taskList` partida por item
  - imagen top-level movida completa a la siguiente hoja
  - parrafo largo con `split` preventivo
  - contenido mixto donde el planificador prioriza el primer bloque en conflicto
  - contenido pegado largo donde el planificador prioriza el primer parrafo
    desbordado antes de bloques posteriores
- Cobertura nueva en `useAppEditor.test.tsx` y `AppEditorSaveAction.test.tsx`:
  - el `onChange` no propaga `pageBreak` automaticos
  - los `pageBreak` manuales se conservan
  - la normalizacion HTML de persistencia elimina metadata visual automatica
- Cobertura nueva en `AppEditor.test.tsx`:
  - el contador de paginas se sincroniza cuando la repaginacion notifica un
    `app-editor-pagination-updated`, sin esperar un resize artificial
  - al volver de modo visual a continuo se retiran contador y estructura
    paginada, evitando regresiones del modo visual sobre el flujo continuo
- Cobertura nueva en `usePageContext.test.tsx`:
  - la pagina actual se sincroniza inmediatamente cuando llega la notificacion
    `app-editor-pagination-updated`
- Cobertura nueva en `autoPagination.test.ts`:
  - el planificador soporta invalidacion incremental comenzando desde un
    bloque top-level especifico
- Cobertura nueva en `useAppEditor.test.tsx`:
  - una escritura/append al final de pagina dispara continuidad a la siguiente
    hoja y actualiza total de paginas + pagina actual en el harness visual
- Validacion adicional:
  - `npm run build` -> falla por residuos ajenos al ticket en
    `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.profiling.test.tsx`
    con `TS1185: Merge conflict marker encountered`
