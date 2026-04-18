# TICKET 01 FE

# =========================================

## Titulo

Reemplazar guías por hojas visuales reales con márgenes en `AppEditor`

---

## Rol

Desarrollador Frontend Senior especializado en:

* React 19 + TypeScript estricto
* Tiptap / ProseMirror
* CSS avanzado para layout de documento
* Clean Architecture
* Testing con Vitest + Testing Library

---

## Objetivo

Evolucionar el modo paginado de `AppEditor` para pasar de una representación
basada en guías de corte a una representación visual de hojas reales tipo
documento (A4), con márgenes visibles y separación clara entre páginas.

La implementación debe mantener el modelo interno de documento continuo
sin fragmentar el contenido.

---

## Contexto obligatorio

Repo:
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react`

Ubicación de documentación de tickets:
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react\docs\Architecture\AppEditor`

Archivos base relevantes:

* `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
* `src/app/Components/UI/AppEditor/AppEditor.module.css`
* `src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts`
* `src/app/Components/UI/AppEditor/domain/editor.types.ts`

---

## Problema actual

El editor soporta paginación visual mediante guías, pero el documento sigue
percibiéndose como una superficie continua:

* las guías atraviesan visualmente el contenido;
* no existe sensación clara de hoja;
* los márgenes no son percibidos como límites del documento;
* la experiencia es inferior a editores tipo Word/Docs.

---

## Alcance exacto

* Reemplazar la representación basada en líneas guía por hojas visuales reales.
* Cada hoja debe representar un formato tipo A4 con:

  * fondo blanco
  * sombra sutil
  * separación vertical clara entre hojas
  * workspace exterior diferenciado
* Renderizar márgenes visibles:

  * top
  * right
  * bottom
  * left
* El contenido debe percibirse dentro de una caja útil de documento.
* Mantener un único editor Tiptap y un documento continuo.

---

## Diseño técnico obligatorio

### Modelo de render

Debe mantenerse:

* un único `.ProseMirror`
* un único flujo de contenido

La representación visual debe construirse mediante capas:

* `canvas` (scroll container)
* `sheet` (hoja visual A4)
* `content layer` (ProseMirror)

---

### Márgenes

Los márgenes deben:

* ser visuales (no estructurales)
* NO usar padding directo sobre `.ProseMirror`
* NO modificar HTML persistido
* representarse como caja interna del documento

---

### Relación con guías existentes

* Las guías dejan de ser visibles como líneas
* PERO siguen siendo la base de cálculo de paginación
* La lógica de `usePaginationMetrics` se mantiene

---

## Reglas obligatorias

* No dividir el documento en múltiples instancias de editor
* No clonar contenido
* No alterar HTML persistido
* No insertar nodos adicionales
* No implementar todavía paginación automática real
* Mantener scroll continuo en el contenedor principal

---

## Compatibilidad obligatoria

NO debe romper:

* paginationMode="visual"
* guías de página (lógica interna)
* contador de página (Página X de Y)
* zoom visual
* saltos de página manual (pageBreak)
* imágenes:

  * resize
  * alineación horizontal
* modo controlled/uncontrolled
* toolbar existente

---

## Reglas arquitectónicas

* application:

  * mantiene cálculo de métricas de página
* presentation:

  * renderiza hojas visuales
  * composición de layout tipo documento
* domain:

  * sin cambios estructurales
* estilos:

  * definidos en `AppEditor.module.css`
  * sin estilos globales

---

## Resultado esperado

* El usuario percibe hojas reales tipo documento
* Los márgenes son visualmente claros
* La separación entre páginas es limpia
* El contenido sigue siendo un documento continuo
* La UX se acerca a editores profesionales

---

## Validaciones obligatorias

1. La hoja A4 se ve claramente delimitada
2. Los márgenes son visibles en los 4 lados
3. La separación entre hojas no cruza el texto
4. El scroll sigue funcionando correctamente
5. No se rompe toolbar ni editor
6. No se rompe contador ni guías internas
7. No cambia el HTML serializado
8. No hay parpadeos ni saltos visuales críticos

---

## Pruebas esperadas

* render de hojas en modo visual
* render de márgenes
* regresión del modo continuo
* regresión de paginación existente
* pruebas de estabilidad visual

---

## Restricciones

* No implementar salto automático por contenido
* No dividir documento internamente
* No alterar persistencia HTML
* No introducir múltiples editores
* No acoplar a módulos consumidores

---

## Instrucción final

Implementar una representación visual de hojas reales con márgenes para el modo
paginado de `AppEditor`, manteniendo el modelo continuo del documento, respetando
la arquitectura actual y garantizando compatibilidad total con las capacidades
existentes del editor.
