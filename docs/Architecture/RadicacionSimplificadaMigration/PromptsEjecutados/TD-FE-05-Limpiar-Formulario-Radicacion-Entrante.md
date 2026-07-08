# TD-FE-05 - Limpiar Formulario De Radicacion Entrante

## Ticket Asociado

```text
SCRUMCORE-300
```

## Que Hace

Define el caso de uso tecnico para limpiar el formulario de radicacion entrante desde una unica entrada controlada, sin modificar el ciclo de vida documental del tramite activo.

El boton `Limpiar` debe representar solo un reinicio de captura UI. No equivale a abandonar tramite, cerrar contexto documental, enviar a pendiente, tomar pendiente, navegar ni ejecutar backend.

## Casos De Uso Cubiertos

- Reiniciar campos administrados por `Form` de Ant Design.
- Reiniciar estado React local del formulario.
- Reiniciar componentes hijos con estado interno mediante `resetKey` o props controladas.
- Limpiar seleccion de tramite y banderas derivadas.
- Limpiar remitente, destinatario, flujo, asunto, autocompletes y errores visuales.
- Mantener intacto `RadicacionDocumentalContext`.
- Mantener intacto el estado de `Documentos` cuando exista tramite documental activo.
- Evitar que `Documentos IA` reutilice accidentalmente la semantica de limpiar captura.

## Arquitectura Implementada

```text
RadicacionForm
  -> useRadicacionFormReset()
  -> handleClearRadicacionForm()
  -> form.resetFields()
  -> reset local state
  -> reset controlled children
  -> formulario listo para nueva captura
```

El reset debe vivir en un hook de caso de uso:

```text
src/modules/radicacion/hooks/useRadicacionFormReset.ts
```

`RadicacionForm` solo debe consumir `handleClearRadicacionForm`. No debe conservar bloques inline con:

```tsx
form.resetFields();
setResetKey(prev => prev + 1);
```

## Regla Arquitectonica

`UI Reset` no es `Workflow Reset`.

Por lo tanto, la limpieza del formulario nunca debe tocar:

```text
RadicacionDocumentalContext
idEstadoRadicado
idTareaWorkflow
estadoActual
requiereGestionDocumental
tieneTramiteDocumentalActivoEstado0
contador de pendientes
tabla de pendientes
startup state
rutas
```

## Estados Que Deben Reiniciarse

Estados directos actuales de `RadicacionForm`:

```text
selectedTramiteId
hasUserChangedTramite
resetKey
modalVisible
usuarioSeleccionado
```

Estados internos relevantes de hijos:

```text
searchText
openSelect
tagMenuOpen
clickAutocompleteActive
value
```

Hooks dependientes que deben volver a su estado inicial al limpiar:

```text
useEstructuraRelacionTipoRestriccion(selectedTramiteId, hasUserChangedTramite)
useFlujosRelacionadosTramite(selectedTramiteId, true)
```

## Decision Sobre Documentos IA

`Documentos IA` no debe usar `handleClearRadicacionForm` por defecto.

La semantica recomendada para SCRUMCORE-300 es:

```text
Documentos IA no reinicia el formulario hasta que producto confirme que esa accion representa limpiar captura.
```

Si producto confirma otro comportamiento, debe implementarse como flujo propio y no como reutilizacion implicita del boton `Limpiar`.

## Archivos Principales

- `src/modules/radicacion/components/RadicacionForm.tsx`
- `src/modules/radicacion/hooks/useRadicacionFormReset.ts`
- `src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.tsx`
- `src/modules/radicacion/hooks/useEstructuraRelacionTipoRestriccion.ts`
- `src/modules/radicacion/hooks/useFlujosRelacionadosTramite.ts`

## Archivos Modificados

- `src/modules/radicacion/hooks/useRadicacionFormReset.ts`
- `src/modules/radicacion/hooks/useRadicacionFormReset.spec.test.tsx`
- `src/modules/radicacion/components/RadicacionForm.tsx`
- `src/modules/radicacion/components/RadicacionForm.spec.test.tsx`

## Pruebas Asociadas

- `src/modules/radicacion/hooks/useRadicacionFormReset.spec.test.tsx`
- `src/modules/radicacion/components/RadicacionForm.spec.test.tsx`

Escenarios cubiertos:

- `Limpiar` invoca el caso de uso.
- `form.resetFields()` se ejecuta una sola vez.
- `selectedTramiteId` vuelve a `null`.
- `hasUserChangedTramite` vuelve a `false`.
- `resetKey` cambia para remontar hijos no controlados.
- `modalVisible` queda en `false`.
- `usuarioSeleccionado` queda en `null`.
- Autocompletes no conservan texto buscado.
- No llama `clearContextoDocumental`.
- No navega.
- No llama APIs de pendientes.

## Validacion Ejecutada

```bash
npm test -- --run src/modules/radicacion/hooks/useRadicacionFormReset.spec.test.tsx src/modules/radicacion/components/RadicacionForm.spec.test.tsx
```

Resultado:

```text
2 test files passed
33 tests passed
```

```bash
npx eslint src/modules/radicacion/hooks/useRadicacionFormReset.ts src/modules/radicacion/hooks/useRadicacionFormReset.spec.test.tsx
```

Resultado:

```text
OK
```

Nota: el lint completo sobre `RadicacionForm.tsx` sigue reportando deuda preexistente de `any` y `react-hooks/set-state-in-effect`; SCRUMCORE-300 no la corrige para no mezclar fases con TD-FE-03.

## Si Falla, Revisar

- Si campos visuales quedan con datos, revisar que el hijo este controlado o reciba `resetKey`.
- Si restricciones de destinatario siguen aplicadas, revisar que `selectedTramiteId` y `hasUserChangedTramite` se reinicien juntos.
- Si desaparece el panel `Documentos`, revisar que no se haya tocado `RadicacionDocumentalContext`.
- Si se dispara backend, revisar que el hook no invalide queries ni ejecute servicios.
- Si `Documentos IA` limpia el formulario, revisar que no comparta handler con `Limpiar` sin decision explicita.

## Restricciones

- No implementar enviar a pendiente.
- No implementar tomar pendiente.
- No implementar registro.
- No modificar bootstrap.
- No modificar rutas.
- No limpiar contexto documental.
- No invalidar queries de pendientes.
- No duplicar logica de reset en botones o componentes hijos.
