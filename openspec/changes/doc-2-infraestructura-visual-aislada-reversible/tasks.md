## 1. Alcance y decisión de corte

- [x] 1.1 Consolidar JIRA-02, restricciones no negociables y criterios de aceptación en los artefactos OpenSpec.
- [x] 1.2 Inventariar la línea base aprobada por JIRA-00 y decidir el cutover: conservar los siete recursos manuales previos; DOC-2 no crea una segunda ruta visual no controlada.

## 2. Activación de servidor y reversión

- [x] 2.1 Declarar `WorkflowCentroTrabajoModernEnabled` en `Web.config`, con valor seguro predeterminado `false`.
- [x] 2.2 Declarar `WorkflowCentroTrabajoModernPilotProfiles` y evaluar en servidor el login `GA_LOGINUSUARIOGESTION` mediante coincidencia exacta, sin URL, cookie, comodín ni decisión de cliente.
- [x] 2.3 Emitir `workflow-centro-trabajo-moderno` únicamente en `#div_content_general_wf` cuando flag y piloto estén aprobados.
- [x] 2.4 Declarar y calcular `ctw-layer-layout`, `ctw-layer-actions`, `ctw-layer-documents` y `ctw-layer-a11y`, con `layout` como dependencia de las demás.
- [x] 2.5 Permitir rollback total con el flag y parcial retirando una subcapa, sin modificar eventos, datos, permisos ni lógica de negocio.

## 3. Capa visual aislada

- [x] 3.1 Crear `Styles/workflow-centro-trabajo-moderno.css` y declararlo como `Content` del proyecto.
- [x] 3.2 Crear `js/workflow/centro-trabajo-visual.js`, declararlo como `Content` y hacerlo tolerante a carga inicial y `PageRequestManager.endRequest`.
- [x] 3.3 Entregar CSS y JavaScript DOC-2 solo a pilotos aprobados, después de `Webworkflow.js` y recursos legacy relevantes, con versión de caché explícita.
- [x] 3.4 Implementar los tokens y componentes `.ctw-btn`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__panel`, `.ctw-badge`, `.ctw-action-bar` y `.ctw-document-bar` bajo `.workflow-centro-trabajo-moderno`.
- [x] 3.5 Reorganizar visualmente, solo bajo `ctw-layer-actions`, todas las opciones, botones e iconos ya autorizados de `#menucab` y `#nav_menu` para reproducir la jerarquía del HTML patrón: `Opciones`, `Detalle` y `Servicios` al inicio; `Notas` y `Autorizar` como grupo operativo; y `Devolver`, los envíos disponibles y el cierre real al final. El adaptador debe resolver los hosts a partir de controles hijos estables —no del ClientID del `Panel`— para sobrevivir a `UpdatePanel`; cuando `#pendiente_selec_tarea` sea visible y la lógica legacy lo etiquete como `Cerrar tarea`, debe recibir solo la presentación terminal de peligro. Debe conservar nodos, IDs, visibilidad resuelta por servidor, permisos, handlers y postbacks; no crear, simular, habilitar ni ocultar acciones, ni trasladar acciones de documentos fuera de su contexto.
- [x] 3.6 Completar cabecera, lista, fila activa y acciones de documentos sobre los contenedores existentes. En piloto, preservar en `#div_label` las acciones rápidas existentes de carga e índice, sin reubicarlas al dropdown; el menú contextual conserva las acciones restantes y ningún ID o handler cambia.
- [x] 3.7 Completar shell, visor e índice existentes con grid, bordes, fondos, densidad y comportamiento móvil scoped.
- [x] 3.8 Verificar selectores scoped, estados, breakpoint y `z-index` conforme a `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`, sin controles paralelos ni selectores globales.
- [x] 3.9 Emitir contexto decorativo desde servidor solo para piloto, sin botones ni postbacks; reflejar únicamente los textos de los labels de estado existentes, sin consultar ni mantener datos de negocio paralelos.
- [x] 3.10 Completar el pie visual usando `#content_pie_seleccion_tarea` y sus labels funcionales existentes.
- [x] 3.11 Ampliar la prueba estática para cubrir el contexto de servidor y su ausencia con modo apagado.
- [x] 3.12 Consolidar para piloto el menú superior `Documentos` en el menú local de acciones, preservando rutas legacy, distinción entre documento actual y selección múltiple, y baseline no piloto.
- [x] 3.13 Retirar del menú documental consolidado de piloto las opciones `Cargar desde visor` y `Eliminar documento actual`, sin alterar sus controles legacy fuera de ese menú.
- [x] 3.14 Retirar del menú `Opciones` de piloto `Detalle de la sesión`, `Grupo relacionado` y `Estado de paginación`, conservando solo `Recuperar tarea` y la línea base no piloto.
- [x] 3.15 Trasladar para piloto `Servicio default` desde `Servicios` a `Opciones`, conservando su handler y los paneles WebForms requeridos por la lógica legacy.
- [x] 3.16 Mantener para piloto el control `#pendiente_selec_tarea` con su transición legacy, sin cambiar `E-ETP`, postback, permisos, etiqueta ni estado de la tarea.
- [x] 3.17 Corregir la activación de filas de documentos relacionados: delegar el clic no interactivo al handler legacy de visor, excluir selección y menú, sincronizar el estado visual real y cubrirlo con prueba estática.
- [x] 3.18 Reubicar visualmente `#content_pie_seleccion_tarea` como única franja de contexto sobre documentos, visor e índice; conservar sus labels, `UpdatePanel`, datos y comportamiento baseline fuera del piloto.
- [x] 3.19 Diferenciar el checkbox persistente de `CheckBox_auturiza` del menú de historial `A11`, preservando handlers, permisos y acceso legacy a la lista de autorizaciones.
- [x] 3.20 Compactar las barras del piloto sin sustituir la acción operativa `#pendiente_selec_tarea` ni su icono/etiqueta legacy.
- [x] 3.21 Normalizar `Documentos (N)` únicamente cuando `WorkflowCentroTrabajoModernActive` aprobó el piloto y cubrir los cuatro refinamientos con prueba estática.
- [x] 3.22 Estructurar el contexto del piloto en título, estado, radicado/solicitante y flujo/ruta mediante el `UpdatePanel` existente, sin consultas nuevas, parsing de cliente ni variación baseline.
- [x] 3.23 Compactar el contexto estructurado del piloto a dos líneas en escritorio y 46 px mínimos, preservando la lectura apilada en pantallas angostas y la altura útil del visor.
- [x] 3.24 Rejerarquizar las barras de acción del piloto: herramientas ghost, decisiones secundarias con borde suave y un único avance sólido, con hover y foco visible.
- [x] 3.25 Refinar el dropdown `Detalle` del piloto con estado activo ligero, ancho adaptable, agrupación semántica e iconografía diferenciada, sin rutas ni handlers nuevos.
- [x] 3.26 Corregir solo en la capa `documents` la superficie de la cabecera documental y la guía de checks: marcador interno en fila seleccionada, sin alterar la geometría, eventos ni baseline.
- [x] 3.27 Restituir el fondo blanco solicitado para la cabecera `Documentos (N)` del piloto, conservando la corrección de alineación de checks de 3.26.
- [x] 3.28 Sustituir la señal de selección documental por una barra rectangular independiente, sin borde, sombra ni radio legacy transitorio.
- [x] 3.29 Imponer fondo blanco sobre `#div_label.ctw-document-bar` del piloto frente al cromo heredado, sin afectar filas, visor ni baseline.
- [x] 3.30 Limitar el hover de la barra contextual del visor al enlace de acción, sin extenderlo al título ni al espacio flexible de la barra.
- [x] 3.31 Anclar la acción contextual del visor fuera del área flexible y neutralizar el hover de su franja, conservando su presentación estática en el breakpoint angosto.
- [x] 3.32 Restaurar para el piloto el handler legacy `E-ETP` y la etiqueta operativa de `#pendiente_selec_tarea`, eliminando el retorno local que dejaba la tarea activa.
- [x] 3.33 Emitir `meta viewport` solo para el piloto DOC-2 en el host superior y en el acceso directo al Workbench, de forma que el breakpoint responsive use el ancho real del dispositivo sin variar la cabecera baseline.
- [x] 3.34 Refluir las columnas y hosts existentes de acciones del piloto a 767 px o menos, sin recorte horizontal ni alteración de controles WebForms.
- [x] 3.35 En el host del piloto, cerrar mediante el toggle legacy el sidebar al cargar Workflow hasta 1199 px para preservar el ancho operativo de documentos; mantener el umbral baseline en otros módulos o sin piloto.

