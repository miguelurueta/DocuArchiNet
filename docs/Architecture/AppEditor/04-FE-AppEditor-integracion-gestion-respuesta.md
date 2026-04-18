# Ticket 04 FE

## Título

Integración de `AppEditor` en `GestionRespuestaEditorContainer`

## Rol

Desarrollador Frontend Senior especializado en:

- React 19 + TypeScript estricto
- Clean Architecture
- Integración de componentes shared UI
- Testing con Vitest + Testing Library
- Accesibilidad (a11y)

## Objetivo

Integrar `AppEditor` dentro de
`GestionRespuestaEditorContainer` para reemplazar el placeholder actual del área
`editorSurface`, manteniendo intacto el shell visual del módulo y sin romper el
layout ni la funcionalidad ya implementada en `AppEditor`.

## Contexto obligatorio

Repo:

`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react`

Archivos clave:

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaEditorContainer.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css`
- `src/app/Components/UI/AppEditor/`

`AppEditor` ya existe y no debe duplicarse ni reimplementarse.

## Objetivo exacto

- NO reemplazar todo `GestionRespuestaEditorContainer`.
- Mantener el shell visual actual:
  - título
  - descripción
  - estructura del contenedor principal
- Reemplazar únicamente el contenido interno del área `editorSurface`, donde hoy
  existe el placeholder:

```text
Aqui se renderizara el editor de contenido y la vista principal de respuesta.
```

- En ese espacio debe renderizarse `AppEditor`.

## Regla arquitectónica (obligatoria)

- `GestionRespuestaEditorContainer` pertenece a la capa de `presentation` del
  módulo.
- `AppEditor` pertenece a UI shared.

Está PROHIBIDO:

- acoplar `AppEditor` al módulo `gestionCorrespondencia`
- mover lógica interna de `AppEditor` al módulo
- introducir lógica de negocio en el container
- romper la separación de capas

## Alcance esperado

- Mantener `GestionRespuestaEditorContainer` como wrapper/shell del módulo.
- Usar `AppEditor` como contenido interno en `editorSurface`.
- Evitar duplicación de header:
  - si el container ya renderiza `title` y `description`, no repetirlos dentro
    de `AppEditor`
- Si es necesario, adaptar `AppEditor` para modo embebido sin romper su API ni
  su comportamiento previo.
- Extender `AppEditor` shared para soportar control de tamaño de imágenes sin
  acoplar la solución al módulo consumidor.

## Baseline compartido a preservar

La integracion en `GestionRespuesta` no puede degradar el `AppEditor` shared ya
existente. Debe preservarse:

- toolbar completa;
- dropdown de encabezados en toolbar;
- boton visible de tema `light/dark`;
- UI visible para enlaces;
- UI visible para imagenes por URL y archivo;
- soporte futuro de resize de imagen como capacidad shared y no del modulo;
- deshacer y rehacer;
- scroll vertical interno del contenido;
- scrollbar adaptado al tema;
- soporte controlled del editor.

## Extension obligatoria: control de tamaño de imagen

El mismo ticket 04 FE debe contemplar la extension de `AppEditor` para que las
imagenes insertadas puedan ajustar su tamaño dentro del editor.

Reglas obligatorias:

- La capacidad debe implementarse en `AppEditor` shared, no en
  `GestionRespuesta`.
- Debe basarse en extension de Tiptap, no en hacks de CSS del modulo.
- El tamaño de la imagen debe persistirse en el documento HTML.
- Debe evitar overflow horizontal del editor.
- Debe seguir funcionando en modo embebido dentro de
  `GestionRespuestaEditorContainer`.
- Debe mantener compatibilidad con imagenes insertadas por URL y por archivo.

## Diseño técnico obligatorio para resize

- Crear o extender una extension de imagen en `infrastructure`.
- Incorporar atributos persistidos de nodo al menos para:
  - `width`
  - `height` o calculo proporcional
- Permitir actualizar tamaño sin romper serializacion HTML.
- Mantener la imagen responsiva respecto al contenedor del editor.
- Si se agrega UI de resize, debe existir alternativa usable por teclado.

No se permite:

- manejar resize solo con estilos temporales no persistidos;
- implementar el tamaño solo desde el modulo `gestionRespuesta`;
- perder el tamaño al rerender o al serializar contenido.

## Regla de estado (obligatoria)

- `AppEditor` debe usarse en modo controlled.

El estado debe vivir en:

- `GestionRespuestaEditorContainer`, o
- `GestionRespuestaMainTabContent`

Ejemplo esperado:

```tsx
const [editorValue, setEditorValue] = useState<string>("");

