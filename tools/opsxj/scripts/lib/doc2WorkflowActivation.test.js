import { readFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";

const testDirectory = path.dirname(fileURLToPath(import.meta.url));
const appRoot = path.resolve(testDirectory, "..", "..", "..", "..");

const readAppFile = (relativePath) => readFile(path.join(appRoot, relativePath), "utf8");

describe("DOC-2 workflow visual activation", () => {
  it("keeps the retired gate configuration closed without governing the official presentation", async () => {
    const [config, codeBehind, page, bootstrap] = await Promise.all([
      readAppFile("Web.config"),
      readAppFile("workflow/Webworkflow.aspx.vb"),
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("workflow/WorkflowModernPresentationBootstrap.vb"),
    ]);

    const active = config.match(/<add\s+key="WorkflowCentroTrabajoModernActive"\s+value="([^"]*)"\s*\/>/i);
    const official = config.match(/<add\s+key="WorkflowCentroTrabajoModernOfficialMode"\s+value="([^"]*)"\s*\/>/i);
    const users = config.match(/<add\s+key="WorkflowCentroTrabajoModernUsers"\s+value="([^"]*)"\s*\/>/i);
    const groups = config.match(/<add\s+key="WorkflowCentroTrabajoModernGroups"\s+value="([^"]*)"\s*\/>/i);

    [active, official, users, groups].forEach((setting) => expect(setting).not.toBeNull());
    expect(active[1].trim().toLowerCase()).toBe("false");
    expect(official[1].trim().toLowerCase()).toBe("false");
    expect(users[1].trim()).toBe("");
    expect(groups[1].trim()).toBe("");
    expect(codeBehind).toContain("WorkflowModernPresentationBootstrap.EstaActivaParaSolicitudActual()");
    expect(codeBehind).toMatch(/WorkflowCentroTrabajoModernPresentationEnabled As Boolean\s+Get\s+Return True/);
    expect(bootstrap).toContain("WorkflowPreviewSessionContextGate");
    expect(bootstrap).toContain("ConfiguracionWorkflowModernFeatureGate");
    expect(bootstrap).toContain("Return False");
    expect(config).toContain('WorkflowCentroTrabajoModernLayers');
    expect(codeBehind).toContain('WorkflowCentroTrabajoModernActive');
    expect(page).toContain('<%= WorkflowCentroTrabajoModernCssAttribute %>');
    expect(page).not.toContain("WorkflowCentroTrabajoModernActive");
  });

  it("emits the real mobile viewport in the official presentation and its iframe host", async () => {
    const [page, codeBehind, designer, shellPage, shellCodeBehind, shellDesigner] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("workflow/Webworkflow.aspx.vb"),
      readAppFile("workflow/Webworkflow.aspx.designer.vb"),
      readAppFile("Defaul/WebFormInicioDocuarchiGestion.aspx"),
      readAppFile("Defaul/WebFormInicioDocuarchiGestion.aspx.vb"),
      readAppFile("Defaul/WebFormInicioDocuarchiGestion.aspx.designer.vb"),
    ]);

    const head = page.match(/<head runat="server">([\s\S]*?)<\/head>/)?.[1] ?? "";
    const shellHead = shellPage.match(/<head runat="server">([\s\S]*?)<\/head>/)?.[1] ?? "";

    expect(page).toMatch(/<head runat="server">\s*<meta id="workflowCentroTrabajoModernViewport"/);
    expect(head).not.toMatch(/<%\s*(?:If|Else|End If)\b/);
    expect(head).toContain('<meta id="workflowCentroTrabajoModernViewport" runat="server" name="viewport" content="width=device-width, initial-scale=1" visible="false" />');
    expect(codeBehind).toMatch(/Private Sub ConfigureWorkflowCentroTrabajoViewport\(\)[\s\S]*?workflowCentroTrabajoModernViewport\.Visible = WorkflowCentroTrabajoModernPresentationEnabled/);
    expect(codeBehind).toMatch(/Protected Overrides Sub Page_Load[\s\S]*?ConfigureWorkflowCentroTrabajoViewport\(\)/);
    expect(codeBehind).not.toContain("Page.Header.Controls.Add(viewport)");
    expect(designer).toContain("Protected WithEvents workflowCentroTrabajoModernViewport As Global.System.Web.UI.HtmlControls.HtmlMeta");
    expect(shellPage).toMatch(/<head runat="server">\s*<meta id="workflowCentroTrabajoModernShellViewport"/);
    expect(shellHead).not.toMatch(/<%\s*(?:If|Else|End If)\b/);
    expect(shellHead).toContain('<meta id="workflowCentroTrabajoModernShellViewport" runat="server" name="viewport" content="width=device-width, initial-scale=1" visible="true" />');
    expect(shellCodeBehind).not.toMatch(/WorkflowCentroTrabajoModernShellActive|ConfigureWorkflowCentroTrabajoShellViewport|GA_LOGINUSUARIOGESTION/);
    expect(shellCodeBehind).not.toContain("Page.Header.Controls.Add(viewport)");
    expect(shellDesigner).toContain("Protected WithEvents workflowCentroTrabajoModernShellViewport As Global.System.Web.UI.HtmlControls.HtmlMeta");
  });

  it("preserves the Workbench width by collapsing only the official shell in the intermediate range", async () => {
    const [shellPage, shellMenu] = await Promise.all([
      readAppFile("Defaul/WebFormInicioDocuarchiGestion.aspx"),
      readAppFile("js/inicio/menu-vertical-responsivo.js"),
    ]);

    expect(shellPage).toContain("menu-vertical-responsivo.js?v=20260813-workbench-shell47");
    [
      "puntoCierreCentroTrabajo = 1199",
      "workflowCentroTrabajoModernShellViewport",
      "ContentPlacenter_ifrm_ds_",
      "rutaCentroTrabajo = /(?:^|\\/)workflow\\/Webworkflow\\.aspx",
      "esCentroTrabajoOficialActivo",
      "puntoCierreActual",
      "iframe.addEventListener(\"load\", sincronizarVista)",
    ].forEach((marker) => expect(shellMenu).toContain(marker));
  });

  it("wraps the existing task actions within the narrow official viewport", async () => {
    const [page, css] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
    ]);
    expect(page).toMatch(/workflow-centro-trabajo-moderno\.css\?v=[A-Za-z0-9._-]+/);
    [
      "@media (max-width: 767px)",
      "#nav_menu #navbarNavDropdown_ > .col-md-9",
      "#nav_menu #navbarNavDropdown_ > .col-md-3",
      "flex: 0 0 100%;",
      "#nav_menu .ctw-action-host",
      "flex-wrap: wrap;",
      "#nav_menu .ctw-action-slot--terminal-start",
      "#nav_menu .ctw-action-slot--send",
      "#nav_menu .ctw-action-slot--handoff",
      ".workflow-centro-trabajo-moderno.ctw-layer-layout {",
      "margin-left: 0 !important;",
      "max-width: 100vw !important;",
      "box-sizing: border-box;",
      "overflow-x: hidden !important;",
      "html:has(.workflow-centro-trabajo-moderno.ctw-layer-layout)",
      "grid-template-columns: minmax(0, 1fr);",
      "grid-column: 1;",
      "> .ctw-action-slot--send > .navbar-nav",
    ].forEach((marker) => expect(css).toContain(marker));
  });

  it("preserves the approved visual baseline and loads DOC-2 after Webworkflow.js", async () => {
    const [page, titleAdapter] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("js/workflow/documentos-relacionados-titulo-visual.js"),
    ]);
    const removedResources = [
      "gridview-moderno.css",
      "workflow-tareas-modernas.css",
      "workflow-documentos-relacionados-modernos.css",
      "workflow-documentos-relacionados-titulo.css",
      "workflow-paginacion-visual.js",
      "documentos-relacionados-visual.js",
      "documentos-relacionados-titulo-visual.js",
    ];

    removedResources.forEach((resource) => expect(page).toContain(resource));
    expect(page.indexOf("Styles/workflow-centro-trabajo-moderno.css")).toBeGreaterThan(page.indexOf("js/workflow/Webworkflow.js"));
    expect(page.indexOf("js/workflow/centro-trabajo-visual.js")).toBeGreaterThan(page.indexOf("Styles/workflow-centro-trabajo-moderno.css"));
    expect(titleAdapter).toContain("conservaAccionesRapidasDelPiloto");
    expect(titleAdapter).toContain("ctw-layer-documents");
  });

  it("keeps the CSS scoped and the adapter presentation-only", async () => {
    const [css, adapter] = await Promise.all([
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("js/workflow/centro-trabajo-visual.js"),
    ]);

    [".ctw-btn", ".ctw-icon-btn", ".ctw-menu", ".ctw-menu__panel", ".ctw-badge", ".ctw-action-bar", ".ctw-document-bar", ".ctw-legacy-modal"].forEach((component) => {
      const escapedComponent = component.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
      expect(css).toMatch(new RegExp(`\\.workflow-centro-trabajo-moderno[^,{]*${escapedComponent}`));
    });
    expect(css).not.toMatch(/(?:^|\n)\s*(body|html|:root)\s*[,{]/);
    expect(adapter).toContain("PageRequestManager");
    ["applyModalLayer", "ctw-modal-header", "ctw-modal-body", "ctw-modal-footer"].forEach((marker) => expect(adapter).toContain(marker));
    expect(adapter).not.toMatch(/console\.|appendChild|insertBefore|removeChild|\.focus\(|setAttribute/);
  });

  it("emits Bootstrap-compatible classes for the route activity modal", async () => {
    const page = await readAppFile("workflow/Webworkflow.aspx");

    [
      'CssClass="modal_content_general ctw-legacy-modal"',
      'id="modal_content_lista_actividades_worflow_ruta" class="modal-content ctw-modal-content"',
      'id="divcabecer2_lista_actividades_worflow_ruta" class="modal_title_superior_ modal-header ctw-modal-header"',
      'class="modal-title ctw-modal-title d-inline "',
      'id="contenido_procesa_lista_actividades_workflow" style="background-color: white; width: 100%; height: 99%; border-bottom:none" class="modal-body ctw-modal-body p-1"',
      'id="contenido_titulo_data_grid_dos_title" class="ctw-workflow-modal-context-anchor" aria-hidden="true"',
      'id="div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion" class="modal-footer ctw-modal-footer ctw-workflow-modal-context" aria-label="Resumen del flujo"',
    ].forEach((marker) => expect(page).toContain(marker));
  });

  it("maps the model shell to existing WebForms panels without creating markup", async () => {
    const [css, adapter] = await Promise.all([
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("js/workflow/centro-trabajo-visual.js"),
    ]);

    [
      "#content_selecion_tarea",
      "#menucab.ctw-action-bar",
      "#content_seleccion_documentos",
      "#GridView_list_documento_relacion_wf",
      "#contenido_imagen",
      "#contenido_indice",
      "#Panel_tolbar_pdf",
      "ctw-documents-pane--empty",
      "ctw-viewer-pane--empty",
      "No hay documentos asociados a esta tarea",
      "Seleccione un documento para visualizarlo",
      "ctw-loading-indicator",
      "@media (max-width: 900px)",
    ].forEach((selector) => expect(css).toContain(selector));
    ["ctw-workspace", "ctw-documents-pane", "ctw-viewer-pane", "ctw-index-pane", "toggleClass", "isRendered"].forEach((className) => {
      expect(adapter).toContain(className);
    });
    expect(adapter).not.toMatch(/innerHTML|outerHTML|insertAdjacentHTML|appendChild|insertBefore|removeChild/);
  });

  it("coordinates both existing action bars without creating or enabling task actions", async () => {
    const [page, css, adapter] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("js/workflow/centro-trabajo-visual.js"),
    ]);

    [
      "#menucab.ctw-action-bar",
      "#nav_menu.ctw-action-bar",
      "ctw-action-slot--operational",
      "ctw-action-slot--terminal",
      "ctw-action-slot--terminal-start",
      "ctw-action-slot--return",
      "ctw-action-slot--handoff",
      "ctw-action-slot--handoff-user",
      "ctw-action-slot--handoff-group",
      "ctw-action-slot--send",
      "ctw-commandbar",
      "ctw-command-menu",
      "#UpdatePanel_menu_cab",
      "#menucab .dropdown.show",
      "#nav_menu .dropdown-menu.show",
      "z-index: var(--ctw-z-menu) !important",
      "svg-inline--fa",
    ].forEach((marker) => expect(css).toContain(marker));
    [
      'id="nav_menu"',
      "Panel_tareas_estado_pendiente",
      "Panel_autoriza",
      'id="A11"',
      'id="ctw-authorize-options"',
      "Panel_devolver_tarea",
      "Panel_enviar_flujo",
      'id="pendiente_selec_tarea"',
    ].forEach((marker) => expect(page).toContain(marker));
    [
      "#nav_menu",
      "ctw-action-host",
      "addClassesToActionHosts",
      "markFirstTerminalActionHost",
      "#pendiente_db",
      ".ctw-authorize-control",
      "ImageButtonEnviarUsuario",
      "ImageButtonEnviaActividad",
      "workflow-group-send-trigger",
      "workflow-transition-trigger",
      "ImageButtonautoterminar",
      "#pendiente_selec_tarea",
      "ctw-commandbar",
      "ctw-command-menu",
    ].forEach((marker) => expect(adapter).toContain(marker));
    expect(adapter).not.toContain("ImageButtonterminar");
    expect(adapter).not.toMatch(/innerHTML|outerHTML|insertAdjacentHTML|appendChild|insertBefore|removeChild|\.click\(/);
  });

  it("uses explicit workflow decisions without changing the authorization control", async () => {
    const [page, workflowScript, returnUserPreviousConfirmation, css] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("js/workflow/Webworkflow.js"),
      readAppFile("js/workflow/workflow-return-user-previous-confirmation.js"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
    ]);

    [
      "Continuar flujo",
      "Elegir actividad anterior",
      "js/workflow/Webworkflow.js?v=20260902-doc45-notes-modal3",
      "workflow-centro-trabajo-moderno.css",
      'aria-label="Seleccionar todos los documentos"',
      'role="status" aria-live="polite"',
      'aria-label="Mostrar acciones de tarea"',
      'aria-label="Abrir detalle del radicado"',
      'id="ctw-workflow-route-modal-title"',
    ].forEach((marker) => expect(page).toContain(marker));
    expect(page).toMatch(/centro-trabajo-visual\.js\?v=[A-Za-z0-9._-]+/);
    expect(workflowScript).toContain("actualiza_titulo_lista_actividades_workflow");
    expect(workflowScript).not.toContain("Se abrirá la lista de actividades anteriores para elegir el destino de la devolución.");
    [
      "Confirmar devolución a usuario anterior",
      "El servidor volverá a validar el historial, el token y la concurrencia antes de devolver la tarea.",
    ].forEach((marker) => expect(returnUserPreviousConfirmation).toContain(marker));
    expect(returnUserPreviousConfirmation).not.toContain("Button_tool_devolver_a_usuario");
    [
      "--ctw-control-height: 44px",
      "min-height: 44px",
      ".ctw-action-slot--handoff .ctw-btn",
      ".ctw-action-slot--handoff-user .ctw-btn",
      ".ctw-action-slot--handoff-group .ctw-btn",
      ".ctw-action-slot--return .ctw-btn",
      "background: var(--ctw-pale)",
    ].forEach((marker) => expect(css).toContain(marker));
    expect(page).toContain('<asp:CheckBox ID="CheckBox_auturiza"');
  });

  it("uses ghost chrome for tools and preserves a single solid workflow advance", async () => {
    const css = await readAppFile("Styles/workflow-centro-trabajo-moderno.css");

    [
      "#menucab .ctw-command-menu > .ctw-menu__trigger",
      ".ctw-action-slot--notes .ctw-btn",
      ".ctw-action-slot--support .ctw-btn",
      "border-color: transparent !important;",
      ".ctw-action-slot--return .ctw-btn",
      ".ctw-action-slot--handoff .ctw-btn",
      ".ctw-action-slot--send .ctw-btn",
      "background: var(--ctw-blue);",
      "outline: 3px solid #8aa9ee;",
      ".ctw-authorize-menu .ctw-authorize-control",
    ].forEach((marker) => expect(css).toContain(marker));
  });

  it("limits viewer-context hover to the action link", async () => {
    const css = await readAppFile("Styles/workflow-centro-trabajo-moderno.css");

    [
      "#Panel_tolbar_pdf .ctw-viewer-document-actions",
      "flex: 0 0 auto;",
      "#Panel_tolbar_pdf .ctw-viewer-action",
      "width: fit-content !important;",
      "#Panel_tolbar_pdf .ctw-viewer-document-context > :not(.ctw-viewer-document-actions)",
      "pointer-events: none;",
      "padding-right: 128px;",
      "position: absolute;",
      "transform: translateY(-50%);",
      "#Panel_tolbar_pdf .ctw-viewer-document-context:hover",
      "background: transparent !important;",
    ].forEach((marker) => expect(css).toContain(marker));
  });

  it("keeps the document header white and the selection track aligned in the pilot", async () => {
    const css = await readAppFile("Styles/workflow-centro-trabajo-moderno.css");

    [
      ".ctw-document-bar",
      "#GridView_list_documento_relacion_wf .GridviewRow.gridview-documento-seleccionado > td:first-child",
      "border-left: 0 !important;",
      "border-top-left-radius: 0 !important;",
      "box-shadow: none !important;",
      "td:first-child::before",
      "pointer-events: none;",
    ].forEach((marker) => expect(css).toContain(marker));
    expect(css).toMatch(/#div_label\.ctw-document-bar\s*\{[^}]*background:\s*#fff !important;/);
  });

  it("refines the Detail menu without changing its legacy routes", async () => {
    const [page, css] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
    ]);

    [
      "ctw-detail-menu",
      "ctw-detail-menu__section",
      "Información",
      "Trazabilidad",
      "Documentos",
      "S-DTS",
      "G-DRR",
      "G-TDR",
      "G-TDW",
      "G-TDWG",
      "a_list_operation_document",
      "a_list_copy_document_expedient",
      "fa-info-circle",
      "fa-file-alt",
      "fa-history",
      "fa-cogs",
      "fa-copy",
      "<% If WorkflowCentroTrabajoModernPresentationEnabled Then %>",
    ].forEach((marker) => expect(page).toContain(marker));
    [
      "width: 380px;",
      "min-width: 360px;",
      "max-width: calc(100vw - 32px);",
      "ctw-detail-menu__section + .ctw-detail-menu__section",
      "ctw-detail-menu__section-label",
      "white-space: normal;",
      "background: #f5f8ff;",
      "box-shadow: none;",
    ].forEach((marker) => expect(css).toContain(marker));
  });

  it("preserves the legacy task-close transition for the official presentation", async () => {
    const [page, workflowScript, adapter, css] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("js/workflow/Webworkflow.js"),
      readAppFile("js/workflow/centro-trabajo-visual.js"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
    ]);

    expect(page).toMatch(/id="pendiente_selec_tarea"[\s\S]*?title="Cerrar tarea seleccionada"[\s\S]*?onclick="inicializa_tipo_adjunto_documento\(event,this,'E-ETP'\);"[\s\S]*?fad fa-check-circle[\s\S]*?<span id="span_pendiente_selec_tarea">Cerrar tarea<\/span>/);
    expect(page).not.toContain("Volver a tareas");
    expect(page).not.toContain("data-ctw-close-view");
    expect(workflowScript).not.toContain("closeViewOnly");
    expect(workflowScript).not.toContain("data-ctw-close-view");
    expect(adapter).not.toContain("ctw-action-slot--close");
    expect(css).not.toContain("ctw-action-slot--close");
    expect(css).toContain("#nav_menu .ctw-action-slot--terminal .ctw-btn");
  });

  it("normalizes contextual menus without replacing their legacy controls", async () => {
    const [page, css, adapter] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("js/workflow/centro-trabajo-visual.js"),
    ]);

    [
      'role="group" aria-label="Opciones de la tarea"',
      'role="group" aria-label="Acciones sobre los documentos"',
      "Acciones para documentos seleccionados",
    ].forEach((marker) => expect(page).toContain(marker));
    ["ctw-menu__divider", "ctw-menu__item--danger", "ctw-document-menu__panel"].forEach((marker) => {
      expect(adapter).toContain(marker);
      expect(css).toContain(marker);
    });
    expect(adapter).not.toMatch(/innerHTML|outerHTML|insertAdjacentHTML|appendChild|insertBefore|removeChild/);
  });

  it("keeps only task recovery in the official options menu", async () => {
    const page = await readAppFile("workflow/Webworkflow.aspx");

    expect(page).toMatch(/aria-label="Opciones de la tarea"[\s\S]*?T-RTW[\s\S]*?<% If WorkflowCentroTrabajoModernPresentationEnabled Then %>[\s\S]*?bnt_eval_event_default[\s\S]*?<% End If %>[\s\S]*?<% If Not WorkflowCentroTrabajoModernPresentationEnabled Then %>[\s\S]*?S-DDS[\s\S]*?S-GAU[\s\S]*?Button_activa_estado_paginacion[\s\S]*?<% End If %>/);
  });

  it("moves the default service to official options without removing required server panels", async () => {
    const page = await readAppFile("workflow/Webworkflow.aspx");

    expect(page).toMatch(/<asp:Panel ID="Panel_tramitar_tarea"[\s\S]*?<% If Not WorkflowCentroTrabajoModernPresentationEnabled Then %>[\s\S]*?Servicios[\s\S]*?bnt_eval_event_default[\s\S]*?<% End If %>[\s\S]*?<\/asp:Panel>/);
    expect(page).toMatch(/<asp:Panel ID="Panel_documentos_tarea"[\s\S]*?<% If Not WorkflowCentroTrabajoModernPresentationEnabled Then %>[\s\S]*?<li[\s\S]*?Documentos[\s\S]*?<\/li>[\s\S]*?<% End If %>[\s\S]*?<\/asp:Panel>/);
  });

  it("consolidates duplicate document commands in the official menu while preserving backend controls", async () => {
    const [page, css, adapter] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("js/workflow/centro-trabajo-visual.js"),
    ]);

    [
      "<% If Not WorkflowCentroTrabajoModernPresentationEnabled Then %>",
      'ID="Panel_documentos_tarea"',
      'id="ctw-document-action-attach-list"',
      'id="ctw-document-action-digitize"',
      'id="ctw-document-action-share-users"',
      'id="ctw-document-action-share-email"',
      "Acciones de documentos",
      "Gestionar selección",
    ].forEach((marker) => expect(page).toContain(marker));
    expect(page).toMatch(/<asp:Panel ID="Panel_documentos_tarea"[\s\S]*?<% If Not WorkflowCentroTrabajoModernPresentationEnabled Then %>[\s\S]*?<\/asp:Panel>/);
    expect(page).toContain("C-DW-ENL");
    expect(page).toContain("C-DW-AUTO");
    expect(page).not.toContain('id="ctw-document-action-upload"');
    expect(page).not.toContain('id="ctw-document-action-delete-current"');
    expect(css).toContain(".ctw-menu__section-label");
  });

  it("keeps workflow menus above the workspace but below global popups", async () => {
    const [css, jqueryUi] = await Promise.all([
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("js/jquery-ui-1.12.1.custom/jquery-ui.structure.min.css"),
    ]);
    const zTokens = Object.fromEntries(
      [...css.matchAll(/--(ctw-z-[a-z-]+):\s*(\d+)\s*;/g)].map(([, name, value]) => [name, Number(value)])
    );

    expect(jqueryUi).toContain(".ui-front{z-index:100}");
    expect(zTokens["ctw-z-popup-floor"]).toBe(100);
    [
      "ctw-z-workspace",
      "ctw-z-command-secondary",
      "ctw-z-command-primary",
      "ctw-z-command-open",
      "ctw-z-menu",
      "ctw-z-document-menu",
    ].forEach((name) => expect(zTokens[name]).toBeLessThan(zTokens["ctw-z-popup-floor"]));

    expect(zTokens["ctw-z-command-secondary"]).toBeLessThan(zTokens["ctw-z-command-primary"]);
    expect(zTokens["ctw-z-command-primary"]).toBeLessThan(zTokens["ctw-z-menu"]);
    expect(zTokens["ctw-z-menu"]).toBeLessThan(zTokens["ctw-z-document-menu"]);
    expect(css).toContain("#UpdatePanel_menu_cab");
    expect(css).toContain("z-index: var(--ctw-z-command-primary);");
    expect(css).toContain("z-index: var(--ctw-z-menu) !important;");
    expect(css).toContain("z-index: var(--ctw-z-document-menu);");
    expect(css).not.toMatch(/z-index:\s*(?:1[0-9]{3,}|[2-9][0-9]{3,})/);
  });

  it("renders the structured task context only inside the existing UpdatePanel", async () => {
    const [page, css, taskSelection] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("workflow/Classselecciotarea.vb"),
    ]);

    [
      "content_pie_seleccion_tarea",
      'id="ctw-task-context"',
      'ID="Label_contexto_tramite"',
      'ID="Label_contexto_estado"',
      "ctw-task-context__title",
      "ctw-task-context__state",
      "ctw-task-context__meta",
      "ctw-task-context__process",
    ].forEach((marker) => {
      expect(page).toContain(marker);
    });
    expect(page).not.toContain('id="ctw-workflow-global"');
    expect((page.match(/ID="Label_estado_tarea_selecion"/g) || []).length).toBe(1);
    expect((page.match(/ID="Label_estado_selecion"/g) || []).length).toBe(1);
    expect(css).toContain("ctw-task-context");
    expect(taskSelection).toContain("Label_contexto_tramite");
    expect(taskSelection).toContain("Label_contexto_estado");
    expect(taskSelection).not.toMatch(/Split\([^\n]*(?:Radicado|Solicitante|Tramite|Trámite)/);
  });

  it("moves the existing task context above the workbench only for the pilot", async () => {
    const [page, css] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
    ]);
    const normalizedCss = css.replace(/\r\n/g, "\n");

    expect(page).toContain('id="content_pie_seleccion_tarea"');
    expect(page).toContain('ID="Label_estado_tarea_selecion"');
    expect(page).toContain('ID="Label_estado_selecion"');
    expect(page).toContain('id="ctw-task-context"');
    [
      "grid-template-rows: auto minmax(0, 1fr);",
      ".workflow-centro-trabajo-moderno.ctw-layer-layout #content_pie_seleccion_tarea {\n  grid-column: 1 / -1;\n  grid-row: 1;",
      ".workflow-centro-trabajo-moderno.ctw-layer-layout #content_seleccion_documentos {\n  grid-column: 1;\n  grid-row: 2;",
      ".workflow-centro-trabajo-moderno.ctw-layer-layout #contenido_imagen {\n  grid-column: 2;\n  grid-row: 2;",
      ".workflow-centro-trabajo-moderno.ctw-layer-layout #contenido_indice {\n  grid-column: 3;\n  grid-row: 2;",
    ].forEach((marker) => expect(normalizedCss).toContain(marker));
  });

  it("separates authorization state from its history and scopes the document counter format", async () => {
    const [page, documentData, taskSelection] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Docuarchi/ClassDaGabinete.vb"),
      readAppFile("workflow/Classselecciotarea.vb"),
    ]);

    [
      'ctw-authorize-state-label">Autorizada',
      "Cambiar estado de autorización de la tarea",
      "<% If WorkflowCentroTrabajoModernPresentationEnabled Then %>Historial<% Else %>Autorizar<% End If %>",
      "Historial de autorizaciones",
      "prevent_autoriza_tarea(event, this);",
      "Cerrar tarea seleccionada",
    ].forEach((marker) => expect(page).toContain(marker));
    expect(documentData).toContain("Optional ByVal modernDocumentCountFormat As Boolean = False");
    expect(documentData).toContain('If(modernDocumentCountFormat, "Documentos (0)", "Documentos 0")');
    expect(documentData).toContain('"Documentos (" & Datset.Tables(0).Rows.Count & ")"');
    expect(taskSelection).toContain("ref_Webworkflow.WorkflowCentroTrabajoModernPresentationEnabled");
    expect(taskSelection).toContain("modernDocumentCountFormat)");
  });

  it("projects task fields structurally without changing their business values", async () => {
    const [page, css, taskSelection] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("workflow/Classselecciotarea.vb"),
    ]);

    [
      "Radicado ",
      '" · "',
      'tipo_proceso_contexto = "Flujo"',
      'tipo_proceso_contexto = "Ruta"',
      "estado_contexto = \"Cerrado\"",
      "estado_contexto = \"Abierto\"",
      "System.Web.HttpUtility.HtmlEncode(tramite)",
      "System.Web.HttpUtility.HtmlEncode(nombre_proceso_contexto)",
    ].forEach((marker) => expect(taskSelection).toContain(marker));
    [
      "text-transform: uppercase;",
      ".ctw-task-context__title",
      ".ctw-task-context__state",
      "min-height: 46px;",
      "grid-template-columns: minmax(0, 1fr) minmax(0, 42%);",
      "grid-column: 1 / -1;",
      "grid-row: 2;",
      "float: none !important;",
    ].forEach((marker) => expect(css).toContain(marker));
    expect(page).toContain("<% If WorkflowCentroTrabajoModernPresentationEnabled Then %>");
  });

  it("projects the selected document bar without duplicating its upload action in the document menu", async () => {
    const [page, css, codeBehind] = await Promise.all([
      readAppFile("workflow/Webworkflow.aspx"),
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("workflow/Webworkflow.aspx.vb"),
    ]);

    expect(page).toContain("WorkflowCentroTrabajoSelectedDocumentAvailable");
    expect(page).not.toContain('id="ctw-document-action-upload"');
    expect(page).toContain('id="ctw-document-action-service"');
    expect(page.indexOf('id="ctw-document-action-service"')).toBeLessThan(page.indexOf("ctw-viewer-document-context"));
    expect(page).toContain("'C-DW-AUTO'");
    expect(page).not.toContain('id="ctw-document-action-versions"');
    expect(page).not.toContain("ctw-document-selected-menu");
    expect(page).toContain("WorkflowCentroTrabajoSelectedDocumentMetadataAvailable");
    [
      "WorkflowCentroTrabajoSelectedDocumentId",
      "WorkflowCentroTrabajoSelectedDocumentReference",
      "WorkflowCentroTrabajoSelectedDocumentRaw",
    ].forEach((marker) => expect(codeBehind).toContain(marker));
    [
      "#div_label",
      "#content_seleccion_documentos",
      "#Panel_tolbar_pdf",
      "ctw-viewer-document-context",
      "ctw-document-action-service",
      "ctw-viewer-action--service",
    ].forEach((marker) => expect(css).toContain(marker));
    expect(css).not.toContain("ctw-layer-layout.ctw-layer-documents #content_seleccion_documentos {\n  display: contents");
  });

  it("routes non-interactive document-row clicks through the existing viewer handler", async () => {
    const [adapter, css, page] = await Promise.all([
      readAppFile("js/workflow/documentos-relacionados-visual.js"),
      readAppFile("Styles/workflow-documentos-relacionados-modernos.css"),
      readAppFile("workflow/Webworkflow.aspx"),
    ]);

    [
      "function esZonaProtegidaDeFila",
      "function activarDocumentoDesdeFila",
      "vis_doc_selecion_wf",
      "window.prevent(event, launcher)",
      "launcher.contains(event.target)",
      'element.classList.contains("dropdown")',
      "sincronizarFilaSeleccionada(table);",
    ].forEach((marker) => expect(adapter).toContain(marker));
    expect(adapter).not.toContain("launcher.click(");
    expect(css).toContain('[tip_event="vis_doc_selecion_wf"]');
    expect(page).toContain("documentos-relacionados-visual.js?v=20260812-docrel5");
  });
});
