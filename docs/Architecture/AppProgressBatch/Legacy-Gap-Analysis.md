# Auditoria de brechas legacy vs requisitos

## Objetivo

Registrar los comportamientos detectados en `JSProgresBar.legacy.js` que no estaban cubiertos o estaban cubiertos de forma incompleta en el modelo inicial de requisitos.

## Brechas detectadas y decision

### 1. Lista vacia

El legacy no inicia si no hay items.

Evidencia:

```txt
this.settings.OptionItemSelect.length == 0
```

Decision:

- Agregar `emptyMessage`.
- No llamar `processItem` cuando no hay items.

Cubierto en:

- `RF-PB-02A`

### 2. Advertencias no bloqueantes

El legacy soporta resultado `CTRL`, que cuenta un elemento no procesado y continua.

Decision:

- Agregar resultado `warning`.
- Incluir conteo de advertencias en el resumen.

Cubierto en:

- `RF-PB-07A`

### 3. Items omitidos

El legacy lleva conteos de elementos no procesados.

Evidencia:

```txt
NumeroElmentNoProcess
ErrorElmentNoProcess
```

Decision:

- Agregar resultado `skipped`.
- Incluir omitidos en resumen.

Cubierto en:

- `RF-PB-07B`

### 4. Ciclo de vida explicito

El legacy usa `estado_control` para cancelar, ejecutar o pausar.

Decision:

- Sustituir numeros por estados nominales: `idle`, `running`, `paused`, `cancelling`, `completed`, `error`.

Cubierto en:

- `RF-PB-11`

### 5. Bloqueo de cierre durante ejecucion

El legacy fuerza decisiones de cancelar/continuar antes de cerrar el proceso.

Decision:

- El cierre durante ejecucion debe pasar por la misma politica de cancelacion.

Cubierto en:

- `RF-PB-12`

### 6. Etiqueta de fase

El legacy actualiza etiquetas de proceso y elemento actual.

Decision:

- Permitir que `processItem` actualice la fase visible.

Cubierto en:

- `RF-PB-13`

## Brechas descartadas intencionalmente

### `name_service`

El legacy decide internamente que funcion ejecutar segun `name_service`.

Decision:

- No migrar.
- Reemplazar por inyeccion de `processItem`.

### Funciones globales

El legacy llama funciones globales como `LoadStampFile`, `ElimnaRegistroDcoumento` o servicios REST globales.

Decision:

- No migrar.
- El consumidor debe encapsular esas operaciones fuera de `AppProgressBatch`.

### HTML Bootstrap manual

El legacy construye modales con strings HTML.

Decision:

- No migrar.
- Usar componentes React accesibles.

