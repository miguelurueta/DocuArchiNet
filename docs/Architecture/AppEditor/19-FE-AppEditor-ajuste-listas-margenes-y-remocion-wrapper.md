# Prompt: 19-FE AppEditor ajuste listas, margenes y remocion de wrapper

Actua como arquitecto de software y analista tecnico senior del proyecto.

Necesito que generes la documentacion tecnica completa del cambio `19-FE` de `AppEditor`.

=====================================================================
DATOS DEL CAMBIO

Nombre del cambio:
Ajuste de listas, margenes y remocion de wrapper intermedio en AppEditor

Ticket:
SCRUMCORE-19-FE

Ticket backend relacionado:
No aplica

Problema base del cambio:
- Las listas con viñetas y numeracion del editor no respetan correctamente la margen visual ya establecida.
- La sangria actual de `ul/ol` se suma al padding base del editor y genera desplazamiento visual excesivo.
- La estructura renderizada contiene un wrapper visual intermedio adicional entre el shell del editor y el contenido editable.
- Se requiere simplificar la jerarquia DOM sin romper toolbar, accesibilidad, scroll ni paginacion visual.

Objetivo tecnico:
- corregir sangria de viñetas y numeracion
- respetar la margen ya establecida del editor
- simplificar la estructura del contenido removiendo la capa intermedia redundante
- preservar semantica, accesibilidad y estabilidad de paginacion visual

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
REGLAS CRÍTICAS DE CONSISTENCIA

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
TRAZABILIDAD CÓDIGO → DOCUMENTACIÓN

Cada cambio debe incluir:

- Archivo exacto
- Ruta completa
- Componente o función afectada

Ejemplo obligatorio:
Archivo: src/.../AppEditor.tsx
Seccion: render del contentEditable
Cambio: eliminacion del wrapper `surface`

No usar descripciones abstractas sin ubicacion real.

=====================================================================
VALIDACIÓN TIPTAP (CRÍTICO)

Si el cambio afecta estructura del contenido:

- Validar impacto en:
  - extensiones TipTap
  - serializacion HTML
  - parsing
  - comandos (listas, etc.)

- Confirmar:
  - no se rompe contenido existente
  - no se pierde formato al guardar/cargar

=====================================================================
ACCESIBILIDAD (OBLIGATORIO)

Validar:

- ul/ol/li mantienen semantica correcta
- navegación por teclado no se rompe
- roles ARIA no se ven afectados

=====================================================================
ARCHIVOS OBLIGATORIOS

### 1. SCRUMCORE-19-FE-Arquitectura.md

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
- rutas y componentes reales
- diagramas Mermaid o pseudo UML

Documentar especificamente:

- ajuste de listas ul/ol/li
- respeto de margenes
- eliminacion del wrapper intermedio (surface si aplica)
- preservacion de frame y editorWrapper

Impacto en:

- AppEditor
- toolbar
- content surface
- paginacion visual
- estilos

=====================================================================

### 2. SCRUMCORE-19-FE-Implementacion-Detallada.md

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

  - JSX anterior vs JSX nuevo
  - selectores CSS afectados
  - comportamiento de listas
  - cambios en tests

- indicar si hubo:
  - refactor estructural
  - ajuste de contrato
  - correccion visual

=====================================================================

### 3. SCRUM-19-FE-Integracion-BackEnd.md

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

### 4. SCRUM-19-FE-Pruebas.md

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

- listas con viñetas
- listas numeradas
- margenes en modo continuo
- margenes en modo paginado
- estructura sin wrapper intermedio
- no regresion en:
  - toolbar
  - zoom
  - page counter

=====================================================================
CRITERIOS DE SALIDA

La documentacion debe:

- usar nombres exactos de archivos
- reflejar rutas reales
- ser implementable
- evitar contenido generico
- mantener consistencia con arquitectura AppEditor
- incluir relaciones entre:
  - frontend
  - estilos
  - hooks
  - TipTap
  - tests
  - backend (si aplica)

=====================================================================
FORMATO DE ENTREGA

Entregar en este orden:

1. SCRUMCORE-19-FE-Arquitectura.md
2. SCRUMCORE-19-FE-Implementacion-Detallada.md
3. SCRUM-19-FE-Integracion-BackEnd.md
4. SCRUM-19-FE-Pruebas.md

Si falta informacion:

Agregar seccion final:
"Supuestos y pendientes"

=====================================================================
REGLA FINAL

No se considera completo el ticket si:

- falta algun documento
- hay inconsistencias entre documentos
- se usa informacion inventada
- no hay trazabilidad con el codigo real