# PROMPT ARQUITECTONICO  Ticket 06 FE
# Modal de Gestion de Documento desde Solicitud de Aprobacion

Rol esperado:
Arquitecto de software senior frontend (React, TypeScript estricto, Design System enterprise, composicion de UI compleja, accesibilidad y reutilizacion de componentes shared)


OBJETIVO

Implementar un modal reusable basado en `AppModal`, disparado desde el boton
`Solicitud de Aprobacion` que esta ubicado dentro del tab **Gestion** de
`GestionRespuesta`, renderizando una interfaz moderna tipo formulario/workbench
fiel al layout descrito en este prompt.


CONTEXTO EXISTENTE

- pagina orquestadora: `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- contenido del primer tab: `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- boton trigger actual en el toolbar interno del tab **Gestion**:
  `Solicitud de Aprobacion`
- componentes shared disponibles:
  - `AppButton`
  - `AppModal`
  - `AppInputSelect`
  - `AppInput` (incluye `type="checkbox"`)
  - `AppInputTags`


ALCANCE

- abrir un `AppModal` desde el boton `Solicitud de Aprobacion`
- construir dentro del modal una UI tipo formulario/workbench
- mantener el comportamiento desacoplado de backend y de logica de negocio
- encapsular el modal como componente propio del modulo

No incluye:

- servicios, API o integracion backend
- validaciones de negocio reales
- persistencia remota
- cambios sobre tabs o workbenches ajenos


UBICACION (OBLIGATORIA)

Nuevo componente:

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/modalGestionDocumento/
```

Archivos sugeridos:

- `GestionDocumentoModal.tsx`
- `GestionDocumentoModal.module.css`


COMPORTAMIENTO GENERAL

- el `AppButton` existente `Solicitud de Aprobacion` debe abrir el modal
- el modal debe ser controlado con `open` y `onClose`
- el componente no debe contener logica de negocio
- solo debe manejar UI y estado local del formulario
- el modal debe poder reutilizarse desde el tab **Gestion** sin acoplamiento al resto del workflow


ESTRUCTURA DEL MODAL (OBLIGATORIA)

El contenido debe construirse sobre `AppModal` con una composicion moderna y clara:

```tsx
<AppModal open={open} onCancel={onClose} title="Gestionar Documento">
  <div className={styles.container}>
    <div className={styles.infoBox}>
      {/* bloque superior de informacion */}
    </div>

    <div className={styles.formGrid}>
      <AppInputSelect ... />
      <AppInput ... type="checkbox" />
      <AppInputTags ... />
    </div>

    <div className={styles.actions}>
      <AppButton variant="secondary">Cancelar</AppButton>
      <AppButton type="primary">Guardar</AppButton>
    </div>
  </div>
</AppModal>
```


REGLAS DE UI (OBLIGATORIAS)

1. Layout

- estructura principal en columna
- spacing consistente entre bloques (`16px` a `24px`)
- grid claro para inputs y controles
- jerarquia visual limpia entre informacion, formulario y acciones

2. InfoBox

Debe ser un bloque visual moderno con:

- fondo suave
- borde sutil
- `border-radius` entre `12px` y `16px`
- padding interno generoso
- tipografia clara y legible

Contenido sugerido:

- descripcion corta del flujo
- metadata o instrucciones de uso
- contexto del documento o de la solicitud

3. Inputs

Debe usar exclusivamente:

- `AppInputSelect`
- `AppInput` con `type="checkbox"`
- `AppInputTags`

Reglas:

- labels visibles
- spacing vertical consistente
- sin mezclar estilos custom que dupliquen el Design System
- grid mantenible para desktop y stack claro en mobile

4. Acciones

- alineadas a la derecha
- separacion clara entre botones
- boton primario visualmente destacado
- accion secundaria para cancelar


ACCESIBILIDAD (OBLIGATORIO)

- `AppModal` debe recibir foco inicial adecuado
- navegacion por teclado funcional dentro del modal
- inputs con nombres accesibles
- botones con labels claros
- cierre consistente por accion explicita y por cancel del modal


REGLAS DE IMPLEMENTACION

- no usar `any`
- no acoplar a servicios ni backend
- no duplicar componentes shared existentes
- no usar estilos globales
- CSS Modules obligatorio
- mantener consistencia con el Design System del proyecto
- el trigger debe quedarse en `GestionRespuestaMainTabContent.tsx`
- el contenido del modal debe vivir en `modalGestionDocumento/`


ESTADO LOCAL SUGERIDO

- `isModalOpen: boolean` en el contenedor del tab
- estado local controlado para:
  - valor de `AppInputSelect`
  - checkbox de control
  - tags del formulario

El objetivo es solo interacción visual básica, no submit real.


CRITERIOS DE ACEPTACION

- el modal se abre desde el boton `Solicitud de Aprobacion`
- el modal usa correctamente `AppModal`
- la UI coincide con una composicion moderna tipo formulario/workbench
- el contenido usa exclusivamente `AppInputSelect`, `AppInput` y `AppInputTags`
- las acciones quedan alineadas a la derecha
- el layout se mantiene limpio, moderno y responsive
- el codigo queda desacoplado y reusable


PRUEBAS OBLIGATORIAS

- abre el modal al hacer click en `Solicitud de Aprobacion`
- renderiza todos los componentes internos esperados
- los inputs interactuan correctamente con estado local
- el boton cerrar/cancelar funciona
- el layout no se rompe en responsive


PRUEBAS MINIMAS SUGERIDAS

- validar apertura y cierre del modal
- validar presencia de:
  - `AppInputSelect`
  - `AppInput` tipo checkbox
  - `AppInputTags`
  - botones `Cancelar` y `Guardar`
- validar que el modal sigue siendo controlado por props
- validar que no hay logica de negocio embebida en el componente
