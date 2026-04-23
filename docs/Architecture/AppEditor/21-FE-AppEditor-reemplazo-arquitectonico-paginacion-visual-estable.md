# Prompt: 21-FE AppEditor reemplazo arquitectonico paginacion visual estable

Actua como arquitecto y desarrollador senior especializado en editores ricos con TipTap/ProseMirror, layout de documentos y UX tipo Word.

=====================================================================
OBJETIVO

Disenar e implementar un reemplazo profesional del sistema actual de paginacion visual de `AppEditor`, eliminando la arquitectura fragil basada en mutacion destructiva del documento y sustituyendola por un modelo mas estable, mantenible y consistente.

=====================================================================
CONTEXTO DEL PROBLEMA

El sistema actual:

- inserta `pageBreak` automaticos
- divide parrafos y listas
- recompone contenido dinamicamente
- remapea cursor/seleccion
- recalcula metricas de forma reactiva
- sincroniza scroll de forma inestable

Esto genera:

- scroll inestable
- cursor que salta
- desincronizacion entre vista y contenido real
- bugs al escribir al final de pagina
- comportamiento fragil entre hojas

=====================================================================
OBJETIVO FUNCIONAL

Mantener experiencia tipo documento paginado donde:

- se visualicen hojas tipo A4
- el contenido continue entre paginas de forma natural
- no exista contenido visible entre hojas
- el cursor funcione sin saltos
- el scroll sea estable
- la UX sea consistente y cercana a Word

=====================================================================
ACLARACION CRITICA

NO se permite:

- reemplazarlo por una hoja infinita sin visual de paginas
- perder la percepcion clara de hojas
- seguir parcheando el sistema actual como estrategia principal

SI se requiere:

- rediseño arquitectonico del motor de paginacion visual
- reduccion drastica de mutaciones del documento durante edicion
- mejora real de estabilidad general

=====================================================================
MODELO DE LAYOUT REQUERIDO (CRITICO)

La solucion debe basarse en una arquitectura desacoplada del documento:

- `Page Shells` -> representacion visual de paginas
- `Content Flow` -> flujo continuo editable del contenido
- `Layout Engine` -> calculo de distribucion visual por paginas

REGLA:

El documento TipTap NO debe fragmentarse como mecanismo principal para representar paginas.

Las paginas deben ser una representacion derivada del layout del flujo de contenido, no una transformacion destructiva del contenido.

=====================================================================
ESTRATEGIA DE PAGINACION (OBLIGATORIO)

Debe definirse con claridad:

- calculo de alto util por pagina
- medicion de contenido renderizado
- determinacion de limites visuales de pagina
- continuidad del contenido entre paginas
- calculo de pagina actual

PROHIBIDO como mecanismo base:

- dividir nodos del editor en cada ciclo
- insertar `pageBreak` automaticos para ajustar layout
- mutar el documento para forzar la paginacion
- usar restauracion de seleccion como base del comportamiento normal

La paginacion debe ser DERIVADA del layout, no del contenido.

=====================================================================
MODELO DE SCROLL (CRITICO)

Debe definirse:

- scroll continuo y estable del editor
- relacion consistente entre scroll y pagina actual
- eliminacion de jitter, correcciones bruscas y saltos de scroll
- comportamiento predecible durante typing, paste, imagenes y resize

El scroll NO debe depender de reescritura destructiva del documento.

=====================================================================
RESTRICCIONES DE PERFORMANCE (CRITICO)

- NO medir todo el documento en cada keystroke si puede evitarse
- NO recalcular todas las paginas en cada cambio menor
- priorizar layout incremental, segmentado o razonablemente acotado

Se debe justificar:

- cuando medir
- que cachear
- como minimizar reflows y trabajo redundante
- como mantener typing fluido

=====================================================================
REQUISITO ARQUITECTONICO PRINCIPAL

La nueva solucion NO debe depender principalmente de:

- `pageBreak` automaticos
- division de parrafos en cada edicion
- recomposicion completa del documento
- restauracion de seleccion como mecanismo base

Debe ser un sistema:

- estable
- predecible
- mantenible
- desacoplado del modelo de contenido

=====================================================================
PRIORIDADES

1. estabilidad de scroll
2. estabilidad de cursor y seleccion
3. continuidad de escritura entre hojas
4. visual de paginas
5. mantenibilidad
6. similitud con Word
7. performance

=====================================================================
RESTRICCIONES TECNICAS

