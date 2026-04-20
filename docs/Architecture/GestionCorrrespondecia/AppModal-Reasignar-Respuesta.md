# [FE] Modal Reasignar Respuesta (AppModal) - Gestion Correspondencia

## Objetivo

Definir la arquitectura de un modal **desacoplado** para "Reasignar Respuesta", construido con el Design System del proyecto (`AppModal`, `AppInputTags`, `AppButton`), invocado desde la opcion **"Reasignar Trámite"** dentro del dropdown de acciones de la tabla del modulo **Gestion Correspondencia**.

Este documento describe **estructura, contratos, UI responsive, accesibilidad y pruebas**. No implementa codigo productivo.

## Alcance

- Nuevo componente UI `ReasignarRespuestaModal` y su CSS Module.
- Integracion desde la accion del dropdown "Reasignar Trámite" para abrir/cerrar el modal.
- Sin logica de negocio: no llamadas API, no mutaciones, no side effects del dominio.
- Pruebas unitarias/UI (Vitest + Testing Library) para comportamiento observable.

## Fuera de alcance

- No modificar la tabla (render, columnas, paging, query, selection).
- No implementar reasignacion real (API, validaciones backend, auditoria).
- No modificar `AppModal` ni estilos globales.

## Ubicacion obligatoria (codigo)

Carpeta:
- `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/`

Archivos:
- `ReasignarRespuestaModal.tsx`
- `ReasignarRespuestaModal.module.css`
- `ReasignarRespuestaModal.test.tsx`
- `index.ts` (opcional)

## Integracion (invocacion) desde el dropdown

Regla: **no tocar la tabla**.

La integracion se hace agregando un contenedor/bridge en el modulo (page/layout/container) que:
- Mantiene el estado `open` del modal.
- Expone un handler para la opcion del dropdown: `onReasignarTramite(...)`.

### Contrato minimo propuesto (bridge)

```ts
export type ReasignarRespuestaContext = {
  radicado: string;
  notaPreview: string;
};
```

## API del componente (presentacional/controlado)

```ts
export type ReasignarRespuestaModalProps = {
  open: boolean;
  onClose: () => void;
  radicado: string;
  nota: string;
  users: string[];
  onAddUser: (value: string) => void;
  onRemoveUser: (value: string) => void;
  onRemoveAllUsers: () => void;
  onSubmit: () => void;
};
```

## Estructura UI (layout)

```tsx
<AppModal open={open} onClose={onClose} title={null} centered hideFooter>
  <div className={styles.container}>
    <header className={styles.header}>
      <div className={styles.title}>
        <Icon aria-hidden="true" />
        <span id={titleId}>Reasignar Respuesta</span>
      </div>
      <div className={styles.meta}>RAD. {radicado}</div>
    </header>

    <AppInputTags
      label="Responsable"
      value={users}
      placeholder="Seleccionar responsable"
      onAddTag={onAddUser}
      onRemoveTag={onRemoveUser}
      onRemoveAll={onRemoveAllUsers}
    />

    <section className={styles.noteSection} aria-label="Nota">
      <div className={styles.noteHeader}>
        <span className={styles.noteLabel}>Nota</span>
        <span className={styles.noteDivider} aria-hidden="true" />
      </div>
      <div className={styles.noteBox}>{nota}</div>
    </section>

    <footer className={styles.actions} aria-label="Acciones">
      <AppButton variant="secondary" onClick={onClose}>Cancelar</AppButton>
      <AppButton onClick={onSubmit}>Enviar</AppButton>
    </footer>
  </div>
</AppModal>
```

## Responsive

- Desktop: modal centrado (`min(720px, 92vw)`).
- Mobile: layout compacto, botones full width (columna).
- Altura: `max-height` basado en `100svh` + safe-area y **scroll interno** si crece contenido.
- Evitar scroll del overlay/wrapper en mobile usando `wrapClassName` + CSS Module del modal.

## Accesibilidad

- Focus inicial al abrir (input interno de `AppInputTags` o boton primario).
- Teclado: `Escape` cierra.
- Titulo ligado con `aria-labelledby`.
- Icono decorativo con `aria-hidden="true"`.

## Pruebas (minimas)

- Apertura/cierre del modal.
- Render y eliminacion de tags (callbacks).
- Render de nota.
- Botones "Cancelar" / "Enviar" visibles y funcionales.

