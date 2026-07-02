# PROMPT ARQUITECTONICO - Radicacion Simplificada
# Fase FE-06 - Inicio del modulo con estado activo y contexto documental

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend senior especialista en:

- React 19 y TypeScript estricto;
- React Router;
- restauracion de contexto al iniciar modulo;
- flujos documentales por estado;
- integracion REST tipada;
- guards de navegacion;
- UX operativa para continuidad de tramite.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar el inicio inteligente de `src/modules/radicacion`.

Al entrar al modulo, el frontend debe consultar si el usuario tiene un tramite documental activo en `estado = 0`.

API backend:

```txt
GET /api/radicacion/pendientes/estado-activo
```

Si existe activo, el modulo debe restaurar el contexto y entrar directamente al panel `Documentos`.

Si no existe activo, el modulo inicia normal y mantiene `Documentos` inactivo hasta que se radique/tome un tramite que quede en `estado = 0`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documentos:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-02-Navegacion-Contextual-Post-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-03-Panel-Documental-Post-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-02-Estado-Activo-Radicacion.md
```

Frontend actual relacionado:

```txt
src/modules/radicacion/pages/RadicacionPage.tsx
src/modules/radicacion/hooks/RadicacionTabs.tsx
src/modules/radicacion/components/RadicacionForm.tsx
src/modules/radicacion/components/CapDocument.tsx
```

Patron visual/contextual de referencia:

```txt
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx
src/modules/gestionCorrespondencia/layout/GestionCorrespondenciaLayout.tsx
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO BACKEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Response esperado:

```ts
type RadicacionPendienteEstadoActivoDto = {
  tieneActivoEstado0: boolean;
  idEstadoRadicado?: number | null;
  idRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idTareaWorkflow?: number | null;
  estadoActual?: 0 | null;
  tramite?: string | null;
  remitente?: string | null;
  plantillaId?: number | null;
  tipoPlantillaId?: number | null;
  requiereGestionDocumental: boolean;
  tieneTramiteDocumentalActivoEstado0: boolean;
  destinoPostRegistro: "resumen" | "documentos";
  contextoDocumental?: {
    idGabinete?: number | null;
    nombreGabinete?: string | null;
    idTipoTramite?: number | null;
    nombreTramite?: string | null;
    utilEstadoPendienteRad?: boolean;
  } | null;
};
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLA FUNCIONAL CENTRAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

```txt
Documentos solo se activa con estado = 0.
```

Implicaciones:

- si `tieneActivoEstado0 = true`, restaurar contexto y navegar a `Documentos`;
- si `tieneActivoEstado0 = false`, iniciar normal y no activar `Documentos`;
- si la API falla, no activar `Documentos` por fallback optimista;
- no permitir tomar otro pendiente si existe activo `estado = 0`;
- no depender solo de `consecutivoRadicado` o gabinete para activar documentos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ARQUITECTURA FRONTEND OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear o extender piezas:

```txt
src/modules/radicacion/types/radicacionContextoDocumental.types.ts
src/modules/radicacion/services/radicacionPendientes.service.ts
src/modules/radicacion/hooks/useRadicacionEstadoActivo.ts
src/modules/radicacion/context/RadicacionDocumentalContext.tsx
src/modules/radicacion/components/RadicacionStartupGuard.tsx
```

Si ya existe contexto post-radicacion creado en FE-02/FE-03, integrarse ahi. No crear store paralelo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FLUJO DE INICIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Al montar la ruta base del modulo:

```txt
1. Mostrar estado de carga discreto del modulo.
2. Llamar GET /api/radicacion/pendientes/estado-activo.
3. Si tieneActivoEstado0=true:
   - guardar contexto documental;
   - marcar tieneTramiteDocumentalActivoEstado0=true;
   - bloquear toma de otro pendiente;
   - navegar a /dashboard/radicacion/registro/{idEstadoRadicado}/documentos.
4. Si tieneActivoEstado0=false:
   - limpiar contexto activo;
   - mantener Documentos inactivo;
   - permitir formulario normal y modal de pendientes.
5. Si error:
   - mostrar error recuperable;
   - no activar Documentos;
   - permitir reintentar.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## GUARD DE DOCUMENTOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

La ruta/tab `Documentos` debe validar:

```txt
tieneTramiteDocumentalActivoEstado0 === true
estadoActual === 0
idEstadoRadicado > 0
```

Si no cumple:

- no renderizar `CapDocument`;
- no cargar gabinete;
- redirigir a `Resumen` o pantalla base;
- mostrar estado funcional de no disponible si aplica.

No permitir consulta documental por:

```txt
consecutivoRadicado solamente
idTareaWorkflow solamente
idGabinete solamente
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INTERACCION CON MODAL DE PENDIENTES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Si hay activo `estado = 0`:

- el modal puede mostrar contador si se desea;
- la accion de tomar pendiente debe quedar bloqueada;
- si backend retorna bloqueo, mostrar mensaje y conservar contexto actual.

Si no hay activo:

- el modal puede listar pendientes `estado = 1`;
- al tomar pendiente exitosamente, FE-05 actualiza el mismo contexto;
- despues de tomar, navegar a `Documentos`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ESTADOS UI
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El modulo debe manejar:

- verificando estado activo;
- activo encontrado;
- sin activo;
- error al verificar;
- reintentando;
- navegando a documentos;
- documentos bloqueado por falta de estado `0`.

Evitar parpadeo de formulario limpio si inmediatamente se va a redirigir a `Documentos`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS REQUERIDAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear/actualizar:

```txt
src/modules/radicacion/hooks/useRadicacionEstadoActivo.spec.test.ts
src/modules/radicacion/components/RadicacionStartupGuard.spec.test.tsx
src/modules/radicacion/context/RadicacionDocumentalContext.spec.test.tsx
src/modules/radicacion/hooks/RadicacionTabs.spec.test.tsx
```

Casos:

- llama `estado-activo` al iniciar modulo;
- con activo `estado = 0`, guarda contexto;
- con activo `estado = 0`, navega a `/dashboard/radicacion/registro/{idEstadoRadicado}/documentos`;
- con activo `estado = 0`, habilita `Documentos`;
- con activo `estado = 0`, bloquea tomar otro pendiente;
- sin activo, no navega;
- sin activo, mantiene `Documentos` inactivo;
- error de API no activa `Documentos`;
- tab/ruta `Documentos` rechaza acceso sin estado `0`;
- no se dispara carga de gabinete sin contexto activo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- El modulo consulta `GET /api/radicacion/pendientes/estado-activo` al iniciar.
- Si existe activo `estado = 0`, se restaura contexto.
- Si existe activo `estado = 0`, se entra directo a `Documentos`.
- Si no existe activo, el modulo inicia normal.
- `Documentos` permanece inactivo sin `estado = 0`.
- No se permite tomar otro pendiente cuando ya hay activo.
- No se crean stores paralelos si ya existe contexto post-radicacion.
- Hay pruebas de inicio, guard y navegacion.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar aqui:

- listado AppTable de pendientes;
- tomar pendiente desde tabla;
- enviar a pendiente;
- upload documental;
- digitalizacion;
- visor.
