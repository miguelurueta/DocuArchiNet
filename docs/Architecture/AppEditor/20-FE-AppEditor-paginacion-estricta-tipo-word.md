# Prompt: 20-FE AppEditor paginacion estricta tipo Word

Actua como arquitecto de software senior especializado en editores ricos (TipTap / ProseMirror) y motores de layout.

Necesito que generes la documentacion tecnica completa del cambio `20-FE` de `AppEditor`.

=====================================================================
DATOS DEL CAMBIO

Nombre del cambio:
Paginacion estricta tipo Word en AppEditor

Ticket:
SCRUMCORE-20-FE

Ticket backend relacionado:
No aplica

Problema base del cambio:
- En `AppEditor` con `paginationMode="visual"` el contenido puede llegar al borde inferior de la hoja y luego ser corregido.
- El comportamiento actual no es estricto ni preventivo; todavia permite que texto, listas o imagenes se perciban fuera de la margen util antes del ajuste.
- La experiencia visual actual se comporta mas como flujo continuo con correccion posterior que como un documento paginado real.
- Se requiere una experiencia equivalente a Word en control de layout, corte y continuidad entre paginas.

Objetivo tecnico:
- lograr paginacion estricta y preventiva tipo Word
- impedir que cualquier contenido se vea o escriba por fuera de la hoja o de la margen util
- definir un modelo explicito de layout y corte antes del desborde
- mantener suavidad visual, continuidad estructural y compatibilidad con TipTap/ProseMirror

=====================================================================
OBJETIVO PRINCIPAL

Quiero que `AppEditor` en `paginationMode="visual"` se comporte de forma equivalente a un editor de documentos tipo Microsoft Word en el manejo del contenido dentro de cada hoja.

El comportamiento NO debe ser aproximado. Debe ser funcionalmente equivalente en control de layout, flujo y paginacion.

=====================================================================
REGLA PRINCIPAL

NO se aceptan soluciones basadas en:

- parches CSS
- overflow hidden
- esconder visualmente el problema
- correcciones despues del desborde
- re-render completo del editor
- hacks visuales sin modelo de layout

La solucion debe resolver la logica real de paginacion antes del render.

=====================================================================
MODELO DE PAGINACION REQUERIDO (CRITICO)

La solucion debe basarse en un modelo explicito de layout, no en CSS.

Debe definirse claramente:

- como se mide el contenido (DOM measurement, layout virtual o hibrido)
- como se calcula el alto util de cada pagina
- como se decide el punto de corte antes del desborde
- como se representa cada pagina (modelo logico, no solo visual)

El sistema debe funcionar como un motor de layout incremental, no como render + correccion.

=====================================================================
COMPORTAMIENTO OBLIGATORIO TIPO WORD

Cuando el usuario llegue al final de la hoja:

- el contenido NO debe invadir el borde inferior
- el salto a la siguiente pagina ocurre ANTES del desborde
- la transicion es suave y natural
- NO hay flicker ni correccion posterior
- NO hay compresion de texto
- NO hay contenido visible fuera de la hoja

=====================================================================
DEFINICION TECNICA DE "EQUIVALENTE A WORD"

Se considera equivalente si:

- nunca hay overflow visible en ningun momento
- el corte ocurre antes del desborde
- no hay parpadeo ni correcciones posteriores
- bloques mantienen integridad visual
- el comportamiento es consistente para todos los tipos de contenido

NO es equivalente si:

- el contenido se sale y luego se corrige
- hay flicker o salto visual
- depende de overflow hidden
- cambia el comportamiento segun el contenido

=====================================================================
TIPOS DE CONTENIDO A SOPORTAR

- parrafos
- headings
- listas con viñetas
- listas numeradas
- task lists
- palabras largas
- contenido multilinea
- contenido pegado
- imagenes
- bloques mixtos
- bloques altos

=====================================================================
ESTRATEGIA DE CORTE DE CONTENIDO (OBLIGATORIO)

Debe definirse explicitamente:

