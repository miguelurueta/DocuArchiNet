# PROMPT ARQUITECTONICO - Radicacion Simplificada
# Fase FE-07 - Enviar tramite activo a pendiente desde la interfaz

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend senior especialista en:

- React 19 y TypeScript estricto;
- UX transaccional;
- mutaciones REST tipadas;
- control de estados documentales;
- integracion con contexto de modulo;
- pruebas de componentes y hooks.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar la accion de interfaz para enviar un tramite documental activo a pendiente.

Transicion:

```txt
estado 0 -> estado 1
```

API backend:

```txt
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
```

Esta accion solo aplica cuando el tramite esta activo para gestion documental.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-04-Enviar-Radicado-Pendiente.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-03-Contador-Pendientes-Radicacion.md
```

Frontend actual:

```txt
src/modules/radicacion/components/RadicacionForm.tsx
src/modules/radicacion/hooks/RadicacionTabs.tsx
src/modules/radicacion/components/Modalpendiente.tsx
```

El boton existe en `RadicacionForm.tsx`, pero no debe quedar como accion global siempre disponible.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLA FUNCIONAL CENTRAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

```txt
Enviar a pendiente solo aparece o se habilita si hay tramite documental activo en estado = 0.
```

Condicion:

```ts
const puedeEnviarAPendiente =
  requiereGestionDocumental === true &&
  tieneTramiteDocumentalActivoEstado0 === true &&
  estadoActual === 0 &&
  idEstadoRadicado > 0;
```

Si no cumple:

- no mostrar el boton, o
- mostrarlo deshabilitado si el diseno exige consistencia visual.

Recomendacion:

```txt
Ocultarlo cuando no aplica.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO BACKEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Request:

```ts
type EnviarRadicadoPendienteRequestDto = {
  motivo?: string;
};
```

Response:

```ts
type EnviarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  consecutivoRadicado?: string;
  estadoAnterior: 0;
  estadoActual: 1;
  tieneTramiteDocumentalActivoEstado0: false;
  destinoPostRegistro: "resumen";
  mensaje: string;
};
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ARQUITECTURA FRONTEND OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear/extender:

```txt
src/modules/radicacion/services/radicacionPendientes.service.ts
src/modules/radicacion/hooks/useEnviarRadicadoPendiente.ts
src/modules/radicacion/components/EnviarPendienteConfirmModal.tsx
src/modules/radicacion/context/RadicacionDocumentalContext.tsx
```

Si ya existe modal de confirmacion compartido, reutilizarlo.

No crear logica de mutation directamente dentro de `RadicacionForm.tsx`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FLUJO UI
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

```txt
1. Usuario tiene tramite activo estado = 0.
2. UI muestra accion Enviar a pendiente.
3. Usuario pulsa accion.
4. UI abre confirmacion.
5. Usuario confirma.
6. Frontend llama POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente.
7. Backend responde estadoActual = 1.
8. Frontend limpia contexto documental activo.
9. Frontend desactiva Documentos.
10. Frontend refresca contador/lista de pendientes.
11. Frontend navega a Resumen o ruta base de radicacion.
```

No cerrar/limpiar contexto antes de la respuesta exitosa.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CAMBIOS DE INTERFAZ
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

En estado activo `0`:

- mostrar accion `Enviar a pendiente`;
- mantener `Documentos` activo;
- bloquear tomar otro pendiente.

Despues de enviar a pendiente:

- ocultar/deshabilitar `Enviar a pendiente`;
- desactivar `Documentos`;
- limpiar `idEstadoRadicado` activo;
- refrescar contador de pendientes;
- mostrar mensaje de exito;
- dejar el modulo en `Resumen` o pantalla base.

En estado pendiente `1`:

- no permitir `Documentos`;
- el tramite solo aparece en modal/listado de pendientes;
- se reactiva con accion `asignacion-tarea` de FE-05.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ESTADOS UI
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Manejar:

- boton disponible;
- confirmacion abierta;
- enviando;
- exito;
- error validacion backend;
- error red;
- estado inconsistente;
- reintento.

Mientras `enviando`:

- bloquear doble click;
- conservar contexto visible;
- no desactivar `Documentos` hasta exito.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INTEGRACION CON CONTADOR Y MODAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Tras exito:

```txt
GET /api/radicacion/pendientes/contador
```

Debe refrescarse si el contador esta montado.

Si el modal de pendientes esta abierto:

- refrescar listado;
- el radicado enviado debe aparecer como `estado = 1`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS REQUERIDAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear/actualizar:

```txt
src/modules/radicacion/hooks/useEnviarRadicadoPendiente.spec.test.ts
src/modules/radicacion/components/EnviarPendienteConfirmModal.spec.test.tsx
src/modules/radicacion/components/RadicacionForm.spec.test.tsx
src/modules/radicacion/context/RadicacionDocumentalContext.spec.test.tsx
```

Casos:

- no muestra accion si no hay `estado = 0`;
- muestra accion si hay `estado = 0`;
- abre confirmacion;
- cancelar no llama API;
- confirmar llama `enviar-pendiente`;
- durante envio bloquea doble click;
- exito `estadoActual = 1` limpia contexto activo;
- exito desactiva `Documentos`;
- exito refresca contador;
- error backend conserva contexto activo;
- error no desactiva `Documentos`;
- response inesperada sin `estadoActual = 1` no limpia contexto.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- `Enviar a pendiente` solo aplica para tramite documental activo `estado = 0`.
- La accion usa hook/service tipado.
- La accion confirma antes de mutar.
- La accion llama `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`.
- El contexto documental se limpia solo con respuesta exitosa `estadoActual = 1`.
- `Documentos` queda inactivo tras enviar a pendiente.
- El contador/listado de pendientes se refresca.
- No se permite enviar a pendiente desde un estado distinto de `0`.
- Hay pruebas de visibilidad, confirmacion, exito y error.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar aqui:

- listado AppTable de pendientes;
- tomar pendiente;
- API backend;
- upload;
- digitalizacion;
- visor documental.