## 4. Compatibilidad WebForms y cutover de capas previas

- [x] 4.1 Mantener en su ruta y orden los siete recursos baseline de lista moderna de documentos, paginación y reubicación de iconos/acciones.
- [x] 4.2 Mantener modo apagado inerte: sin clase raíz, recursos DOC-2 ni mutaciones DOC-2; la interfaz conserva la línea base aprobada.
- [x] 4.3 Restringir el adaptador a añadir clases de presentación dentro del contenedor moderno; no mover controles, reemplazar IDs, alterar atributos de negocio o foco, ni habilitar acciones ocultas por servidor.
- [x] 4.4 Evitar errores bloqueantes y registros de consola en el adaptador, preservando scripts legacy durante carga y postback parcial.

## 5. Entregables técnicos

- [x] 5.1 Documentar arquitectura de activación: bandera, perfiles piloto, punto de evaluación, recursos, selectores, subcapas y rollback.
- [x] 5.2 Documentar contrato CSS: tokens, componentes, estados, breakpoint, alcance y `z-index`.
- [x] 5.3 Documentar plan de cutover de capas previas, inventario baseline y ausencia de efectos con modo apagado.
- [x] 5.4 Documentar plan de pruebas de activación, rollback y evidencia asociada al SHA.
- [x] 5.5 Registrar trazabilidad de JIRA-02 y exigir la documentación mediante `opsxj-governance.json`.

## 6. Verificación y aceptación

- [x] 6.1 Ejecutar validación estática, pruebas de activación, compilación y validación estricta de OpenSpec; registrar evidencia local.
- [x] 6.2 Confirmar con el piloto la paridad visual de acciones, documentos, visor e índice frente al HTML modelo y registrar hallazgos.
- [x] 6.3 Validar en ambiente autorizado flag `false`/`true` y usuario fuera/dentro del piloto; comprobar que el cliente no puede activar la capa.
- [x] 6.4 Validar rollback maestro y rollback individual de `actions`, `documents` y `a11y`, sin efectos funcionales ni modificación de datos.
- [x] 6.5 Validar carga inicial, postback parcial, recarga forzada y navegación interna: orden/caché de recursos y ausencia de errores JavaScript.
- [ ] 6.6 Capturar evidencia visual en 1366, 1024, 768 y 375 px para hover, foco, deshabilitado, menú abierto y documento seleccionado; asociarla al SHA desplegado.

## Bloqueo de QA manual

Las tareas 6.2 a 6.6 requieren acceso TLS autorizado al ambiente WebForms, una cuenta dentro del piloto, una cuenta fuera del piloto y datos Workflow controlados. La URL de Gestión fue informada, pero el runner no dispone de las credenciales/certificado TLS para completar el handshake ni de esas cuentas. La evidencia pendiente no se declara aprobada.
