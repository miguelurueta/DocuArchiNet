# Jira Context - SCRUMCORE-297

## Summary

CREA-COMPONENTE-APPHORIZONTALSCROLLER

## Description

> Crear un cambio OpenSpec para implementar el componente UI base AppHorizontalScroller.
>   SCRUM ID: SCRUMCORE-XXX
> Objetivo
>   Implementar un componente reusable de UI llamado AppHorizontalScroller para renderizar contenido en una fila horizontal  responsive con scroll en X, sin acoplamiento a reglas de negocio, módulos funcionales, contratos backend ni consumo HTTP.
>   El componente debe servir como primitive UI para listados tipo rail/banner, por ejemplo documentos anexos, accesos rápidos,  tarjetas resumidas, acciones frecuentes o colecciones compactas.
>   Debe estar implementado con React 19, TypeScript estricto y CSS Modules, siguiendo los patrones existentes del proyecto.
> Restricción Principal
>   Este SCRUM NO debe consumir APIs externas ni internas.
>   No debe incluir:
> axios
> 
> fetch
> 
> servicios HTTP
> 
> integración directa con GestionCorrespondencia
> 
> lógica de paginación
> 
> dependencia de AppTable
> 
> dependencia de AppTreeTable
> 
> dependencias nuevas sin justificación técnica
> 
> Principio De Diseño
>   AppHorizontalScroller debe ser un componente UI puro.
>   Separación esperada:
> Componente de dominio
>   - obtiene datos
>   - maneja loading/error/empty
>   - renderiza items
>   - define acciones
>   - define mensajes
>         |
>         | children
>         v
> AppHorizontalScroller
>   - layout horizontal
>   - overflow-x
>   - gap
>   - densidad
>   - ancho de items
>   - scroll snap opcional
>   - accesibilidad base
> 
> ## Archivos Sugeridos
> 
> Crear:
> 
> - src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx
> - src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.module.css
> - src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx
> - src/app/Components/UI/AppHorizontalScroller/index.ts
> 
> Actualizar export barrel solo si el proyecto ya usa ese patrón para componentes UI.
> 
> No modificar:
> 
> - AppTable
> - AppTreeTable
> - módulos de negocio
> - servicios HTTP existentes
> 
> ## API Del Componente
> 
> Definir tipos explícitos:
> 
> export type AppHorizontalScrollerDensity = "compact" | "comfortable";
> export type AppHorizontalScrollerGap = "xs" | "sm" | "md" | "lg";
> export type AppHorizontalScrollerSnap = "none" | "start" | "center";
> 
> export interface AppHorizontalScrollerProps {
>   children: React.ReactNode;
>   ariaLabel: string;
>   className?: string;
>   viewportClassName?: string;
>   contentClassName?: string;
>   density?: AppHorizontalScrollerDensity;
>   gap?: AppHorizontalScrollerGap;
>   itemMinWidth?: number | string;
>   itemMaxWidth?: number | string;
>   scrollSnap?: AppHorizontalScrollerSnap;
>   edgeFade?: boolean;
>   testId?: string;
> }
> 
> Defaults recomendados:
> 
> density = "comfortable";
> gap = "md";
> scrollSnap = "none";
> edgeFade = false;
> 
> ## Reglas De Implementación React 19 + TypeScript
> 
> - Usar function component tipado.
> - No usar React.FC si el proyecto evita ese patrón.
> - Usar imports de tipo con import type.
> - Mantener props readonly si el patrón local lo permite.
> - Evitar estado interno si no es necesario.
> - Evitar efectos (useEffect) si el componente solo resuelve layout.
> - No clonar children salvo necesidad real.
> - No transformar la estructura semántica de los hijos.
> - No imponer cards internas.
> - No introducir lógica de negocio.
> - No usar forwardRef salvo que exista un requerimiento claro o patrón local.
> - Usar CSS custom properties para itemMinWidth e itemMaxWidth.
> - Normalizar valores numéricos a px de forma segura.
> - No aceptar valores negativos para dimensiones; si se reciben, fallback al default o no aplicar custom property.
> 
> ## Comportamiento Esperado
> 
> - Renderizar children en una fila horizontal.
> - Usar overflow-x: auto.
> - Mantener overflow-y controlado.
> - Ser responsive en mobile, tablet y desktop.
> - No romper el ancho del contenedor padre.
> - El scroll horizontal debe funcionar con:
>     - touch
>     - trackpad
>     - rueda horizontal nativa
>     - interacción de teclado propia de los hijos enfocables
> 
> - Mantener dimensiones estables para evitar layout shift.
> - Permitir que el consumidor defina ancho mínimo y máximo de ítems.
> - Soportar scroll-snap opcional.
> - Soportar edgeFade opcional como ayuda visual no invasiva.
> - No renderizar botones de navegación en esta primera versión.
> - No implementar paginación.
> - No implementar virtualización.
> - No forzar altura fija global.
> 
> ## Accesibilidad
> 
> - El viewport principal debe usar role="region" y aria-label={ariaLabel}.
> - ariaLabel es obligatorio por TypeScript.
> - No usar tabIndex en el contenedor salvo que se implemente navegación de teclado explícita.
> - No interceptar eventos de teclado.
> - No capturar scroll global de forma manual.
> - El foco visible debe depender de los hijos interactivos.
> - No ocultar outlines/focus rings de los hijos.
> - No depender solo del color para transmitir información.
> - El componente debe funcionar aunque los hijos sean:
>     - botones
>     - links
>     - cards
>     - elementos estáticos
>     - componentes de dominio
> 
> - Si no hay hijos, el componente no debe romper render ni producir errores runtime.
> 
> ## Reglas CSS
> 
> - Usar CSS Modules.
> - No usar estilos globales.
> - No usar paleta de negocio hardcodeada.
> - Evitar gradientes decorativos dominantes.
> - No crear cards internas.
> - No anidar cards.
> - No usar position: fixed.
> - Usar box-sizing: border-box.
> - Usar min-width: 0 donde aplique para evitar overflow accidental.
> - Usar max-width: 100%.
> - Usar scrollbar-gutter: stable solo si no afecta compatibilidad.
> - Usar -webkit-overflow-scrolling: touch para mejorar mobile.
> - Respetar prefers-reduced-motion si se agrega scroll-behavior.
> - No usar font-size escalado por viewport.
> - No usar letter-spacing negativo.
> 
> Densidad recomendada:
> 
> compact:
> - menor padding vertical
> - gap menor si no se overridea
> 
> comfortable:
> - padding más respirado
> - default visual general
> 
> ## Edge Fade
> 
> Si edgeFade=true:
> 
> - Implementar el fade con pseudo-elementos o capas internas.
> - Usar pointer-events: none.
> - No bloquear botones, links, inputs ni elementos interactivos renderizados como hijos.
> - No impedir selección de texto.
> - No ocultar completamente contenido.
> - No depender del fade para comunicar funcionalidad crítica.
> - El fade debe adaptarse a fondos claros/oscuros de forma neutral o usando tokens existentes si existen.
> 
> ## Scroll Snap
> 
> Si scrollSnap="none":
> 
> - No aplicar snap.
> 
> Si scrollSnap="start":
> 
> - Aplicar scroll-snap-type: x proximity.
> - Los ítems deben resolver scroll-snap-align: start.
> 
> Si scrollSnap="center":
> 
> - Aplicar scroll-snap-type: x proximity.
> - Los ítems deben resolver scroll-snap-align: center.
> 
> No usar mandatory para evitar una experiencia rígida en listas largas.
> 
> ## Manejo De Ancho De Ítems
> 
> El componente debe exponer custom properties, por ejemplo:
> 
> --app-horizontal-scroller-item-min-width
> --app-horizontal-scroller-item-max-width
> 
> Reglas:
> 
> - Si itemMinWidth es number, convertir a ${value}px.
> - Si itemMinWidth es string, usarlo tal cual solo si no está vacío.
> - Si itemMaxWidth es number, convertir a ${value}px.
> - Si itemMaxWidth es string, usarlo tal cual solo si no está vacío.
> - Los hijos directos del content deben recibir una regla de layout estable, por ejemplo:
>     - flex: 0 0 auto
>     - min-width: var(--...)
>     - max-width: var(--...)
> 
> No mutar los hijos para inyectar props.
> 
> ## Ejemplo De Uso Esperado
> 
> import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";
> 
> export function ExampleRail() {
>   return (
>     <AppHorizontalScroller
>       ariaLabel="Listado horizontal de elementos"
>       density="compact"
>       gap="sm"
>       itemMinWidth={220}
>       itemMaxWidth={280}
>       scrollSnap="start"
>       edgeFade
>     >
>       <button type="button">Elemento 1</button>
>       <button type="button">Elemento 2</button>
>       <button type="button">Elemento 3</button>
>     </AppHorizontalScroller>
>   );
> }
> 
> ## No Objetivos
> 
> Este SCRUM no debe implementar:
> 
> - consumo del endpoint SCRUM-162
> - servicio HTTP
> - hook de carga
> - cards documentales
> - acciones sobre IdImagen
> - visor documental
> - descarga documental
> - paginación
> - búsqueda
> - filtros
> - virtualización
> - botones prev/next
> - integración con GestionRespuestaMainTabContent
> 
> Eso debe quedar para un SCRUM posterior de integración de dominio.
> 
> ## Relación Con SCRUM-162
> 
> Este componente queda preparado para ser usado posteriormente por un componente de dominio como:
> 
> GestionRespuestaAdjuntosRespuestaRail
> 
> Ese futuro componente sí podrá consumir:
> 
> GET /api/GestionCorrespondencia/solicita-documentos-adjuntos-respuesta-radicado?idRespuestaRadicado=...
> 
> Pero AppHorizontalScroller no debe conocer ese contrato.
> 
> ## Criterios De Aceptación
> 
> - El componente renderiza correctamente sus hijos.
> - El scroll horizontal aparece cuando el contenido excede el ancho disponible.
> - En mobile no rompe layout ni genera overflow horizontal de página fuera del componente.
> - El viewport principal usa role="region" y aria-label.
> - ariaLabel es requerido por TypeScript.
> - density aplica estilos diferenciados.
> - gap aplica separación entre ítems.
> - itemMinWidth aplica ancho mínimo estable.
> - itemMaxWidth aplica ancho máximo estable.
> - scrollSnap aplica comportamiento opcional.
> - edgeFade aplica ayuda visual sin bloquear interacción.
> - No consume APIs.
> - No importa código de módulos de negocio.
> - No modifica AppTable.
> - No modifica AppTreeTable.
> - No introduce dependencias nuevas.
> - Incluye tests unitarios.
> - Incluye documentación enterprise.
> - Queda listo para ser usado por SCRUM-162 sin deuda técnica.
> 
> ## Tests Requeridos
> 
> Crear pruebas con React Testing Library para validar:
> 
> - Renderiza children.
> - Aplica role="region".
> - Aplica aria-label.
> - Aplica data-testid cuando testId existe.
> - Aplica clase de densidad compact.
> - Aplica clase de densidad comfortable.
> - Aplica clase de gap xs, sm, md, lg.
> - Expone custom property para itemMinWidth.
> - Expone custom property para itemMaxWidth.
> - Convierte valores numéricos de ancho a px.
> - Ignora strings vacíos en dimensiones.
> - Activa clase de scroll snap start.
> - Activa clase de scroll snap center.
> - No aplica snap cuando scrollSnap="none".
> - Activa clase de edgeFade cuando corresponde.
> - Renderiza sin fallar con children={null}.
> 
> ## Documentación Enterprise
> 
> Crear documentación en:
> 
> docs/Architecture/AppHorizontalScroller/SCRUMCORE-XXX-AppHorizontalScroller.md
> 
> La documentación debe incluir:
> 
> 1. Objetivo
> 2. Alcance
> 3. No objetivos
> 4. Arquitectura
> 5. API de props
> 6. Contrato visual
> 7. Reglas responsive
> 8. Accesibilidad
> 9. Reglas CSS
> 10. Decisiones técnicas
> 11. Restricciones
> 12. Ejemplos de uso
> 13. Diagrama de composición
> 14. Estrategia de pruebas
> 15. Riesgos y mitigaciones
> 16. Relación con futuros consumidores
> 17. Relación futura con SCRUM-162
> 18. Checklist de aceptación
> 
> ## Diagrama Requerido
> 
> Incluir este diagrama o uno equivalente:
> 
> ┌────────────────────────────────────────────┐
> │ Consumidor de dominio                       │
> │ - obtiene datos                             │
> │ - maneja loading/error/empty                │
> │ - renderiza items                           │
> │ - define acciones                           │
> └──────────────────────┬─────────────────────┘
>                        │ children
>                        ▼
> ┌────────────────────────────────────────────┐
> │ AppHorizontalScroller                       │
> │ - role region + aria-label                  │
> │ - layout horizontal                         │
> │ - overflow-x                                │
> │ - gap/density                               │
> │ - item min/max width                        │
> │ - scroll snap opcional                      │
> │ - edge fade no bloqueante                   │
> └────────────────────────────────────────────┘
> 
> ## Validaciones
> 
> Ejecutar:
> 
> - Tests focalizados del componente.
> - Lint focalizado si existe script.
> - Verificación TypeScript/build si el proyecto lo permite.
> 
> Si el build general falla por errores preexistentes no relacionados, documentar:
> 
> - comando ejecutado
> - error exacto
> - archivo afectado
> - por qué no pertenece al cambio
> 
> ## Resultado Esperado
> 
> Un componente UI base AppHorizontalScroller, estable, responsive, accesible, testeado y documentado, listo para ser usado
> posteriormente por componentes de dominio como el listado horizontal de documentos adjuntos de respuesta radicado de SCRUM-
> 162.
> 
> Este prompt no tiene consumo externo. Es deliberadamente un primitive UI puro para evitar deuda técnica y dejar el consumo
> del contrato para otro SCRUM.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: APPHORIZONTALSCROLLER, COMPONENTE, CREA
