# Casos De Uso Frontend

## UC-FE-01 - Iniciar Modulo Con Tramite Activo

| Campo | Detalle |
|---|---|
| Actor | Usuario radicador |
| Objetivo | Continuar la gestion documental abandonada o no terminada. |
| Precondicion | Backend retorna activo `estado = 0`. |
| Disparador | Usuario entra a `/dashboard/radicacion`. |
| Resultado | El modulo entra directamente a `Documentos`. |

Flujo:

1. El frontend carga plantilla.
2. Consulta `GET /api/radicacion/pendientes/estado-activo`.
3. Backend retorna contexto activo.
4. Frontend guarda contexto documental.
5. El modulo navega a documentos.

Criterio: `CapDocument` solo se renderiza despues de que el guard valida `estado = 0`.

## UC-FE-02 - Iniciar Modulo Sin Tramite Activo

| Campo | Detalle |
|---|---|
| Actor | Usuario radicador |
| Objetivo | Crear una nueva radicacion entrante. |
| Precondicion | Backend no encuentra activo `estado = 0`. |
| Disparador | Usuario entra al modulo. |
| Resultado | Formulario disponible y `Documentos` inactivo. |

Flujo:

1. Frontend consulta estado activo.
2. Backend retorna sin activo.
3. Frontend no crea contexto documental.
4. Se muestra formulario de radicacion.
5. Tab `Documentos` queda disabled.

## UC-FE-03 - Radicar Y Activar Gestion Documental

| Campo | Detalle |
|---|---|
| Actor | Usuario radicador |
| Objetivo | Registrar radicacion y continuar con documentos si aplica. |
| Precondicion | Formulario valido y plantilla cargada. |
| Disparador | Usuario presiona `Radicar`. |
| Resultado | Si backend retorna gestion documental activa, `Documentos` se habilita. |

Flujo:

1. Usuario diligencia formulario.
2. Frontend arma DTO con mapper tipado.
3. Llama registro moderno de radicacion entrante.
4. Recibe `idEstadoRadicado`, `consecutivoRadicado` y metadata.
5. Si `estado = 0`, guarda contexto y navega a documentos.

## UC-FE-04 - Tomar Pendiente Desde Lista

| Campo | Detalle |
|---|---|
| Actor | Usuario radicador |
| Objetivo | Tomar un radicado pendiente por documentacion. |
| Precondicion | Hay pendientes en `estado = 1` y el usuario no tiene activo `estado = 0`. |
| Disparador | Accion `asignacion-tarea` en `AppTable`. |
| Resultado | Pendiente pasa a `estado = 0` y se habilita `Documentos`. |

Flujo:

1. Usuario abre modal de pendientes.
2. `AppTable` carga lista desde backend.
3. Usuario ejecuta accion de asignacion.
4. Frontend llama `tomar`.
5. Backend valida bloqueo de activo y responde contexto.
6. Frontend guarda contexto y navega a documentos.

## UC-FE-05 - Enviar Tramite Activo A Pendiente

| Campo | Detalle |
|---|---|
| Actor | Usuario radicador |
| Objetivo | Dejar un tramite activo como pendiente para retomarlo luego. |
| Precondicion | Existe contexto activo `estado = 0`. |
| Disparador | Usuario presiona `Enviar a Pendientes`. |
| Resultado | El tramite queda en `estado = 1`, contexto limpio y documentos inactivo. |

Flujo:

1. UI valida que existe `idEstadoRadicado`.
2. Usuario confirma la accion si el diseno lo requiere.
3. Frontend llama `enviar-pendiente`.
4. Backend retorna `estadoActual = 1`.
5. Frontend limpia contexto documental.
6. Tab `Documentos` queda inactivo.

## UC-FE-06 - Limpiar Formulario De Radicacion Entrante

| Campo | Detalle |
|---|---|
| Actor | Usuario radicador |
| Objetivo | Vaciar datos de captura para iniciar otra radicacion. |
| Precondicion | Formulario montado. |
| Disparador | Usuario presiona `Limpiar`. |
| Resultado | Campos limpios sin borrar contexto documental activo. |

Flujo:

1. Frontend ejecuta `form.resetFields()`.
2. Limpia estados locales de autocompletes, tramite, flujo, destinatario, remitente y asunto.
3. Reinicia renderers dinamicos si aplica.
4. No llama `limpiarContextoDocumental`.

## UC-FE-07 - Bloquear Documentos Para Consulta

| Campo | Detalle |
|---|---|
| Actor | Usuario radicador |
| Objetivo | Evitar carga documental sin tramite activo. |
| Precondicion | No hay activo `estado = 0`. |
| Disparador | Usuario intenta abrir documentos. |
| Resultado | UI bloquea tab/panel y conserva modulo estable. |

Flujo:

1. Usuario abre modulo o consulta un pendiente.
2. Guard evalua contexto.
3. Si no hay `estado = 0`, no renderiza `CapDocument`.
4. UI muestra estado inactivo o redirige a formulario.

