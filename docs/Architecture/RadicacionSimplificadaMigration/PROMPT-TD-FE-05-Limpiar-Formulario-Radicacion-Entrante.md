# PROMPT DE DEUDA TECNICA - Frontend Radicacion
# TD-FE-05 - Limpiar formulario de radicacion entrante

## Objetivo

Implementar una limpieza completa, deterministica y testeada de los datos de radicacion entrante.

El boton `Limpiar` no debe limitarse a:

```ts
form.resetFields();
setResetKey(prev => prev + 1);
```

Debe restaurar el formulario al estado inicial operativo para una nueva radicacion, sin afectar el contexto documental activo del modulo.

## Evidencia Actual

Archivo:

```txt
src/modules/radicacion/components/RadicacionForm.tsx
```

Estado actual:

```tsx
<Button
  icon={<DeleteFilled />}
  className={styles.btnClear}
  onClick={() => {
    form.resetFields();
    setResetKey(prev => prev + 1);
  }}
>
  Limpiar
</Button>
```

Problema:

```txt
resetFields limpia campos AntD, pero no necesariamente limpia todo el estado React asociado al formulario.
```

## Alcance

Crear una funcion central:

```ts
function handleClearRadicacionForm(): void
```

O un hook:

```txt
src/modules/radicacion/hooks/useRadicacionFormReset.ts
```

La limpieza debe cubrir:

```txt
form.resetFields()
resetKey
selectedTramiteId
hasUserChangedTramite
flujo seleccionado
modalVisible
usuarioSeleccionado
estado local de autocompletes especializados
busquedas internas de remitente/destinatario
validaciones visibles del formulario
errores transitorios de formulario
```

Si algun estado vive dentro de componentes hijos, debe exponerse un mecanismo controlado:

```txt
resetKey
controlled value
callback de reset
form instance
```

No dejar resets parciales dispersos en varios `onClick`.

## Regla Critica

Limpiar formulario de radicacion entrante NO debe limpiar:

```txt
RadicacionDocumentalContext activo
idEstadoRadicado activo estado = 0
idTareaWorkflow activo
contexto documental restaurado por estado-activo
contador/listado de pendientes
```

Razon:

```txt
Limpiar es una accion del formulario de entrada.
No es abandonar tramite documental, no es enviar a pendiente y no es cerrar contexto activo.
```

Para limpiar contexto documental debe existir una accion funcional distinta:

```txt
Enviar a pendiente
Finalizar/cerrar tramite documental
Cancelar contexto activo con confirmacion explicita, si el negocio lo aprueba
```

## Estado Inicial Esperado

Despues de `Limpiar`:

```txt
campos visibles vuelven a valores iniciales/default;
tramite queda sin seleccionar;
flujo queda sin seleccionar;
remitente queda sin seleccionar;
destinatario queda sin seleccionar;
asunto queda vacio;
anexos queda vacio/default;
autocompletes no conservan texto buscado;
restricciones dependientes de tramite quedan en default;
errores visuales desaparecen;
el formulario queda listo para una nueva radicacion.
```

## Integracion con RadicacionForm

`RadicacionForm` debe pasar de:

```tsx
onClick={() => {
  form.resetFields();
  setResetKey(prev => prev + 1);
}}
```

a:

```tsx
onClick={handleClearRadicacionForm}
```

La misma funcion puede usarse para otros comandos de reinicio, pero no debe mezclarse con `Documentos IA` si ese boton tiene semantica distinta.

Si `Documentos IA` actualmente usa el mismo reset, decidir explicitamente:

```txt
opcion A: Documentos IA conserva reset temporal, documentado como deuda;
opcion B: Documentos IA deja de limpiar formulario;
opcion C: Documentos IA llama a un flujo propio.
```

No ocultar esta decision.

## Pruebas Requeridas

Actualizar:

```txt
src/modules/radicacion/components/RadicacionForm.spec.test.tsx
```

Crear si aplica:

```txt
src/modules/radicacion/hooks/useRadicacionFormReset.spec.test.ts
```

Casos minimos:

- limpia campos simples;
- limpia `tramite`;
- limpia `flujo`;
- limpia `remitente`;
- limpia `destinatario`;
- limpia `asunto`;
- incrementa/resetear mecanismo de remount controlado para autocompletes;
- restablece `selectedTramiteId` a `null`;
- restablece `hasUserChangedTramite` a `false`;
- no llama `clearContextoDocumental`;
- no modifica contexto documental activo;
- no dispara API de pendientes;
- no navega a otra ruta;
- no deja errores de validacion visibles.

## Criterios de Aceptacion

- Existe una unica funcion/hook responsable de limpiar formulario de radicacion entrante.
- El boton `Limpiar` usa esa funcion.
- La limpieza es completa para estados locales del formulario.
- El contexto documental activo no se borra.
- No hay resets duplicados dispersos para el mismo comportamiento.
- Tests cubren limpieza completa y preservacion de contexto documental.

## Fuera de Alcance

No implementar aqui:

- enviar a pendiente;
- tomar pendiente;
- registrar radicacion;
- restaurar estado-activo;
- cambiar diseno visual;
- refactor completo de `RadicacionForm`.
