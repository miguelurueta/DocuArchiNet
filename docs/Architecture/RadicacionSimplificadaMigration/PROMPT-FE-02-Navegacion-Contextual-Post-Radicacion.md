# PROMPT ARQUITECTONICO - Radicacion Simplificada
# Fase FE-02 - Navegacion contextual post-radicacion tipo GestionCorrespondencia

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend senior y desarrollador React especialista en:

- React Router;
- React 19;
- TypeScript estricto;
- shell persistente por rutas;
- navegacion contextual;
- composicion de modulos enterprise;
- migracion de flujos legacy a rutas modernas;
- estado tipado post-transaccional;
- accesibilidad y UX operacional.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar para `src/modules/radicacion` un sistema de navegacion contextual inspirado en:

```txt
src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx
src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
```

El objetivo es que, despues de registrar una radicacion, el modulo pueda abrir un panel contextual sobre el formulario principal sin perder el contexto operativo.

Ruta sugerida:

```txt
/dashboard/radicacion
/dashboard/radicacion/registro/:idEstadoRadicado
/dashboard/radicacion/registro/:idEstadoRadicado/documentos
```

La fase debe crear la estructura de navegacion, shell, metadata y contexto post-radicacion. No debe implementar todavia el workbench documental completo.

Caso operativo obligatorio:

```txt
Un radicado puede quedar sin gestion documental y el usuario puede salir del modulo.
Cuando el sistema detecte que existe un tramite documental activo en estado 0,
debe entrar directamente al panel de Documentos.
Si no existe ese tramite pendiente, Documentos debe quedar inactivo.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Documento fuente:

```txt
docs/Architecture/RadicacionSimplificadaMigration/Analisis-Migracion-Legacy-RadicadorSimplificado.md
```

Fase previa esperada:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md
```

Patron de referencia:

```txt
src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx
src/modules/gestionCorrespondencia/style/GestionCorrespondenciaRoute.module.css
src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
```

Decision clave:

```txt
Radicacion no debe depender exclusivamente de idTareaWf.
No todo radicado necesariamente genera workflow.
La llave inicial del panel post-radicacion debe ser idEstadoRadicado.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## DIFERENCIA CON GESTION CORRESPONDENCIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

`gestionCorrespondencia` usa:

```txt
/dashboard/gestion-correspondencia/respuesta/:id
id = idTareaWf
```

Radicacion debe usar inicialmente:

```txt
/dashboard/radicacion/registro/:idEstadoRadicado
```

Metadata post-registro:

```txt
ConsecutivoRadicado
IdRadicado
IdEstadoRadicado
EstadoAsignacion
MetadataOperativa
requiereGestionDocumental
tieneTramiteDocumentalActivoEstado0
destinoPostRegistro
```

`idTareaWorkflow` puede aparecer despues en metadata o por resolucion backend, pero no debe ser requisito para abrir el panel.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ARQUITECTURA OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Estructura sugerida:

```txt
src/modules/radicacion/
  layout/
    RadicacionLayout.tsx

  routes/
    RadicacionRoute.tsx

  pages/
    RadicacionRoutePage.tsx
    RadicacionPostRegistroPage.tsx

  context/
    RadicacionRegistroContext.tsx

  hooks/
    useRadicacionPostRegistroResolver.ts

  services/
    radicacionPostRegistro.service.ts

  types/
    radicacionPostRegistro.types.ts

  style/
    RadicacionRoute.module.css
```

Composicion esperada:

```txt
RadicacionLayout
  -> Outlet

RadicacionRoute
  -> mainRegion: RadicacionRoutePage
  -> detailRegion: RadicacionPostRegistroPage
```

Diagrama:

```txt
┌────────────────────────────────────────────────────────────┐
│                    RadicacionRoute                         │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ mainRegion                                           │  │
│  │ RadicacionRoutePage / RadicacionForm                 │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ detailRegion                                         │  │
│  │ RadicacionPostRegistroPage                           │  │
│  │ Header: Radicado / Estado / Tramite / Destinatario   │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS ARQUITECTONICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROHIBIDO:

- copiar literalmente `GestionCorrespondenciaRoute` sin adaptar dominio;
- usar `idTareaWf` como unica llave obligatoria;
- guardar contexto post-radicacion en variables globales;
- depender de localStorage como fuente principal;
- montar panel si el id de ruta es invalido;
- romper el flujo base `/dashboard/radicacion`;
- desmontar innecesariamente el formulario principal;
- introducir rutas no protegidas;
- introducir `any` nuevo.

OBLIGATORIO:

- mantener ruta base funcional;
- validar `idEstadoRadicado` de URL;
- crear estado `loading | ready | blocked`;
- mostrar retorno visible a la pantalla base;
- mostrar metadata en header del panel;
- usar contexto tipado para datos post-radicacion;
- permitir deep-link solo si existe forma de resolver metadata desde API o estado local controlado;
- dejar estados bloqueados claros si no se puede resolver contexto.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## RUTAS ESPERADAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actualizar rutas globales:

