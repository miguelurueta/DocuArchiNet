# =========================================
# TICKET 03 FE
# =========================================

## Titulo
Agregar alineación horizontal persistida para imágenes en `AppEditor`

---

## Rol
Desarrollador Frontend Senior especializado en:
- React 19 + TypeScript estricto
- Tiptap / ProseMirror
- Modelado de extensiones custom
- Serialización HTML
- Testing con Vitest + Testing Library

---

## Objetivo

Permitir que las imágenes dentro de `AppEditor` puedan alinearse horizontalmente
(izquierda, centro, derecha), manteniendo persistencia en el contenido y correcta
rehidratación, sin afectar funcionalidades existentes como resize y selección.

---

## Contexto obligatorio

Repo:
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react`

Archivos base:

- `src/app/Components/UI/AppEditor/infrastructure/resizable-image.extension.ts`
- `src/app/Components/UI/AppEditor/infrastructure/tiptap.extensions.ts`
- `src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx`
- `src/app/Components/UI/AppEditor/AppEditor.module.css`

---

## Problema actual

- Las imágenes pueden insertarse y redimensionarse.
- No existe control de alineación horizontal.
- No hay persistencia de posicionamiento.

---

## Alcance exacto

- Permitir alineación:
  - `left`
  - `center`
  - `right`
- Persistir la alineación en el HTML serializado.
- Rehidratar correctamente esa alineación.
- Mantener compatibilidad total con resize.
- Mantener comportamiento estable del editor.

---

## Regla técnica principal

La alineación debe implementarse como atributo persistido del nodo de imagen.

### Decisión obligatoria

Usar atributo:

```html
<img data-align="left|center|right" />
```

No usar:
- estilos inline
- clases externas

---

## Diseño técnico obligatorio

### Extensión de imagen

- Extender `resizable-image.extension.ts`
- Agregar atributo:

```ts
align: {
  default: 'left',
  parseHTML: element => element.getAttribute('data-align') || 'left',
  renderHTML: attributes => ({
    'data-align': attributes.align,
  }),
}
```

### Regla de compatibilidad

- La serialización nueva no debe perder atributos existentes de la imagen, por ejemplo:
  - `data-width`
- Debe convivir con el modelo actual del resize.

---

### Comando obligatorio

Definir comando:

```ts
setImageAlign: (align: 'left' | 'center' | 'right')
```

Debe:
- aplicar solo si la imagen está seleccionada o activa como nodo de imagen
- actualizar atributos del nodo
- no romper selección ni foco del editor

---

### Integración con toolbar

- Mostrar controles de alineación SOLO cuando:
  - la imagen esté activa, o
  - exista selección válida sobre el nodo imagen

- Controles:
  - botón left
  - botón center
  - botón right

---

## Comportamiento visual (obligatorio)

Implementar vía CSS basado en atributo:

```css
img[data-align="left"] {
  display: block;
  margin-left: 0;
  margin-right: auto;
}

img[data-align="center"] {
  display: block;
  margin-left: auto;
  margin-right: auto;
}

img[data-align="right"] {
  display: block;
  margin-left: auto;
  margin-right: 0;
}
```

---

## Compatibilidad hacia atrás

- Imágenes sin atributo deben comportarse como:
  - `left`
- No romper contenido existente
- No romper imágenes ya guardadas con `data-width` u otros atributos persistidos

---

## Reglas arquitectónicas

- `infrastructure`:
  - extensión Tiptap
  - atributo persistido
  - comando `setImageAlign`

- `presentation`:
  - toolbar
  - controles visibles de alineación

- `AppEditor.module.css`:
  - render visual de alineación

---

## Resultado esperado

- El usuario puede alinear imágenes fácilmente
- La alineación se mantiene al guardar
- La alineación se mantiene al rehidratar
- Resize sigue funcionando correctamente

---

## Validaciones obligatorias

1. Se puede cambiar alineación de imagen seleccionada
2. La alineación se refleja visualmente
3. El HTML contiene `data-align`
4. Rehidratar mantiene la alineación
5. Resize sigue funcionando
6. No se rompe selección de imagen
7. No se rompe edición del documento
8. No se pierden atributos persistidos existentes de la imagen

---

## Pruebas esperadas

- tests de extensión (atributo `align`)
- tests de serialización HTML
- tests de rehidratación
- tests de comandos (`setImageAlign`)
- tests de integración en `AppEditor`
- regresión de resize

---

## Restricciones

- No implementar posicionamiento libre
- No usar canvas
- No usar estilos inline
- No modificar estructura base del documento

---

## Instrucción final

Implementar alineación horizontal de imágenes como una capacidad persistida,
estable y desacoplada dentro de `AppEditor`, asegurando compatibilidad con el
modelo actual del editor, con resize existente y evitando regresiones.
