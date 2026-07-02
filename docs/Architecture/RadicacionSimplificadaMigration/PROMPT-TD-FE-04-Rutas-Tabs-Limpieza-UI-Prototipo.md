# PROMPT DE DEUDA TECNICA - Frontend Radicacion
# TD-FE-04 - Rutas, tabs y limpieza de UI prototipo

## Objetivo

Eliminar deuda de prototipo en navegacion y UI del modulo `radicacion`, preparando el modulo para flujo contextual real.

## Problemas Detectados

```txt
RadicacionTabs usa keys "1", "2", "3", "4".
onChange hace console.log.
Tabs no sincronizan con ruta.
No existe ruta hija /registro/:idEstadoRadicado/documentos.
CapDocument contiene datos mock visibles.
Modalpendiente contiene datos mock visibles.
```

## Alcance

### Tabs

Cambiar keys a nombres de dominio:

```txt
ia
radicacion
documentos
gestion-radicados
```

Eliminar:

```txt
console.log(key)
```

Preparar tabs para ser controlados por contexto/ruta.

### Rutas

Preparar estructura:

```txt
/dashboard/radicacion
/dashboard/radicacion/registro/:idEstadoRadicado
/dashboard/radicacion/registro/:idEstadoRadicado/documentos
```

Si no se implementa routing completo en este corte, dejar adapters/constantes de ruta para no hardcodear strings en varios componentes.

### UI Prototipo

`CapDocument`:

- no debe mostrar datos falsos como si fueran reales;
- debe mostrar placeholder operativo condicionado por contexto;
- debe quedar listo para FE-03/documentos reales.

`Modalpendiente`:

- si FE-05 no se implementa en este mismo corte, marcarlo claramente como pendiente tecnico;
- no dejar datos mock confundibles con datos reales en entorno productivo.

## Criterios de Aceptacion

- No hay `console.log` en runtime del modulo.
- Tabs usan keys semanticas.
- Las rutas objetivo estan centralizadas o implementadas.
- `CapDocument` no muestra datos falsos de usuario/documentos.
- `Modalpendiente` no muestra datos mock como informacion real, salvo bajo fixture/test explicito.
- Tests cubren keys semanticas y bloqueo/no disponibilidad de Documentos.

## Fuera de Alcance

- AppTable real de pendientes;
- carga documental real;
- digitalizacion;
- registro backend.
