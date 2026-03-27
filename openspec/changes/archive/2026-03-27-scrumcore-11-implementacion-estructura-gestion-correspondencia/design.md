## Context

`SCRUMCORE-11` pide crear la estructura inicial del modulo `gestionCorrespondencia` dentro de la SPA actual, manteniendo React 19, TypeScript estricto, ESM, Ant Design, CSS Modules y el patron de navegacion ya usado en `src/app/routes/routes.tsx`. Hoy el dashboard expone rutas hijas como `workflow` y `radicacion` bajo `DashboardLayout`, pero no existe un modulo de correspondencia ni un patron previo de `Outlet + Drawer` controlado por routing.

El cambio busca dejar una base enterprise y extensible, sin logica de negocio ni integracion real con backend. Eso implica que la primera iteracion debe concentrarse en capas, rutas, placeholders visuales y pruebas de render/composicion. El modulo debe convivir con la arquitectura vigente por dominios (`src/modules/<modulo>`) y reutilizar la semantica de layout protegida del dashboard, sin alterar autenticacion ni el shell principal.

## Goals / Non-Goals

**Goals:**
- Introducir `src/modules/gestionCorrespondencia/` con separacion explicita entre `layout`, `pages` y `routes`.
- Integrar el modulo como ruta hija de `/dashboard` siguiendo el patron real de React Router usado por la aplicacion.
- Implementar un patron `Outlet + Drawer` guiado por la URL, donde la vista principal permanezca visible y la vista secundaria `GestionRespuesta` aparezca como overlay contextual.
- Dejar placeholders visuales profesionales con Ant Design para evitar pantallas vacias y facilitar iteraciones futuras.
- Cubrir con pruebas el render del layout, la pagina principal y el acoplamiento entre routing anidado y Drawer.
- Documentar el modulo en `README.md` para que futuras iteraciones agreguen features sin mezclar responsabilidades.

**Non-Goals:**
- Implementar casos de uso de negocio, formularios finales, validaciones funcionales o llamadas HTTP reales.
- Definir contratos backend, DTOs finales o persistencia de estado del dominio.
- Reestructurar el `DashboardLayout`, el sistema de autenticacion o el arbol global de rutas fuera de lo necesario para registrar el nuevo modulo.
- Introducir nuevas dependencias o un sistema de estado global adicional.

## Decisions

### 1. El modulo vivira en `src/modules/gestionCorrespondencia/` y replicara la convencion por dominio del repositorio

Se crearan las carpetas `layout`, `pages` y `routes`, mas un `README.md`, para alinear el cambio con la organizacion ya usada por `radicacion`, `dashboard`, `login` y otros modulos. La carpeta `pages` contendra solo composicion visual y placeholders; `layout` encapsulara la estructura compartida del modulo; `routes` concentrara la orquestacion del flujo `Outlet + Drawer`.

Alternativas consideradas:
- Agregar todo dentro de `pages/`: se descarta porque mezcla navegacion, shell visual y vistas secundarias en una sola capa.
- Crear el modulo directamente en `src/app/`: se descarta porque rompe la cohesion por dominio que ya existe.

### 2. La entrada del modulo se registrara como ruta hija de `/dashboard`

La ruta global se agregara en `src/app/routes/routes.tsx` como otro child de `DashboardLayout`, igual que `workflow` y `radicacion`. Eso mantiene el nuevo modulo bajo el shell autenticado y aprovecha `Outlet` del dashboard sin cambios arquitectonicos mayores.

La ruta base prevista es `/dashboard/gestion-correspondencia`, con una subruta anidada para la vista secundaria, por ejemplo `/dashboard/gestion-correspondencia/respuesta`. El nombre exacto del segmento puede refinarse en la fase de specs, pero la decision de mantener nesting bajo dashboard queda fija.

Alternativas consideradas:
- Ruta top-level fuera del dashboard: se descarta porque perderia el contexto visual y de autorizacion del shell actual.
- Colgar la funcionalidad dentro de `workflow` o `radicacion`: se descarta porque el ticket pide un modulo propio y desacoplado.

### 3. `GestionCorrespondenciaLayout.tsx` sera el shell del modulo y siempre renderizara un `Outlet`

El layout usara componentes de Ant Design (`Layout`, `Typography`, `Card`, `Space`, `Flex` o equivalentes ya disponibles) para definir encabezado, descripcion y contenedor principal. Este componente no conocera detalles del Drawer ni de las acciones del dominio; solo debe proveer una estructura estable para el contenido principal y para las rutas hijas del modulo.

La pagina principal `GestionCorrespondencia.tsx` se renderizara como child index del layout. Las rutas hijas se proyectaran via `Outlet`, evitando que el layout tenga condicionantes por pathname o estado local.

Alternativas consideradas:
- Hacer que el layout maneje `Drawer` y navegacion: se descarta porque violaria la separacion de responsabilidades pedida por el ticket.
- Renderizar `GestionCorrespondencia` directamente dentro del layout sin ruta index: se descarta porque dificulta el escalado a nuevas rutas del modulo.

### 4. `GestionCorrespondenciaRoute.tsx` sera el adaptador de routing para el patron `Outlet + Drawer`

Como el ticket exige que el Drawer dependa de la URL y no solo de estado local, `GestionCorrespondenciaRoute.tsx` se implementara como wrapper de rutas hijas del modulo. Este componente renderizara la pagina principal y, en paralelo, un `Drawer` de Ant Design cuya apertura depende de si existe match para la subruta de respuesta.

