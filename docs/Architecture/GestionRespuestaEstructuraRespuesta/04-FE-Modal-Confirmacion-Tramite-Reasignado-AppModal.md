# SCRUMCORE-145
# Modal de confirmacion "Tramite Reasignado" con `AppModal`

## Objetivo

Implementar un modal de confirmacion reutilizable y desacoplado en el modulo de gestion de correspondencia para confirmar la reasignacion exitosa de un tramite, usando componentes shared del Design System:

- `AppModal`
- `AppButton`

El modal se muestra despues de validar el formulario de reasignacion y ejecutar la accion de envio en:

- `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.tsx`


## Alcance

### Incluye

- nuevo componente de confirmacion visual `TramiteReasignadoModal`
- estilos locales con CSS Modules
- integracion de apertura/cierre desde `ReasignarRespuestaModal`
- soporte responsive (desktop, tablet, mobile)
- lineamientos de accesibilidad y foco inicial
- pruebas unitarias de render, accion y contenido

### No incluye

- cambios de logica de negocio backend
- cambios en contratos API
- cambios en componentes shared (`AppModal`, `AppButton`)
- estilos globales


## Ubicacion obligatoria

```txt
src/modules/gestionCorrespondencia/components/modalTramiteReasignado/
```

Archivos objetivo:

- `TramiteReasignadoModal.tsx`
- `TramiteReasignadoModal.module.css`


## Integracion funcional esperada

### Flujo UI

1. Usuario abre `ReasignarRespuestaModal`.
2. Usuario diligencia formulario.
3. Formulario valida correctamente.
4. Usuario pulsa `Enviar`.
5. Se cierra el modal de reasignacion.
6. Se abre `TramiteReasignadoModal` con datos de confirmacion:
   - `usuarioAsignado`
   - `radicado`
7. Usuario pulsa `Aceptar` o cierra por `X`.
8. Se cierra modal de confirmacion.

### Reglas de estado recomendadas

- `isReasignarOpen: boolean`
- `isTramiteReasignadoOpen: boolean`
- `confirmacionPayload: { usuarioAsignado: string; radicado: string } | null`

No mezclar estados de ambos modales en una sola bandera.


## Contrato del componente

```ts
type TramiteReasignadoModalProps = {
  open: boolean;
  usuarioAsignado: string;
  radicado: string;
  onClose: () => void;
};
```

Reglas:

- no usar `any`
- props estrictas y explicitas
- contenido desacoplado del origen de datos


## Estructura de render objetivo

```tsx
<AppModal open={open} onCancel={onClose} title={null}>
  <div className={styles.container}>
    <div className={styles.header}>
      <span className={styles.title}>
        Tramite Reasignado
        <IconSuccess />
      </span>
    </div>

    <div className={styles.content}>
      <p>
        <strong>Usuario Asignado:</strong> {usuarioAsignado}
      </p>
      <p>
        <strong>Radicado:</strong> {radicado}
      </p>
    </div>

    <div className={styles.actions}>
      <AppButton variant="primary" onClick={onClose}>
        Aceptar
      </AppButton>
    </div>
  </div>
</AppModal>
```


## Reglas visuales obligatorias

### Header

- titulo centrado: `Tramite Reasignado`
- icono de exito alineado junto al titulo
- boton de cierre (`X`) en esquina superior derecha via comportamiento de `AppModal`
- separador inferior sutil

### Contenido

- texto centrado
- dos lineas de informacion
- etiquetas con `<strong>`
- espaciado vertical limpio y consistente

### Accion principal

- `AppButton` variante primaria
- texto: `Aceptar`
- centrado horizontal
- prominente visualmente, sin saturar el modal


## Responsive obligatorio

### Desktop

- modal centrado
- ancho recomendado: `400px` a `500px`
- contenido centrado

### Tablet

- mantener centrado
- reducir paddings horizontales e internos

### Mobile

- ancho del modal: `90%` a `95%`
- boton `Aceptar` en ancho completo (`full width`)
- tipografia legible
- evitar overflow horizontal


## Accesibilidad obligatoria

- foco inicial automatico en boton `Aceptar`
- navegacion por teclado funcional
- cierre accesible via boton `X` y `Esc` (si `AppModal` ya lo soporta)
- textos y labels claros
- mantener semantica y roles del modal provistos por `AppModal`


## Restricciones tecnicas

- no usar `any`
- no agregar logica de negocio
- no usar estilos globales
- no modificar `AppModal`
- CSS Modules obligatorio
- mantener consistencia con Design System del repo


## Estrategia de implementacion recomendada

1. Crear carpeta `modalTramiteReasignado`.
2. Implementar `TramiteReasignadoModal.tsx` con contrato tipado.
3. Implementar `TramiteReasignadoModal.module.css` con layout centrado y responsive.
4. Integrar en `ReasignarRespuestaModal.tsx` el flujo de apertura tras submit valido.
5. En submit exitoso, inyectar `usuarioAsignado` y `radicado` al modal de confirmacion.
6. Asegurar que `Aceptar` cierre solo el modal de confirmacion.
7. Verificar foco inicial y navegacion de teclado.


## Pruebas obligatorias

- renderiza modal cuando `open=true`
- boton `Aceptar` ejecuta `onClose`
- muestra `Usuario Asignado` y `Radicado` con sus valores
- layout no se rompe en viewport mobile

Archivos de prueba sugeridos:

- `src/modules/gestionCorrespondencia/components/modalTramiteReasignado/TramiteReasignadoModal.test.tsx`
- pruebas de integracion en:
  `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.test.tsx`


## Criterios de aceptacion

- UI consistente con el mock de confirmacion solicitado
- uso correcto de `AppModal` y `AppButton`
- experiencia limpia, centrada y profesional
- responsive sin desbordes en mobile
- codigo desacoplado, reusable y sin deuda tecnica innecesaria
