# TICKET FE
# =========================================

## Titulo
Habilitar almacenamiento temporal de imágenes del `AppEditor` con `IndexedDB`

---

## Rol
Desarrollador Frontend Senior especializado en:
- React 19 + TypeScript estricto
- Tiptap / ProseMirror
- Manejo de archivos en navegador
- IndexedDB
- Clean Architecture
- Testing con Vitest + Testing Library

---

## Objetivo

Implementar una capa de almacenamiento temporal en navegador usando `IndexedDB`
para las imágenes locales insertadas en `AppEditor`, desacoplando completamente
la edición del guardado en backend.

Este ticket NO incluye persistencia final en servidor. Solo infraestructura cliente.

---

## Contexto obligatorio

Repo:
C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react

Archivos base relevantes:

- src/app/Components/UI/AppEditor/presentation/AppEditor.tsx
- src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx
- src/app/Components/UI/AppEditor/infrastructure/resizable-image.extension.ts
- src/app/Components/UI/AppEditor/infrastructure/tiptap.extensions.ts
- src/app/Components/UI/AppEditor/application/useAppEditor.ts
- src/app/Components/UI/AppEditor/AppEditor.module.css

Ubicación de documentación (obligatoria):
C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react\docs\Architecture\AppEditor

---

## Problema actual

Las imágenes locales insertadas en AppEditor:

- no tienen persistencia controlada en cliente
- dependen del flujo inmediato de subida
- no pueden rehidratarse correctamente en sesión
- no están desacopladas del backend

---

## Alcance exacto

### 1. Infraestructura IndexedDB (obligatorio)

Crear adaptador reusable en:
src/app/Components/UI/AppEditor/infrastructure/indexeddb/

Debe soportar:

- init DB
- versionado
- object store
- saveImage(blob)
- getImage(id)
- deleteImage(id)
- clearByScope(documentDraftId | sessionId)

Debe ser desacoplado, testeable y sin dependencia de React.

---

### 2. Modelo de imagen temporal

Definir estructura mínima:

type LocalImage = {
  id: string;
  fileName: string;
  contentType: string;
  size: number;
  blob: Blob;
  createdAt: number;
  documentDraftId?: string;
  sessionId?: string;
};

Generación de ID obligatoria:

- img_local_<uuid>
- estable y único

---

### 3. Integración con AppEditor (flujo correcto)

Flujo obligatorio al insertar imagen local:

1. Usuario selecciona archivo
2. Se genera localImageId
3. Se guarda en IndexedDB
4. Se crea URL temporal segura
5. Se inserta nodo imagen en Tiptap

HTML esperado:

<img
  src="blob:<object-url>"
  data-local-image-id="img_local_xxx"
  data-source="local"
  data-width="50%"
  data-align="center"
/>

IMPORTANTE:
NO usar esquemas tipo local-image:// como src directo.

---

### 4. Gestión de Object URLs (CRÍTICO)

- Crear URL.createObjectURL(blob) al recuperar imagen
- Liberar con URL.revokeObjectURL cuando:
  - se elimina imagen
  - se desmonta componente
  - se reemplaza imagen

---

## Reglas obligatorias

NO hacer:

- subir imágenes al backend
- generar URLs finales
- guardar documento completo
- usar localStorage
- lógica de IndexedDB en JSX
- romper API actual

---

## Reglas arquitectónicas (obligatorio)

- infrastructure:
  - adapter IndexedDB
  - manejo de blobs

- application:
  - orquestación del flujo de imagen

- presentation:
  - selección de archivo
  - interacción con usuario

- AppEditor:
  - debe seguir siendo reusable y desacoplado

---

## Compatibilidad obligatoria

Debe seguir funcionando sin romper:

- inserción por URL remota
- resize de imagen
- alineación horizontal
- serialización HTML
- modo controlled/uncontrolled

---

## Resultado esperado

- imágenes locales se almacenan en IndexedDB
- el editor usa blob URLs válidas para render
- el HTML contiene data-local-image-id
- infraestructura reusable lista para futura subida a backend

---

## Validaciones obligatorias

1. Insertar imagen crea registro en IndexedDB
2. Imagen se renderiza correctamente
3. img contiene data-local-image-id
4. Se generan y liberan correctamente Object URLs
5. Inserción por URL externa sigue funcionando
6. No se rompe resize ni alineación
7. HTML se serializa correctamente
8. Rehidratación básica funciona en sesión

---

## Pruebas esperadas

- tests del adaptador IndexedDB (CRUD)
- tests de generación de IDs
- tests de inserción de imagen local
- tests de serialización HTML
- tests de liberación de Object URLs
- regresión de funcionalidades existentes

---

## Restricciones

- No backend
- No sincronización remota
- No reconciliación de imágenes
- No exportación
- No persistencia final

---

## Entregables obligatorios

- adapter IndexedDB completo
- integración en useAppEditor
- ajuste en flujo de inserción de imagen
- tests unitarios e integración
- documentación en:

docs/Architecture/AppEditor/

---

## Instrucción final

Implementar una capa robusta, desacoplada y performante de almacenamiento temporal de imágenes en IndexedDB para AppEditor, asegurando compatibilidad total con el editor actual y dejando la base lista para futura persistencia en backend.
