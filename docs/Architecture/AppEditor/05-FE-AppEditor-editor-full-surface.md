# Ticket 05 FE

## Titulo

Eliminar el shell interno de `GestionRespuestaEditorContainer` para que `AppEditor` ocupe toda la superficie principal

## Rol

Desarrollador Frontend Senior especializado en:

- React 19 + TypeScript estricto
- Clean Architecture
- UI shared components
- Testing con Vitest + Testing Library
- Integracion de modulos desacoplados

## Objetivo

Ajustar la integracion existente de `AppEditor` dentro del modulo de gestion de
respuesta para eliminar el bloque visual interno que duplicaba la capa de
presentacion del container y permitir que el editor tome como superficie util
todo el espacio del panel principal.

## Contexto obligatorio

Repo:

`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react`

Archivos involucrados:

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaEditorContainer.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.test.tsx`

## Problema a resolver

Despues de integrar `AppEditor`, el container seguia renderizando un header
interno con titulo y descripcion, ademas de una superficie intermedia
`editorSurface`. Ese shell adicional hacia que el editor no tomara realmente
todo el espacio disponible del panel principal.

Se requiere simplificar el container para que actue solo como wrapper neutro y
que `AppEditor` quede como superficie dominante del workspace.

## Alcance exacto

- Convertir `GestionRespuestaEditorContainer` en un contenedor neutro.
- Eliminar props visuales que ya no aportan al layout:
  - `title`
  - `description`
- Eliminar el header interno del container.
- Eliminar la capa visual `editorSurface`.
- Mantener el `aria-label` del container principal.
- Hacer que `AppEditor` ocupe el 100% de la superficie disponible.
- Mantener intacta la logica del editor.
- Mantener intacto el panel lateral derecho.
- Mantener intacta la toolbar del modulo.

## Reglas arquitectonicas

- No mover logica de negocio al container.
- No acoplar `AppEditor` al modulo `gestionCorrespondencia`.
- No modificar el comportamiento interno shared de `AppEditor` salvo layout de
  integracion desde el modulo consumidor.
- Mantener la separacion entre shell del modulo y componente shared.

## Reglas de implementacion

- `GestionRespuestaEditorContainer` debe renderizar solo `children`.
- `GestionRespuestaMainTabContent` debe dejar de enviar `title` y
  `description`.
- El CSS del container debe simplificarse para no agregar bordes, padding ni
  cajas internas redundantes.
- `embeddedAppEditor` debe expandirse con `flex: 1 1 auto`.
- No romper colapso y expansion de `GestionRespuestaRightToolsPanel`.

## Validaciones obligatorias

1. El editor sigue renderizando dentro del panel principal.
2. El container sigue siendo accesible por `aria-label`.
3. El editor ocupa toda la superficie util del panel.
4. El panel lateral derecho sigue colapsando y expandiendo.
5. No quedan referencias activas a `editorSurface`.
6. La prueba del tab se actualiza a la nueva estructura.

## Resultado esperado

- `GestionRespuestaEditorContainer` queda reducido a wrapper neutro.
- `AppEditor` pasa a ser la superficie real del panel principal.
- Se elimina duplicacion visual innecesaria.
- No se afecta funcionalidad existente del editor ni del panel lateral.

## Entregables de gestion

### Prompt para ticket Jira

Crear un ticket FE con este contenido:

**Titulo sugerido**

`[FE] Ajustar GestionRespuestaEditorContainer para que AppEditor ocupe toda la superficie principal`

**Descripcion sugerida**

Se requiere simplificar la integracion actual de `AppEditor` en
`GestionRespuestaMainTab` para eliminar el shell visual interno de
`GestionRespuestaEditorContainer`. El container debe quedar como wrapper neutro
y `AppEditor` debe ocupar toda la superficie util del panel principal, sin
afectar el panel lateral de herramientas ni la toolbar del modulo.

Cambios esperados:

- remover `title` y `description` de `GestionRespuestaEditorContainer`;
- eliminar el header interno y la capa `editorSurface`;
- simplificar estilos del container;
- hacer que `AppEditor` se expanda al 100% del area disponible;
- actualizar pruebas de integracion del tab.

Criterios de aceptacion:

- el editor ocupa toda la superficie principal;
- no se rompe el colapso/expansion del panel lateral;
- no se rompe accesibilidad basica del contenedor;
- pruebas del tab actualizadas y pasando.

### Mensaje de commit sugerido

`refactor(gestionRespuesta): remove editor container shell and let AppEditor fill panel`

### Titulo de PR sugerido

`refactor(gestionRespuesta): simplify editor container and expand AppEditor surface`

### Descripcion de PR sugerida

## Resumen

Este cambio elimina la capa visual interna de
`GestionRespuestaEditorContainer` para que `AppEditor` tome toda la superficie
util del panel principal.

## Cambios realizados

- se removieron `title` y `description` del container;
- se elimino el header interno del container;
- se elimino la capa `editorSurface`;
- se simplifico el CSS del contenedor;
- se ajusto el layout para que `AppEditor` crezca a toda la altura;
- se actualizo la prueba de integracion del tab.

## Impacto esperado

- menos duplicacion visual;
- integracion mas limpia entre el modulo y el editor shared;
- mismo comportamiento funcional del panel lateral.

## Archivos tocados

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaEditorContainer.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.test.tsx`

## Instruccion final

Usar este documento como base para:

1. crear el ticket en Jira;
2. asociar el cambio al ticket;
3. generar commit con mensaje claro;
4. abrir PR con descripcion consistente;
5. dejar el cambio listo para merge una vez el ticket exista.