<AppEditor
  value={editorValue}
  onChange={setEditorValue}
/>
```

Está prohibido:

- duplicar estado
- usar modo uncontrolled en este contexto

## Reglas de implementación

- Reemplazar el placeholder por `AppEditor`.
- Mantener el layout actual intacto.
- No romper integración con:
  - `GestionRespuestaRightToolsPanel`
  - `workbenchBody`
  - toolbar del módulo
- `AppEditor` debe ocupar el área completa disponible.
- No introducir lógica adicional innecesaria.
- Mantener TypeScript estricto.
- Si se implementa resize de imagen, debe quedar encapsulado en `AppEditor`
  shared y no en el container del modulo.

## Reglas de layout (obligatorio)

- `AppEditor` debe ocupar el 100% del ancho y alto de `editorSurface`.
- El contenedor controla el tamaño.
- El scroll debe ser interno al editor.
- Evitar overflow horizontal.
- No romper el flexbox existente.

## Reglas de performance

- No recrear `AppEditor` en cada render.
- Evitar props inline no memoizadas cuando afecten estabilidad.
- No provocar re-render del contenedor completo al escribir.
- Mantener estabilidad del árbol de render.

## Reglas de accesibilidad

- El editor debe ser alcanzable mediante navegación por teclado (`tab`).
- El foco debe entrar correctamente al editor.
- No romper la navegación existente del módulo.
- Mantener accesibilidad básica del editor.

## Validaciones obligatorias

1. El placeholder desaparece.
2. `AppEditor` se renderiza dentro de `editorSurface`.
3. El título y la descripción siguen visibles una sola vez.
4. El layout del workbench no se rompe.
5. El panel lateral derecho sigue funcionando.
6. El editor mantiene scroll interno.
7. No hay overflow ni ruptura visual.
8. Las imagenes insertadas siguen respetando el contenedor del editor.
9. Si se ajusta el tamaño de una imagen, el cambio persiste en el contenido.

## Validaciones visuales

- El editor respeta el padding del contenedor.
- No rompe bordes ni sombras.
- Mantiene coherencia visual con el módulo.

## Pruebas de regresión obligatorias

- No se rompe `GestionRespuestaMainTabContent`.
- No se rompe `GestionRespuestaEditorContainer`.
- No se rompe el panel lateral (colapso/expansión).
- `AppEditor` sigue funcionando en:
  - modo controlled
  - toolbar
  - theme toggle `light/dark`
  - heading dropdown visible
  - links
  - imágenes
  - resize persistido de imágenes si se implementa en este ticket
  - `disabled` / `readOnly`
- No se afectan exports compartidos.
- No se rompen pruebas existentes.

## Pruebas de calidad obligatorias

- Validar render del título y la descripción.
- Validar render de `AppEditor`.
- Validar actualización de contenido.
- Validar accesibilidad básica.
- Validar estabilidad del layout.
- Agregar prueba de integración:
  - `AppEditor` renderizado dentro de `editorSurface`
- Agregar prueba de imagen si se implementa resize:
  - inserción
  - ajuste de tamaño
  - persistencia del atributo serializado

## Comandos mínimos esperados

```powershell
node .\node_modules\vitest\vitest.mjs --run src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditor.integration.test.tsx
```

Además:

- Ejecutar tests del módulo `gestionRespuesta` si existen.
- Ejecutar validación TypeScript.
- Reportar errores ajenos al cambio.

## Resultado esperado

- `GestionRespuestaEditorContainer` sigue siendo el shell visual.
- El placeholder desaparece completamente.
- `AppEditor` queda correctamente embebido.
- No se rompe el layout ni la funcionalidad existente.
- El editor funciona correctamente dentro del módulo.
- Existen pruebas y validaciones de regresión.

## Instrucción final

Implementar respetando estrictamente:

- Clean Architecture
- separación de capas
- uso de componentes shared
- performance
- accesibilidad
- testing
- documentación centralizada en `docs/Architecture/AppEditor`

El resultado debe ser una integración limpia, estable y lista para producción
dentro de un sistema modular tipo SaaS.