- corte de texto basado en lineas reales (no estimaciones)
- continuidad estructural en listas
- manejo de bloques indivisibles (se mueven completos)
- manejo de bloques divisibles (corte elegante)
- reglas claras de cuando dividir vs mover

NO se permite corte arbitrario sin criterio estructural.

=====================================================================
REGLAS POR TIPO DE CONTENIDO

### Texto
- continuidad fluida a siguiente pagina antes del desborde

### Listas
- mantener sangria, numeracion y estructura
- continuidad correcta entre paginas

### Imagenes
- si no caben, se mueven completas
- nunca se cortan de forma visual incorrecta

### Bloques indivisibles
- mover completo a siguiente pagina

### Bloques divisibles
- corte natural y legible

=====================================================================
INTEGRACION CON TIPTAP (CRITICO)

La solucion debe:

- respetar el modelo de nodos de TipTap
- no romper transactions
- no duplicar logica sin sincronizacion
- mantener coherencia entre estado del editor y layout

Debe considerarse:

- extensiones
- node views
- serializacion HTML
- parsing

=====================================================================
RESTRICCIONES DE PERFORMANCE (CRITICO)

- NO medir todo el documento en cada keystroke
- NO re-render completo del editor
- el layout debe ser incremental o segmentado

Se debe justificar:

- frecuencia de medicion
- estrategia de cache
- impacto en typing

=====================================================================
REQUISITO ARQUITECTONICO

- si la arquitectura actual soporta esto → implementarlo correctamente
- si NO lo soporta → declararlo explicitamente
- proponer refactor real del motor de paginacion si aplica

NO entregar soluciones parciales maquilladas como finales.

=====================================================================
RESTRICCION DE HONESTIDAD TECNICA

Debe declararse claramente si:

- no se alcanza comportamiento tipo Word
- existen limitaciones reales

NO afirmar cumplimiento si:

- hay overflow visible
- hay correcciones tardias
- listas o imagenes invaden bordes

=====================================================================
COMPATIBILIDAD OBLIGATORIA

Debe mantenerse estable:

- toolbar
- foco y cursor
- seleccion de texto
- seleccion de imagenes
- zoom
- contador de paginas
- serializacion HTML
- modo continuo
- modo visual
- imagenes locales
- accesibilidad

=====================================================================
VALIDACION OBLIGATORIA

Escenarios minimos:

1. escribir hasta el final de pagina
2. continuar escribiendo sin desbordar
3. listas al borde
4. numeracion al borde
5. editar listas en limite
6. insertar imagenes cerca del final
7. pegar contenido largo
8. contenido mixto
9. nunca overflow visible
10. transicion suave
11. zoom correcto
12. modo continuo sin regresion

=====================================================================
CONTEXTO BASE OBLIGATORIO

- Componente principal:
  src/app/Components/UI/AppEditor/

- Arquitectura de referencia:
  docs/Architecture/AppEditor/AppEditor-Architecture.md

- Ubicacion de salida:
  docs/Architecture/AppEditor/

- Mantener consistencia con documentos existentes
- No inventar rutas, funciones, endpoints ni pruebas inexistentes
- Si algo no aplica: usar "No aplica" o "Pendiente de confirmacion"
- Redactar en español tecnico, concreto y orientado a implementacion
- Priorizar decisiones verificables sobre opiniones

=====================================================================
REGLAS CRITICAS DE CONSISTENCIA

- La informacion entre:
  - Arquitectura
  - Implementacion
  - Pruebas
  debe ser consistente.

- No se permiten contradicciones entre:
  - DOM
  - funciones
  - rutas
  - comportamiento

- Si hay incertidumbre:
  usar "Pendiente de confirmacion"

=====================================================================
TRAZABILIDAD CODIGO → DOCUMENTACION

Cada cambio debe incluir:

- Archivo exacto
- Ruta completa
- Componente o funcion afectada

Ejemplo obligatorio:
Archivo: src/.../useAppEditor.ts
Seccion: motor de auto paginacion visual
Cambio: reemplazo de estrategia correctiva por estrategia preventiva

No usar descripciones abstractas sin ubicacion real.

=====================================================================
ARCHIVOS OBLIGATORIOS DE ENTREGA

