# PROMPT DE DEUDA TÉCNICA - Frontend Radicación

# TD-FE-05 - Caso de Uso de Reinicio del Formulario de Radicación

---

# Contexto Arquitectónico

Esta deuda técnica debe implementarse respetando las decisiones previamente adoptadas:

- TD-FE-01 → Single Source of Truth.
- TD-FE-02 → RadicacionDocumentalContext único.
- FE-05 → Casos de uso para mutaciones.
- FE-06 → Startup Guard responsable del bootstrap.
- FE-07 → El Context sólo cambia mediante operaciones transaccionales exitosas.
- TD-FE-04 → Navegación desacoplada de la UI.

No crear lógica de limpieza distribuida.

No acoplar el formulario al estado documental.

---

# Objetivo

Implementar un mecanismo único, determinístico y completamente testeado para reiniciar el formulario de radicación.

El botón **Limpiar** representa exclusivamente un reinicio del estado de entrada del formulario.

No representa:

- abandonar trámite;
- cerrar contexto documental;
- cancelar gestión;
- enviar a pendiente.

---

# Problema Actual

Actualmente el formulario ejecuta:

```tsx
form.resetFields();

setResetKey(prev => prev + 1);
```

Este comportamiento únicamente reinicia el estado administrado por Ant Design.

No existe una limpieza consistente del resto del estado React asociado al formulario.

---

# Evidencia Actual

Archivo:

```text
src/modules/radicacion/components/RadicacionForm.tsx
```

El botón `Limpiar` ejecuta:

```tsx
onClick={() => {
  form.resetFields();
  setResetKey(prev => prev + 1);
}}
```

El botón `Documentos IA` también ejecuta actualmente el mismo reset parcial:

```tsx
onClick={() => {
  form.resetFields();
  setResetKey(prev => prev + 1);
}}
```

Estados React actuales relacionados con el formulario:

```text
selectedTramiteId
hasUserChangedTramite
resetKey
modalVisible
usuarioSeleccionado
```

Estados internos relevantes en selectores/autocompletes:

```text
searchText
openSelect
tagMenuOpen
clickAutocompleteActive
value
```

Hooks dependientes que deben volver a estado inicial al limpiar el formulario:

```text
useEstructuraRelacionTipoRestriccion(selectedTramiteId, hasUserChangedTramite)
useFlujosRelacionadosTramite(selectedTramiteId, true)
```

---

# Objetivo Arquitectónico

Centralizar completamente el reinicio del formulario.

No deben existir múltiples puntos que intenten limpiar estados parciales.

Todo reinicio debe ejecutarse desde un único caso de uso.

---

# Arquitectura Esperada

Implementar una única entrada para el reinicio.

Opción recomendada:

```text
hooks/

useRadicacionFormReset.ts
```

Exponiendo:

```ts
handleClearRadicacionForm()
```

Toda la lógica de reinicio debe vivir aquí.

---

# Flujo Arquitectónico

```text
Usuario

↓

Botón Limpiar

↓

useRadicacionFormReset()

↓

Reset UI

↓

Formulario listo
```

No debe existir interacción con:

- Context documental;
- Router;
- Backend;
- Pendientes.

---

# Responsabilidades

## Botón Limpiar

Responsable únicamente de invocar el caso de uso.

No contiene lógica de limpieza.

---

## useRadicacionFormReset

Caso de uso responsable de:

- reiniciar el formulario;
- reiniciar estados React;
- reiniciar componentes controlados;
- limpiar validaciones;
- reiniciar autocompletes;
- dejar el formulario listo para una nueva radicación.

---

## RadicacionForm

Únicamente consume el hook.

No implementa lógica de limpieza.

---

# Alcance del Reinicio

Debe restaurar completamente:

```text
form.resetFields()

resetKey

selectedTramiteId

hasUserChangedTramite

flujo

usuario seleccionado

modalVisible

autocompletes

búsquedas internas

validaciones

errores visuales

estado temporal del formulario
```

También debe resetear los estados internos derivados de componentes hijos cuando correspondan:

```text
SelectRemitenteToken
SelectDestinatarioToken
CampoPlantillaAutoCompleteField
CamposPlantillaAutoCompleteRenderer
```

Si componentes hijos mantienen estado interno, deberán exponer mecanismos controlados de reinicio mediante:

