import { readFile } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";

const testDirectory = path.dirname(fileURLToPath(import.meta.url));
const appRoot = path.resolve(testDirectory, "..", "..", "..", "..");

const readAppFile = (relativePath) => readFile(path.join(appRoot, relativePath), "utf8");

describe("DOC-2 workflow visual activation", () => {
  it("keeps the master flag disabled and requires a server-side pilot", async () => {
    const [config, codeBehind, page] = await Promise.all([
      readAppFile("Web.config"),
      readAppFile("workflow/Webworkflow.aspx.vb"),
      readAppFile("workflow/Webworkflow.aspx"),
    ]);

    expect(config).toContain('<add key="WorkflowCentroTrabajoModernEnabled" value="false" />');
    expect(config).toContain('WorkflowCentroTrabajoModernPilotProfiles');
    expect(config).toContain('WorkflowCentroTrabajoModernLayers');
    expect(codeBehind).toContain('Session.Item("GA_LOGINUSUARIOGESTION")');
    expect(codeBehind).toContain('WorkflowCentroTrabajoModernActive');
    expect(page).toContain('<%= WorkflowCentroTrabajoModernCssAttribute %>');
  });

  it("preserves the approved visual baseline and loads DOC-2 after Webworkflow.js", async () => {
    const page = await readAppFile("workflow/Webworkflow.aspx");
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
  });

  it("keeps the CSS scoped and the adapter presentation-only", async () => {
    const [css, adapter] = await Promise.all([
      readAppFile("Styles/workflow-centro-trabajo-moderno.css"),
      readAppFile("js/workflow/centro-trabajo-visual.js"),
    ]);

    [".ctw-btn", ".ctw-icon-btn", ".ctw-menu", ".ctw-menu__panel", ".ctw-badge", ".ctw-action-bar", ".ctw-document-bar"].forEach((component) => {
      const escapedComponent = component.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
      expect(css).toMatch(new RegExp(`\\.workflow-centro-trabajo-moderno[^,{]*${escapedComponent}`));
    });
    expect(css).not.toMatch(/(?:^|\n)\s*(body|html|:root)\s*[,{]/);
    expect(adapter).toContain("PageRequestManager");
    expect(adapter).not.toMatch(/console\.|appendChild|insertBefore|removeChild|\.focus\(|setAttribute/);
  });
});