- NO romper `AppEditor` como componente reusable
- NO romper toolbar, imagenes, listas, links, headings
- NO romper modo controlado/no controlado
- NO romper `readOnly`, `disabled`, `error`, `helperText`
- NO romper integracion con `gestionCorrespondencia`
- NO romper serializacion HTML
- NO romper imagenes locales ni rehidratacion
- NO introducir re-render innecesario del editor completo
- NO duplicar logica sin justificacion
- NO usar hacks de timing del navegador como solucion principal

=====================================================================
ELIMINACION O LIMPIEZA DE LOGICA OBSOLETA

Se debe identificar que piezas quedan obsoletas con el nuevo diseño y:

- eliminarlas si ya no aportan valor
- simplificarlas si parte de la logica sigue siendo util
- evitar convivencia de dos motores competidores

Candidatos principales a revision:

- `autoPagination.ts`
- `autoPageBreak.ts`
- logica de insercion automatica de `pageBreak`
- logica de division de nodos
- logica de repaginacion destructiva
- logica de restauracion forzada de seleccion asociada a repaginacion

NO se permite dejar codigo muerto, rutas huerfanas o complejidad duplicada.

=====================================================================
CRITERIOS DE ACEPTACION FUNCIONAL (OBLIGATORIO)

La solucion final debe cumplir, como minimo:

- al escribir al final de una hoja, el usuario no debe ver contenido en el gap entre paginas
- la continuidad del contenido debe verse en la hoja siguiente
- el cursor no debe saltar a posiciones inesperadas
- el scroll no debe corregirse bruscamente durante escritura
- la pagina actual debe mantenerse estable y coherente
- pegar contenido largo no debe romper scroll ni page context
- imagenes y bloques complejos no deben degradar la estabilidad general
- el comportamiento debe ser claramente mas robusto que el sistema actual

Si alguna expectativa no puede lograrse al nivel de Word, debe declararse explicitamente y proponer la alternativa mas solida posible.

=====================================================================
ENTREGABLES OBLIGATORIOS

### 1. Diagnostico tecnico

Explicar con claridad:

- causa raiz de los problemas actuales
- hooks, modulos y flujos problematicos
- que eliminar, que conservar y que reemplazar

### 2. Nueva arquitectura

Definir:

- layout engine
- representacion visual de paginas
- modelo de scroll
- calculo de pagina actual
- relacion entre contenido real y paginas visuales
- compatibilidad con zoom
- soporte para imagenes, listas y bloques complejos

### 3. Implementacion

- cambios reales en codigo
- no pseudo-soluciones
- no solo analisis

### 4. Limpieza

- eliminacion de logica obsoleta
- eliminacion de duplicacion
- simplificacion del sistema

### 5. Pruebas

Cubrir minimo:

- escritura al final de hoja
- continuidad a siguiente pagina
- ausencia de contenido visible en gaps
- scroll estable
- page counter correcto
- no regresion en imagenes
- no regresion en listas
- no regresion en paste largo
- no regresion en modo no paginado si existe

### 6. Validacion final

Explicar:

- que cambio
- que se elimino
- riesgos existentes
- por que la solucion nueva es superior

=====================================================================
DOCUMENTACION

Si se requiere documentacion tecnica nueva o actualizacion documental, crearla dentro de la estructura existente del repositorio, respetando la convencion actual, especialmente en:

- `docs/Architecture/AppEditor/`

No inventar rutas arbitrarias.
Si algun documento no aplica, indicarlo explicitamente como `No aplica`.

=====================================================================
ARCHIVOS RELEVANTES

- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
- `src/app/Components/UI/AppEditor/application/useAppEditor.ts`
- `src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts`
- `src/app/Components/UI/AppEditor/application/usePageContext.ts`
- `src/app/Components/UI/AppEditor/application/autoPagination.ts`
- `src/app/Components/UI/AppEditor/application/autoPageBreak.ts`
- `src/app/Components/UI/AppEditor/AppEditor.module.css`
- tests en `src/app/Components/UI/AppEditor/`

=====================================================================
CRITERIO DE CALIDAD

- evitar soluciones fragiles
- evitar simulaciones visuales fragiles apoyadas en mutacion destructiva del documento
- priorizar estabilidad sobre complejidad innecesaria
- mantener coherencia arquitectonica
- no maquillar un parche como si fuera una solucion final

Si algo NO puede lograrse al nivel de Word:

- declararlo explicitamente
- justificar por que
- proponer una alternativa solida y profesional

=====================================================================
FORMATO DE RESPUESTA

1. resumen del problema
2. diagnostico tecnico
3. propuesta de arquitectura
4. implementacion
5. pruebas ejecutadas
6. riesgos y limitaciones reales