- props controladas;
- callbacks;
- resetKey;
- Form Instance.

Al limpiar `selectedTramiteId` y `hasUserChangedTramite`, los hooks dependientes deben volver a su estado inicial sin disparar navegación, backend documental ni cambios de contexto documental.

---

# Restricción Crítica

El reinicio del formulario nunca debe modificar el estado documental.

No limpiar:

```text
RadicacionDocumentalContext

idEstadoRadicado

idTareaWorkflow

estado documental

contador de pendientes

listado de pendientes

startup state
```

El formulario representa únicamente la captura de información.

No representa el ciclo de vida documental.

---

# Integridad del Módulo

Después del reinicio deben mantenerse intactos:

```text
Context documental

Startup Guard

Estado activo

Documentos

Pendientes
```

El módulo debe continuar exactamente en el mismo flujo documental.

---

# Estado Esperado

El formulario debe volver al mismo estado que tendría inmediatamente después de ser montado.

Debe quedar listo para iniciar una nueva captura.

No deben permanecer:

- selecciones;
- búsquedas;
- errores;
- validaciones;
- estados derivados.

---

# Integración

Reemplazar:

```tsx
onClick={() => {

form.resetFields();

setResetKey(...);

}}
```

por:

```tsx
onClick={handleClearRadicacionForm}
```

No duplicar esta lógica en otros botones.

---

# Documentos IA

La semántica debe decidirse explícitamente.

Elegir una única opción:

## Opción A

Mantiene el reinicio actual como deuda técnica documentada.

## Opción B

No reinicia el formulario.

## Opción C

Implementa su propio flujo independiente.

La decisión debe quedar documentada.

Recomendación:

```text
Documentos IA no debe reutilizar handleClearRadicacionForm salvo que producto confirme que su semántica es reiniciar captura.
```

---

# Restricciones

No implementar:

- enviar a pendiente;
- tomar pendiente;
- registro;
- bootstrap;
- navegación;
- backend.

No limpiar el Context.

No modificar rutas.

No disparar APIs de pendientes.

---

# Principios Arquitectónicos

Aplicar:

- Single Responsibility.
- UI Reset ≠ Workflow Reset.
- Smart Hooks / Dumb Components.
- Clean Architecture.
- Backward Compatibility.

---

# Testing

## Unitarios

Validar:

- hook;
- limpieza completa;
- reinicio de estados derivados;
- restablecimiento de `selectedTramiteId` a `null`;
- restablecimiento de `hasUserChangedTramite` a `false`;
- incremento o reinicio del mecanismo `resetKey`.

---

## Integración

Validar:

- botón → hook;
- hook → formulario;
- hook → componentes hijos;
- autocompletes no conservan texto buscado;
- restricciones dependientes de trámite vuelven a estado inicial.

---

## Regresión

Validar:

- Context permanece intacto;
- no llama `clearContextoDocumental`;
- navegación intacta;
- Documentos permanece activo si existía;
- no dispara API de pendientes;
- build;
- lint;
- TypeScript.

---

# Criterios de Aceptación

- Existe un único caso de uso para reiniciar el formulario.
- El botón Limpiar utiliza exclusivamente dicho caso de uso.
- Todo el estado del formulario queda reiniciado.
- `selectedTramiteId` vuelve a `null`.
- `hasUserChangedTramite` vuelve a `false`.
- Los autocompletes no conservan búsquedas internas.
- El Context documental permanece intacto.
- No existen reinicios duplicados.
- No existen efectos secundarios sobre el módulo.
- Hay pruebas unitarias y de integración.

---

# Entregables

1. Lista de archivos modificados.

2. Resumen técnico:

- antes vs después;
- centralización del reinicio;
- separación entre UI Reset y Workflow Reset.

3. Resultado de pruebas.

4. Riesgos residuales.

5. Próximas oportunidades de simplificación.

---

# Instrucción Final

Implementar un caso de uso especializado para el reinicio del formulario de Radicación, encapsulando toda la lógica de limpieza en `useRadicacionFormReset`, garantizando un reinicio completo y determinístico del estado de captura sin modificar el `RadicacionDocumentalContext`, preservando el flujo documental activo y evitando lógica distribuida, estados parciales o regresiones.
