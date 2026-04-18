# TICKET FE
# =========================================

## Titulo
Agregar boton `Guardar` con estado visual dirty/save-ready en `AppEditor`, preparado para futura persistencia en backend

---

## Rol

Desarrollador Frontend Senior especializado en:

- React 19 + TypeScript estricto
- UX de formularios y editores enriquecidos
- Manejo de estado controlado/no controlado
- Clean Architecture
- Testing con Vitest + Testing Library

---

## Objetivo

Implementar una accion visual de `Guardar` asociada al uso de `AppEditor`, de forma que el usuario pueda identificar claramente cuando existen cambios pendientes por guardar.

En esta fase NO se debe persistir nada en base de datos ni en API real.

El objetivo es dejar lista la experiencia de estado y la estructura tecnica para conectar backend mas adelante sin rehacer la solucion.

---

## Contexto obligatorio

Repo:
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react`

Ubicacion de documentacion de tickets (OBLIGATORIO):
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react\docs\Architecture\AppEditor`

`AppEditor` ya soporta:

- edicion enriquecida basada en HTML serializado
- modo controlled/uncontrolled
- `headerActions`
- toolbar propia del editor
- paginacion visual multi-hoja
- zoom visual
- contador de pagina
- imagenes
- `PageBreak`

Archivos base relevantes:

- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
- `src/app/Components/UI/AppEditor/domain/editor.types.ts`
- `src/app/Components/UI/AppEditor/application/useAppEditor.ts`
- `src/app/Components/UI/AppEditor/AppEditor.module.css`

---

## Problema actual

Hoy el editor permite modificar contenido, pero no existe una señal clara de estado de guardado.

El usuario necesita que exista un boton `Guardar` que:

- este gris cuando no hay cambios pendientes
- se vuelva negro cuando el contenido cambie respecto al ultimo estado guardado
- vuelva a gris cuando se ejecute el guardado simulado
- vuelva a negro si luego modifica nuevamente el contenido

En esta fase el guardado sera solo local/simulado, pero la solucion debe quedar preparada para futura integracion con backend.

---

## Alcance exacto

### 1. Boton `Guardar`

Agregar un boton visible de `Guardar`.

Ubicacion obligatoria:

- `headerActions` del AppEditor, o
- contenedor inmediato del editor (shell del modulo)

NO debe ubicarse dentro de la toolbar de formato.

---

### 2. Regla visual obligatoria

El boton debe comportarse asi:

- gris:
  - cuando no hay cambios pendientes
  - cuando el valor actual coincide con el ultimo valor guardado

- negro:
  - cuando existen cambios pendientes por guardar
  - cuando el valor actual difiere del ultimo valor guardado

---

### 3. Guardado simulado

Al hacer clic en `Guardar`:

- NO llamar backend
- NO llamar API
- NO usar base de datos

Debe:

- actualizar el baseline (`savedValue`)
- cambiar estado a no dirty
- mantener consistencia con futuras ediciones

---

## Ubicacion obligatoria del estado de guardado

La logica de `dirty state` NO debe vivir dentro de `AppEditor`.

Debe implementarse en:

- el contenedor consumidor del editor, o
- un hook en capa application del modulo consumidor

`AppEditor` debe permanecer completamente agnostico a la persistencia.

---

## Fuente de verdad del contenido

El contenedor debe gestionar:

- `currentValue`
- `savedValue`

Provenientes de:

- `value` + `onChange` (modo controlled), o
- estado interno del contenedor

---

## Regla tecnica principal (dirty state real)

```ts
isDirty = normalize(currentValue) !== normalize(savedValue)
```

NO usar logica heuristica.

---

## Normalizacion obligatoria del HTML

Debe implementarse una funcion de normalizacion compartida.

Debe cubrir al menos:

- trim de strings
- eliminacion de HTML vacio equivalente
- normalizacion de contenido vacio

Ejemplos equivalentes:

- ""
- "<p></p>"
- "<p><br></p>"

Todos deben considerarse iguales.

---

## Sincronizacion con cambios externos (CRITICO)

Si `value` cambia externamente (ej: carga desde backend):

- actualizar `savedValue`
- resetear estado dirty a false

Evita falsos positivos tras carga inicial o refresh.

---

## API / estructura esperada

```ts
type SaveStatus = "idle" | "dirty";
```

Extension futura prevista:

```ts
saveDraft?: (html: string) => Promise<void>;
```

(No implementar aun)

---

## Reglas arquitectonicas

- domain:
  - definir tipos solo si aportan valor

- application:
  - logica de comparacion
  - normalizacion
  - manejo de baseline

- presentation:
  - boton Guardar
  - estados visuales

- infrastructure:
  - NO requerido en este ticket

---

## Compatibilidad obligatoria

NO debe romper:

- AppEditor actual
- modo controlled/uncontrolled
- toolbar
- paginacion
- zoom
- contador
- imagenes
- PageBreak
- HTML serializado
- onChange

---

## Resultado esperado

- boton Guardar visible
- gris sin cambios
- negro con cambios
- vuelve a gris al guardar
- vuelve a negro al editar de nuevo
- arquitectura lista para backend

---

## Validaciones obligatorias

1. estado inicial gris
2. cambia a negro al editar
3. vuelve a gris al guardar
4. vuelve a negro tras nueva edicion
5. comparacion robusta
6. sin falsos positivos por HTML vacio
7. no rompe editor
8. no rompe onChange
9. desacoplado de backend

---

## Pruebas esperadas

- estado inicial
- cambio a dirty
- guardado simulado
- re-edicion
- normalizacion de HTML vacio
- regresion completa del editor

---

## Pruebas de regresion obligatorias

Ejecutar:

- tests del editor
- validacion TypeScript

Errores previos deben reportarse como preexistentes.

---

## Restricciones

- NO backend
- NO API
- NO DB
- NO toolbar de formato
- NO logica heuristica
- NO romper API publica

---

## Instruccion final

Implementar la experiencia de Guardar con dirty state real para `AppEditor`, asegurando:

- comparacion robusta basada en contenido
- normalizacion consistente
- separacion clara de responsabilidades
- preparacion real para backend futuro

La solucion debe ser estable, desacoplada y escalable.