```tsx
{
  path: "radicacion",
  element: <RadicacionLayout />,
  children: [
    {
      index: true,
      element: <RadicacionRoute />,
    },
    {
      path: "registro/:idEstadoRadicado",
      element: <RadicacionRoute detailContent={<RadicacionPostRegistroPage />} />,
    },
  ],
}
```

Si se decide no introducir `RadicacionLayout`, justificarlo y mantener separacion equivalente.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO DE CONTEXTO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear un contexto tipado:

```ts
type RadicacionRegistroContextState = {
  idEstadoRadicado?: number;
  idRadicado?: number;
  consecutivoRadicado?: string;
  estadoAsignacion?: string;
  requiereGestionDocumental?: boolean;
  tieneTramiteDocumentalActivoEstado0?: boolean;
  destinoPostRegistro?: "resumen" | "documentos";
  tramite?: string;
  destinatario?: string;
  remitente?: string;
  metadataOperativa: Record<string, unknown>;
  loading: boolean;
  error?: string;
  reload: () => Promise<void>;
};
```

El provider debe aceptar:

```ts
type RadicacionRegistroProviderProps = {
  idEstadoRadicado?: number;
  initialPostRegistro?: RadicacionPostRegistroState;
  children: ReactNode;
};
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## COMPORTAMIENTO DEL SHELL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Estados:

```ts
type RadicacionDetailState =
  | "loading"
  | "ready"
  | "blocked-invalid-id"
  | "blocked-not-found"
  | "blocked-error";
```

Reglas:

- Si no hay detalle, renderizar solo `RadicacionRoutePage`.
- Si la ruta tiene id invalido, cerrar o bloquear con mensaje claro.
- Si hay `idEstadoRadicado` valido, abrir panel.
- Si existe contexto post-registro en memoria, usarlo para metadata inmediata.
- Si `tieneTramiteDocumentalActivoEstado0 = true`, abrir directamente la seccion `Documentos`.
- Si no existe tramite documental activo en estado `0`, mantener `Documentos` inactivo aunque exista radicado o gabinete.
- Si no existe contexto en memoria, intentar resolver metadata si hay API disponible.
- Si no hay API para resolver, mostrar estado bloqueado indicando que falta endpoint/resolver.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INTEGRACION CON FE-01
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Despues de registro exitoso:

```ts
const idEstadoRadicado = response.data.ReturnRegistraRadicacion.IdEstadoRadicado;
navigate(`/dashboard/radicacion/registro/${idEstadoRadicado}`);
```

Debe transportar metadata post-registro mediante estado React/contexto controlado, no por querystring.

Metadata minima:

```ts
{
  consecutivoRadicado,
  idRadicado,
  idEstadoRadicado,
  estadoAsignacion,
  requiereGestionDocumental,
  tieneTramiteDocumentalActivoEstado0,
  destinoPostRegistro,
  metadataOperativa
}
```

Si FE-01 marca `tieneTramiteDocumentalActivoEstado0 = true` y `destinoPostRegistro = "documentos"`, la navegacion debe preferir:

```ts
navigate(`/dashboard/radicacion/registro/${idEstadoRadicado}/documentos`);
```

Si no se crea una ruta especifica, debe transportarse un estado tipado equivalente:

```ts
navigate(`/dashboard/radicacion/registro/${idEstadoRadicado}`, {
  state: { initialSection: "documentos" },
});
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar en esta fase:

- carga documental;
- digitalizacion;
- visor PDF;
- cambio de tipologia documental;
- pendientes;
- envio workflow manual;
- tabs finales `Resumen/Documentos` con comportamiento completo;
- endpoints backend nuevos;
- persistencia local compleja del contexto.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Existe ruta base `/dashboard/radicacion`.
- Existe ruta hija `/dashboard/radicacion/registro/:idEstadoRadicado`.
- La ruta base sigue renderizando el formulario.
- La ruta hija renderiza el formulario en region principal y panel contextual encima.
- El panel tiene boton claro de volver.
- El panel muestra metadata minima cuando viene de FE-01.
- Si existe tramite documental activo en estado `0`, la navegacion entra directamente a `Documentos`.
- Si no existe tramite documental activo en estado `0`, `Documentos` queda inactivo.
- Id invalido no rompe pantalla.
- No se usa `idTareaWf` como requisito universal.
- No se introducen variables globales.
- Tests cubren:
  - ruta base sin panel;
  - ruta con id valido;
  - ruta con id invalido;
  - cierre del panel;
  - metadata inicial post-registro.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## NOTA DE DISENO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El valor del patron de `gestionCorrespondencia` no esta en el codigo exacto, sino en la idea:

```txt
mantener contexto principal visible + abrir panel contextual gobernado por URL
```

Para radicacion, esto permite continuar naturalmente hacia:

- documentos del radicado;
- digitalizacion;
- pendientes;
- workflow;
- impresion/rotulo;
- detalle operativo.