La decision tecnica es usar hooks de React Router (`useLocation`, `useMatch`, `useNavigate`, o una combinacion equivalente) dentro de la ruta del modulo para:
- detectar si la subruta secundaria esta activa;
- cerrar el Drawer navegando de vuelta a la ruta base del modulo;
- mantener visible la pagina principal mientras el contenido del Drawer cambia por routing.

Dentro del `Drawer`, el contenido secundario se renderizara mediante un `Outlet` anidado o una composicion equivalente basada en children de rutas, de forma que futuras vistas secundarias puedan agregarse sin rehacer el patron.

Alternativas consideradas:
- Controlar el Drawer con `useState`: se descarta porque rompe deep-linking, navegacion con historial y el requisito explicito del ticket.
- Renderizar `GestionRespuesta` reemplazando la pagina principal: se descarta porque el usuario debe mantener el contexto visual de fondo.

### 5. `GestionRespuesta.tsx` sera una vista secundaria desacoplada y sin conocimiento del routing

`GestionRespuesta.tsx` se limitara a renderizar contenido placeholder enterprise dentro del espacio provisto por el Drawer: titulo, descripcion, bloques informativos y llamados a futuras acciones. No cerrara el Drawer por cuenta propia ni resolvera navegacion; ese control quedara en `GestionCorrespondenciaRoute.tsx`.

Alternativas consideradas:
- Hacer que `GestionRespuesta` reciba `open/onClose`: se descarta porque acopla la pagina al mecanismo visual en vez de al contrato de routing.
- Colocar la vista secundaria en `components/`: se descarta porque conceptualmente sigue siendo una pagina de ruta.

### 6. La UI base se construira con Ant Design y CSS Modules, priorizando placeholders reutilizables

El modulo usara Ant Design para tarjetas, tipografia y Drawer, manteniendo consistencia con el stack aprobado por el ticket. Solo se introduciran CSS Modules locales cuando el ajuste visual no pueda resolverse de forma limpia con props del proveedor. Esto reduce sobrecarga y mantiene la capacidad de evolucionar el modulo sin amarrarse a estilos globales.

Alternativas consideradas:
- Usar MUI para el Drawer por cercania con otros componentes: se descarta porque el ticket pide coherencia con Ant Design para este modulo.
- Resolver todo con estilos inline: se descarta porque dificulta escalabilidad y consistencia visual.

### 7. Las pruebas se enfocaran en comportamiento observable del patron de composicion

Los tests se ubicaran junto al modulo y cubriran:
- render del layout base;
- render de la pagina principal con placeholders;
- apertura del Drawer cuando se entra a la subruta secundaria;
- render de `GestionRespuesta` dentro del overlay;
- cierre del Drawer al navegar a la ruta base.

Se usara `MemoryRouter` con configuracion de rutas anidadas para validar el comportamiento real del modulo sin depender del router global completo. Las pruebas no se enfocaran en detalles internos de Ant Design sino en presencia de contenido, rol/dialog accesible y continuidad de la pagina principal.

Alternativas consideradas:
- Snapshots grandes del arbol renderizado: se descarta por bajo valor y alta fragilidad.
- Probar solo componentes aislados sin router: se descarta porque el principal riesgo del cambio esta en la integracion routing + Drawer.

## Risks / Trade-offs

- [No existe patron previo de Drawer gobernado por rutas en el repo] -> Mitigacion: encapsular la orquestacion en `GestionCorrespondenciaRoute.tsx` y validar con pruebas de navegacion usando `MemoryRouter`.
- [Ambiguedad sobre si el `Outlet` debe vivir en el layout o en la ruta adaptadora] -> Mitigacion: fijar un layout con `Outlet` para la estructura base y usar la ruta adaptadora para abrir/cerrar el Drawer segun pathname, evitando mezclar UI shell con navegacion.
- [Ant Design `Drawer` monta su contenido en portal y puede complicar tests] -> Mitigacion: usar queries accesibles y, si hace falta, configurar el render para buscar en `document.body` sin acoplarse a clases internas.
- [La ruta hija inicial puede quedarse corta cuando el modulo crezca] -> Mitigacion: diseñar `GestionCorrespondenciaRoute.tsx` para aceptar mas children del Drawer sin reescribir la pagina principal.
- [Puede aparecer duplicacion visual entre layout del dashboard y layout del modulo] -> Mitigacion: limitar el nuevo layout al contenido interno del modulo, sin reimplementar navbar ni sidebar globales.

## Migration Plan

La incorporacion sera incremental y sin migracion de datos:

1. Crear el arbol `gestionCorrespondencia` con layout, paginas, route adapter, pruebas y README.
2. Registrar la nueva ruta hija bajo `/dashboard`.
3. Verificar que la ruta base renderiza la vista principal y que la subruta secundaria activa el Drawer sin desmontar el fondo.
4. Mantener rollback simple eliminando el registro de ruta y el directorio del modulo si la iteracion inicial no se aprueba.

No se requiere migracion de backend, cambios de contratos ni scripts de despliegue.

## Open Questions

- Cual sera el segmento final de la ruta publica del modulo y de su subruta secundaria (`gestion-correspondencia`, `correspondencia`, `respuesta`, etc.).
- Si el acceso al modulo quedara despues enlazado desde el menu dinamico del dashboard o solo por ruta registrada durante la primera iteracion.
- Si el placeholder de `GestionRespuesta` debe representar lectura de detalle, respuesta a correspondencia o un formulario inicial en futuras historias.
- Si el modulo necesitara un contexto local propio en iteraciones siguientes o si bastara con hooks por pagina.
