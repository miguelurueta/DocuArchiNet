# Jira Context - SCRUMCORE-251

## Summary

TABS-WORKBENCH-GESTION-CORRESPONDENCIA

## Description

> # PROMPT ARQUITECTONICO - SCRUMCORE-251 - Tabs paralelos en Workbench con react-resizable-panels
> 
> ## Rol esperado
> 
> Arquitecto de software senior frontend especializado en React 19, TypeScript estricto, UX enterprise, composicion de layouts, accesibilidad, testing, gestion de estado UI y migraciones seguras sin regresion.
> 
> ## Objetivo
> 
> Implementar en el Workbench de Gestion Correspondencia una opcion opt-in para trabajar en paralelo los tabs normales **Gestion** y **Documentos** usando `react-resizable-panels`.
> 
> El comportamiento base debe conservarse:
> 
> - los tabs siguen existiendo como tabs normales;
> - el usuario puede seguir trabajando como hoy en un solo tab activo;
> - se agrega un boton para activar/desactivar la vista unificada/paralela;
> - al activar la vista paralela, **Gestion** y **Documentos** se muestran simultaneamente en dos paneles redimensionables;
> - al desactivar la vista paralela, se vuelve al comportamiento normal de tabs.
> 
> ## SCRUM ID
> 
> - Ticket: `SCRUMCORE-251`
> - Nombre sugerido: `Tabs paralelos Gestion/Documentos en Workbench con react-resizable-panels`
> - Tipo: Mejora UX / productividad enterprise
> - Alcance: Frontend
> - Backend: No modificado
> 
> 
> ## Contexto funcional
> 
> El Workbench actual permite trabajar con dos superficies principales:
> 
> - **Gestion**: vista principal del tramite/respuesta.
> - **Documentos**: listado/visor/documentos asociados al tramite.
> 
> Hoy estas superficies se consumen como tabs alternables. El usuario debe cambiar entre tabs para comparar informacion, revisar documentos y completar gestion.
> 
> La mejora busca permitir una forma de trabajo mas productiva para usuarios operativos:
> 
> - consultar documentos mientras se gestiona;
> - mantener visible el contexto de Gestion mientras se revisa el visor;
> - reducir cambios de tab;
> - preservar el modo actual para usuarios que prefieren pantalla completa.
> 
> ## Ubicacion esperada de implementacion
> 
> Revisar y aplicar el cambio principalmente en:
> 
> - `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
> - `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
> - `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
> - `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
> - `src/modules/gestionCorrespondencia/style/GestionRespuesta.module.css`
> 
> Revisar tambien, solo si el estado/tab actual lo requiere:
> 
> - `src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx`
> - `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts`
> - `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
> - `src/app/Components/UI/AppTabs`
> 
> ## Dependencia requerida
> 
> Usar la libreria:
> 
> ```bash
> npm install react-resizable-panels
> ```
> 
> Si la dependencia ya existe, no reinstalar.
> 
> Justificacion:
> 
> - resuelve resizing accesible y estable;
> - evita implementar manualmente drag, pointer events y constraints;
> - permite layouts con `PanelGroup`, `Panel`, `PanelResizeHandle`;
> - es adecuada para layout enterprise de paneles.
> 
> ## Regla principal de producto
> 
> La vista paralela debe ser **opt-in**.
> 
> Debe existir un boton visible en el Workbench o en la banda superior de tabs que permita alternar:
> 
> - `Vista normal`
> - `Vista paralela`
> 
> Texto sugerido:
> 
> - Boton inactivo: `Vista paralela`
> - Boton activo: `Vista normal`
> 
> Iconografia sugerida:
> 
> - Si existe icon set local, usar un icono de columnas/paneles.
> - Si se usa Ant Design Icons, usar un icono equivalente a layout/columnas.
> - No dibujar SVG manual si existe icono disponible en la libreria usada por el proyecto.
> 
> ## Comportamiento esperado
> 
> ### Modo normal
> 
> - Se mantiene el comportamiento actual.
> - El usuario ve tabs normales.
> - Solo se renderiza/activa el tab seleccionado segun la logica existente.
> - No debe haber cambios funcionales en validaciones, hooks, requests ni permisos.
> 
> ### Modo paralelo
> 
> - Se renderizan simultaneamente los contenidos de **Gestion** y **Documentos**.
> - Los paneles se muestran lado a lado en desktop.
> - Ambos paneles son redimensionables.
> - Debe existir un divisor visual y manipulable entre paneles.
> - Se deben definir tamanos iniciales y limites:
>   - Gestion: inicial 50%, minimo 35%.
>   - Documentos: inicial 50%, minimo 35%.
>   - Ajustar valores si el diseno actual requiere otra proporcion.
> - El boton permite volver a vista normal sin perder el contexto del documento seleccionado ni el estado de gestion.
> 
> ### Responsive
> 
> - En anchos pequenos o mobile, no forzar dos columnas si degrada la UX.
> - Recomendacion:
>   - desktop/tablet ancho: paneles horizontales;
>   - mobile: mantener tabs normales o usar paneles verticales solo si el diseno queda util.
> - La decision responsive debe documentarse.
> 
> ## Restricciones obligatorias
> 
> - NO cambiar contratos backend.
> - NO cambiar endpoints.
> - NO duplicar requests por montar dos veces el mismo arbol sin control.
> - NO romper `DocumentosWorkbench`.
> - NO romper `GestionRespuestaMainTabContent`.
> - NO afectar firma, visor PDF, AppEditor ni flujo de documentos.
> - NO introducir `any`.
> - NO cambiar la logica de negocio por una decision visual.
> - NO usar timers o hacks de layout.
> - NO dejar estados inconsistentes al alternar entre vista normal/paralela.
> - NO persistir estado en storage si no hay requerimiento explicito.
> 
> ## Estado UI
> 
> Crear un estado controlado local o hook dedicado para el modo layout:
> 
> ```ts
> type GestionWorkbenchLayoutMode = "tabs" | "parallel";
> ```
> 
> Contrato sugerido:
> 
> ```ts
> type UseGestionWorkbenchLayoutResult = {
>   layoutMode: GestionWorkbenchLayoutMode;
>   isParallel: boolean;
>   enableParallel: () => void;
>   disableParallel: () => void;
>   toggleParallel: () => void;
> };
> ```
> 
> Ubicacion sugerida:
> 
> - `src/modules/gestionCorrespondencia/hooks/useGestionWorkbenchLayout.ts`
> 
> Si el estado es simple y solo aplica al componente padre, puede quedar local, pero debe documentarse la razon.
> 
> ## Integracion visual esperada
> 
> La solucion debe sentirse enterprise:
> 
> - bordes sutiles entre areas;
> - divisor claro y usable;
> - boton con estado activo/inactivo;
> - espaciado compacto;
> - sin tarjetas decorativas innecesarias;
> - sin gradientes ornamentales;
> - sin layout marketing;
> - foco en productividad y comparacion de informacion.
> 
> El divisor debe tener:
> 
> - affordance visual;
> - area de click/drag suficiente;
> - estado hover/focus visible;
> - soporte teclado si la libreria lo permite.
> 
> ## Arquitectura recomendada
> 
> Crear un componente presentacional para encapsular el layout paralelo:
> 
> ```txt
> src/modules/gestionCorrespondencia/components/workbenchParallelTabs/
>   GestionWorkbenchParallelTabs.tsx
>   GestionWorkbenchParallelTabs.module.css
>   index.ts
> ```
> 
> Contrato sugerido:
> 
> ```ts
> type GestionWorkbenchParallelTabsProps = {
>   gestion: React.ReactNode;
>   documentos: React.ReactNode;
>   className?: string;
> };
> ```
> 
> Este componente debe:
> 
> - importar `PanelGroup`, `Panel`, `PanelResizeHandle` desde `react-resizable-panels`;
> - renderizar dos paneles;
> - no conocer reglas de negocio;
> - no ejecutar queries;
> - no importar services;
> - solo resolver layout.
> 
> ## Punto de integracion recomendado
> 
> El componente padre que hoy decide entre tab **Gestion** y tab **Documentos** debe decidir:
> 
> ```tsx
> if (layoutMode === "parallel") {
>   return (
>     <GestionWorkbenchParallelTabs
>       gestion={<GestionRespuestaMainTabContent ... />}
>       documentos={<DocumentosWorkbench ... />}
>     />
>   );
> }
> 
> return <TabsActuales ... />;
> ```
> 
> La implementacion debe identificar el punto real del repo donde se construyen esos tabs. Si los tabs no estan centralizados, crear una capa pequena de composicion sin mover logica de negocio.
> 
> ## Reglas para evitar duplicacion de carga
> 
> Antes de implementar, revisar si **Gestion** y **Documentos** ya se montan simultaneamente o solo el tab activo.
> 
> Si hoy solo se monta el tab activo:
> 
> - activar vista paralela puede montar ambos contenidos;
> - validar que no se dupliquen requests innecesarios;
> - usar providers/context existentes para compartir estado;
> - no crear nuevos providers duplicados por panel.
> 
> Si hoy ambos tabs ya permanecen montados:
> 
> - preservar ese comportamiento;
> - el modo paralelo solo cambia layout/visibilidad.
> 
> ## Accesibilidad
> 
> El boton de modo paralelo debe:
> 
> - ser `button`;
> - tener texto o `aria-label` claro;
> - exponer estado con `aria-pressed`;
> - ser operable por teclado.
> 
> El resize handle debe:
> 
> - ser visible en focus;
> - no quedar oculto por overlays;
> - no bloquear scroll interno de los paneles.
> 
> Los paneles deben:
> 
> - tener labels accesibles:
>   - `aria-label="Gestion"`
>   - `aria-label="Documentos"`
> - mantener scroll independiente si el contenido lo requiere.
> 
> ## Testing obligatorio
> 
> ### Unitario / componente
> 
> Agregar o ajustar tests en:
> 
> - `src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx`
> - `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
> - nuevo test para `GestionWorkbenchParallelTabs` si se crea el componente.
> 
> Casos minimos:
> 
> - renderiza modo normal por defecto;
> - boton activa modo paralelo;
> - boton vuelve a modo normal;
> - modo paralelo muestra Gestion y Documentos simultaneamente;
> - boton usa `aria-pressed`;
> - no desaparece el documento seleccionado al alternar modo;
> - no se ejecutan callbacks criticos dos veces por el toggle.
> 
> ### Build
> 
> Ejecutar:
> 
> ```powershell
> npm.cmd run build
> ```
> 
> ### OpenSpec
> 
> Si se crea cambio OpenSpec:
> 
> ```powershell
> npx.cmd openspec validate <change-id> --strict
> ```
> 
> ## QA manual obligatorio
> 
> 1. Abrir Gestion Correspondencia.
> 2. Entrar a un tramite con tabs Gestion y Documentos.
> 3. Verificar modo normal actual.
> 4. Click en `Vista paralela`.
> 5. Confirmar que Gestion y Documentos se ven simultaneamente.
> 6. Redimensionar los paneles.
> 7. Seleccionar un documento en Documentos.
> 8. Volver a `Vista normal`.
> 9. Confirmar que no se perdio seleccion/contexto.
> 10. Reabrir `Vista paralela`.
> 11. Confirmar que no hay doble carga evidente ni errores de consola.
> 12. Probar en ancho reducido.
> 
> ## Criterios de aceptacion
> 
> - El usuario puede trabajar en tabs normales como antes.
> - Existe boton para activar/desactivar vista paralela.
> - En vista paralela se ven **Gestion** y **Documentos** al mismo tiempo.
> - Los paneles son redimensionables con `react-resizable-panels`.
> - La implementacion no cambia endpoints ni reglas de negocio.
> - No hay regresiones en `DocumentosWorkbench`.
> - No hay regresiones en flujo de firma/documentos/visor.
> - El layout es enterprise, sobrio y usable.
> - Build OK.
> - Tests relevantes OK.
> - Documentacion enterprise creada.
> 
> ## Documentacion enterprise obligatoria
> 
> Crear documentacion en:
> 
> ```txt
> docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/
> ```
> 
> Archivos esperados:
> 
> 1. `SCRUMCORE-251-Arquitectura.md`
>    - objetivo;
>    - problema;
>    - solucion;
>    - decision de `react-resizable-panels`;
>    - diagrama de componentes;
>    - flujo modo normal vs paralelo;
>    - restricciones;
>    - riesgos;
>    - decisiones responsive.
> 
> 2. `SCRUMCORE-251-Implementacion-Detallada.md`
>    - archivos creados;
>    - archivos modificados;
>    - componentes;
>    - hooks;
>    - props;
>    - estilos;
>    - comportamiento de toggle;
>    - compatibilidad con tabs existentes.
> 
> 3. `SCRUMCORE-251-Pruebas.md`
>    - pruebas unitarias;
>    - pruebas de integracion UI;
>    - build;
>    - QA manual;
>    - evidencias;
>    - no regresion.
> 
> 4. `SCRUMCORE-251-Metadata.md`
>    - ticket;
>    - rama;
>    - commits;
>    - PR;
>    - fecha;
>    - estado;
>    - riesgos residuales.
> 
> ## Riesgos conocidos
> 
> - Montar simultaneamente Gestion y Documentos podria duplicar requests si los providers no estan compartidos.
> - El visor/documentos puede requerir recalculo de dimensiones al cambiar de tab normal a panel paralelo.
> - Si `DocumentosWorkbench` contiene componentes con altura fija, puede requerir CSS de `min-height: 0` y `overflow: hidden/auto` en contenedores.
> - En mobile, dos paneles horizontales pueden ser inutiles; debe existir fallback.
> 
> ## Instruccion final para Codex
> 
> Antes de implementar:
> 
> 1. Leer `DocumentosWorkbench.tsx`, `GestionRespuestaMainTabContent.tsx` y el componente que define los tabs Gestion/Documentos.
> 2. Confirmar donde vive el estado de tab activo.
> 3. Confirmar si ambos tabs se montan simultaneamente o solo el activo.
> 4. Proponer el punto exacto de integracion.
> 
> Implementar:
> 
> 1. Agregar `react-resizable-panels`.
> 2. Crear componente de layout paralelo.
> 3. Agregar boton de toggle.
> 4. Integrar modo paralelo sin tocar logica de negocio.
> 5. Ajustar CSS enterprise y responsive.
> 6. Agregar tests.
> 7. Crear documentacion enterprise.
> 
> Validar:
> 
> 1. `npm.cmd run build`.
> 2. Tests relevantes.
> 3. QA manual.
> 4. Checklist de no regresion.
> 
> Reportar:
> 
> - archivos modificados;
> - decisiones tomadas;
> - pruebas ejecutadas;
> - riesgos residuales;
> - instrucciones de uso para usuario final.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: GESTIONCORRESPONDENCIA, TABS, WORKBENCH