SCRUMCORE-20-FE-Arquitectura.md
SCRUMCORE-20-FE-Implementacion-Detallada.md
SCRUM-20-FE-Integracion-BackEnd.md
SCRUM-20-FE-Pruebas.md

=====================================================================
CONTENIDO OBLIGATORIO POR ARCHIVO

### 1. SCRUMCORE-20-FE-Arquitectura.md

Debe incluir:

- Requerimiento
- Diagrama de clases
- Diagrama de secuencia
- Diagrama de estados
- Casos de uso
- Flujo de ejecucion
- Justificacion arquitectonica

Adicionalmente:

- objetivo, alcance, restricciones y riesgos
- evaluacion honesta de la arquitectura actual
- estrategia de layout
- estrategia de medicion
- estrategia de corte
- modelo de pagina logica
- compatibilidad con TipTap
- rutas y componentes reales
- diagramas Mermaid o pseudo UML

Documentar especificamente:

- por que el modelo actual no se comporta como Word
- como debe prevenirse el desborde antes del render visible
- manejo de bloques textuales vs no textuales
- comportamiento de listas, task lists e imagenes
- relacion entre motor de layout, zoom, page counter y surface visual

=====================================================================

### 2. SCRUMCORE-20-FE-Implementacion-Detallada.md

Debe incluir:

- Funciones creadas
- Funciones modificadas
- Descripcion de funciones
- Ubicacion exacta (ruta/archivo)
- Decisiones tecnicas

Adicional:

- separar:
  - componentes
  - hooks
  - utilidades
  - extensiones
  - estilos
  - pruebas

- especificar:
  - flujo actual vs flujo propuesto
  - puntos de medicion
  - criterios de split y move
  - contrato entre layout y estado del editor
  - impacto en node views
  - manejo de imagenes y listas
  - cambios en pruebas

- indicar si hubo:
  - refactor del motor de paginacion
  - cambio de estrategia de layout
  - endurecimiento de reglas de corte
  - optimizacion incremental

=====================================================================

### 3. SCRUM-20-FE-Integracion-BackEnd.md

Debe existir SIEMPRE

Para este caso:

- escribir:
  "No aplica por ausencia de integracion backend en este cambio"

Si llegara a aplicar:

- Endpoint(s)
- Parametros
- Respuestas
- Manejo de errores
- Relacion con frontend

=====================================================================

### 4. SCRUM-20-FE-Pruebas.md

Debe incluir:

- Unitarias
- Integracion UI
- Browser interaction
- E2E
- Casos de prueba
- Resultados
- Evidencia de no regresion

Adicional:

- separar:
  - pruebas existentes
  - pruebas nuevas
  - pruebas recomendadas

- identificar archivos reales de test

Validar minimo:

- typing en ultimo renglon
- corte preventivo antes del borde inferior
- listas con viñetas al borde
- listas numeradas al borde
- task lists al borde
- imagenes cerca del final de pagina
- contenido pegado largo
- mezcla de texto + lista + imagen
- no overflow visible
- continuidad suave entre paginas
- compatibilidad con zoom y page counter
- no regresion en modo continuo

=====================================================================
ENTREGABLE TECNICO ESPERADO

La respuesta debe incluir:

- diagnostico de causa raiz
- explicacion del problema actual
- evaluacion de la arquitectura actual
- propuesta tecnica real
- plan de implementacion por fases
- estrategia de layout
- estrategia de corte
- plan de pruebas

=====================================================================
CRITERIO FINAL DE ACEPTACION

La solucion SOLO es valida si:

- ningun contenido se renderiza fuera de la hoja
- el sistema previene el desborde
- la transicion es natural
- listas e imagenes respetan limites
- el comportamiento es consistente

Si no se logra equivalencia real:

- debe declararse explicitamente
- sin maquillar resultados

=====================================================================
REGLA FINAL

No se considera completo el ticket si:

- falta algun documento
- hay inconsistencias entre documentos
- se usa informacion inventada
- no hay trazabilidad con el codigo real
- se afirma equivalencia a Word sin demostrar prevencion real del desborde
